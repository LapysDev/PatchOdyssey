using PatchOdyssey;

/* … */
[PatchExecutionOrder(PatchBehaviour.DefaultExecutionOrder + 1)]
[UnityEngine.RequireComponent(typeof(UnityEngine.Canvas))]
[UnityEngine.RequireComponent(typeof(UnityEngine.CanvasRenderer))]
[UnityEngine.RequireComponent(typeof(UnityEngine.EventSystems.EventSystem))]
[UnityEngine.RequireComponent(typeof(UnityEngine.UI.GraphicRaycaster))]
[UnityEngine.RequireComponent(typeof(UnityEngine.RectTransform))]
public class UI : UnityEngine.MonoBehaviour {
  public static          UI     Main                      = default!;
  public static readonly double SplashLoadDurationMinimum = 10.0 + (Util.Random() * 3.0);

  private UnityEngine.Canvas                   canvas              = default!;
  private UnityEngine.CanvasRenderer           canvasRenderer      = default!;
  public  GameObjectReadOnlyDictionary         components          = new(new GameObjectDictionary(10u) {{"background", null!}, {"combat", null!}, {"credits", null!}, {"dialogue", null!}, {"inventory", null!}, {"menu", null!}, {"pause", null!}, {"splash", null!}, {"tooltips:HUD", null!}, {"tooltips:world", null!}});
  private UnityEngine.EventSystems.EventSystem eventSystem         = default!;
  private UnityEngine.UI.GraphicRaycaster      graphicsRaycaster   = default!;
  public  bool                                 skipSplashLoad      = false;
  private UISequence                           splashExitAnimation = default!;
  private uint                                 splashHintCount     = 0u;
  private uint                                 splashHintIndex     = default!;
  public  double                               splashHintThreshold = Util.Perc(17.5);
  private UISequence                           splashLoadAnimation = default!;
  public  double                               splashLoadDuration  = UI.SplashLoadDurationMinimum;
  private UnityEngine.Vector2                  splashOrigin        = UnityEngine.Vector2.zero;

  /* … */
  private void Awake() {
    UISequence splashExitAnimation = new(!this.skipSplashLoad ? 0.5 : 0.0, !this.skipSplashLoad ? System.Math.Max(this.splashLoadDuration - 0.5, 0.0) : 0.0, UISequence.Idle, UISequence.Idle);
    UISequence splashLoadAnimation = new(!this.skipSplashLoad ? this.splashLoadDuration : 0.0,
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
    this.splashExitAnimation                = splashExitAnimation;
    this.splashLoadAnimation                = splashLoadAnimation;
    this.splashOrigin                       = (this.components["splash"]?.transform as UnityEngine.RectTransform)?.anchoredPosition ?? this.splashOrigin;

    // …
    foreach (UnityEngine.UI.MaskableGraphic maskableGraphic in this.FindHierarchyByComponent<UnityEngine.UI.MaskableGraphic>())
    maskableGraphic.maskable = false;
  }

  private void Start() {
    UnityEngine.GameObject splash = this.components["splash"];

    // …
    if (null != splash) {
      UnityEngine.RectTransform splashTransform = (UnityEngine.RectTransform) splash.transform;

      // … ⟶ Anchor “splash” component to center-bottom of the `UI`
      // … ⟶ Square the dimensions of the “splash” component
      splashTransform.anchorMax = splashTransform.anchorMin = splashTransform.pivot = new(0.5f, 0.0f);
      splashTransform.SetSize(UnityEngine.Vector2.one * UnityEngine.Mathf.Max(splashTransform.rect.height, splashTransform.rect.width));

      this.splashOrigin = splashTransform.anchoredPosition;

      // … ⟶ Allow “splash” component (elements) to render transparently
      foreach (UnityEngine.CanvasRenderer splashRenderer in splash.FindHierarchyByComponent<UnityEngine.CanvasRenderer>())
      splashRenderer.cullTransparentMesh = false;
    }
  }

  private void Update() {
    UnityEngine.GameObject    splash    = this.components["splash"];
    UnityEngine.RectTransform transform = (UnityEngine.RectTransform) this.transform;

    // … ⟶ Animate “splash” component
    if (null != splash) {
      UnityEngine.RectTransform  splashTransform                  = (UnityEngine.RectTransform) splash.transform;
      UnityEngine.Vector2        splashSize                       = UnityEngine.Vector2.Max(transform.GetSize(), splashTransform.GetSize());
      UnityEngine.RectTransform? splashProgressBar                = splash.FindChildByName("Progress")?.transform as UnityEngine.RectTransform;
      float                      splashLoadAnimationProgress      = (float) this.splashLoadAnimation.progress;
      float                      splashLoadAnimationOpacity       = 1.0f - Util.Cast<float>(this.splashLoadAnimation["transparency"]);
      float                      splashLoadAnimationBackgroundTop = Util.Cast<float>(this.splashLoadAnimation["background-top"]);
      TMPro.TextMeshProUGUI?     splashHintText                   = splash.FindDescendantByName("Hint")?.GetComponent<TMPro.TextMeshProUGUI>();
      float                      splashExitAnimationProgress      = UnityEngine.Mathf.Max((float) this.splashExitAnimation.progress, 0.0f);
      UnityEngine.UI.RawImage?   splashBackground                 = splash.FindChildByName("Background")?.GetComponent<UnityEngine.UI.RawImage>();

      // … ⟶ Generate a new “splash” general hint
      if (splashLoadAnimationProgress >= this.splashHintCount * this.splashHintThreshold) {
        this.splashHintCount++;
        this.splashHintIndex = (uint) (Game.GeneralHints.Length * Util.Random());

        if (null != splashHintText)
        splashHintText.text = Game.GeneralHints[this.splashHintIndex];
      }

      // … ⟶ Animate “splash”
      splash.SetAlpha(1.0f - splashExitAnimationProgress); // ⟶ `splashTransform.anchoredPosition = new(splashTransform.anchoredPosition.x, this.splashOrigin.y - (splashExitAnimationProgress * splashSize.y));`

      // … ⟶ Animate “splash” background
      if (null != splashBackground) {
        UnityEngine.RectTransform splashBackgroundTransform = (UnityEngine.RectTransform) splashBackground.transform;

        // …
        splashBackground.SetAlpha(splashLoadAnimationOpacity);
        splashBackgroundTransform.anchoredPosition = new(splashBackgroundTransform.anchoredPosition.x, this.splashOrigin.y - (splashLoadAnimationBackgroundTop * splashSize.y));
      }

      // … ⟶ Animate “splash” progress bar
      if (null != splashProgressBar)
      splashProgressBar.SetWidth(splashLoadAnimationProgress * splashSize.x);

      // … ⟶ Animate “splash” text
      foreach (TMPro.TextMeshProUGUI splashText in splash.FindHierarchyByComponent<TMPro.TextMeshProUGUI>())
      splashText.SetAlpha(splashLoadAnimationOpacity);
    }

    // …
    foreach (PointerInfo pointer in Util.Pointers.Any) {
      System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult> raycasts = new();

      // …
      this.graphicsRaycaster.Raycast(new(this.eventSystem) {position = pointer.position}, raycasts);
    }
  }
}
