using PatchOdyssey;

/* … */
[UnityEngine.DefaultExecutionOrder(4)]
[UnityEngine.DisallowMultipleComponent]
[UnityEngine.RequireComponent(typeof(UnityEngine.BoxCollider))]
public sealed class Lasoo : UnityEngine.MonoBehaviour /* ->> “Lasoo” is intentionally misspelled */ {
  public const float DeployReach  = 17.5f;  // ->> Maximum reach
  public const float DeploySpeed  = 7.0f;   // ->> Units per second
  public const float RetractReach = 1.0f;   // ->> Minimum reach
  public const float RetractSpeed = 20.0f;  // ->> Units   per second
  public const float TurnSpeed    = 135.0f; // ->> Degrees per second

  [ReadOnlyInInspector]                             private          UnityEngine.BoxCollider                               _collider      = null!;
  [ReadOnlyInInspector]                             public           Entity?                                               capture        = null;
  [ReadOnlyInInspector]                             public           UnityEngine.GameObject                                coil           = null!; // ->> Must be set
  [ReadWriteInInspector]                            public  new      ref readonly UnityEngine.BoxCollider                  collider       { get { this._collider = null == this._collider ? this.GetComponent<UnityEngine.BoxCollider>() : this._collider; this._collider = null == this._collider ? this.gameObject.AddComponent<UnityEngine.BoxCollider>() : this._collider; return ref this._collider; } }
  [ReadWriteInInspector]                            public           float                                                 deployReach    =  Lasoo.DeployReach;
  [ReadWriteInInspector]                            public           float                                                 deploySpeed    =  Lasoo.DeploySpeed;
  [ReadOnlyInInspector]                             internal         System.Func<bool>                                     isDeploying    =  static () => false; // --> bool*
  [ReadOnlyInInspector]                             public           float                                                 reach          =  Lasoo.RetractReach; // --> retractReach <= reach <= deployReach
  [ReadOnlyInInspector, UnityEngine.SerializeField] internal         UnityEngine.Vector3                                   reachDirection =  UnityEngine.Vector3.forward;
  [ReadOnlyInInspector]                             public           float                                                 reachProgress  => UnityEngine.Mathf.Clamp01((this.reach - this.retractReach) / (this.deployReach - this.retractReach));
  [ReadWriteInInspector]                            public           float                                                 retractReach   =  Lasoo.RetractReach;
  [ReadWriteInInspector]                            public           float                                                 retractSpeed   =  Lasoo.RetractSpeed;
  [ReadOnlyInInspector]                             public           UnityEngine.GameObject                                rope           =  null!; // ->> Must be set
  [ReadOnlyInInspector]                             private readonly System.Collections.Generic.List<UnityEngine.Color>    ropeColors     =  new();
  [ReadOnlyInInspector]                             private readonly System.Collections.Generic.List<UnityEngine.Material> ropeMaterials  =  new();
  [ReadWriteInInspector]                            public           float                                                 turnSpeed      =  Lasoo.TurnSpeed;
  [ReadOnlyInInspector]                             public           Entity                                                user           =  null!; // ->> Must be set

  /* … ->> Solely responsible for revolving, scaling, coloring, and attaching to its `capture` */
  private void OnDestroy() {
    foreach (UnityEngine.Material ropeMaterial in this.ropeMaterials)
      UnityEngine.Object.Destroy(ropeMaterial);

    this.ropeColors   .Clear();
    this.ropeMaterials.Clear();
  }

  private void Start() {
    UnityEngine.Transform ropeTransform = this.rope.transform;

    // …
    this.collider     .excludeLayers         = (UnityEngine.LayerMask) 0x0;
    this.collider     .hasModifiableContacts = false;
    this.collider     .includeLayers         = (UnityEngine.LayerMask) ~0x0;
    this.collider     .isTrigger             = true;
    this.ropeColors   .Capacity              = ropeTransform.hierarchyCount;
    this.ropeMaterials.Capacity              = ropeTransform.hierarchyCount;

    ropeTransform.ForEach(transform => {
      if (transform.GetComponent<UnityEngine.Renderer>() is UnityEngine.Renderer renderer && null != renderer) {
        this.ropeColors   .Add(renderer.sharedMaterial.color);
        this.ropeMaterials.Add(renderer.material = renderer.material);
      }
    });
  }

  private void Update() {
    UnityEngine.Transform captureTransform = this.capture?.transform!;
    UnityEngine.Vector3   reachSize        = UnityEngine.Vector3.one * (this.reach - this.retractReach);
    UnityEngine.Renderer  ropeRenderer     = this.rope.GetComponent<UnityEngine.Renderer>();
    UnityEngine.Transform ropeTransform    = this.rope.transform;
    UnityEngine.Transform userTransform    = this.user.transform;

    // …
    if (Game.IsPaused)
    return;

    // …
    ropeTransform.localScale = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, reachSize - UnityEngine.Vector3.one) + UnityEngine.Vector3.one;

    for (int index = this.ropeMaterials.Count; 0 != index--; )
    this.ropeMaterials[index].color = UnityEngine.Color.LerpUnclamped(this.ropeColors[index], UnityEngine.Color.red, this.reachProgress * 0.675f);

    if (null != this.capture) {
      this.reach              = UnityEngine.Vector3.Distance(captureTransform.position, userTransform.position);
      this.reachDirection     = (captureTransform.position - userTransform.position).normalized;
      this.transform.position = captureTransform.position;
      this.transform.rotation = UnityEngine.Quaternion.LookRotation(this.transform.position - userTransform.position, UnityEngine.Vector3.up);
    }

    else {
      UnityEngine.Bounds? coilBounds = null;
      UnityEngine.Vector3 coilSize   = UnityEngine.Vector3.one;

      // …
      this.coil.transform.ForEach(transform => {
        UnityEngine.Bounds meshBounds;

        // …
        if (transform == ropeTransform)
        return false;

        if      (transform.GetComponent<UnityEngine.MeshFilter>         () is UnityEngine.MeshFilter          meshFilter          && null != meshFilter)          meshBounds = meshFilter         .sharedMesh.bounds;
        else if (transform.GetComponent<UnityEngine.SkinnedMeshRenderer>() is UnityEngine.SkinnedMeshRenderer skinnedMeshRenderer && null != skinnedMeshRenderer) meshBounds = skinnedMeshRenderer.sharedMesh.bounds;
        else    return true;

        // …
        if (coilBounds is UnityEngine.Bounds bounds) {
          bounds.Encapsulate(meshBounds.center - meshBounds.extents);
          bounds.Encapsulate(meshBounds.center + meshBounds.extents);

          coilBounds = bounds;
        } else coilBounds = meshBounds;

        // …
        return true;
      });

      coilSize                     = coilBounds?.size ?? coilSize;
      this.collider.size           = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, reachSize)                  + coilSize;
      this.collider.center         = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, this.collider.size * -0.5f) + UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, coilSize);
      this.reach                   = UnityEngine.Mathf.Clamp(this.reach + (UnityEngine.Time.unscaledDeltaTime * (!this.isDeploying() ? -this.retractSpeed : +this.deploySpeed)), this.retractReach, this.deployReach);
      this.reachDirection          = UnityEngine.Quaternion.Euler(UnityEngine.Vector3.up * UnityEngine.Time.unscaledDeltaTime * this.turnSpeed) * this.reachDirection;
      this.transform.position      = userTransform.position + (this.reachDirection * this.reach);
      this.transform.localRotation = UnityEngine.Quaternion.LookRotation(this.transform.localPosition - userTransform.localPosition, UnityEngine.Vector3.up);
    }

    ropeTransform.LookAt(userTransform, UnityEngine.Vector3.up);
  }
}

[UnityEngine.DefaultExecutionOrder(3)]
[UnityEngine.RequireComponent(typeof(UnityEngine.SphereCollider))]
public sealed class Player : Tamer /* ->> Source file must be named “Player” */ {
  public enum LasooCaptureProgress : byte { Initiating, Capturing, Finishing }

  [System.Serializable]
  public struct LasooInfo {
    [ReadOnlyInInspector]                             internal UnityEngine.Vector3                                                       captureDirection;
    [ReadOnlyInInspector]                             internal (UnityEngine.MeshFilter?   completed, UnityEngine.MeshFilter?   progress) captureIndicator;
    [ReadOnlyInInspector]                             internal (UnityEngine.Mesh?         completed, UnityEngine.Mesh?         progress) captureIndicatorMesh;
    [ReadOnlyInInspector]                             internal (UnityEngine.MeshRenderer? completed, UnityEngine.MeshRenderer? progress) captureIndicatorRenderer;
    [ReadOnlyInInspector, UnityEngine.SerializeField] internal Player.LasooCaptureProgress                                               captureProgress;
    [ReadOnlyInInspector]                             internal UnityEngine.Vector3                                                       captureProgressDirection;
    [ReadOnlyInInspector, UnityEngine.SerializeField] internal Entity.TurnDirection                                                      captureProgressTurn;
    [ReadOnlyInInspector]                             internal uint                                                                      captureProgressTurnCount;
    [ReadWriteInInspector]                            public   float                                                                     deployReach;
    [ReadWriteInInspector]                            public   float                                                                     deploySpeed;
    [ReadWriteInInspector]                            public   float                                                                     retractReach;
    [ReadWriteInInspector]                            public   float                                                                     retractSpeed;
    [ReadWriteInInspector]                            public   float                                                                     turnSpeed;
  }

  /* … */
  public  static          bool                  AnyInput                           = false;
  private static readonly float                 LasooCaptureProgressThreshold      = (UnityEngine.Vector3.one * 0.25f).sqrMagnitude;
  private const           uint                  LasooCaptureProgressPrecision      = 10u;  // --> Player.LasooCaptureProgressPrecision >= 2
  private const           byte                  LasooCaptureIndicatorPrecision     = 20;   // ->> Number of segments
  private const           float                 LasooCaptureIndicatorOuterRadius   = 1.5f; // --> Player.LasooCaptureIndicatorOuterRadius > Player.LasooCaptureIndicatorInnerRadius
  private static readonly UnityEngine.Vector3[] LasooCaptureIndicatorMeshVertices  = new UnityEngine.Vector3[(Player.LasooCaptureIndicatorPrecision * 2u)];
  private static readonly int                [] LasooCaptureIndicatorMeshTriangles = new int                [(Player.LasooCaptureIndicatorPrecision - 1u) * 6u];
  private const           float                 LasooCaptureIndicatorInnerRadius   = 1.0f; // --> Player.LasooCaptureIndicatorInnerRadius < Player.LasooCaptureIndicatorOuterRadius
  private static readonly float                 LasooCaptureProgressAngle          = 360.0f / Player.LasooCaptureProgressPrecision;

  [UnityEngine.Header("Player")]
  [ReadOnlyInInspector]                             public  bool                isInputing         = false; // ->> Only significant actions count
  [ReadOnlyInInspector]                             public  bool                isLassoing         = false;
  [ReadWriteInInspector]                            public  Player.LasooInfo    lasoo              = new() {captureDirection = UnityEngine.Vector3.zero, captureIndicator = (null, null), captureIndicatorMesh = (null, null), captureIndicatorRenderer = (null, null), captureProgress = Player.LasooCaptureProgress.Initiating, captureProgressDirection = UnityEngine.Vector3.zero, captureProgressTurn = Entity.TurnDirection.Clockwise, captureProgressTurnCount = 0u, deployReach = Lasoo.DeployReach, deploySpeed = Lasoo.DeploySpeed, retractReach = Lasoo.RetractReach, retractSpeed = Lasoo.RetractSpeed, turnSpeed = Lasoo.TurnSpeed};
  [ReadOnlyInInspector]                             public  Lasoo?              lasooing           = null;
  [ReadOnlyInInspector, UnityEngine.SerializeField] private UnityEngine.Vector3 mainCameraDistance = UnityEngine.Vector3.zero;
  [ReadWriteInInspector]                            public  Timeframe           releaseWindow      = new(1.0);

  /* … */
  protected override void Awake() {
    base.Awake();

    // …
    this.collider.isTrigger  = false;
    this.followAutomatically = false;
    this.mainCameraDistance  = null != this.mainCamera ? this.mainCamera.transform.position - this.transform.position : UnityEngine.Vector3.zero;
    this.shootAutomatically  = false;
  }

  protected override void Capture(Entity entity) {
    base.Capture   (entity);
    this.ResetLasoo();
  }

  public bool DeployLasoo() {
    UnityEngine.Transform transform = this.transform;

    // …
    if (0 == this.followers.Count) {
      if (this.lasooing is null) {
        Lasoo                  lasoo     = new UnityEngine.GameObject("Lasoo", typeof(Lasoo)).GetComponent<Lasoo>();
        UnityEngine.GameObject lasooCoil = (UnityEngine.GameObject) UnityEngine.Object.Instantiate(Assets.main.lasoo.meshPrefabrication,     UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity, lasoo    .transform);
        UnityEngine.GameObject lasooRope = (UnityEngine.GameObject) UnityEngine.Object.Instantiate(Assets.main.lasoo.ropeMeshPrefabrication, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity, lasooCoil.transform);

        lasoo.user                                                              = this;
        lasoo.turnSpeed                                                         = this.lasoo.turnSpeed;
        lasoo.rope                                                              = lasooRope;
        lasoo.rope.name                                                         = "Rope";
        lasoo.retractSpeed                                                      = this.lasoo.retractSpeed;
        lasoo.retractReach                                                      = this.lasoo.retractReach;
        lasoo.reachDirection                                                    = UnityEngine.Quaternion.Euler(UnityEngine.Vector3.up * lasoo.turnSpeed * -3.0f) * transform.forward;
        lasoo.reach                                                             = this.lasoo.retractReach; // ->> Starts at minimum
        lasoo.isDeploying                                                       = () => this.isLassoing;
        lasoo.deploySpeed                                                       = this.lasoo.deploySpeed;
        lasoo.deployReach                                                       = this.lasoo.deployReach;
        lasoo.coil                                                              = lasooCoil;
        lasoo.coil.name                                                         = "Knot";
        this.lasooing                                                           = lasoo;
        this.lasoo.captureIndicatorMesh    .completed                           = new() { name = "playerLasooCaptureIndicatorMesh" };
        this.lasoo.captureIndicatorMesh    .progress                            = new() { name = "playerLasooCaptureProgressIndicatorMesh" };
        this.lasoo.captureIndicator        .completed                           = new UnityEngine.GameObject("Indicator", typeof(UnityEngine.MeshFilter), typeof(UnityEngine.MeshRenderer)).GetComponent<UnityEngine.MeshFilter>();
        this.lasoo.captureIndicator        .completed.sharedMesh                = this.lasoo.captureIndicatorMesh.completed;
        this.lasoo.captureIndicator        .progress                            = new UnityEngine.GameObject("Progress",  typeof(UnityEngine.MeshFilter), typeof(UnityEngine.MeshRenderer)).GetComponent<UnityEngine.MeshFilter>();
        this.lasoo.captureIndicator        .progress.sharedMesh                 = this.lasoo.captureIndicatorMesh.progress;
        this.lasoo.captureIndicator        .progress.transform.localPosition   += UnityEngine.Vector3.up * Game.VectorEpsilon; // ->> Avoid Z-fighting
        this.lasoo.captureIndicatorRenderer.completed                           = this.lasoo.captureIndicator.completed.GetComponent<UnityEngine.MeshRenderer>();
        this.lasoo.captureIndicatorRenderer.completed.allowOcclusionWhenDynamic = false;
        this.lasoo.captureIndicatorRenderer.progress                            = this.lasoo.captureIndicator.progress .GetComponent<UnityEngine.MeshRenderer>();
        this.lasoo.captureIndicatorRenderer.progress.allowOcclusionWhenDynamic  = false;

        if (Assets.main.lasoo.captureIndicatorMaterial is not null) {
          for (int index = ((System.Runtime.CompilerServices.ITuple) this.lasoo.captureIndicator).Length; 0 != index--; ) {
            UnityEngine.MeshRenderer lasooCaptureIndicatorRenderer = (UnityEngine.MeshRenderer) ((System.Runtime.CompilerServices.ITuple) this.lasoo.captureIndicatorRenderer)[index];

            // …
            lasooCaptureIndicatorRenderer.receiveShadows    = false;
            lasooCaptureIndicatorRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lasooCaptureIndicatorRenderer.sharedMaterial    = Assets.main.lasoo.captureIndicatorMaterial;
          }

          this.lasoo.captureIndicatorRenderer.completed.material.color = UnityEngine.Color.LerpUnclamped(this.lasoo.captureIndicatorRenderer.completed.material.color, UnityEngine.Color.black, 0.5f);
        }

        this.lasoo.captureIndicatorMesh.completed.MarkDynamic();
        this.lasoo.captureIndicatorMesh.progress .MarkDynamic();
        this.lasoo.captureIndicator.completed.transform.SetParent(lasoo.transform,                                 false);
        this.lasoo.captureIndicator.progress .transform.SetParent(this.lasoo.captureIndicator.completed.transform, false);
        lasoo.transform                                .SetParent(transform,                                       false);
      }

      return true;
    }

    return false;
  }

  protected override void OnApplicationFocus(bool focused) {
    if (!focused) {
      if (this.lasooing is not null) // ->> Instant retraction
        this.lasooing.reach = this.lasooing.retractReach;

      this.RetractLasoo();
    }
  }

  protected override void OnDestroy() {
    base.OnDestroy();
    this.RetractLasoo();
  }

  public void ResetLasoo() {
    this.isLassoing                     = false;
    this.lasoo.captureProgress          = Player.LasooCaptureProgress.Initiating;
    this.lasoo.captureProgressTurnCount = 0u;

    if (this.lasooing is not null) {
      if (this.lasooing.capture is Monster monster && null != monster)
        monster.wrestling = null;

      this.lasooing.capture = null;
    }
  }

  public void RetractLasoo() {
    if (this.lasooing is null)
    return;

    // …
    this.ResetLasoo();
    UnityEngine.Object.Destroy(this.lasoo.captureIndicatorMesh.progress);
    UnityEngine.Object.Destroy(this.lasoo.captureIndicatorMesh.completed);
    UnityEngine.Object.Destroy(this.lasooing.gameObject);

    this.lasooing = null;
  }

  private void Start() => this.followAutomatically = false;

  protected override void Update() {
    UnityEngine.Transform transform = this.transform;

    // …
    base.Update();

    if (Game.IsPaused || this.isDefeated)
    return;

    // … ->> Input
    Player.AnyInput = Player.AnyInput || this.isInputing;
    this.isInputing = false;
      // … ->> Moving
      this.followAutomatically = false;
      this.movement.direction  = UnityEngine.Vector3.zero;
      this.movement.direction += Game.Keyboard.aKey.isPressed || Game.Keyboard.aKey.wasPressedThisFrame || Game.Keyboard.leftArrowKey .isPressed || Game.Keyboard.leftArrowKey .wasPressedThisFrame || UnityEngine.Input.GetKey(UnityEngine.KeyCode.A) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftArrow)  || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.A) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.LeftArrow)  ? UnityEngine.Vector3.left    : UnityEngine.Vector3.zero;
      this.movement.direction += Game.Keyboard.dKey.isPressed || Game.Keyboard.dKey.wasPressedThisFrame || Game.Keyboard.rightArrowKey.isPressed || Game.Keyboard.rightArrowKey.wasPressedThisFrame || UnityEngine.Input.GetKey(UnityEngine.KeyCode.D) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightArrow) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.D) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.RightArrow) ? UnityEngine.Vector3.right   : UnityEngine.Vector3.zero;
      this.movement.direction += Game.Keyboard.sKey.isPressed || Game.Keyboard.sKey.wasPressedThisFrame || Game.Keyboard.downArrowKey .isPressed || Game.Keyboard.downArrowKey .wasPressedThisFrame || UnityEngine.Input.GetKey(UnityEngine.KeyCode.S) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.DownArrow)  || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.S) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.DownArrow)  ? UnityEngine.Vector3.back    : UnityEngine.Vector3.zero;
      this.movement.direction += Game.Keyboard.wKey.isPressed || Game.Keyboard.wKey.wasPressedThisFrame || Game.Keyboard.upArrowKey   .isPressed || Game.Keyboard.upArrowKey   .wasPressedThisFrame || UnityEngine.Input.GetKey(UnityEngine.KeyCode.W) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.UpArrow)    || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.W) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.UpArrow)    ? UnityEngine.Vector3.forward : UnityEngine.Vector3.zero;
      this.isInputing          = UnityEngine.Vector3.zero != this.movement.direction;

      // … ->> Lassoing
      if (Game.Keyboard.enterKey.wasPressedThisFrame || Game.Keyboard.tabKey.wasPressedThisFrame || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Return) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Tab)) {
        if (this.isLassoing) this.ResetLasoo();
        else { this.isInputing = true; this.isLassoing = 0 == this.followers.Count && (this.lasooing is null || this.lasooing.reach == this.lasooing.retractReach); }
      }

      // … ->> Releasing
      if (Game.Keyboard.shiftKey.wasReleasedThisFrame || UnityEngine.Input.GetKeyUp(UnityEngine.KeyCode.LeftShift) || UnityEngine.Input.GetKeyUp(UnityEngine.KeyCode.RightShift)) {
        if (!this.releaseWindow.isElapsed) {
          this.ResetLasoo();

          if (0 != this.followers.Count) {
            this.isInputing = true;
            this.Release(this.followers[0]);
          }
        }

        this.releaseWindow.Reset();
      }

      // … ->> Shooting
      if (Game.Keyboard.spaceKey.wasPressedThisFrame || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space)) {
        if (null != this.Shoot())
        this.isInputing = true;
      }

    // … ->> Application
      // … ->> Moving/ Turning
      this.movement.speedFactor = this.lasooing is not null ? ((1.0f - this.lasooing.reachProgress) * 0.5f) + 0.5f : 1.0f;

      if (this.isInputing || UnityEngine.Vector3.zero != this.movement.direction)
        this.movement.restTimer.Reset();

      else if (this.movement.restTimer.isElapsed && Entity.MovementVelocityThreshold.sqrMagnitude > this.rigidBody.linearVelocity.sqrMagnitude) {
        this.rigidBody.angularVelocity = UnityEngine.Vector3.zero;
        this.rigidBody.linearVelocity  = UnityEngine.Vector3.zero;
        this.turn.direction            = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, ((this.mainCamera?.transform.position ?? (UnityEngine.Vector3.back + transform.position)) - transform.position).normalized);
      }

      // … ->> Lassoing
      if (!this.isLassoing && (this.lasooing is null || this.lasooing.reach == this.lasooing.retractReach))
        this.RetractLasoo();

      else if (this.DeployLasoo()) {
        (float completed, float progress) lasooCaptureAngles = (360.0f, 0.0f); // ->> in Degrees

        // … ->> Capturing
        if (this.lasooing!.capture is not null) {
          switch (this.lasoo.captureProgress) {
            case Player.LasooCaptureProgress.Capturing: {
              foreach (ref readonly Entity.TurnDirection direction in (System.ReadOnlySpan<Entity.TurnDirection>) stackalloc[] {Entity.TurnDirection.Anticlockwise, Entity.TurnDirection.Clockwise}) {
                this.lasoo.captureProgressTurnCount  = 1u;
                this.lasoo.captureProgressTurn       = direction;
                this.lasoo.captureProgressDirection  = UnityEngine.Quaternion.AngleAxis(Player.LasooCaptureProgressAngle, Entity.TurnDirectionToVector3(this.lasoo.captureProgressTurn, UnityEngine.Vector3.up)) * this.lasoo.captureDirection;

                if (Player.LasooCaptureProgressThreshold > (this.lasoo.captureProgressDirection - this.lasooing!.reachDirection).sqrMagnitude) {
                  this.lasoo.captureProgress          = Player.LasooCaptureProgress.Finishing;
                  this.lasoo.captureProgressDirection = UnityEngine.Quaternion.AngleAxis(Player.LasooCaptureProgressAngle, Entity.TurnDirectionToVector3(this.lasoo.captureProgressTurn, UnityEngine.Vector3.up)) * this.lasoo.captureProgressDirection;

                  break;
                }

                #if DEBUG || DEVELOPMENT_BUILD
                  UnityEngine.Debug.DrawRay(UnityEngine.Vector3.zero, this.lasoo.captureProgressDirection * Game.SceneSize, UnityEngine.Color.green, 0.0f, false);
                #endif
              }

              lasooCaptureAngles.progress = UnityEngine.Vector3.Angle(this.lasoo.captureDirection, this.lasooing!.reachDirection);

              #if DEBUG || DEVELOPMENT_BUILD
                UnityEngine.Debug.DrawRay(UnityEngine.Vector3.zero, this.lasoo.captureDirection   * Game.SceneSize, UnityEngine.Color.white, 0.0f, false);
                UnityEngine.Debug.DrawRay(UnityEngine.Vector3.zero, this.lasooing!.reachDirection * Game.SceneSize, UnityEngine.Color.red,   0.0f, false);
              #endif
            } break;

            case Player.LasooCaptureProgress.Initiating: {
              this.lasoo.captureDirection = (this.lasooing!.capture.transform.position - this.transform.position).normalized;
              this.lasoo.captureProgress  = Player.LasooCaptureProgress.Capturing;

              lasooCaptureAngles.progress = 0.0f;

              #if DEBUG || DEVELOPMENT_BUILD
                UnityEngine.Debug.DrawRay(UnityEngine.Vector3.zero, this.lasoo.captureDirection   * Game.SceneSize, UnityEngine.Color.white, 0.0f, false);
                UnityEngine.Debug.DrawRay(UnityEngine.Vector3.zero, this.lasooing!.reachDirection * Game.SceneSize, UnityEngine.Color.red,   0.0f, false);
              #endif
            } break;

            case Player.LasooCaptureProgress.Finishing: {
              if (Player.LasooCaptureProgressPrecision - this.lasoo.captureProgressTurnCount == 1u)
                this.Capture(this.lasooing!.capture);

              else if (Player.LasooCaptureProgressThreshold > (this.lasoo.captureProgressDirection - this.lasooing!.reachDirection).sqrMagnitude) {
                this.lasoo.captureProgressDirection  = UnityEngine.Quaternion.AngleAxis(Player.LasooCaptureProgressAngle, Entity.TurnDirectionToVector3(this.lasoo.captureProgressTurn, UnityEngine.Vector3.up)) * this.lasoo.captureProgressDirection;
                this.lasoo.captureProgressTurnCount += 1u;
              }

              else if (Player.LasooCaptureProgressThreshold > (this.lasoo.captureDirection - this.lasooing!.reachDirection).sqrMagnitude) {
                this.lasoo.captureProgress          = Player.LasooCaptureProgress.Capturing;
                this.lasoo.captureProgressTurnCount = 0u;
              }

              lasooCaptureAngles.progress  = UnityEngine.Vector3.SignedAngle(this.lasoo.captureDirection, this.lasooing!.reachDirection, UnityEngine.Vector3.up) / 360.0f;
              lasooCaptureAngles.progress  = lasooCaptureAngles.progress < +0.0f                                  ? 1.0f + lasooCaptureAngles.progress : lasooCaptureAngles.progress;
              lasooCaptureAngles.progress  = Entity.TurnDirection.Anticlockwise == this.lasoo.captureProgressTurn ? 1.0f - lasooCaptureAngles.progress : lasooCaptureAngles.progress;
              lasooCaptureAngles.progress *= 360.0f;

              #if DEBUG || DEVELOPMENT_BUILD
                UnityEngine.Debug.DrawRay(UnityEngine.Vector3.zero, this.lasoo.captureDirection         * Game.SceneSize, UnityEngine.Color.white, 0.0f, false);
                UnityEngine.Debug.DrawRay(UnityEngine.Vector3.zero, this.lasoo.captureProgressDirection * Game.SceneSize, UnityEngine.Color.blue,  0.0f, false);
                UnityEngine.Debug.DrawRay(UnityEngine.Vector3.zero, this.lasooing!.reachDirection       * Game.SceneSize, UnityEngine.Color.red,   0.0f, false);
              #endif
            } break;
          }
        }

        // … ->> Indicating (capture)
        if (Player.LasooCaptureProgress.Capturing == this.lasoo.captureProgress || Player.LasooCaptureProgress.Finishing == this.lasoo.captureProgress) {
          this.lasoo.captureIndicator.progress!.transform.LookAt(this.lasooing!.user.transform);
          this.lasoo.captureIndicator.progress!.transform.Rotate(UnityEngine.Vector3.up * lasooCaptureAngles.progress * 0.5f, UnityEngine.Space.Self);
        }

        for (int index = ((System.Runtime.CompilerServices.ITuple) this.lasoo.captureIndicator).Length; 0 != index--; ) {
          UnityEngine.MeshRenderer lasooCaptureIndicatorRenderer = (UnityEngine.MeshRenderer) ((System.Runtime.CompilerServices.ITuple) this.lasoo.captureIndicatorRenderer)[index];

          // …
          if (Player.LasooCaptureProgress.Capturing == this.lasoo.captureProgress || Player.LasooCaptureProgress.Finishing == this.lasoo.captureProgress) {
            float            lasooCaptureAngle                      = (float) ((System.Runtime.CompilerServices.ITuple) lasooCaptureAngles)[index];
            float            lasooCaptureAngleDelta                 = lasooCaptureAngle / (Player.LasooCaptureIndicatorPrecision - 1u);
            UnityEngine.Mesh lasooCaptureIndicatorMesh              = (UnityEngine.Mesh) ((System.Runtime.CompilerServices.ITuple) this.lasoo.captureIndicatorMesh)[index];
            uint             lasooCaptureIndicatorMeshTriangleIndex = 0u;
            uint             lasooCaptureIndicatorMeshVertexIndex   = 0u;

            // …
            for (uint subindex = 0u; ; ) {
              UnityEngine.Vector3 origin = (UnityEngine.Quaternion.AngleAxis((lasooCaptureAngleDelta * subindex) + (lasooCaptureAngle * -0.5f), UnityEngine.Vector3.up) * UnityEngine.Vector3.forward).normalized * Player.LasooCaptureIndicatorInnerRadius;

              // …
              Player.LasooCaptureIndicatorMeshVertices[lasooCaptureIndicatorMeshVertexIndex++] = origin;
              Player.LasooCaptureIndicatorMeshVertices[lasooCaptureIndicatorMeshVertexIndex++] = origin.normalized * Player.LasooCaptureIndicatorOuterRadius;

              if (Player.LasooCaptureIndicatorPrecision == ++subindex)
              break;

              Player.LasooCaptureIndicatorMeshTriangles[lasooCaptureIndicatorMeshTriangleIndex++] = (int) lasooCaptureIndicatorMeshVertexIndex - 2;
              Player.LasooCaptureIndicatorMeshTriangles[lasooCaptureIndicatorMeshTriangleIndex++] = (int) lasooCaptureIndicatorMeshVertexIndex - 1;
              Player.LasooCaptureIndicatorMeshTriangles[lasooCaptureIndicatorMeshTriangleIndex++] = (int) lasooCaptureIndicatorMeshVertexIndex - 0;
              Player.LasooCaptureIndicatorMeshTriangles[lasooCaptureIndicatorMeshTriangleIndex++] = (int) lasooCaptureIndicatorMeshVertexIndex + 0;
              Player.LasooCaptureIndicatorMeshTriangles[lasooCaptureIndicatorMeshTriangleIndex++] = (int) lasooCaptureIndicatorMeshVertexIndex - 1;
              Player.LasooCaptureIndicatorMeshTriangles[lasooCaptureIndicatorMeshTriangleIndex++] = (int) lasooCaptureIndicatorMeshVertexIndex + 1;
            }

            lasooCaptureIndicatorRenderer.forceRenderingOff = false;
            lasooCaptureIndicatorMesh.vertices              = Player.LasooCaptureIndicatorMeshVertices;
            lasooCaptureIndicatorMesh.triangles             = Player.LasooCaptureIndicatorMeshTriangles;

            lasooCaptureIndicatorMesh.RecalculateNormals();
          } else lasooCaptureIndicatorRenderer.forceRenderingOff = true;
        }
      }

    // … ->> Camera
    if (null != this.mainCamera && UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, Entity.MovementVelocityThreshold).sqrMagnitude < UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, this.rigidBody.linearVelocity).sqrMagnitude)
    this.mainCamera.transform.position = this.mainCameraDistance + UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, transform.position);
  }
}
