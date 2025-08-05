using PatchOdyssey;

/* … */
[UnityEngine.DefaultExecutionOrder(1)]
[UnityEngine.RequireComponent(typeof(UnityEngine.Collider))]
[UnityEngine.RequireComponent(typeof(UnityEngine.Rigidbody))]
public abstract class Entity : GameComponent /* ->> Source file must be named “Entity” */ {
  [System.Serializable]
  public struct BounceInfo {
    [ReadWriteInInspector] public float                angle; // ->> in Degrees --> 0.0f <= |bounce.angle| <= ~180.0f
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
    [ReadWriteInInspector]                                  public byte                   repeatCount;
    [ReadWriteInInspector]                                  public byte                   repeatRandomCount;
    [ReadWriteInInspector]                                  public float                  repeatDelay;
    [ReadWriteInInspector]                                  public float                  repelForce;
    [ReadWriteInInspector]                                  public float                  speed;
    [ReadWriteInInspector, UnityEngine.Range(0.0f, 360.0f)] public float                  view; // ->> in Degrees
  }

  public enum Team          : byte { Player, Explorer = Player, Enemy, /* ->> Series of `Enemy + …` for other teams */ Nomad = Enemy + 0, Magnate = Enemy + 1 }
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

  [ReadOnlyInInspector]                                 protected                UnityEngine.Camera?                     actualMainCamera    = null;
  [ReadOnlyInInspector]                                 protected                bool                                    actuallyDefeated    = false;
  [ReadWriteInInspector]                                public                   Entity.BounceInfo                       bounce              = new() {angle = 10.00f, force = 0.75f, speed = 5.00f, turn = Entity.TurnDirection.Clockwise};
  [ReadWriteInInspector]                                public                   bool                                    bounceAutomatically = true;
  [ReadOnlyInInspector, UnityEngine.SerializeField]     internal  /* readonly */ System.Collections.Generic.List<Bullet> bullets             = new(8); // ->> “Shots fired”
  [ReadWriteInInspector]                                public                   Entity.ContactInfo                      contact             = new() {damage = 4.00f, repelForce = 2.50f};
  [ReadWriteInInspector]                                public                   Entity.DefeatInfo                       defeat              = new() {position = UnityEngine.Vector3.zero, timeout = new(4.00)};
  [ReadWriteInInspector]                                public                   bool                                    followAutomatically = true;   // ->> Act on `followers`/ `following` relationship
  [ReadWriteInInspector, UnityEngine.SerializeField]    public    /* readonly */ System.Collections.Generic.List<Entity> followers           = new(1); // ->> `Entity`s `following` `this` one;                         unable to automatically update `followers[…].following`
  [ReadWriteInInspector]                                public                   Entity?                                 following           = null;   // ->> `Entity` to go to (typically an ally) when not targeting; unable to automatically update `following   .followers`
  [ReadWriteInInspector, UnityEngine.Range(0.0f, 1.0f)] public                   float                                   health              = 1.0f;
  [ReadWriteInInspector]                                public    /* readonly */ float                                   healthMaximum       = 100.00f;
  [ReadWriteInInspector]                                public                   bool                                    isAggressive        = true; // ->> Sets `target` to `Damage(…, attacker)` i.e. aggressed
  [ReadWriteInInspector]                                public                   bool                                    isDefeated          { get => this.actuallyDefeated; set => this.actuallyDefeated = this.actuallyDefeated || value; } // ->> Cannot be revived
  [ReadWriteInInspector]                                public                   bool                                    isInvincible        = false;
  [ReadOnlyInInspector]                                 protected                ref readonly UnityEngine.Camera?        mainCamera          { get { this.actualMainCamera = null == this.actualMainCamera ? UnityEngine.Camera.main : this.actualMainCamera; /* --> base.camera */ return ref this.actualMainCamera; } }
  [ReadWriteInInspector]                                public                   bool                                    moveAutomatically   = true; // ->> Go to `target`’s position
  [ReadWriteInInspector]                                public                   Entity.MovementInfo                     movement            = new() {damping = 1.00f, direction = UnityEngine.Vector3.zero, pause = 0.00, pauseCooldown = new(0.00), pauseRandomnessFactor = 0.75, speed = 2.00f, speedFactor = 1.00f, speedRandomnessFactor = 0.30f};
  [ReadWriteInInspector]                                public                   Entity.OutlineInfo                      outline             = new() {color = UnityEngine.Color.white, material = null};
  [ReadWriteInInspector, UnityEngine.SerializeField]    protected                Entity.Prefollow                        prefollow           = new() {bounceAutomatically = true, isFollowing = false, isInvincible = false, localScale = UnityEngine.Vector3.one, moveAutomatically = true, movementSpeed = 2.00f, targetBerth = 2.25f};
  [ReadOnlyInInspector]                                 protected                bool                                    prefollowIsUpdated  = false;
  [ReadWriteInInspector]                                public                   Player.RegenInfo                        regeneration        = new() {amount = 0.00f, delay = new(0.00), interval = new(2.00)};
  [ReadWriteInInspector]                                public                   Entity.ShadowInfo                       shadow              = new() {material = null};
  [ReadWriteInInspector]                                public                   Entity.ShootInfo                        shoot               = new() {bulletHealth = (byte) 1u, bulletLifetime = 2.0f, bulletMaterial = null, cooldown = new(1.00), damage = 6.75f, isAllowed = static allowed => allowed, repeatCount = (byte) 0u, repeatDelay = 0.0f, repelForce = 1.0f, repeatRandomCount = (byte) 0u, speed = 3.50f, view = 21.0f};
  [ReadWriteInInspector]                                public                   bool                                    shootAutomatically  = true; // ->> Auto-fire “friendliness pellets”
  [ReadWriteInInspector]                                public                   Entity?                                 target              = null; // ->> `Entity` to go to (typically an enemy)
  [ReadWriteInInspector]                                public                   bool                                    targetAutomatically = true;
  [ReadWriteInInspector]                                public                   float                                   targetBerth         = 2.25f; // ->> Radius
  [ReadWriteInInspector]                                public                   Entity.Team                             team                = Entity.Team.Enemy;
  [ReadWriteInInspector]                                public                   Entity.TurnInfo                         turn                = new() {direction = UnityEngine.Vector3.zero, speed = 9.00f};

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
    base.rigidBody.useGravity             = false;
    base.rigidBody.linearDamping          = this.movement.damping;
    base.rigidBody.isKinematic            = false;
    base.rigidBody.interpolation          = UnityEngine.RigidbodyInterpolation.None == base.rigidBody.interpolation ? UnityEngine.RigidbodyInterpolation.Extrapolate : base.rigidBody.interpolation;
    base.rigidBody.includeLayers          = (UnityEngine.LayerMask) ~0x0;
    base.rigidBody.freezeRotation         = true;
    base.rigidBody.excludeLayers          = (UnityEngine.LayerMask) 0x0;
    base.rigidBody.detectCollisions       = true;
    base.rigidBody.constraints            = UnityEngine.RigidbodyConstraints.FreezePositionY | UnityEngine.RigidbodyConstraints.FreezeRotation;
    base.rigidBody.collisionDetectionMode = UnityEngine.CollisionDetectionMode.Discrete;
    base.collider.isTrigger               = true;
    base.collider.includeLayers           = (UnityEngine.LayerMask) ~0x0;
    base.collider.hasModifiableContacts   = false;
    base.collider.excludeLayers           = (UnityEngine.LayerMask) 0x0;

    Entity.All.Add(this);

    // … ->> Moving
    this.movement.pauseCooldown.Reset();
    this.movement.pauseCooldown.Wait (UnityEngine.Random.value * (this.movement.pause + this.movement.pauseRandomnessFactor));

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
      UnityEngine.Bounds bounds = new(this.transform.position, UnityEngine.Vector3.one * 0.5f);

      // …
      this.transform.ForEach<UnityEngine.Renderer>(renderer => bounds.Encapsulate(renderer.bounds));

      bounds.size                                                = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, bounds.size);
      shadow.GetComponent<UnityEngine.Renderer>().sharedMaterial = Assets.main.shadowMaterial;
      shadow.name                                                = "Shadow";
      shadow.transform.localScale                                = (UnityEngine.Vector3.up * Game.VectorEpsilon * 2.00f) + ((UnityEngine.Vector3.forward + UnityEngine.Vector3.right) * UnityEngine.Mathf.Max(UnityEngine.Mathf.Max(bounds.size.x, bounds.size.y), bounds.size.z) * 0.75f);
      this.shadow.material                                       = new(Assets.main.shadowMaterial);

      shadow.transform.SetParent                  (this.transform, true);
      shadow.transform.SetLocalPositionAndRotation(UnityEngine.Vector3.down * Game.VectorEpsilon, UnityEngine.Quaternion.identity);

      foreach (UnityEngine.Collider collider in shadow.GetComponents<UnityEngine.Collider>())
      UnityEngine.Object.Destroy(collider);
    }

    // … ->> Following
    foreach (Entity follower in this.followers)
    follower.team = this.team;
  }

  public virtual void ContactDamage(Entity entity)                   => this.Damage(entity, this.contact.damage, this);
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

  public virtual void Damage(Entity entity, float amount) => this.Damage(entity, amount, this);
  public virtual void Damage(Entity entity, float amount, Entity? attacker) {
    if (attacker == entity || entity == this)
    return;

    if (!entity.isInvincible)                entity.regeneration.delay.Reset();
    if (entity.outline.material is not null) entity.outline.material.color = !entity.isInvincible ? UnityEngine.Color.red : UnityEngine.Color.deepSkyBlue;

    entity.health = UnityEngine.Mathf.Clamp01(entity.health - (!entity.isInvincible ? amount / entity.healthMaximum : 0.0f));
    entity.target = null != attacker && entity.isAggressive ? attacker : entity.target;
  }

  protected System.Collections.ObjectModel.ReadOnlyCollection<Entity>? FindTargets                                                                                (Entity.FindTargetsOptions options = Entity.FindTargetsOptions.Any) => this.FindTargets(this.FindTargetsFilter(), this.FindTargetsSorter(), options);
  protected System.Collections.ObjectModel.ReadOnlyCollection<Entity>? FindTargets(System.Predicate<Entity>        filter,                                         Entity.FindTargetsOptions options = Entity.FindTargetsOptions.Any) => this.FindTargets(filter,                   this.FindTargetsSorter(), options);
  protected System.Collections.ObjectModel.ReadOnlyCollection<Entity>? FindTargets(System.Converter<Entity, float> sorter,                                         Entity.FindTargetsOptions options = Entity.FindTargetsOptions.Any) => this.FindTargets(this.FindTargetsFilter(), sorter,                   options);
  protected System.Collections.ObjectModel.ReadOnlyCollection<Entity>? FindTargets(System.Predicate<Entity>        filter, System.Converter<Entity, float> sorter, Entity.FindTargetsOptions options = Entity.FindTargetsOptions.Any) {
    if (0 != Entity.All.Count) {
      System.Collections.Generic.SortedList<float, Entity> entities = new(Entity.All.Count, System.Collections.Generic.Comparer<float>.Default);

      // …
      foreach (Entity entity in Entity.All)
      if (entity != this && entity.enabled && (entity is not Monster monster || !monster.isMounted) && (
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

  protected virtual System.Predicate<Entity>        FindTargetsFilter() => static entity => entity.enabled;
  protected virtual System.Converter<Entity, float> FindTargetsSorter() =>        entity => (entity.rigidBody.position - base.rigidBody.position).sqrMagnitude;

  protected virtual void OnApplicationFocus(bool _) { /* Do nothing… */ }
  protected         void OnApplicationQuit ()       => this.OnApplicationFocus(false);

  protected virtual void OnDestroy() {
    Entity.All.Remove(this);

    if (this.outline.material     is not null) UnityEngine.Object.Destroy(this.outline.material);
    if (this.shoot.bulletMaterial is not null) UnityEngine.Object.Destroy(this.shoot.bulletMaterial);

    if (null != this.following)
    this.following.followers.Remove(this);
  }

  protected virtual void OnTriggerEnter(UnityEngine.Collider collider) {
    if (!this.isDefeated && collider.GetComponent<Player>() is Player player && player.team != this.team)
    this.ContactDamage(player);
  }

  protected virtual void OnTriggerStay(UnityEngine.Collider collider) {
    if (!this.isDefeated && collider.GetComponent<Player>() is Player player && player.team != this.team)
    this.ContactRepel(player);
  }

  protected virtual void PrefollowUpdate() {
    if (Game.IsPaused || this.isDefeated)
    return;

    // …
    this.movement.pauseCooldown.duration = this.movement.pauseCooldown.isLooped ? this.movement.pause + (UnityEngine.Random.value * this.movement.pauseRandomnessFactor * (UnityEngine.Random.value < 0.5 ? +1.0 : -1.0)) : this.movement.pauseCooldown.duration;
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

  public    virtual Bullet? Shoot()                                     => this.Shoot(static (bullet, index) => {}); // ->> Override-able because it is called by default
  protected         Bullet? Shoot(System.Action<Bullet>       callback) => this.Shoot(       (bullet, index) => callback(bullet));
  protected         Bullet? Shoot(System.Action<Bullet, ushort> callback) {
    if (this.shoot.cooldown.isLooped) {
      Bullet? bullet = null;
      ushort  count  = (ushort) (this.shoot.repeatCount + (UnityEngine.Random.value * this.shoot.repeatRandomCount));
      ushort  index  = (ushort) 1u;

      /* … */
      System.Collections.IEnumerator DeployBullet(bool isRepeat) {
        if (null != this.transform) {
          UnityEngine.Vector3 shootDirection = UnityEngine.Vector3.zero == this.turn.direction ? this.transform.forward : this.turn.direction;

          // …
          bullet                = ((UnityEngine.GameObject) UnityEngine.Object.Instantiate(Assets.main.bullet.prefabrication, this.transform.position, UnityEngine.Quaternion.LookRotation(shootDirection, UnityEngine.Vector3.up))).AddComponent<Bullet>();
          bullet.health         = this.shoot.bulletHealth;
          bullet.lifetime       = this.shoot.bulletLifetime;
          bullet.name           = "Bullet " + (this.bullets.Count + 1);
          bullet.shootDirection = shootDirection;
          bullet.user           = this;

          bullet.transform.ForEach<UnityEngine.Renderer>(renderer => renderer.sharedMaterial = this.shoot.bulletMaterial ?? new UnityEngine.Material(renderer.sharedMaterial)); // ->> Invisible bullets are indeed possible
          this.bullets.Add(bullet);

          if (isRepeat)
          callback(bullet, index++);
        }

        if (0u != count--) {
          yield return new UnityEngine.WaitForSecondsRealtime(this.shoot.repeatDelay);
          base.StartCoroutine(DeployBullet(true));
        }
      }

      /* … */
      base.StartCoroutine(DeployBullet(false));
      return bullet;
    }

    return null;
  }

  public virtual void ShootDamage(Entity entity)                                   => this.ShootDamage(entity, UnityEngine.Vector3.zero, this);
  public virtual void ShootDamage(Entity entity, Entity? attacker)                 => this.ShootDamage(entity, UnityEngine.Vector3.zero, attacker);
  public virtual void ShootDamage(Entity entity, in UnityEngine.Vector3 direction) => this.ShootDamage(entity, direction,                this);
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

  protected virtual void Update() {
    UnityEngine.Vector3 rotation       = base.rigidBody.rotation.eulerAngles;
    UnityEngine.Vector3 targetDistance = null != this.target ? this.target.transform.position - this.transform.position : UnityEngine.Vector3.zero;

    // …
    if (Game.IsPaused)
    return;

    // … ->> Defeating
    this.isDefeated = this.isDefeated || (this.health <= 0.0f && !this.isInvincible);

    if (!this.isDefeated) {
      this.defeat.position = this.transform.position;
      this.defeat.timeout.Reset();
    }

    else {
      UnityEngine.Vector3 transformToCameraDirection     = null != this.mainCamera ? (this.mainCamera.transform.position - this.transform.position).normalized : UnityEngine.Vector3.forward;
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

      if (this.defeat.timeout.isElapsed)
      UnityEngine.Object.Destroy(this.gameObject);

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
      this.transform.localScale = this.prefollow.localScale    * 1.000f;
      this.movement.speed       = this.prefollow.movementSpeed * 1.000f;
      this.targetBerth          = this.prefollow.targetBerth   * 1.000f;
      this.target               = this.targetAutomatically && null == this.target ? this.FindTargets(Entity.FindTargetsOptions.DifferentTeam)?[0] ?? null : this.target;
    }

    // … ->> Bouncing — Unfortunately bounces entire `Entity` object, rather than just its visible render
    if (this.bounceAutomatically && UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, Entity.MovementVelocityThreshold).sqrMagnitude < UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, base.rigidBody.linearVelocity).sqrMagnitude) {
      float bounceAngleThreshold = (UnityEngine.Vector3.forward * this.bounce.angle).sqrMagnitude;
      float bounceMagnitude;

      // … ->> Rotation — Apply Z-axis orientation (cumulative)
      base.rigidBody.constraints &= ~UnityEngine.RigidbodyConstraints.FreezeRotationZ;
      base.rigidBody.MoveRotation(UnityEngine.Quaternion.Euler(rotation + (Entity.TurnDirectionToVector3(this.bounce.turn, UnityEngine.Vector3.forward) * UnityEngine.Time.unscaledDeltaTime * this.bounce.angle * this.bounce.speed)));

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
      base.rigidBody.constraints &= ~UnityEngine.RigidbodyConstraints.FreezePositionY;
      base.rigidBody.position     =  UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, base.rigidBody.position) + (UnityEngine.Vector3.up * this.bounce.force * (bounceMagnitude / bounceAngleThreshold));
    }

    else {
      // … ->> Rotation — Remove Z-axis orientation
      base.rigidBody.constraints |= UnityEngine.RigidbodyConstraints.FreezeRotationZ;
      base.rigidBody.MoveRotation(UnityEngine.Quaternion.Slerp(
        UnityEngine.Quaternion.Euler(rotation),
        UnityEngine.Quaternion.Euler(UnityEngine.Vector3.Scale(UnityEngine.Vector3.right + UnityEngine.Vector3.up, rotation)),
        UnityEngine.Time.unscaledDeltaTime * this.bounce.speed
      ));

      // … ->> Position
      base.rigidBody.constraints   |= UnityEngine.RigidbodyConstraints.FreezePositionY;
      base.rigidBody.linearVelocity = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, base.rigidBody.linearVelocity);
      base.rigidBody.position       = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, base.rigidBody.position); // --> UnityEngine.Vector3.Slerp(base.rigidBody.position, UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, base.rigidBody.position), !this.movement.pauseCooldown.isElapsed ? UnityEngine.Time.unscaledDeltaTime * this.bounce.force : 1.0f)
    }

    // … ->> Outlining
    if (null != this.outline.material)
    this.outline.material.color = UnityEngine.Color.LerpUnclamped(this.outline.material.color, null != this.following && null != this.following.outline.material ? this.following.outline.material.color : this.outline.color, 0.1f);

    // … ->> Regenerating
    if (this.regeneration.delay.isElapsed) {
      if (this.regeneration.interval.isLooped)
      this.health = UnityEngine.Mathf.Clamp01(this.health + (this.regeneration.amount / this.healthMaximum));
    } else this.regeneration.interval.Wait();

    // … ->> Moving
    this.movement.direction = Player.IsReady && this.targetAutomatically ? targetDistance.sqrMagnitude > (UnityEngine.Vector3.one * this.targetBerth).sqrMagnitude ? targetDistance.normalized : UnityEngine.Vector3.zero : this.movement.direction;

    if (this.moveAutomatically && UnityEngine.Vector3.zero != this.movement.direction && (this is Player || this.movement.pauseCooldown.isElapsed)) {
      base.rigidBody.AddForce(this.movement.direction * this.movement.speedFactor * ((this.movement.speed * (1.0f - this.movement.speedRandomnessFactor)) + (UnityEngine.Random.value * this.movement.speed * this.movement.speedRandomnessFactor)), UnityEngine.ForceMode.Impulse);
      this.turn.direction = this.movement.direction;
    }

    // … ->> Turning
    this.turn.direction = Player.IsReady && this.targetAutomatically && UnityEngine.Vector3.zero == this.turn.direction ? targetDistance.normalized : this.turn.direction;

    if (UnityEngine.Vector3.zero != this.turn.direction)
    base.rigidBody.MoveRotation(UnityEngine.Quaternion.Slerp(base.rigidBody.rotation, UnityEngine.Quaternion.Euler(
      UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, base.rigidBody.rotation.eulerAngles) +                       // ->> Remove Y-axis orientation
      UnityEngine.Vector3.Scale(UnityEngine.Vector3.up, UnityEngine.Quaternion.LookRotation(this.turn.direction, UnityEngine.Vector3.up).eulerAngles) // ->> Apply  Y-axis orientation
    ), UnityEngine.Time.unscaledDeltaTime * this.turn.speed));

    // … ->> Shooting
    if (this.shootAutomatically && null != this.target) {
      if (this.shoot.isAllowed(this is Player || UnityEngine.Mathf.Cos(UnityEngine.Mathf.Deg2Rad * this.shoot.view) <= UnityEngine.Vector3.Dot(UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, this.transform.forward), UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, targetDistance).normalized))) {
        if (this is Player || (this is Monster monster && monster.kind == Monster.Kind.Molem))
        this.Shoot();

        #if false
        #if DEBUG || DEVELOPMENT_BUILD
          UnityEngine.Debug.DrawRay(this.transform.position, Game.SceneSize * this.transform.forward, UnityEngine.Color.green, 0.0f, false);
        #endif
        #endif
      }

      #if false
      #if DEBUG || DEVELOPMENT_BUILD
        UnityEngine.Debug.DrawRay(this.transform.position, Game.SceneSize * (UnityEngine.Quaternion.AngleAxis(+this.shoot.view, UnityEngine.Vector3.up) * this.transform.forward), UnityEngine.Color.red, 0.0f, false);
        UnityEngine.Debug.DrawRay(this.transform.position, Game.SceneSize * (UnityEngine.Quaternion.AngleAxis(-this.shoot.view, UnityEngine.Vector3.up) * this.transform.forward), UnityEngine.Color.red, 0.0f, false);
      #endif
      #endif
    }

    // … ->> Pre-follow
    this.PrefollowUpdate();
  }
}
  [UnityEngine.DefaultExecutionOrder(4)]
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

    /* … */
    private void Awake() {
      base.rigidBody.collisionDetectionMode = UnityEngine.CollisionDetectionMode.ContinuousDynamic;
      base.rigidBody.constraints            = UnityEngine.RigidbodyConstraints.FreezePositionY | UnityEngine.RigidbodyConstraints.FreezeRotationX | UnityEngine.RigidbodyConstraints.FreezeRotationY;
      base.rigidBody.excludeLayers          = (UnityEngine.LayerMask)  0x0;
      base.rigidBody.includeLayers          = (UnityEngine.LayerMask) ~0x0;
      base.rigidBody.interpolation          = UnityEngine.RigidbodyInterpolation.Interpolate;
      base.rigidBody.isKinematic            = false;
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

      if (null != this.user)
      this.user.bullets.Add(this);
    }

    private void OnDestroy() {
      if (null != this.user)
      this.user.bullets.Remove(this);
    }

    private void OnTriggerEnter(UnityEngine.Collider collider) {
      if (null == this.user)
      return;

      if (collider.GetComponent<Bullet>() is Bullet bullet) {
        if ((bullet.user != this.user && (null == bullet.user || bullet.user.team != this.user.team))) {
          this.health -= (byte) (0u == this.health || this.isInvincible ? 0u : 1u);
          /* Do nothing… */
        }

        return;
      }

      if (collider.GetComponent<Entity>() is Entity entity) {
        if (entity != this.user && !this.user.followers.Contains(entity)) {
          this.health -= (byte) (0u == this.health || this.isInvincible ? 0u : 1u);
          this.user.ShootDamage(entity is Monster monster && monster.isMounted ? monster.following! : entity, this.shootDirection);
        }

        return;
      }

      if (collider.GetComponent<Area>() is not null || collider.GetComponent<Lasoo>() is not null)
      return;

      this.health = (byte) 0u;
    }

    private void Update() {
      if (this.lifetime <= +0.0f || null == this.user) {
        UnityEngine.Object.Destroy(this.gameObject);
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

          UnityEngine.Object.Destroy(this.gameObject);
          return;
        }
      }

      else if (0u == this.health) {
        UnityEngine.GameObject? explosion         = null;
        UnityEngine.Renderer?   explosionRenderer = null;
        UnityEngine.Material[]? materials         = null;

        // …
        do {
          if (this.user.shoot.bulletMaterial is not null) {
            materials = new UnityEngine.Material[] {this.user.shoot.bulletMaterial};
            break;
          }

          this.transform.ForEach<UnityEngine.Renderer>(renderer => materials ??= 0 != renderer.sharedMaterials.Length ? renderer.sharedMaterials : null);
          if (materials is not null) break;

          if (Assets.main.bullet.material is not null) {
            materials = new UnityEngine.Material[] {Assets.main.bullet.material};
            break;
          }
        } while (false);

        if (materials is null) {
          UnityEngine.Object.Destroy(this.gameObject);
          return;
        }

        explosion                           = UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Sphere);
        explosion.name                      = (this.name + " Explosion").TrimStart();
        explosionRenderer                   = explosion.GetComponent<UnityEngine.Renderer>() ?? explosion.AddComponent<UnityEngine.MeshRenderer>();
        explosionRenderer.receiveShadows    = false;
        explosionRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        explosionRenderer.sharedMaterials   = System.Array.ConvertAll(materials, static material => new UnityEngine.Material(material));
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

      /* … */
      if (Game.IsPaused)
      return;

      this.lifetime -= UnityEngine.Time.unscaledDeltaTime;
      base.rigidBody.MoveRotation(base.rigidBody.rotation * UnityEngine.Quaternion.AngleAxis(Entity.BulletSpinSpeed * UnityEngine.Time.unscaledDeltaTime, UnityEngine.Vector3.forward));
    }
  }
