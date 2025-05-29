using PatchOdyssey;

/* … */
[UnityEngine.DisallowMultipleComponent]
public class PatchEntity : UnityEngine.MonoBehaviour {
  public enum Team : byte { Peaceful, HostileMonsters, HostilePlayers, HostileTamers, Violent }

  /* … */
  public  static readonly List<PatchEntity>                                                                            Any                           = new(13u);
  private static readonly System.Collections.Generic.Dictionary<(System.Type, string), System.Reflection.MethodInfo[]> EventListeners                = new(12);
  private static          AnimationSequence                                                                            FleeAnimationDefault          = new("flee", 2.0, new() {{"sway", 0.0}}, new() {{"sway", 1.0}}) {
    {Util.Perc(0.00), new() {{"sway", +1.0}}},
    {Util.Perc(0.02), new() {{"sway", -1.0}}},
    {Util.Perc(0.04), new() {{"sway", +1.0}}},
    {Util.Perc(0.06), new() {{"sway", -1.0}}},
    {Util.Perc(0.08), new() {{"sway", +1.0}}},
    {Util.Perc(0.1), new() {{"sway", -1.0}}},
    {Util.Perc(0.12), new() {{"sway", +1.0}}},
    {Util.Perc(0.14), new() {{"sway", -1.0}}},
    {Util.Perc(0.16), new() {{"sway", +1.0}}},
    {Util.Perc(0.18), new() {{"sway", -1.0}}},
    {Util.Perc(0.2), new() {{"sway", +1.0}}},
    {Util.Perc(0.22), new() {{"sway", -1.0}}},
    {Util.Perc(0.24), new() {{"sway", +1.0}}},
    {Util.Perc(0.26), new() {{"sway", -1.0}}},
    {Util.Perc(0.28), new() {{"sway", +1.0}}},
    {Util.Perc(0.3), new() {{"sway", -1.0}}},
    {Util.Perc(0.32), new() {{"sway", +1.0}}},
    {Util.Perc(0.34), new() {{"sway", -1.0}}},
    {Util.Perc(0.36), new() {{"sway", +1.0}}},
    {Util.Perc(0.38), new() {{"sway", -1.0}}},
    {Util.Perc(0.4), new() {{"sway", +1.0}}},
    {Util.Perc(0.42), new() {{"sway", -1.0}}},
    {Util.Perc(0.44), new() {{"sway", +1.0}}},
    {Util.Perc(0.46), new() {{"sway", -1.0}}},
    {Util.Perc(0.48), new() {{"sway", +1.0}}},
    {Util.Perc(0.5), new() {{"sway", -1.0}}},
    {Util.Perc(0.52), new() {{"sway", +1.0}}},
    {Util.Perc(0.54), new() {{"sway", -1.0}}},
    {Util.Perc(0.56), new() {{"sway", +1.0}}},
    {Util.Perc(0.58), new() {{"sway", -1.0}}},
    {Util.Perc(0.6), new() {{"sway", +1.0}}},
    {Util.Perc(0.62), new() {{"sway", -1.0}}},
    {Util.Perc(0.64), new() {{"sway", +1.0}}},
    {Util.Perc(0.66), new() {{"sway", -1.0}}},
    {Util.Perc(0.68), new() {{"sway", +1.0}}},
    {Util.Perc(0.7), new() {{"sway", -1.0}}},
    {Util.Perc(0.72), new() {{"sway", +1.0}}},
    {Util.Perc(0.74), new() {{"sway", -1.0}}},
    {Util.Perc(0.76), new() {{"sway", +1.0}}},
    {Util.Perc(0.78), new() {{"sway", -1.0}}},
    {Util.Perc(0.8), new() {{"sway", +1.0}}},
    {Util.Perc(0.82), new() {{"sway", -1.0}}},
    {Util.Perc(0.84), new() {{"sway", +1.0}}},
    {Util.Perc(0.86), new() {{"sway", -1.0}}},
    {Util.Perc(0.88), new() {{"sway", +1.0}}},
    {Util.Perc(0.9), new() {{"sway", -1.0}}},
    {Util.Perc(0.92), new() {{"sway", +1.0}}},
    {Util.Perc(0.94), new() {{"sway", -1.0}}},
    {Util.Perc(0.96), new() {{"sway", +1.0}}},
    {Util.Perc(0.98), new() {{"sway", -1.0}}}
  };
  public  const           float                                                                                        FleeAnimationIntensity        = 0.05f;
  public  const           float                                                                                        HealthMaximumDefault          = 100.0f;
  public  const           float                                                                                        MoveSpeedDefault              = 1.0f;
  public  const           double                                                                                       ProjectileCooldownDefault     = 2.0; // ⟶ in Seconds
  public  const           float                                                                                        ProjectileDamageDefault       = 7.5f;
  public  const           float                                                                                        ProjectileFiringOffsetDefault = 1.0f;
  public  const           double                                                                                       ProjectileLifetimeDefault     = 60.0; // ⟶ in Seconds
  public  const           float                                                                                        ProjectileSpeedDefault        = 15.0f;
  public  const           UnityEngine.RigidbodyConstraints                                                             RigidBodyConstraintsDefault   = UnityEngine.RigidbodyConstraints.FreezeAll; // ⟶ 🖕 you, `UnityEngine.Rigidbody`
  public  const           float                                                                                        TurnSpeedDefault              = 7.5f;                                       // ⟶ in Degrees
  public  const           double                                                                                       TurnStopDurationDefault       = 0.3;                                        // ⟶ in Seconds

  [ReadOnlyInInspector]                              public             bool                                         canFire                 => this.firingCooldown.isFinished;
  [ReadOnlyInInspector, System.NonSerialized]        public             AnimationSequence                            firingCooldown          =  new(PatchEntity.ProjectileCooldownDefault, AnimationSequence.Idle, AnimationSequence.Idle);
  [ReadWriteInInspector]                             private            AnimationSequence                            fleeAnimation           =  PatchEntity.FleeAnimationDefault;
  [ReadOnlyInInspector]                              public             float                                        fleeAnimationIntensity  =  PatchEntity.FleeAnimationIntensity;
  [ReadWriteInInspector]                             private            UnityEngine.Vector3                          fleeAnimationPosition   =  UnityEngine.Vector3.zero;
  [ReadOnlyInInspector, UnityEngine.SerializeField]  internal           bool                                         fleeing                 =  false; // ⟶ Editor-only
  [ReadWriteInInspector]                             public             float                                        health        { get => (float) this.healthNormalized;        protected set => this.healthNormalized        = !this.isImmortal ? (uint) value : this.healthNormalized; }        // ⟶ Pretty confident `value` is never negative
  [ReadWriteInInspector]                             public             float                                        healthMaximum { get => (float) this.healthMaximumNormalized; protected set => this.healthMaximumNormalized = !this.isImmortal ? (uint) value : this.healthMaximumNormalized; } //    ^^
  [ReadWriteInInspector, System.NonSerialized]       private            uint                                         healthMaximumNormalized                      =  (uint) PatchEntity.HealthMaximumDefault;
  [ReadWriteInInspector, System.NonSerialized]       private            uint                                         healthNormalized                             =  (uint) PatchEntity.HealthMaximumDefault;
  [ReadOnlyInInspector,  UnityEngine.SerializeField] internal           uint                                         HP                                           =  (uint) PatchEntity.HealthMaximumDefault; // ⟶ Editor-only
  [ReadOnlyInInspector]                              public             bool                                         isDefeated                                   => 0.0f == this.health;
  [ReadOnlyInInspector]                              public             bool                                         isFleeing { get; private set; }              =  false;
  [ReadOnlyInInspector]                              public             bool                                         isImmortal                                   =  false;
  [ReadOnlyInInspector]                              public             bool                                         isMoving                                     => UnityEngine.Vector3.zero != this.moveDirection || (null != this.rigidBody && UnityEngine.Vector3.zero != this.rigidBody.linearVelocity);
  [ReadOnlyInInspector]                              public             bool                                         isPoisoned                                   => 0.0f                     != this.poisonedDuration;
  [ReadOnlyInInspector]                              public             bool                                         isPoisonImmune                               => this.healthMaximum       <= this.poisonReduction;
  [ReadOnlyInInspector]                              public             bool                                         isPoisonResistant                            => 0.0f                     != this.poisonReduction;
  [ReadOnlyInInspector]                              public             bool                                         isRegenerating                               => 0.0f                     != this.regenerationDuration;
  [ReadOnlyInInspector]                              public             UnityEngine.Vector3                          moveDirection                                =  UnityEngine.Vector3.zero;
  [ReadOnlyInInspector]                              public             float                                        moveSpeed                                    =  PatchEntity.MoveSpeedDefault;
  [ReadOnlyInInspector]                              public             UnityEngine.Vector3                          moveVelocity                                 =  UnityEngine.Vector3.zero;
  [ReadOnlyInInspector, UnityEngine.SerializeField]  internal           bool                                         poisoned                                     =  false; // ⟶ Editor-only
  [ReadOnlyInInspector, System.NonSerialized]        private            float                                        poisonedDamage                               =  0.0f; // ⟶ Can exceed `::health`
  [ReadOnlyInInspector, System.NonSerialized]        private            double                                       poisonedDuration                             =  0.0f;
  [ReadOnlyInInspector]                              public             float                                        poisonReduction  { get; protected set; }     =  0.0f;
  [ReadOnlyInInspector]                              public             float                                        projectileDamage { get; /*protected*/ set; }     =  0.0f;
  [ReadOnlyInInspector,  System.NonSerialized]       private            float                                        projectileFiringOffset                       =  PatchEntity.ProjectileFiringOffsetDefault;
  [ReadWriteInInspector, UnityEngine.SerializeField] protected          System.Collections.Generic.List<PatchEntity> projectilePrefabs                            =  new(2); // ⟶ Prefab-only
  [ReadOnlyInInspector,  System.NonSerialized]       public    readonly System.Collections.Generic.List<PatchEntity> projectiles                                  =  new(10);
  [ReadOnlyInInspector]                              public             double                                       projectileLifetime { get;/*protected*/set; } =  PatchEntity.ProjectileLifetimeDefault;
  [ReadOnlyInInspector, System.NonSerialized]        private            float                                        projectileSpeed                              =  PatchEntity.ProjectileSpeedDefault;
  [ReadOnlyInInspector, UnityEngine.SerializeField]  internal           bool                                         regenerating                                 =  false; // ⟶ Editor-only
  [ReadOnlyInInspector, System.NonSerialized]        private            float                                        regenerationAmount                           =  0.0f;  // ⟶ Can exceed `::health`
  [ReadOnlyInInspector, System.NonSerialized]        private            double                                       regenerationDuration                         =  0.0;
  [ReadOnlyInInspector]                              protected          UnityEngine.Rigidbody?                       rigidBody                                    =  default!;
  [ReadOnlyInInspector, System.NonSerialized]        private            UnityEngine.RigidbodyConstraints             rigidBodyConstraints                         =  PatchEntity.RigidBodyConstraintsDefault;
  [ReadOnlyInInspector]                              public             PatchEntity.Team                             team                                         =  PatchEntity.Team.Peaceful;

  /* … */
  protected virtual void Awake() {
    this.rigidBody = this.GetComponent<UnityEngine.Rigidbody>();

    if (null != this.rigidBody) {
      this.rigidBody.automaticCenterOfMass  = true;
      this.rigidBody.automaticInertiaTensor = true;
      this.rigidBody.collisionDetectionMode = UnityEngine.CollisionDetectionMode.Discrete;
      this.rigidBody.constraints            = this.rigidBodyConstraints;
      this.rigidBody.detectCollisions       = true;
      this.rigidBody.interpolation          = UnityEngine.RigidbodyInterpolation.None;
      this.rigidBody.isKinematic            = false; // ⟶ Allow collisions, forces, or joints to affect `PatchEntity` rigid bodies
      this.rigidBody.useGravity             = false;
    }
  }

  public static void Cure    (PatchEntity entity, float amount) => entity.DispatchEventListener(propagate: true,  "OnCure", amount);
  public static void CureFull(PatchEntity entity)               => entity.DispatchEventListener(propagate: true,  "OnCureFinished");
  public static void Damage  (PatchEntity entity, float amount) => entity.DispatchEventListener(propagate: true,  "OnDamaged", amount);
  public static void Defeat  (PatchEntity entity)               => entity.DispatchEventListener(propagate: false, "OnDefeated");

  private void DispatchEventListener(string name, params object?[] arguments) => this.DispatchEventListener(true, name, arguments);
  private void DispatchEventListener(bool propagate, string name, params object?[] arguments) {
    System.Collections.Generic.Stack<System.Reflection.MethodInfo> eventListeners;
    System.Type                                                    type = this.GetType();

    // …
    if (PatchEntity.EventListeners.TryGetValue((type, name), out System.Reflection.MethodInfo[] subeventListeners)) {
      foreach (System.Reflection.MethodInfo eventListener in subeventListeners) {
        eventListener.Invoke(this, arguments);
        if (!propagate) return;
      }

      return;
    }

    eventListeners = new(type != typeof(PatchEntity) ? 2 : 1);

    for (System.Type subtype = type; ; subtype = subtype.BaseType) {
      if (subtype.GetMethod(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public) is System.Reflection.MethodInfo eventListener)
      eventListeners.Push(eventListener);

      if (!subtype.IsSubclassOf(typeof(PatchEntity)))
      break;
    }

    for (PatchEntity.EventListeners.Add((type, name), Util.Array<System.Reflection.MethodInfo>.From(eventListeners)); 0 != eventListeners.Count; ) {
      eventListeners.Pop().Invoke(this, arguments);
      if (!propagate) return;
    }
  }

  public static bool Fire(PatchEntity entity)                                                =>                                        PatchEntity.Fire(entity,                              entity.projectileLifetime);
  public static bool Fire(PatchEntity entity,                               double lifetime) => !entity.projectilePrefabs.IsEmpty() && PatchEntity.Fire(entity, entity.projectilePrefabs[0], lifetime);
  public static bool Fire(PatchEntity entity, PatchEntity projectilePrefab)                  =>                                        PatchEntity.Fire(entity, projectilePrefab,            entity.projectileLifetime);
  public static bool Fire(PatchEntity entity, PatchEntity projectilePrefab, double lifetime) {
    UnityEngine.Transform entityTransform = entity.transform;

    // …
    if (entity.firingCooldown.isLooped && entity is not Projectile && Util.Prefab<Projectile>(projectilePrefab, entityTransform.position, entityTransform.rotation) is Projectile projectile) {
      UnityEngine.Transform projectileTransform = projectile.transform;

      // …
      // Util.Wait.Until(lifetime, static data => ((Projectile) data!).Die(), projectile);
      UnityEngine.Object.Destroy(projectile.gameObject, (float) lifetime);

      entity.projectiles.Add(projectile);
      projectile         .firer          = entity;
      projectile         .moveDirection  = entityTransform.forward;
      projectile         .moveSpeed      = entity.projectileSpeed;
      projectile         .name           = "🏹 Projectile (" + entity.name + ')';
      projectileTransform.localPosition += entity.projectileFiringOffset * entityTransform.forward;

      return true;
    }

    return false;
  }

  public static void Flee(PatchEntity entity) {
    if (entity.isFleeing)
    return;

    entity.fleeAnimation         = entity.fleeAnimation.AsCopy();
    entity.fleeAnimationPosition = entity.transform.localPosition;
    entity.isFleeing             = true;

    entity.fleeAnimation.Reset();
    UnityEngine.Object.Destroy(entity.gameObject, (float) (entity.fleeAnimation.delay + entity.fleeAnimation.duration)); // TODO
  }

  public static SharedList<PatchEntity> GetHostiles(PatchEntity entity) => new SharedList<PatchEntity>(entity.team switch {
    PatchEntity.Team.HostileMonsters => Monster.Any,
    PatchEntity.Team.HostilePlayers  => Player .Any,
    PatchEntity.Team.HostileTamers   => Tamer  .Any,
    PatchEntity.Team.Peaceful        => new(),
    PatchEntity.Team.Violent         => PatchEntity.Any,
    _                                => Util.Reference<List<PatchEntity>>.Null
  }).FindAll(hostile => entity != hostile);

  public static bool IsHostileTo(PatchEntity entity, PatchEntity hostile) => entity.team switch {
    PatchEntity.Team.HostileMonsters => hostile is Monster,
    PatchEntity.Team.HostilePlayers  => hostile is Player,
    PatchEntity.Team.HostileTamers   => hostile is Tamer,
    PatchEntity.Team.Peaceful        => false,
    PatchEntity.Team.Violent         => true,
    _                                => Util.Reference<bool>.Null
  };

  public static void Go(PatchEntity entity) {
    if (null != entity.rigidBody) {
      entity.rigidBody.constraints      = entity.rigidBodyConstraints;
      entity.rigidBody.detectCollisions = true;
      entity.rigidBody.freezeRotation   = false;
      entity.rigidBody.isKinematic      = false;

      if (entity.rigidBody.IsSleeping())
      entity.rigidBody.WakeUp();
    }
  }

  public static void Halt(PatchEntity entity) {
    if (null != entity.rigidBody) {
      entity.rigidBodyConstraints       = entity.rigidBody.constraints;
      entity.rigidBody.angularVelocity  = entity.rigidBody.linearVelocity = UnityEngine.Vector3.zero;
      entity.rigidBody.constraints      = UnityEngine.RigidbodyConstraints.FreezeAll;
      entity.rigidBody.detectCollisions = false;
      entity.rigidBody.freezeRotation   = true;
      entity.rigidBody.isKinematic      = true;

      if (!entity.rigidBody.IsSleeping())
      entity.rigidBody.Sleep();
    }
  }

  public static void Heal    (PatchEntity entity, float amount) => entity.DispatchEventListener(propagate: true, "OnHealed", amount);
  public static void Immunize(PatchEntity entity)               => entity.poisonReduction = float.PositiveInfinity; // ⟶ `> ::healthMaximum` is sufficient, too

  protected void OnAI() {
    /* 📅 📅 📅 */
    if (this is not Player && PatchEntity.Team.Peaceful != this.team)
    PatchEntity.Fire(this);

    // foreach (PatchEntity.GetHostiles(this))
    // foreach (Projectile projectile in this.projectiles)
    // if (projectile.GetColliderBounds()?.Intersects() ?? .Contains(projectile.transform.position))
  }

  private void OnCure(float amount) {
    this.poisonedDamage   = this is not Player ? UnityEngine.Mathf.Max(this.poisonedDamage - UnityEngine.Mathf.Abs(amount), 0.0f) : 0.0f;
    this.poisonedDuration = 0.0f == this.poisonedDamage ? 0.0f : this.poisonedDuration;
  }

  private           void OnCureFinished ()             => this.DispatchEventListener("OnCure", this.poisonedDamage);
  private           void OnDamaged      (float amount) => this.health = UnityEngine.Mathf.Max(this.health - UnityEngine.Mathf.Abs(amount), 0.0f);
  private           void OnDefeated     ()             => PatchEntity.Flee(this); // ⟶ “I just want to know… the taste of defeat” 🥊
  protected virtual void OnDestroy      ()             { if (this is Projectile projectile) projectile.firer?.projectiles.Remove(this); else PatchEntity.Any.Remove(this); }
  protected virtual void OnEnable       ()             { if (this is Projectile projectile) projectile.firer?.projectiles.Add   (this); else PatchEntity.Any.Add   (this); }
  private           void OnHealed       (float amount) => this.health = UnityEngine.Mathf.Min(this.health + UnityEngine.Mathf.Abs(amount), this.healthMaximum);
  private           void OnPoisonDamaged()             => this.DispatchEventListener("OnDamaged", this.isPoisonImmune ? 0.0f : UnityEngine.Mathf.Max(this.poisonedDamage - this.poisonReduction, 0.0f));

  private void OnPoisoned(float amount, double duration, bool immediate = false) {
    if (!Game.VerifyDuration(duration))
    return;

    amount                = UnityEngine.Mathf.Abs(amount);
    this.poisonedDamage   = this is not Player ? System.Math.Max(amount, this.poisonedDamage) : amount; // ⟶ Allow weaker poisons override `::poisonedDamage` if `PatchEntity` is a `Player`
    this.poisonedDuration = System.Math.Max(this.poisonedDuration, UnityEngine.Time.realtimeSinceStartupAsDouble + duration);

    if (immediate)
    this.DispatchEventListener("OnPoisonDamaged");
  }

  private void OnRegenerate(float amount, double duration, bool immediate = true) {
    if (!Game.VerifyDuration(duration))
    return;

    if (this is Player && this.isPoisoned) this.DispatchEventListener("OnCure", this.poisonedDamage);
    amount                    = double.IsInfinity(amount) || double.IsNaN(amount) ? 0.0f : UnityEngine.Mathf.Abs(amount);
    this.regenerationAmount   = System.Math.Max(this.regenerationAmount,   amount);
    this.regenerationDuration = System.Math.Max(this.regenerationDuration, UnityEngine.Time.realtimeSinceStartupAsDouble + duration);

    if (immediate)
    this.DispatchEventListener("OnRegenerateHealed");
  }

  private void OnRegenerateFinished() => this.regenerationDuration = this.regenerationAmount = 0.0f;
  private void OnRegenerateHealed  () => this.DispatchEventListener("OnHealed", this.regenerationAmount);

  public static void Poison        (PatchEntity entity)                                => entity.DispatchEventListener(propagate: true, "OnPoisonDamaged");
  public static void Poison        (PatchEntity entity, float amount, double duration) => entity.DispatchEventListener(propagate: true, "OnPoisoned",   amount, duration, true);
  public static void Regenerate    (PatchEntity entity, float amount, double duration) => entity.DispatchEventListener(propagate: true, "OnRegenerate", amount, duration, true);
  public static void RegenerateFull(PatchEntity entity, float amount, double duration) => entity.DispatchEventListener(propagate: true, "OnRegenerateFinished");

  protected virtual void Update() {
    UnityEngine.Rigidbody? rigidBody = this.rigidBody;
    float                  timeDelta = UnityEngine.Time.deltaTime;
    double                 timestamp = UnityEngine.Time.realtimeSinceStartupAsDouble;
    UnityEngine.Transform  transform = this.transform;

    // …
    if (this.health > this.healthMaximum)
      this.health = this.healthMaximum;

    this.fleeing      = this.isFleeing;
    this.HP           = (uint) this.health;
    this.poisoned     = this.isPoisoned;
    this.regenerating = this.isRegenerating;

    // …
    if (Game.Paused)
    return;

    // … ⟶ Update `PatchEntity` stats.
    if (!this.isDefeated) {
      // … ⟶ Handle poison 💀
      if (this.isPoisoned) {
        if (timestamp >= this.poisonedDuration)
        this.DispatchEventListener("OnCure", this.health * Util.Perc(1.0f));
      }

      // … ⟶ Handle regeneration ❤
      if (this.isRegenerating) {
        if (timestamp >= this.regenerationDuration)
        this.DispatchEventListener("OnRegenerateFinished");
      }

      // …
      this.DispatchEventListener("OnPoisonDamaged");
      this.DispatchEventListener("OnRegenerateHealed");

      if (this.isDefeated)
      this.DispatchEventListener(propagate: false, "OnDefeated"); // ⟶ There are no revives
    }

    // … ⟶ “Look who just got ‘defeated’?”
    if (this.isFleeing) {
      PatchEntity.Halt(this);
      transform.localPosition = this.fleeAnimationPosition + (UnityEngine.Vector3.right * (Util.Cast<float>(this.fleeAnimation["sway"]) * this.fleeAnimationIntensity));
      transform.SetMaterialColor(Util.Lerp(Util.Perc(20.0), transform.GetComponent<UnityEngine.Renderer>().material.color, UnityEngine.Color.black)); /* 📅 📅 📅 */

      return;
    }

    // … ⟶ Think & do stuff 🧠
    this.DispatchEventListener(propagate: true, "OnAI");

    // … ⟶ Move `PatchEntity` all over the place
    // rigidBody.linearVelocity = this.moveDirection * this.moveSpeed; // ⟶ Modified outside the purview of `UnityEngine.MonoBehaviour::OnFixedUpdate()`
    transform.localPosition += UnityEngine.Vector3.Scale(UnityEngine.Vector3.one, this.moveDirection * this.moveSpeed * timeDelta);
    /* 📅 📅 📅 */ // if (UnityEngine.Vector3.zero != this.moveDirection) transform.LookAt(transform.position + (this.moveDirection * 42.0f));
  }
}
