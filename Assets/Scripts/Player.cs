using PatchOdyssey;

/* … */
[UnityEngine.DisallowMultipleComponent]
[UnityEngine.RequireComponent(typeof(UnityEngine.BoxCollider))]
public sealed class Lasoo : UnityEngine.MonoBehaviour /* ->> “Lasoo” is intentionally misspelled */ {
  public const float DeployReach  = 17.5f;   // ->> Maximum reach
  public const float DeploySpeed  = 7.0f;    // ->> Units per second
  public const float RetractReach = 1.0f;    // ->> Minimum reach
  public const float RetractSpeed = 20.0f;   // ->> Units   per second
  public const float TurnSpeed    = 135.0f;  // ->> Degrees per second

  [ReadOnlyInInspector]                             private     UnityEngine.BoxCollider              _collider    = null!;
  [ReadOnlyInInspector]                             public      Entity?                              capture      = null;
  [ReadOnlyInInspector]                             public      UnityEngine.GameObject               coil         = null!; // ->> Must be set
  [ReadWriteInInspector]                            public  new ref readonly UnityEngine.BoxCollider collider     { get { this._collider = null == this._collider ? this.GetComponent<UnityEngine.BoxCollider>() : this._collider; this._collider = null == this._collider ? this.gameObject.AddComponent<UnityEngine.BoxCollider>() : this._collider; return ref this._collider; } }
  [ReadWriteInInspector]                            public      float                                deployReach  = Lasoo.DeployReach;
  [ReadWriteInInspector]                            public      float                                deploySpeed  = Lasoo.DeploySpeed;
  [ReadOnlyInInspector]                             internal    System.Func<bool>                    isDeploying  = static () => false; // --> bool*
  [ReadOnlyInInspector, UnityEngine.SerializeField] internal    UnityEngine.Vector3                  offset       = UnityEngine.Vector3.forward;
  [ReadOnlyInInspector]                             public      float                                reach        = Lasoo.RetractReach; // --> retractReach <= reach <= deployReach
  [ReadWriteInInspector]                            public      float                                retractReach = Lasoo.RetractReach;
  [ReadWriteInInspector]                            public      float                                retractSpeed = Lasoo.RetractSpeed;
  [ReadOnlyInInspector]                             public      UnityEngine.GameObject               rope         = null!; // ->> Must be set
  [ReadWriteInInspector]                            public      float                                turnSpeed    = Lasoo.TurnSpeed;
  [ReadOnlyInInspector]                             public      Entity                               user         = null!; // ->> Must be set

  /* … */
  private void Update() {
    UnityEngine.Bounds?   coilBounds    = null;
    UnityEngine.Vector3   coilSize      = UnityEngine.Vector3.one;
    UnityEngine.Vector3   reachSize     = UnityEngine.Vector3.one * (this.reach - this.retractReach);
    UnityEngine.Transform ropeTransform = this.rope.transform;
    UnityEngine.Transform userTransform = this.user.transform;

    // …
    for (System.Collections.Generic.Queue<UnityEngine.Transform> transforms = new(new[] {this.coil.transform /*, this.knot.transform */}); 0 != transforms.Count; ) {
      UnityEngine.Transform transform           = transforms.Dequeue();
      UnityEngine.Bounds?   transformMeshBounds = null;

      // …
      if (ropeTransform == transform)
      continue;

      if      (transform.GetComponent<UnityEngine.MeshFilter>         () is UnityEngine.MeshFilter          meshFilter          && null != meshFilter)          transformMeshBounds = meshFilter         .sharedMesh.bounds;
      else if (transform.GetComponent<UnityEngine.SkinnedMeshRenderer>() is UnityEngine.SkinnedMeshRenderer skinnedMeshRenderer && null != skinnedMeshRenderer) transformMeshBounds = skinnedMeshRenderer.sharedMesh.bounds;

      // …
      if (transformMeshBounds is not null)
      if (coilBounds is UnityEngine.Bounds bounds) {
        bounds.Encapsulate((UnityEngine.Vector3) transformMeshBounds?.center! - (UnityEngine.Vector3) transformMeshBounds?.extents!);
        bounds.Encapsulate((UnityEngine.Vector3) transformMeshBounds?.center! + (UnityEngine.Vector3) transformMeshBounds?.extents!);

        coilBounds = bounds;
      } else coilBounds = transformMeshBounds;

      foreach (UnityEngine.Transform subtransform in transform)
      transforms.Enqueue(subtransform);
    }

    coilSize                     = coilBounds?.size ?? coilSize;
    ropeTransform.localScale     = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, reachSize - UnityEngine.Vector3.one) + UnityEngine.Vector3.one;
    this.collider.size           = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, reachSize)                           + coilSize;
    this.collider.center         = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, this.collider.size * -0.5f)          + UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, coilSize);
    this.offset                  = UnityEngine.Quaternion.Euler(UnityEngine.Vector3.up * UnityEngine.Time.unscaledDeltaTime * this.turnSpeed) * this.offset;
    this.reach                   = UnityEngine.Mathf.Clamp(this.reach + (UnityEngine.Time.deltaTime * (!this.isDeploying() ? -this.retractSpeed : +this.deploySpeed)), this.retractReach, this.deployReach);
    this.transform.position      = userTransform.position + (this.offset * this.reach);
    this.transform.localRotation = UnityEngine.Quaternion.LookRotation(this.transform.localPosition - userTransform.localPosition);
    ropeTransform.LookAt(userTransform, UnityEngine.Vector3.up);
  }
}

[UnityEngine.RequireComponent(typeof(UnityEngine.SphereCollider))]
public sealed class Player : Entity /* ->> Source file must be named “Player” */ {
  [ReadOnlyInInspector]                                      private     UnityEngine.Camera?              _camera                     = null;
  [ReadOnlyInInspector]                                      private new ref readonly UnityEngine.Camera? camera                      { get { this._camera = null == this._camera ? UnityEngine.Camera.main : this._camera; /* --> base.camera */ return ref this._camera; } }
  [ReadWriteInInspector]                                     public  new UnityEngine.SphereCollider       collider                    => (UnityEngine.SphereCollider) base.collider;
  [ReadOnlyInInspector]                                      public      Lasoo?                           lasoo                       = null;
  [ReadWriteInInspector]                                     public      float                            lasooDeployReach            = Lasoo.DeployReach;
  [ReadWriteInInspector]                                     public      float                            lasooDeploySpeed            = Lasoo.DeploySpeed;
  [ReadOnlyInInspector]                                      public      bool                             lasooIsDeploying            = false;
  [ReadWriteInInspector, UnityEngine.Tooltip("Must be set")] public      UnityEngine.GameObject           lasooMeshPrefabrication     = null!;
  [ReadWriteInInspector]                                     public      float                            lasooRetractReach           = Lasoo.RetractReach;
  [ReadWriteInInspector]                                     public      float                            lasooRetractSpeed           = Lasoo.RetractSpeed;
  [ReadWriteInInspector, UnityEngine.Tooltip("Must be set")] public      UnityEngine.GameObject           lasooRopeMeshPrefabrication = null!;
  [ReadWriteInInspector]                                     public      float                            lasooTurnSpeed              = Lasoo.TurnSpeed;
  [ReadOnlyInInspector]                                      public      UnityEngine.Vector3              movementDirection           = UnityEngine.Vector3.zero;
  [ReadWriteInInspector]                                     public      Timeframe                        movementRestCooldown        = new(1.0);
  [ReadOnlyInInspector]                                      private     UnityEngine.Vector3              turnDirection               = UnityEngine.Vector3.zero;
  [ReadWriteInInspector]                                     public      float                            turnSpeed                   = 30.0f; // ->> Degrees per second

  /* … */
  private new void Awake() {
    this.movementDamping = 4.0f;
    base.Awake();

    // …
    this.collider.radius = 1.5f;
  }

  public void DeployLasoo() {
    Lasoo                  lasoo;
    UnityEngine.GameObject lasooCoil;
    UnityEngine.GameObject lasooRope;

    // …
    if (this.lasoo is not null)
    return;

    lasoo                                    = new UnityEngine.GameObject("Lasoo", typeof(Lasoo)).GetComponent<Lasoo>();
    lasooCoil                                = UnityEngine.Object.Instantiate(this.lasooMeshPrefabrication,     UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity, lasoo    .transform);
    lasooRope                                = UnityEngine.Object.Instantiate(this.lasooRopeMeshPrefabrication, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity, lasooCoil.transform);
    lasoo    .user                           = this;
    lasoo    .turnSpeed                      = this.lasooTurnSpeed;
    lasoo    .rope                           = lasooRope;
    lasoo    .rope.name                      = "Rope";
    lasoo    .retractSpeed                   = this.lasooRetractSpeed;
    lasoo    .retractReach                   = this.lasooRetractReach;
    lasoo    .reach                          = this.lasooRetractReach; // ->> Starts at minimum
    lasoo    .offset                         = this.transform.forward * lasoo.reach;
    lasoo    .isDeploying                    = () => this.lasooIsDeploying;
    lasoo    .deploySpeed                    = this.lasooDeploySpeed;
    lasoo    .deployReach                    = this.lasooDeployReach;
    lasoo    .collider.isTrigger             = true;
    lasoo    .collider.isTrigger             = true;
    lasoo    .collider.includeLayers         = (UnityEngine.LayerMask) ~0x0;
    lasoo    .collider.hasModifiableContacts = false;
    lasoo    .collider.excludeLayers         = (UnityEngine.LayerMask) 0x0;
    lasoo    .collider.enabled               = true;
    lasoo    .coil                           = lasooCoil;
    lasoo    .coil.name                      = "Knot";
    this     .lasoo                          = lasoo;

    lasoo.transform.SetParent(this.transform, false);
  }

  protected override void OnApplicationFocus(bool unblurred) {
    if (unblurred)
    return;

    // this.lasooIsDeploying = false;
    // if (this.lasoo is not null) { this.lasoo.reach = this.lasoo.retractReach; }

    // this.RetractLasoo();
  }

  public void RetractLasoo() {
    if (this.lasoo is null)
    return;

    UnityEngine.Object.Destroy(this.lasoo.gameObject);
    this.lasoo = null;
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
      this.lasooIsDeploying = !this.lasooIsDeploying ? (this.lasoo is not null ? this.lasoo.reach == this.lasoo.retractReach : true) : false;

    // … ->> Apply inputs
      // … ->> Turning
      if (UnityEngine.Vector3.zero != this.turnDirection)
      this.rigidBody.MoveRotation(UnityEngine.Quaternion.Slerp(transform.rotation, UnityEngine.Quaternion.LookRotation(this.turnDirection), UnityEngine.Time.deltaTime * this.turnSpeed));

      // … ->> Moving
      if (UnityEngine.Vector3.zero != this.movementDirection) {
        this.movementRestCooldown.Reset   ();
        this.rigidBody           .AddForce(this.movementDirection, UnityEngine.ForceMode.Impulse);

        this.turnDirection = this.movementDirection;
      }

      else if (this.movementRestCooldown.isElapsed && UnityEngine.Vector3.one.sqrMagnitude > this.rigidBody.linearVelocity.sqrMagnitude) {
        this.rigidBody.angularVelocity = UnityEngine.Vector3.zero;
        this.turnDirection             = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, ((this.camera?.transform.position ?? (UnityEngine.Vector3.back + transform.position)) - transform.position).normalized);
      }

      // … ->> Lassoing
      if (this.lasooIsDeploying || (this.lasoo is not null && this.lasoo.reach != this.lasoo.retractReach)) this.DeployLasoo(); else this.RetractLasoo();
  }
}
