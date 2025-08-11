using PatchOdyssey;

/* … */
[UnityEngine.DefaultExecutionOrder(3)]
[UnityEngine.RequireComponent(typeof(UnityEngine.BoxCollider))]
public sealed class Monster : Entity {
  [System.Serializable]
  public struct CamouflageInfo {
    [ReadOnlyInInspector] public UnityEngine.Color          color;
    [ReadOnlyInInspector] public UnityEngine.Color          colorPrior;
    [ReadOnlyInInspector] public UnityEngine.MeshRenderer[] considerations;
    [ReadOnlyInInspector] public UnityEngine.Material?      material;
  }

  public enum Kind : byte { Antillery, Borka, Molem, Sirpens, Tyrage }

  [System.Serializable]
  internal struct MountedInfo {
    [ReadOnlyInInspector] internal                UnityEngine.Material?                                   camouflageMaterial; // ->> Original
    [ReadOnlyInInspector] internal /* readonly */ System.Collections.Generic.List<UnityEngine.Material[]> materials;          // ->> Copied `UnityEngine.Material`
    [ReadOnlyInInspector] internal                UnityEngine.Material?                                   outlineMaterial;    // ->> Original
    [ReadOnlyInInspector] internal /* readonly */ System.Collections.Generic.List<UnityEngine.Renderer>   renderers;          // ->> `UnityEngine.Renderer` using either `Monster::materials` or `Monster::sharedMaterials`
    [ReadOnlyInInspector] internal /* readonly */ System.Collections.Generic.List<UnityEngine.Material[]> sharedMaterials;    // ->> Base `UnityEngine.Material`
  }

  [System.Serializable]
  public struct WrestleInfo {
    [ReadWriteInInspector] public float     force;
    [ReadWriteInInspector] public Timeframe interval;
  }

  /* … */
  [UnityEngine.Header("Monster")]
  [ReadOnlyInInspector, UnityEngine.SerializeField]  internal Monster.CamouflageInfo camouflage         =  new() {color = new(0.537f, 0.678f, 0.416f, 0.500f), considerations = System.Array.Empty<UnityEngine.MeshRenderer>(), material = null};
  [ReadWriteInInspector]                             public   Monster.Kind           kind               =  Monster.Kind.Borka;
  [ReadWriteInInspector]                             public   bool                   isMounted          => this.mountAutomatically && base.following is Tamer tamer && null != tamer && 0 != tamer.followers.Count && this == tamer.followers[0];
  [ReadWriteInInspector]                             private  Monster.MountedInfo    mount              =  new() {camouflageMaterial = null, materials = new(), outlineMaterial = null, renderers = new(), sharedMaterials = new()};
  [ReadWriteInInspector, UnityEngine.SerializeField] private  bool                   mountAutomatically =  true;
  [ReadWriteInInspector, UnityEngine.SerializeField] private  bool                   mountIsUsed        =  false;
  [ReadWriteInInspector]                             public   Monster.WrestleInfo    wrestle            =  new() {force = 30.000f, interval = new(1.125)};
  [ReadWriteInInspector]                             public   Entity?                wrestling          =  null;

  /* … */
  protected override void Awake() {
    base.Awake();
    base.shoot.isAllowed = ShootIsAllowed;

    if (Monster.Kind.Sirpens == this.kind) {
      this.transform.ForEach<UnityEngine.Renderer>(renderer => {
        if (null != this.camouflage.material)
        return false;

        if (0 != renderer.sharedMaterials.Length) {
          UnityEngine.Material[] rendererMaterials = renderer.sharedMaterials;
          int                    index             = rendererMaterials.Length;

          // …
          while (0 != index--) {
            UnityEngine.Material material = rendererMaterials[index];

            // …
            if (
              material != base.outline.material && material != base.shadow.material &&
              // …
              !string.Equals(material.name, base.outline.material?.name ?? "", System.StringComparison.OrdinalIgnoreCase) &&
              !string.Equals(material.name, base.shadow .material?.name ?? "", System.StringComparison.OrdinalIgnoreCase)
            ) break;
          }

          if (index != -1) {
            UnityEngine.Material[] materials = new UnityEngine.Material[rendererMaterials.Length];

            // …
            rendererMaterials.CopyTo(materials, 0);

            this.camouflage.colorPrior    = materials[index].color;
            this.camouflage.material      = materials[index] = new(materials[index]);
            this.camouflage.material.name = "Camouflage";
            renderer.sharedMaterials      = materials;
          }
        }

        return true;
      });

      this.camouflage.considerations = new System.Collections.Generic.List<UnityEngine.MeshRenderer>(UnityEngine.Object.FindObjectsByType<UnityEngine.MeshRenderer>(UnityEngine.FindObjectsInactive.Exclude, UnityEngine.FindObjectsSortMode.None)).FindAll(renderer => (
        // !renderer.gameObject.isStatic &&
        //  renderer.isVisible           &&
         renderer.enabled           &&
        !renderer.forceRenderingOff &&
        !renderer.transform.IsChildOf(this.transform)
      )).ToArray();
    }
  }

  public override void Damage(Entity entity, float amount, Entity? attacker) {
    base.Damage(entity, amount, attacker);

    if (Monster.Kind.Sirpens == this.kind && attacker == this)
    base.PassiveDamage(entity, amount * 0.10f, 0.75f, (ushort) (7u + (UnityEngine.Random.value * 3u)));
  }

  protected override System.Converter<Entity, float> FindTargetsSorter() {
    System.Converter<Entity, float> comparison = base.FindTargetsSorter();
    return this.kind switch {
      Monster.Kind.Antillery =>        entity => comparison(entity) / (entity is Monster monster ? null != monster.following && !monster.isMounted ? -1.0f : 1.0f : 1.0f),        // ->> Target followers
      Monster.Kind.Borka     =>        entity => comparison(entity),                                                                                                              // ->> Target default
      Monster.Kind.Molem     => static entity => 1.0f / entity.health,                                                                                                            // ->> Target strongest
      Monster.Kind.Sirpens   => static entity => 1.0f * entity.health,                                                                                                            // ->> Target weakest
      Monster.Kind.Tyrage    =>        entity => comparison(entity) + ((Game.SceneSize * Game.SceneSize) / ((entity is Player ? 3.0f : 1.0f) * (entity is Tamer ? 2.0f : 1.0f))), // ->> Prioritize `Player`s, then `Tamers`
      _                      =>        comparison
    };
  }

  protected override void OnDestroy() {
    this.camouflage.material = this.mountIsUsed ? this.mount.camouflageMaterial : this.camouflage.material;
    base.outline.material    = this.mountIsUsed ? this.mount.outlineMaterial    : base.outline   .material;

    base.OnDestroy();

    foreach (UnityEngine.Material[] materials in this.mount.materials)
    foreach (UnityEngine.Material   material  in materials)
      UnityEngine.Object.Destroy(material); // ->> Prior `base.outline.material` destroyed here

    if (null != this.camouflage.material)
    UnityEngine.Object.Destroy(this.camouflage.material);
  }

  protected override void OnTriggerEnter(UnityEngine.Collider collider) {
    base.OnTriggerEnter(collider);

    if (!base.isDefeated && null == this.following && collider.TryGetComponent(out Lasoo lasoo) && null == lasoo.capture && lasoo.isDeploying()) {
      lasoo.capture  = this;
      this.wrestling = lasoo.user;
    }
  }

  protected override void PrefollowUpdate() {
    /* Do nothing… */
  }

  public override Bullet? Shoot() {
    UnityEngine.Vector3 priorShootDirection = UnityEngine.Vector3.zero;

    /* … */
    void DeployBullet(Bullet bullet, ushort volleyIndex, ushort volleyCount) {
      switch (this.kind) {
        case Monster.Kind.Antillery:
        case Monster.Kind.Molem: {
          if ((null != Assets.main.primitives.cylinder ? UnityEngine.Object.Instantiate(Assets.main.primitives.cylinder, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity) : UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cylinder)) is UnityEngine.GameObject bulletMesh) {
            UnityEngine.Bounds    bulletBounds        = new(bullet.transform.position, UnityEngine.Vector3.zero);
            UnityEngine.LayerMask bulletLayerMask     = UnityEngine.LayerMask.NameToLayer("Ignore Bullet");
            UnityEngine.Renderer  bulletMeshRenderer  = bulletMesh.GetComponent<UnityEngine.Renderer>();
            UnityEngine.Transform bulletMeshTransform = bulletMesh.transform;

            // …
            bulletMesh.name                      = "Mesh";
            bulletMeshRenderer.lightProbeUsage   = UnityEngine.Rendering.LightProbeUsage.Off;
            bulletMeshRenderer.receiveShadows    = false;
            bulletMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            bulletMeshRenderer.sharedMaterials   = System.Array.Empty<UnityEngine.Material>();
            bulletMeshTransform.localScale       = UnityEngine.Vector3.one * Game.VectorEpsilon;

            bullet.transform.ForEach<UnityEngine.Renderer>(renderer => {
              if (0 == bulletMeshRenderer.sharedMaterials.Length && 0 != renderer.sharedMaterials.Length) {
                if (null != Assets.main.outlineMaterial) {
                  UnityEngine.Material[] materials = new UnityEngine.Material[renderer.sharedMaterials.Length + 1];

                  // …
                  materials[0] = Assets.main.outlineMaterial;
                  renderer.sharedMaterials.CopyTo(materials, 1);

                  bulletMeshRenderer.sharedMaterials = materials;
                } else bulletMeshRenderer.sharedMaterials = renderer.sharedMaterials;
              }

              bulletBounds.Encapsulate(renderer.bounds);
            });

            while (bulletBounds.size.sqrMagnitude > bulletMeshRenderer.bounds.size.sqrMagnitude)
              bulletMeshTransform.localScale *= 1.5f;

            if (bulletLayerMask != -1) {
              base.gameObject.layer          |= bulletLayerMask;
              bullet.collider.excludeLayers  |= (UnityEngine.LayerMask)  (1 << bulletLayerMask);
              bullet.collider.includeLayers  &= (UnityEngine.LayerMask) ~(1 << bulletLayerMask);
              bullet.rigidBody.excludeLayers |= (UnityEngine.LayerMask)  (1 << bulletLayerMask);
              bullet.rigidBody.includeLayers &= (UnityEngine.LayerMask) ~(1 << bulletLayerMask);
              bulletMesh     .layer          |= bulletLayerMask;

              foreach (UnityEngine.Collider collider in bulletMesh.GetComponents<UnityEngine.Collider>()) {
                collider.excludeLayers = (UnityEngine.LayerMask)  (1 << bulletLayerMask);
                collider.includeLayers = (UnityEngine.LayerMask) ~(1 << bulletLayerMask);
                collider.isTrigger     = false;
              }

              if ((bulletMesh.TryGetComponent(out UnityEngine.Rigidbody rigidBody) ? rigidBody : bulletMesh.AddComponent<UnityEngine.Rigidbody>()) is UnityEngine.Rigidbody bulletMeshRigidBody) {
                bulletMeshRigidBody.Sleep(); // ->> Stack overflow triggered without this?
                bulletMeshRigidBody.angularDamping         = 100.0f;
                bulletMeshRigidBody.collisionDetectionMode = UnityEngine.CollisionDetectionMode.Discrete;
                bulletMeshRigidBody.constraints            = UnityEngine.RigidbodyConstraints.FreezeAll;
                bulletMeshRigidBody.excludeLayers          = (UnityEngine.LayerMask) (1 << bulletLayerMask);
                bulletMeshRigidBody.freezeRotation         = true;
                bulletMeshRigidBody.includeLayers          = (UnityEngine.LayerMask) ~(1 << bulletLayerMask);
                bulletMeshRigidBody.interpolation          = UnityEngine.RigidbodyInterpolation.None;
                bulletMeshRigidBody.isKinematic            = false;
                bulletMeshRigidBody.linearDamping          = 100.0f;
                bulletMeshRigidBody.maxAngularVelocity     = 0.0f;
                bulletMeshRigidBody.maxLinearVelocity      = 0.0f;
                bulletMeshRigidBody.useGravity             = false;
                bulletMeshRigidBody.WakeUp();
              }
            }

            foreach (UnityEngine.Transform transform in bullet.transform)
              UnityEngine.Object.Destroy(transform.gameObject);

            bulletMeshTransform.SetParent(bullet.transform, false);

            switch (this.kind) {
              case Monster.Kind.Antillery: {
                bullet.rigidBody.position        += this.transform.right * (0 == (volleyIndex & 1) ? +1.0f : -1.0f);
                bullet.shootDirection             = priorShootDirection;
                bulletMeshTransform.localRotation = UnityEngine.Quaternion.AngleAxis(90.0f, UnityEngine.Vector3.right);
                bulletMeshTransform.localScale    = new(bulletMeshTransform.localScale.x * 0.40f, bulletMeshTransform.localScale.y * 0.75f, bulletMeshTransform.localScale.z * 0.40f);
              } break;

              case Monster.Kind.Molem: {
                bullet.lifetime                   = UnityEngine.Mathf.Max(10.00f, bullet.lifetime * 2.00f);
                bullet.rigidBody.position        += (bullet.shootDirection * 3.25f) + (this.transform.forward * UnityEngine.Random.value * (UnityEngine.Random.value > 0.50f ? +1.00f : -1.00f) * 1.0f) + (this.transform.right * UnityEngine.Random.value * (UnityEngine.Random.value > 0.50f ? +1.00f : -1.00f) * 2.5f) + ((UnityEngine.Vector3.up * volleyIndex * (1.75f / volleyCount)) - (UnityEngine.Vector3.up * (1.75f / volleyCount) * 0.75f));
                bullet.shootDirection             = null != base.target ? (base.target!.transform.position - this.transform.position).normalized : this.transform.forward;
                bulletMeshTransform.localScale    = new(bulletMeshTransform.localScale.x * (0.75f + (UnityEngine.Random.value * 0.25f)), bulletMeshTransform.localScale.y * (0.50f + (UnityEngine.Random.value * 0.25f)), bulletMeshTransform.localScale.z * (0.75f + (UnityEngine.Random.value * 0.25f)));
                bulletMeshTransform.localRotation = UnityEngine.Random.rotation;
              } break;

              default: break;
            }
          }
        } break;

        case Monster.Kind.Borka:
          bullet.transform.localScale *= 1.1f;
          break;

        case Monster.Kind.Sirpens: {
          bullet.isInvincible         = true;
          bullet.transform.localScale = new(bullet.transform.localScale.x * 0.4f, bullet.transform.localScale.y * 1.0f, bullet.transform.localScale.z * 1.0f);

          bullet.transform.ForEach<UnityEngine.Renderer>(static renderer => renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On);
        } break;

        case Monster.Kind.Tyrage:
          bullet.transform.localScale *= 2.0f;
          break;

        default: break;
      }
    }

    /* … */
    if (base.Shoot(DeployBullet) is Bullet bullet) {
      priorShootDirection = bullet.shootDirection;
      DeployBullet(bullet, (ushort) 0u, (ushort) base.shoot.volleyCount);

      return bullet;
    }

    return null;
  }

  private bool ShootIsAllowed(bool allowed) {
    float distance = (null != base.target ? this.transform.position - base.target.transform.position : UnityEngine.Vector3.positiveInfinity).sqrMagnitude;
    return this.kind switch {
      Monster.Kind.Antillery => allowed && distance <= (UnityEngine.Vector3.one * 15.0f).sqrMagnitude,
      Monster.Kind.Borka     => allowed && distance <= (UnityEngine.Vector3.one * 10.0f).sqrMagnitude,
      Monster.Kind.Molem     => allowed && distance <= (UnityEngine.Vector3.one * 3.0f) .sqrMagnitude,
      Monster.Kind.Sirpens   => allowed,
      Monster.Kind.Tyrage    => allowed,
      _                      => allowed
    };
  }

  protected override void Update() {
    base.Update();

    if (Game.IsPaused || base.isDefeated)
    return;

    // … ->> Bullet
    foreach (Bullet bullet in base.bullets)
    if (null != bullet && !bullet.isHit) {
      switch (this.kind) {
        case Monster.Kind.Borka: {
          UnityEngine.Vector3 volleySpreadDirection = bullet.transform.right;
          int                 volleyThreshold       = bullet.volleyCount / 2;

          // …
          volleySpreadDirection *= ((bullet.volleyIndex - volleyThreshold) / (float) volleyThreshold);
          bullet.rigidBody.AddForce(base.shoot.speed * (bullet.shootDirection + volleySpreadDirection).normalized, UnityEngine.ForceMode.Impulse);
        } break;

        case Monster.Kind.Molem:  /* Do nothing… */   break;
        case Monster.Kind.Tyrage: /* Do something… */ break;

        case Monster.Kind.Antillery:
        case Monster.Kind.Sirpens:
        default:
          bullet.Travel();
          break;
      }
    }

    // … ->> Mounted
    base.followAutomatically = !this.isMounted && null == this.wrestling;

    if (this.isMounted) {
      base.rigidBody.Sleep();

      if (base.prefollowIsUpdated) {
        base.bounceAutomatically = false;
        base.isInvincible        = true;
        base.moveAutomatically   = false;
      }

      if (!this.mountIsUsed) {
        this.mount.materials.Capacity       = this.transform.hierarchyCount;
        this.mount.outlineMaterial          = null;
        this.mount.renderers      .Capacity = this.transform.hierarchyCount;
        this.mount.sharedMaterials.Capacity = this.transform.hierarchyCount;
        this.mountIsUsed                    = true;

        this.transform.ForEach<UnityEngine.Renderer>(renderer => {
          UnityEngine.Material[] sharedMaterials = renderer.sharedMaterials;
          UnityEngine.Material[] materials       = new UnityEngine.Material[sharedMaterials.Length];

          // …
          for (uint index = (uint) sharedMaterials.Length; 0u != index--; )
          materials[index] = (
            sharedMaterials[index] == this.camouflage.material ? this.mount.camouflageMaterial = new UnityEngine.Material(this.camouflage.material) :
            sharedMaterials[index] == base.outline   .material ? this.mount.outlineMaterial  ??= new UnityEngine.Material(base.outline   .material) :
            new UnityEngine.Material(sharedMaterials[index])
          );

          renderer.sharedMaterials = materials;

          this.mount.materials      .Add(materials);
          this.mount.renderers      .Add(renderer);
          this.mount.sharedMaterials.Add(sharedMaterials);
        });

        (this.camouflage.material, this.mount.camouflageMaterial) = (this.mount.camouflageMaterial, this.camouflage.material);
        (base.outline   .material, this.mount.outlineMaterial)    = (this.mount.outlineMaterial,    base.outline   .material);
      }
    }

    else {
      base.bounceAutomatically = base.prefollow.bounceAutomatically;
      base.isInvincible        = base.prefollow.isInvincible;
      base.moveAutomatically   = base.prefollow.moveAutomatically;
      base.outline.material    = this.mount.outlineMaterial    ?? base.outline   .material;
      this.camouflage.material = this.mount.camouflageMaterial ?? this.camouflage.material;
      this.mountIsUsed         = false;

      for (int index = this.mount.materials.Count; 0 != index--; ) {
        this.mount.renderers[index].sharedMaterials = this.mount.sharedMaterials[index];

        foreach (UnityEngine.Material material in this.mount.materials[index])
        UnityEngine.Object.Destroy(material); // ->> Prior `base.outline.material` (and `this.camouflage.material`) destroyed here
      }

      base.rigidBody.WakeUp();
      this.mount.materials      .Clear();
      this.mount.renderers      .Clear();
      this.mount.sharedMaterials.Clear();
    }

    // … ->> Following
    if (this.isMounted) {
      UnityEngine.Renderer renderer                = this.GetComponent<UnityEngine.Renderer>();
      UnityEngine.Renderer followingRenderer       = base.following!.transform.GetComponent<UnityEngine.Renderer>();
      float                followingRatioThreshold = ((UnityEngine.Vector3.forward + UnityEngine.Vector3.right) * 8.0f).sqrMagnitude;
      float                followingRatio          = 0.0f;
      UnityEngine.Bounds   followingBounds         = null != followingRenderer ? followingRenderer.bounds : new UnityEngine.Bounds(base.following.transform.position, UnityEngine.Vector3.zero);
      UnityEngine.Bounds   bounds                  = null != renderer          ? renderer         .bounds : new UnityEngine.Bounds(this.transform          .position, UnityEngine.Vector3.zero);

      // …
      this.transform.SetPositionAndRotation(UnityEngine.Vector3.SlerpUnclamped(this.transform.position, base.following.transform.position, 0.65f), UnityEngine.Quaternion.SlerpUnclamped(transform.rotation, base.following.transform.rotation, 0.2f));

      base.outline.color     = base.following!.outline.color;
      base.target            = base.following!.target;
      bounds         .center = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, bounds         .center);
      bounds         .size   = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, bounds         .size);
      followingBounds.center = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, followingBounds.center);
      followingBounds.size   = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, followingBounds.size);
      followingRatio         = UnityEngine.Mathf.Min((followingBounds.min - bounds.max).sqrMagnitude, (followingBounds.max - bounds.min).sqrMagnitude) / followingRatioThreshold;

      for (int  index    = this.mount.materials.Count;                0  != index--; )
      for (uint subindex = (uint) this.mount.materials[index].Length; 0u != subindex--; ) {
        UnityEngine.Material material = this.mount.materials[index][subindex];

        // …
        if (material != base.outline.material && material != this.camouflage.material)
        material.color = UnityEngine.Color.Lerp(this.mount.sharedMaterials[index][subindex].color, UnityEngine.Color.white, followingRatio);
      }
    }

    // … ->> Wrestling
    if (null != this.wrestling) {
      UnityEngine.Vector3 targetDistance = (this.transform.position - this.wrestling.transform.position).normalized;

      // …
      base.movement.direction  = targetDistance;
      base.target              = this.wrestling;
      base.targetAutomatically = false;

      if (this.wrestling is Player player) {
        if (this.wrestle.interval.isLooped) // ->> Pulled toward
        base.rigidBody.AddForce(-targetDistance * (UnityEngine.Random.value * this.wrestle.force), UnityEngine.ForceMode.Impulse);

        if (player.lasooing!.reachProgress >= 1.0f)
        player.ResetLasoo(); // ->> Lasoo stretched too far
      }
    }

    else {
      base.movement.direction  = UnityEngine.Vector3.zero;
      base.targetAutomatically = true;

      this.wrestle.interval.Reset();
    }

    // … ->> Camouflaging
    this.camouflage.color = this.camouflage.colorPrior;

    if (null != this.camouflage.material) {
      if (0u != this.camouflage.considerations.Length && !this.isMounted) {
        const float NonPrioritized = 1.0e7f;
        UnityEngine.Vector3 position = this.transform.position;

        /* … */
        float GetCamouflagePriority(UnityEngine.MeshRenderer consideration) {
          float priority = NonPrioritized;

          // …
          if (null != consideration) {
            UnityEngine.Bounds bounds = consideration.bounds;

            // …
            bounds.Expand(1.0f);

            priority      = -2.0f / UnityEngine.Mathf.Max(Game.VectorEpsilon, position.y - bounds.center.y);
            bounds.center = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, bounds.center);
            position      = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, position);
            priority      = bounds.Contains(position) ? priority + (-1.0f / (
              UnityEngine.Mathf.Abs(bounds.center.x - position.x) +
              UnityEngine.Mathf.Abs(bounds.center.y - position.y) +
              UnityEngine.Mathf.Abs(bounds.center.z - position.z)
            )) : NonPrioritized;
          }

          return priority;
        }

        /* … */
        System.Array.Sort(this.camouflage.considerations, (considerationA, considerationB) => System.Math.Sign(GetCamouflagePriority(considerationA) - GetCamouflagePriority(considerationB)));
        this.camouflage.color = UnityEngine.Color.LerpUnclamped(this.camouflage.colorPrior, this.camouflage.considerations[0].sharedMaterial.color, 0.675f);
      }

      this.camouflage.material.color = UnityEngine.Color.LerpUnclamped(this.camouflage.material.color, this.camouflage.color, 0.2f);
    }

    // … ->> Pre-follow
    base.PrefollowUpdate();
  }
}
