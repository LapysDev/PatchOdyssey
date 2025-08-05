using PatchOdyssey;

/* … */
[UnityEngine.DefaultExecutionOrder(2)]
[UnityEngine.RequireComponent(typeof(UnityEngine.BoxCollider))]
public sealed class Monster : Entity {
  public enum Kind : byte { Antillery, Borka, Molem, Sirpens, Tyrage }

  [System.Serializable]
  internal struct MountedInfo {
    [ReadOnlyInInspector] internal /* readonly */ System.Collections.Generic.List<UnityEngine.Material[]> materials;       // ->> Copied `UnityEngine.Material`
    [ReadOnlyInInspector] internal                UnityEngine.Material?                                   outlineMaterial; // ->> Original
    [ReadOnlyInInspector] internal /* readonly */ System.Collections.Generic.List<UnityEngine.Renderer>   renderers;       // ->> `UnityEngine.Renderer` using either `Monster::materials` or `Monster::sharedMaterials`
    [ReadOnlyInInspector] internal /* readonly */ System.Collections.Generic.List<UnityEngine.Material[]> sharedMaterials; // ->> Base `UnityEngine.Material`
  }

  [System.Serializable]
  public struct WrestleInfo {
    [ReadWriteInInspector] public float     force;
    [ReadWriteInInspector] public Timeframe interval;
  }

  /* … */
  [UnityEngine.Header("Monster")]
  [ReadWriteInInspector]                             public  Monster.Kind        kind               =  Monster.Kind.Borka;
  [ReadWriteInInspector]                             public  bool                isMounted          => this.mountAutomatically && base.following is Tamer tamer && null != tamer && 0 != tamer.followers.Count && this == tamer.followers[0];
  [ReadWriteInInspector]                             private Monster.MountedInfo mount              =  new() {materials = new(), outlineMaterial = null, renderers = new(), sharedMaterials = new()};
  [ReadWriteInInspector, UnityEngine.SerializeField] private bool                mountAutomatically =  true;
  [ReadWriteInInspector, UnityEngine.SerializeField] private bool                mountIsUsed        =  false;
  [ReadWriteInInspector]                             public  Monster.WrestleInfo wrestle            =  new() {force = 30.000f, interval = new(1.125)};
  [ReadWriteInInspector]                             public  Entity?             wrestling          =  null;

  /* … */
  protected override void Awake() {
    base.Awake();
    base.shoot.isAllowed = ShootIsAllowed;
  }

  protected override System.Converter<Entity, float> FindTargetsSorter() {
    System.Converter<Entity, float> comparison = base.FindTargetsSorter();
    return this.kind switch {
      Monster.Kind.Antillery =>        entity => comparison(entity) / (entity is Monster monster ? null != monster.following && !monster.isMounted ? -1.0f : 1.0f : 1.0f),        // ->> Target followers
      Monster.Kind.Borka     =>        entity => comparison(entity),                                                                                                              // ->> Target default
      Monster.Kind.Molem     => static entity => 1.0f / entity.health,                                                                                                            // ->> Target strongest
      Monster.Kind.Sirpens   => static entity => 1.0f * entity.health,                                                                                                            // ->> Target weakest
      Monster.Kind.Tyrage    =>        entity => comparison(entity) + ((Game.SceneSize * Game.SceneSize) / ((entity is Player ? 3.0f : 1.0f) * (entity is Tamer ? 2.0f : 1.0f))), // ->> Prioritize `Player`s, then `Tamers`
      _                      => comparison
    };
  }

  protected override void OnDestroy() {
    base.outline.material = this.mountIsUsed ? this.mount.outlineMaterial : base.outline.material;
    base.OnDestroy();

    foreach (UnityEngine.Material[] materials in this.mount.materials)
    foreach (UnityEngine.Material   material  in materials)
      UnityEngine.Object.Destroy(material); // ->> Prior `base.outline.material` destroyed here
  }

  protected override void OnTriggerEnter(UnityEngine.Collider collider) {
    base.OnTriggerEnter(collider);

    if (!base.isDefeated && null == this.following && collider.GetComponent<Lasoo>() is Lasoo lasoo && null == lasoo.capture && lasoo.isDeploying()) {
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
    void DeployBullet(Bullet bullet, ushort index) {
      switch (this.kind) {
        case Monster.Kind.Antillery:
        case Monster.Kind.Molem: {
          if (UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cylinder) is UnityEngine.GameObject bulletMesh) {
            UnityEngine.Bounds    bulletBounds        = new(bullet.transform.position, UnityEngine.Vector3.zero);
            UnityEngine.Renderer  bulletMeshRenderer  = bulletMesh.GetComponent<UnityEngine.Renderer>() ?? bulletMesh.AddComponent<UnityEngine.MeshRenderer>();
            UnityEngine.Transform bulletMeshTransform = bulletMesh.transform;

            // …
            bulletMeshRenderer.sharedMaterials = System.Array.Empty<UnityEngine.Material>();
            bulletMeshTransform.localScale     = UnityEngine.Vector3.one * Game.VectorEpsilon;
            bulletMeshTransform.localRotation  = Monster.Kind.Molem == this.kind ? UnityEngine.Random.rotation : UnityEngine.Quaternion.AngleAxis(90.0f, UnityEngine.Vector3.right);

            bullet.transform.ForEach<UnityEngine.Renderer>(renderer => { bulletBounds.Encapsulate(renderer.bounds); bulletMeshRenderer.sharedMaterials = 0 == bulletMeshRenderer.sharedMaterials.Length ? renderer.sharedMaterials : bulletMeshRenderer.sharedMaterials; });

            while (bulletBounds.size.sqrMagnitude > bulletMeshRenderer.bounds.size.sqrMagnitude)
              bulletMeshTransform.localScale *= 1.5f;

            switch (this.kind) {
              case Monster.Kind.Antillery: {
                bullet.shootDirection             = priorShootDirection;
                bulletMeshTransform.localPosition = UnityEngine.Vector3.right * (0 == (index & 1) ? +1.0f : -1.0f);
                bulletMeshTransform.localScale    = new(bulletMeshTransform.localScale.x * 0.40f, bulletMeshTransform.localScale.y * 0.75f, bulletMeshTransform.localScale.z * 0.40f);
              } break;

              case Monster.Kind.Molem: {
                bullet.isInvincible             = true;
                bullet.lifetime                 = 20.0f;
                bulletMeshTransform.localScale *= 1.5f;
              } break;

              default: break;
            }

            foreach (UnityEngine.Collider collider in bulletMesh.GetComponents<UnityEngine.Collider>())
              UnityEngine.Object.Destroy(collider);

            foreach (UnityEngine.Transform transform in bullet.transform)
              UnityEngine.Object.Destroy(transform.gameObject);

            bulletMeshTransform.SetParent(bullet.transform, false);
          }
        } break;

        case Monster.Kind.Borka:   bullet.transform.localScale *= 1.1f;                                                                                                                  break;
        case Monster.Kind.Sirpens: bullet.transform.localScale  = new(bullet.transform.localScale.x * 1.0f, bullet.transform.localScale.y * 1.0f, bullet.transform.localScale.z * 0.4f); break;
        case Monster.Kind.Tyrage:  bullet.transform.localScale *= 2.0f;                                                                                                                  break;

        default: break;
      }
    }

    /* … */
    if (base.Shoot(DeployBullet) is Bullet bullet) {
      priorShootDirection = bullet.shootDirection;
      DeployBullet(bullet, (byte) 0u);

      return bullet;
    }

    return null;
  }

  private bool ShootIsAllowed(bool allowed) {
    float distance = (null != base.target ? this.transform.position - base.target.transform.position : UnityEngine.Vector3.positiveInfinity).sqrMagnitude;
    return this.kind switch {
      Monster.Kind.Antillery => allowed && distance <= (UnityEngine.Vector3.one * 12.0f).sqrMagnitude,
      Monster.Kind.Borka     => allowed && distance <= (UnityEngine.Vector3.one * 8.0f) .sqrMagnitude,
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
        case Monster.Kind.Borka: break;
        case Monster.Kind.Molem: /* Do nothing… */ break;
        case Monster.Kind.Sirpens: break;
        case Monster.Kind.Tyrage:  break;

        case Monster.Kind.Antillery:
        default: {
          bullet.rigidBody.AddForce    (bullet.shootDirection * base.shoot.speed, UnityEngine.ForceMode.Impulse);
          bullet.rigidBody.MoveRotation(UnityEngine.Quaternion.Euler(
            UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, bullet.rigidBody.rotation.eulerAngles) +                       // ->> Remove Y-axis orientation
            UnityEngine.Vector3.Scale(UnityEngine.Vector3.up, UnityEngine.Quaternion.LookRotation(bullet.shootDirection, UnityEngine.Vector3.up).eulerAngles) // ->> Apply  Y-axis orientation
          ));
        } break;
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
          this.mount.renderers      .Add(renderer);
          this.mount.sharedMaterials.Add(sharedMaterials);

          for (uint index = (uint) sharedMaterials.Length; 0u != index--; )
            materials[index] = base.outline.material == sharedMaterials[index] ? this.mount.outlineMaterial ??= new UnityEngine.Material(base.outline.material) : new UnityEngine.Material(sharedMaterials[index]);

          renderer.sharedMaterials = materials;
          this.mount.materials.Add(materials);
        });

        (base.outline.material, this.mount.outlineMaterial) = (this.mount.outlineMaterial, base.outline.material);
      }
    }

    else {
      base.bounceAutomatically = base.prefollow.bounceAutomatically;
      base.isInvincible        = base.prefollow.isInvincible;
      base.moveAutomatically   = base.prefollow.moveAutomatically;
      base.outline.material    = this.mount.outlineMaterial;
      this.mountIsUsed         = false;

      for (int index = this.mount.materials.Count; 0 != index--; ) {
        this.mount.renderers[index].sharedMaterials = this.mount.sharedMaterials[index];

        foreach (UnityEngine.Material material in this.mount.materials[index])
        UnityEngine.Object.Destroy(material); // ->> Prior `base.outline.material` destroyed here
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

      base.target            = base.following!.target;
      bounds         .center = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, bounds         .center);
      bounds         .size   = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, bounds         .size);
      followingBounds.center = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, followingBounds.center);
      followingBounds.size   = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, followingBounds.size);
      followingRatio         = UnityEngine.Mathf.Min((followingBounds.min - bounds.max).sqrMagnitude, (followingBounds.max - bounds.min).sqrMagnitude) / followingRatioThreshold;

      for (int  index    = this.mount.materials.Count;                0  != index--; )
      for (uint subindex = (uint) this.mount.materials[index].Length; 0u != subindex--; )
        this.mount.materials[index][subindex].color = UnityEngine.Color.Lerp(this.mount.sharedMaterials[index][subindex].color, UnityEngine.Color.white, followingRatio);
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

    // … ->> Pre-follow
    base.PrefollowUpdate();
  }
}
