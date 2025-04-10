// using PatchOdyssey;

// /* … */
// [UnityEngine.RequireComponent(typeof(UnityEngine.Collider))]
// [UnityEngine.RequireComponent(typeof(UnityEngine.Rigidbody))]
// public class NPC : UnityEngine.MonoBehaviour {
//   public enum Mode : byte {
//     Idle,
//     Combat,
//     Independent,
//     Support
//   };

//   /* … */
//   private const float MOVEMENT_ACCELERATION = 4.0e1f;
//   private const float MOVEMENT_DECELERATION = 2.5e0f;

//   [PatchOdyssey.ReadWriteInInspector]    new public  UnityEngine.Camera? camera                    =  null;
//   [PatchOdyssey.ReadWriteInInspector]        public  UnityEngine.Vector3 cameraAngle               =  new(35.0f, -90.0f, 0.0f);
//   [PatchOdyssey.ReadWriteInInspector]        public  UnityEngine.Vector3 cameraOffset              =  new(12.5f,  10.0f, 0.0f);
//   [PatchOdyssey.ReadWriteInInspector]        public  UnityEngine.Light?  glowlight                 =  null;                     // TODO (Lapys)
//   [PatchOdyssey.ReadOnlyInInspector]         public  float               glowlightIntensity        =  5.0f;                     // TODO (Lapys)
//   [PatchOdyssey.ReadOnlyInInspector]         public  UnityEngine.Vector3 glowlightOrigin           =  UnityEngine.Vector3.zero; // TODO (Lapys)
//   [PatchOdyssey.ReadOnlyInInspector]         public  float               glowlightRange            =  10.0f;                    // TODO (Lapys)
//   [PatchOdyssey.ReadWriteInInspector]        public  NPC.Mode            mode                      =  NPC.Mode.Idle;
//   [PatchOdyssey.ReadWriteInInspector]        public  Mount?              mount                     =  null;
//   [PatchOdyssey.ReadOnlyInInspector]         public  bool                mountIsChanged            =  false; // → Prevent spamming `NPC::Dismount(…)` and `NPC::Mount(…)` calls
//   [PatchOdyssey.ReadOnlyInInspector]         public  NPC.Mode            mountMode                 =  NPC.Mode.Idle;
//   [UnityEngine .HideInInspector] public  UnityEngine.Vector3 movement                  =  UnityEngine.Vector3.zero; // → Preserves scale of movement direction
//   [PatchOdyssey.ReadWriteInInspector]        public  UnityEngine.Vector3 movementDirection         => this.movement / Util.Max(System.Math.Abs(this.movement.x), System.Math.Abs(this.movement.y), System.Math.Abs(this.movement.z));
//   [UnityEngine .HideInInspector] public  float               movementDurationElapsed   =  0.0f;
//   [PatchOdyssey.ReadWriteInInspector]        private UnityEngine.Vector3 movementOrigin            =  UnityEngine.Vector3.zero; // → Recent `….transform.position` before `NPC::MoveBy(…)`, …
//   [PatchOdyssey.ReadWriteInInspector]        public  float               movementSpeed             =  1.0f;
//   [UnityEngine .HideInInspector] public  float               noMovementDurationElapsed =  0.0f;
//   [PatchOdyssey.ReadWriteInInspector]        public  float               rotationSpeed             =  360.0f; // → in Degrees per Second

//   /* … */
//   private void Awake() {
//     this.Ensure();
//   }

//   public void Dismount() {
//     if (null != this.mount && !this.mountIsChanged) {
//       this.mount.mode     = this.mountMode;
//       this.mountMode      = NPC.Mode.Idle;
//       this.mountIsChanged = true;
//     }

//     this.mount = null;
//   }

//   public void Ensure() {
//     for (Mount? mount = this.mount; null != mount; mount = null != mount ? mount.mount : null)
//     if (mount == this) {
//       this.mount = null;
//       break;
//     }

//     if (null != this.mount) {
//       if (null != this.mount.mount)
//       this.mount.mount = null;
//     }
//   }

//   public void Mount(Mount mount) {
//     if (mount != this.mount && !this.mountIsChanged) {
//       if (null != this.mount)
//         this.Dismount();

//       // …
//       this.mount          = mount;
//       this.mountIsChanged = true;
//       this.mountMode      = mount.mode;

//       mount.mode = NPC.Mode.Idle;
//     }
//   }

//   public void MoveBy(UnityEngine.Vector2 distance) {
//     this.MoveBy(new UnityEngine.Vector3(distance.x, 0.0f, distance.y));
//   }

//   public void MoveBy(UnityEngine.Vector3 distance) {
//     if (Game.main?.isPlaying ?? false) {
//       this.movement                  = Util.ExcludeVectorHeightAxes(distance);
//       this.movementOrigin            = this.transform.position;
//       this.noMovementDurationElapsed = 0.0f;
//     }
//   }

//   public void MoveHalt() {
//     if (Game.main?.isPlaying ?? false) {
//       this.movement                = Util.ExcludeVectorForwardAxes(this.movement);
//       this.movementDurationElapsed = 0.0f;
//       this.movementOrigin          = this.transform.position;
//     }
//   }

//   protected void Start() {
//     this.Ensure();

//     if (null != this.glowlight)
//     this.glowlightOrigin = this.glowlight.transform.position;
//   }

//   protected void Update() {
//     UnityEngine.Rigidbody npcRigidBody        = this.gameObject.GetComponent<UnityEngine.Rigidbody>();
//     bool                  npcHasMoveDirection = 0.0f != this.movement.x || 0.0f != this.movement.z;

//     // …
//     this.Ensure();

//     npcRigidBody.collisionDetectionMode     = UnityEngine.CollisionDetectionMode.Discrete;
//     npcRigidBody.constraints               |= UnityEngine.RigidbodyConstraints.FreezeRotationY;
//     npcRigidBody.detectCollisions           = true;
//     npcRigidBody.excludeLayers              = 0x0; // → `~UnityEngine.Physics.AllLayers`
//     npcRigidBody.freezeRotation             = true;
//     npcRigidBody.includeLayers              = 0x0;                   // → `~UnityEngine.Physics.AllLayers`
//     npcRigidBody.linearDamping              = MOVEMENT_DECELERATION; // → This property was painful to learn about T_T
//     npcRigidBody.useGravity                 = true;
//     this        .movementDurationElapsed   +=  npcHasMoveDirection ? UnityEngine.Time.deltaTime : 0.0f;
//     this        .noMovementDurationElapsed += !npcHasMoveDirection ? UnityEngine.Time.deltaTime : 0.0f;

//     if (null != this.camera) {
//       this.camera.transform.position = this.transform.position + this.cameraOffset;
//       this.camera.transform.LookAt(this.transform, UnityEngine.Vector3.up);
//       this.camera.transform.rotation = UnityEngine.Quaternion.Euler(this.cameraAngle);

//       // Strafing and turning around will tilt the camera sideways.
//       // Jumping, falling, and moving forward or backwards will affect the camera's pitch.
//       // Nearby explosions will shake the screen, and
//       // Ceasing movement for enough time will cause the camera to gently sway around while waiting for your input.
//     }

//     if (null != this.glowlight)
//     this.glowlight.type = UnityEngine.LightType.Point;

//     if (Game.main?.isPlaying ?? false) {
//       float movementDepth = Util.GetVectorForwardAxis(this.movement);
//       float movementWidth = Util.GetVectorWidthAxis  (this.movement);

//       // …
//       npcRigidBody.AddForce(this.transform.forward * (MOVEMENT_ACCELERATION * UnityEngine.Time.deltaTime * movementDepth * this.movementSpeed), UnityEngine.ForceMode.Impulse);
//       if (0.0f != movementWidth) this.transform.rotation = UnityEngine.Quaternion.AngleAxis(UnityEngine.Time.deltaTime * this.rotationSpeed, this.transform.up * (movementWidth > 0.0f ? +1.0f : -1.0f)) * this.transform.rotation;
//     }
//   }
// }
