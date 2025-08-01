using PatchOdyssey;

/* … */
[UnityEngine.DefaultExecutionOrder(2)]
[UnityEngine.RequireComponent(typeof(UnityEngine.BoxCollider))]
public sealed class Monster : Entity {
  public enum Kind : byte { Antillery, Borka, Molem, Sirpens, Tyrage }

  [System.Serializable]
  public struct WrestleInfo {
    [ReadWriteInInspector] public float     force;
    [ReadWriteInInspector] public Timeframe interval;
  }

  /* … */
  [UnityEngine.Header("Monster")]
  [ReadWriteInInspector]                             public  new      UnityEngine.BoxCollider                               collider           => (UnityEngine.BoxCollider) base.collider;
  [ReadWriteInInspector]                             public           Monster.Kind                                          kind               =  Monster.Kind.Borka;
  [ReadOnlyInInspector]                              private readonly System.Collections.Generic.List<UnityEngine.Material> materials          =  new();
  [ReadWriteInInspector]                             public           bool                                                  isMounted          => this.mountAutomatically && this.following is Tamer tamer && null != tamer && 0 != tamer.followers.Count && this == tamer.followers[0];
  [ReadWriteInInspector, UnityEngine.SerializeField] private          bool                                                  mountAutomatically =  true;
  [ReadOnlyInInspector]                              private readonly System.Collections.Generic.List<UnityEngine.Renderer> renderers          =  new();
  [ReadOnlyInInspector]                              private readonly System.Collections.Generic.List<UnityEngine.Material> sharedMaterials    =  new();
  [ReadWriteInInspector]                             public           Monster.WrestleInfo                                   wrestle            =  new() {force = 30.000f, interval = new(1.125)};
  [ReadWriteInInspector]                             public           Entity?                                               wrestling          =  null;

  /* … */
  protected override void Awake() {
    base.Awake();

    // …
    this.shoot.isAllowed = this.ShootIsAllowed;
  }

  protected override System.Converter<Entity, float> GetFindNonTeamTargetsDefaultComparison() {
    System.Converter<Entity, float> comparison = base.GetFindTargetsDefaultComparison();
    return this.kind switch {
      Monster.Kind.Antillery => entity => comparison(entity) / (entity is Monster monster ? null != monster.following && !monster.isMounted ? -1.0f : 1.0f : 1.0f),        // ->> Target followers
      Monster.Kind.Borka     => entity => comparison(entity),                                                                                                              // ->> Target default
      Monster.Kind.Molem     => entity => 1.0f / entity.health,                                                                                                            // ->> Target strongest
      Monster.Kind.Sirpens   => entity => 1.0f * entity.health,                                                                                                            // ->> Target weakest
      Monster.Kind.Tyrage    => entity => comparison(entity) + ((Game.SceneSize * Game.SceneSize) / ((entity is Player ? 3.0f : 1.0f) * (entity is Tamer ? 2.0f : 1.0f))), // ->> Prioritize `Player`s, then `Tamers`
      _                      => comparison
    };
  }

  protected override void OnDestroy() {
    base.OnDestroy();

    foreach (UnityEngine.Material material in this.materials)
      UnityEngine.Object.Destroy(material);

    this.materials      .Clear();
    this.renderers      .Clear();
    this.sharedMaterials.Clear();
  }

  protected override void OnTriggerEnter(UnityEngine.Collider collider) {
    base.OnTriggerEnter(collider);

    if (this.isDefeated)
    return;

    // …
    if (collider.GetComponent<Lasoo>() is Lasoo lasoo && null == lasoo.capture && lasoo.isDeploying()) {
      lasoo.capture  = this;
      this.wrestling = lasoo.user;
    }
  }

  public override Bullet? Shoot() {
    void DeployBullet(Bullet bullet) {
      switch (this.kind) {
        case Monster.Kind.Antillery:
        case Monster.Kind.Molem: {
          UnityEngine.Bounds     bulletBounds = new(bullet.transform.position, UnityEngine.Vector3.zero);
          UnityEngine.GameObject bulletMesh   = UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cylinder);

          // … ->> Replace existing “mesh” with `bulletMesh`
          if (bulletMesh is not null) {
            UnityEngine.Renderer  bulletMeshRenderer  = bulletMesh.GetComponent<UnityEngine.Renderer>() ?? bulletMesh.AddComponent<UnityEngine.MeshRenderer>();
            UnityEngine.Transform bulletMeshTransform = bulletMesh.transform;

            // …
            bulletMeshRenderer.sharedMaterials = System.Array.Empty<UnityEngine.Material>();
            bulletMeshTransform.localScale     = UnityEngine.Vector3.one * Game.VectorEpsilon;

            bullet.transform.ForEach(transform => {
              if (transform.GetComponent<UnityEngine.Renderer>() is UnityEngine.Renderer renderer && null != renderer) {
                bulletMeshRenderer.sharedMaterials = 0 == bulletMeshRenderer.sharedMaterials.Length ? renderer.sharedMaterials : bulletMeshRenderer.sharedMaterials;

                bulletBounds.Encapsulate(renderer.bounds.center - renderer.bounds.extents);
                bulletBounds.Encapsulate(renderer.bounds.center + renderer.bounds.extents);
              }
            });

            // …
            while (bulletBounds.size.sqrMagnitude > bulletMeshRenderer.bounds.size.sqrMagnitude)
              bulletMeshTransform.localScale *= 1.5f;

            switch (this.kind) {
              case Monster.Kind.Antillery:
                bulletMeshTransform.localScale = new(bulletMeshTransform.localScale.x * 0.4f, bulletMeshTransform.localScale.y * 0.9f, bulletMeshTransform.localScale.z * 0.4f);
                break;

              case Monster.Kind.Molem: {
                bullet.isInvincible             = true;
                bullet.lifetime                 = 20.0f;
                bulletMeshTransform.localScale *= 1.5f;
              } break;
            }

            foreach (UnityEngine.Transform transform in bullet.transform)
              UnityEngine.Object.Destroy(transform.gameObject);

            bulletMeshTransform.SetParent                  (bullet.transform, false);
            bulletMeshTransform.SetLocalPositionAndRotation(UnityEngine.Vector3.zero, Monster.Kind.Molem == this.kind ? UnityEngine.Random.rotation : UnityEngine.Quaternion.AngleAxis(90.0f, UnityEngine.Vector3.right));
            UnityEngine.Object.Destroy(bulletMesh.GetComponent<UnityEngine.Collider>());
          }
        } break;

        case Monster.Kind.Borka:   bullet.transform.localScale *= 1.1f;                                                                                                                  break;
        case Monster.Kind.Sirpens: bullet.transform.localScale  = new(bullet.transform.localScale.x * 1.0f, bullet.transform.localScale.y * 1.0f, bullet.transform.localScale.z * 0.4f); break;
        case Monster.Kind.Tyrage:  bullet.transform.localScale *= 2.0f;                                                                                                                  break;

        default: break;
      }
    }

    if (this.Shoot(DeployBullet /* Handle repeated firing */) is Bullet bullet) {
      // Handle original bullet and let caller register it
      DeployBullet(bullet);
      return bullet;
    }

    return null;
  }

  private bool ShootIsAllowed(bool allowed) {
    float distance = (null != this.target ? this.transform.position - this.target.transform.position : UnityEngine.Vector3.positiveInfinity).sqrMagnitude;
    return this.kind switch {
      Monster.Kind.Antillery => allowed && distance <= (UnityEngine.Vector3.one * 10.0f).sqrMagnitude,
      Monster.Kind.Borka     => allowed && distance <= (UnityEngine.Vector3.one * 6.0f) .sqrMagnitude,
      Monster.Kind.Molem     => allowed && distance <= (UnityEngine.Vector3.one * 2.0f) .sqrMagnitude,
      Monster.Kind.Sirpens   => allowed,
      Monster.Kind.Tyrage    => allowed,
      _                      => allowed
    };
  }

  protected override void Update() {
    UnityEngine.Transform transform = this.transform;

    // …
    base.Update();

    if (Game.IsPaused || this.isDefeated)
    return;

    // … ->> Bullet
    foreach (Bullet bullet in this.bullets)
    if (null != bullet && !bullet.isHit) {
      switch (this.kind) {
        case Monster.Kind.Borka:   break;
        case Monster.Kind.Molem:   break;
        case Monster.Kind.Sirpens: break;
        case Monster.Kind.Tyrage:  break;

        case Monster.Kind.Antillery:
        default: {
          bullet.rigidBody.AddForce    (bullet.shootDirection * this.shoot.speed, UnityEngine.ForceMode.Impulse);
          bullet.rigidBody.MoveRotation(UnityEngine.Quaternion.Euler(
            UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, bullet.rigidBody.rotation.eulerAngles) +                       // ->> Remove Y-axis orientation
            UnityEngine.Vector3.Scale(UnityEngine.Vector3.up, UnityEngine.Quaternion.LookRotation(bullet.shootDirection, UnityEngine.Vector3.up).eulerAngles) // ->> Apply  Y-axis orientation
          ));
        } break;
      }
    }

    // … ->> Mounted
    this.followAutomatically = !this.isMounted && null == this.wrestling;

    if (this.isMounted) {
      this.isInvincible = true;

      if (this.prefollowIsUpdated) {
        this.bounceAutomatically = false;
        this.moveAutomatically   = false;
      }

      if (0 == this.materials.Count) {
        this.materials      .Capacity = transform.hierarchyCount;
        this.renderers      .Capacity = transform.hierarchyCount;
        this.sharedMaterials.Capacity = transform.hierarchyCount;

        transform.ForEach(transform => {
          if (transform.GetComponent<UnityEngine.Renderer>() is UnityEngine.Renderer renderer && null != renderer) {
            this.sharedMaterials.Add(renderer.sharedMaterial);
            this.renderers      .Add(renderer);
            this.materials      .Add(renderer.material = renderer.material);
          }
        });
      }

      this.rigidBody.Sleep();
    }

    else {
      this.bounceAutomatically = this.prefollow.bounceAutomatically;
      this.isInvincible        = this.prefollow.isInvincible;
      this.moveAutomatically   = this.prefollow.moveAutomatically;

      for (int index = this.materials.Count; 0 != index--; ) {
        this.renderers[index].sharedMaterial = this.sharedMaterials[index];
        UnityEngine.Object.Destroy(this.materials[index]);
      }

      this.materials.Clear();
      this.renderers.Clear();
      this.rigidBody.WakeUp();
      this.sharedMaterials.Clear();
    }

    // … ->> Following
    if (this.isMounted) {
      UnityEngine.Renderer  renderer           = this.GetComponent<UnityEngine.Renderer>();
      UnityEngine.Transform followingTransform = this.following!.transform;
      UnityEngine.Renderer  followingRenderer  = followingTransform.GetComponent<UnityEngine.Renderer>();
      UnityEngine.Bounds    followingBounds    = null != followingRenderer ? followingRenderer.bounds : new(followingTransform.position, UnityEngine.Vector3.zero);
      UnityEngine.Bounds    bounds             = null != renderer ? renderer.bounds : new(transform.position, UnityEngine.Vector3.zero);

      // …
      transform.SetPositionAndRotation(UnityEngine.Vector3.SlerpUnclamped(transform.position, followingTransform.position, 0.65f), UnityEngine.Quaternion.SlerpUnclamped(transform.rotation, followingTransform.rotation, 0.2f));
      this.target = this.following!.target;

      for (int index = this.materials.Count; 0 != index--; )
      this.materials[index].color = UnityEngine.Color.Lerp(this.sharedMaterials[index].color, UnityEngine.Color.white, UnityEngine.Mathf.Min((followingBounds.min - bounds.max).sqrMagnitude, (followingBounds.max - bounds.min).sqrMagnitude) / (UnityEngine.Vector3.one * 8.0f).sqrMagnitude);
    }

    // … ->> Wrestling
    if (null != this.wrestling) {
      UnityEngine.Vector3 targetDistance = (transform.position - this.wrestling.transform.position).normalized;

      // …
      this.movement.direction  = targetDistance;
      this.target              = this.wrestling;
      this.targetAutomatically = false;

      if (this.wrestling is Player player) {
        if (this.wrestle.interval.isLooped) // ->> Pulled toward
        this.rigidBody.AddForce(-targetDistance * (UnityEngine.Random.value * this.wrestle.force), UnityEngine.ForceMode.Impulse);

        if (player.lasooing!.reachProgress >= 1.0f)
        player.ResetLasoo(); // ->> Lasoo stretched too far
      }
    }

    else {
      this.movement.direction  = UnityEngine.Vector3.zero;
      this.targetAutomatically = true;

      this.wrestle.interval.Reset();
    }
  }
}
