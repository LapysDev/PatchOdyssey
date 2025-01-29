using PatchOdyssey;

/* … */
#nullable enable annotations

/* … */
[UnityEngine.RequireComponent(typeof(UnityEngine.Rigidbody))]
public class NPC : UnityEngine.MonoBehaviour {
  public enum Mode : byte {
    Idle,
    Combat,
    Independent,
    Follow
  };

  /* … */
  private const float MOVEMENT_ACCELERATION = 4.0e1f;
  private const float MOVEMENT_DECELERATION = 4.0e0f;

  [ReadWriteInInspector]    new public  UnityEngine.Camera? camera                    =  null;
  [ReadWriteInInspector]        public  UnityEngine.Vector3 cameraAngle               =  new(35.0f, -90.0f, 0.0f);
  [ReadWriteInInspector]        public  UnityEngine.Vector3 cameraOffset              =  new(15.0f,  12.5f, 0.0f);
  [ReadWriteInInspector]        public  NPC.Mode            mode                      =  NPC.Mode.Idle;
  [UnityEngine.HideInInspector] public  UnityEngine.Vector3 movement                  =  UnityEngine.Vector3.zero; // → Preserves scale of movement direction
  [ReadWriteInInspector]        public  UnityEngine.Vector3 movementDirection         => this.movement / Util.Max(System.Math.Abs(this.movement.x), System.Math.Abs(this.movement.y), System.Math.Abs(this.movement.z));
  [UnityEngine.HideInInspector] public  float               movementDurationElapsed   =  0.0f;
  [ReadWriteInInspector]        private UnityEngine.Vector3 movementOrigin            =  UnityEngine.Vector3.zero; // → Recent `….transform.position` before `NPC::MoveBy(…)`, …
  [ReadWriteInInspector]        public  float               movementSpeed             =  1.0f;
  [UnityEngine.HideInInspector] public  float               noMovementDurationElapsed =  0.0f;
  [ReadWriteInInspector]        public  float               rotationSpeed             =  360.0f; // → in Degrees per Second

  /* … */
  public void MoveBy(UnityEngine.Vector2 distance) {
    this.MoveBy(new UnityEngine.Vector3(distance.x, 0.0f, distance.y));
  }

  public void MoveBy(UnityEngine.Vector3 distance) {
    if (Game.main?.isPlaying ?? false) {
      this.movement                  = Util.IgnoreVectorHeightAxes(distance);
      this.movementOrigin            = this.transform.position;
      this.noMovementDurationElapsed = 0.0f;
    }
  }

  public void MoveHalt() {
    if (Game.main?.isPlaying ?? false) {
      this.movement                = Util.IgnoreVectorForwardAxes(this.movement);
      this.movementDurationElapsed = 0.0f;
      this.movementOrigin          = this.transform.position;
    }
  }

  protected void Start() {}

  protected void Update() {
    UnityEngine.Rigidbody npcRigidBody        = this.gameObject.GetComponent<UnityEngine.Rigidbody>();
    bool                  npcHasMoveDirection = 0.0f != this.movement.x || 0.0f != this.movement.z;

    // → Move the `NPC` Non-Player Character
    npcRigidBody.collisionDetectionMode     = UnityEngine.CollisionDetectionMode.Discrete;
    npcRigidBody.constraints               |= UnityEngine.RigidbodyConstraints.FreezeRotationY;
    npcRigidBody.detectCollisions           = true;
    npcRigidBody.excludeLayers              = 0x0; // → `~UnityEngine.Physics.AllLayers`
    npcRigidBody.freezeRotation             = true;
    npcRigidBody.includeLayers              = 0x0;                   // → `~UnityEngine.Physics.AllLayers`
    npcRigidBody.linearDamping              = MOVEMENT_DECELERATION; // → This property was painful to learn about T_T
    npcRigidBody.useGravity                 = true;
    this        .movementDurationElapsed   +=  npcHasMoveDirection ? UnityEngine.Time.deltaTime : 0.0f;
    this        .noMovementDurationElapsed += !npcHasMoveDirection ? UnityEngine.Time.deltaTime : 0.0f;

    if (null != this.camera) {
      this.camera.transform.position = this.transform.position + this.cameraOffset;
      this.camera.transform.LookAt(this.transform, UnityEngine.Vector3.up);
      this.camera.transform.rotation = UnityEngine.Quaternion.Euler(this.cameraAngle);
    }

    if (Game.main?.isPlaying ?? false) {
      float movementDepth = Util.GetVectorForwardAxis(this.movement);
      float movementWidth = Util.GetVectorWidthAxis  (this.movement);

      // …
      npcRigidBody.AddForce(this.transform.forward * (MOVEMENT_ACCELERATION * UnityEngine.Time.deltaTime * movementDepth * this.movementSpeed), UnityEngine.ForceMode.Impulse);
      if (0.0f != movementWidth) this.transform.rotation = UnityEngine.Quaternion.AngleAxis(UnityEngine.Time.deltaTime * this.rotationSpeed, this.transform.up * (movementWidth > 0.0f ? +1.0f : -1.0f)) * this.transform.rotation;
    }
  }
}
