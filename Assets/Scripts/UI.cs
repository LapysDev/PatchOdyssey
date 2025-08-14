using PatchOdyssey;

/* … */
[UnityEngine.DefaultExecutionOrder(1)]
[UnityEngine.DisallowMultipleComponent]
[UnityEngine.RequireComponent(typeof(UnityEngine.Canvas))]
[UnityEngine.RequireComponent(typeof(UnityEngine.EventSystems.EventSystem))]
[UnityEngine.RequireComponent(typeof(UnityEngine.UI.GraphicRaycaster))]
public sealed class UI : UnityEngine.MonoBehaviour {
  public enum Activity : byte { HUD, MainMenu }

  [System.Serializable]
  public /* readonly */ struct ActivityBackground {
    [ReadWriteInInspector]                            public   UnityEngine.UI.MaskableGraphic? entry;
    [ReadWriteInInspector]                            public   UnityEngine.UI.MaskableGraphic? exit;
    [ReadWriteInInspector]                            public   Timeframe                       transition;
    [ReadOnlyInInspector, UnityEngine.SerializeField] internal UI.ActivityTransitionInfo       transitionEntry;
    [ReadOnlyInInspector, UnityEngine.SerializeField] internal UI.ActivityTransitionInfo       transitionExit;
  }

  public enum ActivityState : byte { Any, Credits, End, Gameover, Main, Options, Pause, Play, Statistics, Win }

  [System.Serializable]
  public sealed class ActivityTransitionInfo {
    [ReadWriteInInspector] public UI.Activity             activity  = default; // ->> Unique ID
    [ReadWriteInInspector] public UnityEngine.CanvasGroup container = null!;

    [ReadWriteInInspector] public bool                   backgroundIsVisible = true;
    [ReadWriteInInspector] public UnityEngine.Color      color               = new(0.0f, 0.0f, 0.0f, 0.0f);
    [ReadWriteInInspector] public UnityEngine.Material?  material            = null;
    [ReadWriteInInspector] public UnityEngine.Sprite?    sprite              = null;
    [ReadWriteInInspector] public UnityEngine.Texture2D? texture             = null;
  }

  [System.Serializable]
  public /* readonly */ struct Buttons {
    [ReadWriteInInspector] public UnityEngine.UI.Button? chat;
    [ReadWriteInInspector] public UnityEngine.UI.Button? play;
    [ReadWriteInInspector] public UnityEngine.UI.Button? quit;
  }

  public enum ContainerVisibility { Hidden, Visible }

  [System.Serializable]
  public /* readonly */ struct Entities {
    [System.Serializable]
    public /* readonly */ struct StatisticsInfo {
      [ReadOnlyInInspector] public /* readonly */ System.Collections.Generic.List<(Entity, UnityEngine.RectTransform)> health;
      [ReadOnlyInInspector] public /* readonly */ System.Collections.Generic.List<(Entity, UnityEngine.RectTransform)> shoot;
    }

    /* … */
    [ReadOnlyInInspector]  internal UnityEngine.Vector3        playerHorizontalMovementDirection; // ->> Refers to the `Player`s found via `UI::FindPlayer()`
    [ReadWriteInInspector] public   Timeframe                  playerHorizontalMovementDuration;
    [ReadOnlyInInspector]  internal UnityEngine.Vector3        playerVerticalMovementDirection;
    [ReadWriteInInspector] public   Timeframe                  playerVerticalMovementDuration;
    [ReadOnlyInInspector]  internal UI.Entities.StatisticsInfo statistics;
  }

  [System.Serializable]
  public /* readonly */ struct HeadsUpDisplay {
    [System.Serializable]
    public sealed class Containers {
      [ReadWriteInInspector]                                 public TMPro.TextMeshProUGUI?   area     = null;
      [ReadWriteInInspector]                                 public UnityEngine.CanvasGroup? chat     = null;
      [ReadWriteInInspector]                                 public TMPro.TextMeshProUGUI?   chatName = null;
      [ReadWriteInInspector]                                 public TMPro.TextMeshProUGUI?   chatText = null;
      [ReadWriteInInspector, RenameInInspector("Game Over")] public UnityEngine.CanvasGroup? gameover = null; // --> [UnityEngine.InspectorName("Game Over")]
      [ReadWriteInInspector]                                 public UnityEngine.CanvasGroup? pause    = null;
      [ReadWriteInInspector]                                 public TMPro.TextMeshProUGUI?   score    = null;
      [ReadWriteInInspector]                                 public UnityEngine.CanvasGroup? win      = null;
      [ReadWriteInInspector]                                 public TMPro.TextMeshProUGUI?   winText  = null;
    }

    [System.Serializable]
    public sealed class Controls {
      [ReadWriteInInspector] public /* readonly */ System.Collections.Generic.List<UnityEngine.UI.Button> home      = new(1);
      [ReadWriteInInspector] public /* readonly */ System.Collections.Generic.List<UnityEngine.UI.Button> lasso     = new(1);
      [ReadWriteInInspector] public /* readonly */ System.Collections.Generic.List<UnityEngine.UI.Button> menu      = new(1);
      [ReadWriteInInspector] public /* readonly */ System.Collections.Generic.List<UnityEngine.UI.Button> moveDown  = new(1);
      [ReadWriteInInspector] public /* readonly */ System.Collections.Generic.List<UnityEngine.UI.Button> moveLeft  = new(1);
      [ReadWriteInInspector] public /* readonly */ System.Collections.Generic.List<UnityEngine.UI.Button> moveRight = new(1);
      [ReadWriteInInspector] public /* readonly */ System.Collections.Generic.List<UnityEngine.UI.Button> moveUp    = new(1);
      [ReadWriteInInspector] public /* readonly */ System.Collections.Generic.List<UnityEngine.UI.Button> release   = new(1);
      [ReadWriteInInspector] public /* readonly */ System.Collections.Generic.List<UnityEngine.UI.Button> resume    = new(1);
      [ReadWriteInInspector] public /* readonly */ System.Collections.Generic.List<UnityEngine.UI.Button> shoot     = new(1);
      [ReadWriteInInspector] public /* readonly */ System.Collections.Generic.List<UnityEngine.UI.Button> stop      = new(1);
    }

    /* … */
    [ReadWriteInInspector, System.NonSerialized] internal UI.HeadsUpDisplay.Containers[] containers;
    [ReadWriteInInspector, System.NonSerialized] internal UI.HeadsUpDisplay.Controls  [] controls;
    [ReadOnlyInInspector]                        public   UnityEngine.CanvasGroup?       layout;
    [ReadOnlyInInspector, System.NonSerialized]  public   UI.HeadsUpDisplay.Containers   layoutContainers;
    [ReadOnlyInInspector, System.NonSerialized]  public   UI.HeadsUpDisplay.Controls     layoutControls;

    [ReadWriteInInspector] public UnityEngine.CanvasGroup?     desktop;
    [ReadWriteInInspector] public UI.HeadsUpDisplay.Containers desktopContainers;
    [ReadWriteInInspector] public UI.HeadsUpDisplay.Controls   desktopControls;

    [ReadWriteInInspector] public UnityEngine.CanvasGroup?     mobile;
    [ReadWriteInInspector] public UI.HeadsUpDisplay.Containers mobileContainers;
    [ReadWriteInInspector] public UI.HeadsUpDisplay.Controls   mobileControls;
  }

  public enum Layout : byte { Responsive, Desktop, Mobile }

  /* … */
  public static UI main = null!;

  [ReadOnlyInInspector]  private UnityEngine.Canvas                                         _canvas                     = null!;
  [ReadOnlyInInspector]  private UnityEngine.CanvasRenderer                                 _canvasRenderer             = null!;
  [ReadOnlyInInspector]  private UnityEngine.UI.CanvasScaler                                _canvasScaler               = null!;
  [ReadOnlyInInspector]  private UnityEngine.EventSystems.EventSystem                       _eventSystem                = null!;
  [ReadOnlyInInspector]  private UnityEngine.UI.GraphicRaycaster                            _graphicsRaycaster          = null!;
  [ReadOnlyInInspector]  private UnityEngine.RectTransform                                  _rectTransform              = null!;
  [ReadOnlyInInspector]  private UnityEngine.Camera?                                        _worldCamera                = null;
  [ReadWriteInInspector] public  UI.Activity                                                activity                    = UI.Activity.MainMenu;
  [ReadWriteInInspector] public  Timeframe                                                  activityChangeDuration      = new(1.00);
  [ReadOnlyInInspector]  private UI.Activity?                                               activityPrior               = null;
  [ReadWriteInInspector] public  UI.ActivityState                                           activityState               = UI.ActivityState.Any; // ->> For example: Game/ Pause menu in Heads-Up Display
  [ReadOnlyInInspector]  private UI.ActivityState?                                          activityStatePrior          = null;
  [ReadWriteInInspector] public  Timeframe                                                  activityStateChangeDuration = new(0.25);
  [ReadWriteInInspector] public  System.Collections.Generic.List<UI.ActivityTransitionInfo> activityTransitions         = new(System.Enum.GetValues(typeof(UI.Activity)).Length); // --> System.Collections.Generic.Dictionary<UI.Activity, …>
  [ReadOnlyInInspector]  private int                                                        applicationHeight           = -1;
  [ReadOnlyInInspector]  private int                                                        applicationWidth            = -1;
  [ReadWriteInInspector] public  UI.ActivityBackground                                      background                  = new() {entry = null, exit = null, transition = new(1.5), transitionEntry = new() {color = new(0.00f, 0.00f, 0.00f, 0.00f), container = null!, material = null!, sprite = null!, texture = null!}, transitionExit = new() {color = new(0.00f, 0.00f, 0.00f, 0.00f), container = null!, material = null!, sprite = null!, texture = null!}};
  [ReadWriteInInspector] public  UI.Buttons                                                 buttons                     = new() {play = null, quit = null};
  [ReadOnlyInInspector]  public  ref readonly UnityEngine.Canvas                            canvas                      { get { if (this._canvas         is null && base.TryGetComponent(out UnityEngine.Canvas          canvas))         { this._canvas         = canvas; }         return ref this._canvas!; } }
  [ReadOnlyInInspector]  public  ref readonly UnityEngine.CanvasRenderer                    canvasRenderer              { get { if (this._canvasRenderer is null && base.TryGetComponent(out UnityEngine.CanvasRenderer  canvasRenderer)) { this._canvasRenderer = canvasRenderer; } return ref this._canvasRenderer!; } }
  [ReadWriteInInspector] public  ref readonly UnityEngine.UI.CanvasScaler                   canvasScaler                { get { if (this._canvasScaler   is null && base.TryGetComponent(out UnityEngine.UI.CanvasScaler canvasScaler))   { this._canvasScaler   = canvasScaler; }   return ref this._canvasScaler!; } }
  [ReadWriteInInspector] public  UnityEngine.AudioClip?                                     clickAudioClip              = null;
  [ReadWriteInInspector] public  Timeframe                                                  controlSwitchDuration       = new(0.70);
  [ReadWriteInInspector] public  Timeframe                                                  controlSwitchInterval       = new(5.00);
  [ReadOnlyInInspector]  public  UI.Entities                                                entities                    = new() {playerHorizontalMovementDirection = UnityEngine.Vector3.zero, playerHorizontalMovementDuration = new(2.50), playerVerticalMovementDirection = UnityEngine.Vector3.zero, playerVerticalMovementDuration = new(2.50), statistics = new() {health = new(16), shoot = new(16)}};
  [ReadOnlyInInspector]  public  ref readonly UnityEngine.EventSystems.EventSystem          eventSystem                 { get { if (this._eventSystem       is null && base.TryGetComponent(out UnityEngine.EventSystems.EventSystem eventSystem))       { this._eventSystem       = eventSystem; }       return ref this._eventSystem!; } }
  [ReadOnlyInInspector]  public  ref readonly UnityEngine.UI.GraphicRaycaster               graphicsRaycaster           { get { if (this._graphicsRaycaster is null && base.TryGetComponent(out UnityEngine.UI.GraphicRaycaster      graphicsRaycaster)) { this._graphicsRaycaster = graphicsRaycaster; } return ref this._graphicsRaycaster!; } }
  [ReadWriteInInspector] public  UI.HeadsUpDisplay                                          HUD                         = new() {desktop = null, desktopContainers = new(), desktopControls = new(), layout = null, layoutContainers = new(), layoutControls = new(), mobile = null, mobileContainers = new(), mobileControls = new()};
  [ReadOnlyInInspector]  public  UI.Layout                                                  layout                      = UI.Layout.Responsive;
  [ReadOnlyInInspector]  public  ref readonly UnityEngine.RectTransform                     rectTransform               { get { this._rectTransform ??= (UnityEngine.RectTransform) base.transform; return ref this._rectTransform; } }
  [ReadOnlyInInspector]  public  Timeframe                                                  scoreTimer                  = new(2.00, Timeframe.EaseInOut);
  [ReadOnlyInInspector]  private System.Collections.Generic.List<UnityEngine.Sprite>        sprites                     = new(1);
  [ReadOnlyInInspector]  private ref readonly UnityEngine.Camera?                           worldCamera                 { get { this._worldCamera ??= UnityEngine.Camera.main; return ref this._worldCamera; } }

  /* … */
  private void Awake() {
    if (null != UI.main && UI.main != this) {
      UnityEngine.Object.DestroyImmediate(this, false);
      return;
    }

    UI.main                                           = this;
    UI.main.gameObject.isStatic                       = true;
    UI.main.HUD.containers                            = new[] {UI.main.HUD.desktopContainers, UI.main.HUD.mobileContainers};
    UI.main.HUD.controls                              = new[] {UI.main.HUD.desktopControls,   UI.main.HUD.mobileControls};
    UnityEngine.Screen.autorotateToLandscapeLeft      = true;
    UnityEngine.Screen.autorotateToLandscapeRight     = true;
    UnityEngine.Screen.autorotateToPortrait           = true;
    UnityEngine.Screen.autorotateToPortraitUpsideDown = true;
    UnityEngine.Screen.orientation                    = UnityEngine.ScreenOrientation.AutoRotation;
    UnityEngine.Screen.sleepTimeout                   = UnityEngine.SleepTimeout.NeverSleep;

    UI.main.OnRectTransformDimensionsChange();
  }

  public void ChangeActivity(UI.Activity activity, UI.ActivityState state = UI.ActivityState.Any) {
    UI.main.activity      = activity;
    UI.main.activityState = state;

    if (UI.ActivityState.Any == state)
    switch (activity) {
      case UI.Activity.HUD:      UI.main.activityState = UI.ActivityState.Play; break;
      case UI.Activity.MainMenu: UI.main.activityState = UI.ActivityState.Main; break;
      default: break;
    }
  }

  public void ChangeContainer(UnityEngine.CanvasGroup container, UI.ContainerVisibility visibility) => UI.main.ChangeContainer(container, visibility, new(0.0));
  public void ChangeContainer(UnityEngine.CanvasGroup container, UI.ContainerVisibility visibility, in Timeframe transition) {
    switch (visibility) {
      case UI.ContainerVisibility.Hidden: {
        container.alpha          = 1.0f - (float) transition.progress;
        container.blocksRaycasts = false; // ->> “Block Ray Casts,” seriously Unity? What kind of backwards naming is this?
        container.interactable   = false;
      } break;

      case UI.ContainerVisibility.Visible: {
        container.alpha          = 0.0f + (float) transition.progress;
        container.blocksRaycasts = true;
        container.interactable   = true;
      } break;
    }
  }

  public void ChangeLayout(UI.Layout layout) {
    UI.main.layout = layout;

    switch (layout) {
      case UI.Layout.Desktop: {
        UI.main.HUD.layout           = UI.main.HUD.desktop;
        UI.main.HUD.layoutContainers = UI.main.HUD.desktopContainers;
        UI.main.HUD.layoutControls   = UI.main.HUD.desktopControls;
      } break;

      case UI.Layout.Mobile: {
        UI.main.HUD.layout           = UI.main.HUD.mobile;
        UI.main.HUD.layoutContainers = UI.main.HUD.mobileContainers;
        UI.main.HUD.layoutControls   = UI.main.HUD.mobileControls;
      } break;

      default: break;
    }
  }

  private Player? FindPlayer() {
    if (UI.main.worldCamera is UnityEngine.Camera worldCamera) {
      System.Collections.Generic.SortedList<float, Player> players = new(1);

      // …
      foreach (Entity entity in Entity.All)
      if (entity is Player player && player.enabled) {
        float distance = (player.transform.position - worldCamera.transform.position).sqrMagnitude;

        // …
        distance = !player.tracking.cameras.Contains(worldCamera) ? -1.0f / distance : distance;

        if (!players.ContainsKey(distance))
        players.Add(distance, player);
      }

      return 0 != players.Count ? players.Values[0] /* --> players.GetValueAtIndex(0) */ : null;
    }

    return UnityEngine.Object.FindFirstObjectByType<Player>(UnityEngine.FindObjectsInactive.Exclude);
  }

  private void OnDestroy() {
    if (UI.main != this)
    return;

    foreach (UnityEngine.Sprite sprite in UI.main.sprites)
      UnityEngine.Object.Destroy(sprite);

    UI.main = null!;
  }

  private void OnRectTransformDimensionsChange() {
    if (UI.main is not null)
    UI.main.ChangeLayout(UnityEngine.Screen.height <= UnityEngine.Screen.width || UnityEngine.Screen.orientation switch { UnityEngine.ScreenOrientation.LandscapeLeft or UnityEngine.ScreenOrientation.LandscapeRight => true, _ => false } ? UI.Layout.Desktop : UI.Layout.Mobile);
  }

  private void PlayClickSound() {
    if (null != this.clickAudioClip && base.TryGetComponent(out UnityEngine.AudioSource audioSource))
    audioSource.PlayOneShot(this.clickAudioClip, 0.5f);
  }

  private void Start() {
    static void TryHorizontalMove(in UnityEngine.Vector3 direction) {
      UI.main.entities.playerHorizontalMovementDirection = direction;

      if (UI.main.entities.playerHorizontalMovementDuration.isLooped) UI.main.StartCoroutine(TryHorizontalMoves());
      else                                                            UI.main.entities.playerHorizontalMovementDuration.Reset();
    }

    static System.Collections.IEnumerator TryHorizontalMoves() {
      if (Game.IsPaused || UI.main.entities.playerHorizontalMovementDuration.isElapsed) {
        UI.main.entities.playerHorizontalMovementDirection = UnityEngine.Vector3.zero;
        yield break;
      }

      if      (UI.main.entities.playerHorizontalMovementDirection == UnityEngine.Vector3.left)  UI.main.FindPlayer()?.TryMoveLeft (true);
      else if (UI.main.entities.playerHorizontalMovementDirection == UnityEngine.Vector3.right) UI.main.FindPlayer()?.TryMoveRight(true);

      yield return new UnityEngine.WaitForSecondsRealtime(0.0f);
      UI.main.StartCoroutine(TryHorizontalMoves());
    }

    static void TryVerticalMove(in UnityEngine.Vector3 direction) {
      UI.main.entities.playerVerticalMovementDirection = direction;

      if (UI.main.entities.playerVerticalMovementDuration.isLooped) UI.main.StartCoroutine(TryVerticalMoves());
      else                                                          UI.main.entities.playerVerticalMovementDuration.Reset();
    }

    static System.Collections.IEnumerator TryVerticalMoves() {
      if (Game.IsPaused || UI.main.entities.playerVerticalMovementDuration.isElapsed) {
        UI.main.entities.playerVerticalMovementDirection = UnityEngine.Vector3.zero;
        yield break;
      }

      if      (UI.main.entities.playerVerticalMovementDirection == UnityEngine.Vector3.down) UI.main.FindPlayer()?.TryMoveDown(true);
      else if (UI.main.entities.playerVerticalMovementDirection == UnityEngine.Vector3.up)   UI.main.FindPlayer()?.TryMoveUp  (true);

      yield return new UnityEngine.WaitForSecondsRealtime(0.0f);
      UI.main.StartCoroutine(TryVerticalMoves());
    }

    /* … ->> Unfortunately, no pointed cursor for `UnityEngine.Cursor.SetCursor(null, UnityEngine.Vector2.zero, UnityEngine.CursorMode.Auto)` */
    foreach (UI.HeadsUpDisplay.Controls controls in UI.main.HUD.controls) {
      foreach (UnityEngine.UI.Button button in controls.home)    button.onClick.AddListener(static delegate { UI.main.PlayClickSound(); UI.main.ChangeActivity(UI.Activity.MainMenu); Area.Reset(); Entity.Reset(); Stats.Reset(); Game.IsPaused = true; });
      foreach (UnityEngine.UI.Button button in controls.lasso)   button.onClick.AddListener(static delegate { UI.main.FindPlayer()?.TryLasso(); });
      foreach (UnityEngine.UI.Button button in controls.menu)    button.onClick.AddListener(static delegate { UI.main.PlayClickSound(); UI.main.ChangeActivity(UI.Activity.HUD, UI.ActivityState.Pause); Game.IsPaused = true; });
      foreach (UnityEngine.UI.Button button in controls.shoot)   button.onClick.AddListener(static delegate { UI.main.FindPlayer()?.TryShoot  (); });
      foreach (UnityEngine.UI.Button button in controls.release) button.onClick.AddListener(static delegate { UI.main.FindPlayer()?.TryRelease(); });
      foreach (UnityEngine.UI.Button button in controls.resume)  button.onClick.AddListener(static delegate { UI.main.PlayClickSound(); UI.main.ChangeActivity(UI.Activity.HUD, UI.ActivityState.Play); Game.IsPaused = false; });
      foreach (UnityEngine.UI.Button button in controls.stop)    button.onClick.AddListener(static delegate { UI.main.entities.playerHorizontalMovementDuration.Finish(); UI.main.entities.playerVerticalMovementDuration.Finish(); foreach (Entity entity in Entity.All) if (entity is Player) entity.rigidBody.angularVelocity = entity.rigidBody.linearVelocity = UnityEngine.Vector3.zero; });

      foreach (var (buttons, listener) in new System.ValueTuple<System.Collections.Generic.List<UnityEngine.UI.Button>, UnityEngine.Events.UnityAction>[] {
        (controls.moveDown,  static delegate { TryVerticalMove  (UnityEngine.Vector3.down); }),
        (controls.moveLeft,  static delegate { TryHorizontalMove(UnityEngine.Vector3.left); }),
        (controls.moveRight, static delegate { TryHorizontalMove(UnityEngine.Vector3.right); }),
        (controls.moveUp,    static delegate { TryVerticalMove  (UnityEngine.Vector3.up); })
      }) {
        foreach (UnityEngine.UI.Button button in buttons) {
          UnityEngine.EventSystems.EventTrigger.Entry buttonEvent   = new() {eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown};
          UnityEngine.EventSystems.EventTrigger       buttonTrigger = button.GetComponent<UnityEngine.EventSystems.EventTrigger>() ?? button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

          // …
          button.onClick      .AddListener(listener);
          buttonEvent.callback.AddListener((UnityEngine.EventSystems.BaseEventData buttonEventData) => listener());
          buttonTrigger.triggers.Add(buttonEvent);
        }
      }
    }

    foreach (UI.HeadsUpDisplay.Controls controls in UI.main.HUD.controls)
    foreach (System.Collections.Generic.List<UnityEngine.UI.Button> buttons in new[] {controls.home, controls.lasso, controls.menu, controls.moveDown, controls.moveLeft, controls.moveRight, controls.moveUp, controls.release, controls.resume, controls.shoot, controls.stop}) {
      for ((int index, bool marked) = (0, false); buttons.Count != index; ++index) {
        UnityEngine.UI.Button button = buttons[index];

        // …
        if (string.Equals(Game.SpecialTag, button.tag, System.StringComparison.OrdinalIgnoreCase))
          marked = true;

        else if (marked) {
          button.interactable = false;

          if (button.targetGraphic is not null)
          button.targetGraphic.color = button.targetGraphic.color.Transparent();
        }
      }
    }

    if (null != UI.main.buttons.chat) UI.main.buttons.chat.onClick.AddListener(static delegate { UI.main.PlayClickSound(); foreach (Entity entity in Entity.All) { if (entity is Player player) player.isChatting = true; } });
    if (null != UI.main.buttons.play) UI.main.buttons.play.onClick.AddListener(static delegate { UI.main.PlayClickSound(); System.Collections.IEnumerator DeployPlayer() { yield return new UnityEngine.WaitForEndOfFrame(); UnityEngine.Object.Instantiate(Assets.main.playerPrefabrication, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity); } Entity.Reset(true); UI.main.ChangeActivity(UI.Activity.HUD); UI.main.StartCoroutine(DeployPlayer()); Game.IsPaused = false; });
    if (null != UI.main.buttons.quit) UI.main.buttons.quit.onClick.AddListener(static delegate { Game.Quit(); });
  }

  public void TryChangeActivityToWin() {
    if (null != UI.main.HUD.layoutContainers!.win) {
      foreach (Entity entity in Entity.All) {
        if (entity is Player)
        entity.isInvincible = true;
      }

      if (0 != UI.main.HUD.layoutControls!.home.Count)
      UI.main.HUD.layoutControls!.home[0].onClick.Invoke();

      foreach (UI.HeadsUpDisplay.Containers containers in UI.main.HUD.containers) {
        if (null != containers.winText)
        containers.winText.text = "Thank you, Explorer!\nYour Quest is over\nWe present you a new Odyssey\n\nScore: " + Stats.Score;
      }

      UI.main.ChangeActivity(UI.Activity.HUD, UI.ActivityState.Win);
    }
  }

  private void Update() {
    int  activityIndex          = UI.main.activityTransitions.FindIndex(static entry => UI.main.activity == entry.activity);
    bool activityIsChanged      = UI.main.activity      != UI.main.activityPrior;
    bool activityStateIsChanged = UI.main.activityState != UI.main.activityStatePrior;
    var  entitiesRemove         = new {statistics = new {
      health = new bool[UI.main.entities.statistics.health.Count], // --> System.Collections.BitArray
      shoot  = new bool[UI.main.entities.statistics.shoot .Count]
    }};

    // … ->> Layout
    if (UnityEngine.Screen.height != this.applicationHeight || UnityEngine.Screen.width != this.applicationWidth) {
      this.applicationHeight = UnityEngine.Screen.height;
      this.applicationWidth  = UnityEngine.Screen.width;

      this.OnRectTransformDimensionsChange();
    }

    switch (UI.main.layout) {
      case UI.Layout.Desktop: {
        if (null != UI.main.HUD.desktop) {
          UI.main.ChangeContainer(UI.main.HUD.desktop, UI.ContainerVisibility.Visible);

          if (null != UI.main.HUD.mobile)
          UI.main.ChangeContainer(UI.main.HUD.mobile, UI.ContainerVisibility.Hidden);
        }
      } break;

      case UI.Layout.Mobile: {
        if (null != UI.main.HUD.mobile) {
          UI.main.ChangeContainer(UI.main.HUD.mobile, UI.ContainerVisibility.Visible);

          if (null != UI.main.HUD.desktop)
          UI.main.ChangeContainer(UI.main.HUD.desktop, UI.ContainerVisibility.Hidden);
        }
      } break;

      default: break;
    }

    // … ->> Control Switching
    if (UI.main.controlSwitchInterval.isLooped) {
      foreach (UI.HeadsUpDisplay.Controls controls in UI.main.HUD.controls)
      foreach (System.Collections.Generic.List<UnityEngine.UI.Button> buttons in new[] {controls.home, controls.lasso, controls.menu, controls.moveDown, controls.moveLeft, controls.moveRight, controls.moveUp, controls.release, controls.resume, controls.shoot, controls.stop}) {
        if (buttons.Count > 1)
        for (int index = buttons.Count; 0 != index--; ) {
          UnityEngine.UI.Button button = buttons[index];

          // …
          if (button.interactable && !string.Equals(Game.SpecialTag, button.tag, System.StringComparison.OrdinalIgnoreCase)) {
            for (int switchIndex = (index + 1) % buttons.Count; index != switchIndex; switchIndex = (switchIndex + 1) % buttons.Count) {
              UnityEngine.UI.Button switchButton = buttons[switchIndex];

              // …
              if (!string.Equals(Game.SpecialTag, switchButton.tag, System.StringComparison.OrdinalIgnoreCase)) {
                button      .interactable = false;
                switchButton.interactable = true;

                button      .targetGraphic?.CrossFadeColor(button      .targetGraphic.color.Transparent(), (float) UI.main.controlSwitchDuration.duration, true, true);
                switchButton.targetGraphic?.CrossFadeColor(switchButton.targetGraphic.color.Opaque     (), (float) UI.main.controlSwitchDuration.duration, true, true);

                break;
              }
            }

            break;
          }
        }
      }

      UI.main.controlSwitchDuration.Reset(); // ->> For no reason, thanks to the `UnityEngine.UI.Graphic::CrossFade*(…)` methods
    }

    // … ->> Activity Switching/ Background Transitioning
    if (activityIsChanged) {
      UI.main.background.transitionExit = UI.main.background.transitionEntry;

      UI.main.activityChangeDuration.Reset();
      UI.main.background.transition .Reset();
    }

    if (activityIndex != -1 && (null != UI.main.background.entry || null != UI.main.background.exit)) {
      var background = new {
        entry = null != UI.main.background.entry ? UI.main.background.entry : UI.main.background.exit!,
        exit  = null != UI.main.background.exit  ? UI.main.background.exit  : UI.main.background.entry!
      };

      /* … */
      static System.Collections.IEnumerator UnloadUnusedSprites() {
        yield return new UnityEngine.WaitUntil(static () => UI.main.activityChangeDuration.isElapsed);

        /* … */
        for ((var backgroundImage, int index) = (new {
          entry = null != UI.main.background.entry ? UI.main.background.entry as UnityEngine.UI.Image : null,
          exit  = null != UI.main.background.exit  ? UI.main.background.exit  as UnityEngine.UI.Image : null
        }, UI.main.sprites.Count); 0 != index--; ) {
          if (
            (null == backgroundImage.entry || UI.main.sprites[index] != backgroundImage.entry.sprite) &&
            (null == backgroundImage.exit  || UI.main.sprites[index] != backgroundImage.exit.sprite)
          ) {
            UI.main.sprites.RemoveAt(index);
            UnityEngine.Object.Destroy(UI.main.sprites[index]);
          }
        }

        yield break;
      }

      /* … */
      UI.main.background.transitionEntry = UI.main.activityTransitions[activityIndex];
      UI.main.ChangeContainer(UI.main.background.transitionEntry.container, UI.ContainerVisibility.Visible, null != UI.main.activityPrior ? UI.main.activityChangeDuration : new(0.0));

      for (int index = UI.main.activityTransitions.Count; 0 != index--; ) {
        if (activityIndex != index)
        UI.main.ChangeContainer(UI.main.activityTransitions[index].container, UI.ContainerVisibility.Hidden, null != UI.main.activityPrior ? UI.main.activityChangeDuration : new(0.0));
      }

      // … ->> Color
      if (background.entry != background.exit) {
        background.exit .color = UnityEngine.Color.LerpUnclamped(UI.main.background.transitionExit.color, UI.main.background.transitionExit .color.Transparent(), (float) UI.main.background.transition.progress);
        background.entry.color = (
          UI.main.background.transitionEntry.backgroundIsVisible
          ? UnityEngine.Color.LerpUnclamped(UI.main.background.transitionExit.color.Transparent(), UI.main.background.transitionEntry.color,               (float) UI.main.background.transition.progress)
          : UnityEngine.Color.LerpUnclamped(UI.main.background.transitionExit.color,               UI.main.background.transitionEntry.color.Transparent(), (float) UI.main.background.transition.progress)
        );
      }

      else if (activityIsChanged)
        background.entry.color = UI.main.background.transitionEntry.color.Opacity(UI.main.background.transitionEntry.backgroundIsVisible ? 1.0f : 0.0f);

      // … ->> Material, Sprite, Texture
      if (activityIsChanged) {
        background.exit .material = UI.main.background.transitionExit .material!;
        background.entry.material = UI.main.background.transitionEntry.material!;

        switch (background.exit) {
          case UnityEngine.UI.RawImage backgroundRawImage: backgroundRawImage.texture = UI.main.background.transitionExit.texture!; break;
          case UnityEngine.UI.Image    backgroundImage:    backgroundImage.sprite     = UI.main.background.transitionExit.sprite!;  break;
        }

        switch (background.entry) {
          case UnityEngine.UI.RawImage backgroundRawImage: backgroundRawImage.texture = UI.main.background.transitionEntry.texture!; break;
          case UnityEngine.UI.Image    backgroundImage:    backgroundImage.sprite     = UI.main.background.transitionEntry.sprite!; {
            UI.main.StartCoroutine(UnloadUnusedSprites()); // ->> ¯\ˍ(ツ)ˍ/¯

            if (null == UI.main.background.transitionEntry.sprite && null != UI.main.background.transitionEntry.texture) {
              backgroundImage.sprite = UnityEngine.Sprite.Create(UI.main.background.transitionEntry.texture, new(0.0f, 0.0f, UI.main.background.transitionEntry.texture.width, UI.main.background.transitionEntry.texture.height), new(0.5f, 0.5f));
              UI.main.sprites.Add(backgroundImage.sprite);
            }
          } break;
        }
      }
    }

    UI.main.activityPrior = UI.main.activity;

    // … ->> Activity State
    if (activityStateIsChanged)
    UI.main.activityStateChangeDuration.Reset();

    switch (UI.main.activity) {
      case UI.Activity.HUD: {
        if (activityIndex != -1)
        UI.main.activityTransitions[activityIndex].backgroundIsVisible = UI.ActivityState.Play != UI.main.activityState;

        foreach (UI.HeadsUpDisplay.Containers containers in UI.main.HUD.containers)
        switch (UI.main.activityState) {
          case UI.ActivityState.Gameover: {
            if (null != containers.gameover) UI.main.ChangeContainer(containers.gameover, UI.ContainerVisibility.Visible, UI.main.activityStateChangeDuration);
            if (null != containers.pause)    UI.main.ChangeContainer(containers.pause,    UI.ContainerVisibility.Hidden,  new(0.0));
            if (null != containers.win)      UI.main.ChangeContainer(containers.win,      UI.ContainerVisibility.Hidden,  new(0.0));
          } break;

          case UI.ActivityState.Pause: {
            if (null != containers.gameover) UI.main.ChangeContainer(containers.gameover, UI.ContainerVisibility.Hidden,  new(0.0));
            if (null != containers.pause)    UI.main.ChangeContainer(containers.pause,    UI.ContainerVisibility.Visible, UI.main.activityStateChangeDuration);
            if (null != containers.win)      UI.main.ChangeContainer(containers.win,      UI.ContainerVisibility.Hidden,  new(0.0));
          } break;

          case UI.ActivityState.Play:
          default: {
            if (null != containers.gameover) UI.main.ChangeContainer(containers.gameover, UI.ContainerVisibility.Hidden, new(0.0));
            if (null != containers.pause)    UI.main.ChangeContainer(containers.pause,    UI.ContainerVisibility.Hidden, new(0.0));
            if (null != containers.win)      UI.main.ChangeContainer(containers.win,      UI.ContainerVisibility.Hidden, new(0.0));
          } break;

          case UI.ActivityState.Win: {
            if (null != containers.gameover) UI.main.ChangeContainer(containers.gameover, UI.ContainerVisibility.Hidden,  new(0.0));
            if (null != containers.pause)    UI.main.ChangeContainer(containers.pause,    UI.ContainerVisibility.Hidden,  new(0.0));
            if (null != containers.win)      UI.main.ChangeContainer(containers.win,      UI.ContainerVisibility.Visible, UI.main.activityStateChangeDuration);
          } break;
        }
      } break;

      case UI.Activity.MainMenu:
      default: break;
    }

    UI.main.activityStatePrior = UI.main.activityState;

    // … ->> Activity
    if (UI.Activity.HUD == UI.main.activity && UI.main.activityState switch { UI.ActivityState.Gameover or UI.ActivityState.Win => false, _ => true })
    UI.main.ChangeActivity(UI.main.activity, Game.IsPaused ? UI.ActivityState.Pause : UI.ActivityState.Play);

    // … ->> Score
    foreach (UI.HeadsUpDisplay.Containers containers in UI.main.HUD.containers) {
      if (null != containers.score)
      containers.score.text = ((ulong) ((double) Stats.ScorePrior + (((double) Stats.Score - Stats.ScorePrior) * UI.main.scoreTimer.easedProgress))).ToString(
        UI.main.HUD.desktopContainers == containers ? "D6" :
        UI.main.HUD.mobileContainers  == containers ? "D5" :
        "D0"
      );
    }

    // … ->> Statistics
    System.Array.Fill(entitiesRemove.statistics.health, true);
    System.Array.Fill(entitiesRemove.statistics.shoot,  true);

    foreach (var (statistics, statisticsRemove, DiagnoseStatistic) in new System.ValueTuple<System.Collections.Generic.List<(Entity entity, UnityEngine.RectTransform graphic)>, bool[], System.Func<Entity, (UnityEngine.RectTransform? prefabrication, float value)?>>[] {
      (UI.main.entities.statistics.health, entitiesRemove.statistics.health, static entity => !entity.isInvincible && entity.statistics.health && (entity is not Monster monster || !monster.isMounted) ? (Assets.main.UI.entities.healthStatisticPrefabrication, (float) entity.health)                       : null),
      (UI.main.entities.statistics.shoot,  entitiesRemove.statistics.shoot,  static entity => entity.statistics.shoot                                                                                   ? (Assets.main.UI.entities.shootStatisticPrefabrication,  (float) entity.shoot.cooldown.easedProgress) : null)
    }) {
      if (UI.Activity.HUD == UI.main.activity)
      foreach (Entity entity in Entity.All) {
        UnityEngine.RectTransform? statisticPrefabrication = DiagnoseStatistic(entity)?.prefabrication;
        bool                       statisticAutomatically  = !entity.isDefeated && null != statisticPrefabrication;
        int                        index                   = statistics.Count;

        // …
        while (0 != index--) {
          if (entity == statistics[index].entity)
          break;
        }

        if (index != -1) {
          statisticsRemove[index] = !statisticAutomatically;
          continue;
        }

        if (statisticAutomatically) {
          UnityEngine.RectTransform statisticGraphic = (UnityEngine.RectTransform) ((UnityEngine.GameObject) UnityEngine.Object.Instantiate(statisticPrefabrication!.gameObject, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity)).transform;

          // …
          statistics.Add((entity, statisticGraphic));
          statisticGraphic.SetParent(UI.main.rectTransform, false);
        }
      }

      for (int index = statisticsRemove.Length; 0 != index--; )
      if (statisticsRemove[index]) {
        UnityEngine.Object.Destroy(statistics[index].graphic.gameObject);
        statistics.RemoveAt(index);
      }

      // …
      if (UI.Activity.HUD == UI.main.activity)
      foreach ((Entity entity, UnityEngine.RectTransform graphic) in statistics) {
        (UnityEngine.RectTransform? statisticPrefabrication, float statisticValue) = ((UnityEngine.RectTransform?, float)) DiagnoseStatistic(entity)!;
        System.Collections.IEnumerator enumerator            = graphic.GetEnumerator();
        UnityEngine.RectTransform      statisticGraphic      = enumerator.MoveNext() ? (UnityEngine.RectTransform) enumerator.Current : graphic;
        float                          statisticGraphicWidth = statisticPrefabrication?.rect.width ?? 100.0f; // ->> Presumed

        // …
        graphic.localScale = (UnityEngine.Vector3.forward + UnityEngine.Vector3.up) + (UnityEngine.Vector3.right * (entity is Player ? 1.00f : 0.45f));

        if (graphic != statisticGraphic) {
          statisticGraphicWidth = graphic.rect.width;
          graphic.SetSizeWithCurrentAnchors(UnityEngine.RectTransform.Axis.Horizontal, statisticGraphicWidth);
        }

        graphic.SetParent      (UI.main.HUD.layout!.transform, false);
        graphic.SetSiblingIndex(0);
        statisticGraphic.SetSizeWithCurrentAnchors(UnityEngine.RectTransform.Axis.Horizontal, statisticGraphicWidth * statisticValue);

        if (entity.worldCamera is not null) {
          UnityEngine.Vector3 entityPosition   = entity.transform.position;
          UnityEngine.Vector2 graphicExtents   = UnityEngine.RectTransformUtility.PixelAdjustRect(graphic, UI.main.canvas).size * 0.5f;
          UnityEngine.Vector2 graphicOffset    = new(0.0f, 20.0f + (UI.main.entities.statistics.shoot == statistics ? -17.5f : 0.0f)); // ->> Presumed
          UnityEngine.Vector2 graphicPosition  = entity.worldCamera.WorldToViewportPoint(new(entityPosition.x, entityPosition.y - entity.bounce.estimatedHeight, entityPosition.z));
          UnityEngine.Vector2 graphicPrecision = new(1.0f, 1.0f); // ->> Previously `new(50.0f, 30.0f)`
          UnityEngine.Vector2 uiExtents        = new UnityEngine.Vector2(this.applicationWidth, this.applicationHeight) * 0.5f;

          // …
          graphicPosition          = (UnityEngine.Vector2.Scale(graphicPosition, UI.main.rectTransform.sizeDelta) - (UI.main.rectTransform.sizeDelta * 0.5f)) / UI.main.canvas.scaleFactor;
          graphicPosition          = new(graphicPrecision.x * UnityEngine.Mathf.Round(graphicPosition.x / graphicPrecision.x), graphicPrecision.y * UnityEngine.Mathf.Round(graphicPosition.y / graphicPrecision.y)); // ->> Snap between positions
          graphicPosition          = new(uiExtents.x < graphicExtents.x + UnityEngine.Mathf.Abs(graphicPosition.x) ? (uiExtents.x - (/* graphic.localScale.x * */ graphicExtents.x * 2.0f)) * UnityEngine.Mathf.Sign(graphicPosition.x) : graphicPosition.x, uiExtents.y < graphicExtents.y + UnityEngine.Mathf.Abs(graphicPosition.y) ? (uiExtents.y - (/* graphic.localScale.y * */ graphicExtents.y * 2.0f)) * UnityEngine.Mathf.Sign(graphicPosition.y) : graphicPosition.y); // ->> Prevent bounds clipping
          graphic.anchoredPosition = UnityEngine.Vector2.LerpUnclamped(graphic.anchoredPosition, graphicOffset + graphicPosition, 0.1f);
          graphic.anchorMax        = new(0.5f, 0.5f);
          graphic.anchorMin        = new(0.5f, 0.5f);
        }
      }
    }
  }
}
