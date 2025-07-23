using PatchOdyssey;

/* … */
public sealed class Player : Entity {
  [ReadOnlyInInspector]                                      private     UnityEngine.Camera?              _camera                     = null;
  [ReadOnlyInInspector]                                      private new ref readonly UnityEngine.Camera? camera                      { get { this._camera ??= UnityEngine.Camera.main; return ref this._camera; } }
  [ReadOnlyInInspector]                                      public      bool                             isLassoing                  = false;
  [ReadOnlyInInspector]                                      public      UnityEngine.GameObject?          lasoo                       = null; // ->> “lasoo” is intentionally misspelled
  [ReadOnlyInInspector]                                      private     UnityEngine.Vector3              lasooLocalPosition          = UnityEngine.Vector3.zero;
  [ReadWriteInInspector, UnityEngine.Tooltip("Must be set")] public      UnityEngine.GameObject           lasooMeshPrefabrication     = null!;
  [ReadOnlyInInspector]                                      public      float                            lasooReach                  = 1.0f;
  [ReadWriteInInspector]                                     public      float                            lasooReturnReach            = 1.0f; // ->> Lasoo minimum reach
  [ReadWriteInInspector]                                     public      float                            lasooReturnSpeed            = 3.0f; // ->> Units per second
  [ReadOnlyInInspector]                                      public      UnityEngine.GameObject?          lasooRope                   = null;
  [ReadWriteInInspector, UnityEngine.Tooltip("Must be set")] public      UnityEngine.GameObject           lasooRopeMeshPrefabrication = null!;
  [ReadWriteInInspector]                                     public      float                            lasooTurnSpeed              = 90.0f; // ->> Degrees per second
  [ReadWriteInInspector]                                     public      float                            lasooUseReach               = 3.0f;  // ->> Lasoo maximum reach
  [ReadWriteInInspector]                                     public      float                            lasooUseSpeed               = 1.0f;  // ->> Units per second
  [ReadOnlyInInspector]                                      public      UnityEngine.Vector3              movementDirection           = UnityEngine.Vector3.zero;
  [ReadWriteInInspector]                                     public      Timeframe                        restTime                    = new Timeframe(1.00);
  [ReadOnlyInInspector]                                      private     UnityEngine.Vector3              turnDirection               = UnityEngine.Vector3.zero;
  [ReadWriteInInspector]                                     public      float                            turnSpeed                   = 30.0f; // ->> Degrees per second

  /* … */
  private new void Awake() {
    base.Awake();
    this.rigidBody.linearDamping = 4.0f;
  }

  private void OnApplicationFocus(bool unblurred) {
    if (unblurred)
    return;

    // this.isLassoing = false;
    // this.lasooReach = this.lasooReturnReach;

    // this.ReturnLasoo();
  }

  private void OnApplicationQuit() => this.OnApplicationFocus(false);

  public void ReturnLasoo() {
    if (this.lasoo is null)
    return;

    UnityEngine.Object.Destroy(this.lasoo);
    this.lasoo     = null;
    this.lasooRope = null;
  }

  private void Update() {
    UnityEngine.Transform transform  = this.transform;

    // … ->> Acknowledge inputs
      // … ->> Moving
      this.movementDirection  = UnityEngine.Vector3.zero;
      this.movementDirection += Game.Keyboard.aKey.isPressed || Game.Keyboard.aKey.wasPressedThisFrame || Game.Keyboard.leftArrowKey .isPressed || Game.Keyboard.leftArrowKey .wasPressedThisFrame || UnityEngine.Input.GetKey(UnityEngine.KeyCode.A) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftArrow)  || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.A) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.LeftArrow)  ? UnityEngine.Vector3.left    : UnityEngine.Vector3.zero;
      this.movementDirection += Game.Keyboard.dKey.isPressed || Game.Keyboard.dKey.wasPressedThisFrame || Game.Keyboard.rightArrowKey.isPressed || Game.Keyboard.rightArrowKey.wasPressedThisFrame || UnityEngine.Input.GetKey(UnityEngine.KeyCode.D) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightArrow) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.D) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.RightArrow) ? UnityEngine.Vector3.right   : UnityEngine.Vector3.zero;
      this.movementDirection += Game.Keyboard.sKey.isPressed || Game.Keyboard.sKey.wasPressedThisFrame || Game.Keyboard.downArrowKey .isPressed || Game.Keyboard.downArrowKey .wasPressedThisFrame || UnityEngine.Input.GetKey(UnityEngine.KeyCode.S) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.DownArrow)  || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.S) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.DownArrow)  ? UnityEngine.Vector3.back    : UnityEngine.Vector3.zero;
      this.movementDirection += Game.Keyboard.wKey.isPressed || Game.Keyboard.wKey.wasPressedThisFrame || Game.Keyboard.upArrowKey   .isPressed || Game.Keyboard.upArrowKey   .wasPressedThisFrame || UnityEngine.Input.GetKey(UnityEngine.KeyCode.W) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.UpArrow)    || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.W) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.UpArrow)    ? UnityEngine.Vector3.forward : UnityEngine.Vector3.zero;

      // … ->> Lassoing
      if (Game.Keyboard.enterKey.wasPressedThisFrame || Game.Keyboard.tabKey.wasPressedThisFrame || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Return) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Tab))
      this.isLassoing = !this.isLassoing ? this.lasooReach == this.lasooReturnReach : false;

    // … ->> Apply inputs
      // … ->> Turning
      if (UnityEngine.Vector3.zero != this.turnDirection)
      this.rigidBody.MoveRotation(UnityEngine.Quaternion.Slerp(transform.rotation, UnityEngine.Quaternion.LookRotation(this.turnDirection), UnityEngine.Time.deltaTime * this.turnSpeed));

      // … ->> Moving
      if (UnityEngine.Vector3.zero != this.movementDirection) {
        this.restTime .Reset   ();
        this.rigidBody.AddForce(this.movementDirection, UnityEngine.ForceMode.Impulse);

        this.turnDirection = this.movementDirection;
      }

      else if (this.restTime.isElapsed && UnityEngine.Vector3.one.sqrMagnitude > this.rigidBody.linearVelocity.sqrMagnitude) {
        this.rigidBody.angularVelocity = UnityEngine.Vector3.zero;
        this.turnDirection             = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, ((this.camera?.transform.position ?? (UnityEngine.Vector3.back + transform.position)) - transform.position).normalized);
      }

      // … ->> Lassoing
      this.lasooReach = UnityEngine.Mathf.Clamp(this.lasooReach + (UnityEngine.Time.deltaTime * (this.isLassoing ? +this.lasooUseSpeed : -this.lasooReturnSpeed)), this.lasooReturnReach, this.lasooUseReach);

      if (this.lasooReach != this.lasooReturnReach) {
        UnityEngine.Transform lasooTransform;

        // …
        this.UseLasoo();

        this.lasooRope!.transform.position   = transform.position;
        this.lasooRope!.transform.localScale = UnityEngine.Vector3.one + (UnityEngine.Vector3.forward * (this.lasooReach - this.lasooReturnReach - 1.0f));
        this.lasooLocalPosition              = UnityEngine.Quaternion.Euler(UnityEngine.Vector3.up * UnityEngine.Time.unscaledDeltaTime * this.lasooTurnSpeed) * this.lasooLocalPosition;
        lasooTransform                       = this.lasoo!.transform;
        lasooTransform.position              = transform.position + (this.lasooLocalPosition * this.lasooReach);
        lasooTransform.localRotation         = UnityEngine.Quaternion.LookRotation(lasooTransform.localPosition - transform.localPosition);
      } else this.ReturnLasoo();
  }

  public void UseLasoo() {
    UnityEngine.Transform lasooTransform;

    // …
    if (this.lasoo is not null)
    return;

    this.lasooReach         = this.lasooReturnReach;
    this.lasooLocalPosition = transform.forward * this.lasooReach;
    this.lasoo              = new UnityEngine.GameObject("🪢");
    lasooTransform          = this.lasoo.transform;
    this.lasooRope          = UnityEngine.Object.Instantiate(this.lasooRopeMeshPrefabrication, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity, UnityEngine.Object.Instantiate(this.lasooMeshPrefabrication, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity, lasooTransform).transform);

    lasooTransform.SetParent(transform, false);
  }
}
