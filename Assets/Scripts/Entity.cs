using PatchOdyssey;

/* … */
[UnityEngine.DisallowMultipleComponent]
[UnityEngine.RequireComponent(typeof(UnityEngine.Collider))]
[UnityEngine.RequireComponent(typeof(UnityEngine.Rigidbody))]
public abstract class Entity : UnityEngine.MonoBehaviour {
  public enum Team          : byte { Explorer /* ->> Player */, Enemy /* ->> Series of `Enemy + …` for other teams */ }
  public enum TurnDirection : byte { Anticlockwise, Clockwise }

  /* … */
  public const float                DefeatedShrinkFactor               = 0.03f;
  public const float                DefeatedSinkHeight                 = 0.50f;
  public const float                DefeatedSwayDuration               = 0.10f;
  public const float                DefeatedSwayFactor                 = 1.00f;
  public static UnityEngine.Vector3 MovementVelocityThreshold { get; } = UnityEngine.Vector3.one * 1.0f;

  [ReadOnlyInInspector]                                         private            UnityEngine.Collider                    _collider                      = null!;
  [ReadOnlyInInspector]                                         private            UnityEngine.Rigidbody                   _rigidBody                     = null!;
  [ReadOnlyInInspector]                                         protected          UnityEngine.Camera?                     actualMainCamera               = null;
  [ReadOnlyInInspector]                                         protected          bool                                    actuallyDefeated               = false;
  [ReadWriteInInspector]                                        public             float                                   bounceAngle                    = 10.00f; // ->> in Degrees --> 0.0f <= |bounceAngle| <= ~180.0f
  [ReadWriteInInspector]                                        public             bool                                    bounceAutomatically            = true;
  [ReadWriteInInspector]                                        public             float                                   bounceForce                    = 7.00f;
  [ReadWriteInInspector]                                        public             float                                   bounceSpeed                    = 5.00f; // ->> Degrees per second
  [ReadOnlyInInspector]                                         public             Entity.TurnDirection                    bounceTurn                     = Entity.TurnDirection.Clockwise;
  [ReadWriteInInspector]                                        public    new      ref readonly UnityEngine.Collider       collider                       { get { this._collider = null == this._collider ? this.GetComponent<UnityEngine.Collider>() : this._collider; /* --> base.collider */ return ref this._collider; } }
  [ReadWriteInInspector]                                        public             UnityEngine.Vector3                     defeatedPosition               = UnityEngine.Vector3.zero;
  [ReadWriteInInspector]                                        public             Timeframe                               defeatTimeout                  = new(4.00);
  [ReadWriteInInspector, System.NonSerialized]                  public    readonly System.Collections.Generic.List<Entity> followers                      = new(1);
  [ReadOnlyInInspector]                                         public             Entity?                                 following                      = null; // ->> Entity to follow (typically ally)
  [ReadWriteInInspector]                                        public             uint                                    health                         = 100u;
  [ReadWriteInInspector]                                        public             bool                                    isDefeated                     { get => this.actuallyDefeated; set => this.actuallyDefeated = this.actuallyDefeated || value; } // ->> Cannot be revived
  [ReadOnlyInInspector]                                         protected          ref readonly UnityEngine.Camera?        mainCamera                     { get { this.actualMainCamera = null == this.actualMainCamera ? UnityEngine.Camera.main : this.actualMainCamera; /* --> base.camera */ return ref this.actualMainCamera; } }
  [ReadWriteInInspector]                                        public             bool                                    moveAutomatically              = true; // ->> Reach for `target` entity
  [ReadWriteInInspector, UnityEngine.Tooltip("For setup only")] public             float                                   movementDamping                = 1.0f;
  [ReadOnlyInInspector]                                         public             UnityEngine.Vector3                     movementDirection              = UnityEngine.Vector3.zero;
  [ReadOnlyInInspector]                                         public             Timeframe                               movementRestCooldown           = new(0.00);
  [ReadWriteInInspector]                                        public             double                                  movementRestDuration           = 0.00;
  [ReadWriteInInspector]                                        public             double                                  movementRestDurationRandomness = 0.75;
  [ReadWriteInInspector]                                        public             float                                   movementSpeed                  = 2.00f;
  [ReadOnlyInInspector, UnityEngine.Range(0.25f, 2.00f)]        public             float                                   movementSpeedFactor            = 1.00f;
  [ReadOnlyInInspector, UnityEngine.Range(0.00f, 1.00f)]        public             float                                   movementSpeedRandomnessFactor  = 0.3f;
  [ReadWriteInInspector]                                        public             ref readonly UnityEngine.Rigidbody      rigidBody                      { get { this._rigidBody = null == this._rigidBody ? this.GetComponent<UnityEngine.Rigidbody>() : this._rigidBody; return ref this._rigidBody; } }
  [ReadWriteInInspector]                                        public             Timeframe                               shootCooldown                  = new(1.00);
  [ReadWriteInInspector]                                        public             Entity?                                 target                         = null;  // ->> Entity to reach (typically enemy)
  [ReadWriteInInspector]                                        public             float                                   targetBerth                    = 2.25f; // ->> Radius
  [ReadOnlyInInspector]                                         public             UnityEngine.Vector3                     turnDirection                  = UnityEngine.Vector3.zero;
  [ReadWriteInInspector]                                        public             float                                   turnSpeed                      = 9.00f; // ->> Degrees per second

  /* … */
  protected virtual void Awake() {
    this.rigidBody.useGravity             = false;
    this.rigidBody.mass                   = 1.0f;
    this.rigidBody.linearDamping          = this.movementDamping;
    this.rigidBody.isKinematic            = false;
    this.rigidBody.interpolation          = UnityEngine.RigidbodyInterpolation.None;
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
    this.movementRestCooldown.duration    = this.movementRestDuration;
    this.defeatTimeout.easing             = Timeframe.EaseInOutQuintic;
    this.collider.isTrigger               = true;
    this.collider.includeLayers           = (UnityEngine.LayerMask) ~0x0;
    this.collider.hasModifiableContacts   = false;
    this.collider.excludeLayers           = (UnityEngine.LayerMask) 0x0;
    this.collider.enabled                 = true;

    if (Assets.main.outline is not null && Assets.main.outlineAutomatically)
    this.transform.ForEach(static transform => {
      if (transform.GetComponent<UnityEngine.Renderer>() is UnityEngine.Renderer renderer && null != renderer) {
        UnityEngine.Material[] materials    = renderer.sharedMaterials;
        UnityEngine.Material[] submaterials = new UnityEngine.Material[materials.Length + 1];

        // …
        materials.CopyTo(submaterials, 0);

        submaterials[materials.Length] = Assets.main.outline;
        renderer.receiveShadows        = false;
        renderer.shadowCastingMode     = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.sharedMaterials       = submaterials;
      }
    });
  }

  protected virtual void LateUpdate() {
    if (Game.IsPaused || this.isDefeated)
    return;

    // …
    if (this.movementRestCooldown.isLooped)
    this.movementRestCooldown.duration = this.movementRestDuration + (Game.Randomizer.NextDouble() * this.movementRestDurationRandomness * (Game.Randomizer.NextDouble() < 0.5 ? +1.0 : -1.0));
  }

  protected virtual void OnApplicationFocus(bool _) { /* Do nothing… */ }
  protected         void OnApplicationQuit ()       => this.OnApplicationFocus(false);

  public static UnityEngine.Vector3 TurnDirectionToVector3(Entity.TurnDirection turn, UnityEngine.Vector3 direction) => turn switch {
    Entity.TurnDirection.Anticlockwise => -direction,
    Entity.TurnDirection.Clockwise     =>  direction,
    _                                  =>  UnityEngine.Vector3.zero
  };

  protected virtual void Update() {
    UnityEngine.Vector3 rotation = this.rigidBody.rotation.eulerAngles;

    // …
    if (Game.IsPaused)
    return;

    // … ->> Defeating
    if (!this.isDefeated) {
      this.defeatedPosition = this.transform.position;
      this.defeatTimeout.Reset();
    }

    else {
      UnityEngine.Transform transform                      = this.transform;
      UnityEngine.Vector3   transformToCameraDirection     = null != this.mainCamera ? (this.mainCamera.transform.position - transform.position).normalized : UnityEngine.Vector3.forward;
      float                 transformForwardToCameraFactor = UnityEngine.Mathf.Abs(UnityEngine.Vector3.Dot(transformToCameraDirection, transform.forward));
      float                 transformRightToCameraFactor   = UnityEngine.Mathf.Abs(UnityEngine.Vector3.Dot(transformToCameraDirection, transform.right));
      float                 transformToCameraFactors       = transformForwardToCameraFactor + transformRightToCameraFactor;
      float                 swayForce                      = (float) this.defeatTimeout.elapsed / Entity.DefeatedSwayDuration;
      UnityEngine.Vector3   swayDirection                  = Entity.TurnDirectionToVector3(0 == (((uint) swayForce) & 1) ? Entity.TurnDirection.Anticlockwise : Entity.TurnDirection.Clockwise, UnityEngine.Vector3.right);

      // …
      this.rigidBody.interpolation          = UnityEngine.RigidbodyInterpolation.None;
      this.rigidBody.includeLayers          = (UnityEngine.LayerMask) 0x0;
      this.rigidBody.freezeRotation         = true;
      this.rigidBody.excludeLayers          = (UnityEngine.LayerMask) ~0x0;
      this.rigidBody.detectCollisions       = false;
      this.rigidBody.constraints            = UnityEngine.RigidbodyConstraints.FreezeAll;
      this.rigidBody.automaticInertiaTensor = false;
      this.rigidBody.automaticCenterOfMass  = false;
      transform.position                    = this.defeatedPosition + (swayDirection * ((swayForce - (uint) swayForce) - 0.5f) * Entity.DefeatedSwayFactor) + (UnityEngine.Vector3.up * -Entity.DefeatedSinkHeight * (float) this.defeatTimeout.easedProgress);
      transform.localScale                  = UnityEngine.Vector3.Scale(transform.localScale,
        (UnityEngine.Vector3.forward * (1.0f - (Entity.DefeatedShrinkFactor * (transformForwardToCameraFactor / transformToCameraFactors)))) +
        (UnityEngine.Vector3.right   * (1.0f - (Entity.DefeatedShrinkFactor * (transformRightToCameraFactor   / transformToCameraFactors)))) +
        (UnityEngine.Vector3.up      * (1.0f - 0.0f))
      );

      this.rigidBody.Sleep();
      transform.SetParent(null, true);

      if (this.defeatTimeout.isElapsed)
      UnityEngine.Object.Destroy(this.gameObject);

      return;
    }

    // … ->> Bouncing
    if (this.bounceAutomatically && UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, Entity.MovementVelocityThreshold).sqrMagnitude < UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, this.rigidBody.linearVelocity).sqrMagnitude) {
      float bounceAngleMagnitudeSquared = (UnityEngine.Vector3.forward * this.bounceAngle).sqrMagnitude;
      float bounceMagnitude;

      // … ->> Rotation
      this.rigidBody.constraints &= ~UnityEngine.RigidbodyConstraints.FreezeRotationZ;
      this.rigidBody.MoveRotation(UnityEngine.Quaternion.Euler(
        rotation + // ->> Apply Z-axis orientation (cumulative)
        (Entity.TurnDirectionToVector3(this.bounceTurn, UnityEngine.Vector3.forward) * UnityEngine.Time.unscaledDeltaTime * this.bounceAngle * this.bounceSpeed)
      ));

      rotation        = this.rigidBody.rotation.eulerAngles;
      rotation        = new(rotation.x > 180.0f ? rotation.x - 360.0f : rotation.x, rotation.y > 180.0f ? rotation.y - 360.0f : rotation.y, rotation.z > 180.0f ? rotation.z - 360.0f : rotation.z);
      bounceMagnitude = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, rotation).sqrMagnitude;

      if (bounceMagnitude >= bounceAngleMagnitudeSquared) {
        // … ->> Checked Z-axis orientation for `bounceAngle` threshold
        this.rigidBody.MoveRotation(UnityEngine.Quaternion.Euler(
          UnityEngine.Vector3.Scale(UnityEngine.Vector3.right + UnityEngine.Vector3.up, rotation) +                               // ->> Remove Z-axis orientation
          (Entity.TurnDirectionToVector3(this.bounceTurn, UnityEngine.Vector3.forward) * (this.bounceAngle - Game.VectorEpsilon)) // ->> Apply  Z-axis orientation
        ));

        this.bounceTurn = this.bounceTurn switch { Entity.TurnDirection.Anticlockwise => Entity.TurnDirection.Clockwise, Entity.TurnDirection.Clockwise => Entity.TurnDirection.Anticlockwise, _ => this.bounceTurn };
      }

      // … ->> Position
      #if true
        this.rigidBody.constraints &= ~UnityEngine.RigidbodyConstraints.FreezePositionY;
        this.rigidBody.position     = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, this.rigidBody.position) + (UnityEngine.Vector3.up * this.bounceForce * (bounceMagnitude / bounceAngleMagnitudeSquared));
      #else
        this.rigidBody.constraints &= ~UnityEngine.RigidbodyConstraints.FreezePositionY;
        this.rigidBody.useGravity   = true;

        if (this.rigidBody.position.y <= +0.0f) {
          this.rigidBody.position = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, this.rigidBody.position);
          this.rigidBody.AddForce(UnityEngine.Vector3.up * this.bounceForce, UnityEngine.ForceMode.Impulse);
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
        UnityEngine.Time.unscaledDeltaTime * this.bounceSpeed
      ));

      // … ->> Position
      this.rigidBody.linearVelocity = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, this.rigidBody.linearVelocity);

      #if true
        this.rigidBody.constraints |= UnityEngine.RigidbodyConstraints.FreezePositionY;
        this.rigidBody.position     = UnityEngine.Vector3.Slerp(this.rigidBody.position, UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, this.rigidBody.position), !this.movementRestCooldown.isElapsed ? UnityEngine.Time.unscaledDeltaTime * this.bounceForce : 1.0f);
      #else
        if (this.rigidBody.position.y <= +0.0f) {
          this.rigidBody.constraints |= UnityEngine.RigidbodyConstraints.FreezePositionY;
          this.rigidBody.position     = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, this.rigidBody.position);
          this.rigidBody.useGravity   = false;
        }
      #endif
    }

    // … ->> Moving
    if (UnityEngine.Vector3.zero != this.movementDirection && this.moveAutomatically && (this is Player || this.movementRestCooldown.isElapsed)) {
      this.rigidBody.AddForce(this.movementDirection * this.movementSpeedFactor * ((this.movementSpeed * (1.0f - this.movementSpeedRandomnessFactor)) + (this.movementSpeed * this.movementSpeedRandomnessFactor * (float) Game.Randomizer.NextDouble())), UnityEngine.ForceMode.Impulse);
      this.turnDirection = this.movementDirection;
    }

    // … ->> Turning
    if (UnityEngine.Vector3.zero != this.turnDirection)
    this.rigidBody.MoveRotation(UnityEngine.Quaternion.Slerp(
      this.rigidBody.rotation,
      UnityEngine.Quaternion.Euler(
        UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, this.rigidBody.rotation.eulerAngles) + // ->> Remove Y-axis orientation
        UnityEngine.Vector3.Scale(UnityEngine.Vector3.up, UnityEngine.Quaternion.LookRotation(this.turnDirection).eulerAngles)    // ->> Apply  Y-axis orientation
      ),
      UnityEngine.Time.unscaledDeltaTime * this.turnSpeed
    ));
  }
}
