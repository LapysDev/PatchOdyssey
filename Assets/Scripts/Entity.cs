using PatchOdyssey;

/* … */
[UnityEngine.DisallowMultipleComponent]
[UnityEngine.RequireComponent(typeof(UnityEngine.Rigidbody))]
public abstract class Entity : UnityEngine.MonoBehaviour {
  public const float ShootCooldown = 1.0f;

  private            bool                                     _dying        = false;
  private            UnityEngine.Rigidbody                    _rigidBody    = null!;
  public             bool                                     dying         { get => this._dying; set => this._dying = this._dying || value; } // ->> Cannot be revived
  public             uint                                     health        = 100u;
  public    readonly System.Collections.Generic.List<Monster> monsters      = new(1); // ->> Untamed
  public             ref readonly UnityEngine.Rigidbody       rigidBody     { get { this._rigidBody ??= this.GetComponent<UnityEngine.Rigidbody>(); return ref this._rigidBody; } }
  protected          Timeframe                                shootCooldown = new Timeframe(Entity.ShootCooldown);

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
  }
}
