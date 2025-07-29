using PatchOdyssey;

/* … */
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
    this.collider     .enabled               = true;
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

    if (this.capture is not null) {
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
      this.transform.localRotation = UnityEngine.Quaternion.LookRotation(this.transform.localPosition - userTransform.localPosition);
    }

    ropeTransform.LookAt(userTransform, UnityEngine.Vector3.up);
  }
}

[UnityEngine.RequireComponent(typeof(UnityEngine.SphereCollider))]
public sealed class Player : Tamer /* ->> Source file must be named “Player” */ {
  public enum LasooCaptureProgress : byte { Initiating, Capturing, Finishing }

  /* … */
  private static readonly float                 LasooCaptureProgressThreshold      = (UnityEngine.Vector3.one * 0.25f).sqrMagnitude;
  private const           uint                  LasooCaptureProgressPrecision      = 10u;  // --> Player.LasooCaptureProgressPrecision >= 2
  private const           byte                  LasooCaptureIndicatorPrecision     = 20;   // ->> Number of segments
  private const           float                 LasooCaptureIndicatorOuterRadius   = 1.5f; // --> Player.LasooCaptureIndicatorOuterRadius > Player.LasooCaptureIndicatorInnerRadius
  private static readonly UnityEngine.Vector3[] LasooCaptureIndicatorMeshVertices  = new UnityEngine.Vector3[(Player.LasooCaptureIndicatorPrecision * 2u)];
  private static readonly int                [] LasooCaptureIndicatorMeshTriangles = new int                [(Player.LasooCaptureIndicatorPrecision - 1u) * 6u];
  private const           float                 LasooCaptureIndicatorInnerRadius   = 1.0f; // --> Player.LasooCaptureIndicatorInnerRadius < Player.LasooCaptureIndicatorOuterRadius
  private static readonly float                 LasooCaptureProgressAngle          = 360.0f / Player.LasooCaptureProgressPrecision;

  [ReadOnlyInInspector]                             public      Lasoo?                                                                    lasoo                         =  null;
  [ReadOnlyInInspector]                             private     UnityEngine.Vector3                                                       lasooCaptureDirection         =  UnityEngine.Vector3.zero;
  [ReadOnlyInInspector]                             private     (UnityEngine.MeshFilter?   completed, UnityEngine.MeshFilter?   progress) lasooCaptureIndicator         =  (null, null);
  [ReadOnlyInInspector]                             private     (UnityEngine.Mesh?         completed, UnityEngine.Mesh?         progress) lasooCaptureIndicatorMesh     =  (null, null);
  [ReadOnlyInInspector]                             private     (UnityEngine.MeshRenderer? completed, UnityEngine.MeshRenderer? progress) lasooCaptureIndicatorRenderer =  (null, null);
  [ReadOnlyInInspector, UnityEngine.SerializeField] private     Player.LasooCaptureProgress                                               lasooCaptureProgress          =  Player.LasooCaptureProgress.Initiating;
  [ReadOnlyInInspector]                             private     UnityEngine.Vector3                                                       lasooCaptureProgressDirection =  UnityEngine.Vector3.zero;
  [ReadOnlyInInspector, UnityEngine.SerializeField] private     Entity.TurnDirection                                                      lasooCaptureProgressTurn      =  Entity.TurnDirection.Clockwise;
  [ReadOnlyInInspector]                             private     uint                                                                      lasooCaptureProgressTurnCount =  0u;
  [ReadWriteInInspector]                            public      float                                                                     lasooDeployReach              =  Lasoo.DeployReach;
  [ReadWriteInInspector]                            public      float                                                                     lasooDeploySpeed              =  Lasoo.DeploySpeed;
  [ReadOnlyInInspector]                             public      bool                                                                      lasooIsDeploying              =  false;
  [ReadWriteInInspector]                            public      float                                                                     lasooRetractReach             =  Lasoo.RetractReach;
  [ReadWriteInInspector]                            public      float                                                                     lasooRetractSpeed             =  Lasoo.RetractSpeed;
  [ReadWriteInInspector]                            public      float                                                                     lasooTurnSpeed                =  Lasoo.TurnSpeed;
  [ReadOnlyInInspector]                             public      Timeframe                                                                 releaseWindow                 =  new(1.0);

  /* … */
  protected override void Awake() {
    this.movementDamping = 4.0f;
    base.Awake();

    // …
    this.collider.radius               = 1.5f;
    this.movementSpeedRandomnessFactor = 0.0f;
  }

  protected override void Capture(Entity entity) {
    base.Capture   (entity);
    this.ResetLasoo();
  }

  public bool DeployLasoo() {
    if (0 == this.followers.Count) {
      Lasoo                  lasoo;
      UnityEngine.GameObject lasooCoil;
      UnityEngine.GameObject lasooRope;
      UnityEngine.Transform  transform = this.transform;

      // …
      if (this.lasoo is null) {
        lasoo     = new UnityEngine.GameObject("Lasoo", typeof(Lasoo)).GetComponent<Lasoo>();
        lasooCoil = UnityEngine.Object.Instantiate(Assets.main.lasooMeshPrefabrication,     UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity, lasoo    .transform);
        lasooRope = UnityEngine.Object.Instantiate(Assets.main.lasooRopeMeshPrefabrication, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity, lasooCoil.transform);

        lasoo.user                                                             = this;
        lasoo.turnSpeed                                                        = this.lasooTurnSpeed;
        lasoo.rope                                                             = lasooRope;
        lasoo.rope.name                                                        = "Rope";
        lasoo.retractSpeed                                                     = this.lasooRetractSpeed;
        lasoo.retractReach                                                     = this.lasooRetractReach;
        lasoo.reachDirection                                                   = UnityEngine.Quaternion.Euler(UnityEngine.Vector3.up * this.turnSpeed * -4.0f) * transform.forward;
        lasoo.reach                                                            = this.lasooRetractReach; // ->> Starts at minimum
        lasoo.isDeploying                                                      = () => this.lasooIsDeploying;
        lasoo.deploySpeed                                                      = this.lasooDeploySpeed;
        lasoo.deployReach                                                      = this.lasooDeployReach;
        lasoo.coil                                                             = lasooCoil;
        lasoo.coil.name                                                        = "Knot";
        this.lasooCaptureIndicatorMesh    .completed                           = new() { name = "playerLasooCaptureIndicatorMesh" };
        this.lasooCaptureIndicatorMesh    .progress                            = new() { name = "playerLasooCaptureProgressIndicatorMesh" };
        this.lasooCaptureIndicator        .completed                           = new UnityEngine.GameObject("Indicator", typeof(UnityEngine.MeshFilter), typeof(UnityEngine.MeshRenderer)).GetComponent<UnityEngine.MeshFilter>();
        this.lasooCaptureIndicator        .completed.sharedMesh                = this.lasooCaptureIndicatorMesh.completed;
        this.lasooCaptureIndicator        .progress                            = new UnityEngine.GameObject("Progress",  typeof(UnityEngine.MeshFilter), typeof(UnityEngine.MeshRenderer)).GetComponent<UnityEngine.MeshFilter>();
        this.lasooCaptureIndicator        .progress.sharedMesh                 = this.lasooCaptureIndicatorMesh.progress;
        this.lasooCaptureIndicator        .progress.transform.localPosition   += UnityEngine.Vector3.up * Game.VectorEpsilon; // ->> Avoid Z-fighting
        this.lasooCaptureIndicatorRenderer.completed                           = this.lasooCaptureIndicator.completed.GetComponent<UnityEngine.MeshRenderer>();
        this.lasooCaptureIndicatorRenderer.completed.allowOcclusionWhenDynamic = false;
        this.lasooCaptureIndicatorRenderer.progress                            = this.lasooCaptureIndicator.progress .GetComponent<UnityEngine.MeshRenderer>();
        this.lasooCaptureIndicatorRenderer.progress.allowOcclusionWhenDynamic  = false;
        this.lasoo                                                             = lasoo;

        if (Assets.main.lasooCaptureIndicatorMaterial is not null) {
          for (int index = ((System.Runtime.CompilerServices.ITuple) this.lasooCaptureIndicator).Length; 0 != index--; ) {
            UnityEngine.MeshRenderer lasooCaptureIndicatorRenderer = (UnityEngine.MeshRenderer) ((System.Runtime.CompilerServices.ITuple) this.lasooCaptureIndicatorRenderer)[index];

            // …
            lasooCaptureIndicatorRenderer.receiveShadows    = false;
            lasooCaptureIndicatorRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lasooCaptureIndicatorRenderer.sharedMaterial    = Assets.main.lasooCaptureIndicatorMaterial;
          }

          this.lasooCaptureIndicatorRenderer.completed.material.color = UnityEngine.Color.LerpUnclamped(this.lasooCaptureIndicatorRenderer.completed.material.color, UnityEngine.Color.black, 0.5f);
        }

        this.lasooCaptureIndicatorMesh.completed.MarkDynamic();
        this.lasooCaptureIndicatorMesh.progress .MarkDynamic();
        this.lasooCaptureIndicator.completed.transform.SetParent(lasoo.transform,                                false);
        this.lasooCaptureIndicator.progress .transform.SetParent(this.lasooCaptureIndicator.completed.transform, false);
        lasoo.transform                               .SetParent(transform,                                      false);
      }

      return true;
    }

    return false;
  }

  protected override void OnApplicationFocus(bool focused) {
    if (!focused) {
      if (this.lasoo is not null) // ->> Instant retraction
        this.lasoo.reach = this.lasoo.retractReach;

      this.RetractLasoo();
    }
  }

  private void OnDestroy() {
    this.RetractLasoo();
  }

  public void ResetLasoo() {
    this.lasooCaptureProgress          = Player.LasooCaptureProgress.Initiating;
    this.lasooCaptureProgressTurnCount = 0u;
    this.lasooIsDeploying              = false;

    if (this.lasoo is not null) {
      if (this.lasoo.capture is Monster monster)
        monster.wrestling = null;

      this.lasoo.capture = null;
    }
  }

  public void RetractLasoo() {
    if (this.lasoo is null)
    return;

    // …
    this.ResetLasoo();
    UnityEngine.Object.Destroy(this.lasooCaptureIndicatorMesh.progress);
    UnityEngine.Object.Destroy(this.lasooCaptureIndicatorMesh.completed);
    UnityEngine.Object.Destroy(this.lasoo.gameObject);

    this.lasoo = null;
  }

  protected override void Update() {
    UnityEngine.Transform transform = this.transform;

    // …
    base.Update();

    if (Game.IsPaused || this.isDefeated)
    return;

    // … ->> Acknowledge inputs
      // … ->> Moving
      this.movementDirection  = UnityEngine.Vector3.zero;
      this.movementDirection += Game.Keyboard.aKey.isPressed || Game.Keyboard.aKey.wasPressedThisFrame || Game.Keyboard.leftArrowKey .isPressed || Game.Keyboard.leftArrowKey .wasPressedThisFrame || UnityEngine.Input.GetKey(UnityEngine.KeyCode.A) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftArrow)  || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.A) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.LeftArrow)  ? UnityEngine.Vector3.left    : UnityEngine.Vector3.zero;
      this.movementDirection += Game.Keyboard.dKey.isPressed || Game.Keyboard.dKey.wasPressedThisFrame || Game.Keyboard.rightArrowKey.isPressed || Game.Keyboard.rightArrowKey.wasPressedThisFrame || UnityEngine.Input.GetKey(UnityEngine.KeyCode.D) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightArrow) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.D) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.RightArrow) ? UnityEngine.Vector3.right   : UnityEngine.Vector3.zero;
      this.movementDirection += Game.Keyboard.sKey.isPressed || Game.Keyboard.sKey.wasPressedThisFrame || Game.Keyboard.downArrowKey .isPressed || Game.Keyboard.downArrowKey .wasPressedThisFrame || UnityEngine.Input.GetKey(UnityEngine.KeyCode.S) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.DownArrow)  || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.S) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.DownArrow)  ? UnityEngine.Vector3.back    : UnityEngine.Vector3.zero;
      this.movementDirection += Game.Keyboard.wKey.isPressed || Game.Keyboard.wKey.wasPressedThisFrame || Game.Keyboard.upArrowKey   .isPressed || Game.Keyboard.upArrowKey   .wasPressedThisFrame || UnityEngine.Input.GetKey(UnityEngine.KeyCode.W) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.UpArrow)    || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.W) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.UpArrow)    ? UnityEngine.Vector3.forward : UnityEngine.Vector3.zero;

      // … ->> Lassoing
      if (Game.Keyboard.enterKey.wasPressedThisFrame || Game.Keyboard.tabKey.wasPressedThisFrame || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Return) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Tab)) {
        if (this.lasooIsDeploying) this.ResetLasoo();
        else this.lasooIsDeploying = 0 == this.followers.Count && (this.lasoo is null || this.lasoo.reach == this.lasoo.retractReach);
      }

      // … ->> Releasing
      if (Game.Keyboard.shiftKey.wasReleasedThisFrame || UnityEngine.Input.GetKeyUp(UnityEngine.KeyCode.LeftShift) || UnityEngine.Input.GetKeyUp(UnityEngine.KeyCode.RightShift)) {
        if (!this.releaseWindow.isElapsed) {
          this.ResetLasoo();

          if (0 != this.followers.Count)
          this.Release(this.followers[0]);
        }

        this.releaseWindow.Reset();
      }

    // … ->> Apply inputs
      // … ->> Moving/ Turning
      this.movementSpeedFactor = this.lasoo is not null ? ((1.0f - this.lasoo.reachProgress) * 0.5f) + 0.5f : 1.0f;

      if (UnityEngine.Vector3.zero != this.movementDirection)
        this.movementRestCooldown.Reset();

      else if (this.movementRestCooldown.isElapsed && Entity.MovementVelocityThreshold.sqrMagnitude > this.rigidBody.linearVelocity.sqrMagnitude) {
        this.rigidBody.angularVelocity = UnityEngine.Vector3.zero;
        this.rigidBody.linearVelocity  = UnityEngine.Vector3.zero;
        this.turnDirection             = UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward, ((this.mainCamera?.transform.position ?? (UnityEngine.Vector3.back + transform.position)) - transform.position).normalized);
      }

      // … ->> Lassoing
      if (!this.lasooIsDeploying && (this.lasoo is null || this.lasoo.reach == this.lasoo.retractReach))
        this.RetractLasoo();

      else if (this.DeployLasoo()) {
        (float completed, float progress) lasooCaptureAngles = (360.0f, 0.0f); // ->> in Degrees

        // … ->> Capturing
        if (this.lasoo!.capture is not null) {
          switch (this.lasooCaptureProgress) {
            case Player.LasooCaptureProgress.Capturing: {
              foreach (ref readonly Entity.TurnDirection direction in (System.ReadOnlySpan<Entity.TurnDirection>) stackalloc[] {Entity.TurnDirection.Anticlockwise, Entity.TurnDirection.Clockwise}) {
                this.lasooCaptureProgressTurnCount = 1u;
                this.lasooCaptureProgressTurn      = direction;
                this.lasooCaptureProgressDirection = UnityEngine.Quaternion.AngleAxis(Player.LasooCaptureProgressAngle, Entity.TurnDirectionToVector3(this.lasooCaptureProgressTurn, UnityEngine.Vector3.up)) * this.lasooCaptureDirection;

                if (Player.LasooCaptureProgressThreshold > (this.lasooCaptureProgressDirection - this.lasoo!.reachDirection).sqrMagnitude) {
                  this.lasooCaptureProgress          = Player.LasooCaptureProgress.Finishing;
                  this.lasooCaptureProgressDirection = UnityEngine.Quaternion.AngleAxis(Player.LasooCaptureProgressAngle, Entity.TurnDirectionToVector3(this.lasooCaptureProgressTurn, UnityEngine.Vector3.up)) * this.lasooCaptureProgressDirection;

                  break;
                }

                #if DEBUG || DEVELOPMENT_BUILD
                  UnityEngine.Debug.DrawRay(UnityEngine.Vector3.zero, this.lasooCaptureProgressDirection * 100.0f, UnityEngine.Color.green, 0.0f, false);
                #endif
              }

              lasooCaptureAngles.progress = UnityEngine.Vector3.Angle(this.lasooCaptureDirection, this.lasoo!.reachDirection);

              #if DEBUG || DEVELOPMENT_BUILD
                UnityEngine.Debug.DrawRay(UnityEngine.Vector3.zero, this.lasooCaptureDirection * 100.0f, UnityEngine.Color.white, 0.0f, false);
                UnityEngine.Debug.DrawRay(UnityEngine.Vector3.zero, this.lasoo!.reachDirection * 100.0f, UnityEngine.Color.red,   0.0f, false);
              #endif
            } break;

            case Player.LasooCaptureProgress.Initiating: {
              this.lasooCaptureDirection = (this.lasoo!.capture.transform.position - this.transform.position).normalized;
              this.lasooCaptureProgress  = Player.LasooCaptureProgress.Capturing;

              lasooCaptureAngles.progress = 0.0f;

              #if DEBUG || DEVELOPMENT_BUILD
                UnityEngine.Debug.DrawRay(UnityEngine.Vector3.zero, this.lasooCaptureDirection * 100.0f, UnityEngine.Color.white, 0.0f, false);
                UnityEngine.Debug.DrawRay(UnityEngine.Vector3.zero, this.lasoo!.reachDirection * 100.0f, UnityEngine.Color.red,   0.0f, false);
              #endif
            } break;

            case Player.LasooCaptureProgress.Finishing: {
              if (Player.LasooCaptureProgressPrecision - this.lasooCaptureProgressTurnCount == 1u)
                this.Capture(this.lasoo!.capture);

              else if (Player.LasooCaptureProgressThreshold > (this.lasooCaptureProgressDirection - this.lasoo!.reachDirection).sqrMagnitude) {
                this.lasooCaptureProgressDirection =  UnityEngine.Quaternion.AngleAxis(Player.LasooCaptureProgressAngle, Entity.TurnDirectionToVector3(this.lasooCaptureProgressTurn, UnityEngine.Vector3.up)) * this.lasooCaptureProgressDirection;
                this.lasooCaptureProgressTurnCount += 1u;
              }

              else if (Player.LasooCaptureProgressThreshold > (this.lasooCaptureDirection - this.lasoo!.reachDirection).sqrMagnitude) {
                this.lasooCaptureProgress          = Player.LasooCaptureProgress.Capturing;
                this.lasooCaptureProgressTurnCount = 0u;
              }

              lasooCaptureAngles.progress  = UnityEngine.Vector3.SignedAngle(this.lasooCaptureDirection, this.lasoo!.reachDirection, UnityEngine.Vector3.up) / 360.0f;
              lasooCaptureAngles.progress  = lasooCaptureAngles.progress < +0.0f                                 ? 1.0f + lasooCaptureAngles.progress : lasooCaptureAngles.progress;
              lasooCaptureAngles.progress  = Entity.TurnDirection.Anticlockwise == this.lasooCaptureProgressTurn ? 1.0f - lasooCaptureAngles.progress : lasooCaptureAngles.progress;
              lasooCaptureAngles.progress *= 360.0f;

              #if DEBUG || DEVELOPMENT_BUILD
                UnityEngine.Debug.DrawRay(UnityEngine.Vector3.zero, this.lasooCaptureDirection         * 100.0f, UnityEngine.Color.white, 0.0f, false);
                UnityEngine.Debug.DrawRay(UnityEngine.Vector3.zero, this.lasooCaptureProgressDirection * 100.0f, UnityEngine.Color.blue,  0.0f, false);
                UnityEngine.Debug.DrawRay(UnityEngine.Vector3.zero, this.lasoo!.reachDirection         * 100.0f, UnityEngine.Color.red,   0.0f, false);
              #endif
            } break;
          }
        }

        // … ->> Indicating (capture)
        if (Player.LasooCaptureProgress.Capturing == this.lasooCaptureProgress || Player.LasooCaptureProgress.Finishing == this.lasooCaptureProgress) {
          this.lasooCaptureIndicator.progress!.transform.LookAt(this.lasoo!.user.transform);
          this.lasooCaptureIndicator.progress!.transform.Rotate(UnityEngine.Vector3.up * lasooCaptureAngles.progress * 0.5f, UnityEngine.Space.Self);
        }

        for (int index = ((System.Runtime.CompilerServices.ITuple) this.lasooCaptureIndicator).Length; 0 != index--; ) {
          UnityEngine.MeshRenderer lasooCaptureIndicatorRenderer = (UnityEngine.MeshRenderer) ((System.Runtime.CompilerServices.ITuple) this.lasooCaptureIndicatorRenderer)[index];

          // …
          if (Player.LasooCaptureProgress.Capturing == this.lasooCaptureProgress || Player.LasooCaptureProgress.Finishing == this.lasooCaptureProgress) {
            float                  lasooCaptureAngle                      = (float) ((System.Runtime.CompilerServices.ITuple) lasooCaptureAngles)[index];
            float                  lasooCaptureAngleDelta                 = lasooCaptureAngle / (Player.LasooCaptureIndicatorPrecision - 1u);
            UnityEngine.MeshFilter lasooCaptureIndicator                  = (UnityEngine.MeshFilter) ((System.Runtime.CompilerServices.ITuple) this.lasooCaptureIndicator)    [index];
            UnityEngine.Mesh       lasooCaptureIndicatorMesh              = (UnityEngine.Mesh)       ((System.Runtime.CompilerServices.ITuple) this.lasooCaptureIndicatorMesh)[index];
            uint                   lasooCaptureIndicatorMeshTriangleIndex = 0u;
            uint                   lasooCaptureIndicatorMeshVertexIndex   = 0u;

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

            lasooCaptureIndicatorRenderer.enabled   = true;
            lasooCaptureIndicatorMesh    .vertices  = Player.LasooCaptureIndicatorMeshVertices;
            lasooCaptureIndicatorMesh    .triangles = Player.LasooCaptureIndicatorMeshTriangles;
            lasooCaptureIndicatorMesh    .RecalculateNormals();
          } else lasooCaptureIndicatorRenderer.enabled = false;
        }
      }
  }
}
