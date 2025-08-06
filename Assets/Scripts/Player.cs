using PatchOdyssey;

/* … */
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
  private const           float                 VignetteIntensity                  = 0.15f;
  private static readonly UnityEngine.Color     VignetteColor                      = UnityEngine.Color.black;
  private static readonly float                 LasooCaptureProgressThreshold      = (UnityEngine.Vector3.one * 0.25f).sqrMagnitude;
  private const           uint                  LasooCaptureProgressPrecision      = 10u; // --> Player.LasooCaptureProgressPrecision >= 2
  private const           float                 LasooCaptureProgressAngle          = 360.0f / Player.LasooCaptureProgressPrecision;
  private const           byte                  LasooCaptureIndicatorPrecision     = 20;   // ->> Number of segments
  private const           float                 LasooCaptureIndicatorOuterRadius   = 1.5f; // --> Player.LasooCaptureIndicatorOuterRadius > Player.LasooCaptureIndicatorInnerRadius
  private static readonly UnityEngine.Vector3[] LasooCaptureIndicatorMeshVertices  = new UnityEngine.Vector3[(Player.LasooCaptureIndicatorPrecision * 2u)];
  private static readonly int                [] LasooCaptureIndicatorMeshTriangles = new int                [(Player.LasooCaptureIndicatorPrecision - 1u) * 6u];
  private const           float                 LasooCaptureIndicatorInnerRadius   = 1.0f; // --> Player.LasooCaptureIndicatorInnerRadius < Player.LasooCaptureIndicatorOuterRadius
  public  static          bool                  IsReady                            = false;

  [UnityEngine.Header("Player")]
  [ReadOnlyInInspector]  public bool             isInputing    = false; // ->> Only significant actions count
  [ReadOnlyInInspector]  public bool             isLassoing    = false;
  [ReadWriteInInspector] public Player.LasooInfo lasoo         = new() {captureDirection = UnityEngine.Vector3.zero, captureIndicator = (null, null), captureIndicatorMesh = (null, null), captureIndicatorRenderer = (null, null), captureProgress = Player.LasooCaptureProgress.Initiating, captureProgressDirection = UnityEngine.Vector3.zero, captureProgressTurn = Entity.TurnDirection.Clockwise, captureProgressTurnCount = 0u, deployReach = Lasoo.DeployReach, deploySpeed = Lasoo.DeploySpeed, retractReach = Lasoo.RetractReach, retractSpeed = Lasoo.RetractSpeed, turnSpeed = Lasoo.TurnSpeed};
  [ReadOnlyInInspector]  public Lasoo?           lasooing      = null;
  [ReadWriteInInspector] public Timeframe        releaseWindow = new(1.00);

  /* … */
  protected override void Awake() {
    base.Awake();

    // …
    base.collider.isTrigger  = false;
    base.followAutomatically = false;
    base.shootAutomatically  = false;

    if (null != Assets.main.volume && Assets.main.volume.profile is UnityEngine.Rendering.VolumeProfile volumeProfile)
    switch (base.worldVignette = (
      volumeProfile.TryGet<UnityEngine.Rendering.Universal     .Vignette>(out UnityEngine.Rendering.Universal     .Vignette _1) ? _1 as UnityEngine.Rendering.VolumeComponent :
      volumeProfile.TryGet<UnityEngine.Rendering.HighDefinition.Vignette>(out UnityEngine.Rendering.HighDefinition.Vignette _2) ? _2 as UnityEngine.Rendering.VolumeComponent :
      null
    )) {
      case UnityEngine.Rendering.HighDefinition.Vignette highDefinitionRenderVignette: {
        highDefinitionRenderVignette.color    .overrideState = true;
        highDefinitionRenderVignette.intensity.overrideState = true;
      } break;

      case UnityEngine.Rendering.Universal.Vignette universalRenderVignette: {
        universalRenderVignette.color    .overrideState = true;
        universalRenderVignette.intensity.overrideState = true;
      } break;
    }

    // … ->> Grab the main camera and setup `Entity::tracking.cameras` early
    ((System.Action<UnityEngine.Camera?>) (static _ => {}))(base.worldCamera);
  }

  protected override void Capture(Entity entity) {
    base.Capture   (entity);
    this.ResetLasoo();
  }

  public bool DeployLasoo() {
    UnityEngine.Transform transform = this.transform;

    // …
    if (0 == base.followers.Count) {
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
      // if (this.lasooing is not null) // ->> Instant retraction
      //   this.lasooing.reach = this.lasooing.retractReach;

      // this.RetractLasoo();
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

  protected override void Update() {
    UnityEngine.Transform transform = this.transform;

    // …
    base.Update();

    if (Game.IsPaused || base.isDefeated)
    return;

    // … ->> Input
    Player.IsReady = Player.IsReady || this.isInputing;
    this.isInputing = false;
      // … ->> Moving
      base.followAutomatically = false;
      base.movement.direction  = UnityEngine.Vector3.zero;
      base.movement.direction += Game.Keyboard.aKey.isPressed || Game.Keyboard.aKey.wasPressedThisFrame || Game.Keyboard.leftArrowKey .isPressed || Game.Keyboard.leftArrowKey .wasPressedThisFrame || UnityEngine.Input.GetKey(UnityEngine.KeyCode.A) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftArrow)  || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.A) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.LeftArrow)  ? UnityEngine.Vector3.left    : UnityEngine.Vector3.zero;
      base.movement.direction += Game.Keyboard.dKey.isPressed || Game.Keyboard.dKey.wasPressedThisFrame || Game.Keyboard.rightArrowKey.isPressed || Game.Keyboard.rightArrowKey.wasPressedThisFrame || UnityEngine.Input.GetKey(UnityEngine.KeyCode.D) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightArrow) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.D) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.RightArrow) ? UnityEngine.Vector3.right   : UnityEngine.Vector3.zero;
      base.movement.direction += Game.Keyboard.sKey.isPressed || Game.Keyboard.sKey.wasPressedThisFrame || Game.Keyboard.downArrowKey .isPressed || Game.Keyboard.downArrowKey .wasPressedThisFrame || UnityEngine.Input.GetKey(UnityEngine.KeyCode.S) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.DownArrow)  || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.S) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.DownArrow)  ? UnityEngine.Vector3.back    : UnityEngine.Vector3.zero;
      base.movement.direction += Game.Keyboard.wKey.isPressed || Game.Keyboard.wKey.wasPressedThisFrame || Game.Keyboard.upArrowKey   .isPressed || Game.Keyboard.upArrowKey   .wasPressedThisFrame || UnityEngine.Input.GetKey(UnityEngine.KeyCode.W) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.UpArrow)    || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.W) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.UpArrow)    ? UnityEngine.Vector3.forward : UnityEngine.Vector3.zero;
      this.isInputing          = UnityEngine.Vector3.zero != base.movement.direction;

      // … ->> Lassoing
      if (Game.Keyboard.enterKey.wasPressedThisFrame || Game.Keyboard.tabKey.wasPressedThisFrame || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Return) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Tab)) {
        if (this.isLassoing) this.ResetLasoo();
        else { this.isInputing = true; this.isLassoing = 0 == base.followers.Count && (this.lasooing is null || this.lasooing.reach == this.lasooing.retractReach); }
      }

      // … ->> Releasing
      if (Game.Keyboard.shiftKey.wasReleasedThisFrame || UnityEngine.Input.GetKeyUp(UnityEngine.KeyCode.LeftShift) || UnityEngine.Input.GetKeyUp(UnityEngine.KeyCode.RightShift)) {
        if (!this.releaseWindow.isElapsed) {
          this.ResetLasoo();

          if (0 != base.followers.Count) {
            this.isInputing = true;
            base.Release(base.followers[0]);
          }
        }

        this.releaseWindow.Reset();
      }

      // … ->> Shooting
      if (Game.Keyboard.spaceKey.wasPressedThisFrame || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space)) {
        if (null != base.Shoot())
        this.isInputing = true;
      }

    // … ->> Application
      // … ->> Moving/ Turning
      base.movement.speedFactor = this.lasooing is not null ? ((1.0f - this.lasooing.reachProgress) * 0.5f) + 0.5f : 1.0f;

      if (this.isInputing || UnityEngine.Vector3.zero != base.movement.direction)
        base.movement.pauseCooldown.Reset();

      else if (base.movement.pauseCooldown.isElapsed && Entity.MovementVelocityThreshold.sqrMagnitude > base.rigidBody.linearVelocity.sqrMagnitude) {
        base.rigidBody.angularVelocity = UnityEngine.Vector3.zero;
        base.rigidBody.linearVelocity  = UnityEngine.Vector3.zero;
        base.turn.direction            = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, ((base.worldCamera?.transform.position ?? (UnityEngine.Vector3.back + transform.position)) - transform.position).normalized);
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

    // … ->> Vignette
    switch (base.worldVignette) {
      case UnityEngine.Rendering.HighDefinition.Vignette highDefinitionRenderVignette: {
        highDefinitionRenderVignette.color    .value = UnityEngine.Color.LerpUnclamped(highDefinitionRenderVignette.color    .value, Player.VignetteColor,     0.1f);
        highDefinitionRenderVignette.intensity.value = UnityEngine.Mathf.LerpUnclamped(highDefinitionRenderVignette.intensity.value, Player.VignetteIntensity, 0.1f);
      } break;

      case UnityEngine.Rendering.Universal.Vignette universalRenderVignette: {
        universalRenderVignette.color    .value = UnityEngine.Color.LerpUnclamped(universalRenderVignette.color    .value, Player.VignetteColor,     0.1f);
        universalRenderVignette.intensity.value = UnityEngine.Mathf.LerpUnclamped(universalRenderVignette.intensity.value, Player.VignetteIntensity, 0.1f);
      } break;
    }
  }
}
  [UnityEngine.DefaultExecutionOrder(4)]
  [UnityEngine.RequireComponent(typeof(UnityEngine.BoxCollider))]
  public sealed class Lasoo : GameComponent /* ->> “Lasoo” is intentionally misspelled */ {
    public const float DeployReach  = 17.5f;  // ->> Maximum reach
    public const float DeploySpeed  = 7.0f;   // ->> Units per second
    public const float RetractReach = 1.0f;   // ->> Minimum reach
    public const float RetractSpeed = 20.0f;  // ->> Units   per second
    public const float TurnSpeed    = 135.0f; // ->> Degrees per second

    public            Entity?                                               capture        =  null;
    public   new      UnityEngine.BoxCollider                               collider       => (UnityEngine.BoxCollider) base.collider;
    public            UnityEngine.GameObject                                coil           =  null!; // ->> Must be set
    public            float                                                 deployReach    =  Lasoo.DeployReach;
    public            float                                                 deploySpeed    =  Lasoo.DeploySpeed;
    internal          System.Func<bool>                                     isDeploying    =  static () => false; // --> bool*
    public            float                                                 reach          =  Lasoo.RetractReach; // --> retractReach <= reach <= deployReach
    internal          UnityEngine.Vector3                                   reachDirection =  UnityEngine.Vector3.forward;
    public            float                                                 reachProgress  => UnityEngine.Mathf.Clamp01((this.reach - this.retractReach) / (this.deployReach - this.retractReach));
    public            float                                                 retractReach   =  Lasoo.RetractReach;
    public            float                                                 retractSpeed   =  Lasoo.RetractSpeed;
    public            UnityEngine.GameObject                                rope           =  null!; // ->> Must be set
    private  readonly System.Collections.Generic.List<UnityEngine.Color>    ropeColors     =  new();
    private  readonly System.Collections.Generic.List<UnityEngine.Material> ropeMaterials  =  new();
    public            float                                                 turnSpeed      =  Lasoo.TurnSpeed;
    public            Entity                                                user           =  null!; // ->> Must be set

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
      base.collider     .excludeLayers         = (UnityEngine.LayerMask) 0;
      base.collider     .hasModifiableContacts = false;
      base.collider     .includeLayers         = (UnityEngine.LayerMask) ~0;
      base.collider     .isTrigger             = true;
      this.ropeColors   .Capacity              = ropeTransform.hierarchyCount;
      this.ropeMaterials.Capacity              = ropeTransform.hierarchyCount;

      ropeTransform.ForEach<UnityEngine.Renderer>(renderer => {
        this.ropeColors   .Add(renderer.sharedMaterial.color);
        this.ropeMaterials.Add(renderer.material = renderer.material);
      });
    }

    private void Update() {
      UnityEngine.Vector3   reachSize     = UnityEngine.Vector3.one * (this.reach - this.retractReach);
      UnityEngine.Transform ropeTransform = this.rope.transform;

      // …
      if (Game.IsPaused)
      return;

      // …
      ropeTransform.localScale = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, reachSize - UnityEngine.Vector3.one) + UnityEngine.Vector3.one;

      for (int index = this.ropeMaterials.Count; 0 != index--; )
      this.ropeMaterials[index].color = UnityEngine.Color.LerpUnclamped(this.ropeColors[index], UnityEngine.Color.red, null != this.capture ? this.reachProgress * 0.675f : 0.000f);

      if (null != this.capture) {
        this.reach              = UnityEngine.Vector3.Distance(this.capture.transform.position, this.user.transform.position);
        this.reachDirection     = (this.capture.transform.position - this.user.transform.position).normalized;
        this.transform.position = this.capture.transform.position;
        this.transform.rotation = UnityEngine.Quaternion.LookRotation(this.user.transform.position - this.transform.position, UnityEngine.Vector3.up);
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
          if (coilBounds is not UnityEngine.Bounds bounds) coilBounds = meshBounds;
          else { bounds.Encapsulate(meshBounds); coilBounds = bounds; }

          // …
          return true;
        });

        coilSize                     = coilBounds?.size ?? coilSize;
        this.collider.size           = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, reachSize)                  + coilSize;
        this.collider.center         = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, this.collider.size * -0.5f) + UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, coilSize);
        this.reach                   = UnityEngine.Mathf.Clamp(this.reach + (UnityEngine.Time.unscaledDeltaTime * (!this.isDeploying() ? -this.retractSpeed : +this.deploySpeed)), this.retractReach, this.deployReach);
        this.reachDirection          = UnityEngine.Quaternion.Euler(UnityEngine.Vector3.up * UnityEngine.Time.unscaledDeltaTime * this.turnSpeed) * this.reachDirection;
        this.transform.position      = this.user.transform.position + (this.reachDirection * this.reach);
        this.transform.localRotation = UnityEngine.Quaternion.LookRotation(this.user.transform.position - this.transform.position, UnityEngine.Vector3.up);
      }

      ropeTransform.LookAt(this.user.transform, UnityEngine.Vector3.up);
    }
  }
