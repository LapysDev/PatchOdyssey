using PatchOdyssey;

/* … */
[UnityEngine.RequireComponent(typeof(UnityEngine.Canvas))]
[UnityEngine.RequireComponent(typeof(UnityEngine.EventSystems.EventSystem))]
[UnityEngine.RequireComponent(typeof(UnityEngine.UI.GraphicRaycaster))]
[UnityEngine.RequireComponent(typeof(UnityEngine.RectTransform))]
public class UI : UnityEngine.MonoBehaviour {
  public enum T : byte {} // → Dummy type

  public sealed class ComponentLoadInfo {
    public Animation.UISequence   animation  = null!;
    public UnityEngine.GameObject gameObject = null!;
    public bool                   loading    = true;
  }

  public class EventDataInfo {
    public UnityEngine.InputSystem.InputDevice? device    = null;  //
    public string                               id        = null!; // → `UnityEngine.InputSystem.InputAction.CallbackContext::control.path`, …
    public float                                timestamp = 0.0f;  // → `UnityEngine.Time.realtimeSinceStartup`
  }
    public sealed class KeyboardInfo : EventDataInfo { public UnityEngine.KeyCode key = UnityEngine.KeyCode.None; }
    public        class PointerInfo  : EventDataInfo {}
      public sealed class PointerActivationInfo : PointerInfo { public bool                value = false; }
      public sealed class PointerPositionInfo   : PointerInfo { public UnityEngine.Vector2 value = UnityEngine.Vector2.zero; }
      public sealed class PointerComponentInfo  : PointerInfo {
        public UnityEngine.GameObject gameObject = null!;
        public UnityEngine.Vector2    position   = UnityEngine.Vector2.zero;
      }

  public sealed class EventInputInfo {
    public readonly (T, System.Collections.Generic.List<UI.KeyboardInfo>          active)                                                                   keyboards = (default(T), new());
    public readonly (T, System.Collections.Generic.List<UI.PointerActivationInfo> active, System.Collections.Generic.List<UI.PointerPositionInfo> position) pointers  = (default(T), new(), new());
  }

  [System.Flags]
  public enum LoadAction : byte {
    AsMusic       = (byte) 0x4u,
    AsSoundEffect = (byte) 0x8u,
    Deferred      = (byte) 0x1u,
    Immediately   = (byte) 0x2u
  }

  /* … */
  private static readonly System.Func<double, double> TEETER_ANIMATION_FUNCTION              = PatchOdyssey.Animation.Function.EaseInOutCircular; // → 𝑓
  private const           float                       TEETER_ANIMATION_DURATION              = 2.00f;                                             // → in Seconds greater than `UnityEngine.Time.deltaTime`
  private static readonly System.Func<double, double> LOAD_ANIMATION_FUNCTION                = PatchOdyssey.Animation.Function.EaseInOutQuintic;  // → 𝑓
  private const           float                       LOAD_ANIMATION_DURATION                = 0.40f;                                             // → in Seconds greater than `UnityEngine.Time.deltaTime`
  private static readonly float                       IMMEDIATE_TEXT_BACKGROUND_TRANSPARENCY = Util.Percent(65.0f);                               //
  private const           float                       COMPONENT_TAB_INITIAL_DELAY            = 1.00f;                                             // → in Seconds; UI keyboard repeat delay
  private const           float                       BACKGROUND_UNLOAD_DURATION             = 2.00f;                                             //
  private static readonly float                       BACKGROUND_TRANSPARENCY                = Util.Percent(65.0f);                               //
  private const           float                       BACKGROUND_RESPONSIVENESS              = 20.00f;                                            // → Higher values are less responsive
  private static readonly (
    string BACK,
    string MENU, string MENU_CREDITS, string MENU_OPTIONS, string MENU_QUIT, string MENU_START
  ) COMPONENT_TAGS = ("BackButton", "MenuButton", "MenuCreditsButton", "MenuOptionsButton", "MenuQuitButton", "MenuStartButton"); // → Tag identifiers for functional UI components

  [PatchOdyssey.ReadWriteInInspector] private                float                                                                                                   animationDurationElapsed        =  0.0f; // → Generic; For all/ any animations
  [PatchOdyssey.ReadOnlyInInspector]  public  static         float                                                                                                   averageDeltaTime                => 0uL != UI.updateCount ? (float) (UI.totalDeltaTime / UI.updateCount) : 0.0f;
  [PatchOdyssey.ReadWriteInInspector] private readonly       System.Collections.Generic.Dictionary<string, UnityEngine.AudioSource?>                                 audios                          =  new(); // → List of `UnityEngine.AudioSource` objects spawned via `UI::LoadAudio…()`
  [PatchOdyssey.ReadWriteInInspector] public                 UnityEngine.UI.RawImage?                                                                                background                      =  null;
  [PatchOdyssey.ReadWriteInInspector] private readonly       System.Collections.Generic.List<UI.ComponentLoadInfo>                                                   componentLoads                  =  new(); // → List of UI `UI::components` (un-)loaded from the interface
  [PatchOdyssey.ReadWriteInInspector] public  /* readonly */ GameObjectReadOnlyDictionary                                                                            components                      =  new(11) {{"credits", null!}, {"combat", null!}, {"dialogue", null!}, {"inventory", null!}, {"menu", null!}, {"options", null!}, {"pause", null!}, {"prompt", null!}, {"splash", null!}, {"tooltip:hud", null!}, {"tooltip:scene", null!}};
  [PatchOdyssey.ReadWriteInInspector] private                bool                                                                                                    componentTabActive              =  false;
  [PatchOdyssey.ReadWriteInInspector] private                int                                                                                                     componentTabIndex               =  -1;
  [PatchOdyssey.ReadWriteInInspector] private                float                                                                                                   componentTabInitialDelayElapsed =  0.0f;
  [PatchOdyssey.ReadOnlyInInspector]  private /* readonly */ System.Collections.Generic.List<UnityEngine.GameObject>                                                 componentTabList                => this.tabbed; // → List of "tabbable" UI components
  [PatchOdyssey.ReadOnlyInInspector]  public  /* readonly */ UnityEngine.GameObject                                                                                  immediateText                   =  null!;       // → Debug UI messages
  [PatchOdyssey.ReadWriteInInspector] private readonly       UI.EventInputInfo                                                                                       inputs                          =  new();
  [UnityEngine .HideInInspector]      public  readonly       System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<UI.KeyboardInfo>>         keyboards                       =  new(3) {{"active", new()}, {"active:begin", new()}, {"active:end", new()}}; // → ⌨️
  [PatchOdyssey.ReadOnlyInInspector]  public  static         UI?                                                                                                     main                            =  null;
  [PatchOdyssey.ReadOnlyInInspector]  public  static         float                                                                                                   maximumDeltaTime                =  float.NegativeInfinity;
  [PatchOdyssey.ReadOnlyInInspector]  public  static         float                                                                                                   minimumDeltaTime                =  float.PositiveInfinity;
  [PatchOdyssey.ReadOnlyInInspector]  public                 UnityEngine.AudioSource?                                                                                music                           =  null; // → Current background music (intended to be) playing
  [UnityEngine .HideInInspector]      public  readonly       System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<UI.PointerComponentInfo>> pointers                        =  new(6) {{"active", new()}, {"active:begin", new()}, {"active:end", new()}, {"hover", new()}, {"hover:begin", new()}, {"hover:end", new()}}; // → 👆 🖱️
  [PatchOdyssey.ReadOnlyInInspector]  public                 bool                                                                                                    prompted                        =  false; // → Examples: "Is the 'Enter' key pressed?" or "Is the 'Proceed' button tapped?"
  [PatchOdyssey.ReadOnlyInInspector]  public  /* readonly */ System.Collections.Generic.List<UnityEngine.GameObject>                                                 tabbed                          =  new();
  [PatchOdyssey.ReadOnlyInInspector]  public  static         double                                                                                                  totalDeltaTime                  =  0.0;
  [PatchOdyssey.ReadOnlyInInspector]  public  static         ulong                                                                                                   updateCount                     =  0uL;

  /* … */
  private void Awake() {
    UI.main ??= this;

    // … → Make rendering faster
    this.GetComponent<UnityEngine.Canvas>().pixelPerfect = false;
  }

  void EndEventData<T>(System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<T>> informationList) where T : UI.EventDataInfo {
    this.inputs.keyboards.active  .Clear();
    this.inputs.pointers .active  .Clear();
    this.inputs.pointers .position.Clear();

    foreach (string key in informationList.Keys)
    if (!key.Contains(":")) {
      System.Collections.Generic.List<T> _     = informationList[key];
      System.Collections.Generic.List<T> begin = informationList.TryGetValue(key + ":begin", out begin) ? begin : new();
      System.Collections.Generic.List<T> end   = informationList.TryGetValue(key + ":end",   out end)   ? end   : new();

      // …
      foreach (T information in Util.ArrayFrom(_, begin)) {
        if (!end.Exists(subinformation => information.id == subinformation.id))
        end.Add(information);
      }

      _    .Clear();
      begin.Clear();
    }
  }

  public void HideText(float duration = 0.0f) {
    if (!(this.immediateText?.activeSelf ?? false))
    return;

    this.immediateText.SetActive(false);

    foreach (UnityEngine.UI.RawImage image in this.immediateText.FindHierarchyByComponent<UnityEngine.UI.RawImage>()) {
      image.CrossFadeAlpha(0.0f, duration, true);
      image.color = Util.IIFE<UnityEngine.Color>(color => color.a = 0.0f)(image.color);
    }

    foreach (TMPro.TextMeshProUGUI textMesh in this.immediateText.FindHierarchyByComponent<TMPro.TextMeshProUGUI>()) {
      textMesh.alpha = 0.0f;
      textMesh.text  = "";
    }
  }

  private bool IsPointerFromMouseInput(UI.PointerInfo information) {
    return null == information.device && null == information.id;
  }

  private bool IsPointerFromMouseInputSystem(UI.PointerInfo information) {
    return information.device is UnityEngine.InputSystem.Mouse;
  }

  private bool IsPointerFromTouchscreenInput(UI.PointerInfo information) {
    return null == information.device && System.Array.TrueForAll((information.id ?? "").ToCharArray(), char.IsDigit);
  }

  private bool IsPointerFromTouchscreenInputSystem(UI.PointerInfo information) {
    return information.device is UnityEngine.InputSystem.Touchscreen;
  }

  public bool IsSelected(UnityEngine.Component component) {
    return this.IsSelected(component.gameObject);
  }

  public bool IsSelected(UnityEngine.GameObject gameObject) {
    return null != gameObject && (
      (this.componentTabActive && this.componentTabIndex != -1 && null != this.componentTabList && this.prompted ? gameObject == this.componentTabList[this.componentTabIndex] : false) || (
        this.pointers["active:end"].Exists(pointerInformation => gameObject == pointerInformation.gameObject) &&
        this.pointers["hover"]     .Exists(pointerInformation => gameObject == pointerInformation.gameObject)
      )
    );
  }

  public void LoadAudio(string path, UI.LoadAction action = UI.LoadAction.AsSoundEffect | UI.LoadAction.Deferred, System.Action<UnityEngine.AudioSource?>? callback = null) {
    void HandleAudio(UnityEngine.AudioSource audioSource) {
      audioSource.volume = System.Math.Clamp((Settings.GetPropertyAsFloat($"Options/{(UI.LoadAction.AsMusic == (action & UI.LoadAction.AsMusic) ? "Volume" : "Sound")}") ?? 100.0f) / 100.0f, 0.0f, 1.0f);

      // …
      if (null != this)
      audioSource.transform.SetParent(this.transform, false);

      if (0x0u != (action & (UI.LoadAction.Deferred | UI.LoadAction.Immediately))) {
        if (0x0u != (action & UI.LoadAction.Deferred) ? !audioSource.isPlaying : true)
        audioSource.Play();

        return;
      }

      audioSource.Stop();
    }

    // …
    if (null == path)
    return;

    path = System.IO.Path.Combine(
      UI.LoadAction.AsMusic       == (action & UI.LoadAction.AsMusic)       ? new[] {Util.GetAssetPath(), "Music", path} :
      UI.LoadAction.AsSoundEffect == (action & UI.LoadAction.AsSoundEffect) ? new[] {Util.GetAssetPath(), "SFX",   path} :
      new[] {Util.GetAssetPath(), path}
    );

    if (this.audios.TryGetValue(path, out UnityEngine.AudioSource? audioSource) ? null != audioSource : false) {
      HandleAudio     (audioSource!);
      callback?.Invoke(audioSource!);

      return;
    }

    Util.LoadURIAsAudioClip(path, audioClip => {
      UnityEngine.AudioSource? audioSource = null;

      // …
      if (null != audioClip) {
        audioSource              = new UnityEngine.GameObject((UI.LoadAction.AsMusic == (action & UI.LoadAction.AsMusic) ? "🎼 " : UI.LoadAction.AsSoundEffect == (action & UI.LoadAction.AsSoundEffect) ? "🔊 " : "🎵 ") + System.IO.Path.GetFileNameWithoutExtension(path), typeof(UnityEngine.RectTransform)).AddComponent<UnityEngine.AudioSource>();
        audioSource.clip         = audioClip;
        audioSource.loop         = UI.LoadAction.AsMusic       == (action & UI.LoadAction.AsMusic);
        audioSource.spatialBlend = UI.LoadAction.AsSoundEffect == (action & UI.LoadAction.AsSoundEffect) ? Util.Percent(100.0f) : Util.Percent(0.0f);
        this.audios[path]        = audioSource;

        HandleAudio(audioSource);
      }

      callback?.Invoke(audioSource);
    }, Util.LoadAsynchronously);

    if (!this.audios.ContainsKey(path))
    this.audios.Add(path, null);
  }

  public void LoadBackground(string path, float loadDuration = LOAD_ANIMATION_DURATION, UI.LoadAction action = default) {
    if (null != this.background)
    this.LoadTexture(path, action, texture => {
      this.background.texture = texture!;
      this.background.CrossFadeAlpha(Game.main?.isLoaded ?? false ? BACKGROUND_TRANSPARENCY : Util.Percent(100.0f), loadDuration, true);
    });
  }

  public void LoadComponent(string name, UI.LoadAction action = UI.LoadAction.Deferred) {
    this.componentLoads.RemoveAll(static _ => null == _.gameObject);

    if (this.components.TryGetValue(name, out UnityEngine.GameObject gameObject))
    if (null != gameObject) {
      foreach (UI.ComponentLoadInfo componentLoad in this.componentLoads)
      if (componentLoad.gameObject == gameObject) {
        this.OnTabBlur();

        if (false == componentLoad.loading) componentLoad.animation.Reset();
        componentLoad.animation.duration = 0x0u != (action & UI.LoadAction.Immediately) ? 0.0 : 0.4;
        componentLoad.loading            = true;

        break;
      }
    }
  }

  private void LoadEventData<T>(System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<T>> informationList, UI.LoadAction action = UI.LoadAction.Deferred) where T : UI.EventDataInfo {
    foreach (string key in informationList.Keys)
    if (!key.Contains(":")) {
      System.Collections.Generic.List<T> _         = informationList[key];
      System.Collections.Generic.List<T> begin     = informationList.TryGetValue(key + ":begin", out begin) ? begin : new();
      System.Collections.Generic.List<T> end       = informationList.TryGetValue(key + ":end",   out end)   ? end   : new();
      float                              timestamp = UnityEngine.Time.realtimeSinceStartup;

      // …
      foreach (T information in end) {
        _    .RemoveAll(subinformation => information.id == subinformation.id);
        begin.RemoveAll(subinformation => information.id == subinformation.id);
      }

      (Util.Switch(typeof(T), new() {
        {typeof(UI.KeyboardInfo), (System.Action<System.Collections.Generic.List<UI.KeyboardInfo>>) ((System.Collections.Generic.List<UI.KeyboardInfo> _) => {
          foreach (var information in (begin as System.Collections.Generic.List<UI.KeyboardInfo>)!) {
            if (!_.Exists(subinformation => information.id == subinformation.id))
            _.Add(new() {device = information.device, id = information.id, key = information.key, timestamp = timestamp});
          }
        })},

        {typeof(UI.PointerComponentInfo), (System.Action<System.Collections.Generic.List<UI.PointerComponentInfo>>) ((System.Collections.Generic.List<UI.PointerComponentInfo> _) => {
          foreach (var information in (begin as System.Collections.Generic.List<UI.PointerComponentInfo>)!) {
            if (!_.Exists(subinformation => information.gameObject == subinformation.gameObject && information.id == subinformation.id))
            _.Add(new() {device = information.device, gameObject = information.gameObject, id = information.id, position = information.position, timestamp = timestamp});
          }
        })}
      }) as System.Action<System.Collections.Generic.List<T>>)?.DynamicInvoke(_);
    }
  }

  public void LoadMusic(string path, UI.LoadAction action = UI.LoadAction.Immediately, System.Action<UnityEngine.AudioSource?>? callback = null) {
    this.LoadAudio(path, UI.LoadAction.AsMusic | action, audioSource => {
      if (null != audioSource && null != this.music)
        this.music.Stop();

      this.music = audioSource;
      callback?.Invoke(audioSource);
    });
  }

  public void LoadSoundEffect(string path, UI.LoadAction action = UI.LoadAction.Deferred, System.Action<UnityEngine.AudioSource?>? callback = null) {
    this.LoadAudio(path, UI.LoadAction.AsSoundEffect | action, audioSource => callback?.Invoke(audioSource));
  }

  public void LoadTexture(string path, UI.LoadAction action = UI.LoadAction.Immediately, System.Action<UnityEngine.Texture?>? callback = null) {
    if (null != path)
    Util.LoadURIAsTexture2D(System.IO.Path.Combine(new[] {Util.GetAssetPath(), "UI", path}), texture => callback?.Invoke(texture), Util.LoadAsynchronously);
  }

  public void OnInputKey(UnityEngine.InputSystem.InputAction.CallbackContext context) {
    float           uiTimestamp = UnityEngine.Time.realtimeSinceStartup;
    UI.KeyboardInfo information = new() {device = context.control.device, id = context.control.path, key = context.ReadValue<UnityEngine.KeyCode>(), timestamp = uiTimestamp};

    // …
    if (UnityEngine.InputSystem.InputActionPhase.Performed != context.phase) {
      this.inputs.pointers.active.RemoveAll(keyboardInformation => information.id == keyboardInformation.id);
      this.keyboards["active:end"].Clear();

      foreach (System.Collections.Generic.List<UI.KeyboardInfo> keyboardInformationList in new[] {this.keyboards["active"], this.keyboards["active:begin"]}) {
        foreach (UI.KeyboardInfo keyboardInformation in keyboardInformationList)
        this.keyboards["active:end"].Add(new() {device = keyboardInformation.device, id = keyboardInformation.id, key = keyboardInformation.key, timestamp = uiTimestamp});
      }
    }

    else {
      if (!this.inputs.keyboards.active.Exists(keyboardInformation => information.id == keyboardInformation.id))
      this.inputs.keyboards.active.Add(information);
    }
  }

  public void OnInputPointer(UnityEngine.InputSystem.InputAction.CallbackContext context) {
    float                    uiTimestamp = UnityEngine.Time.realtimeSinceStartup;
    UI.PointerActivationInfo information = new() {device = context.control.device, id = context.control.path, timestamp = uiTimestamp, value = context.ReadValueAsButton()};

    // …
    if (UnityEngine.InputSystem.InputActionPhase.Performed != context.phase) {
      this.inputs.pointers.active.RemoveAll(pointerInformation => information.id == pointerInformation.id);
      this.pointers["active:end"].Clear();

      foreach (System.Collections.Generic.List<UI.PointerComponentInfo> pointerInformationList in new[] {this.pointers["active"], this.pointers["active:begin"]}) {
        foreach (UI.PointerComponentInfo pointerInformation in pointerInformationList)
        this.pointers["active:end"].Add(new() {device = pointerInformation.device, gameObject = pointerInformation.gameObject, id = pointerInformation.id, position = pointerInformation.position, timestamp = uiTimestamp});
      }
    }

    else if (information.value) {
      if (!this.inputs.pointers.active.Exists(pointerInformation => information.id == pointerInformation.id)) { this.inputs.pointers.active.Add(information); }
      this.OnInputPointer(information, this.OnPointerBegin);
    }

    else {
      this.inputs.pointers.active.RemoveAll(pointerInformation => information.id == pointerInformation.id);
      this.OnInputPointer(information, this.OnPointerEnd);
    }
  }

  private void OnInputPointer(UI.PointerActivationInfo information, System.Action<UI.PointerPositionInfo> method) {
    foreach (UI.PointerPositionInfo pointerInformation in this.inputs.pointers.position)
    if (information.device == pointerInformation.device) {
      if (information.device is UnityEngine.InputSystem.Touchscreen && (
        new string(System.Array.FindAll(information       .id.ToCharArray(), char.IsDigit)) ==
        new string(System.Array.FindAll(pointerInformation.id.ToCharArray(), char.IsDigit))
      )) continue;

      method(pointerInformation); // → `UI.OnPointer(…)`, `UI.OnPointerBegin(…)`, `UI.OnPointerEnd(…)`
      break;
    }
  }

  public void OnInputPointerMove(UnityEngine.InputSystem.InputAction.CallbackContext context) {
    float                  uiTimestamp = UnityEngine.Time.realtimeSinceStartup;
    UI.PointerPositionInfo information = new() {device = context.control.device, id = context.control.path, timestamp = uiTimestamp, value = context.ReadValue<UnityEngine.Vector2>()};

    // …
    if (UnityEngine.InputSystem.InputActionPhase.Performed != context.phase) {
      this.inputs.pointers.position.RemoveAll(pointerInformation => information.id == pointerInformation.id);
      this.pointers["hover:end"]   .Clear();

      foreach (System.Collections.Generic.List<UI.PointerComponentInfo> pointerInformationList in new[] {this.pointers["hover"], this.pointers["hover:begin"]}) {
        foreach (UI.PointerComponentInfo pointerInformation in pointerInformationList)
        this.pointers["hover:end"].Add(new() {device = pointerInformation.device, gameObject = pointerInformation.gameObject, id = pointerInformation.id, position = pointerInformation.position, timestamp = uiTimestamp});
      }
    }

    else {
      int index = this.inputs.pointers.position.FindIndex(pointerInformation => information.id == pointerInformation.id);

      // …
      if (index == -1) this.inputs.pointers.position.Add(information);
      else             this.inputs.pointers.position[index] = information;

      this.OnPointerMove(information);
    }
  }

  private void OnKey(UI.KeyboardInfo information) {
    if (!this.keyboards["active"].Exists(keyboardInformation => keyboardInformation.key == information.key))
    this.keyboards["active"].Add(new() {key = information.key, timestamp = UnityEngine.Time.realtimeSinceStartup});
  }

  private void OnKeyBegin(UI.KeyboardInfo information) {
    if (!this.keyboards["active:begin"].Exists(keyboardInformation => keyboardInformation.key == information.key))
    this.keyboards["active:begin"].Add(new() {key = information.key, timestamp = UnityEngine.Time.realtimeSinceStartup});
  }

  private void OnKeyEnd(UI.KeyboardInfo information) {
    if (!this.keyboards["active:end"].Exists(keyboardInformation => keyboardInformation.key == information.key))
    this.keyboards["active:end"].Add(new() {key = information.key, timestamp = UnityEngine.Time.realtimeSinceStartup});
  }

  public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context) {
    Game.main?.player?.MoveBy(context.ReadValue<UnityEngine.Vector2>());
  }

  private void OnPointer(UI.PointerPositionInfo information) {
    foreach (UI.PointerComponentInfo pointerInformation in this.pointers["active"])
    if (information.id == pointerInformation.id) {
      pointerInformation.device   = information.device;
      pointerInformation.position = information.value;
    }
  }

  private void OnPointerBegin(UI.PointerPositionInfo information) {
    UnityEngine.EventSystems.EventSystem?                                   uiEventSystem = this.GetComponent<UnityEngine.EventSystems.EventSystem>();
    System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult> uiRaycasts    = new();
    float                                                                   uiTimestamp   = UnityEngine.Time.realtimeSinceStartup;

    // …
    if (null != uiEventSystem)
    this.GetComponent<UnityEngine.UI.GraphicRaycaster>()?.Raycast(new(uiEventSystem) {position = information.value}, uiRaycasts);

    if (
      // → Avoid `UnityEngine.Input` and `UnityEngine.InputSystem` colliding
      this.IsPointerFromTouchscreenInputSystem(information) ? false == this.pointers["active:begin"].Exists(this.IsPointerFromTouchscreenInput)       :
      this.IsPointerFromMouseInputSystem      (information) ? false == this.pointers["active:begin"].Exists(this.IsPointerFromMouseInput)             :
      this.IsPointerFromTouchscreenInput      (information) ? false == this.pointers["active:begin"].Exists(this.IsPointerFromTouchscreenInputSystem) :
      this.IsPointerFromMouseInput            (information) ? false == this.pointers["active:begin"].Exists(this.IsPointerFromMouseInputSystem)       :
      true
    ) {
      if (0 == uiRaycasts.Count) {
        if (!this.pointers["active:begin"].Exists(pointerInformation => information.id == pointerInformation.id))
        this.pointers["active:begin"].Add(new() {device = information.device, id = information.id, position = information.value, timestamp = uiTimestamp});
      }

      else foreach (UnityEngine.EventSystems.RaycastResult uiRaycast in uiRaycasts) {
        if (!this.pointers["active:begin"].Exists(pointerInformation => information.id == pointerInformation.id && pointerInformation.gameObject == uiRaycast.gameObject))
        this.pointers["active:begin"].Add(new() {device = information.device, gameObject = uiRaycast.gameObject, id = information.id, position = information.value, timestamp = uiTimestamp});
      }
    }
  }

  private void OnPointerEnd(UI.PointerPositionInfo information) {
    UnityEngine.EventSystems.EventSystem?                                   uiEventSystem = this.GetComponent<UnityEngine.EventSystems.EventSystem>();
    System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult> uiRaycasts    = new();
    float                                                                   uiTimestamp   = UnityEngine.Time.realtimeSinceStartup;

    // …
    if (null != uiEventSystem)
    this.GetComponent<UnityEngine.UI.GraphicRaycaster>()?.Raycast(new(uiEventSystem) {position = information.value}, uiRaycasts);

    if (
      // → Avoid `UnityEngine.Input` and `UnityEngine.InputSystem` colliding
      this.IsPointerFromTouchscreenInputSystem(information) ? false == this.pointers["active:end"].Exists(this.IsPointerFromTouchscreenInput)       :
      this.IsPointerFromMouseInputSystem      (information) ? false == this.pointers["active:end"].Exists(this.IsPointerFromMouseInput)             :
      this.IsPointerFromTouchscreenInput      (information) ? false == this.pointers["active:end"].Exists(this.IsPointerFromTouchscreenInputSystem) :
      this.IsPointerFromMouseInput            (information) ? false == this.pointers["active:end"].Exists(this.IsPointerFromMouseInputSystem)       :
      true
    ) {
      foreach (UnityEngine.EventSystems.RaycastResult uiRaycast in uiRaycasts) {
        if (!this.pointers["active:end"].Exists(pointerInformation => information.id == pointerInformation.id && pointerInformation.gameObject == uiRaycast.gameObject))
        this.pointers["active:end"].Add(new() {device = information.device, gameObject = uiRaycast.gameObject, id = information.id, position = information.value, timestamp = uiTimestamp});
      }

      foreach (System.Collections.Generic.List<UI.PointerComponentInfo> pointerInformationList in new[] {this.pointers["active"], this.pointers["active:begin"]})
      foreach (UI.PointerComponentInfo pointerInformation in pointerInformationList) {
        if (information.id == pointerInformation.id && !this.pointers["active:end"].Exists(pointerInformation => information.id == pointerInformation.id)) {
          this.pointers["active:end"].Add(new() {device = information.device, id = information.id, position = information.value, timestamp = uiTimestamp});
          break;
        }
      }
    }
  }

  private void OnPointerMove(UI.PointerPositionInfo information) {
    UI.PointerComponentInfo?                                                pointerInformation = this.pointers["hover"].Find(pointerInformation => information.id == pointerInformation.id);
    UnityEngine.EventSystems.EventSystem?                                   uiEventSystem      = this.GetComponent<UnityEngine.EventSystems.EventSystem>();
    System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult> uiRaycasts         = new();
    float                                                                   uiTimestamp        = UnityEngine.Time.realtimeSinceStartup;

    // …
    if (null != pointerInformation) {
      pointerInformation.device   = information.device;
      pointerInformation.position = information.value;
    }

    // …
    if (null != uiEventSystem)
    this.GetComponent<UnityEngine.UI.GraphicRaycaster>()?.Raycast(new(uiEventSystem) {position = information.value}, uiRaycasts);

    if (
      // → Avoid `UnityEngine.Input` and `UnityEngine.InputSystem` colliding
      this.IsPointerFromTouchscreenInputSystem(information) ? false == this.pointers["hover:begin"].Exists(this.IsPointerFromTouchscreenInput)       :
      this.IsPointerFromMouseInputSystem      (information) ? false == this.pointers["hover:begin"].Exists(this.IsPointerFromMouseInput)             :
      this.IsPointerFromTouchscreenInput      (information) ? false == this.pointers["hover:begin"].Exists(this.IsPointerFromTouchscreenInputSystem) :
      this.IsPointerFromMouseInput            (information) ? false == this.pointers["hover:begin"].Exists(this.IsPointerFromMouseInputSystem)       :
      true
    ) foreach (UnityEngine.EventSystems.RaycastResult uiRaycast in uiRaycasts) {
      if (!this.pointers["hover:begin"].Exists(pointerInformation => information.id == pointerInformation.id && pointerInformation.gameObject == uiRaycast.gameObject))
      this.pointers["hover:begin"].Add(new() {device = information.device, gameObject = uiRaycast.gameObject, id = information.id, position = information.value, timestamp = uiTimestamp});
    }

    this.pointers["hover:begin"].ForEach(pointerInformation => {
      if (information.id == pointerInformation.id && !uiRaycasts.Exists(uiRaycast => pointerInformation.gameObject == uiRaycast.gameObject)) {
        this.pointers["hover:end"].Add(new() {device = information.device, gameObject = pointerInformation.gameObject, id = information.id, position = information.value, timestamp = uiTimestamp});
        return;
      }
    });
  }

  public void OnPrompt(UnityEngine.InputSystem.InputAction.CallbackContext context) {
    this.prompted = true;
  }

  private void OnTabBlur() {
    this.componentTabActive              = false;
    this.componentTabIndex               = -1;
    this.componentTabInitialDelayElapsed = 0.0f;
    this.componentTabList.Clear();
  }

  private void OnTabFocus() {
    if (0 != this.componentTabList.Count) {
      bool shifted = false;
      bool tabbed  = false;

      // …
      foreach (UI.KeyboardInfo keyboardInformation in this.keyboards["active"])
      switch (keyboardInformation.key) {
        case UnityEngine.KeyCode.Escape:                                         this.OnTabBlur(); return;
        case UnityEngine.KeyCode.LeftShift: case UnityEngine.KeyCode.RightShift: shifted = true;  break;
        case UnityEngine.KeyCode.Tab:                                            tabbed  = true;  break;
      }

      if (tabbed) {
        if (0.0f == this.componentTabInitialDelayElapsed || COMPONENT_TAB_INITIAL_DELAY < this.componentTabInitialDelayElapsed) {
          this.componentTabActive = true;
          this.componentTabIndex  = (shifted && this.componentTabIndex == -1 ? this.componentTabList.Count : this.componentTabIndex) + (!shifted ? +1 : -1);
          this.componentTabIndex  = (this.componentTabIndex == -1 ? this.componentTabList.Count - 1 : this.componentTabIndex) % this.componentTabList.Count;
        }

        this.componentTabInitialDelayElapsed += UnityEngine.Time.deltaTime + 1.0e-3f;
      } else this.componentTabInitialDelayElapsed = 0.0f;

      return;
    }

    this.OnTabBlur();
  }

  public void ShowText(string text, float duration = 0.0f) {
    if (this.immediateText?.activeSelf ?? true)
    return;

    this.immediateText.SetActive(true);

    foreach (UnityEngine.UI.RawImage image in this.immediateText.FindHierarchyByComponent<UnityEngine.UI.RawImage>()) {
      image.CrossFadeAlpha(IMMEDIATE_TEXT_BACKGROUND_TRANSPARENCY, duration, true);
      image.color = Util.IIFE<UnityEngine.Color>(color => color.a = IMMEDIATE_TEXT_BACKGROUND_TRANSPARENCY)(image.color);
    }

    foreach (TMPro.TextMeshProUGUI textMesh in this.immediateText.FindHierarchyByComponent<TMPro.TextMeshProUGUI>()) {
      textMesh.alpha = 1.0f;
      textMesh.text  = text;
    }
  }

  private void Start() {
    #pragma warning disable CS0162
      if (false)
      return;
    #pragma warning restore CS0162
    #if false
      // TODO (Lapys) → Test `ComponentLoad` animations
      UnityEngine.RectTransform? splashTransform = this.components["splash"]?.transform as UnityEngine.RectTransform;

      // …
      if (null != splashTransform) {
        float size = Util.Max(splashTransform.rect.height, splashTransform.rect.width);
        splashTransform.SetSize(new(size, size));
      }

      if (null != this.background)
        this.LoadTexture("menu-background.png", default, uiTexture => { if (null != this.background) this.background.texture = uiTexture; });

      if (null == this.immediateText) {
        UnityEngine.GameObject    uiImmediateTextImageGameObject = new("[Debug]", typeof(UnityEngine.RectTransform), typeof(UnityEngine.UI.RawImage));
        UnityEngine.GameObject    uiImmediateTextGameObject      = new("[Text]",  typeof(UnityEngine.RectTransform), typeof(TMPro.TextMeshProUGUI));
        UnityEngine.UI.RawImage   uiImmediateTextImage           = uiImmediateTextImageGameObject.GetComponent<UnityEngine.UI.RawImage>();
        UnityEngine.RectTransform uiImmediateTextImageTransform  = uiImmediateTextImageGameObject.transform as UnityEngine.RectTransform;
        UnityEngine.RectTransform uiImmediateTextTransform       = uiImmediateTextGameObject     .transform as UnityEngine.RectTransform;
        TMPro.TextMeshProUGUI     uiImmediateText                = uiImmediateTextGameObject     .GetComponent<TMPro.TextMeshProUGUI>();
        UnityEngine.Rect?         uiRectangle                    = Util.WorldRectFromRectTransform(this.transform as UnityEngine.RectTransform);
        float                     uiImmediateTextHeight          = System.Math.Min(uiRectangle?.height ?? UnityEngine.Screen.height, uiRectangle?.width ?? UnityEngine.Screen.width) * Util.Percent(10.0f);

        // …
        uiImmediateTextTransform     .SetParent(uiImmediateTextImageTransform, true);
        uiImmediateTextImageTransform.SetParent(this.transform,                true);
        uiImmediateTextImageTransform.SetHeight(uiImmediateTextHeight);

        this                         .immediateText    = uiImmediateTextImageGameObject;
        uiImmediateText              .alignment        = TMPro.TextAlignmentOptions.CenterGeoAligned;
        uiImmediateText              .color            = UnityEngine.Color.white;
        uiImmediateText              .enableAutoSizing = true;
        uiImmediateText              .fontSize         = uiImmediateTextHeight * Util.Percent(90.0f);
        uiImmediateText              .margin           = UnityEngine.Vector4.zero;
        uiImmediateText              .richText         = true;
        uiImmediateText              .text             = "";
        uiImmediateTextImage         .color            = Util.IIFE<UnityEngine.Color>(color => color.a = IMMEDIATE_TEXT_BACKGROUND_TRANSPARENCY)(UnityEngine.Color.black);
        uiImmediateTextImage         .raycastTarget    = false;
        uiImmediateTextImageTransform.localPosition    = UnityEngine.Vector3.zero;
        uiImmediateTextImageTransform.localScale       = UnityEngine.Vector3.one;
        uiImmediateTextImageTransform.anchorMax        = new(1.0f, 0.5f);
        uiImmediateTextImageTransform.anchorMin        = new(0.0f, 0.5f);
        uiImmediateTextImageTransform.offsetMax        = new(0.0f, uiImmediateTextImageTransform.offsetMax.y);
        uiImmediateTextImageTransform.offsetMin        = new(0.0f, uiImmediateTextImageTransform.offsetMin.y);
        uiImmediateTextTransform     .anchorMax        = new(1.0f, 0.5f);
        uiImmediateTextTransform     .anchorMin        = new(0.0f, 0.5f);
        uiImmediateTextTransform     .offsetMax        = new(0.0f, uiImmediateTextTransform.offsetMax.y);
        uiImmediateTextTransform     .offsetMin        = new(0.0f, uiImmediateTextTransform.offsetMin.y);

        this.HideText();
      }

      // → Sequence menus
      /* VOLUME DB IS EXPONENTIAL, NOT LINEAR */
      /* CACHE `Camera.main` */
      /* USE `Object.ReferenceEquals(…)` for non user scripts */
      /* Cache properties like `transform`, fuck man, Unity! */
      /* System.ReadOnlySpan<UnmanagedT> bruh = stackalloc UnmanagedT[begin.Count]; like `int` */
      /* FLOAT * FLOAT * VECTOR */
      /* OBJECT POOL FOR BULLETS: INSTANTIATE OBJECTS THEN RE-CYCLE THEM.. IT'S FUCKING CUSTOM */
      /* `interface` FOR BULLET INHERITANCE AND SIMILAR; See `NPC` class */
      /* Use `in` for const references and readonly structs (or struct properties) only */
      /* `Transform.SetPositionAndRotation(…)` */
      /* Prefer local `stackalloc Span<T>` to `T[]` */
      /* Consider `[System.Serializable] public class MyArgs { public int arg1; public string arg2; public bool arg3; } myArgs;` */
      /* `checked` and `unchecked` for integer overflow */
      this.UnloadAllComponents(UI.LoadAction.Immediately);
      this.LoadComponent      ("splash", UI.LoadAction.Immediately);
      this.LoadMusic          ("calm.mp3");

      Util.WaitUntil(1.5, () => {
        this.components["splash"]?.GetComponent<UnityEngine.UI.RawImage>()?.CrossFadeAlpha(0.0f, BACKGROUND_UNLOAD_DURATION, true); // → Thank goodness this method exists; T_T
        System.Array.ForEach(this.components["splash"]?.FindDescendantsByComponent<TMPro.TextMeshProUGUI>(), _ => _.alpha = 0.0f);

        this.LoadComponent  ("menu", UI.LoadAction.Immediately);
        Util.WaitUntil(BACKGROUND_UNLOAD_DURATION, () => this.UnloadComponent("splash", UI.LoadAction.Immediately));
      });

      // → Acknowledge application events
      UnityEngine.Application.focusChanged += focus => {
        if (!focus) {
          this.EndEventData(this.keyboards);
          this.EndEventData(this.pointers);
          this.OnTabBlur   ();

          Game.main?.Pause();
        }
      };
    #endif
  }

  public void UnloadAllComponents(UI.LoadAction action = UI.LoadAction.Deferred) {
    foreach (string name in this.components.Keys)
    this.UnloadComponent(name, action);
  }

  public void UnloadBackground(float unloadDuration = LOAD_ANIMATION_DURATION, UI.LoadAction action = default) {
    if (null != this.background) {
      this.background.CrossFadeAlpha(0.0f, unloadDuration, true);
      Util.WaitUntil(unloadDuration, () => this.background.texture = null!);
    }
  }

  public void UnloadComponent(string name, UI.LoadAction action = UI.LoadAction.Deferred) {
    this.componentLoads.RemoveAll(static _ => null == _.gameObject);

    if (this.components.TryGetValue(name, out UnityEngine.GameObject gameObject))
    if (null != gameObject && gameObject.transform is UnityEngine.RectTransform) {
      UnityEngine.RectTransform transform     = (gameObject.transform as UnityEngine.RectTransform)!;
      UnityEngine.Vector3       position      = transform.position;
      UI.ComponentLoadInfo      componentLoad = this.componentLoads.Find(_ => gameObject == _.gameObject) ?? this.componentLoads.Append(new());

      // …
      componentLoad.loading    = false;
      componentLoad.gameObject = gameObject;
      componentLoad.animation  = new("unload", 0x0u != (action & UI.LoadAction.Immediately) ? 0.0 : 0.4, PatchOdyssey.Animation.Function.EaseInOutQuintic,
        new() {{"position", position}},
        new() {{"position", new UnityEngine.Vector3(position.x, (float) Util.WorldRectFromRectTransform(this.transform as UnityEngine.RectTransform)?.yMin! - (float) Util.WorldBoundsFromRectTransform(transform)?.extents.y!, position.z)}}
      );
    }
  }

  private void UnloadEventData<T>(System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<T>> informationList, UI.LoadAction action = UI.LoadAction.Deferred) where T : UI.EventDataInfo {
    foreach (string key in informationList.Keys) {
      if (key.Contains(":end"))
      informationList[key].Clear();
    }
  }

  private void Update() {
    #pragma warning disable CS0162
      if (false)
      return;
    #pragma warning restore CS0162

    (T, UnityEngine.Vector3 eulerAngles, UnityEngine.Vector3 localScale) teeterAnimationStart = (default(T), UnityEngine.Vector3.zero, UnityEngine.Vector3.one);
    System.Collections.Generic.List<UnityEngine.GameObject>              uiAnimatable         = new((int) this.gameObject.CountDescendants());
    System.Action<UnityEngine.AudioSource?>                              uiAsSoundEffect      = static uiAudio => { if (null != uiAudio) uiAudio.spatialBlend = 0.0f; };
    UnityEngine.InputSystem.PlayerInput?                                 uiInput              = this.GetComponent<UnityEngine.InputSystem.PlayerInput>();
    System.Collections.Generic.List<UnityEngine.GameObject>              uiDeanimatable       = new((int) this.gameObject.CountDescendants());
    float                                                                uiTimestamp          = UnityEngine.Time.realtimeSinceStartup;

    // → Acknowledge keyboard events
    foreach (UnityEngine.KeyCode keyCode in Util.GetKeys()) {
      if (UnityEngine.Input.GetKey    (keyCode)) this.OnKey     (new() {key = keyCode, timestamp = uiTimestamp});
      if (UnityEngine.Input.GetKeyDown(keyCode)) this.OnKeyBegin(new() {key = keyCode, timestamp = uiTimestamp});
      if (UnityEngine.Input.GetKeyUp  (keyCode)) this.OnKeyEnd  (new() {key = keyCode, timestamp = uiTimestamp});
    }

    foreach (UI.KeyboardInfo keyboardInformation in this.inputs.keyboards.active) {
      this.OnKeyBegin(new() {device = keyboardInformation.device, id = keyboardInformation.id, key = keyboardInformation.key, timestamp = uiTimestamp});
      this.OnKey     (new() {device = keyboardInformation.device, id = keyboardInformation.id, key = keyboardInformation.key, timestamp = uiTimestamp});
      this.OnKeyEnd  (new() {device = keyboardInformation.device, id = keyboardInformation.id, key = keyboardInformation.key, timestamp = uiTimestamp});
    }

    this.prompted = this.prompted || this.keyboards["active:end"].Exists(_ => UnityEngine.KeyCode.KeypadEnter == _.key || UnityEngine.KeyCode.Return == _.key);

    // → Acknowledge pointer events
    this.pointers["active:begin"].Clear();

    if (null == uiInput?.actions) {
      string[]            mouseIDs      = {"LEFT", "RIGHT", "MIDDLE"};
      UnityEngine.Vector2 mousePosition = UnityEngine.Input.mousePosition;

      // …
      this.OnPointerMove(new() {timestamp = uiTimestamp, value = mousePosition});

      foreach (int button in Util.GetMouseButtons()) {
        if (UnityEngine.Input.GetMouseButton    (button)) this.OnPointer     (new() {id = mouseIDs[button], timestamp = uiTimestamp, value = mousePosition});
        if (UnityEngine.Input.GetMouseButtonDown(button)) this.OnPointerBegin(new() {id = mouseIDs[button], timestamp = uiTimestamp, value = mousePosition});
        if (UnityEngine.Input.GetMouseButtonUp  (button)) this.OnPointerEnd  (new() {id = mouseIDs[button], timestamp = uiTimestamp, value = mousePosition});
      }

      for (int index = UnityEngine.Input.touchCount; 0 != index--; ) {
        UnityEngine.Touch touch = UnityEngine.Input.GetTouch(index);

        switch (touch.phase) {
          case UnityEngine.TouchPhase.Began:                                            this.OnPointerBegin(new() {id = touch.fingerId.ToString(), timestamp = uiTimestamp, value = touch.position}); break;
          case UnityEngine.TouchPhase.Canceled: case UnityEngine.TouchPhase.Ended:      this.OnPointerEnd  (new() {id = touch.fingerId.ToString(), timestamp = uiTimestamp, value = touch.position}); break;
          case UnityEngine.TouchPhase.Moved:    case UnityEngine.TouchPhase.Stationary: this.OnPointer     (new() {id = touch.fingerId.ToString(), timestamp = uiTimestamp, value = touch.position}); break;
        }
      }
    }

    foreach (UI.PointerActivationInfo pointerInformation in this.inputs.pointers.active)
    this.OnInputPointer(pointerInformation, this.OnPointer);

    // → Update events
    this.LoadEventData(this.keyboards);
    this.LoadEventData(this.pointers);

    if (Game.main?.isPlaying ?? false) {
      // → Gameplay events
      if (null == uiInput?.actions) {
        bool                move     = 0 != this.keyboards["active:end"].Count;
        UnityEngine.Vector3 movement = UnityEngine.Vector3.zero;

        // …
        if (this.keyboards["active:end"].Exists(_ => UnityEngine.KeyCode.Escape  == _.key))
        Game.main.Pause();

        if (this.keyboards["active"].Exists(_ => UnityEngine.KeyCode.DownArrow  == _.key || UnityEngine.KeyCode.S == _.key)) { move = true; movement += UnityEngine.Vector3.back; }
        if (this.keyboards["active"].Exists(_ => UnityEngine.KeyCode.LeftArrow  == _.key || UnityEngine.KeyCode.A == _.key)) { move = true; movement += UnityEngine.Vector3.left; }
        if (this.keyboards["active"].Exists(_ => UnityEngine.KeyCode.RightArrow == _.key || UnityEngine.KeyCode.D == _.key)) { move = true; movement += UnityEngine.Vector3.right; }
        if (this.keyboards["active"].Exists(_ => UnityEngine.KeyCode.UpArrow    == _.key || UnityEngine.KeyCode.W == _.key)) { move = true; movement += UnityEngine.Vector3.forward; }

        if (move)
        Game.main.player?.MoveBy(movement);
      }
    }

    else {
      // → Navigate UI components
      this.componentTabList.Clear();

      foreach ((UnityEngine.GameObject uiComponent, UnityEngine.GameObject[] uiElements) in new[] {
        (this.components["credits"], System.Array.ConvertAll(Util.ArrayFrom(this.components["credits"]?.FindDescendantsByComponent<UnityEngine.UI.Button>()!), static _ => _.gameObject)),
        (this.components["menu"],    System.Array.ConvertAll(Util.ArrayFrom(this.components["menu"]   ?.FindDescendantsByComponent<UnityEngine.UI.Button>()!), static _ => _.gameObject)),
        (this.components["pause"],   System.Array.ConvertAll(Util.ArrayFrom(this.components["pause"]  ?.FindDescendantsByComponent<UnityEngine.UI.Button>()!), static _ => _.gameObject))
      }) {
        this.componentTabList.AddRange(uiComponent?.activeSelf ?? false ? uiElements : new UnityEngine.GameObject[0]);
        uiDeanimatable       .AddRange(uiElements);
      }

      this.OnTabFocus();
      uiAnimatable.AddRange(this.pointers["hover"].ConvertAll(_ => _.gameObject).FindAll(this.componentTabList.Contains)); // → All "tabbable" UI components can be animated

      this.componentTabIndex = 0 != uiAnimatable.Count ? this.componentTabList.IndexOf(uiAnimatable[0]) : this.componentTabIndex; // → Tab index affected by UI pointer selection
      if (this.componentTabIndex != -1 && null != this.componentTabList) {
        this.componentTabActive = this.componentTabActive && !this.pointers["hover:end"].Exists(_ => _.gameObject == this.componentTabList[this.componentTabIndex]);

        if (this.componentTabActive && !uiAnimatable.Contains(this.componentTabList[this.componentTabIndex]))
        uiAnimatable.Add(this.componentTabList[this.componentTabIndex]);
      }

      uiDeanimatable.RemoveAll(uiAnimatable.Contains); // → Do not animate UI components not meant to be animated

      // → Animate UI components
      foreach (UnityEngine.GameObject uiElement in uiDeanimatable) {
        uiElement.gameObject.transform.eulerAngles = teeterAnimationStart.eulerAngles;
        uiElement.gameObject.transform.localScale  = teeterAnimationStart.localScale;

        if (this.components["menu"]?.HasDescendant(uiElement) ?? false)
        this.LoadTexture("menu-button-1.png", default, menuTexture => { UnityEngine.UI.RawImage? menuImage = uiElement.GetComponent<UnityEngine.UI.RawImage>(); if (null != menuImage) menuImage.texture = menuTexture!; });
      }

      foreach (UnityEngine.GameObject uiElement in uiAnimatable) {
        const uint animationKeyframes   = 4u;
        float      animationProgress    = (this.animationDurationElapsed - (TEETER_ANIMATION_DURATION * (int) (this.animationDurationElapsed / TEETER_ANIMATION_DURATION))) / TEETER_ANIMATION_DURATION;
        float      animationSubprogress = animationKeyframes * (animationProgress % (1.0f / animationKeyframes));
        var        animationStart       = teeterAnimationStart;
        uint       animationFrameIndex  = (uint) (animationProgress / (1.0f / animationKeyframes));
        var        animationEnd         = (default(T), eulerAngles: UnityEngine.Vector3.forward * (animationFrameIndex < animationKeyframes / 2u ? +5.0f : -5.0f), localScale: animationStart.localScale * 1.1f);
        var        animationDelta       = (default(T), eulerAngles: animationEnd.eulerAngles - animationStart.eulerAngles,                                         localScale: animationEnd.localScale - animationStart.localScale);

        // …
        if (animationFrameIndex % 2 == 1) {
          (animationDelta.eulerAngles, animationDelta.localScale) = (-animationDelta.eulerAngles, -animationDelta.localScale);

          (animationEnd.eulerAngles, animationStart.eulerAngles) = (animationStart.eulerAngles, animationEnd.eulerAngles);
          (animationEnd.localScale,  animationStart.localScale)  = (animationStart.localScale,  animationEnd.localScale);
        }

        uiElement.gameObject.transform.eulerAngles = animationStart.eulerAngles + (animationDelta.eulerAngles * (float) TEETER_ANIMATION_FUNCTION(animationSubprogress));
        uiElement.gameObject.transform.localScale  = animationStart.localScale  + (animationDelta.localScale  * (float) TEETER_ANIMATION_FUNCTION(animationSubprogress));

        if (this.components["menu"]?.HasDescendant(uiElement) ?? false)
        this.LoadTexture($"menu-button-{(COMPONENT_TAGS.MENU_QUIT == uiElement.tag ? 4u : 3u)}.png", default, menuTexture => { UnityEngine.UI.RawImage? menuImage = uiElement.GetComponent<UnityEngine.UI.RawImage>(); if (null != menuImage) menuImage.texture = menuTexture!; });
      }

      // → Sequence UI components
      if (0 != this.keyboards["active:end"].Count)
      this.LoadSoundEffect("click.mp3", UI.LoadAction.Deferred, uiAsSoundEffect);

      foreach (UnityEngine.UI.Button creditsButton in Util.ArrayFrom(this.components["credits"]?.FindDescendantsByComponent<UnityEngine.UI.Button>()!))
      if (this.IsSelected(creditsButton)) {
        this.LoadSoundEffect("click.mp3", UI.LoadAction.Immediately, uiAsSoundEffect);
        if (COMPONENT_TAGS.BACK == creditsButton.tag) {
          this.LoadComponent  ("menu");
          this.UnloadComponent("credits");
        }
      }

      foreach (UnityEngine.UI.Button menuButton in Util.ArrayFrom(this.components["menu"]?.FindDescendantsByComponent<UnityEngine.UI.Button>()!))
      if (this.IsSelected(menuButton)) {
        this.LoadSoundEffect("click.mp3", UI.LoadAction.Immediately, uiAsSoundEffect);
        (Util.Switch(menuButton.tag, new() {
          {COMPONENT_TAGS.MENU_QUIT,  (System.Action) (() => Game.main?.Exit())},
          {COMPONENT_TAGS.MENU_START, (System.Action) (() => Game.main?.Play())},

          {COMPONENT_TAGS.MENU_CREDITS, (System.Action) (() => {
            this.LoadComponent  ("credits");
            this.UnloadComponent("menu");
          })},

          {COMPONENT_TAGS.MENU_OPTIONS, (System.Action) (() => {
            // TODO (Lapys) → Do something…
          })}
        }) as System.Action)?.DynamicInvoke();
      }

      foreach (UnityEngine.UI.Button pauseButton in Util.ArrayFrom(this.components["pause"]?.FindDescendantsByComponent<UnityEngine.UI.Button>()!))
      if (this.IsSelected(pauseButton)) {
        this.LoadSoundEffect("click.mp3", UI.LoadAction.Immediately, uiAsSoundEffect);
        (Util.Switch(pauseButton.tag, new() {
          {COMPONENT_TAGS.BACK, (System.Action) (() => Game.main?.Play  ())},
          {COMPONENT_TAGS.MENU, (System.Action) (() => Game.main?.ToMenu())},
        }) as System.Action)?.DynamicInvoke();
      }

      if (this.keyboards["active:end"].Exists(_ => UnityEngine.KeyCode.Escape  == _.key))
      Game.main?.Play();
    }

    // → Transition UI components
    if (null != this.background) {
      UnityEngine.Vector2 pointerPosition = this.pointers["hover"].Find(static _ => true)?.position ?? UnityEngine.Input.mousePosition;

      // → Background responds to the on-screen pointer position
      pointerPosition = 0.0f != pointerPosition.x * 0.0f || 0.0f != pointerPosition.y * 0.0f ? UnityEngine.Vector2.zero : pointerPosition;
      this.background.transform.localPosition = pointerPosition / BACKGROUND_RESPONSIVENESS;
    }

    for (int index = this.componentLoads.Count; 0 != index; ) {
      UI.ComponentLoadInfo componentLoad = this.componentLoads[--index];

      // …
      if (componentLoad.animation.isDone || null == componentLoad.gameObject) {
        this.componentLoads.RemoveAt(index);
        continue;
      }

      componentLoad.gameObject.transform.position = (UnityEngine.Vector3) componentLoad.animation["position"]!;
      componentLoad.gameObject.SetActive(componentLoad.loading || !componentLoad.animation.isDone);
    }

    // …
    Util.StopWaiting    ();
    this.UnloadEventData(this.keyboards);
    this.UnloadEventData(this.pointers);

    this.animationDurationElapsed += UnityEngine.Time.deltaTime;
    this.prompted                  = false;

    UI.maximumDeltaTime = UnityEngine.Time.deltaTime >= UI.maximumDeltaTime ? UnityEngine.Time.deltaTime : UI.maximumDeltaTime;
    UI.minimumDeltaTime = UnityEngine.Time.deltaTime <= UI.minimumDeltaTime ? UnityEngine.Time.deltaTime : UI.minimumDeltaTime;
    UI.totalDeltaTime  += UnityEngine.Time.deltaTime;
    UI.updateCount     += 1uL;
  }
}
