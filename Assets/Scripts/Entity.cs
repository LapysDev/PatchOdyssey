using PatchOdyssey;

/* … */
[UnityEngine.DefaultExecutionOrder(4)]
[UnityEngine.DisallowMultipleComponent]
[UnityEngine.RequireComponent(typeof(UnityEngine.Collider))]
[UnityEngine.RequireComponent(typeof(UnityEngine.Rigidbody))]
public class Bullet : UnityEngine.MonoBehaviour {
  private                 UnityEngine.Collider               _collider  = null!;
  private                 UnityEngine.Rigidbody              _rigidBody = null!;
  public   new            ref readonly UnityEngine.Collider  collider { get { this._collider = null == this._collider ? this.GetComponent<UnityEngine.Collider>() : this._collider; this._collider = null == this._collider ? this.gameObject.AddComponent<UnityEngine.Collider>() : this._collider; return ref this._collider; } }
  private                 UnityEngine.Renderer?              explosion                = null;
  public                  UnityEngine.Vector3                explosionDetonationScale = UnityEngine.Vector3.one;
  public                  byte                               health                   = (byte) 1u;
  public                  Timeframe                          hitTimeout               = new(0.5);
  internal                bool                               isHit                    = false; // ->> Final hit
  public                  bool                               isInvincible             = false;
  public                  float                              lifetime                 = 2.0f;
  public                  ref readonly UnityEngine.Rigidbody rigidBody { get { this._rigidBody = null == this._rigidBody ? this.GetComponent<UnityEngine.Rigidbody>() : this._rigidBody; return ref this._rigidBody; } }
  public                  UnityEngine.Vector3                shootDirection = UnityEngine.Vector3.zero; // ->> Initial line of fire
  public   /* readonly */ double                             shootTimestamp = 0.0;
  public                  Entity                             user           = null!; // ->> Must be set

  /* … */
  private void Awake() {
    this.collider.isTrigger               = true;
    this.explosionDetonationScale         = UnityEngine.Vector3.one * ((UnityEngine.Random.value * 2.0f) + 1.0f);
    this.hitTimeout.easing                = Timeframe.EaseOutSine;
    this.rigidBody.collisionDetectionMode = UnityEngine.CollisionDetectionMode.ContinuousDynamic;
    this.rigidBody.constraints            = UnityEngine.RigidbodyConstraints.FreezePositionY | UnityEngine.RigidbodyConstraints.FreezeRotationX | UnityEngine.RigidbodyConstraints.FreezeRotationY;
    this.rigidBody.excludeLayers          = (UnityEngine.LayerMask) 0x0;
    this.rigidBody.includeLayers          = (UnityEngine.LayerMask) ~0x0;
    this.rigidBody.interpolation          = UnityEngine.RigidbodyInterpolation.Interpolate;
    this.rigidBody.isKinematic            = false;
    this.shootTimestamp                   = Timeframe.CurrentTimestamp;

    switch (this.collider) {
      case UnityEngine.BoxCollider     boxCollider:     boxCollider.size       *= 1.5f;                                 break;
      case UnityEngine.CapsuleCollider capsuleCollider: capsuleCollider.height *= 1.5f; capsuleCollider.radius *= 1.5f; break;
      case UnityEngine.SphereCollider  sphereCollider:  sphereCollider.radius  *= 1.5f;                                 break;
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
      }

      return;
    }

    if (collider.GetComponent<Entity>() is Entity entity) {
      if (entity != this.user && !this.user.followers.Contains(entity)) {
        this.health -= (byte) (0u == this.health || this.isInvincible ? 0u : 1u);
        this.user.ShootDamage(entity, this.shootDirection);
      }

      return;
    }

    if (collider.GetComponent<Area>() is not null || collider.GetComponent<Lasoo>() is not null)
    return;

    this.health = (byte) 0u;
  }

  private void Start() {
    if (null == this.user)
    UnityEngine.Object.Destroy(this.gameObject);
  }

  private void Update() {
    if (null == this.user || this.lifetime <= +0.0f)
      UnityEngine.Object.Destroy(this.gameObject);

    else if (this.isHit) {
      if (this.hitTimeout.isElapsed) {
        if (this.explosion is not null) {
          foreach (UnityEngine.Material material in this.explosion.sharedMaterials)
          UnityEngine.Object.Destroy(material);
        }

        UnityEngine.Object.Destroy(this.gameObject);
      }

      else if (this.explosion is not null) {
        float explosionDetonationProgress = (float) this.hitTimeout.easedProgress;

        // …
        this.explosion.transform.localScale = UnityEngine.Vector3.SlerpUnclamped(UnityEngine.Vector3.one, UnityEngine.Vector3.one + this.explosionDetonationScale, explosionDetonationProgress);

        foreach (UnityEngine.Material material in this.explosion.sharedMaterials)
        material.color = new(material.color.r, material.color.g, material.color.b, material.color.a * (1.0f - explosionDetonationProgress));
      }
    }

    else if (0u == this.health) {
      UnityEngine.Material[]? materials = null;

      // …
      this.rigidBody.useGravity             = false;
      this.rigidBody.linearVelocity         = UnityEngine.Vector3.zero;
      this.rigidBody.interpolation          = UnityEngine.RigidbodyInterpolation.None;
      this.rigidBody.includeLayers          = (UnityEngine.LayerMask) 0x0;
      this.rigidBody.freezeRotation         = true;
      this.rigidBody.excludeLayers          = (UnityEngine.LayerMask) ~0x0;
      this.rigidBody.detectCollisions       = false;
      this.rigidBody.constraints            = UnityEngine.RigidbodyConstraints.FreezeAll;
      this.rigidBody.collisionDetectionMode = UnityEngine.CollisionDetectionMode.Discrete;
      this.rigidBody.automaticInertiaTensor = false;
      this.rigidBody.automaticCenterOfMass  = false;
      this.rigidBody.angularVelocity        = UnityEngine.Vector3.zero;
      this.isHit                            = true;

      this.rigidBody.Sleep();

      if (materials is null && this.user.bulletMaterial is not null)
      materials = new UnityEngine.Material[] {this.user.bulletMaterial};

      if (materials is null)
      this.transform.ForEach(transform => {
        if (transform.GetComponent<UnityEngine.Renderer>() is UnityEngine.Renderer renderer && null != renderer)
        materials ??= renderer.sharedMaterials;
      });

      if (materials is null && Assets.main.bullet.material is not null)
      materials = new UnityEngine.Material[] {Assets.main.bullet.material};

      // …
      if (materials is not null) {
        UnityEngine.GameObject explosion          = UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Sphere);
        UnityEngine.Renderer   explosionRenderer  = explosion.GetComponent<UnityEngine.Renderer>() ?? explosion.AddComponent<UnityEngine.MeshRenderer>();
        UnityEngine.Transform  explosionTransform = explosion.transform;

        // …
        explosionRenderer.receiveShadows    = false;
        explosionRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        explosionRenderer.sharedMaterials   = System.Array.ConvertAll(materials, static material => new UnityEngine.Material(material));
        this.explosion                      = explosionRenderer;

        foreach (UnityEngine.Transform transform in this.transform)
          UnityEngine.Object.Destroy(transform.gameObject);

        explosionTransform.SetParent(this.transform, false);
        explosionTransform.SetLocalPositionAndRotation(UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity);
      } else UnityEngine.Object.Destroy(this.gameObject);
    }

    else
      this.hitTimeout.Reset();

    /* … */
    if (Game.IsPaused)
    return;

    this.lifetime -= UnityEngine.Time.unscaledDeltaTime;
    this.rigidBody.MoveRotation(this.rigidBody.rotation * UnityEngine.Quaternion.AngleAxis(Entity.BulletSpinSpeed * UnityEngine.Time.unscaledDeltaTime, UnityEngine.Vector3.forward));
  }
}

[UnityEngine.DefaultExecutionOrder(1)]
[UnityEngine.DisallowMultipleComponent]
[UnityEngine.RequireComponent(typeof(UnityEngine.Collider))]
[UnityEngine.RequireComponent(typeof(UnityEngine.Rigidbody))]
public abstract class Entity : UnityEngine.MonoBehaviour /* ->> Source file must be named “Entity” */ {
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

  [System.Serializable]
  public struct MovementInfo {
    [ReadWriteInInspector, UnityEngine.Tooltip("For setup only")] public float               damping;
    [ReadOnlyInInspector]                                         public UnityEngine.Vector3 direction;
    [ReadWriteInInspector]                                        public double              restCooldown;
    [ReadWriteInInspector]                                        public double              restCooldownRandomness;
    [ReadOnlyInInspector, System.NonSerialized]                   public Timeframe           restTimer;
    [ReadWriteInInspector]                                        public float               speed;
    [ReadWriteInInspector, UnityEngine.Range(0.25f, 2.00f)]       public float               speedFactor;
    [ReadWriteInInspector, UnityEngine.Range(0.00f, 1.00f)]       public float               speedRandomnessFactor;
  }

  [System.Serializable]
  protected /* readonly */ struct Prefollow {
    [ReadOnlyInInspector] public bool                bounceAutomatically;
    [ReadOnlyInInspector] public bool                isFollowing;
    [ReadOnlyInInspector] public bool                isInvincible;
    [ReadOnlyInInspector] public UnityEngine.Vector3 localScale;
    [ReadOnlyInInspector] public bool                moveAutomatically;
    [ReadOnlyInInspector] public float               movementSpeed;
    [ReadOnlyInInspector] public float               targetBerth;
  }

  [System.Serializable]
  public struct ShootInfo {
    [ReadWriteInInspector]                                  public Timeframe               cooldown;
    [ReadWriteInInspector]                                  public float                   damage;
    [ReadWriteInInspector]                                  public byte                    health;
    [ReadWriteInInspector]                                  public float                   lifetime;
    [ReadWriteInInspector]                                  public System.Func<bool, bool> isAllowed;
    [ReadWriteInInspector]                                  public byte                    repeatCount;
    [ReadWriteInInspector]                                  public float                   repeatDelay;
    [ReadWriteInInspector]                                  public float                   repelForce;
    [ReadWriteInInspector]                                  public float                   speed;
    [ReadWriteInInspector, UnityEngine.Range(0.0f, 360.0f)] public float                   view; // ->> in Degrees
  }

  [System.Serializable]
  public struct TurnInfo {
    [ReadOnlyInInspector]  public UnityEngine.Vector3 direction;
    [ReadWriteInInspector] public float               speed; // ->> Degrees per second
  }

  public enum Team          : byte { Player, Explorer = Player, Enemy, /* ->> Series of `Enemy + …` for other teams */ Nomad = Enemy + 0, Magnate = Enemy + 1 }
  public enum TurnDirection : byte { Anticlockwise, Clockwise }

  /* … */
  public static readonly System.Collections.Generic.List<Entity> All                                = new(32); // --> UnityEngine.Object.FindObjectsByType<Entity>(UnityEngine.FindObjectsInactive.Include, UnityEngine.FindObjectsSortMode.None)
  public const           float                                   BulletSpinSpeed                    = 180.0f;  // ->> Degrees per second
  public const           float                                   DefeatedShrinkFactor               = 0.03f;
  public const           float                                   DefeatedSinkHeight                 = 0.50f;
  public const           float                                   DefeatedSwayDuration               = 0.10f;
  public const           float                                   DefeatedSwayFactor                 = 1.00f;
  public static          UnityEngine.Vector3                     MovementVelocityThreshold { get; } = UnityEngine.Vector3.one * 1.0f;

  [ReadOnlyInInspector]                              private                  UnityEngine.Collider                    _collider           = null!;
  [ReadOnlyInInspector]                              private                  UnityEngine.Rigidbody                   _rigidBody          = null!;
  [ReadOnlyInInspector]                              protected                UnityEngine.Camera?                     actualMainCamera    = null;
  [ReadOnlyInInspector]                              protected                bool                                    actuallyDefeated    = false;
  [ReadWriteInInspector]                             public                   Entity.BounceInfo                       bounce              = new() {angle = 10.00f, force = 0.75f, speed = 5.00f, turn = Entity.TurnDirection.Clockwise};
  [ReadWriteInInspector]                             public                   bool                                    bounceAutomatically = true;
  [ReadWriteInInspector]                             public                   UnityEngine.Material?                   bulletMaterial      = null;
  [ReadOnlyInInspector, UnityEngine.SerializeField]  internal  /* readonly */ System.Collections.Generic.List<Bullet> bullets             = new(8);
  [ReadWriteInInspector]                             public    new            ref readonly UnityEngine.Collider       collider            { get { this._collider = null == this._collider ? this.GetComponent<UnityEngine.Collider>() : this._collider; /* --> base.collider */ return ref this._collider; } }
  [ReadWriteInInspector]                             public                   Entity.ContactInfo                      contact             = new() {damage = 4.00f, repelForce = 2.50f};
  [ReadWriteInInspector]                             public                   Entity.DefeatInfo                       defeat              = new() {position = UnityEngine.Vector3.zero, timeout = new(4.00)};
  [ReadWriteInInspector]                             public                   bool                                    followAutomatically = true;
  [ReadWriteInInspector, UnityEngine.SerializeField] public    /* readonly */ System.Collections.Generic.List<Entity> followers           = new(1);
  [ReadWriteInInspector]                             public                   Entity?                                 following           = null; // ->> Entity to follow (typically ally)
  [ReadWriteInInspector]                             public                   float                                   health              = 100.00f;
  [ReadWriteInInspector]                             public                   bool                                    isDefeated          { get => this.actuallyDefeated; set => this.actuallyDefeated = this.actuallyDefeated || value; } // ->> Cannot be revived
  [ReadWriteInInspector]                             public                   bool                                    isInvincible        = false;
  [ReadOnlyInInspector]                              protected                ref readonly UnityEngine.Camera?        mainCamera          { get { this.actualMainCamera = null == this.actualMainCamera ? UnityEngine.Camera.main : this.actualMainCamera; /* --> base.camera */ return ref this.actualMainCamera; } }
  [ReadWriteInInspector]                             public                   bool                                    moveAutomatically   = true; // ->> Reach for `target` entity
  [ReadWriteInInspector]                             public                   Entity.MovementInfo                     movement            = new() {damping = 1.00f, direction = UnityEngine.Vector3.zero, restTimer = new(0.00), restCooldown = 0.00, restCooldownRandomness = 0.75, speed = 2.00f, speedFactor = 1.00f, speedRandomnessFactor = 0.30f};
  [ReadWriteInInspector, UnityEngine.SerializeField] protected                Entity.Prefollow                        prefollow           = new() {bounceAutomatically = true, isFollowing = false, isInvincible = false, localScale = UnityEngine.Vector3.one, moveAutomatically = true, movementSpeed = 2.00f, targetBerth = 2.25f};
  [ReadOnlyInInspector]                              protected                bool                                    prefollowIsUpdated  = false;
  [ReadWriteInInspector]                             public                   ref readonly UnityEngine.Rigidbody      rigidBody           { get { this._rigidBody = null == this._rigidBody ? this.GetComponent<UnityEngine.Rigidbody>() : this._rigidBody; return ref this._rigidBody; } }
  [ReadWriteInInspector]                             public                   Entity.ShootInfo                        shoot               = new() {cooldown = new(1.00), damage = 6.75f, health = (byte) 1u, isAllowed = static allowed => allowed, lifetime = 2.0f, repeatCount = (byte) 0u, repeatDelay = 0.0f, repelForce = 1.0f, speed = 3.50f, view = 21.0f};
  [ReadWriteInInspector]                             public                   bool                                    shootAutomatically  = true;
  [ReadWriteInInspector]                             public                   Entity?                                 target              = null; // ->> Entity to reach (typically enemy)
  [ReadWriteInInspector]                             public                   bool                                    targetAutomatically = true;
  [ReadWriteInInspector]                             public                   float                                   targetBerth         = 2.25f; // ->> Radius
  [ReadWriteInInspector]                             public                   Entity.Team                             team                = Entity.Team.Enemy;
  [ReadWriteInInspector]                             public                   Entity.TurnInfo                         turn                = new() {direction = UnityEngine.Vector3.zero, speed = 9.00f};

  /* … */
  protected virtual void Awake() {
    this.rigidBody.useGravity             = false;
    this.rigidBody.mass                   = 1.0f;
    this.rigidBody.linearDamping          = this.movement.damping;
    this.rigidBody.isKinematic            = false;
    this.rigidBody.interpolation          = UnityEngine.RigidbodyInterpolation.Extrapolate;
    this.rigidBody.inertiaTensorRotation  = UnityEngine.Quaternion.identity;
    this.rigidBody.inertiaTensor          = UnityEngine.Vector3.one;
    this.rigidBody.includeLayers          = (UnityEngine.LayerMask) ~0x0;
    this.rigidBody.freezeRotation         = true;
    this.rigidBody.excludeLayers          = (UnityEngine.LayerMask) 0x0;
    this.rigidBody.detectCollisions       = true;
    this.rigidBody.constraints            = UnityEngine.RigidbodyConstraints.FreezePositionY | UnityEngine.RigidbodyConstraints.FreezeRotation;
    this.rigidBody.collisionDetectionMode = UnityEngine.CollisionDetectionMode.Discrete;
    this.rigidBody.automaticInertiaTensor = false;
    this.rigidBody.automaticCenterOfMass  = true;
    this.prefollow.targetBerth            = this.targetBerth;
    this.prefollow.movementSpeed          = this.movement.speed;
    this.prefollow.moveAutomatically      = this.moveAutomatically;
    this.prefollow.localScale             = this.transform.localScale;
    this.prefollow.isInvincible           = this.isInvincible;
    this.prefollow.bounceAutomatically    = this.bounceAutomatically;
    this.movement.restTimer.duration      = this.movement.restCooldown;
    this.defeat.timeout.easing            = Timeframe.EaseInOutQuintic;
    this.collider.isTrigger               = true;
    this.collider.includeLayers           = (UnityEngine.LayerMask) ~0x0;
    this.collider.hasModifiableContacts   = false;
    this.collider.excludeLayers           = (UnityEngine.LayerMask) 0x0;
    this.bulletMaterial                 ??= Assets.main.bullet.material;
    this.bulletMaterial                   = null != this.bulletMaterial ? new UnityEngine.Material(this.bulletMaterial) : null;

    Entity.All.Add(this);
    this.movement.restTimer.Reset();
    this.movement.restTimer.Wait (UnityEngine.Random.value * (this.movement.restCooldown + this.movement.restCooldownRandomness));

    if (this.bulletMaterial is not null)
    this.bulletMaterial.name = "bullet";

    if (Assets.main.outlineAutomatically && Assets.main.outlineMaterial is not null)
    this.transform.ForEach(static transform => {
      if (transform.GetComponent<UnityEngine.Renderer>() is UnityEngine.Renderer renderer && null != renderer) {
        UnityEngine.Material[] materials    = renderer.sharedMaterials;
        UnityEngine.Material[] submaterials = new UnityEngine.Material[materials.Length + 1];

        // …
        materials.CopyTo(submaterials, 0);

        submaterials[materials.Length] = Assets.main.outlineMaterial;
        renderer.receiveShadows        = false;
        renderer.shadowCastingMode     = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.sharedMaterials       = submaterials;
      }
    });

    foreach (Entity follower in this.followers)
    follower.team = this.team;
  }

  public virtual void ContactDamage(Entity entity)                   => this.Damage(entity, this.contact.damage, this.GetDamageDefaultAttacker());
  public virtual void ContactDamage(Entity entity, Entity? attacker) => this.Damage(entity, this.contact.damage, attacker);

  public    void ContactRepel(Entity entity) => this.ContactRepel(entity, (entity.collider.bounds.center - this.collider.bounds.center).normalized);
  protected void ContactRepel(Entity entity, in UnityEngine.Vector3 direction) {
    entity.rigidBody.AddForce(direction * this.contact.repelForce, UnityEngine.ForceMode.Impulse);

    #if DEBUG || DEVELOPMENT_BUILD
      UnityEngine.Debug.DrawRay(entity.collider.bounds.center, direction * Game.SceneSize,          UnityEngine.Color.red,   0.0f, false);
      UnityEngine.Debug.DrawRay(entity.collider.bounds.center, direction * this.contact.repelForce, UnityEngine.Color.green, 0.0f, false);
    #endif
  }

  public virtual void Damage(Entity entity, float amount) => this.Damage(entity, amount, this.GetDamageDefaultAttacker());
  public virtual void Damage(Entity entity, float amount, Entity? attacker) {
    amount        = entity.isInvincible ? 0u : amount;
    entity.health = entity.health > amount ? entity.health - amount : 0.0f;
    entity.target = entity is Monster || entity is Tamer ? attacker ?? entity.target : entity.target;
  }

  protected System.Collections.ObjectModel.ReadOnlyCollection<Entity>? FindTargets   ()                                                                                                              => this.FindTargets(this.GetFindTargetsDefaultPredicate(), this.GetFindTargetsDefaultComparison());
  protected System.Collections.ObjectModel.ReadOnlyCollection<Entity>? FindTargets   (System.Predicate<Entity>    predicate)                                                                         => this.FindTargets(predicate,                             this.GetFindTargetsDefaultComparison());
  protected System.Collections.ObjectModel.ReadOnlyCollection<Entity>? FindTargets<T>(System.Converter<Entity, T> comparison)                                        where T : System.IComparable<T> => this.FindTargets(this.GetFindTargetsDefaultPredicate(), comparison);
  protected System.Collections.ObjectModel.ReadOnlyCollection<Entity>? FindTargets<T>(System.Predicate<Entity>    predicate, System.Converter<Entity, T> comparison) where T : System.IComparable<T> {
    if (0 != Entity.All.Count) {
      System.Collections.Generic.SortedList<T, Entity> entities = new(Entity.All.Count, System.Collections.Generic.Comparer<T>.Default);

      // …
      foreach (Entity entity in Entity.All) {
        T entityComparison = comparison(entity);

        // …
        if (entity != this && (entity is not Monster monster || !monster.isMounted) && !entities.ContainsKey(entityComparison) && predicate(entity))
        entities.Add(entityComparison, entity);
      }

      entities.TrimExcess(); // --> entities.Capacity = entities.Count;
      return new System.Collections.ObjectModel.ReadOnlyCollection<Entity>(entities.Values);
    }

    return null; // --> System.Collections.ObjectModel.ReadOnlyCollection<Entity>.Empty;
  }
    protected System.Collections.ObjectModel.ReadOnlyCollection<Entity>? FindNonTeamTargets   ()                                                                                                              => this.FindTargets(this.GetFindNonTeamTargetsDefaultPredicate(), this.GetFindNonTeamTargetsDefaultComparison());
    protected System.Collections.ObjectModel.ReadOnlyCollection<Entity>? FindNonTeamTargets   (System.Predicate<Entity>    predicate)                                                                         => this.FindTargets(predicate,                                    this.GetFindNonTeamTargetsDefaultComparison());
    protected System.Collections.ObjectModel.ReadOnlyCollection<Entity>? FindNonTeamTargets<T>(System.Converter<Entity, T> comparison)                                        where T : System.IComparable<T> => this.FindTargets(this.GetFindNonTeamTargetsDefaultPredicate(), comparison);
    protected System.Collections.ObjectModel.ReadOnlyCollection<Entity>? FindNonTeamTargets<T>(System.Predicate<Entity>    predicate, System.Converter<Entity, T> comparison) where T : System.IComparable<T> => this.FindTargets(predicate,                                    comparison);

    protected System.Collections.ObjectModel.ReadOnlyCollection<Entity>? FindTeamTargets   ()                                                                                                              => this.FindTargets(this.GetFindTeamTargetsDefaultPredicate(), this.GetFindTeamTargetsDefaultComparison());
    protected System.Collections.ObjectModel.ReadOnlyCollection<Entity>? FindTeamTargets   (System.Predicate<Entity>    predicate)                                                                         => this.FindTargets(predicate,                                 this.GetFindTeamTargetsDefaultComparison());
    protected System.Collections.ObjectModel.ReadOnlyCollection<Entity>? FindTeamTargets<T>(System.Converter<Entity, T> comparison)                                        where T : System.IComparable<T> => this.FindTargets(this.GetFindTeamTargetsDefaultPredicate(), comparison);
    protected System.Collections.ObjectModel.ReadOnlyCollection<Entity>? FindTeamTargets<T>(System.Predicate<Entity>    predicate, System.Converter<Entity, T> comparison) where T : System.IComparable<T> => this.FindTargets(predicate,                                 comparison);

  private Entity GetDamageDefaultAttacker() => this;

  private System.Predicate<Entity> GetFindTargetsDefaultPredicate() => static entity => entity.enabled;
    private System.Predicate<Entity> GetFindNonTeamTargetsDefaultPredicate() { Entity.Team team = this.team; return team switch { Entity.Team.Explorer => static entity => entity.enabled && Entity.Team.Explorer != entity.team, Entity.Team.Nomad => static entity => entity.enabled && Entity.Team.Nomad != entity.team, Entity.Team.Magnate => static entity => entity.enabled && Entity.Team.Magnate != entity.team, _ => entity => entity.enabled && entity.team != team }; }
    private System.Predicate<Entity> GetFindTeamTargetsDefaultPredicate   () { Entity.Team team = this.team; return team switch { Entity.Team.Explorer => static entity => entity.enabled && Entity.Team.Explorer == entity.team, Entity.Team.Nomad => static entity => entity.enabled && Entity.Team.Nomad == entity.team, Entity.Team.Magnate => static entity => entity.enabled && Entity.Team.Magnate == entity.team, _ => entity => entity.enabled && entity.team == team }; }

  protected virtual System.Converter<Entity, float> GetFindTargetsDefaultComparison() { UnityEngine.Vector3 position = this.transform.position; return entity => (entity.transform.position - position).sqrMagnitude; }
    protected virtual System.Converter<Entity, float> GetFindNonTeamTargetsDefaultComparison() => this.GetFindTargetsDefaultComparison();
    protected virtual System.Converter<Entity, float> GetFindTeamTargetsDefaultComparison   () => this.GetFindTargetsDefaultComparison();

  private void LateUpdate() {
    if (Game.IsPaused || this.isDefeated)
    return;

    // …
    this.prefollowIsUpdated = false;

    if (this.movement.restTimer.isLooped)
    this.movement.restTimer.duration = this.movement.restCooldown + (UnityEngine.Random.value * this.movement.restCooldownRandomness * (UnityEngine.Random.value < 0.5 ? +1.0 : -1.0));

    if (
      null == this.following ||   // ->> Track live changes
      !this.prefollow.isFollowing // ->> Store previous values before following
    ) {
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

  protected virtual void OnApplicationFocus(bool _) { /* Do nothing… */ }
  protected         void OnApplicationQuit ()       => this.OnApplicationFocus(false);

  protected virtual void OnDestroy() {
    Entity.All.Remove(this);

    if (this.bulletMaterial is not null)
    UnityEngine.Object.Destroy(this.bulletMaterial);

    if (null != this.following)
    this.following.followers.Remove(this);
  }

  protected virtual void OnTriggerEnter(UnityEngine.Collider collider) {
    if (this.isDefeated)
    return;

    // …
    if (collider.GetComponent<Player>() is Player player && player.team != this.team)
    this.ContactDamage(player);
  }

  protected virtual void OnTriggerStay(UnityEngine.Collider collider) {
    if (this.isDefeated)
    return;

    // …
    if (collider.GetComponent<Player>() is Player player && player.team != this.team)
    this.ContactRepel(player);
  }

  public virtual Bullet? Shoot() => this.Shoot(static bullet => {});

  protected Bullet? Shoot(System.Action<Bullet> callback) {
    if (this.shoot.cooldown.isLooped) {
      Bullet?               bullet    = null;
      byte                  count     = (byte) (0u != this.shoot.repeatCount ? this.shoot.repeatCount - 1u : 0u);
      UnityEngine.Transform transform = this.transform;

      /* … */
      System.Collections.IEnumerator DeployBullet(bool isRepeat) {
        if (null != transform) {
          UnityEngine.Vector3 shootDirection = UnityEngine.Vector3.zero == this.turn.direction ? transform.forward : this.turn.direction;

          // …
          bullet                = ((UnityEngine.GameObject) UnityEngine.Object.Instantiate(Assets.main.bullet.prefabrication, transform.position, UnityEngine.Quaternion.LookRotation(shootDirection, UnityEngine.Vector3.up))).AddComponent<Bullet>();
          bullet.health         = this.shoot.health;
          bullet.lifetime       = this.shoot.lifetime;
          bullet.name           = "Bullet";
          bullet.shootDirection = shootDirection;
          bullet.user           = this;

          this.bullets.Add(bullet);
          bullet.transform.ForEach(transform => {
            if (transform.GetComponent<UnityEngine.Renderer>() is UnityEngine.Renderer renderer && null != renderer)
            renderer.sharedMaterial = this.bulletMaterial ?? new UnityEngine.Material(renderer.sharedMaterial);
          });

          if (isRepeat)
          callback(bullet);
        }

        if (0u != count) {
          --count;

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

  public virtual void ShootDamage(Entity entity)                                   => this.ShootDamage(entity, UnityEngine.Vector3.zero, this.GetDamageDefaultAttacker());
  public virtual void ShootDamage(Entity entity, Entity? attacker)                 => this.ShootDamage(entity, UnityEngine.Vector3.zero, attacker);
  public virtual void ShootDamage(Entity entity, in UnityEngine.Vector3 direction) => this.ShootDamage(entity, direction,                this.GetDamageDefaultAttacker());
  public virtual void ShootDamage(Entity entity, in UnityEngine.Vector3 direction, Entity? attacker) {
    if (UnityEngine.Vector3.zero != direction)
      this.ShootRepel(entity, direction);

    this.Damage(entity, this.shoot.damage);
  }

  private void ShootRepel(Entity entity, in UnityEngine.Vector3 direction) {
    float contactRepelForce = this.contact.repelForce;

    // … ->> Badly designed?
    this.contact.repelForce = this.shoot.repelForce;
    this.ContactRepel(entity, direction);
    this.contact.repelForce = contactRepelForce;
  }

  public static UnityEngine.Vector3 TurnDirectionToVector3(Entity.TurnDirection turn, in UnityEngine.Vector3 direction) => turn switch {
    Entity.TurnDirection.Anticlockwise => -direction,
    Entity.TurnDirection.Clockwise     =>  direction,
    _                                  =>  UnityEngine.Vector3.zero
  };

  protected virtual void Update() {
    UnityEngine.Transform transform      = this.transform;
    UnityEngine.Vector3   targetDistance = null != this.target ? this.target.transform.position - transform.position : UnityEngine.Vector3.zero;
    UnityEngine.Vector3   rotation       = this.rigidBody.rotation.eulerAngles;

    // …
    if (Game.IsPaused)
    return;

    // … ->> Defeating
    this.isDefeated = 0.0f == this.health && !this.isInvincible;

    if (!this.isDefeated) {
      this.defeat.position = transform.position;
      this.defeat.timeout.Reset();
    }

    else {
      UnityEngine.Vector3 transformToCameraDirection     = null != this.mainCamera ? (this.mainCamera.transform.position - transform.position).normalized : UnityEngine.Vector3.forward;
      float               transformForwardToCameraFactor = UnityEngine.Mathf.Abs(UnityEngine.Vector3.Dot(transformToCameraDirection, transform.forward));
      float               transformRightToCameraFactor   = UnityEngine.Mathf.Abs(UnityEngine.Vector3.Dot(transformToCameraDirection, transform.right));
      float               transformToCameraFactors       = transformForwardToCameraFactor + transformRightToCameraFactor;
      float               swayForce                      = (float) this.defeat.timeout.elapsed / Entity.DefeatedSwayDuration;
      UnityEngine.Vector3 swayDirection                  = Entity.TurnDirectionToVector3(0 == (((uint) swayForce) & 1) ? Entity.TurnDirection.Anticlockwise : Entity.TurnDirection.Clockwise, UnityEngine.Vector3.right);

      // …
      this.rigidBody.interpolation          = UnityEngine.RigidbodyInterpolation.None;
      this.rigidBody.includeLayers          = (UnityEngine.LayerMask) 0x0;
      this.rigidBody.freezeRotation         = true;
      this.rigidBody.excludeLayers          = (UnityEngine.LayerMask) ~0x0;
      this.rigidBody.detectCollisions       = false;
      this.rigidBody.constraints            = UnityEngine.RigidbodyConstraints.FreezeAll;
      this.rigidBody.automaticInertiaTensor = false;
      this.rigidBody.automaticCenterOfMass  = false;
      transform.position                    = this.defeat.position + (swayDirection * ((swayForce - (uint) swayForce) - 0.5f) * Entity.DefeatedSwayFactor) + (UnityEngine.Vector3.up * -Entity.DefeatedSinkHeight * (float) this.defeat.timeout.easedProgress);
      transform.localScale                  = UnityEngine.Vector3.Scale(transform.localScale,
        (UnityEngine.Vector3.forward * (1.0f - (Entity.DefeatedShrinkFactor * (transformRightToCameraFactor   / transformToCameraFactors)))) +
        (UnityEngine.Vector3.right   * (1.0f - (Entity.DefeatedShrinkFactor * (transformForwardToCameraFactor / transformToCameraFactors)))) +
        (UnityEngine.Vector3.up      * (1.0f - 0.0f))
      );

      this.rigidBody.Sleep();
      transform.SetParent(null, true);

      if (this.defeat.timeout.isElapsed)
      UnityEngine.Object.Destroy(this.gameObject);

      return;
    }

    // … ->> Following/ Targeting
    if (this.followAutomatically && null != this.following) {
      this.movement.restTimer.Finish();

      transform.localScale = this.prefollow.localScale    * 0.875f;
      this.movement.speed  = this.prefollow.movementSpeed * 1.365f;
      this.targetBerth     = this.prefollow.targetBerth   * 1.150f;
      this.target          = null != this.following.target && this.following.target != this ? this.following.target : this.following;
    }

    else {
      transform.localScale = this.prefollow.localScale    * 1.000f;
      this.movement.speed  = this.prefollow.movementSpeed * 1.000f;
      this.targetBerth     = this.prefollow.targetBerth   * 1.000f;
      this.target          = this.targetAutomatically && null == this.target ? this.FindNonTeamTargets()?[0] ?? null : this.target;
    }

    // … ->> Bouncing
    if (this.bounceAutomatically && UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, Entity.MovementVelocityThreshold).sqrMagnitude < UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, this.rigidBody.linearVelocity).sqrMagnitude) {
      float bounceAngleMagnitudeSquared = (UnityEngine.Vector3.forward * this.bounce.angle).sqrMagnitude;
      float bounceMagnitude;

      // … ->> Rotation
      this.rigidBody.constraints &= ~UnityEngine.RigidbodyConstraints.FreezeRotationZ;
      this.rigidBody.MoveRotation(UnityEngine.Quaternion.Euler(
        rotation + // ->> Apply Z-axis orientation (cumulative)
        (Entity.TurnDirectionToVector3(this.bounce.turn, UnityEngine.Vector3.forward) * UnityEngine.Time.unscaledDeltaTime * this.bounce.angle * this.bounce.speed)
      ));

      rotation        = this.rigidBody.rotation.eulerAngles;
      rotation        = new(rotation.x > 180.0f ? rotation.x - 360.0f : rotation.x, rotation.y > 180.0f ? rotation.y - 360.0f : rotation.y, rotation.z > 180.0f ? rotation.z - 360.0f : rotation.z);
      bounceMagnitude = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, rotation).sqrMagnitude;

      if (bounceMagnitude >= bounceAngleMagnitudeSquared) {
        // … ->> Checked Z-axis orientation for `bounce.angle` threshold
        this.rigidBody.MoveRotation(UnityEngine.Quaternion.Euler(
          UnityEngine.Vector3.Scale(UnityEngine.Vector3.right + UnityEngine.Vector3.up, rotation) +                                 // ->> Remove Z-axis orientation
          (Entity.TurnDirectionToVector3(this.bounce.turn, UnityEngine.Vector3.forward) * (this.bounce.angle - Game.VectorEpsilon)) // ->> Apply  Z-axis orientation
        ));

        this.bounce.turn = this.bounce.turn switch {
          Entity.TurnDirection.Anticlockwise => Entity.TurnDirection.Clockwise,
          Entity.TurnDirection.Clockwise     => Entity.TurnDirection.Anticlockwise,
          _                                  => this.bounce.turn
        };
      }

      // … ->> Position
      #if true
        this.rigidBody.constraints &= ~UnityEngine.RigidbodyConstraints.FreezePositionY;
        this.rigidBody.position     = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, this.rigidBody.position) + (UnityEngine.Vector3.up * this.bounce.force * (bounceMagnitude / bounceAngleMagnitudeSquared));
      #else
        this.rigidBody.constraints &= ~UnityEngine.RigidbodyConstraints.FreezePositionY;
        this.rigidBody.useGravity   = true;

        if (this.rigidBody.position.y <= +0.0f) {
          this.rigidBody.position = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, this.rigidBody.position);
          this.rigidBody.AddForce(UnityEngine.Vector3.up * this.bounce.force, UnityEngine.ForceMode.Impulse);
        }
      #endif
    }

    else {
      // … ->> Rotation
      this.rigidBody.constraints |= UnityEngine.RigidbodyConstraints.FreezeRotationZ;
      this.rigidBody.MoveRotation(UnityEngine.Quaternion.Slerp(
        // ->> Remove Z-axis orientation
        UnityEngine.Quaternion.Euler(rotation),
        UnityEngine.Quaternion.Euler(UnityEngine.Vector3.Scale(UnityEngine.Vector3.right + UnityEngine.Vector3.up, rotation)),
        UnityEngine.Time.unscaledDeltaTime * this.bounce.speed
      ));

      // … ->> Position
      this.rigidBody.linearVelocity = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, this.rigidBody.linearVelocity);

      #if true
        this.rigidBody.constraints |= UnityEngine.RigidbodyConstraints.FreezePositionY;
        this.rigidBody.position     = UnityEngine.Vector3.Slerp(this.rigidBody.position, UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, this.rigidBody.position), !this.movement.restTimer.isElapsed ? UnityEngine.Time.unscaledDeltaTime * this.bounce.force : 1.0f);
      #else
        if (this.rigidBody.position.y <= +0.0f) {
          this.rigidBody.constraints |= UnityEngine.RigidbodyConstraints.FreezePositionY;
          this.rigidBody.position     = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, this.rigidBody.position);
          this.rigidBody.useGravity   = false;
        }
      #endif
    }

    // … ->> Moving
    if (Player.AnyInput && this.targetAutomatically)
    this.movement.direction = targetDistance.sqrMagnitude > (UnityEngine.Vector3.one * this.targetBerth).sqrMagnitude ? targetDistance.normalized : UnityEngine.Vector3.zero;

    if (this.moveAutomatically && UnityEngine.Vector3.zero != this.movement.direction && (this is Player || this.movement.restTimer.isElapsed)) {
      this.rigidBody.AddForce(this.movement.direction * this.movement.speedFactor * ((this.movement.speed * (1.0f - this.movement.speedRandomnessFactor)) + (UnityEngine.Random.value * this.movement.speed * this.movement.speedRandomnessFactor)), UnityEngine.ForceMode.Impulse);
      this.turn.direction = this.movement.direction;
    }

    // … ->> Turning
    if (Player.AnyInput && this.targetAutomatically && UnityEngine.Vector3.zero == this.turn.direction)
    this.turn.direction = targetDistance.normalized;

    if (UnityEngine.Vector3.zero != this.turn.direction)
    this.rigidBody.MoveRotation(UnityEngine.Quaternion.Slerp(
      this.rigidBody.rotation,
      UnityEngine.Quaternion.Euler(
        UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, this.rigidBody.rotation.eulerAngles) +                       // ->> Remove Y-axis orientation
        UnityEngine.Vector3.Scale(UnityEngine.Vector3.up, UnityEngine.Quaternion.LookRotation(this.turn.direction, UnityEngine.Vector3.up).eulerAngles) // ->> Apply  Y-axis orientation
      ),
      UnityEngine.Time.unscaledDeltaTime * this.turn.speed
    ));

    // … ->> Shooting
    if (this.shootAutomatically && null != this.target) {
      if (this.shoot.isAllowed(this is Player || UnityEngine.Mathf.Cos(UnityEngine.Mathf.Deg2Rad * this.shoot.view) <= UnityEngine.Vector3.Dot(UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, this.transform.forward), UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, targetDistance).normalized))) {
        if (this is Player || (this is Monster monster && monster.kind == Monster.Kind.Antillery))
        this.Shoot();

        #if false
          #if DEBUG || DEVELOPMENT_BUILD
            UnityEngine.Debug.DrawRay(transform.position, Game.SceneSize * transform.forward, UnityEngine.Color.green, 0.0f, false);
          #endif
        #endif
      }

      #if false
        #if DEBUG || DEVELOPMENT_BUILD
          UnityEngine.Debug.DrawRay(transform.position, Game.SceneSize * (UnityEngine.Quaternion.AngleAxis(+this.shoot.view, UnityEngine.Vector3.up) * transform.forward), UnityEngine.Color.red, 0.0f, false);
          UnityEngine.Debug.DrawRay(transform.position, Game.SceneSize * (UnityEngine.Quaternion.AngleAxis(-this.shoot.view, UnityEngine.Vector3.up) * transform.forward), UnityEngine.Color.red, 0.0f, false);
        #endif
      #endif
    }
  }
}
