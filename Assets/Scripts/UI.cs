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
    [PatchMethod(NoInlining)]
    internal static void Idle<T>(T _) {}

    [PatchMethod(NoInlining)] public static void PlayMusic(UnityEngine.AudioSource audioSource) { if (null == audioSource) return; if (null != UI.CurrentMusic && UI.CurrentMusic.isPlaying) { UI.CurrentMusic.Stop(); } audioSource.Play        (); }
    [PatchMethod(NoInlining)] public static void PlaySound(UnityEngine.AudioSource audioSource) { if (null == audioSource) return;                                                                                       audioSource.PlayOneShot(audioSource.clip, audioSource.volume); }

    [PatchMethod(AggressiveInlining)]
    private static void UriAsAudio(System.Uri path, bool effect, bool immediate, System.Action<UnityEngine.AudioSource> callback, System.Action fallback) => Util.Load.UriAsAudioClip(path, Util.EvaluateAudioTypeFromExtension(System.IO.Path.GetExtension(path.ToString())), (object? _, in Events.LoadEvent data) => {
      if (data.payload is UnityEngine.AudioClip clip) {
        System.ReadOnlySpan<uint> semitones = stackalloc[] {0u, 2u, 4u, 7u, 9u}; // ⟶ Pentatonic scale intervals
        UnityEngine.AudioSource   source;

        // … ⟶ Cache the appropriate `UnityEngine.AudioSource` for the loaded `UnityEngine.AudioClip`
        if (!UI.AudioSources.TryGetValue(clip, out source)) {
          source      = new UnityEngine.GameObject((effect ? "🎵 " : "🎼 ") + System.IO.Path.GetFileNameWithoutExtension(data.path.ToString()), typeof(UnityEngine.RectTransform)).AddComponent<UnityEngine.AudioSource>();
          source.clip = clip;

          UI.AudioSources.Add(clip, source);
          if (null != UI.Main) source.transform.SetParent(UI.Main.transform);
        }

        // … ⟶ Configure the `UnityEngine.AudioSource` accordingly
        source.loop         = !effect;
        source.mute         = false;
        source.pitch        = effect && source.pitch == 1.0f ? UnityEngine.Mathf.Pow(2.0f, semitones.Random() / 12.0f) /* ⟶ 2ⁿᐟ¹² for semitone steps */ : source.pitch; /* 📅 📅 📅 */
        // source.spatialBlend = Util.Perc(effect ? 100.0f : 0.0f);
        source.volume       = Util.Perc(Settings.GetProperty<float>($"Options/{(effect ? "Sound" : "Music")}") ?? (effect ? 100.0f : 67.5f)); // ⟶ Already normalized (from logarithmic decibels)

        // … ⟶ Ensure the `UnityEngine.AudioSource` plays (that’s usually the reason it’s loaded)
        if (effect) {
          if (immediate)
          source.PlayOneShot(source.clip, source.volume);
        }

        else if (UI.CurrentMusic != source) {
          if (immediate && null != UI.CurrentMusic)
            UI.CurrentMusic.Stop();

          UI.CurrentMusic = source;
          if (immediate) source.Play();
        }

        // …
        callback(source);
      } else fallback();
    }, Util.Load.Asynchronously, Util.Load.WithReadOnlyCache, 3u, (object? _, in Events.LoadEvent _) => fallback());
      [PatchMethod(AggressiveInlining)] public static void UriAsMusic(string path,                 System.Action<UnityEngine.AudioSource> callback, System.Action? fallback = null) => Load.UriAsMusic(path, true, callback, fallback);
      [PatchMethod(AggressiveInlining)] public static void UriAsSound(string path,                 System.Action<UnityEngine.AudioSource> callback, System.Action? fallback = null) => Load.UriAsSound(path, true, callback, fallback);
      [PatchMethod(AggressiveInlining)] public static void UriAsMusic(string path, bool immediate, System.Action<UnityEngine.AudioSource> callback, System.Action? fallback = null) => Load.UriAsAudio(new(System.IO.Path.Combine(new[] {Util.Path.Assets, "Music", path})), false, immediate, callback, fallback ?? ([PatchMethod(AggressiveInlining)] static () => {}));
      [PatchMethod(AggressiveInlining)] public static void UriAsSound(string path, bool immediate, System.Action<UnityEngine.AudioSource> callback, System.Action? fallback = null) => Load.UriAsAudio(new(System.IO.Path.Combine(new[] {Util.Path.Assets, "SFX",   path})), true,  immediate, callback, fallback ?? ([PatchMethod(AggressiveInlining)] static () => {}));

    [PatchMethod(AggressiveInlining)]
    public static void UriAsTexture(string path, System.Action<UnityEngine.Texture> callback, System.Action? fallback = null) => Util.Load.UriAsTexture2D(new(System.IO.Path.Combine(new[] {Util.Path.Assets, "UI", path})), (object? _, in Events.LoadEvent data) => {
      if (data.payload is UnityEngine.Texture texture) callback(texture);
      else                                             fallback?.Invoke();
    }, Util.Load.Asynchronously, Util.Load.WithReadOnlyCache, 3u, fallback is null ? static (object? _, in Events.LoadEvent _) => {} : (object? _, in Events.LoadEvent _) => fallback());
  }

  public static class Preload {
    [PatchMethod(AggressiveInlining)] public static void UriAsMusic  (string path) => Util.Preload.UriAsAudioClip(new(System.IO.Path.Combine(new[] {Util.Path.Assets, "Music", path})), Util.EvaluateAudioTypeFromExtension(System.IO.Path.GetExtension(path)));
    [PatchMethod(AggressiveInlining)] public static void UriAsSound  (string path) => Util.Preload.UriAsAudioClip(new(System.IO.Path.Combine(new[] {Util.Path.Assets, "SFX",   path})), Util.EvaluateAudioTypeFromExtension(System.IO.Path.GetExtension(path)));
    [PatchMethod(AggressiveInlining)] public static void UriAsTexture(string path) => Util.Preload.UriAsTexture2D(new(System.IO.Path.Combine(new[] {Util.Path.Assets, "UI",    path})));
  }

  public enum SelectionMode : byte { None, Pointed, Tabbed }

  /* … */
  private  static readonly System.Collections.Generic.Dictionary<UnityEngine.AudioClip, UnityEngine.AudioSource> AudioSources              = new(3);
  internal static          UnityEngine.AudioSource?                                                              CurrentMusic              = null;
  public   static          UI                                                                                    Main                      = default!;
  public   static readonly double                                                                                SplashLoadDurationMinimum = 10.0 + (Util.Random() * 3.0);

  [ReadOnlyInInspector, System.NonSerialized]        private           UnityEngine.Vector2                                          backgroundAnchoredPosition     =  UnityEngine.Vector2.zero;
  [ReadOnlyInInspector, System.NonSerialized]        private           bool                                                         backgroundIsLoaded             =  true;
  [ReadWriteInInspector]                             private           AnimationSequence                                            backgroundLoadAnimation        =  default!;
  [ReadWriteInInspector]                             private           AnimationSequence                                            buttonEntryAnimation           =  new("enter", 1.75, Animation.Function.EaseIn,  AnimationSequence.Idle,  AnimationSequence.Idle);
  [ReadWriteInInspector]                             public            float                                                        buttonEntryAnimationDelay      =  0.4f;
  [ReadWriteInInspector]                             private           AnimationSequence                                            buttonSelectedAnimation        =  new("hover", 1.75, new() {{"teeter", 0.0}}, new() {{"teeter", 0.0}}) {{Util.Perc(25.0), new() {{"teeter", -1.0}}}, {Util.Perc(50.0), new() {{"teeter", 0.0}}}, {Util.Perc(75.0), new() {{"teeter", +1.0}}}};
  [ReadOnlyInInspector]                              public            UI.SelectionMode                                             buttonSelectionMode            =  UI.SelectionMode.None;
  [ReadOnlyInInspector]                              private           UI.SelectionMode                                             buttonSelectionPreviousMode    =  UI.SelectionMode.None;
  [ReadOnlyInInspector, System.NonSerialized]        public            UnityEngine.UI.Button[]                                      buttons                        =  System.Array.Empty<UnityEngine.UI.Button>();
  [ReadOnlyInInspector, System.NonSerialized]        private           UnityEngine.Vector2  []                                      buttonsAnchoredPositions       =  System.Array.Empty<UnityEngine.Vector2>  ();
  [ReadOnlyInInspector, System.NonSerialized]        private           AnimationSequence    []                                      buttonsEntryAnimations         =  System.Array.Empty<AnimationSequence>    ();
  [ReadOnlyInInspector, System.NonSerialized]        private           UnityEngine.Vector3  []                                      buttonsLocalEulerAngles        =  System.Array.Empty<UnityEngine.Vector3>  ();
  [ReadOnlyInInspector, System.NonSerialized]        private           UnityEngine.Vector3  []                                      buttonsLocalPositions          =  System.Array.Empty<UnityEngine.Vector3>  (); // ⟶ Forgot about `::buttonsAnchoredPositions`
  [ReadOnlyInInspector, System.NonSerialized]        private           UnityEngine.Vector3  []                                      buttonsLocalScales             =  System.Array.Empty<UnityEngine.Vector3>  ();
  [ReadOnlyInInspector, System.NonSerialized]        private           UnityEngine.Texture  []                                      buttonsTextures                =  System.Array.Empty<UnityEngine.Texture>  ();
  [ReadOnlyInInspector]                              private           UnityEngine.Canvas                                           canvas                         =  default!;
  [ReadOnlyInInspector]                              private           UnityEngine.CanvasRenderer                                   canvasRenderer                 =  default!;
  [ReadOnlyInInspector]                              private           ref string                                                   componentCurrentLoaded         => ref this.componentLoaded;
  [ReadWriteInInspector]                             private           AnimationSequence                                            componentLoadAnimation         =  new("load", 1.25, 0.00, Animation.Function.EaseOut, AnimationSequence.Idle,  AnimationSequence.Idle);
  [ReadOnlyInInspector, UnityEngine.SerializeField]  private           string                                                       componentLoaded                =  "menu";
  [ReadOnlyInInspector]                              private           string                                                       componentPreviousLoaded        =  "menu";
  [ReadOnlyInInspector]                              private           string[]                                                     componentSpecials              =  new string[] {"background", "splash", "transition", "tooltips:HUD", "tooltips:world"}; // ⟶ Components that can not be (un-)loaded
  [ReadWriteInInspector]                             public            GameObjectReadOnlyDictionary                                 components                     =  new(new GameObjectDictionary(13u) {{string.Empty, null!}, {"background", null!}, {"combat", null!}, {"credits", null!}, {"dialogue", null!}, {"inventory", null!}, {"menu", null!}, {"options", null!}, {"pause", null!}, {"splash", null!}, {"tooltips:HUD", null!}, {"tooltips:world", null!}, {"transition", null!}});
  [ReadWriteInInspector]                             private           FloatReadOnlyDictionary                                      componentsAlphas               =  FloatReadOnlyDictionary              .Empty;
  [ReadWriteInInspector]                             private           ReadOnlyDictionary<AnimationSequence>                        componentsLoadAnimations       =  ReadOnlyDictionary<AnimationSequence>.Empty;
  [ReadWriteInInspector]                             private           Vector2ReadOnlyDictionary                                    componentsOrigins              =  Vector2ReadOnlyDictionary            .Empty;
  [ReadOnlyInInspector]                              private           UnityEngine.EventSystems.EventSystem                         eventSystem                    =  default!;
  [ReadOnlyInInspector]                              private           UnityEngine.UI.GraphicRaycaster                              graphicRaycaster               =  default!;
  [ReadWriteInInspector]                             private           UnityEngine.Vector2                                          menuLogoOrigin                 =  UnityEngine.Vector2.zero;
  [ReadWriteInInspector, UnityEngine.SerializeField] internal          UnityEngine.AudioSource?                                     music                          =  null; // ⟶ Editor-only
  [ReadOnlyInInspector,  System.NonSerialized]       private  readonly GameObjectReadOnlyDictionary                                 optionsProperties              =  new(new GameObjectDictionary(2u) {{"Music", null!}, {"Sound", null!}});
  [ReadOnlyInInspector,  System.NonSerialized]       private  readonly RefDictionary<UnityEngine.GameObject, (string, PointerInfo)> optionsSliderButtonInformation =  new(2u);
  [ReadWriteInInspector]                             public            UnityEngine.RectTransform                                    rectTransform                  => (UnityEngine.RectTransform) this.transform;
  [ReadWriteInInspector]                             private           UnityEngine.Vector2                                          splashBackgroundOrigin         =  UnityEngine.Vector2.zero;
  [ReadWriteInInspector]                             private           AnimationSequence                                            splashExitAnimation            =  new("splash:exit", default, Animation.Function.EaseOut, AnimationSequence.Idle, AnimationSequence.Idle);
  [ReadWriteInInspector]                             private           uint                                                         splashHintCount                =  0u;
  [ReadOnlyInInspector]                              private           uint                                                         splashHintIndex                =  default!;
  [UnityEngine.Range(0.0f, 100.0f)]                  public            float                                                        splashHintThreshold            =  17.5f;
  [ReadWriteInInspector]                             private           AnimationSequence                                            splashLoadAnimation            =  new("splash:load", default, new() {{"background-top", 1.0}, {"transparency", 1.0}}, new() {{"background-top", 1.0}, {"transparency", 1.0}}) {{Util.Perc(15.0), new() {{"background-top", 0.0}, {"transparency", 0.0}}}, {Util.Perc(50.0), new() {{"background-top", 0.0}}}, {Util.Perc(67.5), new() {{"transparency", 0.0}}}};
  [ReadWriteInInspector]                             public            double                                                       splashLoadDuration             =  UI.SplashLoadDurationMinimum;
  [ReadWriteInInspector]                             public            bool                                                         splashLoadSkipped              =  false;
  [ReadOnlyInInspector]                              private           UnityEngine.GameObject?                                      tabbed                         =  null;
  [ReadOnlyInInspector]                              private           ref UnityEngine.GameObject?                                  tabbedCurrent                  => ref this.tabbed;
  [ReadOnlyInInspector]                              private           UnityEngine.GameObject?                                      tabbedPrevious                 =  null;
  [ReadWriteInInspector]                             private           AnimationSequence                                            transitionAnimation            =  new("transition", default, new() {{"zoom", 1.0}}, new() {{"zoom", 1.0}}) {{Util.Perc(20.0), new() {{"zoom", 0.1}}}, {Util.Perc(30.0), new() {{"zoom", 0.2}}}, {Util.Perc(40.0), new() {{"zoom", 0.1}}}, {Util.Perc(65.0), new() {{"zoom", 0.0}}}, {Util.Perc(80.0), new() {{"zoom", 0.0}}}};
  [ReadWriteInInspector]                             public            bool                                                         transitionOnComponentLoad      =  false;
  [ReadWriteInInspector]                             private           UnityEngine.Vector2                                          transitionLocalSize            =  UnityEngine.Vector3.zero;
  [ReadWriteInInspector]                             public            double                                                       transitionSpeed                =  4.00;

  public TMPro.TextMeshProUGUI PROJECT_DEADLINE_CAME_UP          = null!; /* 📅 📅 📅 */
  public TMPro.TextMeshProUGUI PROJECT_DEADLINE_CAME_UP_SCO1     = null!; /* 📅 📅 📅 */
  public TMPro.TextMeshProUGUI PROJECT_DEADLINE_CAME_UP_SCO2     = null!; /* 📅 📅 📅 */
  public AnimationSequence     PROJECT_DEADLINE_CAME_UP_ANIM     = new(5.0, Animation.Function.EaseOut, AnimationSequence.Idle, AnimationSequence.Idle); /* 📅 📅 📅 */
  public uint                  PROJECT_DEADLINE_CAME_UP_COUNT    = 0u; /* 📅 📅 📅 */
  public double                PROJECT_DEADLINE_CAME_UP_SCORE    = 0.0; /* 📅 📅 📅 */
  public uint                  PROJECT_DEADLINE_CAME_UP_PRESCORE = 0u; /* 📅 📅 📅 */
  public uint                  PROJECT_DEADLINE_CAME_UP_MIN      = 0u; /* 📅 📅 📅 */
  public Monster               PROJECT_DEADLINE_CAME_UP_MON      = null!; /* 📅 📅 📅 */
  public UnityEngine.Transform PROJECT_DEADLINE_CAME_UP_LIFE      = null!; /* 📅 📅 📅 */

  /* … */
  private void Awake() {
    if (null == this.components[string.Empty])     this.components[string.Empty]     = UnityEngine.GameObject.Find("UI/…");
    if (null == this.components["background"])     this.components["background"]     = UnityEngine.GameObject.Find("UI/Background");
    if (null == this.components["combat"])         this.components["combat"]         = UnityEngine.GameObject.Find("UI/Combat");
    if (null == this.components["credits"])        this.components["credits"]        = UnityEngine.GameObject.Find("UI/Credits");
    if (null == this.components["dialogue"])       this.components["dialogue"]       = UnityEngine.GameObject.Find("UI/Dialogue");
    if (null == this.components["inventory"])      this.components["inventory"]      = UnityEngine.GameObject.Find("UI/Inventory");
    if (null == this.components["menu"])           this.components["menu"]           = UnityEngine.GameObject.Find("UI/Menu");
    if (null == this.components["options"])        this.components["options"]        = UnityEngine.GameObject.Find("UI/Options");
    if (null == this.components["pause"])          this.components["pause"]          = UnityEngine.GameObject.Find("UI/Pause");
    if (null == this.components["splash"])         this.components["splash"]         = UnityEngine.GameObject.Find("UI/Splash");
    if (null == this.components["transition"])     this.components["transition"]     = UnityEngine.GameObject.Find("UI/Transition");
    if (null == this.components["tooltips:HUD"])   this.components["tooltips:HUD"]   = UnityEngine.GameObject.Find("UI/Tooltips/HUD");
    if (null == this.components["tooltips:world"]) this.components["tooltips:world"] = UnityEngine.GameObject.Find("UI/Tooltips/World");
    if (null == this.components["tooltips:world"]) this.components["tooltips:world"] = UnityEngine.GameObject.Find("UI/Tooltips/Scene");
    if (null == this.components["tooltips:world"]) this.components["tooltips:world"] = UnityEngine.GameObject.Find("UI/Tooltip");

    this.componentLoadAnimation.delay       = this.transitionOnComponentLoad ? transitionAnimation.duration + transitionAnimation.delay : 0.00;
    this.backgroundLoadAnimation            = this.componentLoadAnimation.AsCopy();
    this.canvas                             = this.GetComponent<UnityEngine.Canvas>();
    this.canvas.pixelPerfect                = true;
    this.canvasRenderer                     = this.GetComponent<UnityEngine.CanvasRenderer>();
    this.canvasRenderer.cullTransparentMesh = false;
    this.eventSystem                        = this.GetComponent<UnityEngine.EventSystems.EventSystem>() ?? UnityEngine.EventSystems.EventSystem.current;
    this.graphicRaycaster                   = this.GetComponent<UnityEngine.UI.GraphicRaycaster>     ();
    this.splashLoadAnimation.duration       = !this.splashLoadSkipped ? this.splashLoadDuration : 0.0;
    this.splashExitAnimation.delay          = !this.splashLoadSkipped ? System.Math.Max(this.splashLoadDuration - 0.5, 0.0) : 0.0;
    this.splashExitAnimation.duration       = !this.splashLoadSkipped ? 0.5 : 0.0;
    this.transitionAnimation.duration       = this.transitionSpeed;

    foreach (UnityEngine.UI.MaskableGraphic maskableGraphic in this.FindHierarchyByComponent<UnityEngine.UI.MaskableGraphic>())
      maskableGraphic.maskable = false;

    UI.Main                     ??= this;
    Settings.HumanReadable        = true;
    Util.Pointed.EventSystem      = this.eventSystem;
    Util.Pointed.GraphicRaycaster = this.graphicRaycaster;
    Util.UI.TabIndexPreserved     = true;

    // … ⟶ Prep (streaming) assets before they’re requested later in runtime
    this.buttonSelectedAnimation.Finish               ();
    this.transitionAnimation    .Finish               ();
    this                        .PreserveFromSceneLoad();

    UI.Preload.UriAsMusic  ("calm.mp3");
    UI.Preload.UriAsSound  ("click.mp3");
    UI.Preload.UriAsSound  ("dialogue.mp3");
    UI.Preload.UriAsSound  ("entry.mp3");
    UI.Preload.UriAsSound  ("exit.mp3");
    UI.Preload.UriAsSound  ("move.mp3");
    UI.Preload.UriAsSound  ("shoot.mp3");
    UI.Preload.UriAsSound  ("toggle.mp3");
    UI.Preload.UriAsSound  ("waves.mp3");
    UI.Preload.UriAsTexture("menu-button-3.png");
    UI.Preload.UriAsTexture("menu-button-4.png");
  }

  public string                   GetLoadedComponent() => this.componentLoaded;
  public UnityEngine.AudioSource? GetMusic          () => UI.CurrentMusic;
  public UnityEngine.GameObject?  GetTabbed         () => this.tabbed;

  public void LoadBackgroundComponent() {
    UnityEngine.GameObject background = this.components["background"];

    // …
    if (null != background && !this.backgroundIsLoaded) {
      this.backgroundIsLoaded = true;

      background                  .SetActive(true);
      this.backgroundLoadAnimation.Finish   ();
    }
  }

  public  void LoadComponent(string componentName, bool immediate = false) => this.LoadComponent(componentName, immediate, true);
  private void LoadComponent(string componentName, bool immediate, bool enforce) {
    if (!enforce || this.components.ContainsKey(componentName)) {
      UnityEngine.GameObject component              = this.components              [componentName];
      AnimationSequence      componentLoadAnimation = this.componentsLoadAnimations[componentName];

      // …
      if (null == component || (enforce ? componentName == this.componentCurrentLoaded : false))
      return;

      // … ⟶ Unload the soon-to-be-previously loaded component
      if (this.componentCurrentLoaded != this.componentPreviousLoaded)
        this.UnloadComponent(this.componentCurrentLoaded, immediate: false, enforce: enforce);

      // … ⟶ Load the component by name of `componentName`
      if (this.transitionOnComponentLoad)
        this.Transition();

      component.SetActive(true);

      if (!immediate) componentLoadAnimation.Reset ();
      else            componentLoadAnimation.Finish();

      this.componentPreviousLoaded = this.componentCurrentLoaded;
      this.componentCurrentLoaded  = componentName;
    }
  }

  private void Start() {
    UnityEngine.GameObject        background               = this.components["background"];
    FloatDictionary               componentsAlphas         = new(this.components.Count, this.components.Comparer);
    Dictionary<AnimationSequence> componentsLoadAnimations = new(this.components.Count, this.components.Comparer);
    Vector2Dictionary             componentsOrigins        = new(this.components.Count, this.components.Comparer);
    UnityEngine.GameObject        menu                     = this.components["menu"];
    UnityEngine.GameObject        options                  = this.components["options"];
    UnityEngine.GameObject        splash                   = this.components["splash"];
    UnityEngine.GameObject        transition               = this.components["transition"];

    // … ⟶ Ensure components are (un-)loadable
    foreach (string componentName in this.components.Keys)
    if (!this.componentSpecials.Contains(componentName)) {
      UnityEngine.GameObject component = this.components[componentName];

      // …
      componentsAlphas        .Add(componentName, component                  .GetAlpha());
      componentsLoadAnimations.Add(componentName, this.componentLoadAnimation.AsCopy  ());
      componentsOrigins       .Add(componentName, null != component ? component.transform.localPosition : UnityEngine.Vector2.zero);
    }

    this.componentsAlphas         = new(componentsAlphas);
    this.componentsLoadAnimations = new(componentsLoadAnimations);
    this.componentsOrigins        = new(componentsOrigins);

    // … ⟶ Ensure “background” is dynamic
    if (null != background) {
      UnityEngine.RectTransform backgroundRectTransform = (UnityEngine.RectTransform) background.transform;
      this.backgroundAnchoredPosition = backgroundRectTransform.anchoredPosition;
    }

    // … ⟶ Ensure “menu” components are animatable
    if (null != menu)
    this.menuLogoOrigin = (menu.FindChildByName("Logo")?.transform as UnityEngine.RectTransform)?.anchoredPosition ?? ((UnityEngine.RectTransform) menu.transform).anchoredPosition;

    // … ⟶ Initialize “options” components
    if (null != options)
    foreach (string optionProperty in this.optionsProperties.Keys) {
      UnityEngine.GameObject? optionsProperty     = this.optionsProperties[optionProperty] = options.FindDescendantByName(optionProperty)!;
      TMPro.TextMeshProUGUI?  optionsPropertyText = null != optionsProperty ? optionsProperty.FindDescendantByName("Text")?.GetComponent<TMPro.TextMeshProUGUI>() ?? optionsProperty.FindDescendantByComponent<TMPro.TextMeshProUGUI>() : null;

      // … ⟶ Set initial “options” text components
      if (null == optionsProperty)
      continue;

      if (optionProperty == "Music" || optionProperty == "Sound") {
        float                   optionPropertyFallback = 100.0f;
        float                   optionPropertyValue    = Settings.GetProperty<float>($"Options/{optionProperty}") ?? optionPropertyFallback; // ⟶ INFO: Should the `UI` thread be noticeably halted a la `Settings.LoadTimeoutMaximum` to load these settings? 🤔
        UnityEngine.GameObject? optionsSlider          = optionsProperty.FindChildByName("Slider");
        UnityEngine.GameObject? optionsSliderButton    = optionsSlider ?.FindChildByName("Button");

        // …
        optionPropertyValue = float.IsInfinity(optionPropertyValue) || float.IsNaN(optionPropertyValue) ? optionPropertyFallback : optionPropertyValue;

        if (null != optionsPropertyText) optionsPropertyText.text = optionPropertyValue.ToString("F0");
        if (null != optionsSliderButton) optionsSliderButton.SetAnchoredPosition(Util.Vector.ExcludeX(optionsSliderButton.GetAnchoredPosition(), value: optionsSlider!.GetRectWidth(this.canvas) * Util.Perc(optionPropertyValue)));
      }
    }

    // … ⟶ Ensure “splash” components are animatable
    if (null != splash) {
      UnityEngine.RectTransform  splashRectTransform           = (UnityEngine.RectTransform) splash.transform;
      float                      splashRectSizeDelta           = UnityEngine.Mathf.Abs(splashRectTransform.GetRectHeight(this.canvas) - splashRectTransform.GetRectWidth(this.canvas));
      UnityEngine.RectTransform? splashBackgroundRectTransform = splash.FindChildByName("Background")?.transform as UnityEngine.RectTransform;

      // … ⟶ Square the dimensions of the “splash” component
      splashRectTransform.anchorMax = splashRectTransform.anchorMin = splashRectTransform.pivot = new(0.5f, 0.0f); // ⟶ Bottom-center
      splashRectTransform.SetRectSize(this.rectTransform.GetRectSize(this.canvas));
      splashRectTransform.SetRectSize(UnityEngine.Vector2.one * UnityEngine.Mathf.Max(splashRectTransform.GetRectHeight(this.canvas), splashRectTransform.GetRectWidth(this.canvas)));
      this.splashBackgroundOrigin = splashRectTransform.anchoredPosition;

      if (null != splashBackgroundRectTransform) {
        splashBackgroundRectTransform.pivot            = new(0.5f, 0.0f); // ⟶ Bottom-center
        splashBackgroundRectTransform.anchorMin        = new(0.0f, 0.5f); // ⟶ Mid-center
        splashBackgroundRectTransform.anchorMax        = new(1.0f, 0.5f); //    ^^
        splashBackgroundRectTransform.anchoredPosition = Util.Vector.ExcludeY(splashBackgroundRectTransform.anchoredPosition, splashBackgroundRectTransform.anchoredPosition.y - Util.PercOf(splashRectTransform.GetRectHeight(this.canvas) - splashBackgroundRectTransform.GetRectHeight(this.canvas), 50.0f) - Util.PercOf(splashRectSizeDelta, 50.0f));
        splashBackgroundRectTransform.SetRectHeight(splashRectTransform.GetRectHeight(this.canvas));
        this.splashBackgroundOrigin = splashBackgroundRectTransform.anchoredPosition;
      }

      // … ⟶ Allow “splash” component (elements) to render transparently
      foreach (UnityEngine.CanvasRenderer splashRenderer in splash.FindHierarchyByComponent<UnityEngine.CanvasRenderer>())
      splashRenderer.cullTransparentMesh = false;
    }

    // … ⟶ Ensure “transition” component is animatable
    if (null != transition)
    this.transitionLocalSize = transition.GetRectSize(this.canvas);

    // … ⟶ Unload all components
    foreach (string componentName in this.components.Keys) {
      if (!this.componentSpecials.Contains(componentName))
      this.UnloadComponent(componentName, immediate: true, enforce: false);
    }
  }

  public void Transition() {
    this.transitionAnimation.Reset();
    UI.Load.UriAsSound("transition.mp3", Load.Idle); // ⟶ only play the effect if it’s been more than a half-second
  }

  public void UnloadBackgroundComponent() {
    UnityEngine.GameObject background = this.components["background"];

    // …
    if (null != background && this.backgroundIsLoaded) {
      this.backgroundIsLoaded = false;

      background                  .SetActive(true);
      this.backgroundLoadAnimation.Reset    ();
    }
  }

  public  void UnloadComponent(string componentName, bool immediate = false) => this.UnloadComponent(componentName, immediate, true);
  private void UnloadComponent(string componentName, bool immediate, bool enforce) {
    if (!enforce || this.components.ContainsKey(componentName)) {
      UnityEngine.GameObject component              = this.components              [componentName];
      AnimationSequence      componentLoadAnimation = this.componentsLoadAnimations[componentName];

      // …
      if (null == component || (enforce ? componentName == this.componentCurrentLoaded : false))
      return;

      if (!immediate) {
        if (componentLoadAnimation.isFinished) { component.SetActive(true); }
        componentLoadAnimation.Reset();
      } else {
        component             .SetActive(false);
        componentLoadAnimation.Finish   ();
      }
    }
  }

  private void Update() {
    UnityEngine.GameObject                                 background       = this.components["background"];
    System.Collections.Generic.List<UnityEngine.UI.Button> componentButtons = new(5);
    UnityEngine.GameObject                                 credits          = this.components["credits"];
    UnityEngine.GameObject                                 menu             = this.components["menu"];
    UnityEngine.GameObject                                 options          = this.components["options"];
    UnityEngine.GameObject                                 pause            = this.components["pause"];
    UnityEngine.GameObject                                 splash           = this.components["splash"];
    double                                                 timestamp        = UnityEngine.Time.realtimeSinceStartupAsDouble;
    UnityEngine.GameObject                                 transition       = this.components["transition"];

    // …
    this.tabbedPrevious                   =  Util.UI.TabIsActive ? this.tabbedPrevious : null;                                                                                                                  // ⟶ Track tabbed `UnityEngine.GameObject` objects
    this.tabbedCurrent                    =  Util.UI.TabIsActive ? this.tabbedCurrent  : null;                                                                                                                  //    ^^
    this.music                            =  UI.CurrentMusic;                                                                                                                                                   //
    this.canvas.additionalShaderChannels &= ~(UnityEngine.AdditionalCanvasShaderChannels.Normal | UnityEngine.AdditionalCanvasShaderChannels.Tangent);                                                          // ⟶ Is this alright?
    this.buttonSelectionPreviousMode      =  this.buttonSelectionMode;                                                                                                                                          //
    this.buttonSelectionMode              =  Util.Pointers.IsPointing || (Util.Pointers.IsMoving && UI.SelectionMode.Tabbed != this.buttonSelectionMode) ? UI.SelectionMode.Pointed : this.buttonSelectionMode; // ⟶ Determine how components will be selected events-wise
    this.buttonSelectionMode              = !Util.Pointers.IsPointing && Util.UI.TabIsChanging                                                           ? UI.SelectionMode.Tabbed  : this.buttonSelectionMode; // ⟶ Prioritize (digitized) tabbed because pointers are analog
    this.buttonSelectionMode              =  UI.SelectionMode.None == this.buttonSelectionMode                                                           ? UI.SelectionMode.Pointed : this.buttonSelectionMode; //

    if (this.buttonSelectionMode != this.buttonSelectionPreviousMode) /* ⟶ Acknowledge the selection mode changes */ {
      if (this.buttonSelectionMode switch {
        UI.SelectionMode.Pointed => !Util.Pointed.Any.IsEmpty(),
        _                        => false
      }) Util.UI.BlurTabs();
    }

    if (this.buttonSelectedAnimation.isFinished)
    this.buttonSelectedAnimation.Reset(timestamp); // ⟶ Reset component button animation

    if (null != UI.CurrentMusic && !UI.CurrentMusic.isPlaying && !UI.CurrentMusic.loop) /* ⟶ Ensure (background) music is always playing */ {
      UI.CurrentMusic.loop = true;

      UI.CurrentMusic.Stop();
      UI.CurrentMusic.Play();
    }

    progressOptionsSliderButtonInformation:
    foreach (UnityEngine.GameObject optionsSliderButton in this.optionsSliderButtonInformation.Keys) /* ⟶ Release/ Update focused grip on “options” slider components */ {
      ref (string, PointerInfo pointer) information = ref this.optionsSliderButtonInformation[optionsSliderButton];

      // … ⟶ Update
      foreach (ref readonly PointerInfo pointer in Util.Pointers.Any)
      if (information.pointer.id == pointer.id) {
        information.pointer = pointer; // ⟶ `this.optionsSliderButtonInformation[optionsSliderButton] = …;`
        break;
      }

      // … ⟶ Release 🪣
      if (
        Util.UI.Blurred                                ||
        DeviceState.LEAVE == information.pointer.state ||
        (optionsSliderButton is not null && null == optionsSliderButton)
      ) {
        this.optionsSliderButtonInformation.Remove(optionsSliderButton!);
        goto progressOptionsSliderButtonInformation;
      }
    }

    // … ⟶ Track component buttons
    if (null != credits && this.componentCurrentLoaded == "credits") componentButtons.AddRange(credits.FindDescendantsByComponent<UnityEngine.UI.Button>());
    if (null != menu    && this.componentCurrentLoaded == "menu")    componentButtons.AddRange(menu   .FindDescendantsByComponent<UnityEngine.UI.Button>());
    if (null != options && this.componentCurrentLoaded == "options") componentButtons.AddRange(options.FindDescendantsByComponent<UnityEngine.UI.Button>());
    if (null != pause   && this.componentCurrentLoaded == "pause")   componentButtons.AddRange(pause  .FindDescendantsByComponent<UnityEngine.UI.Button>());

    if ((!componentButtons.IsEmpty() && componentButtons.Count != this.buttons.Length) || this.buttons.Exists([PatchMethod(AggressiveInlining)] static (button) => null == button)) {
      uint                                                    count                    = (uint) componentButtons.Count;
      UnityEngine.UI.Button []                                buttons                  = Util.Array<UnityEngine.UI.Button>.Create(count);
      UnityEngine.Texture   []                                buttonsTextures          = Util.Array<UnityEngine.Texture>  .Create(count);
      UnityEngine.Vector3   []                                buttonsLocalScales       = Util.Array<UnityEngine.Vector3>  .Create(count);
      UnityEngine.Vector3   []                                buttonsLocalEulerAngles  = Util.Array<UnityEngine.Vector3>  .Create(count);
      UnityEngine.Vector3   []                                buttonsLocalPositions    = Util.Array<UnityEngine.Vector3>  .Create(count);
      AnimationSequence     []                                buttonsEntryAnimations   = Util.Array<AnimationSequence>    .Create(count);
      UnityEngine.Vector2   []                                buttonsAnchoredPositions = Util.Array<UnityEngine.Vector2>  .Create(count);
      System.Collections.Generic.List<UnityEngine.GameObject> buttonsTabbed            = new(0);

      // …
      componentButtons.Sort(static (buttonA, buttonB) => {
        (UnityEngine.Vector2 positionA, UnityEngine.Vector2 positionB) = (buttonA.GetAnchoredPosition(fallback: true), buttonB.GetAnchoredPosition(fallback: true));
        return positionA.y < positionB.y ? -1 : positionA.y > positionB.y ? +1 : positionA.x <= positionB.x ? -1 : +1;
      });

      buttonsTabbed = componentButtons.ConvertAll(static button => button.gameObject);

      buttonsTabbed  .Reverse ();
      Util.UI        .BlurTabs(ignore: true);
      Util.UI.TabList.Clear   ();
      Util.UI.TabList.AddRange(buttonsTabbed);

      foreach (UnityEngine.UI.Button button in componentButtons) {
        UnityEngine.Transform buttonTransform = button.transform;
        int                   index           = this.buttons.IndexOf(button);

        // …
        count--;
        buttonsTextures         [count]       = index == -1 ? button.GetTexture()!               : this.buttonsTextures        [index];
        buttonsLocalScales      [count]       = index == -1 ? buttonTransform.localScale         : this.buttonsLocalScales     [index];
        buttonsLocalPositions   [count]       = index == -1 ? buttonTransform.localPosition      : this.buttonsLocalPositions  [index];
        buttonsLocalEulerAngles [count]       = index == -1 ? buttonTransform.localEulerAngles   : this.buttonsLocalEulerAngles[index];
        buttonsEntryAnimations  [count]       = index == -1 ? this.buttonEntryAnimation.AsCopy() : this.buttonsEntryAnimations [index];
        buttonsEntryAnimations  [count].delay = this.buttonEntryAnimationDelay * (componentButtons.Count - count - 1);
        buttonsAnchoredPositions[count]       = index == -1 ? buttonTransform.GetAnchoredPosition(fallback: true) : this.buttonsAnchoredPositions[index];
        buttons                 [count]       = index == -1 ? button                                              : this.buttons                 [index];

        buttonsEntryAnimations[count].Reset(timestamp);
      }

      this.buttons                  = buttons;
      this.buttonsAnchoredPositions = buttonsAnchoredPositions;
      this.buttonsEntryAnimations   = buttonsEntryAnimations;
      this.buttonsLocalEulerAngles  = buttonsLocalEulerAngles;
      this.buttonsLocalPositions    = buttonsLocalPositions;
      this.buttonsLocalScales       = buttonsLocalScales;
      this.buttonsTextures          = buttonsTextures;
    }

    // … ⟶ Animate/ Invoke selected objects
    if (null != options) /* ⟶ “options” components */ {
      // … ⟶ Find “options” components to invoke 🔍
      foreach (ref readonly PointedInfo pointed in Util.Pointed.Any)
      if (DeviceState.ENTER == pointed.pointerState && pointed.IsPointing() && options.IsDescendant(pointed.graphic)) {
        string                  optionProperty      = string.Empty;
        UnityEngine.GameObject? optionsSlider       = null;
        UnityEngine.GameObject? optionsSliderButton = null;

        // …
        foreach (string suboptionProperty in this.optionsProperties.Keys)
        if (null != this.optionsProperties[suboptionProperty] && this.optionsProperties[suboptionProperty].IsDescendant(pointed.graphic)) {
          optionProperty      = suboptionProperty;
          optionsSlider       = this.optionsProperties[suboptionProperty].FindChildByName("Slider");
          optionsSliderButton = optionsSlider                           ?.FindChildByName("Button");

          break;
        }

        if (null != optionsSlider && null != optionsSliderButton) {
          UnityEngine.Rect optionsSliderButtonRect = optionsSliderButton.GetRect(this.canvas);
          UnityEngine.Rect optionsSliderRect       = optionsSlider      .GetRect(this.canvas);

          // … ⟶ Acknowledge selected “options” slider component
          foreach (ref readonly PointerInfo pointer in pointed.pointersList)
          if (pointer.IsPointing()) {
            bool optionsSliderButtonAcknowledged = false;

            // … ⟶ Ensure another “options” slider component is not acknowledged
            if (!(optionsSliderButtonAcknowledged = this.optionsSliderButtonInformation.ContainsKey(optionsSliderButton))) {
              foreach (ref readonly (string, PointerInfo pointer) information in this.optionsSliderButtonInformation.Values)
              if (information.pointer.id == pointer.id) { optionsSliderButtonAcknowledged = true; break; }
            }

            // … ⟶ Focus-grip on “options” slider component
            if (!optionsSliderButtonAcknowledged && (optionsSliderRect.Contains(pointer.position) || optionsSliderButtonRect.Contains(pointer.position))) {
              UI.Load.UriAsSound("toggle.mp3", Load.Idle);
              this.optionsSliderButtonInformation.Add(optionsSliderButton, (optionProperty, pointer));

              break;
            }
          }
        }
      }

      // … ⟶ Do stuff; be invoked‥ 💡
      foreach ((UnityEngine.GameObject optionsSliderButton, (string optionsProperty, PointerInfo pointer) information) in this.optionsSliderButtonInformation) {
        UnityEngine.GameObject optionsProperty    = this.optionsProperties[information.optionsProperty];
        UnityEngine.GameObject optionsSlider      = optionsSliderButton.GetParent();
        UnityEngine.Rect       optionsSliderRect  = optionsSlider.GetRect(this.canvas);
        TMPro.TextMeshProUGUI? optionsSliderText  = optionsProperty.FindDescendantByName("Text")?.GetComponent<TMPro.TextMeshProUGUI>() ?? optionsProperty.FindDescendantByComponent<TMPro.TextMeshProUGUI>();
        float                  optionsSliderValue = UnityEngine.Mathf.Clamp(information.pointer.x - optionsSliderRect.x, 0.0f, optionsSliderRect.width) / optionsSliderRect.width;

        // …
        optionsSliderButton.SetAnchoredPosition(Util.Vector.ExcludeX(optionsSliderButton.GetAnchoredPosition(), value: optionsSliderRect.width * optionsSliderValue));
        Settings.SetProperty<float>($"Options/{information.optionsProperty}", optionsSliderValue * 100.0f);

        if (null != optionsSliderText)
        optionsSliderText.text = ((uint) (optionsSliderValue * 100u)).ToString();

        // …
        if      (information.optionsProperty == "Music") { if (null != UI.CurrentMusic) UI.CurrentMusic.volume = optionsSliderValue; }
        else if (information.optionsProperty == "Sound") { UI.Load.UriAsSound("shoot.mp3", immediate: false, Load.PlaySound); }
      }
    }

    if (DeviceState.RELEASE == Util.Keys.Specials.Escape && this.componentCurrentLoaded == "menu") /* ⟶ “menu” component buttons */ {
      // … ⟶ Select the Quit button if the Escape key is actuated
      for (uint index = (uint) this.buttons.Length; 0u != index--; )
      if (this.buttons[index].tag == "MenuQuitButton") {
        Util.UI.TabIndex = (int) index;
        break;
      }

      this.buttonSelectionMode = UI.SelectionMode.Tabbed;
    }

    for ((uint index, uint refreshIndex, bool skipped) = ((uint) this.buttons.Length, (uint) this.buttons.Length, false); 0u != index--; ) /* ⟶ Component buttons */ {
      UnityEngine.UI.Button  button                  = this.buttons               [index];
      AnimationSequence      buttonEntryAnimation    = this.buttonsEntryAnimations[index];
      bool                   buttonIsBackPrompted    = UI.SelectionMode.Pointed == this.buttonSelectionMode && DeviceState.RELEASE == Util.Pointers.MouseButtonBackState    && button.tag == "BackButton";
      bool                   buttonIsForwardPrompted = UI.SelectionMode.Pointed == this.buttonSelectionMode && DeviceState.RELEASE == Util.Pointers.MouseButtonForwardState && button.tag == "NextButton"; // ⟶ Doesn’t exist at the moment
      bool                   buttonIsPointed         = Util.Pointed.IsPointed(button, out PointedInfo pointed); // ⟶ `::state` is `DeviceState.UNKNOWN` by default
      bool                   buttonIsPrompted        = buttonIsBackPrompted || buttonIsForwardPrompted;
      DeviceState            buttonPointedState      = UI.SelectionMode.Pointed != this.buttonSelectionMode ? DeviceState.UNKNOWN : pointed.state;
      DeviceState            buttonTabbedState       = UI.SelectionMode.Tabbed  != this.buttonSelectionMode ? DeviceState.UNKNOWN : button.gameObject == this.tabbedPrevious ? DeviceState.LEAVE : Util.UI.Tabbed == button.gameObject ? button.gameObject == this.tabbedCurrent ? DeviceState.ONGOING : DeviceState.ENTER : DeviceState.UNKNOWN;
      UnityEngine.Transform  buttonTransform         = button.transform;
      bool                   refresh                 = false;
      bool                   refreshed               = index > refreshIndex;

      // …
      if (!skipped) {
        // … ⟶ Acknowledge further selection mode changes
        if (UI.SelectionMode.Pointed != this.buttonSelectionMode && DeviceState.ENTER == pointed.state) {
          this.buttonSelectionMode = UI.SelectionMode.Pointed;
          refresh                  = true;
        }

        if (refresh && index < refreshIndex) {
          refreshIndex = index;
          index        = (uint) this.buttons.Length;

          continue;
        }

        // …
        if (Util.UI.TabIsActive || Util.UI.TabIsBlurred || buttonIsPointed || buttonIsPrompted) {
          // … ⟶ Don’t be non-responsive waiting for the component button’s entry animation
          if (!buttonEntryAnimation.isFinished && (Util.UI.TabIsChanging || buttonIsPointed || buttonIsPrompted)) {
            for (uint subindex = (uint) this.buttonsEntryAnimations.Length; 0u != subindex--; )
              this.buttonsEntryAnimations[subindex].Finish();

            index   = (uint) this.buttons.Length;
            skipped = true; // ⟶ Skip everything and instead acknowledge selections on the next frame

            continue;
          }

          // …
          if (UI.SelectionMode.None != this.buttonSelectionMode) {
            // … ⟶ Play a sound effect when entered/ selected 🎵
            if (
              DeviceState.ENTER == buttonPointedState || (pointed.IsPointing() && DeviceState.BEGIN == pointed.pointerState) ||
              DeviceState.ENTER == buttonTabbedState  || (Util.Keys.IsPrompted && DeviceState.LEAVE >  buttonTabbedState)    ||
              buttonIsPrompted
            ) {
              if (this.buttonSelectedAnimation.elapsed > 0.5)
                UI.Load.UriAsSound("click.mp3", Load.Idle); // ⟶ only play the effect if it’s been more than a half-second

              this.buttonSelectedAnimation.Reset(timestamp);

              // … ⟶ Acknowledge tabbed from pointed
              if (DeviceState.ENTER == buttonPointedState) {
                for (int subindex = Util.UI.TabList.Count; 0 != subindex--; )
                if (button.gameObject == Util.UI.TabList[subindex]) {
                  Util.UI.TabIndex         = subindex;
                  Util.UI.TabPreviousIndex = subindex;

                  break;
                }
              }
            }

            // … ⟶ Hover over animation 🎭
            if (DeviceState.LEAVE > buttonPointedState || DeviceState.LEAVE > buttonTabbedState) {
              float buttonAnimationTeeter = Util.Cast<float>(this.buttonSelectedAnimation["teeter"]);

              // …
              buttonAnimationTeeter            = (index & 1) == 1 ? -buttonAnimationTeeter : +buttonAnimationTeeter;
              buttonTransform.localEulerAngles = this.buttonsLocalEulerAngles[index] + (Util.Vector.MaskZ(UnityEngine.Vector3.one) * buttonAnimationTeeter * 5.0f /* ⟶ in Degrees */);
              buttonTransform.localPosition    = this.buttonsLocalPositions  [index] + (UnityEngine.Vector3.one * Util.Lerp(this.buttonSelectedAnimation.easedProgress, 0.0f, 1.0f));
              buttonTransform.localScale       = this.buttonsLocalScales     [index] + (this.buttonsLocalScales[index] * UnityEngine.Mathf.Abs(buttonAnimationTeeter) * Util.Perc(10.0f));

              if (button.tag != "MenuQuitButton") {
                if (button.tag == "MenuCreditsButton" || button.tag == "MenuOptionsButton" || button.tag == "MenuStartButton")
                UI.Load.UriAsTexture("menu-button-3.png", button.SetTexture);
              } else UI.Load.UriAsTexture("menu-button-4.png", button.SetTexture);
            }

            // … ⟶ Do stuff; be invoked‥ 💡
            if (DeviceState.LEAVE > buttonPointedState || DeviceState.LEAVE > buttonTabbedState || buttonIsPrompted) {
              bool buttonIsKeyPrompted     = DeviceState.LEAVE > buttonTabbedState  && Util.Keys.IsPrompted;
              bool buttonIsPointerPrompted = DeviceState.LEAVE > buttonPointedState && pointed.IsClicking();

              // …
              if (buttonIsKeyPrompted || buttonIsPointerPrompted || buttonIsPrompted) {
                buttonIsPrompted = true;

                if (button.interactable) // ⟶ Only for posterity in lieu of `button.onClick?.Invoke()`
                foreach (ref readonly PointerInfo pointer in pointed.pointers) {
                  if (buttonIsPointerPrompted)
                    button.OnPointerClick(Util.Pointers.MakePointerEventData(Util.Pointed.EventSystem!, in pointer, button.gameObject));

                  button.OnSubmit(new(Util.Pointed.EventSystem!) {/* currentInputModule = Util.Pointed.EventSystem!.currentInputModule ?? button.GetComponent<UnityEngine.EventSystems.BaseInputModule>(), */ selectedObject = button.gameObject});
                }

                if (button.tag != "MenuQuitButton") {
                  if (button.tag == "BackButton") switch (Game.GetState()) {
                    case Game.State.Gameplay: Game.Unpause();             break;
                    case Game.State.Menu    : this.LoadComponent("menu"); break;
                  }

                  else if (button.tag == "MenuButton")        Game.QuitToMenu   ();
                  else if (button.tag == "MenuStartButton")   Game.Play         ();
                  else if (button.tag == "MenuCreditsButton") this.LoadComponent("credits");
                  else if (button.tag == "MenuOptionsButton") this.LoadComponent("options");
                } else                                        Game.Quit         (); // ⟶ Confusing `… else` format? 🙃
              }
            }

            // … ⟶ Reset the component button’s state e.g. animation, ‥
            if (DeviceState.LEAVE == buttonPointedState || DeviceState.LEAVE == buttonTabbedState || Util.UI.TabIsBlurred || buttonIsPrompted || this.buttonSelectionMode != this.buttonSelectionPreviousMode) {
              button         .SetTexture         (this.buttonsTextures         [index]);
              buttonTransform.SetAnchoredPosition(this.buttonsAnchoredPositions[index], fallback: true);

              buttonTransform.localEulerAngles = this.buttonsLocalEulerAngles[index];
              buttonTransform.localPosition    = this.buttonsLocalPositions  [index];
              buttonTransform.localScale       = this.buttonsLocalScales     [index];
            }

            continue;
          }
        }
      }

      // … ⟶ Entry animation
      button         .SetAlpha           (Util.Perc(100.0f) * (float) buttonEntryAnimation.progress);
      buttonTransform.SetAnchoredPosition(Util.Vector.ExcludeY(this.buttonsAnchoredPositions[index], value: this.buttonsAnchoredPositions[index].y + (Util.PercOf(buttonTransform.GetRectHeight(this.canvas), 50.0f) * (float) (1.0 - buttonEntryAnimation.easedProgress))), fallback: true);
    }

    // … ⟶ Animate component (un)loads
    foreach (string componentName in this.components.Keys)
    if (!this.componentSpecials.Contains(componentName)) {
      UnityEngine.GameObject component = this.components[componentName];

      // …
      if (null != component && component.activeSelf) {
        UnityEngine.RectTransform        componentRectTransform         = (UnityEngine.RectTransform) component.transform;
        ref readonly UnityEngine.Vector2 componentOrigin                = ref this.componentsOrigins   [componentName];
        AnimationSequence                componentLoadAnimation         = this.componentsLoadAnimations[componentName];
        bool                             componentIsLoaded              = componentName == this.componentCurrentLoaded;
        float                            componentLoadAnimationProgress = (float) (componentIsLoaded ? Util.Perc(100.0) - componentLoadAnimation.easedProgress : componentLoadAnimation.easedProgress);
        float                            componentHeight                = UnityEngine.Mathf.Max(componentRectTransform.GetRectHeight(this.canvas), this.rectTransform.GetRectHeight(this.canvas));
        float                            componentAlpha                 = this.componentsAlphas[componentName];

        // …
        componentRectTransform.anchoredPosition = Util.Vector.ExcludeY(componentRectTransform.anchoredPosition, value: componentOrigin.y - (componentHeight * componentLoadAnimationProgress));

        component.SetAlpha(componentAlpha * (Util.Perc(100.0f) - componentLoadAnimationProgress));
        if (!componentIsLoaded && componentLoadAnimation.isFinished) component.SetActive(false);
      }
    }

    // … ⟶ Animate “background” component
    if (null != background && background.activeInHierarchy) {
      float                     backgroundLoadAnimationProgress = (float) (this.backgroundIsLoaded ? Util.Perc(100.0) - this.backgroundLoadAnimation.easedProgress : this.backgroundLoadAnimation.easedProgress);
      UnityEngine.Vector2       backgroundOffset                = Util.Vector.Clamp(!Util.Pointers.Begin.IsEmpty() ? Util.Pointers.Begin[0].position : !Util.Pointers.Current.IsEmpty() ? Util.Pointers.Current[0].position : UnityEngine.Vector2.zero, UnityEngine.Vector2.zero, this.rectTransform.GetRectSize(this.canvas));
      UnityEngine.RectTransform backgroundRectTransform         = (UnityEngine.RectTransform) background.transform;
      UnityEngine.Vector2       rectCenter                      = this.rectTransform.GetRectCenter(this.canvas);

      // …
      if (Game.GetState() != Game.State.Gameplay) backgroundRectTransform.SetAnchoredPosition(this.backgroundAnchoredPosition + ((backgroundRectTransform.GetRectSize(this.canvas) * Util.Perc(1.0f)) * ((rectCenter - backgroundOffset) / /* ⟶ Avoid division-by-zero */ Util.Vector.Coalesce(rectCenter, Util.Vector.Epsilon<UnityEngine.Vector2>()))));
      background.SetAlpha(Util.Perc(100.0f) - backgroundLoadAnimationProgress);
      if (!this.backgroundIsLoaded && this.backgroundLoadAnimation.isFinished) background.SetActive(false);
    }

    // … ⟶ Animate “splash” component
    if (null != splash && splash.activeSelf) {
      UnityEngine.UI.Graphic?    splashBackground                 = splash.FindChildByName("Background")?.GetComponent<UnityEngine.UI.Graphic>();
      float                      splashExitAnimationProgress      = UnityEngine.Mathf.Max((float) this.splashExitAnimation.progress, 0.0f);
      TMPro.TextMeshProUGUI?     splashHintText                   = splash.FindDescendantByName("Hint")?.GetComponent<TMPro.TextMeshProUGUI>();
      float                      splashLoadAnimationBackgroundTop = Util.Cast<float>(this.splashLoadAnimation["background-top"]);
      float                      splashLoadAnimationOpacity       = 1.0f - Util.Cast<float>(this.splashLoadAnimation["transparency"]);
      float                      splashLoadAnimationProgress      = (float) this.splashLoadAnimation.progress;
      UnityEngine.RectTransform? splashProgressBar                = splash.FindChildByName("Progress")?.transform as UnityEngine.RectTransform;
      UnityEngine.Vector2        splashRectSize                   = splash.GetRectSize(this.canvas);

      // … ⟶ Generate a new “splash” general hint
      if (!Game.GeneralHints.IsEmpty() && splashLoadAnimationProgress >= this.splashHintCount * Util.Perc(this.splashHintThreshold)) {
        uint index = this.splashHintIndex;

        // …
        while (Game.GeneralHints.Length != 1 && index == this.splashHintIndex)
          index = (uint) (Game.GeneralHints.Length * Util.Random());

        this.splashHintCount++;
        this.splashHintIndex = index;

        if (null != splashHintText)
        splashHintText.text = Game.GeneralHints[this.splashHintIndex];
      }

      // … ⟶ Animate “splash”
      splash.SetAlpha(Util.Perc(100.0f) - splashExitAnimationProgress);

      // … ⟶ Animate “splash” background
      if (null != splashBackground) {
        UnityEngine.RectTransform splashBackgroundRectTransform = splashBackground.rectTransform;

        // …
        splashBackground.SetAlpha(splashLoadAnimationOpacity);
        splashBackgroundRectTransform.anchoredPosition = Util.Vector.ExcludeY(splashBackgroundRectTransform.anchoredPosition, value: this.splashBackgroundOrigin.y - (splashLoadAnimationBackgroundTop * splashRectSize.y));
      }

      // … ⟶ Animate “splash” progress bar
      if (null != splashProgressBar) {
        splashProgressBar.SetAlpha    (Util.Perc(100.0f) - splashExitAnimationProgress);
        splashProgressBar.SetRectWidth(splashLoadAnimationProgress * splashRectSize.x);
      }

      // … ⟶ Animate “splash” text
      foreach (TMPro.TextMeshProUGUI splashText in splash.FindHierarchyByComponent<TMPro.TextMeshProUGUI>())
      splashText.SetAlpha(splashLoadAnimationOpacity);

      // … ⟶ Load “menu” component
      if (this.splashExitAnimation.isFinished) {
        splash.SetActive(false);
        this.LoadComponent("menu", immediate: true, enforce: false);

        foreach (AnimationSequence buttonEntryAnimation in this.buttonsEntryAnimations)
        buttonEntryAnimation.Reset(timestamp); // ⟶ Assume the current `UI::buttons` are all “menu” components
      }
    }

    // … ⟶ Animate “menu” component
    if (null != menu && menu.activeInHierarchy && this.componentCurrentLoaded == "menu") {
      UnityEngine.UI.Graphic?    menuLogo              = menu.FindChildByName("Logo")?.GetComponent<UnityEngine.UI.Graphic>();
      UnityEngine.RectTransform? menuLogoRectTransform = null != menuLogo ? menuLogo.rectTransform : null;
      float                      menuLogoTopOffset     = 50.0f;

      // … ⟶ Play main “menu” music
      UI.Load.UriAsMusic("calm.mp3", Load.Idle);

      // … ⟶ Animate “menu” logo
      if (null != menuLogoRectTransform) {
        menuLogoRectTransform.pivot            = new(0.5f, 0.5f);
        menuLogoRectTransform.anchorMin        = new(0.0f, 1.0f); // ⟶ Top-center
        menuLogoRectTransform.anchorMax        = new(1.0f, 1.0f); //    ^^
        menuLogoRectTransform.anchoredPosition = new(menuLogoRectTransform.anchoredPosition.x, this.menuLogoOrigin.y + (menuLogoTopOffset * (1.0f - (float) this.componentsLoadAnimations["menu"].easedProgress)));
      }
    }

    // … ⟶ Animate “pause” component
    if (null != pause) {
      if (!Game.Paused && this.componentCurrentLoaded != "pause" && DeviceState.BEGIN == Util.Keys.Specials.Escape)
      Game.Pause();
    }

    // … ⟶ Animate “transition” component
    if (null != transition) {
      double                    transitionAnimationZoom = 1.0 - Util.Cast<double>(this.transitionAnimation["zoom"]);
      UnityEngine.UI.Graphic?   transitionFade          = transition.FindChildByName("Fade")?.GetComponent<UnityEngine.UI.Graphic>();
      UnityEngine.RectTransform transitionRectTransform = (UnityEngine.RectTransform) transition.transform;

      // …
      transitionRectTransform.pivot = new(0.5f, 0.5f); // ⟶ Mid-center

      transitionRectTransform.SetRectSize(Util.Vector.Coalesce(this.transitionLocalSize, this.rectTransform.GetRectSize(this.canvas)) * Util.Lerp(transitionAnimationZoom, Util.Perc(250.0f), Util.Perc(7.5f)));
      if (null != transitionFade) transitionFade.SetAlpha(Util.Lerp(transitionAnimationZoom, Util.Perc(0.0f), Util.Perc(67.5f)));
    }

    // …
    this.tabbedPrevious = Util.UI.Tabbed != this.tabbedCurrent ? this.tabbedCurrent : this.tabbedPrevious; // ⟶ Track tabbed `UnityEngine.GameObject` objects
    this.tabbedCurrent  = Util.UI.TabIsActive ? Util.UI.TabList[Util.UI.TabIndex] : this.tabbedCurrent;    //    ^^
  }
}
