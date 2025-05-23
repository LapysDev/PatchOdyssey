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
  public static readonly double SplashLoadDurationMinimum = 5.0 + (Util.Random() * 3.0);

  private     UnityEngine.Canvas                   canvas                          = default!;
  private     UnityEngine.CanvasRenderer           canvasRenderer                  = default!;
  public      GameObjectReadOnlyDictionary         components                      = new(new GameObjectDictionary(10u) {{"background", null!}, {"combat", null!}, {"credits", null!}, {"dialogue", null!}, {"inventory", null!}, {"menu", null!}, {"pause", null!}, {"splash", null!}, {"tooltips:HUD", null!}, {"tooltips:world", null!}});
  private     UnityEngine.EventSystems.EventSystem eventSystem                     = default!;
  private     UnityEngine.UI.GraphicRaycaster      graphicsRaycaster               = default!;
  public      bool                                 skipSplashLoad                  = false;
  private     UISequence                           splashLoadAnimation             = default!;
  public      double                               splashLoadDuration              = UI.SplashLoadDurationMinimum;
  public  new UnityEngine.RectTransform            transform { get; private set; } = default!;

  /* … */
  private void Awake() {
    UISequence splashLoadAnimation = new(this.skipSplashLoad ? 0.0 : this.splashLoadDuration,
      // new() {{"top", 1.0}, {"transparency", 1.0}},
      // new() {{"top", 1.0}, {"transparency", 1.0}}

      new() {{"top", 1.0}, {"transparency", 1.0}},
      new() {{"top", 1.0}, {"transparency", 0.0}}
    ) {
      // {Util.Perc(15.0), new() {{"top", 0.0}, {"transparency", 0.0}}},
      // {Util.Perc(50.0), new() {{"top", 0.0}}},
      // {Util.Perc(67.5), new() {{"transparency", 0.0}}}
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
    this.splashLoadAnimation                = splashLoadAnimation;
    this.transform                          = this.GetComponent<UnityEngine.RectTransform>();

    // …
    foreach (UnityEngine.UI.MaskableGraphic maskableGraphic in this.FindHierarchyByComponent<UnityEngine.UI.MaskableGraphic>())
    maskableGraphic.maskable = false;
  }

  private void Start() {
    UnityEngine.GameObject splash = this.components["splash"];

    // … ⟶ Square the dimensions of the “splash” component
    if (null != splash) {
      UnityEngine.RectTransform splashTransform = (UnityEngine.RectTransform) splash.transform;

      // … ⟶ Anchor “splash” component to center-bottom of the `UI`
      splashTransform.anchorMax = splashTransform.anchorMin = splashTransform.pivot = new(0.5f, 0.0f);
      splashTransform.SetSize(UnityEngine.Vector2.one * System.Math.Max(splashTransform.rect.height, splashTransform.rect.width));

      // … ⟶ Allow “splash” component (elements) to render transparently
      foreach (UnityEngine.CanvasRenderer splashRenderer in splash.FindHierarchyByComponent<UnityEngine.CanvasRenderer>())
      splashRenderer.cullTransparentMesh = false;
    }
  }

  private void Update() {
    UnityEngine.GameObject splash = this.components["splash"];

    // … ⟶ Animate “splash” component
    if (null != splash) {
      float                     splashLoadAnimationOpacity = 1.0f - Util.Cast<float>(this.splashLoadAnimation["transparency"]);
      double                    splashLoadAnimationTop     = Util.Cast<double>(this.splashLoadAnimation["top"]);
      UnityEngine.RectTransform splashTransform            = (UnityEngine.RectTransform) splash.transform;

      // …
      // position’s all fucked up somehow... HMMM I wonder fucking how?!
      foreach (UnityEngine.UI.RawImage splashImage in splash.FindHierarchyByComponent<UnityEngine.UI.RawImage>()) splashImage.color = new(splashImage.color.r, splashImage.color.g, splashImage.color.b, splashLoadAnimationOpacity);
      foreach (TMPro.TextMeshProUGUI   splashText  in splash.FindHierarchyByComponent<TMPro.TextMeshProUGUI>  ()) splashText .alpha = splashLoadAnimationOpacity;
    }

    // …
    foreach (PointerInfo pointer in Util.Pointers.Any) {
      System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult> raycasts = new();

      // …
      this.graphicsRaycaster.Raycast(new(this.eventSystem) {position = pointer.position}, raycasts);
    }
  }
}
