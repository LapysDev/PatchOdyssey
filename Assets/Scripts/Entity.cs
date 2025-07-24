using PatchOdyssey;

/* … */
[UnityEngine.DisallowMultipleComponent]
[UnityEngine.RequireComponent(typeof(UnityEngine.Collider))]
[UnityEngine.RequireComponent(typeof(UnityEngine.Rigidbody))]
public abstract class Entity : UnityEngine.MonoBehaviour {
  [ReadOnlyInInspector]                                      private            bool                                     _dying                   = false;
  [ReadOnlyInInspector]                                      private            UnityEngine.Collider                     _collider                = null!;
  [ReadOnlyInInspector]                                      private            UnityEngine.Rigidbody                    _rigidBody               = null!;
  [ReadWriteInInspector, UnityEngine.Tooltip("Must be set")] public             UnityEngine.GameObject                   bulletMeshPrefabrication = null!;
  [ReadWriteInInspector]                                     public    new      ref readonly UnityEngine.Collider        collider                 { get { this._collider = null == this._collider ? this.GetComponent<UnityEngine.Collider>() : this._collider; /* --> base.collider */ return ref this._collider; } }
  [ReadWriteInInspector]                                     public             bool                                     dying                    { get => this._dying; set => this._dying = this._dying || value; } // ->> Cannot be revived
  [ReadWriteInInspector]                                     public             uint                                     health                   = 100u;
  [ReadWriteInInspector]                                     public             float                                    movementDamping          = 1.0f;
  [ReadWriteInInspector]                                     public    readonly System.Collections.Generic.List<Monster> monsters                 = new(1); // ->> Untamed
  [ReadWriteInInspector]                                     public             ref readonly UnityEngine.Rigidbody       rigidBody                { get { this._rigidBody = null == this._rigidBody ? this.GetComponent<UnityEngine.Rigidbody>() : this._rigidBody; return ref this._rigidBody; } }
  [ReadWriteInInspector]                                     public             Timeframe                                shootCooldown            = new(1.0);

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

  protected virtual void OnApplicationFocus(bool _) { /* Do nothing… */ }
  protected         void OnApplicationQuit ()       => this.OnApplicationFocus(false);
}
