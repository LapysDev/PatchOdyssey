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

    [PatchMethod(AggressiveInlining)]
    private static void UriAsAudio(System.Uri path, bool effect, System.Action<UnityEngine.AudioSource> callback, System.Action fallback) => Util.Load.UriAsAudioClip(path, Util.EvaluateAudioTypeFromExtension(System.IO.Path.GetExtension(path.ToString())), (object? target, in Events.LoadEvent data) => {
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
        source.pitch        = effect && source.pitch == 1.0f ? UnityEngine.Mathf.Pow(2.0f, semitones.Random() / 12.0f) /* ⟶ 2ⁿᐟ¹² for semitone steps */ : source.pitch;
        source.spatialBlend = Util.Perc(effect ? 100.0f : 0.0f);
        source.volume       = 1.0f; // ⟶ Already normalized

        if (effect)
          source.PlayOneShot(source.clip, 1.0f);

        else if (UI.CurrentMusic != source) {
          if (null != UI.CurrentMusic)
            UI.CurrentMusic.Stop();

          UI.CurrentMusic = source;
          source.Play();
        }

        // …
        callback(source);
      } else fallback();
    }, Util.Load.Asynchronously, Util.Load.WithReadOnlyCache, 3u, (object? _, in Events.LoadEvent _) => fallback());
      [PatchMethod(AggressiveInlining)] public static void UriAsMusic(string path, System.Action<UnityEngine.AudioSource> callback, System.Action? fallback = null) => Load.UriAsAudio(new(System.IO.Path.Combine(new[] {Util.Path.Assets, "Music", path})), false, callback, fallback ?? ([PatchMethod(AggressiveInlining)] static () => {}));
      [PatchMethod(AggressiveInlining)] public static void UriAsSound(string path, System.Action<UnityEngine.AudioSource> callback, System.Action? fallback = null) => Load.UriAsAudio(new(System.IO.Path.Combine(new[] {Util.Path.Assets, "SFX",   path})), true,  callback, fallback ?? ([PatchMethod(AggressiveInlining)] static () => {}));

    [PatchMethod(AggressiveInlining)]
    public static void UriAsTexture(string path, System.Action<UnityEngine.Texture> callback, System.Action? fallback = null) => Util.Load.UriAsTexture2D(new(System.IO.Path.Combine(new[] {Util.Path.Assets, "UI", path})), (object? target, in Events.LoadEvent data) => {
      if (data.payload is UnityEngine.Texture texture) callback(texture);
      else                                             fallback?.Invoke();
    }, Util.Load.Asynchronously, Util.Load.WithReadOnlyCache, 3u, fallback is null ? static (object? _, in Events.LoadEvent _) => {} : (object? _, in Events.LoadEvent _) => fallback());
  }

  public static class Preload {
    [PatchMethod(AggressiveInlining)] public static void UriAsMusic  (string path) => Util.Preload.UriAsAudioClip(new(System.IO.Path.Combine(new[] {Util.Path.Assets, "Music", path})), Util.EvaluateAudioTypeFromExtension(System.IO.Path.GetExtension(path)));
    [PatchMethod(AggressiveInlining)] public static void UriAsSound  (string path) => Util.Preload.UriAsAudioClip(new(System.IO.Path.Combine(new[] {Util.Path.Assets, "SFX",   path})), Util.EvaluateAudioTypeFromExtension(System.IO.Path.GetExtension(path)));
    [PatchMethod(AggressiveInlining)] public static void UriAsTexture(string path) => Util.Preload.UriAsTexture2D(new(System.IO.Path.Combine(new[] {Util.Path.Assets, "UI",    path})));
  }

  /* … */
  private static readonly System.Collections.Generic.Dictionary<UnityEngine.AudioClip, UnityEngine.AudioSource> AudioSources              = new(3);
  public  static          UnityEngine.AudioSource?                                                              CurrentMusic              = null;
  public  static          UI                                                                                    Main                      = default!;
  public  static readonly double                                                                                SplashLoadDurationMinimum = 10.0 + (Util.Random() * 3.0);

  [ReadWriteInInspector]                            private AnimationSequence                     buttonEntryAnimation     =  default!;
  [ReadWriteInInspector]                            public  float                                 buttonEntryAnimationDelay=  0.4f;
  [ReadWriteInInspector]                            private AnimationSequence                     buttonSelectedAnimation  =  default!;
  [ReadOnlyInInspector]                             public  UnityEngine.UI.Button[]               buttons                  =  System.Array.Empty<UnityEngine.UI.Button>();
  [ReadOnlyInInspector]                             private UnityEngine.Vector2  []               buttonsAnchoredPositions =  System.Array.Empty<UnityEngine.Vector2>  ();
  [ReadOnlyInInspector]                             private AnimationSequence    []               buttonsEntryAnimations   =  System.Array.Empty<AnimationSequence>    ();
  [ReadOnlyInInspector]                             private UnityEngine.Vector3  []               buttonsLocalEulerAngles  =  System.Array.Empty<UnityEngine.Vector3>  ();
  [ReadOnlyInInspector]                             private UnityEngine.Vector3  []               buttonsLocalScales       =  System.Array.Empty<UnityEngine.Vector3>  ();
  [ReadOnlyInInspector]                             private UnityEngine.Texture  []               buttonsTextures          =  System.Array.Empty<UnityEngine.Texture>  ();
  [ReadOnlyInInspector]                             private UnityEngine.Canvas                    canvas                   =  default!;
  [ReadOnlyInInspector]                             private UnityEngine.CanvasRenderer            canvasRenderer           =  default!;
  [ReadOnlyInInspector]                             private ref string                            componentCurrentLoaded   => ref this.componentLoaded;
  [ReadWriteInInspector]                            private AnimationSequence                     componentLoadAnimation   =  default!;
  [ReadOnlyInInspector, UnityEngine.SerializeField] private string                                componentLoaded          =  "menu";
  [ReadOnlyInInspector]                             private string                                componentPreviousLoaded  =  "menu";
  [ReadOnlyInInspector]                             private string[]                              componentSpecials        =  new string[] {"splash"}; // ⟶ Components that can not be (un-)loaded
  [ReadWriteInInspector]                            public  GameObjectReadOnlyDictionary          components               =  new(new GameObjectDictionary(11u) {{"background", null!}, {"combat", null!}, {"credits", null!}, {"dialogue", null!}, {"inventory", null!}, {"menu", null!}, {"pause", null!}, {"settings", null!}, {"splash", null!}, {"tooltips:HUD", null!}, {"tooltips:world", null!}});
  [ReadWriteInInspector]                            private ReadOnlyDictionary<AnimationSequence> componentsLoadAnimations =  ReadOnlyDictionary<AnimationSequence>.Empty;
  [ReadWriteInInspector]                            private Vector2ReadOnlyDictionary             componentsOrigins        =  Vector2ReadOnlyDictionary            .Empty;
  [ReadOnlyInInspector]                             private UnityEngine.EventSystems.EventSystem  eventSystem              =  default!;
  [ReadOnlyInInspector]                             private UnityEngine.UI.GraphicRaycaster       graphicRaycaster         =  default!;
  [ReadWriteInInspector]                            private UnityEngine.Vector2                   menuLogoOrigin           =  UnityEngine.Vector2.zero;
  [ReadWriteInInspector]                            public  UnityEngine.RectTransform             rectTransform            => (UnityEngine.RectTransform) this.transform;
  [ReadWriteInInspector]                            private UnityEngine.Vector2                   splashBackgroundOrigin   =  UnityEngine.Vector2.zero;
  [ReadWriteInInspector]                            private AnimationSequence                     splashExitAnimation      =  default!;
  [ReadWriteInInspector]                            private uint                                  splashHintCount          =  0u;
  [ReadOnlyInInspector]                             private uint                                  splashHintIndex          =  default!;
  [UnityEngine.Range(0.0f, 100.0f)]                 public  float                                 splashHintThreshold      =  17.5f;
  [ReadWriteInInspector]                            private AnimationSequence                     splashLoadAnimation      =  default!;
  [ReadWriteInInspector]                            public  double                                splashLoadDuration       =  UI.SplashLoadDurationMinimum;
  [ReadWriteInInspector]                            public  bool                                  splashLoadSkipped        =  false;
  [ReadOnlyInInspector, UnityEngine.SerializeField] private UnityEngine.GameObject?               tabbed                   =  null;
  [ReadOnlyInInspector]                             private ref UnityEngine.GameObject?           tabbedCurrent            => ref this.tabbed;
  [ReadOnlyInInspector]                             private UnityEngine.GameObject?               tabbedPrevious           =  null;

  /* … */
  private void Awake() {
    AnimationSequence buttonEntryAnimation     = new(1.75, 0.00, Animation.Function.EaseIn,  AnimationSequence.Idle,  AnimationSequence.Idle);
    AnimationSequence buttonSelectedAnimation  = new(1.75, 0.00, Animation.Function.Linear,  new() {{"teeter", 0.0}}, new() {{"teeter", 0.0}}) {{Util.Perc(25.0), new() {{"teeter", -1.0}}}, {Util.Perc(50.0), new() {{"teeter", 0.0}}}, {Util.Perc(75.0), new() {{"teeter", +1.0}}}};
    AnimationSequence componentLoadAnimation   = new(1.25, 0.00, Animation.Function.EaseOut, AnimationSequence.Idle,  AnimationSequence.Idle);
    AnimationSequence splashExitAnimation      = new(!this.splashLoadSkipped ? 0.5 : 0.0, !this.splashLoadSkipped ? System.Math.Max(this.splashLoadDuration - 0.5, 0.0) : 0.0, Animation.Function.EaseOut, AnimationSequence.Idle, AnimationSequence.Idle);
    AnimationSequence splashLoadAnimation      = new(!this.splashLoadSkipped ? this.splashLoadDuration : 0.0,
      new() {{"background-top", 1.0}, {"progress", 0.0}, {"transparency", 1.0}},
      new() {{"background-top", 1.0}, {"progress", 1.0}, {"transparency", 1.0}}
    ) {
      {Util.Perc(15.0), new() {{"background-top", 0.0}, {"transparency", 0.0}}},
      {Util.Perc(50.0), new() {{"background-top", 0.0}}},
      {Util.Perc(67.5), new() {{"transparency",   0.0}}}
    };

    //
    if (null == this.components["background"])     this.components["background"]     = UnityEngine.GameObject.Find("UI/Background");
    if (null == this.components["combat"])         this.components["combat"]         = UnityEngine.GameObject.Find("UI/Combat");
    if (null == this.components["credits"])        this.components["credits"]        = UnityEngine.GameObject.Find("UI/Credits");
    if (null == this.components["dialogue"])       this.components["dialogue"]       = UnityEngine.GameObject.Find("UI/Dialogue");
    if (null == this.components["inventory"])      this.components["inventory"]      = UnityEngine.GameObject.Find("UI/Inventory");
    if (null == this.components["menu"])           this.components["menu"]           = UnityEngine.GameObject.Find("UI/Menu");
    if (null == this.components["pause"])          this.components["pause"]          = UnityEngine.GameObject.Find("UI/Pause");
    if (null == this.components["settings"])       this.components["settings"]       = UnityEngine.GameObject.Find("UI/Settings");
    if (null == this.components["splash"])         this.components["splash"]         = UnityEngine.GameObject.Find("UI/Splash");
    if (null == this.components["tooltips:HUD"])   this.components["tooltips:HUD"]   = UnityEngine.GameObject.Find("UI/Tooltips/HUD");
    if (null == this.components["tooltips:world"]) this.components["tooltips:world"] = UnityEngine.GameObject.Find("UI/Tooltips/World");
    if (null == this.components["tooltips:world"]) this.components["tooltips:world"] = UnityEngine.GameObject.Find("UI/Tooltips/Scene");
    if (null == this.components["tooltips:world"]) this.components["tooltips:world"] = UnityEngine.GameObject.Find("UI/Tooltip");

    this.buttonEntryAnimation               = buttonEntryAnimation;
    this.buttonSelectedAnimation            = buttonSelectedAnimation;
    this.canvas                             = this.GetComponent<UnityEngine.Canvas>();
    this.canvas.pixelPerfect                = true;
    this.canvasRenderer                     = this.GetComponent<UnityEngine.CanvasRenderer>();
    this.canvasRenderer.cullTransparentMesh = false;
    this.componentLoadAnimation             = componentLoadAnimation;
    this.eventSystem                        = this.GetComponent<UnityEngine.EventSystems.EventSystem>();
    this.graphicRaycaster                   = this.GetComponent<UnityEngine.UI.GraphicRaycaster>     ();
    this.splashExitAnimation                = splashExitAnimation;
    this.splashLoadAnimation                = splashLoadAnimation;

    foreach (UnityEngine.UI.MaskableGraphic maskableGraphic in this.FindHierarchyByComponent<UnityEngine.UI.MaskableGraphic>())
      maskableGraphic.maskable = false;

    UI.Main                     ??= this;
    Util.Pointed.EventSystem      = this.eventSystem;
    Util.Pointed.GraphicRaycaster = this.graphicRaycaster;

    // … ⟶ Prep (streaming) assets before they’re requested later in runtime
    UI.Preload.UriAsMusic  ("calm.mp3");
    UI.Preload.UriAsSound  ("click.mp3");
    UI.Preload.UriAsSound  ("dialogue.mp3");
    UI.Preload.UriAsSound  ("entry.mp3");
    UI.Preload.UriAsSound  ("exit.mp3");
    UI.Preload.UriAsSound  ("move.mp3");
    UI.Preload.UriAsSound  ("waves.mp3");
    UI.Preload.UriAsTexture("menu-button-3.png");
    UI.Preload.UriAsTexture("menu-button-4.png");
  }

  public string                  GetLoadedComponent() => this.componentLoaded;
  public UnityEngine.GameObject? GetTabbed         () => this.tabbed;

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
      this.UnloadAsComponent(this.componentCurrentLoaded, immediate: false, enforce: enforce);

      // … ⟶ Load the component by name of `componentName`
      component.SetActive(true);

      if (!immediate) componentLoadAnimation.Reset ();
      else            componentLoadAnimation.Finish();

      this.componentPreviousLoaded = this.componentCurrentLoaded;
      this.componentCurrentLoaded  = componentName;
    }
  }

  private void Start() {
    Dictionary<AnimationSequence> componentsLoadAnimations = new(this.components.Count, this.components.Comparer);
    Vector2Dictionary             componentsOrigins        = new(this.components.Count, this.components.Comparer);
    UnityEngine.GameObject        menu                     = this.components["menu"];
    UnityEngine.GameObject        splash                   = this.components["splash"];

    // … ⟶ Ensure components are (un-)loadable
    foreach (string componentName in this.components.Keys)
    if (!this.componentSpecials.Contains(componentName)) {
      UnityEngine.GameObject component = this.components[componentName];

      // …
      componentsLoadAnimations.Add(componentName, this.componentLoadAnimation.AsCopy());
      componentsOrigins       .Add(componentName, null != component ? component.transform.localPosition : UnityEngine.Vector2.zero);
    }

    this.componentsLoadAnimations = new(componentsLoadAnimations);
    this.componentsOrigins        = new(componentsOrigins);

    // … ⟶ Ensure “menu” components are animatable
    if (null != menu) {
      UnityEngine.RectTransform menuRectTransform = (UnityEngine.RectTransform) menu.transform;

      // …
      this.menuLogoOrigin = (menu.FindChildByName("Logo")?.transform as UnityEngine.RectTransform)?.anchoredPosition ?? menuRectTransform.anchoredPosition;
    }

    // … ⟶ Ensure “splash” components are animatable
    if (null != splash) {
      UnityEngine.RectTransform  splashRectTransform           = (UnityEngine.RectTransform) splash.transform;
      float                      splashRectSizeDelta           = UnityEngine.Mathf.Abs(splashRectTransform.rect.height - splashRectTransform.rect.width);
      UnityEngine.RectTransform? splashBackgroundRectTransform = splash.FindChildByName("Background")?.transform as UnityEngine.RectTransform;

      // … ⟶ Square the dimensions of the “splash” component
      splashRectTransform.anchorMax = splashRectTransform.anchorMin = splashRectTransform.pivot = new(0.5f, 0.0f); // ⟶ Bottom-center
      splashRectTransform.SetRectSize(this.rectTransform.GetRectSize());
      splashRectTransform.SetRectSize(UnityEngine.Vector2.one * UnityEngine.Mathf.Max(splashRectTransform.rect.height, splashRectTransform.rect.width));
      this.splashBackgroundOrigin = splashRectTransform.anchoredPosition;

      if (null != splashBackgroundRectTransform) {
        splashBackgroundRectTransform.pivot            = new(0.5f, 0.0f); // ⟶ Bottom-center
        splashBackgroundRectTransform.anchorMin        = new(0.0f, 0.5f); // ⟶ Mid-center
        splashBackgroundRectTransform.anchorMax        = new(1.0f, 0.5f); //    ^^
        splashBackgroundRectTransform.anchoredPosition = Util.Vector.ExcludeY(splashBackgroundRectTransform.anchoredPosition, splashBackgroundRectTransform.anchoredPosition.y - Util.PercOf(splashRectTransform.GetRectHeight() - splashBackgroundRectTransform.GetRectHeight(), 50.0f) - Util.PercOf(splashRectSizeDelta, 50.0f));
        splashBackgroundRectTransform.SetRectHeight(splashRectTransform.GetRectHeight());
        this.splashBackgroundOrigin = splashBackgroundRectTransform.anchoredPosition;
      }

      // … ⟶ Allow “splash” component (elements) to render transparently
      foreach (UnityEngine.CanvasRenderer splashRenderer in splash.FindHierarchyByComponent<UnityEngine.CanvasRenderer>())
      splashRenderer.cullTransparentMesh = false;
    }

    // … ⟶ Unload all components
    foreach (string componentName in this.components.Keys) {
      if (!this.componentSpecials.Contains(componentName))
      this.UnloadAsComponent(componentName, immediate: true, enforce: false);
    }
  }

  public  void UnloadAsComponent(string componentName, bool immediate = false) => this.UnloadAsComponent(componentName, immediate, true);
  private void UnloadAsComponent(string componentName, bool immediate, bool enforce) {
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
    UnityEngine.UI.Button[]? componentButtons = null;
    UnityEngine.GameObject   menu             = this.components["menu"];
    UnityEngine.GameObject   splash           = this.components["splash"];

    // …
    this.canvas.additionalShaderChannels &= ~(UnityEngine.AdditionalCanvasShaderChannels.Normal | UnityEngine.AdditionalCanvasShaderChannels.Tangent); // ⟶ Is this alright?
    this.tabbedCurrent                    = Util.UI.TabIsActive ? this.tabbedCurrent  : null;                                                          // ⟶ Track tabbed `UnityEngine.GameObject` objects
    this.tabbedPrevious                   = Util.UI.TabIsActive ? this.tabbedPrevious : null;                                                          //    ^^

    if (this.buttonSelectedAnimation.isFinished)
    this.buttonSelectedAnimation.Reset(); // ⟶ Reset component button animation

    if (null != UI.CurrentMusic && !UI.CurrentMusic.isPlaying && !UI.CurrentMusic.loop) /* ⟶ Ensure (background) music is always playing */ {
      UI.CurrentMusic.loop = true;

      UI.CurrentMusic.Stop();
      UI.CurrentMusic.Play();
    }

    switch (Game.CurrentState) /* ⟶ Track component buttons */ {
      case Game.State.Menu: if (null != menu) componentButtons = menu.FindDescendantsByComponent<UnityEngine.UI.Button>().ToArray(); break;
      default: break;
    } if (componentButtons is not null && componentButtons.Length != this.buttons.Length) {
      uint                     count                    = (uint) componentButtons.Length;
      UnityEngine.UI.Button [] buttons                  = Util.Array<UnityEngine.UI.Button> .Create(count);
      UnityEngine.Texture   [] buttonsTextures          = Util.Array<UnityEngine.Texture>   .Create(count);
      UnityEngine.Vector3   [] buttonsLocalScales       = Util.Array<UnityEngine.Vector3>   .Create(count);
      UnityEngine.Vector3   [] buttonsLocalEulerAngles  = Util.Array<UnityEngine.Vector3>   .Create(count);
      AnimationSequence     [] buttonsEntryAnimations   = Util.Array<AnimationSequence>     .Create(count);
      UnityEngine.Vector2   [] buttonsAnchoredPositions = Util.Array<UnityEngine.Vector2>   .Create(count);
      UnityEngine.GameObject[] buttonsTabbed            = Util.Array<UnityEngine.GameObject>.Create(0u);

      // …
      componentButtons.Sort(static (buttonA, buttonB) => {
        (UnityEngine.Vector2 positionA, UnityEngine.Vector2 positionB) = (buttonA.GetAnchoredPosition(fallback: true), buttonB.GetAnchoredPosition(fallback: true));
        return positionA.y < positionB.y ? -1 : positionA.y > positionB.y ? +1 : positionA.x <= positionB.x ? -1 : +1;
      });

      buttonsTabbed = componentButtons.ConvertAll(static button => button.gameObject);

      buttonsTabbed  .Reverse ();
      Util.UI.TabList.Clear   ();
      Util.UI.TabList.AddRange(buttonsTabbed);

      foreach (UnityEngine.UI.Button button in componentButtons) {
        UnityEngine.Transform buttonTransform = button.transform;
        int                   index           = this.buttons.IndexOf(button);

        // …
        count--;
        buttonsTextures         [count]       = index == -1 ? button.GetTexture()!               : this.buttonsTextures         [index];
        buttonsLocalScales      [count]       = index == -1 ? buttonTransform.localScale         : this.buttonsLocalScales      [index];
        buttonsLocalEulerAngles [count]       = index == -1 ? buttonTransform.localEulerAngles   : this.buttonsLocalEulerAngles [index];
        buttonsEntryAnimations  [count]       = index == -1 ? this.buttonEntryAnimation.AsCopy() : this.buttonsEntryAnimations  [index];
        buttonsEntryAnimations  [count].delay = this.buttonEntryAnimationDelay * (componentButtons.Length - count - 1);
        buttonsAnchoredPositions[count]       = index == -1 ? buttonTransform.GetAnchoredPosition(fallback: true) : this.buttonsAnchoredPositions[index];
        buttons                 [count]       = index == -1 ? button                                              : this.buttons                 [index];

        buttonsEntryAnimations[count].Reset();
      }

      this.buttons                  = buttons;
      this.buttonsAnchoredPositions = buttonsAnchoredPositions;
      this.buttonsEntryAnimations   = buttonsEntryAnimations;
      this.buttonsLocalEulerAngles  = buttonsLocalEulerAngles;
      this.buttonsLocalScales       = buttonsLocalScales;
      this.buttonsTextures          = buttonsTextures;
    }

    // … ⟶ Animate/ Invoke selected (pointed/ tabbed) objects
    for (uint index = (uint) this.buttons.Length; 0u != index--; ) {
      UnityEngine.UI.Button button          = this.buttons[index];
      bool                  buttonIsPointed = Util.Pointed.IsPointed(button, out PointedInfo pointed); // ⟶ `::state` is `DeviceState.UNKNOWN` by default
      DeviceState           buttonTabState  = button.gameObject == this.tabbedPrevious ? DeviceState.LEAVE : Util.UI.Tabbed == button.gameObject ? button.gameObject == this.tabbedCurrent ? DeviceState.ONGOING : DeviceState.ENTER : DeviceState.UNKNOWN;
      UnityEngine.Transform buttonTransform = button.transform;

      // … ⟶ Component buttons
      if (buttonIsPointed)
      Util.UI.BlurTabs();

      if (Util.UI.TabIsActive || Util.UI.TabIsBlurred || buttonIsPointed) {
        // … ⟶ Reset the component button’s animation state
        if (DeviceState.LEAVE == buttonTabState || DeviceState.LEAVE == pointed.state || Util.UI.TabIsBlurred) {
          button         .SetTexture         (this.buttonsTextures         [index]);
          buttonTransform.SetAnchoredPosition(this.buttonsAnchoredPositions[index], fallback: true);

          buttonTransform.localEulerAngles = this.buttonsLocalEulerAngles[index];
          buttonTransform.localScale       = this.buttonsLocalScales     [index];
        }

        // … ⟶ Make a clicking noise when entered/ selected 🎵
        if (
          DeviceState.ENTER == buttonTabState || (Util.Keys.Prompted   && DeviceState.END   >  buttonTabState) ||
          DeviceState.ENTER == pointed.state  || (pointed.IsPointing() && DeviceState.BEGIN == pointed.pointerState)
        ) {
          buttonTransform.SetAnchoredPosition(this.buttonsAnchoredPositions[index], fallback: true);
          this.buttonSelectedAnimation.Reset();

          UI.Load.UriAsSound("click.mp3", Load.Idle);
        }

        // … ⟶ Do stuff; be invoked‥ 💡
        if (DeviceState.END > buttonTabState || DeviceState.END > pointed.state) {
          float buttonAnimationTeeter = Util.Cast<float>(this.buttonSelectedAnimation["teeter"]);
          bool  buttonKeyPrompted     = DeviceState.END > buttonTabState && Util.Keys.Prompted;
          bool  buttonPointerPrompted = DeviceState.END > pointed.state  && pointed.IsClicking();
          bool  buttonPrompted        = buttonKeyPrompted || buttonPointerPrompted;

          // … ⟶ Animate
          buttonTransform.localEulerAngles = this.buttonsLocalEulerAngles[index] + (Util.Vector.MaskZ(UnityEngine.Vector3.one) * buttonAnimationTeeter * 5.0f /* ⟶ in Degrees */);
          buttonTransform.localScale       = this.buttonsLocalScales     [index] + (this.buttonsLocalScales[index] * UnityEngine.Mathf.Abs(buttonAnimationTeeter) * Util.Perc(10.0f));

          // … ⟶ Invoke
          if (buttonPointerPrompted) {
            foreach (ref readonly PointerInfo pointer in pointed.pointers) // ⟶ Only for posterity
            button.OnPointerClick(new(Util.Pointed.EventSystem!) {position = pointer.position});
          }

          if (button.tag == "MenuQuitButton") {
            UI.Load.UriAsTexture("menu-button-4.png", button.SetTexture);
            if (buttonPrompted) Util.Game.Quit();
          }

          else {
            UI.Load.UriAsTexture("menu-button-3.png", button.SetTexture);

            if (buttonPrompted) {
              if      (button.tag == "MenuStartButton")    Game.LoadState(Game.State.Tutorial);
              else if (button.tag == "MenuCreditsButton")  Game.LoadState(Game.State.Credits);
              else if (button.tag == "MenuSettingsButton") Game.LoadState(Game.State.Settings);
            }
          }
        }
      } else buttonTransform.SetAnchoredPosition(new(this.buttonsAnchoredPositions[index].x, this.buttonsAnchoredPositions[index].y + (Util.PercOf(buttonTransform.GetRectHeight(), 50.0f) * (float) (1.0 - this.buttonsEntryAnimations[index].easedProgress))), fallback: true);
    }

    // … ⟶ Animate component (un)loads
    foreach (string componentName in this.components.Keys)
    if (!this.componentSpecials.Contains(componentName)) {
      UnityEngine.GameObject component = this.components[componentName];

      // …
      if (null != component && component.activeSelf) {
        UnityEngine.RectTransform        componentRectTransform = (UnityEngine.RectTransform) component.transform;
        ref readonly UnityEngine.Vector2 componentOrigin        = ref this.componentsOrigins[componentName];
        AnimationSequence                componentLoadAnimation = this.componentsLoadAnimations[componentName];
        bool                             componentIsLoaded      = componentName == this.componentCurrentLoaded;
        float                            componentHeight        = UnityEngine.Mathf.Max(this.rectTransform.GetRectHeight(), componentRectTransform.GetRectHeight());

        // …
        componentRectTransform.anchoredPosition = Util.Vector.ExcludeY(componentRectTransform.anchoredPosition, value: componentOrigin.y - (componentHeight * (float) (componentIsLoaded ? 1.0 - componentLoadAnimation.easedProgress : componentLoadAnimation.easedProgress)));
        if (!componentIsLoaded && componentLoadAnimation.isFinished) component.SetActive(false);
      }
    }

    // … ⟶ Animate “splash” component
    if (null != splash && splash.activeSelf) {
      UnityEngine.UI.RawImage?   splashBackground                 = splash.FindChildByName("Background")?.GetComponent<UnityEngine.UI.RawImage>();
      float                      splashExitAnimationProgress      = UnityEngine.Mathf.Max((float) this.splashExitAnimation.progress, 0.0f);
      TMPro.TextMeshProUGUI?     splashHintText                   = splash.FindDescendantByName("Hint")?.GetComponent<TMPro.TextMeshProUGUI>();
      float                      splashLoadAnimationBackgroundTop = Util.Cast<float>(this.splashLoadAnimation["background-top"]);
      float                      splashLoadAnimationOpacity       = 1.0f - Util.Cast<float>(this.splashLoadAnimation["transparency"]);
      float                      splashLoadAnimationProgress      = (float) this.splashLoadAnimation.progress;
      UnityEngine.RectTransform? splashProgressBar                = splash.FindChildByName("Progress")?.transform as UnityEngine.RectTransform;
      UnityEngine.Vector2        splashRectSize                   = ((UnityEngine.RectTransform) splash.transform).GetRectSize();

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
      splash.SetAlpha(1.0f - splashExitAnimationProgress);

      // … ⟶ Animate “splash” background
      if (null != splashBackground) {
        UnityEngine.RectTransform splashBackgroundRectTransform = splashBackground.rectTransform;

        // …
        splashBackground.SetAlpha(splashLoadAnimationOpacity);
        splashBackgroundRectTransform.anchoredPosition = Util.Vector.ExcludeY(splashBackgroundRectTransform.anchoredPosition, value: this.splashBackgroundOrigin.y - (splashLoadAnimationBackgroundTop * splashRectSize.y));
      }

      // … ⟶ Animate “splash” progress bar
      if (null != splashProgressBar) {
        splashProgressBar.SetAlpha    (1.0f - splashExitAnimationProgress);
        splashProgressBar.SetRectWidth(splashLoadAnimationProgress * splashRectSize.x);
      }

      // … ⟶ Animate “splash” text
      foreach (TMPro.TextMeshProUGUI splashText in splash.FindHierarchyByComponent<TMPro.TextMeshProUGUI>())
      splashText.SetAlpha(splashLoadAnimationOpacity);

      // … ⟶ Load “menu” component
      if (this.splashExitAnimation.isFinished) {
        splash.SetActive(false);
        this.LoadComponent("menu", immediate: true, enforce: false);
      }
    }

    // … ⟶ Animate “menu” component
    if (null != menu && menu.activeInHierarchy && this.componentCurrentLoaded == "menu") {
      UnityEngine.UI.RawImage?   menuLogo          = menu.FindChildByName("Logo")?.GetComponent<UnityEngine.UI.RawImage>();
      float                      menuLogoTopOffset = 50.0f;
      UnityEngine.RectTransform? menuLogoTransform = null != menuLogo ? menuLogo.rectTransform : null;

      // … ⟶ Play main “menu” music
      UI.Load.UriAsMusic("calm.mp3", Load.Idle);

      // … ⟶ Animate “menu” logo
      if (null != menuLogoTransform) {
        menuLogoTransform.pivot            = new(0.5f, 0.5f);
        menuLogoTransform.anchorMin        = new(0.0f, 1.0f); // ⟶ Top-center
        menuLogoTransform.anchorMax        = new(1.0f, 1.0f); //    ^^
        menuLogoTransform.anchoredPosition = new(menuLogoTransform.anchoredPosition.x, this.menuLogoOrigin.y + (menuLogoTopOffset * (1.0f - (float) this.componentsLoadAnimations["menu"].easedProgress)));
      }
    }

    // …
    this.tabbedPrevious = Util.UI.Tabbed != this.tabbedCurrent ? this.tabbedCurrent : this.tabbedPrevious; // ⟶ Track tabbed `UnityEngine.GameObject` objects
    this.tabbedCurrent  = Util.UI.TabIsActive ? Util.UI.TabList[Util.UI.TabIndex] : this.tabbedCurrent;    //    ^^
  }
}
