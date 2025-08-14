using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

/* … */
#if !(NET5_0 || NET5_0_OR_GREATER)
  namespace System.Runtime.CompilerServices {
    // ->> `init` @ `https://web.archive.org/web/20220918192058/https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/init`
    internal static class IsExternalInit {}
  }
#endif

namespace PatchOdyssey {
  [UnityEngine.InputSystem.Layouts.InputControlLayout(displayName = "PatchKeyboard")] // “cOuLd nOt rEcReAtE DeViCe 'DuMmYkEyBoArD' wItH LaYoUt 'DuMmYkEyBoArD' aFtEr dOmAiN ReLoAd”
  public sealed class DummyKeyboard : UnityEngine.InputSystem.Keyboard /* , UnityEngine.InputSystem.LowLevel.ITextInputReceiver */ {
    protected override void FinishSetup() => base.FinishSetup();
  }

  public static class Game {
    public  static          bool                                                                                                                  _IsPaused                                  = false;
    private static          UnityEngine.InputSystem.Keyboard?                                                                                     _Keyboard                                  = null;
    public  static          bool                                                                                                                  IsFocused           { get; internal set; } = false;
    public  static          bool                                                                                                                  IsKeyboardAvailable { get; internal set; } = false;
    public  static          bool                                                                                                                  IsLoaded            { get; internal set; } = false;
    public  static          bool                                                                                                                  IsPaused            { get => Game._IsPaused || Game.IsFocused; set => Game._IsPaused = value; }
    public  static          bool                                                                                                                  IsQuitting          { get; internal set; } = false;
    public  static          UnityEngine.InputSystem.Keyboard                                                                                      Keyboard            { get { if (!Game.IsKeyboardAvailable && UnityEngine.InputSystem.Keyboard.current is UnityEngine.InputSystem.Keyboard keyboard) { Game._Keyboard = keyboard; Game.IsKeyboardAvailable = true; } return Game._Keyboard!; } }
    public  const           byte                                                                                                                  NumberPrecision      = (byte) 5u; // ->> Maximum precision supported by Unity (or `float`s); Should be about 6.0–7.2 digits
    private static readonly double                                                                                                                NumberPrecisionScale = System.Math.Pow(10.0, (double) Game.NumberPrecision);
    private static          UnityEngine.GameObject?                                                                                               Object               = null;
    public  static readonly System.Random                                                                                                         Randomizer           = new();  // ->> Superseded (mostly) by `UnityEngine.Random`
    public  const           float                                                                                                                 SceneSize            = 100.0f; // ->> Meant for sizing objects that bound the entire visible part of the scene
    public  static readonly string                                                                                                                SpecialTag           = "…";
    private static readonly System.Collections.Generic.Dictionary<System.Action<UnityEngine.Transform>, System.Func<UnityEngine.Transform, bool>> TransformTraversers  = new();
    public  const           float                                                                                                                 VectorEpsilon        = 0.09f; // ->> Minimal amount to prevent Z-fighting and other false positives

    // …
    public static bool AskToSave() => Game.AskToSave(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    public static bool AskToSave(in UnityEngine.SceneManagement.Scene scene) {
      // ... ->> Undo’s not bothered with for the time begin
      #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        return UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
      #endif

      return false;
    }

    public static void Encapsulate(this UnityEngine.Bounds bounds, in UnityEngine.Bounds encapsulated) {
      bounds.Encapsulate(encapsulated.center - encapsulated.extents);
      bounds.Encapsulate(encapsulated.center + encapsulated.extents);
    }

    public static void ForEach   (this UnityEngine.Transform transform, System.Action   <UnityEngine.Transform> iterator)                                 => transform.ForEach<UnityEngine.Transform>(transform => { iterator(transform); return true; });
    public static void ForEach   (this UnityEngine.Transform transform, System.Predicate<UnityEngine.Transform> iterator)                                 => transform.ForEach<UnityEngine.Transform>(iterator);
    public static void ForEach<T>(this UnityEngine.Transform transform, System.Action   <T>                     iterator) where T : UnityEngine.Component => transform.ForEach<T>                    (transform => { iterator(transform); return true; });
    public static void ForEach<T>(this UnityEngine.Transform transform, System.Predicate<T>                     iterator) where T : UnityEngine.Component {
      if (transform is null)
      return;

      // …
      System.Collections.Generic.Queue<UnityEngine.Transform> transforms = new(transform.hierarchyCount);

      for (transforms.Enqueue(transform); 0 != transforms.Count; )
      for (System.Collections.IEnumerator enumerator = transforms.Dequeue().GetEnumerator(); ; ) {
        if (enumerator.MoveNext()) {
          UnityEngine.Transform subtransform = (UnityEngine.Transform) enumerator.Current;

          // …
          if (!subtransform.TryGetComponent(out T component) || iterator(component))
          transforms.Enqueue(subtransform);
        } else break; // --> (enumerator as System.IDisposable)?.Dispose()
      }
    }

    public static void GetSize(this UnityEngine.RectTransform rectTransform, out UnityEngine.Vector2 size, UnityEngine.Canvas canvas = null!) {
      for (UnityEngine.Transform transform = (UnityEngine.Transform) rectTransform; transform is not null; transform = transform.parent)
      if (canvas is not null || transform.TryGetComponent(out canvas)) {
        size = UnityEngine.RectTransformUtility.PixelAdjustRect(rectTransform, canvas).size;
        return;
      }

      // …
      size = rectTransform.sizeDelta;

      for (UnityEngine.Transform transform = (UnityEngine.Transform) rectTransform; transform is not null; transform = transform.parent) {
        if (transform.TryGetComponent(out UnityEngine.UI.CanvasScaler scaler))
        size *= scaler.scaleFactor;
      }
    }

    public static float Normalize(float value) => (float) (System.Math.Truncate(Game.NumberPrecisionScale * (double) value) / Game.NumberPrecisionScale); // --> (float) System.Math.Round((double) value, Game.NumberPrecision, System.MidpointRounding.ToZero)

    public static UnityEngine.Color Opacity(in this UnityEngine.Color color, float opacity) => new(color.r, color.g, color.b, opacity);
    public static UnityEngine.Color Opaque (in this UnityEngine.Color color)                => color.Opacity(1.0f);

    [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Main() {
      // … ->> Ensure `Game.Keyboard` is non-null
      if (Game.Keyboard is null) {
        if (!new System.Collections.Generic.List<string>(InputSystem.ListLayouts()).Contains("PatchKeyboard"))
        InputSystem.RegisterLayout<DummyKeyboard>("PatchKeyboard", null as UnityEngine.InputSystem.Layouts.InputDeviceMatcher?);

        if (InputSystem.GetDevice<DummyKeyboard>() is null) {
          try { Game._Keyboard = InputSystem.AddDevice<DummyKeyboard>("PatchKeyboard"); }
          catch (System.InvalidOperationException) {
            #if DEBUG || DEVELOPMENT_BUILD
              UnityEngine.Debug.LogWarning("Missing `UnityEngine.InputSystem.Keyboard` component for `Game.Keyboard`");
            #endif
          }
        }
      }

      #if DEBUG || DEVELOPMENT_BUILD
        if (null == Assets.main)
        UnityEngine.Debug.LogError($"Missing `Assets` component for `Assets.main`");

        if (null == UI.main)
        UnityEngine.Debug.LogError("Missing `UI` component for `UI.main`");
      #endif

      // …
      #if UNITY_EDITOR
        UnityEditor.EditorApplication.focusChanged += static (bool focused) => Game.IsFocused  = !focused; // --> !UnityEditor.EditorApplication.isFocused;
        UnityEditor.EditorApplication.quitting     += static ()             => Game.IsQuitting =  true;
        UnityEditor.EditorApplication.wantsToQuit  += static ()             => { UnityEngine.Object.Destroy(Game.Object); return true; };

        #if false
          UnityEditor.EditorApplication.pauseStateChanged += static (UnityEditor.PauseState state) => Game.IsPaused = state switch {
            UnityEditor.PauseState.Paused   => true,
            UnityEditor.PauseState.Unpaused => false,
            _                               => UnityEditor.EditorApplication.isPaused /* --> !UnityEditor.EditorApplication.isPlaying */
          };
        #endif
      #endif

      Game.Object          = new UnityEngine.GameObject("🎮", typeof(GameBehaviour));
      Game.Object.isStatic = true;
    }

    public static void Quit(int code = 0x0) /* --> EXIT_SUCCESS */ {
      if (Game.IsQuitting)
      return;

      Game.IsQuitting = true;
      UnityEngine.Application.Quit(code);
      #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        UnityEditor.EditorApplication.ExitPlaymode();
      #endif
    }

    public static UnityEngine.Color Transparent(in this UnityEngine.Color color) => color.Opacity(0.0f);
  }

  [Unity.Profiling.IgnoredByDeepProfiler]
  [UnityEngine.DefaultExecutionOrder(0)]
  internal sealed class GameBehaviour : UnityEngine.MonoBehaviour {
    private void OnApplicationFocus(bool focused) => Game.IsFocused  = !focused;
    private void OnApplicationPause(bool paused)  => Game.IsFocused  =  paused;
    private void OnApplicationQuit ()             => Game.IsQuitting =  true;
    private void OnDestroy         ()             => Game.Quit(0x1); // --> EXIT_FAILURE ->> Don’t bother continuing the application/ game if `GameBehaviour` is prematurely destroyed
    private void Start             ()             => Game.IsLoaded              = true;
    private void Update            ()             => UnityEngine.Time.timeScale = Game.IsPaused ? 0.0f : 1.0f;
  }

  [UnityEngine.DisallowMultipleComponent]
  [UnityEngine.RequireComponent(typeof(UnityEngine.AudioSource))]
  public abstract class GameComponent : UnityEngine.MonoBehaviour {
    private       UnityEngine.AudioSource              _audioSource              = null!;
    private       UnityEngine.Collider                 _collider           = null!;
    private       UnityEngine.Collider[]               _colliders          = null!;
    private       UnityEngine.Rigidbody                _rigidBody          = null!;
    private       UnityEngine.Transform                _transform          = null!;
    protected     UnityEngine.Camera?                  actualWorldCamera   = null;
    public        ref readonly UnityEngine.AudioSource audioSource         { get { if (this._audioSource is null && base.TryGetComponent(out UnityEngine.AudioSource audioSource)) { this._audioSource = audioSource; /* --> base.audio */ }    return ref this._audioSource!; } }
    public    new ref readonly UnityEngine.Collider    collider            { get { if (this._collider    is null && base.TryGetComponent(out UnityEngine.Collider    collider))    { this._collider    = collider;    /* --> base.collider */ } return ref this._collider!; } }
    public        ref readonly UnityEngine.Collider[]  colliders           { get { this._colliders ??= base.GetComponents<UnityEngine.Collider>();                                                                                  return ref this._colliders; } }
    public        ref readonly UnityEngine.Rigidbody   rigidBody           { get { if (this._rigidBody is null && base.TryGetComponent(out UnityEngine.Rigidbody rigidBody)) { this._rigidBody = rigidBody; }                       return ref this._rigidBody!; } }
    public    new ref readonly UnityEngine.Transform   transform           { get { this._transform ??= base.transform;                                                                                                              return ref this._transform; } }
    public        ref readonly UnityEngine.Camera?     worldCamera         { get { if (this.actualWorldCamera is null) { this.actualWorldCamera = UnityEngine.Camera.main; if (this.actualWorldCamera is not null) { this.worldCameraDistance = this.actualWorldCamera.transform.position - this.transform.position; } } return ref this.actualWorldCamera; } }
    protected     UnityEngine.Vector3                  worldCameraDistance = UnityEngine.Vector3.zero;

    /* … */
    protected virtual void Update() {
      if (Game.IsPaused && this.rigidBody is not null) {
        this.rigidBody.angularVelocity = UnityEngine.Vector3.zero;
        this.rigidBody.linearVelocity  = UnityEngine.Vector3.zero;
      }
    }
  }

  [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false, Inherited = false)]
  public sealed class ReadOnlyInInspectorAttribute : UnityEngine.PropertyAttribute {}

  [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false, Inherited = false)]
  public sealed class ReadWriteInInspectorAttribute : UnityEngine.PropertyAttribute {}

  [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false, Inherited = false)]
  public sealed class RenameInInspectorAttribute : UnityEngine.PropertyAttribute {
    public string name = string.Empty;
    public RenameInInspectorAttribute(string name) => this.name = name;
  }

  [System.Serializable]
  public struct Timeframe {
    public static double CurrentTimestamp => Game.IsLoaded && !Game.IsQuitting ? UnityEngine.Time.realtimeSinceStartupAsDouble : 0.0;

    [System.NonSerialized]       private readonly double                      delay         = 0.0;
    [UnityEngine.SerializeField] public           double                      duration      = 0.0;
    public                               readonly double                      easedProgress { get { double elapsed = Timeframe.CurrentTimestamp - this.timestamp; return System.Math.Min(System.Math.Round(this.delay > elapsed ? (elapsed - this.delay) / this.duration : this.duration <= elapsed - this.delay ? 1.0 : this.easing((elapsed - this.delay) / this.duration), 2, System.MidpointRounding.AwayFromZero), 1.0); } }
    public                                        System.Func<double, double> easing        =  Timeframe.Linear; // --> UnityEngine.AnimationCurve
    public                               readonly double                      elapsed       => Timeframe.CurrentTimestamp >= this.timestamp ? Timeframe.CurrentTimestamp - this.timestamp : 0.0;
    public                               readonly bool                        isElapsed     => Timeframe.CurrentTimestamp >= this.timestamp + this.duration + this.delay;
    public                                        bool                        isLooped      { get { double duration = this.delay + this.duration; if (0.0 != duration) { if (Timeframe.CurrentTimestamp > duration + this.timestamp) { this.timestamp += duration * (1uL + (ulong) ((Timeframe.CurrentTimestamp - (duration + this.timestamp)) / duration)); return true; } return false; } return true; } }
    public                                        uint                        loops         { get { double duration = this.delay + this.duration; return 0.0 != duration ? Timeframe.CurrentTimestamp > duration + this.timestamp ? (uint) ((Timeframe.CurrentTimestamp - (duration + this.timestamp)) / duration) : 0u : uint.MaxValue; } }
    public                                        double                      progress      => System.Math.Min(System.Math.Round(((Timeframe.CurrentTimestamp - this.timestamp) - this.delay) / this.duration, 2, System.MidpointRounding.AwayFromZero), 1.0);
    [System.NonSerialized]       public           double                      timestamp     =  0.0;

    /* … */
    public Timeframe(double duration = 0.0)                                       => this.duration = duration;
    public Timeframe(double duration, System.Func<double, double> easing = null!) {  this.duration = duration; this.easing = easing ?? this.easing; }

    /* … ->> See `UnityEngine.AnimationCurve` */
    public static double CubicBézier    (double time, double p0, double p1, double p2, double p3) { return (p0 * System.Math.Pow(1.0 - time, 3.0)) + (p1 * time * 3.0 * System.Math.Pow(1.0 - time, 2.0)) + (p2 * (1.0 - time) * 3.0 * System.Math.Pow(time, 2.0)) + (p3 * System.Math.Pow(time, 3.0)); }
    public static double QuadraticBézier(double time, double p0, double p1, double p2)            { return (p0 * System.Math.Pow(1.0 - time, 2.0)) + (p1 * time * 2.0 * System.Math.Pow(1.0 - time, 1.0))                                                          + (p2 * System.Math.Pow(time, 2.0)); }

    public static double Ease                (double time) { return Timeframe.CubicBézier(time, 0.25, 0.10, 0.25, 1.00); }
    public static double EaseIn              (double time) { return Timeframe.CubicBézier(time, 0.42, 0.00, 1.00, 1.00); }
    public static double EaseInBack          (double time) { return (System.Math.Pow(time, 3.0) * 2.70158) - (System.Math.Pow(time, 2.0) * 1.70158); }
    public static double EaseInBounce        (double time) { return 1.0 - Timeframe.EaseOutBounce(1.0 - time); }
    public static double EaseInCircular      (double time) { return 1.0 - System.Math.Sqrt(1.0 - System.Math.Pow(time, 2.0)); }
    public static double EaseInCubic         (double time) { return System.Math.Pow(time, 3.0); }
    public static double EaseInElastic       (double time) { return time != 0.0 && time != 1.0 ? -System.Math.Pow(2.0, (time * 10.0) - 10.0) * System.Math.Sin(((time * 10.0) - 10.75) * ((System.Math.PI * 2.0) / 3.0)) : time; }
    public static double EaseInExponential   (double time) { return time != 0.0 ? System.Math.Pow(2.0, (time * 10.0) - 10.0) : 0.0; }
    public static double EaseInOut           (double time) { return Timeframe.CubicBézier(time, 0.42, 0.00, 0.58, 1.00); }
    public static double EaseInOutBack       (double time) { return (time < 0.5 ? System.Math.Pow(time * 2.0, 2.0) * ((7.189819f * time) - 2.5949095) : ((System.Math.Pow((time * 2.0) - 2.0, 2.0) * ((((time * 2.0) - 2.0) * 3.5949095) + 2.5949095)) + 2.0)) / 2.0; }
    public static double EaseInOutBounce     (double time) { return (time < 0.5 ? (1.0 - Timeframe.EaseOutBounce(1.0 - (time * 2.0))) : (1.0 + Timeframe.EaseOutBounce((time * 2.0) - 1.0))) / 2.0; }
    public static double EaseInOutCircular   (double time) { return (time < 0.5 ? (1.0 - System.Math.Sqrt(1.0 - System.Math.Pow(time * 2.0, 2.0))) : (1.0 + System.Math.Sqrt(1.0 - System.Math.Pow((time * -2.0) + 2.0, 2.0)))) / 2.0; }
    public static double EaseInOutCubic      (double time) { return time < 0.5 ? 4.0 * System.Math.Pow(time, 3.0) : (1.0 - (System.Math.Pow((time * -2.0) + 2.0, 3.0) / 2.0)); }
    public static double EaseInOutElastic    (double time) { return time != 0.0 && time != 1.0 ? time < 0.5 ? -(System.Math.Pow(2.0, (time * 20.0) - 10.0) * System.Math.Sin(((time * 20.0) - 11.125) * ((System.Math.PI * 2.0) / 4.5))) / 2.0 : ((System.Math.Pow(2.0, (time * -20.0) + 10.0) * System.Math.Sin(((time * 20.0) - 11.125) * ((System.Math.PI * 2.0) / 4.5))) / 2.0 + 1.0) : time; }
    public static double EaseInOutExponential(double time) { return time != 0.0 && time != 1.0 ? (time < 0.5 ? System.Math.Pow(2.0, (time * 20.0) - 10.0) : (2.0 - System.Math.Pow(2.0, (time * -20.0) + 10.0))) / 2.0 : time; }
    public static double EaseInOutQuadratic  (double time) { return time < 0.5 ?  2.0 * System.Math.Pow(time, 2.0) : (1.0 - (System.Math.Pow((time * -2.0) + 2.0, 2.0) / 2.0)); }
    public static double EaseInOutQuartic    (double time) { return time < 0.5 ?  8.0 * System.Math.Pow(time, 4.0) : (1.0 - (System.Math.Pow((time * -2.0) + 2.0, 4.0) / 2.0)); }
    public static double EaseInOutQuintic    (double time) { return time < 0.5 ? 16.0 * System.Math.Pow(time, 5.0) : (1.0 - (System.Math.Pow((time * -2.0) + 2.0, 5.0) / 2.0)); }
    public static double EaseInOutSine       (double time) { return -(System.Math.Cos(System.Math.PI * time) - 1.0) / 2.0; }
    public static double EaseInQuadratic     (double time) { return System.Math.Pow(time, 2.0); }
    public static double EaseInQuartic       (double time) { return System.Math.Pow(time, 4.0); }
    public static double EaseInQuintic       (double time) { return System.Math.Pow(time, 5.0); }
    public static double EaseInSine          (double time) { return 1.0 - System.Math.Cos((System.Math.PI * time) / 2.0); }
    public static double EaseOut             (double time) { return Timeframe.CubicBézier(time, 0.00, 0.00, 0.58, 1.00); }
    public static double EaseOutBack         (double time) { return 1.0 + (2.70158 * System.Math.Pow(time - 1.0, 3.0)) + (System.Math.Pow(time - 1.0, 2.0) * 1.70158); }
    public static double EaseOutBounce       (double time) { return time < 1.0 / 2.75 ? System.Math.Pow(time, 2.0) * 7.5625 : time < 2.0 / 2.75 ? (System.Math.Pow(time - (1.5 / 2.75), 2.0) * 7.5625) + 0.75 : time < 2.5 / 2.75 ? (System.Math.Pow(time - (2.25 / 2.75), 2.0) * 7.5625) + 0.9375 : (System.Math.Pow(time - (2.625 / 2.75), 2.0) * 7.5625) + 0.984375; }
    public static double EaseOutCircular     (double time) { return System.Math.Sqrt(1.0 - System.Math.Pow(time - 1.0, 2.0)); }
    public static double EaseOutCubic        (double time) { return 1.0 - System.Math.Pow(1.0 - time, 3.0); }
    public static double EaseOutElastic      (double time) { return time != 0.0 && time != 1.0 ? (System.Math.Pow(2.0, time * -10.0) * System.Math.Sin(((time * 10.0) - 0.75) * ((System.Math.PI * 2.0) / 3.0))) + 1.0 : time; }
    public static double EaseOutExponential  (double time) { return time != 1.0 ? 1.0 - System.Math.Pow(2.0, time * -10.0) : 1.0; }
    public static double EaseOutQuadratic    (double time) { return 1.0 - System.Math.Pow(1.0 - time, 2.0); }
    public static double EaseOutQuartic      (double time) { return 1.0 - System.Math.Pow(1.0 - time, 4.0); }
    public static double EaseOutQuintic      (double time) { return 1.0 - System.Math.Pow(1.0 - time, 5.0); }
    public static double EaseOutSine         (double time) { return System.Math.Sin((System.Math.PI * time) / 2.0); }
    public static double Linear              (double time) { return time; }

    public void Finish()                 => this.timestamp = 0.0;
    public void Reset ()                 => this.Reset(Timeframe.CurrentTimestamp);
    public void Reset (double timestamp) => this.timestamp = timestamp;
    public void Wait  ()                 => this.Wait(this.elapsed);
    public void Wait  (double duration)  => this.timestamp += duration;
  }

  #if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(ReadOnlyInInspectorAttribute))]
    public class ReadOnlyInInspectorDrawer : UnityEditor.PropertyDrawer {
      public override float GetPropertyHeight(UnityEditor.SerializedProperty property, UnityEngine.GUIContent         label) => UnityEditor.EditorGUI.GetPropertyHeight(property, label, true);
      public override void  OnGUI            (UnityEngine.Rect               position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) { UnityEngine.GUI.enabled = false; using (new UnityEditor.EditorGUI.DisabledScope(true)) { UnityEditor.EditorGUI.PropertyField(position, property, label, true); } UnityEngine.GUI.enabled = true; }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(RenameInInspectorAttribute))]
    public class RenameInInspectorDrawer : UnityEditor.PropertyDrawer {
      public override float GetPropertyHeight(UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) => base.GetPropertyHeight(property, label);
      public override void  OnGUI            (UnityEngine.Rect position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) {
        RenameInInspectorAttribute attribute = (RenameInInspectorAttribute) base.attribute;

        label.text = attribute.name;
        UnityEditor.EditorGUI.PropertyField(position, property, label, true);
      }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(Timeframe))]
    public class TimeframeDrawer : UnityEditor.PropertyDrawer {
      private static UnityEditor.SerializedObject? SerializedObject = null;

      /* … */
      public override float GetPropertyHeight(UnityEditor.SerializedProperty property, UnityEngine.GUIContent         label) => base.GetPropertyHeight(property, label) * 2.0f;
      public override void  OnGUI            (UnityEngine.Rect               position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) {
        UnityEngine.Color              color              = UnityEngine.GUI.color;
        double                         duration           = default;
        bool                           multipleIsSelected = property.hasMultipleDifferentValues; // --> property.serializedObject.targetObjects.Length > 1;
        string[]                       propertyPath       = property.propertyPath.Substring(property.propertyPath.IndexOf('.', System.StringComparison.OrdinalIgnoreCase) + 1).Split('.', System.StringSplitOptions.RemoveEmptyEntries /* | System.StringSplitOptions.TrimEntries */);
        UnityEngine.GUIStyle           style              = UnityEngine.GUIStyle.none;
        Timeframe                      timeframe          = default;
        UnityEditor.SerializedProperty timeframeDuration  = null!;

        // … ->> Always update because `Timeframe` could either be `isElapsed` or not --> bool UnityEditor.Editor::RequiresConstantRepaint() => true
        TimeframeDrawer.SerializedObject      = property.serializedObject;
        UnityEditor.EditorApplication.update -= TimeframeDrawer.Repaint;
        UnityEditor.EditorApplication.update += TimeframeDrawer.Repaint;

        // …
        for (uint index = 1u; index != (uint) propertyPath.Length; ++index)
          property = property.FindPropertyRelative(propertyPath[index]);

        timeframe         = (Timeframe) property.boxedValue;
        timeframeDuration = property.FindPropertyRelative("duration");

        UnityEditor.EditorGUI.BeginProperty(position, label, property);
          UnityEngine.GUI.color = new(color.r, color.g, color.b, color.a * 0.5f);
          style                 = new(UnityEngine.GUI.skin.label);
          style.alignment       = UnityEngine.TextAnchor.MiddleRight;
          UnityEngine.GUI.Label(new UnityEngine.Rect(position.x + UnityEditor.EditorGUIUtility.labelWidth, position.y, position.width - UnityEditor.EditorGUIUtility.labelWidth, position.height * 0.5f), $"{(Game.IsLoaded && !Game.IsQuitting ? (timeframe.easedProgress * 100.0).ToString("F2") : "-.--")}% ({(Game.IsLoaded && !Game.IsQuitting ? timeframe.loops.ToString() : "—")})", style);

          UnityEngine.GUI.color = color;
          style                 = UnityEngine.GUI.skin.label;
          UnityEngine.GUI.Label(new UnityEngine.Rect(position.x,                            position.y,                            UnityEditor.EditorGUIUtility.labelWidth, position.height * 0.5f), label.text,     style);
          UnityEngine.GUI.Label(new UnityEngine.Rect(position.x + (position.width * 0.05f), position.y + (position.height * 0.5f), position.width * 0.95f,                  position.height * 0.5f), "Duration (s)", style);

          style = UnityEditor.EditorStyles.numberField;
          UnityEditor.EditorGUI.BeginChangeCheck();
            UnityEditor.EditorGUI.showMixedValue = multipleIsSelected;
            duration                             = UnityEditor.EditorGUI.DoubleField(new UnityEngine.Rect(position.x + (position.width * 0.5f), position.y + (position.height * 0.5f), position.width * 0.5f, position.height * 0.5f), timeframeDuration.doubleValue, style);
            UnityEditor.EditorGUI.showMixedValue = false;
          if (UnityEditor.EditorGUI.EndChangeCheck()) { timeframeDuration.doubleValue = duration; property.serializedObject.ApplyModifiedProperties(); } // --> Game.AskToSave()
        UnityEditor.EditorGUI.EndProperty();
      }

      private static void Repaint() {
        foreach (UnityEditor.Editor editor in UnityEditor.ActiveEditorTracker.sharedTracker.activeEditors)
        if (TimeframeDrawer.SerializedObject == editor.serializedObject) {
          editor.Repaint();
          return;
        }
      }
    }
  #endif
}
