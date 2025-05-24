using PatchOdyssey;

/* … */
[PatchExecutionOrder(PatchBehaviour.DefaultExecutionOrder + 1)]
[UnityEngine.RequireComponent(typeof(UnityEngine.Canvas))]
[UnityEngine.RequireComponent(typeof(UnityEngine.CanvasRenderer))]
[UnityEngine.RequireComponent(typeof(UnityEngine.EventSystems.EventSystem))]
[UnityEngine.RequireComponent(typeof(UnityEngine.UI.GraphicRaycaster))]
[UnityEngine.RequireComponent(typeof(UnityEngine.RectTransform))]
public sealed class UI : UnityEngine.MonoBehaviour {
  public static class Load {
    [PatchMethod(AggressiveInlining)]
    public static void UriAsTexture2D(string path, System.Action<UnityEngine.Texture2D> callback, System.Action? fallback = null) => Util.Load.UriAsTexture2D(new(System.IO.Path.Combine(new[] {Util.Path.Assets, "UI", path})), (object? target, in Events.LoadEvent data) => {
      if (data.payload is not null) callback((UnityEngine.Texture2D) data.payload);
      else                          fallback?.Invoke();
    }, Util.Load.Asynchronously, Util.Load.WithReadOnlyCache, 3u, fallback is null ? static (object? _, in Events.LoadEvent _) => {} : (object? _, in Events.LoadEvent _) => fallback());
  }

  /* … */
  public static          UI                                                                               Main                      = default!;
  public static readonly System.Collections.Generic.List<(UnityEngine.UI.Graphic graphic, bool selected)> PointedList               = new();
  public static readonly double                                                                           SplashLoadDurationMinimum = 10.0 + (Util.Random() * 3.0);

  [ReadWriteInInspector]            private UnityEngine.Canvas                   canvas              =  default!;
  [ReadWriteInInspector]            private UnityEngine.CanvasRenderer           canvasRenderer      =  default!;
  [ReadWriteInInspector]            public  GameObjectReadOnlyDictionary         components          =  new(new GameObjectDictionary(10u) {{"background", null!}, {"combat", null!}, {"credits", null!}, {"dialogue", null!}, {"inventory", null!}, {"menu", null!}, {"pause", null!}, {"splash", null!}, {"tooltips:HUD", null!}, {"tooltips:world", null!}});
  [ReadWriteInInspector]            private UnityEngine.EventSystems.EventSystem eventSystem         =  default!;
  [ReadWriteInInspector]            private UnityEngine.UI.GraphicRaycaster      graphicsRaycaster   =  default!;
  [ReadWriteInInspector]            private UnityEngine.UI.Button[]              menuButtons         =  System.Array.Empty<UnityEngine.UI.Button>();
  [ReadWriteInInspector]            private UnityEngine.Vector2  []              menuButtonsOrigins  =  System.Array.Empty<UnityEngine.Vector2>  ();
  [ReadWriteInInspector]            private UnityEngine.Vector2                  menuLogoOrigin      =  UnityEngine.Vector2.zero;
  [ReadWriteInInspector]            private UISequence                           menuEntryAnimation  =  default!;
  [ReadWriteInInspector]            public  UnityEngine.RectTransform            rectTransform       => (UnityEngine.RectTransform) this.transform;
  [ReadWriteInInspector]            public  bool                                 skipSplashLoad      =  false;
  [ReadWriteInInspector]            private UISequence                           splashExitAnimation =  default!;
  [ReadWriteInInspector]            private uint                                 splashHintCount     =  0u;
  [ReadWriteInInspector]            private uint                                 splashHintIndex     =  default!;
  [UnityEngine.Range(0.0f, 100.0f)] public  float                                splashHintThreshold =  17.5f;
  [ReadWriteInInspector]            private UISequence                           splashLoadAnimation =  default!;
  [ReadWriteInInspector]            public  double                               splashLoadDuration  =  UI.SplashLoadDurationMinimum;
  [ReadWriteInInspector]            private UnityEngine.Vector2                  splashOrigin        =  UnityEngine.Vector2.zero;

  /* … */
  private void Awake() {
    UISequence menuEntryAnimation  = new(1.5,                              0.75,                                                                             Animation.Function.EaseIn,  UISequence.Idle, UISequence.Idle);
    UISequence splashExitAnimation = new(!this.skipSplashLoad ? 0.5 : 0.0, !this.skipSplashLoad ? System.Math.Max(this.splashLoadDuration - 0.5, 0.0) : 0.0, Animation.Function.EaseOut, UISequence.Idle, UISequence.Idle);
    UISequence splashLoadAnimation = new(!this.skipSplashLoad ? this.splashLoadDuration : 0.0, Animation.Function.Linear,
      new() {{"background-top", 1.0}, {"progress", 0.0}, {"transparency", 1.0}},
      new() {{"background-top", 1.0}, {"progress", 1.0}, {"transparency", 1.0}}
    ) {
      {Util.Perc(15.0), new() {{"background-top", 0.0}, {"transparency", 0.0}}},
      {Util.Perc(50.0), new() {{"background-top", 0.0}}},
      {Util.Perc(67.5), new() {{"transparency",   0.0}}}
    };

    // …
    UI.Main ??= this;

    this.canvas                             = this.GetComponent<UnityEngine.Canvas>();
    this.canvas.pixelPerfect                = true;
    this.canvasRenderer                     = this.GetComponent<UnityEngine.CanvasRenderer>();
    this.canvasRenderer.cullTransparentMesh = false;
    this.components["background"]         ??= UnityEngine.GameObject.Find("UI/Background");
    this.components["combat"]             ??= UnityEngine.GameObject.Find("UI/Combat");
    this.components["credits"]            ??= UnityEngine.GameObject.Find("UI/Credits");
    this.components["dialogue"]           ??= UnityEngine.GameObject.Find("UI/Dialogue");
    this.components["inventory"]          ??= UnityEngine.GameObject.Find("UI/Inventory");
    this.components["menu"]               ??= UnityEngine.GameObject.Find("UI/Menu");
    this.components["pause"]              ??= UnityEngine.GameObject.Find("UI/Pause");
    this.components["splash"]             ??= UnityEngine.GameObject.Find("UI/Splash");
    this.components["tooltips:HUD"]       ??= UnityEngine.GameObject.Find("UI/Tooltips/HUD");
    this.components["tooltips:world"]     ??= UnityEngine.GameObject.Find("UI/Tooltips/World") ?? UnityEngine.GameObject.Find("UI/Tooltips/Scene") ?? UnityEngine.GameObject.Find("UI/Tooltip");
    this.eventSystem                        = this.GetComponent<UnityEngine.EventSystems.EventSystem>();
    this.graphicsRaycaster                  = this.GetComponent<UnityEngine.UI.GraphicRaycaster>     ();
    this.menuButtons                        = this.components["menu"]?.FindDescendantsByComponent<UnityEngine.UI.Button>().ToArray() ?? this.menuButtons;
    this.menuButtonsOrigins                 = this.menuButtons.ConvertAll(static menuButton => ((UnityEngine.RectTransform) menuButton.transform).anchoredPosition);
    this.menuLogoOrigin                     = (this.components["menu"]?.FindChildByName("Logo")?.transform as UnityEngine.RectTransform)?.anchoredPosition ?? this.menuLogoOrigin;
    this.menuEntryAnimation                 = menuEntryAnimation;
    this.splashExitAnimation                = splashExitAnimation;
    this.splashLoadAnimation                = splashLoadAnimation;
    this.splashOrigin                       = (this.components["splash"]?.transform as UnityEngine.RectTransform)?.anchoredPosition ?? this.splashOrigin;

    foreach (UnityEngine.UI.MaskableGraphic maskableGraphic in this.FindHierarchyByComponent<UnityEngine.UI.MaskableGraphic>())
    maskableGraphic.maskable = false;
  }

  private void Start() {
    UnityEngine.GameObject splash = this.components["splash"];

    // … ⟶ Ensure “splash” component is animatable
    if (null != splash) {
      UnityEngine.RectTransform splashTransform = (UnityEngine.RectTransform) splash.transform;

      // … ⟶ Square the dimensions of the “splash” component
      splashTransform.anchorMax = splashTransform.anchorMin = splashTransform.pivot = new(0.5f, 0.0f);
      splashTransform.SetSize(UnityEngine.Vector2.one * UnityEngine.Mathf.Max(splashTransform.rect.height, splashTransform.rect.width));

      this.splashOrigin = splashTransform.anchoredPosition;

      // … ⟶ Allow “splash” component (elements) to render transparently
      foreach (UnityEngine.CanvasRenderer splashRenderer in splash.FindHierarchyByComponent<UnityEngine.CanvasRenderer>())
      splashRenderer.cullTransparentMesh = false;
    }

    // … ⟶ Ensure “menu” component is animatable
    this.menuEntryAnimation.Reset();
  }

  private void Update() {
    UnityEngine.GameObject menu            = this.components["menu"];
    uint                   menuButtonCount = null != menu ? (uint) menu.CountDescendantsByComponent<UnityEngine.UI.Button>()! : (uint) this.menuButtons.Length;
    UnityEngine.GameObject splash          = this.components["splash"];

    // …
    this.canvas.additionalShaderChannels &= ~(UnityEngine.AdditionalCanvasShaderChannels.Normal | UnityEngine.AdditionalCanvasShaderChannels.Tangent); // ⟶ Is this alright?

    // … ⟶ Track pointed `UnityEngine.UI.Graphic`s
    UI.PointedList.Clear();

    foreach (ref readonly PointerInfo pointer in Util.Pointers.Any) {
      System.Collections.Generic.List<UnityEngine.UI.Graphic> pointedList = Util.Pointers.Raycast(this.graphicsRaycaster, this.eventSystem, in pointer);

      // …
      if (UI.PointedList.Capacity < UI.PointedList.Count + pointedList.Count)
      UI.PointedList.Capacity = UI.PointedList.Count + pointedList.Count;

      foreach (UnityEngine.UI.Graphic pointed in pointedList) {
        if (!UI.PointedList.Exists(_ => _.graphic == pointed))
        UI.PointedList.Add((pointed, pointer.IsPointing()));
      }
    }

    // … ⟶ Track “menu” component buttons
    if (null != menu && menuButtonCount != this.menuButtons.Length) {
      UnityEngine.UI.Button[] menuButtons        = Util.Array<UnityEngine.UI.Button>.Create(menuButtonCount);
      UnityEngine.Vector2  [] menuButtonsOrigins = Util.Array<UnityEngine.Vector2>  .Create(menuButtonCount);

      // …
      foreach (UnityEngine.UI.Button menuButton in menu.FindDescendantsByComponent<UnityEngine.UI.Button>()) {
        int index = this.menuButtons.IndexOf(menuButton);

        // …
        --menuButtonCount;

        if (index != -1) {
          menuButtons       [menuButtonCount] = this.menuButtons       [index];
          menuButtonsOrigins[menuButtonCount] = this.menuButtonsOrigins[index];
        }

        else {
          menuButtons       [menuButtonCount] = menuButton;
          menuButtonsOrigins[menuButtonCount] = ((UnityEngine.RectTransform) menuButton.transform).anchoredPosition;
        }
      }

      menuButtonCount         = (uint) this.menuButtons.Length; // ⟶ Redundant
      this.menuButtons        = menuButtons;
      this.menuButtonsOrigins = menuButtonsOrigins;
    }

    // … ⟶ Invoke/ Resprite pointed `UnityEngine.UI.Graphic`s
    foreach ((UnityEngine.UI.Graphic pointed, bool selected) in UI.PointedList) {
      foreach (UnityEngine.UI.Button menuButton in this.menuButtons)
      if (menuButton.gameObject == pointed.gameObject) {
        if (menuButton.tag == "MenuQuitButton") {
          UI.Load.UriAsTexture2D("menu-button-4.png", menuButton.SetTexture);

          if (selected)
          Util.Game.Quit();
        }

        break;
      }
    }

    // … ⟶ Animate “splash” component
    if (null != splash) {
      UnityEngine.RectTransform  splashTransform                  = (UnityEngine.RectTransform) splash.transform;
      UnityEngine.Vector2        splashSize                       = UnityEngine.Vector2.Max(this.rectTransform.GetSize(), splashTransform.GetSize());
      UnityEngine.RectTransform? splashProgressBar                = splash.FindChildByName("Progress")?.transform as UnityEngine.RectTransform;
      float                      splashLoadAnimationProgress      = (float) this.splashLoadAnimation.progress;
      float                      splashLoadAnimationOpacity       = 1.0f - Util.Cast<float>(this.splashLoadAnimation["transparency"]);
      float                      splashLoadAnimationBackgroundTop = Util.Cast<float>(this.splashLoadAnimation["background-top"]);
      TMPro.TextMeshProUGUI?     splashHintText                   = splash.FindDescendantByName("Hint")?.GetComponent<TMPro.TextMeshProUGUI>();
      float                      splashExitAnimationProgress      = UnityEngine.Mathf.Max((float) this.splashExitAnimation.progress, 0.0f);
      UnityEngine.UI.RawImage?   splashBackground                 = splash.FindChildByName("Background")?.GetComponent<UnityEngine.UI.RawImage>();

      // … ⟶ Generate a new “splash” general hint
      if (splashLoadAnimationProgress >= this.splashHintCount * Util.Perc(this.splashHintThreshold)) {
        this.splashHintCount++;
        this.splashHintIndex = (uint) (Game.GeneralHints.Length * Util.Random());

        if (null != splashHintText)
        splashHintText.text = Game.GeneralHints[this.splashHintIndex];
      }

      // … ⟶ Animate “splash”
      splash.SetAlpha(1.0f - splashExitAnimationProgress); // ⟶ `splashTransform.anchoredPosition = new(splashTransform.anchoredPosition.x, this.splashOrigin.y - (splashExitAnimationProgress * splashSize.y));`

      // … ⟶ Animate “splash” background
      if (null != splashBackground) {
        UnityEngine.RectTransform splashBackgroundTransform = splashBackground.rectTransform;

        // …
        splashBackground.SetAlpha(splashLoadAnimationOpacity);

        splashBackgroundTransform.anchorMax = splashBackgroundTransform.anchorMin = splashBackgroundTransform.pivot = new(0.5f, 0.0f);
        splashBackgroundTransform.anchoredPosition = new(splashBackgroundTransform.anchoredPosition.x, this.splashOrigin.y - (splashLoadAnimationBackgroundTop * splashSize.y));
      }

      // … ⟶ Animate “splash” progress bar
      if (null != splashProgressBar)
      splashProgressBar.SetWidth(splashLoadAnimationProgress * splashSize.x);

      // … ⟶ Animate “splash” text
      foreach (TMPro.TextMeshProUGUI splashText in splash.FindHierarchyByComponent<TMPro.TextMeshProUGUI>())
      splashText.SetAlpha(splashLoadAnimationOpacity);
    }

    // … ⟶ Animate “menu” component
    if (null != menu) {
      float                    menuEntryAnimationTop = (1.0f - (float) this.menuEntryAnimation.easedProgress) * 50.0f;
      UnityEngine.UI.RawImage? menuLogo              = menu.FindChildByName("Logo")?.GetComponent<UnityEngine.UI.RawImage>();

      // … ⟶ Wait until “splash” component is loaded
      if (!this.splashLoadAnimation.isDone)
      this.menuEntryAnimation.Reset();

      // … ⟶ Animate “menu” logo
      if (null != menuLogo) {
        UnityEngine.RectTransform menuLogoTransform = menuLogo.rectTransform;

        // …
        menuLogoTransform.pivot            = new(0.5f, 0.5f);
        menuLogoTransform.anchorMin        = new(0.0f, 1.0f);
        menuLogoTransform.anchorMax        = new(1.0f, 1.0f);
        menuLogoTransform.anchoredPosition = new(menuLogoTransform.anchoredPosition.x, this.menuLogoOrigin.y + menuEntryAnimationTop);
      }

      // … ⟶ Animate “menu” buttons
      for (uint index = (uint) this.menuButtons.Length; 0u != index--; ) {
        UnityEngine.UI.Button            menuButton          =     this.menuButtons       [index];
        ref readonly UnityEngine.Vector2 menuButtonOrigin    = ref this.menuButtonsOrigins[index];
        UnityEngine.RectTransform        menuButtonTransform = (UnityEngine.RectTransform) menuButton.transform;

        // …
        if (!UI.PointedList.Exists(pointed => menuButton.gameObject == pointed.graphic.gameObject)) {
          menuButtonTransform.pivot            = new(0.5f, 0.5f);
          menuButtonTransform.anchorMin        = new(0.0f, 0.0f);
          menuButtonTransform.anchorMax        = new(1.0f, 0.0f);
          menuButtonTransform.anchoredPosition = new(menuButtonTransform.anchoredPosition.x, menuButtonOrigin.y - menuEntryAnimationTop);
        }
      }
    }
  }
}
