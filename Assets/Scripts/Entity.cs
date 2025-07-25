using PatchOdyssey;

/* … */
[UnityEngine.DisallowMultipleComponent]
[UnityEngine.RequireComponent(typeof(UnityEngine.Collider))]
[UnityEngine.RequireComponent(typeof(UnityEngine.Rigidbody))]
public abstract class Entity : UnityEngine.MonoBehaviour {
  public enum TurnDirection : byte { Anticlockwise, Clockwise }

  /* … */
  public static readonly UnityEngine.Vector3 MovementVelocityThreshold = UnityEngine.Vector3.one * 1.0f;

  [ReadOnlyInInspector]                                         private          bool                                     _dying                   = false;
  [ReadOnlyInInspector]                                         private          UnityEngine.Collider                     _collider                = null!;
  [ReadOnlyInInspector]                                         private          UnityEngine.Rigidbody                    _rigidBody               = null!;
  [ReadWriteInInspector]                                        public           bool                                     bounce                   = false;
  [ReadWriteInInspector]                                        public           float                                    bounceAngle              = 10.0f; // ->> in Degrees --> 0.0f <= |bounceAngle| <= ~180.0f
  [ReadWriteInInspector]                                        public           float                                    bounceForce              = 7.0f;
  [ReadWriteInInspector]                                        public           float                                    bounceSpeed              = 5.0f; // ->> Degrees per second
  [ReadOnlyInInspector]                                         public           Entity.TurnDirection                     bounceTurn               = Entity.TurnDirection.Clockwise;
  [ReadWriteInInspector, UnityEngine.Tooltip("Must be set")]    public           UnityEngine.GameObject                   bulletMeshPrefabrication = null!;
  [ReadWriteInInspector]                                        public  new      ref readonly UnityEngine.Collider        collider                 { get { this._collider = null == this._collider ? this.GetComponent<UnityEngine.Collider>() : this._collider; /* --> base.collider */ return ref this._collider; } }
  [ReadWriteInInspector]                                        public           bool                                     dying                    { get => this._dying; set => this._dying = this._dying || value; } // ->> Cannot be revived
  [ReadWriteInInspector]                                        public           uint                                     health                   = 100u;
  [ReadWriteInInspector]                                        public  readonly System.Collections.Generic.List<Monster> monsters                 = new(1); // ->> Untamed
  [ReadWriteInInspector, UnityEngine.Tooltip("For setup only")] public           float                                    movementDamping          = 1.0f;
  [ReadOnlyInInspector]                                         public           UnityEngine.Vector3                      movementDirection        = UnityEngine.Vector3.zero;
  [ReadWriteInInspector]                                        public           float                                    movementSpeed            = 2.0f;
  [ReadWriteInInspector]                                        public           ref readonly UnityEngine.Rigidbody       rigidBody                { get { this._rigidBody = null == this._rigidBody ? this.GetComponent<UnityEngine.Rigidbody>() : this._rigidBody; return ref this._rigidBody; } }
  [ReadWriteInInspector]                                        public           Timeframe                                shootCooldown            = new(1.0);
  [ReadOnlyInInspector]                                         public           UnityEngine.Vector3                      turnDirection            = UnityEngine.Vector3.zero;
  [ReadWriteInInspector]                                        public           float                                    turnSpeed                = 9.0f; // ->> Degrees per second

  /* … */
  protected void Awake() {
    this.rigidBody.useGravity             = false;
    this.rigidBody.mass                   = 1.0f;
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
    this.collider .isTrigger              = false;
    this.collider .includeLayers          = (UnityEngine.LayerMask) ~0x0;
    this.collider .hasModifiableContacts  = false;
    this.collider .excludeLayers          = (UnityEngine.LayerMask) 0x0;
    this.collider .enabled                = true;

    this.rigidBody.linearDamping = this.movementDamping;
  }

  protected virtual void                OnApplicationFocus    (bool _)                                                   { /* Do nothing… */ }
  protected         void                OnApplicationQuit     ()                                                         => this.OnApplicationFocus(false);
  public    static  UnityEngine.Vector3 TurnDirectionToVector3(Entity.TurnDirection turn, UnityEngine.Vector3 direction) => turn switch { Entity.TurnDirection.Anticlockwise => -direction, Entity.TurnDirection.Clockwise => direction, _ => UnityEngine.Vector3.zero };

  protected virtual void Update() {
    UnityEngine.Vector3   rotation  = this.rigidBody.rotation.eulerAngles;
    UnityEngine.Transform transform = this.transform;

    // … ->> Bouncing
    if (this.bounce || UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, Entity.MovementVelocityThreshold).sqrMagnitude < UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, this.rigidBody.linearVelocity).sqrMagnitude) {
      this.rigidBody.constraints &= ~(UnityEngine.RigidbodyConstraints.FreezePositionY | UnityEngine.RigidbodyConstraints.FreezeRotationZ);
      this.rigidBody.useGravity   = true;

      // … ->> Rotation
      this.rigidBody.MoveRotation(UnityEngine.Quaternion.Euler(
        rotation + // ->> Apply Z-axis orientation (cumulative)
        (Entity.TurnDirectionToVector3(this.bounceTurn, UnityEngine.Vector3.forward) * UnityEngine.Time.deltaTime * this.bounceAngle * this.bounceSpeed)
      ));

      rotation = this.rigidBody.rotation.eulerAngles;
      rotation = new(rotation.x > 180.0f ? rotation.x - 360.0f : rotation.x, rotation.y > 180.0f ? rotation.y - 360.0f : rotation.y, rotation.z > 180.0f ? rotation.z - 360.0f : rotation.z);

      if (UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, rotation).sqrMagnitude >= (UnityEngine.Vector3.forward * this.bounceAngle).sqrMagnitude) {
        // … ->> Checked Z-axis orientation for `bounceAngle` threshold
        this.rigidBody.MoveRotation(UnityEngine.Quaternion.Euler(
          UnityEngine.Vector3.Scale(UnityEngine.Vector3.right + UnityEngine.Vector3.up, rotation) +                               // ->> Remove Z-axis orientation
          (Entity.TurnDirectionToVector3(this.bounceTurn, UnityEngine.Vector3.forward) * (this.bounceAngle - Game.VectorEpsilon)) // ->> Apply  Z-axis orientation
        ));

        this.bounceTurn = this.bounceTurn switch { Entity.TurnDirection.Anticlockwise => Entity.TurnDirection.Clockwise, Entity.TurnDirection.Clockwise => Entity.TurnDirection.Anticlockwise, _ => this.bounceTurn };
      }

      // … ->> Position
      if (this.rigidBody.position.y <= +0.0f) {
        this.rigidBody.position = UnityEngine.Vector3.Scale(this.rigidBody.position, new(1.0f, 0.0f, 1.0f));
        this.rigidBody.AddForce(UnityEngine.Vector3.up * this.bounceForce, UnityEngine.ForceMode.Impulse);
      }
    } else {
      // … ->> Rotation
      this.rigidBody.constraints |= UnityEngine.RigidbodyConstraints.FreezeRotationZ;

      this.rigidBody.MoveRotation(UnityEngine.Quaternion.Slerp(
        // ->> Remove Z-axis orientation
        UnityEngine.Quaternion.Euler(rotation),
        UnityEngine.Quaternion.Euler(UnityEngine.Vector3.Scale(UnityEngine.Vector3.right + UnityEngine.Vector3.up, rotation)),
        UnityEngine.Time.deltaTime * this.bounceSpeed
      ));

      // … ->> Position
      if (this.rigidBody.position.y <= +0.0f) {
        this.rigidBody.constraints |= UnityEngine.RigidbodyConstraints.FreezePositionY;
        this.rigidBody.position     = UnityEngine.Vector3.Scale(this.rigidBody.position, new(1.0f, 0.0f, 1.0f));
        this.rigidBody.useGravity   = false;
      }
    }
  }
}
