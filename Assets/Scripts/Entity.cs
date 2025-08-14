using PatchOdyssey;

/* … */
[UnityEngine.DefaultExecutionOrder(2)]
[UnityEngine.RequireComponent(typeof(UnityEngine.Collider))]
[UnityEngine.RequireComponent(typeof(UnityEngine.Rigidbody))]
public abstract class Entity : GameComponent /* ->> Source file must be named “Entity” */ {
  [System.Serializable]
  public struct AudioClipInfo {
    [ReadWriteInInspector] public UnityEngine.AudioClip? capturing;
    [ReadWriteInInspector] public UnityEngine.AudioClip? defeating;
    [ReadWriteInInspector] public UnityEngine.AudioClip? introducing;
    [ReadWriteInInspector] public UnityEngine.AudioClip? lasooing;
    [ReadWriteInInspector] public UnityEngine.AudioClip? moving;
    [ReadWriteInInspector] public UnityEngine.AudioClip? shooting;
    [ReadWriteInInspector] public UnityEngine.AudioClip? wrestling;
  }

  [System.Serializable]
  public struct BounceInfo {
    [ReadWriteInInspector] public float                angle; // ->> in Degrees --> 0.0f <= |bounce.angle| <= ~180.0f
    [ReadOnlyInInspector]  public float                estimatedHeight { get; internal set; }
    [ReadWriteInInspector] public float                force;
    [ReadWriteInInspector] public float                speed; // ->> Degrees per second
    [ReadWriteInInspector] public Entity.TurnDirection turn;
  }

  [System.Serializable]
  public struct ContactInfo {
    [ReadWriteInInspector] public float damage;
    [ReadWriteInInspector] public float repelForce;
  }

  [System.Serializable]
  public struct DefeatInfo {
    [ReadOnlyInInspector]  public UnityEngine.Vector3 position;
    [ReadWriteInInspector] public Timeframe           timeout;
  }

  [System.Flags]
  public enum FindTargetsOptions : byte { Any = 0x1, DifferentTeam = 0x2, SameTeam = 0x4 }

  [System.Serializable]
  public struct MovementInfo {
    [ReadWriteInInspector, UnityEngine.Tooltip("For setup only")] public float               damping; // ->> Movement entirely handled by attached `UnityEngine.Rigidbody`
    [ReadOnlyInInspector]                                         public UnityEngine.Vector3 direction;
    [ReadWriteInInspector]                                        public double              pause; // ->> Discretized movement
    [ReadOnlyInInspector, System.NonSerialized]                   public Timeframe           pauseCooldown;
    [ReadWriteInInspector]                                        public double              pauseRandomnessFactor;
    [ReadWriteInInspector]                                        public float               speed;
    [ReadWriteInInspector, UnityEngine.Range(0.25f, 2.00f)]       public float               speedFactor;
    [ReadWriteInInspector, UnityEngine.Range(0.00f, 1.00f)]       public float               speedRandomnessFactor; // ->> How randomly `movement.speed` is evaluated
  }

  [System.Serializable]
  public struct OutlineInfo {
    [ReadOnlyInInspector] public UnityEngine.Color     color;
    [ReadOnlyInInspector] public UnityEngine.Material? material;
  }

  [System.Serializable]
  protected /* readonly */ struct Prefollow /* ->> Cached set of values before and after following another `Entity` */ {
    [ReadOnlyInInspector] public bool                bounceAutomatically;
    [ReadOnlyInInspector] public bool                isFollowing;
    [ReadOnlyInInspector] public bool                isInvincible;
    [ReadOnlyInInspector] public UnityEngine.Vector3 localScale;
    [ReadOnlyInInspector] public bool                moveAutomatically;
    [ReadOnlyInInspector] public float               movementSpeed;
    [ReadOnlyInInspector] public float               targetBerth;
  }

  [System.Serializable]
  public struct RegenInfo {
    [ReadWriteInInspector] public float     amount;
    [ReadWriteInInspector] public Timeframe delay;
    [ReadWriteInInspector] public Timeframe interval;
  }

  [System.Serializable]
  public struct ShadowInfo {
    [ReadOnlyInInspector] public UnityEngine.Material? material;
  }

  [System.Serializable]
  public struct ShootInfo {
    [ReadWriteInInspector]                                  public byte                   bulletHealth;
    [ReadWriteInInspector]                                  public float                  bulletLifetime;
    [ReadWriteInInspector]                                  public UnityEngine.Material?  bulletMaterial;
    [ReadWriteInInspector]                                  public Timeframe              cooldown;
    [ReadWriteInInspector]                                  public float                  damage;
    [ReadWriteInInspector]                                  public System.Predicate<bool> isAllowed;
    [ReadWriteInInspector]                                  public float                  repelForce;
    [ReadWriteInInspector]                                  public float                  speed;
    [ReadWriteInInspector]                                  public bool                   spinAutomatically;
    [ReadWriteInInspector, UnityEngine.Range(0.0f, 360.0f)] public float                  view; // ->> in Degrees
    [ReadWriteInInspector]                                  public byte                   volleyCount;
    [ReadWriteInInspector]                                  public byte                   volleyRandomCount;
    [ReadWriteInInspector]                                  public float                  volleyDelay;
  }

  [System.Serializable]
  public struct StatisticsInfo {
    [ReadWriteInInspector] public bool health;
    [ReadWriteInInspector] public bool shoot;
  }

  public enum Team : byte { Player, Explorer = Player, Enemy, /* ->> Series of `Enemy + …` for other teams */ Nomad = Enemy + 1, Magnate = Enemy + 2 }

  public readonly record struct Tracked(UnityEngine.Behaviour tracking, UnityEngine.Vector3 distance = default, UnityEngine.Vector3 origin = default); // ->> Cameras, lights, e.t.c. that follow this `Entity`

  [System.Serializable]
  public struct TrackingInfo {
    [ReadWriteInInspector] public /* readonly */ System.Collections.Generic.List<UnityEngine.Camera> cameras;
    [ReadWriteInInspector] public /* readonly */ System.Collections.Generic.List<UnityEngine.Light>  lights;
  }

  public enum TurnDirection : byte { Anticlockwise, Clockwise }

  [System.Serializable]
  public struct TurnInfo {
    [ReadOnlyInInspector]  public UnityEngine.Vector3 direction;
    [ReadWriteInInspector] public float               speed; // ->> Degrees per second
  }

  /* … */
  public static readonly System.Collections.Generic.List<Entity> All                                = new(32); // --> UnityEngine.Object.FindObjectsByType<Entity>(UnityEngine.FindObjectsInactive.Include, UnityEngine.FindObjectsSortMode.None)
  public const           float                                   BulletSpinSpeed                    = 180.0f;  // ->> Degrees per second
  public const           float                                   DefeatedShrinkFactor               = 0.03f;
  public const           float                                   DefeatedSinkHeight                 = 0.50f;
  public const           float                                   DefeatedSwayDuration               = 0.10f;
  public const           float                                   DefeatedSwayFactor                 = 1.00f;
  public static          UnityEngine.Vector3                     MovementVelocityThreshold { get; } = UnityEngine.Vector3.one * 1.0f; // ->> Threshold velocity determining `Entity` movement

  [ReadWriteInInspector]                                          public                   Entity.AudioClipInfo                                          audioClips          = new() { /* … = null */ };
  [ReadOnlyInInspector]                                           protected                bool                                                          actuallyDefeated    = false;
  [ReadWriteInInspector]                                          public                   Entity.BounceInfo                                             bounce              = new() {angle = 10.00f, estimatedHeight = 0.00f, force = 0.75f, speed = 5.00f, turn = Entity.TurnDirection.Clockwise};
  [ReadWriteInInspector]                                          public                   bool                                                          bounceAutomatically = true;
  [ReadOnlyInInspector, UnityEngine.SerializeField]               internal  /* readonly */ System.Collections.Generic.List<Bullet>                       bullets             = new(8); // ->> “Shots fired”
  [ReadWriteInInspector]                                          public                   Entity.ContactInfo                                            contact             = new() {damage = 4.00f, repelForce = 2.50f};
  [ReadWriteInInspector]                                          public                   Entity.DefeatInfo                                             defeat              = new() {position = UnityEngine.Vector3.zero, timeout = new(4.00)};
  [ReadWriteInInspector, UnityEngine.Tooltip("For testing only")] public                   bool                                                          defeatAutomatically = false;  // ->> Defeats the `Entity` whenever updated and `true`
  [ReadWriteInInspector]                                          public                   bool                                                          followAutomatically = true;   // ->> Act on `followers`/ `following` relationship
  [ReadWriteInInspector, UnityEngine.SerializeField]              public    /* readonly */ System.Collections.Generic.List<Entity>                       followers           = new(1); // ->> `Entity`s `following` `this` one;                         unable to automatically update `followers[…].following`
  [ReadWriteInInspector]                                          public                   Entity?                                                       following           = null;   // ->> `Entity` to go to (typically an ally) when not targeting; unable to automatically update `following   .followers`
  [ReadWriteInInspector, UnityEngine.Range(0.0f, 1.0f)]           public                   float                                                         health              = 1.0f;
  [ReadWriteInInspector]                                          public    /* readonly */ float                                                         healthMaximum       = 100.00f;
  [ReadWriteInInspector]                                          public                   bool                                                          isAggressive        = true; // ->> Sets `target` to `Damage(…, attacker)` i.e. aggressed
  [ReadWriteInInspector]                                          public                   bool                                                          isDefeated          { get => this.actuallyDefeated; set => this.actuallyDefeated = this.actuallyDefeated || value; } // ->> Cannot be revived
  [ReadWriteInInspector]                                          public                   bool                                                          isInvincible        = false;
  [ReadWriteInInspector]                                          public                   bool                                                          moveAutomatically   = true; // ->> Go to `target`’s position
  [ReadWriteInInspector]                                          public                   Entity.MovementInfo                                           movement            = new() {damping = 1.00f, direction = UnityEngine.Vector3.zero, pause = 0.00, pauseCooldown = new(0.00), pauseRandomnessFactor = 0.75, speed = 2.00f, speedFactor = 1.00f, speedRandomnessFactor = 0.30f};
  [ReadWriteInInspector]                                          public                   Entity.OutlineInfo                                            outline             = new() {color = UnityEngine.Color.white, material = null};
  [ReadWriteInInspector, UnityEngine.SerializeField]              protected                Entity.Prefollow                                              prefollow           = new() {bounceAutomatically = true, isFollowing = false, isInvincible = false, localScale = UnityEngine.Vector3.one, moveAutomatically = true, movementSpeed = 2.00f, targetBerth = 2.25f};
  [ReadOnlyInInspector]                                           protected                bool                                                          prefollowIsUpdated  = false;
  [ReadWriteInInspector]                                          public                   Entity.RegenInfo                                              regeneration        = new() {amount = 0.00f, delay = new(0.00), interval = new(2.00)};
  [ReadWriteInInspector]                                          public                   uint                                                          score               = 10u;
  [ReadWriteInInspector]                                          public                   Entity.ShadowInfo                                             shadow              = new() {material = null};
  [ReadWriteInInspector]                                          public                   Entity.ShootInfo                                              shoot               = new() {bulletHealth = (byte) 1u, bulletLifetime = 2.0f, bulletMaterial = null, cooldown = new(1.00, Timeframe.EaseOutSine), damage = 6.75f, isAllowed = static allowed => allowed, repelForce = 1.0f, speed = 3.50f, spinAutomatically = true, view = 21.0f, volleyCount = (byte) 0u, volleyRandomCount = (byte) 0u, volleyDelay = 0.0f};
  [ReadWriteInInspector]                                          public                   bool                                                          shootAutomatically  = true; // ->> Auto-fire “friendliness pellets”
  [ReadWriteInInspector]                                          public                   Entity.StatisticsInfo                                         statistics          = new() {health = true, shoot = true};
  [ReadWriteInInspector]                                          public                   Entity?                                                       target              = null; // ->> `Entity` to go to (typically an enemy)
  [ReadWriteInInspector]                                          public                   bool                                                          targetAutomatically = true;
  [ReadWriteInInspector]                                          public                   float                                                         targetBerth         = 2.25f; // ->> Radius
  [ReadWriteInInspector]                                          public                   Entity.Team                                                   team                = Entity.Team.Enemy;
  [ReadOnlyInInspector, System.NonSerialized]                     protected /* readonly */ System.Collections.Generic.List<Entity.Tracked>               tracked             = new(1);
  [ReadWriteInInspector]                                          public                   Entity.TrackingInfo                                           tracking            = new() {cameras = new(1), lights = new(1)};
  [ReadWriteInInspector]                                          public                   Entity.TurnInfo                                               turn                = new() {direction = UnityEngine.Vector3.zero, speed = 9.00f};
  [ReadOnlyInInspector, UnityEngine.SerializeField]               protected /* readonly */ System.Collections.Generic.List<UnityEngine.Rendering.Volume> volumes             = new(1);
  [ReadOnlyInInspector, System.NonSerialized]                     public                   UnityEngine.Rendering.VolumeComponent?                        worldVignette       = null;

  /* … */
  protected virtual void Awake() {
    this.shoot.bulletMaterial           ??= Assets.main.bullet.material;
    this.shoot.bulletMaterial             = null != this.shoot.bulletMaterial ? new UnityEngine.Material(this.shoot.bulletMaterial) : null;
    this.prefollow.targetBerth            = this.targetBerth;
    this.prefollow.movementSpeed          = this.movement.speed;
    this.prefollow.moveAutomatically      = this.moveAutomatically;
    this.prefollow.localScale             = this.transform.localScale;
    this.prefollow.isInvincible           = this.isInvincible;
    this.prefollow.bounceAutomatically    = this.bounceAutomatically;
    this.movement.pauseCooldown.duration  = this.movement.pause;
    this.defeat.timeout.easing            = Timeframe.EaseInOutQuintic;
    this.defeat.position                  = this.transform.localPosition;
    base.rigidBody.useGravity             = false;
    base.rigidBody.linearDamping          = this.movement.damping;
    base.rigidBody.isKinematic            = false;
    base.rigidBody.interpolation          = UnityEngine.RigidbodyInterpolation.None == base.rigidBody.interpolation ? UnityEngine.RigidbodyInterpolation.Extrapolate : base.rigidBody.interpolation;
    base.rigidBody.includeLayers          = (UnityEngine.LayerMask) ~0;
    base.rigidBody.freezeRotation         = true;
    base.rigidBody.excludeLayers          = (UnityEngine.LayerMask) 0;
    base.rigidBody.detectCollisions       = true;
    base.rigidBody.constraints            = UnityEngine.RigidbodyConstraints.FreezePositionY | UnityEngine.RigidbodyConstraints.FreezeRotation;
    base.rigidBody.collisionDetectionMode = UnityEngine.CollisionDetectionMode.Discrete;
    base.collider.isTrigger               = true;
    base.collider.includeLayers           = (UnityEngine.LayerMask) ~0;
    base.collider.hasModifiableContacts   = false;
    base.collider.excludeLayers           = (UnityEngine.LayerMask) 0;

    // … ->> Moving
    this.movement.pauseCooldown.Reset();
    this.movement.pauseCooldown.Wait (UnityEngine.Random.value * (this.movement.pause + this.movement.pauseRandomnessFactor));

    // … ->> Shooting
    this.shoot.cooldown.Reset();

    // … ->> Outlining
    if (Assets.main.outlineAutomatically && null != Assets.main.outlineMaterial) {
      this.outline.color    = Assets.main.outlineMaterial.color;
      this.outline.material = new(Assets.main.outlineMaterial);

      this.transform.ForEach<UnityEngine.Renderer>(renderer => {
        UnityEngine.Material[] materials    = renderer.sharedMaterials;
        UnityEngine.Material[] submaterials = new UnityEngine.Material[materials.Length + 1];

        // …
        materials.CopyTo(submaterials, 0);

        submaterials[materials.Length] = this.outline.material;
        renderer.receiveShadows        = false;
        renderer.shadowCastingMode     = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.sharedMaterials       = submaterials;
      });
    }

    // … ->> Shadowing
    if (Assets.main.shadowAutomatically && null != Assets.main.shadowMaterial)
    if (UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cylinder) is UnityEngine.GameObject shadow) {
      UnityEngine.Bounds   bounds         = new(this.transform.position, UnityEngine.Vector3.one * 0.5f);
      UnityEngine.Renderer shadowRenderer = shadow.GetComponent<UnityEngine.Renderer>();

      // …
      this.transform.ForEach<UnityEngine.Renderer>(renderer => bounds.Encapsulate(renderer.bounds));

      bounds.size                      = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, bounds.size);
      shadowRenderer.lightProbeUsage   = UnityEngine.Rendering.LightProbeUsage.Off;
      shadowRenderer.receiveShadows    = false;
      shadowRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
      shadowRenderer.sharedMaterial    = Assets.main.shadowMaterial;
      shadow.name                      = "Shadow";
      shadow.transform.localScale      = (UnityEngine.Vector3.up * Game.VectorEpsilon * 2.00f) + ((UnityEngine.Vector3.forward + UnityEngine.Vector3.right) * UnityEngine.Mathf.Max(UnityEngine.Mathf.Max(bounds.size.x, bounds.size.y), bounds.size.z) * 0.75f);
      this.shadow.material             = new(Assets.main.shadowMaterial);

      shadow.transform.SetParent                  (this.transform, true);
      shadow.transform.SetLocalPositionAndRotation(UnityEngine.Vector3.down * Game.VectorEpsilon, UnityEngine.Quaternion.identity);

      foreach (UnityEngine.Collider collider in shadow.GetComponents<UnityEngine.Collider>())
      UnityEngine.Object.Destroy(collider);
    }

    // … ->> Following
    foreach (Entity follower in this.followers)
    follower.team = this.team;

    // …
    if (null != this.audioClips.introducing) {
      base.audioSource.pitch = (UnityEngine.Random.value * 2.0f) + 1.0f;
      base.audioSource.PlayOneShot(this.audioClips.introducing, 0.2f);
    }
  }

  public         void ContactDamage(Entity entity)                   => this.Damage(entity, this.contact.damage, this);
  public virtual void ContactDamage(Entity entity, Entity? attacker) => this.Damage(entity, this.contact.damage, attacker);

  public    void ContactRepel(Entity entity) => this.ContactRepel(entity, (entity.collider.bounds.center - base.collider.bounds.center).normalized);
  protected void ContactRepel(Entity entity, in UnityEngine.Vector3 direction) {
    if (UnityEngine.Vector3.zero != direction)
    entity.rigidBody.AddForce(direction * this.contact.repelForce, UnityEngine.ForceMode.Impulse);

    #if DEBUG || DEVELOPMENT_BUILD
      UnityEngine.Debug.DrawRay(entity.rigidBody.position, direction * Game.SceneSize,          UnityEngine.Color.red,   0.0f, false);
      UnityEngine.Debug.DrawRay(entity.rigidBody.position, direction * this.contact.repelForce, UnityEngine.Color.green, 0.0f, false);
    #endif
  }

  public         void Damage(Entity entity, float amount) => this.Damage(entity, amount, this);
  public virtual void Damage(Entity entity, float amount, Entity? attacker) {
    if (entity == this || (null != attacker && attacker.team == entity.team))
    return;

    amount        = !entity.isInvincible ? amount / entity.healthMaximum : 0.0f;
    entity.health = UnityEngine.Mathf.Clamp01(entity.health - amount);
    entity.target = null != attacker && !attacker.isDefeated && entity.isAggressive ? attacker : entity.target;

    if (0.0f != amount && null != attacker)
    entity.rigidBody.linearVelocity *= 0.9f;

    if (!entity.isInvincible) {
      // … ->> Regenerating
      entity.regeneration.delay.Reset();

      // … ->> Outlining
      if (null != entity.outline.material)
      entity.outline.material.color = UnityEngine.Color.red;

      // … ->> Vignette
      if (entity is Player)
      switch (entity.worldVignette) {
        case UnityEngine.Rendering.HighDefinition.Vignette highDefinitionRenderVignette: {
          highDefinitionRenderVignette.color    .value  = UnityEngine.Color.red;
          highDefinitionRenderVignette.intensity.value *= 1.1f;
        } break;

        case UnityEngine.Rendering.Universal.Vignette universalRenderVignette: {
          universalRenderVignette.color    .value  = UnityEngine.Color.red;
          universalRenderVignette.intensity.value *= 1.1f;
        } break;
      }
    }

    else {
      // … ->> Outlining
      if (null != entity.outline.material)
      entity.outline.material.color = UnityEngine.Color.deepSkyBlue;
    }
  }

  protected System.Collections.ObjectModel.ReadOnlyCollection<Entity>? FindTargets                                                                                (Entity.FindTargetsOptions options = Entity.FindTargetsOptions.Any) => this.FindTargets(this.FindTargetsFilter(), this.FindTargetsSorter(), options);
  protected System.Collections.ObjectModel.ReadOnlyCollection<Entity>? FindTargets(System.Predicate<Entity>        filter,                                         Entity.FindTargetsOptions options = Entity.FindTargetsOptions.Any) => this.FindTargets(filter,                   this.FindTargetsSorter(), options);
  protected System.Collections.ObjectModel.ReadOnlyCollection<Entity>? FindTargets(System.Converter<Entity, float> sorter,                                         Entity.FindTargetsOptions options = Entity.FindTargetsOptions.Any) => this.FindTargets(this.FindTargetsFilter(), sorter,                   options);
  protected System.Collections.ObjectModel.ReadOnlyCollection<Entity>? FindTargets(System.Predicate<Entity>        filter, System.Converter<Entity, float> sorter, Entity.FindTargetsOptions options = Entity.FindTargetsOptions.Any) {
    if (0 != Entity.All.Count) {
      System.Collections.Generic.SortedList<float, Entity> entities = new(Entity.All.Count, System.Collections.Generic.Comparer<float>.Default);

      // …
      foreach (Entity entity in Entity.All)
      if (entity != this && entity.enabled && !entity.isDefeated && (entity is not Monster monster || !monster.isMounted) && (
        0x0 != (options & Entity.FindTargetsOptions.DifferentTeam) ? entity.team != this.team :
        0x0 != (options & Entity.FindTargetsOptions.SameTeam)      ? entity.team == this.team :
        true
      ) && filter(entity)) {
        float entityComparison = sorter(entity);

        // …
        if (!entities.ContainsKey(entityComparison))
        entities.Add(entityComparison, entity);
      }

      entities.TrimExcess(); // --> entities.Capacity = entities.Count;
      return new System.Collections.ObjectModel.ReadOnlyCollection<Entity>(entities.Values);
    }

    return null; // --> System.Collections.ObjectModel.ReadOnlyCollection<Entity>.Empty;
  }

  protected virtual System.Predicate<Entity>        FindTargetsFilter() => static entity => entity.enabled && !entity.isDefeated;
  protected virtual System.Converter<Entity, float> FindTargetsSorter() =>        entity => (entity.rigidBody.position - base.rigidBody.position).sqrMagnitude;

  private void FixedUpdate() {
    base.rigidBody.inertiaTensor         = UnityEngine.Vector3.forward + UnityEngine.Vector3.up;
    base.rigidBody.inertiaTensorRotation = UnityEngine.Quaternion.identity;
  }

  protected virtual void OnApplicationFocus(bool _) { /* Do nothing… */ }
  protected         void OnApplicationQuit ()       => this.OnApplicationFocus(false);

  protected virtual void OnDestroy() {
    if (null != this.outline.material)     UnityEngine.Object.Destroy(this.outline.material);
    if (null != this.shoot.bulletMaterial) UnityEngine.Object.Destroy(this.shoot.bulletMaterial);
    if (null != this.following)            this.following.followers.Remove(this);

    foreach (Entity.Tracked tracked in this.tracked) {
      if (null != tracked.tracking)
      tracked.tracking.transform.position = tracked.origin;
    }

    if (null != this.audioClips.defeating) {
      base.audioSource.pitch = (UnityEngine.Random.value * 2.0f) + 1.0f;
      base.audioSource.PlayOneShot(this.audioClips.defeating, 0.2f);
    }
  }

  private void OnDisable() => Entity.All.Remove(this);
  private void OnEnable () => Entity.All.Add   (this);

  protected virtual void OnTriggerEnter(UnityEngine.Collider collider) {
    if (!this.isDefeated && collider.TryGetComponent(out Player player) && player.team != this.team)
    this.ContactDamage(player);
  }

  protected virtual void OnTriggerStay(UnityEngine.Collider collider) {
    if (!this.isDefeated && collider.TryGetComponent(out Player player) && player.team != this.team)
    this.ContactRepel(player);
  }

  public         void PassiveDamage(Entity entity, float amount, float interval) => this.PassiveDamage(entity, amount, interval, ushort.MaxValue);
  public virtual void PassiveDamage(Entity entity, float amount, float interval, ushort count) {
    System.Collections.IEnumerator Damage() {
      if (0u != count--) {
        yield return new UnityEngine.WaitForSeconds(interval);

        this.Damage(entity, amount, null);
        base.StartCoroutine(Damage());
      }

      yield break;
    }

    base.StartCoroutine(Damage());
  }

  protected virtual void PrefollowUpdate() {
    if (Game.IsPaused || this.isDefeated)
    return;

    // …
    this.movement.pauseCooldown.duration = (this is not Player ? this.movement.pauseCooldown.isLooped : this.movement.pauseCooldown.isElapsed) ? this.movement.pause + (UnityEngine.Random.value * this.movement.pauseRandomnessFactor * (UnityEngine.Random.value < 0.5 ? +1.0 : -1.0)) : this.movement.pauseCooldown.duration;
    this.prefollowIsUpdated              = false;

    if (null == this.following || this.prefollow.isFollowing != (null != this.following)) {
      this.prefollow.bounceAutomatically = this.bounceAutomatically;
      this.prefollow.isInvincible        = this.isInvincible;
      this.prefollow.localScale          = this.transform.localScale;
      this.prefollow.moveAutomatically   = this.moveAutomatically;
      this.prefollow.movementSpeed       = this.movement.speed;
      this.prefollow.targetBerth         = this.targetBerth;
      this.prefollowIsUpdated            = true;
    }

    this.prefollow.isFollowing = null != this.following;
  }

  protected void PublishTracked() {
    UnityEngine.Vector3 position      = this.transform.position;
    bool[]              trackedRemove = new bool[this.tracked.Count]; // --> System.Collections.BitArray

    /* … */
    void Publish<T>(System.Collections.Generic.List<T> trackingList) where T : UnityEngine.Behaviour {
      foreach (T tracking in trackingList) {
        int index = this.tracked.Count;

        // …
        while (0 != index--) {
          if (tracking == this.tracked[index].tracking)
          break;
        }

        if (index != -1) trackedRemove[index] = false;
        else this.tracked.Add(new(tracking) {distance = tracking.transform.position - position, origin = tracking.transform.position});
      }
    }

    /* … */
    System.Array.Fill(trackedRemove, true);

    Publish(this.tracking.cameras);
    Publish(this.tracking.lights);

    for (int index = trackedRemove.Length; 0 != index--; ) {
      if (trackedRemove[index])
      this.tracked.RemoveAt(index);
    }
  }

  public static void Reset(bool strict = false) {
    for (int index = Entity.All.Count; 0 != index--; ) {
      Entity entity = Entity.All[index];

      // …
      if (strict || !entity.isInvincible)
      UnityEngine.Object.Destroy(entity.gameObject); // --> ….isDefeated = true
    }
  }

  public    virtual Bullet? Shoot()                                               => this.Shoot(static (bullet, index, count) => {}); // ->> Override-able because it is called by default
  protected         Bullet? Shoot(System.Action<Bullet>                 callback) => this.Shoot(       (bullet, index, count) => callback(bullet));
  protected         Bullet? Shoot(System.Action<Bullet, ushort>         callback) => this.Shoot(       (bullet, index, count) => callback(bullet, index));
  protected         Bullet? Shoot(System.Action<Bullet, ushort, ushort> callback) {
    if (this.shoot.cooldown.isLooped) {
      Bullet? bullet = null;
      ushort  count  = (ushort) (this.shoot.volleyCount + (UnityEngine.Random.value * this.shoot.volleyRandomCount));
      ushort  index  = (ushort) 1u;

      /* … */
      System.Collections.IEnumerator DeployBullet(bool isSuccessive) {
        if (null != this.transform) {
          UnityEngine.Vector3 shootDirection = UnityEngine.Vector3.zero == this.turn.direction ? this.transform.forward : this.turn.direction;

          // …
          bullet                = ((UnityEngine.GameObject) UnityEngine.Object.Instantiate(Assets.main.bullet.prefabrication, this.transform.position, UnityEngine.Quaternion.LookRotation(shootDirection, UnityEngine.Vector3.up))).AddComponent<Bullet>();
          bullet.health         = this.shoot.bulletHealth;
          bullet.lifetime       = this.shoot.bulletLifetime;
          bullet.name           = "Bullet " + (this.bullets.Count + 1);
          bullet.shootDirection = shootDirection;
          bullet.user           = this;
          bullet.volleyCount    = count;

          this.bullets.Add(bullet);
          bullet.transform.ForEach<UnityEngine.Renderer>(renderer => {
            renderer.lightProbeUsage   = UnityEngine.Rendering.LightProbeUsage.Off;
            renderer.receiveShadows    = false;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.sharedMaterial    = this.shoot.bulletMaterial ?? new UnityEngine.Material(renderer.sharedMaterial); // ->> Invisible bullets are indeed possible
          });

          if (isSuccessive) {
            bullet.volleyIndex = index;
            callback(bullet, index, count);
          }

          if (null != this.audioClips.shooting) {
            base.audioSource.pitch = (UnityEngine.Random.value * 2.0f) + 1.0f;
            base.audioSource.PlayOneShot(this.audioClips.shooting, 0.2f);
          }

          #if DEBUG || DEVELOPMENT_BUILD
            UnityEngine.Debug.DrawRay(bullet.rigidBody.position, shootDirection * Game.SceneSize, isSuccessive ? UnityEngine.Color.blue : UnityEngine.Color.red, 0.0f, false);
          #endif
        }

        if (count >= index++) {
          yield return new UnityEngine.WaitForSeconds(this.shoot.volleyDelay);
          base.StartCoroutine(DeployBullet(true));
        }

        yield break;
      }

      /* … */
      base.StartCoroutine(DeployBullet(false));
      return bullet;
    }

    return null;
  }

  public         void ShootDamage(Entity entity)                                   => this.ShootDamage(entity, UnityEngine.Vector3.zero, this);
  public         void ShootDamage(Entity entity, Entity? attacker)                 => this.ShootDamage(entity, UnityEngine.Vector3.zero, attacker);
  public         void ShootDamage(Entity entity, in UnityEngine.Vector3 direction) => this.ShootDamage(entity, direction,                this);
  public virtual void ShootDamage(Entity entity, in UnityEngine.Vector3 direction, Entity? attacker) {
    this.Damage(entity, this.shoot.damage);

    if (UnityEngine.Vector3.zero != direction)
    this.ShootRepel(entity, direction);
  }

  private void ShootRepel(Entity entity, in UnityEngine.Vector3 direction) {
    float contactRepelForce = this.contact.repelForce;

    // … ->> Badly designed?
    this.contact.repelForce = this.shoot.repelForce;
    this.ContactRepel(entity, direction);
    this.contact.repelForce = contactRepelForce;
  }

  public static Entity.TurnDirection TurnDirectionReverse(Entity.TurnDirection turn) => turn switch {
    Entity.TurnDirection.Anticlockwise => Entity.TurnDirection.Clockwise,
    Entity.TurnDirection.Clockwise     => Entity.TurnDirection.Anticlockwise,
    _                                  => turn
  };

  public static UnityEngine.Vector3 TurnDirectionToVector3(Entity.TurnDirection turn, in UnityEngine.Vector3 direction) => turn switch {
    Entity.TurnDirection.Anticlockwise => -direction,
    Entity.TurnDirection.Clockwise     =>  direction,
    _                                  =>  UnityEngine.Vector3.zero
  };

  protected override void Update() {
    UnityEngine.Vector3 targetDistance  = null != this.target ? this.target.transform.position - this.transform.position : UnityEngine.Vector3.zero;
    UnityEngine.Vector3 targetDirection = targetDistance.normalized;
    UnityEngine.Vector3 rotation        = base.rigidBody.rotation.eulerAngles;

    // …
    base.Update();

    if (Game.IsPaused)
    return;

    // … ->> Defeating
    this.isDefeated = this.isDefeated || ((this.defeatAutomatically || this.health <= 0.0f) && !this.isInvincible);

    if (!this.isDefeated) {
      this.defeat.position = this.transform.position;
      this.defeat.timeout.Reset();
    }

    else {
      UnityEngine.Vector3 transformToCameraDirection     = base.worldCamera is not null ? (base.worldCamera.transform.position - this.transform.position).normalized : UnityEngine.Vector3.forward;
      float               transformForwardToCameraFactor = UnityEngine.Mathf.Abs(UnityEngine.Vector3.Dot(transformToCameraDirection, this.transform.forward));
      float               transformRightToCameraFactor   = UnityEngine.Mathf.Abs(UnityEngine.Vector3.Dot(transformToCameraDirection, this.transform.right));
      float               transformToCameraFactors       = transformForwardToCameraFactor + transformRightToCameraFactor;
      float               swayForce                      = (float) this.defeat.timeout.elapsed / Entity.DefeatedSwayDuration;
      UnityEngine.Vector3 swayDirection                  = Entity.TurnDirectionToVector3(0 == (((uint) swayForce) & 1) ? Entity.TurnDirection.Anticlockwise : Entity.TurnDirection.Clockwise, UnityEngine.Vector3.right);

      // … ->> Shrink, sink, and sway
      this.transform.position   = this.defeat.position + (swayDirection * ((swayForce - (uint) swayForce) - 0.5f) * Entity.DefeatedSwayFactor) + (UnityEngine.Vector3.up * -Entity.DefeatedSinkHeight * (float) this.defeat.timeout.easedProgress);
      this.transform.localScale = UnityEngine.Vector3.Scale(this.transform.localScale,
        (UnityEngine.Vector3.forward * (1.0f - (Entity.DefeatedShrinkFactor * (transformRightToCameraFactor   / transformToCameraFactors)))) +
        (UnityEngine.Vector3.right   * (1.0f - (Entity.DefeatedShrinkFactor * (transformForwardToCameraFactor / transformToCameraFactors)))) +
        (UnityEngine.Vector3.up      * (1.0f - 0.0f))
      );

      base.rigidBody.Sleep();
      this.transform.SetParent(null, true);

      // …
      if (this is Tamer tamer) {
        for (int index = this.followers.Count; 0 != index--; )
        tamer.Release(this.followers[index]);
      }

      foreach (Bullet bullet in this.bullets)
      bullet.health = (byte) 0u;

      // …
      if (this.defeat.timeout.isElapsed)
      UnityEngine.Object.Destroy(base.gameObject);

      return;
    }

    // … ->> Following/ Targeting
    if (this.followAutomatically && null != this.following) {
      this.movement.pauseCooldown.Finish();

      this.transform.localScale = this.prefollow.localScale    * 0.875f;
      this.movement.speed       = this.prefollow.movementSpeed * 1.365f;
      this.targetBerth          = this.prefollow.targetBerth   * 1.150f;
      this.target               = null != this.following.target && this.following.target != this ? this.following.target : this.following;
    }

    else {
      System.Collections.ObjectModel.ReadOnlyCollection<Entity> targets = this.FindTargets(Entity.FindTargetsOptions.DifferentTeam) ?? new(new System.Collections.Generic.List<Entity>(0)); // --> … ?? System.Collections.ObjectModel.ReadOnlyCollection<Entity>.Empty

      // …
      this.transform.localScale = this.prefollow.localScale    * 1.000f;
      this.movement.speed       = this.prefollow.movementSpeed * 1.000f;
      this.targetBerth          = this.prefollow.targetBerth   * 1.000f;
      this.target               = this.targetAutomatically && null == this.target && 0 != targets.Count ? targets[0] : this.target;
    }

    // … ->> Bouncing — Unfortunately bounces entire `Entity` object, rather than just its visible render
    if (this.bounceAutomatically && UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, Entity.MovementVelocityThreshold).sqrMagnitude < UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, base.rigidBody.linearVelocity).sqrMagnitude) {
      float bounceAngleThreshold = (UnityEngine.Vector3.forward * this.bounce.angle).sqrMagnitude;
      float bounceMagnitude;

      // … ->> Rotation — Apply Z-axis orientation (cumulative)
      base.rigidBody.constraints &= ~UnityEngine.RigidbodyConstraints.FreezeRotationZ;
      base.rigidBody.MoveRotation(UnityEngine.Quaternion.Euler(rotation + (Entity.TurnDirectionToVector3(this.bounce.turn, UnityEngine.Vector3.forward) * UnityEngine.Time.deltaTime * this.bounce.angle * this.bounce.speed)));

      rotation        = base.rigidBody.rotation.eulerAngles;
      rotation        = new(rotation.x > 180.0f ? rotation.x - 360.0f : rotation.x, rotation.y > 180.0f ? rotation.y - 360.0f : rotation.y, rotation.z > 180.0f ? rotation.z - 360.0f : rotation.z);
      bounceMagnitude = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, rotation).sqrMagnitude;

      if (bounceAngleThreshold <= bounceMagnitude) {
        base.rigidBody.MoveRotation(UnityEngine.Quaternion.Euler(
          UnityEngine.Vector3.Scale(UnityEngine.Vector3.right + UnityEngine.Vector3.up, rotation) +                                 // ->> Remove Z-axis orientation
          (Entity.TurnDirectionToVector3(this.bounce.turn, UnityEngine.Vector3.forward) * (this.bounce.angle - Game.VectorEpsilon)) // ->> Apply  Z-axis orientation
        ));

        this.bounce.turn = Entity.TurnDirectionReverse(this.bounce.turn);
      }

      // … ->> Position
      this.bounce.estimatedHeight     =  this.bounce.force * (bounceMagnitude / bounceAngleThreshold);
      base.rigidBody.position     =  UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, base.rigidBody.position) + (UnityEngine.Vector3.up * this.bounce.estimatedHeight);
      base.rigidBody.constraints &= ~UnityEngine.RigidbodyConstraints.FreezePositionY;
    }

    else {
      // … ->> Rotation — Remove Z-axis orientation
      base.rigidBody.constraints |= UnityEngine.RigidbodyConstraints.FreezeRotationZ;
      base.rigidBody.MoveRotation(UnityEngine.Quaternion.Slerp(
        UnityEngine.Quaternion.Euler(rotation),
        UnityEngine.Quaternion.Euler(UnityEngine.Vector3.Scale(UnityEngine.Vector3.right + UnityEngine.Vector3.up, rotation)),
        UnityEngine.Time.deltaTime * this.bounce.speed
      ));

      // … ->> Position
      this.bounce.estimatedHeight       = 0.0f;
      base.rigidBody.position       = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, base.rigidBody.position); // --> UnityEngine.Vector3.Slerp(base.rigidBody.position, UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, base.rigidBody.position), !this.movement.pauseCooldown.isElapsed ? UnityEngine.Time.deltaTime * this.bounce.force : 1.0f)
      base.rigidBody.linearVelocity = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, base.rigidBody.linearVelocity);
      base.rigidBody.constraints   |= UnityEngine.RigidbodyConstraints.FreezePositionY;
    }

    // … ->> Regenerating
    if (this.regeneration.delay.isElapsed) {
      if (this.regeneration.interval.isLooped)
      this.health = UnityEngine.Mathf.Clamp01(this.health + (this.regeneration.amount / this.healthMaximum));
    } else this.regeneration.interval.Wait();

    // … ->> Outlining
    if (null != this.outline.material)
    this.outline.material.color = UnityEngine.Color.LerpUnclamped(this.outline.material.color, this.outline.color, 0.1f);

    // … ->> Moving
    this.movement.direction = Player.IsReady && this.targetAutomatically ? targetDistance.sqrMagnitude > (UnityEngine.Vector3.one * this.targetBerth).sqrMagnitude ? targetDirection : UnityEngine.Vector3.zero : this.movement.direction;

    if (this.moveAutomatically && UnityEngine.Vector3.zero != this.movement.direction && (this is Player || this.movement.pauseCooldown.isElapsed)) {
      base.rigidBody.AddForce(this.movement.direction * this.movement.speedFactor * ((this.movement.speed * (1.0f - this.movement.speedRandomnessFactor)) + (UnityEngine.Random.value * this.movement.speed * this.movement.speedRandomnessFactor)), UnityEngine.ForceMode.Impulse);
      this.turn.direction = this.movement.direction;

      if (null != this.audioClips.moving && !base.audioSource.isPlaying) {
        base.audioSource.pitch = (UnityEngine.Random.value * 2.0f) + 1.0f;
        base.audioSource.PlayOneShot(this.audioClips.moving, 0.2f);
      }
    }

    // … ->> Turning
    this.turn.direction = Player.IsReady && this.targetAutomatically && (this is not Player || UnityEngine.Vector3.zero == this.turn.direction) ? targetDirection : this.turn.direction;

    if (UnityEngine.Vector3.zero != this.turn.direction)
    base.rigidBody.MoveRotation(UnityEngine.Quaternion.Slerp(base.rigidBody.rotation, UnityEngine.Quaternion.Euler(
      UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, base.rigidBody.rotation.eulerAngles) +                       // ->> Remove Y-axis orientation
      UnityEngine.Vector3.Scale(UnityEngine.Vector3.up, UnityEngine.Quaternion.LookRotation(this.turn.direction, UnityEngine.Vector3.up).eulerAngles) // ->> Apply  Y-axis orientation
    ), UnityEngine.Time.deltaTime * this.turn.speed));

    // … ->> Shooting
    if (Player.IsReady && this.shootAutomatically && null != this.target) {
      bool shootIsTargeted = this is Player || UnityEngine.Mathf.Cos(UnityEngine.Mathf.Deg2Rad * this.shoot.view) <= UnityEngine.Vector3.Dot(UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, this.transform.forward), UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, targetDirection));
      bool shootIsAllowed  = (this is not Monster monster || monster.following is not Player || !monster.isMounted) && this.shoot.isAllowed(shootIsTargeted);

      // …
      if (shootIsAllowed)
      this.Shoot();

      #if false
      #if DEBUG || DEVELOPMENT_BUILD
        UnityEngine.Debug.DrawLine(this.transform.position, this.target.transform.position, shootIsAllowed ? UnityEngine.Color.green : shootIsTargeted ? UnityEngine.Color.orange : UnityEngine.Color.red, 0.0f, false);
        UnityEngine.Debug.DrawRay (this.transform.position, Game.SceneSize * (UnityEngine.Quaternion.AngleAxis(+this.shoot.view, UnityEngine.Vector3.up) * this.transform.forward), UnityEngine.Color.blue, 0.0f, false);
        UnityEngine.Debug.DrawRay (this.transform.position, Game.SceneSize * (UnityEngine.Quaternion.AngleAxis(-this.shoot.view, UnityEngine.Vector3.up) * this.transform.forward), UnityEngine.Color.blue, 0.0f, false);
      #endif
      #endif
    }

    // … ->> Tracking
    this.PublishTracked();

    foreach (Entity.Tracked tracked in this.tracked)
    tracked.tracking.transform.position = tracked.distance + UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, this.rigidBody.position);

    // … ->> Pre-follow
    this.PrefollowUpdate();
  }
}
  [UnityEngine.DefaultExecutionOrder(5)]
  [UnityEngine.RequireComponent(typeof(UnityEngine.Collider))]
  [UnityEngine.RequireComponent(typeof(UnityEngine.Rigidbody))]
  public class Bullet : GameComponent {
    public const float CollisionScaleFactor = 1.5f;

    private                 UnityEngine.Renderer? explosion                = null;
    public                  UnityEngine.Vector3   explosionDetonationScale = UnityEngine.Vector3.one;
    public                  Timeframe             explosionDuration        = new(0.5);
    public                  byte                  health                   = (byte) 1u;
    internal                bool                  isHit                    = false; // ->> Final hit
    public                  bool                  isInvincible             = false;
    public                  float                 lifetime                 = 2.0f;
    public                  UnityEngine.Vector3   shootDirection           = UnityEngine.Vector3.zero; // ->> Initial line of fire
    public   /* readonly */ double                shootTimestamp           = 0.0;
    public                  Entity                user                     = null!; // ->> Must be set
    public                  ushort                volleyCount              = (ushort) 0u;
    public                  ushort                volleyIndex              = (ushort) 0u;

    /* … */
    private void Awake() {
      UnityEngine.Collider[] colliders = base.GetComponents<UnityEngine.Collider>();

      // …
      base.rigidBody.collisionDetectionMode = UnityEngine.CollisionDetectionMode.ContinuousDynamic;
      base.rigidBody.constraints            = UnityEngine.RigidbodyConstraints.FreezePositionY | UnityEngine.RigidbodyConstraints.FreezeRotationX | UnityEngine.RigidbodyConstraints.FreezeRotationY;
      base.rigidBody.excludeLayers          = (UnityEngine.LayerMask)  0;
      base.rigidBody.includeLayers          = (UnityEngine.LayerMask) ~0;
      base.rigidBody.interpolation          = UnityEngine.RigidbodyInterpolation.Interpolate;
      base.rigidBody.isKinematic            = false;
      base.collider.excludeLayers           = (UnityEngine.LayerMask) 0;
      base.collider.hasModifiableContacts   = false;
      base.collider.includeLayers           = (UnityEngine.LayerMask) ~0;
      base.collider.isTrigger               = true;
      this.shootTimestamp                   = Timeframe.CurrentTimestamp;
      this.explosionDetonationScale         = UnityEngine.Vector3.one * ((UnityEngine.Random.value * 2.0f) + 1.0f);
      this.explosionDuration.easing         = Timeframe.EaseOutSine;

      switch (base.collider) {
        case UnityEngine.BoxCollider     boxCollider:     boxCollider.size       *= Bullet.CollisionScaleFactor;                                                        break;
        case UnityEngine.CapsuleCollider capsuleCollider: capsuleCollider.height *= Bullet.CollisionScaleFactor; capsuleCollider.radius *= Bullet.CollisionScaleFactor; break;
        case UnityEngine.SphereCollider  sphereCollider:  sphereCollider.radius  *= Bullet.CollisionScaleFactor;                                                        break;
        case UnityEngine.MeshCollider    meshCollider: {
          UnityEngine.Mesh      mesh     = meshCollider.sharedMesh;
          UnityEngine.Vector3[] vertices = mesh.vertices;

          // …
          meshCollider.sharedMesh = null;

          for (uint index = (uint) vertices.Length; 0u != index--; )
            vertices[index] *= Bullet.CollisionScaleFactor; // ->> `vertices[index] += normals[index] * 1.0f` to additively scale

          mesh.vertices = vertices;
          mesh.RecalculateNormals();
          mesh.RecalculateBounds ();
          meshCollider.sharedMesh = mesh;
        } break;
      }

      for (uint index = (uint) colliders.Length; 0u != --index; )
      UnityEngine.Object.Destroy(colliders[index]);

      if (null != this.user)
      this.user.bullets.Add(this);
    }

    private void OnDestroy() {
      if (null != this.user)
      this.user.bullets.Remove(this);
    }

    private void OnTriggerEnter(UnityEngine.Collider collider) {
      if (this.isHit || null == this.user)
      return;


      if (collider.TryGetComponent(out Bullet bullet)) {
        if ((bullet.user != this.user && (null == bullet.user || bullet.user.team != this.user.team))) {
          this.health -= (byte) (0u == this.health || this.isInvincible ? 0u : 1u);
          /* Do nothing… */
        }

        return;
      }

      if (collider.TryGetComponent(out Entity entity)) {
        if (entity != this.user && entity.team != this.user.team && !this.user.followers.Contains(entity)) {
          this.health -= (byte) (0u == this.health || this.isInvincible ? 0u : 1u);
          this.user.ShootDamage(entity is Monster monster && monster.isMounted ? monster.following! : entity, this.shootDirection);
        }

        return;
      }

      if (collider.TryGetComponent(out Area _) || collider.TryGetComponent(out Lasoo _))
      return;

      this.health = (byte) 0u;
    }

    public void Travel() {
      if (this.isHit || null == this.user)
      return;

      base.rigidBody.AddForce    (this.shootDirection * this.user.shoot.speed, UnityEngine.ForceMode.Impulse);
      base.rigidBody.MoveRotation(UnityEngine.Quaternion.Euler(
        UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, base.rigidBody.rotation.eulerAngles) +                       // ->> Remove Y-axis orientation
        UnityEngine.Vector3.Scale(UnityEngine.Vector3.up, UnityEngine.Quaternion.LookRotation(this.shootDirection, UnityEngine.Vector3.up).eulerAngles) // ->> Apply  Y-axis orientation
      ));
    }

    protected override void Update() {
      base.Update();

      // …
      if (this.lifetime <= +0.0f || null == this.user) {
        UnityEngine.Object.Destroy(base.gameObject);
        return;
      }

      if (this.isHit) {
        if (this.explosion is not null) {
          float explosionDetonationProgress = (float) this.explosionDuration.easedProgress;

          // …
          this.explosion.transform.localScale = UnityEngine.Vector3.SlerpUnclamped(UnityEngine.Vector3.one, UnityEngine.Vector3.one + this.explosionDetonationScale, explosionDetonationProgress);

          foreach (UnityEngine.Material material in this.explosion.sharedMaterials)
          material.color = new(material.color.r, material.color.g, material.color.b, material.color.a * (1.0f - explosionDetonationProgress));
        }

        if (this.explosionDuration.isElapsed) {
          if (this.explosion is not null) {
            foreach (UnityEngine.Material material in this.explosion.sharedMaterials)
            UnityEngine.Object.Destroy(material);
          }

          UnityEngine.Object.Destroy(base.gameObject);
          return;
        }
      }

      else if (0u == this.health) {
        UnityEngine.GameObject? explosion         = null;
        UnityEngine.Renderer?   explosionRenderer = null;
        UnityEngine.Material[]? materials         = null;

        // …
        do {
          if (null != this.user.shoot.bulletMaterial) {
            materials = new UnityEngine.Material[] {this.user.shoot.bulletMaterial};
            break;
          }

          this.transform.ForEach<UnityEngine.Renderer>(renderer => materials ??= 0 != renderer.sharedMaterials.Length ? renderer.sharedMaterials : null);
          if (materials is not null) break;

          if (null != Assets.main.bullet.material) {
            materials = new UnityEngine.Material[] {Assets.main.bullet.material};
            break;
          }
        } while (false);

        if (materials is null) {
          UnityEngine.Object.Destroy(base.gameObject);
          return;
        }

        explosion                           = UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Sphere);
        explosion.name                      = (this.name + " Explosion").TrimStart();
        explosionRenderer                   = explosion.GetComponent<UnityEngine.Renderer>();
        explosionRenderer.receiveShadows    = false;
        explosionRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        explosionRenderer.sharedMaterials   = System.Array.ConvertAll(materials, static material => null == material ? null : new UnityEngine.Material(material));
        this.explosion                      = explosionRenderer;
        this.isHit                          = true;

        foreach (UnityEngine.Collider collider in explosion.GetComponents<UnityEngine.Collider>())
          UnityEngine.Object.Destroy(collider);

        foreach (UnityEngine.Transform transform in this.transform)
          UnityEngine.Object.Destroy(transform.gameObject);

        explosion.transform.SetParent                  (this.transform, false);
        explosion.transform.SetLocalPositionAndRotation(UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity);
        base.rigidBody.Sleep();
      }

      else
        this.explosionDuration.Reset();

      // …
      if (Game.IsPaused)
      return;

      this.lifetime -= UnityEngine.Time.deltaTime;

      if (this.user.shoot.spinAutomatically)
      base.rigidBody.MoveRotation(base.rigidBody.rotation * UnityEngine.Quaternion.AngleAxis(Entity.BulletSpinSpeed * UnityEngine.Time.deltaTime, UnityEngine.Vector3.forward));
    }
  }
