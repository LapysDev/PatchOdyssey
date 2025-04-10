global using Animation  = PatchOdyssey.Animation;                                                     //
global using PatchBurst = Unity.Burst.BurstCompile;                                                   //
#if NET7_0 || NET7_0_OR_GREATER                                                                       // ⟶ Modify constructor with diagnostic features
  global using PatchConstructor = System.Diagnostics.CodeAnalysis.SetsRequiredMembersAttribute;       //    ^^
#endif                                                                                                //
global using PatchLayout = System.Runtime.InteropServices.StructLayoutAttribute;                      // ⟶ Un-manages the structure of class types
global using PatchMethod = System.Runtime.CompilerServices.MethodImplAttribute;                       // ⟶ Modify function with compile attributes e.g. inlined, optimized, unmanaged, …, e.t.c.
global using PatchOdyssey.Collections;                                                                // ⟶ Expose custom collections e.g. `EditorDictionary<T>`, `GameObjectSharedList<T>`, …
global using PatchOffset = System.Runtime.InteropServices.FieldOffsetAttribute;                       // ⟶ Adjust offset of fields within structurally-unmanaged class types
#if NET9_0 || NET9_0_OR_GREATER                                                                       // ⟶ Modify function priority over another overload during resolution; Defaults to `PatchResolution(0)` for all functions
  global using PatchResolution = System.Runtime.CompilerServices.OverloadResolutionPriorityAttribute; //    ^^
#endif                                                                                                //
global using PatchUnburst = Unity.Burst.BurstDiscard;                                                 // ⟶ `Unity.Burst.CompilerServices.IgnoreWarning(…)`?
global using static System.Runtime.CompilerServices.MethodImplOptions;                                // ⟶ Use case: 𝑓 `PatchMethod(AggressiveOptimization, …)`
global using static System.Runtime.InteropServices.LayoutKind;                                        // ⟶ Use case: 𝑓 `PatchLayout(Sequential, …)`

/* C# Polyfills */
#if !(NET5_0 || NET5_0_OR_GREATER)
  namespace System.Runtime.CompilerServices {
    // ⟶`init` @ `https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/init`
    internal static class IsExternalInit {}
  }
#endif

#if !(NET7_0 || NET7_0_OR_GREATER)
  namespace System.Runtime.CompilerServices {
    // ⟶ `required` @ `https://github.com/dotnet/core/issues/8016` `https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/required`
    public        class CompilerFeatureRequiredAttribute : System.Attribute { public CompilerFeatureRequiredAttribute(string name) {} }
    public sealed class RequiredMemberAttribute          : System.Attribute {}
  }

  // ⟶ `System.Diagnostics.CodeAnalysis.SetsRequiredMembers` @ `https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.setsrequiredmembersattribute`
  [System.Diagnostics.DebuggerNonUserCode]
  [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
  [System.AttributeUsage(System.AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
  public sealed class PatchConstructor : System.Attribute {}
#endif

#if !(NET9_0 || NET9_0_OR_GREATER)
  // ⟶ `System.Runtime.CompilerServices.OverloadResolutionPriorityAttribute` @ `https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.overloadresolutionpriorityattribute`
  [System.Diagnostics.DebuggerNonUserCode]
  [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
  [System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Constructor | System.AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
  public sealed class PatchResolution : System.Attribute {
    // ⟶ Polyfill functionally does nothing
    public int Priority { get; }
    public PatchResolution(int priority) => this.Priority = priority;
  }
#endif

/* PatchOdyssey */
namespace PatchOdyssey /* ⟶ Class types and delegates */ {
  namespace Animation {
    public class UIKeyframe /* ⟶ Not a `struct`, inherited by `Animation.UISequence` */ {
      public readonly System.Collections.ObjectModel.ReadOnlyDictionary<string, object?>? begin      = null;
      public          System.Collections.ObjectModel.ReadOnlyDictionary<string, object?>? end        { get => this.properties; init => this.properties = value!; }
      public readonly string?                                                             name       = null;
      public readonly System.Collections.ObjectModel.ReadOnlyDictionary<string, object?>  properties = new(new System.Collections.Generic.Dictionary<string, object?>());

      /* … ⟶ Will modify specified `::begin` and `::end` properties to ensure they have the same keys */
      [PatchConstructor]
      internal UIKeyframe(in string name, in System.Collections.Generic.IDictionary<string, object?> properties) {
        this.name = name;

        if (properties is not null)
        this.properties = new(properties);
      }

      [PatchConstructor]
      internal UIKeyframe(in string name, in System.Collections.Generic.IDictionary<string, object?> begin, in System.Collections.Generic.IDictionary<string, object?> end) {
        this.name = name;

        if (begin is not null && end is not null) {
          (int begin, int end) offsets = (begin.Keys.Count, end.Keys.Count);
          string[]             names   = new string[offsets.begin + offsets.end];

          // …
          begin.Keys.CopyTo(names, 0);
          end  .Keys.CopyTo(names, offsets.begin);

          foreach (string _ in new System.ReadOnlySpan<string>(names, offsets.begin, offsets.end))   { if (!begin.ContainsKey(_)) end  .Remove(_); }
          foreach (string _ in new System.ReadOnlySpan<string>(names, 0,             offsets.begin)) { if (!end  .ContainsKey(_)) begin.Remove(_); }

          this.begin = begin.AsReadOnly();
          this.end   = end  .AsReadOnly(); // ⟶ `this.end = …;`
        }
      }
    }

    public class UISequence : PatchOdyssey.Animation.UIKeyframe, System.Collections.IEnumerable {
      private static   System.Collections.Generic.Dictionary<System.Type, (System.Reflection.MethodInfo multiplication, System.Reflection.MethodInfo subtraction)> INTERPOLATION_OPS =  new(3);
      public  required double                                                                                                                                      delay             =  0.0;
      public  required double                                                                                                                                      duration          =  0.0;
      public  required PatchOdyssey.Tweener                                                                                                                        easing            =  PatchOdyssey.Animation.Function  .Linear;
      public  required PatchOdyssey.Interpolator                                                                                                                   interpolator      =  PatchOdyssey.Animation.UISequence.Interpolate;
      public           bool                                                                                                                                        isDone            => UnityEngine.Time.realtimeSinceStartupAsDouble >= this.delay + this.duration + this.timestamp;
      private readonly System.Collections.Generic.SortedList<double, PatchOdyssey.Animation.UIKeyframe>                                                            keyframes         =  new(1);
      private          double                                                                                                                                      timestamp         =  0.0;

      /* … ⟶ 𝑓 `UISequence([optional] name, duration, [optional] delay, [optional] easing, [optional] interpolator, begin, end) { … }` */
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration,                                                                                    System.Collections.Generic.Dictionary <string, object?> begin, System.Collections.Generic.Dictionary <string, object?> end) : this(null!, duration, 0.0,   null!,  null!,        begin,                                                             end)                                                             {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration,                                                                                    System.Collections.Generic.IDictionary<string, object?> begin, System.Collections.Generic.IDictionary<string, object?> end) : this(null!, duration, 0.0,   null!,  null!,        new System.Collections.Generic.Dictionary<string, object?>(begin), new System.Collections.Generic.Dictionary<string, object?>(end)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration, double delay,                                                                      System.Collections.Generic.Dictionary <string, object?> begin, System.Collections.Generic.Dictionary <string, object?> end) : this(null!, duration, 0.0,   null!,  null!,        begin,                                                             end)                                                             {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration, double delay,                                                                      System.Collections.Generic.IDictionary<string, object?> begin, System.Collections.Generic.IDictionary<string, object?> end) : this(null!, duration, 0.0,   null!,  null!,        new System.Collections.Generic.Dictionary<string, object?>(begin), new System.Collections.Generic.Dictionary<string, object?>(end)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration,               PatchOdyssey.Tweener easing,                                         System.Collections.Generic.Dictionary <string, object?> begin, System.Collections.Generic.Dictionary <string, object?> end) : this(null!, duration, 0.0,   easing, null!,        begin,                                                             end)                                                             {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration,               PatchOdyssey.Tweener easing,                                         System.Collections.Generic.IDictionary<string, object?> begin, System.Collections.Generic.IDictionary<string, object?> end) : this(null!, duration, 0.0,   easing, null!,        new System.Collections.Generic.Dictionary<string, object?>(begin), new System.Collections.Generic.Dictionary<string, object?>(end)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration, double delay, PatchOdyssey.Tweener easing,                                         System.Collections.Generic.Dictionary <string, object?> begin, System.Collections.Generic.Dictionary <string, object?> end) : this(null!, duration, delay, easing, null!,        begin,                                                             end)                                                             {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration, double delay, PatchOdyssey.Tweener easing,                                         System.Collections.Generic.IDictionary<string, object?> begin, System.Collections.Generic.IDictionary<string, object?> end) : this(null!, duration, delay, easing, null!,        new System.Collections.Generic.Dictionary<string, object?>(begin), new System.Collections.Generic.Dictionary<string, object?>(end)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration,                                            PatchOdyssey.Interpolator interpolator, System.Collections.Generic.Dictionary <string, object?> begin, System.Collections.Generic.Dictionary <string, object?> end) : this(null!, duration, 0.0,   null!,  interpolator, begin,                                                             end)                                                             {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration,                                            PatchOdyssey.Interpolator interpolator, System.Collections.Generic.IDictionary<string, object?> begin, System.Collections.Generic.IDictionary<string, object?> end) : this(null!, duration, 0.0,   null!,  interpolator, new System.Collections.Generic.Dictionary<string, object?>(begin), new System.Collections.Generic.Dictionary<string, object?>(end)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration, double delay,                              PatchOdyssey.Interpolator interpolator, System.Collections.Generic.Dictionary <string, object?> begin, System.Collections.Generic.Dictionary <string, object?> end) : this(null!, duration, delay, null!,  interpolator, begin,                                                             end)                                                             {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration, double delay,                              PatchOdyssey.Interpolator interpolator, System.Collections.Generic.IDictionary<string, object?> begin, System.Collections.Generic.IDictionary<string, object?> end) : this(null!, duration, delay, null!,  interpolator, new System.Collections.Generic.Dictionary<string, object?>(begin), new System.Collections.Generic.Dictionary<string, object?>(end)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration,               PatchOdyssey.Tweener easing, PatchOdyssey.Interpolator interpolator, System.Collections.Generic.Dictionary <string, object?> begin, System.Collections.Generic.Dictionary <string, object?> end) : this(null!, duration, 0.0,   easing, interpolator, begin,                                                             end)                                                             {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration,               PatchOdyssey.Tweener easing, PatchOdyssey.Interpolator interpolator, System.Collections.Generic.IDictionary<string, object?> begin, System.Collections.Generic.IDictionary<string, object?> end) : this(null!, duration, 0.0,   easing, interpolator, new System.Collections.Generic.Dictionary<string, object?>(begin), new System.Collections.Generic.Dictionary<string, object?>(end)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration, double delay, PatchOdyssey.Tweener easing, PatchOdyssey.Interpolator interpolator, System.Collections.Generic.Dictionary <string, object?> begin, System.Collections.Generic.Dictionary <string, object?> end) : this(null!, duration, delay, easing, interpolator, begin,                                                             end)                                                             {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration, double delay, PatchOdyssey.Tweener easing, PatchOdyssey.Interpolator interpolator, System.Collections.Generic.IDictionary<string, object?> begin, System.Collections.Generic.IDictionary<string, object?> end) : this(null!, duration, delay, easing, interpolator, new System.Collections.Generic.Dictionary<string, object?>(begin), new System.Collections.Generic.Dictionary<string, object?>(end)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration,                                                                                    System.Collections.Generic.Dictionary <string, object?> begin, System.Collections.Generic.Dictionary <string, object?> end) : this(name,  duration, 0.0,   null!,  null!,        begin,                                                             end)                                                             {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration,                                                                                    System.Collections.Generic.IDictionary<string, object?> begin, System.Collections.Generic.IDictionary<string, object?> end) : this(name,  duration, 0.0,   null!,  null!,        new System.Collections.Generic.Dictionary<string, object?>(begin), new System.Collections.Generic.Dictionary<string, object?>(end)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration, double delay,                                                                      System.Collections.Generic.Dictionary <string, object?> begin, System.Collections.Generic.Dictionary <string, object?> end) : this(name,  duration, 0.0,   null!,  null!,        begin,                                                             end)                                                             {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration, double delay,                                                                      System.Collections.Generic.IDictionary<string, object?> begin, System.Collections.Generic.IDictionary<string, object?> end) : this(name,  duration, 0.0,   null!,  null!,        new System.Collections.Generic.Dictionary<string, object?>(begin), new System.Collections.Generic.Dictionary<string, object?>(end)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration,               PatchOdyssey.Tweener easing,                                         System.Collections.Generic.Dictionary <string, object?> begin, System.Collections.Generic.Dictionary <string, object?> end) : this(name,  duration, 0.0,   easing, null!,        begin,                                                             end)                                                             {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration,               PatchOdyssey.Tweener easing,                                         System.Collections.Generic.IDictionary<string, object?> begin, System.Collections.Generic.IDictionary<string, object?> end) : this(name,  duration, 0.0,   easing, null!,        new System.Collections.Generic.Dictionary<string, object?>(begin), new System.Collections.Generic.Dictionary<string, object?>(end)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration, double delay, PatchOdyssey.Tweener easing,                                         System.Collections.Generic.Dictionary <string, object?> begin, System.Collections.Generic.Dictionary <string, object?> end) : this(name,  duration, delay, easing, null!,        begin,                                                             end)                                                             {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration, double delay, PatchOdyssey.Tweener easing,                                         System.Collections.Generic.IDictionary<string, object?> begin, System.Collections.Generic.IDictionary<string, object?> end) : this(name,  duration, delay, easing, null!,        new System.Collections.Generic.Dictionary<string, object?>(begin), new System.Collections.Generic.Dictionary<string, object?>(end)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration,                                            PatchOdyssey.Interpolator interpolator, System.Collections.Generic.Dictionary <string, object?> begin, System.Collections.Generic.Dictionary <string, object?> end) : this(name,  duration, 0.0,   null!,  interpolator, begin,                                                             end)                                                             {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration,                                            PatchOdyssey.Interpolator interpolator, System.Collections.Generic.IDictionary<string, object?> begin, System.Collections.Generic.IDictionary<string, object?> end) : this(name,  duration, 0.0,   null!,  interpolator, new System.Collections.Generic.Dictionary<string, object?>(begin), new System.Collections.Generic.Dictionary<string, object?>(end)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration, double delay,                              PatchOdyssey.Interpolator interpolator, System.Collections.Generic.Dictionary <string, object?> begin, System.Collections.Generic.Dictionary <string, object?> end) : this(name,  duration, delay, null!,  interpolator, begin,                                                             end)                                                             {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration, double delay,                              PatchOdyssey.Interpolator interpolator, System.Collections.Generic.IDictionary<string, object?> begin, System.Collections.Generic.IDictionary<string, object?> end) : this(name,  duration, delay, null!,  interpolator, new System.Collections.Generic.Dictionary<string, object?>(begin), new System.Collections.Generic.Dictionary<string, object?>(end)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration,               PatchOdyssey.Tweener easing, PatchOdyssey.Interpolator interpolator, System.Collections.Generic.Dictionary <string, object?> begin, System.Collections.Generic.Dictionary <string, object?> end) : this(name,  duration, 0.0,   easing, interpolator, begin,                                                             end)                                                             {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration,               PatchOdyssey.Tweener easing, PatchOdyssey.Interpolator interpolator, System.Collections.Generic.IDictionary<string, object?> begin, System.Collections.Generic.IDictionary<string, object?> end) : this(name,  duration, 0.0,   easing, interpolator, new System.Collections.Generic.Dictionary<string, object?>(begin), new System.Collections.Generic.Dictionary<string, object?>(end)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration, double delay, PatchOdyssey.Tweener easing, PatchOdyssey.Interpolator interpolator, System.Collections.Generic.IDictionary<string, object?> begin, System.Collections.Generic.IDictionary<string, object?> end) : this(name,  duration, delay, easing, interpolator, new System.Collections.Generic.Dictionary<string, object?>(begin), new System.Collections.Generic.Dictionary<string, object?>(end)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)]
      public UISequence(string name, double duration, double delay, PatchOdyssey.Tweener easing, PatchOdyssey.Interpolator interpolator, System.Collections.Generic.Dictionary<string, object?> begin, System.Collections.Generic.Dictionary<string, object?> end) : base(name, begin ?? new System.Collections.Generic.Dictionary<string, object?>(), end ?? new System.Collections.Generic.Dictionary<string, object?>()) {
        this.delay        = delay;
        this.duration     = duration;
        this.easing       = easing       is not null ? new(easing!)       : this.easing;
        this.interpolator = interpolator is not null ? new(interpolator!) : this.interpolator;
        this.timestamp    = UnityEngine.Time.realtimeSinceStartupAsDouble;
      }

      /* … ⟶ 𝑓 `void Add([optional] keyframe, progress, properties) { … }` */
      [PatchMethod(AggressiveInlining)] public void Add                 (double progress, System.Collections.Generic.Dictionary <string, object?> properties) => this.Add($"#{this.keyframes.Count + 1}", progress, properties);
      [PatchMethod(AggressiveInlining)] public void Add                 (double progress, System.Collections.Generic.IDictionary<string, object?> properties) => this.Add($"#{this.keyframes.Count + 1}", progress, properties);
      [PatchMethod(AggressiveInlining)] public void Add(string keyframe, double progress, System.Collections.Generic.IDictionary<string, object?> properties) => this.Add(keyframe,                       progress, new System.Collections.Generic.Dictionary<string, object?>(properties));
      public void Add(string keyframe, double progress, System.Collections.Generic.Dictionary<string, object?> properties) {
        foreach (System.Collections.Generic.KeyValuePair<double, PatchOdyssey.Animation.UIKeyframe> enumerated in this.keyframes) {
          if (enumerated.Key == progress || (keyframe is null ? false : keyframe == enumerated.Value.name))
          return;
        }

        this.keyframes.Add(progress, new(keyframe, properties)); // ⟶ `SortedList` keeps `::keyframes` sorted
      }

      [PatchMethod(AggressiveInlining)]
      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
        return null!;
      }

      private static object? Interpolate(double progress, object? a, object? b) /* ⟶ `(b - a) * progress` */ {
        (System.Reflection.MethodInfo multiplication, System.Reflection.MethodInfo subtraction) operators    = (null!, null!);
        System.Type                                                                             progressType = typeof(double);
        System.Type?                                                                            type         = a?.GetType();

        // …
        if (type is null || type != b?.GetType())
        return null;

        // …
        if (type == typeof(double))  return (double)  ((double) ((double)  b! - (double)  a!) * progress);
        if (type == typeof(float))   return (float)   ((double) ((float)   b! - (float)   a!) * progress);
        if (type == typeof(decimal)) return (decimal) ((double) ((decimal) b! - (decimal) a!) * progress);
        if (type == typeof(int))     return (int)     ((double) ((int)     b! - (int)     a!) * progress);
        if (type == typeof(nint))    return (nint)    ((double) ((nint)    b! - (nint)    a!) * progress);
        if (type == typeof(long))    return (long)    ((double) ((long)    b! - (long)    a!) * progress);
        if (type == typeof(uint))    return (uint)    ((double) ((uint)    b! - (uint)    a!) * progress);
        if (type == typeof(nuint))   return (nuint)   ((double) ((nuint)   b! - (nuint)   a!) * progress);
        if (type == typeof(ulong))   return (ulong)   ((double) ((ulong)   b! - (ulong)   a!) * progress);
        if (type == typeof(short))   return (short)   ((double) ((short)   b! - (short)   a!) * progress);
        if (type == typeof(ushort))  return (ushort)  ((double) ((ushort)  b! - (ushort)  a!) * progress);
        if (type == typeof(byte))    return (byte)    ((double) ((byte)    b! - (byte)    a!) * progress);
        if (type == typeof(sbyte))   return (sbyte)   ((double) ((sbyte)   b! - (sbyte)   a!) * progress);

        // …  ⟶ also interpolate between eligible class types e.g. `UnityEngine.Color`, `UnityEngine.Vector3`, …
        if (!INTERPOLATION_OPS.TryGetValue(type, out operators) && (operators.subtraction = type.GetMethod("op_Subtraction", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, new[] {type, type}, null)) is not null)
        foreach (System.Reflection.MethodInfo method in type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)) {
          System.Reflection.ParameterInfo[] parameters = method.GetParameters();

          if (method.Name == "op_Multiply" && parameters.Length == 2 && parameters[0].ParameterType == operators.subtraction.ReturnType && (
            parameters[1].ParameterType == (progressType = typeof(double))  ||
            parameters[1].ParameterType == (progressType = typeof(float))   ||
            parameters[1].ParameterType == (progressType = typeof(decimal)) ||
            parameters[1].ParameterType == (progressType = typeof(int))     ||
            parameters[1].ParameterType == (progressType = typeof(nint))    ||
            parameters[1].ParameterType == (progressType = typeof(long))    ||
            parameters[1].ParameterType == (progressType = typeof(uint))    ||
            parameters[1].ParameterType == (progressType = typeof(nuint))   ||
            parameters[1].ParameterType == (progressType = typeof(ulong))   ||
            parameters[1].ParameterType == (progressType = typeof(short))   ||
            parameters[1].ParameterType == (progressType = typeof(ushort))  ||
            parameters[1].ParameterType == (progressType = typeof(byte))    ||
            parameters[1].ParameterType == (progressType = typeof(sbyte))
          )) {
            operators.multiplication = method;
            INTERPOLATION_OPS.Add(type, operators);

            break;
          }
        }

        return operators.multiplication?.Invoke(null, new[] {operators.subtraction?.Invoke(null, new[] {b, a}), System.Convert.ChangeType(progress, progressType)});
      }

      [PatchMethod(AggressiveInlining)]
      public void Reset() {
        this.timestamp = UnityEngine.Time.realtimeSinceStartupAsDouble;
      }

      /* … ⟶ Simultaneous (un-)boxing is minimally slow 🐢 */
      public object? this[string property] { get {
        if (property is not null && this.end!.ContainsKey(property)) {
          System.Collections.ObjectModel.ReadOnlyDictionary<string, object?> begin    = this.begin!;
          double                                                             elapsed  = UnityEngine.Time.realtimeSinceStartupAsDouble - this.timestamp;
          System.Collections.ObjectModel.ReadOnlyDictionary<string, object?> end      = this.end!;
          double                                                             progress = (elapsed - this.delay) / this.duration;

          // …
          foreach (System.Collections.Generic.KeyValuePair<double, PatchOdyssey.Animation.UIKeyframe> enumerated in this.keyframes)
          if (enumerated.Value.properties.ContainsKey(property)) {
            if      (enumerated.Key <= progress) begin = enumerated.Value.properties;
            else if (enumerated.Key >= progress) end   = enumerated.Value.properties;
          }

          if (begin.ContainsKey(property)) // ⟶ `interpolator(easing(delay … duration), begin[property], end[property])`
          return (this.interpolator ?? PatchOdyssey.Animation.UISequence.Interpolate)(this.delay <= elapsed ? 0.0 : this.duration <= elapsed - this.delay ? 1.0 : (this.easing ?? PatchOdyssey.Animation.Function.Linear)((elapsed - this.delay) / this.duration), begin[property], end[property]);
        }

        return null;
      } }
    }
  }

  namespace Collections {
    [System.Serializable]
    public class EditorDictionary<TKey, TValue> : System.Collections.Generic.IDictionary<TKey, TValue> /* ⟶ `https://discussions.unity.com/t/finally-a-serializable-dictionary-for-unity-extracted-from-system-collections-generic/586385/1` */ {
      public struct Enumerator : System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>> {
        public           System.Collections.Generic.KeyValuePair  <TKey, TValue>  Current                                => (System.Collections.Generic.KeyValuePair<TKey, TValue>) this.current!;
        private          System.Collections.Generic.KeyValuePair  <TKey, TValue>? current                                =  null;
        private readonly PatchOdyssey.Collections.EditorDictionary<TKey, TValue>  dictionary                             =  null!;
        private          uint                                                     index                                  =  0u;
        private readonly uint                                                     version                                =  0u;
        object                                                                    System.Collections.IEnumerator.Current => this.Current;

        /* … */
        [PatchConstructor, PatchMethod(AggressiveInlining)]
        internal Enumerator(PatchOdyssey.Collections.EditorDictionary<TKey, TValue> dictionary) {
          this.current    = null;
          this.dictionary = dictionary;
          this.index      = 0u;
          this.version    = dictionary.version;
        }

        /* … */
        [PatchMethod(AggressiveInlining)]
        public void Dispose() {}

        public bool MoveNext() {
          if (this.dictionary.version != this.version)
          throw new System.InvalidOperationException("Dictionary changed during enumeration");

          for (; this.dictionary.count > this.index; ++this.index)
          if (this.dictionary.hashes[this.index] >= 0) {
            this.current = new(this.dictionary.keys[this.index], this.dictionary.values[this.index]);
            this.index  += 1u;

            return true;
          }

          this.current = null;
          this.index   = this.dictionary.count + 1u;

          return false;
        }

        void System.Collections.IEnumerator.Reset() {
          if (this.dictionary.version != this.version)
          throw new System.InvalidOperationException("Dictionary changed during enumeration");

          this.current = null;
          this.index   = 0;
        }
      }

      /* … */
      internal const           uint                                                    CapacityMaximum = 0x7FEFFFFDu;
      internal static readonly System.Collections.ObjectModel.ReadOnlyCollection<uint> Primes          = new[] {3u, 7u, 11u, 17u, 23u, 29u, 37u, 47u, 59u, 71u, 89u, 107u, 131u, 163u, 197u, 239u, 293u, 353u, 431u, 521u, 631u, 761u, 919u, 1103u, 1327u, 1597u, 1931u, 2333u, 2801u, 3371u, 4049u, 4861u, 5839u, 7013u, 8419u, 10103u, 12143u, 14591u, 17519u, 21023u, 25229u, 30293u, 36353u, 43627u, 52361u, 62851u, 75431u, 90523u, 108631u, 130363u, 156437u, 187751u, 225307u, 270371u, 324449u, 389357u, 467237u, 560689u, 672827u, 807403u, 968897u, 1162687u, 1395263u, 1674319u, 2009191u, 2411033u, 2893249u, 3471899u, 4166287u, 4999559u, 5999471u, 7199369u}.AsReadOnly();
      internal const           uint                                                    PrimeMaximum    = 0x7FFFFFFFu;

      public                                                                     int                                                Count                                                                                                    => (int) (this.count - this.freeCount);
      public                                                                     System.Collections.Generic.ICollection<TKey>       Keys                                                                                                     { get { TKey[]   keys   = new TKey  [this.Count]; System.Array.Copy(this.keys,   0, keys,   0, this.Count); return keys;   } }
      public                                                                     System.Collections.Generic.ICollection<TValue>     Values                                                                                                   { get { TValue[] values = new TValue[this.Count]; System.Array.Copy(this.values, 0, values, 0, this.Count); return values; } }
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] private          int[]                                              buckets                                                                                                  =  new int[0];
      private                                                           readonly System.Collections.Generic.IEqualityComparer<TKey> comparer                                                                                                 =  System.Collections.Generic.EqualityComparer<TKey>.Default;
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] private          uint                                               count                                                                                                    =  0u;
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] private          uint                                               freeCount                                                                                                =  0u;
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] private          int                                                freeList                                                                                                 =  0;
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] private          int   []                                           hashes                                                                                                   =  new int   [0];
      public                                                            const    bool                                               IsReadOnly                                                                                               =  true;
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] private          TKey  []                                           keys                                                                                                     =  new TKey  [0];
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] private          int   []                                           next                                                                                                     =  new int   [0];
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] private          TValue[]                                           values                                                                                                   =  new TValue[0];
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] public           uint                                               version                                                                                                  =  0u;
      bool                                                                                                                          System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.IsReadOnly => false;

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)] public EditorDictionary()                                                                                                                                                                            : this(0,                                                                                                          null!)    {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public EditorDictionary(int                                                                                                 capacity)                                                                : this(capacity,                                                                                                   null!)    {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public EditorDictionary(System.Collections.Generic.IEqualityComparer<TKey>                                                  comparer)                                                                : this(0,                                                                                                          comparer) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public EditorDictionary(System.Collections.Generic.IDictionary      <TKey, TValue>                                          dictionary)                                                              : this((System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>) dictionary, null!)    {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public EditorDictionary(System.Collections.Generic.IEnumerable      <System.Collections.Generic.KeyValuePair<TKey, TValue>> enumerable)                                                              : this(enumerable,                                                                                                 null!)    {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public EditorDictionary(System.Collections.Generic.IDictionary      <TKey, TValue>                                          dictionary, System.Collections.Generic.IEqualityComparer<TKey> comparer) : this((System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>) dictionary, comparer) {}

      [PatchConstructor, PatchMethod(AggressiveInlining)]
      public EditorDictionary(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>> enumerable, System.Collections.Generic.IEqualityComparer<TKey> comparer) : this(enumerable is null ? 0 : (int) Util.EnumerableCount(enumerable), comparer) {
        if (enumerable is null)
          throw new System.ArgumentNullException($"Source provided is null");

        this.AddRange(enumerable);
      }

      [PatchConstructor]
      public EditorDictionary(int capacity, System.Collections.Generic.IEqualityComparer<TKey> comparer) {
        if (capacity < 0)
          throw new System.ArgumentOutOfRangeException($"Expected positive dictionary capacity");

        EditorDictionary<TKey, TValue>.Ensure();
        this.Initialize((uint) capacity);
        this.comparer = comparer ?? System.Collections.Generic.EqualityComparer<TKey>.Default;
      }

      /* … */
      [PatchMethod(AggressiveInlining)] public void Add(System.Collections.Generic.KeyValuePair<TKey, TValue> element) => this.Add(element.Key, element.Value);
      [PatchMethod(AggressiveInlining)]
      public void Add(TKey key, TValue value) {
        this.Insert(key, value, true);
      }

      [PatchMethod(AggressiveInlining)]
      public void AddRange(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>> enumerable) {
        foreach (System.Collections.Generic.KeyValuePair<TKey, TValue> element in enumerable)
        this.Add(element);
      }

      [PatchMethod(AggressiveInlining)]
      public void Clear() {
        if (0u == this.count)
        return;

        this.buckets.Fill(-1);
        this.hashes .Clear(0, (int) this.count);
        this.keys   .Clear(0, (int) this.count);
        this.next   .Clear(0, (int) this.count);
        this.values .Clear(0, (int) this.count);

        this.count     =  0;
        this.freeCount =  0;
        this.freeList  = -1;
        this.version  += +1;
      }

      [PatchMethod(AggressiveInlining)]
      public bool Contains(System.Collections.Generic.KeyValuePair<TKey, TValue> element) {
        int index = this.FindIndex(element.Key);
        return index != -1 && System.Collections.Generic.EqualityComparer<TValue>.Default.Equals(this.values[index], element.Value);
      }

      [PatchMethod(AggressiveInlining)]
      public bool ContainsKey(TKey key) {
        return this.FindIndex(key) != -1;
      }

      [PatchMethod(AggressiveInlining)]
      public bool ContainsValue(TValue value) {
        for ((uint index, System.Func<TValue, TValue, bool> comparer) = (this.count, value is null ? static (value, _) => value is null : System.Collections.Generic.EqualityComparer<TValue>.Default.Equals); 0u != index--; ) {
          if (this.hashes[index] >= 0 && comparer(this.values[index], value))
          return true;
        }

        return false;
      }

      public void CopyTo(System.Collections.Generic.KeyValuePair<TKey, TValue>[] array, int offset) {
        if (array is null)                       throw new System.ArgumentNullException      ($"Destination array provided is null");
        if (offset < 0 || array.Length < offset) throw new System.ArgumentOutOfRangeException($"Invalid offset provided");
        if (this.Count >  array.Length - offset) throw new System.ArgumentException          ($"Dictionary element count is more than the specified space provided");

        for (int index = 0; index != this.count; ++index) {
          if (this.hashes[index] >= 0)
          array[offset++] = new(this.keys[index], this.values[index]);
        }
      }

      public static System.Func<UnityEngine.Rect, object, object>? DelegateGUIField<T>() {
        if (typeof(T) == typeof(bool))                              return static (position, value) => UnityEditor.EditorGUI.Toggle         (position,                              (System.Boolean)             value)                  as object;
        if (typeof(T) == typeof(double))                            return static (position, value) => UnityEditor.EditorGUI.DoubleField    (position,                              (System.Double)              value)                  as object;
        if (typeof(T) == typeof(float))                             return static (position, value) => UnityEditor.EditorGUI.FloatField     (position,                              (System.Single)              value)                  as object;
        if (typeof(T) == typeof(int))                               return static (position, value) => UnityEditor.EditorGUI.IntField       (position,                              (System.Int32)               value)                  as object;
        if (typeof(T) == typeof(long))                              return static (position, value) => UnityEditor.EditorGUI.LongField      (position,                              (System.Int64)               value)                  as object;
        if (typeof(T) == typeof(string))                            return static (position, value) => UnityEditor.EditorGUI.TextField      (position,                              (System.String)              value)                  as object;
        if (typeof(T) == typeof(UnityEngine.AnimationCurve))        return static (position, value) => UnityEditor.EditorGUI.CurveField     (position,                              (UnityEngine.AnimationCurve) value)                  as object;
        if (typeof(T) == typeof(UnityEngine.Bounds))                return static (position, value) => UnityEditor.EditorGUI.BoundsField    (position,                              (UnityEngine.Bounds)         value)                  as object;
        if (typeof(T) == typeof(UnityEngine.BoundsInt))             return static (position, value) => UnityEditor.EditorGUI.BoundsIntField (position,                              (UnityEngine.BoundsInt)      value)                  as object;
        if (typeof(T) == typeof(UnityEngine.Color))                 return static (position, value) => UnityEditor.EditorGUI.ColorField     (position,                              (UnityEngine.Color)          value)                  as object;
        if (typeof(T) == typeof(UnityEngine.Gradient))              return static (position, value) => UnityEditor.EditorGUI.GradientField  (position,                              (UnityEngine.Gradient)       value)                  as object;
        if (typeof(T) == typeof(UnityEngine.Rect))                  return static (position, value) => UnityEditor.EditorGUI.RectField      (position,                              (UnityEngine.Rect)           value)                  as object;
        if (typeof(T) == typeof(UnityEngine.RectInt))               return static (position, value) => UnityEditor.EditorGUI.RectIntField   (position,                              (UnityEngine.RectInt)        value)                  as object;
        if (typeof(T) == typeof(UnityEngine.Vector2))               return static (position, value) => UnityEditor.EditorGUI.Vector2Field   (position, UnityEngine.GUIContent.none, (UnityEngine.Vector2)        value)                  as object;
        if (typeof(T) == typeof(UnityEngine.Vector2Int))            return static (position, value) => UnityEditor.EditorGUI.Vector2IntField(position, UnityEngine.GUIContent.none, (UnityEngine.Vector2Int)     value)                  as object;
        if (typeof(T) == typeof(UnityEngine.Vector3))               return static (position, value) => UnityEditor.EditorGUI.Vector3Field   (position, UnityEngine.GUIContent.none, (UnityEngine.Vector3)        value)                  as object;
        if (typeof(T) == typeof(UnityEngine.Vector3Int))            return static (position, value) => UnityEditor.EditorGUI.Vector3IntField(position, UnityEngine.GUIContent.none, (UnityEngine.Vector3Int)     value)                  as object;
        if (typeof(T) == typeof(UnityEngine.Vector4))               return static (position, value) => UnityEditor.EditorGUI.Vector4Field   (position, UnityEngine.GUIContent.none, (UnityEngine.Vector4)        value)                  as object;
        if (typeof(T).IsEnum)                                       return static (position, value) => UnityEditor.EditorGUI.EnumPopup      (position,                              (System.Enum)                value)                  as object;
        if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T))) return static (position, value) => UnityEditor.EditorGUI.ObjectField    (position,                              (UnityEngine.Object)         value, typeof(T), true) as object;

        return null;
      }

      [PatchMethod(AggressiveInlining)]
      public static void Ensure() {
        if (EditorDictionary<TKey, TValue>.DelegateGUIField<TKey>  () is null) throw new System.NotSupportedException($"[{UnityEngine.Application.productName}]: Type `{typeof(TKey)}` is not supported for `EditorDictionary`");
        if (EditorDictionary<TKey, TValue>.DelegateGUIField<TValue>() is null) throw new System.NotSupportedException($"[{UnityEngine.Application.productName}]: Type `{typeof(TValue)}` is not supported for `EditorDictionary`");
      }

      private int FindIndex(TKey key) {
        if (key is null)
        throw new System.ArgumentNullException($"Dictionary key is null");

        if (0 != this.buckets.Length) {
          int hash  = this.comparer.GetHashCode(key) & (int) EditorDictionary<TKey, TValue>.PrimeMaximum;
          int index = this.buckets[hash % this.buckets.Length];

          // …
          for (; index >= 0; index = this.next[index]) {
            if (hash == this.hashes[index] && this.comparer.Equals(this.keys[index], key))
            return index;
          }
        }

        return -1;
      }

      [PatchMethod(AggressiveInlining)]
      public EditorDictionary<TKey, TValue>.Enumerator GetEnumerator() {
        return new(this);
      }

      public static uint GetPrime(uint minimum) {
        [PatchMethod(AggressiveInlining)]
        static uint Sqrt(uint number) {
          uint residue = 1u;
          uint root    = 0u;
          uint term;

          // …
          while (residue <= number)
          residue <<= 2;

          while (residue > 1u) {
            residue >>= 2;
            term      = number - residue - root;
            root    >>= 1;

            if (term >= 0u) {
              number = term;
              root  += residue;
            }
          }

          return root;
        }

        /* … */
        if (minimum < 0u)
        throw new System.ArgumentException($"Expected positive dictionary prime");

        foreach (uint prime in EditorDictionary<TKey, TValue>.Primes) {
          if (minimum <= prime)
          return prime;
        }

        for (uint index = minimum | 1u; index < EditorDictionary<TKey, TValue>.PrimeMaximum; index += 2u)
        if (0 != (index & 1u)) {
          uint limit  = Sqrt(index);
          bool primed = true;

          // … ⟶ Magic numbers here — “Bippity bappity boo”! 🪄
          for (uint subindex = 3u; limit >= subindex; subindex += 2u)
          if (0u == index % subindex) {
            primed = false;
            break;
          }

          if (primed && 0u != (index - 1u) % 101u)
          return index;
        }

        return minimum;
      }

      private void Initialize(uint capacity) {
        uint prime = EditorDictionary<TKey, TValue>.GetPrime(capacity);

        // …
        this.buckets  = new int[prime];
        this.freeList = -1;
        this.hashes   = new int   [prime];
        this.keys     = new TKey  [prime];
        this.next     = new int   [prime];
        this.values   = new TValue[prime];

        this.buckets.Fill(-1);
      }

      private void Insert(TKey key, TValue value, bool strict) {
        int freeIndex = 0;
        int hash      = 0;
        int hashIndex = 0;

        // …
        if (key is null)                 throw new System.ArgumentNullException($"Dictionary key is null");
        if (0    == this.buckets.Length) this.Initialize(0u);

        hash      = this.comparer.GetHashCode(key) & (int) EditorDictionary<TKey, TValue>.PrimeMaximum;
        hashIndex = hash % this.buckets.Length;

        for (int index = this.buckets[hashIndex]; index >= 0; ++freeIndex, index = next[index])
        if (hash == this.hashes[index] && this.comparer.Equals(this.keys[index], key)) {
          if (!strict) {
            this.values[index] = value;
            this.version      += 1u;

            return;
          }

          throw new System.ArgumentException($"Dictionary key already exists: `{key}`");
        }

        if (0u != this.freeCount) {
          this.freeCount -= 1u;
          this.freeList   = this.next[freeIndex = this.freeList];
        }

        else {
          if (this.count == this.keys.Length)
            this.Resize(EditorDictionary<TKey, TValue>.CapacityMaximum >> 0 > this.count && EditorDictionary<TKey, TValue>.CapacityMaximum >> 1 < this.count ? EditorDictionary<TKey, TValue>.CapacityMaximum : EditorDictionary<TKey, TValue>.GetPrime(this.count << 1));

          freeIndex = (int) this.count++;
        }

        this.next   [freeIndex] = this.buckets[hashIndex];
        this.buckets[hashIndex] = freeIndex;
        this.hashes [freeIndex] = hash;
        this.keys   [freeIndex] = key;
        this.values [freeIndex] = value;
        this.version           += 1u;
      }

      [PatchMethod(AggressiveInlining)] public bool Remove(System.Collections.Generic.KeyValuePair<TKey, TValue> element) => this.Remove(element.Key);
      public bool Remove(TKey key) {
        int freeIndex = -1;
        int hash      =  0;
        int hashIndex =  0;

        // …
        if (key is null)
          throw new System.ArgumentNullException($"Dictionary key is null");

        hash      = this.comparer.GetHashCode(key) & (int) EditorDictionary<TKey, TValue>.PrimeMaximum;
        hashIndex = hash % this.buckets.Length;

        for (int index = this.buckets[hashIndex]; index >= 0; freeIndex = index = this.next[index])
        if (hash == this.hashes[index] && this.comparer.Equals(this.keys[index], key)) {
          if (freeIndex < 0) this.buckets[hashIndex] = this.next[index];
          else               this.next   [freeIndex] = this.next[index];

          this.version      += 1u;
          this.values[index] = default!;
          this.next  [index] = this.freeList;
          this.keys  [index] = default!;
          this.hashes[index] = -1;
          this.freeList      = index;
          this.freeCount    += 1u;

          return true;
        }

        return false;
      }

      private void Resize(uint capacity) {
        int[]    buckets = new int   [capacity];
        int[]    hashes  = new int   [capacity];
        TKey[]   keys    = new TKey  [capacity];
        int[]    next    = new int   [capacity];
        TValue[] values  = new TValue[capacity];

        // …
        buckets.Fill(-1);

        System.Array.Copy(this.hashes, 0, hashes, 0, this.count);
        System.Array.Copy(this.keys,   0, keys,   0, this.count);
        System.Array.Copy(this.next,   0, next,   0, this.count);
        System.Array.Copy(this.values, 0, values, 0, this.count);

        for (int index = 0; index != this.count; ++index) {
          int hashIndex = hashes[index] % (int) capacity;

          // …
          next   [index]     = buckets[hashIndex];
          buckets[hashIndex] = index;
        }

        this.buckets = buckets;
        this.hashes  = hashes;
        this.keys    = keys;
        this.next    = next;
        this.values  = values;
      }

      [PatchMethod(AggressiveInlining)]
      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
      }

      [PatchMethod(AggressiveInlining)]
      System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>> System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>.GetEnumerator() {
        return this.GetEnumerator();
      }

      [PatchMethod(AggressiveInlining)] public bool TryAdd(System.Collections.Generic.KeyValuePair<TKey, TValue> element) => this.TryAdd(element);
      [PatchMethod(AggressiveInlining)]
      public bool TryAdd(TKey key, TValue value) {
        if (!this.ContainsKey(key)) {
          this.Add(key, value);
          return true;
        }

        return false;
      }

      [PatchMethod(AggressiveInlining)]
      public bool TryGetValue(TKey key, out TValue value) {
        int index = this.FindIndex(key);

        value = index != -1 ? this.values[index] : default!;
        return index != -1;
      }

      public TValue this[TKey key] {
        [PatchMethod(AggressiveInlining)]
        get {
          int index = this.FindIndex(key);

          if (index != -1) return this.values[index];
          throw new System.Collections.Generic.KeyNotFoundException(key?.ToString() ?? "");
        }

        [PatchMethod(AggressiveInlining)]
        set {
          this.Insert(key, value, false);
        }
      }

      public TValue this[TKey key, TValue _] {
        [PatchMethod(AggressiveInlining)]
        get {
          int index = this.FindIndex(key);
          return index != -1 ? this.values[index] : _;
        }
      }
    }
      [System.Serializable] public class AnimationCurveDictionary : PatchOdyssey.Collections.EditorDictionary<string, UnityEngine.AnimationCurve> { [PatchMethod(AggressiveInlining)] public AnimationCurveDictionary() : base() {} [PatchMethod(AggressiveInlining)] public AnimationCurveDictionary(int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public AnimationCurveDictionary(System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public AnimationCurveDictionary(System.Collections.Generic.IDictionary<string, UnityEngine.AnimationCurve> dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public AnimationCurveDictionary(int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public AnimationCurveDictionary(System.Collections.Generic.IDictionary<string, UnityEngine.AnimationCurve> dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class BooleanDictionary        : PatchOdyssey.Collections.EditorDictionary<string, System     .Boolean>        { [PatchMethod(AggressiveInlining)] public BooleanDictionary       () : base() {} [PatchMethod(AggressiveInlining)] public BooleanDictionary       (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public BooleanDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public BooleanDictionary       (System.Collections.Generic.IDictionary<string, System     .Boolean>        dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public BooleanDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public BooleanDictionary       (System.Collections.Generic.IDictionary<string, System     .Boolean>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class BoundsDictionary         : PatchOdyssey.Collections.EditorDictionary<string, UnityEngine.Bounds>         { [PatchMethod(AggressiveInlining)] public BoundsDictionary        () : base() {} [PatchMethod(AggressiveInlining)] public BoundsDictionary        (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public BoundsDictionary        (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public BoundsDictionary        (System.Collections.Generic.IDictionary<string, UnityEngine.Bounds>         dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public BoundsDictionary        (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public BoundsDictionary        (System.Collections.Generic.IDictionary<string, UnityEngine.Bounds>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class BoundsIntDictionary      : PatchOdyssey.Collections.EditorDictionary<string, UnityEngine.BoundsInt>      { [PatchMethod(AggressiveInlining)] public BoundsIntDictionary     () : base() {} [PatchMethod(AggressiveInlining)] public BoundsIntDictionary     (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public BoundsIntDictionary     (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public BoundsIntDictionary     (System.Collections.Generic.IDictionary<string, UnityEngine.BoundsInt>      dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public BoundsIntDictionary     (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public BoundsIntDictionary     (System.Collections.Generic.IDictionary<string, UnityEngine.BoundsInt>      dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class ColorDictionary          : PatchOdyssey.Collections.EditorDictionary<string, UnityEngine.Color>          { [PatchMethod(AggressiveInlining)] public ColorDictionary         () : base() {} [PatchMethod(AggressiveInlining)] public ColorDictionary         (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public ColorDictionary         (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public ColorDictionary         (System.Collections.Generic.IDictionary<string, UnityEngine.Color>          dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public ColorDictionary         (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public ColorDictionary         (System.Collections.Generic.IDictionary<string, UnityEngine.Color>          dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class DoubleDictionary         : PatchOdyssey.Collections.EditorDictionary<string, System     .Double>         { [PatchMethod(AggressiveInlining)] public DoubleDictionary        () : base() {} [PatchMethod(AggressiveInlining)] public DoubleDictionary        (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public DoubleDictionary        (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public DoubleDictionary        (System.Collections.Generic.IDictionary<string, System     .Double>         dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public DoubleDictionary        (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public DoubleDictionary        (System.Collections.Generic.IDictionary<string, System     .Double>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class FloatDictionary          : PatchOdyssey.Collections.EditorDictionary<string, System     .Single>         { [PatchMethod(AggressiveInlining)] public FloatDictionary         () : base() {} [PatchMethod(AggressiveInlining)] public FloatDictionary         (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public FloatDictionary         (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public FloatDictionary         (System.Collections.Generic.IDictionary<string, System     .Single>         dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public FloatDictionary         (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public FloatDictionary         (System.Collections.Generic.IDictionary<string, System     .Single>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class GameObjectDictionary     : PatchOdyssey.Collections.EditorDictionary<string, UnityEngine.GameObject>     { [PatchMethod(AggressiveInlining)] public GameObjectDictionary    () : base() {} [PatchMethod(AggressiveInlining)] public GameObjectDictionary    (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public GameObjectDictionary    (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public GameObjectDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.GameObject>     dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public GameObjectDictionary    (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public GameObjectDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.GameObject>     dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class GradientDictionary       : PatchOdyssey.Collections.EditorDictionary<string, UnityEngine.Gradient>       { [PatchMethod(AggressiveInlining)] public GradientDictionary      () : base() {} [PatchMethod(AggressiveInlining)] public GradientDictionary      (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public GradientDictionary      (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public GradientDictionary      (System.Collections.Generic.IDictionary<string, UnityEngine.Gradient>       dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public GradientDictionary      (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public GradientDictionary      (System.Collections.Generic.IDictionary<string, UnityEngine.Gradient>       dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class IntDictionary            : PatchOdyssey.Collections.EditorDictionary<string, System     .Int32>          { [PatchMethod(AggressiveInlining)] public IntDictionary           () : base() {} [PatchMethod(AggressiveInlining)] public IntDictionary           (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public IntDictionary           (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public IntDictionary           (System.Collections.Generic.IDictionary<string, System     .Int32>          dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public IntDictionary           (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public IntDictionary           (System.Collections.Generic.IDictionary<string, System     .Int32>          dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class LongDictionary           : PatchOdyssey.Collections.EditorDictionary<string, System     .Int64>          { [PatchMethod(AggressiveInlining)] public LongDictionary          () : base() {} [PatchMethod(AggressiveInlining)] public LongDictionary          (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public LongDictionary          (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public LongDictionary          (System.Collections.Generic.IDictionary<string, System     .Int64>          dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public LongDictionary          (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public LongDictionary          (System.Collections.Generic.IDictionary<string, System     .Int64>          dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class RectDictionary           : PatchOdyssey.Collections.EditorDictionary<string, UnityEngine.Rect>           { [PatchMethod(AggressiveInlining)] public RectDictionary          () : base() {} [PatchMethod(AggressiveInlining)] public RectDictionary          (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public RectDictionary          (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public RectDictionary          (System.Collections.Generic.IDictionary<string, UnityEngine.Rect>           dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public RectDictionary          (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public RectDictionary          (System.Collections.Generic.IDictionary<string, UnityEngine.Rect>           dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class RectIntDictionary        : PatchOdyssey.Collections.EditorDictionary<string, UnityEngine.RectInt>        { [PatchMethod(AggressiveInlining)] public RectIntDictionary       () : base() {} [PatchMethod(AggressiveInlining)] public RectIntDictionary       (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public RectIntDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public RectIntDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.RectInt>        dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public RectIntDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public RectIntDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.RectInt>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class StringDictionary         : PatchOdyssey.Collections.EditorDictionary<string, System     .String>         { [PatchMethod(AggressiveInlining)] public StringDictionary        () : base() {} [PatchMethod(AggressiveInlining)] public StringDictionary        (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public StringDictionary        (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public StringDictionary        (System.Collections.Generic.IDictionary<string, System     .String>         dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public StringDictionary        (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public StringDictionary        (System.Collections.Generic.IDictionary<string, System     .String>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class UIntDictionary           : PatchOdyssey.Collections.EditorDictionary<string, System     .UInt32>         { [PatchMethod(AggressiveInlining)] public UIntDictionary          () : base() {} [PatchMethod(AggressiveInlining)] public UIntDictionary          (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public UIntDictionary          (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public UIntDictionary          (System.Collections.Generic.IDictionary<string, System     .UInt32>         dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public UIntDictionary          (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public UIntDictionary          (System.Collections.Generic.IDictionary<string, System     .UInt32>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class ULongDictionary          : PatchOdyssey.Collections.EditorDictionary<string, System     .UInt64>         { [PatchMethod(AggressiveInlining)] public ULongDictionary         () : base() {} [PatchMethod(AggressiveInlining)] public ULongDictionary         (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public ULongDictionary         (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public ULongDictionary         (System.Collections.Generic.IDictionary<string, System     .UInt64>         dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public ULongDictionary         (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public ULongDictionary         (System.Collections.Generic.IDictionary<string, System     .UInt64>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class Vector2Dictionary        : PatchOdyssey.Collections.EditorDictionary<string, UnityEngine.Vector2>        { [PatchMethod(AggressiveInlining)] public Vector2Dictionary       () : base() {} [PatchMethod(AggressiveInlining)] public Vector2Dictionary       (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public Vector2Dictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public Vector2Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2>        dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public Vector2Dictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public Vector2Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class Vector2IntDictionary     : PatchOdyssey.Collections.EditorDictionary<string, UnityEngine.Vector2Int>     { [PatchMethod(AggressiveInlining)] public Vector2IntDictionary    () : base() {} [PatchMethod(AggressiveInlining)] public Vector2IntDictionary    (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public Vector2IntDictionary    (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public Vector2IntDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2Int>     dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public Vector2IntDictionary    (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public Vector2IntDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2Int>     dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class Vector3Dictionary        : PatchOdyssey.Collections.EditorDictionary<string, UnityEngine.Vector3>        { [PatchMethod(AggressiveInlining)] public Vector3Dictionary       () : base() {} [PatchMethod(AggressiveInlining)] public Vector3Dictionary       (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public Vector3Dictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public Vector3Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3>        dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public Vector3Dictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public Vector3Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class Vector3IntDictionary     : PatchOdyssey.Collections.EditorDictionary<string, UnityEngine.Vector3Int>     { [PatchMethod(AggressiveInlining)] public Vector3IntDictionary    () : base() {} [PatchMethod(AggressiveInlining)] public Vector3IntDictionary    (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public Vector3IntDictionary    (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public Vector3IntDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3Int>     dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public Vector3IntDictionary    (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public Vector3IntDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3Int>     dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class Vector4Dictionary        : PatchOdyssey.Collections.EditorDictionary<string, UnityEngine.Vector4>        { [PatchMethod(AggressiveInlining)] public Vector4Dictionary       () : base() {} [PatchMethod(AggressiveInlining)] public Vector4Dictionary       (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public Vector4Dictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public Vector4Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector4>        dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public Vector4Dictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public Vector4Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector4>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }

    [System.Serializable]
    public class EditorReadOnlyDictionary<TKey, TValue> : PatchOdyssey.Collections.EditorDictionary<TKey, TValue>, System.Collections.Generic.IReadOnlyDictionary<TKey, TValue> {
      private           uint                                           initializerListCount =  0u;
      public  new const bool                                           IsReadOnly           =  true;
      public  new       System.Collections.Generic.IEnumerable<TKey>   Keys                 => base.Keys;
      public  new       System.Collections.Generic.IEnumerable<TValue> Values               => base.Values;

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)] public EditorReadOnlyDictionary()                                                                                                                                                                            : base()                     {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public EditorReadOnlyDictionary(int                                                                                                 capacity)                                                                : base(capacity)             { this.initializerListCount = (uint) capacity; }
      [PatchConstructor, PatchMethod(AggressiveInlining)] public EditorReadOnlyDictionary(System.Collections.Generic.IEqualityComparer<TKey>                                                  comparer)                                                                : base(comparer)             {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public EditorReadOnlyDictionary(System.Collections.Generic.IDictionary      <TKey, TValue>                                          dictionary)                                                              : base(dictionary)           {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public EditorReadOnlyDictionary(System.Collections.Generic.IEnumerable      <System.Collections.Generic.KeyValuePair<TKey, TValue>> enumerable)                                                              : base(enumerable)           {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public EditorReadOnlyDictionary(int                                                                                                 capacity,   System.Collections.Generic.IEqualityComparer<TKey> comparer) : base(capacity,   comparer) { this.initializerListCount = (uint) capacity; }
      [PatchConstructor, PatchMethod(AggressiveInlining)] public EditorReadOnlyDictionary(System.Collections.Generic.IDictionary      <TKey, TValue>                                          dictionary, System.Collections.Generic.IEqualityComparer<TKey> comparer) : base(dictionary, comparer) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public EditorReadOnlyDictionary(System.Collections.Generic.IEnumerable      <System.Collections.Generic.KeyValuePair<TKey, TValue>> enumerable, System.Collections.Generic.IEqualityComparer<TKey> comparer) : base(enumerable, comparer) {}

      /* … ⟶ Intended for initializer lists only */
      [PatchMethod(AggressiveInlining)] public new void Add(System.Collections.Generic.KeyValuePair<TKey, TValue> element) => this.Add(element.Key, element.Value);
      [PatchMethod(AggressiveInlining)]
      public new void Add(TKey key, TValue value) {
        if (0u != this.initializerListCount--) {
          base.Add(key, value);

          if (0u == this.initializerListCount)
          this.TrimExcess(/* this.Count */);
        } else throw new System.NotSupportedException("Can not add item to `EditorReadOnlyDictionary`");
      }

      [PatchMethod(AggressiveInlining)] public  new void AddRange(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>> enumerable) { throw new System.NotSupportedException("Can not add item to `EditorReadOnlyDictionary`"); }
      [PatchMethod(AggressiveInlining)] private new void Clear   ()                                                                                                         {}
      [PatchMethod(AggressiveInlining)] private new bool Remove  (TKey                                                  key)                                                => false;
      [PatchMethod(AggressiveInlining)] private new bool Remove  (System.Collections.Generic.KeyValuePair<TKey, TValue> element)                                            => false;
      [PatchMethod(AggressiveInlining)] public  new bool TryAdd  (TKey                                                  key, TValue value)                                  { throw new System.NotSupportedException("Can not add item to `EditorReadOnlyDictionary`"); }
      [PatchMethod(AggressiveInlining)] public  new bool TryAdd  (System.Collections.Generic.KeyValuePair<TKey, TValue> element)                                            => this.TryAdd(element.Key, element.Value);
    }
      [System.Serializable] public class AnimationCurveReadOnlyDictionary : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, UnityEngine.AnimationCurve> { [PatchMethod(AggressiveInlining)] public AnimationCurveReadOnlyDictionary(int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public AnimationCurveReadOnlyDictionary(System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public AnimationCurveReadOnlyDictionary(System.Collections.Generic.IDictionary<string, UnityEngine.AnimationCurve> dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public AnimationCurveReadOnlyDictionary(int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public AnimationCurveReadOnlyDictionary(System.Collections.Generic.IDictionary<string, UnityEngine.AnimationCurve> dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class BooleanReadOnlyDictionary        : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, System     .Boolean>        { [PatchMethod(AggressiveInlining)] public BooleanReadOnlyDictionary       (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public BooleanReadOnlyDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public BooleanReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, System     .Boolean>        dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public BooleanReadOnlyDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public BooleanReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, System     .Boolean>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class BoundsReadOnlyDictionary         : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, UnityEngine.Bounds>         { [PatchMethod(AggressiveInlining)] public BoundsReadOnlyDictionary        (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public BoundsReadOnlyDictionary        (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public BoundsReadOnlyDictionary        (System.Collections.Generic.IDictionary<string, UnityEngine.Bounds>         dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public BoundsReadOnlyDictionary        (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public BoundsReadOnlyDictionary        (System.Collections.Generic.IDictionary<string, UnityEngine.Bounds>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class BoundsIntReadOnlyDictionary      : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, UnityEngine.BoundsInt>      { [PatchMethod(AggressiveInlining)] public BoundsIntReadOnlyDictionary     (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public BoundsIntReadOnlyDictionary     (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public BoundsIntReadOnlyDictionary     (System.Collections.Generic.IDictionary<string, UnityEngine.BoundsInt>      dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public BoundsIntReadOnlyDictionary     (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public BoundsIntReadOnlyDictionary     (System.Collections.Generic.IDictionary<string, UnityEngine.BoundsInt>      dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class ColorReadOnlyDictionary          : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, UnityEngine.Color>          { [PatchMethod(AggressiveInlining)] public ColorReadOnlyDictionary         (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public ColorReadOnlyDictionary         (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public ColorReadOnlyDictionary         (System.Collections.Generic.IDictionary<string, UnityEngine.Color>          dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public ColorReadOnlyDictionary         (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public ColorReadOnlyDictionary         (System.Collections.Generic.IDictionary<string, UnityEngine.Color>          dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class DoubleReadOnlyDictionary         : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, System     .Double>         { [PatchMethod(AggressiveInlining)] public DoubleReadOnlyDictionary        (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public DoubleReadOnlyDictionary        (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public DoubleReadOnlyDictionary        (System.Collections.Generic.IDictionary<string, System     .Double>         dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public DoubleReadOnlyDictionary        (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public DoubleReadOnlyDictionary        (System.Collections.Generic.IDictionary<string, System     .Double>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class FloatReadOnlyDictionary          : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, System     .Single>         { [PatchMethod(AggressiveInlining)] public FloatReadOnlyDictionary         (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public FloatReadOnlyDictionary         (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public FloatReadOnlyDictionary         (System.Collections.Generic.IDictionary<string, System     .Single>         dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public FloatReadOnlyDictionary         (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public FloatReadOnlyDictionary         (System.Collections.Generic.IDictionary<string, System     .Single>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class GameObjectReadOnlyDictionary     : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, UnityEngine.GameObject>     { [PatchMethod(AggressiveInlining)] public GameObjectReadOnlyDictionary    (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public GameObjectReadOnlyDictionary    (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public GameObjectReadOnlyDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.GameObject>     dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public GameObjectReadOnlyDictionary    (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public GameObjectReadOnlyDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.GameObject>     dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class GradientReadOnlyDictionary       : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, UnityEngine.Gradient>       { [PatchMethod(AggressiveInlining)] public GradientReadOnlyDictionary      (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public GradientReadOnlyDictionary      (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public GradientReadOnlyDictionary      (System.Collections.Generic.IDictionary<string, UnityEngine.Gradient>       dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public GradientReadOnlyDictionary      (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public GradientReadOnlyDictionary      (System.Collections.Generic.IDictionary<string, UnityEngine.Gradient>       dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class IntReadOnlyDictionary            : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, System     .Int32>          { [PatchMethod(AggressiveInlining)] public IntReadOnlyDictionary           (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public IntReadOnlyDictionary           (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public IntReadOnlyDictionary           (System.Collections.Generic.IDictionary<string, System     .Int32>          dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public IntReadOnlyDictionary           (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public IntReadOnlyDictionary           (System.Collections.Generic.IDictionary<string, System     .Int32>          dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class LongReadOnlyDictionary           : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, System     .Int64>          { [PatchMethod(AggressiveInlining)] public LongReadOnlyDictionary          (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public LongReadOnlyDictionary          (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public LongReadOnlyDictionary          (System.Collections.Generic.IDictionary<string, System     .Int64>          dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public LongReadOnlyDictionary          (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public LongReadOnlyDictionary          (System.Collections.Generic.IDictionary<string, System     .Int64>          dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class RectReadOnlyDictionary           : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, UnityEngine.Rect>           { [PatchMethod(AggressiveInlining)] public RectReadOnlyDictionary          (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public RectReadOnlyDictionary          (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public RectReadOnlyDictionary          (System.Collections.Generic.IDictionary<string, UnityEngine.Rect>           dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public RectReadOnlyDictionary          (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public RectReadOnlyDictionary          (System.Collections.Generic.IDictionary<string, UnityEngine.Rect>           dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class RectIntReadOnlyDictionary        : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, UnityEngine.RectInt>        { [PatchMethod(AggressiveInlining)] public RectIntReadOnlyDictionary       (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public RectIntReadOnlyDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public RectIntReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.RectInt>        dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public RectIntReadOnlyDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public RectIntReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.RectInt>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class StringReadOnlyDictionary         : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, System     .String>         { [PatchMethod(AggressiveInlining)] public StringReadOnlyDictionary        (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public StringReadOnlyDictionary        (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public StringReadOnlyDictionary        (System.Collections.Generic.IDictionary<string, System     .String>         dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public StringReadOnlyDictionary        (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public StringReadOnlyDictionary        (System.Collections.Generic.IDictionary<string, System     .String>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class UIntReadOnlyDictionary           : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, System     .UInt32>         { [PatchMethod(AggressiveInlining)] public UIntReadOnlyDictionary          (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public UIntReadOnlyDictionary          (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public UIntReadOnlyDictionary          (System.Collections.Generic.IDictionary<string, System     .UInt32>         dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public UIntReadOnlyDictionary          (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public UIntReadOnlyDictionary          (System.Collections.Generic.IDictionary<string, System     .UInt32>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class ULongReadOnlyDictionary          : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, System     .UInt64>         { [PatchMethod(AggressiveInlining)] public ULongReadOnlyDictionary         (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public ULongReadOnlyDictionary         (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public ULongReadOnlyDictionary         (System.Collections.Generic.IDictionary<string, System     .UInt64>         dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public ULongReadOnlyDictionary         (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public ULongReadOnlyDictionary         (System.Collections.Generic.IDictionary<string, System     .UInt64>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class Vector2ReadOnlyDictionary        : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, UnityEngine.Vector2>        { [PatchMethod(AggressiveInlining)] public Vector2ReadOnlyDictionary       (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public Vector2ReadOnlyDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public Vector2ReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2>        dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public Vector2ReadOnlyDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public Vector2ReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class Vector2IntReadOnlyDictionary     : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, UnityEngine.Vector2Int>     { [PatchMethod(AggressiveInlining)] public Vector2IntReadOnlyDictionary    (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public Vector2IntReadOnlyDictionary    (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public Vector2IntReadOnlyDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2Int>     dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public Vector2IntReadOnlyDictionary    (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public Vector2IntReadOnlyDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2Int>     dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class Vector3ReadOnlyDictionary        : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, UnityEngine.Vector3>        { [PatchMethod(AggressiveInlining)] public Vector3ReadOnlyDictionary       (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public Vector3ReadOnlyDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public Vector3ReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3>        dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public Vector3ReadOnlyDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public Vector3ReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class Vector3IntReadOnlyDictionary     : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, UnityEngine.Vector3Int>     { [PatchMethod(AggressiveInlining)] public Vector3IntReadOnlyDictionary    (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public Vector3IntReadOnlyDictionary    (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public Vector3IntReadOnlyDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3Int>     dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public Vector3IntReadOnlyDictionary    (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public Vector3IntReadOnlyDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3Int>     dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class Vector4ReadOnlyDictionary        : PatchOdyssey.Collections.EditorReadOnlyDictionary<string, UnityEngine.Vector4>        { [PatchMethod(AggressiveInlining)] public Vector4ReadOnlyDictionary       (int capacity) : base(capacity) {} [PatchMethod(AggressiveInlining)] public Vector4ReadOnlyDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchMethod(AggressiveInlining)] public Vector4ReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector4>        dictionary) : base(dictionary) {} [PatchMethod(AggressiveInlining)] public Vector4ReadOnlyDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchMethod(AggressiveInlining)] public Vector4ReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector4>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }

    public class Event : System.EventArgs {
      public /* required */ object? data { get; internal set; } = null;

      [PatchConstructor, PatchMethod(AggressiveInlining)]
      public Event() {}
    }
      public class WaitForTimerEvent : PatchOdyssey.Collections.Event {
        public new (double delay, double timestamp) data { get; internal set; } = (0.0, 0.0);

        public double delay     { get => this.data.delay;     set => this.data = (value, this.data.timestamp); } // ⟶ Specified delay
        public double timestamp { get => this.data.timestamp; set => this.data = (this.data.delay, value); }     // ⟶ Next available timestamp to signal a `WaitForTimer` event (which could be in the past chronologically)

        /* … */
        [PatchConstructor, PatchMethod(AggressiveInlining)] public WaitForTimerEvent() {}
      }

    public static class EventHandler /* : PatchOdyssey.Collections.EventHandler<PatchOdyssey.Collections.Event> */ {
      // ⟶ Based on by `System.Delegate.Combine(𝑓, 𝑓)`
      [PatchMethod(AggressiveInlining)]
      public static void Combine<T>(in PatchOdyssey.Collections.EventHandler<T> eventHandler, in PatchOdyssey.Collections.HandlerInfo<T> handler) where T : PatchOdyssey.Collections.Event, new() {
        if (EventHandler<T>.DefaultHandlerValue != (handler.value ?? EventHandler<T>.DefaultHandlerValue))
        eventHandler.handlers.Add(handler);
      }

      // ⟶ Based on by `System.Delegate.Remove(𝑓, 𝑓)`
      [PatchMethod(AggressiveInlining)]
      public static void Remove<T>(in PatchOdyssey.Collections.EventHandler<T> eventHandler, in PatchOdyssey.Collections.HandlerInfo<T> handler) where T : PatchOdyssey.Collections.Event, new() {
        for (int index = eventHandler.handlers.Count; 0 != index--; )
        if (eventHandler.handlers[index].value == handler.value) {
          eventHandler.handlers.RemoveAt(index);
          return;
        }
      }
    }

    public readonly struct EventHandler<T> where T : PatchOdyssey.Collections.Event, new() /* ⟶ `event` @ `https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/event` */ {
      internal static readonly PatchOdyssey.Handler<T>                                                  DefaultHandlerValue = PatchOdyssey.Collections.HandlerInfo<T>.DefaultValue;
      internal        readonly System.Collections.Generic.List<PatchOdyssey.Collections.HandlerInfo<T>> handlers            = new(1); // ⟶ Private composition over inheritance

      /* … ⟶ See `struct PatchOdyssey.Collections.HandlerInfo` constructor */
      [PatchConstructor, PatchMethod(AggressiveInlining)]                     public EventHandler()                                                     {}
      [PatchConstructor, PatchMethod(AggressiveInlining)]                     public EventHandler(EventHandler                        <T> eventHandler) { if (eventHandler != this) this.handlers.AddRange(eventHandler.handlers); }
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public EventHandler(PatchOdyssey.Collections.HandlerInfo<T> handler)      { PatchOdyssey.Collections.EventHandler.Combine(this, handler); }

      /* … */
      [PatchMethod(AggressiveInlining)] public /* virtual */ object            Clone            ()                            => new EventHandler<T>(this);
      [PatchMethod(AggressiveInlining)] public               void              DynamicInvoke    (params object?[]? arguments) { foreach (PatchOdyssey.Collections.HandlerInfo<T> handler in this.handlers) handler.DynamicInvoke(); }
      [PatchMethod(AggressiveInlining)] public override      bool              Equals           (object?           value)     => base.Equals     (value);
      [PatchMethod(AggressiveInlining)] public override      int               GetHashCode      ()                            => base.GetHashCode();
      [PatchMethod(AggressiveInlining)] public /* virtual */ System.Delegate[] GetInvocationList()                            => this.handlers.ConvertAll(static handler => (System.Delegate) (() => handler.Invoke())).ToArray(); // ⟶ Caching unnecessary here
      [PatchMethod(AggressiveInlining)] public               void              Invoke           ()                            { foreach (PatchOdyssey.Collections.HandlerInfo<T> handler in this.handlers) handler.Invoke(); }

      /* … */
      [PatchMethod(AggressiveInlining)] public static EventHandler<T> operator +(EventHandler<T> eventHandler, PatchOdyssey.Collections.HandlerInfo<T> handler) { PatchOdyssey.Collections.EventHandler.Combine(eventHandler, handler); return eventHandler; }
      [PatchMethod(AggressiveInlining)] public static EventHandler<T> operator -(EventHandler<T> eventHandler, PatchOdyssey.Collections.HandlerInfo<T> handler) { PatchOdyssey.Collections.EventHandler.Remove (eventHandler, handler); return eventHandler; }

      [PatchMethod(AggressiveInlining)] public static implicit operator EventHandler<T>(System.ValueTuple<PatchOdyssey.Handler<T>, object?, T> tuple)        => new(new PatchOdyssey.Collections.HandlerInfo<T>(tuple.Item1, tuple.Item2, tuple.Item3));
      [PatchMethod(AggressiveInlining)] public static implicit operator System.Action  (EventHandler<T>                                        eventHandler) => () => eventHandler.Invoke();
    }

    public struct GameObjectEnumerator : System.Collections.Generic.IEnumerator<UnityEngine.GameObject>, System.Collections.Generic.IEnumerable<UnityEngine.GameObject> /* ⟶ Ranged `foreach …` shorthand support */ {
      private enum HierarchyCurrent : sbyte { Unmoved = -1, Pending,     Moved };
      public  enum Kind             : byte  { Children,     Descendants, Hierarchy };

      public           UnityEngine.GameObject                                   Current                                => (GameObjectEnumerator.HierarchyCurrent.Pending == this.uproot ? this.root : (UnityEngine.Transform) this.subenumerator.Current).gameObject;
      private readonly GameObjectEnumerator.Kind                                kind                                   =  default;
      private readonly System.Collections.Generic.Queue<UnityEngine.Transform>? hierarchy                              =  null;
      private readonly UnityEngine.Transform                                    root                                   =  null!;
      private          System.Collections.IEnumerator                           subenumerator                          =  null!;
      private          GameObjectEnumerator.HierarchyCurrent                    uproot                                 =  GameObjectEnumerator.HierarchyCurrent.Unmoved;
      object                                                                    System.Collections.IEnumerator.Current => this.Current;

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)]
      internal GameObjectEnumerator(GameObjectEnumerator.Kind kind, UnityEngine.Transform gameObjectTransform) {
        this.kind          = kind;
        this.root          = gameObjectTransform;
        this.subenumerator = gameObjectTransform.GetEnumerator();

        switch (this.kind) {
          case GameObjectEnumerator.Kind.Children   :                                                                                                                       break;
          case GameObjectEnumerator.Kind.Descendants: this.hierarchy = new(gameObjectTransform.childCount) {};                                                              break;
          case GameObjectEnumerator.Kind.Hierarchy  : this.hierarchy = new(gameObjectTransform.childCount) {}; this.uproot = GameObjectEnumerator.HierarchyCurrent.Unmoved; break;
        }
      }

      /* … */
      [PatchMethod(AggressiveInlining)]
      public void Dispose() { /* ⟶ `System.Collections.IEnumerator::Dispose()` unneeded */ }

      [PatchMethod(AggressiveInlining)]
      public GameObjectEnumerator GetEnumerator() {
        return this;
      }

      public bool MoveNext() {
        switch (kind) {
          case GameObjectEnumerator.Kind.Children:
            return this.subenumerator.MoveNext();

          case GameObjectEnumerator.Kind.Descendants:
            if (this.subenumerator.MoveNext()) {
              this.hierarchy!.Enqueue((UnityEngine.Transform) this.subenumerator.Current);
              return true;
            }

            do {
              if (0 == this.hierarchy!.Count)
              return false;

              this.subenumerator = this.hierarchy!.Dequeue().GetEnumerator();
            } while (!this.subenumerator.MoveNext()); // ⟶ Move `System.Collections.IEnumerator` to first enumerable item — if existing

            return true;

          case GameObjectEnumerator.Kind.Hierarchy:
            switch (this.uproot) {
              case GameObjectEnumerator.HierarchyCurrent.Moved  :                                                              break;
              case GameObjectEnumerator.HierarchyCurrent.Pending: this.uproot = GameObjectEnumerator.HierarchyCurrent.Moved;   break;
              case GameObjectEnumerator.HierarchyCurrent.Unmoved: this.uproot = GameObjectEnumerator.HierarchyCurrent.Pending; return true;
            }

            goto case GameObjectEnumerator.Kind.Descendants;
        }

        return false;
      }

      void System.Collections.IEnumerator.Reset() {
        this.hierarchy?.Clear();
        this.subenumerator = this.root.GetEnumerator(); // ⟶ `System.Collections.IEnumerator::Reset()` unneeded
        this.uproot        = GameObjectEnumerator.HierarchyCurrent.Unmoved;
      }

      [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator                                 System.Collections.IEnumerable.GetEnumerator                                () => this.GetEnumerator();
      [PatchMethod(AggressiveInlining)] System.Collections.Generic.IEnumerator<UnityEngine.GameObject> System.Collections.Generic.IEnumerable<UnityEngine.GameObject>.GetEnumerator() => this.GetEnumerator();
    }

    public readonly struct HandlerInfo<T> /* : System.Delegate */ where T : PatchOdyssey.Collections.Event, new() {
      internal readonly PatchOdyssey.Handler<T> value    = HandlerInfo<T>.DefaultValue; // ⟶ Callback function(s)
      internal readonly object?                 target   = null;                        // ⟶ Callback target
      internal readonly T                       metadata = new();                       // ⟶ Callback data

      /* … ⟶ See `struct PatchOdyssey.Collections.EventHandler` constructor */
      [PatchConstructor, PatchMethod(AggressiveInlining)] public HandlerInfo(HandlerInfo<T> handler) : this(handler.value, handler.target, handler.metadata) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)]
      public HandlerInfo(PatchOdyssey.Handler<T> value, object? target, T metadata) {
        this.metadata = metadata;
        this.target   = target;
        this.value    = value ?? HandlerInfo<T>.DefaultValue;
      }

      /* … */
      [PatchMethod(AggressiveInlining)] internal /* virtual */ object            Clone            ()                                 => new HandlerInfo<T>((PatchOdyssey.Handler<T>) this.value.Clone(), this.target, new() {data = this.metadata.data});
      [PatchMethod(NoInlining)]         internal static        void              DefaultValue     (object?           target, T data) {}
      [PatchMethod(AggressiveInlining)] internal               void              DynamicInvoke    (params object?[]? arguments)      => this.value.DynamicInvoke(new object?[] {this.target, this.metadata});
      [PatchMethod(AggressiveInlining)] public   override      bool              Equals           (object?           value)          => this.value.Equals           (value);
      [PatchMethod(AggressiveInlining)] public   override      int               GetHashCode      ()                                 => this.value.GetHashCode      ();
      [PatchMethod(AggressiveInlining)] internal /* virtual */ System.Delegate[] GetInvocationList()                                 => this.value.GetInvocationList();
      [PatchMethod(AggressiveInlining)] internal               void              Invoke           ()                                 => this.value.Invoke           (this.target, this.metadata);

      /* … */
      [PatchMethod(AggressiveInlining)] public static implicit operator PatchOdyssey.Handler<T>(HandlerInfo<T> handler) => handler.value;
    }

    public class SharedList<T> : System.Collections.Generic.IEnumerable<T> {
      public                    int                                Count => LIST.Count;
      protected internal static System.Collections.Generic.List<T> LIST  =  new();

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(uint                                      capacity = 0u)                                                                                                { LIST.Capacity = System.Math.Max(LIST.Capacity, (int) capacity); }
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(T[]                                       array)                   : this(array,                  (uint) array       .Length)                           {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(System.Array                              array)                   : this(array,                  (uint) array       .Length)                           {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(System.ArraySegment<T>                    arraySegment)            : this(arraySegment,           (uint) arraySegment.Count)                            {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(System.Collections.ArrayList              arrayList)               : this(arrayList,              (uint) arrayList   .Count)                            {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(System.Collections.Generic.HashSet    <T> hashset)                 : this(hashset,                (uint) hashset     .Count)                            {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(System.Collections.Generic.IEnumerable<T> enumerable)              : this(enumerable,             (uint) SharedList<T>.GetEnumerableLength(enumerable)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(System.Collections.Generic.LinkedList <T> list)                    : this(list,                   (uint) list     .Count)                               {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(System.Collections.Generic.List       <T> list)                    : this(list,                   (uint) list     .Count)                               {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(System.Collections.Generic.Queue      <T> queue)                   : this(queue,                  (uint) queue    .Count)                               {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(System.Collections.Generic.SortedSet  <T> sortedSet)               : this(sortedSet,              (uint) sortedSet.Count)                               {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(System.Collections.Generic.Stack      <T> stack)                   : this(stack,                  (uint) stack    .Count)                               {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(System.Collections.Hashtable              hashtable)               : this(hashtable,              (uint) hashtable.Count)                               {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(System.Collections.IEnumerable            enumerable)              : this(enumerable,             (uint) SharedList<T>.GetEnumerableLength(enumerable)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(System.Collections.Queue                  queue)                   : this(queue,                  (uint) queue     .Count)                              {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(System.Collections.SortedList             sortedList)              : this(sortedList,             (uint) sortedList.Count)                              {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(System.Collections.Stack                  stack)                   : this(stack,                  (uint) stack     .Count)                              {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(System.Memory        <T>                  memory)                  : this(Util.ArrayFrom(memory), (uint) memory    .Length)                             {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(System.ReadOnlyMemory<T>                  memory)                  : this(Util.ArrayFrom(memory), (uint) memory    .Length)                             {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(System.Collections.IEnumerable            enumerable, uint length) : this(length)                                                                       { if (enumerable is SharedList<T>) return; LIST.Clear(); foreach (T value in enumerable) LIST.Add(value); }
      [PatchConstructor, PatchMethod(AggressiveInlining)] public SharedList(System.Collections.Generic.IEnumerable<T> enumerable, uint length) : this(length)                                                                       { if (enumerable is SharedList<T>) return; LIST.Clear(); LIST.AddRange(enumerable); }

      /* … */
      [PatchMethod(AggressiveInlining)] public void                                                 Add          (T                                         element)                                                             =>     LIST.Add          (element);
      [PatchMethod(AggressiveInlining)] public void                                                 AddRange     (System.Collections.Generic.IEnumerable<T> enumerable)                                                          =>     LIST.AddRange     (enumerable);
      [PatchMethod(AggressiveInlining)] public T                                                    Append       (T                                         element)                                                             =>     LIST.Append       (element);
      [PatchMethod(AggressiveInlining)] public System.Collections.ObjectModel.ReadOnlyCollection<T> AsReadOnly   ()                                                                                                              =>     LIST.AsReadOnly   ();
      [PatchMethod(AggressiveInlining)] public int                                                  BinarySearch (T    element)                                                                                                  =>     LIST.BinarySearch (element);
      [PatchMethod(AggressiveInlining)] public int                                                  BinarySearch (T    element,                      System.Collections.Generic.IComparer<T>? comparer)                          =>     LIST.BinarySearch (element, comparer);
      [PatchMethod(AggressiveInlining)] public int                                                  BinarySearch (uint index, uint count, T element, System.Collections.Generic.IComparer<T>? comparer)                          =>     LIST.BinarySearch ((int) index, (int) count, element, comparer);
      [PatchMethod(AggressiveInlining)] public void                                                 Clear        ()                                                                                                              =>     LIST.Clear        ();
      [PatchMethod(AggressiveInlining)] public bool                                                 Contains     (T                      element)                                                                                =>     LIST.Contains     (element);
      [PatchMethod(AggressiveInlining)] public SharedList<U>                                        ConvertAll<U>(System.Converter<T, U> converter)                                                                              => new(LIST.ConvertAll<U>(converter));
      [PatchMethod(AggressiveInlining)] public void                                                 CopyTo       (T[]                    array)                                                                                  =>     LIST.CopyTo       (array);
      [PatchMethod(AggressiveInlining)] public void                                                 CopyTo       (T[]                    array, uint index)                                                                      =>     LIST.CopyTo       (array, (int) index);
      [PatchMethod(AggressiveInlining)] public void                                                 CopyTo       (uint                   index, T[]  array, uint arrayIndex, uint count)                                         =>     LIST.CopyTo       ((int) index, array, (int) arrayIndex, (int) count);
      [PatchMethod(AggressiveInlining)] public bool                                                 Exists       (System.Predicate<T>    predicate)                                                                              =>     LIST.Exists       (predicate);
      [PatchMethod(AggressiveInlining)] public T?                                                   Find         (System.Predicate<T>    predicate)                                                                              =>     LIST.Find         (predicate);
      [PatchMethod(AggressiveInlining)] public SharedList<T>                                        FindAll      (System.Predicate<T>    predicate)                                                                              => new(LIST.FindAll      (predicate));
      [PatchMethod(AggressiveInlining)] public int                                                  FindIndex    (System.Predicate<T>    predicate)                                                                              =>     LIST.FindIndex    (predicate);
      [PatchMethod(AggressiveInlining)] public int                                                  FindIndex    (uint                   index,             System.Predicate<T> predicate)                                       =>     LIST.FindIndex    ((int) index,              predicate);
      [PatchMethod(AggressiveInlining)] public int                                                  FindIndex    (uint                   index, uint count, System.Predicate<T> predicate)                                       =>     LIST.FindIndex    ((int) index, (int) count, predicate);
      [PatchMethod(AggressiveInlining)] public T?                                                   FindLast     (System.Predicate<T>    predicate)                                                                              =>     LIST.FindLast     (predicate);
      [PatchMethod(AggressiveInlining)] public int                                                  FindLastIndex(System.Predicate<T>    predicate)                                                                              =>     LIST.FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining)] public int                                                  FindLastIndex(uint                   index,             System.Predicate<T> predicate)                                       =>     LIST.FindLastIndex((int) index,              predicate);
      [PatchMethod(AggressiveInlining)] public int                                                  FindLastIndex(uint                   index, uint count, System.Predicate<T> predicate)                                       =>     LIST.FindLastIndex((int) index, (int) count, predicate);
      [PatchMethod(AggressiveInlining)] public void                                                 ForEach      (System.Action<T>       action)                                                                                 =>     LIST.ForEach      (action);
      [PatchMethod(AggressiveInlining)] public System.Collections.Generic.List<T>.Enumerator        GetEnumerator()                                                                                                              =>     LIST.GetEnumerator();
      [PatchMethod(AggressiveInlining)] public SharedList<T>                                        GetRange     (uint                index, uint count)                                                                         => new(LIST.GetRange     ((int) index, (int) count));
      [PatchMethod(AggressiveInlining)] public int                                                  IndexOf      (T                   element)                                                                                   =>     LIST.IndexOf      (element);
      [PatchMethod(AggressiveInlining)] public int                                                  IndexOf      (T                   element, uint                                      index)                                  =>     LIST.IndexOf      (element, (int) index);
      [PatchMethod(AggressiveInlining)] public int                                                  IndexOf      (T                   element, uint                                      index, uint count)                      =>     LIST.IndexOf      (element, (int) index, (int) count);
      [PatchMethod(AggressiveInlining)] public void                                                 Insert       (uint                index,   T                                         element)                                =>     LIST.Insert       ((int) index,   element);
      [PatchMethod(AggressiveInlining)] public void                                                 InsertRange  (uint                index,   System.Collections.Generic.IEnumerable<T> enumerable)                             =>     LIST.InsertRange  ((int) index,   enumerable);
      [PatchMethod(AggressiveInlining)] public int                                                  LastIndexOf  (T                   element)                                                                                   =>     LIST.LastIndexOf  (element);
      [PatchMethod(AggressiveInlining)] public int                                                  LastIndexOf  (T                   element, uint index)                                                                       =>     LIST.LastIndexOf  (element, (int) index);
      [PatchMethod(AggressiveInlining)] public int                                                  LastIndexOf  (T                   element, uint index, uint count)                                                           =>     LIST.LastIndexOf  (element, (int) index, (int) count);
      [PatchMethod(AggressiveInlining)] public T                                                    Prepend      (T                   element)                                                                                   =>     LIST.Prepend      (element);
      [PatchMethod(AggressiveInlining)] public bool                                                 Remove       (T                   element)                                                                                   =>     LIST.Remove       (element);
      [PatchMethod(AggressiveInlining)] public int                                                  RemoveAll    (System.Predicate<T> predicate)                                                                                 =>     LIST.RemoveAll    (predicate);
      [PatchMethod(AggressiveInlining)] public void                                                 RemoveAt     (int                 index)                                                                                     =>     LIST.RemoveAt     (index);
      [PatchMethod(AggressiveInlining)] public void                                                 RemoveRange  (uint                index, uint count)                                                                         =>     LIST.RemoveRange  ((int) index, (int) count);
      [PatchMethod(AggressiveInlining)] public void                                                 Reverse      ()                                                                                                              =>     LIST.Reverse      ();
      [PatchMethod(AggressiveInlining)] public void                                                 Reverse      (uint index, uint count)                                                                                        =>     LIST.Reverse      ((int) index, (int) count);
      [PatchMethod(AggressiveInlining)] public void                                                 Sort         ()                                                                                                              =>     LIST.Sort         ();
      [PatchMethod(AggressiveInlining)] public void                                                 Sort         (System.Collections.Generic.IComparer<T>? comparer)                                                             =>     LIST.Sort         (comparer);
      [PatchMethod(AggressiveInlining)] public void                                                 Sort         (System.Comparison                   <T>? comparison)                                                           =>     LIST.Sort         (comparison);
      [PatchMethod(AggressiveInlining)] public void                                                 Sort         (uint                                     index, uint count, System.Collections.Generic.IComparer<T>? comparer) =>     LIST.Sort         ((int) index, (int) count, comparer);
      [PatchMethod(AggressiveInlining)] public T[]                                                  ToArray      ()                                                                                                              =>     LIST.ToArray      ();
      [PatchMethod(AggressiveInlining)] public void                                                 TrimExcess   ()                                                                                                              =>     LIST.TrimExcess   ();
      [PatchMethod(AggressiveInlining)] public bool                                                 TrueForAll   (System.Predicate<T> predicate)                                                                                 =>     LIST.TrueForAll   (predicate);

      [PatchMethod(AggressiveInlining)]
      private protected static uint GetEnumerableLength(System.Collections.IEnumerable enumerable) {
        System.Collections.IEnumerator enumerator = enumerable.GetEnumerator();
        uint                           length     = 0u;

        // …
        while (enumerator.MoveNext())
          ++length;

        (enumerator as System.IDisposable)?.Dispose();
        return length;
      }

      [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator            System.Collections.IEnumerable.GetEnumerator           () => this.GetEnumerator();
      [PatchMethod(AggressiveInlining)] System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() => this.GetEnumerator();

      public T this[uint index] {
        [PatchMethod(AggressiveInlining)]
        get {
          return LIST[(int) index];
        }
      }
    }
      public class GameObjectSharedList<T> : PatchOdyssey.Collections.SharedList<T> where T : UnityEngine.Object /* ⟶ UnityEngine.Component or UnityEngine.GameObject */ {
        [PatchConstructor, PatchMethod(AggressiveInlining)] internal GameObjectSharedList(uint                    capacity = 0u) : base(capacity) {}
        [PatchConstructor, PatchMethod(AggressiveInlining)] internal GameObjectSharedList(GameObjectSharedList<T> list)          : base(list)     {}
        [PatchConstructor, PatchMethod(AggressiveInlining)] private  GameObjectSharedList(SharedList          <T> list)          : base(list)     {}

        /* … */
        [PatchMethod(AggressiveInlining)] internal           new void                    Add          (T                                         element)                                                             =>     base.Add          (element);
        [PatchMethod(AggressiveInlining)] internal           new void                    AddRange     (System.Collections.Generic.IEnumerable<T> enumerable)                                                          =>     base.AddRange     (enumerable);
        [PatchMethod(AggressiveInlining)] internal           new T                       Append       (T                                         element)                                                             =>     base.Append       (element);
        [PatchMethod(AggressiveInlining)] internal           new void                    Clear        ()                                                                                                              =>     base.Clear        ();
        [PatchMethod(AggressiveInlining)] public             new GameObjectSharedList<U> ConvertAll<U>(System.Converter<T, U> converter) where U : UnityEngine.Object /* ⟶ T */                                      => new(base.ConvertAll<U>(converter));
        [PatchMethod(AggressiveInlining)] internal           new GameObjectSharedList<T> FindAll      (System.Predicate<T>    predicate)                                                                              => new(base.FindAll      (predicate));
        [PatchMethod(AggressiveInlining)] internal           new GameObjectSharedList<T> GetRange     (uint                   index, uint                                      count)                                 => new(base.GetRange     (index, count));
        [PatchMethod(AggressiveInlining)] internal           new void                    Insert       (uint                   index, T                                         element)                               =>     base.Insert       (index,   element);
        [PatchMethod(AggressiveInlining)] internal           new void                    InsertRange  (uint                   index, System.Collections.Generic.IEnumerable<T> enumerable)                            =>     base.InsertRange  (index,   enumerable);
        [PatchMethod(AggressiveInlining)] internal           new T                       Prepend      (T                      element)                                                                                =>     base.Prepend      (element);
        [PatchMethod(AggressiveInlining)] internal           new bool                    Remove       (T                      element)                                                                                =>     base.Remove       (element);
        [PatchMethod(AggressiveInlining)] internal           new int                     RemoveAll    (System.Predicate<T>    predicate)                                                                              =>     base.RemoveAll    (predicate);
        [PatchMethod(AggressiveInlining)] internal           new void                    RemoveAt     (int                    index)                                                                                  =>     base.RemoveAt     (index);
        [PatchMethod(AggressiveInlining)] internal           new void                    RemoveRange  (uint                   index, uint count)                                                                      =>     base.RemoveRange  (index, count);
        [PatchMethod(AggressiveInlining)] internal           new void                    Reverse      ()                                                                                                              =>     base.Reverse      ();
        [PatchMethod(AggressiveInlining)] internal           new void                    Reverse      (uint index, uint count)                                                                                        =>     base.Reverse      (index, count);
        [PatchMethod(AggressiveInlining)] internal           new void                    Sort         ()                                                                                                              =>     base.Sort         ();
        [PatchMethod(AggressiveInlining)] internal           new void                    Sort         (System.Collections.Generic.IComparer<T>? comparer)                                                             =>     base.Sort         (comparer);
        [PatchMethod(AggressiveInlining)] internal           new void                    Sort         (System.Comparison                   <T>? comparison)                                                           =>     base.Sort         (comparison);
        [PatchMethod(AggressiveInlining)] internal           new void                    Sort         (uint                                     index, uint count, System.Collections.Generic.IComparer<T>? comparer) =>     base.Sort         (index, count, comparer);
        [PatchMethod(AggressiveInlining)] protected internal new void                    TrimExcess   ()                                                                                                              =>     base.TrimExcess   ();

        [PatchMethod(AggressiveInlining)] public GameObjectSharedList<U> ByComponent<U>() where U : UnityEngine.Component => this.ByComponent(typeof(U)).ConvertAll(static element => (U) element);
        public GameObjectSharedList<UnityEngine.Component> ByComponent(System.Type type) {
          GameObjectSharedList<UnityEngine.Component> list = new((uint) this.Count);

          // … ⟶ Avoid overriding underlying `GameObjectSharedList<UnityEngine.Component>.LIST` prematurely
          if (typeof(T) != typeof(UnityEngine.Component)) {
            list.Clear();

            foreach (T value in this) {
              UnityEngine.Component? subcomponent = value switch { UnityEngine.Component component => component.GetComponent(type), UnityEngine.GameObject gameObject => gameObject.GetComponent(type), _ => null };

              if (subcomponent is not null)
              list.Add(subcomponent);
            }
          }

          return list;
        }

        [PatchMethod(AggressiveInlining)]
        public GameObjectSharedList<T> ByName(string name) {
          this.RemoveAll(value => name != value.name);
          return new();
        }

        [PatchMethod(AggressiveInlining)]
        public GameObjectSharedList<T> ByTag(string tag) {
          this.RemoveAll(value => value switch {
            UnityEngine.Component  component  => component .tag == tag,
            UnityEngine.GameObject gameObject => gameObject.tag == tag,
            _                                 => false
          });
          return new();
        }
      }

    internal readonly struct WaitInfo /* ⟶ Considered `UnityEngine.MonoBehaviour::Invoke[Repeating](nameof(𝑓) or ((System.Delegate) 𝑓).Method.Name, delay[, interval])` */ {
      internal static readonly System.Collections.Generic.SortedDictionary<double, PatchOdyssey.Collections.WaitInfo> WAITS = new(new System.Collections.Generic.Dictionary<double, PatchOdyssey.Collections.WaitInfo>(16)) {{double.NaN, new()}};

      internal readonly UnityEngine.Coroutine?                                                            coroutine = null;
      internal readonly PatchOdyssey.Collections.EventHandler<PatchOdyssey.Collections.WaitForTimerEvent> handlers  = new();

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)]
      public WaitInfo() {}

      [PatchConstructor, PatchMethod(AggressiveInlining)]
      internal WaitInfo(PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Collections.WaitForTimerEvent> handler) {
        this.handlers += handler;
      }

      [PatchConstructor, PatchMethod(AggressiveInlining)]
      internal WaitInfo(UnityEngine.Coroutine coroutine, PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Collections.WaitForTimerEvent> handler) {
        this.coroutine = coroutine;
        this.handlers += handler;
      }
    }
  }

  /* … */
  public delegate void    Handler        (object? target,   PatchOdyssey.Collections.Event data);         // ⟶ Handles completed `Load`, `Wait`, … operations i.e. `System.EventHandler`
  public delegate void    Handler<T>     (object? target,   T                              data);         //    ^^
  public delegate object? Interpolator   (double  progress, object?                        a, object? b); // ⟶ Interpolates `::begin` and `::end` properties in `Animation.UIKeyframe["…"]`
  public delegate T       Interpolator<T>(double  progress, T                              a, T       b); //    ^^
  public delegate double  Tweener        (double  time);                                                  // ⟶ Adjusts interpolation be-tween `Interpolator(…)`'s `progress` from `a` to `b`

  public sealed class ReadOnlyInInspectorAttribute : UnityEngine.PropertyAttribute {
    /* ⟶ Display property in Unity Inspector as “read-only” */
  }

  [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false, Inherited = false)]
  public sealed class ReadWriteInInspectorAttribute : UnityEngine.PropertyAttribute /* ⟶ System.Attribute */ {
    /* ⟶ Display property in Unity Inspector as “read-write” */
  }

  public readonly ref struct Void {
    /* ⟶ “error CS0590: UsEr-DeFiNeD oPeRaToRs CaNnOt ReTuRn VoId” */
  }

  /* … */
  #if UNITY_EDITOR
    public class EditorDictionaryDrawer<TKey, TValue> : UnityEditor.PropertyDrawer {
      private bool foldout = false;

      /* … */
      private System.Collections.Generic.IDictionary<TKey, TValue> Ensure(UnityEditor.SerializedProperty property) {
        System.Collections.Generic.IDictionary<TKey, TValue> dictionary = this.fieldInfo.GetValue(property.serializedObject.targetObject) as System.Collections.Generic.IDictionary<TKey, TValue> ?? new PatchOdyssey.Collections.EditorDictionary<TKey, TValue>();

        this.fieldInfo.SetValue(property.serializedObject.targetObject, dictionary);
        return dictionary;
      }

      public override float GetPropertyHeight(UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) {
        return base.GetPropertyHeight(property, label) * (this.foldout || UnityEditor.EditorPrefs.GetBool(label.text) ? System.Math.Max(this.Ensure(property).Count, 1) + 1 : 1);
      }

      public override void OnGUI(UnityEngine.Rect position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) {
        System.Collections.Generic.IDictionary<TKey, TValue>                     dictionary           = this.Ensure(property);
        bool                                                                     dictionaryIsReadOnly = dictionary.IsReadOnly; // ⟶ `PatchOdyssey.Collections.EditorReadOnlyDictionary<TKey, TValue>` or `System.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue>`
        float                                                                    size                 = position.height = base.GetPropertyHeight(property, label);
        (UnityEngine.Rect add, UnityEngine.Rect clear, UnityEngine.Rect foldout) positions            = (
          add    : new(position.x + (position.width - Util.PercentOf(size, 200.0f)), position.y, 0.0f           + size, position.height),
          clear  : new(position.x + (position.width - Util.PercentOf(size, 100.0f)), position.y, 0.0f           + size, position.height),
          foldout: new(position.x,                                                   position.y, position.width - size, position.height)
        );

        // …
        if (!dictionaryIsReadOnly) {
          if (UnityEngine.GUI.Button(positions.add, new UnityEngine.GUIContent("+", "Add field"), UnityEditor.EditorStyles.miniButton))
          dictionary.TryAdd(
            typeof(TKey) != typeof(string) ? System.Activator.CreateInstance<TKey>  () : (TKey)   (""   as object),
            typeof(TValue).IsValueType     ? System.Activator.CreateInstance<TValue>() : (TValue) (null as object)!
          );

          if (UnityEngine.GUI.Button(positions.clear, new UnityEngine.GUIContent("×", "Clear dictionary"), UnityEditor.EditorStyles.miniButtonRight))
          dictionary.Clear();
        }

        UnityEditor.EditorGUI.BeginChangeCheck();
          this.foldout = UnityEditor.EditorPrefs.GetBool(label.text);
          this.foldout = UnityEditor.EditorGUI.  Foldout(positions.foldout, this.foldout, label, true);
        if (UnityEditor.EditorGUI.EndChangeCheck()) UnityEditor.EditorPrefs.SetBool(label.text, this.foldout);

        // …
        if (!this.foldout)
        return;

        if (0 == dictionary.Count) {
          UnityEngine.GUI.Label(new(position.x, position.y + position.height, position.width, position.height), "Dictionary is empty");
          return;
        }

        foreach (System.Collections.Generic.KeyValuePair<TKey, TValue> element in dictionary)
        if (element.Key is not null) {
          (TKey key, TValue value)                                                            = (element.Key, element.Value);
          (UnityEngine.Rect key, UnityEngine.Rect value, UnityEngine.Rect clear) subpositions = (
            key  : new(position.x                                                 + (dictionaryIsReadOnly ? size : 0.0f), position.y += position.height, Util.PercentOf(position.width - size, 40.0f), position.height),
            value: new(position.x + Util.PercentOf(position.width - size,  40.0f) + (dictionaryIsReadOnly ? size : 0.0f), position.y,                    Util.PercentOf(position.width - size, 60.0f), position.height),
            clear: new(position.x + Util.PercentOf(position.width - size, 100.0f),                                        position.y,                    size,                                         position.height)
          );

          // …
          UnityEditor.EditorGUI.BeginChangeCheck();
            if (dictionaryIsReadOnly) UnityEngine.GUI.Label(subpositions.key, key.ToString());
            else key = (TKey) PatchOdyssey.Collections.EditorDictionary<TKey, TValue>.DelegateGUIField<TKey>()!(subpositions.key, key);
          if (UnityEditor.EditorGUI.EndChangeCheck()) {
            dictionary.Remove(element.Key);
            dictionary.Add   (key, value);

            break;
          }

          UnityEditor.EditorGUI.BeginChangeCheck();
            value = (TValue) PatchOdyssey.Collections.EditorDictionary<TKey, TValue>.DelegateGUIField<TValue>()!(subpositions.value, value!);
          if (UnityEditor.EditorGUI.EndChangeCheck()) {
            dictionary[key] = value;
            break;
          }

          if (!dictionaryIsReadOnly)
          if (UnityEngine.GUI.Button(subpositions.clear, new UnityEngine.GUIContent("×", "Clear item"), UnityEditor.EditorStyles.miniButtonRight)) {
            dictionary.Remove(key);
            break;
          }
        }
      }
    }
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.AnimationCurveDictionary))]         public class AnimationCurveDictionaryDrawer         : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.AnimationCurve> {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.AnimationCurveReadOnlyDictionary))] public class AnimationCurveReadOnlyDictionaryDrawer : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.AnimationCurve> {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.BooleanDictionary))]                public class BooleanDictionaryDrawer                : PatchOdyssey.EditorDictionaryDrawer<string, System     .Boolean>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.BooleanReadOnlyDictionary))]        public class BooleanReadOnlyDictionaryDrawer        : PatchOdyssey.EditorDictionaryDrawer<string, System     .Boolean>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.BoundsDictionary))]                 public class BoundsDictionaryDrawer                 : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.Bounds>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.BoundsReadOnlyDictionary))]         public class BoundsReadOnlyDictionaryDrawer         : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.Bounds>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.BoundsIntDictionary))]              public class BoundsIntDictionaryDrawer              : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.BoundsInt>      {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.BoundsIntReadOnlyDictionary))]      public class BoundsIntReadOnlyDictionaryDrawer      : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.BoundsInt>      {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.ColorDictionary))]                  public class ColorDictionaryDrawer                  : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.Color>          {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.ColorReadOnlyDictionary))]          public class ColorReadOnlyDictionaryDrawer          : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.Color>          {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.DoubleDictionary))]                 public class DoubleDictionaryDrawer                 : PatchOdyssey.EditorDictionaryDrawer<string, System     .Double>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.DoubleReadOnlyDictionary))]         public class DoubleReadOnlyDictionaryDrawer         : PatchOdyssey.EditorDictionaryDrawer<string, System     .Double>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.FloatDictionary))]                  public class FloatDictionaryDrawer                  : PatchOdyssey.EditorDictionaryDrawer<string, System     .Single>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.FloatReadOnlyDictionary))]          public class FloatReadOnlyDictionaryDrawer          : PatchOdyssey.EditorDictionaryDrawer<string, System     .Single>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.GameObjectDictionary))]             public class GameObjectDictionaryDrawer             : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.GameObject>     {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.GameObjectReadOnlyDictionary))]     public class GameObjectReadOnlyDictionaryDrawer     : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.GameObject>     {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.GradientDictionary))]               public class GradientDictionaryDrawer               : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.Gradient>       {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.GradientReadOnlyDictionary))]       public class GradientReadOnlyDictionaryDrawer       : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.Gradient>       {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.IntDictionary))]                    public class IntDictionaryDrawer                    : PatchOdyssey.EditorDictionaryDrawer<string, System     .Int32>          {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.IntReadOnlyDictionary))]            public class IntReadOnlyDictionaryDrawer            : PatchOdyssey.EditorDictionaryDrawer<string, System     .Int32>          {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.LongDictionary))]                   public class LongDictionaryDrawer                   : PatchOdyssey.EditorDictionaryDrawer<string, System     .Int64>          {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.LongReadOnlyDictionary))]           public class LongReadOnlyDictionaryDrawer           : PatchOdyssey.EditorDictionaryDrawer<string, System     .Int64>          {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.RectDictionary))]                   public class RectDictionaryDrawer                   : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.Rect>           {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.RectReadOnlyDictionary))]           public class RectReadOnlyDictionaryDrawer           : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.Rect>           {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.RectIntDictionary))]                public class RectIntDictionaryDrawer                : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.RectInt>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.RectIntReadOnlyDictionary))]        public class RectIntReadOnlyDictionaryDrawer        : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.RectInt>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.StringDictionary))]                 public class StringDictionaryDrawer                 : PatchOdyssey.EditorDictionaryDrawer<string, System     .String>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.StringReadOnlyDictionary))]         public class StringReadOnlyDictionaryDrawer         : PatchOdyssey.EditorDictionaryDrawer<string, System     .String>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.UIntDictionary))]                   public class UIntDictionaryDrawer                   : PatchOdyssey.EditorDictionaryDrawer<string, System     .UInt32>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.UIntReadOnlyDictionary))]           public class UIntReadOnlyDictionaryDrawer           : PatchOdyssey.EditorDictionaryDrawer<string, System     .UInt32>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.ULongDictionary))]                  public class ULongDictionaryDrawer                  : PatchOdyssey.EditorDictionaryDrawer<string, System     .UInt64>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.ULongReadOnlyDictionary))]          public class ULongReadOnlyDictionaryDrawer          : PatchOdyssey.EditorDictionaryDrawer<string, System     .UInt64>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector2Dictionary))]                public class Vector2DictionaryDrawer                : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.Vector2>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector2ReadOnlyDictionary))]        public class Vector2ReadOnlyDictionaryDrawer        : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.Vector2>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector2IntDictionary))]             public class Vector2IntDictionaryDrawer             : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.Vector2Int>     {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector2IntReadOnlyDictionary))]     public class Vector2IntReadOnlyDictionaryDrawer     : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.Vector2Int>     {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector3Dictionary))]                public class Vector3DictionaryDrawer                : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.Vector3>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector3ReadOnlyDictionary))]        public class Vector3ReadOnlyDictionaryDrawer        : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.Vector3>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector3IntDictionary))]             public class Vector3IntDictionaryDrawer             : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.Vector3Int>     {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector3IntReadOnlyDictionary))]     public class Vector3IntReadOnlyDictionaryDrawer     : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.Vector3Int>     {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector4Dictionary))]                public class Vector4DictionaryDrawer                : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.Vector4>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector4ReadOnlyDictionary))]        public class Vector4ReadOnlyDictionaryDrawer        : PatchOdyssey.EditorDictionaryDrawer<string, UnityEngine.Vector4>        {}

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

    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.ReadWriteInInspectorAttribute))]
    public class ReadWriteInInspectorDrawer : UnityEditor.PropertyDrawer {
      public override float GetPropertyHeight(UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) {
        return UnityEditor.EditorGUI.GetPropertyHeight(property, label, true);
      }

      public override void OnGUI(UnityEngine.Rect position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) {
        UnityEditor.EditorGUI.PropertyField(position, property, label, true);
      }
    }
  #endif
}

namespace PatchOdyssey /* ⟶ …everything else */ {
  namespace Animation {
    public static partial class Function {
      [PatchMethod(AggressiveInlining)] public static double CubicBézier    (double time, double p0, double p1, double p2, double p3) { return (p0 * System.Math.Pow(1.0 - time, 3.0)) + (p1 * time * 3.0 * System.Math.Pow(1.0 - time, 2.0)) + (p2 * (1.0 - time) * 3.0 * System.Math.Pow(time, 2.0)) + (p3 * System.Math.Pow(time, 3.0)); }
      [PatchMethod(AggressiveInlining)] public static double QuadraticBézier(double time, double p0, double p1, double p2)            { return (p0 * System.Math.Pow(1.0 - time, 2.0)) + (p1 * time * 2.0 * System.Math.Pow(1.0 - time, 1.0))                                                          + (p2 * System.Math.Pow(time, 2.0)); }

      [PatchMethod(AggressiveInlining)] public static double Ease                (double time) { return PatchOdyssey.Animation.Function.CubicBézier(time, 0.25, 0.10, 0.25, 1.00); }
      [PatchMethod(AggressiveInlining)] public static double EaseIn              (double time) { return PatchOdyssey.Animation.Function.CubicBézier(time, 0.42, 0.00, 1.00, 1.00); }
      [PatchMethod(AggressiveInlining)] public static double EaseInBack          (double time) { return (System.Math.Pow(time, 3.0) * 2.70158) - (System.Math.Pow(time, 2.0) * 1.70158); }
      [PatchMethod(AggressiveInlining)] public static double EaseInBounce        (double time) { return 1.0 - PatchOdyssey.Animation.Function.EaseOutBounce(1.0 - time); }
      [PatchMethod(AggressiveInlining)] public static double EaseInCircular      (double time) { return 1.0 - System.Math.Sqrt(1.0 - System.Math.Pow(time, 2.0)); }
      [PatchMethod(AggressiveInlining)] public static double EaseInCubic         (double time) { return System.Math.Pow(time, 3.0); }
      [PatchMethod(AggressiveInlining)] public static double EaseInElastic       (double time) { return time != 0.0 && time != 1.0 ? -System.Math.Pow(2.0, (time * 10.0) - 10.0) * System.Math.Sin(((time * 10.0) - 10.75) * ((System.Math.PI * 2.0) / 3.0)) : time; }
      [PatchMethod(AggressiveInlining)] public static double EaseInExponential   (double time) { return time != 0.0 ? System.Math.Pow(2.0, (time * 10.0) - 10.0) : 0.0; }
      [PatchMethod(AggressiveInlining)] public static double EaseInOut           (double time) { return PatchOdyssey.Animation.Function.CubicBézier(time, 0.42, 0.00, 0.58, 1.00); }
      [PatchMethod(AggressiveInlining)] public static double EaseInOutBack       (double time) { return (time < 0.5 ? System.Math.Pow(time * 2.0, 2.0) * ((7.189819f * time) - 2.5949095) : ((System.Math.Pow((time * 2.0) - 2.0, 2.0) * ((((time * 2.0) - 2.0) * 3.5949095) + 2.5949095)) + 2.0)) / 2.0; }
      [PatchMethod(AggressiveInlining)] public static double EaseInOutBounce     (double time) { return (time < 0.5 ? (1.0 - PatchOdyssey.Animation.Function.EaseOutBounce(1.0 - (time * 2.0))) : (1.0 + PatchOdyssey.Animation.Function.EaseOutBounce((time * 2.0) - 1.0))) / 2.0; }
      [PatchMethod(AggressiveInlining)] public static double EaseInOutCircular   (double time) { return (time < 0.5 ? (1.0 - System.Math.Sqrt(1.0 - System.Math.Pow(time * 2.0, 2.0))) : (1.0 + System.Math.Sqrt(1.0 - System.Math.Pow((time * -2.0) + 2.0, 2.0)))) / 2.0; }
      [PatchMethod(AggressiveInlining)] public static double EaseInOutCubic      (double time) { return time < 0.5 ? 4.0 * System.Math.Pow(time, 3.0) : (1.0 - (System.Math.Pow((time * -2.0) + 2.0, 3.0) / 2.0)); }
      [PatchMethod(AggressiveInlining)] public static double EaseInOutElastic    (double time) { return time != 0.0 && time != 1.0 ? time < 0.5 ? -(System.Math.Pow(2.0, (time * 20.0) - 10.0) * System.Math.Sin(((time * 20.0) - 11.125) * ((System.Math.PI * 2.0) / 4.5))) / 2.0 : ((System.Math.Pow(2.0, (time * -20.0) + 10.0) * System.Math.Sin(((time * 20.0) - 11.125) * ((System.Math.PI * 2.0) / 4.5))) / 2.0 + 1.0) : time; }
      [PatchMethod(AggressiveInlining)] public static double EaseInOutExponential(double time) { return time != 0.0 && time != 1.0 ? (time < 0.5 ? System.Math.Pow(2.0, (time * 20.0) - 10.0) : (2.0 - System.Math.Pow(2.0, (time * -20.0) + 10.0))) / 2.0 : time; }
      [PatchMethod(AggressiveInlining)] public static double EaseInOutQuadratic  (double time) { return time < 0.5 ?  2.0 * System.Math.Pow(time, 2.0) : (1.0 - (System.Math.Pow((time * -2.0) + 2.0, 2.0) / 2.0)); }
      [PatchMethod(AggressiveInlining)] public static double EaseInOutQuartic    (double time) { return time < 0.5 ?  8.0 * System.Math.Pow(time, 4.0) : (1.0 - (System.Math.Pow((time * -2.0) + 2.0, 4.0) / 2.0)); }
      [PatchMethod(AggressiveInlining)] public static double EaseInOutQuintic    (double time) { return time < 0.5 ? 16.0 * System.Math.Pow(time, 5.0) : (1.0 - (System.Math.Pow((time * -2.0) + 2.0, 5.0) / 2.0)); }
      [PatchMethod(AggressiveInlining)] public static double EaseInOutSine       (double time) { return -(System.Math.Cos(System.Math.PI * time) - 1.0) / 2.0; }
      [PatchMethod(AggressiveInlining)] public static double EaseInQuadratic     (double time) { return System.Math.Pow(time, 2.0); }
      [PatchMethod(AggressiveInlining)] public static double EaseInQuartic       (double time) { return System.Math.Pow(time, 4.0); }
      [PatchMethod(AggressiveInlining)] public static double EaseInQuintic       (double time) { return System.Math.Pow(time, 5.0); }
      [PatchMethod(AggressiveInlining)] public static double EaseInSine          (double time) { return 1.0 - System.Math.Cos((System.Math.PI * time) / 2.0); }
      [PatchMethod(AggressiveInlining)] public static double EaseOut             (double time) { return PatchOdyssey.Animation.Function.CubicBézier(time, 0.00, 0.00, 0.58, 1.00); }
      [PatchMethod(AggressiveInlining)] public static double EaseOutBack         (double time) { return 1.0 + (2.70158 * System.Math.Pow(time - 1.0, 3.0)) + (System.Math.Pow(time - 1.0, 2.0) * 1.70158); }
      [PatchMethod(AggressiveInlining)] public static double EaseOutBounce       (double time) { return time < 1.0 / 2.75 ? System.Math.Pow(time, 2.0) * 7.5625 : time < 2.0 / 2.75 ? (System.Math.Pow(time - (1.5 / 2.75), 2.0) * 7.5625) + 0.75 : time < 2.5 / 2.75 ? (System.Math.Pow(time - (2.25 / 2.75), 2.0) * 7.5625) + 0.9375 : (System.Math.Pow(time - (2.625 / 2.75), 2.0) * 7.5625) + 0.984375; }
      [PatchMethod(AggressiveInlining)] public static double EaseOutCircular     (double time) { return System.Math.Sqrt(1.0 - System.Math.Pow(time - 1.0, 2.0)); }
      [PatchMethod(AggressiveInlining)] public static double EaseOutCubic        (double time) { return 1.0 - System.Math.Pow(1.0 - time, 3.0); }
      [PatchMethod(AggressiveInlining)] public static double EaseOutElastic      (double time) { return time != 0.0 && time != 1.0 ? (System.Math.Pow(2.0, time * -10.0) * System.Math.Sin(((time * 10.0) - 0.75) * ((System.Math.PI * 2.0) / 3.0))) + 1.0 : time; }
      [PatchMethod(AggressiveInlining)] public static double EaseOutExponential  (double time) { return time != 1.0 ? 1.0 - System.Math.Pow(2.0, time * -10.0) : 1.0; }
      [PatchMethod(AggressiveInlining)] public static double EaseOutQuadratic    (double time) { return 1.0 - System.Math.Pow(1.0 - time, 2.0); }
      [PatchMethod(AggressiveInlining)] public static double EaseOutQuartic      (double time) { return 1.0 - System.Math.Pow(1.0 - time, 4.0); }
      [PatchMethod(AggressiveInlining)] public static double EaseOutQuintic      (double time) { return 1.0 - System.Math.Pow(1.0 - time, 5.0); }
      [PatchMethod(AggressiveInlining)] public static double EaseOutSine         (double time) { return System.Math.Sin((System.Math.PI * time) / 2.0); }
      [PatchMethod(AggressiveInlining)] public static double Linear              (double time) { return time; }
    }
  }

  /* … */
  public static class Extensions /* ⟶ Method extensions only e.g. `System.Array.Add(this …)` */ {
    [PatchMethod(AggressiveInlining)] public static void Add              (this System.Collections.Queue                             queue,      object?                                               element) => queue     .Enqueue(element);                    // ⟶ Intended for initializer lists only i.e. `new() {…}`
    [PatchMethod(AggressiveInlining)] public static void Add<T>           (this System.Collections.Queue                             queue,      T                                                     element) => queue     .Enqueue(element);                    //    ^^
    [PatchMethod(AggressiveInlining)] public static void Add<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary, System.Collections.Generic.KeyValuePair<TKey, TValue> element) => dictionary.Add    (element.Key, element.Value); //
    [PatchMethod(AggressiveInlining)] public static void Add<T>           (this System.Collections.Generic.Queue      <T>            queue,      T                                                     element) => queue     .Enqueue(element);                    // ⟶ Intended for initializer lists only i.e. `new() {…}`
    [PatchMethod(AggressiveInlining)] public static void Add<T>           (this System.Collections.Generic.Stack      <T>            stack,      T                                                     element) => stack     .Push   (element);                    //    ^^
    [PatchMethod(AggressiveInlining)] public static void Add              (this System.Collections.Stack                             stack,      object?                                               element) => stack     .Push   (element);                    //
    [PatchMethod(AggressiveInlining)] public static void Add<T>           (this System.Collections.Stack                             stack,      T                                                     element) => stack     .Push   (element);                    //

    [PatchMethod(AggressiveInlining)] public static void AddRange<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>> enumerable) { foreach (System.Collections.Generic.KeyValuePair<TKey, TValue> element in dictionary) dictionary.Add(element.Key, element.Value); }
    [PatchMethod(AggressiveInlining)] public static void AddRange<T>           (this System.Collections.Generic.IList      <T>            list,       System.Collections.Generic.IEnumerable<T>                                                     enumerable) { foreach (T                                                     value   in enumerable) list      .Add(value); }

    [PatchMethod(AggressiveInlining)] public static object? Append   (this System.Collections.ArrayList             arrayList, object? value) { arrayList.Add    (value); return value; }
    [PatchMethod(AggressiveInlining)] public static T       Append<T>(this System.Collections.ArrayList             arrayList, T       value) { arrayList.Add    (value); return value; }
    [PatchMethod(AggressiveInlining)] public static T       Append<T>(this System.Collections.Generic.LinkedList<T> list,      T       value) { list     .AddLast(value); return value; }
    [PatchMethod(AggressiveInlining)] public static T       Append<T>(this System.Collections.Generic.List      <T> list,      T       value) { list     .Add    (value); return value; }

    [PatchMethod(AggressiveInlining)] public static System.Collections.ObjectModel.ReadOnlyCollection<T>            AsReadOnly<T>           (this T[]                                                  array)      => System.Array.AsReadOnly<T>(array);
    [PatchMethod(AggressiveInlining)] public static System.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary) { (dictionary as System.Collections.Generic.Dictionary<TKey, TValue>)?.TrimExcess(/* dictionary.Keys.Count */); return new(dictionary); } // ⟶ `System.Collections.Generic.CollectionExtensions.AsReadOnly<TKey, TValue>(dictionary);`
    [PatchMethod(AggressiveInlining)] public static System.Collections.ObjectModel.ReadOnlyCollection<T>            AsReadOnly<T>           (this System.Collections.Generic.IList      <T>            list)       { (list       as System.Collections.Generic.List      <T>)           ?.TrimExcess(/* … */);                     return new(list); }       // ⟶ `System.Collections.Generic.CollectionExtensions.AsReadOnly<T>           (list);`

    [PatchMethod(AggressiveInlining)] public static int BinarySearch   (this System.Array array,                        object? value)                                         { return System.Array.BinarySearch(array, value); }
    [PatchMethod(AggressiveInlining)] public static int BinarySearch   (this System.Array array,                        object? value, System.Collections.IComparer? comparer) { return System.Array.BinarySearch(array, value, comparer); }
    [PatchMethod(AggressiveInlining)] public static int BinarySearch   (this System.Array array, int index, int length, object? value)                                         { return System.Array.BinarySearch(array, index, length, value); }
    [PatchMethod(AggressiveInlining)] public static int BinarySearch   (this System.Array array, int index, int length, object? value, System.Collections.IComparer? comparer) { return System.Array.BinarySearch(array, index, length, value, comparer); }
    [PatchMethod(AggressiveInlining)] public static int BinarySearch<T>(this T[]          array,                        T       value)                                         { return System.Array.BinarySearch(array, value); }
    [PatchMethod(AggressiveInlining)] public static int BinarySearch<T>(this T[]          array,                        T       value, System.Collections.IComparer? comparer) { return System.Array.BinarySearch(array, value, comparer); }
    [PatchMethod(AggressiveInlining)] public static int BinarySearch<T>(this T[]          array, int index, int length, T       value)                                         { return System.Array.BinarySearch(array, index, length, value); }
    [PatchMethod(AggressiveInlining)] public static int BinarySearch<T>(this T[]          array, int index, int length, T       value, System.Collections.IComparer? comparer) { return System.Array.BinarySearch(array, index, length, value, comparer); }

    [PatchMethod(AggressiveInlining)] public static void Clear              (this System.Array                                         array)                        => array.Clear(0, array.Length);
    [PatchMethod(AggressiveInlining)] public static void Clear              (this System.Array                                         array, int index, int length) { System.Array.Clear(array, index, length); }
    [PatchMethod(AggressiveInlining)] public static void Clear<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary)                   { foreach (TKey key in dictionary.Keys) dictionary.Remove(key); } // ⟶ `::Capacity` remains unchanged

    [PatchMethod(AggressiveInlining)] public static bool Contains(this System.Array array, object value) => array.Contains(value, null as System.Collections.IEqualityComparer);
    [PatchMethod(AggressiveInlining)]
    public static bool Contains(this System.Array array, object value, System.Collections.IEqualityComparer? comparer) {
      for (System.Collections.IEnumerator enumerator = array.GetEnumerator(); enumerator.MoveNext(); ) {
        if (comparer?.Equals(enumerator.Current, value) ?? System.Object.ReferenceEquals(enumerator.Current, value))
        return true; // ⟶ `enumerator.Dispose()` unneeded
      }

      return false;
    }

    [PatchMethod(AggressiveInlining)] public static bool Contains<T>(this T[] array, T value) => array.Contains<T>(value, null as System.Collections.Generic.IEqualityComparer<T>);
    [PatchMethod(AggressiveInlining)]
    public static bool Contains<T>(this T[] array, T value, System.Collections.Generic.IEqualityComparer<T>? comparer) {
      foreach (T element in array) {
        if ((comparer ?? System.Collections.Generic.EqualityComparer<T>.Default).Equals(element, value)) // ⟶ Benefits from devirtualization and likely inlining
        return true;
      }

      return false;
    }

    [PatchMethod(AggressiveInlining)]
    public static bool ContainsValue<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary, TValue value) {
      foreach (TValue dictionaryValue in dictionary.Values) {
        if ((value as System.IEquatable<TValue>)?.Equals(dictionaryValue) ?? System.Object.ReferenceEquals(value, dictionaryValue))
        return true;
      }

      return false;
    }

    [PatchMethod(AggressiveInlining)]
    public static U[] ConvertAll<T, U>(this T[] array, System.Converter<T, U> converter) {
      return System.Array.ConvertAll<T, U>(array, converter);
    }

    [PatchMethod(AggressiveInlining)] public static uint CountChildren(this UnityEngine.Component  component) => component.CountChildrenByComponent(component.GetType());
    [PatchMethod(AggressiveInlining)] public static uint CountChildren(this UnityEngine.Transform  transform) => transform.gameObject.CountChildren();
    [PatchMethod(AggressiveInlining)] public static uint CountChildren(this UnityEngine.GameObject gameObject) { return (uint) gameObject.transform.childCount; }

    [PatchMethod(AggressiveInlining)] public static uint CountChildrenByComponent<T>(this UnityEngine.Component  component)                    => component.gameObject.CountChildrenByComponent<T>();
    [PatchMethod(AggressiveInlining)] public static uint CountChildrenByComponent<T>(this UnityEngine.GameObject gameObject)                   => gameObject          .CountChildrenByComponent(typeof(T));
    [PatchMethod(AggressiveInlining)] public static uint CountChildrenByComponent   (this UnityEngine.Component  component,  System.Type type) => component.gameObject.CountChildrenByComponent(type);
    [PatchMethod(AggressiveInlining)] public static uint CountChildrenByComponent   (this UnityEngine.GameObject gameObject, System.Type type) { uint count = 0u; foreach (UnityEngine.GameObject child in gameObject.EnumerateChildren()) { count += child.GetComponent(type) is not null ? 1u : 0u; } return count; }

    [PatchMethod(AggressiveInlining)] public static uint CountChildrenByName(this UnityEngine.Component  component,  string name) => component.gameObject.CountChildrenByName(name);
    [PatchMethod(AggressiveInlining)] public static uint CountChildrenByName(this UnityEngine.GameObject gameObject, string name) { uint count = 0u; foreach (UnityEngine.GameObject child in gameObject.EnumerateChildren()) { count += child.name == name ? 1u : 0u; } return count; }

    [PatchMethod(AggressiveInlining)] public static uint CountChildrenByTag(this UnityEngine.Component  component,  string tag) => component.gameObject.CountChildrenByTag(tag);
    [PatchMethod(AggressiveInlining)] public static uint CountChildrenByTag(this UnityEngine.GameObject gameObject, string tag) { uint count = 0u; foreach (UnityEngine.GameObject child in gameObject.EnumerateChildren()) { count += child.tag == tag ? 1u : 0u; } return count; }

    [PatchMethod(AggressiveInlining)] public static uint CountDescendants(this UnityEngine.Component  component)  => component.CountDescendantsByComponent(component.GetType());
    [PatchMethod(AggressiveInlining)] public static uint CountDescendants(this UnityEngine.Transform  transform)  => transform.gameObject.CountDescendants();
    [PatchMethod(AggressiveInlining)] public static uint CountDescendants(this UnityEngine.GameObject gameObject) { return (uint) gameObject.transform.hierarchyCount - 1; }

    [PatchMethod(AggressiveInlining)] public static uint CountDescendantsByComponent<T>(this UnityEngine.Component  component)                    => component.gameObject.CountDescendantsByComponent<T>();
    [PatchMethod(AggressiveInlining)] public static uint CountDescendantsByComponent<T>(this UnityEngine.GameObject gameObject)                   => gameObject          .CountDescendantsByComponent(typeof(T));
    [PatchMethod(AggressiveInlining)] public static uint CountDescendantsByComponent   (this UnityEngine.Component  component,  System.Type type) => component.gameObject.CountDescendantsByComponent(type);
    [PatchMethod(AggressiveInlining)] public static uint CountDescendantsByComponent   (this UnityEngine.GameObject gameObject, System.Type type) { uint count = 0u; foreach (UnityEngine.GameObject descendant in gameObject.EnumerateDescendants()) { count += descendant.GetComponent(type) is not null ? 1u : 0u; } return count; }

    [PatchMethod(AggressiveInlining)] public static uint CountDescendantsByName(this UnityEngine.Component  component,  string name) => component.gameObject.CountDescendantsByName(name);
    [PatchMethod(AggressiveInlining)] public static uint CountDescendantsByName(this UnityEngine.GameObject gameObject, string name) { uint count = 0u; foreach (UnityEngine.GameObject descendant in gameObject.EnumerateDescendants()) { count += descendant.name == name ? 1u : 0u; } return count; }

    [PatchMethod(AggressiveInlining)] public static uint CountDescendantsByTag(this UnityEngine.Component  component,  string tag) => component.gameObject.CountDescendantsByTag(tag);
    [PatchMethod(AggressiveInlining)] public static uint CountDescendantsByTag(this UnityEngine.GameObject gameObject, string tag) { uint count = 0u; foreach (UnityEngine.GameObject descendant in gameObject.EnumerateDescendants()) { count += descendant.tag == tag ? 1u : 0u; } return count; }

    [PatchMethod(AggressiveInlining)] public static uint CountHierarchy(this UnityEngine.Component  component)  => component.gameObject.CountHierarchyByComponent(component.GetType());
    [PatchMethod(AggressiveInlining)] public static uint CountHierarchy(this UnityEngine.Transform  transform)  => transform.gameObject.CountHierarchy();
    [PatchMethod(AggressiveInlining)] public static uint CountHierarchy(this UnityEngine.GameObject gameObject) { return gameObject.CountDescendants() + 1u; }

    [PatchMethod(AggressiveInlining)] public static uint CountHierarchyByComponent<T>(this UnityEngine.Component  component)                    => component.gameObject.CountHierarchyByComponent<T>();
    [PatchMethod(AggressiveInlining)] public static uint CountHierarchyByComponent<T>(this UnityEngine.GameObject gameObject)                   => gameObject          .CountHierarchyByComponent(typeof(T));
    [PatchMethod(AggressiveInlining)] public static uint CountHierarchyByComponent   (this UnityEngine.Component  component,  System.Type type) => component.gameObject.CountHierarchyByComponent(type);
    [PatchMethod(AggressiveInlining)] public static uint CountHierarchyByComponent   (this UnityEngine.GameObject gameObject, System.Type type) { return gameObject.CountDescendantsByComponent(type) + (gameObject.GetComponent(type) is not null ? 1u : 0u); }

    [PatchMethod(AggressiveInlining)] public static uint CountHierarchyByName(this UnityEngine.Component  component,  string name) => component.gameObject.CountHierarchyByName(name);
    [PatchMethod(AggressiveInlining)] public static uint CountHierarchyByName(this UnityEngine.GameObject gameObject, string name) { return gameObject.CountDescendantsByName(name) + (gameObject.name == name ? 1u : 0u); }

    [PatchMethod(AggressiveInlining)] public static uint CountHierarchyByTag(this UnityEngine.Component  component,  string tag) => component.gameObject.CountHierarchyByTag(tag);
    [PatchMethod(AggressiveInlining)] public static uint CountHierarchyByTag(this UnityEngine.GameObject gameObject, string tag) { return gameObject.CountDescendantsByTag(tag) + (gameObject.tag == tag ? 1u : 0u); }

    [PatchMethod(AggressiveInlining)]
    public static PatchOdyssey.Collections.GameObjectEnumerator EnumerateChildren(this UnityEngine.GameObject gameObject) {
      return new(GameObjectEnumerator.Kind.Children, gameObject.transform);
    }

    [PatchMethod(AggressiveInlining)]
    public static PatchOdyssey.Collections.GameObjectEnumerator EnumerateDescendants(this UnityEngine.GameObject gameObject) {
      return new(GameObjectEnumerator.Kind.Descendants, gameObject.transform);
    }

    [PatchMethod(AggressiveInlining)]
    public static PatchOdyssey.Collections.GameObjectEnumerator EnumerateHierarchy(this UnityEngine.GameObject gameObject) {
      return new(GameObjectEnumerator.Kind.Hierarchy, gameObject.transform);
    }

    [PatchMethod(AggressiveInlining)] public static T                     EnsureComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => (T) gameObject.EnsureComponent(typeof(T));
    [PatchMethod(AggressiveInlining)] public static UnityEngine.Component EnsureComponent   (this UnityEngine.GameObject gameObject, System.Type type)               { UnityEngine.Component? component = gameObject.GetComponent(type); return null != component ? component : gameObject.AddComponent(type); }

    [PatchMethod(AggressiveInlining)]
    public static bool Exists<T>(this T[] array, System.Predicate<T> match) {
      return System.Array.Exists<T>(array, match);
    }

    [PatchMethod(AggressiveInlining)] public static void Fill<T>(this T[] array, T value)                       { System.Array.Fill<T>(array, value); }
    [PatchMethod(AggressiveInlining)] public static void Fill<T>(this T[] array, T value, int index, int count) { System.Array.Fill<T>(array, value, index, count); }

    [PatchMethod(AggressiveInlining)]
    public static T? Find<T>(this T[] array, System.Predicate<T> match) {
      return System.Array.Find<T>(array, match);
    }

    [PatchMethod(AggressiveInlining)]
    public static T[] FindAll<T>(this T[] array, System.Predicate<T> match) {
      return System.Array.FindAll<T>(array, match);
    }

    [PatchMethod(AggressiveInlining)] public static UnityEngine.Component?  FindChild   (this UnityEngine.Component  component,  System.Predicate<UnityEngine.Component>  predicate)                                 => component.gameObject.FindChild<UnityEngine.Component>(predicate);
    [PatchMethod(AggressiveInlining)] public static T?                      FindChild<T>(this UnityEngine.Component  component,  System.Predicate<T>                      predicate) where T : UnityEngine.Component => component.gameObject.FindChild<T>                    (predicate);
    [PatchMethod(AggressiveInlining)] public static UnityEngine.GameObject? FindChild   (this UnityEngine.GameObject gameObject, System.Predicate<UnityEngine.GameObject> predicate)                                 { foreach (UnityEngine.GameObject child in gameObject.EnumerateChildren()) {                                         if                          (predicate(child))     return child; }     return null; }
    [PatchMethod(AggressiveInlining)] public static T?                      FindChild<T>(this UnityEngine.GameObject gameObject, System.Predicate<T>                      predicate) where T : UnityEngine.Component { foreach (UnityEngine.GameObject child in gameObject.EnumerateChildren()) { T? component = child.GetComponent<T>(); if (component is not null && predicate(component)) return component; } return null; }

    [PatchMethod(AggressiveInlining)] public static T?                     FindChildByComponent<T>(this UnityEngine.Component  component)  where T : UnityEngine.Component => component.gameObject.FindChildByComponent<T>();
    [PatchMethod(AggressiveInlining)] public static T?                     FindChildByComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject          .FindChildByComponent(typeof(T)) as T;
    [PatchMethod(AggressiveInlining)] public static UnityEngine.Component? FindChildByComponent   (this UnityEngine.Component  component,  System.Type type)               => component.gameObject.FindChildByComponent(type);
    [PatchMethod(AggressiveInlining)] public static UnityEngine.Component? FindChildByComponent   (this UnityEngine.GameObject gameObject, System.Type type)               { foreach (UnityEngine.GameObject child in gameObject.EnumerateChildren()) { UnityEngine.Component? component = child.GetComponent(type); if (component is not null) return component; } return null; }

    [PatchMethod(AggressiveInlining)] public static UnityEngine.GameObject? FindChildByIndex(this UnityEngine.Component  component,  uint index) => component.gameObject.FindChildByIndex(index);
    [PatchMethod(AggressiveInlining)] public static UnityEngine.GameObject? FindChildByIndex(this UnityEngine.GameObject gameObject, uint index) { for (System.Collections.IEnumerator enumerator = gameObject.transform.GetEnumerator(); enumerator.MoveNext(); ) { if (0u == index--) return ((UnityEngine.Transform) enumerator.Current).gameObject; } return null; }

    [PatchMethod(AggressiveInlining)] public static UnityEngine.GameObject? FindChildByName(this UnityEngine.Component  component,  string name) => component.gameObject.FindChildByName(name);
    [PatchMethod(AggressiveInlining)] public static UnityEngine.GameObject? FindChildByName(this UnityEngine.GameObject gameObject, string name) { foreach (UnityEngine.GameObject child in gameObject.EnumerateChildren()) { if (child.name == name) return child; } return null; }

    [PatchMethod(AggressiveInlining)] public static UnityEngine.GameObject? FindChildByTag(this UnityEngine.Component  component,  string tag) => component.gameObject.FindChildByTag(tag);
    [PatchMethod(AggressiveInlining)] public static UnityEngine.GameObject? FindChildByTag(this UnityEngine.GameObject gameObject, string tag) { foreach (UnityEngine.GameObject child in gameObject.EnumerateChildren()) { if (child.tag == tag) return child; } return null; }

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component>  FindChildren   (this UnityEngine.Component  component,  System.Predicate<UnityEngine.Component>  predicate)                                 => component.gameObject.FindChildren<UnityEngine.Component>(predicate);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      FindChildren<T>(this UnityEngine.Component  component,  System.Predicate<T>                      predicate) where T : UnityEngine.Component => component.gameObject.FindChildren<T>                    (predicate);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindChildren   (this UnityEngine.GameObject gameObject, System.Predicate<UnityEngine.GameObject> predicate)                                 { GameObjectSharedList<UnityEngine.GameObject> children = new((uint) gameObject.transform.childCount); children.Clear(); foreach (UnityEngine.GameObject child in gameObject.EnumerateChildren()) {                                         if                          (predicate(child))     children.Add(child); }     return children; }
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      FindChildren<T>(this UnityEngine.GameObject gameObject, System.Predicate<T>                      predicate) where T : UnityEngine.Component { GameObjectSharedList<T>                      children = new((uint) gameObject.transform.childCount); children.Clear(); foreach (UnityEngine.GameObject child in gameObject.EnumerateChildren()) { T? component = child.GetComponent<T>(); if (component is not null && predicate(component)) children.Add(component); } return children; }

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                     FindChildrenByComponent<T>(this UnityEngine.Component  component)  where T : UnityEngine.Component => component.gameObject.FindChildrenByComponent<T>();
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                     FindChildrenByComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject          .FindChildrenByComponent(typeof(T)).ConvertAll(static child => (T) child);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component> FindChildrenByComponent   (this UnityEngine.Component  component,  System.Type type)               => component.gameObject.FindChildrenByComponent(type);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component> FindChildrenByComponent   (this UnityEngine.GameObject gameObject, System.Type type)               { GameObjectSharedList<UnityEngine.Component> children = new((uint) gameObject.transform.childCount); children.Clear(); foreach (UnityEngine.GameObject child in gameObject.EnumerateChildren()) { UnityEngine.Component? component = child.GetComponent(type); if (component is not null) children.Add(component); } return children; }

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindChildrenByName(this UnityEngine.Component  component,  string name) => component.gameObject.FindChildrenByName(name);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindChildrenByName(this UnityEngine.GameObject gameObject, string name) { PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> children = new((uint) gameObject.transform.childCount); children.Clear(); foreach (UnityEngine.GameObject child in gameObject.EnumerateChildren()) { if (child.name == name) children.Add(child); } return children; }

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindChildrenByTag(this UnityEngine.Component  component,  string tag) => component.gameObject.FindChildrenByTag(tag);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindChildrenByTag(this UnityEngine.GameObject gameObject, string tag) { PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> children = new((uint) gameObject.transform.childCount); children.Clear(); foreach (UnityEngine.GameObject child in gameObject.EnumerateChildren()) { if (child.tag == tag) children.Add(child); } return children; }

    [PatchMethod(AggressiveInlining)] public static UnityEngine.Component?  FindDescendant   (this UnityEngine.Component  component,  System.Predicate<UnityEngine.Component>  predicate)                                 => component.gameObject.FindDescendant<UnityEngine.Component>(predicate);
    [PatchMethod(AggressiveInlining)] public static T?                      FindDescendant<T>(this UnityEngine.Component  component,  System.Predicate<T>                      predicate) where T : UnityEngine.Component => component.gameObject.FindDescendant<T>                    (predicate);
    [PatchMethod(AggressiveInlining)] public static UnityEngine.GameObject? FindDescendant   (this UnityEngine.GameObject gameObject, System.Predicate<UnityEngine.GameObject> predicate)                                 { foreach (UnityEngine.GameObject child in gameObject.EnumerateDescendants()) {                                         if                          (predicate(child))     return child; }     return null; }
    [PatchMethod(AggressiveInlining)] public static T?                      FindDescendant<T>(this UnityEngine.GameObject gameObject, System.Predicate<T>                      predicate) where T : UnityEngine.Component { foreach (UnityEngine.GameObject child in gameObject.EnumerateDescendants()) { T? component = child.GetComponent<T>(); if (component is not null && predicate(component)) return component; } return null; }

    [PatchMethod(AggressiveInlining)] public static T?                     FindDescendantByComponent<T>(this UnityEngine.Component  component)  where T : UnityEngine.Component => component.gameObject.FindDescendantByComponent<T>();
    [PatchMethod(AggressiveInlining)] public static T?                     FindDescendantByComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject          .FindDescendantByComponent(typeof(T)) as T;
    [PatchMethod(AggressiveInlining)] public static UnityEngine.Component? FindDescendantByComponent   (this UnityEngine.Component  component,  System.Type type)               => component.gameObject.FindDescendantByComponent(type);
    [PatchMethod(AggressiveInlining)] public static UnityEngine.Component? FindDescendantByComponent   (this UnityEngine.GameObject gameObject, System.Type type)               { foreach (UnityEngine.GameObject child in gameObject.EnumerateDescendants()) { UnityEngine.Component? component = child.GetComponent(type); if (component is not null) return component; } return null; }

    [PatchMethod(AggressiveInlining)] public static UnityEngine.GameObject? FindDescendantByIndex(this UnityEngine.Component  component,  uint index) => component.gameObject.FindDescendantByIndex(index);
    [PatchMethod(AggressiveInlining)] public static UnityEngine.GameObject? FindDescendantByIndex(this UnityEngine.GameObject gameObject, uint index) { for (System.Collections.IEnumerator enumerator = gameObject.transform.GetEnumerator(); enumerator.MoveNext(); ) { if (0u == index--) return ((UnityEngine.Transform) enumerator.Current).gameObject; } return null; }

    [PatchMethod(AggressiveInlining)] public static UnityEngine.GameObject? FindDescendantByName(this UnityEngine.Component  component,  string name) => component.gameObject.FindDescendantByName(name);
    [PatchMethod(AggressiveInlining)] public static UnityEngine.GameObject? FindDescendantByName(this UnityEngine.GameObject gameObject, string name) { foreach (UnityEngine.GameObject child in gameObject.EnumerateDescendants()) { if (child.name == name) return child; } return null; }

    [PatchMethod(AggressiveInlining)] public static UnityEngine.GameObject? FindDescendantByTag(this UnityEngine.Component  component,  string tag) => component.gameObject.FindDescendantByTag(tag);
    [PatchMethod(AggressiveInlining)] public static UnityEngine.GameObject? FindDescendantByTag(this UnityEngine.GameObject gameObject, string tag) { foreach (UnityEngine.GameObject child in gameObject.EnumerateDescendants()) { if (child.tag == tag) return child; } return null; }

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component>  FindDescendants   (this UnityEngine.Component  component,  System.Predicate<UnityEngine.Component>  predicate)                                 => component.gameObject.FindDescendants<UnityEngine.Component>(predicate);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      FindDescendants<T>(this UnityEngine.Component  component,  System.Predicate<T>                      predicate) where T : UnityEngine.Component => component.gameObject.FindDescendants<T>                    (predicate);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindDescendants   (this UnityEngine.GameObject gameObject, System.Predicate<UnityEngine.GameObject> predicate)                                 { GameObjectSharedList<UnityEngine.GameObject> descendants = new((uint) gameObject.transform.childCount); descendants.Clear(); foreach (UnityEngine.GameObject descendant in gameObject.EnumerateDescendants()) {                                              if                          (predicate(descendant)) descendants.Add(descendant); } return descendants; }
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      FindDescendants<T>(this UnityEngine.GameObject gameObject, System.Predicate<T>                      predicate) where T : UnityEngine.Component { GameObjectSharedList<T>                      descendants = new((uint) gameObject.transform.childCount); descendants.Clear(); foreach (UnityEngine.GameObject descendant in gameObject.EnumerateDescendants()) { T? component = descendant.GetComponent<T>(); if (component is not null && predicate(component))  descendants.Add(component); }  return descendants; }

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                     FindDescendantsByComponent<T>(this UnityEngine.Component  component)  where T : UnityEngine.Component => component.gameObject.FindDescendantsByComponent<T>();
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                     FindDescendantsByComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject          .FindDescendantsByComponent(typeof(T)).ConvertAll(static descendant => (T) descendant);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component> FindDescendantsByComponent   (this UnityEngine.Component  component,  System.Type type)               => component.gameObject.FindDescendantsByComponent(type);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component> FindDescendantsByComponent   (this UnityEngine.GameObject gameObject, System.Type type)               { GameObjectSharedList<UnityEngine.Component> descendants = new((uint) gameObject.transform.childCount); descendants.Clear(); foreach (UnityEngine.GameObject descendant in gameObject.EnumerateDescendants()) { UnityEngine.Component? component = descendant.GetComponent(type); if (component is not null) descendants.Add(component); } return descendants; }

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindDescendantsByName(this UnityEngine.Component  component,  string name) => component.gameObject.FindDescendantsByName(name);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindDescendantsByName(this UnityEngine.GameObject gameObject, string name) { PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> descendants = new((uint) gameObject.transform.childCount); descendants.Clear(); foreach (UnityEngine.GameObject descendant in gameObject.EnumerateDescendants()) { if (descendant.name == name) descendants.Add(descendant); } return descendants; }

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindDescendantsByTag(this UnityEngine.Component  component,  string tag) => component.gameObject.FindDescendantsByTag(tag);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindDescendantsByTag(this UnityEngine.GameObject gameObject, string tag) { PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> descendants = new((uint) gameObject.transform.childCount); descendants.Clear(); foreach (UnityEngine.GameObject descendant in gameObject.EnumerateDescendants()) { if (descendant.tag == tag) descendants.Add(descendant); } return descendants; }

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component>  FindHierarchy   (this UnityEngine.Component  component,  System.Predicate<UnityEngine.Component>  predicate)                                 => component.gameObject.FindHierarchy<UnityEngine.Component>(predicate);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      FindHierarchy<T>(this UnityEngine.Component  component,  System.Predicate<T>                      predicate) where T : UnityEngine.Component => component.gameObject.FindHierarchy<T>                    (predicate);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindHierarchy   (this UnityEngine.GameObject gameObject, System.Predicate<UnityEngine.GameObject> predicate)                                 { GameObjectSharedList<UnityEngine.GameObject> hierarchy = new((uint) gameObject.transform.childCount); hierarchy.Clear(); foreach (UnityEngine.GameObject child in gameObject.EnumerateHierarchy()) {                                         if                          (predicate(child))     hierarchy.Add(child); }     return hierarchy; }
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      FindHierarchy<T>(this UnityEngine.GameObject gameObject, System.Predicate<T>                      predicate) where T : UnityEngine.Component { GameObjectSharedList<T>                      hierarchy = new((uint) gameObject.transform.childCount); hierarchy.Clear(); foreach (UnityEngine.GameObject child in gameObject.EnumerateHierarchy()) { T? component = child.GetComponent<T>(); if (component is not null && predicate(component)) hierarchy.Add(component); } return hierarchy; }

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                     FindHierarchyByComponent<T>(this UnityEngine.Component  component)  where T : UnityEngine.Component => component.gameObject.FindHierarchyByComponent<T>();
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                     FindHierarchyByComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject          .FindHierarchyByComponent(typeof(T)).ConvertAll(static _ => (T) _);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component> FindHierarchyByComponent   (this UnityEngine.Component  component,  System.Type type)               => component.gameObject.FindHierarchyByComponent(type);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component> FindHierarchyByComponent   (this UnityEngine.GameObject gameObject, System.Type type)               { GameObjectSharedList<UnityEngine.Component> hierarchy = new((uint) gameObject.transform.childCount); hierarchy.Clear(); foreach (UnityEngine.GameObject child in gameObject.EnumerateHierarchy()) { UnityEngine.Component? component = child.GetComponent(type); if (component is not null) hierarchy.Add(component); } return hierarchy; }

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindHierarchyByName(this UnityEngine.Component  component,  string name) => component.gameObject.FindHierarchyByName(name);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindHierarchyByName(this UnityEngine.GameObject gameObject, string name) { PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> hierarchy = new((uint) gameObject.transform.childCount); hierarchy.Clear(); foreach (UnityEngine.GameObject child in gameObject.EnumerateHierarchy()) { if (child.name == name) hierarchy.Add(child); } return hierarchy; }

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindHierarchyByTag(this UnityEngine.Component  component,  string tag) => component.gameObject.FindHierarchyByTag(tag);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindHierarchyByTag(this UnityEngine.GameObject gameObject, string tag) { PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> hierarchy = new((uint) gameObject.transform.childCount); hierarchy.Clear(); foreach (UnityEngine.GameObject child in gameObject.EnumerateHierarchy()) { if (child.tag == tag) hierarchy.Add(child); } return hierarchy; }

    [PatchMethod(AggressiveInlining)] public static int FindIndex<T>(this T[] array,                       System.Predicate<T> match) { return System.Array.FindIndex<T>(array, match); }
    [PatchMethod(AggressiveInlining)] public static int FindIndex<T>(this T[] array, int index,            System.Predicate<T> match) { return System.Array.FindIndex<T>(array, index, match); }
    [PatchMethod(AggressiveInlining)] public static int FindIndex<T>(this T[] array, int index, int count, System.Predicate<T> match) { return System.Array.FindIndex<T>(array, index, count, match); }

    [PatchMethod(AggressiveInlining)]
    public static T? FindLast<T>(this T[] array, System.Predicate<T> match) {
      return System.Array.FindLast<T>(array, match);
    }

    [PatchMethod(AggressiveInlining)] public static int FindLastIndex<T>(this T[] array,                       System.Predicate<T> match) { return System.Array.FindLastIndex<T>(array, match); }
    [PatchMethod(AggressiveInlining)] public static int FindLastIndex<T>(this T[] array, int index,            System.Predicate<T> match) { return System.Array.FindLastIndex<T>(array, index, match); }
    [PatchMethod(AggressiveInlining)] public static int FindLastIndex<T>(this T[] array, int index, int count, System.Predicate<T> match) { return System.Array.FindLastIndex<T>(array, index, count, match); }

    [PatchMethod(AggressiveInlining)]
    public static void ForEach<T>(this T[] array, System.Action<T> action) {
      System.Array.ForEach<T>(array, action);
    }

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component>  GetChildren   (this UnityEngine.Component  component)                                  => component .FindChildrenByComponent(component.GetType());
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      GetChildren<T>(this UnityEngine.Component  component) where T : UnityEngine.Component  => component .FindChildrenByComponent<T>();
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> GetChildren   (this UnityEngine.GameObject gameObject)                                 => gameObject.FindChildren   (static child => true);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      GetChildren<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject.FindChildren<T>(static child => true);

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component>  GetDescendants   (this UnityEngine.Component  component)                                  => component .FindDescendantsByComponent(component.GetType());
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      GetDescendants<T>(this UnityEngine.Component  component) where T : UnityEngine.Component  => component .FindDescendantsByComponent<T>();
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> GetDescendants   (this UnityEngine.GameObject gameObject)                                 => gameObject.FindDescendants   (static child => true);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      GetDescendants<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject.FindDescendants<T>(static child => true);

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component>  GetHierarchy   (this UnityEngine.Component  component)                                  => component .FindHierarchyByComponent(component.GetType());
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      GetHierarchy<T>(this UnityEngine.Component  component) where T : UnityEngine.Component  => component .FindHierarchyByComponent<T>();
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> GetHierarchy   (this UnityEngine.GameObject gameObject)                                 => gameObject.FindHierarchy   (static child => true);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      GetHierarchy<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject.FindHierarchy<T>(static child => true);

    [PatchMethod(AggressiveInlining)] public static UnityEngine.GameObject GetParent(this UnityEngine.Component  component)  => component.gameObject.GetParent();
    [PatchMethod(AggressiveInlining)] public static UnityEngine.GameObject GetParent(this UnityEngine.GameObject gameObject) { return gameObject.transform.parent.gameObject; }

    [PatchMethod(AggressiveInlining)] public static bool HasChild(this UnityEngine.Component  component,  UnityEngine.Component  child)  => component.gameObject.HasChild(child);
    [PatchMethod(AggressiveInlining)] public static bool HasChild(this UnityEngine.Component  component,  UnityEngine.GameObject child)  => component.gameObject.HasChild(child);
    [PatchMethod(AggressiveInlining)] public static bool HasChild(this UnityEngine.GameObject gameObject, UnityEngine.Component  child)  => gameObject          .HasChild(child.gameObject);
    [PatchMethod(AggressiveInlining)] public static bool HasChild(this UnityEngine.GameObject gameObject, UnityEngine.GameObject target) { foreach (UnityEngine.GameObject child in gameObject.EnumerateChildren()) { if (child == target) return true; } return false; }

    [PatchMethod(AggressiveInlining)] public static bool HasComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject.HasComponent(typeof(T));
    [PatchMethod(AggressiveInlining)] public static bool HasComponent   (this UnityEngine.GameObject gameObject, System.Type type)               { return null != gameObject.GetComponent(type); }

    [PatchMethod(AggressiveInlining)] public static bool HasDescendant(this UnityEngine.Component  component,  UnityEngine.Component  child)  => component.gameObject.HasDescendant(child);
    [PatchMethod(AggressiveInlining)] public static bool HasDescendant(this UnityEngine.Component  component,  UnityEngine.GameObject child)  => component.gameObject.HasDescendant(child);
    [PatchMethod(AggressiveInlining)] public static bool HasDescendant(this UnityEngine.GameObject gameObject, UnityEngine.Component  child)  => gameObject          .HasDescendant(child.gameObject);
    [PatchMethod(AggressiveInlining)] public static bool HasDescendant(this UnityEngine.GameObject gameObject, UnityEngine.GameObject target) { foreach (UnityEngine.GameObject child in gameObject.EnumerateDescendants()) { if (child == target) return true; } return false; }

    [PatchMethod(AggressiveInlining)] public static int IndexOf   (this System.Array array, object? value)                       => System.Array.IndexOf   (array, value);
    [PatchMethod(AggressiveInlining)] public static int IndexOf   (this System.Array array, object? value, int index)            => System.Array.IndexOf   (array, value, index);
    [PatchMethod(AggressiveInlining)] public static int IndexOf   (this System.Array array, object? value, int index, int count) => System.Array.IndexOf   (array, value, index, count);
    [PatchMethod(AggressiveInlining)] public static int IndexOf<T>(this T[]          array, T       value)                       => System.Array.IndexOf<T>(array, value);
    [PatchMethod(AggressiveInlining)] public static int IndexOf<T>(this T[]          array, T       value, int index)            => System.Array.IndexOf<T>(array, value, index);
    [PatchMethod(AggressiveInlining)] public static int IndexOf<T>(this T[]          array, T       value, int index, int count) => System.Array.IndexOf<T>(array, value, index, count);

    [PatchMethod(AggressiveInlining)] public static int LastIndexOf   (this System.Array array, object? value)                       => System.Array.LastIndexOf   (array, value);
    [PatchMethod(AggressiveInlining)] public static int LastIndexOf   (this System.Array array, object? value, int index)            => System.Array.LastIndexOf   (array, value, index);
    [PatchMethod(AggressiveInlining)] public static int LastIndexOf   (this System.Array array, object? value, int index, int count) => System.Array.LastIndexOf   (array, value, index, count);
    [PatchMethod(AggressiveInlining)] public static int LastIndexOf<T>(this T[]          array, T       value)                       => System.Array.LastIndexOf<T>(array, value);
    [PatchMethod(AggressiveInlining)] public static int LastIndexOf<T>(this T[]          array, T       value, int index)            => System.Array.LastIndexOf<T>(array, value, index);
    [PatchMethod(AggressiveInlining)] public static int LastIndexOf<T>(this T[]          array, T       value, int index, int count) => System.Array.LastIndexOf<T>(array, value, index, count);

    [PatchMethod(AggressiveInlining)] public static object? Prepend   (this System.Collections.ArrayList             arrayList, object? value) { arrayList.Insert  (0, value); return value; }
    [PatchMethod(AggressiveInlining)] public static T       Prepend<T>(this System.Collections.ArrayList             arrayList, T       value) { arrayList.Insert  (0, value); return value; }
    [PatchMethod(AggressiveInlining)] public static T       Prepend<T>(this System.Collections.Generic.LinkedList<T> list,      T       value) { list     .AddFirst(value);    return value; }
    [PatchMethod(AggressiveInlining)] public static T       Prepend<T>(this System.Collections.Generic.List      <T> list,      T       value) { list     .Insert  (0, value); return value; }

    [PatchMethod(AggressiveInlining)]
    public static void Reset(this UnityEngine.Transform transform) {
      transform.localRotation = UnityEngine.Quaternion.identity;
      transform.localScale    = UnityEngine.Vector3   .one;
      transform.position      = UnityEngine.Vector3   .zero;
    }

    [PatchMethod(AggressiveInlining)] public static void Reverse   (this System.Array array)                        => System.Array.Reverse(array);
    [PatchMethod(AggressiveInlining)] public static void Reverse   (this System.Array array, int index, int length) => System.Array.Reverse(array, index, length);
    [PatchMethod(AggressiveInlining)] public static void Reverse<T>(this T[]          array)                        => array.Reverse<T>(0, array.Length);
    [PatchMethod(AggressiveInlining)] public static void Reverse<T>(this T[]          array, int index, int length) => array.Reverse   (index, length);

    [PatchMethod(AggressiveInlining)]
    public static void SetHeight(this UnityEngine.RectTransform transform, float height) {
      UnityEngine.RectTransform? parentTransform = transform.parent?.transform as UnityEngine.RectTransform;
      transform.sizeDelta = new(transform.sizeDelta.x, height - (parentTransform is not null ? parentTransform.rect.height * (transform.anchorMax.y - transform.anchorMin.y) : 0.0f));
    }

    [PatchMethod(AggressiveInlining)]
    public static void SetSize(this UnityEngine.RectTransform transform, UnityEngine.Vector2 size) {
      UnityEngine.RectTransform? parentTransform = transform.parent?.transform as UnityEngine.RectTransform;
      transform.sizeDelta = size - (parentTransform is not null ? UnityEngine.Vector2.Scale(parentTransform.rect.size, transform.anchorMax - transform.anchorMin) : UnityEngine.Vector2.zero);
    }

    [PatchMethod(AggressiveInlining)]
    public static void SetWidth(this UnityEngine.RectTransform transform, float width) {
      UnityEngine.RectTransform? parentTransform = transform.parent?.transform as UnityEngine.RectTransform;
      transform.sizeDelta = new(width - (parentTransform is not null ? parentTransform.rect.width * (transform.anchorMax.x - transform.anchorMin.x) : 0.0f), transform.sizeDelta.y);
    }

    [PatchMethod(AggressiveInlining)] public static void Sort   (this System.Array array)                                                                                                                => System.Array.Sort   (array);
    [PatchMethod(AggressiveInlining)] public static void Sort   (this System.Array array, System.Collections.IComparer? comparer)                                                                        => System.Array.Sort   (array, comparer);
    [PatchMethod(AggressiveInlining)] public static void Sort   (this System.Array array, int                           index, int length)                                                               => System.Array.Sort   (array, index, length);
    [PatchMethod(AggressiveInlining)] public static void Sort   (this System.Array array, int                           index, int length, System.Collections.IComparer? comparer)                       => System.Array.Sort   (array, index, length, comparer);
    [PatchMethod(AggressiveInlining)] public static void Sort<T>(this T[]          array)                                                                                                                => System.Array.Sort<T>(array);
    [PatchMethod(AggressiveInlining)] public static void Sort<T>(this T[]          array, System.Comparison<T>                     comparison)                                                           => System.Array.Sort<T>(array, comparison);
    [PatchMethod(AggressiveInlining)] public static void Sort<T>(this T[]          array, System.Collections.Generic.IComparer<T>? comparer)                                                             => System.Array.Sort<T>(array, comparer);
    [PatchMethod(AggressiveInlining)] public static void Sort<T>(this T[]          array, int                                      index, int length)                                                    => System.Array.Sort<T>(array, index, length);
    [PatchMethod(AggressiveInlining)] public static void Sort<T>(this T[]          array, int                                      index, int length, System.Collections.Generic.IComparer<T>? comparer) => System.Array.Sort<T>(array, index, length, comparer);

    [PatchMethod(AggressiveInlining)] public static void TrimExcess<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary)               => (dictionary as System.Collections.Generic.Dictionary<TKey, TValue>)?.TrimExcess();
    [PatchMethod(AggressiveInlining)] public static void TrimExcess<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary, int capacity) => (dictionary as System.Collections.Generic.Dictionary<TKey, TValue>)?.TrimExcess(capacity);

    [PatchMethod(AggressiveInlining)]
    public static bool TrueForAll<T>(this T[] array, System.Predicate<T> match) {
      return System.Array.TrueForAll<T>(array, match);
    }

    [PatchMethod(AggressiveInlining)]
    public static bool TryAdd<T>(this System.Collections.Generic.IList<T> list, T element) {
      if (!list.Contains(element)) {
        list.Add(element);
        return true;
      }

      return false;
    }

    [PatchMethod(AggressiveInlining)] public static bool TryAdd<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary, System.Collections.Generic.KeyValuePair<TKey, TValue> element) => dictionary.TryAdd(element.Key, element.Value);
    [PatchMethod(AggressiveInlining)]
    public static bool TryAdd<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary, TKey key, TValue value) {
      if (!dictionary.ContainsKey(key)) {
        dictionary.Add(key, value);
        return true;
      }

      return false;
    }

    #if !(NETCOREAPP2_1 || NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1 || NETSTANDARD2_1_OR_GREATER)
      [PatchMethod(AggressiveInlining)] public static void TrimExcess<TKey, TValue>(this System.Collections.Generic.Dictionary<TKey, TValue> dictionary)               { /* ⟶ Do nothing… */ }
      [PatchMethod(AggressiveInlining)] public static void TrimExcess<TKey, TValue>(this System.Collections.Generic.Dictionary<TKey, TValue> dictionary, int capacity) { /* ⟶ Do nothing… */ }
    #endif
  }

  public static partial class Util /* ⟶ Utilities */ {
    /* TODO */
    private sealed class LoadInfo {
    //   internal       object?              data     = null;  //
    //   internal event PatchOdyssey.Handler handlers = null!; // ⟶ Queued callbacks when the `::data` is correctly loaded
    //   internal       bool                 pending  = false; // ⟶ Denotes            if the `::data` is still     loading

    //   [PatchConstructor, PatchMethod(AggressiveInlining)]
    //   internal LoadInfo(object? data, System.Collections.Generic.IEnumerable<PatchOdyssey.Handler> handlers, bool pending) {
    //     this.data     = data;
    //     // this.handlers = new(handlers is null ? new PatchOdyssey.Handler[0] : handlers);
    //     this.pending  = pending;
    //   }
    }

    /* … */
    public   const           double  LoadAsynchronously = 0.0f;  // ⟶ `LoadURI*(…, double? loadDurationMaximum, …)`
    public   const           bool    LoadCached         = false; // ⟶ `LoadURI*(…, bool nocache)`
    public   const           bool    LoadDirectly       = true;  // ⟶ `LoadURI*(…, bool nocache)`
    public   static readonly double? LoadSynchronously  = null;  // ⟶ `LoadURI*(…, double? loadDurationMaximum, …)`
    public   const           int     MouseButtonLeft    = 0x0;   //
    public   const           int     MouseButtonMiddle  = 0x2;   //
    public   const           int     MouseButtonRight   = 0x1;   //
    internal const           int     RefSize            = 8;     // ⟶ Presumed byte size of managed/ reference types as structured within class types (i.e. `sizeof(void*)`) — relative liberal guess to avoid object splicing

    private static readonly System.Collections.ObjectModel.ReadOnlyCollection<int>                                                                         MOUSE_BUTTONS          = new[] {Util.MouseButtonLeft, Util.MouseButtonRight, Util.MouseButtonMiddle}.AsReadOnly();
    private static readonly System.Collections.Generic    .Dictionary        <string, Util.LoadInfo>                                                       LOADS                  = new(16);
    private static readonly System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.KeyCode>                                                         KEYS                   = new[] {UnityEngine.KeyCode.A, UnityEngine.KeyCode.Alpha0, UnityEngine.KeyCode.Alpha1, UnityEngine.KeyCode.Alpha2, UnityEngine.KeyCode.Alpha3, UnityEngine.KeyCode.Alpha4, UnityEngine.KeyCode.Alpha5, UnityEngine.KeyCode.Alpha6, UnityEngine.KeyCode.Alpha7, UnityEngine.KeyCode.Alpha8, UnityEngine.KeyCode.Alpha9, UnityEngine.KeyCode.AltGr, UnityEngine.KeyCode.Ampersand, UnityEngine.KeyCode.Asterisk, UnityEngine.KeyCode.At, UnityEngine.KeyCode.B, UnityEngine.KeyCode.BackQuote, UnityEngine.KeyCode.Backslash, UnityEngine.KeyCode.Backspace, UnityEngine.KeyCode.Break, UnityEngine.KeyCode.C, UnityEngine.KeyCode.CapsLock, UnityEngine.KeyCode.Caret, UnityEngine.KeyCode.Clear, UnityEngine.KeyCode.Colon, UnityEngine.KeyCode.Comma, UnityEngine.KeyCode.D, UnityEngine.KeyCode.Delete, UnityEngine.KeyCode.Dollar, UnityEngine.KeyCode.DoubleQuote, UnityEngine.KeyCode.DownArrow, UnityEngine.KeyCode.E, UnityEngine.KeyCode.End, UnityEngine.KeyCode.Equals, UnityEngine.KeyCode.Escape, UnityEngine.KeyCode.Exclaim, UnityEngine.KeyCode.F, UnityEngine.KeyCode.F1, UnityEngine.KeyCode.F10, UnityEngine.KeyCode.F11, UnityEngine.KeyCode.F12, UnityEngine.KeyCode.F13, UnityEngine.KeyCode.F14, UnityEngine.KeyCode.F15, UnityEngine.KeyCode.F2, UnityEngine.KeyCode.F3, UnityEngine.KeyCode.F4, UnityEngine.KeyCode.F5, UnityEngine.KeyCode.F6, UnityEngine.KeyCode.F7, UnityEngine.KeyCode.F8, UnityEngine.KeyCode.F9, UnityEngine.KeyCode.G, UnityEngine.KeyCode.Greater, UnityEngine.KeyCode.H, UnityEngine.KeyCode.Hash, UnityEngine.KeyCode.Help, UnityEngine.KeyCode.Home, UnityEngine.KeyCode.I, UnityEngine.KeyCode.Insert, UnityEngine.KeyCode.J, UnityEngine.KeyCode.K, UnityEngine.KeyCode.Keypad0, UnityEngine.KeyCode.Keypad1, UnityEngine.KeyCode.Keypad2, UnityEngine.KeyCode.Keypad3, UnityEngine.KeyCode.Keypad4, UnityEngine.KeyCode.Keypad5, UnityEngine.KeyCode.Keypad6, UnityEngine.KeyCode.Keypad7, UnityEngine.KeyCode.Keypad8, UnityEngine.KeyCode.Keypad9, UnityEngine.KeyCode.KeypadDivide, UnityEngine.KeyCode.KeypadEnter, UnityEngine.KeyCode.KeypadEquals, UnityEngine.KeyCode.KeypadMinus, UnityEngine.KeyCode.KeypadMultiply, UnityEngine.KeyCode.KeypadPeriod, UnityEngine.KeyCode.KeypadPlus, UnityEngine.KeyCode.L, UnityEngine.KeyCode.LeftAlt, UnityEngine.KeyCode.LeftApple, UnityEngine.KeyCode.LeftArrow, UnityEngine.KeyCode.LeftBracket, UnityEngine.KeyCode.LeftCommand, UnityEngine.KeyCode.LeftControl, UnityEngine.KeyCode.LeftCurlyBracket, UnityEngine.KeyCode.LeftMeta, UnityEngine.KeyCode.LeftParen, UnityEngine.KeyCode.LeftShift, UnityEngine.KeyCode.LeftWindows, UnityEngine.KeyCode.Less, UnityEngine.KeyCode.M, UnityEngine.KeyCode.Menu, UnityEngine.KeyCode.Minus, UnityEngine.KeyCode.N, UnityEngine.KeyCode.Numlock, UnityEngine.KeyCode.O, UnityEngine.KeyCode.P, UnityEngine.KeyCode.PageDown, UnityEngine.KeyCode.PageUp, UnityEngine.KeyCode.Pause, UnityEngine.KeyCode.Percent, UnityEngine.KeyCode.Period, UnityEngine.KeyCode.Pipe, UnityEngine.KeyCode.Plus, UnityEngine.KeyCode.Print, UnityEngine.KeyCode.Q, UnityEngine.KeyCode.Question, UnityEngine.KeyCode.Quote, UnityEngine.KeyCode.R, UnityEngine.KeyCode.Return, UnityEngine.KeyCode.RightAlt, UnityEngine.KeyCode.RightApple, UnityEngine.KeyCode.RightArrow, UnityEngine.KeyCode.RightBracket, UnityEngine.KeyCode.RightCommand, UnityEngine.KeyCode.RightControl, UnityEngine.KeyCode.RightCurlyBracket, UnityEngine.KeyCode.RightMeta, UnityEngine.KeyCode.RightParen, UnityEngine.KeyCode.RightShift, UnityEngine.KeyCode.RightWindows, UnityEngine.KeyCode.S, UnityEngine.KeyCode.ScrollLock, UnityEngine.KeyCode.Semicolon, UnityEngine.KeyCode.Slash, UnityEngine.KeyCode.Space, UnityEngine.KeyCode.SysReq, UnityEngine.KeyCode.T, UnityEngine.KeyCode.Tab, UnityEngine.KeyCode.Tilde, UnityEngine.KeyCode.U, UnityEngine.KeyCode.Underscore, UnityEngine.KeyCode.UpArrow, UnityEngine.KeyCode.V, UnityEngine.KeyCode.W, UnityEngine.KeyCode.X, UnityEngine.KeyCode.Y, UnityEngine.KeyCode.Z}.AsReadOnly();
    private static readonly System.Collections.Generic    .Dictionary        <System.ValueTuple<System.Type, System.Type>, System.Delegate>                DELEGATED_CONVERTS     = new(1);
    private static readonly System.Collections.ObjectModel.ReadOnlyDictionary<System.Type, System.Collections.ObjectModel.ReadOnlyCollection<System.Type>> IMPLICIT_TYPE_CONVERTS = new System.Collections.Generic.Dictionary<System.Type, System.Collections.ObjectModel.ReadOnlyCollection<System.Type>>() {
      {typeof(byte),   new[] {typeof(decimal), typeof(double), typeof(float), typeof(int), typeof(long), typeof(nint), typeof(nuint), typeof(short), typeof(uint), typeof(ulong), typeof(ushort)}.AsReadOnly()},
      {typeof(float),  new[] {typeof(double)}                                                                                                                                                    .AsReadOnly()},
      {typeof(int),    new[] {typeof(decimal), typeof(double), typeof(float), typeof(long), typeof(nint)}                                                                                        .AsReadOnly()},
      {typeof(long),   new[] {typeof(decimal), typeof(double), typeof(float)}                                                                                                                    .AsReadOnly()},
      {typeof(nint),   new[] {typeof(decimal), typeof(double), typeof(float), typeof(long)}                                                                                                      .AsReadOnly()},
      {typeof(nuint),  new[] {typeof(decimal), typeof(double), typeof(float), typeof(ulong)}                                                                                                     .AsReadOnly()},
      {typeof(sbyte),  new[] {typeof(decimal), typeof(double), typeof(float), typeof(int),  typeof(long),  typeof(nint), typeof(short)}                                                          .AsReadOnly()},
      {typeof(short),  new[] {typeof(decimal), typeof(double), typeof(float), typeof(int),  typeof(long),  typeof(nint)}                                                                         .AsReadOnly()},
      {typeof(uint),   new[] {typeof(decimal), typeof(double), typeof(float), typeof(long), typeof(nuint), typeof(ulong)}                                                                        .AsReadOnly()},
      {typeof(ulong),  new[] {typeof(decimal), typeof(double), typeof(float)}                                                                                                                    .AsReadOnly()},
      {typeof(ushort), new[] {typeof(decimal), typeof(double), typeof(float), typeof(int), typeof(long), typeof(nint), typeof(nuint), typeof(uint), typeof(ulong)}                               .AsReadOnly()}
    }.AsReadOnly();

    /* … */
    [PatchMethod(AggressiveInlining)] public static object[]     ArrayFrom   ()                                                 { return new object    [0]; }
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>()                                                 { return new T         [0]; }
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(T[]                                 array)        { return array ?? new T[0]; }
    [PatchMethod(AggressiveInlining)] public static System.Array ArrayFrom   (System.Array                        array)        { return array; }
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(System.Array                        array)        { return (T[]) array; }
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(System.ArraySegment<T>              arraySegment) { return       arraySegment.ToArray(); }
    [PatchMethod(AggressiveInlining)] public static object[]     ArrayFrom   (System.Collections.ArrayList        arrayList)    { return       arrayList   .ToArray(); }
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(System.Collections.ArrayList        arrayList)    { return (T[]) arrayList   .ToArray(typeof(T)); }
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(System.Collections.IEnumerable      enumerable)   { return Util.ArrayFrom(enumerable).ConvertAll(static element => (T) element); }
    [PatchMethod(AggressiveInlining)] public static object[]     ArrayFrom   (System.Collections.Queue            queue)        { return queue .ToArray(); }
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(System.Collections.Queue            queue)        { return queue .ToArray().ConvertAll(static element => (T) element); }
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(System.Collections.Generic.List <T> list)         { return list  .ToArray(); }
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(System.Collections.Generic.Queue<T> queue)        { return queue .ToArray(); }
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(System.Collections.Generic.Stack<T> stack)        { return stack .ToArray(); }
    [PatchMethod(AggressiveInlining)] public static object[]     ArrayFrom   (System.Collections.Stack            stack)        { return stack .ToArray(); }
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(System.Collections.Stack            stack)        { return stack .ToArray().ConvertAll(static element => (T) element); }
    [PatchMethod(AggressiveInlining)] public static byte  []     ArrayFrom   (System.IO.MemoryStream              stream)       { return stream.ToArray(); }
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(System.Memory        <T>            memory)       { return memory.ToArray(); }
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(System.ReadOnlyMemory<T>            memory)       { return memory.ToArray(); }

    [PatchMethod(AggressiveInlining)]
    public static bool[] ArrayFrom(System.Collections.BitArray bits) {
      bool[] array  = new bool[bits.Length];
      uint   length = 0u;

      // …
      foreach (bool bit in bits)
      array[length++] = bit;

      return array;
    }

    [PatchMethod(AggressiveInlining)]
    public static object[] ArrayFrom(System.Collections.IEnumerable enumerable) {
      System.Collections.Generic.List<object> array = new();

      // …
      foreach (object value in enumerable)
      array.Add(value);

      return Util.ArrayFrom(array);
    }

    [PatchMethod(AggressiveInlining)]
    public static T[] ArrayFrom<T>(System.Collections.Generic.IEnumerable<T> enumerable) {
      System.Collections.Generic.List<T> array = new();

      // …
      foreach (T value in enumerable)
      array.Add(value);

      return Util.ArrayFrom(array);
    }

    public static T[] ArrayFrom<T>(System.Collections.Generic.IEnumerable<T> enumerableA, System.Collections.Generic.IEnumerable<T> enumerableB) {
      if (enumerableB is null) return enumerableA is not null ? Util.ArrayFrom(enumerableA) : new T[0];
      if (enumerableA is null) return enumerableB is not null ? Util.ArrayFrom(enumerableB) : new T[0];

      return Util.ArrayFrom(new[] {enumerableA, enumerableB});
    }

    public static T[] ArrayFrom<T>(params System.Collections.Generic.IEnumerable<T>[] enumerables) {
      System.Collections.Generic.List<T>? concatenation = null;

      // … ⟶ Mimic optimization of `Util.ArrayFrom<T>(T, T)` where the only non-null `enumerable` would be evaluated as-is, rather than concatenated.
      #if false
        (System.Collections.Generic.IEnumerable<T>? alone, System.Collections.Generic.IEnumerable<T>? accompanying) enumerated = (null, null);

        // …
        foreach (System.Collections.Generic.IEnumerable<T> enumerable in enumerables)
        if (enumerable is not null) {
          if (enumerated.alone is not null) {
            concatenation ??=  new(enumerated.alone);
            concatenation.AddRange(enumerated.accompanying = enumerable);
          }

          enumerated.alone ??= enumerable;
        }

        if (enumerated.alone is not null && enumerated.accompanying is null) return Util.ArrayFrom(enumerated.alone);
        if (concatenation is null)                                           return new T[0];
      #else
        concatenation = new(enumerables.Length);

        foreach (System.Collections.Generic.IEnumerable<T> enumerable in enumerables) {
          if (enumerable is not null)
          concatenation.AddRange(enumerable);
        }
      #endif

      return Util.ArrayFrom(concatenation);
    }

    public static T[] ArrayFromMembers<T>(object structure) where T : class? {
      static System.Func<object?, object?[]?, object?> DelegateCast<U>() where U : class? {
        foreach (System.Reflection.MethodInfo method in typeof(U).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)) {
          System.Reflection.ParameterInfo[] parameters = method.GetParameters();

          if (method.Name == "op_Implicit" && method.ReturnType == typeof(U) && 0 != parameters.Length && parameters[0].ParameterType == typeof(U))
          return method.Invoke;
        }

        return static (object? target, object?[]? parameters) => (object?) (parameters![0] as U); // ⟶ `𝑓 delegate::Invoke(…)`
      }

      if (structure is not null) {
        System.Reflection.MemberInfo[]     members    = structure.GetType().GetMembers();
        System.Collections.Generic.List<T> decomposed = new(members.Length);
        T?[][]                             arrays     = members.ConvertAll(member => {
          object?      value = member switch { System.Reflection.FieldInfo field => field.GetValue(structure), System.Reflection.PropertyInfo property => property.GetValue(structure), _ => null };
          System.Type? type  = value?.GetType();

          // … ⟶ Decompose array field/ property members also
          if (Util.IsConvertibleType(type!, typeof(T)))                    return new[]          {DelegateCast<T>   ().Invoke(null, new[] {value}) as T};
          if (Util.IsConvertibleType(type!, typeof(T [])) && value is T[]) return Util.ArrayFrom((DelegateCast<T []>().Invoke(null, new[] {value}) as T [])!);
          if (Util.IsConvertibleType(type!, typeof(T?[])))                 return Util.ArrayFrom((DelegateCast<T?[]>().Invoke(null, new[] {value}) as T?[])!);

          return new T?[0];
        });

        // …
        foreach (T?[] array in arrays)
        foreach (T?   value in array) {
          if (value is not null)
          decomposed.Add(value);
        }

        return Util.ArrayFrom(decomposed);
      }

      return null!; // ⟶ `structure` is null
    }

    public static uint CheckWaitForTimer() {
      uint                                                                                           count        = 0u;
      double                                                                                         timestamp    = UnityEngine.Time.realtimeSinceStartupAsDouble;
      PatchOdyssey.Collections.WaitInfo                                                              wait         = WaitInfo.WAITS[double.NaN];
      ref readonly PatchOdyssey.Collections.EventHandler<PatchOdyssey.Collections.WaitForTimerEvent> waitHandlers = ref wait.handlers;

      // … ⟶ Enumeration is messy because the final design could not succinctly account for a timer-based model
      for (int index = waitHandlers.handlers.Count; 0 != index--; ) {
        PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Collections.WaitForTimerEvent> waitHandler = waitHandlers.handlers[index];
        (double waitDelay, double waitTimestamp)                                                     = waitHandler.metadata.data;

        // …
        if (!(timestamp < waitTimestamp)) {
          if (double.IsNaN(waitDelay) || 0.0 >= waitDelay) { ++count; waitHandlers.handlers.RemoveAt(index); }                               // ⟶ Remove `WaitForTimerUntil(…)` handlers, or
          else waitHandlers.handlers[index] = new(waitHandler.value, waitHandler.target, new() {data = (waitDelay, timestamp + waitDelay)}); // ⟶ Update `WaitForTimerEvery(…)` handlers

          // … ⟶ The wait has elapsed
          waitHandler.metadata.delay = System.Math.Abs(waitHandler.metadata.delay);
          waitHandler.Invoke();
        }
      }

      return count;
    }

    public static UnityEngine.Vector3[] CornersFromRect(UnityEngine.Rect rectangle) {
      // ⟶ Origin begins from bottom-left rather than top-left
      return new UnityEngine.Vector3[4] {
        new(rectangle.xMin, rectangle.yMax, 0.0f),
        new(rectangle.xMin, rectangle.yMin, 0.0f),
        new(rectangle.xMax, rectangle.yMin, 0.0f),
        new(rectangle.xMax, rectangle.yMax, 0.0f)
      };
    }

    public static UnityEngine.Vector3[]? CornersFromRectTransform(System.Action<UnityEngine.Vector3[]?>? transformMethod) {
      UnityEngine.Vector3[]? corners = null;

      // …
      if (transformMethod?.Target is UnityEngine.RectTransform)
      transformMethod(corners = new UnityEngine.Vector3[4]);

      return corners;
    }

    public static System.Delegate DelegateConvert(System.Type typeA, System.Type typeB) {
      if (typeA == typeB)                                                                return (System.Func<object, object>) (static _ => _);
      if (DELEGATED_CONVERTS.TryGetValue((typeA, typeB), out System.Delegate converter)) return converter;

      System.Linq.Expressions.ParameterExpression expression = System.Linq.Expressions.Expression.Parameter(typeA);
      return DELEGATED_CONVERTS[(typeA, typeB)] = System.Linq.Expressions.Expression.Lambda(System.Linq.Expressions.Expression.Convert(expression, typeB), expression).Compile();
    }

    public static System.Predicate<T> DelegateEquals<T>(T value) {
      return subvalue => (value as System.IEquatable<T>)?.Equals(subvalue) ?? (object) value! == (object) subvalue!;
    }

    [PatchMethod(AggressiveInlining)] public static uint EnumerableCount<T>(System.Collections.IEnumerable            enumerable) => Util.EnumerableCount((System.Collections.IEnumerable) enumerable);
    [PatchMethod(AggressiveInlining)] public static uint EnumerableCount<T>(System.Collections.Generic.IEnumerable<T> enumerable) => Util.EnumerableCount((System.Collections.IEnumerable) enumerable);
    [PatchMethod(AggressiveInlining)]
    public static uint EnumerableCount(System.Collections.IEnumerable enumerable) {
      uint                           count      = 0u;
      System.Collections.IEnumerator enumerator = enumerable.GetEnumerator();

      // …
      while (enumerator.MoveNext())
        ++count;

      (enumerator as System.IDisposable)?.Dispose();
      return count;
    }

    public static UnityEngine.Vector2    ExcludeVectorAxes       (UnityEngine.Vector2    vector, UnityEngine.Vector2    axes) { return new(axes.x != 1.0f ? vector.x : 0.0f, axes.y != 1.0f ? vector.y : 0.0f); }
    public static UnityEngine.Vector2    ExcludeVectorAxes       (UnityEngine.Vector2    vector, UnityEngine.Vector2Int axes) => Util.ExcludeVectorAxes(vector, new UnityEngine.Vector2   ((float) axes.x, (float) axes.y));
    public static UnityEngine.Vector2    ExcludeVectorAxes       (UnityEngine.Vector2    vector, UnityEngine.Vector3    axes) => Util.ExcludeVectorAxes(vector, new UnityEngine.Vector2   ((float) axes.x, (float) axes.y));
    public static UnityEngine.Vector2    ExcludeVectorAxes       (UnityEngine.Vector2    vector, UnityEngine.Vector3Int axes) => Util.ExcludeVectorAxes(vector, new UnityEngine.Vector2   ((float) axes.x, (float) axes.y));
    public static UnityEngine.Vector2    ExcludeVectorAxes       (UnityEngine.Vector2    vector, UnityEngine.Vector4    axes) => Util.ExcludeVectorAxes(vector, new UnityEngine.Vector2   ((float) axes.x, (float) axes.y));
    public static UnityEngine.Vector2Int ExcludeVectorAxes       (UnityEngine.Vector2Int vector, UnityEngine.Vector2    axes) => Util.ExcludeVectorAxes(vector, new UnityEngine.Vector2Int((int)   axes.x, (int)   axes.y));
    public static UnityEngine.Vector2Int ExcludeVectorAxes       (UnityEngine.Vector2Int vector, UnityEngine.Vector2Int axes) { return new(axes.x != 1 ? vector.x : 0, axes.y != 1 ? vector.y : 0); }
    public static UnityEngine.Vector2Int ExcludeVectorAxes       (UnityEngine.Vector2Int vector, UnityEngine.Vector3    axes) => Util.ExcludeVectorAxes(vector, new UnityEngine.Vector2Int((int)   axes.x, (int)   axes.y));
    public static UnityEngine.Vector2Int ExcludeVectorAxes       (UnityEngine.Vector2Int vector, UnityEngine.Vector3Int axes) => Util.ExcludeVectorAxes(vector, new UnityEngine.Vector2Int((int)   axes.x, (int)   axes.y));
    public static UnityEngine.Vector2Int ExcludeVectorAxes       (UnityEngine.Vector2Int vector, UnityEngine.Vector4    axes) => Util.ExcludeVectorAxes(vector, new UnityEngine.Vector2Int((int)   axes.x, (int)   axes.y));
    public static UnityEngine.Vector3    ExcludeVectorAxes       (UnityEngine.Vector3    vector, UnityEngine.Vector2    axes) => Util.ExcludeVectorAxes(vector, new UnityEngine.Vector3   ((float) axes.x, (float) axes.y, (float) 1.0f));
    public static UnityEngine.Vector3    ExcludeVectorAxes       (UnityEngine.Vector3    vector, UnityEngine.Vector2Int axes) => Util.ExcludeVectorAxes(vector, new UnityEngine.Vector3   ((float) axes.x, (float) axes.y, (float) 1.0f));
    public static UnityEngine.Vector3    ExcludeVectorAxes       (UnityEngine.Vector3    vector, UnityEngine.Vector3    axes) { return new(axes.x != 1.0f ? vector.x : 0.0f, axes.y != 1.0f ? vector.y : 0.0f, axes.z != 1.0f ? vector.z : 0.0f); }
    public static UnityEngine.Vector3    ExcludeVectorAxes       (UnityEngine.Vector3    vector, UnityEngine.Vector3Int axes) => Util.ExcludeVectorAxes(vector, new UnityEngine.Vector3   ((float) axes.x, (float) axes.y, (float) axes.z));
    public static UnityEngine.Vector3    ExcludeVectorAxes       (UnityEngine.Vector3    vector, UnityEngine.Vector4    axes) => Util.ExcludeVectorAxes(vector, new UnityEngine.Vector3   ((float) axes.x, (float) axes.y, (float) axes.z));
    public static UnityEngine.Vector3Int ExcludeVectorAxes       (UnityEngine.Vector3Int vector, UnityEngine.Vector2    axes) => Util.ExcludeVectorAxes(vector, new UnityEngine.Vector3Int((int)   axes.x, (int)   axes.y, (int)   0));
    public static UnityEngine.Vector3Int ExcludeVectorAxes       (UnityEngine.Vector3Int vector, UnityEngine.Vector2Int axes) => Util.ExcludeVectorAxes(vector, new UnityEngine.Vector3Int((int)   axes.x, (int)   axes.y, (int)   0));
    public static UnityEngine.Vector3Int ExcludeVectorAxes       (UnityEngine.Vector3Int vector, UnityEngine.Vector3    axes) => Util.ExcludeVectorAxes(vector, new UnityEngine.Vector3Int((int)   axes.x, (int)   axes.y, (int)   axes.z));
    public static UnityEngine.Vector3Int ExcludeVectorAxes       (UnityEngine.Vector3Int vector, UnityEngine.Vector3Int axes) { return new(axes.x != 1 ? vector.x : 0, axes.y != 1 ? vector.y : 0, axes.z != 1 ? vector.z : 0); }
    public static UnityEngine.Vector3Int ExcludeVectorAxes       (UnityEngine.Vector3Int vector, UnityEngine.Vector4    axes) => Util.ExcludeVectorAxes(vector, new UnityEngine.Vector3Int((int)   axes.x, (int)   axes.y, (int)   axes.z));
    public static UnityEngine.Vector4    ExcludeVectorAxes       (UnityEngine.Vector4    vector, UnityEngine.Vector2    axes) => Util.ExcludeVectorAxes(vector, new UnityEngine.Vector4   ((float) axes.x, (float) axes.y, (float) 1.0f,   (float) 1.0f));
    public static UnityEngine.Vector4    ExcludeVectorAxes       (UnityEngine.Vector4    vector, UnityEngine.Vector2Int axes) => Util.ExcludeVectorAxes(vector, new UnityEngine.Vector4   ((float) axes.x, (float) axes.y, (float) 1.0f,   (float) 1.0f));
    public static UnityEngine.Vector4    ExcludeVectorAxes       (UnityEngine.Vector4    vector, UnityEngine.Vector3    axes) => Util.ExcludeVectorAxes(vector, new UnityEngine.Vector4   ((float) axes.x, (float) axes.y, (float) axes.z, (float) 1.0f));
    public static UnityEngine.Vector4    ExcludeVectorAxes       (UnityEngine.Vector4    vector, UnityEngine.Vector3Int axes) => Util.ExcludeVectorAxes(vector, new UnityEngine.Vector4   ((float) axes.x, (float) axes.y, (float) axes.z, (float) 1.0f));
    public static UnityEngine.Vector4    ExcludeVectorAxes       (UnityEngine.Vector4    vector, UnityEngine.Vector4    axes) { return new(axes.x != 1.0f ? vector.x : 0.0f, axes.y != 1.0f ? vector.y : 0.0f, axes.z != 1.0f ? vector.z : 0.0f, axes.w != 1.0f ? vector.w : 0.0f); }
    public static UnityEngine.Vector3    ExcludeVectorBackAxes   (UnityEngine.Vector3    vector)                              => Util.ExcludeVectorAxes       (vector, UnityEngine.Vector3   .back);
    public static UnityEngine.Vector3Int ExcludeVectorBackAxes   (UnityEngine.Vector3Int vector)                              => Util.ExcludeVectorAxes       (vector, UnityEngine.Vector3Int.back);
    public static UnityEngine.Vector2    ExcludeVectorDownAxes   (UnityEngine.Vector2    vector)                              => Util.ExcludeVectorAxes       (vector, UnityEngine.Vector2   .down);
    public static UnityEngine.Vector2Int ExcludeVectorDownAxes   (UnityEngine.Vector2Int vector)                              => Util.ExcludeVectorAxes       (vector, UnityEngine.Vector2Int.down);
    public static UnityEngine.Vector3    ExcludeVectorDownAxes   (UnityEngine.Vector3    vector)                              => Util.ExcludeVectorAxes       (vector, UnityEngine.Vector3   .down);
    public static UnityEngine.Vector3Int ExcludeVectorDownAxes   (UnityEngine.Vector3Int vector)                              => Util.ExcludeVectorAxes       (vector, UnityEngine.Vector3Int.down);
    public static UnityEngine.Vector3    ExcludeVectorForwardAxes(UnityEngine.Vector3    vector)                              => Util.ExcludeVectorAxes       (vector, UnityEngine.Vector3   .forward);
    public static UnityEngine.Vector3Int ExcludeVectorForwardAxes(UnityEngine.Vector3Int vector)                              => Util.ExcludeVectorAxes       (vector, UnityEngine.Vector3Int.forward);
    public static UnityEngine.Vector2    ExcludeVectorLeftAxes   (UnityEngine.Vector2    vector)                              => Util.ExcludeVectorAxes       (vector, UnityEngine.Vector2   .left);
    public static UnityEngine.Vector2Int ExcludeVectorLeftAxes   (UnityEngine.Vector2Int vector)                              => Util.ExcludeVectorAxes       (vector, UnityEngine.Vector2Int.left);
    public static UnityEngine.Vector3    ExcludeVectorLeftAxes   (UnityEngine.Vector3    vector)                              => Util.ExcludeVectorAxes       (vector, UnityEngine.Vector3   .left);
    public static UnityEngine.Vector3Int ExcludeVectorLeftAxes   (UnityEngine.Vector3Int vector)                              => Util.ExcludeVectorAxes       (vector, UnityEngine.Vector3Int.left);
    public static UnityEngine.Vector2    ExcludeVectorRightAxes  (UnityEngine.Vector2    vector)                              => Util.ExcludeVectorAxes       (vector, UnityEngine.Vector2   .right);
    public static UnityEngine.Vector2Int ExcludeVectorRightAxes  (UnityEngine.Vector2Int vector)                              => Util.ExcludeVectorAxes       (vector, UnityEngine.Vector2Int.right);
    public static UnityEngine.Vector3    ExcludeVectorRightAxes  (UnityEngine.Vector3    vector)                              => Util.ExcludeVectorAxes       (vector, UnityEngine.Vector3   .right);
    public static UnityEngine.Vector3Int ExcludeVectorRightAxes  (UnityEngine.Vector3Int vector)                              => Util.ExcludeVectorAxes       (vector, UnityEngine.Vector3Int.right);
    public static UnityEngine.Vector2    ExcludeVectorUpAxes     (UnityEngine.Vector2    vector)                              => Util.ExcludeVectorAxes       (vector, UnityEngine.Vector2   .up);
    public static UnityEngine.Vector2Int ExcludeVectorUpAxes     (UnityEngine.Vector2Int vector)                              => Util.ExcludeVectorAxes       (vector, UnityEngine.Vector2Int.up);
    public static UnityEngine.Vector3    ExcludeVectorUpAxes     (UnityEngine.Vector3    vector)                              => Util.ExcludeVectorAxes       (vector, UnityEngine.Vector3   .up);
    public static UnityEngine.Vector3Int ExcludeVectorUpAxes     (UnityEngine.Vector3Int vector)                              => Util.ExcludeVectorAxes       (vector, UnityEngine.Vector3Int.up);
    public static UnityEngine.Vector3    ExcludeVectorDepthAxes  (UnityEngine.Vector3    vector)                              => Util.ExcludeVectorForwardAxes(vector);
    public static UnityEngine.Vector3Int ExcludeVectorDepthAxes  (UnityEngine.Vector3Int vector)                              => Util.ExcludeVectorForwardAxes(vector);
    public static UnityEngine.Vector2    ExcludeVectorHeightAxes (UnityEngine.Vector2    vector)                              => Util.ExcludeVectorUpAxes     (vector);
    public static UnityEngine.Vector2Int ExcludeVectorHeightAxes (UnityEngine.Vector2Int vector)                              => Util.ExcludeVectorUpAxes     (vector);
    public static UnityEngine.Vector3    ExcludeVectorHeightAxes (UnityEngine.Vector3    vector)                              => Util.ExcludeVectorUpAxes     (vector);
    public static UnityEngine.Vector3Int ExcludeVectorHeightAxes (UnityEngine.Vector3Int vector)                              => Util.ExcludeVectorUpAxes     (vector);
    public static UnityEngine.Vector2    ExcludeVectorWidthAxes  (UnityEngine.Vector2    vector)                              => Util.ExcludeVectorRightAxes  (vector);
    public static UnityEngine.Vector2Int ExcludeVectorWidthAxes  (UnityEngine.Vector2Int vector)                              => Util.ExcludeVectorRightAxes  (vector);
    public static UnityEngine.Vector3    ExcludeVectorWidthAxes  (UnityEngine.Vector3    vector)                              => Util.ExcludeVectorRightAxes  (vector);
    public static UnityEngine.Vector3Int ExcludeVectorWidthAxes  (UnityEngine.Vector3Int vector)                              => Util.ExcludeVectorRightAxes  (vector);
    public static UnityEngine.Vector2    ExcludeVectorXAxes      (UnityEngine.Vector2    vector)                              => Util.ExcludeVectorAxes       (vector, new UnityEngine.Vector2   ((float) 1.0f, (float) 0.0f));
    public static UnityEngine.Vector2Int ExcludeVectorXAxes      (UnityEngine.Vector2Int vector)                              => Util.ExcludeVectorAxes       (vector, new UnityEngine.Vector2Int((int)   1,    (int)   0));
    public static UnityEngine.Vector3    ExcludeVectorXAxes      (UnityEngine.Vector3    vector)                              => Util.ExcludeVectorAxes       (vector, new UnityEngine.Vector3   ((float) 1.0f, (float) 0.0f, (float) 0.0f));
    public static UnityEngine.Vector3Int ExcludeVectorXAxes      (UnityEngine.Vector3Int vector)                              => Util.ExcludeVectorAxes       (vector, new UnityEngine.Vector3Int((int)   1,    (int)   0,    (int)   0));
    public static UnityEngine.Vector4    ExcludeVectorXAxes      (UnityEngine.Vector4    vector)                              => Util.ExcludeVectorAxes       (vector, new UnityEngine.Vector4   ((float) 1.0f, (float) 0.0f, (float) 0.0f, (float) 0.0f));
    public static UnityEngine.Vector2    ExcludeVectorYAxes      (UnityEngine.Vector2    vector)                              => Util.ExcludeVectorAxes       (vector, new UnityEngine.Vector2   ((float) 0.0f, (float) 1.0f));
    public static UnityEngine.Vector2Int ExcludeVectorYAxes      (UnityEngine.Vector2Int vector)                              => Util.ExcludeVectorAxes       (vector, new UnityEngine.Vector2Int((int)   0,    (int)   1));
    public static UnityEngine.Vector3    ExcludeVectorYAxes      (UnityEngine.Vector3    vector)                              => Util.ExcludeVectorAxes       (vector, new UnityEngine.Vector3   ((float) 0.0f, (float) 1.0f, (float) 0.0f));
    public static UnityEngine.Vector3Int ExcludeVectorYAxes      (UnityEngine.Vector3Int vector)                              => Util.ExcludeVectorAxes       (vector, new UnityEngine.Vector3Int((int)   0,    (int)   1,    (int)   0));
    public static UnityEngine.Vector4    ExcludeVectorYAxes      (UnityEngine.Vector4    vector)                              => Util.ExcludeVectorAxes       (vector, new UnityEngine.Vector4   ((float) 0.0f, (float) 1.0f, (float) 0.0f, (float) 0.0f));
    public static UnityEngine.Vector3    ExcludeVectorZAxes      (UnityEngine.Vector3    vector)                              => Util.ExcludeVectorAxes       (vector, new UnityEngine.Vector3   ((float) 0.0f, (float) 0.0f, (float) 1.0f));
    public static UnityEngine.Vector3Int ExcludeVectorZAxes      (UnityEngine.Vector3Int vector)                              => Util.ExcludeVectorAxes       (vector, new UnityEngine.Vector3Int((int)   0,    (int)   0,    (int)   1));
    public static UnityEngine.Vector4    ExcludeVectorZAxes      (UnityEngine.Vector4    vector)                              => Util.ExcludeVectorAxes       (vector, new UnityEngine.Vector4   ((float) 0.0f, (float) 0.0f, (float) 1.0f, (float) 0.0f));
    public static UnityEngine.Vector4    ExcludeVectorWAxes      (UnityEngine.Vector4    vector)                              => Util.ExcludeVectorAxes       (vector, new UnityEngine.Vector4   ((float) 0.0f, (float) 0.0f, (float) 0.0f, (float) 1.0f));

    public static              string                                                                 GetAssetPath   () { return Util.NormalizeURI(UnityEngine.Application.streamingAssetsPath); }
    public static              string                                                                 GetDataPath    () { return Util.NormalizeURI(UnityEngine.Application.persistentDataPath); }
    public static ref readonly System.Collections.ObjectModel.ReadOnlyCollection<int>                 GetMouseButtons() { return ref Util.MOUSE_BUTTONS; }
    public static ref readonly System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.KeyCode> GetKeys        () { return ref Util.KEYS; }

    public static UnityEngine.Vector2    GetVectorAxes       (UnityEngine.Vector2    vector, UnityEngine.Vector2    axes) { return new(0.0f != axes.x ? vector.x : 0.0f, 0.0f != axes.y ? vector.y : 0.0f); }
    public static UnityEngine.Vector2    GetVectorAxes       (UnityEngine.Vector2    vector, UnityEngine.Vector2Int axes) => Util.GetVectorAxes(vector, new UnityEngine.Vector2((float) axes.x, (float) axes.y));
    public static UnityEngine.Vector2    GetVectorAxes       (UnityEngine.Vector2    vector, UnityEngine.Vector3    axes) => Util.GetVectorAxes(vector, new UnityEngine.Vector2((float) axes.x, (float) axes.y));
    public static UnityEngine.Vector2    GetVectorAxes       (UnityEngine.Vector2    vector, UnityEngine.Vector3Int axes) => Util.GetVectorAxes(vector, new UnityEngine.Vector2((float) axes.x, (float) axes.y));
    public static UnityEngine.Vector2    GetVectorAxes       (UnityEngine.Vector2    vector, UnityEngine.Vector4    axes) => Util.GetVectorAxes(vector, new UnityEngine.Vector2((float) axes.x, (float) axes.y));
    public static UnityEngine.Vector2Int GetVectorAxes       (UnityEngine.Vector2Int vector, UnityEngine.Vector2Int axes) { return new(0 != axes.x ? vector.x : 0, 0 != axes.y ? vector.y : 0); }
    public static UnityEngine.Vector2Int GetVectorAxes       (UnityEngine.Vector2Int vector, UnityEngine.Vector2    axes) => Util.GetVectorAxes(vector, new UnityEngine.Vector2Int((int) axes.x, (int) axes.y));
    public static UnityEngine.Vector2Int GetVectorAxes       (UnityEngine.Vector2Int vector, UnityEngine.Vector3    axes) => Util.GetVectorAxes(vector, new UnityEngine.Vector2Int((int) axes.x, (int) axes.y));
    public static UnityEngine.Vector2Int GetVectorAxes       (UnityEngine.Vector2Int vector, UnityEngine.Vector3Int axes) => Util.GetVectorAxes(vector, new UnityEngine.Vector2Int((int) axes.x, (int) axes.y));
    public static UnityEngine.Vector2Int GetVectorAxes       (UnityEngine.Vector2Int vector, UnityEngine.Vector4    axes) => Util.GetVectorAxes(vector, new UnityEngine.Vector2Int((int) axes.x, (int) axes.y));
    public static UnityEngine.Vector3    GetVectorAxes       (UnityEngine.Vector3    vector, UnityEngine.Vector3    axes) { return new(0.0f != axes.x ? vector.x : 0.0f, 0.0f != axes.y ? vector.y : 0.0f, 0.0f != axes.z ? vector.z : 0.0f); }
    public static UnityEngine.Vector3    GetVectorAxes       (UnityEngine.Vector3    vector, UnityEngine.Vector2    axes) => Util.GetVectorAxes(vector, new UnityEngine.Vector3((float) axes.x, (float) axes.y, (float) 0.0f));
    public static UnityEngine.Vector3    GetVectorAxes       (UnityEngine.Vector3    vector, UnityEngine.Vector2Int axes) => Util.GetVectorAxes(vector, new UnityEngine.Vector3((float) axes.x, (float) axes.y, (float) 0.0f));
    public static UnityEngine.Vector3    GetVectorAxes       (UnityEngine.Vector3    vector, UnityEngine.Vector3Int axes) => Util.GetVectorAxes(vector, new UnityEngine.Vector3((float) axes.x, (float) axes.y, (float) axes.z));
    public static UnityEngine.Vector3    GetVectorAxes       (UnityEngine.Vector3    vector, UnityEngine.Vector4    axes) => Util.GetVectorAxes(vector, new UnityEngine.Vector3((float) axes.x, (float) axes.y, (float) axes.z));
    public static UnityEngine.Vector3Int GetVectorAxes       (UnityEngine.Vector3Int vector, UnityEngine.Vector3Int axes) { return new(0 != axes.x ? vector.x : 0, 0 != axes.y ? vector.y : 0, 0 != axes.z ? vector.z : 0); }
    public static UnityEngine.Vector3Int GetVectorAxes       (UnityEngine.Vector3Int vector, UnityEngine.Vector2    axes) => Util.GetVectorAxes(vector, new UnityEngine.Vector3Int((int) axes.x, (int) axes.y, (int) 0));
    public static UnityEngine.Vector3Int GetVectorAxes       (UnityEngine.Vector3Int vector, UnityEngine.Vector2Int axes) => Util.GetVectorAxes(vector, new UnityEngine.Vector3Int((int) axes.x, (int) axes.y, (int) 0));
    public static UnityEngine.Vector3Int GetVectorAxes       (UnityEngine.Vector3Int vector, UnityEngine.Vector3    axes) => Util.GetVectorAxes(vector, new UnityEngine.Vector3Int((int) axes.x, (int) axes.y, (int) axes.z));
    public static UnityEngine.Vector3Int GetVectorAxes       (UnityEngine.Vector3Int vector, UnityEngine.Vector4    axes) => Util.GetVectorAxes(vector, new UnityEngine.Vector3Int((int) axes.x, (int) axes.y, (int) axes.z));
    public static UnityEngine.Vector4    GetVectorAxes       (UnityEngine.Vector4    vector, UnityEngine.Vector4    axes) { return new(0.0f != axes.x ? vector.x : 0.0f, 0.0f != axes.y ? vector.y : 0.0f, 0.0f != axes.z ? vector.z : 0.0f, 0.0f != axes.w ? vector.w : 0.0f); }
    public static UnityEngine.Vector4    GetVectorAxes       (UnityEngine.Vector4    vector, UnityEngine.Vector2    axes) => Util.GetVectorAxes(vector, new UnityEngine.Vector4((float) axes.x, (float) axes.y, (float) 0.0f,   (float) 0.0f));
    public static UnityEngine.Vector4    GetVectorAxes       (UnityEngine.Vector4    vector, UnityEngine.Vector2Int axes) => Util.GetVectorAxes(vector, new UnityEngine.Vector4((float) axes.x, (float) axes.y, (float) 0.0f,   (float) 0.0f));
    public static UnityEngine.Vector4    GetVectorAxes       (UnityEngine.Vector4    vector, UnityEngine.Vector3    axes) => Util.GetVectorAxes(vector, new UnityEngine.Vector4((float) axes.x, (float) axes.y, (float) axes.z, (float) 0.0f));
    public static UnityEngine.Vector4    GetVectorAxes       (UnityEngine.Vector4    vector, UnityEngine.Vector3Int axes) => Util.GetVectorAxes(vector, new UnityEngine.Vector4((float) axes.x, (float) axes.y, (float) axes.z, (float) 0.0f));
    public static float                  GetVectorAxis       (UnityEngine.Vector2    vector, UnityEngine.Vector2    axis) => (float) Util.GetVectorAxis(new UnityEngine.Vector4((float) vector.x, (float) vector.y, (float) 0.0f, (float) 0.0f),     new UnityEngine.Vector4   ((float) axis.x, (float) axis.y, (float) 0.0f, (float) 0.0f));
    public static float                  GetVectorAxis       (UnityEngine.Vector2    vector, UnityEngine.Vector2Int axis) => (float) Util.GetVectorAxis(vector,                                                                                      new UnityEngine.Vector2   ((float) axis.x, (float) axis.y));
    public static float                  GetVectorAxis       (UnityEngine.Vector2    vector, UnityEngine.Vector3    axis) => (float) Util.GetVectorAxis(vector,                                                                                      new UnityEngine.Vector2   ((float) axis.x, (float) axis.y));
    public static float                  GetVectorAxis       (UnityEngine.Vector2    vector, UnityEngine.Vector3Int axis) => (float) Util.GetVectorAxis(vector,                                                                                      new UnityEngine.Vector2   ((float) axis.x, (float) axis.y));
    public static float                  GetVectorAxis       (UnityEngine.Vector2    vector, UnityEngine.Vector4    axis) => (float) Util.GetVectorAxis(vector,                                                                                      new UnityEngine.Vector2   ((float) axis.x, (float) axis.y));
    public static int                    GetVectorAxis       (UnityEngine.Vector2Int vector, UnityEngine.Vector2Int axis) => (int)   Util.GetVectorAxis(new UnityEngine.Vector4((int) vector.x, (int) vector.y, (int) 0.0f, (int) 0.0f),             new UnityEngine.Vector4   ((int)   axis.x, (int)   axis.y, (int) 0.0f, (int) 0.0f));
    public static int                    GetVectorAxis       (UnityEngine.Vector2Int vector, UnityEngine.Vector2    axis) => (int)   Util.GetVectorAxis(vector,                                                                                      new UnityEngine.Vector2Int((int)   axis.x, (int)   axis.y));
    public static int                    GetVectorAxis       (UnityEngine.Vector2Int vector, UnityEngine.Vector3    axis) => (int)   Util.GetVectorAxis(vector,                                                                                      new UnityEngine.Vector2Int((int)   axis.x, (int)   axis.y));
    public static int                    GetVectorAxis       (UnityEngine.Vector2Int vector, UnityEngine.Vector3Int axis) => (int)   Util.GetVectorAxis(vector,                                                                                      new UnityEngine.Vector2Int((int)   axis.x, (int)   axis.y));
    public static int                    GetVectorAxis       (UnityEngine.Vector2Int vector, UnityEngine.Vector4    axis) => (int)   Util.GetVectorAxis(vector,                                                                                      new UnityEngine.Vector2Int((int)   axis.x, (int)   axis.y));
    public static float                  GetVectorAxis       (UnityEngine.Vector3    vector, UnityEngine.Vector3    axis) => (float) Util.GetVectorAxis(new UnityEngine.Vector4((float) vector.x, (float) vector.y, (float) vector.z, (float) 0.0f), new UnityEngine.Vector4   ((float) axis.x, (float) axis.y, (float) axis.z, (float) 0.0f));
    public static float                  GetVectorAxis       (UnityEngine.Vector3    vector, UnityEngine.Vector2    axis) => (float) Util.GetVectorAxis(vector,                                                                                      new UnityEngine.Vector3   ((float) axis.x, (float) axis.y, (float) 0.0f));
    public static float                  GetVectorAxis       (UnityEngine.Vector3    vector, UnityEngine.Vector2Int axis) => (float) Util.GetVectorAxis(vector,                                                                                      new UnityEngine.Vector3   ((float) axis.x, (float) axis.y, (float) 0.0f));
    public static float                  GetVectorAxis       (UnityEngine.Vector3    vector, UnityEngine.Vector3Int axis) => (float) Util.GetVectorAxis(vector,                                                                                      new UnityEngine.Vector3   ((float) axis.x, (float) axis.y, (float) axis.z));
    public static float                  GetVectorAxis       (UnityEngine.Vector3    vector, UnityEngine.Vector4    axis) => (float) Util.GetVectorAxis(vector,                                                                                      new UnityEngine.Vector3   ((float) axis.x, (float) axis.y, (float) axis.z));
    public static int                    GetVectorAxis       (UnityEngine.Vector3Int vector, UnityEngine.Vector3Int axis) => (int)   Util.GetVectorAxis(new UnityEngine.Vector4((int) vector.x, (int) vector.y, (int) vector.z, (int) 0.0f),         new UnityEngine.Vector4   ((int)   axis.x, (int)   axis.y, (int)   axis.z, (int) 0.0f));
    public static int                    GetVectorAxis       (UnityEngine.Vector3Int vector, UnityEngine.Vector2    axis) => (int)   Util.GetVectorAxis(vector,                                                                                      new UnityEngine.Vector3Int((int)   axis.x, (int)   axis.y, (int)   0.0f));
    public static int                    GetVectorAxis       (UnityEngine.Vector3Int vector, UnityEngine.Vector2Int axis) => (int)   Util.GetVectorAxis(vector,                                                                                      new UnityEngine.Vector3Int((int)   axis.x, (int)   axis.y, (int)   0.0f));
    public static int                    GetVectorAxis       (UnityEngine.Vector3Int vector, UnityEngine.Vector3    axis) => (int)   Util.GetVectorAxis(vector,                                                                                      new UnityEngine.Vector3Int((int)   axis.x, (int)   axis.y, (int)   axis.z));
    public static int                    GetVectorAxis       (UnityEngine.Vector3Int vector, UnityEngine.Vector4    axis) => (int)   Util.GetVectorAxis(vector,                                                                                      new UnityEngine.Vector3Int((int)   axis.x, (int)   axis.y, (int)   axis.z));
    public static float                  GetVectorAxis       (UnityEngine.Vector4    vector, UnityEngine.Vector2    axis) => (float) Util.GetVectorAxis(vector,                                                                                      new UnityEngine.Vector4   ((float) axis.x, (float) axis.y, (float) 0.0f,   (float) 0.0f));
    public static float                  GetVectorAxis       (UnityEngine.Vector4    vector, UnityEngine.Vector2Int axis) => (float) Util.GetVectorAxis(vector,                                                                                      new UnityEngine.Vector4   ((float) axis.x, (float) axis.y, (float) 0.0f,   (float) 0.0f));
    public static float                  GetVectorAxis       (UnityEngine.Vector4    vector, UnityEngine.Vector3    axis) => (float) Util.GetVectorAxis(vector,                                                                                      new UnityEngine.Vector4   ((float) axis.x, (float) axis.y, (float) axis.z, (float) 0.0f));
    public static float                  GetVectorAxis       (UnityEngine.Vector4    vector, UnityEngine.Vector3Int axis) => (float) Util.GetVectorAxis(vector,                                                                                      new UnityEngine.Vector4   ((float) axis.x, (float) axis.y, (float) axis.z, (float) 0.0f));
    public static float                  GetVectorAxis       (UnityEngine.Vector4    vector, UnityEngine.Vector4    axis) { return 0.0f != axis.x ? vector.x : 0.0f != axis.y ? vector.y : 0.0f != axis.z ? vector.z : 0.0f != axis.w ? vector.w : float.NaN; }
    public static UnityEngine.Vector3    GetVectorBackAxes   (UnityEngine.Vector3    vector)                              => Util.GetVectorAxes       (vector, UnityEngine.Vector3   .back);
    public static UnityEngine.Vector3Int GetVectorBackAxes   (UnityEngine.Vector3Int vector)                              => Util.GetVectorAxes       (vector, UnityEngine.Vector3Int.back);
    public static float                  GetVectorBackAxis   (UnityEngine.Vector3    vector)                              => Util.GetVectorAxis       (vector, UnityEngine.Vector3   .back);
    public static int                    GetVectorBackAxis   (UnityEngine.Vector3Int vector)                              => Util.GetVectorAxis       (vector, UnityEngine.Vector3Int.back);
    public static UnityEngine.Vector3    GetVectorDepthAxes  (UnityEngine.Vector3    vector)                              => Util.GetVectorForwardAxes(vector);
    public static UnityEngine.Vector3Int GetVectorDepthAxes  (UnityEngine.Vector3Int vector)                              => Util.GetVectorForwardAxes(vector);
    public static float                  GetVectorDepthAxis  (UnityEngine.Vector3    vector)                              => Util.GetVectorForwardAxis(vector);
    public static int                    GetVectorDepthAxis  (UnityEngine.Vector3Int vector)                              => Util.GetVectorForwardAxis(vector);
    public static UnityEngine.Vector2    GetVectorDownAxes   (UnityEngine.Vector2    vector)                              => Util.GetVectorAxes       (vector, UnityEngine.Vector2   .down);
    public static UnityEngine.Vector2Int GetVectorDownAxes   (UnityEngine.Vector2Int vector)                              => Util.GetVectorAxes       (vector, UnityEngine.Vector2Int.down);
    public static UnityEngine.Vector3    GetVectorDownAxes   (UnityEngine.Vector3    vector)                              => Util.GetVectorAxes       (vector, UnityEngine.Vector3   .down);
    public static UnityEngine.Vector3Int GetVectorDownAxes   (UnityEngine.Vector3Int vector)                              => Util.GetVectorAxes       (vector, UnityEngine.Vector3Int.down);
    public static float                  GetVectorDownAxis   (UnityEngine.Vector2    vector)                              => Util.GetVectorAxis       (vector, UnityEngine.Vector2   .down);
    public static int                    GetVectorDownAxis   (UnityEngine.Vector2Int vector)                              => Util.GetVectorAxis       (vector, UnityEngine.Vector2Int.down);
    public static float                  GetVectorDownAxis   (UnityEngine.Vector3    vector)                              => Util.GetVectorAxis       (vector, UnityEngine.Vector3   .down);
    public static int                    GetVectorDownAxis   (UnityEngine.Vector3Int vector)                              => Util.GetVectorAxis       (vector, UnityEngine.Vector3Int.down);
    public static UnityEngine.Vector3    GetVectorForwardAxes(UnityEngine.Vector3    vector)                              => Util.GetVectorAxes       (vector, UnityEngine.Vector3   .forward);
    public static UnityEngine.Vector3Int GetVectorForwardAxes(UnityEngine.Vector3Int vector)                              => Util.GetVectorAxes       (vector, UnityEngine.Vector3Int.forward);
    public static float                  GetVectorForwardAxis(UnityEngine.Vector3    vector)                              => Util.GetVectorAxis       (vector, UnityEngine.Vector3   .forward);
    public static int                    GetVectorForwardAxis(UnityEngine.Vector3Int vector)                              => Util.GetVectorAxis       (vector, UnityEngine.Vector3Int.forward);
    public static UnityEngine.Vector2    GetVectorHeightAxes (UnityEngine.Vector2    vector)                              => Util.GetVectorUpAxes     (vector);
    public static UnityEngine.Vector2Int GetVectorHeightAxes (UnityEngine.Vector2Int vector)                              => Util.GetVectorUpAxes     (vector);
    public static UnityEngine.Vector3    GetVectorHeightAxes (UnityEngine.Vector3    vector)                              => Util.GetVectorUpAxes     (vector);
    public static UnityEngine.Vector3Int GetVectorHeightAxes (UnityEngine.Vector3Int vector)                              => Util.GetVectorUpAxes     (vector);
    public static float                  GetVectorHeightAxis (UnityEngine.Vector2    vector)                              => Util.GetVectorUpAxis     (vector);
    public static int                    GetVectorHeightAxis (UnityEngine.Vector2Int vector)                              => Util.GetVectorUpAxis     (vector);
    public static float                  GetVectorHeightAxis (UnityEngine.Vector3    vector)                              => Util.GetVectorUpAxis     (vector);
    public static int                    GetVectorHeightAxis (UnityEngine.Vector3Int vector)                              => Util.GetVectorUpAxis     (vector);
    public static UnityEngine.Vector2    GetVectorLeftAxes   (UnityEngine.Vector2    vector)                              => Util.GetVectorAxes       (vector, UnityEngine.Vector2   .left);
    public static UnityEngine.Vector2Int GetVectorLeftAxes   (UnityEngine.Vector2Int vector)                              => Util.GetVectorAxes       (vector, UnityEngine.Vector2Int.left);
    public static UnityEngine.Vector3    GetVectorLeftAxes   (UnityEngine.Vector3    vector)                              => Util.GetVectorAxes       (vector, UnityEngine.Vector3   .left);
    public static UnityEngine.Vector3Int GetVectorLeftAxes   (UnityEngine.Vector3Int vector)                              => Util.GetVectorAxes       (vector, UnityEngine.Vector3Int.left);
    public static float                  GetVectorLeftAxis   (UnityEngine.Vector2    vector)                              => Util.GetVectorAxis       (vector, UnityEngine.Vector2   .left);
    public static int                    GetVectorLeftAxis   (UnityEngine.Vector2Int vector)                              => Util.GetVectorAxis       (vector, UnityEngine.Vector2Int.left);
    public static float                  GetVectorLeftAxis   (UnityEngine.Vector3    vector)                              => Util.GetVectorAxis       (vector, UnityEngine.Vector3   .left);
    public static int                    GetVectorLeftAxis   (UnityEngine.Vector3Int vector)                              => Util.GetVectorAxis       (vector, UnityEngine.Vector3Int.left);
    public static UnityEngine.Vector2    GetVectorRightAxes  (UnityEngine.Vector2    vector)                              => Util.GetVectorAxes       (vector, UnityEngine.Vector2   .right);
    public static UnityEngine.Vector2Int GetVectorRightAxes  (UnityEngine.Vector2Int vector)                              => Util.GetVectorAxes       (vector, UnityEngine.Vector2Int.right);
    public static UnityEngine.Vector3    GetVectorRightAxes  (UnityEngine.Vector3    vector)                              => Util.GetVectorAxes       (vector, UnityEngine.Vector3   .right);
    public static UnityEngine.Vector3Int GetVectorRightAxes  (UnityEngine.Vector3Int vector)                              => Util.GetVectorAxes       (vector, UnityEngine.Vector3Int.right);
    public static float                  GetVectorRightAxis  (UnityEngine.Vector2    vector)                              => Util.GetVectorAxis       (vector, UnityEngine.Vector2   .right);
    public static int                    GetVectorRightAxis  (UnityEngine.Vector2Int vector)                              => Util.GetVectorAxis       (vector, UnityEngine.Vector2Int.right);
    public static float                  GetVectorRightAxis  (UnityEngine.Vector3    vector)                              => Util.GetVectorAxis       (vector, UnityEngine.Vector3   .right);
    public static int                    GetVectorRightAxis  (UnityEngine.Vector3Int vector)                              => Util.GetVectorAxis       (vector, UnityEngine.Vector3Int.right);
    public static UnityEngine.Vector2    GetVectorUpAxes     (UnityEngine.Vector2    vector)                              => Util.GetVectorAxes       (vector, UnityEngine.Vector2   .up);
    public static UnityEngine.Vector2Int GetVectorUpAxes     (UnityEngine.Vector2Int vector)                              => Util.GetVectorAxes       (vector, UnityEngine.Vector2Int.up);
    public static UnityEngine.Vector3    GetVectorUpAxes     (UnityEngine.Vector3    vector)                              => Util.GetVectorAxes       (vector, UnityEngine.Vector3   .up);
    public static UnityEngine.Vector3Int GetVectorUpAxes     (UnityEngine.Vector3Int vector)                              => Util.GetVectorAxes       (vector, UnityEngine.Vector3Int.up);
    public static float                  GetVectorUpAxis     (UnityEngine.Vector2    vector)                              => Util.GetVectorAxis       (vector, UnityEngine.Vector2   .up);
    public static int                    GetVectorUpAxis     (UnityEngine.Vector2Int vector)                              => Util.GetVectorAxis       (vector, UnityEngine.Vector2Int.up);
    public static float                  GetVectorUpAxis     (UnityEngine.Vector3    vector)                              => Util.GetVectorAxis       (vector, UnityEngine.Vector3   .up);
    public static int                    GetVectorUpAxis     (UnityEngine.Vector3Int vector)                              => Util.GetVectorAxis       (vector, UnityEngine.Vector3Int.up);
    public static UnityEngine.Vector2    GetVectorWidthAxes  (UnityEngine.Vector2    vector)                              => Util.GetVectorRightAxes  (vector);
    public static UnityEngine.Vector2Int GetVectorWidthAxes  (UnityEngine.Vector2Int vector)                              => Util.GetVectorRightAxes  (vector);
    public static UnityEngine.Vector3    GetVectorWidthAxes  (UnityEngine.Vector3    vector)                              => Util.GetVectorRightAxes  (vector);
    public static UnityEngine.Vector3Int GetVectorWidthAxes  (UnityEngine.Vector3Int vector)                              => Util.GetVectorRightAxes  (vector);
    public static float                  GetVectorWidthAxis  (UnityEngine.Vector2    vector)                              => Util.GetVectorRightAxis  (vector);
    public static int                    GetVectorWidthAxis  (UnityEngine.Vector2Int vector)                              => Util.GetVectorRightAxis  (vector);
    public static float                  GetVectorWidthAxis  (UnityEngine.Vector3    vector)                              => Util.GetVectorRightAxis  (vector);
    public static int                    GetVectorWidthAxis  (UnityEngine.Vector3Int vector)                              => Util.GetVectorRightAxis  (vector);
    public static UnityEngine.Vector2    GetVectorXAxes      (UnityEngine.Vector2    vector)                              => Util.GetVectorAxes       (vector, new UnityEngine.Vector2   ((float) 1.0f, (float) 0.0f));
    public static UnityEngine.Vector2Int GetVectorXAxes      (UnityEngine.Vector2Int vector)                              => Util.GetVectorAxes       (vector, new UnityEngine.Vector2Int((int)   1,    (int)   0));
    public static UnityEngine.Vector3    GetVectorXAxes      (UnityEngine.Vector3    vector)                              => Util.GetVectorAxes       (vector, new UnityEngine.Vector3   ((float) 1.0f, (float) 0.0f, (float) 0.0f));
    public static UnityEngine.Vector3Int GetVectorXAxes      (UnityEngine.Vector3Int vector)                              => Util.GetVectorAxes       (vector, new UnityEngine.Vector3Int((int)   1,    (int)   0,    (int)   0));
    public static UnityEngine.Vector4    GetVectorXAxes      (UnityEngine.Vector4    vector)                              => Util.GetVectorAxes       (vector, new UnityEngine.Vector4   ((float) 1.0f, (float) 0.0f, (float) 0.0f, (float) 0.0f));
    public static UnityEngine.Vector2    GetVectorYAxes      (UnityEngine.Vector2    vector)                              => Util.GetVectorAxes       (vector, new UnityEngine.Vector2   ((float) 0.0f, (float) 1.0f));
    public static UnityEngine.Vector2Int GetVectorYAxes      (UnityEngine.Vector2Int vector)                              => Util.GetVectorAxes       (vector, new UnityEngine.Vector2Int((int)   0,    (int)   1));
    public static UnityEngine.Vector3    GetVectorYAxes      (UnityEngine.Vector3    vector)                              => Util.GetVectorAxes       (vector, new UnityEngine.Vector3   ((float) 0.0f, (float) 1.0f, (float) 0.0f));
    public static UnityEngine.Vector3Int GetVectorYAxes      (UnityEngine.Vector3Int vector)                              => Util.GetVectorAxes       (vector, new UnityEngine.Vector3Int((int)   0,    (int)   1,    (int)   0));
    public static UnityEngine.Vector4    GetVectorYAxes      (UnityEngine.Vector4    vector)                              => Util.GetVectorAxes       (vector, new UnityEngine.Vector4   ((float) 0.0f, (float) 1.0f, (float) 0.0f, (float) 0.0f));
    public static UnityEngine.Vector3    GetVectorZAxes      (UnityEngine.Vector3    vector)                              => Util.GetVectorAxes       (vector, new UnityEngine.Vector3   ((float) 0.0f, (float) 0.0f, (float) 1.0f));
    public static UnityEngine.Vector3Int GetVectorZAxes      (UnityEngine.Vector3Int vector)                              => Util.GetVectorAxes       (vector, new UnityEngine.Vector3Int((int)   0,    (int)   0,    (int)   1));
    public static UnityEngine.Vector4    GetVectorZAxes      (UnityEngine.Vector4    vector)                              => Util.GetVectorAxes       (vector, new UnityEngine.Vector4   ((float) 0.0f, (float) 0.0f, (float) 1.0f, (float) 0.0f));
    public static UnityEngine.Vector4    GetVectorWAxes      (UnityEngine.Vector4    vector)                              => Util.GetVectorAxes       (vector, new UnityEngine.Vector4   ((float) 0.0f, (float) 0.0f, (float) 0.0f, (float) 1.0f));

    public static System.Func<T, T> IIFE<T>(System.Action<T>    function) { return _ => { function(_); return _; }; }
    public static System.Func<T, T> IIFE<T>(System.Func  <T, T> function) { return function; } // ⟶ Immediately-Invoked Function Expression

    public static bool IsConvertibleType(System.Type typeA, System.Type typeB) {
      if (typeA is null) return typeB is null;
      if (typeB is null) return typeA is null;

      for (System.Collections.Generic.Queue<System.Type> pending = new(1) {typeB}; 0 != pending.Count; ) {
        System.Type type = pending.Dequeue();

        // …
        if (type == typeA || type.IsAssignableFrom(typeA) || new[] {type, typeA}.Exists(_ => _.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Exists(method => {
          System.Reflection.ParameterInfo[] parameters = method.GetParameters();
          return method.Name == "op_Implicit" && 0 != parameters.Length
            && (method       .ReturnType    == type  || Util.IsImplicitConvertibleType(typeA, type))
            && (parameters[0].ParameterType == typeA || Util.IsImplicitConvertibleType(typeA, parameters[0].ParameterType));
        }))) return true;

        foreach (System.Type? subtype in Util.ArrayFrom(
          new[] {(type.IsEnum ? System.Enum    .GetUnderlyingType : (System.Func<System.Type, System.Type>) null!)?.Invoke(type)},
          new[] {(true        ? System.Nullable.GetUnderlyingType : (System.Func<System.Type, System.Type>) null!)?.Invoke(type)},
          IMPLICIT_TYPE_CONVERTS.TryGetValue(typeA, out System.Collections.ObjectModel.ReadOnlyCollection<System.Type> types) && types.Contains(typeB) ? types : new System.Collections.ObjectModel.ReadOnlyCollection<System.Type>(new System.Collections.Generic.List<System.Type>())
        )) {
          if (subtype is not null && !pending.Contains(subtype))
          pending.Enqueue(subtype);
        }
      }

      return false;
    }

    private static bool IsImplicitConvertibleType(System.Type typeA, System.Type typeB) {
      return IMPLICIT_TYPE_CONVERTS.TryGetValue(typeA, out System.Collections.ObjectModel.ReadOnlyCollection<System.Type> types) && types.Contains(typeB);
    }

    // private static object? LoadURI(
    //   string id, string path, System.Action<object?> callback, double? loadDurationMaximum, bool nocache,
    //   System.Func<object, object>                                  preparser, // ⟶ `Util.LoadURI(…)` cache hit on `LOADS` for desired URI data
    //   System.Func<string, UnityEngine.Networking.UnityWebRequest>  requester, // ⟶ Map `path` URI to preempted `UnityEngine.Networking.UnityWebRequest`
    //   System.Func<UnityEngine.Networking.UnityWebRequest, object?> parser     // ⟶ Map `UnityEngine.Networking.UnityWebRequest` result to desired URI data
    // ) {
    //   Util.LoadInfo                                        load = new(data: null, handlers: new[] {callback}, pending: false);
    //   UnityEngine.Networking.UnityWebRequestAsyncOperation operation;
    //   UnityEngine.Networking.UnityWebRequest               request; // ⟶ Able to access the `UnityEngine.Application.streamingAssetsPath` directory
    //   System.Diagnostics.Stopwatch                         stopwatch = new();

    //   // …
    //   static object? HandleURIData(ref Util.LoadInfo load, object? data) {
    //     while (0 != load.handlers.Count)
    //     load.handlers.Pop()(data);

    //     return data;
    //   }

    //   object? HandleURIRequest(ref Util.LoadInfo load, ref UnityEngine.Networking.UnityWebRequest request) {
    //     object? data = parser(request);

    //     // …
    //     request.Dispose();
    //     load.data = data is not null && !nocache ? data : load.data;

    //     return HandleURIData(ref load, data);
    //   }

    //   // …
    //   if (path is null)
    //   return null;

    //   if (LOADS.ContainsKey(id + path)) {
    //     load = LOADS[id + path];
    //     load.handlers.Push(callback);

    //     if (load.data is not null && !nocache)
    //     return HandleURIData(ref load, preparser(load.data));
    //   } else LOADS.Add(id + path, load);

    //   if (load.pending && loadDurationMaximum is not null)
    //   return null; // ⟶ Prevent spamming multiple `UnityEngine.Networking.UnityWebRequest`s

    //   stopwatch.Start();
    //   request      = requester(path);
    //   operation    = request.SendWebRequest();
    //   load.pending = true;

    //   while (UnityEngine.Networking.UnityWebRequest.Result.Success != request.result)
    //   switch (request.result) {
    //     case UnityEngine.Networking.UnityWebRequest.Result.ConnectionError    :
    //     case UnityEngine.Networking.UnityWebRequest.Result.DataProcessingError:
    //     case UnityEngine.Networking.UnityWebRequest.Result.ProtocolError      : {
    //       stopwatch.Stop   ();
    //       request  .Dispose();

    //       load.pending = false;
    //     } return null;

    //     case UnityEngine.Networking.UnityWebRequest.Result.InProgress: {
    //       if (stopwatch.Elapsed.TotalSeconds < (loadDurationMaximum ?? double.PositiveInfinity))
    //       continue;

    //       stopwatch.Stop();
    //       operation.completed += operation => {
    //         UnityEngine.Networking.UnityWebRequest request = ((UnityEngine.Networking.UnityWebRequestAsyncOperation) operation).webRequest!;

    //         // …
    //         load.pending = false;

    //         if (UnityEngine.Networking.UnityWebRequest.Result.Success != request.result) request.Dispose();
    //         else                                                                         HandleURIRequest(ref load, ref request);
    //       };
    //     } return null;
    //   }

    //   stopwatch.Stop();
    //   load.pending = false;

    //   return HandleURIRequest(ref load, ref request);
    // }

    // public static byte[]? LoadURI(string path, System.Action<byte[]?>? callback = null, double? loadDurationMaximum = null, bool nocache = Util.LoadCached) {
    //   return Util.LoadURI(
    //     typeof(byte[]).ToString(), path, callback is null ? static _ => {} : _ => callback(_ as byte[]), loadDurationMaximum, nocache,
    //     static predata => predata, // ⟶ `(predata as byte[]).Clone() as byte[]`
    //     static path    => UnityEngine.Networking.UnityWebRequest.Get(path),
    //     static request => request.downloadHandler.data
    //   ) as byte[];
    // }

    // public static UnityEngine.AudioClip? LoadURIAsAudioClip(string path, System.Action<UnityEngine.AudioClip?>? callback = null, double? loadDurationMaximum = null, bool nocache = Util.LoadCached, UnityEngine.AudioType? encoding = null) {
    //   return Util.LoadURI(
    //     typeof(UnityEngine.AudioClip).ToString(), path, callback is null ? static _ => {} : _ => callback(_ as UnityEngine.AudioClip), loadDurationMaximum, nocache,
    //     static predata => {
    //       #if false // ⟶ Consume less memory resources, please T_T
    //         UnityEngine.AudioClip                preaudioClip     = predata as UnityEngine.AudioClip;
    //         Unity.Collections.NativeArray<float> preaudioClipData = new(preaudioClip.channels * preaudioClip.samples, Unity.Collections.Allocator.Temp, Unity.Collections.NativeArrayOptions.UninitializedMemory);
    //         UnityEngine.AudioClip                audioClip        = UnityEngine.AudioClip.Create("🎵 " + System.IO.Path.GetFileName(path), preaudioClip.samples, preaudioClip.channels, preaudioClip.frequency, false);

    //         // …
    //         if (!preaudioClip.GetData(preaudioClipData, 0)) return preaudioClip;
    //         if (!audioClip   .SetData(preaudioClipData, 0)) return preaudioClip;

    //         return audioClip;
    //       #endif
    //       return predata;
    //     },
    //     path           => UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip(path, encoding ?? UnityEngine.AudioType.MPEG),
    //     static request => {
    //       UnityEngine.AudioClip? audioClip = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(request);

    //       // …
    //       if (audioClip is not null)
    //       audioClip.name = request.url is not null ? "🎵 " + System.IO.Path.GetFileName(request.url) : "🎵";

    //       return audioClip;
    //     }
    //   ) as UnityEngine.AudioClip;
    // }

    // public static string? LoadURIAsText(string path, System.Action<string?>? callback = null, double? loadDurationMaximum = null, bool nocache = Util.LoadCached, System.Text.Encoding? encoding = null) {
    //   return Util.LoadURI(
    //     typeof(string).ToString(), path, callback is null ? static _ => {} : _ => callback(_ as string), loadDurationMaximum, nocache,
    //     static predata => predata, // ⟶ `new string((predata as string).ToCharArray())`
    //     static path    => UnityEngine.Networking.UnityWebRequest.Get(path),
    //     request        => {
    //       string? text = encoding is not null ? null : request.downloadHandler.text; // ⟶ UTF-8

    //       // …
    //       try { text ??= encoding!.GetString(request.downloadHandler.data); }
    //       catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.ArgumentNullException || exception is System.Text.DecoderFallbackException) {}

    //       return text;
    //     }
    //   ) as string;
    // }

    // public static UnityEngine.Texture2D? LoadURIAsTexture2D(string path, System.Action<UnityEngine.Texture2D?>? callback = null, double? loadDurationMaximum = null, bool nocache = Util.LoadCached) {
    //   return Util.LoadURI(
    //     typeof(UnityEngine.Texture2D).ToString(), path, callback is null ? static _ => {} : _ => callback(_ as UnityEngine.Texture2D), loadDurationMaximum, nocache,
    //     static predata => {
    //       #if false // ⟶ Consume less memory resources, please T_T
    //         UnityEngine.Texture2D pretexture = predata as UnityEngine.Texture2D;
    //         UnityEngine.Texture2D texture    = new(pretexture.width, pretexture.height, pretexture.format, pretexture.mipmapCount, false);

    //         UnityEngine.Graphics.CopyTexture(pretexture, texture);
    //         return texture;
    //       #endif
    //       return predata;
    //     },
    //     static path => UnityEngine.Networking.UnityWebRequest.Get(path),
    //     request     => {
    //       UnityEngine.Texture2D texture = new(2, 2, UnityEngine.TextureFormat.RGBA32, -1, false);

    //       texture.name = "🖼️ " + System.IO.Path.GetFileName(path);
    //       return UnityEngine.ImageConversion.LoadImage(texture, request.downloadHandler.data, true) ? texture : null;
    //     }
    //   ) as UnityEngine.Texture2D;
    // }

    public static UnityEngine.Bounds? LocalBoundsFromRectTransform(UnityEngine.RectTransform? transform) {
      UnityEngine.Rect? rectangle = Util.LocalRectFromRectTransform(transform);
      return rectangle is null ? null : new(transform!.position, new(rectangle?.width ?? 0.0f, rectangle?.height ?? 0.0f, 0.0f));
    }

    public static UnityEngine.Vector3[]? LocalCornersFromRectTransform(UnityEngine.RectTransform? transform) {
      return Util.CornersFromRectTransform(transform is null ? null : transform.GetLocalCorners);
    }

    public static UnityEngine.Rect? LocalRectFromRectTransform(UnityEngine.RectTransform? transform) {
      UnityEngine.Vector3[]? corners = Util.LocalCornersFromRectTransform(transform);
      return corners is null ? null : Util.RectFromCorners(corners);
    }

    public static void Loop(int begin, int end, System.Delegate callback) {
      object[]                          arguments  = new object[] {begin, System.Math.Abs(end - begin), begin, end};
      int                               direction  = System.Math.Sign(end - begin);
      System.Reflection.ParameterInfo[] parameters = callback.Method.GetParameters();

      // …
      try { System.Array.Resize(ref arguments, parameters.Length); }
      catch (System.Exception) { throw new System.NotSupportedException("Cannot `Loop(…)` given specified callback; Too many parameters"); }

      for (int index = 1; index < parameters.Length; ++index)
      arguments[index] = Util.DelegateConvert(arguments[index].GetType(), parameters[index].ParameterType).DynamicInvoke(arguments[index]);

      for (int index = begin; ; index += direction) {
        if (parameters.Length > 0) arguments[0] = Util.DelegateConvert(typeof(int), parameters[0].ParameterType).DynamicInvoke(index);

        callback.DynamicInvoke(arguments);
        if (end == index) return;
      }
    }

    public static void Loop<T>(System.Collections.Generic.IEnumerable<T> enumerable, System.Delegate callback) {
      object[]                          arguments  = new object[] {null!, 0};
      int                               index      = 0;
      System.Reflection.ParameterInfo[] parameters = callback.Method.GetParameters();

      // …
      try { System.Array.Resize(ref arguments, parameters.Length); }
      catch (System.Exception) { throw new System.NotSupportedException("Cannot `Loop(…)` given specified callback; Too many parameters"); }

      foreach (T value in enumerable) {
        if (parameters.Length > 0) arguments[0] = value!;
        if (parameters.Length > 1) arguments[1] = Util.DelegateConvert(typeof(int), parameters[1].ParameterType).DynamicInvoke(index++);

        callback.DynamicInvoke(arguments);
      }
    }

    public static void Loop   (int count,                                            System.Delegate                   callback) { if (0 != count) Util.Loop(System.Math.Sign(count), count, callback); }
    public static void Loop   (int count,                                            System.Action<int>                callback) => Util.Loop(count,      (System.Delegate) callback);
    public static void Loop   (int count,                                            System.Action<int, int>           callback) => Util.Loop(count,      (System.Delegate) callback);
    public static void Loop   (int count,                                            System.Action<int, int, int, int> callback) => Util.Loop(count,      (System.Delegate) callback);
    public static void Loop   (int begin, int end,                                   System.Action<int>                callback) => Util.Loop(begin, end, (System.Delegate) callback);
    public static void Loop   (int begin, int end,                                   System.Action<int, int>           callback) => Util.Loop(begin, end, (System.Delegate) callback);
    public static void Loop   (int begin, int end,                                   System.Action<int, int, int, int> callback) => Util.Loop(begin, end, (System.Delegate) callback);
    public static void Loop<T>(System.Collections.Generic.IEnumerable<T> enumerable, System.Action<T>                  callback) => Util.Loop(enumerable, (System.Delegate) callback);
    public static void Loop<T>(System.Collections.Generic.IEnumerable<T> enumerable, System.Action<T, int>             callback) => Util.Loop(enumerable, (System.Delegate) callback);

    public static T Max<T>(System.Collections.Generic.IEnumerable<T> enumerable) where T : System.IComparable<T> {
      T[] maximum = null!;

      // …
      foreach (T value in enumerable) {
        if (maximum is null)                 maximum    = new[] {value};
        if (value.CompareTo(maximum[0]) > 0) maximum[0] = value;
      }

      return maximum![0]; // ⟶ `System.IndexOutOfRangeException`
    }
      public static T Max<T>(T valueA, T valueB) where T : System.IComparable<T> => valueA.CompareTo(valueB) > 0 ? valueA : valueB;
      public static T Max<T>(params T[] values)                                  => Util.Max(values);

    public static T Min<T>(System.Collections.Generic.IEnumerable<T> enumerable) where T : System.IComparable<T> {
      T[] minimum = null!;

      // …
      foreach (T value in enumerable) {
        if (minimum is null)                 minimum    = new[] {value};
        if (value.CompareTo(minimum[0]) < 0) minimum[0] = value;
      }

      return minimum![0]; // ⟶ `System.IndexOutOfRangeException`
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

    public static decimal Percent(decimal percentage) { return percentage / 100.0m; }
    public static double  Percent(double  percentage) { return percentage / 100.0; }
    public static float   Percent(float   percentage) { return percentage / 100.0f; }

    public static decimal PercentOf(decimal value, decimal percentage) { return System.Math.Min(value, (decimal) percentage) * (System.Math.Max(value, (decimal) percentage) / 100.0m); }
    public static decimal PercentOf(decimal value, double  percentage) { return System.Math.Min(value, (decimal) percentage) * (System.Math.Max(value, (decimal) percentage) / 100.0m); }
    public static decimal PercentOf(decimal value, float   percentage) { return System.Math.Min(value, (decimal) percentage) * (System.Math.Max(value, (decimal) percentage) / 100.0m); }
    public static double  PercentOf(double  value, decimal percentage) { return System.Math.Min(value, (double)  percentage) * (System.Math.Max(value, (double)  percentage) / 100.0); }
    public static double  PercentOf(double  value, double  percentage) { return System.Math.Min(value, (double)  percentage) * (System.Math.Max(value, (double)  percentage) / 100.0); }
    public static double  PercentOf(double  value, float   percentage) { return System.Math.Min(value, (double)  percentage) * (System.Math.Max(value, (double)  percentage) / 100.0); }
    public static float   PercentOf(float   value, decimal percentage) { return System.Math.Min(value, (float)   percentage) * (System.Math.Max(value, (float)   percentage) / 100.0f); }
    public static float   PercentOf(float   value, double  percentage) { return System.Math.Min(value, (float)   percentage) * (System.Math.Max(value, (float)   percentage) / 100.0f); }
    public static float   PercentOf(float   value, float   percentage) { return System.Math.Min(value, (float)   percentage) * (System.Math.Max(value, (float)   percentage) / 100.0f); }

    // public static void PreloadURI           (string path)                                         => Util.LoadURI           (path, null, Util.LoadAsynchronously, Util.LoadCached);
    // public static void PreloadURIAsAudioClip(string path, UnityEngine.AudioType? encoding = null) => Util.LoadURIAsAudioClip(path, null, Util.LoadAsynchronously, Util.LoadCached, encoding);
    // public static void PreloadURIAsText     (string path, System.Text.Encoding?  encoding = null) => Util.LoadURIAsText     (path, null, Util.LoadAsynchronously, Util.LoadCached, encoding);
    // public static void PreloadURIAsTexture2D(string path)                                         => Util.LoadURIAsTexture2D(path, null, Util.LoadAsynchronously, Util.LoadCached);

    public static UnityEngine.Rect RectFromCorners(UnityEngine.Vector3[] corners) {
      // ⟶ Origin begins from bottom-left rather than top-left
      return new(corners[0].x, corners[0].y, corners[3].x - corners[0].x, corners[1].y - corners[0].y);
    }

    public static UnityEngine.Vector2 SetVectorAxis(UnityEngine.Vector2 vector, UnityEngine.Vector2Int axis, float value) => Util.SetVectorAxis(vector, new UnityEngine.Vector2((float) axis.x, (float) axis.y), value);
    public static UnityEngine.Vector2 SetVectorAxis(UnityEngine.Vector2 vector, UnityEngine.Vector3    axis, float value) => Util.SetVectorAxis(vector, new UnityEngine.Vector2((float) axis.x, (float) axis.y), value);
    public static UnityEngine.Vector2 SetVectorAxis(UnityEngine.Vector2 vector, UnityEngine.Vector3Int axis, float value) => Util.SetVectorAxis(vector, new UnityEngine.Vector2((float) axis.x, (float) axis.y), value);
    public static UnityEngine.Vector2 SetVectorAxis(UnityEngine.Vector2 vector, UnityEngine.Vector4    axis, float value) => Util.SetVectorAxis(vector, new UnityEngine.Vector2((float) axis.x, (float) axis.y), value);
    public static UnityEngine.Vector2 SetVectorAxis(UnityEngine.Vector2 vector, UnityEngine.Vector2    axis, float value) {
      vector.x = 0.0f != axis.x ? value : vector.x;
      vector.y = 0.0f != axis.y ? value : vector.y;

      return vector;
    }

    public static UnityEngine.Vector2Int SetVectorAxis(UnityEngine.Vector2Int vector, UnityEngine.Vector2    axis, int value) => Util.SetVectorAxis(vector, new UnityEngine.Vector2Int((int) axis.x, (int) axis.y), value);
    public static UnityEngine.Vector2Int SetVectorAxis(UnityEngine.Vector2Int vector, UnityEngine.Vector3    axis, int value) => Util.SetVectorAxis(vector, new UnityEngine.Vector2Int((int) axis.x, (int) axis.y), value);
    public static UnityEngine.Vector2Int SetVectorAxis(UnityEngine.Vector2Int vector, UnityEngine.Vector3Int axis, int value) => Util.SetVectorAxis(vector, new UnityEngine.Vector2Int((int) axis.x, (int) axis.y), value);
    public static UnityEngine.Vector2Int SetVectorAxis(UnityEngine.Vector2Int vector, UnityEngine.Vector4    axis, int value) => Util.SetVectorAxis(vector, new UnityEngine.Vector2Int((int) axis.x, (int) axis.y), value);
    public static UnityEngine.Vector2Int SetVectorAxis(UnityEngine.Vector2Int vector, UnityEngine.Vector2Int axis, int value) {
      vector.x = 0 != axis.x ? value : vector.x;
      vector.y = 0 != axis.y ? value : vector.y;

      return vector;
    }

    public static UnityEngine.Vector3 SetVectorAxis(UnityEngine.Vector3 vector, UnityEngine.Vector2    axis, float value) => Util.SetVectorAxis(vector, new UnityEngine.Vector3   ((float) axis.x, (float) axis.y, (float) 0.0f),   value);
    public static UnityEngine.Vector3 SetVectorAxis(UnityEngine.Vector3 vector, UnityEngine.Vector2Int axis, float value) => Util.SetVectorAxis(vector, new UnityEngine.Vector3   ((float) axis.x, (float) axis.y, (float) 0.0f),   value);
    public static UnityEngine.Vector3 SetVectorAxis(UnityEngine.Vector3 vector, UnityEngine.Vector3Int axis, float value) => Util.SetVectorAxis(vector, new UnityEngine.Vector3   ((float) axis.x, (float) axis.y, (float) axis.z), value);
    public static UnityEngine.Vector3 SetVectorAxis(UnityEngine.Vector3 vector, UnityEngine.Vector4    axis, float value) => Util.SetVectorAxis(vector, new UnityEngine.Vector3   ((float) axis.x, (float) axis.y, (float) axis.z), value);
    public static UnityEngine.Vector3 SetVectorAxis(UnityEngine.Vector3 vector, UnityEngine.Vector3    axis, float value) {
      vector.x = 0.0f != axis.x ? value : vector.x;
      vector.y = 0.0f != axis.y ? value : vector.y;
      vector.z = 0.0f != axis.z ? value : vector.z;

      return vector;
    }

    public static UnityEngine.Vector3Int SetVectorAxis(UnityEngine.Vector3Int vector, UnityEngine.Vector2    axis, int value) => Util.SetVectorAxis(vector, new UnityEngine.Vector3Int((int) axis.x, (int) axis.y, (int) 0.0f),   value);
    public static UnityEngine.Vector3Int SetVectorAxis(UnityEngine.Vector3Int vector, UnityEngine.Vector2Int axis, int value) => Util.SetVectorAxis(vector, new UnityEngine.Vector3Int((int) axis.x, (int) axis.y, (int) 0.0f),   value);
    public static UnityEngine.Vector3Int SetVectorAxis(UnityEngine.Vector3Int vector, UnityEngine.Vector3    axis, int value) => Util.SetVectorAxis(vector, new UnityEngine.Vector3Int((int) axis.x, (int) axis.y, (int) axis.z), value);
    public static UnityEngine.Vector3Int SetVectorAxis(UnityEngine.Vector3Int vector, UnityEngine.Vector4    axis, int value) => Util.SetVectorAxis(vector, new UnityEngine.Vector3Int((int) axis.x, (int) axis.y, (int) axis.z), value);
    public static UnityEngine.Vector3Int SetVectorAxis(UnityEngine.Vector3Int vector, UnityEngine.Vector3Int axis, int value) {
      vector.x = 0 != axis.x ? value : vector.x;
      vector.y = 0 != axis.y ? value : vector.y;
      vector.z = 0 != axis.z ? value : vector.z;

      return vector;
    }

    public static UnityEngine.Vector4 SetVectorAxis(UnityEngine.Vector4 vector, UnityEngine.Vector2    axis, float value) => Util.SetVectorAxis(vector, new UnityEngine.Vector4   ((float) axis.x, (float) axis.y, (float) 0.0f,   (float) 0.0f), value);
    public static UnityEngine.Vector4 SetVectorAxis(UnityEngine.Vector4 vector, UnityEngine.Vector2Int axis, float value) => Util.SetVectorAxis(vector, new UnityEngine.Vector4   ((float) axis.x, (float) axis.y, (float) 0.0f,   (float) 0.0f), value);
    public static UnityEngine.Vector4 SetVectorAxis(UnityEngine.Vector4 vector, UnityEngine.Vector3    axis, float value) => Util.SetVectorAxis(vector, new UnityEngine.Vector4   ((float) axis.x, (float) axis.y, (float) axis.z, (float) 0.0f), value);
    public static UnityEngine.Vector4 SetVectorAxis(UnityEngine.Vector4 vector, UnityEngine.Vector3Int axis, float value) => Util.SetVectorAxis(vector, new UnityEngine.Vector4   ((float) axis.x, (float) axis.y, (float) axis.z, (float) 0.0f), value);
    public static UnityEngine.Vector4 SetVectorAxis(UnityEngine.Vector4 vector, UnityEngine.Vector4    axis, float value) {
      vector.x = 0.0f != axis.x ? value : vector.x;
      vector.y = 0.0f != axis.y ? value : vector.y;
      vector.z = 0.0f != axis.z ? value : vector.z;
      vector.w = 0.0f != axis.w ? value : vector.w;

      return vector;
    }

    public static UnityEngine.Vector3    SetVectorBackAxis   (UnityEngine.Vector3    vector, float value) => Util.SetVectorAxis       (vector, UnityEngine.Vector3   .back, value);
    public static UnityEngine.Vector3Int SetVectorBackAxis   (UnityEngine.Vector3Int vector, int   value) => Util.SetVectorAxis       (vector, UnityEngine.Vector3Int.back, value);
    public static UnityEngine.Vector3    SetVectorDepthAxis  (UnityEngine.Vector3    vector, float value) => Util.SetVectorForwardAxis(vector, value);
    public static UnityEngine.Vector3Int SetVectorDepthAxis  (UnityEngine.Vector3Int vector, int   value) => Util.SetVectorForwardAxis(vector, value);
    public static UnityEngine.Vector2    SetVectorDownAxis   (UnityEngine.Vector2    vector, float value) => Util.SetVectorAxis       (vector, UnityEngine.Vector2   .down,    value);
    public static UnityEngine.Vector2Int SetVectorDownAxis   (UnityEngine.Vector2Int vector, int   value) => Util.SetVectorAxis       (vector, UnityEngine.Vector2Int.down,    value);
    public static UnityEngine.Vector3    SetVectorDownAxis   (UnityEngine.Vector3    vector, float value) => Util.SetVectorAxis       (vector, UnityEngine.Vector3   .down,    value);
    public static UnityEngine.Vector3Int SetVectorDownAxis   (UnityEngine.Vector3Int vector, int   value) => Util.SetVectorAxis       (vector, UnityEngine.Vector3Int.down,    value);
    public static UnityEngine.Vector3    SetVectorForwardAxis(UnityEngine.Vector3    vector, float value) => Util.SetVectorAxis       (vector, UnityEngine.Vector3   .forward, value);
    public static UnityEngine.Vector3Int SetVectorForwardAxis(UnityEngine.Vector3Int vector, int   value) => Util.SetVectorAxis       (vector, UnityEngine.Vector3Int.forward, value);
    public static UnityEngine.Vector2    SetVectorHeightAxis (UnityEngine.Vector2    vector, float value) => Util.SetVectorUpAxis     (vector, value);
    public static UnityEngine.Vector2Int SetVectorHeightAxis (UnityEngine.Vector2Int vector, int   value) => Util.SetVectorUpAxis     (vector, value);
    public static UnityEngine.Vector3    SetVectorHeightAxis (UnityEngine.Vector3    vector, float value) => Util.SetVectorUpAxis     (vector, value);
    public static UnityEngine.Vector3Int SetVectorHeightAxis (UnityEngine.Vector3Int vector, int   value) => Util.SetVectorUpAxis     (vector, value);
    public static UnityEngine.Vector2    SetVectorLeftAxis   (UnityEngine.Vector2    vector, float value) => Util.SetVectorAxis       (vector, UnityEngine.Vector2   .left,  value);
    public static UnityEngine.Vector2Int SetVectorLeftAxis   (UnityEngine.Vector2Int vector, int   value) => Util.SetVectorAxis       (vector, UnityEngine.Vector2Int.left,  value);
    public static UnityEngine.Vector3    SetVectorLeftAxis   (UnityEngine.Vector3    vector, float value) => Util.SetVectorAxis       (vector, UnityEngine.Vector3   .left,  value);
    public static UnityEngine.Vector3Int SetVectorLeftAxis   (UnityEngine.Vector3Int vector, int   value) => Util.SetVectorAxis       (vector, UnityEngine.Vector3Int.left,  value);
    public static UnityEngine.Vector2    SetVectorRightAxis  (UnityEngine.Vector2    vector, float value) => Util.SetVectorAxis       (vector, UnityEngine.Vector2   .right, value);
    public static UnityEngine.Vector2Int SetVectorRightAxis  (UnityEngine.Vector2Int vector, int   value) => Util.SetVectorAxis       (vector, UnityEngine.Vector2Int.right, value);
    public static UnityEngine.Vector3    SetVectorRightAxis  (UnityEngine.Vector3    vector, float value) => Util.SetVectorAxis       (vector, UnityEngine.Vector3   .right, value);
    public static UnityEngine.Vector3Int SetVectorRightAxis  (UnityEngine.Vector3Int vector, int   value) => Util.SetVectorAxis       (vector, UnityEngine.Vector3Int.right, value);
    public static UnityEngine.Vector2    SetVectorUpAxis     (UnityEngine.Vector2    vector, float value) => Util.SetVectorAxis       (vector, UnityEngine.Vector2   .up,    value);
    public static UnityEngine.Vector2Int SetVectorUpAxis     (UnityEngine.Vector2Int vector, int   value) => Util.SetVectorAxis       (vector, UnityEngine.Vector2Int.up,    value);
    public static UnityEngine.Vector3    SetVectorUpAxis     (UnityEngine.Vector3    vector, float value) => Util.SetVectorAxis       (vector, UnityEngine.Vector3   .up,    value);
    public static UnityEngine.Vector3Int SetVectorUpAxis     (UnityEngine.Vector3Int vector, int   value) => Util.SetVectorAxis       (vector, UnityEngine.Vector3Int.up,    value);
    public static UnityEngine.Vector2    SetVectorWidthAxis  (UnityEngine.Vector2    vector, float value) => Util.SetVectorRightAxis  (vector, value);
    public static UnityEngine.Vector2Int SetVectorWidthAxis  (UnityEngine.Vector2Int vector, int   value) => Util.SetVectorRightAxis  (vector, value);
    public static UnityEngine.Vector3    SetVectorWidthAxis  (UnityEngine.Vector3    vector, float value) => Util.SetVectorRightAxis  (vector, value);
    public static UnityEngine.Vector3Int SetVectorWidthAxis  (UnityEngine.Vector3Int vector, int   value) => Util.SetVectorRightAxis  (vector, value);

    /* TODO */
    public static void StopWaitForTimer(PatchOdyssey.Handler<PatchOdyssey.Collections.WaitForTimerEvent> callback) {}
    public static void StopWaitForTimer(double delay, PatchOdyssey.Handler<PatchOdyssey.Collections.WaitForTimerEvent> callback) {}

    public static object? Switch<T>(T value, System.Collections.Generic.Dictionary<T, object> expression, object? fallback = null) => Util.Switch<T>(value, (System.Collections.Generic.IReadOnlyDictionary<T, object>) expression, fallback);
    public static object? Switch<T>(T value, System.Collections.Generic.IReadOnlyDictionary<T, object> expression, object? fallback = null) {
      return expression?.TryGetValue(value, out object callback) ?? false ? callback : fallback;
    }

    [PatchMethod(AggressiveInlining)]
    public static void WaitForTimerEvery(double delay, PatchOdyssey.Handler<PatchOdyssey.Collections.WaitForTimerEvent> callback) {
      PatchOdyssey.Collections.EventHandler.Combine(WaitInfo.WAITS[double.NaN].handlers, new(callback, null, new() {data = (+delay, UnityEngine.Time.realtimeSinceStartupAsDouble + delay)}));
    }

    [PatchMethod(AggressiveInlining)]
    public static void WaitForTimerUntil(double delay, PatchOdyssey.Handler<PatchOdyssey.Collections.WaitForTimerEvent> callback) {
      PatchOdyssey.Collections.EventHandler.Combine(WaitInfo.WAITS[double.NaN].handlers, new(callback, null, new() {data = (-delay, UnityEngine.Time.realtimeSinceStartupAsDouble + delay)}));
    }

    public static UnityEngine.Bounds? WorldBoundsFromRectTransform(UnityEngine.RectTransform? transform) {
      UnityEngine.Rect? rectangle = Util.WorldRectFromRectTransform(transform);
      return rectangle is null ? null : new(transform!.position, new(rectangle?.width ?? 0.0f, rectangle?.height ?? 0.0f, 0.0f));
    }

    public static UnityEngine.Vector3[]? WorldCornersFromRectTransform(UnityEngine.RectTransform? transform) {
      return Util.CornersFromRectTransform(transform is null ? null : transform.GetWorldCorners);
    }

    public static UnityEngine.Rect? WorldRectFromRectTransform(UnityEngine.RectTransform? transform) {
      UnityEngine.Vector3[]? corners = Util.WorldCornersFromRectTransform(transform);
      return corners is null ? null : Util.RectFromCorners(corners);
    }
  }
}

namespace PatchOdyssey /* ⟶ Cheeky pre-initialization/ setup entry point */ {
  public static class Debug {
    [UnityEngine.RuntimeInitializeOnLoadMethod]
    private static void Main() {
      new UnityEngine.GameObject("…", typeof(PatchBehaviour));
    }
  }
}

internal sealed class PatchBehaviour : UnityEngine.MonoBehaviour {
  private static void Awake      () {}
  private static void FixedUpdate() {}
  private static void OnDestroy  () {}
}
// private sealed class WaitBehaviour : UnityEngine.MonoBehaviour {}

// private sealed class WaitInfo {
//   internal readonly struct HandlerInfo {
//     internal readonly ECS.Handler value;
//     internal readonly object?     metadata;
//   }

//   internal UnityEngine.Coroutine                        coroutine = null!;
//   internal System.Collections.Generic.List<HandlerInfo> handlers  = new(1);
// }

// /* … */
// private static UnityEngine.MonoBehaviour                                   WAIT  = new UnityEngine.GameObject("…").AddComponent<ECS.WaitBehaviour>();
// private static System.Collections.Generic.Dictionary<double, ECS.WaitInfo> WAITS = new(16);

// /* … */
// public static uint StopWait() {
//   /* StopWait all */
// }

// public static uint StopWait(ECS.Handler callback) {
//   uint     count     = 0u;
//   double[] durations = new double[WAITS.Count];

//   // …
//   WAITS.Keys.CopyTo(durations, 0);

//   foreach (double delay in durations)
//     count += StopWait(delay, callback);

//   return count;
// }

// public static uint StopWait(double delay, ECS.Handler callback) {
//   uint count = 0u;

//   // …
//   if (WAITS.TryGetValue(delay, out ECS.WaitInfo wait)) {
//     for (int index = wait.handlers.Count; 0 != index--; )
//     if (callback == wait.handlers[index].value) {
//       ++count;
//       wait.handlers.RemoveAt(index);
//     }

//     if (0 == wait.handlers.Count)
//     WAITS.Remove(delay);
//   }

//   return count;
// }

// public static void Wait(double delay, ECS.Handler callback, uint count, object? data = null) {
//   static System.Collections.IEnumerator EnumerateRoutine(double delay, uint count) {
//     bool         forever = 0u == count;
//     ECS.WaitInfo wait    = WAITS[delay];

//     // …
//     while (forever || 0u != count--) {
//       yield return new UnityEngine.WaitForSecondsRealtime((float) delay);

//       if (0 == wait.handlers.Count)
//       break;

//       foreach (var handler in wait.handlers)
//       handler.value(handler.metadata);
//     }

//     WAIT.StopCoroutine(wait.coroutine);
//   }

//   if (!WAITS.TryGetValue(delay, out ECS.WaitInfo wait)) {
//     WAITS.Add(delay, wait = new());

//     wait.handlers  = new(1) {new(value: callback, metadata: data)};
//     wait.coroutine = WAIT.StartCoroutine(EnumerateRoutine(delay, count));
//   } else wait.handlers.Add(new(value: callback, metadata: data));
// }

// public static void WaitEvery(double delay, ECS.Handler callback, object? data = null) => Wait(delay, callback, 0u, data);
// public static void WaitUntil(double delay, ECS.Handler callback, object? data = null) => Wait(delay, callback, 1u, data);
