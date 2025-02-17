#nullable enable annotations

namespace PatchOdyssey /* → Class types */ {
  private class AnimationKeyframe {
    public              System.Collections.ObjectModel.ReadOnlyDictionary<string, object?>? begin      =  null;
    public     readonly System.Collections.ObjectModel.ReadOnlyDictionary<string, object?>  end        =  new(new System.Collections.Generic.Dictionary<string, object?>());
    public     readonly string                                                              name       =  null;
    public ref readonly System.Collections.ObjectModel.ReadOnlyDictionary<string, object?>  properties => ref this.end;
    public     readonly double                                                              timestamp  =  0.0;

    /* … */
    public AnimationKeyframe(string name, System.Collections.Generic.IDictionary<string, object?> properties) {
      this.name      = name;
      this.timestamp = UnityEngine.Time.realtimeSinceStartupAsDouble;

      if (null == properties)
      return;

      this.properties = new(new System.Collections.Generic.Dictionary<string, object?>(properties));
    }

    public AnimationKeyframe(string name, System.Collections.Generic.IDictionary<string, object?> begin, System.Collections.Generic.IDictionary<string, object?> end) {
      (System.Collections.Generic.Dictionary<string, object?> begin, System.Collections.Generic.Dictionary<string, object?> end) properties = (new(begin?.Keys.Count ?? 0), new(end?.Keys.Count ?? 0));

      // …
      this.name      = name;
      this.timestamp = UnityEngine.Time.realtimeSinceStartupAsDouble;

      if (null == begin || null == end)
      return;

      foreach (string property in begin.Keys) {
        if (end.ContainsKey(property))
        properties.begin.Add(property, begin[property]);
      }

      foreach (string proeprty in end.Keys) {
        if (begin.ContainsKey(property))
        properties.end.Add(property, end[property]);
      }

      properties.begin.Capacity = properties.begin.Count;
      properties.end  .Capacity = properties.end  .Count;
      this.begin                = properties.begin;
      this.end                  = properties.end;
    }
  }

  public class AnimationSequence : AnimationKeyframe {
    /* … */
    public  double                                                          delay        = 0.0;
    public  double                                                          duration     = 0.0;
    public  System.Func                    <double, double>                 easing       = PatchOdyssey.AnimationFunction.Linear;
    public  System.Func                    <double, object, object>         interpolator = PatchOdyssey.AnimationFunction.Linear;
    private System.Collections.Generic.List<PatchOdyssey.AnimationKeyframe> keyframes;

    /* … */
    public AnimationSequence(string name, double duration,               System.Func<double, double> easing, System.Func<double, object, object> interpolator, System.Collections.Generic.Dictionary <string, object?> begin, System.Collections.Generic.Dictionary <string, object?> end) : this(name, duration, 0.0,   easing, begin, end)                                                                                                                         {}
    public AnimationSequence(string name, double duration,               System.Func<double, double> easing, System.Func<double, object, object> interpolator, System.Collections.Generic.IDictionary<string, object?> begin, System.Collections.Generic.IDictionary<string, object?> end) : this(name, duration, 0.0,   easing, begin, end)                                                                                                                         {}
    public AnimationSequence(string name, double duration, double delay, System.Func<double, double> easing, System.Func<double, object, object> interpolator, System.Collections.Generic.Dictionary <string, object?> begin, System.Collections.Generic.Dictionary <string, object?> end) : this(name, duration, delay, easing, begin as System.Collections.Generic.IDictionary<string, object?>, end as System.Collections.Generic.IDictionary<string, object?>) {}
    public AnimationSequence(string name, double duration, double delay, System.Func<double, double> easing, System.Func<double, object, object> interpolator, System.Collections.Generic.IDictionary<string, object?> begin, System.Collections.Generic.IDictionary<string, object?> end) : base(name, begin ?? new System.Collections.Generic.Dictionary<string, object?>(0), end) {
      this.delay        = delay;
      this.duration     = duration;
      this.easing       = easing;
      this.interpolator = interpolator;
    }

    /* … */
    public void Add(double progress, System.Collections.Generic.IDictionary<string, object?> properties) {
      new AnimationSequence(5.0, AnimationFunction.Linear, null, new() {"color", new(255, 0, 255)}, new() {"color", new(255, 255, 255)}) {
        {null, Util.Percent(30.0), {"color", new(255, 127, 255)}},
        {"renamed", {}}
      }
    }

    public void Remove(double progress) {}
    public void Remove(string keyframe) {}
    public void Remove(double begin, double end) {}

    private static object Interpolate(double progress, object? a, object? b) {
      System.Type? type = a?.GetType();

      if (type != b?.GetType())    return null;
      if (type == typeof(double))  return (double)  ((double) ((double)  b - (double)  a) * progress);
      if (type == typeof(float))   return (float)   ((double) ((float)   b - (float)   a) * progress);
      if (type == typeof(decimal)) return (decimal) ((double) ((decimal) b - (decimal) a) * progress);
      if (type == typeof(int))     return (int)     ((double) ((int)     b - (int)     a) * progress);
      if (type == typeof(nint))    return (nint)    ((double) ((nint)    b - (nint)    a) * progress);
      if (type == typeof(long))    return (long)    ((double) ((long)    b - (long)    a) * progress);
      if (type == typeof(uint))    return (uint)    ((double) ((uint)    b - (uint)    a) * progress);
      if (type == typeof(nuint))   return (nuint)   ((double) ((nuint)   b - (nuint)   a) * progress);
      if (type == typeof(ulong))   return (ulong)   ((double) ((ulong)   b - (ulong)   a) * progress);
      if (type == typeof(short))   return (short)   ((double) ((short)   b - (short)   a) * progress);
      if (type == typeof(ushort))  return (ushort)  ((double) ((ushort)  b - (ushort)  a) * progress);
      if (type == typeof(byte))    return (byte)    ((double) ((byte)    b - (byte)    a) * progress);
      if (type == typeof(sbyte))   return (sbyte)   ((double) ((sbyte)   b - (sbyte)   a) * progress);

      // …
      System.Reflection.MethodInfo? subtractionOverload    = type?.GetMethod("op_Subtraction", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, new[] {type, type}, null);
      System.Type                   progressType           = progress.GetType();
      System.Reflection.MethodInfo? multiplicationOverload = System.Array.Find(type?.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static), method => {
        if (method.Name != "op_Multiply") return false;
        System.Reflection.ParameterInfo[] parameters = method.GetParameters();

        return parameters.Length == 2 && parameters[0].ParameterType == (subtractionOverload?.ReturnType ?? type) && System.Array.Exists(new[] {typeof(double), typeof(float), typeof(decimal), typeof(int), typeof(nint), typeof(long), typeof(uint), typeof(nuint), typeof(ulong), typeof(short), typeof(ushort), typeof(byte), typeof(sbyte)}, type => {
          if (parameters[1].ParameterType != type) return false;

          progressType = type;
          return true;
        });
      });

      return multiplicationOverload?.Invoke(null, new[] {subtractionOverload?.Invoke(null, new[] {b, a}), System.Convert.ChangeType(progress, progressType)});
    }

    public void Reset() => this.timestamp = UnityEngine.Time.realtimeSinceStartupAsDouble;

    public object? this[string property] { get {
      double elapsed = UnityEngine.Time.realtimeSinceStartupAsDouble - this.timestamp;
      return (this.interpolator ?? PatchOdyssey.AnimationSequence.Interpolate)(this.delay <= elapsed ? 0.0 : this.duration <= elapsed - this.delay ? 1.0 : (this.easing ?? PatchOdyssey.AnimationFunction.Linear)((elapsed - this.delay) / this.duration), this.begin[property], this.end[property]);
    } }
  }

  public sealed class ReadOnlyInInspectorAttribute : UnityEngine.PropertyAttribute {}

  [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false, Inherited = false)]
  public sealed class ReadWriteInInspectorAttribute : System.Attribute {}

  [System.Serializable]
  public class SerializedDictionary<TKey, TValue> : System.Collections.Generic.Dictionary<TKey, TValue> {
    public SerializedDictionary()                                                                                                                                                                            : base()                     => this.Ensure();
    public SerializedDictionary(int                                                                                                 capacity)                                                                : base(capacity)             => this.Ensure();
    public SerializedDictionary(System.Collections.Generic.IEqualityComparer<TKey>                                                  comparer)                                                                : base(comparer)             => this.Ensure();
    public SerializedDictionary(System.Collections.Generic.IDictionary      <TKey, TValue>                                          dictionary)                                                              : base(dictionary)           => this.Ensure();
    public SerializedDictionary(System.Collections.Generic.IEnumerable      <System.Collections.Generic.KeyValuePair<TKey, TValue>> enumerable)                                                              : base(enumerable)           => this.Ensure();
    public SerializedDictionary(int                                                                                                 capacity,   System.Collections.Generic.IEqualityComparer<TKey> comparer) : base(capacity,   comparer) => this.Ensure();
    public SerializedDictionary(System.Collections.Generic.IDictionary<TKey, TValue>                                                dictionary, System.Collections.Generic.IEqualityComparer<TKey> comparer) : base(dictionary, comparer) => this.Ensure();
    public SerializedDictionary(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>       enumerable, System.Collections.Generic.IEqualityComparer<TKey> comparer) : base(enumerable, comparer) => this.Ensure();

    /* … */
    public static System.Func<UnityEngine.Rect, object, object> DelegateGUIField<T>() => (System.Func<UnityEngine.Rect, object, object>) Util.Switch(typeof(T), new() {
      {typeof(bool),                       (System.Func<UnityEngine.Rect, object, object>) (static (position, value) => UnityEditor.EditorGUI.Toggle         (position,                              (System.Boolean)             value) as object)},
      {typeof(double),                     (System.Func<UnityEngine.Rect, object, object>) (static (position, value) => UnityEditor.EditorGUI.DoubleField    (position,                              (System.Double)              value) as object)},
      {typeof(float),                      (System.Func<UnityEngine.Rect, object, object>) (static (position, value) => UnityEditor.EditorGUI.FloatField     (position,                              (System.Single)              value) as object)},
      {typeof(int),                        (System.Func<UnityEngine.Rect, object, object>) (static (position, value) => UnityEditor.EditorGUI.IntField       (position,                              (System.Int32)               value) as object)},
      {typeof(long),                       (System.Func<UnityEngine.Rect, object, object>) (static (position, value) => UnityEditor.EditorGUI.LongField      (position,                              (System.Int64)               value) as object)},
      {typeof(string),                     (System.Func<UnityEngine.Rect, object, object>) (static (position, value) => UnityEditor.EditorGUI.TextField      (position,                              (System.String)              value) as object)},
      {typeof(UnityEngine.AnimationCurve), (System.Func<UnityEngine.Rect, object, object>) (static (position, value) => UnityEditor.EditorGUI.CurveField     (position,                              (UnityEngine.AnimationCurve) value) as object)},
      {typeof(UnityEngine.Bounds),         (System.Func<UnityEngine.Rect, object, object>) (static (position, value) => UnityEditor.EditorGUI.BoundsField    (position,                              (UnityEngine.Bounds)         value) as object)},
      {typeof(UnityEngine.BoundsInt),      (System.Func<UnityEngine.Rect, object, object>) (static (position, value) => UnityEditor.EditorGUI.BoundsIntField (position,                              (UnityEngine.BoundsInt)      value) as object)},
      {typeof(UnityEngine.Color),          (System.Func<UnityEngine.Rect, object, object>) (static (position, value) => UnityEditor.EditorGUI.ColorField     (position,                              (UnityEngine.Color)          value) as object)},
      {typeof(UnityEngine.Gradient),       (System.Func<UnityEngine.Rect, object, object>) (static (position, value) => UnityEditor.EditorGUI.GradientField  (position,                              (UnityEngine.Gradient)       value) as object)},
      {typeof(UnityEngine.Rect),           (System.Func<UnityEngine.Rect, object, object>) (static (position, value) => UnityEditor.EditorGUI.RectField      (position,                              (UnityEngine.Rect)           value) as object)},
      {typeof(UnityEngine.RectInt),        (System.Func<UnityEngine.Rect, object, object>) (static (position, value) => UnityEditor.EditorGUI.RectIntField   (position,                              (UnityEngine.RectInt)        value) as object)},
      {typeof(UnityEngine.Vector2),        (System.Func<UnityEngine.Rect, object, object>) (static (position, value) => UnityEditor.EditorGUI.Vector2Field   (position, UnityEngine.GUIContent.none, (UnityEngine.Vector2)        value) as object)},
      {typeof(UnityEngine.Vector2Int),     (System.Func<UnityEngine.Rect, object, object>) (static (position, value) => UnityEditor.EditorGUI.Vector2IntField(position, UnityEngine.GUIContent.none, (UnityEngine.Vector2Int)     value) as object)},
      {typeof(UnityEngine.Vector3),        (System.Func<UnityEngine.Rect, object, object>) (static (position, value) => UnityEditor.EditorGUI.Vector3Field   (position, UnityEngine.GUIContent.none, (UnityEngine.Vector3)        value) as object)},
      {typeof(UnityEngine.Vector3Int),     (System.Func<UnityEngine.Rect, object, object>) (static (position, value) => UnityEditor.EditorGUI.Vector3IntField(position, UnityEngine.GUIContent.none, (UnityEngine.Vector3Int)     value) as object)},
      {typeof(UnityEngine.Vector4),        (System.Func<UnityEngine.Rect, object, object>) (static (position, value) => UnityEditor.EditorGUI.Vector4Field   (position, UnityEngine.GUIContent.none, (UnityEngine.Vector4)        value) as object)}
    }) ?? (
      typeof(T).IsEnum                                       ? static (position, value) => UnityEditor.EditorGUI.EnumPopup  (position, (System.Enum)        value)                  as object :
      typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)) ?        (position, value) => UnityEditor.EditorGUI.ObjectField(position, (UnityEngine.Object) value, typeof(T), true) as object :
      null
    );

    public void Ensure() {
      if (null == PatchOdyssey.SerializedDictionary<TKey, TValue>.DelegateGUIField<TKey>  ()) throw new System.NotSupportedException($"[{UnityEngine.Application.productName}]: Type `{typeof(TKey)}` is not supported for `System.*.IDictionary` object");
      if (null == PatchOdyssey.SerializedDictionary<TKey, TValue>.DelegateGUIField<TValue>()) throw new System.NotSupportedException($"[{UnityEngine.Application.productName}]: Type `{typeof(TValue)}` is not supported for `System.*.IDictionary` object");
    }
  }
    [System.Serializable] public class AnimationCurveDictionary : PatchOdyssey.SerializedDictionary<string, UnityEngine.AnimationCurve> { public AnimationCurveDictionary() : base() {} public AnimationCurveDictionary(int capacity) : base(capacity) {} public AnimationCurveDictionary(System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public AnimationCurveDictionary(System.Collections.Generic.IDictionary<string, UnityEngine.AnimationCurve> dictionary) : base(dictionary) {} public AnimationCurveDictionary(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.AnimationCurve>> enumerable) : base(enumerable) {} public AnimationCurveDictionary(int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public AnimationCurveDictionary(System.Collections.Generic.IDictionary<string, UnityEngine.AnimationCurve> dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public AnimationCurveDictionary(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.AnimationCurve>> enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class BooleanDictionary        : PatchOdyssey.SerializedDictionary<string, System.Boolean>             { public BooleanDictionary       () : base() {} public BooleanDictionary       (int capacity) : base(capacity) {} public BooleanDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public BooleanDictionary       (System.Collections.Generic.IDictionary<string, System.Boolean>             dictionary) : base(dictionary) {} public BooleanDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Boolean>>             enumerable) : base(enumerable) {} public BooleanDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public BooleanDictionary       (System.Collections.Generic.IDictionary<string, System.Boolean>             dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public BooleanDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Boolean>>             enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class BoundsDictionary         : PatchOdyssey.SerializedDictionary<string, UnityEngine.Bounds>         { public BoundsDictionary        () : base() {} public BoundsDictionary        (int capacity) : base(capacity) {} public BoundsDictionary        (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public BoundsDictionary        (System.Collections.Generic.IDictionary<string, UnityEngine.Bounds>         dictionary) : base(dictionary) {} public BoundsDictionary        (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Bounds>>         enumerable) : base(enumerable) {} public BoundsDictionary        (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public BoundsDictionary        (System.Collections.Generic.IDictionary<string, UnityEngine.Bounds>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public BoundsDictionary        (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Bounds>>         enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class BoundsIntDictionary      : PatchOdyssey.SerializedDictionary<string, UnityEngine.BoundsInt>      { public BoundsIntDictionary     () : base() {} public BoundsIntDictionary     (int capacity) : base(capacity) {} public BoundsIntDictionary     (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public BoundsIntDictionary     (System.Collections.Generic.IDictionary<string, UnityEngine.BoundsInt>      dictionary) : base(dictionary) {} public BoundsIntDictionary     (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.BoundsInt>>      enumerable) : base(enumerable) {} public BoundsIntDictionary     (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public BoundsIntDictionary     (System.Collections.Generic.IDictionary<string, UnityEngine.BoundsInt>      dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public BoundsIntDictionary     (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.BoundsInt>>      enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class ColorDictionary          : PatchOdyssey.SerializedDictionary<string, UnityEngine.Color>          { public ColorDictionary         () : base() {} public ColorDictionary         (int capacity) : base(capacity) {} public ColorDictionary         (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public ColorDictionary         (System.Collections.Generic.IDictionary<string, UnityEngine.Color>          dictionary) : base(dictionary) {} public ColorDictionary         (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Color>>          enumerable) : base(enumerable) {} public ColorDictionary         (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public ColorDictionary         (System.Collections.Generic.IDictionary<string, UnityEngine.Color>          dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public ColorDictionary         (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Color>>          enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class DoubleDictionary         : PatchOdyssey.SerializedDictionary<string, System.Double>              { public DoubleDictionary        () : base() {} public DoubleDictionary        (int capacity) : base(capacity) {} public DoubleDictionary        (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public DoubleDictionary        (System.Collections.Generic.IDictionary<string, System.Double>              dictionary) : base(dictionary) {} public DoubleDictionary        (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Double>>              enumerable) : base(enumerable) {} public DoubleDictionary        (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public DoubleDictionary        (System.Collections.Generic.IDictionary<string, System.Double>              dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public DoubleDictionary        (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Double>>              enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class FloatDictionary          : PatchOdyssey.SerializedDictionary<string, System.Single>              { public FloatDictionary         () : base() {} public FloatDictionary         (int capacity) : base(capacity) {} public FloatDictionary         (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public FloatDictionary         (System.Collections.Generic.IDictionary<string, System.Single>              dictionary) : base(dictionary) {} public FloatDictionary         (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Single>>              enumerable) : base(enumerable) {} public FloatDictionary         (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public FloatDictionary         (System.Collections.Generic.IDictionary<string, System.Single>              dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public FloatDictionary         (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Single>>              enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class GameObjectDictionary     : PatchOdyssey.SerializedDictionary<string, UnityEngine.GameObject>     { public GameObjectDictionary    () : base() {} public GameObjectDictionary    (int capacity) : base(capacity) {} public GameObjectDictionary    (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public GameObjectDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.GameObject>     dictionary) : base(dictionary) {} public GameObjectDictionary    (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.GameObject>>     enumerable) : base(enumerable) {} public GameObjectDictionary    (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public GameObjectDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.GameObject>     dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public GameObjectDictionary    (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.GameObject>>     enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class GradientDictionary       : PatchOdyssey.SerializedDictionary<string, UnityEngine.Gradient>       { public GradientDictionary      () : base() {} public GradientDictionary      (int capacity) : base(capacity) {} public GradientDictionary      (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public GradientDictionary      (System.Collections.Generic.IDictionary<string, UnityEngine.Gradient>       dictionary) : base(dictionary) {} public GradientDictionary      (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Gradient>>       enumerable) : base(enumerable) {} public GradientDictionary      (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public GradientDictionary      (System.Collections.Generic.IDictionary<string, UnityEngine.Gradient>       dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public GradientDictionary      (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Gradient>>       enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class IntDictionary            : PatchOdyssey.SerializedDictionary<string, System.Int32>               { public IntDictionary           () : base() {} public IntDictionary           (int capacity) : base(capacity) {} public IntDictionary           (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public IntDictionary           (System.Collections.Generic.IDictionary<string, System.Int32>               dictionary) : base(dictionary) {} public IntDictionary           (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Int32>>               enumerable) : base(enumerable) {} public IntDictionary           (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public IntDictionary           (System.Collections.Generic.IDictionary<string, System.Int32>               dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public IntDictionary           (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Int32>>               enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class LongDictionary           : PatchOdyssey.SerializedDictionary<string, System.Int64>               { public LongDictionary          () : base() {} public LongDictionary          (int capacity) : base(capacity) {} public LongDictionary          (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public LongDictionary          (System.Collections.Generic.IDictionary<string, System.Int64>               dictionary) : base(dictionary) {} public LongDictionary          (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Int64>>               enumerable) : base(enumerable) {} public LongDictionary          (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public LongDictionary          (System.Collections.Generic.IDictionary<string, System.Int64>               dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public LongDictionary          (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Int64>>               enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class RectDictionary           : PatchOdyssey.SerializedDictionary<string, UnityEngine.Rect>           { public RectDictionary          () : base() {} public RectDictionary          (int capacity) : base(capacity) {} public RectDictionary          (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public RectDictionary          (System.Collections.Generic.IDictionary<string, UnityEngine.Rect>           dictionary) : base(dictionary) {} public RectDictionary          (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Rect>>           enumerable) : base(enumerable) {} public RectDictionary          (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public RectDictionary          (System.Collections.Generic.IDictionary<string, UnityEngine.Rect>           dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public RectDictionary          (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Rect>>           enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class RectIntDictionary        : PatchOdyssey.SerializedDictionary<string, UnityEngine.RectInt>        { public RectIntDictionary       () : base() {} public RectIntDictionary       (int capacity) : base(capacity) {} public RectIntDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public RectIntDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.RectInt>        dictionary) : base(dictionary) {} public RectIntDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.RectInt>>        enumerable) : base(enumerable) {} public RectIntDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public RectIntDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.RectInt>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public RectIntDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.RectInt>>        enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class StringDictionary         : PatchOdyssey.SerializedDictionary<string, System.String>              { public StringDictionary        () : base() {} public StringDictionary        (int capacity) : base(capacity) {} public StringDictionary        (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public StringDictionary        (System.Collections.Generic.IDictionary<string, System.String>              dictionary) : base(dictionary) {} public StringDictionary        (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.String>>              enumerable) : base(enumerable) {} public StringDictionary        (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public StringDictionary        (System.Collections.Generic.IDictionary<string, System.String>              dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public StringDictionary        (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.String>>              enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class UIntDictionary           : PatchOdyssey.SerializedDictionary<string, System.UInt32>              { public UIntDictionary          () : base() {} public UIntDictionary          (int capacity) : base(capacity) {} public UIntDictionary          (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public UIntDictionary          (System.Collections.Generic.IDictionary<string, System.UInt32>              dictionary) : base(dictionary) {} public UIntDictionary          (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.UInt32>>              enumerable) : base(enumerable) {} public UIntDictionary          (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public UIntDictionary          (System.Collections.Generic.IDictionary<string, System.UInt32>              dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public UIntDictionary          (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.UInt32>>              enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class ULongDictionary          : PatchOdyssey.SerializedDictionary<string, System.UInt64>              { public ULongDictionary         () : base() {} public ULongDictionary         (int capacity) : base(capacity) {} public ULongDictionary         (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public ULongDictionary         (System.Collections.Generic.IDictionary<string, System.UInt64>              dictionary) : base(dictionary) {} public ULongDictionary         (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.UInt64>>              enumerable) : base(enumerable) {} public ULongDictionary         (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public ULongDictionary         (System.Collections.Generic.IDictionary<string, System.UInt64>              dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public ULongDictionary         (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.UInt64>>              enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class Vector2Dictionary        : PatchOdyssey.SerializedDictionary<string, UnityEngine.Vector2>        { public Vector2Dictionary       () : base() {} public Vector2Dictionary       (int capacity) : base(capacity) {} public Vector2Dictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public Vector2Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2>        dictionary) : base(dictionary) {} public Vector2Dictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector2>>        enumerable) : base(enumerable) {} public Vector2Dictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public Vector2Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public Vector2Dictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector2>>        enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class Vector2IntDictionary     : PatchOdyssey.SerializedDictionary<string, UnityEngine.Vector2Int>     { public Vector2IntDictionary    () : base() {} public Vector2IntDictionary    (int capacity) : base(capacity) {} public Vector2IntDictionary    (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public Vector2IntDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2Int>     dictionary) : base(dictionary) {} public Vector2IntDictionary    (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector2Int>>     enumerable) : base(enumerable) {} public Vector2IntDictionary    (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public Vector2IntDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2Int>     dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public Vector2IntDictionary    (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector2Int>>     enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class Vector3Dictionary        : PatchOdyssey.SerializedDictionary<string, UnityEngine.Vector3>        { public Vector3Dictionary       () : base() {} public Vector3Dictionary       (int capacity) : base(capacity) {} public Vector3Dictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public Vector3Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3>        dictionary) : base(dictionary) {} public Vector3Dictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector3>>        enumerable) : base(enumerable) {} public Vector3Dictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public Vector3Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public Vector3Dictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector3>>        enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class Vector3IntDictionary     : PatchOdyssey.SerializedDictionary<string, UnityEngine.Vector3Int>     { public Vector3IntDictionary    () : base() {} public Vector3IntDictionary    (int capacity) : base(capacity) {} public Vector3IntDictionary    (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public Vector3IntDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3Int>     dictionary) : base(dictionary) {} public Vector3IntDictionary    (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector3Int>>     enumerable) : base(enumerable) {} public Vector3IntDictionary    (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public Vector3IntDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3Int>     dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public Vector3IntDictionary    (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector3Int>>     enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class Vector4Dictionary        : PatchOdyssey.SerializedDictionary<string, UnityEngine.Vector4>        { public Vector4Dictionary       () : base() {} public Vector4Dictionary       (int capacity) : base(capacity) {} public Vector4Dictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public Vector4Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector4>        dictionary) : base(dictionary) {} public Vector4Dictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector4>>        enumerable) : base(enumerable) {} public Vector4Dictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public Vector4Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector4>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public Vector4Dictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector4>>        enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }

  [System.Serializable]
  public class SerializedReadOnlyDictionary<TKey, TValue> : System.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue> {
    #pragma warning disable CS0108
    public readonly int Capacity = 0;
    #pragma warning restore CS0108

    /* … */
    public SerializedReadOnlyDictionary(int                                                                                                 capacity)                                                                : base(new System.Collections.Generic.Dictionary<TKey, TValue>(capacity))             => this.Capacity = capacity;
    public SerializedReadOnlyDictionary(System.Collections.Generic.IEqualityComparer<TKey>                                                  comparer)                                                                : base(new System.Collections.Generic.Dictionary<TKey, TValue>(comparer))             {}
    public SerializedReadOnlyDictionary(System.Collections.Generic.IDictionary      <TKey, TValue>                                          dictionary)                                                              : base(dictionary)                                                                    {}
    public SerializedReadOnlyDictionary(System.Collections.Generic.IEnumerable      <System.Collections.Generic.KeyValuePair<TKey, TValue>> enumerable)                                                              : base(new System.Collections.Generic.Dictionary<TKey, TValue>(enumerable))           {}
    public SerializedReadOnlyDictionary(int                                                                                                 capacity,   System.Collections.Generic.IEqualityComparer<TKey> comparer) : base(new System.Collections.Generic.Dictionary<TKey, TValue>(capacity,   comparer)) => this.Capacity = capacity;
    public SerializedReadOnlyDictionary(System.Collections.Generic.IDictionary<TKey, TValue>                                                dictionary, System.Collections.Generic.IEqualityComparer<TKey> comparer) : base(new System.Collections.Generic.Dictionary<TKey, TValue>(dictionary, comparer)) {}
    public SerializedReadOnlyDictionary(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>       enumerable, System.Collections.Generic.IEqualityComparer<TKey> comparer) : base(new System.Collections.Generic.Dictionary<TKey, TValue>(enumerable, comparer)) {}

    /* … */
    public void Add(TKey key, TValue value) {
      if (this.Capacity <= this.Count)
        throw new System.NotSupportedException("Can not add item to `SerializedReadOnlyDictionary`");

      this.Dictionary.Add(key, value);
    }
  }
    [System.Serializable] public class AnimationCurveReadOnlyDictionary : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.AnimationCurve> { public AnimationCurveReadOnlyDictionary(int capacity) : base(capacity) {} public AnimationCurveReadOnlyDictionary(System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public AnimationCurveReadOnlyDictionary(System.Collections.Generic.IDictionary<string, UnityEngine.AnimationCurve> dictionary) : base(dictionary) {} public AnimationCurveReadOnlyDictionary(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.AnimationCurve>> enumerable) : base(enumerable) {} public AnimationCurveReadOnlyDictionary(int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public AnimationCurveReadOnlyDictionary(System.Collections.Generic.IDictionary<string, UnityEngine.AnimationCurve> dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public AnimationCurveReadOnlyDictionary(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.AnimationCurve>> enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class BooleanReadOnlyDictionary        : PatchOdyssey.SerializedReadOnlyDictionary<string, System.Boolean>             { public BooleanReadOnlyDictionary       (int capacity) : base(capacity) {} public BooleanReadOnlyDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public BooleanReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, System.Boolean>             dictionary) : base(dictionary) {} public BooleanReadOnlyDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Boolean>>             enumerable) : base(enumerable) {} public BooleanReadOnlyDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public BooleanReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, System.Boolean>             dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public BooleanReadOnlyDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Boolean>>             enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class BoundsReadOnlyDictionary         : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.Bounds>         { public BoundsReadOnlyDictionary        (int capacity) : base(capacity) {} public BoundsReadOnlyDictionary        (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public BoundsReadOnlyDictionary        (System.Collections.Generic.IDictionary<string, UnityEngine.Bounds>         dictionary) : base(dictionary) {} public BoundsReadOnlyDictionary        (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Bounds>>         enumerable) : base(enumerable) {} public BoundsReadOnlyDictionary        (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public BoundsReadOnlyDictionary        (System.Collections.Generic.IDictionary<string, UnityEngine.Bounds>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public BoundsReadOnlyDictionary        (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Bounds>>         enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class BoundsIntReadOnlyDictionary      : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.BoundsInt>      { public BoundsIntReadOnlyDictionary     (int capacity) : base(capacity) {} public BoundsIntReadOnlyDictionary     (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public BoundsIntReadOnlyDictionary     (System.Collections.Generic.IDictionary<string, UnityEngine.BoundsInt>      dictionary) : base(dictionary) {} public BoundsIntReadOnlyDictionary     (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.BoundsInt>>      enumerable) : base(enumerable) {} public BoundsIntReadOnlyDictionary     (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public BoundsIntReadOnlyDictionary     (System.Collections.Generic.IDictionary<string, UnityEngine.BoundsInt>      dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public BoundsIntReadOnlyDictionary     (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.BoundsInt>>      enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class ColorReadOnlyDictionary          : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.Color>          { public ColorReadOnlyDictionary         (int capacity) : base(capacity) {} public ColorReadOnlyDictionary         (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public ColorReadOnlyDictionary         (System.Collections.Generic.IDictionary<string, UnityEngine.Color>          dictionary) : base(dictionary) {} public ColorReadOnlyDictionary         (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Color>>          enumerable) : base(enumerable) {} public ColorReadOnlyDictionary         (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public ColorReadOnlyDictionary         (System.Collections.Generic.IDictionary<string, UnityEngine.Color>          dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public ColorReadOnlyDictionary         (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Color>>          enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class DoubleReadOnlyDictionary         : PatchOdyssey.SerializedReadOnlyDictionary<string, System.Double>              { public DoubleReadOnlyDictionary        (int capacity) : base(capacity) {} public DoubleReadOnlyDictionary        (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public DoubleReadOnlyDictionary        (System.Collections.Generic.IDictionary<string, System.Double>              dictionary) : base(dictionary) {} public DoubleReadOnlyDictionary        (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Double>>              enumerable) : base(enumerable) {} public DoubleReadOnlyDictionary        (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public DoubleReadOnlyDictionary        (System.Collections.Generic.IDictionary<string, System.Double>              dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public DoubleReadOnlyDictionary        (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Double>>              enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class FloatReadOnlyDictionary          : PatchOdyssey.SerializedReadOnlyDictionary<string, System.Single>              { public FloatReadOnlyDictionary         (int capacity) : base(capacity) {} public FloatReadOnlyDictionary         (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public FloatReadOnlyDictionary         (System.Collections.Generic.IDictionary<string, System.Single>              dictionary) : base(dictionary) {} public FloatReadOnlyDictionary         (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Single>>              enumerable) : base(enumerable) {} public FloatReadOnlyDictionary         (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public FloatReadOnlyDictionary         (System.Collections.Generic.IDictionary<string, System.Single>              dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public FloatReadOnlyDictionary         (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Single>>              enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class GameObjectReadOnlyDictionary     : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.GameObject>     { public GameObjectReadOnlyDictionary    (int capacity) : base(capacity) {} public GameObjectReadOnlyDictionary    (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public GameObjectReadOnlyDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.GameObject>     dictionary) : base(dictionary) {} public GameObjectReadOnlyDictionary    (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.GameObject>>     enumerable) : base(enumerable) {} public GameObjectReadOnlyDictionary    (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public GameObjectReadOnlyDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.GameObject>     dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public GameObjectReadOnlyDictionary    (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.GameObject>>     enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class GradientReadOnlyDictionary       : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.Gradient>       { public GradientReadOnlyDictionary      (int capacity) : base(capacity) {} public GradientReadOnlyDictionary      (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public GradientReadOnlyDictionary      (System.Collections.Generic.IDictionary<string, UnityEngine.Gradient>       dictionary) : base(dictionary) {} public GradientReadOnlyDictionary      (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Gradient>>       enumerable) : base(enumerable) {} public GradientReadOnlyDictionary      (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public GradientReadOnlyDictionary      (System.Collections.Generic.IDictionary<string, UnityEngine.Gradient>       dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public GradientReadOnlyDictionary      (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Gradient>>       enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class IntReadOnlyDictionary            : PatchOdyssey.SerializedReadOnlyDictionary<string, System.Int32>               { public IntReadOnlyDictionary           (int capacity) : base(capacity) {} public IntReadOnlyDictionary           (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public IntReadOnlyDictionary           (System.Collections.Generic.IDictionary<string, System.Int32>               dictionary) : base(dictionary) {} public IntReadOnlyDictionary           (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Int32>>               enumerable) : base(enumerable) {} public IntReadOnlyDictionary           (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public IntReadOnlyDictionary           (System.Collections.Generic.IDictionary<string, System.Int32>               dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public IntReadOnlyDictionary           (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Int32>>               enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class LongReadOnlyDictionary           : PatchOdyssey.SerializedReadOnlyDictionary<string, System.Int64>               { public LongReadOnlyDictionary          (int capacity) : base(capacity) {} public LongReadOnlyDictionary          (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public LongReadOnlyDictionary          (System.Collections.Generic.IDictionary<string, System.Int64>               dictionary) : base(dictionary) {} public LongReadOnlyDictionary          (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Int64>>               enumerable) : base(enumerable) {} public LongReadOnlyDictionary          (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public LongReadOnlyDictionary          (System.Collections.Generic.IDictionary<string, System.Int64>               dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public LongReadOnlyDictionary          (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Int64>>               enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class RectReadOnlyDictionary           : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.Rect>           { public RectReadOnlyDictionary          (int capacity) : base(capacity) {} public RectReadOnlyDictionary          (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public RectReadOnlyDictionary          (System.Collections.Generic.IDictionary<string, UnityEngine.Rect>           dictionary) : base(dictionary) {} public RectReadOnlyDictionary          (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Rect>>           enumerable) : base(enumerable) {} public RectReadOnlyDictionary          (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public RectReadOnlyDictionary          (System.Collections.Generic.IDictionary<string, UnityEngine.Rect>           dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public RectReadOnlyDictionary          (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Rect>>           enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class RectIntReadOnlyDictionary        : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.RectInt>        { public RectIntReadOnlyDictionary       (int capacity) : base(capacity) {} public RectIntReadOnlyDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public RectIntReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.RectInt>        dictionary) : base(dictionary) {} public RectIntReadOnlyDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.RectInt>>        enumerable) : base(enumerable) {} public RectIntReadOnlyDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public RectIntReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.RectInt>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public RectIntReadOnlyDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.RectInt>>        enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class StringReadOnlyDictionary         : PatchOdyssey.SerializedReadOnlyDictionary<string, System.String>              { public StringReadOnlyDictionary        (int capacity) : base(capacity) {} public StringReadOnlyDictionary        (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public StringReadOnlyDictionary        (System.Collections.Generic.IDictionary<string, System.String>              dictionary) : base(dictionary) {} public StringReadOnlyDictionary        (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.String>>              enumerable) : base(enumerable) {} public StringReadOnlyDictionary        (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public StringReadOnlyDictionary        (System.Collections.Generic.IDictionary<string, System.String>              dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public StringReadOnlyDictionary        (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.String>>              enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class UIntReadOnlyDictionary           : PatchOdyssey.SerializedReadOnlyDictionary<string, System.UInt32>              { public UIntReadOnlyDictionary          (int capacity) : base(capacity) {} public UIntReadOnlyDictionary          (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public UIntReadOnlyDictionary          (System.Collections.Generic.IDictionary<string, System.UInt32>              dictionary) : base(dictionary) {} public UIntReadOnlyDictionary          (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.UInt32>>              enumerable) : base(enumerable) {} public UIntReadOnlyDictionary          (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public UIntReadOnlyDictionary          (System.Collections.Generic.IDictionary<string, System.UInt32>              dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public UIntReadOnlyDictionary          (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.UInt32>>              enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class ULongReadOnlyDictionary          : PatchOdyssey.SerializedReadOnlyDictionary<string, System.UInt64>              { public ULongReadOnlyDictionary         (int capacity) : base(capacity) {} public ULongReadOnlyDictionary         (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public ULongReadOnlyDictionary         (System.Collections.Generic.IDictionary<string, System.UInt64>              dictionary) : base(dictionary) {} public ULongReadOnlyDictionary         (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.UInt64>>              enumerable) : base(enumerable) {} public ULongReadOnlyDictionary         (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public ULongReadOnlyDictionary         (System.Collections.Generic.IDictionary<string, System.UInt64>              dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public ULongReadOnlyDictionary         (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.UInt64>>              enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class Vector2ReadOnlyDictionary        : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.Vector2>        { public Vector2ReadOnlyDictionary       (int capacity) : base(capacity) {} public Vector2ReadOnlyDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public Vector2ReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2>        dictionary) : base(dictionary) {} public Vector2ReadOnlyDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector2>>        enumerable) : base(enumerable) {} public Vector2ReadOnlyDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public Vector2ReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public Vector2ReadOnlyDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector2>>        enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class Vector2IntReadOnlyDictionary     : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.Vector2Int>     { public Vector2IntReadOnlyDictionary    (int capacity) : base(capacity) {} public Vector2IntReadOnlyDictionary    (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public Vector2IntReadOnlyDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2Int>     dictionary) : base(dictionary) {} public Vector2IntReadOnlyDictionary    (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector2Int>>     enumerable) : base(enumerable) {} public Vector2IntReadOnlyDictionary    (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public Vector2IntReadOnlyDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2Int>     dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public Vector2IntReadOnlyDictionary    (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector2Int>>     enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class Vector3ReadOnlyDictionary        : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.Vector3>        { public Vector3ReadOnlyDictionary       (int capacity) : base(capacity) {} public Vector3ReadOnlyDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public Vector3ReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3>        dictionary) : base(dictionary) {} public Vector3ReadOnlyDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector3>>        enumerable) : base(enumerable) {} public Vector3ReadOnlyDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public Vector3ReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public Vector3ReadOnlyDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector3>>        enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class Vector3IntReadOnlyDictionary     : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.Vector3Int>     { public Vector3IntReadOnlyDictionary    (int capacity) : base(capacity) {} public Vector3IntReadOnlyDictionary    (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public Vector3IntReadOnlyDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3Int>     dictionary) : base(dictionary) {} public Vector3IntReadOnlyDictionary    (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector3Int>>     enumerable) : base(enumerable) {} public Vector3IntReadOnlyDictionary    (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public Vector3IntReadOnlyDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3Int>     dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public Vector3IntReadOnlyDictionary    (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector3Int>>     enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }
    [System.Serializable] public class Vector4ReadOnlyDictionary        : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.Vector4>        { public Vector4ReadOnlyDictionary       (int capacity) : base(capacity) {} public Vector4ReadOnlyDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} public Vector4ReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector4>        dictionary) : base(dictionary) {} public Vector4ReadOnlyDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector4>>        enumerable) : base(enumerable) {} public Vector4ReadOnlyDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} public Vector4ReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector4>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} public Vector4ReadOnlyDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector4>>        enumerable, System.Collections.Generic.IEqualityComparer<string> comparer) : base(enumerable, comparer) {} }

  #if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.ReadOnlyInInspectorAttribute))]
    public class ReadOnlyInInspectorDrawer : UnityEditor.PropertyDrawer {
      public override float GetPropertyHeight(UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) {
        return UnityEditor.EditorGUI.GetPropertyHeight(property, label, true);
      }

      public override void OnGUI(UnityEngine.Rect position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) {
        UnityEngine.GUI.enabled = false;
        UnityEditor.EditorGUI.PropertyField(position, property, label, true);
        UnityEngine.GUI.enabled = true;
      }
    }

    public class SerializedDictionaryDrawer<TKey, TValue> : UnityEditor.PropertyDrawer {
      private bool foldout = false;

      /* … */
      private System.Collections.Generic.IDictionary<TKey, TValue> Ensure(UnityEditor.SerializedProperty property) {
        System.Collections.Generic.IDictionary<TKey, TValue> dictionary = (this.fieldInfo.GetValue(property.serializedObject.targetObject) ?? new PatchOdyssey.SerializedDictionary<TKey, TValue>()) as System.Collections.Generic.IDictionary<TKey, TValue>;

        // …
        this.fieldInfo.SetValue(property.serializedObject.targetObject, dictionary);
        return dictionary;
      }

      public override float GetPropertyHeight(UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) {
        return base.GetPropertyHeight(property, label) * (this.foldout || UnityEditor.EditorPrefs.GetBool(label.text) ? Util.Max(this.Ensure(property).Count, 1) + 1 : 1);
      }

      public override void OnGUI(UnityEngine.Rect position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) {
        System.Collections.Generic.IDictionary<TKey, TValue> dictionary           = this.Ensure(property);
        bool                                                 dictionaryIsReadOnly = dictionary is System.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue>;
        float                                                size                 = position.height = base.GetPropertyHeight(property, label);
        var                                                  positions            = new {
          add     = new UnityEngine.Rect(position.x + (position.width - Util.PercentOf(size, 200.0f)), position.y, 0.0f           + size, position.height),
          clear   = new UnityEngine.Rect(position.x + (position.width - Util.PercentOf(size, 100.0f)), position.y, 0.0f           + size, position.height),
          foldout = new UnityEngine.Rect(position.x,                                                   position.y, position.width - size, position.height)
        };

        // …
        if (!dictionaryIsReadOnly) {
          if (UnityEngine.GUI.Button(positions.add, new UnityEngine.GUIContent("+", "Add field"), UnityEditor.EditorStyles.miniButton))
          dictionary.TryAdd(
            typeof(TKey) != typeof(string) ? System.Activator.CreateInstance<TKey>  () : (TKey)   (""   as object),
            typeof(TValue).IsValueType     ? System.Activator.CreateInstance<TValue>() : (TValue) (null as object)
          );

          if (UnityEngine.GUI.Button(positions.clear, new UnityEngine.GUIContent("×", "Clear dictionary"), UnityEditor.EditorStyles.miniButtonRight))
          dictionary.Clear();
        }

        UnityEditor.EditorGUI.BeginChangeCheck(); {
          this.foldout = UnityEditor.EditorPrefs.GetBool(label.text);
          this.foldout = UnityEditor.EditorGUI.  Foldout(positions.foldout, this.foldout, label, true);
        } if (UnityEditor.EditorGUI.EndChangeCheck()) UnityEditor.EditorPrefs.SetBool(label.text, this.foldout);

        if (!this.foldout)
        return;

        if (0 == dictionary.Count) UnityEngine.GUI.Label(new(position.x, position.y + position.height, position.width, position.height), "Dictionary is empty");
        else foreach (System.Collections.Generic.KeyValuePair<TKey, TValue> item in dictionary) {
          (TKey key, TValue value) = (item.Key, item.Value);
          var subpositions         = new {
            key   = new UnityEngine.Rect(position.x                                                 + (dictionaryIsReadOnly ? size : 0.0f), position.y += position.height, Util.PercentOf(position.width - size, 40.0f), position.height),
            value = new UnityEngine.Rect(position.x + Util.PercentOf(position.width - size,  40.0f) + (dictionaryIsReadOnly ? size : 0.0f), position.y,                    Util.PercentOf(position.width - size, 60.0f), position.height),
            clear = new UnityEngine.Rect(position.x + Util.PercentOf(position.width - size, 100.0f),                                        position.y,                    size,                                         position.height)
          };

          // …
          UnityEditor.EditorGUI.BeginChangeCheck();
            key = (TKey) PatchOdyssey.SerializedDictionary<TKey, TValue>.DelegateGUIField<TKey>()(subpositions.key, key);
          if (UnityEditor.EditorGUI.EndChangeCheck()) { dictionary.Remove(item.Key); dictionary.Add(key, value); break; }

          UnityEditor.EditorGUI.BeginChangeCheck();
            value = (TValue) PatchOdyssey.SerializedDictionary<TKey, TValue>.DelegateGUIField<TValue>()(subpositions.value, value);
          if (UnityEditor.EditorGUI.EndChangeCheck()) { dictionary[key] = value; break; }

          if (!dictionaryIsReadOnly)
          if (UnityEngine.GUI.Button(subpositions.clear, new UnityEngine.GUIContent("×", "Clear item"), UnityEditor.EditorStyles.miniButtonRight)) {
            dictionary.Remove(key);
            break;
          }
        }
      }
    }
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.AnimationCurveDictionary))]         public class AnimationCurveDictionaryDrawer         : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.AnimationCurve> {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.AnimationCurveReadOnlyDictionary))] public class AnimationCurveReadOnlyDictionaryDrawer : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.AnimationCurve> {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.BooleanDictionary))]                public class BooleanDictionaryDrawer                : PatchOdyssey.SerializedDictionaryDrawer<string, System.Boolean>             {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.BooleanReadOnlyDictionary))]        public class BooleanReadOnlyDictionaryDrawer        : PatchOdyssey.SerializedDictionaryDrawer<string, System.Boolean>             {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.BoundsDictionary))]                 public class BoundsDictionaryDrawer                 : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.Bounds>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.BoundsReadOnlyDictionary))]         public class BoundsReadOnlyDictionaryDrawer         : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.Bounds>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.BoundsIntDictionary))]              public class BoundsIntDictionaryDrawer              : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.BoundsInt>      {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.BoundsIntReadOnlyDictionary))]      public class BoundsIntReadOnlyDictionaryDrawer      : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.BoundsInt>      {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.ColorDictionary))]                  public class ColorDictionaryDrawer                  : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.Color>          {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.ColorReadOnlyDictionary))]          public class ColorReadOnlyDictionaryDrawer          : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.Color>          {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.DoubleDictionary))]                 public class DoubleDictionaryDrawer                 : PatchOdyssey.SerializedDictionaryDrawer<string, System.Double>              {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.DoubleReadOnlyDictionary))]         public class DoubleReadOnlyDictionaryDrawer         : PatchOdyssey.SerializedDictionaryDrawer<string, System.Double>              {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.FloatDictionary))]                  public class FloatDictionaryDrawer                  : PatchOdyssey.SerializedDictionaryDrawer<string, System.Single>              {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.FloatReadOnlyDictionary))]          public class FloatReadOnlyDictionaryDrawer          : PatchOdyssey.SerializedDictionaryDrawer<string, System.Single>              {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.GameObjectDictionary))]             public class GameObjectDictionaryDrawer             : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.GameObject>     {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.GameObjectReadOnlyDictionary))]     public class GameObjectReadOnlyDictionaryDrawer     : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.GameObject>     {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.GradientDictionary))]               public class GradientDictionaryDrawer               : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.Gradient>       {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.GradientReadOnlyDictionary))]       public class GradientReadOnlyDictionaryDrawer       : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.Gradient>       {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.IntDictionary))]                    public class IntDictionaryDrawer                    : PatchOdyssey.SerializedDictionaryDrawer<string, System.Int32>               {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.IntReadOnlyDictionary))]            public class IntReadOnlyDictionaryDrawer            : PatchOdyssey.SerializedDictionaryDrawer<string, System.Int32>               {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.LongDictionary))]                   public class LongDictionaryDrawer                   : PatchOdyssey.SerializedDictionaryDrawer<string, System.Int64>               {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.LongReadOnlyDictionary))]           public class LongReadOnlyDictionaryDrawer           : PatchOdyssey.SerializedDictionaryDrawer<string, System.Int64>               {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.RectDictionary))]                   public class RectDictionaryDrawer                   : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.Rect>           {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.RectReadOnlyDictionary))]           public class RectReadOnlyDictionaryDrawer           : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.Rect>           {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.RectIntDictionary))]                public class RectIntDictionaryDrawer                : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.RectInt>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.RectIntReadOnlyDictionary))]        public class RectIntReadOnlyDictionaryDrawer        : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.RectInt>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.StringDictionary))]                 public class StringDictionaryDrawer                 : PatchOdyssey.SerializedDictionaryDrawer<string, System.String>              {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.StringReadOnlyDictionary))]         public class StringReadOnlyDictionaryDrawer         : PatchOdyssey.SerializedDictionaryDrawer<string, System.String>              {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.UIntDictionary))]                   public class UIntDictionaryDrawer                   : PatchOdyssey.SerializedDictionaryDrawer<string, System.UInt32>              {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.UIntReadOnlyDictionary))]           public class UIntReadOnlyDictionaryDrawer           : PatchOdyssey.SerializedDictionaryDrawer<string, System.UInt32>              {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.ULongDictionary))]                  public class ULongDictionaryDrawer                  : PatchOdyssey.SerializedDictionaryDrawer<string, System.UInt64>              {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.ULongReadOnlyDictionary))]          public class ULongReadOnlyDictionaryDrawer          : PatchOdyssey.SerializedDictionaryDrawer<string, System.UInt64>              {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Vector2Dictionary))]                public class Vector2DictionaryDrawer                : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.Vector2>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Vector2ReadOnlyDictionary))]        public class Vector2ReadOnlyDictionaryDrawer        : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.Vector2>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Vector2IntDictionary))]             public class Vector2IntDictionaryDrawer             : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.Vector2Int>     {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Vector2IntReadOnlyDictionary))]     public class Vector2IntReadOnlyDictionaryDrawer     : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.Vector2Int>     {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Vector3Dictionary))]                public class Vector3DictionaryDrawer                : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.Vector3>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Vector3ReadOnlyDictionary))]        public class Vector3ReadOnlyDictionaryDrawer        : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.Vector3>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Vector3IntDictionary))]             public class Vector3IntDictionaryDrawer             : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.Vector3Int>     {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Vector3IntReadOnlyDictionary))]     public class Vector3IntReadOnlyDictionaryDrawer     : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.Vector3Int>     {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Vector4Dictionary))]                public class Vector4DictionaryDrawer                : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.Vector4>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Vector4ReadOnlyDictionary))]        public class Vector4ReadOnlyDictionaryDrawer        : PatchOdyssey.SerializedDictionaryDrawer<string, UnityEngine.Vector4>        {}
  #endif
}

namespace PatchOdyssey /* → …everything else */ {
  public static class AnimationFunction {
    public static double CubicBézier    (double time, double p0, double p1, double p2, double p3) => (p0 * System.Math.Pow(1.0 - time, 3.0)) + (p1 * time * 3.0 * System.Math.Pow(1.0 - time, 2.0)) + (p2 * (1.0 - time) * 3.0 * System.Math.Pow(time, 2.0)) + (p3 * System.Math.Pow(time, 3.0));
    public static double QuadraticBézier(double time, double p0, double p1, double p2)            => (p0 * System.Math.Pow(1.0 - time, 2.0)) + (p1 * time * 2.0 * System.Math.Pow(1.0 - time, 1.0))                                                          + (p2 * System.Math.Pow(time, 2.0));

    public static double Ease                (double time) => PatchOdyssey.AnimationFunction.CubicBézier(time, 0.25, 0.10, 0.25, 1.00);
    public static double EaseIn              (double time) => PatchOdyssey.AnimationFunction.CubicBézier(time, 0.42, 0.00, 1.00, 1.00);
    public static double EaseInBack          (double time) => (System.Math.Pow(time, 3.0) * 2.70158) - (System.Math.Pow(time, 2.0) * 1.70158);
    public static double EaseInBounce        (double time) => 1.0 - PatchOdyssey.AnimationFunction.EaseOutBounce(1.0 - time);
    public static double EaseInCircular      (double time) => 1.0 - System.Math.Sqrt(1.0 - System.Math.Pow(time, 2.0));
    public static double EaseInCubic         (double time) => System.Math.Pow(time, 3.0);
    public static double EaseInElastic       (double time) => time != 0.0 && time != 1.0 ? -System.Math.Pow(2.0, (time * 10.0) - 10.0) * System.Math.Sin(((time * 10.0) - 10.75) * ((System.Math.PI * 2.0) / 3.0)) : time;
    public static double EaseInExponential   (double time) => time != 0.0 ? System.Math.Pow(2.0, (time * 10.0) - 10.0) : 0.0;
    public static double EaseInOut           (double time) => PatchOdyssey.AnimationFunction.CubicBézier(time, 0.42, 0.00, 0.58, 1.00);
    public static double EaseInOutBack       (double time) => (time < 0.5 ? System.Math.Pow(time * 2.0, 2.0) * ((7.189819f * time) - 2.5949095) : ((System.Math.Pow((time * 2.0) - 2.0, 2.0) * ((((time * 2.0) - 2.0) * 3.5949095) + 2.5949095)) + 2.0)) / 2.0;
    public static double EaseInOutBounce     (double time) => (time < 0.5 ? (1.0 - PatchOdyssey.AnimationFunction.EaseOutBounce(1.0 - (time * 2.0))) : (1.0 + PatchOdyssey.AnimationFunction.EaseOutBounce((time * 2.0) - 1.0))) / 2.0;
    public static double EaseInOutCircular   (double time) => (time < 0.5 ? (1.0 - System.Math.Sqrt(1.0 - System.Math.Pow(time * 2.0, 2.0))) : (1.0 + System.Math.Sqrt(1.0 - System.Math.Pow((time * -2.0) + 2.0, 2.0)))) / 2.0;
    public static double EaseInOutCubic      (double time) => time < 0.5 ? 4.0 * System.Math.Pow(time, 3.0) : (1.0 - (System.Math.Pow((time * -2.0) + 2.0, 3.0) / 2.0));
    public static double EaseInOutElastic    (double time) => time != 0.0 && time != 1.0 ? time < 0.5 ? -(System.Math.Pow(2.0, (time * 20.0) - 10.0) * System.Math.Sin(((time * 20.0) - 11.125) * ((System.Math.PI * 2.0) / 4.5))) / 2.0 : ((System.Math.Pow(2.0, (time * -20.0) + 10.0) * System.Math.Sin(((time * 20.0) - 11.125) * ((System.Math.PI * 2.0) / 4.5))) / 2.0 + 1.0) : time;
    public static double EaseInOutExponential(double time) => time != 0.0 && time != 1.0 ? (time < 0.5 ? System.Math.Pow(2.0, (time * 20.0) - 10.0) : (2.0 - System.Math.Pow(2.0, (time * -20.0) + 10.0))) / 2.0 : time;
    public static double EaseInOutQuadratic  (double time) => time < 0.5 ?  2.0 * System.Math.Pow(time, 2.0) : (1.0 - (System.Math.Pow((time * -2.0) + 2.0, 2.0) / 2.0));
    public static double EaseInOutQuartic    (double time) => time < 0.5 ?  8.0 * System.Math.Pow(time, 4.0) : (1.0 - (System.Math.Pow((time * -2.0) + 2.0, 4.0) / 2.0));
    public static double EaseInOutQuintic    (double time) => time < 0.5 ? 16.0 * System.Math.Pow(time, 5.0) : (1.0 - (System.Math.Pow((time * -2.0) + 2.0, 5.0) / 2.0));
    public static double EaseInOutSine       (double time) => -(System.Math.Cos(System.Math.PI * time) - 1.0) / 2.0;
    public static double EaseInQuadratic     (double time) => System.Math.Pow(time, 2.0);
    public static double EaseInQuartic       (double time) => System.Math.Pow(time, 4.0);
    public static double EaseInQuintic       (double time) => System.Math.Pow(time, 5.0);
    public static double EaseInSine          (double time) => 1.0 - System.Math.Cos((System.Math.PI * time) / 2.0);
    public static double EaseOut             (double time) => PatchOdyssey.AnimationFunction.CubicBézier(time, 0.00, 0.00, 0.58, 1.00);
    public static double EaseOutBack         (double time) => 1.0 + (2.70158 * System.Math.Pow(time - 1.0, 3.0)) + (System.Math.Pow(time - 1.0, 2.0) * 1.70158);
    public static double EaseOutBounce       (double time) => time < 1.0 / 2.75 ? System.Math.Pow(time, 2.0) * 7.5625 : time < 2.0 / 2.75 ? (System.Math.Pow(time - (1.5 / 2.75), 2.0) * 7.5625) + 0.75 : time < 2.5 / 2.75 ? (System.Math.Pow(time - (2.25 / 2.75), 2.0) * 7.5625) + 0.9375 : (System.Math.Pow(time - (2.625 / 2.75), 2.0) * 7.5625) + 0.984375;
    public static double EaseOutCircular     (double time) => System.Math.Sqrt(1.0 - System.Math.Pow(time - 1.0, 2.0));
    public static double EaseOutCubic        (double time) => 1.0 - System.Math.Pow(1.0 - time, 3.0);
    public static double EaseOutElastic      (double time) => time != 0.0 && time != 1.0 ? (System.Math.Pow(2.0, time * -10.0) * System.Math.Sin(((time * 10.0) - 0.75) * ((System.Math.PI * 2.0) / 3.0))) + 1.0 : time;
    public static double EaseOutExponential  (double time) => time != 1.0 ? 1.0 - System.Math.Pow(2.0, time * -10.0) : 1.0;
    public static double EaseOutQuadratic    (double time) => 1.0 - System.Math.Pow(1.0 - time, 2.0);
    public static double EaseOutQuartic      (double time) => 1.0 - System.Math.Pow(1.0 - time, 4.0);
    public static double EaseOutQuintic      (double time) => 1.0 - System.Math.Pow(1.0 - time, 5.0);
    public static double EaseOutSine         (double time) => System.Math.Sin((System.Math.PI * time) / 2.0);
    public static double Linear              (double time) => time;
  }

  public static class Extensions {
    public static void Clear<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary) {
      foreach (TKey key in dictionary.Keys)
      dictionary.Remove(key);
    }

    public static bool ContainsValue(this System.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue> dictionary, TValue value) {
      foreach (TValue dictionaryValue in dictionary.Values) {
        if ((value as System.IEquatable<TValue>)?.Equals(dictionaryValue) ?? (object) value == (object) dictionaryValue)
        return true;
      }

      return false;
    }

    public static uint CountChildren(this UnityEngine.GameObject gameObject) {
      uint count = 0u;

      // …
      foreach (UnityEngine.Transform transform in gameObject.transform)
      ++count;

      return count;
    }

    public static uint CountDescendants(this UnityEngine.GameObject gameObject) {
      uint count = 0u;

      // …
      for (System.Collections.Generic.List<UnityEngine.GameObject> pending = new() {gameObject}; 0 != pending.Count; pending.RemoveAt(0))
      foreach (UnityEngine.Transform transform in pending[0].transform) {
        ++count;
        pending.Add(transform.gameObject);
      }

      return count;
    }

    public static T                     EnsureComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => null == gameObject.GetComponent<T>()  ? gameObject.AddComponent<T>()  : gameObject.GetComponent<T>();
    public static UnityEngine.Component EnsureComponent   (this UnityEngine.GameObject gameObject, System.Type type)               => null == gameObject.GetComponent(type) ? gameObject.AddComponent(type) : gameObject.GetComponent(type);

    public static UnityEngine.GameObject? FindChild(this UnityEngine.GameObject gameObject, System.Predicate<UnityEngine.GameObject> predicate) {
      foreach (UnityEngine.Transform transform in gameObject.transform) {
        if (predicate(transform.gameObject))
        return transform.gameObject;
      }

      return null;
    }

      public static T?                      FindChildByComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject.FindChild(child => null != child.GetComponent<T>()) ?.GetComponent<T>();
      public static UnityEngine.Component?  FindChildByComponent   (this UnityEngine.GameObject gameObject, System.Type type)               => gameObject.FindChild(child => null != child.GetComponent(type))?.GetComponent(type);
      public static UnityEngine.GameObject? FindChildByIndex       (this UnityEngine.GameObject gameObject, uint        index)              => gameObject.FindChild(child => 0u   == index--);
      public static UnityEngine.GameObject? FindChildByTag         (this UnityEngine.GameObject gameObject, string      tag)                => gameObject.FindChild(child => tag  == child.tag);

    public static UnityEngine.GameObject[] FindChildren(this UnityEngine.GameObject gameObject, System.Predicate<UnityEngine.GameObject> predicate) {
      System.Collections.Generic.List<UnityEngine.GameObject> children = new();

      // …
      foreach (UnityEngine.Transform transform in gameObject.transform) {
        if (predicate(transform.gameObject))
        children.Add(transform.gameObject);
      }

      return children.ToArray();
    }
      public static T                     [] FindChildrenByComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => System.Array.ConvertAll(gameObject.FindChildren(child => null != child.GetComponent<T>()),  child => child.GetComponent<T>());
      public static UnityEngine.Component [] FindChildrenByComponent   (this UnityEngine.GameObject gameObject, System.Type type)               => System.Array.ConvertAll(gameObject.FindChildren(child => null != child.GetComponent(type)), child => child.GetComponent(type));
      public static UnityEngine.GameObject[] FindChildrenByTag         (this UnityEngine.GameObject gameObject, string      tag)                => gameObject.FindChildren(child => child.tag == tag);

    public static UnityEngine.GameObject? FindDescendant(this UnityEngine.GameObject gameObject, System.Predicate<UnityEngine.GameObject> predicate) {
      for (System.Collections.Generic.List<UnityEngine.GameObject> pending = new() {gameObject}; 0 != pending.Count; pending.RemoveAt(0))
      foreach (UnityEngine.Transform transform in pending[0].transform) {
        pending.Add(transform.gameObject);

        if (predicate(transform.gameObject))
        return transform.gameObject;
      }

      return null;
    }
      public static T?                      FindDescendantByComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject.FindDescendant(child => null != child.GetComponent<T>()) ?.GetComponent<T>();
      public static UnityEngine.Component?  FindDescendantByComponent   (this UnityEngine.GameObject gameObject, System.Type type)               => gameObject.FindDescendant(child => null != child.GetComponent(type))?.GetComponent(type);
      public static UnityEngine.GameObject? FindDescendantByTag         (this UnityEngine.GameObject gameObject, string      tag)                => gameObject.FindDescendant(child => tag  == child.tag);

    public static UnityEngine.GameObject[] FindDescendants(this UnityEngine.GameObject gameObject, System.Predicate<UnityEngine.GameObject> predicate) {
      System.Collections.Generic.List<UnityEngine.GameObject> descendants = new();

      // …
      for (System.Collections.Generic.List<UnityEngine.GameObject> pending = new() {gameObject}; 0 != pending.Count; pending.RemoveAt(0))
      descendants.AddRange(pending[0].FindChildren(descendant => { pending.Add(descendant); return predicate(descendant); }));

      return descendants.ToArray();
    }
      public static T                     [] FindDescendantsByComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => System.Array.ConvertAll(gameObject.FindDescendants(descendant => null != descendant.GetComponent<T>()),  descendant => descendant.GetComponent<T>());
      public static UnityEngine.Component [] FindDescendantsByComponent   (this UnityEngine.GameObject gameObject, System.Type type)               => System.Array.ConvertAll(gameObject.FindDescendants(descendant => null != descendant.GetComponent(type)), descendant => descendant.GetComponent(type));
      public static UnityEngine.GameObject[] FindDescendantsByTag         (this UnityEngine.GameObject gameObject, string      tag)                => gameObject.FindDescendants(descendant => descendant.tag == tag);

    public static UnityEngine.GameObject[] FindLineage(this UnityEngine.GameObject gameObject, System.Predicate<UnityEngine.GameObject> predicate) {
      return Util.ArrayFrom(predicate(gameObject) ? new[] {gameObject} : new UnityEngine.GameObject[0], gameObject.FindDescendants(predicate));
    }
      public static T                     [] FindLineageByComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => System.Array.ConvertAll(gameObject.FindLineage(successor => null != successor.GetComponent<T>()),  successor => successor.GetComponent<T>());
      public static UnityEngine.Component [] FindLineageByComponent   (this UnityEngine.GameObject gameObject, System.Type type)               => System.Array.ConvertAll(gameObject.FindLineage(successor => null != successor.GetComponent(type)), successor => successor.GetComponent(type));
      public static UnityEngine.GameObject[] FindLineageByTag         (this UnityEngine.GameObject gameObject, string      tag)                => gameObject.FindLineage(successor => successor.tag == tag);

    public static UnityEngine.GameObject[] GetChildren   (this UnityEngine.GameObject gameObject) => gameObject.FindChildren   (_ => true);
    public static UnityEngine.GameObject[] GetDescendants(this UnityEngine.GameObject gameObject) => gameObject.FindDescendants(_ => true);
    public static UnityEngine.GameObject[] GetLineage    (this UnityEngine.GameObject gameObject) => gameObject.FindLineage    (_ => true);
    public static UnityEngine.GameObject   GetParent     (this UnityEngine.GameObject gameObject) => gameObject.transform.parent.gameObject;

    public static bool HasChild     (this UnityEngine.GameObject gameObject, UnityEngine.GameObject child)      => null != gameObject.FindChild     (_ => _ == child);
    public static bool HasDescendant(this UnityEngine.GameObject gameObject, UnityEngine.GameObject descendant) => null != gameObject.FindDescendant(_ => _ == descendant);

    public static void Reset(this UnityEngine.Transform transform) {
      transform.localRotation = UnityEngine.Quaternion.identity;
      transform.localScale    = UnityEngine.Vector3   .one;
      transform.position      = UnityEngine.Vector3   .zero;
    }

    public static void SetHeight(this UnityEngine.RectTransform transform, float height) {
      UnityEngine.RectTransform? parentTransform = transform.parent?.transform as UnityEngine.RectTransform;
      transform.sizeDelta = new(transform.sizeDelta.x, height - (null != parentTransform ? parentTransform.rect.height * (transform.anchorMax.y - transform.anchorMin.y) : 0.0f));
    }

    public static void SetSize(this UnityEngine.RectTransform transform, UnityEngine.Vector2 size) {
      UnityEngine.RectTransform? parentTransform = transform.parent?.transform as UnityEngine.RectTransform;
      transform.sizeDelta = size - (null != parentTransform ? UnityEngine.Vector2.Scale(parentTransform.rect.size, transform.anchorMax - transform.anchorMin) : UnityEngine.Vector2.zero);
    }

    public static void SetWidth(this UnityEngine.RectTransform transform, float width) {
      UnityEngine.RectTransform? parentTransform = transform.parent?.transform as UnityEngine.RectTransform;
      transform.sizeDelta = new(width - (null != parentTransform ? parentTransform.rect.width * (transform.anchorMax.x - transform.anchorMin.x) : 0.0f), transform.sizeDelta.y);
    }

    #if true || !(NET5_0 || NET5_0_OR_GREATER || NET6_0 || NET6_0_OR_GREATER || NET7_0 || NET7_0_OR_GREATER || NET8_0 || NET8_0_OR_GREATER || NET9_0 || NETCOREAPP2_0 || NETCOREAPP2_0_OR_GREATER || NETCOREAPP2_1 || NETCOREAPP2_1_OR_GREATER || NETCOREAPP2_2 || NETCOREAPP2_2_OR_GREATER || NETCOREAPP3_0 || NETCOREAPP3_0_OR_GREATER || NETCOREAPP3_1 || NETCOREAPP3_1_OR_GREATER || NETSTANDARD2_1 || NETSTANDARD2_1_OR_GREATER)
      public static bool TryAdd<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary, TKey key, TValue value) {
        if (!dictionary.ContainsKey(key)) {
          dictionary.Add(key, value);
          return true;
        }

        return false;
      }
    #endif
  }

  public static class Util /* → Utilities */ {
    private sealed class Load {
      public object?                                                 data     = null;
      public System.Collections.Generic.List<System.Action<object?>> handlers = new();
      public bool                                                    pending  = false;
    }

    private sealed class Wait {
      public System.Action callback  = null;
      public float         interval  = 0.0f;
      public float         timestamp = 0.0f;
    }

    /* … */
    private static readonly System.Collections.Generic.Dictionary<string, Util.Load> LOADED = new();
    private static readonly System.Collections.Generic.List<Util.Wait>               WAITS  = new();

    public const           float  LoadAsynchronously = 0.0f;  // → `LoadURI*(…, float? loadDurationMaximum, …)`
    public const           bool   LoadCached         = false; // → `LoadURI*(…, bool uncached)`
    public const           bool   LoadDirectly       = true;  // → `LoadURI*(…, bool uncached)`
    public static readonly float? LoadSynchronously  = null;  // → `LoadURI*(…, float? loadDurationMaximum, …)`

    /* … */
    public static object[] ArrayFrom() {
      return new object[0];
    }

    public static T[] ArrayFrom<T>() {
      return new T[0];
    }

    public static T[] ArrayFrom<T>(params System.Collections.Generic.IEnumerable<T>[] enumerables) {
      System.Collections.Generic.List<T> concatenation = new();

      // …
      foreach (System.Collections.Generic.IEnumerable<T> enumerable in enumerables) {
        if (null != enumerable)
        concatenation.AddRange(enumerable);
      }

      return concatenation.ToArray();
    }

    public static T[] ArrayFromMembers<T>(object structure) where T : class? {
      if (null == structure)
      return null as T?[];

      T[]    array     = null;
      uint   length    = 0u;
      T?[][] sequences = System.Array.ConvertAll(structure.GetType().GetMembers(), member => {
        System.Type? memberType  = null;
        object?      memberValue = member.MemberType switch {
          System.Reflection.MemberTypes.Field    => new System.Func<object?>(() => (member as System.Reflection.FieldInfo)   .GetValue(structure))(),
          System.Reflection.MemberTypes.Property => new System.Func<object?>(() => (member as System.Reflection.PropertyInfo).GetValue(structure))(),
          _                                      => null
        };

        System.Reflection.MethodInfo? GetConversionOperator(System.Type type) => System.Array.Find(
          memberType?.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static) ?? new System.Reflection.MethodInfo[] {},
          method =>
            method                                               .Name          == "op_Implicit" &&
            method                                               .ReturnType    == type          &&
            System.Array.Find(method.GetParameters(), _ => true)?.ParameterType == memberType
        );

        // …
        memberType = memberValue?.GetType();

        if (Util.IsConvertibleType(memberType, typeof(T)))                                 return new[] {memberValue as T    ?? GetConversionOperator(typeof(T))   ?.Invoke(null, new[] {memberValue}) as T};
        if (Util.IsConvertibleType(memberType, typeof(T[])) && null != memberValue as T[]) return        memberValue as T[]  ?? GetConversionOperator(typeof(T[])) ?.Invoke(null, new[] {memberValue}) as T[];
        if (Util.IsConvertibleType(memberType, typeof(T?[])))                              return        memberValue as T?[] ?? GetConversionOperator(typeof(T?[]))?.Invoke(null, new[] {memberValue}) as T?[];

        return null as T?[];
      });

      // …
      foreach (T?[] sequence in sequences) {
        foreach (T? sequenced in sequence ?? new T?[] {null})
        length += null != sequenced ? 1u : 0u;
      }

      if (0u != length) {
        array  = new T?[length];
        length = 0u;

        foreach (T?[] sequence  in sequences)
        foreach (T?   sequenced in sequence ?? new T?[] {null}) {
          if (null != sequenced)
          array[length++] = sequenced;
        }
      }

      return array;
    }

    public static UnityEngine.Vector3[] CornersFromRect(UnityEngine.Rect rectangle) {
      // → Origin begins from bottom-left rather than top-left
      return new UnityEngine.Vector3[4] {
        new(rectangle.xMin, rectangle.yMax, 0.0f),
        new(rectangle.xMin, rectangle.yMin, 0.0f),
        new(rectangle.xMax, rectangle.yMin, 0.0f),
        new(rectangle.xMax, rectangle.yMax, 0.0f)
      };
    }

    public static UnityEngine.Vector3[]? CornersFromRectTransform(System.Action<UnityEngine.Vector3[]>? transformMethod) {
      UnityEngine.Vector3[] corners = null;

      // …
      if (transformMethod?.Target is UnityEngine.RectTransform)
      transformMethod(corners = new UnityEngine.Vector3[4]);

      return corners;
    }

    public static System.Delegate DelegateConvert(System.Type typeA, System.Type typeB) {
      System.Linq.Expressions.ParameterExpression expression = System.Linq.Expressions.Expression.Parameter(typeA);
      return System.Linq.Expressions.Expression.Lambda(System.Linq.Expressions.Expression.Convert(expression, typeB), expression).Compile();
    }

    public static System.Predicate<T> DelegateEquals<T>(T value) where T : System.IEquatable<T> {
      return subvalue => value.Equals(subvalue);
    }

    public static string GetAssetPath() {
      return Util.NormalizeURI(UnityEngine.Application.streamingAssetsPath);
    }

    public static System.Collections.Generic.Dictionary<System.Type, System.Type[]> GetConvertibleImplicitTypes() {
      return new() {
        {typeof(byte),   new[] {typeof(decimal), typeof(double), typeof(float), typeof(int), typeof(long), typeof(nint), typeof(nuint), typeof(short), typeof(uint), typeof(ulong), typeof(ushort)}},
        {typeof(float),  new[] {typeof(double)}},
        {typeof(int),    new[] {typeof(decimal), typeof(double), typeof(float), typeof(long), typeof(nint)}},
        {typeof(long),   new[] {typeof(decimal), typeof(double), typeof(float)}},
        {typeof(nint),   new[] {typeof(decimal), typeof(double), typeof(float), typeof(long)}},
        {typeof(nuint),  new[] {typeof(decimal), typeof(double), typeof(float), typeof(ulong)}},
        {typeof(sbyte),  new[] {typeof(decimal), typeof(double), typeof(float), typeof(int), typeof(long), typeof(nint), typeof(short)}},
        {typeof(short),  new[] {typeof(decimal), typeof(double), typeof(float), typeof(int), typeof(long), typeof(nint)}},
        {typeof(uint),   new[] {typeof(decimal), typeof(double), typeof(float), typeof(long), typeof(nuint), typeof(ulong)}},
        {typeof(ulong),  new[] {typeof(decimal), typeof(double), typeof(float)}},
        {typeof(ushort), new[] {typeof(decimal), typeof(double), typeof(float), typeof(int), typeof(long), typeof(nint), typeof(nuint), typeof(uint), typeof(ulong)}}
      };
    }

    public static string GetDataPath() {
      return Util.NormalizeURI(UnityEngine.Application.persistentDataPath);
    }

    public static UnityEngine.KeyCode[] GetKeyCodes    () => new[] {UnityEngine.KeyCode.A, UnityEngine.KeyCode.Alpha0, UnityEngine.KeyCode.Alpha1, UnityEngine.KeyCode.Alpha2, UnityEngine.KeyCode.Alpha3, UnityEngine.KeyCode.Alpha4, UnityEngine.KeyCode.Alpha5, UnityEngine.KeyCode.Alpha6, UnityEngine.KeyCode.Alpha7, UnityEngine.KeyCode.Alpha8, UnityEngine.KeyCode.Alpha9, UnityEngine.KeyCode.AltGr, UnityEngine.KeyCode.Ampersand, UnityEngine.KeyCode.Asterisk, UnityEngine.KeyCode.At, UnityEngine.KeyCode.B, UnityEngine.KeyCode.BackQuote, UnityEngine.KeyCode.Backslash, UnityEngine.KeyCode.Backspace, UnityEngine.KeyCode.Break, UnityEngine.KeyCode.C, UnityEngine.KeyCode.CapsLock, UnityEngine.KeyCode.Caret, UnityEngine.KeyCode.Clear, UnityEngine.KeyCode.Colon, UnityEngine.KeyCode.Comma, UnityEngine.KeyCode.D, UnityEngine.KeyCode.Delete, UnityEngine.KeyCode.Dollar, UnityEngine.KeyCode.DoubleQuote, UnityEngine.KeyCode.DownArrow, UnityEngine.KeyCode.E, UnityEngine.KeyCode.End, UnityEngine.KeyCode.Equals, UnityEngine.KeyCode.Escape, UnityEngine.KeyCode.Exclaim, UnityEngine.KeyCode.F, UnityEngine.KeyCode.F1, UnityEngine.KeyCode.F10, UnityEngine.KeyCode.F11, UnityEngine.KeyCode.F12, UnityEngine.KeyCode.F13, UnityEngine.KeyCode.F14, UnityEngine.KeyCode.F15, UnityEngine.KeyCode.F2, UnityEngine.KeyCode.F3, UnityEngine.KeyCode.F4, UnityEngine.KeyCode.F5, UnityEngine.KeyCode.F6, UnityEngine.KeyCode.F7, UnityEngine.KeyCode.F8, UnityEngine.KeyCode.F9, UnityEngine.KeyCode.G, UnityEngine.KeyCode.Greater, UnityEngine.KeyCode.H, UnityEngine.KeyCode.Hash, UnityEngine.KeyCode.Help, UnityEngine.KeyCode.Home, UnityEngine.KeyCode.I, UnityEngine.KeyCode.Insert, UnityEngine.KeyCode.J, UnityEngine.KeyCode.K, UnityEngine.KeyCode.Keypad0, UnityEngine.KeyCode.Keypad1, UnityEngine.KeyCode.Keypad2, UnityEngine.KeyCode.Keypad3, UnityEngine.KeyCode.Keypad4, UnityEngine.KeyCode.Keypad5, UnityEngine.KeyCode.Keypad6, UnityEngine.KeyCode.Keypad7, UnityEngine.KeyCode.Keypad8, UnityEngine.KeyCode.Keypad9, UnityEngine.KeyCode.KeypadDivide, UnityEngine.KeyCode.KeypadEnter, UnityEngine.KeyCode.KeypadEquals, UnityEngine.KeyCode.KeypadMinus, UnityEngine.KeyCode.KeypadMultiply, UnityEngine.KeyCode.KeypadPeriod, UnityEngine.KeyCode.KeypadPlus, UnityEngine.KeyCode.L, UnityEngine.KeyCode.LeftAlt, UnityEngine.KeyCode.LeftApple, UnityEngine.KeyCode.LeftArrow, UnityEngine.KeyCode.LeftBracket, UnityEngine.KeyCode.LeftCommand, UnityEngine.KeyCode.LeftControl, UnityEngine.KeyCode.LeftCurlyBracket, UnityEngine.KeyCode.LeftMeta, UnityEngine.KeyCode.LeftParen, UnityEngine.KeyCode.LeftShift, UnityEngine.KeyCode.LeftWindows, UnityEngine.KeyCode.Less, UnityEngine.KeyCode.M, UnityEngine.KeyCode.Menu, UnityEngine.KeyCode.Minus, UnityEngine.KeyCode.N, UnityEngine.KeyCode.Numlock, UnityEngine.KeyCode.O, UnityEngine.KeyCode.P, UnityEngine.KeyCode.PageDown, UnityEngine.KeyCode.PageUp, UnityEngine.KeyCode.Pause, UnityEngine.KeyCode.Percent, UnityEngine.KeyCode.Period, UnityEngine.KeyCode.Pipe, UnityEngine.KeyCode.Plus, UnityEngine.KeyCode.Print, UnityEngine.KeyCode.Q, UnityEngine.KeyCode.Question, UnityEngine.KeyCode.Quote, UnityEngine.KeyCode.R, UnityEngine.KeyCode.Return, UnityEngine.KeyCode.RightAlt, UnityEngine.KeyCode.RightApple, UnityEngine.KeyCode.RightArrow, UnityEngine.KeyCode.RightBracket, UnityEngine.KeyCode.RightCommand, UnityEngine.KeyCode.RightControl, UnityEngine.KeyCode.RightCurlyBracket, UnityEngine.KeyCode.RightMeta, UnityEngine.KeyCode.RightParen, UnityEngine.KeyCode.RightShift, UnityEngine.KeyCode.RightWindows, UnityEngine.KeyCode.S, UnityEngine.KeyCode.ScrollLock, UnityEngine.KeyCode.Semicolon, UnityEngine.KeyCode.Slash, UnityEngine.KeyCode.Space, UnityEngine.KeyCode.SysReq, UnityEngine.KeyCode.T, UnityEngine.KeyCode.Tab, UnityEngine.KeyCode.Tilde, UnityEngine.KeyCode.U, UnityEngine.KeyCode.Underscore, UnityEngine.KeyCode.UpArrow, UnityEngine.KeyCode.V, UnityEngine.KeyCode.W, UnityEngine.KeyCode.X, UnityEngine.KeyCode.Y, UnityEngine.KeyCode.Z};
    public static int                [] GetMouseButtons() => new[] {0x0, 0x1, 0x2};

    public static UnityEngine.Vector2    GetVectorAxes(UnityEngine.Vector2    vector, UnityEngine.Vector2    axes) { return new(0.0f != axes.x ? vector.x : 0.0f, 0.0f != axes.y ? vector.y : 0.0f); }
    public static UnityEngine.Vector2Int GetVectorAxes(UnityEngine.Vector2Int vector, UnityEngine.Vector2Int axes) { return new   (0 != axes.x ? vector.x : 0,       0 != axes.y ? vector.y : 0); }
    public static UnityEngine.Vector3    GetVectorAxes(UnityEngine.Vector3    vector, UnityEngine.Vector3    axes) { return new(0.0f != axes.x ? vector.x : 0.0f, 0.0f != axes.y ? vector.y : 0.0f, 0.0f != axes.z ? vector.z : 0.0f); }
    public static UnityEngine.Vector3Int GetVectorAxes(UnityEngine.Vector3Int vector, UnityEngine.Vector3Int axes) { return new   (0 != axes.x ? vector.x : 0,       0 != axes.y ? vector.y : 0,       0 != axes.z ? vector.z : 0); }
    public static UnityEngine.Vector4    GetVectorAxes(UnityEngine.Vector4    vector, UnityEngine.Vector4    axes) { return new(0.0f != axes.x ? vector.x : 0.0f, 0.0f != axes.y ? vector.y : 0.0f, 0.0f != axes.z ? vector.z : 0.0f, 0.0f != axes.w ? vector.w : 0.0f); }
      public static UnityEngine.Vector2    GetVectorAxes(UnityEngine.Vector2    vector, UnityEngine.Vector2Int axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector2   ((float) axes.x, (float) axes.y)); }
      public static UnityEngine.Vector2    GetVectorAxes(UnityEngine.Vector2    vector, UnityEngine.Vector3    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector2   ((float) axes.x, (float) axes.y)); }
      public static UnityEngine.Vector2    GetVectorAxes(UnityEngine.Vector2    vector, UnityEngine.Vector3Int axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector2   ((float) axes.x, (float) axes.y)); }
      public static UnityEngine.Vector2    GetVectorAxes(UnityEngine.Vector2    vector, UnityEngine.Vector4    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector2   ((float) axes.x, (float) axes.y)); }
      public static UnityEngine.Vector2Int GetVectorAxes(UnityEngine.Vector2Int vector, UnityEngine.Vector2    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector2Int((int)   axes.x, (int)   axes.y)); }
      public static UnityEngine.Vector2Int GetVectorAxes(UnityEngine.Vector2Int vector, UnityEngine.Vector3    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector2Int((int)   axes.x, (int)   axes.y)); }
      public static UnityEngine.Vector2Int GetVectorAxes(UnityEngine.Vector2Int vector, UnityEngine.Vector3Int axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector2Int((int)   axes.x, (int)   axes.y)); }
      public static UnityEngine.Vector2Int GetVectorAxes(UnityEngine.Vector2Int vector, UnityEngine.Vector4    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector2Int((int)   axes.x, (int)   axes.y)); }
      public static UnityEngine.Vector3    GetVectorAxes(UnityEngine.Vector3    vector, UnityEngine.Vector2    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3   ((float) axes.x, (float) axes.y, (float) 0.0f)); }
      public static UnityEngine.Vector3    GetVectorAxes(UnityEngine.Vector3    vector, UnityEngine.Vector2Int axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3   ((float) axes.x, (float) axes.y, (float) 0.0f)); }
      public static UnityEngine.Vector3    GetVectorAxes(UnityEngine.Vector3    vector, UnityEngine.Vector3Int axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3   ((float) axes.x, (float) axes.y, (float) axes.z)); }
      public static UnityEngine.Vector3    GetVectorAxes(UnityEngine.Vector3    vector, UnityEngine.Vector4    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3   ((float) axes.x, (float) axes.y, (float) axes.z)); }
      public static UnityEngine.Vector3Int GetVectorAxes(UnityEngine.Vector3Int vector, UnityEngine.Vector2    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3Int((int)   axes.x, (int)   axes.y, (int)   0)); }
      public static UnityEngine.Vector3Int GetVectorAxes(UnityEngine.Vector3Int vector, UnityEngine.Vector2Int axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3Int((int)   axes.x, (int)   axes.y, (int)   0)); }
      public static UnityEngine.Vector3Int GetVectorAxes(UnityEngine.Vector3Int vector, UnityEngine.Vector3    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3Int((int)   axes.x, (int)   axes.y, (int)   axes.z)); }
      public static UnityEngine.Vector3Int GetVectorAxes(UnityEngine.Vector3Int vector, UnityEngine.Vector4    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3Int((int)   axes.x, (int)   axes.y, (int)   axes.z)); }
      public static UnityEngine.Vector4    GetVectorAxes(UnityEngine.Vector4    vector, UnityEngine.Vector2    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector4   ((float) axes.x, (float) axes.y, (float) 0.0f,   (float) 0.0f)); }
      public static UnityEngine.Vector4    GetVectorAxes(UnityEngine.Vector4    vector, UnityEngine.Vector2Int axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector4   ((float) axes.x, (float) axes.y, (float) 0.0f,   (float) 0.0f)); }
      public static UnityEngine.Vector4    GetVectorAxes(UnityEngine.Vector4    vector, UnityEngine.Vector3    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector4   ((float) axes.x, (float) axes.y, (float) axes.z, (float) 0.0f)); }
      public static UnityEngine.Vector4    GetVectorAxes(UnityEngine.Vector4    vector, UnityEngine.Vector3Int axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector4   ((float) axes.x, (float) axes.y, (float) axes.z, (float) 0.0f)); }

    public static float GetVectorAxis(UnityEngine.Vector4 vector, UnityEngine.Vector4 axis) {
      if (0.0f != axis.x) return vector.x;
      if (0.0f != axis.y) return vector.y;
      if (0.0f != axis.z) return vector.z;
      if (0.0f != axis.w) return vector.w;

      return float.NaN;
    }
      public static float GetVectorAxis(UnityEngine.Vector2    vector, UnityEngine.Vector2    axis) { return (float) Util.GetVectorAxis(new UnityEngine.Vector4((float) vector.x, (float) vector.y, (float) 0.0f,     (float) 0.0f), new UnityEngine.Vector4((float) axis.x, (float) axis.y, (float) 0.0f,   (float) 0.0f)); }
      public static int   GetVectorAxis(UnityEngine.Vector2Int vector, UnityEngine.Vector2Int axis) { return (int)   Util.GetVectorAxis(new UnityEngine.Vector4((int)   vector.x, (int)   vector.y, (int)   0.0f,     (int)   0.0f), new UnityEngine.Vector4((int)   axis.x, (int)   axis.y, (int)   0.0f,   (int)   0.0f)); }
      public static float GetVectorAxis(UnityEngine.Vector3    vector, UnityEngine.Vector3    axis) { return (float) Util.GetVectorAxis(new UnityEngine.Vector4((float) vector.x, (float) vector.y, (float) vector.z, (float) 0.0f), new UnityEngine.Vector4((float) axis.x, (float) axis.y, (float) axis.z, (float) 0.0f)); }
      public static int   GetVectorAxis(UnityEngine.Vector3Int vector, UnityEngine.Vector3Int axis) { return (int)   Util.GetVectorAxis(new UnityEngine.Vector4((int)   vector.x, (int)   vector.y, (int)   vector.z, (int)   0.0f), new UnityEngine.Vector4((int)   axis.x, (int)   axis.y, (int)   axis.z, (int)   0.0f)); }

      public static float GetVectorAxis(UnityEngine.Vector2    vector, UnityEngine.Vector2Int axis) { return (float) Util.GetVectorAxis(vector, new UnityEngine.Vector2   ((float) axis.x, (float) axis.y)); }
      public static float GetVectorAxis(UnityEngine.Vector2    vector, UnityEngine.Vector3    axis) { return (float) Util.GetVectorAxis(vector, new UnityEngine.Vector2   ((float) axis.x, (float) axis.y)); }
      public static float GetVectorAxis(UnityEngine.Vector2    vector, UnityEngine.Vector3Int axis) { return (float) Util.GetVectorAxis(vector, new UnityEngine.Vector2   ((float) axis.x, (float) axis.y)); }
      public static float GetVectorAxis(UnityEngine.Vector2    vector, UnityEngine.Vector4    axis) { return (float) Util.GetVectorAxis(vector, new UnityEngine.Vector2   ((float) axis.x, (float) axis.y)); }
      public static int   GetVectorAxis(UnityEngine.Vector2Int vector, UnityEngine.Vector2    axis) { return (int)   Util.GetVectorAxis(vector, new UnityEngine.Vector2Int((int)   axis.x, (int)   axis.y)); }
      public static int   GetVectorAxis(UnityEngine.Vector2Int vector, UnityEngine.Vector3    axis) { return (int)   Util.GetVectorAxis(vector, new UnityEngine.Vector2Int((int)   axis.x, (int)   axis.y)); }
      public static int   GetVectorAxis(UnityEngine.Vector2Int vector, UnityEngine.Vector3Int axis) { return (int)   Util.GetVectorAxis(vector, new UnityEngine.Vector2Int((int)   axis.x, (int)   axis.y)); }
      public static int   GetVectorAxis(UnityEngine.Vector2Int vector, UnityEngine.Vector4    axis) { return (int)   Util.GetVectorAxis(vector, new UnityEngine.Vector2Int((int)   axis.x, (int)   axis.y)); }
      public static float GetVectorAxis(UnityEngine.Vector3    vector, UnityEngine.Vector2    axis) { return (float) Util.GetVectorAxis(vector, new UnityEngine.Vector3   ((float) axis.x, (float) axis.y, (float) 0.0f)); }
      public static float GetVectorAxis(UnityEngine.Vector3    vector, UnityEngine.Vector2Int axis) { return (float) Util.GetVectorAxis(vector, new UnityEngine.Vector3   ((float) axis.x, (float) axis.y, (float) 0.0f)); }
      public static float GetVectorAxis(UnityEngine.Vector3    vector, UnityEngine.Vector3Int axis) { return (float) Util.GetVectorAxis(vector, new UnityEngine.Vector3   ((float) axis.x, (float) axis.y, (float) axis.z)); }
      public static float GetVectorAxis(UnityEngine.Vector3    vector, UnityEngine.Vector4    axis) { return (float) Util.GetVectorAxis(vector, new UnityEngine.Vector3   ((float) axis.x, (float) axis.y, (float) axis.z)); }
      public static int   GetVectorAxis(UnityEngine.Vector3Int vector, UnityEngine.Vector2    axis) { return (int)   Util.GetVectorAxis(vector, new UnityEngine.Vector3Int((int)   axis.x, (int)   axis.y, (int)   0.0f)); }
      public static int   GetVectorAxis(UnityEngine.Vector3Int vector, UnityEngine.Vector2Int axis) { return (int)   Util.GetVectorAxis(vector, new UnityEngine.Vector3Int((int)   axis.x, (int)   axis.y, (int)   0.0f)); }
      public static int   GetVectorAxis(UnityEngine.Vector3Int vector, UnityEngine.Vector3    axis) { return (int)   Util.GetVectorAxis(vector, new UnityEngine.Vector3Int((int)   axis.x, (int)   axis.y, (int)   axis.z)); }
      public static int   GetVectorAxis(UnityEngine.Vector3Int vector, UnityEngine.Vector4    axis) { return (int)   Util.GetVectorAxis(vector, new UnityEngine.Vector3Int((int)   axis.x, (int)   axis.y, (int)   axis.z)); }
      public static float GetVectorAxis(UnityEngine.Vector4    vector, UnityEngine.Vector2    axis) { return (float) Util.GetVectorAxis(vector, new UnityEngine.Vector4   ((float) axis.x, (float) axis.y, (float) 0.0f,   (float) 0.0f)); }
      public static float GetVectorAxis(UnityEngine.Vector4    vector, UnityEngine.Vector2Int axis) { return (float) Util.GetVectorAxis(vector, new UnityEngine.Vector4   ((float) axis.x, (float) axis.y, (float) 0.0f,   (float) 0.0f)); }
      public static float GetVectorAxis(UnityEngine.Vector4    vector, UnityEngine.Vector3    axis) { return (float) Util.GetVectorAxis(vector, new UnityEngine.Vector4   ((float) axis.x, (float) axis.y, (float) axis.z, (float) 0.0f)); }
      public static float GetVectorAxis(UnityEngine.Vector4    vector, UnityEngine.Vector3Int axis) { return (float) Util.GetVectorAxis(vector, new UnityEngine.Vector4   ((float) axis.x, (float) axis.y, (float) axis.z, (float) 0.0f)); }

      public static UnityEngine.Vector3    GetVectorBackAxes   (UnityEngine.Vector3    vector) { return Util.GetVectorAxes(vector, UnityEngine.Vector3   .back); }
      public static UnityEngine.Vector3Int GetVectorBackAxes   (UnityEngine.Vector3Int vector) { return Util.GetVectorAxes(vector, UnityEngine.Vector3Int.back); }
      public static float                  GetVectorBackAxis   (UnityEngine.Vector3    vector) { return Util.GetVectorAxis(vector, UnityEngine.Vector3   .back); }
      public static int                    GetVectorBackAxis   (UnityEngine.Vector3Int vector) { return Util.GetVectorAxis(vector, UnityEngine.Vector3Int.back); }
      public static UnityEngine.Vector2    GetVectorDownAxes   (UnityEngine.Vector2    vector) { return Util.GetVectorAxes(vector, UnityEngine.Vector2   .down); }
      public static UnityEngine.Vector2Int GetVectorDownAxes   (UnityEngine.Vector2Int vector) { return Util.GetVectorAxes(vector, UnityEngine.Vector2Int.down); }
      public static UnityEngine.Vector3    GetVectorDownAxes   (UnityEngine.Vector3    vector) { return Util.GetVectorAxes(vector, UnityEngine.Vector3   .down); }
      public static UnityEngine.Vector3Int GetVectorDownAxes   (UnityEngine.Vector3Int vector) { return Util.GetVectorAxes(vector, UnityEngine.Vector3Int.down); }
      public static float                  GetVectorDownAxis   (UnityEngine.Vector2    vector) { return Util.GetVectorAxis(vector, UnityEngine.Vector2   .down); }
      public static float                  GetVectorDownAxis   (UnityEngine.Vector3    vector) { return Util.GetVectorAxis(vector, UnityEngine.Vector3   .down); }
      public static int                    GetVectorDownAxis   (UnityEngine.Vector2Int vector) { return Util.GetVectorAxis(vector, UnityEngine.Vector2Int.down); }
      public static int                    GetVectorDownAxis   (UnityEngine.Vector3Int vector) { return Util.GetVectorAxis(vector, UnityEngine.Vector3Int.down); }
      public static UnityEngine.Vector3    GetVectorForwardAxes(UnityEngine.Vector3    vector) { return Util.GetVectorAxes(vector, UnityEngine.Vector3   .forward); }
      public static UnityEngine.Vector3Int GetVectorForwardAxes(UnityEngine.Vector3Int vector) { return Util.GetVectorAxes(vector, UnityEngine.Vector3Int.forward); }
      public static float                  GetVectorForwardAxis(UnityEngine.Vector3    vector) { return Util.GetVectorAxis(vector, UnityEngine.Vector3   .forward); }
      public static int                    GetVectorForwardAxis(UnityEngine.Vector3Int vector) { return Util.GetVectorAxis(vector, UnityEngine.Vector3Int.forward); }
      public static UnityEngine.Vector2    GetVectorLeftAxes   (UnityEngine.Vector2    vector) { return Util.GetVectorAxes(vector, UnityEngine.Vector2   .left); }
      public static UnityEngine.Vector2Int GetVectorLeftAxes   (UnityEngine.Vector2Int vector) { return Util.GetVectorAxes(vector, UnityEngine.Vector2Int.left); }
      public static UnityEngine.Vector3    GetVectorLeftAxes   (UnityEngine.Vector3    vector) { return Util.GetVectorAxes(vector, UnityEngine.Vector3   .left); }
      public static UnityEngine.Vector3Int GetVectorLeftAxes   (UnityEngine.Vector3Int vector) { return Util.GetVectorAxes(vector, UnityEngine.Vector3Int.left); }
      public static float                  GetVectorLeftAxis   (UnityEngine.Vector2    vector) { return Util.GetVectorAxis(vector, UnityEngine.Vector2   .left); }
      public static float                  GetVectorLeftAxis   (UnityEngine.Vector3    vector) { return Util.GetVectorAxis(vector, UnityEngine.Vector3   .left); }
      public static int                    GetVectorLeftAxis   (UnityEngine.Vector2Int vector) { return Util.GetVectorAxis(vector, UnityEngine.Vector2Int.left); }
      public static int                    GetVectorLeftAxis   (UnityEngine.Vector3Int vector) { return Util.GetVectorAxis(vector, UnityEngine.Vector3Int.left); }
      public static UnityEngine.Vector2    GetVectorRightAxes  (UnityEngine.Vector2    vector) { return Util.GetVectorAxes(vector, UnityEngine.Vector2   .right); }
      public static UnityEngine.Vector2Int GetVectorRightAxes  (UnityEngine.Vector2Int vector) { return Util.GetVectorAxes(vector, UnityEngine.Vector2Int.right); }
      public static UnityEngine.Vector3    GetVectorRightAxes  (UnityEngine.Vector3    vector) { return Util.GetVectorAxes(vector, UnityEngine.Vector3   .right); }
      public static UnityEngine.Vector3Int GetVectorRightAxes  (UnityEngine.Vector3Int vector) { return Util.GetVectorAxes(vector, UnityEngine.Vector3Int.right); }
      public static float                  GetVectorRightAxis  (UnityEngine.Vector2    vector) { return Util.GetVectorAxis(vector, UnityEngine.Vector2   .right); }
      public static float                  GetVectorRightAxis  (UnityEngine.Vector3    vector) { return Util.GetVectorAxis(vector, UnityEngine.Vector3   .right); }
      public static int                    GetVectorRightAxis  (UnityEngine.Vector2Int vector) { return Util.GetVectorAxis(vector, UnityEngine.Vector2Int.right); }
      public static int                    GetVectorRightAxis  (UnityEngine.Vector3Int vector) { return Util.GetVectorAxis(vector, UnityEngine.Vector3Int.right); }
      public static UnityEngine.Vector2    GetVectorUpAxes     (UnityEngine.Vector2    vector) { return Util.GetVectorAxes(vector, UnityEngine.Vector2   .up); }
      public static UnityEngine.Vector2Int GetVectorUpAxes     (UnityEngine.Vector2Int vector) { return Util.GetVectorAxes(vector, UnityEngine.Vector2Int.up); }
      public static UnityEngine.Vector3    GetVectorUpAxes     (UnityEngine.Vector3    vector) { return Util.GetVectorAxes(vector, UnityEngine.Vector3   .up); }
      public static UnityEngine.Vector3Int GetVectorUpAxes     (UnityEngine.Vector3Int vector) { return Util.GetVectorAxes(vector, UnityEngine.Vector3Int.up); }
      public static float                  GetVectorUpAxis     (UnityEngine.Vector2    vector) { return Util.GetVectorAxis(vector, UnityEngine.Vector2   .up); }
      public static float                  GetVectorUpAxis     (UnityEngine.Vector3    vector) { return Util.GetVectorAxis(vector, UnityEngine.Vector3   .up); }
      public static int                    GetVectorUpAxis     (UnityEngine.Vector2Int vector) { return Util.GetVectorAxis(vector, UnityEngine.Vector2Int.up); }
      public static int                    GetVectorUpAxis     (UnityEngine.Vector3Int vector) { return Util.GetVectorAxis(vector, UnityEngine.Vector3Int.up); }

      public static UnityEngine.Vector3    GetVectorDepthAxes (UnityEngine.Vector3    vector) { return Util.GetVectorForwardAxes(vector); }
      public static UnityEngine.Vector3Int GetVectorDepthAxes (UnityEngine.Vector3Int vector) { return Util.GetVectorForwardAxes(vector); }
      public static float                  GetVectorDepthAxis (UnityEngine.Vector3    vector) { return Util.GetVectorForwardAxis(vector); }
      public static int                    GetVectorDepthAxis (UnityEngine.Vector3Int vector) { return Util.GetVectorForwardAxis(vector); }
      public static UnityEngine.Vector2    GetVectorHeightAxes(UnityEngine.Vector2    vector) { return Util.GetVectorUpAxes     (vector); }
      public static UnityEngine.Vector2Int GetVectorHeightAxes(UnityEngine.Vector2Int vector) { return Util.GetVectorUpAxes     (vector); }
      public static UnityEngine.Vector3    GetVectorHeightAxes(UnityEngine.Vector3    vector) { return Util.GetVectorUpAxes     (vector); }
      public static UnityEngine.Vector3Int GetVectorHeightAxes(UnityEngine.Vector3Int vector) { return Util.GetVectorUpAxes     (vector); }
      public static float                  GetVectorHeightAxis(UnityEngine.Vector2    vector) { return Util.GetVectorUpAxis     (vector); }
      public static float                  GetVectorHeightAxis(UnityEngine.Vector3    vector) { return Util.GetVectorUpAxis     (vector); }
      public static int                    GetVectorHeightAxis(UnityEngine.Vector2Int vector) { return Util.GetVectorUpAxis     (vector); }
      public static int                    GetVectorHeightAxis(UnityEngine.Vector3Int vector) { return Util.GetVectorUpAxis     (vector); }
      public static UnityEngine.Vector2    GetVectorWidthAxes (UnityEngine.Vector2    vector) { return Util.GetVectorRightAxes  (vector); }
      public static UnityEngine.Vector2Int GetVectorWidthAxes (UnityEngine.Vector2Int vector) { return Util.GetVectorRightAxes  (vector); }
      public static UnityEngine.Vector3    GetVectorWidthAxes (UnityEngine.Vector3    vector) { return Util.GetVectorRightAxes  (vector); }
      public static UnityEngine.Vector3Int GetVectorWidthAxes (UnityEngine.Vector3Int vector) { return Util.GetVectorRightAxes  (vector); }
      public static float                  GetVectorWidthAxis (UnityEngine.Vector2    vector) { return Util.GetVectorRightAxis  (vector); }
      public static float                  GetVectorWidthAxis (UnityEngine.Vector3    vector) { return Util.GetVectorRightAxis  (vector); }
      public static int                    GetVectorWidthAxis (UnityEngine.Vector2Int vector) { return Util.GetVectorRightAxis  (vector); }
      public static int                    GetVectorWidthAxis (UnityEngine.Vector3Int vector) { return Util.GetVectorRightAxis  (vector); }

      public static UnityEngine.Vector2    GetVectorXAxes(UnityEngine.Vector2    vector) { return Util.GetVectorAxes(vector, new UnityEngine.Vector2   ((float) 1.0f, (float) 0.0f)); }
      public static UnityEngine.Vector2Int GetVectorXAxes(UnityEngine.Vector2Int vector) { return Util.GetVectorAxes(vector, new UnityEngine.Vector2Int((int)   1,    (int)   0)); }
      public static UnityEngine.Vector3    GetVectorXAxes(UnityEngine.Vector3    vector) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3   ((float) 1.0f, (float) 0.0f, (float) 0.0f)); }
      public static UnityEngine.Vector3Int GetVectorXAxes(UnityEngine.Vector3Int vector) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3Int((int)   1,    (int)   0,    (int)   0)); }
      public static UnityEngine.Vector4    GetVectorXAxes(UnityEngine.Vector4    vector) { return Util.GetVectorAxes(vector, new UnityEngine.Vector4   ((float) 1.0f, (float) 0.0f, (float) 0.0f, (float) 0.0f)); }
      public static UnityEngine.Vector2    GetVectorYAxes(UnityEngine.Vector2    vector) { return Util.GetVectorAxes(vector, new UnityEngine.Vector2   ((float) 0.0f, (float) 1.0f)); }
      public static UnityEngine.Vector2Int GetVectorYAxes(UnityEngine.Vector2Int vector) { return Util.GetVectorAxes(vector, new UnityEngine.Vector2Int((int)   0,    (int)   1)); }
      public static UnityEngine.Vector3    GetVectorYAxes(UnityEngine.Vector3    vector) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3   ((float) 0.0f, (float) 1.0f, (float) 0.0f)); }
      public static UnityEngine.Vector3Int GetVectorYAxes(UnityEngine.Vector3Int vector) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3Int((int)   0,    (int)   1,    (int)   0)); }
      public static UnityEngine.Vector4    GetVectorYAxes(UnityEngine.Vector4    vector) { return Util.GetVectorAxes(vector, new UnityEngine.Vector4   ((float) 0.0f, (float) 1.0f, (float) 0.0f, (float) 0.0f)); }
      public static UnityEngine.Vector3    GetVectorZAxes(UnityEngine.Vector3    vector) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3   ((float) 0.0f, (float) 0.0f, (float) 1.0f)); }
      public static UnityEngine.Vector3Int GetVectorZAxes(UnityEngine.Vector3Int vector) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3Int((int)   0,    (int)   0,    (int)   1)); }
      public static UnityEngine.Vector4    GetVectorZAxes(UnityEngine.Vector4    vector) { return Util.GetVectorAxes(vector, new UnityEngine.Vector4   ((float) 0.0f, (float) 0.0f, (float) 1.0f, (float) 0.0f)); }
      public static UnityEngine.Vector4    GetVectorWAxes(UnityEngine.Vector4    vector) { return Util.GetVectorAxes(vector, new UnityEngine.Vector4   ((float) 0.0f, (float) 0.0f, (float) 0.0f, (float) 1.0f)); }

    public static System.Func<T, T> IIFE<T>(System.Action<T> function) {
      return _ => { function(_); return _; };
    }

    public static System.Func<T, T> IIFE<T>(System.Func<T, T> function) /* → Immediately-Invoked Function Expression */ {
      return function;
    }

    public static UnityEngine.Vector2    IgnoreVectorAxes(UnityEngine.Vector2    vector, UnityEngine.Vector2    axes) { return new(axes.x != 1.0f ? vector.x : 0.0f, axes.y != 1.0f ? vector.y : 0.0f); }
    public static UnityEngine.Vector2Int IgnoreVectorAxes(UnityEngine.Vector2Int vector, UnityEngine.Vector2Int axes) { return new(axes.x != 1    ? vector.x : 0,    axes.y != 1    ? vector.y : 0); }
    public static UnityEngine.Vector3    IgnoreVectorAxes(UnityEngine.Vector3    vector, UnityEngine.Vector3    axes) { return new(axes.x != 1.0f ? vector.x : 0.0f, axes.y != 1.0f ? vector.y : 0.0f, axes.z != 1.0f ? vector.z : 0.0f); }
    public static UnityEngine.Vector3Int IgnoreVectorAxes(UnityEngine.Vector3Int vector, UnityEngine.Vector3Int axes) { return new(axes.x != 1    ? vector.x : 0,    axes.y != 1    ? vector.y : 0,    axes.z != 1    ? vector.z : 0); }
    public static UnityEngine.Vector4    IgnoreVectorAxes(UnityEngine.Vector4    vector, UnityEngine.Vector4    axes) { return new(axes.x != 1.0f ? vector.x : 0.0f, axes.y != 1.0f ? vector.y : 0.0f, axes.z != 1.0f ? vector.z : 0.0f, axes.w != 1.0f ? vector.w : 0.0f); }
      public static UnityEngine.Vector2    IgnoreVectorAxes(UnityEngine.Vector2    vector, UnityEngine.Vector2Int axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector2   ((float) axes.x, (float) axes.y)); }
      public static UnityEngine.Vector2    IgnoreVectorAxes(UnityEngine.Vector2    vector, UnityEngine.Vector3    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector2   ((float) axes.x, (float) axes.y)); }
      public static UnityEngine.Vector2    IgnoreVectorAxes(UnityEngine.Vector2    vector, UnityEngine.Vector3Int axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector2   ((float) axes.x, (float) axes.y)); }
      public static UnityEngine.Vector2    IgnoreVectorAxes(UnityEngine.Vector2    vector, UnityEngine.Vector4    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector2   ((float) axes.x, (float) axes.y)); }
      public static UnityEngine.Vector2Int IgnoreVectorAxes(UnityEngine.Vector2Int vector, UnityEngine.Vector2    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector2Int((int)   axes.x, (int)   axes.y)); }
      public static UnityEngine.Vector2Int IgnoreVectorAxes(UnityEngine.Vector2Int vector, UnityEngine.Vector3    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector2Int((int)   axes.x, (int)   axes.y)); }
      public static UnityEngine.Vector2Int IgnoreVectorAxes(UnityEngine.Vector2Int vector, UnityEngine.Vector3Int axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector2Int((int)   axes.x, (int)   axes.y)); }
      public static UnityEngine.Vector2Int IgnoreVectorAxes(UnityEngine.Vector2Int vector, UnityEngine.Vector4    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector2Int((int)   axes.x, (int)   axes.y)); }
      public static UnityEngine.Vector3    IgnoreVectorAxes(UnityEngine.Vector3    vector, UnityEngine.Vector2    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3   ((float) axes.x, (float) axes.y, (float) 1.0f)); }
      public static UnityEngine.Vector3    IgnoreVectorAxes(UnityEngine.Vector3    vector, UnityEngine.Vector2Int axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3   ((float) axes.x, (float) axes.y, (float) 1.0f)); }
      public static UnityEngine.Vector3    IgnoreVectorAxes(UnityEngine.Vector3    vector, UnityEngine.Vector3Int axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3   ((float) axes.x, (float) axes.y, (float) axes.z)); }
      public static UnityEngine.Vector3    IgnoreVectorAxes(UnityEngine.Vector3    vector, UnityEngine.Vector4    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3   ((float) axes.x, (float) axes.y, (float) axes.z)); }
      public static UnityEngine.Vector3Int IgnoreVectorAxes(UnityEngine.Vector3Int vector, UnityEngine.Vector2    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3Int((int)   axes.x, (int)   axes.y, (int)   0)); }
      public static UnityEngine.Vector3Int IgnoreVectorAxes(UnityEngine.Vector3Int vector, UnityEngine.Vector2Int axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3Int((int)   axes.x, (int)   axes.y, (int)   0)); }
      public static UnityEngine.Vector3Int IgnoreVectorAxes(UnityEngine.Vector3Int vector, UnityEngine.Vector3    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3Int((int)   axes.x, (int)   axes.y, (int)   axes.z)); }
      public static UnityEngine.Vector3Int IgnoreVectorAxes(UnityEngine.Vector3Int vector, UnityEngine.Vector4    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector3Int((int)   axes.x, (int)   axes.y, (int)   axes.z)); }
      public static UnityEngine.Vector4    IgnoreVectorAxes(UnityEngine.Vector4    vector, UnityEngine.Vector2    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector4   ((float) axes.x, (float) axes.y, (float) 1.0f,   (float) 1.0f)); }
      public static UnityEngine.Vector4    IgnoreVectorAxes(UnityEngine.Vector4    vector, UnityEngine.Vector2Int axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector4   ((float) axes.x, (float) axes.y, (float) 1.0f,   (float) 1.0f)); }
      public static UnityEngine.Vector4    IgnoreVectorAxes(UnityEngine.Vector4    vector, UnityEngine.Vector3    axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector4   ((float) axes.x, (float) axes.y, (float) axes.z, (float) 1.0f)); }
      public static UnityEngine.Vector4    IgnoreVectorAxes(UnityEngine.Vector4    vector, UnityEngine.Vector3Int axes) { return Util.GetVectorAxes(vector, new UnityEngine.Vector4   ((float) axes.x, (float) axes.y, (float) axes.z, (float) 1.0f)); }

      public static UnityEngine.Vector3    IgnoreVectorBackAxes   (UnityEngine.Vector3    vector) { return Util.IgnoreVectorAxes(vector, UnityEngine.Vector3   .back); }
      public static UnityEngine.Vector3Int IgnoreVectorBackAxes   (UnityEngine.Vector3Int vector) { return Util.IgnoreVectorAxes(vector, UnityEngine.Vector3Int.back); }
      public static UnityEngine.Vector2    IgnoreVectorDownAxes   (UnityEngine.Vector2    vector) { return Util.IgnoreVectorAxes(vector, UnityEngine.Vector2   .down); }
      public static UnityEngine.Vector2Int IgnoreVectorDownAxes   (UnityEngine.Vector2Int vector) { return Util.IgnoreVectorAxes(vector, UnityEngine.Vector2Int.down); }
      public static UnityEngine.Vector3    IgnoreVectorDownAxes   (UnityEngine.Vector3    vector) { return Util.IgnoreVectorAxes(vector, UnityEngine.Vector3   .down); }
      public static UnityEngine.Vector3Int IgnoreVectorDownAxes   (UnityEngine.Vector3Int vector) { return Util.IgnoreVectorAxes(vector, UnityEngine.Vector3Int.down); }
      public static UnityEngine.Vector3    IgnoreVectorForwardAxes(UnityEngine.Vector3    vector) { return Util.IgnoreVectorAxes(vector, UnityEngine.Vector3   .forward); }
      public static UnityEngine.Vector3Int IgnoreVectorForwardAxes(UnityEngine.Vector3Int vector) { return Util.IgnoreVectorAxes(vector, UnityEngine.Vector3Int.forward); }
      public static UnityEngine.Vector2    IgnoreVectorLeftAxes   (UnityEngine.Vector2    vector) { return Util.IgnoreVectorAxes(vector, UnityEngine.Vector2   .left); }
      public static UnityEngine.Vector2Int IgnoreVectorLeftAxes   (UnityEngine.Vector2Int vector) { return Util.IgnoreVectorAxes(vector, UnityEngine.Vector2Int.left); }
      public static UnityEngine.Vector3    IgnoreVectorLeftAxes   (UnityEngine.Vector3    vector) { return Util.IgnoreVectorAxes(vector, UnityEngine.Vector3   .left); }
      public static UnityEngine.Vector3Int IgnoreVectorLeftAxes   (UnityEngine.Vector3Int vector) { return Util.IgnoreVectorAxes(vector, UnityEngine.Vector3Int.left); }
      public static UnityEngine.Vector2    IgnoreVectorRightAxes  (UnityEngine.Vector2    vector) { return Util.IgnoreVectorAxes(vector, UnityEngine.Vector2   .right); }
      public static UnityEngine.Vector2Int IgnoreVectorRightAxes  (UnityEngine.Vector2Int vector) { return Util.IgnoreVectorAxes(vector, UnityEngine.Vector2Int.right); }
      public static UnityEngine.Vector3    IgnoreVectorRightAxes  (UnityEngine.Vector3    vector) { return Util.IgnoreVectorAxes(vector, UnityEngine.Vector3   .right); }
      public static UnityEngine.Vector3Int IgnoreVectorRightAxes  (UnityEngine.Vector3Int vector) { return Util.IgnoreVectorAxes(vector, UnityEngine.Vector3Int.right); }
      public static UnityEngine.Vector2    IgnoreVectorUpAxes     (UnityEngine.Vector2    vector) { return Util.IgnoreVectorAxes(vector, UnityEngine.Vector2   .up); }
      public static UnityEngine.Vector2Int IgnoreVectorUpAxes     (UnityEngine.Vector2Int vector) { return Util.IgnoreVectorAxes(vector, UnityEngine.Vector2Int.up); }
      public static UnityEngine.Vector3    IgnoreVectorUpAxes     (UnityEngine.Vector3    vector) { return Util.IgnoreVectorAxes(vector, UnityEngine.Vector3   .up); }
      public static UnityEngine.Vector3Int IgnoreVectorUpAxes     (UnityEngine.Vector3Int vector) { return Util.IgnoreVectorAxes(vector, UnityEngine.Vector3Int.up); }

      public static UnityEngine.Vector3    IgnoreVectorDepthAxes (UnityEngine.Vector3    vector) { return Util.IgnoreVectorForwardAxes(vector); }
      public static UnityEngine.Vector3Int IgnoreVectorDepthAxes (UnityEngine.Vector3Int vector) { return Util.IgnoreVectorForwardAxes(vector); }
      public static UnityEngine.Vector2    IgnoreVectorHeightAxes(UnityEngine.Vector2    vector) { return Util.IgnoreVectorUpAxes     (vector); }
      public static UnityEngine.Vector2Int IgnoreVectorHeightAxes(UnityEngine.Vector2Int vector) { return Util.IgnoreVectorUpAxes     (vector); }
      public static UnityEngine.Vector3    IgnoreVectorHeightAxes(UnityEngine.Vector3    vector) { return Util.IgnoreVectorUpAxes     (vector); }
      public static UnityEngine.Vector3Int IgnoreVectorHeightAxes(UnityEngine.Vector3Int vector) { return Util.IgnoreVectorUpAxes     (vector); }
      public static UnityEngine.Vector2    IgnoreVectorWidthAxes (UnityEngine.Vector2    vector) { return Util.IgnoreVectorRightAxes  (vector); }
      public static UnityEngine.Vector2Int IgnoreVectorWidthAxes (UnityEngine.Vector2Int vector) { return Util.IgnoreVectorRightAxes  (vector); }
      public static UnityEngine.Vector3    IgnoreVectorWidthAxes (UnityEngine.Vector3    vector) { return Util.IgnoreVectorRightAxes  (vector); }
      public static UnityEngine.Vector3Int IgnoreVectorWidthAxes (UnityEngine.Vector3Int vector) { return Util.IgnoreVectorRightAxes  (vector); }

      public static UnityEngine.Vector2    IgnoreVectorXAxes(UnityEngine.Vector2    vector) { return Util.IgnoreVectorAxes(vector, new UnityEngine.Vector2   ((float) 1.0f, (float) 0.0f)); }
      public static UnityEngine.Vector2Int IgnoreVectorXAxes(UnityEngine.Vector2Int vector) { return Util.IgnoreVectorAxes(vector, new UnityEngine.Vector2Int((int)   1,    (int)   0)); }
      public static UnityEngine.Vector3    IgnoreVectorXAxes(UnityEngine.Vector3    vector) { return Util.IgnoreVectorAxes(vector, new UnityEngine.Vector3   ((float) 1.0f, (float) 0.0f, (float) 0.0f)); }
      public static UnityEngine.Vector3Int IgnoreVectorXAxes(UnityEngine.Vector3Int vector) { return Util.IgnoreVectorAxes(vector, new UnityEngine.Vector3Int((int)   1,    (int)   0,    (int)   0)); }
      public static UnityEngine.Vector4    IgnoreVectorXAxes(UnityEngine.Vector4    vector) { return Util.IgnoreVectorAxes(vector, new UnityEngine.Vector4   ((float) 1.0f, (float) 0.0f, (float) 0.0f, (float) 0.0f)); }
      public static UnityEngine.Vector2    IgnoreVectorYAxes(UnityEngine.Vector2    vector) { return Util.IgnoreVectorAxes(vector, new UnityEngine.Vector2   ((float) 0.0f, (float) 1.0f)); }
      public static UnityEngine.Vector2Int IgnoreVectorYAxes(UnityEngine.Vector2Int vector) { return Util.IgnoreVectorAxes(vector, new UnityEngine.Vector2Int((int)   0,    (int)   1)); }
      public static UnityEngine.Vector3    IgnoreVectorYAxes(UnityEngine.Vector3    vector) { return Util.IgnoreVectorAxes(vector, new UnityEngine.Vector3   ((float) 0.0f, (float) 1.0f, (float) 0.0f)); }
      public static UnityEngine.Vector3Int IgnoreVectorYAxes(UnityEngine.Vector3Int vector) { return Util.IgnoreVectorAxes(vector, new UnityEngine.Vector3Int((int)   0,    (int)   1,    (int)   0)); }
      public static UnityEngine.Vector4    IgnoreVectorYAxes(UnityEngine.Vector4    vector) { return Util.IgnoreVectorAxes(vector, new UnityEngine.Vector4   ((float) 0.0f, (float) 1.0f, (float) 0.0f, (float) 0.0f)); }
      public static UnityEngine.Vector3    IgnoreVectorZAxes(UnityEngine.Vector3    vector) { return Util.IgnoreVectorAxes(vector, new UnityEngine.Vector3   ((float) 0.0f, (float) 0.0f, (float) 1.0f)); }
      public static UnityEngine.Vector3Int IgnoreVectorZAxes(UnityEngine.Vector3Int vector) { return Util.IgnoreVectorAxes(vector, new UnityEngine.Vector3Int((int)   0,    (int)   0,    (int)   1)); }
      public static UnityEngine.Vector4    IgnoreVectorZAxes(UnityEngine.Vector4    vector) { return Util.IgnoreVectorAxes(vector, new UnityEngine.Vector4   ((float) 0.0f, (float) 0.0f, (float) 1.0f, (float) 0.0f)); }
      public static UnityEngine.Vector4    IgnoreVectorWAxes(UnityEngine.Vector4    vector) { return Util.IgnoreVectorAxes(vector, new UnityEngine.Vector4   ((float) 0.0f, (float) 0.0f, (float) 0.0f, (float) 1.0f)); }

    public static bool IsConvertibleType(System.Type typeA, System.Type typeB) {
      if (null == typeA) return null == typeB;
      if (null == typeB) return null == typeA;

      for (System.Collections.Generic.List<System.Type> types = new() {typeB}; 0 != types.Count; types.RemoveAt(0)) {
        System.Type type = types[0];

        // …
        if (
          type               == typeA  ||
          type.IsAssignableFrom(typeA) ||
          System.Array.Exists(new[] {type, typeA}, _ => System.Array.Exists(_.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static), method => {
            System.Reflection.ParameterInfo? parameter = System.Array.Find(method.GetParameters(), _ => true);

            return method.Name == "op_Implicit"
              && (method    .ReturnType    == type  || Util.IsImplicitConvertibleType(typeA, type))
              && (parameter?.ParameterType == typeA || Util.IsImplicitConvertibleType(typeA, parameter?.ParameterType));
          }))
        ) return true;

        // … → Recurse `IsConvertibleType(…)` loop
        if (Util.GetConvertibleImplicitTypes().TryGetValue(typeA, out System.Type[] implicitTypes))
        if (System.Array.IndexOf(implicitTypes, typeB) != implicitTypes.GetLowerBound(0) - 1) {
          foreach (System.Type implicitType in implicitTypes) {
            if (!types.Contains(implicitType))
            types.Add(implicitType);
          }
        }

        foreach (var GetUnderlyingType in new System.Func<System.Type, System.Type>?[] {
          type.IsEnum ? System.Enum    .GetUnderlyingType : _ => null,
          true        ? System.Nullable.GetUnderlyingType : _ => null
        }) {
          System.Type underlyingType = GetUnderlyingType(type);

          if (!types.Contains(underlyingType) && null != underlyingType)
          types.Add(underlyingType);
        }
      }

      return false;
    }

    private static bool IsImplicitConvertibleType(System.Type typeA, System.Type typeB) {
      if (Util.GetConvertibleImplicitTypes().TryGetValue(typeA, out System.Type[] implicitTypes))
        return System.Array.IndexOf(implicitTypes, typeB) != implicitTypes.GetLowerBound(0) - 1;

      return false;
    }

    private static object LoadURI(
      string id, string path, System.Action<object?> callback, float? loadDurationMaximum, bool uncached,
      System.Func<object, object>                                  preparser, // → `Util.LoadURI(…)` cache hit on `LOADED` for desired URI data
      System.Func<string, UnityEngine.Networking.UnityWebRequest>  requester, // → Map `path` URI to preempted `UnityEngine.Networking.UnityWebRequest`
      System.Func<UnityEngine.Networking.UnityWebRequest, object?> parser     // → Map `UnityEngine.Networking.UnityWebRequest` result to desired URI data
    ) {
      Util.Load                                            load = new() {data = null, handlers = new(new[] {callback}), pending = false};
      UnityEngine.Networking.UnityWebRequestAsyncOperation operation;
      UnityEngine.Networking.UnityWebRequest               request; // → Able to access the `UnityEngine.Application.streamingAssetsPath` directory
      System.Diagnostics.Stopwatch                         stopwatch = new();

      // …
      object? HandleURIData(object? data) {
        for (int index = load.handlers.Count; 0 != index--; ) {
          System.Action<object?> callback = load.handlers[index];

          // …
          load.handlers.RemoveAt(index);
          callback              (data);
        }

        load.handlers.Capacity = 0;
        return data;
      }

      object? HandleURIRequest(UnityEngine.Networking.UnityWebRequest request) {
        object? data = parser(request);

        // …
        request.Dispose();

        load.data = null != data && !uncached ? data : load.data;
        return HandleURIData(data);
      }

      // …
      if (null == path)
      return null;

      if (LOADED.ContainsKey(id + path)) {
        load = LOADED[id + path];
        load.handlers.Add(callback);

        if (null != load.data && !uncached)
        return HandleURIData(preparser(load.data));
      } else LOADED.Add(id + path, load);

      if (load.pending && null != loadDurationMaximum)
      return null; // → Prevent spamming multiple `UnityEngine.Networking.UnityWebRequest`s

      stopwatch.Start();
      request      = requester(path);
      operation    = request.SendWebRequest();
      load.pending = true;

      while (UnityEngine.Networking.UnityWebRequest.Result.Success != request.result)
      switch (request.result) {
        case UnityEngine.Networking.UnityWebRequest.Result.ConnectionError    :
        case UnityEngine.Networking.UnityWebRequest.Result.DataProcessingError:
        case UnityEngine.Networking.UnityWebRequest.Result.ProtocolError      : {
          stopwatch.Stop   ();
          request  .Dispose();
          load.pending = false;
        } return null;

        case UnityEngine.Networking.UnityWebRequest.Result.InProgress: {
          if (stopwatch.Elapsed.TotalSeconds < (loadDurationMaximum ?? float.PositiveInfinity))
          continue;

          stopwatch.Stop();
          operation.completed += _ => {
            operation    = _ as UnityEngine.Networking.UnityWebRequestAsyncOperation;
            load.pending = false;

            if (request != operation.webRequest) request.Dispose();
            if (UnityEngine.Networking.UnityWebRequest.Result.Success != operation.webRequest.result) { operation.webRequest.Dispose(); return; }

            HandleURIRequest(operation.webRequest);
          };
        } return null;
      }

      stopwatch.Stop();
      load.pending = false;

      return HandleURIRequest(request);
    }

    public static byte[] LoadURI(string path, System.Action<byte[]?>? callback = null, float? loadDurationMaximum = null, bool uncached = Util.LoadCached) {
      return Util.LoadURI(
        typeof(byte[]).ToString(), path, _ => callback?.Invoke(_ as byte[]), loadDurationMaximum, uncached,
        predata => predata, // → `(predata as byte[]).Clone() as byte[]`
        path    => UnityEngine.Networking.UnityWebRequest.Get(path),
        request => request.downloadHandler.data
      ) as byte[];
    }

    public static UnityEngine.AudioClip? LoadURIAsAudioClip(string path, System.Action<UnityEngine.AudioClip?>? callback = null, float? loadDurationMaximum = null, bool uncached = Util.LoadCached, UnityEngine.AudioType? encoding = null) {
      return Util.LoadURI(
        typeof(UnityEngine.AudioClip).ToString(), path, _ => callback?.Invoke(_ as UnityEngine.AudioClip), loadDurationMaximum, uncached,
        predata => {
          // → WARN (Lapys): Consume less memory resources, please T_T
          #if false
            UnityEngine.AudioClip                preaudioClip     = predata as UnityEngine.AudioClip;
            Unity.Collections.NativeArray<float> preaudioClipData = new(preaudioClip.channels * preaudioClip.samples, Unity.Collections.Allocator.Temp, Unity.Collections.NativeArrayOptions.UninitializedMemory);
            UnityEngine.AudioClip                audioClip        = UnityEngine.AudioClip.Create("🎵 " + System.IO.Path.GetFileName(path), preaudioClip.samples, preaudioClip.channels, preaudioClip.frequency, false);

            // …
            if (!preaudioClip.GetData(preaudioClipData, 0)) return preaudioClip;
            if (!audioClip   .SetData(preaudioClipData, 0)) return preaudioClip;

            return audioClip;
          #endif
          return predata;
        },
        path    => UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip(path, encoding ?? UnityEngine.AudioType.MPEG),
        request => {
          UnityEngine.AudioClip? audioClip = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(request);

          // …
          if (null != audioClip)
          audioClip.name = null != request.url ? "🎵 " + System.IO.Path.GetFileName(request.url) : "🎵";

          return audioClip;
        }
      ) as UnityEngine.AudioClip;
    }

    public static string? LoadURIAsText(string path, System.Action<string?>? callback = null, float? loadDurationMaximum = null, bool uncached = Util.LoadCached, System.Text.Encoding? encoding = null) {
      return Util.LoadURI(
        typeof(string).ToString(), path, _ => callback?.Invoke(_ as string), loadDurationMaximum, uncached,
        predata => predata, // → `new string((predata as string).ToCharArray())`
        path    => UnityEngine.Networking.UnityWebRequest.Get(path),
        request => {
          string text = null != encoding ? null : request.downloadHandler.text; // → UTF-8

          // …
          try { text ??= encoding.GetString(request.downloadHandler.data); }
          catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.ArgumentNullException || exception is System.Text.DecoderFallbackException) {}

          return text;
        }
      ) as string;
    }

    public static UnityEngine.Texture2D? LoadURIAsTexture2D(string path, System.Action<UnityEngine.Texture2D?>? callback = null, float? loadDurationMaximum = null, bool uncached = Util.LoadCached) {
      return Util.LoadURI(
        typeof(UnityEngine.Texture2D).ToString(), path, _ => callback?.Invoke(_ as UnityEngine.Texture2D), loadDurationMaximum, uncached,
        predata => {
          // → WARN (Lapys): Consume less memory resources, please T_T
          #if false
            UnityEngine.Texture2D pretexture = predata as UnityEngine.Texture2D;
            UnityEngine.Texture2D texture    = new(pretexture.width, pretexture.height, pretexture.format, pretexture.mipmapCount, false);

            UnityEngine.Graphics.CopyTexture(pretexture, texture);
            return texture;
          #endif
          return predata;
        },
        path    => UnityEngine.Networking.UnityWebRequest.Get(path),
        request => {
          UnityEngine.Texture2D texture = new(2, 2, UnityEngine.TextureFormat.RGBA32, -1, false);

          texture.name = "🖼️ " + System.IO.Path.GetFileName(path);
          return UnityEngine.ImageConversion.LoadImage(texture, request.downloadHandler.data, true) ? texture : null;
        }
      ) as UnityEngine.Texture2D;
    }

    public static UnityEngine.Bounds? LocalBoundsFromRectTransform(UnityEngine.RectTransform? transform) {
      UnityEngine.Rect? rectangle = Util.LocalRectFromRectTransform(transform);
      return null == rectangle ? null : new(transform.position, new(rectangle?.width ?? 0.0f, rectangle?.height ?? 0.0f, 0.0f));
    }

    public static UnityEngine.Vector3[]? LocalCornersFromRectTransform(UnityEngine.RectTransform? transform) {
      return Util.CornersFromRectTransform(null == transform ? null : transform.GetLocalCorners);
    }

    public static UnityEngine.Rect? LocalRectFromRectTransform(UnityEngine.RectTransform? transform) {
      UnityEngine.Vector3[]? corners = Util.LocalCornersFromRectTransform(transform);
      return null == corners ? null : Util.RectFromCorners(corners);
    }

    public static void Loop(int begin, int end, System.Delegate callback, bool reversed = false) {
      System.Reflection.ParameterInfo[]? parameters = callback?.Method.GetParameters();
      System.Reflection.MethodInfo?      method     = callback?.GetType().GetMethod("Invoke");
      int                                direction  = System.Math.Sign(end - begin);
      object[]                           arguments  = new object[] {begin, System.Math.Abs(end - begin), begin, end};

      // …
      if (null == method || null == parameters) return;
      if (reversed) { (begin, end) = (end, begin); direction = -direction; }

      try { System.Array.Resize(ref arguments, parameters.Length); }
      catch (System.Exception) { throw new System.NotSupportedException("Cannot `Loop(…)` given specified callback; Too many parameters"); }

      for (int index = 0; index != parameters.Length; ++index)
      arguments[index] = Util.DelegateConvert(arguments[index].GetType(), parameters[index].ParameterType).DynamicInvoke(arguments[index]);

      for ((int index, System.Delegate Convert) = (begin, 0 != parameters.Length ? Util.DelegateConvert(typeof(int), parameters[0].ParameterType) : null); ; index += direction) {
        if (null != Convert)
          arguments[0] = Convert.DynamicInvoke(index);

        method.Invoke(callback, arguments);
        if (end == index) return;
      }
    }

    public static void Loop<T>(System.Collections.Generic.IEnumerable<T> enumerable, System.Delegate callback, bool reversed = false) {
      System.Reflection.ParameterInfo[]?      parameters = callback?.Method.GetParameters();
      System.Reflection.MethodInfo?           method     = callback?.GetType().GetMethod("Invoke");
      System.Collections.Generic.List<object> loopable   = new();
      int                                     index      = 0;
      object[]                                arguments  = new object[] {null, index};

      // …
      if (null == method || null == parameters) return;
      foreach (T value in enumerable) { loopable.Add(value); ++index; }
      if      (reversed)                loopable.Reverse();

      try { System.Array.Resize(ref arguments, parameters.Length); }
      catch (System.Exception) { throw new System.NotSupportedException("Cannot `Loop(…)` given specified callback; Too many parameters"); }

      loopable.Capacity = index;
      index             = reversed ? index - 1 : 0;

      foreach (object value in loopable) {
        if (parameters.Length > 0) arguments[0] = (T) value;
        if (parameters.Length > 1) arguments[1] = Util.DelegateConvert(typeof(int), parameters[1].ParameterType).DynamicInvoke(index);

        index += reversed ? -1 : +1;
        method.Invoke(callback, arguments);
      }
    }
      public static void Loop   (int count,                                            System.Delegate                   callback, bool reversed = false) { if (0 != count) Loop(System.Math.Sign(count), count, callback, reversed); }
      public static void Loop   (int count,                                            System.Action<int>                callback, bool reversed = false) => Loop(count,      (System.Delegate) callback, reversed);
      public static void Loop   (int count,                                            System.Action<int, int>           callback, bool reversed = false) => Loop(count,      (System.Delegate) callback, reversed);
      public static void Loop   (int count,                                            System.Action<int, int, int, int> callback, bool reversed = false) => Loop(count,      (System.Delegate) callback, reversed);
      public static void Loop   (int begin, int end,                                   System.Action<int>                callback, bool reversed = false) => Loop(begin, end, (System.Delegate) callback, reversed);
      public static void Loop   (int begin, int end,                                   System.Action<int, int>           callback, bool reversed = false) => Loop(begin, end, (System.Delegate) callback, reversed);
      public static void Loop   (int begin, int end,                                   System.Action<int, int, int, int> callback, bool reversed = false) => Loop(begin, end, (System.Delegate) callback, reversed);
      public static void Loop<T>(System.Collections.Generic.IEnumerable<T> enumerable, System.Action<T>                  callback, bool reversed = false) => Loop(enumerable, (System.Delegate) callback, reversed);
      public static void Loop<T>(System.Collections.Generic.IEnumerable<T> enumerable, System.Action<T, int>             callback, bool reversed = false) => Loop(enumerable, (System.Delegate) callback, reversed);

    public static T Max<T>(System.Collections.Generic.IEnumerable<T> enumerable) where T : System.IComparable<T> {
      T[] maximum = null;

      // …
      foreach (T value in enumerable) {
        if (null == maximum)                 maximum    = new[] {value};
        if (value.CompareTo(maximum[0]) > 0) maximum[0] = value;
      }

      return maximum[0]; // → `System.IndexOutOfRangeException`
    }
      public static T Max<T>(T valueA, T valueB) where T : System.IComparable<T> => valueA.CompareTo(valueB) > 0 ? valueA : valueB;
      public static T Max<T>(params T[] values)                                  => Util.Max(values);

    public static T Min<T>(System.Collections.Generic.IEnumerable<T> enumerable) where T : System.IComparable<T> {
      T[] minimum = null;

      // …
      foreach (T value in enumerable) {
        if (null == minimum)                 minimum    = new[] {value};
        if (value.CompareTo(minimum[0]) < 0) minimum[0] = value;
      }

      return minimum[0]; // → `System.IndexOutOfRangeException`
    }
      public static T Min<T>(T valueA, T valueB) where T : System.IComparable<T> => valueA.CompareTo(valueB) < 0 ? valueA : valueB;
      public static T Min<T>(params T[] values)                                  => Util.Min(values);

    private static string NormalizeURI(string path) {
      path = path.TrimEnd().Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);

      // …
      while (0 != path.Length && (path.EndsWith(System.IO.Path.DirectorySeparatorChar) || System.String.IsNullOrWhiteSpace(path.Substring(path.Length - 1))))
        path = path.TrimEnd(System.IO.Path.DirectorySeparatorChar).TrimEnd();

      return path;
    }

    public static decimal Percent(decimal percentage) => percentage / 100.0m;
    public static double  Percent(double  percentage) => percentage / 100.0;
    public static float   Percent(float   percentage) => percentage / 100.0f;

    public static decimal PercentOf(decimal value, decimal percentage) => System.Math.Min(value, (decimal) percentage) * (System.Math.Max(value, (decimal) percentage) / 100.0m);
    public static decimal PercentOf(decimal value, double  percentage) => System.Math.Min(value, (decimal) percentage) * (System.Math.Max(value, (decimal) percentage) / 100.0m);
    public static decimal PercentOf(decimal value, float   percentage) => System.Math.Min(value, (decimal) percentage) * (System.Math.Max(value, (decimal) percentage) / 100.0m);
    public static double  PercentOf(double  value, decimal percentage) => System.Math.Min(value, (double)  percentage) * (System.Math.Max(value, (double)  percentage) / 100.0);
    public static double  PercentOf(double  value, double  percentage) => System.Math.Min(value, (double)  percentage) * (System.Math.Max(value, (double)  percentage) / 100.0);
    public static double  PercentOf(double  value, float   percentage) => System.Math.Min(value, (double)  percentage) * (System.Math.Max(value, (double)  percentage) / 100.0);
    public static float   PercentOf(float   value, decimal percentage) => System.Math.Min(value, (float)   percentage) * (System.Math.Max(value, (float)   percentage) / 100.0f);
    public static float   PercentOf(float   value, double  percentage) => System.Math.Min(value, (float)   percentage) * (System.Math.Max(value, (float)   percentage) / 100.0f);
    public static float   PercentOf(float   value, float   percentage) => System.Math.Min(value, (float)   percentage) * (System.Math.Max(value, (float)   percentage) / 100.0f);

    public static void  PreloadURI           (string path)                                         => Util.LoadURI           (path, null, Util.LoadAsynchronously, Util.LoadCached);
    public static void  PreloadURIAsAudioClip(string path, UnityEngine.AudioType? encoding = null) => Util.LoadURIAsAudioClip(path, null, Util.LoadAsynchronously, Util.LoadCached, encoding);
    public static void  PreloadURIAsText     (string path, System.Text.Encoding?  encoding = null) => Util.LoadURIAsText     (path, null, Util.LoadAsynchronously, Util.LoadCached, encoding);
    public static void  PreloadURIAsTexture2D(string path)                                         => Util.LoadURIAsTexture2D(path, null, Util.LoadAsynchronously, Util.LoadCached);

    public static UnityEngine.Rect RectFromCorners(UnityEngine.Vector3[] corners) {
      // → Origin begins from bottom-left rather than top-left
      return new(corners[0].x, corners[0].y, corners[3].x - corners[0].x, corners[1].y - corners[0].y);
    }

    public static UnityEngine.Vector2 SetVectorAxis(UnityEngine.Vector2 vector, UnityEngine.Vector2 axis, float value) {
      if (0.0f != axis.x) vector.x = value;
      if (0.0f != axis.y) vector.y = value;

      return vector;
    }

    public static UnityEngine.Vector2Int SetVectorAxis(UnityEngine.Vector2Int vector, UnityEngine.Vector2Int axis, int value) {
      if (0 != axis.x) vector.x = value;
      if (0 != axis.y) vector.y = value;

      return vector;
    }

    public static UnityEngine.Vector3 SetVectorAxis(UnityEngine.Vector3 vector, UnityEngine.Vector3 axis, float value) {
      if (0.0f != axis.x) vector.x = value;
      if (0.0f != axis.y) vector.y = value;
      if (0.0f != axis.z) vector.z = value;

      return vector;
    }

    public static UnityEngine.Vector3Int SetVectorAxis(UnityEngine.Vector3Int vector, UnityEngine.Vector3Int axis, int value) {
      if (0 != axis.x) vector.x = value;
      if (0 != axis.y) vector.y = value;
      if (0 != axis.z) vector.z = value;

      return vector;
    }

    public static UnityEngine.Vector4 SetVectorAxis(UnityEngine.Vector4 vector, UnityEngine.Vector4 axis, float value) {
      if (0.0f != axis.x) vector.x = value;
      if (0.0f != axis.y) vector.y = value;
      if (0.0f != axis.z) vector.z = value;
      if (0.0f != axis.w) vector.w = value;

      return vector;
    }

      public static UnityEngine.Vector2    SetVectorAxis(UnityEngine.Vector2    vector, UnityEngine.Vector2Int axis, float value) { return Util.SetVectorAxis(vector, new UnityEngine.Vector2   ((float) axis.x, (float) axis.y),                               value); }
      public static UnityEngine.Vector2    SetVectorAxis(UnityEngine.Vector2    vector, UnityEngine.Vector3    axis, float value) { return Util.SetVectorAxis(vector, new UnityEngine.Vector2   ((float) axis.x, (float) axis.y),                               value); }
      public static UnityEngine.Vector2    SetVectorAxis(UnityEngine.Vector2    vector, UnityEngine.Vector3Int axis, float value) { return Util.SetVectorAxis(vector, new UnityEngine.Vector2   ((float) axis.x, (float) axis.y),                               value); }
      public static UnityEngine.Vector2    SetVectorAxis(UnityEngine.Vector2    vector, UnityEngine.Vector4    axis, float value) { return Util.SetVectorAxis(vector, new UnityEngine.Vector2   ((float) axis.x, (float) axis.y),                               value); }
      public static UnityEngine.Vector2Int SetVectorAxis(UnityEngine.Vector2Int vector, UnityEngine.Vector2    axis, int   value) { return Util.SetVectorAxis(vector, new UnityEngine.Vector2Int((int)   axis.x, (int)   axis.y),                               value); }
      public static UnityEngine.Vector2Int SetVectorAxis(UnityEngine.Vector2Int vector, UnityEngine.Vector3    axis, int   value) { return Util.SetVectorAxis(vector, new UnityEngine.Vector2Int((int)   axis.x, (int)   axis.y),                               value); }
      public static UnityEngine.Vector2Int SetVectorAxis(UnityEngine.Vector2Int vector, UnityEngine.Vector3Int axis, int   value) { return Util.SetVectorAxis(vector, new UnityEngine.Vector2Int((int)   axis.x, (int)   axis.y),                               value); }
      public static UnityEngine.Vector2Int SetVectorAxis(UnityEngine.Vector2Int vector, UnityEngine.Vector4    axis, int   value) { return Util.SetVectorAxis(vector, new UnityEngine.Vector2Int((int)   axis.x, (int)   axis.y),                               value); }
      public static UnityEngine.Vector3    SetVectorAxis(UnityEngine.Vector3    vector, UnityEngine.Vector2    axis, float value) { return Util.SetVectorAxis(vector, new UnityEngine.Vector3   ((float) axis.x, (float) axis.y, (float) 0.0f),                 value); }
      public static UnityEngine.Vector3    SetVectorAxis(UnityEngine.Vector3    vector, UnityEngine.Vector2Int axis, float value) { return Util.SetVectorAxis(vector, new UnityEngine.Vector3   ((float) axis.x, (float) axis.y, (float) 0.0f),                 value); }
      public static UnityEngine.Vector3    SetVectorAxis(UnityEngine.Vector3    vector, UnityEngine.Vector3Int axis, float value) { return Util.SetVectorAxis(vector, new UnityEngine.Vector3   ((float) axis.x, (float) axis.y, (float) axis.z),               value); }
      public static UnityEngine.Vector3    SetVectorAxis(UnityEngine.Vector3    vector, UnityEngine.Vector4    axis, float value) { return Util.SetVectorAxis(vector, new UnityEngine.Vector3   ((float) axis.x, (float) axis.y, (float) axis.z),               value); }
      public static UnityEngine.Vector3Int SetVectorAxis(UnityEngine.Vector3Int vector, UnityEngine.Vector2    axis, int   value) { return Util.SetVectorAxis(vector, new UnityEngine.Vector3Int((int)   axis.x, (int)   axis.y, (int)   0.0f),                 value); }
      public static UnityEngine.Vector3Int SetVectorAxis(UnityEngine.Vector3Int vector, UnityEngine.Vector2Int axis, int   value) { return Util.SetVectorAxis(vector, new UnityEngine.Vector3Int((int)   axis.x, (int)   axis.y, (int)   0.0f),                 value); }
      public static UnityEngine.Vector3Int SetVectorAxis(UnityEngine.Vector3Int vector, UnityEngine.Vector3    axis, int   value) { return Util.SetVectorAxis(vector, new UnityEngine.Vector3Int((int)   axis.x, (int)   axis.y, (int)   axis.z),               value); }
      public static UnityEngine.Vector3Int SetVectorAxis(UnityEngine.Vector3Int vector, UnityEngine.Vector4    axis, int   value) { return Util.SetVectorAxis(vector, new UnityEngine.Vector3Int((int)   axis.x, (int)   axis.y, (int)   axis.z),               value); }
      public static UnityEngine.Vector4    SetVectorAxis(UnityEngine.Vector4    vector, UnityEngine.Vector2    axis, float value) { return Util.SetVectorAxis(vector, new UnityEngine.Vector4   ((float) axis.x, (float) axis.y, (float) 0.0f,   (float) 0.0f), value); }
      public static UnityEngine.Vector4    SetVectorAxis(UnityEngine.Vector4    vector, UnityEngine.Vector2Int axis, float value) { return Util.SetVectorAxis(vector, new UnityEngine.Vector4   ((float) axis.x, (float) axis.y, (float) 0.0f,   (float) 0.0f), value); }
      public static UnityEngine.Vector4    SetVectorAxis(UnityEngine.Vector4    vector, UnityEngine.Vector3    axis, float value) { return Util.SetVectorAxis(vector, new UnityEngine.Vector4   ((float) axis.x, (float) axis.y, (float) axis.z, (float) 0.0f), value); }
      public static UnityEngine.Vector4    SetVectorAxis(UnityEngine.Vector4    vector, UnityEngine.Vector3Int axis, float value) { return Util.SetVectorAxis(vector, new UnityEngine.Vector4   ((float) axis.x, (float) axis.y, (float) axis.z, (float) 0.0f), value); }

      public static UnityEngine.Vector3    SetVectorBackAxis   (UnityEngine.Vector3    vector, float value) { return Util.SetVectorAxis(vector, UnityEngine.Vector3   .back,    value); }
      public static UnityEngine.Vector3Int SetVectorBackAxis   (UnityEngine.Vector3Int vector, int   value) { return Util.SetVectorAxis(vector, UnityEngine.Vector3Int.back,    value); }
      public static UnityEngine.Vector2    SetVectorDownAxis   (UnityEngine.Vector2    vector, float value) { return Util.SetVectorAxis(vector, UnityEngine.Vector2   .down,    value); }
      public static UnityEngine.Vector3    SetVectorDownAxis   (UnityEngine.Vector3    vector, float value) { return Util.SetVectorAxis(vector, UnityEngine.Vector3   .down,    value); }
      public static UnityEngine.Vector2Int SetVectorDownAxis   (UnityEngine.Vector2Int vector, int   value) { return Util.SetVectorAxis(vector, UnityEngine.Vector2Int.down,    value); }
      public static UnityEngine.Vector3Int SetVectorDownAxis   (UnityEngine.Vector3Int vector, int   value) { return Util.SetVectorAxis(vector, UnityEngine.Vector3Int.down,    value); }
      public static UnityEngine.Vector3    SetVectorForwardAxis(UnityEngine.Vector3    vector, float value) { return Util.SetVectorAxis(vector, UnityEngine.Vector3   .forward, value); }
      public static UnityEngine.Vector3Int SetVectorForwardAxis(UnityEngine.Vector3Int vector, int   value) { return Util.SetVectorAxis(vector, UnityEngine.Vector3Int.forward, value); }
      public static UnityEngine.Vector2    SetVectorLeftAxis   (UnityEngine.Vector2    vector, float value) { return Util.SetVectorAxis(vector, UnityEngine.Vector2   .left,    value); }
      public static UnityEngine.Vector3    SetVectorLeftAxis   (UnityEngine.Vector3    vector, float value) { return Util.SetVectorAxis(vector, UnityEngine.Vector3   .left,    value); }
      public static UnityEngine.Vector2Int SetVectorLeftAxis   (UnityEngine.Vector2Int vector, int   value) { return Util.SetVectorAxis(vector, UnityEngine.Vector2Int.left,    value); }
      public static UnityEngine.Vector3Int SetVectorLeftAxis   (UnityEngine.Vector3Int vector, int   value) { return Util.SetVectorAxis(vector, UnityEngine.Vector3Int.left,    value); }
      public static UnityEngine.Vector2    SetVectorRightAxis  (UnityEngine.Vector2    vector, float value) { return Util.SetVectorAxis(vector, UnityEngine.Vector2   .right,   value); }
      public static UnityEngine.Vector3    SetVectorRightAxis  (UnityEngine.Vector3    vector, float value) { return Util.SetVectorAxis(vector, UnityEngine.Vector3   .right,   value); }
      public static UnityEngine.Vector2Int SetVectorRightAxis  (UnityEngine.Vector2Int vector, int   value) { return Util.SetVectorAxis(vector, UnityEngine.Vector2Int.right,   value); }
      public static UnityEngine.Vector3Int SetVectorRightAxis  (UnityEngine.Vector3Int vector, int   value) { return Util.SetVectorAxis(vector, UnityEngine.Vector3Int.right,   value); }
      public static UnityEngine.Vector2    SetVectorUpAxis     (UnityEngine.Vector2    vector, float value) { return Util.SetVectorAxis(vector, UnityEngine.Vector2   .up,      value); }
      public static UnityEngine.Vector3    SetVectorUpAxis     (UnityEngine.Vector3    vector, float value) { return Util.SetVectorAxis(vector, UnityEngine.Vector3   .up,      value); }
      public static UnityEngine.Vector2Int SetVectorUpAxis     (UnityEngine.Vector2Int vector, int   value) { return Util.SetVectorAxis(vector, UnityEngine.Vector2Int.up,      value); }
      public static UnityEngine.Vector3Int SetVectorUpAxis     (UnityEngine.Vector3Int vector, int   value) { return Util.SetVectorAxis(vector, UnityEngine.Vector3Int.up,      value); }

      public static UnityEngine.Vector3    SetVectorDepthAxis (UnityEngine.Vector3    vector, float value) { return Util.SetVectorForwardAxis(vector, value); }
      public static UnityEngine.Vector3Int SetVectorDepthAxis (UnityEngine.Vector3Int vector, int   value) { return Util.SetVectorForwardAxis(vector, value); }
      public static UnityEngine.Vector2    SetVectorHeightAxis(UnityEngine.Vector2    vector, float value) { return Util.SetVectorUpAxis     (vector, value); }
      public static UnityEngine.Vector3    SetVectorHeightAxis(UnityEngine.Vector3    vector, float value) { return Util.SetVectorUpAxis     (vector, value); }
      public static UnityEngine.Vector2Int SetVectorHeightAxis(UnityEngine.Vector2Int vector, int   value) { return Util.SetVectorUpAxis     (vector, value); }
      public static UnityEngine.Vector3Int SetVectorHeightAxis(UnityEngine.Vector3Int vector, int   value) { return Util.SetVectorUpAxis     (vector, value); }
      public static UnityEngine.Vector2    SetVectorWidthAxis (UnityEngine.Vector2    vector, float value) { return Util.SetVectorRightAxis  (vector, value); }
      public static UnityEngine.Vector3    SetVectorWidthAxis (UnityEngine.Vector3    vector, float value) { return Util.SetVectorRightAxis  (vector, value); }
      public static UnityEngine.Vector2Int SetVectorWidthAxis (UnityEngine.Vector2Int vector, int   value) { return Util.SetVectorRightAxis  (vector, value); }
      public static UnityEngine.Vector3Int SetVectorWidthAxis (UnityEngine.Vector3Int vector, int   value) { return Util.SetVectorRightAxis  (vector, value); }

    public static void StopWaiting() {
      float timestamp = UnityEngine.Time.realtimeSinceStartup;

      // …
      for (int index = WAITS.Count; 0 != index--; ) {
        Util.Wait wait = WAITS[index];

        // → Exclude `float.PositiveInfinity` and normalized greater `wait.[interval|timestamp]`
        if (!(timestamp < wait.timestamp)) {
          if (0.0f >= wait.interval || /* → `wait.interval != wait.interval` */ float.IsNaN(wait.interval))
            WAITS.RemoveAt(index);

          wait.timestamp += wait.interval;
          wait.callback?.Invoke();
        }
      }
    }

    public static object? Switch<T>(T value, System.Collections.Generic.Dictionary<T, object> expression, object? fallback = null) {
      return expression?.TryGetValue(value, out object callback) ?? false ? callback : fallback;
    }

    public static void WaitAtLeastEvery(float delay, System.Action callback) {
      WAITS.Add(new() {callback = callback, interval = delay, timestamp = UnityEngine.Time.realtimeSinceStartup + delay});
    }

    public static void WaitAtLeastOnce(float delay, System.Action callback) {
      WAITS.Add(new() {callback = callback, interval = 0.0f, timestamp = UnityEngine.Time.realtimeSinceStartup + delay});
    }

    public static UnityEngine.Bounds? WorldBoundsFromRectTransform(UnityEngine.RectTransform? transform) {
      UnityEngine.Rect? rectangle = Util.WorldRectFromRectTransform(transform);
      return null == rectangle ? null : new(transform.position, new(rectangle?.width ?? 0.0f, rectangle?.height ?? 0.0f, 0.0f));
    }

    public static UnityEngine.Vector3[]? WorldCornersFromRectTransform(UnityEngine.RectTransform? transform) {
      return Util.CornersFromRectTransform(null == transform ? null : transform.GetWorldCorners);
    }

    public static UnityEngine.Rect? WorldRectFromRectTransform(UnityEngine.RectTransform? transform) {
      UnityEngine.Vector3[]? corners = Util.WorldCornersFromRectTransform(transform);
      return null == corners ? null : Util.RectFromCorners(corners);
    }
  }
}
