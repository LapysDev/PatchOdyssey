global using Animation  = PatchOdyssey.Animation;                                                     //
global using PatchBurst = Unity.Burst.BurstCompileAttribute;                                          //
#if NET7_0 || NET7_0_OR_GREATER                                                                       // ⟶ Modify constructor with diagnostic features
  global using PatchConstructor = System.Diagnostics.CodeAnalysis.SetsRequiredMembersAttribute;       //    ^^
#endif                                                                                                //
global using PatchLayout = System.Runtime.InteropServices.StructLayoutAttribute;                      // ⟶ Un-manages the structure of class types
global using PatchMethod = System.Runtime.CompilerServices.MethodImplAttribute;                       // ⟶ Modify function with compile attributes e.g. inlined, optimized, unmanaged, …, e.t.c.
global using PatchOdyssey.Collections;                                                                // ⟶ Expose custom collections e.g. `GameObjectSharedList<T>`, `RefDictionary<T>`, …
global using PatchOffset = System.Runtime.InteropServices.FieldOffsetAttribute;                       // ⟶ Adjust offset of fields within structurally-unmanaged class types
#if NET9_0 || NET9_0_OR_GREATER                                                                       // ⟶ Modify function priority over another overload during resolution; Defaults to `PatchResolution(0)` for all functions
  global using PatchResolution = System.Runtime.CompilerServices.OverloadResolutionPriorityAttribute; //    ^^
#endif                                                                                                //
global using PatchUnburst = Unity.Burst.BurstDiscardAttribute;                                        // ⟶ `Unity.Burst.CompilerServices.IgnoreWarning(…)`?
global using static System.Runtime.CompilerServices.MethodImplOptions;                                // ⟶ Use case: `𝑓 PatchMethod(AggressiveOptimization, …)`
global using static System.Runtime.InteropServices.LayoutKind;                                        // ⟶ Use case: `𝑓 PatchLayout(Sequential, …)`

/* C# Polyfills */
namespace System.Runtime.Versioning {
  [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Constructor | System.AttributeTargets.Method | System.AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
  internal sealed class NonVersionableAttribute : System.Attribute {
    public NonVersionableAttribute() {}
  }
}

#if !(NET5_0 || NET5_0_OR_GREATER)
  namespace System.Runtime.CompilerServices {
    // ⟶ `init` @ `https://web.archive.org/web/20220918192058/https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/init`
    internal static class IsExternalInit {}
  }
#endif

#if !(NET7_0 || NET7_0_OR_GREATER)
  namespace System.Runtime.CompilerServices {
    // ⟶ `required` @ `https://web.archive.org/web/20220924164132/https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/required`
    public        class CompilerFeatureRequiredAttribute : System.Attribute { public CompilerFeatureRequiredAttribute(string name) {} }
    public sealed class RequiredMemberAttribute          : System.Attribute {}
  }

  namespace System.Diagnostics.CodeAnalysis {
    // ⟶ `System.Diagnostics.CodeAnalysis.UnscopedRefAttribute` @ `https://web.archive.org/web/20250401033710/https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.unscopedrefattribute?view=net-9.0`
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.Diagnostics.DebuggerNonUserCode]
    [System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Parameter | System.AttributeTargets.Property, Inherited = false)]
    sealed class UnscopedRefAttribute : System.Attribute {}
  }

  // ⟶ `System.Diagnostics.CodeAnalysis.SetsRequiredMembers` @ `https://web.archive.org/web/20250401051947/https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.setsrequiredmembersattribute?view=net-9.0`
  [System.AttributeUsage(System.AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
  [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
  [System.Diagnostics.DebuggerNonUserCode]
  public sealed class PatchConstructor : System.Attribute {}
#endif

#if !(NET8_0 || NET8_0_OR_GREATER)
  namespace System.Runtime.CompilerServices {
    // ⟶ Collection Expressions @ `https://web.archive.org/web/20231114165920/https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/collection-expressions`
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Interface | System.AttributeTargets.Struct, Inherited = false)]
    [System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal sealed class CollectionBuilderAttribute : System.Attribute {
      public System.Type BuilderType { get; }
      public string      MethodName  { get; }

      /* … */
      public CollectionBuilderAttribute(System.Type builderType, string methodName) {
        this.BuilderType = builderType;
        this.MethodName  = methodName;
      }
    }
  }
#endif

#if !(NET9_0 || NET9_0_OR_GREATER)
  // ⟶ `System.Runtime.CompilerServices.OverloadResolutionPriorityAttribute` @ `https://web.archive.org/web/20240920192519/https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.overloadresolutionpriorityattribute?view=net-9.0`
  [System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Constructor | System.AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
  [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
  [System.Diagnostics.DebuggerNonUserCode]
  public sealed class PatchResolution : System.Attribute {
    public int Priority { get; }

    /* … */
    public PatchResolution(int priority) => this.Priority = priority;
  }
#endif

#if NET46X || NET47X || NET48X || NETCOREAPP2X || NETSTANDARD2_0
  namespace System {
    // ⟶ `System.Index` @ `https://web.archive.org/web/20241129171203/https://learn.microsoft.com/en-us/dotnet/api/system.index?view=net-9.0`
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.Diagnostics.DebuggerNonUserCode]
    public readonly struct Index : System.IEquatable<Index> {
      public  static Index        Start     => new(0);
      public  static Index        End       => new(~0);
      public         bool         IsFromEnd => this.value < 0;
      public         int          Value     => this.value < 0 ? ~this.value : this.value;
      private        readonly int value;

      /* … */
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] private Index(int value)                       { this.value = value; }
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] public  Index(int value, bool fromEnd = false) { if (value < 0) { throw new System.ArgumentOutOfRangeException(nameof(value)); } this.value = fromEnd ? ~value : value; }

      /* … */
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] public  override bool   Equals                         (object? value)  => value is Index index && this.Equals(index);
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] public           bool   Equals                         (Index   index)  => index.value == this.value;
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] public  static   Index  FromEnd                        (int     value)  { if (value < 0) { throw new System.IndexOutOfRangeException(nameof(value)); } return new(~value); }
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] public  static   Index  FromStart                      (int     value)  { if (value < 0) { throw new System.IndexOutOfRangeException(nameof(value)); } return new(value); }
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] public  override int    GetHashCode                    ()               => this.value;
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] public           int    GetOffset                      (int length)     => this.IsFromEnd ? this.value + (length + 1) : this.value;
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] public  override string ToString                       ()               => this.IsFromEnd ? this.ToStringFromEnd() : ((uint) this.Value).ToString();
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] private          string ToStringFromEnd                ()               => $"^{this.Value.ToString()}";
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] bool                    System.IEquatable<Index>.Equals(Index index)    => this.Equals(index);

      /* … */
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
      public static implicit operator Index(int value) => Index.FromStart(value);
    }

    // ⟶ `System.Range` @ `https://web.archive.org/web/20241231015910/https://learn.microsoft.com/en-us/dotnet/api/system.range?view=net-9.0`
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.Diagnostics.DebuggerNonUserCode]
    public readonly struct Range : System.IEquatable<Range> {
      public static /* readonly */ Range All => new(System.Index.Start, System.Index.End);

      public readonly System.Index End   { get; } = System.Index.End;
      public readonly System.Index Start { get; } = System.Index.Start;

      /* … */
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
      public Range(System.Index start, System.Index end) {
        this.End   = end;
        this.Start = start;
      }

      /* … */
      public static Range EndAt(System.Index end) {
        return new(System.Index.Start, end);
      }

      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] public override bool Equals     (object?  value) => value is Range range && this.Equals(range);
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] public          bool Equals     (in Range range) => this.End.Equals(range.End) && this.Start.Equals(range.Start);
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] public override int  GetHashCode()               => System.HashCode.Combine(this.End, this.Start);

      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
      public (int Offset, int Length) GetOffsetAndLength(int length) {
        (System.Index endIndex, System.Index startIndex) = (this.End, this.Start);
        (int          end,      int          start)      = (endIndex.IsFromEnd ? length - endIndex.Value : endIndex.Value, startIndex.IsFromEnd ? length - startIndex.Value : startIndex.Value);

        // …
        if ((uint) end > (uint) length || (uint) end < (uint) start)
        throw new System.ArgumentOutOfRangeException(nameof(length));

        return (start, end - start);
      }

      public static Range StartAt(System.Index start) {
        return new(start, System.Index.End);
      }

      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] public override string ToString                       ()            => $"{this.Start}..{this.End}";
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] bool                   System.IEquatable<Range>.Equals(Range range) => this.Equals(range);
    }
  }

  namespace System.Runtime.CompilerServices {
    // ⟶ `System.Runtime.CompilerServices.RuntimeHelpers.GetSubArray<…>(…)` @ `https://web.archive.org/web/20250420233858/https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.runtimehelpers.getsubarray?view=net-9.0`
    public static partial class RuntimeHelpers {
      public static T[] GetSubArray<T>(T[] array, System.Range range) {
        (int offset, int length) = range.GetOffsetAndLength(array.Length);
        T[] subarray;

        // …
        if (typeof(T[]) == array.GetType()) {
          if (0 == length)
          return System.Array.Empty<T>();

          subarray = new T[length];
        } else subarray = System.Runtime.CompilerServices.Unsafe.As<T[]>(System.Array.CreateInstance(array.GetType(), length));

        System.Array.Copy(array, offset, subarray, 0, length);
        return subarray;
      }
    }
  }
#endif

/* PatchOdyssey */
namespace PatchOdyssey /* ⟶ Class types and delegates */ {
  namespace Animation {
    public class UIKeyframe : System.Collections.Specialized.ListDictionary, System.Collections.IDictionary, System.Collections.Generic.IReadOnlyDictionary<string, object?> /* ⟶ Optimized for less than 10 properties using an internal singly-linked list; See `https://web.archive.org/web/20241209052731/https://learn.microsoft.com/en-us/dotnet/api/system.collections.specialized.listdictionary?view=net-9.0` */ {
      public struct Enumerator : System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, object?>>, System.Collections.IDictionaryEnumerator {
        public  System.Collections.Generic.KeyValuePair<string, object?> Current { get { System.Collections.DictionaryEntry element = this.enumerator.Entry; return new((string) element.Key, element.Value); } }
        private System.Collections.IDictionaryEnumerator                 enumerator;
        System.Collections.Generic.KeyValuePair<string, object?>         System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, object?>>.Current => this           .Current;
        System.Collections.DictionaryEntry                               System.Collections.IDictionaryEnumerator.Entry                                                           => this.enumerator.Entry;
        object                                                           System.Collections.IDictionaryEnumerator.Key                                                             => this.enumerator.Key;
        object?                                                          System.Collections.IDictionaryEnumerator.Value                                                           => this.enumerator.Value;
        object                                                           System.Collections.IEnumerator.Current                                                                   => this           .Current;

        /* … */
        [PatchConstructor, PatchMethod(AggressiveInlining)]
        internal Enumerator(UIKeyframe keyframe) => this.enumerator = ((System.Collections.Specialized.ListDictionary) keyframe).GetEnumerator();

        /* … */
        [PatchMethod(AggressiveInlining)] public void Dispose                                () { /* Do nothing… */ }
        [PatchMethod(AggressiveInlining)] public bool MoveNext                               () => this.enumerator.MoveNext();
        [PatchMethod(AggressiveInlining)] public void Reset                                  () => this.enumerator.Reset   ();
        [PatchMethod(AggressiveInlining)] bool        System.Collections.IEnumerator.MoveNext() => this           .MoveNext();
        [PatchMethod(AggressiveInlining)] void        System.Collections.IEnumerator.Reset   () => this           .Reset   ();
        [PatchMethod(AggressiveInlining)] void        System.IDisposable.Dispose             () => this           .Dispose ();
      }

      public sealed class KeyEnumerable : System.Collections.Generic.IEnumerable<string> {
        private readonly UIKeyframe keyframe;

        /* … */
        [PatchConstructor, PatchMethod(AggressiveInlining)]
        internal KeyEnumerable(UIKeyframe keyframe) => this.keyframe = keyframe;

        /* … */
        [PatchMethod(AggressiveInlining)] public UIKeyframe.KeyEnumerator                GetEnumerator                                               () => new(keyframe);
        [PatchMethod(AggressiveInlining)] System.Collections.Generic.IEnumerator<string> System.Collections.Generic.IEnumerable<string>.GetEnumerator() => this.GetEnumerator();
        [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator                 System.Collections.IEnumerable.GetEnumerator                () => this.GetEnumerator();
      }

      public struct KeyEnumerator : System.Collections.Generic.IEnumerator<string> {
        public  string                                   Current => (string) this.enumerator.Key;
        private System.Collections.IDictionaryEnumerator enumerator;
        string                                           System.Collections.Generic.IEnumerator<string>.Current => this.Current;
        object                                           System.Collections.IEnumerator.Current                 => this.Current!;

        /* … */
        [PatchConstructor, PatchMethod(AggressiveInlining)]
        internal KeyEnumerator(UIKeyframe keyframe) => this.enumerator = ((System.Collections.Specialized.ListDictionary) keyframe).GetEnumerator();

        /* … */
        [PatchMethod(AggressiveInlining)] public void Dispose                                () { /* Do nothing… */ }
        [PatchMethod(AggressiveInlining)] public bool MoveNext                               () => this.enumerator.MoveNext();
        [PatchMethod(AggressiveInlining)] public void Reset                                  () => this.enumerator.Reset   ();
        [PatchMethod(AggressiveInlining)] bool        System.Collections.IEnumerator.MoveNext() => this           .MoveNext();
        [PatchMethod(AggressiveInlining)] void        System.Collections.IEnumerator.Reset   () => this           .Reset   ();
        [PatchMethod(AggressiveInlining)] void        System.IDisposable.Dispose             () => this           .Dispose ();
      }

      public sealed class ValueEnumerable : System.Collections.Generic.IEnumerable<object?> {
        private readonly UIKeyframe keyframe;

        /* … */
        [PatchConstructor, PatchMethod(AggressiveInlining)]
        internal ValueEnumerable(UIKeyframe keyframe) => this.keyframe = keyframe;

        /* … */
        [PatchMethod(AggressiveInlining)] public UIKeyframe.ValueEnumerator               GetEnumerator                                                () => new(keyframe);
        [PatchMethod(AggressiveInlining)] System.Collections.Generic.IEnumerator<object?> System.Collections.Generic.IEnumerable<object?>.GetEnumerator() => this.GetEnumerator();
        [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator                  System.Collections.IEnumerable.GetEnumerator                 () => this.GetEnumerator();
      }

      public struct ValueEnumerator : System.Collections.Generic.IEnumerator<object?> {
        public  object?                                  Current => this.enumerator.Value;
        private System.Collections.IDictionaryEnumerator enumerator;
        object?                                          System.Collections.Generic.IEnumerator<object?>.Current => this.Current;
        object                                           System.Collections.IEnumerator.Current                  => this.Current!;

        /* … */
        [PatchConstructor, PatchMethod(AggressiveInlining)]
        internal ValueEnumerator(UIKeyframe keyframe) => this.enumerator = ((System.Collections.Specialized.ListDictionary) keyframe).GetEnumerator();

        /* … */
        [PatchMethod(AggressiveInlining)] public void Dispose                                () { /* Do nothing… */ }
        [PatchMethod(AggressiveInlining)] public bool MoveNext                               () => this.enumerator.MoveNext();
        [PatchMethod(AggressiveInlining)] public void Reset                                  () => this.enumerator.Reset   ();
        [PatchMethod(AggressiveInlining)] bool        System.Collections.IEnumerator.MoveNext() => this           .MoveNext();
        [PatchMethod(AggressiveInlining)] void        System.Collections.IEnumerator.Reset   () => this           .Reset   ();
        [PatchMethod(AggressiveInlining)] void        System.IDisposable.Dispose             () => this           .Dispose ();
      }

      /* … */
      public   new uint                                                    Count                                                                                                          => ((uint) base.Count);
      internal     readonly System.Collections.Specialized.ListDictionary? begin                                                                                                          =  null;
      internal     System.Collections.Specialized.ListDictionary?          end                                                                                                            { get => this.properties; private init {} }
      public       readonly string?                                        name                                                                                                           =  null; // ⟶ Explicitly not `string.Empty`
      public       System.Collections.Specialized.ListDictionary           properties                                                                                                     => (System.Collections.Specialized.ListDictionary) this;
      int                                                                  System.Collections.Generic.IReadOnlyCollection<System.Collections.Generic.KeyValuePair<string, object?>>.Count => base.Count;
      System.Collections.Generic.IEnumerable<string>                       System.Collections.Generic.IReadOnlyDictionary<string, object?>.Keys                                           => new UIKeyframe.KeyEnumerable  (this);
      System.Collections.Generic.IEnumerable<object?>                      System.Collections.Generic.IReadOnlyDictionary<string, object?>.Values                                         => new UIKeyframe.ValueEnumerable(this);
      int                                                                  System.Collections.ICollection.Count                                                                           => base.Count;
      bool                                                                 System.Collections.ICollection.IsSynchronized                                                                  => false;
      object                                                               System.Collections.ICollection.SyncRoot                                                                        => this;
      bool                                                                 System.Collections.IDictionary.IsFixedSize                                                                     => true;
      bool                                                                 System.Collections.IDictionary.IsReadOnly                                                                      => true;
      System.Collections.ICollection                                       System.Collections.IDictionary.Keys                                                                            => base.Keys;
      System.Collections.ICollection                                       System.Collections.IDictionary.Values                                                                          => base.Values;

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)]
      public UIKeyframe(string name, System.Collections.Generic.IReadOnlyDictionary<string, object?> properties) {
        this.name = name;

        foreach (System.Collections.Generic.KeyValuePair<string, object?> property in properties)
        this.properties.Add(property.Key, property.Value);
      }

      [PatchConstructor, PatchMethod(AggressiveInlining)]
      protected internal UIKeyframe(string name, System.Collections.Generic.IReadOnlyDictionary<string, object?> begin, System.Collections.Generic.IReadOnlyDictionary<string, object?> end) {
        this.begin = new();
        this.end   = new();
        this.name  = name;

        foreach (System.Collections.Generic.KeyValuePair<string, object?> property in begin) { if (end  .ContainsKey(property.Key)) this.begin.Add(property.Key, property.Value); }
        foreach (System.Collections.Generic.KeyValuePair<string, object?> property in end)   { if (begin.ContainsKey(property.Key)) this.end  .Add(property.Key, property.Value); }
      }

      /* … */
      [PatchMethod(AggressiveInlining)] System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, object?>> System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object?>>.GetEnumerator()                                      => new UIKeyframe.Enumerator(this);
      [PatchMethod(AggressiveInlining)] bool                                                                                             System.Collections.Generic.IReadOnlyDictionary<string, object?>.ContainsKey                                   (string       key)                      => base.Contains(key);
      [PatchMethod(AggressiveInlining)] bool                                                                                             System.Collections.Generic.IReadOnlyDictionary<string, object?>.TryGetValue                                   (string       key,   out object? value) { if (base.Contains(key)) { value = base[key]; return true; } value = default; return false; }
      [PatchMethod(AggressiveInlining)] void                                                                                             System.Collections.ICollection.CopyTo                                                                         (System.Array array, int         index) => base.CopyTo(array, index);
      [PatchMethod(AggressiveInlining)] void                                                                                             System.Collections.IDictionary.Add                                                                            (object       key,   object?     value) => throw new System.NotSupportedException("UI keyframe is read-only");
      [PatchMethod(AggressiveInlining)] void                                                                                             System.Collections.IDictionary.Clear                                                                          ()                                      => throw new System.NotSupportedException("UI keyframe is read-only");
      [PatchMethod(AggressiveInlining)] bool                                                                                             System.Collections.IDictionary.Contains                                                                       (object key)                            => base.Contains     (key);
      [PatchMethod(AggressiveInlining)] System.Collections.IDictionaryEnumerator                                                         System.Collections.IDictionary.GetEnumerator                                                                  ()                                      => base.GetEnumerator();
      [PatchMethod(AggressiveInlining)] void                                                                                             System.Collections.IDictionary.Remove                                                                         (object key)                            => throw new System.NotSupportedException("UI keyframe is read-only");
      [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator                                                                   System.Collections.IEnumerable.GetEnumerator                                                                  ()                                      => base.GetEnumerator();

      public object? this                                                                [string key] { get => base[key]; set => base[key] = value; }
      object?        System.Collections.Generic.IReadOnlyDictionary<string, object?>.this[string key] => base[key];
      object?        System.Collections.IDictionary.this                                 [object key] { get => base[key]; set => base[key] = value; }
    }

    public class UISequence : PatchOdyssey.Animation.UIKeyframe, System.Collections.Generic.IReadOnlyDictionary<string, object?>, System.Collections.IDictionary {
      public new struct Enumerator : System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, object?>>, System.Collections.IDictionaryEnumerator {
        public  System.Collections.Generic.KeyValuePair<string, object?> Current => new(this.enumerator.Current, this.sequence[this.enumerator.Current]);
        private PatchOdyssey.Animation.UIKeyframe.KeyEnumerator          enumerator;
        private UISequence                                               sequence;
        System.Collections.Generic.KeyValuePair<string, object?>         System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, object?>>.Current => this.Current;
        System.Collections.DictionaryEntry                               System.Collections.IDictionaryEnumerator.Entry                                                           => new(this.Current.Key, this.Current.Value);
        object                                                           System.Collections.IDictionaryEnumerator.Key                                                             => this.Current.Key;
        object?                                                          System.Collections.IDictionaryEnumerator.Value                                                           => this.Current.Value;
        object                                                           System.Collections.IEnumerator.Current                                                                   => this.Current!;

        [PatchConstructor, PatchMethod(AggressiveInlining)]
        internal Enumerator(UISequence sequence) => this.enumerator = new PatchOdyssey.Animation.UIKeyframe.KeyEnumerable(this.sequence = sequence).GetEnumerator();

        [PatchMethod(AggressiveInlining)] public void Dispose                                () => this.enumerator.Dispose ();
        [PatchMethod(AggressiveInlining)] public bool MoveNext                               () => this.enumerator.MoveNext();
        [PatchMethod(AggressiveInlining)] public void Reset                                  () => this.enumerator.Reset   ();
        [PatchMethod(AggressiveInlining)] bool        System.Collections.IEnumerator.MoveNext() => this           .MoveNext();
        [PatchMethod(AggressiveInlining)] void        System.Collections.IEnumerator.Reset   () => this           .Reset   ();
        [PatchMethod(AggressiveInlining)] void        System.IDisposable.Dispose             () => this           .Dispose ();
      }

      public sealed class ValueCollection : System.Collections.Generic.ICollection<object?>, System.Collections.ICollection {
        private readonly UISequence sequence;
        int                         System.Collections.Generic.ICollection<object?>.Count      => ((int) this.sequence.Count);
        bool                        System.Collections.Generic.ICollection<object?>.IsReadOnly => false;
        int                         System.Collections.ICollection.Count                       => ((int) this.sequence.Count);
        bool                        System.Collections.ICollection.IsSynchronized              => false;
        object                      System.Collections.ICollection.SyncRoot                    => this;

        /* … */
        [PatchConstructor, PatchMethod(AggressiveInlining)]
        internal ValueCollection(UISequence sequence) => this.sequence = sequence;

        /* … */
        [PatchMethod(AggressiveInlining)] private void                                    CopyTo                                                       (System.Array array, uint index) { foreach (object? value in this) array.SetValue(value, index++); }
        [PatchMethod(AggressiveInlining)] public UISequence.ValueEnumerator               GetEnumerator                                                ()                               => new(sequence);
        [PatchMethod(AggressiveInlining)] void                                            System.Collections.Generic.ICollection<object?>.Add          (object? keyframe)               => throw new System.NotSupportedException("UI sequence values are dynamically generated and non-persistent");
        [PatchMethod(AggressiveInlining)] void                                            System.Collections.Generic.ICollection<object?>.Clear        ()                               => throw new System.NotSupportedException("UI sequence values are dynamically generated and non-persistent");
        [PatchMethod(AggressiveInlining)] bool                                            System.Collections.Generic.ICollection<object?>.Contains     (object?   keyframe)             => throw new System.NotSupportedException("UI sequence values are dynamically generated and non-persistent");
        [PatchMethod(AggressiveInlining)] void                                            System.Collections.Generic.ICollection<object?>.CopyTo       (object?[] array, int index)     => this.CopyTo(array, (uint) index);
        [PatchMethod(AggressiveInlining)] bool                                            System.Collections.Generic.ICollection<object?>.Remove       (object?   keyframe)             => throw new System.NotSupportedException("UI sequence values are dynamically generated and non-persistent");
        [PatchMethod(AggressiveInlining)] System.Collections.Generic.IEnumerator<object?> System.Collections.Generic.IEnumerable<object?>.GetEnumerator()                               => this.GetEnumerator();
        [PatchMethod(AggressiveInlining)] void                                            System.Collections.ICollection.CopyTo                        (System.Array array, int index)  => this.CopyTo(array, (uint) index);
        [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator                  System.Collections.IEnumerable.GetEnumerator                 ()                               => this.GetEnumerator();
      }

      public new struct ValueEnumerator : System.Collections.Generic.IEnumerator<object?> {
        public  object?                                         Current => this.sequence[this.enumerator.Current];
        private PatchOdyssey.Animation.UIKeyframe.KeyEnumerator enumerator;
        private UISequence                                      sequence;
        object?                                                 System.Collections.Generic.IEnumerator<object?>.Current => this.Current;
        object                                                  System.Collections.IEnumerator.Current                  => this.Current!;

        [PatchConstructor, PatchMethod(AggressiveInlining)]
        internal ValueEnumerator(UISequence sequence) => this.enumerator = new PatchOdyssey.Animation.UIKeyframe.KeyEnumerable(this.sequence = sequence).GetEnumerator();

        [PatchMethod(AggressiveInlining)] public void Dispose                                () => this.enumerator.Dispose ();
        [PatchMethod(AggressiveInlining)] public bool MoveNext                               () => this.enumerator.MoveNext();
        [PatchMethod(AggressiveInlining)] public void Reset                                  () => this.enumerator.Reset   ();
        [PatchMethod(AggressiveInlining)] bool        System.Collections.IEnumerator.MoveNext() => this           .MoveNext();
        [PatchMethod(AggressiveInlining)] void        System.Collections.IEnumerator.Reset   () => this           .Reset   ();
        [PatchMethod(AggressiveInlining)] void        System.IDisposable.Dispose             () => this           .Dispose ();
      }

      /* … */
      private static            System.Collections.Generic.Dictionary<(System.Type, System.Type), ((System.Reflection.MethodInfo, System.Reflection.MethodInfo), System.Type)> Interpolations                                                                                                 =  new(3);
      public  required          double                                                                                                                                         delay                                                                                                          =  0.0;
      public  required          double                                                                                                                                         duration                                                                                                       =  0.0;
      public  required          PatchOdyssey.Tweener                                                                                                                           easing                                                                                                         =  PatchOdyssey.Animation.Function  .Linear;
      public  required          PatchOdyssey.Interpolator                                                                                                                      interpolator                                                                                                   =  PatchOdyssey.Animation.UISequence.Interpolate;
      public                    bool                                                                                                                                           isDone                                                                                                         => UnityEngine.Time.realtimeSinceStartupAsDouble >= this.delay + this.duration + this.timestamp;
      private          readonly System.Collections.Generic.SortedList<double, PatchOdyssey.Animation.UIKeyframe>                                                               keyframes                                                                                                      =  new(1);
      private                   double                                                                                                                                         timestamp                                                                                                      =  0.0;
      int                                                                                                                                                                      System.Collections.Generic.IReadOnlyCollection<System.Collections.Generic.KeyValuePair<string, object?>>.Count => ((int) base.Count);
      System.Collections.Generic.IEnumerable<string>                                                                                                                           System.Collections.Generic.IReadOnlyDictionary<string, object?>.Keys                                           => new PatchOdyssey.Animation.UIKeyframe.KeyEnumerable(this);
      System.Collections.Generic.IEnumerable<object?>                                                                                                                          System.Collections.Generic.IReadOnlyDictionary<string, object?>.Values                                         => new UISequence.ValueCollection(this);
      int                                                                                                                                                                      System.Collections.ICollection.Count                                                                           => ((int) base.Count);
      bool                                                                                                                                                                     System.Collections.ICollection.IsSynchronized                                                                  => false;
      object                                                                                                                                                                   System.Collections.ICollection.SyncRoot                                                                        => this;
      bool                                                                                                                                                                     System.Collections.IDictionary.IsFixedSize                                                                     => false;
      bool                                                                                                                                                                     System.Collections.IDictionary.IsReadOnly                                                                      => false;
      System.Collections.ICollection                                                                                                                                           System.Collections.IDictionary.Keys                                                                            => base.Keys;
      System.Collections.ICollection                                                                                                                                           System.Collections.IDictionary.Values                                                                          => new UISequence.ValueCollection(this);

      /* … ⟶ `𝑓 UISequence([optional] name, duration, [optional] delay, [optional] easing, [optional] interpolator, begin, end) { … }` */
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration,                                                                                    System.Collections.Generic.Dictionary         <string, object?> begin, System.Collections.Generic.Dictionary         <string, object?> end) : this(null!, duration, 0.0,   null!,  null!,        (System.Collections.Generic.IReadOnlyDictionary<string, object?>) begin, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) end) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration,                                                                                    System.Collections.Generic.IReadOnlyDictionary<string, object?> begin, System.Collections.Generic.IReadOnlyDictionary<string, object?> end) : this(null!, duration, 0.0,   null!,  null!,        begin,                                                                    end)                                                                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration, double delay,                                                                      System.Collections.Generic.Dictionary         <string, object?> begin, System.Collections.Generic.Dictionary         <string, object?> end) : this(null!, duration, 0.0,   null!,  null!,        (System.Collections.Generic.IReadOnlyDictionary<string, object?>) begin, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) end) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration, double delay,                                                                      System.Collections.Generic.IReadOnlyDictionary<string, object?> begin, System.Collections.Generic.IReadOnlyDictionary<string, object?> end) : this(null!, duration, 0.0,   null!,  null!,        begin,                                                                    end)                                                                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration,               PatchOdyssey.Tweener easing,                                         System.Collections.Generic.Dictionary         <string, object?> begin, System.Collections.Generic.Dictionary         <string, object?> end) : this(null!, duration, 0.0,   easing, null!,        (System.Collections.Generic.IReadOnlyDictionary<string, object?>) begin, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) end) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration,               PatchOdyssey.Tweener easing,                                         System.Collections.Generic.IReadOnlyDictionary<string, object?> begin, System.Collections.Generic.IReadOnlyDictionary<string, object?> end) : this(null!, duration, 0.0,   easing, null!,        begin,                                                                    end)                                                                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration, double delay, PatchOdyssey.Tweener easing,                                         System.Collections.Generic.Dictionary         <string, object?> begin, System.Collections.Generic.Dictionary         <string, object?> end) : this(null!, duration, delay, easing, null!,        (System.Collections.Generic.IReadOnlyDictionary<string, object?>) begin, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) end) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration, double delay, PatchOdyssey.Tweener easing,                                         System.Collections.Generic.IReadOnlyDictionary<string, object?> begin, System.Collections.Generic.IReadOnlyDictionary<string, object?> end) : this(null!, duration, delay, easing, null!,        begin,                                                                    end)                                                                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration,                                            PatchOdyssey.Interpolator interpolator, System.Collections.Generic.Dictionary         <string, object?> begin, System.Collections.Generic.Dictionary         <string, object?> end) : this(null!, duration, 0.0,   null!,  interpolator, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) begin, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) end) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration,                                            PatchOdyssey.Interpolator interpolator, System.Collections.Generic.IReadOnlyDictionary<string, object?> begin, System.Collections.Generic.IReadOnlyDictionary<string, object?> end) : this(null!, duration, 0.0,   null!,  interpolator, begin,                                                                    end)                                                                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration, double delay,                              PatchOdyssey.Interpolator interpolator, System.Collections.Generic.Dictionary         <string, object?> begin, System.Collections.Generic.Dictionary         <string, object?> end) : this(null!, duration, delay, null!,  interpolator, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) begin, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) end) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration, double delay,                              PatchOdyssey.Interpolator interpolator, System.Collections.Generic.IReadOnlyDictionary<string, object?> begin, System.Collections.Generic.IReadOnlyDictionary<string, object?> end) : this(null!, duration, delay, null!,  interpolator, begin,                                                                    end)                                                                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration,               PatchOdyssey.Tweener easing, PatchOdyssey.Interpolator interpolator, System.Collections.Generic.Dictionary         <string, object?> begin, System.Collections.Generic.Dictionary         <string, object?> end) : this(null!, duration, 0.0,   easing, interpolator, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) begin, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) end) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration,               PatchOdyssey.Tweener easing, PatchOdyssey.Interpolator interpolator, System.Collections.Generic.IReadOnlyDictionary<string, object?> begin, System.Collections.Generic.IReadOnlyDictionary<string, object?> end) : this(null!, duration, 0.0,   easing, interpolator, begin,                                                                    end)                                                                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration, double delay, PatchOdyssey.Tweener easing, PatchOdyssey.Interpolator interpolator, System.Collections.Generic.Dictionary         <string, object?> begin, System.Collections.Generic.Dictionary         <string, object?> end) : this(null!, duration, delay, easing, interpolator, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) begin, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) end) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence             (double duration, double delay, PatchOdyssey.Tweener easing, PatchOdyssey.Interpolator interpolator, System.Collections.Generic.IReadOnlyDictionary<string, object?> begin, System.Collections.Generic.IReadOnlyDictionary<string, object?> end) : this(null!, duration, delay, easing, interpolator, begin,                                                                    end)                                                                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration,                                                                                    System.Collections.Generic.Dictionary         <string, object?> begin, System.Collections.Generic.Dictionary         <string, object?> end) : this(name,  duration, 0.0,   null!,  null!,        (System.Collections.Generic.IReadOnlyDictionary<string, object?>) begin, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) end) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration,                                                                                    System.Collections.Generic.IReadOnlyDictionary<string, object?> begin, System.Collections.Generic.IReadOnlyDictionary<string, object?> end) : this(name,  duration, 0.0,   null!,  null!,        begin,                                                                    end)                                                                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration, double delay,                                                                      System.Collections.Generic.Dictionary         <string, object?> begin, System.Collections.Generic.Dictionary         <string, object?> end) : this(name,  duration, 0.0,   null!,  null!,        (System.Collections.Generic.IReadOnlyDictionary<string, object?>) begin, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) end) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration, double delay,                                                                      System.Collections.Generic.IReadOnlyDictionary<string, object?> begin, System.Collections.Generic.IReadOnlyDictionary<string, object?> end) : this(name,  duration, 0.0,   null!,  null!,        begin,                                                                    end)                                                                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration,               PatchOdyssey.Tweener easing,                                         System.Collections.Generic.Dictionary         <string, object?> begin, System.Collections.Generic.Dictionary         <string, object?> end) : this(name,  duration, 0.0,   easing, null!,        (System.Collections.Generic.IReadOnlyDictionary<string, object?>) begin, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) end) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration,               PatchOdyssey.Tweener easing,                                         System.Collections.Generic.IReadOnlyDictionary<string, object?> begin, System.Collections.Generic.IReadOnlyDictionary<string, object?> end) : this(name,  duration, 0.0,   easing, null!,        begin,                                                                    end)                                                                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration, double delay, PatchOdyssey.Tweener easing,                                         System.Collections.Generic.Dictionary         <string, object?> begin, System.Collections.Generic.Dictionary         <string, object?> end) : this(name,  duration, delay, easing, null!,        (System.Collections.Generic.IReadOnlyDictionary<string, object?>) begin, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) end) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration, double delay, PatchOdyssey.Tweener easing,                                         System.Collections.Generic.IReadOnlyDictionary<string, object?> begin, System.Collections.Generic.IReadOnlyDictionary<string, object?> end) : this(name,  duration, delay, easing, null!,        begin,                                                                    end)                                                                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration,                                            PatchOdyssey.Interpolator interpolator, System.Collections.Generic.Dictionary         <string, object?> begin, System.Collections.Generic.Dictionary         <string, object?> end) : this(name,  duration, 0.0,   null!,  interpolator, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) begin, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) end) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration,                                            PatchOdyssey.Interpolator interpolator, System.Collections.Generic.IReadOnlyDictionary<string, object?> begin, System.Collections.Generic.IReadOnlyDictionary<string, object?> end) : this(name,  duration, 0.0,   null!,  interpolator, begin,                                                                    end)                                                                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration, double delay,                              PatchOdyssey.Interpolator interpolator, System.Collections.Generic.Dictionary         <string, object?> begin, System.Collections.Generic.Dictionary         <string, object?> end) : this(name,  duration, delay, null!,  interpolator, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) begin, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) end) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration, double delay,                              PatchOdyssey.Interpolator interpolator, System.Collections.Generic.IReadOnlyDictionary<string, object?> begin, System.Collections.Generic.IReadOnlyDictionary<string, object?> end) : this(name,  duration, delay, null!,  interpolator, begin,                                                                    end)                                                                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration,               PatchOdyssey.Tweener easing, PatchOdyssey.Interpolator interpolator, System.Collections.Generic.Dictionary         <string, object?> begin, System.Collections.Generic.Dictionary         <string, object?> end) : this(name,  duration, 0.0,   easing, interpolator, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) begin, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) end) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration,               PatchOdyssey.Tweener easing, PatchOdyssey.Interpolator interpolator, System.Collections.Generic.IReadOnlyDictionary<string, object?> begin, System.Collections.Generic.IReadOnlyDictionary<string, object?> end) : this(name,  duration, 0.0,   easing, interpolator, begin,                                                                    end)                                                                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration, double delay, PatchOdyssey.Tweener easing, PatchOdyssey.Interpolator interpolator, System.Collections.Generic.Dictionary         <string, object?> begin, System.Collections.Generic.Dictionary         <string, object?> end) : this(name,  duration, delay, easing, interpolator, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) begin, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) end) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public UISequence(string name, double duration, double delay, PatchOdyssey.Tweener easing, PatchOdyssey.Interpolator interpolator, System.Collections.Generic.IReadOnlyDictionary<string, object?> begin, System.Collections.Generic.IReadOnlyDictionary<string, object?> end) : base(name, begin, end)                                                                                                                                                                             { this.delay = delay; this.duration = duration; this.easing = easing ?? this.easing; this.interpolator = interpolator ?? this.interpolator; this.timestamp = UnityEngine.Time.realtimeSinceStartupAsDouble; }

      /* … ⟶ `𝑓 Add([optional] keyframe, progress, properties) { … }` */
      [PatchMethod(AggressiveInlining)] public void Add             (double progress, System.Collections.Generic.Dictionary         <string, object?> properties) => this.Add($"#{this.keyframes.Count + 1}", progress, properties);
      [PatchMethod(AggressiveInlining)] public void Add             (double progress, System.Collections.Generic.IReadOnlyDictionary<string, object?> properties) => this.Add($"#{this.keyframes.Count + 1}", progress, properties);
      [PatchMethod(AggressiveInlining)] public void Add(string name, double progress, System.Collections.Generic.Dictionary         <string, object?> properties) => this.Add(name,                           progress, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) properties);
      [PatchMethod(AggressiveInlining)] public void Add(string name, double progress, System.Collections.Generic.IReadOnlyDictionary<string, object?> properties) { foreach (System.Collections.Generic.KeyValuePair<double, PatchOdyssey.Animation.UIKeyframe> enumerated in this.keyframes) { if (enumerated.Key == progress || (name is not null && enumerated.Value.name == name)) return; } this.keyframes.Add(progress, new(name, properties)); } // ⟶ `System.Collections.Generic.SortedList<…>` keeps `::keyframes` sorted

      private static object? Interpolate(double progress, object? a, object? b) /* ⟶ `(b - a) * progress` */ {
        (System.Type a, System.Type b) types = (a?.GetType()!, b?.GetType()!);

        // …
        if (!UISequence.Interpolations.TryGetValue(types, out ((System.Reflection.MethodInfo multiplication, System.Reflection.MethodInfo subtraction) operators, System.Type progress) interpolation)) {
          if (types.a != types.b)
          return null;

          switch (a) {
            case double  value: return (double)  ((double) ((double)  b! - value) * progress);
            case float   value: return (float)   ((double) ((float)   b! - value) * progress);
            case decimal value: return (decimal) ((double) ((decimal) b! - value) * progress);
            case int     value: return (int)     ((double) ((int)     b! - value) * progress);
            case nint    value: return (nint)    ((double) ((nint)    b! - value) * progress);
            case long    value: return (long)    ((double) ((long)    b! - value) * progress);
            case uint    value: return (uint)    ((double) ((uint)    b! - value) * progress);
            case nuint   value: return (nuint)   ((double) ((nuint)   b! - value) * progress);
            case ulong   value: return (ulong)   ((double) ((ulong)   b! - value) * progress);
            case short   value: return (short)   ((double) ((short)   b! - value) * progress);
            case ushort  value: return (ushort)  ((double) ((ushort)  b! - value) * progress);
            case byte    value: return (byte)    ((double) ((byte)    b! - value) * progress);
            case sbyte   value: return (sbyte)   ((double) ((sbyte)   b! - value) * progress);
            case null         : return null;
          }

          // …  ⟶ Interpolate also between eligible class types e.g. `UnityEngine.Color`, `UnityEngine.Vector3`, …
          if ((interpolation.operators.subtraction = types.a.GetMethod("op_Subtraction", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, new[] {types.b, types.a}, null)) is not null)
          foreach (System.Reflection.MethodInfo method in types.a.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)) {
            System.Reflection.ParameterInfo[]            parameters = method.GetParameters();
            ref readonly System.Reflection.ParameterInfo multiplier = ref Util.Reference<System.Reflection.ParameterInfo>.At(parameters, 0u);
            ref readonly System.Reflection.ParameterInfo target     = ref Util.Reference<System.Reflection.ParameterInfo>.At(parameters, 1u);

            if (method.Name == "op_Multiply" && parameters.Length == 2 && (multiplier.ParameterType.GetElementType() ?? multiplier.ParameterType).IsAssignableFrom(interpolation.operators.subtraction.ReturnType) && (
              target.ParameterType == (interpolation.progress = typeof(double))  ||
              target.ParameterType == (interpolation.progress = typeof(float))   ||
              target.ParameterType == (interpolation.progress = typeof(decimal)) ||
              target.ParameterType == (interpolation.progress = typeof(int))     ||
              target.ParameterType == (interpolation.progress = typeof(nint))    ||
              target.ParameterType == (interpolation.progress = typeof(long))    ||
              target.ParameterType == (interpolation.progress = typeof(uint))    ||
              target.ParameterType == (interpolation.progress = typeof(nuint))   ||
              target.ParameterType == (interpolation.progress = typeof(ulong))   ||
              target.ParameterType == (interpolation.progress = typeof(short))   ||
              target.ParameterType == (interpolation.progress = typeof(ushort))  ||
              target.ParameterType == (interpolation.progress = typeof(byte))    ||
              target.ParameterType == (interpolation.progress = typeof(sbyte))
            )) {
              interpolation.operators.multiplication = method;
              UISequence.Interpolations.Add(types, (interpolation.operators, interpolation.progress));

              break;
            }
          }
        }

        return interpolation.operators.multiplication?.Invoke(null, new[] {interpolation.operators.subtraction?.Invoke(null, new[] {b, a}), System.Convert.ChangeType(progress, interpolation.progress)});
      }

      [PatchMethod(AggressiveInlining)] public void                                                                                      Reset                                                                                                         ()                                      => this.timestamp = UnityEngine.Time.realtimeSinceStartupAsDouble;
      [PatchMethod(AggressiveInlining)] System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, object?>> System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object?>>.GetEnumerator()                                      => new UISequence.Enumerator(this);
      [PatchMethod(AggressiveInlining)] bool                                                                                             System.Collections.Generic.IReadOnlyDictionary<string, object?>.ContainsKey                                   (string       key)                      => base.Contains(key);
      [PatchMethod(AggressiveInlining)] bool                                                                                             System.Collections.Generic.IReadOnlyDictionary<string, object?>.TryGetValue                                   (string       key,   out object? value) { if (base.Contains(key)) { value = this[key]; return true; } value = default; return false; }
      [PatchMethod(AggressiveInlining)] void                                                                                             System.Collections.ICollection.CopyTo                                                                         (System.Array array, int         index) { foreach (System.Collections.Generic.KeyValuePair<string, object?> property in this) array.SetValue(property, index++); }
      [PatchMethod(AggressiveInlining)] void                                                                                             System.Collections.IDictionary.Add                                                                            (object       key,   object?     value) => throw new System.NotSupportedException("UI sequence does not support adding entries; Use `UISequence::Add(…)` method");
      [PatchMethod(AggressiveInlining)] void                                                                                             System.Collections.IDictionary.Clear                                                                          ()                                      => this.keyframes.Clear();
      [PatchMethod(AggressiveInlining)] bool                                                                                             System.Collections.IDictionary.Contains                                                                       (object key)                            => base.Contains(key);
      [PatchMethod(AggressiveInlining)] System.Collections.IDictionaryEnumerator                                                         System.Collections.IDictionary.GetEnumerator                                                                  ()                                      => new UISequence.Enumerator(this);
      [PatchMethod(AggressiveInlining)] void                                                                                             System.Collections.IDictionary.Remove                                                                         (object key)                            => throw new System.NotSupportedException("UI sequence does not support removing entries");
      [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator                                                                   System.Collections.IEnumerable.GetEnumerator                                                                  ()                                      => new UISequence.Enumerator(this);

      public new object? this[string property] { get {
        System.Collections.Specialized.ListDictionary begin = base.begin!;
        System.Collections.Specialized.ListDictionary end   = base.end!;

        // … ⟶ Simultaneous (un-)boxing is minimally slow 🐢
        if (begin.Contains(property) && end.Contains(property)) {
          double elapsed  = UnityEngine.Time.realtimeSinceStartupAsDouble - this.timestamp;
          double progress = (elapsed - this.delay) / this.duration;

          // …
          foreach (System.Collections.Generic.KeyValuePair<double, PatchOdyssey.Animation.UIKeyframe> keyframe in this.keyframes)
          if (keyframe.Value.Contains(property)) {
            if (keyframe.Key <= progress) { begin = keyframe.Value; continue; }
            if (keyframe.Key >= progress) { end   = keyframe.Value; continue; }
          }

          progress = this.delay <= elapsed ? 0.0 : this.duration <= elapsed - this.delay ? 1.0 : this.easing((elapsed - this.delay) / this.duration);
          return this.interpolator(progress, begin[property], end[property]);
        }

        return null;
      } }

      object? System.Collections.Generic.IReadOnlyDictionary<string, object?>.this[string property] => this[property];
      object? System.Collections.IDictionary.this                                 [object property] { get => this[property]; set => this[property] = value; }
    }
  }

  namespace Collections {
    public sealed class EventHandler<T> : PatchOdyssey.Collections.RefList<PatchOdyssey.Collections.HandlerInfo<T>>, PatchOdyssey.Collections.IRefReadOnlyEquatable<EventHandler<T>>, System.ICloneable where T : PatchOdyssey.Events, new() /* ⟶ `event` @ `https://web.archive.org/web/20220923174214/https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/event` */ {
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  EventHandler()                                                                               : base(1u)                                                                                 {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  EventHandler(EventHandler<T> events)                                                         : this((PatchOdyssey.Collections.RefList<PatchOdyssey.Collections.HandlerInfo<T>>) events) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] private EventHandler(PatchOdyssey.Collections.RefList<PatchOdyssey.Collections.HandlerInfo<T>> list) : base(list)                                                                               {}

      /* … ⟶ Keep `𝑓 Add(…)`, `𝑓 AddRange(…)`, `𝑓 Append(…)`, `𝑓 Clear(…)`, `𝑓 Insert(…)`, `𝑓 InsertRange(…)`, `𝑓 Prepend(…)`, `𝑓 Remove(…)`, `𝑓 RemoveAll(…)`, `𝑓 RemoveAt(…)`, `𝑓 RemoveRange(…)` accessible for multicast queuing, “privately” inherit `class System.Collections.Generic.List` methods */
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public   new               void                                                                                 Add                                                                   (in PatchOdyssey.Collections.HandlerInfo<T>                                      handler)                                                                                                                                                                                                                  { if (PatchOdyssey.Collections.HandlerInfo<T>.DefaultValue != handler.value) base.Add(in handler); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public   new               void                                                                                 AddRange                                                              (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.HandlerInfo<T>> handlers)                                                                                                                                                                                                                 { foreach (PatchOdyssey.Collections.HandlerInfo<T> handler in handlers)      this.Add(in handler); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public   new ref           readonly PatchOdyssey.Collections.HandlerInfo<T>                                     Append                                                                (in PatchOdyssey.Collections.HandlerInfo<T>                                      handler)                                                                                                                                                                                                                  => ref (PatchOdyssey.Collections.HandlerInfo<T>.DefaultValue != handler.value ? ref base.Append(in handler) : ref handler);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               EventHandler<T>                                                                      AsCopy                                                                ()                                                                                                                                                                                                                                                                                                         => new(this);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               EventHandler<T>                                                                      AsReadOnly                                                            ()                                                                                                                                                                                                                                                                                                         =>     this;
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               int                                                                                  BinarySearch                                                          (in PatchOdyssey.Collections.HandlerInfo<T> handler)                                                                                                                                                                                                                                                       =>     base.BinarySearch(in handler);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] private  new               int                                                                                  BinarySearch                                                          (in PatchOdyssey.Collections.HandlerInfo<T> handler, PatchOdyssey.Collections.IRefReadOnlyComparer<PatchOdyssey.Collections.HandlerInfo<T>>? comparer)                                                                                                                                                     =>     base.BinarySearch(in handler, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               int                                                                                  BinarySearch                                                          (in PatchOdyssey.Collections.HandlerInfo<T> handler, System.Collections.Generic.IComparer<PatchOdyssey.Collections.HandlerInfo<T>>?          comparer)                                                                                                                                                     =>     base.BinarySearch(in handler, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] private  new               int                                                                                  BinarySearch                                                          (uint                                       index,   uint                                                                                    length, in PatchOdyssey.Collections.HandlerInfo<T> handler, PatchOdyssey.Collections.IRefReadOnlyComparer<PatchOdyssey.Collections.HandlerInfo<T>>? comparer) =>     base.BinarySearch(index, length, in handler, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               int                                                                                  BinarySearch                                                          (uint                                       index,   uint                                                                                    length, in PatchOdyssey.Collections.HandlerInfo<T> handler, System.Collections.Generic.IComparer         <PatchOdyssey.Collections.HandlerInfo<T>>? comparer) =>     base.BinarySearch(index, length, in handler, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public   new               void                                                                                 Clear                                                                 ()                                                                                                                                                                                                                                                                                                         =>     base.Clear       ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public   new /* virtual */ object                                                                               Clone                                                                 ()                                                                                                                                                                                                                                                                                                         =>     base.Clone       ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public   static            void                                                                                 Combine                                                               (EventHandler                           <T>                                                                                events, in PatchOdyssey.Collections.HandlerInfo<T> handler)                                                                                                                     { if (PatchOdyssey.Collections.HandlerInfo<T>.DefaultValue != handler.value) events.Add(in handler); } // ⟶ Based on `𝑓 System.Delegate.Combine(…)`
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               bool                                                                                 Contains                                                              (in PatchOdyssey.Collections.HandlerInfo<T>                                                                                handler)                                                                                                                                                                        =>     base.Contains(in handler);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public                     uint                                                                                 CountInvocationList                                                   ()                                                                                                                                                                                                                                                                                                         =>     base.Count;
      [PatchMethod(AggressiveInlining), PatchResolution(2)] private  new               PatchOdyssey.Collections.RefList<U>                                                  ConvertAll<U>                                                         (PatchOdyssey.RefConverter              <PatchOdyssey.Collections.HandlerInfo<T>, U>                                       converter) where U : PatchOdyssey.Events, new()                                                                                                                                 =>     base.ConvertAll(converter);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] private  new               PatchOdyssey.Collections.RefList<U>                                                  ConvertAll<U>                                                         (PatchOdyssey.RefReadOnlyConverter      <PatchOdyssey.Collections.HandlerInfo<T>, U>                                       converter) where U : PatchOdyssey.Events, new()                                                                                                                                 =>     base.ConvertAll(converter);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               PatchOdyssey.Collections.RefList<U>                                                  ConvertAll<U>                                                         (System.Converter                       <PatchOdyssey.Collections.HandlerInfo<T>, U>                                       converter) where U : PatchOdyssey.Events, new()                                                                                                                                 =>     base.ConvertAll(converter);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] private                    EventHandler<U>                                                                      ConvertAll<U>                                                         (PatchOdyssey.RefConverter              <PatchOdyssey.Collections.HandlerInfo<T>, PatchOdyssey.Collections.HandlerInfo<U>> converter) where U : PatchOdyssey.Events, new()                                                                                                                                 => new(base.ConvertAll(converter));
      [PatchMethod(AggressiveInlining), PatchResolution(1)] private                    EventHandler<U>                                                                      ConvertAll<U>                                                         (PatchOdyssey.RefReadOnlyConverter      <PatchOdyssey.Collections.HandlerInfo<T>, PatchOdyssey.Collections.HandlerInfo<U>> converter) where U : PatchOdyssey.Events, new()                                                                                                                                 => new(base.ConvertAll(converter));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private                    EventHandler<U>                                                                      ConvertAll<U>                                                         (System.Converter                       <PatchOdyssey.Collections.HandlerInfo<T>, PatchOdyssey.Collections.HandlerInfo<U>> converter) where U : PatchOdyssey.Events, new()                                                                                                                                 => new(base.ConvertAll(converter));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               void                                                                                 CopyTo                                                                (PatchOdyssey.Collections.HandlerInfo<T>[]                                                                                 array)                                                                                                                                                                          =>     base.CopyTo    (array);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               void                                                                                 CopyTo                                                                (PatchOdyssey.Collections.HandlerInfo<T>[]                                                                                 array, uint                                      index)                                                                                                                         =>     base.CopyTo    (array, index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               void                                                                                 CopyTo                                                                (uint                                                                                                                      index, PatchOdyssey.Collections.HandlerInfo<T>[] array, uint arrayIndex, uint length)                                                                                           =>     base.CopyTo    (index, array, arrayIndex, length);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public                     void                                                                                 DynamicInvoke                                                         (params object?[]?                                                                                                         arguments)                                                                                                                                                                      { using (PatchOdyssey.Collections.RefList<PatchOdyssey.Collections.HandlerInfo<T>>.Enumerator enumerator = this.GetEnumerator()) while (enumerator.MoveNext()) enumerator.Current.DynamicInvoke(); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public                     bool                                                                                 Equals                                                                (in EventHandler<T>                                                                                                        events)                                                                                                                                                                         =>     base.Equals       ((PatchOdyssey.Collections.RefList<PatchOdyssey.Collections.HandlerInfo<T>>) events);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public   override          bool                                                                                 Equals                                                                (object?                                                                                                                   value)                                                                                                                                                                          =>     base.Equals       (value);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] private  new               bool                                                                                 Exists                                                                (PatchOdyssey.RefPredicate        <PatchOdyssey.Collections.HandlerInfo<T>>                                                predicate)                                                                                                                                                                      =>     base.Exists       (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] private  new               bool                                                                                 Exists                                                                (PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>>                                                predicate)                                                                                                                                                                      =>     base.Exists       (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               bool                                                                                 Exists                                                                (System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>>                                                predicate)                                                                                                                                                                      =>     base.Exists       (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] private  new               ref PatchOdyssey.Collections.HandlerInfo<T>                                          Find                                                                  (PatchOdyssey.RefPredicate        <PatchOdyssey.Collections.HandlerInfo<T>>                                                predicate)                                                                                                                                                                      => ref base.Find         (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] private  new               ref PatchOdyssey.Collections.HandlerInfo<T>                                          Find                                                                  (PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>>                                                predicate)                                                                                                                                                                      => ref base.Find         (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               ref PatchOdyssey.Collections.HandlerInfo<T>                                          Find                                                                  (System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>>                                                predicate)                                                                                                                                                                      => ref base.Find         (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] private  new               EventHandler<T>                                                                      FindAll                                                               (PatchOdyssey.RefPredicate        <PatchOdyssey.Collections.HandlerInfo<T>>                                                predicate)                                                                                                                                                                      => new(base.FindAll      (predicate));
      [PatchMethod(AggressiveInlining), PatchResolution(2)] private  new               EventHandler<T>                                                                      FindAll                                                               (PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>>                                                predicate)                                                                                                                                                                      => new(base.FindAll      (predicate));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               EventHandler<T>                                                                      FindAll                                                               (System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>>                                                predicate)                                                                                                                                                                      => new(base.FindAll      (predicate));
      [PatchMethod(AggressiveInlining), PatchResolution(1)] private  new               int                                                                                  FindIndex                                                             (PatchOdyssey.RefPredicate        <PatchOdyssey.Collections.HandlerInfo<T>>                                                predicate)                                                                                                                                                                      =>     base.FindIndex    (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] private  new               int                                                                                  FindIndex                                                             (PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>>                                                predicate)                                                                                                                                                                      =>     base.FindIndex    (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               int                                                                                  FindIndex                                                             (System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>>                                                predicate)                                                                                                                                                                      =>     base.FindIndex    (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] private  new               int                                                                                  FindIndex                                                             (uint                                                                                                                      index,              PatchOdyssey.RefPredicate        <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                       =>     base.FindIndex    (index,         predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] private  new               int                                                                                  FindIndex                                                             (uint                                                                                                                      index,              PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                       =>     base.FindIndex    (index,         predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               int                                                                                  FindIndex                                                             (uint                                                                                                                      index,              System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                       =>     base.FindIndex    (index,         predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] private  new               int                                                                                  FindIndex                                                             (uint                                                                                                                      index, uint length, PatchOdyssey.RefPredicate        <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                       =>     base.FindIndex    (index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] private  new               int                                                                                  FindIndex                                                             (uint                                                                                                                      index, uint length, PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                       =>     base.FindIndex    (index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               int                                                                                  FindIndex                                                             (uint                                                                                                                      index, uint length, System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                       =>     base.FindIndex    (index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] private  new               ref PatchOdyssey.Collections.HandlerInfo<T>                                          FindLast                                                              (PatchOdyssey.RefPredicate        <PatchOdyssey.Collections.HandlerInfo<T>>                                                predicate)                                                                                                                                                                      => ref base.FindLast     (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] private  new               ref PatchOdyssey.Collections.HandlerInfo<T>                                          FindLast                                                              (PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>>                                                predicate)                                                                                                                                                                      => ref base.FindLast     (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               ref PatchOdyssey.Collections.HandlerInfo<T>                                          FindLast                                                              (System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>>                                                predicate)                                                                                                                                                                      => ref base.FindLast     (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] private  new               int                                                                                  FindLastIndex                                                         (PatchOdyssey.RefPredicate        <PatchOdyssey.Collections.HandlerInfo<T>>                                                predicate)                                                                                                                                                                      =>     base.FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] private  new               int                                                                                  FindLastIndex                                                         (PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>>                                                predicate)                                                                                                                                                                      =>     base.FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               int                                                                                  FindLastIndex                                                         (System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>>                                                predicate)                                                                                                                                                                      =>     base.FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] private  new               int                                                                                  FindLastIndex                                                         (uint                                                                                                                      index,              PatchOdyssey.RefPredicate        <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                       =>     base.FindLastIndex(index,         predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] private  new               int                                                                                  FindLastIndex                                                         (uint                                                                                                                      index,              PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                       =>     base.FindLastIndex(index,         predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               int                                                                                  FindLastIndex                                                         (uint                                                                                                                      index,              System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                       =>     base.FindLastIndex(index,         predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] private  new               int                                                                                  FindLastIndex                                                         (uint                                                                                                                      index, uint length, PatchOdyssey.RefPredicate        <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                       =>     base.FindLastIndex(index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] private  new               int                                                                                  FindLastIndex                                                         (uint                                                                                                                      index, uint length, PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                       =>     base.FindLastIndex(index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               int                                                                                  FindLastIndex                                                         (uint                                                                                                                      index, uint length, System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                       =>     base.FindLastIndex(index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] private  new               void                                                                                 ForEach                                                               (PatchOdyssey.RefAction        <PatchOdyssey.Collections.HandlerInfo<T>>                                                   action)                                                                                                                                                                         =>     base.ForEach      (action);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] private  new               void                                                                                 ForEach                                                               (PatchOdyssey.RefReadOnlyAction<PatchOdyssey.Collections.HandlerInfo<T>>                                                   action)                                                                                                                                                                         =>     base.ForEach      (action);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               void                                                                                 ForEach                                                               (System.Action                 <PatchOdyssey.Collections.HandlerInfo<T>>                                                   action)                                                                                                                                                                         =>     base.ForEach      (action);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               PatchOdyssey.Collections.RefList<PatchOdyssey.Collections.HandlerInfo<T>>.Enumerator GetEnumerator                                                         ()                                                                                                                                                                                                                                                                                                         =>     base.GetEnumerator();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public   override          int                                                                                  GetHashCode                                                           ()                                                                                                                                                                                                                                                                                                         =>     base.GetHashCode  ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public   /* virtual */     System.Delegate[]                                                                    GetInvocationList                                                     ()                                                                                                                                                                                                                                                                                                         =>     base.ConvertAll([PatchMethod(AggressiveInlining)] static (in PatchOdyssey.Collections.HandlerInfo<T> handler) => (System.Delegate) handler.Invoke).Items; // ⟶ `Util.Array<PatchOdyssey.Collections.RefReadOnlyList<System.Delegate>>.From(…)`
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               EventHandler<T>                                                                      GetRange                                                              (uint                                       index, uint length)                                                                                                                                                                                                                                            => new(base.GetRange(index, length));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               int                                                                                  IndexOf                                                               (in PatchOdyssey.Collections.HandlerInfo<T> handler)                                                                                                                                                                                                                                                       =>     base.IndexOf (in handler);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               int                                                                                  IndexOf                                                               (in PatchOdyssey.Collections.HandlerInfo<T> handler, uint                                                                            index)                                                                                                                                                                =>     base.IndexOf (in handler, index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               int                                                                                  IndexOf                                                               (in PatchOdyssey.Collections.HandlerInfo<T> handler, uint                                                                            index, uint length)                                                                                                                                                   =>     base.IndexOf (in handler, index, length);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] internal new               void                                                                                 Insert                                                                (uint                                       index,   in PatchOdyssey.Collections.HandlerInfo<T>                                      handler)                                                                                                                                                              { if (PatchOdyssey.Collections.HandlerInfo<T>.DefaultValue != handler.value) base.Insert(index, in handler); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] internal new               void                                                                                 InsertRange                                                           (uint                                       index,   System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.HandlerInfo<T>> handlers)                                                                                                                                                             { foreach (PatchOdyssey.Collections.HandlerInfo<T> handler in handlers) { if (PatchOdyssey.Collections.HandlerInfo<T>.DefaultValue != handler.value) this.Insert(index++, in handler); } }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public                     void                                                                                 Invoke                                                                ()                                                                                                                                                                                                                                                                                                         { using (PatchOdyssey.Collections.RefList<PatchOdyssey.Collections.HandlerInfo<T>>.Enumerator enumerator = this.GetEnumerator()) while (enumerator.MoveNext()) enumerator.Current.Invoke(); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               int                                                                                  LastIndexOf                                                           (in PatchOdyssey.Collections.HandlerInfo<T>                                       handler)                                                                                                                                                                                                                 => base.LastIndexOf(in handler);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               int                                                                                  LastIndexOf                                                           (in PatchOdyssey.Collections.HandlerInfo<T>                                       handler, uint index)                                                                                                                                                                                                     => base.LastIndexOf(in handler, index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               int                                                                                  LastIndexOf                                                           (in PatchOdyssey.Collections.HandlerInfo<T>                                       handler, uint index, uint length)                                                                                                                                                                                        => base.LastIndexOf(in handler, index, length);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public   new               ref readonly PatchOdyssey.Collections.HandlerInfo<T>                                 Prepend                                                               (in PatchOdyssey.Collections.HandlerInfo<T>                                       handler)                                                                                                                                                                                                                 => ref (PatchOdyssey.Collections.HandlerInfo<T>.DefaultValue != handler.value ? ref base.Prepend(in handler) : ref handler);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public   new               bool                                                                                 Remove                                                                (in PatchOdyssey.Collections.HandlerInfo<T>                                       handler)                                                                                                                                                                                                                 => base.Remove(in handler);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public   static            void                                                                                 Remove                                                                (EventHandler                           <T>                                       events, in PatchOdyssey.Collections.HandlerInfo<T> handler)                                                                                                                                                              { for (uint index = events.Count; 0u != index--; ) if (events[index].value == handler.value) { events.RemoveAt(index); return; } } // ⟶ Based on `𝑓 System.Delegate.Remove(…)`
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public   new               uint                                                                                 RemoveAll                                                             (PatchOdyssey.RefPredicate              <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                                                                                                                               => base.RemoveAll  (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public   new               uint                                                                                 RemoveAll                                                             (PatchOdyssey.RefReadOnlyPredicate      <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                                                                                                                               => base.RemoveAll  (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public   new               uint                                                                                 RemoveAll                                                             (System.Predicate                       <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                                                                                                                               => base.RemoveAll  (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public   new               void                                                                                 RemoveAt                                                              (uint                                                                             index)                                                                                                                                                                                                                   => base.RemoveAt   (index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public   new               void                                                                                 RemoveRange                                                           (uint                                                                             index, uint length)                                                                                                                                                                                                      => base.RemoveRange(index, length);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               void                                                                                 Reverse                                                               ()                                                                                                                                                                                                                                                                                                         => base.Reverse    ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               void                                                                                 Reverse                                                               (uint index, uint length)                                                                                                                                                                                                                                                                                  => base.Reverse    (index, length);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               void                                                                                 Sort                                                                  ()                                                                                                                                                                                                                                                                                                         => base.Sort       ();
      [PatchMethod(AggressiveInlining), PatchResolution(1)] private  new               void                                                                                 Sort                                                                  (PatchOdyssey.RefComparison          <PatchOdyssey.Collections.HandlerInfo<T>>? comparison)                                                                                                                                                                                                                => base.Sort       (comparison);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] private  new               void                                                                                 Sort                                                                  (PatchOdyssey.RefReadOnlyComparison  <PatchOdyssey.Collections.HandlerInfo<T>>? comparison)                                                                                                                                                                                                                => base.Sort       (comparison);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               void                                                                                 Sort                                                                  (System.Collections.Generic.IComparer<PatchOdyssey.Collections.HandlerInfo<T>>? comparer)                                                                                                                                                                                                                  => base.Sort       (comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               void                                                                                 Sort                                                                  (System.Comparison                   <PatchOdyssey.Collections.HandlerInfo<T>>? comparison)                                                                                                                                                                                                                => base.Sort       (comparison);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] private  new               void                                                                                 Sort                                                                  (uint                                                                           index, uint length, PatchOdyssey.Collections.IRefComparer        <PatchOdyssey.Collections.HandlerInfo<T>>? comparer)                                                                                                      => base.Sort       (index, length, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] private  new               void                                                                                 Sort                                                                  (uint                                                                           index, uint length, PatchOdyssey.Collections.IRefReadOnlyComparer<PatchOdyssey.Collections.HandlerInfo<T>>? comparer)                                                                                                      => base.Sort       (index, length, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               void                                                                                 Sort                                                                  (uint                                                                           index, uint length, System.Collections.Generic.IComparer         <PatchOdyssey.Collections.HandlerInfo<T>>? comparer)                                                                                                      => base.Sort       (index, length, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               PatchOdyssey.Collections.HandlerInfo<T>[]                                            ToArray                                                               ()                                                                                                                                                                                                                                                                                                         => base.ToArray    ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               void                                                                                 TrimExcess                                                            ()                                                                                                                                                                                                                                                                                                         => base.TrimExcess ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               void                                                                                 TrimExcess                                                            (uint                                                                       capacity)                                                                                                                                                                                                                      => base.TrimExcess (capacity);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] private  new               bool                                                                                 TrueForAll                                                            (PatchOdyssey.RefPredicate        <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                                                                                                                                     => base.TrueForAll (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] private  new               bool                                                                                 TrueForAll                                                            (PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                                                                                                                                     => base.TrueForAll (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private  new               bool                                                                                 TrueForAll                                                            (System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                                                                                                                                     => base.TrueForAll (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                                                                            PatchOdyssey.Collections.IRefReadOnlyEquatable<EventHandler<T>>.Equals(in EventHandler                  <T>                                       events)                                                                                                                                                                                                                        => this.Equals     (in events);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] object                                                                                                          System.ICloneable.Clone                                               ()                                                                                                                                                                                                                                                                                                         => base.Clone      ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                                                                            System.IEquatable<EventHandler<T>>.Equals                             (EventHandler<T> events)                                                                                                                                                                                                                                                                                   => this.Equals     (events);

      /* … */
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public static EventHandler<T> operator +(EventHandler<T> events, in PatchOdyssey.Collections.HandlerInfo<T> handler) { EventHandler<T>.Combine(events, in handler); return events; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public static EventHandler<T> operator -(EventHandler<T> events, in PatchOdyssey.Collections.HandlerInfo<T> handler) { EventHandler<T>.Remove (events, in handler); return events; }

      [PatchMethod(AggressiveInlining), PatchResolution(0)]
      public static implicit operator System.Delegate(in EventHandler<T> events) => events.Invoke;
    }

    public struct GameObjectEnumerator : PatchOdyssey.Collections.IRefEquatable<GameObjectEnumerator>, /* ⟶ Ranged `foreach …` shorthand support */ System.Collections.Generic.IEnumerable<UnityEngine.GameObject>, System.Collections.Generic.IEnumerator<UnityEngine.GameObject> {
      private enum HierarchyCurrent : sbyte { Unmoved = -1, Pending,     Moved };
      public  enum Kind             : byte  { Children,     Descendants, Hierarchy };

      public                 UnityEngine.GameObject                                           Current                                                                => (GameObjectEnumerator.HierarchyCurrent.Pending == this.uproot ? this.root : (UnityEngine.Transform) this.subenumerator.Current).gameObject;
      private                readonly GameObjectEnumerator.Kind                               kind                                                                   =  default;
      private                readonly System.Collections.Generic.Queue<UnityEngine.Transform> hierarchy                                                              =  new();
      private                readonly UnityEngine.Transform                                   root                                                                   =  null!;
      private /* required */          System.Collections.IEnumerator                          subenumerator                                                          =  null!;
      private /* required */          GameObjectEnumerator.HierarchyCurrent                   uproot                                                                 =  GameObjectEnumerator.HierarchyCurrent.Unmoved;
      UnityEngine.GameObject                                                                  System.Collections.Generic.IEnumerator<UnityEngine.GameObject>.Current => this.Current;
      object                                                                                  System.Collections.IEnumerator.Current                                 => this.Current!;

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)]
      internal GameObjectEnumerator(GameObjectEnumerator.Kind kind, UnityEngine.Transform gameObjectTransform) {
        this.kind          = kind;
        this.root          = gameObjectTransform;
        this.subenumerator = gameObjectTransform.GetEnumerator();

        switch (this.kind) {
          case GameObjectEnumerator.Kind.Children   :                                                                                                                    break;
          case GameObjectEnumerator.Kind.Descendants: this.hierarchy = new(gameObjectTransform.childCount);                                                              break;
          case GameObjectEnumerator.Kind.Hierarchy  : this.hierarchy = new(gameObjectTransform.childCount); this.uproot = GameObjectEnumerator.HierarchyCurrent.Unmoved; break;
        }
      }

      /* … */
      [PatchMethod(AggressiveInlining)] public void                 Dispose      ()                                   { /* Do nothing… */ }
      [PatchMethod(AggressiveInlining)] public bool                 Equals       (in GameObjectEnumerator enumerator) => false;
      [PatchMethod(AggressiveInlining)] public GameObjectEnumerator GetEnumerator()                                   => this;

      public bool MoveNext() {
        switch (kind) {
          case GameObjectEnumerator.Kind.Children:
            return this.subenumerator.MoveNext();

          case GameObjectEnumerator.Kind.Descendants:
            if (this.subenumerator.MoveNext()) {
              this.hierarchy.Enqueue((UnityEngine.Transform) this.subenumerator.Current);
              return true;
            }

            do {
              if (this.hierarchy.IsEmpty())
              return false;

              this.subenumerator = this.hierarchy.Dequeue().GetEnumerator();
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

      [PatchMethod(AggressiveInlining)]
      public void Reset() {
        this.hierarchy.Clear();
        this.subenumerator = this.root.GetEnumerator(); // ⟶ `System.Collections.IEnumerator::Reset()` unneeded
        this.uproot        = GameObjectEnumerator.HierarchyCurrent.Unmoved;
      }

      [PatchMethod(AggressiveInlining)] bool                                                           PatchOdyssey.Collections.IRefEquatable<GameObjectEnumerator>.Equals         (ref GameObjectEnumerator enumerator) => this.Equals       (in enumerator);
      [PatchMethod(AggressiveInlining)] bool                                                           PatchOdyssey.Collections.IRefReadOnlyEquatable<GameObjectEnumerator>.Equals (in  GameObjectEnumerator enumerator) => this.Equals       (in enumerator);
      [PatchMethod(AggressiveInlining)] System.Collections.Generic.IEnumerator<UnityEngine.GameObject> System.Collections.Generic.IEnumerable<UnityEngine.GameObject>.GetEnumerator()                                    => this.GetEnumerator();
      [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator                                 System.Collections.IEnumerable.GetEnumerator                                ()                                    => this.GetEnumerator();
      [PatchMethod(AggressiveInlining)] bool                                                           System.Collections.IEnumerator.MoveNext                                     ()                                    => this.MoveNext     ();
      [PatchMethod(AggressiveInlining)] void                                                           System.Collections.IEnumerator.Reset                                        ()                                    => this.Reset        ();
      [PatchMethod(AggressiveInlining)] void                                                           System.IDisposable.Dispose                                                  ()                                    => this.Dispose      ();
      [PatchMethod(AggressiveInlining)] bool                                                           System.IEquatable<GameObjectEnumerator>.Equals                              (GameObjectEnumerator enumerator)     => this.Equals       (enumerator);
    }

    public struct HandlerInfo<T> : PatchOdyssey.Collections.IRefReadOnlyEquatable<HandlerInfo<T>> where T : PatchOdyssey.Events, new() /* ⟶ Based on `System.MulticastDelegate` */ {
      internal readonly       PatchOdyssey.Handler<T> value    = HandlerInfo<T>.DefaultValue; // ⟶ Callback function(s)
      internal readonly       object?                 target   = null;                        // ⟶ Callback source
      internal /* readonly */ T                       metadata = default!;                    // ⟶ Callback event data

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)] public HandlerInfo(in HandlerInfo<T>       handler) : this(handler.value, handler.target, handler.metadata) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public HandlerInfo(PatchOdyssey.Handler<T> value, object? target, in T metadata)                            { this.metadata = metadata; this.target = target; this.value = value ?? this.value; }

      /* … */
      [PatchMethod(AggressiveInlining)] internal /* virtual */ object            Clone                                                                ()                                    => new HandlerInfo<T>((PatchOdyssey.Handler<T>) this.value.Clone(), this.target, this.metadata);
      [PatchMethod(NoInlining)]         internal static        void              DefaultValue                                                         (object?           target, in T data) {}
      [PatchMethod(AggressiveInlining)] internal               void              DynamicInvoke                                                        (params object?[]? arguments)         => this.value.DynamicInvoke(new object?[] {this.target, this.metadata});
      [PatchMethod(AggressiveInlining)] public                 bool              Equals                                                               (in HandlerInfo<T> handler)           => handler.target == this.target && handler.value == this.value && System.Collections.Generic.EqualityComparer<T>.Default.Equals(this.metadata, handler.metadata);
      [PatchMethod(AggressiveInlining)] public   override      bool              Equals                                                               (object?           value)             => value is not null && value switch { HandlerInfo<T> handler => this.Equals(handler), _ => this.GetHashCode() == value!.GetHashCode() };
      [PatchMethod(AggressiveInlining)] public   override      int               GetHashCode                                                          ()                                    => System.HashCode.Combine(this.metadata, this.target, this.value);
      [PatchMethod(AggressiveInlining)] internal /* virtual */ System.Delegate[] GetInvocationList                                                    ()                                    => this.value.GetInvocationList();
      [PatchMethod(AggressiveInlining)] internal               void              Invoke                                                               ()                                    => this.value.Invoke(this.target, in this.metadata);
      [PatchMethod(AggressiveInlining)] bool                                     PatchOdyssey.Collections.IRefReadOnlyEquatable<HandlerInfo<T>>.Equals(in HandlerInfo<T> handler)           => this.Equals(in handler);
      [PatchMethod(AggressiveInlining)] bool                                     System.IEquatable<HandlerInfo<T>>.Equals                             (HandlerInfo   <T> handler)           => this.Equals(handler);

      /* … */
      [PatchMethod(AggressiveInlining)]
      public static implicit operator PatchOdyssey.Handler<T>(in HandlerInfo<T> handler) => handler.value;
    }

    public interface IMono {
      public abstract bool    HasValue { get; }
      public abstract object? Value    { get; }
    }

    internal struct Index : PatchOdyssey.Collections.IRefComparable<Index>, PatchOdyssey.Collections.IRefEquatable<Index>, System.IConvertible {
      internal uint value;

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)]
      public Index(uint value = default) => this.value = value;

      /* … */
      [PatchMethod(AggressiveInlining)] public static   ulong                             BigMul                                                          (in Index indexA, in Index indexB)                                                                                                                                             => (ulong) System.Math.BigMul((int) indexA.value, (int) indexB.value);
      [PatchMethod(AggressiveInlining)] public static   Index                             Clamp                                                           (in Index index,  in Index minimum, in Index maximum)                                                                                                                          => new    (System.Math.Clamp (index.value, minimum.value, maximum.value));
      [PatchMethod(AggressiveInlining)] public          int                               CompareTo                                                       (in Index index)                                                                                                                                                               => this.value.CompareTo(index.value);
      [PatchMethod(AggressiveInlining)] public          int                               CompareTo                                                       (object?  value)                                                                                                                                                               => this.value.CompareTo(value);
      [PatchMethod(AggressiveInlining)] public static   (Index Quotient, Index Remainder) DivRem                                                          (in Index indexA, in Index indexB)                                                                                                                                             { long quotient = System.Math.DivRem((long) indexA.value, (long) indexB.value, out long remainder); return (new((uint) quotient), new((uint) remainder)); }
      [PatchMethod(AggressiveInlining)] public          bool                              Equals                                                          (in Index index)                                                                                                                                                               => this.value.Equals     (index.value);
      [PatchMethod(AggressiveInlining)] public override bool                              Equals                                                          (object?  value)                                                                                                                                                               => this.value.Equals     (value);
      [PatchMethod(AggressiveInlining)] public override int                               GetHashCode                                                     ()                                                                                                                                                                             => this.value.GetHashCode();
      [PatchMethod(AggressiveInlining)] public          System.TypeCode                   GetTypeCode                                                     ()                                                                                                                                                                             => System.Type.GetTypeCode(typeof(Index));
      [PatchMethod(AggressiveInlining)] public static   bool                              IsEvenInteger                                                   (in Index                     index)                                                                                                                                           => 0 == (index.value & 1);
      [PatchMethod(AggressiveInlining)] public static   bool                              IsOddInteger                                                    (in Index                     index)                                                                                                                                           => 1 == (index.value & 1);
      [PatchMethod(AggressiveInlining)] public static   bool                              IsPow2                                                          (in Index                     index)                                                                                                                                           { Index logarithm = Index.Log2(index); uint exponent = 0u; while (0u != logarithm.value--) { exponent = 0u != exponent ? exponent << 1 : 2u; } return exponent == index.value; }
      [PatchMethod(AggressiveInlining)] public static   byte                              LeadingZeroCount                                                (in Index                     index)                                                                                                                                           { byte n = 32; uint x = index.value, y; y = x >>> 16; if (0u != y) { n -= 16; x = y; } y = x >>> 8; if (0u != y) { n -= 8; x = y; } y = x >>> 4; if (0u != y) { n -= 4; x = y; } y = x >>> 2; if (0u != y) { n -= 2; x = y; } y = x >>> 1; if (0u != y) { return (byte) (n - 2u); } return (byte) (n - x); }
      [PatchMethod(AggressiveInlining)] public static   Index                             Log2                                                            (in Index                     index)                                                                                                                                           { uint logarithm = 0u; for (uint exponent = index.value; 0u != exponent; exponent >>>= 1) { ++logarithm; } return new(logarithm); }
      [PatchMethod(AggressiveInlining)] public static   ref readonly Index                Max                                                             (in Index                     indexA, in Index indexB)                                                                                                                         => ref (indexB.value > indexA.value ? ref indexB : ref indexA);
      [PatchMethod(AggressiveInlining)] public static   ref readonly Index                Min                                                             (in Index                     indexA, in Index indexB)                                                                                                                         => ref (indexB.value < indexA.value ? ref indexB : ref indexA);
      [PatchMethod(AggressiveInlining)] public static   Index                             Parse                                                           (string                       text)                                                                                                                                            => new((uint) long.Parse(text));
      [PatchMethod(AggressiveInlining)] public static   Index                             Parse                                                           (string                       text,       System.Globalization.NumberStyles style)                                                                                             => new((uint) long.Parse(text, style));
      [PatchMethod(AggressiveInlining)] public static   Index                             Parse                                                           (string                       text,       System.IFormatProvider?           provider)                                                                                          => new((uint) long.Parse(text, provider));
      [PatchMethod(AggressiveInlining)] public static   Index                             Parse                                                           (in System.ReadOnlySpan<byte> bytes,      System.IFormatProvider?           provider)                                                                                          => new       (uint.Parse(System.Runtime.InteropServices.MemoryMarshal.Cast<byte, char>(bytes), System.Globalization.NumberStyles.Integer, provider));
      [PatchMethod(AggressiveInlining)] public static   Index                             Parse                                                           (in System.ReadOnlySpan<char> characters, System.IFormatProvider?           provider)                                                                                          => new       (uint.Parse(characters,                                                           System.Globalization.NumberStyles.Integer, provider));
      [PatchMethod(AggressiveInlining)] public static   Index                             Parse                                                           (string                       text,       System.Globalization.NumberStyles style,                                             System.IFormatProvider? provider)               => new((uint) long.Parse(text,                                                                 style,                                     provider));
      [PatchMethod(AggressiveInlining)] public static   Index                             Parse                                                           (in System.ReadOnlySpan<byte> bytes,      System.Globalization.NumberStyles style = System.Globalization.NumberStyles.Integer, System.IFormatProvider? provider = null)        => new(       uint.Parse(System.Runtime.InteropServices.MemoryMarshal.Cast<byte, char>(bytes), style,                                     provider));
      [PatchMethod(AggressiveInlining)] public static   Index                             Parse                                                           (in System.ReadOnlySpan<char> characters, System.Globalization.NumberStyles style = System.Globalization.NumberStyles.Integer, System.IFormatProvider? provider = null)        => new       (uint.Parse(characters,                                                           style,                                     provider));
      [PatchMethod(AggressiveInlining)] public static   byte                              PopCount                                                        (in Index                     index)                                                                                                                                           { byte count = 0; for (uint value = index.value; 0u != value; value >>>= 1) { if (0 != (value & 1)) ++count; } return count; }
      [PatchMethod(AggressiveInlining)] public static   Index                             RotateLeft                                                      (in Index                     index, uint count)                                                                                                                               => (index.value <<  (int) count) | (index.value >>> (int) (32u - count));
      [PatchMethod(AggressiveInlining)] public static   Index                             RotateRight                                                     (in Index                     index, uint count)                                                                                                                               => (index.value >>> (int) count) | (index.value <<  (int) (32u - count));
      [PatchMethod(AggressiveInlining)] public static   int                               Sign                                                            (in Index                     index)                                                                                                                                           => 0u == index.value ? 0 : 1;
      [PatchMethod(AggressiveInlining)] public override string                            ToString                                                        ()                                                                                                                                                                             => this.value.ToString();
      [PatchMethod(AggressiveInlining)] public          string                            ToString                                                        (string?                      format)                                                                                                                                          => this.value.ToString(format);
      [PatchMethod(AggressiveInlining)] public          string                            ToString                                                        (System.IFormatProvider?      provider)                                                                                                                                        => this.value.ToString(provider);
      [PatchMethod(AggressiveInlining)] public          string                            ToString                                                        (string?                      format, System.IFormatProvider? provider)                                                                                                        => this.value.ToString(format, provider);
      [PatchMethod(AggressiveInlining)] public static   byte                              TrailingZeroCount                                               (in Index                     index)                                                                                                                                           { byte n = 31; uint x = index.value, y; if (0u == x) return 32; y = x << 16; if (0u != y) { n -= 16; x = y; } y = x << 8; if (0u != y) { n -= 8; x = y; } y = x << 4; if (0u != y) { n -= 4; x = y; } y = x << 2; if (0u != y) { n -= 2; x = y; } return (byte) (n - ((x << 1) >>> 31)); }
      [PatchMethod(AggressiveInlining)] public          bool                              TryFormat                                                       (in System.Span<byte>         bytes,       out uint                          count, in System.ReadOnlySpan<char> format = default, System.IFormatProvider? provider = default) { bool formatted = this.value.TryFormat(System.Runtime.InteropServices.MemoryMarshal.Cast<byte, char>(bytes), out int subcount, format, provider);                               count  = (uint) subcount;       return formatted; }
      [PatchMethod(AggressiveInlining)] public          bool                              TryFormat                                                       (in System.Span<char>         characters,  out uint                          count, in System.ReadOnlySpan<char> format = default, System.IFormatProvider? provider = default) { bool formatted = this.value.TryFormat(characters,                                                           out int subcount, format, provider);                               count  = (uint) subcount;       return formatted; }
      [PatchMethod(AggressiveInlining)] public static   bool                              TryParse                                                        (string?                      text,        out Index                         result)                                                                                           { bool parsed    = long.TryParse(text,                                                                 System.Globalization.NumberStyles.Integer, null,     out long subresult); result = new((uint) subresult); return parsed; }
      [PatchMethod(AggressiveInlining)] public static   bool                              TryParse                                                        (in System.ReadOnlySpan<byte> bytes,       out Index                         result)                                                                                           { bool parsed    = uint.TryParse(System.Runtime.InteropServices.MemoryMarshal.Cast<byte, char>(bytes), System.Globalization.NumberStyles.Integer, null,     out uint subresult); result = new(subresult);        return parsed; }
      [PatchMethod(AggressiveInlining)] public static   bool                              TryParse                                                        (in System.ReadOnlySpan<char> characters,  out Index                         result)                                                                                           { bool parsed    = uint.TryParse(characters,                                                           System.Globalization.NumberStyles.Integer, null,     out uint subresult); result = new(subresult);        return parsed; }
      [PatchMethod(AggressiveInlining)] public static   bool                              TryParse                                                        (string?                      text,        System.IFormatProvider?           provider, out Index               result)                                                         { bool parsed    = uint.TryParse(text,                                                                 System.Globalization.NumberStyles.Integer, provider, out uint subresult); result = new(subresult);        return parsed; }
      [PatchMethod(AggressiveInlining)] public static   bool                              TryParse                                                        (in System.ReadOnlySpan<byte> bytes,       System.IFormatProvider?           provider, out Index               result)                                                         { bool parsed    = uint.TryParse(System.Runtime.InteropServices.MemoryMarshal.Cast<byte, char>(bytes), System.Globalization.NumberStyles.Integer, provider, out uint subresult); result = new(subresult);        return parsed; }
      [PatchMethod(AggressiveInlining)] public static   bool                              TryParse                                                        (in System.ReadOnlySpan<char> characters,  System.IFormatProvider?           provider, out Index               result)                                                         { bool parsed    = uint.TryParse(characters,                                                           System.Globalization.NumberStyles.Integer, provider, out uint subresult); result = new(subresult);        return parsed; }
      [PatchMethod(AggressiveInlining)] public static   bool                              TryParse                                                        (string?                      text,        System.Globalization.NumberStyles style,    System.IFormatProvider? provider, out Index result)                                     { bool parsed    = long.TryParse(text,                                                                 style,                                     provider, out long subresult); result = new((uint) subresult); return parsed; }
      [PatchMethod(AggressiveInlining)] public static   bool                              TryParse                                                        (in System.ReadOnlySpan<byte> bytes,       System.Globalization.NumberStyles style,    System.IFormatProvider? provider, out Index result)                                     { bool parsed    = uint.TryParse(System.Runtime.InteropServices.MemoryMarshal.Cast<byte, char>(bytes), style,                                     provider, out uint subresult); result = new(subresult);        return parsed; }
      [PatchMethod(AggressiveInlining)] public static   bool                              TryParse                                                        (in System.ReadOnlySpan<char> characters,  System.Globalization.NumberStyles style,    System.IFormatProvider? provider, out Index result)                                     { bool parsed    = uint.TryParse(characters,                                                           style,                                     provider, out uint subresult); result = new(subresult);        return parsed; }
      [PatchMethod(AggressiveInlining)] int                                               PatchOdyssey.Collections.IRefComparable<Index>.CompareTo        (ref Index                    index)                                                                                                                                           => this.value.CompareTo  (index.value);
      [PatchMethod(AggressiveInlining)] int                                               PatchOdyssey.Collections.IRefReadOnlyComparable<Index>.CompareTo(in  Index                    index)                                                                                                                                           => this.value.CompareTo  (index.value);
      [PatchMethod(AggressiveInlining)] bool                                              PatchOdyssey.Collections.IRefEquatable<Index>.Equals            (ref Index                    index)                                                                                                                                           => this.value.Equals     (index.value);
      [PatchMethod(AggressiveInlining)] bool                                              PatchOdyssey.Collections.IRefReadOnlyEquatable<Index>.Equals    (in  Index                    index)                                                                                                                                           => this.value.Equals     (index.value);
      [PatchMethod(AggressiveInlining)] int                                               System.IComparable.CompareTo                                    (object?                      index)                                                                                                                                           => this.value.CompareTo  (index);
      [PatchMethod(AggressiveInlining)] int                                               System.IComparable<Index>.CompareTo                             (Index                        index)                                                                                                                                           => this.value.CompareTo  (index.value);
      [PatchMethod(AggressiveInlining)] System.TypeCode                                   System.IConvertible.GetTypeCode                                 ()                                                                                                                                                                             => this.value.GetTypeCode();
      [PatchMethod(AggressiveInlining)] bool                                              System.IConvertible.ToBoolean                                   (System.IFormatProvider? provider)                                                                                                                                             => 0u != this.value ? true : false;
      [PatchMethod(AggressiveInlining)] byte                                              System.IConvertible.ToByte                                      (System.IFormatProvider? provider)                                                                                                                                             => (byte) this.value;
      [PatchMethod(AggressiveInlining)] char                                              System.IConvertible.ToChar                                      (System.IFormatProvider? provider)                                                                                                                                             => (char) this.value;
      [PatchMethod(AggressiveInlining)] System.DateTime                                   System.IConvertible.ToDateTime                                  (System.IFormatProvider? provider)                                                                                                                                             => new(this.value);
      [PatchMethod(AggressiveInlining)] decimal                                           System.IConvertible.ToDecimal                                   (System.IFormatProvider? provider)                                                                                                                                             => (decimal) this.value;
      [PatchMethod(AggressiveInlining)] double                                            System.IConvertible.ToDouble                                    (System.IFormatProvider? provider)                                                                                                                                             => (double)  this.value;
      [PatchMethod(AggressiveInlining)] short                                             System.IConvertible.ToInt16                                     (System.IFormatProvider? provider)                                                                                                                                             => (short)   this.value;
      [PatchMethod(AggressiveInlining)] int                                               System.IConvertible.ToInt32                                     (System.IFormatProvider? provider)                                                                                                                                             => (int)     this.value;
      [PatchMethod(AggressiveInlining)] long                                              System.IConvertible.ToInt64                                     (System.IFormatProvider? provider)                                                                                                                                             => (long)    this.value;
      [PatchMethod(AggressiveInlining)] sbyte                                             System.IConvertible.ToSByte                                     (System.IFormatProvider? provider)                                                                                                                                             => (sbyte)   this.value;
      [PatchMethod(AggressiveInlining)] float                                             System.IConvertible.ToSingle                                    (System.IFormatProvider? provider)                                                                                                                                             => (float)   this.value;
      [PatchMethod(AggressiveInlining)] string                                            System.IConvertible.ToString                                    (System.IFormatProvider? provider)                                                                                                                                             => this.ToString();
      [PatchMethod(AggressiveInlining)] object                                            System.IConvertible.ToType                                      (System.Type             type, System.IFormatProvider? provider)                                                                                                               => System.Convert.ChangeType(this.value, type, provider);
      [PatchMethod(AggressiveInlining)] ushort                                            System.IConvertible.ToUInt16                                    (System.IFormatProvider? provider)                                                                                                                                             => (ushort) this.value;
      [PatchMethod(AggressiveInlining)] uint                                              System.IConvertible.ToUInt32                                    (System.IFormatProvider? provider)                                                                                                                                             => (uint)   this.value;
      [PatchMethod(AggressiveInlining)] ulong                                             System.IConvertible.ToUInt64                                    (System.IFormatProvider? provider)                                                                                                                                             => (ulong)  this.value;
      [PatchMethod(AggressiveInlining)] bool                                              System.IEquatable<Index>.Equals                                 (Index                   index)                                                                                                                                                => this.value.Equals(index.value);

      [PatchMethod(AggressiveInlining)] public static Index operator +  (in Index index)                   => new((uint) +index.value);
      [PatchMethod(AggressiveInlining)] public static Index operator -  (in Index index)                   => new((uint) -index.value);
      [PatchMethod(AggressiveInlining)] public static bool  operator !  (in Index index)                   => 0u == index.value;
      [PatchMethod(AggressiveInlining)] public static Index operator ~  (in Index index)                   => new((uint) ~index.value);
      [PatchMethod(AggressiveInlining)] public static Index operator ++ (in Index index)                   => new(index.value + 1u);
      [PatchMethod(AggressiveInlining)] public static Index operator -- (in Index index)                   => new(index.value - 1u);
      [PatchMethod(AggressiveInlining)] public static Index operator +  (in Index indexA, in Index indexB) => new        (indexA.value +         indexB.value);
      [PatchMethod(AggressiveInlining)] public static Index operator -  (in Index indexA, in Index indexB) => new        (indexA.value -         indexB.value);
      [PatchMethod(AggressiveInlining)] public static Index operator *  (in Index indexA, in Index indexB) => new        (indexA.value *         indexB.value);
      [PatchMethod(AggressiveInlining)] public static Index operator /  (in Index indexA, in Index indexB) => new        (indexA.value /         indexB.value);
      [PatchMethod(AggressiveInlining)] public static Index operator %  (in Index indexA, in Index indexB) => new        (indexA.value %         indexB.value);
      [PatchMethod(AggressiveInlining)] public static Index operator &  (in Index indexA, in Index indexB) => new((uint) (indexA.value &   (int) indexB.value));
      [PatchMethod(AggressiveInlining)] public static Index operator |  (in Index indexA, in Index indexB) => new        (indexA.value |         indexB.value);
      [PatchMethod(AggressiveInlining)] public static Index operator ^  (in Index indexA, in Index indexB) => new((uint) (indexA.value ^   (int) indexB.value));
      [PatchMethod(AggressiveInlining)] public static Index operator << (in Index indexA, in Index indexB) => new        (indexA.value <<  (int) indexB.value);
      [PatchMethod(AggressiveInlining)] public static Index operator >> (in Index indexA, in Index indexB) => new        (indexA.value >>  (int) indexB.value);
      [PatchMethod(AggressiveInlining)] public static Index operator >>>(in Index indexA, in Index indexB) => new        (indexA.value >>> (int) indexB.value);
      [PatchMethod(AggressiveInlining)] public static bool  operator == (in Index indexA, in Index indexB) =>             indexA.value ==        indexB.value;
      [PatchMethod(AggressiveInlining)] public static bool  operator != (in Index indexA, in Index indexB) =>             indexA.value !=        indexB.value;
      [PatchMethod(AggressiveInlining)] public static bool  operator <  (in Index indexA, in Index indexB) =>             indexA.value <         indexB.value;
      [PatchMethod(AggressiveInlining)] public static bool  operator >  (in Index indexA, in Index indexB) =>             indexA.value >         indexB.value;
      [PatchMethod(AggressiveInlining)] public static bool  operator <= (in Index indexA, in Index indexB) =>             indexA.value <=        indexB.value;
      [PatchMethod(AggressiveInlining)] public static bool  operator >= (in Index indexA, in Index indexB) =>             indexA.value >=        indexB.value;

      [PatchMethod(AggressiveInlining)] public static implicit operator Index(uint  value) => new(value);
      [PatchMethod(AggressiveInlining)] public static implicit operator uint (Index index) => index.value;
    }

    public interface IRefComparable<T> : PatchOdyssey.Collections.IRefReadOnlyComparable<T> {
      public int CompareTo(ref T value);
    }

    public interface IRefComparer<T> : PatchOdyssey.Collections.IRefReadOnlyComparer<T>, System.Collections.Generic.IComparer<T>, System.Collections.IComparer {
      public int Compare(ref T a, ref T b);
    }

    public interface IRefEqualityComparer<T> : PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<T>, System.Collections.Generic.IEqualityComparer<T>, System.Collections.IEqualityComparer {
      public bool Equals     (ref T a, ref T b);
      public int  GetHashCode(ref T value);
    }

    public interface IRefEquatable<T> : PatchOdyssey.Collections.IRefReadOnlyEquatable<T> {
      public bool Equals(ref T value);
    }

    public interface IRefReadOnlyComparable<T> : System.IComparable<T>, System.IComparable {
      public int CompareTo(in T value);
    }

    public interface IRefReadOnlyComparer<T> : System.Collections.Generic.IComparer<T>, System.Collections.IComparer {
      public int Compare(in T a, in T b);
    }

    public interface IRefReadOnlyEqualityComparer<T> : System.Collections.Generic.IEqualityComparer<T>, System.Collections.IEqualityComparer {
      public bool Equals     (in T a, in T b);
      public int  GetHashCode(in T value);
    }

    public interface IRefReadOnlyEquatable<T> : System.IEquatable<T> {
      public bool Equals(in T value);
    }

    internal struct LoadInfo : PatchOdyssey.Collections.IRefEquatable<LoadInfo> {
      internal object?                                                              cached  = null;
      internal PatchOdyssey.Collections.EventHandler<PatchOdyssey.Events.LoadEvent> events  = new();
      internal object?                                                              payload = null;

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)]
      public LoadInfo() {}

      /* … */
      [PatchMethod(AggressiveInlining)] public bool Equals                                                         (in  LoadInfo load) => load.events == this.events;
      [PatchMethod(AggressiveInlining)] bool        PatchOdyssey.Collections.IRefEquatable<LoadInfo>.Equals        (ref LoadInfo load) => this.Equals(in load);
      [PatchMethod(AggressiveInlining)] bool        PatchOdyssey.Collections.IRefReadOnlyEquatable<LoadInfo>.Equals(in  LoadInfo load) => this.Equals(in load);
      [PatchMethod(AggressiveInlining)] bool        System.IEquatable<LoadInfo>.Equals                             (LoadInfo     load) => this.Equals(load);
    }

    public readonly struct Mono<T> : PatchOdyssey.Collections.IMono, PatchOdyssey.Collections.IRefEquatable<Mono<T>> /* ⟶ Based on `System.Nullable<T>` */ {
      public  readonly bool HasValue                                =  false;
      public  readonly T    Value                                   =  default!;
      bool                  PatchOdyssey.Collections.IMono.HasValue => this.HasValue;
      object?               PatchOdyssey.Collections.IMono.Value    => this.HasValue ? this.Value : null;

      /* … */
      [PatchMethod(AggressiveInlining)] public  Mono()           {}
      [PatchMethod(AggressiveInlining)] private Mono(in T value) { this.HasValue = true; this.Value = value; }

      /* … */
      [PatchMethod(AggressiveInlining)] public          bool       Equals                                                        (in Mono<T> mono)  => mono.HasValue == this.HasValue && PatchOdyssey.Collections.RefEqualityComparer<T>.Default.Equals(mono.Value, this.Value);
      [PatchMethod(AggressiveInlining)] public override bool       Equals                                                        (object?    value) => value is not null && value switch { Mono<T> mono => this.Equals(mono), PatchOdyssey.Collections.IMono mono => object.Equals(mono.Value, ((PatchOdyssey.Collections.IMono) this).Value), _ => object.Equals(this.Value, value) };
      [PatchMethod(AggressiveInlining)] public override int        GetHashCode                                                   ()                 => this.HasValue ? this.Value!.GetHashCode() : base.GetHashCode();
      [PatchMethod(AggressiveInlining)] public          readonly T GetValueOrDefault                                             ()                 => this.GetValueOrDefault(default!);
      [PatchMethod(AggressiveInlining)] public          readonly T GetValueOrDefault                                             (in T fallback)    => this.HasValue ? this.Value : fallback;
      [PatchMethod(AggressiveInlining)] public override string?    ToString                                                      ()                 => this.HasValue ? this.Value!.ToString() : string.Empty;
      [PatchMethod(AggressiveInlining)] bool                       PatchOdyssey.Collections.IRefEquatable<Mono<T>>.Equals        (ref Mono<T> mono) => this.Equals(in mono);
      [PatchMethod(AggressiveInlining)] bool                       PatchOdyssey.Collections.IRefReadOnlyEquatable<Mono<T>>.Equals(in  Mono<T> mono) => this.Equals(in mono);
      [PatchMethod(AggressiveInlining)] bool                       System.IEquatable<Mono<T>>.Equals                             (Mono    <T> mono) => this.Equals(mono);

      [PatchMethod(AggressiveInlining)] public static Mono<T> operator +(in Mono<T> mono, in T value) => mono.HasValue ? mono : new(value);

      [PatchMethod(AggressiveInlining)] public static explicit operator T      (in Mono<T> mono)  => mono.Value;
      [PatchMethod(AggressiveInlining)] public static implicit operator Mono<T>(in T       value) => new(value);
    }

    public /* sealed */ class RefComparer<T> : PatchOdyssey.Collections.RefReadOnlyComparer<T>, PatchOdyssey.Collections.IRefComparer<T> {
      private readonly struct Sentinel : PatchOdyssey.Collections.IRefComparable<Sentinel> {
        int PatchOdyssey.Collections.IRefComparable<Sentinel>.CompareTo        (ref Sentinel value) => default;
        int PatchOdyssey.Collections.IRefReadOnlyComparable<Sentinel>.CompareTo(in  Sentinel value) => default;
        int System.IComparable.CompareTo                                       (object?      value) => default;
        int System.IComparable<Sentinel>.CompareTo                             (Sentinel     value) => default;
      }

      /* … */
      private static readonly PatchOdyssey.RefComparison<T> CompareValue = (PatchOdyssey.RefComparison<T>) (
        typeof(PatchOdyssey.Collections.IRefComparable        <T>).IsAssignableFrom(typeof(T)) ? ((PatchOdyssey.RefComparison<RefComparer<T>.Sentinel>) RefComparer<RefComparer<T>.Sentinel>.RefComparableCompare)        .Method.GetGenericMethodDefinition().MakeGenericMethod(typeof(T)).CreateDelegate(typeof(PatchOdyssey.RefComparison<T>)) :
        typeof(PatchOdyssey.Collections.IRefReadOnlyComparable<T>).IsAssignableFrom(typeof(T)) ? ((PatchOdyssey.RefComparison<RefComparer<T>.Sentinel>) RefComparer<RefComparer<T>.Sentinel>.RefReadOnlyComparableCompare).Method.GetGenericMethodDefinition().MakeGenericMethod(typeof(T)).CreateDelegate(typeof(PatchOdyssey.RefComparison<T>)) :
        typeof(System.IComparable                             <T>).IsAssignableFrom(typeof(T)) ? ((PatchOdyssey.RefComparison<RefComparer<T>.Sentinel>) RefComparer<RefComparer<T>.Sentinel>.ComparableCompare)           .Method.GetGenericMethodDefinition().MakeGenericMethod(typeof(T)).CreateDelegate(typeof(PatchOdyssey.RefComparison<T>)) :
        typeof(System.IComparable)                                .IsAssignableFrom(typeof(T)) ? ((PatchOdyssey.RefComparison<RefComparer<T>.Sentinel>) RefComparer<RefComparer<T>.Sentinel>.ComparableCompare2)          .Method.GetGenericMethodDefinition().MakeGenericMethod(typeof(T)).CreateDelegate(typeof(PatchOdyssey.RefComparison<T>)) :
        (PatchOdyssey.RefComparison<T>) RefComparer<T>.ObjectCompare<T>
      );
      protected        new readonly PatchOdyssey.RefComparison<T> comparison       = RefComparer<T>.CompareValue;
      public    static new          RefComparer               <T> Default { get; } = new();

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)] public    RefComparer()                                                 : base()           {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] protected RefComparer(PatchOdyssey.RefComparison        <T> comparison) : base()           => this.comparison = comparison;
      [PatchConstructor, PatchMethod(AggressiveInlining)] protected RefComparer(PatchOdyssey.RefReadOnlyComparison<T> comparison) : base(comparison) {}

      /* … */
      [PatchMethod(AggressiveInlining)] private          static int            ComparableCompare <U>                                   (ref U                                 a, ref U b) where U : System.IComparable<U>                                => a .CompareTo(b);
      [PatchMethod(AggressiveInlining)] private          static int            ComparableCompare2<U>                                   (ref U                                 a, ref U b) where U : System.IComparable                                   => a!.CompareTo(b);
      [PatchMethod(AggressiveInlining)] public  virtual         int            Compare                                                 (ref T                                 a, ref T b)                                                                { int comparison = this.comparison(ref a, ref b); return PatchOdyssey.Collections.RefEqualityComparer<T>.Default.Equals(ref a, ref b) ? 0 : comparison; }
      [PatchMethod(AggressiveInlining)] public  override        int            Compare                                                 (T                                     a, T     b)                                                                => a is null ? (b is null ? 0 : -1) : b is null ? (a is null ? 0 : +1) : this.RefReadOnlyCompare(in a, in b);
      [PatchMethod(AggressiveInlining)] public           static RefComparer<T> Create                                                  (PatchOdyssey.RefComparison        <T> comparison)                                                                => new(comparison);
      [PatchMethod(AggressiveInlining)] public  new      static RefComparer<T> Create                                                  (PatchOdyssey.RefReadOnlyComparison<T> comparison)                                                                => new(comparison);
      [PatchMethod(AggressiveInlining)] private          static int            ObjectCompare               <U>                         (ref U                                 a, ref U   b)                                                              => System.Collections.Comparer.Default.Compare(a, b);
      [PatchMethod(AggressiveInlining)] private          static int            RefComparableCompare        <U>                         (ref U                                 a, ref U   b) where U : PatchOdyssey.Collections.IRefComparable        <U> => a.CompareTo(ref b);
      [PatchMethod(AggressiveInlining)] private          static int            RefReadOnlyComparableCompare<U>                         (ref U                                 a, ref U   b) where U : PatchOdyssey.Collections.IRefReadOnlyComparable<U> => a.CompareTo(in  b);
      [PatchMethod(AggressiveInlining)] private                 int            RefReadOnlyCompare                                      (in  T                                 a, in  T   b)                                                              { if (RefComparer<T>.CompareValue != this.comparison) throw new System.NotSupportedException("Reference comparer can not compare as modifiable references"); return base.Compare(in a, in b); }
      [PatchMethod(AggressiveInlining)] int                                    PatchOdyssey.Collections.IRefComparer<T>.Compare        (ref T                                 a, ref T   b)                                                              => this.Compare           (ref a, ref b);
      [PatchMethod(AggressiveInlining)] int                                    PatchOdyssey.Collections.IRefReadOnlyComparer<T>.Compare(in  T                                 a, in  T   b)                                                              => this.RefReadOnlyCompare(in  a, in  b);
      [PatchMethod(AggressiveInlining)] int                                    System.Collections.Generic.IComparer<T>.Compare         (T?                                    a, T?      b)                                                              => a is null ? (b is null ? 0 : -1) : b is null ? (a is null ? 0 : +1) : this.RefReadOnlyCompare(in a!, in b!);
      [PatchMethod(AggressiveInlining)] int                                    System.Collections.IComparer.Compare                    (object?                               a, object? b)                                                              => a is null ? (b is null ? 0 : -1) : b is null ? (a is null ? 0 : +1) : this.RefReadOnlyCompare((T) a, (T) b);
    }

    [System.Serializable]
    public class RefDictionary<TKey, TValue> : PatchOdyssey.Collections.IRefEquatable<RefDictionary<TKey, TValue>>, System.Collections.Generic.IDictionary<TKey, TValue>, System.Collections.ICollection, System.Collections.IDictionary /* ⟶ Based on `System.Collections.Generic.Dictionary<TKey, TValue>`; See `https://web.archive.org/web/20240722203244/https://discussions.unity.com/t/finally-a-serializable-dictionary-for-unity-extracted-from-system-collections-generic/586385` */ {
      public readonly struct AlternateLookup<TAlternateKey> /* ⟶ Based on `System.Collections.Generic.Dictionary<TKey, TValue>.AlternateLookup<TAlternateKey>` */ {
        public readonly RefDictionary<TKey,TValue> Dictionary;

        /* … */
        [PatchMethod(AggressiveInlining)]
        internal AlternateLookup(RefDictionary<TKey, TValue> dictionary) => this.Dictionary = dictionary;

        /* … */
        [PatchMethod(AggressiveInlining)] public bool ContainsKey(in TAlternateKey key)                                         => false;
        [PatchMethod(AggressiveInlining)] public bool Remove     (in TAlternateKey key)                                         => false;
        [PatchMethod(AggressiveInlining)] public bool Remove     (in TAlternateKey key, out TKey   actualKey, out TValue value) { actualKey = default!; value = default!; return false; }
        [PatchMethod(AggressiveInlining)] public bool TryAdd     (in TAlternateKey key, in TValue  value)                       => false;
        [PatchMethod(AggressiveInlining)] public bool TryGetValue(in TAlternateKey key, out TValue value)                       {                       value = default!; return false; }
        [PatchMethod(AggressiveInlining)] public bool TryGetValue(in TAlternateKey key, out TKey   actualKey, out TValue value) { actualKey = default!; value = default!; return false; }

        public ref TValue this[in TAlternateKey key] => ref Util.Reference<TValue>.Null;
      }

      public struct Enumerator : System.Collections.Generic.IEnumerator<PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>>, System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.IDictionaryEnumerator /* ⟶ Based on `System.Collections.Generic.Dictionary<TKey, TValue>.Enumerator` */ {
        private class Index { public uint value = 0u; }

        public                 ref PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>   Current => ref Util.Reference<PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>>.Only(this.current);
        private /* required */ PatchOdyssey.Collections.RefKeyValuePair    <TKey, TValue>[] current;
        private                readonly RefDictionary                      <TKey, TValue>   dictionary;
        private /* required */ Enumerator.Index                                             index;
        PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>                              System.Collections.Generic.IEnumerator<PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>>.Current => this.Current;
        System.Collections.Generic.KeyValuePair <TKey, TValue>                              System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Current  => this.Current;
        System.Collections.DictionaryEntry                                                  System.Collections.IDictionaryEnumerator.Entry                                                         => new(this.Current.Key, this.Current.Value);
        object                                                                              System.Collections.IDictionaryEnumerator.Key                                                           => this.Current.Key!;
        object?                                                                             System.Collections.IDictionaryEnumerator.Value                                                         => this.Current.Value;
        object                                                                              System.Collections.IEnumerator.Current                                                                 => this.Current;

        /* … */
        [PatchConstructor, PatchMethod(AggressiveInlining)]
        internal Enumerator(RefDictionary<TKey, TValue> dictionary) {
          this.current = null!; // ⟶ “error CS8618: NoN-nUlLaBlE fIeLd 'current' MuSt CoNtAiN a NoN-nUlL vAlUe WhEn ExItInG cOnStRuCtOr. CoNsIdEr AdDiNg ThE 'required' mOdIfIeR oR dEcLaRiNg ThE fIeLd As NuLlAbLe.”
          this.index   = null!; //    ^^

          this.Reset();
          this.dictionary = dictionary;
        }

        /* … */
        [PatchMethod(AggressiveInlining)]
        public void Dispose() { /* Do nothing… */ }

        [PatchMethod(AggressiveInlining)]
        public bool MoveNext() {
          for (; this.dictionary.count > this.index.value; ++this.index.value)
          if (this.dictionary.hashes[this.index.value] >= 0) {
            Util.Reference<PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>>.Only(this.current) = new(this.dictionary, this.index.value++);
            return true;
          }

          Util.Reference<PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>>.Only(this.current) = default;
          this.index.value                                                                          = this.dictionary.count;

          return false;
        }

        [PatchMethod(AggressiveInlining)] public void Reset                                  () { this.current = new PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>[] {default}; this.index = new(); }
        [PatchMethod(AggressiveInlining)] bool        System.Collections.IEnumerator.MoveNext() => this.MoveNext();
        [PatchMethod(AggressiveInlining)] void        System.Collections.IEnumerator.Reset   () => this.Reset   ();
        [PatchMethod(AggressiveInlining)] void        System.IDisposable.Dispose             () => this.Dispose ();
      }

      public /* sealed */ class KeyCollection : System.Collections.Generic.ICollection<TKey>, System.Collections.Generic.IReadOnlyCollection<TKey>, System.Collections.ICollection /* ⟶ Based on `System.Collections.Generic.Dictionary<TKey, TValue>.KeyCollection` */ {
        public struct Enumerator : System.Collections.Generic.IEnumerator<TKey> /* ⟶ Based on `System.Collections.Generic.Dictionary<TKey, TValue>.KeyCollection.Enumerator` */ {
          public  ref readonly TKey                                   Current => ref this.enumerator.Current.Key;
          private     readonly RefDictionary<TKey, TValue>.Enumerator enumerator;
          TKey                                                        System.Collections.Generic.IEnumerator<TKey>.Current => this.Current;
          object                                                      System.Collections.IEnumerator.Current               => this.Current!;

          /* … */
          [PatchConstructor, PatchMethod(AggressiveInlining)]
          internal Enumerator(RefDictionary<TKey, TValue> dictionary) => this.enumerator = dictionary.GetEnumerator();

          /* … */
          [PatchMethod(AggressiveInlining)] public void Dispose                                () => this.enumerator.Dispose ();
          [PatchMethod(AggressiveInlining)] public bool MoveNext                               () => this.enumerator.MoveNext();
          [PatchMethod(AggressiveInlining)] public void Reset                                  () => this.enumerator.Reset   ();
          [PatchMethod(AggressiveInlining)] bool        System.Collections.IEnumerator.MoveNext() => this           .MoveNext();
          [PatchMethod(AggressiveInlining)] void        System.Collections.IEnumerator.Reset   () => this           .Reset   ();
          [PatchMethod(AggressiveInlining)] void        System.IDisposable.Dispose             () => this           .Dispose ();
        }

        /* … */
        public           int                         Count                                                      => (int) this.dictionary.Count;
        private readonly RefDictionary<TKey, TValue> dictionary                                                 =  null!;
        bool                                         System.Collections.Generic.ICollection<TKey>.IsReadOnly    => true;
        int                                          System.Collections.Generic.IReadOnlyCollection<TKey>.Count => this.Count;
        int                                          System.Collections.ICollection.Count                       => this.Count;
        bool                                         System.Collections.ICollection.IsSynchronized              => false;
        object                                       System.Collections.ICollection.SyncRoot                    => this;

        /* … */
        [PatchMethod(AggressiveInlining)]
        public KeyCollection(RefDictionary<TKey, TValue> dictionary) => this.dictionary = dictionary;

        /* … */
        [PatchMethod(AggressiveInlining)] public bool                                  Contains                                                  (in TKey element)               => this.dictionary.ContainsKey(in element);
        [PatchMethod(AggressiveInlining)] public void                                  CopyTo                                                    (TKey[]  array, uint index)     { foreach (ref readonly TKey element in this) array[index++] = element; }
        [PatchMethod(AggressiveInlining)] public KeyCollection.Enumerator              GetEnumerator                                             ()                              => new(this.dictionary);
        [PatchMethod(AggressiveInlining)] void                                         System.Collections.Generic.ICollection<TKey>.Add          (TKey element)                  => throw new System.NotSupportedException("Dictionary key collection is read-only");
        [PatchMethod(AggressiveInlining)] void                                         System.Collections.Generic.ICollection<TKey>.Clear        ()                              => throw new System.NotSupportedException("Dictionary key collection is read-only");
        [PatchMethod(AggressiveInlining)] bool                                         System.Collections.Generic.ICollection<TKey>.Contains     (TKey   element)                => this.Contains(in element);
        [PatchMethod(AggressiveInlining)] void                                         System.Collections.Generic.ICollection<TKey>.CopyTo       (TKey[] array, int index)       => this.CopyTo  (array, (uint) index);
        [PatchMethod(AggressiveInlining)] bool                                         System.Collections.Generic.ICollection<TKey>.Remove       (TKey   element)                => throw new System.NotSupportedException("Dictionary key collection is read-only");
        [PatchMethod(AggressiveInlining)] System.Collections.Generic.IEnumerator<TKey> System.Collections.Generic.IEnumerable<TKey>.GetEnumerator()                              => this.GetEnumerator();
        [PatchMethod(AggressiveInlining)] void                                         System.Collections.ICollection.CopyTo                     (System.Array array, int index) { foreach (ref readonly TKey element in this) array.SetValue(element, index++); }
        [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator               System.Collections.IEnumerable.GetEnumerator              ()                              => this.GetEnumerator();
      }

      public /* sealed */ class ValueCollection : System.Collections.Generic.ICollection<TValue>, System.Collections.Generic.IReadOnlyCollection<TValue>, System.Collections.ICollection /* ⟶ Based on `System.Collections.Generic.Dictionary<TKey, TValue>.ValueCollection` */ {
        public struct Enumerator : System.Collections.Generic.IEnumerator<TValue> /* ⟶ Based on `System.Collections.Generic.Dictionary<TKey, TValue>.ValueCollection.Enumerator` */ {
          public  ref      TValue                                 Current => ref this.enumerator.Current.Value;
          private readonly RefDictionary<TKey, TValue>.Enumerator enumerator;
          TValue                                                  System.Collections.Generic.IEnumerator<TValue>.Current => this.Current;
          object                                                  System.Collections.IEnumerator.Current                 => this.Current!;

          /* … */
          [PatchConstructor, PatchMethod(AggressiveInlining)]
          internal Enumerator(RefDictionary<TKey, TValue> dictionary) => this.enumerator = dictionary.GetEnumerator();

          /* … */
          [PatchMethod(AggressiveInlining)] public void Dispose                                () => this.enumerator.Dispose ();
          [PatchMethod(AggressiveInlining)] public bool MoveNext                               () => this.enumerator.MoveNext();
          [PatchMethod(AggressiveInlining)] public void Reset                                  () => this.enumerator.Reset   ();
          [PatchMethod(AggressiveInlining)] bool        System.Collections.IEnumerator.MoveNext() => this           .MoveNext();
          [PatchMethod(AggressiveInlining)] void        System.Collections.IEnumerator.Reset   () => this           .Reset   ();
          [PatchMethod(AggressiveInlining)] void        System.IDisposable.Dispose             () => this           .Dispose ();
        }

        /* … */
        public             int                                  Count                                                        => (int) this.dictionary.Count;
        protected internal readonly RefDictionary<TKey, TValue> dictionary                                                   =  null!;
        bool                                                    System.Collections.Generic.ICollection<TValue>.IsReadOnly    => true;
        int                                                     System.Collections.Generic.IReadOnlyCollection<TValue>.Count => this.Count;
        int                                                     System.Collections.ICollection.Count                         => this.Count;
        bool                                                    System.Collections.ICollection.IsSynchronized                => false;
        object                                                  System.Collections.ICollection.SyncRoot                      => this;

        /* … */
        [PatchMethod(AggressiveInlining)]
        public ValueCollection(RefDictionary<TKey, TValue> dictionary) => this.dictionary = dictionary;

        /* … */
        [PatchMethod(AggressiveInlining)] public bool                                    Contains                                                    (in TValue element)             => this.dictionary.ContainsValue(in element);
        [PatchMethod(AggressiveInlining)] public void                                    CopyTo                                                      (TValue[]  array, uint index)   { foreach (ref readonly TValue element in this) array[index++] = element; }
        [PatchMethod(AggressiveInlining)] public ValueCollection.Enumerator              GetEnumerator                                               ()                              => new(this.dictionary);
        [PatchMethod(AggressiveInlining)] void                                           System.Collections.Generic.ICollection<TValue>.Add          (TValue element)                => throw new System.NotSupportedException("Dictionary key collection is read-only");
        [PatchMethod(AggressiveInlining)] void                                           System.Collections.Generic.ICollection<TValue>.Clear        ()                              => throw new System.NotSupportedException("Dictionary key collection is read-only");
        [PatchMethod(AggressiveInlining)] bool                                           System.Collections.Generic.ICollection<TValue>.Contains     (TValue   element)              => this.Contains(in element);
        [PatchMethod(AggressiveInlining)] void                                           System.Collections.Generic.ICollection<TValue>.CopyTo       (TValue[] array, int index)     => this.CopyTo  (array, (uint) index);
        [PatchMethod(AggressiveInlining)] bool                                           System.Collections.Generic.ICollection<TValue>.Remove       (TValue   element)              => throw new System.NotSupportedException("Dictionary key collection is read-only");
        [PatchMethod(AggressiveInlining)] System.Collections.Generic.IEnumerator<TValue> System.Collections.Generic.IEnumerable<TValue>.GetEnumerator()                              => this.GetEnumerator();
        [PatchMethod(AggressiveInlining)] void                                           System.Collections.ICollection.CopyTo                       (System.Array array, int index) { foreach (ref readonly TValue element in this) array.SetValue(element, index++); }
        [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator                 System.Collections.IEnumerable.GetEnumerator                ()                              => this.GetEnumerator();
      }

      /* … */
      private const  uint                                                    CapacityMaximum = 0x7FEFFFFDu;
      private static readonly PatchOdyssey.Collections.RefReadOnlyList<uint> Primes          = new(new[] {3u, 7u, 11u, 17u, 23u, 29u, 37u, 47u, 59u, 71u, 89u, 107u, 131u, 163u, 197u, 239u, 293u, 353u, 431u, 521u, 631u, 761u, 919u, 1103u, 1327u, 1597u, 1931u, 2333u, 2801u, 3371u, 4049u, 4861u, 5839u, 7013u, 8419u, 10103u, 12143u, 14591u, 17519u, 21023u, 25229u, 30293u, 36353u, 43627u, 52361u, 62851u, 75431u, 90523u, 108631u, 130363u, 156437u, 187751u, 225307u, 270371u, 324449u, 389357u, 467237u, 560689u, 672827u, 807403u, 968897u, 1162687u, 1395263u, 1674319u, 2009191u, 2411033u, 2893249u, 3471899u, 4166287u, 4999559u, 5999471u, 7199369u});
      private const  uint                                                    PrimeMaximum    = 0x7FFFFFFFu;

      [UnityEngine.HideInInspector, UnityEngine.SerializeField] private  PatchOdyssey.Collections.RefList<int>               buckets                                                                                                  =  new();
      public                                                             uint                                                Capacity                                                                                                 => this.count;
      public                                                             PatchOdyssey.Collections.IRefEqualityComparer<TKey> Comparer { get; private init; }                                                                          =  PatchOdyssey.Collections.RefEqualityComparer<TKey>.Default;
      public                                                             uint                                                Count                                                                                                    => this.count - this.free.count;
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] private  uint                                                count                                                                                                    =  0u;
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] private  (uint count, int list)                              free                                                                                                     =  (0u, -1);
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] private  PatchOdyssey.Collections.RefList<int>               hashes                                                                                                   =  new();
      public                                                             RefDictionary<TKey, TValue>.KeyCollection           Keys                                                                                                     => new(this);
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] internal PatchOdyssey.Collections.RefList<TKey>              keys                                                                                                     =  new();
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] private  PatchOdyssey.Collections.RefList<int>               next                                                                                                     =  new();
      public                                                             RefDictionary<TKey, TValue>.ValueCollection         Values                                                                                                   => new(this);
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] internal PatchOdyssey.Collections.RefList<TValue>            values                                                                                                   =  new();
      bool                                                                                                                   System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.IsReadOnly => false;
      int                                                                                                                    System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Count      => ((int) this.Count);
      System.Collections.Generic.ICollection<TKey>                                                                           System.Collections.Generic.IDictionary<TKey, TValue>.Keys                                                => this.Keys;
      System.Collections.Generic.ICollection<TValue>                                                                         System.Collections.Generic.IDictionary<TKey, TValue>.Values                                              => this.Values;
      int                                                                                                                    System.Collections.ICollection.Count                                                                     => ((int) this.Count);
      bool                                                                                                                   System.Collections.ICollection.IsSynchronized                                                            => false;
      object                                                                                                                 System.Collections.ICollection.SyncRoot                                                                  => this;
      bool                                                                                                                   System.Collections.IDictionary.IsFixedSize                                                               => false;
      bool                                                                                                                   System.Collections.IDictionary.IsReadOnly                                                                => false;
      System.Collections.ICollection                                                                                         System.Collections.IDictionary.Keys                                                                      => this.Keys;
      System.Collections.ICollection                                                                                         System.Collections.IDictionary.Values                                                                    => this.Values;

      /* … ⟶ Availability of `System.Runtime.InteropServices.CollectionMarshal.GetValueRefOrNullRef(…)` would replace `RefDictionary<T…>`’s entire purpose */
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RefDictionary()                                                                                                                                                                                       : this(0u,                                                                                                          null!)    {}
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RefDictionary(uint                                                                                                          capacity)                                                                 : this(capacity,                                                                                                    null!)    {}
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RefDictionary(PatchOdyssey.Collections.IRefEqualityComparer<TKey>                                                           comparer)                                                                 : this(2u,                                                                                                          comparer) {}
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RefDictionary(System.Collections.Generic.IDictionary       <TKey, TValue>                                                   dictionary)                                                               : this((System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>>) dictionary, null!)    {}
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public RefDictionary(System.Collections.Generic.IEnumerable       <PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>>         enumerable)                                                               : this(enumerable,                                                                                                  null!)    {}
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public RefDictionary(System.Collections.Generic.IEnumerable       <PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>> enumerable)                                                               : this(enumerable,                                                                                                  null!)    {}
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RefDictionary(System.Collections.Generic.IEnumerable       <System.Collections.Generic.KeyValuePair<TKey, TValue>>          enumerable)                                                               : this(enumerable,                                                                                                  null!)    {}
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RefDictionary(System.Collections.Generic.IDictionary       <TKey, TValue>                                                   dictionary, PatchOdyssey.Collections.IRefEqualityComparer<TKey> comparer) : this((System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>>) dictionary, comparer) {}
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public RefDictionary(System.Collections.Generic.IEnumerable       <PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>>         enumerable, PatchOdyssey.Collections.IRefEqualityComparer<TKey> comparer) : this(enumerable is null ? 0u : Util.EnumerableCount(enumerable),                                                  comparer) => this.AddRange(enumerable!);
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public RefDictionary(System.Collections.Generic.IEnumerable       <PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>> enumerable, PatchOdyssey.Collections.IRefEqualityComparer<TKey> comparer) : this(enumerable is null ? 0u : Util.EnumerableCount(enumerable),                                                  comparer) => this.AddRange(enumerable!);
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RefDictionary(System.Collections.Generic.IEnumerable       <System.Collections.Generic.KeyValuePair<TKey, TValue>>          enumerable, PatchOdyssey.Collections.IRefEqualityComparer<TKey> comparer) : this(enumerable is null ? 0u : Util.EnumerableCount(enumerable),                                                  comparer) => this.AddRange(enumerable!);
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RefDictionary(uint                                                                                                          capacity,   PatchOdyssey.Collections.IRefEqualityComparer<TKey> comparer)                                                                                                                               { this.Initialize(capacity); this.Comparer = comparer ?? PatchOdyssey.Collections.RefEqualityComparer<TKey>.Default; }

      /* … */
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public void                                                         Add       (in PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>                                              element)              => this.Add(in element.Key, in element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                                         Add       (in System.Collections.Generic.KeyValuePair <TKey, TValue>                                              element)              => this.Add(element.Key,    element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                                         Add       (in TKey                                                                                                key, in TValue value) { if (key is not null) this.Insert(in key, in value, true); }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public void                                                         AddRange  (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>>         enumerable)           { foreach (PatchOdyssey.Collections.RefKeyValuePair        <TKey, TValue> element in enumerable) this.Add(in element); }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public void                                                         AddRange  (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>> enumerable)           { foreach (PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue> element in enumerable) this.Add((System.Collections.Generic.KeyValuePair<TKey, TValue>) element); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                                         AddRange  (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>          enumerable)           { foreach (System.Collections.Generic.KeyValuePair         <TKey, TValue> element in enumerable) this.Add(element); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public RefDictionary<TKey, TValue>                                  AsCopy    ()                                                                                                                            => new((System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>>) this, this.Comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public PatchOdyssey.Collections.RefReadOnlyDictionary<TKey, TValue> AsReadOnly()                                                                                                                            { RefDictionary<TKey, TValue> dictionary = this; return Util.Reinterpret<RefDictionary<TKey, TValue>, PatchOdyssey.Collections.RefReadOnlyDictionary<TKey, TValue>>(ref dictionary); } // ⟶ `Util.Reinterpret(…)` used to subvert the inverted design of `RefDictionary<…>` and `RefReadOnlyDictionary<…>` 😭

      [PatchMethod(AggressiveInlining)]
      public void Clear() {
        this.buckets.Fill (-1);
        this.hashes .Clear();
        this.keys   .Clear();
        this.next   .Clear();
        this.values .Clear();

        this.count = 0u;
        this.free  = (count: 0u, list: -1);
      }

      [PatchMethod(AggressiveInlining), PatchResolution(1)] public bool Contains      (in PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>         element)           { int index = this.FindIndex(element.Key); return index != -1 && PatchOdyssey.Collections.RefEqualityComparer        <TValue>.Default.Equals(ref this.values[(uint) index], ref element.Value); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public bool Contains      (in System.Collections.Generic.KeyValuePair <TKey, TValue>         element)           { int index = this.FindIndex(element.Key); return index != -1 && PatchOdyssey.Collections.RefReadOnlyEqualityComparer<TValue>.Default.Equals(in  this.values[(uint) index], element.Value); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public bool ContainsKey   (in TKey                                                           key)               => this.FindIndex(in key) != -1;
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public bool ContainsValue (in TValue                                                         value)             { PatchOdyssey.RefReadOnlyEqualityComparison<TValue> comparer = value is null ? ([PatchMethod(AggressiveInlining)] static (in TValue value, in TValue _) => value is null) : PatchOdyssey.Collections.RefReadOnlyEqualityComparer<TValue>.Default.Equals; for (uint index = this.count; 0u != index--; ) { if (this.hashes[index] >= 0 && comparer(in this.values[index], in value)) return true; } return false; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void CopyTo        (PatchOdyssey.Collections.RefKeyValuePair        <TKey, TValue>[]  array, uint index) { for (uint subindex = 0u; subindex != this.count; ++subindex) { if (this.hashes[subindex] >= 0) array[index++] = new(in this.keys[subindex], in this.values[subindex]); } }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void CopyTo        (PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>[]  array, uint index) { for (uint subindex = 0u; subindex != this.count; ++subindex) { if (this.hashes[subindex] >= 0) array[index++] = new(in this.keys[subindex], in this.values[subindex]); } }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void CopyTo        (System.Collections.Generic.KeyValuePair         <TKey, TValue>[]  array, uint index) { for (uint subindex = 0u; subindex != this.count; ++subindex) { if (this.hashes[subindex] >= 0) array[index++] = new(this.keys[subindex],     this.values[subindex]); } }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void EnsureCapacity(uint                                                              capacity)          { /* Do nothing… */ }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public bool Equals        (in RefDictionary<TKey, TValue>                                    dictionary)        => object.Equals(dictionary, this);

      [PatchMethod(AggressiveInlining), PatchResolution(0)]
      protected int FindIndex(in TKey key) {
        if (key is not null && !this.buckets.IsEmpty()) {
          int hash = this.Comparer.GetHashCode(in key) & (int) RefDictionary<TKey, TValue>.PrimeMaximum;

          // …
          for (int index = this.buckets[(uint) (hash % this.buckets.Count)]; index >= 0; index = this.next[(uint) index]) {
            if (hash == this.hashes[(uint) index] && this.Comparer.Equals(in this.keys[(uint) index], in key))
            return index;
          }
        }

        return -1;
      }

      [PatchMethod(AggressiveInlining), PatchResolution(0)] public RefDictionary<TKey, TValue>.AlternateLookup<TAlternateKey> GetAlternateLookup<TAlternateKey>() => new(this);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public RefDictionary<TKey, TValue>.Enumerator                     GetEnumerator                    () => new(this);

      [PatchMethod(AggressiveInlining), PatchResolution(0)]
      public static uint GetPrime(uint minimum) {
        [PatchMethod(AggressiveInlining)]
        static uint Sqrt(uint number) /* ⟶ `System.Math.Sqrt(…) `(or `System.Math.Sqrt(double)`) exists but is suboptimal for (unsigned) integer-only roots */{
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
        for (uint index = 0u, prime; index != (uint) RefDictionary<TKey, TValue>.Primes.Count; ++index) {
          if (minimum <= (prime = RefDictionary<TKey, TValue>.Primes[index]))
          return prime;
        }

        for (uint index = minimum | 1u; index < RefDictionary<TKey, TValue>.PrimeMaximum; index += 2u)
        if (0 != (index & 1)) {
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

      [PatchMethod(AggressiveInlining), PatchResolution(0)]
      private void Initialize(uint capacity) {
        capacity = RefDictionary<TKey, TValue>.GetPrime(capacity);

        this.buckets.EnsureCapacity(capacity, true);
        this.hashes .EnsureCapacity(capacity, true);
        this.keys   .EnsureCapacity(capacity, true);
        this.next   .EnsureCapacity(capacity, true);
        this.values .EnsureCapacity(capacity, true);

        this.buckets.Count = this.hashes.Count = this.keys.Count = this.next.Count = this.values.Count = capacity;
        this.free.list     = -1;

        this.buckets.Fill(-1);
      }

      private ref TValue Insert(in TKey key, in TValue value, bool immutable) {
        if (this.buckets.IsEmpty()) this.Initialize(0u);

        uint freeIndex = 0u;
        int  hash      = this.Comparer.GetHashCode(in key) & (int) RefDictionary<TKey, TValue>.PrimeMaximum;
        uint hashIndex = (uint) (hash % this.buckets.Count);

        // …
        for (int index = this.buckets[hashIndex]; index >= 0; ++freeIndex, index = this.next[(uint) index])
        if (hash == this.hashes[(uint) index] && this.Comparer.Equals(in this.keys[(uint) index], in key)) {
          if (!immutable) {
            this.values[(uint) index] = value;
            return ref this.values[(uint) index];
          }

          throw new System.ArgumentException($"Dictionary key already exists: `{key}`");
        }

        if (0u != this.free.count) {
          freeIndex = (uint) this.free.list;
          this.free = (count: this.free.count - 1u, list: this.next[freeIndex]);
        }

        else {
          if (this.count == this.keys.Count)
            this.Resize(RefDictionary<TKey, TValue>.CapacityMaximum >>> 0 > this.count && RefDictionary<TKey, TValue>.CapacityMaximum >>> 1 < this.count ? RefDictionary<TKey, TValue>.CapacityMaximum : RefDictionary<TKey, TValue>.GetPrime(this.count << 1));

          freeIndex   = this.count;
          this.count += 1u;
        }

        this.next   [freeIndex] = this.buckets[hashIndex];
        this.buckets[hashIndex] = (int) freeIndex;
        this.hashes [freeIndex] = hash;
        this.keys   [freeIndex] = key;
        this.values [freeIndex] = value;

        return ref this.values[freeIndex];
      }

      [PatchMethod(AggressiveInlining), PatchResolution(0)] public bool IsEmpty()                                                                  => 0u == this.Count;
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public bool Remove (in PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue> element) { int index = this.FindIndex(in element.Key); return index != -1 && PatchOdyssey.Collections.RefEqualityComparer<TValue>.Default.Equals(in this.values[(uint) index], in element.Value) ? this.Remove(in element.Key) : false; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public bool Remove (in System.Collections.Generic.KeyValuePair <TKey, TValue> element) { int index = this.FindIndex(element.Key);    return index != -1 && PatchOdyssey.Collections.RefEqualityComparer<TValue>.Default.Equals(in this.values[(uint) index], element.Value)    ? this.Remove(element.Key)    : false; }

      public bool Remove(in TKey key) {
        if (key is not null) {
          int  freeIndex = -1;
          int  hash      = this.Comparer.GetHashCode(in key) & (int) RefDictionary<TKey, TValue>.PrimeMaximum;
          uint hashIndex = (uint) (hash % this.buckets.Count);

          // …
          for (int index = this.buckets[hashIndex]; index >= 0; freeIndex = index = this.next[(uint) index])
          if (hash == this.hashes[(uint) index] && this.Comparer.Equals(in this.keys[(uint) index], in key)) {
            (freeIndex < 0 ? ref this.buckets[hashIndex] : ref this.next[(uint) freeIndex]) = this.next[(uint) index];

            this.values[(uint) index] = default!;
            this.next  [(uint) index] = this.free.list;
            this.keys  [(uint) index] = default!;
            this.hashes[(uint) index] = -1;
            this.free                 = (count: this.free.count + 1u, list: index);

            return true;
          }
        }

        return false;
      }

      private void Resize(uint capacity) {
        this.buckets.EnsureCapacity(capacity, true);
        this.hashes .EnsureCapacity(capacity, true);
        this.keys   .EnsureCapacity(capacity, true);
        this.next   .EnsureCapacity(capacity, true);
        this.values .EnsureCapacity(capacity, true);

        this.buckets.Count = this.hashes.Count = this.keys.Count = this.next.Count = this.values.Count = capacity;
        this.buckets.Fill(-1);

        for (uint index = 0; index != this.count; ++index) {
          uint hashIndex = (uint) (this.hashes[index] % capacity);

          // …
          this.next   [index]     = this.buckets[hashIndex];
          this.buckets[hashIndex] = (int) index;
        }
      }

      [PatchMethod(AggressiveInlining), PatchResolution(0)] public override string?                                                                                       ToString                                                                                                   ()                                                                                     { uint end = this.Count, index = 0u; if (end != index) unsafe { System.Text.StringBuilder builder = new(); char* separator = stackalloc char[] {',', ' '}; foreach (ref readonly PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue> element in this) { builder.Append(element); if (end == ++index) return builder.ToString(); builder.Append(separator, 2); } } return string.Empty; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                                                                          TrimExcess                                                                                                 ()                                                                                     { /* Do nothing… */ }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                                                                          TrimExcess                                                                                                 (uint                                                           capacity)              { /* Do nothing… */ }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          bool                                                                                          TryAdd                                                                                                     (in PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>      element)               => this.TryAdd(in element.Key, in element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                                                                          TryAdd                                                                                                     (in System.Collections.Generic.KeyValuePair <TKey, TValue>      element)               => this.TryAdd(element.Key,    element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                                                                          TryAdd                                                                                                     (in TKey                                                        key, in TValue value)  { if (key is not null && !this.ContainsKey(in key)) { this.Add(in key, in value); return true; } return false; }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          ref TValue                                                                                    TryAppend                                                                                                  (in PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>      element)               => ref this.TryAppend(in element.Key, in element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          ref TValue                                                                                    TryAppend                                                                                                  (in System.Collections.Generic.KeyValuePair <TKey, TValue>      element)               { (TKey key, TValue value)[] pair = new[] {(element.Key, element.Value)}; return ref this.TryAppend(in Util.Reference<(TKey key, TValue)>.Only(pair).key, in Util.Reference<(TKey, TValue value)>.Only(pair).value); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          ref TValue                                                                                    TryAppend                                                                                                  (in TKey                                                        key, in TValue  value) { int index = this.FindIndex(in key); return ref (index != -1 ? ref this.values[(uint) index] : ref (key is null ? ref Util.Reference<TValue>.Null : ref this.Insert(in key, in value, true))); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                                                                          TryGetAlternateLookup<TAlternateKey>                                                                       (out RefDictionary<TKey, TValue>.AlternateLookup<TAlternateKey> lookup)                { lookup = this.GetAlternateLookup<TAlternateKey>(); return false; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                                                                          TryGetValue                                                                                                (in TKey                                                        key, out TValue value) { int index = this.FindIndex(in key); if (index != -1) { value = this.values[(uint) index]; return true; } value = default!; return false; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 bool                                                                                          PatchOdyssey.Collections.IRefEquatable<RefDictionary<TKey, TValue>>.Equals                                 (ref RefDictionary                      <TKey, TValue> dictionary)                     => this.Equals  (in dictionary);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 bool                                                                                          PatchOdyssey.Collections.IRefReadOnlyEquatable<RefDictionary<TKey, TValue>>.Equals                         (in  RefDictionary                      <TKey, TValue> dictionary)                     => this.Equals  (in dictionary);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 void                                                                                          System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Add          (System.Collections.Generic.KeyValuePair<TKey, TValue> element)                        => this.Add     (in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 void                                                                                          System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Clear        ()                                                                                     => this.Clear   ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 bool                                                                                          System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Contains     (System.Collections.Generic.KeyValuePair<TKey, TValue>   element)                      => this.Contains(in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 void                                                                                          System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.CopyTo       (System.Collections.Generic.KeyValuePair<TKey, TValue>[] array, int index)             { using (RefDictionary<TKey, TValue>.Enumerator enumerator = this.GetEnumerator()) { while (enumerator.MoveNext()) array[index++] = enumerator.Current; } }
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 bool                                                                                          System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Remove       (System.Collections.Generic.KeyValuePair<TKey, TValue>   element)                      => this.Remove       (in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>> System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>.GetEnumerator()                                                                                     => this.GetEnumerator();
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 void                                                                                          System.Collections.Generic.IDictionary<TKey, TValue>.Add                                                   (TKey         key, TValue value)                                                       => this.Add          (in key, in value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 bool                                                                                          System.Collections.Generic.IDictionary<TKey, TValue>.ContainsKey                                           (TKey         key)                                                                     => this.ContainsKey  (in key);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 bool                                                                                          System.Collections.Generic.IDictionary<TKey, TValue>.Remove                                                (TKey         key)                                                                     => this.Remove       (in key);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 bool                                                                                          System.Collections.Generic.IDictionary<TKey, TValue>.TryGetValue                                           (TKey         key,   out TValue value)                                                 => this.TryGetValue  (in key, out value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 void                                                                                          System.Collections.ICollection.CopyTo                                                                      (System.Array array, int        index)                                                 { foreach (ref readonly PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue> element in this) array.SetValue(element, index++); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 void                                                                                          System.Collections.IDictionary.Add                                                                         (object       key,   object?    value)                                                 => this.Add          ((TKey) key, (TValue) value!);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 void                                                                                          System.Collections.IDictionary.Clear                                                                       ()                                                                                     => this.Clear        ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 bool                                                                                          System.Collections.IDictionary.Contains                                                                    (object key)                                                                           => this.ContainsKey  ((TKey) key);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 System.Collections.IDictionaryEnumerator                                                      System.Collections.IDictionary.GetEnumerator                                                               ()                                                                                     => this.GetEnumerator();
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 void                                                                                          System.Collections.IDictionary.Remove                                                                      (object key)                                                                           => this.Remove       ((TKey) key);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 System.Collections.IEnumerator                                                                System.Collections.IEnumerable.GetEnumerator                                                               ()                                                                                     => this.GetEnumerator();
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 bool                                                                                          System.IEquatable<RefDictionary<TKey, TValue>>.Equals                                                      (RefDictionary<TKey, TValue> dictionary)                                               => this.Equals       (dictionary);

      public ref TValue          this                                                     [in TKey key]                  { [PatchMethod(AggressiveInlining)] get { int index = this.FindIndex(in key); if (index != -1) { return ref this.values[(uint) index]; } if (key is null) throw new System.Collections.Generic.KeyNotFoundException(key?.ToString() ?? string.Empty); return ref this.Insert(in key, Util.Reference<TValue>.Null, false); } }
      public ref readonly TValue this                                                     [in TKey key, in TValue value] { [PatchMethod(AggressiveInlining)] get { int index = this.FindIndex(in key); if (index != -1) { return ref this.values[(uint) index]; } return ref value; } }
      TValue                     System.Collections.Generic.IDictionary<TKey, TValue>.this[TKey    key]                  { [PatchMethod(AggressiveInlining)] get { int index = this.FindIndex(in key); if (index != -1) { return     this.values[(uint) index]; } throw new System.Collections.Generic.KeyNotFoundException(key?.ToString() ?? string.Empty); } set => this[in key] = value; }
      object?                    System.Collections.IDictionary.this                      [object  key]                  { get => ((System.Collections.Generic.IDictionary<TKey, TValue>) this)[(TKey) key]; set => ((System.Collections.Generic.IDictionary<TKey, TValue>) this)[(TKey) key] = (TValue) value!; }
    }
      [System.Serializable] public class AnimationCurveDictionary : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.AnimationCurve> /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public AnimationCurveDictionary(PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public AnimationCurveDictionary(uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public AnimationCurveDictionary(System.Collections.Generic.IDictionary<string, UnityEngine.AnimationCurve> dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public AnimationCurveDictionary(System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.AnimationCurve>> enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public AnimationCurveDictionary(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.AnimationCurve>> enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class BooleanDictionary        : PatchOdyssey.Collections.RefDictionary<string, System     .Boolean>        /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BooleanDictionary       (PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BooleanDictionary       (uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BooleanDictionary       (System.Collections.Generic.IDictionary<string, System     .Boolean>        dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public BooleanDictionary       (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, System     .Boolean>>        enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BooleanDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System     .Boolean>>        enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class BoundsDictionary         : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Bounds>         /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BoundsDictionary        (PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BoundsDictionary        (uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BoundsDictionary        (System.Collections.Generic.IDictionary<string, UnityEngine.Bounds>         dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public BoundsDictionary        (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.Bounds>>         enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BoundsDictionary        (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Bounds>>         enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class BoundsIntDictionary      : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.BoundsInt>      /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BoundsIntDictionary     (PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BoundsIntDictionary     (uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BoundsIntDictionary     (System.Collections.Generic.IDictionary<string, UnityEngine.BoundsInt>      dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public BoundsIntDictionary     (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.BoundsInt>>      enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BoundsIntDictionary     (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.BoundsInt>>      enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class ColorDictionary          : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Color>          /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public ColorDictionary         (PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public ColorDictionary         (uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public ColorDictionary         (System.Collections.Generic.IDictionary<string, UnityEngine.Color>          dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public ColorDictionary         (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.Color>>          enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public ColorDictionary         (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Color>>          enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class DoubleDictionary         : PatchOdyssey.Collections.RefDictionary<string, System     .Double>         /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public DoubleDictionary        (PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public DoubleDictionary        (uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public DoubleDictionary        (System.Collections.Generic.IDictionary<string, System     .Double>         dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public DoubleDictionary        (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, System     .Double>>         enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public DoubleDictionary        (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System     .Double>>         enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class FloatDictionary          : PatchOdyssey.Collections.RefDictionary<string, System     .Single>         /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public FloatDictionary         (PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public FloatDictionary         (uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public FloatDictionary         (System.Collections.Generic.IDictionary<string, System     .Single>         dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public FloatDictionary         (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, System     .Single>>         enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public FloatDictionary         (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System     .Single>>         enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class GameObjectDictionary     : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.GameObject>     /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public GameObjectDictionary    (PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public GameObjectDictionary    (uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public GameObjectDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.GameObject>     dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public GameObjectDictionary    (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.GameObject>>     enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public GameObjectDictionary    (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.GameObject>>     enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class GradientDictionary       : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Gradient>       /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public GradientDictionary      (PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public GradientDictionary      (uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public GradientDictionary      (System.Collections.Generic.IDictionary<string, UnityEngine.Gradient>       dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public GradientDictionary      (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.Gradient>>       enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public GradientDictionary      (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Gradient>>       enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class IntDictionary            : PatchOdyssey.Collections.RefDictionary<string, System     .Int32>          /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public IntDictionary           (PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public IntDictionary           (uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public IntDictionary           (System.Collections.Generic.IDictionary<string, System     .Int32>          dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public IntDictionary           (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, System     .Int32>>          enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public IntDictionary           (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System     .Int32>>          enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class LongDictionary           : PatchOdyssey.Collections.RefDictionary<string, System     .Int64>          /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public LongDictionary          (PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public LongDictionary          (uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public LongDictionary          (System.Collections.Generic.IDictionary<string, System     .Int64>          dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public LongDictionary          (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, System     .Int64>>          enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public LongDictionary          (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System     .Int64>>          enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class RectDictionary           : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Rect>           /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RectDictionary          (PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RectDictionary          (uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RectDictionary          (System.Collections.Generic.IDictionary<string, UnityEngine.Rect>           dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public RectDictionary          (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.Rect>>           enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RectDictionary          (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Rect>>           enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class RectIntDictionary        : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.RectInt>        /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RectIntDictionary       (PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RectIntDictionary       (uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RectIntDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.RectInt>        dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public RectIntDictionary       (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.RectInt>>        enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RectIntDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.RectInt>>        enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class StringDictionary         : PatchOdyssey.Collections.RefDictionary<string, System     .String>         /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public StringDictionary        (PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public StringDictionary        (uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public StringDictionary        (System.Collections.Generic.IDictionary<string, System     .String>         dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public StringDictionary        (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, System     .String>>         enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public StringDictionary        (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System     .String>>         enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class UIntDictionary           : PatchOdyssey.Collections.RefDictionary<string, System     .UInt32>         /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public UIntDictionary          (PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public UIntDictionary          (uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public UIntDictionary          (System.Collections.Generic.IDictionary<string, System     .UInt32>         dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public UIntDictionary          (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, System     .UInt32>>         enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public UIntDictionary          (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System     .UInt32>>         enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class ULongDictionary          : PatchOdyssey.Collections.RefDictionary<string, System     .UInt64>         /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public ULongDictionary         (PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public ULongDictionary         (uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public ULongDictionary         (System.Collections.Generic.IDictionary<string, System     .UInt64>         dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public ULongDictionary         (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, System     .UInt64>>         enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public ULongDictionary         (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System     .UInt64>>         enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class Vector2Dictionary        : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Vector2>        /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector2Dictionary       (PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector2Dictionary       (uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector2Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2>        dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public Vector2Dictionary       (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.Vector2>>        enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector2Dictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector2>>        enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class Vector2IntDictionary     : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Vector2Int>     /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector2IntDictionary    (PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector2IntDictionary    (uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector2IntDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2Int>     dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public Vector2IntDictionary    (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.Vector2Int>>     enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector2IntDictionary    (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector2Int>>     enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class Vector3Dictionary        : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Vector3>        /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector3Dictionary       (PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector3Dictionary       (uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector3Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3>        dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public Vector3Dictionary       (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.Vector3>>        enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector3Dictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector3>>        enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class Vector3IntDictionary     : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Vector3Int>     /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector3IntDictionary    (PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector3IntDictionary    (uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector3IntDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3Int>     dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public Vector3IntDictionary    (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.Vector3Int>>     enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector3IntDictionary    (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector3Int>>     enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class Vector4Dictionary        : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Vector4>        /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.ICollection`, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector4Dictionary       (PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector4Dictionary       (uint capacity, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector4Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector4>        dictionary, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public Vector4Dictionary       (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.Vector4>>        enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector4Dictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, UnityEngine.Vector4>>        enumerable, PatchOdyssey.Collections.IRefEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }

    public /* sealed */ class RefEqualityComparer<T> : PatchOdyssey.Collections.RefReadOnlyEqualityComparer<T>, PatchOdyssey.Collections.IRefEqualityComparer<T> {
      private readonly struct Sentinel : PatchOdyssey.Collections.IRefEquatable<Sentinel> {
        bool PatchOdyssey.Collections.IRefEquatable<Sentinel>.Equals        (ref Sentinel value) => default;
        bool PatchOdyssey.Collections.IRefReadOnlyEquatable<Sentinel>.Equals(in  Sentinel value) => default;
        bool System.IEquatable<Sentinel>.Equals                             (Sentinel     value) => default;
      }

      /* … */
      private static readonly PatchOdyssey.RefHasher            <T> GetHashCodeValue = [PatchMethod(AggressiveInlining)] static (ref T value) => value!.GetHashCode();
      private static readonly PatchOdyssey.RefEqualityComparison<T> EqualsValue      = (PatchOdyssey.RefEqualityComparison<T>) (
        typeof(PatchOdyssey.Collections.IRefEquatable        <T>).IsAssignableFrom(typeof(T)) ? ((PatchOdyssey.RefEqualityComparison<RefEqualityComparer<T>.Sentinel>) RefEqualityComparer<RefEqualityComparer<T>.Sentinel>.RefEquatableEquals)        .Method.GetGenericMethodDefinition().MakeGenericMethod(typeof(T)).CreateDelegate(typeof(PatchOdyssey.RefEqualityComparison<T>)) :
        typeof(PatchOdyssey.Collections.IRefReadOnlyEquatable<T>).IsAssignableFrom(typeof(T)) ? ((PatchOdyssey.RefEqualityComparison<RefEqualityComparer<T>.Sentinel>) RefEqualityComparer<RefEqualityComparer<T>.Sentinel>.RefReadOnlyEquatableEquals).Method.GetGenericMethodDefinition().MakeGenericMethod(typeof(T)).CreateDelegate(typeof(PatchOdyssey.RefEqualityComparison<T>)) :
        typeof(System.IEquatable                             <T>).IsAssignableFrom(typeof(T)) ? ((PatchOdyssey.RefEqualityComparison<RefEqualityComparer<T>.Sentinel>) RefEqualityComparer<RefEqualityComparer<T>.Sentinel>.EquatableEquals)           .Method.GetGenericMethodDefinition().MakeGenericMethod(typeof(T)).CreateDelegate(typeof(PatchOdyssey.RefEqualityComparison<T>)) :
        (PatchOdyssey.RefEqualityComparison<T>) RefEqualityComparer<T>.ObjectEquals<T>
      );
      protected        new readonly PatchOdyssey.RefEqualityComparison<T> comparison       = RefEqualityComparer<T>.EqualsValue;
      protected        new readonly PatchOdyssey.RefHasher            <T> hasher           = RefEqualityComparer<T>.GetHashCodeValue;
      public    static new RefEqualityComparer                        <T> Default { get; } = new();

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public    RefEqualityComparer()                                                                                                    : base()                   {}
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] protected RefEqualityComparer(PatchOdyssey.RefEqualityComparison        <T> comparison, PatchOdyssey.RefHasher        <T>? hasher) : base()                   { this.hasher = hasher ?? this.hasher; this.comparison = comparison; }
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] protected RefEqualityComparer(PatchOdyssey.RefEqualityComparison        <T> comparison, PatchOdyssey.RefReadOnlyHasher<T>? hasher) : base()                   { base.hasher = hasher ?? base.hasher; this.comparison = comparison; }
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] protected RefEqualityComparer(PatchOdyssey.RefReadOnlyEqualityComparison<T> comparison, PatchOdyssey.RefHasher        <T>? hasher) : base(comparison, null)   { this.hasher = hasher ?? this.hasher; }
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] protected RefEqualityComparer(PatchOdyssey.RefReadOnlyEqualityComparison<T> comparison, PatchOdyssey.RefReadOnlyHasher<T>? hasher) : base(comparison, hasher) {}

      /* … */
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public           static RefEqualityComparer<T> Create                                                              (PatchOdyssey.RefEqualityComparison        <T> comparison, PatchOdyssey.RefHasher        <T>? hasher = null)                    => new(comparison, hasher);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public           static RefEqualityComparer<T> Create                                                              (PatchOdyssey.RefEqualityComparison        <T> comparison, PatchOdyssey.RefReadOnlyHasher<T>? hasher = null)                    => new(comparison, hasher);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public           static RefEqualityComparer<T> Create                                                              (PatchOdyssey.RefReadOnlyEqualityComparison<T> comparison, PatchOdyssey.RefHasher        <T>? hasher = null)                    => new(comparison, hasher);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  new      static RefEqualityComparer<T> Create                                                              (PatchOdyssey.RefReadOnlyEqualityComparison<T> comparison, PatchOdyssey.RefReadOnlyHasher<T>? hasher = null)                    => new(comparison, hasher);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  virtual         bool                   Equals                                                              (ref T                                         a,          ref T                              b)                                => this.comparison(ref a, ref b);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  override        bool                   Equals                                                              (T                                             a,          T                                  b)                                => a is null ? b is null : b is null ? a is null : this.RefReadOnlyEquals(in a, in b);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private          static bool                   EquatableEquals<U>                                                  (ref U                                         a,          ref U                              b) where U : System.IEquatable<U> => a.Equals(b);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  virtual         int                    GetHashCode                                                         (ref T                                         value)                                                                           => this.hasher(ref value!);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  override        int                    GetHashCode                                                         (T                                             value)                                                                           => this.RefReadOnlyGetHashCode(in value!);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private          static bool                   ObjectEquals      <U>                                               (ref U                                         a, ref U b)                                                                      => a!.Equals(b);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private          static bool                   RefEquatableEquals<U>                                               (ref U                                         a, ref U b) where U : PatchOdyssey.Collections.IRefEquatable<U>                  => a .Equals(ref b);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private                 bool                   RefReadOnlyEquals                                                   (in  T                                         a, in  T b)                                                                      { if (RefEqualityComparer<T>.EqualsValue != this.comparison) throw new System.NotSupportedException("Reference equality comparer can not compare as modifiable references"); return base.Equals(in a, in b); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private          static bool                   RefReadOnlyEquatableEquals<U>                                       (ref U                                         a, ref U b) where U : PatchOdyssey.Collections.IRefReadOnlyEquatable<U>          => a.Equals(in b);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private                 int                    RefReadOnlyGetHashCode                                              (in  T                                         value)                                                                           { if (RefEqualityComparer<T>.GetHashCodeValue != this.hasher) throw new System.NotSupportedException("Reference equality comparer can not get hash code as modifiable references"); return base.GetHashCode(in value); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                           PatchOdyssey.Collections.IRefEqualityComparer<T>.Equals             (ref T                                         a, ref T b)                                                                      => this.Equals                (ref a, ref b);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] int                                            PatchOdyssey.Collections.IRefEqualityComparer<T>.GetHashCode        (ref T                                         value)                                                                           => this.GetHashCode           (ref value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                           PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<T>.Equals     (in  T                                         a, in T b)                                                                       => this.RefReadOnlyEquals     (in  a, in b);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] int                                            PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<T>.GetHashCode(in  T                                         value)                                                                           => this.RefReadOnlyGetHashCode(in  value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                           System.Collections.Generic.IEqualityComparer<T>.Equals              (T?                                            a, T? b)                                                                         => a is null ? b is null : b is null ? a is null : this.RefReadOnlyEquals(in a!, in b!);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] int                                            System.Collections.Generic.IEqualityComparer<T>.GetHashCode         (T                                             value)                                                                           => this.RefReadOnlyGetHashCode(in value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                           System.Collections.IEqualityComparer.Equals                         (object?                                       a, object? b)                                                                    => a is null ? b is null : b is null ? a is null : this.RefReadOnlyEquals((T) a, (T) b);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] int                                            System.Collections.IEqualityComparer.GetHashCode                    (object                                        value)                                                                           => this.RefReadOnlyGetHashCode((T) value);
    }

    public struct RefKeyValuePair<TKey, TValue> /* ⟶ Based on `System.Collections.Generic.KeyValuePair<TKey, TValue>` (hence no `System.IEquatable<RefKeyValuePair<TKey, TValue>>` implementation) */ {
      internal readonly     PatchOdyssey.Collections.RefDictionary<TKey, TValue> dictionary;
      internal readonly     uint                                                 index;
      public   ref readonly TKey                                                 Key => ref (this.referenced ? ref this.dictionary.keys[this.index] : ref Util.Reference<(TKey key, TValue)>.Only(this.pair).key);
      internal readonly     (TKey key, TValue value)[]                           pair; // ⟶ `System.Collections.Generic.KeyValuePair<TKey, TValue>` but `ref`-errable
      internal readonly     bool                                                 referenced;
      public   ref          TValue                                               Value => ref (this.referenced ? ref this.dictionary.values[this.index] : ref Util.Reference<(TKey, TValue value)>.Only(this.pair).value);

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)] internal RefKeyValuePair(PatchOdyssey.Collections.RefDictionary<TKey, TValue> dictionary, uint      index) { this.dictionary = dictionary; this.index = index; this.pair = null!;                this.referenced = true; }
      [PatchConstructor, PatchMethod(AggressiveInlining)] public   RefKeyValuePair(in TKey                                              key,        in TValue value) { this.dictionary = null!;      this.index = 0u;    this.pair = new[] {(key, value)}; this.referenced = false; }
      [PatchConstructor, PatchMethod(AggressiveInlining)] public   RefKeyValuePair(TKey                                                 key,        TValue    value) { this.dictionary = null!;      this.index = 0u;    this.pair = new[] {(key, value)}; this.referenced = false; }

      /* … */
      [PatchMethod(AggressiveInlining)] public          void   Deconstruct(out TKey key, out TValue value) { key = this.Key; value = this.Value; }
      [PatchMethod(AggressiveInlining)] public override string ToString   ()                               => $"[{this.Key}, {this.Value}]";

      [PatchMethod(AggressiveInlining)]
      public static implicit operator System.Collections.Generic.KeyValuePair<TKey, TValue>(in RefKeyValuePair<TKey, TValue> pair) => new(pair.Key, pair.Value);
    }

    [System.Serializable]
    public class RefList<T> : PatchOdyssey.Collections.RefReadOnlyList<T>, PatchOdyssey.Collections.IRefEquatable<RefList<T>>, System.Collections.Generic.IList<T>, System.Collections.ICollection, System.Collections.IList, System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable, System.ICloneable {
      public new struct Enumerator : System.Collections.Generic.IEnumerator<T> {
        public ref      T                                                      Current => ref this.enumerator.list.GetValue((uint) this.enumerator.index);
        public readonly PatchOdyssey.Collections.RefReadOnlyList<T>.Enumerator enumerator;
        T                                                                      System.Collections.Generic.IEnumerator<T>.Current => this.Current;
        object                                                                 System.Collections.IEnumerator.Current            => this.Current!;

        /* … */
        [PatchConstructor, PatchMethod(AggressiveInlining)]
        public Enumerator(RefList<T> list) {
          this.enumerator = new(list);
        }

        /* … */
        [PatchMethod(AggressiveInlining)] public void Dispose                                () => this.enumerator.Dispose ();
        [PatchMethod(AggressiveInlining)] public bool MoveNext                               () => this.enumerator.MoveNext();
        [PatchMethod(AggressiveInlining)] public void Reset                                  () => this.enumerator.Reset   ();
        [PatchMethod(AggressiveInlining)] bool        System.Collections.IEnumerator.MoveNext() => this           .MoveNext();
        [PatchMethod(AggressiveInlining)] void        System.Collections.IEnumerator.Reset   () => this           .Reset   ();
        [PatchMethod(AggressiveInlining)] void        System.IDisposable.Dispose             () => this           .Dispose ();
      }

      /* … */
      public                                                            new uint Capacity                                             { get => this.capacity; set => this.EnsureCapacity(value); }
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] private     uint capacity                                             =  0u;
      int                                                                        System.Collections.Generic.ICollection<T>.Count      => ((int) this.Count);
      bool                                                                       System.Collections.Generic.ICollection<T>.IsReadOnly => false;
      int                                                                        System.Collections.ICollection.Count                 => ((int) this.Count);
      bool                                                                       System.Collections.ICollection.IsSynchronized        => false;
      object                                                                     System.Collections.ICollection.SyncRoot              => this;
      bool                                                                       System.Collections.IList.IsFixedSize                 => false;
      bool                                                                       System.Collections.IList.IsReadOnly                  => false;

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)] public    RefList()                                                     : base()                                                   {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public    RefList(uint                                        capacity) : base(capacity = RefList<T>.GetCapacity(capacity))        => this.capacity = capacity;
      [PatchConstructor, PatchMethod(AggressiveInlining)] public    RefList(RefList<T>                                  list)     : this((PatchOdyssey.Collections.RefReadOnlyList<T>) list) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public    RefList(PatchOdyssey.Collections.RefReadOnlyList<T> list)     : this(list.Items, 0u, list.Count)                         {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] protected RefList(T[]                                         array, uint index, uint length)                                      => Util.Array<T>.Copy(array, index, this.Items = PatchOdyssey.Collections.RefReadOnlyList<T>.CreateInstance(this.capacity = RefList<T>.GetCapacity(this.Count = length)), 0u, length);

      [PatchConstructor, PatchMethod(AggressiveInlining)]
      public RefList(System.Collections.Generic.IEnumerable<T> enumerable) {
        base.Count = Util.EnumerableCount(enumerable);

        if (!base.IsEmpty()) {
          this.capacity = RefList<T>.GetCapacity(base.Count);
          this.Items    = new T[this.capacity];

          using (System.Collections.Generic.IEnumerator<T> enumerator = enumerable.GetEnumerator()) {
            for (uint index = 0u; enumerator.MoveNext(); ++index)
            this.SetValue(enumerator.Current, index);
          }
        }
      }

      /* … */
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public           void                  Add           (in T                                      element)                                                                                          => this.Insert     (this.Count, in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public           ref T                 Append        (in T                                      element)                                                                                          {  this.Insert     (this.Count, in element); return ref base.GetValue(this.Count - 1u); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public           void                  AddRange      (System.Collections.Generic.IEnumerable<T> enumerable)                                                                                       => this.InsertRange(this.Count, enumerable);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public    new    RefList<T>            AsCopy        ()                                                                                                                                           => new(this);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public           void                  Clear         ()                                                                                                                                           { this.capacity = this.Count = 0u; base.Items = System.Array.Empty<T>(); }
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public           RefList<U>            ConvertAll<U> (PatchOdyssey.RefConverter        <T, U> converter)                                                                                          { RefList<U> list = new(this.Count); for (; list.Count != this.Count; ++list.Count) { list.SetValue(converter(ref base.GetValue(list.Count)), list.Count); } return list; }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public    new    RefList<U>            ConvertAll<U> (PatchOdyssey.RefReadOnlyConverter<T, U> converter)                                                                                          => this.ConvertAll([PatchMethod(AggressiveInlining)] (ref T element) => converter(in element));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public    new    RefList<U>            ConvertAll<U> (System.Converter                 <T, U> converter)                                                                                          => this.ConvertAll([PatchMethod(AggressiveInlining)] (ref T element) => converter(element));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public           uint                  EnsureCapacity(uint                                    capacity, bool precise = false)                                                                     { if (capacity > this.capacity) { T[] list = base.Items; Util.Array<T>.Copy(list, 0u, base.Items = RefReadOnlyList<T>.CreateInstance(this.capacity = !precise ? RefList<T>.GetCapacity(capacity) : capacity), 0u, this.Count); } return this.capacity; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public           bool                  Equals        (in RefList                       <T>    list)                                                                                               => object.Equals(list, this);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public           bool                  Exists        (PatchOdyssey.RefPredicate        <T>    predicate)                                                                                          { for (uint index = 0u; index != this.Count; ++index) { if (predicate(ref base.GetValue(index))) return true; } return false; }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public    new    bool                  Exists        (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                          => base      .Exists(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public    new    bool                  Exists        (System.Predicate                 <T>    predicate)                                                                                          => base      .Exists(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public           void                  Fill          (in T                                    element)                                                                                            => base.Items.Fill  (in element);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public           ref T                 Find          (PatchOdyssey.RefPredicate        <T>    predicate)                                                                                          { for (uint index = 0u; index != this.Count; ++index) { ref T element = ref base.GetValue(index); if (predicate(ref element)) return ref element; } return ref base.Null; }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public    new    ref T                 Find          (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                          => ref base.Find(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public    new    ref T                 Find          (System.Predicate                 <T>    predicate)                                                                                          => ref base.Find(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public           RefList<T>            FindAll       (PatchOdyssey.RefPredicate        <T>    predicate)                                                                                          { RefList<T> list = new(this.Count); for (uint index = 0u; index != this.Count; ++index) { ref T element = ref base.GetValue(index); if (predicate(ref element)) list.SetValue(in element, list.Count++); } list.TrimExcess(); return list; }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public    new    RefList<T>            FindAll       (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                          => this.FindAll  ([PatchMethod(AggressiveInlining)] (ref T element) => predicate(in element));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public    new    RefList<T>            FindAll       (System.Predicate                 <T>    predicate)                                                                                          => this.FindAll  ([PatchMethod(AggressiveInlining)] (ref T element) => predicate(element));
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public           int                   FindIndex     (PatchOdyssey.RefPredicate        <T>    predicate)                                                                                          => this.FindIndex(0u, this.Count, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public    new    int                   FindIndex     (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                          => base.FindIndex(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public    new    int                   FindIndex     (System.Predicate                 <T>    predicate)                                                                                          => base.FindIndex(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public           int                   FindIndex     (uint                                    index, PatchOdyssey.RefPredicate        <T> predicate)                                              { for (; index < this.Count; ++index) { if (predicate(ref base.GetValue(index))) return (int) index; } return -1; }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public    new    int                   FindIndex     (uint                                    index, PatchOdyssey.RefReadOnlyPredicate<T> predicate)                                              => base.FindIndex(index, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public    new    int                   FindIndex     (uint                                    index, System.Predicate                 <T> predicate)                                              => base.FindIndex(index, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public           int                   FindIndex     (uint                                    index, uint                                 length, PatchOdyssey.RefPredicate        <T> predicate) { for (uint end = System.Math.Min(this.Count, index + length); index < end; ++index) { if (predicate(ref base.GetValue(index))) return (int) index; } return -1; }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public    new    int                   FindIndex     (uint                                    index, uint                                 length, PatchOdyssey.RefReadOnlyPredicate<T> predicate) => base.FindIndex(index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public    new    int                   FindIndex     (uint                                    index, uint                                 length, System.Predicate                 <T> predicate) => base.FindIndex(index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public           ref T                 FindLast      (PatchOdyssey.RefPredicate        <T>    predicate)                                                                                          { for (uint index = this.Count; 0 != index--; ) { ref T element = ref base.GetValue(index); if (predicate(ref element)) return ref element; } return ref base.Null; }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public    new    ref T                 FindLast      (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                          => ref base.FindLast     (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public    new    ref T                 FindLast      (System.Predicate                 <T>    predicate)                                                                                          => ref base.FindLast     (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public           int                   FindLastIndex (PatchOdyssey.RefPredicate        <T>    predicate)                                                                                          =>     this.FindLastIndex(0u, this.Count, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public    new    int                   FindLastIndex (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                          =>     base.FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public    new    int                   FindLastIndex (System.Predicate                 <T>    predicate)                                                                                          =>     base.FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public           int                   FindLastIndex (uint                                    index, PatchOdyssey.RefPredicate        <T> predicate)                                              { for (uint end = this.Count; end-- > index; ) { if (predicate(ref base.GetValue(end))) return (int) end; } return -1; }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public    new    int                   FindLastIndex (uint                                    index, PatchOdyssey.RefReadOnlyPredicate<T> predicate)                                              => base.FindLastIndex(index, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public    new    int                   FindLastIndex (uint                                    index, System.Predicate                 <T> predicate)                                              => base.FindLastIndex(index, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public           int                   FindLastIndex (uint                                    index, uint                                 length, PatchOdyssey.RefPredicate        <T> predicate) { for (uint end = System.Math.Min(this.Count, index + length); end-- > index; ) { if (predicate(ref base.GetValue(end))) return (int) end; } return -1; }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public    new    int                   FindLastIndex (uint                                    index, uint                                 length, PatchOdyssey.RefReadOnlyPredicate<T> predicate) => base.FindLastIndex(index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public    new    int                   FindLastIndex (uint                                    index, uint                                 length, System.Predicate                 <T> predicate) => base.FindLastIndex(index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public           void                  ForEach       (PatchOdyssey.RefAction        <T>       action)                                                                                             { for (uint index = 0u; index != this.Count; ++index) action(ref base.GetValue(index)); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public    new    void                  ForEach       (PatchOdyssey.RefReadOnlyAction<T>       action)                                                                                             => base.ForEach(action);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public    new    void                  ForEach       (System.Action                 <T>       action)                                                                                             => base.ForEach(action);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] protected static uint                  GetCapacity   (uint                                    count)                                                                                              { if (0u != count) { count = System.Math.Max(count, 4u) - 1u; count |= count >>> 1; count |= count >>> 2; count |= count >>> 4; count |= count >>> 8; count |= count >>> 16; return count + 1u; } return 0u; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public    new    RefList<T>.Enumerator GetEnumerator ()                                                                                                                                           => new(this);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public    new    RefList<T>            GetRange      (uint index, uint length)                                                                                                                    => new(base.Items, index, length);

      private void HeapSort(uint begin, uint end, PatchOdyssey.RefComparison<T> comparison) /* ⟶ See `https://web.archive.org/web/20170509032649/http://www.cdn.geeksforgeeks.org/heap-sort/` */ {
        [PatchMethod(AggressiveInlining)]
        static void Build(RefList<T> list, uint end, uint index, PatchOdyssey.RefComparison<T> comparison) {
          uint largest = index;
          uint left    = (index * 2u) + 1u;
          uint right   = (index * 2u) + 2u;

          // …
          largest = end > left  && comparison(ref list.GetValue(largest), ref list.GetValue(left))  < 0 ? left  : largest;
          largest = end > right && comparison(ref list.GetValue(largest), ref list.GetValue(right)) < 0 ? right : largest;

          if (index != largest) {
            (list.GetValue(index), list.GetValue(largest)) = (list.GetValue(largest), list.GetValue(index));
            Build(list, end, largest, comparison);
          }
        }

        for (uint index = begin + ((end - begin) >>> 1); begin != index--; )
        Build(this, end, index, comparison);

        for (uint index = end; begin != index--; ) {
          (base.GetValue(begin), base.GetValue(index)) = (base.GetValue(index), base.GetValue(begin));
          Build(this, index, begin, comparison);
        }
      }

      [PatchMethod(AggressiveInlining), PatchResolution(0)]
      public void Insert(uint index, in T element) {
        if (this.capacity == this.Count)
        this.EnsureCapacity(this.Count + 1u);

        if (index <= this.Count) {
          Util.Array<T>.Copy(this.Items, index, this.Items, index + 1u, this.Count - index);
          base.SetValue(in element, index);
        }

        ++this.Count;
      }

      private void InsertionSort(uint begin, uint end, PatchOdyssey.RefComparison<T> comparison) /* ⟶ See `https://web.archive.org/web/20240629152053/https://www.geeksforgeeks.org/insertion-sort-algorithm/` */ {
        for (uint index = begin + 1u; end != index; ++index) {
          T   element  = base.GetValue(index);
          int subindex = ((int) index) - 1;

          // …
          for (; subindex >= 0 && comparison(ref base.GetValue((uint) subindex), ref element) > 0; --subindex)
            base.SetValue(in base.GetValue((uint) subindex + 0u), (uint) subindex + 1u);

          base.SetValue(in element, (uint) ++subindex);
        }
      }

      public void InsertRange(uint index, System.Collections.Generic.IEnumerable<T> enumerable) {
        uint count = Util.EnumerableCount(enumerable);

        // …
        if (this.capacity < this.Count + count)
        this.EnsureCapacity(this.Count + count);

        if (index < this.Count) {
          Util.Array<T>.Copy(this.Items, index, this.Items, index + count, this.Count - index);
          this.Count += count;
        } else /* if (index > this.Count) */ (count, this.Count) = (count - System.Math.Min(count, index - this.Count), this.Count + count);

        using (System.Collections.Generic.IEnumerator<T> enumerator = enumerable.GetEnumerator())
        while (0u != count--) {
          enumerator.MoveNext();
          base.SetValue(enumerator.Current, index++);
        }
      }

      [PatchMethod(AggressiveInlining), PatchResolution(0)]
      public ref T Prepend(in T element) {
        this.Insert(0u, in element);
        return ref base.GetValue(0u);
      }

      private void QuickSort(uint begin, uint end, PatchOdyssey.RefComparison<T> comparison) /* ⟶ See `https://web.archive.org/web/20240629152053/https://www.geeksforgeeks.org/quick-sort-algorithm/` */ {
        [PatchMethod(AggressiveInlining)]
        static uint Partition(RefList<T> list, uint begin, uint end, PatchOdyssey.RefComparison<T> comparison) {
          int   index = ((int) begin) - 1;
          ref T pivot = ref list.GetValue(end);

          // …
          for (uint subindex = begin; end > subindex; ++subindex)
          if (comparison(ref list.GetValue(subindex), ref pivot) < 0) {
            ++index;
            (list.GetValue((uint) index), list.GetValue(subindex)) = (list.GetValue(subindex), list.GetValue((uint) index));
          }

          ++index;
          (list.GetValue((uint) index), list.GetValue(end)) = (list.GetValue(end), list.GetValue((uint) index));

          return (uint) index;
        }

        // …
        if (begin < end) {
          uint index = Partition(this, begin, end, comparison);

          this.QuickSort(begin,      index - 1u, comparison);
          this.QuickSort(index + 1u, end,        comparison);
        }
      }

      [PatchMethod(AggressiveInlining), PatchResolution(0)] public      bool       Remove                                                           (in T                                 element)                                                                                       { int index = base.IndexOf(in element); if (index != -1) { this.RemoveAt((uint) index); return true; } return false; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public      uint       RemoveAll                                                        (PatchOdyssey.RefPredicate        <T> predicate)                                                                                     { T[] array = (T[]) base.Items.Clone(); uint length = 0u; for (uint index = 0u; index != base.Count; ++index) { ref T element = ref base.GetValue(index); if (!predicate(ref element)) array[length++] = element; } length = base.Count - length; base.Count -= length; this.Items = array; return length; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public      uint       RemoveAll                                                        (PatchOdyssey.RefReadOnlyPredicate<T> predicate)                                                                                     => this.RemoveAll([PatchMethod(AggressiveInlining)] (ref T element) => predicate(in element));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public      uint       RemoveAll                                                        (System.Predicate                 <T> predicate)                                                                                     => this.RemoveAll([PatchMethod(AggressiveInlining)] (ref T element) => predicate   (element));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public      void       RemoveAt                                                         (uint                                 index)                                                                                         { if (index < base.Count) { Util.Array<T>.Copy(base.Items, index + 1u, base.Items, index, base.Count - index - 1u); --base.Count; } }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public      void       RemoveRange                                                      (uint                                 index, uint length)                                                                            { if (index < base.Count) { length = base.Count < index + length ? base.Count - index : length; Util.Array<T>.Copy(base.Items, index + length, base.Items, index, base.Count - index - length); base.Count -= length; } }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public      void       Reverse                                                          ()                                                                                                                                   => this      .Reverse(0u,          base.Count);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public      void       Reverse                                                          (uint index, uint length)                                                                                                            => base.Items.Reverse((int) index, (int) length);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  new RefList<T> Slice                                                            (uint index, uint length)                                                                                                            => new(base.Items, index, length);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public      void       Sort                                                             ()                                                                                                                                   => this.Sort(0u, base.Count, (PatchOdyssey.RefComparison        <T>) PatchOdyssey.Collections.RefComparer<T>.Default.Compare);
      [PatchMethod(AggressiveInlining), PatchResolution(6)] public      void       Sort                                                             (PatchOdyssey.Collections.IRefComparer        <T>? comparer)                                                                         => this.Sort(0u, base.Count, (PatchOdyssey.RefComparison        <T>) (comparer is not null ? comparer.Compare : PatchOdyssey.Collections.RefComparer        <T>.Default.Compare));
      [PatchMethod(AggressiveInlining), PatchResolution(5)] public      void       Sort                                                             (PatchOdyssey.Collections.IRefReadOnlyComparer<T>? comparer)                                                                         => this.Sort(0u, base.Count, (PatchOdyssey.RefReadOnlyComparison<T>) (comparer is not null ? comparer.Compare : PatchOdyssey.Collections.RefReadOnlyComparer<T>.Default.Compare));
      [PatchMethod(AggressiveInlining), PatchResolution(3)] public      void       Sort                                                             (PatchOdyssey.RefComparison                   <T>? comparison)                                                                       => this.Sort(0u, base.Count, comparison ??                                                                                               PatchOdyssey.Collections.RefComparer<T>.Default.Compare);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public      void       Sort                                                             (PatchOdyssey.RefReadOnlyComparison           <T>? comparison)                                                                       => this.Sort(0u, base.Count, comparison is not null ? ([PatchMethod(AggressiveInlining)] (ref T a, ref T b) => comparison(in a, in b)) : PatchOdyssey.Collections.RefComparer<T>.Default.Compare);
      [PatchMethod(AggressiveInlining), PatchResolution(4)] public      void       Sort                                                             (System.Collections.Generic.IComparer         <T>? comparer)                                                                         => this.Sort(0u, base.Count, comparer   is not null ? comparer.Compare                                                                 : System.Collections.Generic.Comparer <T>.Default.Compare);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public      void       Sort                                                             (System.Comparison                            <T>? comparison)                                                                       => this.Sort(0u, base.Count, comparison is not null ? ([PatchMethod(AggressiveInlining)] (ref T a, ref T b) => comparison(a, b))       : PatchOdyssey.Collections.RefComparer<T>.Default.Compare);
      [PatchMethod(AggressiveInlining), PatchResolution(3)] public      void       Sort                                                             (uint                                              index, uint length, PatchOdyssey.Collections.IRefComparer        <T>? comparer)   => this.Sort(index, System.Math.Min(base.Count, index + length), (PatchOdyssey.RefComparison<T>) (comparer is not null ? comparer.Compare                                                                       : PatchOdyssey.Collections.RefComparer<T>.Default.Compare));
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public      void       Sort                                                             (uint                                              index, uint length, PatchOdyssey.Collections.IRefReadOnlyComparer<T>? comparer)   => this.Sort(index, System.Math.Min(base.Count, index + length), (PatchOdyssey.RefComparison<T>) (comparer is not null ? ([PatchMethod(AggressiveInlining)] (ref T a, ref T b) => comparer.Compare(in a, in b)) : PatchOdyssey.Collections.RefComparer<T>.Default.Compare));
      [PatchMethod(AggressiveInlining), PatchResolution(2)] private     void       Sort                                                             (uint                                              begin, uint end,    PatchOdyssey.RefComparison                   <T>  comparison) { if (base.Count <= 16u) this.InsertionSort(begin, end, comparison); else if (base.Count > System.Math.Log((double) base.Count) * 2.0) this.HeapSort(begin, end, comparison); else this.QuickSort(begin, end - 1u, comparison); }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] private     void       Sort                                                             (uint                                              begin, uint end,    PatchOdyssey.RefReadOnlyComparison           <T>  comparison) => this.Sort(begin, end, [PatchMethod(AggressiveInlining)] (ref T a, ref T b) => comparison(in a, in b));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public      void       Sort                                                             (uint                                              index, uint length, System.Collections.Generic.IComparer         <T>? comparer)   => this.Sort(index, System.Math.Min(base.Count, index + length), comparer is not null ? ([PatchMethod(AggressiveInlining)] (ref T a, ref T b) => comparer.Compare(a, b)) : PatchOdyssey.Collections.RefComparer<T>.Default.Compare);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private     void       Sort                                                             (uint                                              begin, uint end,    System.Comparison                            <T>  comparison) => this.Sort(begin, end, [PatchMethod(AggressiveInlining)] (ref T a, ref T b) => comparison(a, b));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public      void       TrimExcess                                                       ()                                                                                                                                   => this.TrimExcess(base.Count);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public      void       TrimExcess                                                       (uint                                 capacity)                                                                                      { capacity = RefList<T>.GetCapacity(capacity); if (capacity < this.capacity && capacity >= base.Count) System.Array.Resize(ref base.Items, (int) (this.capacity = capacity)); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public      bool       TrueForAll                                                       (PatchOdyssey.RefPredicate        <T> predicate)                                                                                     { for (uint index = 0u; index != base.Count; ++index) { if (!predicate(ref base.GetValue(index))) return false; } return true; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  new bool       TrueForAll                                                       (PatchOdyssey.RefReadOnlyPredicate<T> predicate)                                                                                     => base.TrueForAll(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  new bool       TrueForAll                                                       (System.Predicate                 <T> predicate)                                                                                     => base.TrueForAll(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                   PatchOdyssey.Collections.IRefEquatable<RefList<T>>.Equals        (ref RefList                      <T> list)                                                                                          => this.Equals    (in list);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                   PatchOdyssey.Collections.IRefReadOnlyEquatable<RefList<T>>.Equals(in  RefList                      <T> list)                                                                                          => this.Equals    (in list);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                   System.Collections.Generic.ICollection<T>.Add                    (T                                    element)                                                                                       => this.Add       (element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                   System.Collections.Generic.ICollection<T>.Clear                  ()                                                                                                                                   => this.Clear     ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                   System.Collections.Generic.ICollection<T>.Contains               (T            element)                                                                                                               => base.Contains  (element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                   System.Collections.Generic.ICollection<T>.CopyTo                 (T[]          array, int index)                                                                                                      => base.CopyTo    (array, (uint) index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                   System.Collections.Generic.ICollection<T>.Remove                 (T            element)                                                                                                               => this.Remove    (element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] int                    System.Collections.Generic.IList<T>.IndexOf                      (T            element)                                                                                                               => base.IndexOf   (element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                   System.Collections.Generic.IList<T>.Insert                       (int          index, T element)                                                                                                      => this.Insert    ((uint) index, element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                   System.Collections.Generic.IList<T>.RemoveAt                     (int          index)                                                                                                                 => this.RemoveAt  ((uint) index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                   System.Collections.ICollection.CopyTo                            (System.Array array, int index)                                                                                                      { foreach (ref T element in this) array.SetValue(element, index++); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] int                    System.Collections.IList.Add                                     (object?      element)                                                                                                               {  this    .Add        ((T) element!); return (int) this.Count; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                   System.Collections.IList.Clear                                   ()                                                                                                                                   => this    .Clear      ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                   System.Collections.IList.Contains                                (object? element)                                                                                                                    => base    .Contains   ((T) element!);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] int                    System.Collections.IList.IndexOf                                 (object? element)                                                                                                                    => base    .IndexOf    ((T) element!);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                   System.Collections.IList.Insert                                  (int     index, object? element)                                                                                                     => this    .Insert     ((uint) index, (T) element!);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                   System.Collections.IList.Remove                                  (object? element)                                                                                                                    => this    .Remove     ((T) element!);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                   System.Collections.IList.RemoveAt                                (int     index)                                                                                                                      => this    .RemoveAt   ((uint) index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] int                    System.Collections.IStructuralComparable.CompareTo               (object?                              value, System.Collections.IComparer         comparer)                                          => comparer.Compare    (this, value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                   System.Collections.IStructuralEquatable.Equals                   (object?                              value, System.Collections.IEqualityComparer comparer)                                          => comparer.Equals     (this, value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] int                    System.Collections.IStructuralEquatable.GetHashCode              (System.Collections.IEqualityComparer comparer)                                                                                      => comparer.GetHashCode(this);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] object                 System.ICloneable.Clone                                          ()                                                                                                                                   => base    .Clone      ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                   System.IEquatable<RefList<T>>.Equals                             (RefList<T> list)                                                                                                                    => this    .Equals     (list);

      /* … */
      public new ref T          this                                    [uint         index] => ref base.GetValue(index);
      public new     RefList<T> this                                    [System.Range range] => new(this.Items[range]);
      T                         System.Collections.Generic.IList<T>.this[int          index] { get => this[(uint) index]; set => this[(uint) index] = value; }
      object?                   System.Collections.IList.this           [int          index] { get => this[(uint) index]; set => this[(uint) index] = (T) value!; }
    }
      [System.Serializable] public class AnimationCurveList : PatchOdyssey.Collections.RefList<UnityEngine.AnimationCurve> { [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveList() : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveList(uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveList(System.Collections.Generic.IEnumerable<UnityEngine.AnimationCurve> enumerable) : base(enumerable) {} }
      [System.Serializable] public class BooleanList        : PatchOdyssey.Collections.RefList<System     .Boolean>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanList       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanList       (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanList       (System.Collections.Generic.IEnumerable<System     .Boolean>        enumerable) : base(enumerable) {} }
      [System.Serializable] public class BoundsList         : PatchOdyssey.Collections.RefList<UnityEngine.Bounds>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsList        () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsList        (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsList        (System.Collections.Generic.IEnumerable<UnityEngine.Bounds>         enumerable) : base(enumerable) {} }
      [System.Serializable] public class BoundsIntList      : PatchOdyssey.Collections.RefList<UnityEngine.BoundsInt>      { [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntList     () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntList     (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntList     (System.Collections.Generic.IEnumerable<UnityEngine.BoundsInt>      enumerable) : base(enumerable) {} }
      [System.Serializable] public class ColorList          : PatchOdyssey.Collections.RefList<UnityEngine.Color>          { [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorList         () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorList         (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorList         (System.Collections.Generic.IEnumerable<UnityEngine.Color>          enumerable) : base(enumerable) {} }
      [System.Serializable] public class DoubleList         : PatchOdyssey.Collections.RefList<System     .Double>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleList        () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleList        (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleList        (System.Collections.Generic.IEnumerable<System     .Double>         enumerable) : base(enumerable) {} }
      [System.Serializable] public class FloatList          : PatchOdyssey.Collections.RefList<System     .Single>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatList         () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatList         (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatList         (System.Collections.Generic.IEnumerable<System     .Single>         enumerable) : base(enumerable) {} }
      [System.Serializable] public class GameObjectList     : PatchOdyssey.Collections.RefList<UnityEngine.GameObject>     { [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectList    () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectList    (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectList    (System.Collections.Generic.IEnumerable<UnityEngine.GameObject>     enumerable) : base(enumerable) {} }
      [System.Serializable] public class GradientList       : PatchOdyssey.Collections.RefList<UnityEngine.Gradient>       { [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientList      () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientList      (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientList      (System.Collections.Generic.IEnumerable<UnityEngine.Gradient>       enumerable) : base(enumerable) {} }
      [System.Serializable] public class IntList            : PatchOdyssey.Collections.RefList<System     .Int32>          { [PatchConstructor, PatchMethod(AggressiveInlining)] public IntList           () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public IntList           (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public IntList           (System.Collections.Generic.IEnumerable<System     .Int32>          enumerable) : base(enumerable) {} }
      [System.Serializable] public class LongList           : PatchOdyssey.Collections.RefList<System     .Int64>          { [PatchConstructor, PatchMethod(AggressiveInlining)] public LongList          () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public LongList          (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public LongList          (System.Collections.Generic.IEnumerable<System     .Int64>          enumerable) : base(enumerable) {} }
      [System.Serializable] public class RectList           : PatchOdyssey.Collections.RefList<UnityEngine.Rect>           { [PatchConstructor, PatchMethod(AggressiveInlining)] public RectList          () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectList          (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectList          (System.Collections.Generic.IEnumerable<UnityEngine.Rect>           enumerable) : base(enumerable) {} }
      [System.Serializable] public class RectIntList        : PatchOdyssey.Collections.RefList<UnityEngine.RectInt>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntList       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntList       (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntList       (System.Collections.Generic.IEnumerable<UnityEngine.RectInt>        enumerable) : base(enumerable) {} }
      [System.Serializable] public class StringList         : PatchOdyssey.Collections.RefList<System     .String>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public StringList        () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public StringList        (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public StringList        (System.Collections.Generic.IEnumerable<System     .String>         enumerable) : base(enumerable) {} }
      [System.Serializable] public class UIntList           : PatchOdyssey.Collections.RefList<System     .UInt32>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntList          () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntList          (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntList          (System.Collections.Generic.IEnumerable<System     .UInt32>         enumerable) : base(enumerable) {} }
      [System.Serializable] public class ULongList          : PatchOdyssey.Collections.RefList<System     .UInt64>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongList         () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongList         (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongList         (System.Collections.Generic.IEnumerable<System     .UInt64>         enumerable) : base(enumerable) {} }
      [System.Serializable] public class Vector2List        : PatchOdyssey.Collections.RefList<UnityEngine.Vector2>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2List       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2List       (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2List       (System.Collections.Generic.IEnumerable<UnityEngine.Vector2>        enumerable) : base(enumerable) {} }
      [System.Serializable] public class Vector2IntList     : PatchOdyssey.Collections.RefList<UnityEngine.Vector2Int>     { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntList    () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntList    (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntList    (System.Collections.Generic.IEnumerable<UnityEngine.Vector2Int>     enumerable) : base(enumerable) {} }
      [System.Serializable] public class Vector3List        : PatchOdyssey.Collections.RefList<UnityEngine.Vector3>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3List       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3List       (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3List       (System.Collections.Generic.IEnumerable<UnityEngine.Vector3>        enumerable) : base(enumerable) {} }
      [System.Serializable] public class Vector3IntList     : PatchOdyssey.Collections.RefList<UnityEngine.Vector3Int>     { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntList    () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntList    (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntList    (System.Collections.Generic.IEnumerable<UnityEngine.Vector3Int>     enumerable) : base(enumerable) {} }
      [System.Serializable] public class Vector4List        : PatchOdyssey.Collections.RefList<UnityEngine.Vector4>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4List       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4List       (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4List       (System.Collections.Generic.IEnumerable<UnityEngine.Vector4>        enumerable) : base(enumerable) {} }

    public /* sealed */ class RefReadOnlyComparer<T> : System.Collections.Generic.Comparer<T>, PatchOdyssey.Collections.IRefReadOnlyComparer<T>, System.Collections.Generic.IComparer<T>, System.Collections.IComparer {
      private abstract class Sentinel : PatchOdyssey.Collections.IRefReadOnlyComparable<Sentinel> {
        int PatchOdyssey.Collections.IRefReadOnlyComparable<Sentinel>.CompareTo(in Sentinel value) => default;
        int System.IComparable.CompareTo                                       (object?     value) => default;
        int System.IComparable<Sentinel>.CompareTo                             (Sentinel    value) => default;
      }

      /* … */
      private static readonly PatchOdyssey.RefReadOnlyComparison<T> CompareValue = (PatchOdyssey.RefReadOnlyComparison<T>) (
        typeof(PatchOdyssey.Collections.IRefReadOnlyComparable<T>).IsAssignableFrom(typeof(T)) ? ((PatchOdyssey.RefReadOnlyComparison<RefReadOnlyComparer<T>.Sentinel>) RefReadOnlyComparer<RefReadOnlyComparer<T>.Sentinel>.RefReadOnlyComparableCompare).Method.GetGenericMethodDefinition().MakeGenericMethod(typeof(T)).CreateDelegate(typeof(PatchOdyssey.RefReadOnlyComparison<T>)) :
        typeof(System.IComparable                             <T>).IsAssignableFrom(typeof(T)) ? ((PatchOdyssey.RefReadOnlyComparison<RefReadOnlyComparer<T>.Sentinel>) RefReadOnlyComparer<RefReadOnlyComparer<T>.Sentinel>.ComparableCompare)           .Method.GetGenericMethodDefinition().MakeGenericMethod(typeof(T)).CreateDelegate(typeof(PatchOdyssey.RefReadOnlyComparison<T>)) :
        typeof(System.IComparable)                                .IsAssignableFrom(typeof(T)) ? ((PatchOdyssey.RefReadOnlyComparison<RefReadOnlyComparer<T>.Sentinel>) RefReadOnlyComparer<RefReadOnlyComparer<T>.Sentinel>.ComparableCompare2)          .Method.GetGenericMethodDefinition().MakeGenericMethod(typeof(T)).CreateDelegate(typeof(PatchOdyssey.RefReadOnlyComparison<T>)) :
        (PatchOdyssey.RefReadOnlyComparison<T>) RefReadOnlyComparer<T>.ObjectCompare<T>
      );
      protected        readonly PatchOdyssey.RefReadOnlyComparison<T> comparison       = RefReadOnlyComparer<T>.CompareValue;
      public    static new      RefReadOnlyComparer               <T> Default { get; } = new();

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)] public    RefReadOnlyComparer()                                                 : base() {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] protected RefReadOnlyComparer(PatchOdyssey.RefReadOnlyComparison<T> comparison) : base() => this.comparison = comparison;

      /* … */
      [PatchMethod(AggressiveInlining)] private static   int                    ComparableCompare <U>                                   (in U                                  a, in U    b) where U : System.IComparable<U>                              => a .CompareTo(b);
      [PatchMethod(AggressiveInlining)] private static   int                    ComparableCompare2<U>                                   (in U                                  a, in U    b) where U : System.IComparable                                 => a!.CompareTo(b);
      [PatchMethod(AggressiveInlining)] public  virtual  int                    Compare                                                 (in T                                  a, in T    b)                                                              { int comparison = this.comparison(in a, in b); return PatchOdyssey.Collections.RefReadOnlyEqualityComparer<T>.Default.Equals(in a, in b) ? 0 : comparison; }
      [PatchMethod(AggressiveInlining)] public  override int                    Compare                                                 (T                                     a, T       b)                                                              => a is null ? (b is null ? 0 : -1) : b is null ? (a is null ? 0 : +1) : this.Compare(in a, in b);
      [PatchMethod(AggressiveInlining)] public  static   RefReadOnlyComparer<T> Create                                                  (PatchOdyssey.RefReadOnlyComparison<T> comparison)                                                                => new(comparison);
      [PatchMethod(AggressiveInlining)] private static   int                    ObjectCompare               <U>                         (in U                                  a, in U    b)                                                              => System.Collections.Comparer.Default.Compare(a, b);
      [PatchMethod(AggressiveInlining)] private static   int                    RefReadOnlyComparableCompare<U>                         (in U                                  a, in U    b) where U : PatchOdyssey.Collections.IRefReadOnlyComparable<U> => a.CompareTo(in b);
      [PatchMethod(AggressiveInlining)] int                                     PatchOdyssey.Collections.IRefReadOnlyComparer<T>.Compare(in T                                  a, in T    b)                                                              => this.Compare(in a, in b);
      [PatchMethod(AggressiveInlining)] int                                     System.Collections.Generic.IComparer<T>.Compare         (T?                                    a, T?      b)                                                              => a is null ? (b is null ? 0 : -1) : b is null ? (a is null ? 0 : +1) : this.Compare(in a!, in b!);
      [PatchMethod(AggressiveInlining)] int                                     System.Collections.IComparer.Compare                    (object?                               a, object? b)                                                              => a is null ? (b is null ? 0 : -1) : b is null ? (a is null ? 0 : +1) : this.Compare((T) a, (T) b);
    }

    [System.Serializable]
    public class RefReadOnlyDictionary<TKey, TValue> : PatchOdyssey.Collections.RefDictionary<TKey, TValue>, PatchOdyssey.Collections.IRefEquatable<RefReadOnlyDictionary<TKey, TValue>>, System.Collections.Generic.IDictionary<TKey, TValue>, System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>, System.Collections.IDictionary /* ⟶ Retroactively designed after `RefDictionary<…>`’s implementation */ {
      public new struct Enumerator : System.Collections.Generic.IEnumerator<PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>>, System.Collections.IDictionaryEnumerator /* ⟶ Based on `System.Collections.Generic.Dictionary<TKey, TValue>.KeyCollection.Enumerator` */ {
        private class Iterator { public PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>[] value = new PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>[1]; }

        public  readonly ref                                           PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue> Current { get { Util.Reference<PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>>.Only(this.current.value) = new PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>(this.enumerator.Current); return ref Util.Reference<PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>>.Only(this.current.value); } }
        private                                                        Enumerator.Iterator                                            current;
        private readonly                                               RefDictionary<TKey, TValue>.Enumerator                         enumerator;
        PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>                                                                System.Collections.Generic.IEnumerator<PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>>.Current => this.Current;
        System.Collections.DictionaryEntry                                                                                            System.Collections.IDictionaryEnumerator.Entry                                                                 => new(this.Current.Key, this.Current.Value);
        object                                                                                                                        System.Collections.IDictionaryEnumerator.Key                                                                   => this.Current.Key!;
        object?                                                                                                                       System.Collections.IDictionaryEnumerator.Value                                                                 => this.Current.Value;
        object                                                                                                                        System.Collections.IEnumerator.Current                                                                         => this.Current!;

        /* … */
        [PatchConstructor, PatchMethod(AggressiveInlining)]
        internal Enumerator(RefReadOnlyDictionary<TKey, TValue> dictionary) {
          this.current    = new();
          this.enumerator = ((PatchOdyssey.Collections.RefDictionary<TKey, TValue>) dictionary).GetEnumerator();
        }

        /* … */
        [PatchMethod(AggressiveInlining)] public void Dispose                                () => this.enumerator.Dispose ();
        [PatchMethod(AggressiveInlining)] public bool MoveNext                               () => this.enumerator.MoveNext();
        [PatchMethod(AggressiveInlining)] public void Reset                                  () => this.enumerator.Reset   ();
        [PatchMethod(AggressiveInlining)] bool        System.Collections.IEnumerator.MoveNext() => this           .MoveNext();
        [PatchMethod(AggressiveInlining)] void        System.Collections.IEnumerator.Reset   () => this           .Reset   ();
        [PatchMethod(AggressiveInlining)] void        System.IDisposable.Dispose             () => this           .Dispose ();
      }

      public new sealed class KeyCollection : PatchOdyssey.Collections.RefDictionary<TKey, TValue>.KeyCollection {
        // ⟶ Aliases `PatchOdyssey.Collections.RefDictionary<TKey, TValue>.KeyCollection.Enumerator` for its `struct Enumerator` subtype
        [PatchMethod(AggressiveInlining)]
        public KeyCollection(RefReadOnlyDictionary<TKey, TValue> dictionary) : base((PatchOdyssey.Collections.RefDictionary<TKey, TValue>) dictionary) {}
      }

      public new sealed class ValueCollection : PatchOdyssey.Collections.RefDictionary<TKey, TValue>.ValueCollection {
        public new struct Enumerator : System.Collections.Generic.IEnumerator<TValue> /* ⟶ Based on `System.Collections.Generic.Dictionary<TKey, TValue>.ValueCollection.Enumerator` */ {
          public  ref readonly TValue                                                 Current => ref this.enumerator.Current;
          private readonly     RefDictionary<TKey, TValue>.ValueCollection.Enumerator enumerator;
          TValue                                                                      System.Collections.Generic.IEnumerator<TValue>.Current => this.Current;
          object                                                                      System.Collections.IEnumerator.Current                 => this.Current!;

          /* … */
          [PatchConstructor, PatchMethod(AggressiveInlining)]
          internal Enumerator(RefDictionary<TKey, TValue> dictionary) => this.enumerator = dictionary.Values.GetEnumerator();

          /* … */
          [PatchMethod(AggressiveInlining)] public void Dispose                                () => this.enumerator.Dispose ();
          [PatchMethod(AggressiveInlining)] public bool MoveNext                               () => this.enumerator.MoveNext();
          [PatchMethod(AggressiveInlining)] public void Reset                                  () => this.enumerator.Reset   ();
          [PatchMethod(AggressiveInlining)] bool        System.Collections.IEnumerator.MoveNext() => this           .MoveNext();
          [PatchMethod(AggressiveInlining)] void        System.Collections.IEnumerator.Reset   () => this           .Reset   ();
          [PatchMethod(AggressiveInlining)] void        System.IDisposable.Dispose             () => this           .Dispose ();
        }

        /* … */
        [PatchMethod(AggressiveInlining)]
        public ValueCollection(RefReadOnlyDictionary<TKey, TValue> dictionary) : base((PatchOdyssey.Collections.RefDictionary<TKey, TValue>) dictionary) {}

        /* … */
        [PatchMethod(AggressiveInlining)]
        public new ValueCollection.Enumerator GetEnumerator() => new(base.dictionary);
      }

      public static readonly RefReadOnlyDictionary<TKey, TValue>                 Empty                                                                                                       =  new();
      public new             RefReadOnlyDictionary<TKey, TValue>.KeyCollection   Keys                                                                                                        => new(this);
      public new             RefReadOnlyDictionary<TKey, TValue>.ValueCollection Values                                                                                                      => new(this);
      bool                                                                       System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.IsReadOnly    => true;
      int                                                                        System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Count         => ((int) base.Count);
      System.Collections.Generic.ICollection<TKey>                               System.Collections.Generic.IDictionary<TKey, TValue>.Keys                                                   => this.Keys;
      System.Collections.Generic.ICollection<TValue>                             System.Collections.Generic.IDictionary<TKey, TValue>.Values                                                 => this.Values;
      int                                                                        System.Collections.Generic.IReadOnlyCollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Count => ((int) base.Count);
      System.Collections.Generic.IEnumerable<TKey>                               System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.Keys                                           => this.Keys;
      System.Collections.Generic.IEnumerable<TValue>                             System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.Values                                         => this.Values;
      bool                                                                       System.Collections.IDictionary.IsFixedSize                                                                  => false;
      bool                                                                       System.Collections.IDictionary.IsReadOnly                                                                   => true;
      System.Collections.ICollection                                             System.Collections.IDictionary.Keys                                                                         => this.Keys;
      System.Collections.ICollection                                             System.Collections.IDictionary.Values                                                                       => this.Values;

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RefReadOnlyDictionary()                                                                                                                                                                                               : base()                                                                                 {}
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RefReadOnlyDictionary(PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<TKey>                                                   comparer)                                                                         : base(RefReadOnlyDictionary<TKey, TValue>.AsDangerousRefComparer(comparer))             {}
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RefReadOnlyDictionary(PatchOdyssey.Collections.RefDictionary               <TKey, TValue>                                           dictionary)                                                                       : base(dictionary)                                                                       {}
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public RefReadOnlyDictionary(System.Collections.Generic.IEnumerable               <PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>> enumerable)                                                                       : base(enumerable)                                                                       {}
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RefReadOnlyDictionary(System.Collections.Generic.IEnumerable               <System.Collections.Generic.KeyValuePair <TKey, TValue>> enumerable)                                                                       : base(enumerable)                                                                       {}
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RefReadOnlyDictionary(PatchOdyssey.Collections.RefDictionary               <TKey, TValue>                                           dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<TKey> comparer) : base(dictionary, RefReadOnlyDictionary<TKey, TValue>.AsDangerousRefComparer(comparer)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public RefReadOnlyDictionary(System.Collections.Generic.IEnumerable               <PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>> enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<TKey> comparer) : base(enumerable, RefReadOnlyDictionary<TKey, TValue>.AsDangerousRefComparer(comparer)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RefReadOnlyDictionary(System.Collections.Generic.IEnumerable               <System.Collections.Generic.KeyValuePair <TKey, TValue>> enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<TKey> comparer) : base(enumerable, RefReadOnlyDictionary<TKey, TValue>.AsDangerousRefComparer(comparer)) {}

      /* … */
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public  new    void                                               Add                                                                                       (in PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>                                                             element)              => this.Add(element.Key, element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  new    void                                               Add                                                                                       (in System.Collections.Generic.KeyValuePair <TKey, TValue>                                                             element)              => this.Add(element.Key, element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  new    void                                               Add                                                                                       (in TKey                                                                                                               key, in TValue value) => throw new System.NotSupportedException("Dictionary is read-only");
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  new    void                                               AddRange                                                                                  (System.Collections.Generic.IEnumerable               <PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>>         enumerable)           => throw new System.NotSupportedException("Dictionary is read-only");
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  new    void                                               AddRange                                                                                  (System.Collections.Generic.IEnumerable               <PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>> enumerable)           => throw new System.NotSupportedException("Dictionary is read-only");
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  new    void                                               AddRange                                                                                  (System.Collections.Generic.IEnumerable               <System.Collections.Generic.KeyValuePair<TKey, TValue>>          enumerable)           => throw new System.NotSupportedException("Dictionary is read-only");
      [PatchMethod(AggressiveInlining), PatchResolution(0)] private static PatchOdyssey.Collections.RefEqualityComparer<TKey> AsDangerousRefComparer                                                                    (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<TKey>                                                           comparer)             => PatchOdyssey.Collections.RefEqualityComparer<TKey>.Create([PatchMethod(AggressiveInlining)] (ref TKey a, ref TKey b) => comparer.Equals(in a, in b), [PatchMethod(AggressiveInlining)] (ref TKey value) => comparer.GetHashCode(in value));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  new    void                                               Clear                                                                                     ()                                                                                                                                           => throw new System.NotSupportedException("Dictionary is read-only");
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public         bool                                               Equals                                                                                    (in RefReadOnlyDictionary<TKey, TValue> dictionary)                                                                                          => base.Equals((PatchOdyssey.Collections.RefDictionary<TKey, TValue>) dictionary);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  new    RefReadOnlyDictionary<TKey, TValue>.Enumerator     GetEnumerator                                                                             ()                                                                                                                                           => new(this); // ⟶ `System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>>`
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  new    bool                                               Remove                                                                                    (in TKey                                                   key)                                                                              => throw new System.NotSupportedException("Dictionary is read-only");
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public  new    bool                                               Remove                                                                                    (in PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue> element)                                                                          => throw new System.NotSupportedException("Dictionary is read-only");
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  new    bool                                               Remove                                                                                    (in System.Collections.Generic.KeyValuePair <TKey, TValue> element)                                                                          => throw new System.NotSupportedException("Dictionary is read-only");
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public  new    ref readonly TValue                                TryAppend                                                                                 (in PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue> element)                                                                          => throw new System.NotSupportedException("Dictionary is read-only");
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  new    ref readonly TValue                                TryAppend                                                                                 (in System.Collections.Generic.KeyValuePair <TKey, TValue> element)                                                                          => throw new System.NotSupportedException("Dictionary is read-only");
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  new    ref readonly TValue                                TryAppend                                                                                 (in TKey                                                   key, in TValue value)                                                             => throw new System.NotSupportedException("Dictionary is read-only");
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public  new    bool                                               TryAdd                                                                                    (in PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue> element)                                                                          => this.TryAdd(in element.Key, in element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  new    bool                                               TryAdd                                                                                    (in System.Collections.Generic.KeyValuePair <TKey, TValue> element)                                                                          => this.TryAdd(element.Key, element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public  new    bool                                               TryAdd                                                                                    (in TKey                                                   key, in TValue value)                                                             => throw new System.NotSupportedException("Dictionary is read-only");
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                              PatchOdyssey.Collections.IRefEquatable<RefReadOnlyDictionary<TKey, TValue>>.Equals        (ref RefReadOnlyDictionary<TKey, TValue>                   dictionary)                                                                       => this.Equals     (in dictionary);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                              PatchOdyssey.Collections.IRefReadOnlyEquatable<RefReadOnlyDictionary<TKey, TValue>>.Equals(in  RefReadOnlyDictionary<TKey, TValue>                   dictionary)                                                                       => this.Equals     (in dictionary);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                              System.Collections.Generic.IDictionary<TKey, TValue>.Add                                  (TKey                                                      key, TValue    value)                                                             => this.Add        (in key, in value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                              System.Collections.Generic.IDictionary<TKey, TValue>.ContainsKey                          (TKey                                                      key)                                                                              => this.ContainsKey(in key);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                              System.Collections.Generic.IDictionary<TKey, TValue>.Remove                               (TKey                                                      key)                                                                              => this.Remove     (in key);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                              System.Collections.Generic.IDictionary<TKey, TValue>.TryGetValue                          (TKey                                                      key, out TValue value)                                                            => this.TryGetValue(in key, out value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                              System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.ContainsKey                  (TKey                                                      key)                                                                              => this.ContainsKey(in key);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                              System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.TryGetValue                  (TKey                                                      key,   out TValue value)                                                          => this.TryGetValue(in key, out value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                              System.Collections.ICollection.CopyTo                                                     (System.Array                                              array, int        index)                                                          { foreach (ref readonly PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue> element in this) array.SetValue(element, index++); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                              System.Collections.IDictionary.Add                                                        (object                                                    key,   object?    value)                                                          => this.Add          ((TKey) key, (TValue) value!);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                              System.Collections.IDictionary.Clear                                                      ()                                                                                                                                           => this.Clear        ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                              System.Collections.IDictionary.Contains                                                   (object key)                                                                                                                                 => this.ContainsKey  ((TKey) key);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] System.Collections.IDictionaryEnumerator                          System.Collections.IDictionary.GetEnumerator                                              ()                                                                                                                                           => this.GetEnumerator();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                              System.Collections.IDictionary.Remove                                                     (object                              key)                                                                                                    => this.Remove       ((TKey) key);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                              System.IEquatable<RefReadOnlyDictionary<TKey, TValue>>.Equals                             (RefReadOnlyDictionary<TKey, TValue> dictionary)                                                                                             => this.Equals       (dictionary);

      TValue System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.this[TKey key] { [PatchMethod(AggressiveInlining)] get { int index = base.FindIndex(in key); if (index == -1) throw new System.Collections.Generic.KeyNotFoundException(key?.ToString() ?? string.Empty); return base.values[(uint) index]; } }
    }
      [System.Serializable] public class AnimationCurveReadOnlyDictionary : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.AnimationCurve> /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public AnimationCurveReadOnlyDictionary(PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public AnimationCurveReadOnlyDictionary(PatchOdyssey.Collections.RefDictionary<string, UnityEngine.AnimationCurve> dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public AnimationCurveReadOnlyDictionary(System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.AnimationCurve>> enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public AnimationCurveReadOnlyDictionary(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, UnityEngine.AnimationCurve>> enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class BooleanReadOnlyDictionary        : PatchOdyssey.Collections.RefReadOnlyDictionary<string, System     .Boolean>        /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BooleanReadOnlyDictionary       (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BooleanReadOnlyDictionary       (PatchOdyssey.Collections.RefDictionary<string, System     .Boolean>        dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public BooleanReadOnlyDictionary       (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, System     .Boolean>>        enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BooleanReadOnlyDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, System     .Boolean>>        enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class BoundsReadOnlyDictionary         : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.Bounds>         /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BoundsReadOnlyDictionary        (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BoundsReadOnlyDictionary        (PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Bounds>         dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public BoundsReadOnlyDictionary        (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.Bounds>>         enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BoundsReadOnlyDictionary        (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, UnityEngine.Bounds>>         enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class BoundsIntReadOnlyDictionary      : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.BoundsInt>      /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BoundsIntReadOnlyDictionary     (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BoundsIntReadOnlyDictionary     (PatchOdyssey.Collections.RefDictionary<string, UnityEngine.BoundsInt>      dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public BoundsIntReadOnlyDictionary     (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.BoundsInt>>      enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public BoundsIntReadOnlyDictionary     (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, UnityEngine.BoundsInt>>      enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class ColorReadOnlyDictionary          : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.Color>          /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public ColorReadOnlyDictionary         (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public ColorReadOnlyDictionary         (PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Color>          dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public ColorReadOnlyDictionary         (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.Color>>          enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public ColorReadOnlyDictionary         (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, UnityEngine.Color>>          enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class DoubleReadOnlyDictionary         : PatchOdyssey.Collections.RefReadOnlyDictionary<string, System     .Double>         /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public DoubleReadOnlyDictionary        (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public DoubleReadOnlyDictionary        (PatchOdyssey.Collections.RefDictionary<string, System     .Double>         dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public DoubleReadOnlyDictionary        (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, System     .Double>>         enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public DoubleReadOnlyDictionary        (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, System     .Double>>         enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class FloatReadOnlyDictionary          : PatchOdyssey.Collections.RefReadOnlyDictionary<string, System     .Single>         /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public FloatReadOnlyDictionary         (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public FloatReadOnlyDictionary         (PatchOdyssey.Collections.RefDictionary<string, System     .Single>         dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public FloatReadOnlyDictionary         (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, System     .Single>>         enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public FloatReadOnlyDictionary         (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, System     .Single>>         enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class GameObjectReadOnlyDictionary     : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.GameObject>     /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public GameObjectReadOnlyDictionary    (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public GameObjectReadOnlyDictionary    (PatchOdyssey.Collections.RefDictionary<string, UnityEngine.GameObject>     dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public GameObjectReadOnlyDictionary    (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.GameObject>>     enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public GameObjectReadOnlyDictionary    (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, UnityEngine.GameObject>>     enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class GradientReadOnlyDictionary       : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.Gradient>       /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public GradientReadOnlyDictionary      (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public GradientReadOnlyDictionary      (PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Gradient>       dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public GradientReadOnlyDictionary      (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.Gradient>>       enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public GradientReadOnlyDictionary      (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, UnityEngine.Gradient>>       enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class IntReadOnlyDictionary            : PatchOdyssey.Collections.RefReadOnlyDictionary<string, System     .Int32>          /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public IntReadOnlyDictionary           (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public IntReadOnlyDictionary           (PatchOdyssey.Collections.RefDictionary<string, System     .Int32>          dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public IntReadOnlyDictionary           (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, System     .Int32>>          enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public IntReadOnlyDictionary           (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, System     .Int32>>          enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class LongReadOnlyDictionary           : PatchOdyssey.Collections.RefReadOnlyDictionary<string, System     .Int64>          /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public LongReadOnlyDictionary          (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public LongReadOnlyDictionary          (PatchOdyssey.Collections.RefDictionary<string, System     .Int64>          dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public LongReadOnlyDictionary          (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, System     .Int64>>          enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public LongReadOnlyDictionary          (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, System     .Int64>>          enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class RectReadOnlyDictionary           : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.Rect>           /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RectReadOnlyDictionary          (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RectReadOnlyDictionary          (PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Rect>           dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public RectReadOnlyDictionary          (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.Rect>>           enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RectReadOnlyDictionary          (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, UnityEngine.Rect>>           enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class RectIntReadOnlyDictionary        : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.RectInt>        /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RectIntReadOnlyDictionary       (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RectIntReadOnlyDictionary       (PatchOdyssey.Collections.RefDictionary<string, UnityEngine.RectInt>        dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public RectIntReadOnlyDictionary       (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.RectInt>>        enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public RectIntReadOnlyDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, UnityEngine.RectInt>>        enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class StringReadOnlyDictionary         : PatchOdyssey.Collections.RefReadOnlyDictionary<string, System     .String>         /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public StringReadOnlyDictionary        (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public StringReadOnlyDictionary        (PatchOdyssey.Collections.RefDictionary<string, System     .String>         dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public StringReadOnlyDictionary        (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, System     .String>>         enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public StringReadOnlyDictionary        (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, System     .String>>         enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class UIntReadOnlyDictionary           : PatchOdyssey.Collections.RefReadOnlyDictionary<string, System     .UInt32>         /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public UIntReadOnlyDictionary          (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public UIntReadOnlyDictionary          (PatchOdyssey.Collections.RefDictionary<string, System     .UInt32>         dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public UIntReadOnlyDictionary          (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, System     .UInt32>>         enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public UIntReadOnlyDictionary          (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, System     .UInt32>>         enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class ULongReadOnlyDictionary          : PatchOdyssey.Collections.RefReadOnlyDictionary<string, System     .UInt64>         /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public ULongReadOnlyDictionary         (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public ULongReadOnlyDictionary         (PatchOdyssey.Collections.RefDictionary<string, System     .UInt64>         dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public ULongReadOnlyDictionary         (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, System     .UInt64>>         enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public ULongReadOnlyDictionary         (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, System     .UInt64>>         enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class Vector2ReadOnlyDictionary        : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.Vector2>        /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector2ReadOnlyDictionary       (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector2ReadOnlyDictionary       (PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Vector2>        dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public Vector2ReadOnlyDictionary       (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.Vector2>>        enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector2ReadOnlyDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, UnityEngine.Vector2>>        enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class Vector2IntReadOnlyDictionary     : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.Vector2Int>     /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector2IntReadOnlyDictionary    (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector2IntReadOnlyDictionary    (PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Vector2Int>     dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public Vector2IntReadOnlyDictionary    (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.Vector2Int>>     enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector2IntReadOnlyDictionary    (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, UnityEngine.Vector2Int>>     enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class Vector3ReadOnlyDictionary        : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.Vector3>        /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector3ReadOnlyDictionary       (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector3ReadOnlyDictionary       (PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Vector3>        dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public Vector3ReadOnlyDictionary       (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.Vector3>>        enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector3ReadOnlyDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, UnityEngine.Vector3>>        enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class Vector3IntReadOnlyDictionary     : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.Vector3Int>     /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector3IntReadOnlyDictionary    (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector3IntReadOnlyDictionary    (PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Vector3Int>     dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public Vector3IntReadOnlyDictionary    (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.Vector3Int>>     enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector3IntReadOnlyDictionary    (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, UnityEngine.Vector3Int>>     enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }
      [System.Serializable] public class Vector4ReadOnlyDictionary        : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.Vector4>        /* ⟶ `System.Collections.Generic.IDictionary<string, …>`, `System.Collections.Generic.IReadOnlyDictionary<string, …>, `System.Collections.IDictionary` */ { [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector4ReadOnlyDictionary       (PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector4ReadOnlyDictionary       (PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Vector4>        dictionary, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(dictionary, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(1)] public Vector4ReadOnlyDictionary       (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<string, UnityEngine.Vector4>>        enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining), PatchResolution(0)] public Vector4ReadOnlyDictionary       (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair <string, UnityEngine.Vector4>>        enumerable, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<string> comparer = null!) : base(enumerable, comparer) {} }

    public /* sealed */ class RefReadOnlyEqualityComparer<T> : System.Collections.Generic.EqualityComparer<T>, PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<T> {
      private abstract class Sentinel : PatchOdyssey.Collections.IRefReadOnlyEquatable<Sentinel> {
        bool PatchOdyssey.Collections.IRefReadOnlyEquatable<Sentinel>.Equals(in Sentinel value) => default;
        bool System.IEquatable<Sentinel>.Equals                             (Sentinel    value) => default;
      }

      /* … */
      private static readonly PatchOdyssey.RefReadOnlyHasher            <T> GetHashCodeValue = [PatchMethod(AggressiveInlining)] static (in T value) => value!.GetHashCode();
      private static readonly PatchOdyssey.RefReadOnlyEqualityComparison<T> EqualsValue      = (PatchOdyssey.RefReadOnlyEqualityComparison<T>) (
        typeof(PatchOdyssey.Collections.IRefReadOnlyEquatable<T>).IsAssignableFrom(typeof(T)) ? ((PatchOdyssey.RefReadOnlyEqualityComparison<RefReadOnlyEqualityComparer<T>.Sentinel>) RefReadOnlyEqualityComparer<RefReadOnlyEqualityComparer<T>.Sentinel>.RefReadOnlyEquatableEquals).Method.GetGenericMethodDefinition().MakeGenericMethod(typeof(T)).CreateDelegate(typeof(PatchOdyssey.RefReadOnlyEqualityComparison<T>)) :
        typeof(System.IEquatable                             <T>).IsAssignableFrom(typeof(T)) ? ((PatchOdyssey.RefReadOnlyEqualityComparison<RefReadOnlyEqualityComparer<T>.Sentinel>) RefReadOnlyEqualityComparer<RefReadOnlyEqualityComparer<T>.Sentinel>.EquatableEquals)           .Method.GetGenericMethodDefinition().MakeGenericMethod(typeof(T)).CreateDelegate(typeof(PatchOdyssey.RefReadOnlyEqualityComparison<T>)) :
        (PatchOdyssey.RefReadOnlyEqualityComparison<T>) RefReadOnlyEqualityComparer<T>.ObjectEquals<T>
      );

      protected        readonly PatchOdyssey.RefReadOnlyEqualityComparison<T> comparison       = RefReadOnlyEqualityComparer<T>.EqualsValue;
      protected        PatchOdyssey.RefReadOnlyHasher                     <T> hasher           = RefReadOnlyEqualityComparer<T>.GetHashCodeValue;
      public    static new RefReadOnlyEqualityComparer                    <T> Default { get; } = new();

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)] public    RefReadOnlyEqualityComparer()                                                                                                    : base() {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] protected RefReadOnlyEqualityComparer(PatchOdyssey.RefReadOnlyEqualityComparison<T> comparison, PatchOdyssey.RefReadOnlyHasher<T>? hasher) : base() { this.comparison = comparison; this.hasher = hasher ?? this.hasher; }

      /* … */
      [PatchMethod(AggressiveInlining)] public           static RefReadOnlyEqualityComparer<T> Create                                                              (PatchOdyssey.RefReadOnlyEqualityComparison<T> comparison, PatchOdyssey.RefReadOnlyHasher<T>? hasher = null)                    => new(comparison, hasher);
      [PatchMethod(AggressiveInlining)] public  virtual         bool                           Equals                                                              (in T                                          a, in T                                        b)                                => this.comparison(in a, in b);
      [PatchMethod(AggressiveInlining)] public  override        bool                           Equals                                                              (T                                             a, T                                           b)                                => a is null ? b is null : b is null ? a is null : this.Equals(in a, in b);
      [PatchMethod(AggressiveInlining)] private          static bool                           EquatableEquals<U>                                                  (in U                                          a, in U                                        b) where U : System.IEquatable<U> => a.Equals(b);
      [PatchMethod(AggressiveInlining)] public  virtual         int                            GetHashCode                                                         (in T                                          value)                                                                           => this.hasher     (in value);
      [PatchMethod(AggressiveInlining)] public  override        int                            GetHashCode                                                         (T                                             value)                                                                           => this.GetHashCode(in value!);
      [PatchMethod(AggressiveInlining)] private          static bool                           ObjectEquals              <U>                                       (in U                                          a, in U b)                                                                       => a is null ? b is null : b is null ? a is null : a.Equals(b);
      [PatchMethod(AggressiveInlining)] private          static bool                           RefReadOnlyEquatableEquals<U>                                       (in U                                          a, in U b) where U : PatchOdyssey.Collections.IRefReadOnlyEquatable<U>           => a.Equals(in b);
      [PatchMethod(AggressiveInlining)] bool                                                   PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<T>.Equals     (in T                                          a, in T b)                                                                       => this.Equals     (in a, in b);
      [PatchMethod(AggressiveInlining)] int                                                    PatchOdyssey.Collections.IRefReadOnlyEqualityComparer<T>.GetHashCode(in T                                          value)                                                                           => this.GetHashCode(in value);
      [PatchMethod(AggressiveInlining)] bool                                                   System.Collections.Generic.IEqualityComparer<T>.Equals              (T?                                            a, T? b)                                                                         => a is null ? b is null : b is null ? a is null : this.Equals(in a!, in b!);
      [PatchMethod(AggressiveInlining)] int                                                    System.Collections.Generic.IEqualityComparer<T>.GetHashCode         (T                                             value)                                                                           => this.GetHashCode(in value);
      [PatchMethod(AggressiveInlining)] bool                                                   System.Collections.IEqualityComparer.Equals                         (object?                                       a, object? b)                                                                    => a is null ? b is null : b is null ? a is null : this.Equals((T) a, (T) b);
      [PatchMethod(AggressiveInlining)] int                                                    System.Collections.IEqualityComparer.GetHashCode                    (object                                        value)                                                                           => this.GetHashCode((T) value);
    }

    public struct RefReadOnlyKeyValuePair<TKey, TValue> /* ⟶ See `PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>` */ {
      public  ref readonly TKey                                                   Key => ref this.pair.Key;
      private readonly     PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue> pair;
      public  ref readonly TValue                                                 Value => ref this.pair.Value;

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)] internal RefReadOnlyKeyValuePair(in PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue> pair)                                               => this.pair = pair;
      [PatchConstructor, PatchMethod(AggressiveInlining)] public   RefReadOnlyKeyValuePair(in TKey                                                   key, in TValue value) : this(new(in key, in value)) {}

      /* … */
      [PatchMethod(AggressiveInlining)] public          void   Deconstruct(out TKey key, out TValue value) => this.pair.Deconstruct(out key, out value);
      [PatchMethod(AggressiveInlining)] public override string ToString   ()                               => this.pair.ToString   ();

      [PatchMethod(AggressiveInlining)]
      public static implicit operator System.Collections.Generic.KeyValuePair<TKey, TValue>(in RefReadOnlyKeyValuePair<TKey, TValue> pair) => new(pair.Key, pair.Value);
    }

    [System.Serializable]
    public class RefReadOnlyList<T> : PatchOdyssey.Collections.IRefEquatable<RefReadOnlyList<T>>, System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IReadOnlyList<T>, System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable, System.ICloneable /* ⟶ Based on `System.Collections.Generic.List<T>` and `System.Collections.ObjectModel.ReadOnlyCollection<T>` */ {
      public struct Enumerator : System.Collections.Generic.IEnumerator<T> {
        public   ref readonly T                  Current => ref this.list.GetValue((uint) this.index);
        internal              int                index;
        internal readonly     RefReadOnlyList<T> list;
        T                                        System.Collections.Generic.IEnumerator<T>.Current => this.Current;
        object                                   System.Collections.IEnumerator.Current            => this.Current!;

        /* … */
        [PatchConstructor, PatchMethod(AggressiveInlining)]
        public Enumerator(RefReadOnlyList<T> list) {
          this.Reset();
          this.list = list;
        }

        /* … */
        [PatchMethod(AggressiveInlining)] public void Dispose                                () { /* Do nothing… */ }
        [PatchMethod(AggressiveInlining)] public bool MoveNext                               () => this.index < this.list.Count - 1 ? (++this.index, _: true)._ : false;
        [PatchMethod(AggressiveInlining)] public void Reset                                  () => this.index = -1;
        [PatchMethod(AggressiveInlining)] bool        System.Collections.IEnumerator.MoveNext() => this.MoveNext();
        [PatchMethod(AggressiveInlining)] void        System.Collections.IEnumerator.Reset   () => this.Reset   ();
        [PatchMethod(AggressiveInlining)] void        System.IDisposable.Dispose             () => this.Dispose ();
      }

      /* … */
      public                                                                    uint                        Capacity                                                => this.Count;
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] public          uint                        Count { get; internal set; }                            =  0u;
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] internal        T[]                         Items                                                   =  System.Array.Empty<T>();
      public                                                                    ref T                       Null                                                    => ref this.nullElement;
      private                                                                   T                           nullElement                                             =  default!;
      public                                                             static readonly RefReadOnlyList<T> Empty                                                   =  new();
      int                                                                                                   System.Collections.Generic.IReadOnlyCollection<T>.Count => ((int) this.Count);

      /* … ⟶ Availability of `System.Runtime.InteropServices.CollectionMarshal.AsSpan(…)` would replace `RefReadOnlyList<T>`’s entire purpose */
      [PatchConstructor, PatchMethod(AggressiveInlining)] public             RefReadOnlyList()                                                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] protected          RefReadOnlyList(uint               capacity)                       => this.Items = RefReadOnlyList<T>.CreateInstance(capacity);
      [PatchConstructor, PatchMethod(AggressiveInlining)] public             RefReadOnlyList(RefReadOnlyList<T> list)                           => this.Count = (uint) (this.Items = (T[]) list.Items.Clone())                                                        .Length;
      [PatchConstructor, PatchMethod(AggressiveInlining)] protected internal RefReadOnlyList(T[]                array)                          => this.Count = (uint) (this.Items = array)                                                                           .Length;
      [PatchConstructor, PatchMethod(AggressiveInlining)] protected          RefReadOnlyList(T[]                array, uint index, uint length) => this.Count = (uint) (this.Items = Util.Array<T>.From(new System.ArraySegment<T>(array, (int) index, (int) length))).Length;

      [PatchConstructor, PatchMethod(AggressiveInlining)]
      public RefReadOnlyList(System.Collections.Generic.IEnumerable<T> enumerable) {
        this.Count = Util.EnumerableCount(enumerable);

        if (!this.IsEmpty()) {
          this.Items = new T[this.Count];

          using (System.Collections.Generic.IEnumerator<T> enumerator = enumerable.GetEnumerator()) {
            for (uint index = 0u; enumerator.MoveNext(); ++index)
            this.SetValue(enumerator.Current, index);
          }
        }
      }

      /* … */
      [PatchMethod(AggressiveInlining)] public            RefReadOnlyList<T>            AsCopy                                                                   ()                                                                                                                                                                  => new(this);
      [PatchMethod(AggressiveInlining)] public            RefReadOnlyList<T>            AsReadOnly                                                               ()                                                                                                                                                                  =>     this;
      [PatchMethod(AggressiveInlining)] public            int                           BinarySearch                                                             (in T element)                                                                                                                                                      => this.BinarySearch(0u, this.Count, in element, null as PatchOdyssey.Collections.IRefReadOnlyComparer<T>);
      [PatchMethod(AggressiveInlining)] public            int                           BinarySearch                                                             (in T element, PatchOdyssey.Collections.IRefReadOnlyComparer<T>? comparer)                                                                                          => this.BinarySearch(0u, this.Count, in element, comparer);
      [PatchMethod(AggressiveInlining)] public            int                           BinarySearch                                                             (in T element, System.Collections.Generic.IComparer         <T>? comparer)                                                                                          => this.BinarySearch(0u, this.Count, in element, comparer);
      [PatchMethod(AggressiveInlining)] public            int                           BinarySearch                                                             (uint index,   uint                                              length, in T element, PatchOdyssey.Collections.IRefReadOnlyComparer<T>? comparer)                  { for (length = System.Math.Min(length, this.Count); index <= length; ) { uint subindex = index + ((length - index) >>> 1); int comparison = (comparer ?? PatchOdyssey.Collections.RefReadOnlyComparer<T>.Default).Compare(in this.GetValue(subindex), in element); if (comparison < 0) { index  = subindex + 1u; continue; } if (comparison > 0) { length = subindex - 1u; continue; } return (int) subindex; } return -1; } // ⟶ Requires pre-sorted in ascending order
      [PatchMethod(AggressiveInlining)] public            int                           BinarySearch                                                             (uint index,   uint                                              length, in T element, System.Collections.Generic.IComparer         <T>? comparer)                  => this.BinarySearch(index, length, in element, comparer is not null ? PatchOdyssey.Collections.RefReadOnlyComparer<T>.Create([PatchMethod(AggressiveInlining)] (in T a, in T b) => comparer.Compare(a, b)) : null);
      [PatchMethod(AggressiveInlining)] public            object                        Clone                                                                    ()                                                                                                                                                                  => this      .AsCopy      ();
      [PatchMethod(AggressiveInlining)] public            bool                          Contains                                                                 (in T                                    element)                                                                                                                   => this      .IndexOf     (element, 0u, this.Count) != -1;
      [PatchMethod(AggressiveInlining)] public            RefReadOnlyList<U>            ConvertAll<U>                                                            (PatchOdyssey.RefReadOnlyConverter<T, U> converter)                                                                                                                 { RefReadOnlyList<U> list = new(this.Count); for (; list.Count != this.Count; ++list.Count) { list.SetValue(converter(in this.GetValue(list.Count)), list.Count); } return list; }
      [PatchMethod(AggressiveInlining)] public            RefReadOnlyList<U>            ConvertAll<U>                                                            (System.Converter                 <T, U> converter)                                                                                                                 => this.ConvertAll([PatchMethod(AggressiveInlining)] (in T element) => converter(element));
      [PatchMethod(AggressiveInlining)] public            void                          CopyTo                                                                   (T[]                                     array)                                                                                                                     => this.CopyTo    (0u, array, 0u,    this.Count);
      [PatchMethod(AggressiveInlining)] public            void                          CopyTo                                                                   (T[]                                     array, uint index)                                                                                                         => this.CopyTo    (0u, array, index, this.Count);
      [PatchMethod(AggressiveInlining)] public            void                          CopyTo                                                                   (uint                                    index, T[]  array, uint arrayIndex, uint length)                                                                           => Util.Array<T>.Copy(this.Items, index, array, arrayIndex, length); // ⟶ Possible over-read
      [PatchMethod(AggressiveInlining)] internal static   T[]                           CreateInstance                                                           (uint                                    length)                                                                                                                    => 0u != length ? new T[length] : System.Array.Empty<T>(); // ⟶ Faster with `System.GC.AllocateUninitializedArray<T>(…, false)`
      [PatchMethod(AggressiveInlining)] public            bool                          Equals                                                                   (in RefReadOnlyList               <T>    list)                                                                                                                      => base.Equals((PatchOdyssey.Collections.RefList<T>) list);
      [PatchMethod(AggressiveInlining)] public            bool                          Exists                                                                   (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                                                 { for (uint index = 0u; index != this.Count; ++index) { if (predicate(in this.GetValue(index))) return true; } return false; }
      [PatchMethod(AggressiveInlining)] public            bool                          Exists                                                                   (System.Predicate                 <T>    predicate)                                                                                                                 => this.Exists([PatchMethod(AggressiveInlining)] (in T element) => predicate(element));
      [PatchMethod(AggressiveInlining)] public            ref T                         Find                                                                     (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                                                 { for (uint index = 0u; index != this.Count; ++index) { ref T element = ref this.GetValue(index); if (predicate(in element)) return ref element; } return ref this.Null; }
      [PatchMethod(AggressiveInlining)] public            ref T                         Find                                                                     (System.Predicate                 <T>    predicate)                                                                                                                 => ref this.Find([PatchMethod(AggressiveInlining)] (in T element) => predicate(element));
      [PatchMethod(AggressiveInlining)] public            RefReadOnlyList<T>            FindAll                                                                  (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                                                 { RefReadOnlyList<T> list = new(this.Count); for (uint index = 0u; index != this.Count; ++index) { ref readonly T element = ref this.GetValue(index); if (predicate(in element)) list.SetValue(in element, list.Count++); } return list; }
      [PatchMethod(AggressiveInlining)] public            RefReadOnlyList<T>            FindAll                                                                  (System.Predicate                 <T>    predicate)                                                                                                                 => this.FindAll  ([PatchMethod(AggressiveInlining)] (in T element) => predicate(element));
      [PatchMethod(AggressiveInlining)] public            int                           FindIndex                                                                (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                                                 => this.FindIndex(0u, this.Count, predicate);
      [PatchMethod(AggressiveInlining)] public            int                           FindIndex                                                                (System.Predicate                 <T>    predicate)                                                                                                                 => this.FindIndex(0u, this.Count, predicate);
      [PatchMethod(AggressiveInlining)] public            int                           FindIndex                                                                (uint                                    index, PatchOdyssey.RefReadOnlyPredicate<T> predicate)                                                                     { for (; index < this.Count; ++index) { if (predicate(in this.GetValue(index))) return (int) index; } return -1; }
      [PatchMethod(AggressiveInlining)] public            int                           FindIndex                                                                (uint                                    index, System.Predicate                 <T> predicate)                                                                     => this.FindIndex(index, index <= this.Count ? this.Count - index : 0u, predicate);
      [PatchMethod(AggressiveInlining)] public            int                           FindIndex                                                                (uint                                    index, uint                                 length, PatchOdyssey.RefReadOnlyPredicate<T> predicate)                        { for (uint end = System.Math.Min(this.Count, index + length); end > index; ++index) { if (predicate(in this.GetValue(index))) return (int) index; } return -1; }
      [PatchMethod(AggressiveInlining)] public            int                           FindIndex                                                                (uint                                    index, uint                                 length, System.Predicate                 <T> predicate)                        => this.Items.FindIndex((int) index, (int) length, predicate);
      [PatchMethod(AggressiveInlining)] public            ref T                         FindLast                                                                 (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                                                 { for (uint index = this.Count; 0u != index--; ) { ref T element = ref this.GetValue(index); if (predicate(in element)) return ref element; } return ref this.Null; }
      [PatchMethod(AggressiveInlining)] public            ref T                         FindLast                                                                 (System.Predicate                 <T>    predicate)                                                                                                                 => ref this.FindLast     ([PatchMethod(AggressiveInlining)] (in T element) => predicate(element));
      [PatchMethod(AggressiveInlining)] public            int                           FindLastIndex                                                            (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                                                 =>     this.FindLastIndex(0u, this.Count, predicate);
      [PatchMethod(AggressiveInlining)] public            int                           FindLastIndex                                                            (System.Predicate                 <T>    predicate)                                                                                                                 =>     this.FindLastIndex(0u, this.Count, predicate);
      [PatchMethod(AggressiveInlining)] public            int                           FindLastIndex                                                            (uint                                    index, PatchOdyssey.RefReadOnlyPredicate<T> predicate)                                                                     { for (uint end = this.Count; end-- > index; ) { if (predicate(in this.GetValue(end))) return (int) end; } return -1; }
      [PatchMethod(AggressiveInlining)] public            int                           FindLastIndex                                                            (uint                                    index, System.Predicate                 <T> predicate)                                                                     => this.FindLastIndex(index, index <= this.Count ? this.Count - index : 0u, predicate);
      [PatchMethod(AggressiveInlining)] public            int                           FindLastIndex                                                            (uint                                    index, uint                                 length, PatchOdyssey.RefReadOnlyPredicate<T> predicate)                        { for (uint end = System.Math.Min(this.Count, index + length); end-- > index; ) { if (predicate(in this.GetValue(end))) return (int) end; } return -1; }
      [PatchMethod(AggressiveInlining)] public            int                           FindLastIndex                                                            (uint                                    index, uint                                 length, System.Predicate                 <T> predicate)                        => this.Items.FindLastIndex((int) index, (int) length, predicate);
      [PatchMethod(AggressiveInlining)] public            void                          ForEach                                                                  (PatchOdyssey.RefReadOnlyAction<T>       action)                                                                                                                    { for (uint index = 0u; index != this.Count; ++index) action(in this.GetValue(index)); }
      [PatchMethod(AggressiveInlining)] public            void                          ForEach                                                                  (System.Action                 <T>       action)                                                                                                                    => this.ForEach([PatchMethod(AggressiveInlining)] (in T element) => action(element));
      [PatchMethod(AggressiveInlining)] public            RefReadOnlyList<T>.Enumerator GetEnumerator                                                            ()                                                                                                                                                                  => new(this);
      [PatchMethod(AggressiveInlining)] public            RefReadOnlyList<T>            GetRange                                                                 (uint index, uint length)                                                                                                                                           => new(this.Items, index, length);
      [PatchMethod(AggressiveInlining)] public            ref T                         GetValue                                                                 (uint index)                                                                                                                                                        => ref Util.Reference<T>.At(this.Items, index);
      [PatchMethod(AggressiveInlining)] public            int                           IndexOf                                                                  (in T element)                                                                                                                                                      => this.IndexOf(in element, 0u,    this.Count);
      [PatchMethod(AggressiveInlining)] public            int                           IndexOf                                                                  (in T element, uint index)                                                                                                                                          => this.IndexOf(in element, index, this.Count);
      [PatchMethod(AggressiveInlining)] public            int                           IndexOf                                                                  (in T element, uint index, uint length)                                                                                                                             { for (uint end = System.Math.Min(this.Count, index + length); end > index; ++index) { if (PatchOdyssey.Collections.RefReadOnlyEqualityComparer<T>.Default.Equals(in this.GetValue(index), in element)) return (int) index; } return -1; }
      [PatchMethod(AggressiveInlining)] public            bool                          IsEmpty                                                                  ()                                                                                                                                                                  => 0u == this.Count;
      [PatchMethod(AggressiveInlining)] public            int                           LastIndexOf                                                              (in T element)                                                                                                                                                      => this.LastIndexOf(in element, 0u,    this.Count);
      [PatchMethod(AggressiveInlining)] public            int                           LastIndexOf                                                              (in T element, uint index)                                                                                                                                          => this.LastIndexOf(in element, index, this.Count);
      [PatchMethod(AggressiveInlining)] public            int                           LastIndexOf                                                              (in T element, uint index, uint length)                                                                                                                             { for (uint end = System.Math.Min(this.Count, index + length); end-- > index; ) { if (PatchOdyssey.Collections.RefReadOnlyEqualityComparer<T>.Default.Equals(in this.GetValue(end), in element)) return (int) end; } return -1; }
      [PatchMethod(AggressiveInlining)] public            void                          SetValue                                                                 (in T element, uint index)                                                                                                                                          => Util.Reference<T>.At(this.Items, index) = element;
      [PatchMethod(AggressiveInlining)] public            RefReadOnlyList<T>            Slice                                                                    (uint index,   uint length)                                                                                                                                         => new(this.Items, index, length);
      [PatchMethod(AggressiveInlining)] public            T[]                           ToArray                                                                  ()                                                                                                                                                                  { T[] array = (T[]) this.Items.Clone(); System.Array.Resize(ref array, (int) this.Count); return array; }
      [PatchMethod(AggressiveInlining)] public   override string?                       ToString                                                                 ()                                                                                                                                                                  { uint end = this.Count, index = 0u; if (end != index) unsafe { System.Text.StringBuilder builder = new(); for (char* separator = stackalloc char[] {',', ' '}; ; builder.Append(separator, 2)) { builder.Append(this.GetValue(index)); if (end == ++index) return builder.ToString(); } } return string.Empty; }
      [PatchMethod(AggressiveInlining)] public            bool                          TrueForAll                                                               (PatchOdyssey.RefReadOnlyPredicate<T> predicate)                                                                                                                    { for (uint index = 0u; index != this.Count; ++index) { if (!predicate(in this.GetValue(index))) return false; } return true; }
      [PatchMethod(AggressiveInlining)] public            bool                          TrueForAll                                                               (System.Predicate                 <T> predicate)                                                                                                                    => this.TrueForAll([PatchMethod(AggressiveInlining)] (in T element) => predicate(element));
      [PatchMethod(AggressiveInlining)] bool                                            PatchOdyssey.Collections.IRefEquatable<RefReadOnlyList<T>>.Equals        (ref RefReadOnlyList              <T> list)                                                                                                                         => this.Equals(in list);
      [PatchMethod(AggressiveInlining)] bool                                            PatchOdyssey.Collections.IRefReadOnlyEquatable<RefReadOnlyList<T>>.Equals(in  RefReadOnlyList              <T> list)                                                                                                                         => this.Equals(in list);
      [PatchMethod(AggressiveInlining)] System.Collections.Generic.IEnumerator<T>       System.Collections.Generic.IEnumerable<T>.GetEnumerator                  ()                                                                                                                                                                  => (System.Collections.Generic.IEnumerator<T>) this.GetEnumerator();
      [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator                  System.Collections.IEnumerable.GetEnumerator                             ()                                                                                                                                                                  => (System.Collections.IEnumerator)            this.GetEnumerator();
      [PatchMethod(AggressiveInlining)] int                                             System.Collections.IStructuralComparable.CompareTo                       (object?                              value, System.Collections.IComparer         comparer)                                                                         => comparer.Compare    (this, value);
      [PatchMethod(AggressiveInlining)] bool                                            System.Collections.IStructuralEquatable.Equals                           (object?                              value, System.Collections.IEqualityComparer comparer)                                                                         => comparer.Equals     (this, value);
      [PatchMethod(AggressiveInlining)] int                                             System.Collections.IStructuralEquatable.GetHashCode                      (System.Collections.IEqualityComparer comparer)                                                                                                                     => comparer.GetHashCode(this);
      [PatchMethod(AggressiveInlining)] object                                          System.ICloneable.Clone                                                  ()                                                                                                                                                                  => this    .Clone      ();
      [PatchMethod(AggressiveInlining)] bool                                            System.IEquatable<RefReadOnlyList<T>>.Equals                             (RefReadOnlyList<T> list)                                                                                                                                           => this    .Equals     (list);

      /* … */
      public ref readonly T     this                                            [uint         index] => ref this.GetValue(index);
      public RefReadOnlyList<T> this                                            [System.Range range] => new(this.Items[range]);
      T                         System.Collections.Generic.IReadOnlyList<T>.this[int          index] => this[(uint) index];
    }
      [System.Serializable] public class AnimationCurveReadOnlyList : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.AnimationCurve> { [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveReadOnlyList() : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveReadOnlyList(System.Collections.Generic.IEnumerable<UnityEngine.AnimationCurve> enumerable) : base(enumerable) {} }
      [System.Serializable] public class BooleanReadOnlyList        : PatchOdyssey.Collections.RefReadOnlyList<System     .Boolean>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanReadOnlyList       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanReadOnlyList       (System.Collections.Generic.IEnumerable<System     .Boolean>        enumerable) : base(enumerable) {} }
      [System.Serializable] public class BoundsReadOnlyList         : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.Bounds>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsReadOnlyList        () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsReadOnlyList        (System.Collections.Generic.IEnumerable<UnityEngine.Bounds>         enumerable) : base(enumerable) {} }
      [System.Serializable] public class BoundsIntReadOnlyList      : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.BoundsInt>      { [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntReadOnlyList     () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntReadOnlyList     (System.Collections.Generic.IEnumerable<UnityEngine.BoundsInt>      enumerable) : base(enumerable) {} }
      [System.Serializable] public class ColorReadOnlyList          : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.Color>          { [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorReadOnlyList         () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorReadOnlyList         (System.Collections.Generic.IEnumerable<UnityEngine.Color>          enumerable) : base(enumerable) {} }
      [System.Serializable] public class DoubleReadOnlyList         : PatchOdyssey.Collections.RefReadOnlyList<System     .Double>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleReadOnlyList        () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleReadOnlyList        (System.Collections.Generic.IEnumerable<System     .Double>         enumerable) : base(enumerable) {} }
      [System.Serializable] public class FloatReadOnlyList          : PatchOdyssey.Collections.RefReadOnlyList<System     .Single>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatReadOnlyList         () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatReadOnlyList         (System.Collections.Generic.IEnumerable<System     .Single>         enumerable) : base(enumerable) {} }
      [System.Serializable] public class GameObjectReadOnlyList     : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.GameObject>     { [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectReadOnlyList    () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectReadOnlyList    (System.Collections.Generic.IEnumerable<UnityEngine.GameObject>     enumerable) : base(enumerable) {} }
      [System.Serializable] public class GradientReadOnlyList       : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.Gradient>       { [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientReadOnlyList      () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientReadOnlyList      (System.Collections.Generic.IEnumerable<UnityEngine.Gradient>       enumerable) : base(enumerable) {} }
      [System.Serializable] public class IntReadOnlyList            : PatchOdyssey.Collections.RefReadOnlyList<System     .Int32>          { [PatchConstructor, PatchMethod(AggressiveInlining)] public IntReadOnlyList           () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public IntReadOnlyList           (System.Collections.Generic.IEnumerable<System     .Int32>          enumerable) : base(enumerable) {} }
      [System.Serializable] public class LongReadOnlyList           : PatchOdyssey.Collections.RefReadOnlyList<System     .Int64>          { [PatchConstructor, PatchMethod(AggressiveInlining)] public LongReadOnlyList          () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public LongReadOnlyList          (System.Collections.Generic.IEnumerable<System     .Int64>          enumerable) : base(enumerable) {} }
      [System.Serializable] public class RectReadOnlyList           : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.Rect>           { [PatchConstructor, PatchMethod(AggressiveInlining)] public RectReadOnlyList          () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectReadOnlyList          (System.Collections.Generic.IEnumerable<UnityEngine.Rect>           enumerable) : base(enumerable) {} }
      [System.Serializable] public class RectIntReadOnlyList        : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.RectInt>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntReadOnlyList       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntReadOnlyList       (System.Collections.Generic.IEnumerable<UnityEngine.RectInt>        enumerable) : base(enumerable) {} }
      [System.Serializable] public class StringReadOnlyList         : PatchOdyssey.Collections.RefReadOnlyList<System     .String>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public StringReadOnlyList        () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public StringReadOnlyList        (System.Collections.Generic.IEnumerable<System     .String>         enumerable) : base(enumerable) {} }
      [System.Serializable] public class UIntReadOnlyList           : PatchOdyssey.Collections.RefReadOnlyList<System     .UInt32>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntReadOnlyList          () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntReadOnlyList          (System.Collections.Generic.IEnumerable<System     .UInt32>         enumerable) : base(enumerable) {} }
      [System.Serializable] public class ULongReadOnlyList          : PatchOdyssey.Collections.RefReadOnlyList<System     .UInt64>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongReadOnlyList         () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongReadOnlyList         (System.Collections.Generic.IEnumerable<System     .UInt64>         enumerable) : base(enumerable) {} }
      [System.Serializable] public class Vector2ReadOnlyList        : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.Vector2>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2ReadOnlyList       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2ReadOnlyList       (System.Collections.Generic.IEnumerable<UnityEngine.Vector2>        enumerable) : base(enumerable) {} }
      [System.Serializable] public class Vector2IntReadOnlyList     : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.Vector2Int>     { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntReadOnlyList    () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntReadOnlyList    (System.Collections.Generic.IEnumerable<UnityEngine.Vector2Int>     enumerable) : base(enumerable) {} }
      [System.Serializable] public class Vector3ReadOnlyList        : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.Vector3>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3ReadOnlyList       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3ReadOnlyList       (System.Collections.Generic.IEnumerable<UnityEngine.Vector3>        enumerable) : base(enumerable) {} }
      [System.Serializable] public class Vector3IntReadOnlyList     : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.Vector3Int>     { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntReadOnlyList    () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntReadOnlyList    (System.Collections.Generic.IEnumerable<UnityEngine.Vector3Int>     enumerable) : base(enumerable) {} }
      [System.Serializable] public class Vector4ReadOnlyList        : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.Vector4>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4ReadOnlyList       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4ReadOnlyList       (System.Collections.Generic.IEnumerable<UnityEngine.Vector4>        enumerable) : base(enumerable) {} }

    public class RefSortedCollection<TSort, T> : PatchOdyssey.Collections.IRefEquatable<RefSortedCollection<TSort, T>>, System.Collections.Generic.IList<T>, System.Collections.ICollection, System.Collections.IList, System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable, System.ICloneable /* ⟶ No read-only analogue */ {
      public  uint                                                 Capacity                                             => this.items.Capacity;
      public  PatchOdyssey.Collections.IRefReadOnlyComparer<TSort> Comparer { get; private init; }                      =  PatchOdyssey.Collections.RefComparer<TSort>.Default;
      public  uint                                                 Count                                                => this.items.Count;
      private PatchOdyssey.Collections.RefList        <T>          items                                                =  new();
      public  PatchOdyssey.Collections.RefReadOnlyList<TSort>      Keys                                                 => (PatchOdyssey.Collections.RefReadOnlyList<TSort>) this.ranks;
      private PatchOdyssey.Collections.RefList        <TSort>      ranks                                                =  new();
      public  PatchOdyssey.Collections.RefReadOnlyList<T>          Values                                               => (PatchOdyssey.Collections.RefReadOnlyList<T>)     this.items;
      int                                                          System.Collections.Generic.ICollection<T>.Count      => ((int) this.Count);
      bool                                                         System.Collections.Generic.ICollection<T>.IsReadOnly => false;
      int                                                          System.Collections.ICollection.Count                 => ((int) this.Count);
      bool                                                         System.Collections.ICollection.IsSynchronized        => false;
      object                                                       System.Collections.ICollection.SyncRoot              => this;
      bool                                                         System.Collections.IList.IsFixedSize                 => false;
      bool                                                         System.Collections.IList.IsReadOnly                  => false;

      /* … */
      [PatchMethod(AggressiveInlining)] public RefSortedCollection()                                                                                                                                  : this(0u,                      null!)    {}
      [PatchMethod(AggressiveInlining)] public RefSortedCollection(uint                                                    capacity)                                                                  : this(capacity,                null!)    {}
      [PatchMethod(AggressiveInlining)] public RefSortedCollection(PatchOdyssey.Collections.IRefReadOnlyComparer<TSort>    comparer)                                                                  : this(2u,                      comparer) {}
      [PatchMethod(AggressiveInlining)] public RefSortedCollection(System.Collections.Generic.IDictionary       <TSort, T> dictionary)                                                                : this((uint) dictionary.Count, null!)    {}
      [PatchMethod(AggressiveInlining)] public RefSortedCollection(System.Collections.Generic.IDictionary       <TSort, T> dictionary, PatchOdyssey.Collections.IRefReadOnlyComparer<TSort> comparer) : this((uint) dictionary.Count, comparer) { foreach (System.Collections.Generic.KeyValuePair<TSort, T> element in dictionary) this.Add(element); }
      [PatchMethod(AggressiveInlining)] public RefSortedCollection(uint                                                    capacity,   PatchOdyssey.Collections.IRefReadOnlyComparer<TSort> comparer)                                           { this.Comparer = comparer ?? this.Comparer; this.items.EnsureCapacity(capacity); this.ranks.EnsureCapacity(capacity); }

      /* … */
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          void                                           Add                                                                                 (in PatchOdyssey.Collections.RefKeyValuePair<TSort, T>                                              element)                                                         => this.Add(in element.Key, in element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                           Add                                                                                 (in System.Collections.Generic.KeyValuePair <TSort, T>                                              element)                                                         => this.Add(element.Key,    element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                           Add                                                                                 (in TSort                                                                                           rank, in T                                      element)         { this.ranks.Add(in rank); this.ranks.Sort(this.Comparer); this.items.Insert((uint) this.ranks.LastIndexOf(in rank), in element); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                           AddRange                                                                            (in TSort                                                                                           rank, System.Collections.Generic.IEnumerable<T> enumerable)      { foreach (T                                                          element in enumerable) this.Add(in rank, in element); }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          void                                           AddRange                                                                            (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<TSort, T>>         enumerable)                                                      { foreach (PatchOdyssey.Collections.RefKeyValuePair        <TSort, T> element in enumerable) this.Add(in element); }
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          void                                           AddRange                                                                            (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TSort, T>> enumerable)                                                      { foreach (PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TSort, T> element in enumerable) this.Add((System.Collections.Generic.KeyValuePair<TSort, T>) element); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                           AddRange                                                                            (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TSort, T>>          enumerable)                                                      { foreach (System.Collections.Generic.KeyValuePair         <TSort, T> element in enumerable) this.Add(in element); }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          ref T                                          Append                                                                              (in PatchOdyssey.Collections.RefKeyValuePair<TSort, T>                                              element)                                                         => ref this.Append(in element.Key, in element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          ref T                                          Append                                                                              (in System.Collections.Generic.KeyValuePair <TSort, T>                                              element)                                                         { (TSort rank, T element)[] pair = new[] {(element.Key, element.Value)}; return ref this.Append(in Util.Reference<(TSort rank, T)>.Only(pair).rank, in Util.Reference<(TSort, T element)>.Only(pair).element); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          ref T                                          Append                                                                              (in TSort                                                                                           rank, in T element)                                              { this.ranks.Add(in rank); this.ranks.Sort(this.Comparer); uint index = (uint) this.ranks.LastIndexOf(in rank); this.items.Insert(index, in element);    return ref this.items[index]; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          RefSortedCollection<TSort, T>                  AsCopy                                                                              ()                                                                                                                                                                   { RefSortedCollection<TSort, T> sorted = new(this.Count, this.Comparer); this.items.CopyTo(sorted.items.Items); this.ranks.CopyTo(sorted.ranks.Items); sorted.items.Count = sorted.ranks.Count = this.Count; return sorted; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          RefSortedCollection<TSort, T>                  AsReadOnly                                                                          ()                                                                                                                                                                   => this;
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            BinarySearch                                                                        (in T     element)                                                                                                                                                   => this.items.BinarySearch(in element);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          int                                            BinarySearch                                                                        (in T     element, PatchOdyssey.Collections.IRefReadOnlyComparer<T>? comparer)                                                                                       => this.items.BinarySearch(in element, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            BinarySearch                                                                        (in T     element, System.Collections.Generic.IComparer         <T>? comparer)                                                                                       => this.items.BinarySearch(in element, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          int                                            BinarySearch                                                                        (uint     index,   uint                                              length, in T element, PatchOdyssey.Collections.IRefReadOnlyComparer<T>? comparer)               => this.items.BinarySearch(index, length, in element, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            BinarySearch                                                                        (uint     index,   uint                                              length, in T element, System.Collections.Generic.IComparer         <T>? comparer)               => this.items.BinarySearch(index, length, in element, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            BinarySearchRank                                                                    (in TSort rank)                                                                                                                                                      => this.ranks.BinarySearch(in rank);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          int                                            BinarySearchRank                                                                    (in TSort rank,  PatchOdyssey.Collections.IRefReadOnlyComparer<TSort>? comparer)                                                                                     => this.ranks.BinarySearch(in rank, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            BinarySearchRank                                                                    (in TSort rank,  System.Collections.Generic.IComparer         <TSort>? comparer)                                                                                     => this.ranks.BinarySearch(in rank, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          int                                            BinarySearchRank                                                                    (uint     index, uint                                                  length, in TSort rank, PatchOdyssey.Collections.IRefReadOnlyComparer<TSort>? comparer)        => this.ranks.BinarySearch(index, length, in rank, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            BinarySearchRank                                                                    (uint     index, uint                                                  length, in TSort rank, System.Collections.Generic.IComparer         <TSort>? comparer)        => this.ranks.BinarySearch(index, length, in rank, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                           Clear                                                                               ()                                                                                                                                                                   { this.items.Clear(); this.ranks.Clear(); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          object                                         Clone                                                                               ()                                                                                                                                                                   => this      .AsCopy  ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                           Contains                                                                            (in T                                                    element)                                                                                                    => this.items.Contains(in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                           ContainsRank                                                                        (in TSort                                                rank)                                                                                                       => this.ranks.Contains(in rank);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          RefSortedCollection<TSort, U>                  ConvertAll<U>                                                                       (PatchOdyssey.RefConverter        <T, U>                 converter)                                                                                                  { RefSortedCollection<TSort, U> sorted = new(this.Count, this.Comparer); this.items.ConvertAll(converter).CopyTo(sorted.items.Items); this.ranks.CopyTo(sorted.ranks.Items); sorted.items.Count = sorted.ranks.Count = this.Count; return sorted; }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          RefSortedCollection<TSort, U>                  ConvertAll<U>                                                                       (PatchOdyssey.RefReadOnlyConverter<T, U>                 converter)                                                                                                  => this      .ConvertAll([PatchMethod(AggressiveInlining)] (ref T element) => converter(in element));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          RefSortedCollection<TSort, U>                  ConvertAll<U>                                                                       (System.Converter                 <T, U>                 converter)                                                                                                  => this      .ConvertAll([PatchMethod(AggressiveInlining)] (ref T element) => converter(element));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                           CopyTo                                                                              (T[]                                                     array)                                                                                                      => this.items.CopyTo    (array);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          void                                           CopyTo                                                                              (in PatchOdyssey.Collections.RefKeyValuePair<TSort, T>[] array)                                                                                                      => this      .CopyTo    (array, 0u);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                           CopyTo                                                                              (in System.Collections.Generic.KeyValuePair <TSort, T>[] array)                                                                                                      => this      .CopyTo    (array, 0u);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                           CopyTo                                                                              (T[]                                                     array,    uint index)                                                                                       => this.items.CopyTo    (array, index);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          void                                           CopyTo                                                                              (in PatchOdyssey.Collections.RefKeyValuePair<TSort, T>[] array,    uint                                                    index)                                    => this      .CopyTo    (0u, array, index, array.Length > index ? (uint) array.Length - index : 0u);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                           CopyTo                                                                              (in System.Collections.Generic.KeyValuePair <TSort, T>[] array,    uint                                                    index)                                    => this      .CopyTo    (0u, array, index, array.Length > index ? (uint) array.Length - index : 0u);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                           CopyTo                                                                              (uint                                                    index,    T[]                                                     array, uint arrayIndex, uint length)      => this.items.CopyTo    (index, array, arrayIndex, length);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          void                                           CopyTo                                                                              (uint                                                    index,    in PatchOdyssey.Collections.RefKeyValuePair<TSort, T>[] array, uint arrayIndex, uint length)      { for (; 0u != length-- && index < this.Count; ++index) array[arrayIndex++] = new(in this.ranks[index], in this.items[index]); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                           CopyTo                                                                              (uint                                                    index,    in System.Collections.Generic.KeyValuePair <TSort, T>[] array, uint arrayIndex, uint length)      { for (; 0u != length-- && index < this.Count; ++index) array[arrayIndex++] = new(this.ranks[index],    this.items[index]); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                           EnsureCapacity                                                                      (uint                                                    capacity, bool                                                    precise = false)                          { this.items.EnsureCapacity(capacity, precise); this.ranks.EnsureCapacity(capacity, precise); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                           Equals                                                                              (in RefSortedCollection           <TSort, T>             collection)                                                                                                 => object.Equals(collection, this);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          bool                                           Exists                                                                              (PatchOdyssey.RefPredicate        <T>                    predicate)                                                                                                  =>     this.items.Exists       (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          bool                                           Exists                                                                              (PatchOdyssey.RefReadOnlyPredicate<T>                    predicate)                                                                                                  =>     this.items.Exists       (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                           Exists                                                                              (System.Predicate                 <T>                    predicate)                                                                                                  =>     this.items.Exists       (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                           ExistsRank                                                                          (PatchOdyssey.RefPredicate        <TSort>                predicate)                                                                                                  =>     this.ranks.Exists       (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          bool                                           ExistsRank                                                                          (PatchOdyssey.RefReadOnlyPredicate<TSort>                predicate)                                                                                                  =>     this.ranks.Exists       (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                           ExistsRank                                                                          (System.Predicate                 <TSort>                predicate)                                                                                                  =>     this.ranks.Exists       (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                           Fill                                                                                (in T                                                    element)                                                                                                    =>     this.items.Fill         (in element);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          ref T                                          Find                                                                                (PatchOdyssey.RefPredicate        <T>                    predicate)                                                                                                  => ref this.items.Find         (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          ref T                                          Find                                                                                (PatchOdyssey.RefReadOnlyPredicate<T>                    predicate)                                                                                                  => ref this.items.Find         (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          ref T                                          Find                                                                                (System.Predicate                 <T>                    predicate)                                                                                                  => ref this.items.Find         (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          PatchOdyssey.Collections.RefList<T>            FindAll                                                                             (PatchOdyssey.RefPredicate        <T>                    predicate)                                                                                                  =>     this.items.FindAll      (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          PatchOdyssey.Collections.RefList<T>            FindAll                                                                             (PatchOdyssey.RefReadOnlyPredicate<T>                    predicate)                                                                                                  =>     this.items.FindAll      (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          PatchOdyssey.Collections.RefList<T>            FindAll                                                                             (System.Predicate                 <T>                    predicate)                                                                                                  =>     this.items.FindAll      (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          int                                            FindIndex                                                                           (PatchOdyssey.RefPredicate        <T>                    predicate)                                                                                                  =>     this.items.FindIndex    (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          int                                            FindIndex                                                                           (PatchOdyssey.RefReadOnlyPredicate<T>                    predicate)                                                                                                  =>     this.items.FindIndex    (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            FindIndex                                                                           (System.Predicate                 <T>                    predicate)                                                                                                  =>     this.items.FindIndex    (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          int                                            FindIndex                                                                           (uint                                                    index, PatchOdyssey.RefPredicate        <T> predicate)                                                      =>     this.items.FindIndex    (index, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          int                                            FindIndex                                                                           (uint                                                    index, PatchOdyssey.RefReadOnlyPredicate<T> predicate)                                                      =>     this.items.FindIndex    (index, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            FindIndex                                                                           (uint                                                    index, System.Predicate                 <T> predicate)                                                      =>     this.items.FindIndex    (index, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          int                                            FindIndex                                                                           (uint                                                    index, uint                                 length, PatchOdyssey.RefPredicate        <T> predicate)         =>     this.items.FindIndex    (index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          int                                            FindIndex                                                                           (uint                                                    index, uint                                 length, PatchOdyssey.RefReadOnlyPredicate<T> predicate)         =>     this.items.FindIndex    (index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            FindIndex                                                                           (uint                                                    index, uint                                 length, System.Predicate                 <T> predicate)         =>     this.items.FindIndex    (index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          ref T                                          FindLast                                                                            (PatchOdyssey.RefPredicate        <T>                    predicate)                                                                                                  => ref this.items.FindLast     (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          ref T                                          FindLast                                                                            (PatchOdyssey.RefReadOnlyPredicate<T>                    predicate)                                                                                                  => ref this.items.FindLast     (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          ref T                                          FindLast                                                                            (System.Predicate                 <T>                    predicate)                                                                                                  => ref this.items.FindLast     (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          int                                            FindLastIndex                                                                       (PatchOdyssey.RefPredicate        <T>                    predicate)                                                                                                  =>     this.items.FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          int                                            FindLastIndex                                                                       (PatchOdyssey.RefReadOnlyPredicate<T>                    predicate)                                                                                                  =>     this.items.FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            FindLastIndex                                                                       (System.Predicate                 <T>                    predicate)                                                                                                  =>     this.items.FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          int                                            FindLastIndex                                                                       (uint                                                    index, PatchOdyssey.RefPredicate        <T> predicate)                                                      =>     this.items.FindLastIndex(index, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          int                                            FindLastIndex                                                                       (uint                                                    index, PatchOdyssey.RefReadOnlyPredicate<T> predicate)                                                      =>     this.items.FindLastIndex(index, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            FindLastIndex                                                                       (uint                                                    index, System.Predicate                 <T> predicate)                                                      =>     this.items.FindLastIndex(index, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            FindLastIndex                                                                       (uint                                                    index, uint                                 length, PatchOdyssey.RefPredicate        <T> predicate)         =>     this.items.FindLastIndex(index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          int                                            FindLastIndex                                                                       (uint                                                    index, uint                                 length, PatchOdyssey.RefReadOnlyPredicate<T> predicate)         =>     this.items.FindLastIndex(index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            FindLastIndex                                                                       (uint                                                    index, uint                                 length, System.Predicate                 <T> predicate)         =>     this.items.FindLastIndex(index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          ref TSort                                      FindRank                                                                            (PatchOdyssey.RefPredicate        <TSort>                predicate)                                                                                                  => ref this.ranks.Find         (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          ref TSort                                      FindRank                                                                            (PatchOdyssey.RefReadOnlyPredicate<TSort>                predicate)                                                                                                  => ref this.ranks.Find         (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          ref TSort                                      FindRank                                                                            (System.Predicate                 <TSort>                predicate)                                                                                                  => ref this.ranks.Find         (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          int                                            FindRankIndex                                                                       (PatchOdyssey.RefPredicate        <TSort>                predicate)                                                                                                  =>     this.ranks.FindIndex    (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          int                                            FindRankIndex                                                                       (PatchOdyssey.RefReadOnlyPredicate<TSort>                predicate)                                                                                                  =>     this.ranks.FindIndex    (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            FindRankIndex                                                                       (System.Predicate                 <TSort>                predicate)                                                                                                  =>     this.ranks.FindIndex    (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          int                                            FindRankIndex                                                                       (uint                                                    index, PatchOdyssey.RefPredicate        <TSort> predicate)                                                  =>     this.ranks.FindIndex    (index, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          int                                            FindRankIndex                                                                       (uint                                                    index, PatchOdyssey.RefReadOnlyPredicate<TSort> predicate)                                                  =>     this.ranks.FindIndex    (index, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            FindRankIndex                                                                       (uint                                                    index, System.Predicate                 <TSort> predicate)                                                  =>     this.ranks.FindIndex    (index, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          int                                            FindRankIndex                                                                       (uint                                                    index, uint                                     length, PatchOdyssey.RefPredicate        <TSort> predicate) =>     this.ranks.FindIndex    (index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          int                                            FindRankIndex                                                                       (uint                                                    index, uint                                     length, PatchOdyssey.RefReadOnlyPredicate<TSort> predicate) =>     this.ranks.FindIndex    (index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            FindRankIndex                                                                       (uint                                                    index, uint                                     length, System.Predicate                 <TSort> predicate) =>     this.ranks.FindIndex    (index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          ref TSort                                      FindRankLast                                                                        (PatchOdyssey.RefPredicate        <TSort>                predicate)                                                                                                  => ref this.ranks.FindLast     (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          ref TSort                                      FindRankLast                                                                        (PatchOdyssey.RefReadOnlyPredicate<TSort>                predicate)                                                                                                  => ref this.ranks.FindLast     (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          ref TSort                                      FindRankLast                                                                        (System.Predicate                 <TSort>                predicate)                                                                                                  => ref this.ranks.FindLast     (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          int                                            FindRankLastIndex                                                                   (PatchOdyssey.RefPredicate        <TSort>                predicate)                                                                                                  =>     this.ranks.FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          int                                            FindRankLastIndex                                                                   (PatchOdyssey.RefReadOnlyPredicate<TSort>                predicate)                                                                                                  =>     this.ranks.FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            FindRankLastIndex                                                                   (System.Predicate                 <TSort>                predicate)                                                                                                  =>     this.ranks.FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          int                                            FindRankLastIndex                                                                   (uint                                                    index, PatchOdyssey.RefPredicate        <TSort> predicate)                                                  =>     this.ranks.FindLastIndex(index, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          int                                            FindRankLastIndex                                                                   (uint                                                    index, PatchOdyssey.RefReadOnlyPredicate<TSort> predicate)                                                  =>     this.ranks.FindLastIndex(index, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            FindRankLastIndex                                                                   (uint                                                    index, System.Predicate                 <TSort> predicate)                                                  =>     this.ranks.FindLastIndex(index, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          int                                            FindRankLastIndex                                                                   (uint                                                    index, uint                                     length, PatchOdyssey.RefPredicate        <TSort> predicate) =>     this.ranks.FindLastIndex(index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          int                                            FindRankLastIndex                                                                   (uint                                                    index, uint                                     length, PatchOdyssey.RefReadOnlyPredicate<TSort> predicate) =>     this.ranks.FindLastIndex(index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            FindRankLastIndex                                                                   (uint                                                    index, uint                                     length, System.Predicate                 <TSort> predicate) =>     this.ranks.FindLastIndex(index, length, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          void                                           ForEach                                                                             (PatchOdyssey.RefAction        <T>                       action)                                                                                                     =>     this.items.ForEach      (action);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          void                                           ForEach                                                                             (PatchOdyssey.RefReadOnlyAction<T>                       action)                                                                                                     =>     this.items.ForEach      (action);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                           ForEach                                                                             (System.Action                 <T>                       action)                                                                                                     =>     this.items.ForEach      (action);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          PatchOdyssey.Collections.RefList<T>.Enumerator GetEnumerator                                                                       ()                                                                                                                                                                   =>     this.items.GetEnumerator();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          RefSortedCollection<TSort, T>                  GetRange                                                                            (uint                                                  index, uint length)                                                                                           { RefSortedCollection<TSort, T> sorted = new(this.Count, this.Comparer); sorted.items = this.items.GetRange(index, length); sorted.ranks = this.ranks.GetRange(index, length); return sorted; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          ref T                                          GetValue                                                                            (uint                                                  index)                                                                                                        => ref this.items[index];
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          ref T                                          GetValueByRank                                                                      (in TSort                                              rank)                                                                                                         { int index = this.ranks.LastIndexOf(in rank); return ref (index != -1 ? ref this.items[(uint) index] : ref this.items.Null); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            IndexOf                                                                             (in T                                                  element)                                                                                                      => this.items.IndexOf(in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            IndexOf                                                                             (in T                                                  element, uint index)                                                                                          => this.items.IndexOf(in element, index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            IndexOf                                                                             (in T                                                  element, uint index, uint length)                                                                             => this.items.IndexOf(in element, index, length);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            IndexOfRank                                                                         (in TSort                                              rank)                                                                                                         => this.ranks.IndexOf(in rank);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            IndexOfRank                                                                         (in TSort                                              rank, uint index)                                                                                             => this.ranks.IndexOf(in rank, index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            IndexOfRank                                                                         (in TSort                                              rank, uint index, uint length)                                                                                => this.ranks.IndexOf(in rank, index, length);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                           IsEmpty                                                                             ()                                                                                                                                                                   => 0u == this.Count;
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            LastIndexOf                                                                         (in T                                                  element)                                                                                                      => this.items.LastIndexOf(in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            LastIndexOf                                                                         (in T                                                  element, uint index)                                                                                          => this.items.LastIndexOf(in element, index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            LastIndexOf                                                                         (in T                                                  element, uint index, uint length)                                                                             => this.items.LastIndexOf(in element, index, length);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            LastIndexOfRank                                                                     (in TSort                                              rank)                                                                                                         => this.ranks.LastIndexOf(in rank);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            LastIndexOfRank                                                                     (in TSort                                              rank, uint index)                                                                                             => this.ranks.LastIndexOf(in rank, index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          int                                            LastIndexOfRank                                                                     (in TSort                                              rank, uint index, uint length)                                                                                => this.ranks.LastIndexOf(in rank, index, length);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                           Remove                                                                              (in T                                                  element)                                                                                                      { int index = this.items.IndexOf(in element); if (index != -1) { this.RemoveAt((uint) index); return true; } return false; }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          bool                                           Remove                                                                              (in PatchOdyssey.Collections.RefKeyValuePair<TSort, T> element)                                                                                                      { for (uint index = 0u; index != this.Count; ++index) if (0 == this.Comparer.Compare(in this.ranks[index], in element.Key) && PatchOdyssey.Collections.RefEqualityComparer<T>.Default.Equals(ref this.items[index], ref element.Value)) { this.RemoveAt(index); return true; } return false; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                           Remove                                                                              (in System.Collections.Generic.KeyValuePair <TSort, T> element)                                                                                                      => this.Remove(new PatchOdyssey.Collections.RefKeyValuePair<TSort, T>(element.Key, element.Value));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                           RemoveByRank                                                                        (in TSort                                              rank)                                                                                                         { int index = this.ranks.IndexOf(in rank); if (index != -1) { this.RemoveAt((uint) index); return true; } return false; }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          bool                                           RemoveByRank                                                                        (in PatchOdyssey.Collections.RefKeyValuePair<TSort, T> element)                                                                                                      => this.Remove(in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                           RemoveByRank                                                                        (in System.Collections.Generic.KeyValuePair <TSort, T> element)                                                                                                      => this.Remove(in element);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          uint                                           RemoveAll                                                                           (PatchOdyssey.RefPredicate                  <T>        predicate)                                                                                                    { uint count = 0u; foreach (ref          T element in this.items) { count += this.Remove(in element) ? 1u : 0u; } return count; }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          uint                                           RemoveAll                                                                           (PatchOdyssey.RefReadOnlyPredicate          <T>        predicate)                                                                                                    { uint count = 0u; foreach (ref readonly T element in this.items) { count += this.Remove(in element) ? 1u : 0u; } return count; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          uint                                           RemoveAll                                                                           (System.Predicate                           <T>        predicate)                                                                                                    { uint count = 0u; foreach (T              element in this.items) { count += this.Remove(in element) ? 1u : 0u; } return count; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                           RemoveAt                                                                            (uint                                                  index)                                                                                                        {  this.items.RemoveAt(index); this.ranks.RemoveAt(index); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                           SetValue                                                                            (uint                                                  index, in T element)                                                                                          => this.items[index] = element;
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                           SetValueByRank                                                                      (in TSort                                              rank,  in T element)                                                                                          { int index = this.ranks.LastIndexOf(in rank); if (index != -1) this.items[(uint) index] = element; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          RefSortedCollection<TSort, T>                  Slice                                                                               (uint                                                  index, uint length)                                                                                           { RefSortedCollection<TSort, T> sorted = new(this.Count, this.Comparer); sorted.items = this.items.Slice(index, length); sorted.ranks = this.ranks.Slice(index, length); return sorted; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          T[]                                            ToArray                                                                             ()                                                                                                                                                                   => this.items.ToArray   ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public override string?                                        ToString                                                                            ()                                                                                                                                                                   => this.items.ToString  ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                           TrimExcess                                                                          ()                                                                                                                                                                   {  this.items.TrimExcess();         this.ranks.TrimExcess(); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                           TrimExcess                                                                          (uint                                                  capacity)                                                                                                     {  this.items.TrimExcess(capacity); this.ranks.TrimExcess(capacity); }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          bool                                           TryAdd                                                                              (in PatchOdyssey.Collections.RefKeyValuePair<TSort, T> element)                                                                                                      => this.TryAdd(in element.Key, in element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                           TryAdd                                                                              (in System.Collections.Generic.KeyValuePair <TSort, T> element)                                                                                                      => this.TryAdd(element.Key,    element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                           TryAdd                                                                              (in TSort                                              rank, in T element)                                                                                           { if (!this.ranks.Contains(in rank)) { this.Add(in rank, in element); return true; } return false; }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          ref T                                          TryAppend                                                                           (in PatchOdyssey.Collections.RefKeyValuePair<TSort, T> element)                                                                                                      => ref this.TryAppend(in element.Key, in element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          ref T                                          TryAppend                                                                           (in System.Collections.Generic.KeyValuePair <TSort, T> element)                                                                                                      { (TSort rank, T element)[] pair = new[] {(element.Key, element.Value)}; return ref this.TryAppend(in Util.Reference<(TSort rank, T)>.Only(pair).rank, in Util.Reference<(TSort, T element)>.Only(pair).element); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          ref T                                          TryAppend                                                                           (in TSort                                              rank, in T element)                                                                                           { int index = this.ranks.LastIndexOf(in rank); if (index == -1) { this.ranks.Add(in rank); this.ranks.Sort(this.Comparer); index = this.ranks.LastIndexOf(in rank); this.items.Insert((uint) index, in element); } return ref this.items[(uint) index]; }
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public          bool                                           TrueForAll                                                                          (PatchOdyssey.RefPredicate        <T>                  predicate)                                                                                                    => this.items.TrueForAll(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          bool                                           TrueForAll                                                                          (PatchOdyssey.RefReadOnlyPredicate<T>                  predicate)                                                                                                    => this.items.TrueForAll(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                           TrueForAll                                                                          (System.Predicate                 <T>                  predicate)                                                                                                    => this.items.TrueForAll(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                           PatchOdyssey.Collections.IRefEquatable<RefSortedCollection<TSort, T>>.Equals        (ref RefSortedCollection          <TSort, T>           collection)                                                                                                   => this      .Equals    (in collection);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                           PatchOdyssey.Collections.IRefReadOnlyEquatable<RefSortedCollection<TSort, T>>.Equals(in  RefSortedCollection          <TSort, T>           collection)                                                                                                   => this      .Equals    (in collection);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                           System.Collections.Generic.ICollection<T>.Add                                       (T                                                     element)                                                                                                      => throw new System.NotSupportedException("Sorted collection requires a sorting rank for modification");
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                           System.Collections.Generic.ICollection<T>.Clear                                     ()                                                                                                                                                                   => this.Clear        ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                           System.Collections.Generic.ICollection<T>.Contains                                  (T            element)                                                                                                                                               => this.Contains     (element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                           System.Collections.Generic.ICollection<T>.CopyTo                                    (T[]          array, int index)                                                                                                                                      => this.CopyTo       (array, (uint) index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                           System.Collections.Generic.ICollection<T>.Remove                                    (T            element)                                                                                                                                               => this.Remove       (element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] System.Collections.Generic.IEnumerator<T>                      System.Collections.Generic.IEnumerable<T>.GetEnumerator                             ()                                                                                                                                                                   => this.GetEnumerator();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] System.Collections.IEnumerator                                 System.Collections.IEnumerable.GetEnumerator                                        ()                                                                                                                                                                   => this.GetEnumerator();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] int                                                            System.Collections.Generic.IList<T>.IndexOf                                         (T            element)                                                                                                                                               => this.IndexOf      (element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                           System.Collections.Generic.IList<T>.Insert                                          (int          index, T element)                                                                                                                                      => throw new System.NotSupportedException("Sorted collection requires a sorting rank for modification");
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                           System.Collections.Generic.IList<T>.RemoveAt                                        (int          index)                                                                                                                                                 => this.RemoveAt((uint) index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                           System.Collections.ICollection.CopyTo                                               (System.Array array, int index)                                                                                                                                      => throw new System.NotSupportedException("Sorted collection expected typed array for copy");
      [PatchMethod(AggressiveInlining), PatchResolution(0)] int                                                            System.Collections.IList.Add                                                        (object?      element)                                                                                                                                               => throw new System.NotSupportedException("Sorted collection requires a sorting rank for modification");
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                           System.Collections.IList.Clear                                                      ()                                                                                                                                                                   => this.Clear   ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                           System.Collections.IList.Contains                                                   (object?                              element)                                                                                                                       => this.Contains((T) element!);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] int                                                            System.Collections.IList.IndexOf                                                    (object?                              element)                                                                                                                       => this.IndexOf ((T) element!);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                           System.Collections.IList.Insert                                                     (int                                  index, object? element)                                                                                                        => throw new System.NotSupportedException("Sorted collection requires a sorting rank for modification");
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                           System.Collections.IList.Remove                                                     (object?                              element)                                                                                                                       => this    .Remove     ((T) element!);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                           System.Collections.IList.RemoveAt                                                   (int                                  index)                                                                                                                         => this    .RemoveAt   ((uint) index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] int                                                            System.Collections.IStructuralComparable.CompareTo                                  (object?                              value, System.Collections.IComparer         comparer)                                                                          => comparer.Compare    (this, value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                           System.Collections.IStructuralEquatable.Equals                                      (object?                              value, System.Collections.IEqualityComparer comparer)                                                                          => comparer.Equals     (this, value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] int                                                            System.Collections.IStructuralEquatable.GetHashCode                                 (System.Collections.IEqualityComparer comparer)                                                                                                                      => comparer.GetHashCode(this);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] object                                                         System.ICloneable.Clone                                                             ()                                                                                                                                                                   => this    .Clone      ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                           System.IEquatable<RefSortedCollection<TSort, T>>.Equals                             (RefSortedCollection<TSort, T> collection)                                                                                                                           => this    .Equals     (collection);

      /* … */
      public ref T                             this                                    [uint         index] => ref this.items[index];
      public     RefSortedCollection<TSort, T> this                                    [System.Range range] { get { (int index, int length) = range.GetOffsetAndLength((int) this.Count); return this.GetRange((uint) index, (uint) length); } }
      T                                        System.Collections.Generic.IList<T>.this[int          index] { get => this[(uint) index]; set => this[(uint) index] = value; }
      object?                                  System.Collections.IList.this           [int          index] { get => this[(uint) index]; set => this[(uint) index] = (T) value!; }
    }

    [System.Runtime.CompilerServices.CollectionBuilder(typeof(Sequence), nameof(Sequence.Create))]
    internal class Sequence : System.Collections.Generic.IEnumerable<int> /* ⟶ Based on collection expressions i.e. `[1, 2, …, 3]` */ {
      private readonly int[] values;

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)]
      public Sequence(System.ReadOnlySpan<int> values) {
        this.values = values.ToArray();
      }

      /* … */
      [PatchMethod(AggressiveInlining)] public static Sequence                                    Create                                      (System.ReadOnlySpan<int> values) => new(values);
      [PatchMethod(AggressiveInlining)] public        System.Collections.Generic.IEnumerator<int> GetEnumerator                               ()                                => ((System.Collections.Generic.IEnumerable<int>) this.values).GetEnumerator();
      [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator                            System.Collections.IEnumerable.GetEnumerator()                                => this.values.GetEnumerator();

      [PatchMethod(AggressiveInlining)] public static explicit operator int[](Sequence sequence) => sequence.values;
    }

    public class SharedList<T> : PatchOdyssey.Collections.IRefEquatable<SharedList<T>>, System.Collections.Generic.IList<T>, System.Collections.IList, System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable, System.ICloneable {
      protected internal static PatchOdyssey.Collections.RefList<T> List = new(); // ⟶ Singleton composition over inheritance
      public uint Capacity                                             { get => SharedList<T>.List.Capacity; set => SharedList<T>.List.Capacity = value; }
      public uint Count                                                => SharedList<T>.List.Count;
      int         System.Collections.Generic.ICollection<T>.Count      => ((int) SharedList<T>.List.Count);
      bool        System.Collections.Generic.ICollection<T>.IsReadOnly => false;
      int         System.Collections.ICollection.Count                 => ((int) SharedList<T>.List.Count);
      bool        System.Collections.ICollection.IsSynchronized        => false;
      object      System.Collections.ICollection.SyncRoot              => SharedList<T>.List;
      bool        System.Collections.IList.IsFixedSize                 => false;
      bool        System.Collections.IList.IsReadOnly                  => false;

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(uint                                      capacity = 0u)                                                                                       { SharedList<T>.List.EnsureCapacity(System.Math.Max(capacity, SharedList<T>.List.Capacity)); }
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(T[]                                       array)                   : this(array,                      (uint) array       .Length)              {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Array                              array)                   : this(array,                      (uint) array       .Length)              {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.ArraySegment<T>                    arraySegment)            : this(arraySegment,               (uint) arraySegment.Count)               {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.ArrayList              arrayList)               : this(arrayList,                  (uint) arrayList   .Count)               {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.Generic.HashSet    <T> hashset)                 : this(hashset,                    (uint) hashset     .Count)               {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.Generic.IEnumerable<T> enumerable)              : this(enumerable,                 (uint) Util.EnumerableCount(enumerable)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.Generic.LinkedList <T> list)                    : this(list,                       (uint) list     .Count)                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.Generic.List       <T> list)                    : this(list,                       (uint) list     .Count)                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.Generic.Queue      <T> queue)                   : this(queue,                      (uint) queue    .Count)                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.Generic.SortedSet  <T> sortedSet)               : this(sortedSet,                  (uint) sortedSet.Count)                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.Generic.Stack      <T> stack)                   : this(stack,                      (uint) stack    .Count)                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.IEnumerable            enumerable)              : this(enumerable,                 (uint) Util.EnumerableCount(enumerable)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.Queue                  queue)                   : this(queue,                      (uint) queue     .Count)                 {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.SortedList             sortedList)              : this(sortedList,                 (uint) sortedList.Count)                 {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.Stack                  stack)                   : this(stack,                      (uint) stack     .Count)                 {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Memory        <T>                  memory)                  : this(Util.Array<T>.From(memory), (uint) memory    .Length)                {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.ReadOnlyMemory<T>                  memory)                  : this(Util.Array<T>.From(memory), (uint) memory    .Length)                {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] private SharedList(System.Collections.IEnumerable            enumerable, uint length) : this(length)                                                              { if (enumerable is SharedList<T>) return; SharedList<T>.List.Clear(); foreach (object value in enumerable) SharedList<T>.List.Add((T) value); }
      [PatchConstructor, PatchMethod(AggressiveInlining)] private SharedList(System.Collections.Generic.IEnumerable<T> enumerable, uint length) : this(length)                                                              { if (enumerable is SharedList<T>) return; SharedList<T>.List.Clear(); SharedList<T>.List.AddRange(enumerable); }

      /* … */
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           Add                                                                 (in T                                      element)                                                                               =>     SharedList<T>.List.Add          (in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           AddRange                                                            (System.Collections.Generic.IEnumerable<T> enumerable)                                                                            =>     SharedList<T>.List.AddRange     (enumerable);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public ref T                                          Append                                                              (in T                                      element)                                                                               => ref SharedList<T>.List.Append       (in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public PatchOdyssey.Collections.RefReadOnlyList<T>    AsReadOnly                                                          ()                                                                                                                                =>     SharedList<T>.List.AsReadOnly   ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public int                                            BinarySearch                                                        (in T element)                                                                                                                    =>     SharedList<T>.List.BinarySearch (in element);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public int                                            BinarySearch                                                        (in T element,                         PatchOdyssey.Collections.IRefReadOnlyComparer<T>? comparer)                                =>     SharedList<T>.List.BinarySearch (in element, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public int                                            BinarySearch                                                        (in T element,                         System.Collections.Generic.IComparer         <T>? comparer)                                =>     SharedList<T>.List.BinarySearch (in element, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public int                                            BinarySearch                                                        (uint index, uint count, in T element, PatchOdyssey.Collections.IRefReadOnlyComparer<T>? comparer)                                =>     SharedList<T>.List.BinarySearch (index, count, in element, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public int                                            BinarySearch                                                        (uint index, uint count, in T element, System.Collections.Generic.IComparer         <T>? comparer)                                =>     SharedList<T>.List.BinarySearch (index, count, in element, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           Clear                                                               ()                                                                                                                                =>     SharedList<T>.List.Clear        ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public object                                         Clone                                                               ()                                                                                                                                =>     SharedList<T>.List.Clone        ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public bool                                           Contains                                                            (in T                                    element)                                                                                 =>     SharedList<T>.List.Contains     (in element);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public SharedList<U>                                  ConvertAll<U>                                                       (PatchOdyssey.RefConverter        <T, U> converter)                                                                               => new(SharedList<T>.List.ConvertAll<U>(converter));
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public SharedList<U>                                  ConvertAll<U>                                                       (PatchOdyssey.RefReadOnlyConverter<T, U> converter)                                                                               => new(SharedList<T>.List.ConvertAll<U>(converter));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public SharedList<U>                                  ConvertAll<U>                                                       (System.Converter                 <T, U> converter)                                                                               => new(SharedList<T>.List.ConvertAll<U>(converter));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           CopyTo                                                              (T[]                                     array)                                                                                   =>     SharedList<T>.List.CopyTo       (array);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           CopyTo                                                              (T[]                                     array, uint index)                                                                       =>     SharedList<T>.List.CopyTo       (array, index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           CopyTo                                                              (uint                                    index, T[]  array, uint arrayIndex, uint count)                                          =>     SharedList<T>.List.CopyTo       (index, array, arrayIndex, count);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public bool                                           Equals                                                              (in SharedList                    <T>    list)                                                                                    =>     true;
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public bool                                           Exists                                                              (PatchOdyssey.RefPredicate        <T>    predicate)                                                                               =>     SharedList<T>.List                                   .Exists       (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public bool                                           Exists                                                              (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                               =>     SharedList<T>.List                                   .Exists       (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public bool                                           Exists                                                              (System.Predicate                 <T>    predicate)                                                                               =>     SharedList<T>.List                                   .Exists       (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           Fill                                                                (in T                                    element)                                                                                 =>     SharedList<T>.List                                   .Fill         (in element);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public ref T                                          Find                                                                (PatchOdyssey.RefPredicate        <T>    predicate)                                                                               => ref SharedList<T>.List                                   .Find         (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public ref T                                          Find                                                                (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                               => ref SharedList<T>.List                                   .Find         (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public ref T                                          Find                                                                (System.Predicate                 <T>    predicate)                                                                               => ref SharedList<T>.List                                   .Find         (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public SharedList<T>                                  FindAll                                                             (PatchOdyssey.RefPredicate        <T>    predicate)                                                                               => new(SharedList<T>.List                                   .FindAll      (predicate));
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public SharedList<T>                                  FindAll                                                             (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                               => new(SharedList<T>.List                                   .FindAll      (predicate));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public SharedList<T>                                  FindAll                                                             (System.Predicate                 <T>    predicate)                                                                               => new(SharedList<T>.List                                   .FindAll      (predicate));
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public int                                            FindIndex                                                           (PatchOdyssey.RefPredicate        <T>    predicate)                                                                               =>     SharedList<T>.List                                   .FindIndex    (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public int                                            FindIndex                                                           (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                               =>     SharedList<T>.List                                   .FindIndex    (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public int                                            FindIndex                                                           (System.Predicate                 <T>    predicate)                                                                               =>     SharedList<T>.List                                   .FindIndex    (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public int                                            FindIndex                                                           (uint                                    index,             PatchOdyssey.RefPredicate        <T> predicate)                       =>     SharedList<T>.List                                   .FindIndex    (index,        predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public int                                            FindIndex                                                           (uint                                    index,             PatchOdyssey.RefReadOnlyPredicate<T> predicate)                       =>     SharedList<T>.List                                   .FindIndex    (index,        predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public int                                            FindIndex                                                           (uint                                    index,             System.Predicate                 <T> predicate)                       =>     SharedList<T>.List                                   .FindIndex    (index,        predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public int                                            FindIndex                                                           (uint                                    index, uint count, PatchOdyssey.RefPredicate        <T> predicate)                       =>     SharedList<T>.List                                   .FindIndex    (index, count, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public int                                            FindIndex                                                           (uint                                    index, uint count, PatchOdyssey.RefReadOnlyPredicate<T> predicate)                       =>     SharedList<T>.List                                   .FindIndex    (index, count, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public int                                            FindIndex                                                           (uint                                    index, uint count, System.Predicate                 <T> predicate)                       =>     SharedList<T>.List                                   .FindIndex    (index, count, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public ref T                                          FindLast                                                            (PatchOdyssey.RefPredicate        <T>    predicate)                                                                               => ref SharedList<T>.List                                   .FindLast     (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public ref T                                          FindLast                                                            (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                               => ref SharedList<T>.List                                   .FindLast     (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public ref T                                          FindLast                                                            (System.Predicate                 <T>    predicate)                                                                               => ref SharedList<T>.List                                   .FindLast     (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public int                                            FindLastIndex                                                       (PatchOdyssey.RefPredicate        <T>    predicate)                                                                               =>     SharedList<T>.List                                   .FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public int                                            FindLastIndex                                                       (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                               =>     SharedList<T>.List                                   .FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public int                                            FindLastIndex                                                       (System.Predicate                 <T>    predicate)                                                                               =>     SharedList<T>.List                                   .FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public int                                            FindLastIndex                                                       (uint                                    index,             PatchOdyssey.RefPredicate        <T> predicate)                       =>     SharedList<T>.List                                   .FindLastIndex(index,        predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public int                                            FindLastIndex                                                       (uint                                    index,             PatchOdyssey.RefReadOnlyPredicate<T> predicate)                       =>     SharedList<T>.List                                   .FindLastIndex(index,        predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public int                                            FindLastIndex                                                       (uint                                    index,             System.Predicate                 <T> predicate)                       =>     SharedList<T>.List                                   .FindLastIndex(index,        predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public int                                            FindLastIndex                                                       (uint                                    index, uint count, PatchOdyssey.RefPredicate        <T> predicate)                       =>     SharedList<T>.List                                   .FindLastIndex(index, count, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public int                                            FindLastIndex                                                       (uint                                    index, uint count, PatchOdyssey.RefReadOnlyPredicate<T> predicate)                       =>     SharedList<T>.List                                   .FindLastIndex(index, count, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public int                                            FindLastIndex                                                       (uint                                    index, uint count, System.Predicate                 <T> predicate)                       =>     SharedList<T>.List                                   .FindLastIndex(index, count, predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public void                                           ForEach                                                             (PatchOdyssey.RefAction        <T>       action)                                                                                  =>     SharedList<T>.List                                   .ForEach      (action);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public void                                           ForEach                                                             (PatchOdyssey.RefReadOnlyAction<T>       action)                                                                                  =>     SharedList<T>.List                                   .ForEach      (action);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           ForEach                                                             (System.Action                 <T>       action)                                                                                  =>     SharedList<T>.List                                   .ForEach      (action);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public PatchOdyssey.Collections.RefList<T>.Enumerator GetEnumerator                                                       ()                                                                                                                                =>     SharedList<T>.List                                   .GetEnumerator();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public SharedList<T>                                  GetRange                                                            (uint                                 index, uint count)                                                                          => new(SharedList<T>.List                                   .GetRange     (index, count));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public ref T                                          GetValue                                                            (uint                                 index)                                                                                      => ref SharedList<T>.List                                   [index];
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public int                                            IndexOf                                                             (in T                                 element)                                                                                    =>     SharedList<T>.List                                   .IndexOf      (in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public int                                            IndexOf                                                             (in T                                 element, uint                                      index)                                   =>     SharedList<T>.List                                   .IndexOf      (in element, index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public int                                            IndexOf                                                             (in T                                 element, uint                                      index, uint count)                       =>     SharedList<T>.List                                   .IndexOf      (in element, index, count);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           Insert                                                              (uint                                 index,   in T                                      element)                                 =>     SharedList<T>.List                                   .Insert       (index,   element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           InsertRange                                                         (uint                                 index,   System.Collections.Generic.IEnumerable<T> enumerable)                              =>     SharedList<T>.List                                   .InsertRange  (index,   enumerable);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public bool                                           IsEmpty                                                             ()                                                                                                                                =>     SharedList<T>.List                                   .IsEmpty      ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public int                                            LastIndexOf                                                         (in T                                 element)                                                                                    =>     SharedList<T>.List                                   .LastIndexOf  (in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public int                                            LastIndexOf                                                         (in T                                 element, uint index)                                                                        =>     SharedList<T>.List                                   .LastIndexOf  (in element, index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public int                                            LastIndexOf                                                         (in T                                 element, uint index, uint count)                                                            =>     SharedList<T>.List                                   .LastIndexOf  (in element, index, count);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public ref T                                          Prepend                                                             (in T                                 element)                                                                                    => ref SharedList<T>.List                                   .Prepend      (in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public bool                                           Remove                                                              (in T                                 element)                                                                                    =>     SharedList<T>.List                                   .Remove       (in element);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public uint                                           RemoveAll                                                           (PatchOdyssey.RefPredicate        <T> predicate)                                                                                  =>     SharedList<T>.List                                   .RemoveAll    (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public uint                                           RemoveAll                                                           (PatchOdyssey.RefReadOnlyPredicate<T> predicate)                                                                                  =>     SharedList<T>.List                                   .RemoveAll    (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public uint                                           RemoveAll                                                           (System.Predicate                 <T> predicate)                                                                                  =>     SharedList<T>.List                                   .RemoveAll    (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           RemoveAt                                                            (uint                                 index)                                                                                      =>     SharedList<T>.List                                   .RemoveAt     (index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           RemoveRange                                                         (uint                                 index, uint count)                                                                          =>     SharedList<T>.List                                   .RemoveRange  (index, count);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           Reverse                                                             ()                                                                                                                                =>     SharedList<T>.List                                   .Reverse      ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           Reverse                                                             (uint index, uint count)                                                                                                          =>     SharedList<T>.List                                   .Reverse      (index, count);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           SetValue                                                            (uint index, in T element)                                                                                                        =>     SharedList<T>.List                                   [index] = element;
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           Sort                                                                ()                                                                                                                                =>     SharedList<T>.List                                   .Sort         ();
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public void                                           Sort                                                                (PatchOdyssey.Collections.IRefComparer        <T>? comparer)                                                                      =>     SharedList<T>.List                                   .Sort         (comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public void                                           Sort                                                                (PatchOdyssey.Collections.IRefReadOnlyComparer<T>? comparer)                                                                      =>     SharedList<T>.List                                   .Sort         (comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public void                                           Sort                                                                (PatchOdyssey.RefComparison                   <T>  comparison)                                                                    =>     SharedList<T>.List                                   .Sort         (comparison);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public void                                           Sort                                                                (PatchOdyssey.RefReadOnlyComparison           <T>  comparison)                                                                    =>     SharedList<T>.List                                   .Sort         (comparison);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           Sort                                                                (System.Collections.Generic.IComparer         <T>? comparer)                                                                      =>     SharedList<T>.List                                   .Sort         (comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           Sort                                                                (System.Comparison                            <T>  comparison)                                                                    =>     SharedList<T>.List                                   .Sort         (comparison);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public void                                           Sort                                                                (uint                                              index, uint count, PatchOdyssey.Collections.IRefComparer        <T>? comparer) =>     SharedList<T>.List                                   .Sort         (index, count, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public void                                           Sort                                                                (uint                                              index, uint count, PatchOdyssey.Collections.IRefReadOnlyComparer<T>? comparer) =>     SharedList<T>.List                                   .Sort         (index, count, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           Sort                                                                (uint                                              index, uint count, System.Collections.Generic.IComparer         <T>? comparer) =>     SharedList<T>.List                                   .Sort         (index, count, comparer);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public T[]                                            ToArray                                                             ()                                                                                                                                =>     SharedList<T>.List                                   .ToArray      ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           TrimExcess                                                          ()                                                                                                                                =>     SharedList<T>.List                                   .TrimExcess   ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void                                           TrimExcess                                                          (uint capacity)                                                                                                                   =>     SharedList<T>.List                                   .TrimExcess   (capacity);
      [PatchMethod(AggressiveInlining), PatchResolution(2)] public bool                                           TrueForAll                                                          (PatchOdyssey.RefPredicate        <T> predicate)                                                                                  =>     SharedList<T>.List                                   .TrueForAll   (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public bool                                           TrueForAll                                                          (PatchOdyssey.RefReadOnlyPredicate<T> predicate)                                                                                  =>     SharedList<T>.List                                   .TrueForAll   (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public bool                                           TrueForAll                                                          (System.Predicate                 <T> predicate)                                                                                  =>     SharedList<T>.List                                   .TrueForAll   (predicate);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                  PatchOdyssey.Collections.IRefEquatable<SharedList<T>>.Equals        (ref SharedList                   <T> list)                                                                                       =>     this                                                 .Equals       (in list);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                  PatchOdyssey.Collections.IRefReadOnlyEquatable<SharedList<T>>.Equals(in  SharedList                   <T> list)                                                                                       =>     this                                                 .Equals       (in list);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                  System.Collections.Generic.ICollection<T>.Add                       (T                                    element)                                                                                    =>     this                                                 .Add          (in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                  System.Collections.Generic.ICollection<T>.Clear                     ()                                                                                                                                =>     this                                                 .Clear        ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                  System.Collections.Generic.ICollection<T>.Contains                  (T   element)                                                                                                                     =>     this                                                 .Contains     (in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                  System.Collections.Generic.ICollection<T>.CopyTo                    (T[] array, int index)                                                                                                            =>     this                                                 .CopyTo       (array, (uint) index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                  System.Collections.Generic.ICollection<T>.Remove                    (T   element)                                                                                                                     =>     this                                                 .Remove       (in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] System.Collections.Generic.IEnumerator<T>             System.Collections.Generic.IEnumerable<T>.GetEnumerator             ()                                                                                                                                =>     this                                                 .GetEnumerator();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] int                                                   System.Collections.Generic.IList<T>.IndexOf                         (T            element)                                                                                                            =>     this                                                 .IndexOf      (in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                  System.Collections.Generic.IList<T>.Insert                          (int          index, T element)                                                                                                   =>     this                                                 .Insert       ((uint) index, in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                  System.Collections.Generic.IList<T>.RemoveAt                        (int          index)                                                                                                              =>     this                                                 .RemoveAt     ((uint) index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                  System.Collections.ICollection.CopyTo                               (System.Array array, int index)                                                                                                   =>     ((System.Collections.ICollection) SharedList<T>.List).CopyTo       (array, index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] System.Collections.IEnumerator                        System.Collections.IEnumerable.GetEnumerator                        ()                                                                                                                                =>     this                                                 .GetEnumerator();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] int                                                   System.Collections.IList.Add                                        (object? element)                                                                                                                 {      this                                                 .Add          ((T) element!); return (int) this.Count; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                  System.Collections.IList.Clear                                      ()                                                                                                                                =>     this                                                 .Clear        ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                  System.Collections.IList.Contains                                   (object? element)                                                                                                                 =>     this                                                 .Contains     ((T) element!);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] int                                                   System.Collections.IList.IndexOf                                    (object? element)                                                                                                                 =>     this                                                 .IndexOf      ((T) element!);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                  System.Collections.IList.Insert                                     (int     index, object? element)                                                                                                  =>     this                                                 .Insert       ((uint) index, (T) element!);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                  System.Collections.IList.Remove                                     (object? element)                                                                                                                 =>     this                                                 .Remove       ((T) element!);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                  System.Collections.IList.RemoveAt                                   (int     index)                                                                                                                   =>     this                                                 .RemoveAt     ((uint) index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] int                                                   System.Collections.IStructuralComparable.CompareTo                  (object?                              value, System.Collections.IComparer         comparer)                                       =>     0;
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                  System.Collections.IStructuralEquatable.Equals                      (object?                              value, System.Collections.IEqualityComparer comparer)                                       =>     false;
      [PatchMethod(AggressiveInlining), PatchResolution(0)] int                                                   System.Collections.IStructuralEquatable.GetHashCode                 (System.Collections.IEqualityComparer comparer)                                                                                   =>     0;
      [PatchMethod(AggressiveInlining), PatchResolution(0)] object                                                System.ICloneable.Clone                                             ()                                                                                                                                =>     this.Clone ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                  System.IEquatable<SharedList<T>>.Equals                             (SharedList<T> list)                                                                                                              =>     this.Equals(list);

      public ref T          this                                    [uint         index] => ref SharedList<T>.List[index];
      public     RefList<T> this                                    [System.Range range] =>     SharedList<T>.List[range];
      T                     System.Collections.Generic.IList<T>.this[int          index] { get => this[(uint) index]; set => this[(uint) index] = value; }
      object?               System.Collections.IList.this           [int          index] { get => this[(uint) index]; set => this[(uint) index] = (T) value!; }
    }
      public class GameObjectSharedList<T> : PatchOdyssey.Collections.SharedList<T>, PatchOdyssey.Collections.IRefEquatable<GameObjectSharedList<T>>, System.Collections.Generic.IList<T>, System.Collections.IList, System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable, System.ICloneable where T : UnityEngine.Object /* ⟶ `UnityEngine.Component` or `UnityEngine.GameObject` */ {
        int    System.Collections.Generic.ICollection<T>.Count      => ((int) base.Count);
        bool   System.Collections.Generic.ICollection<T>.IsReadOnly => false;
        int    System.Collections.ICollection.Count                 => ((int) base.Count);
        bool   System.Collections.ICollection.IsSynchronized        => false;
        object System.Collections.ICollection.SyncRoot              => PatchOdyssey.Collections.SharedList<T>.List;
        bool   System.Collections.IList.IsFixedSize                 => false;
        bool   System.Collections.IList.IsReadOnly                  => false;

        /* … */
        [PatchConstructor, PatchMethod(AggressiveInlining)] internal GameObjectSharedList(uint                                   capacity = 0u) : base(capacity) {}
        [PatchConstructor, PatchMethod(AggressiveInlining)] internal GameObjectSharedList(GameObjectSharedList               <T> list)          : base(list)     {}
        [PatchConstructor, PatchMethod(AggressiveInlining)] private  GameObjectSharedList(PatchOdyssey.Collections.SharedList<T> list)          : base(list)     {}

        /* … */
        [PatchMethod(AggressiveInlining), PatchResolution(0)] internal new void           Add     (in T                                      element)    =>     base.Add     (element);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] internal new void           AddRange(System.Collections.Generic.IEnumerable<T> enumerable) =>     base.AddRange(enumerable);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] internal new ref readonly T Append  (in T                                      element)    => ref base.Append  (element);

        [PatchMethod(AggressiveInlining), PatchResolution(0)] public GameObjectSharedList<U> ByComponent<U>() where U : UnityEngine.Component => this.ByComponent(typeof(U)).ConvertAll(static element => (U) element);
        public GameObjectSharedList<UnityEngine.Component> ByComponent(System.Type type) {
          GameObjectSharedList<UnityEngine.Component> list = new(this.Count);

          // … ⟶ Avoid overriding underlying `GameObjectSharedList<UnityEngine.Component>.List` prematurely
          if (typeof(T) != typeof(UnityEngine.Component)) {
            list.Clear();

            foreach (T value in this)
            switch (value) {
              case UnityEngine.Component  component : list.Add(component .GetComponent(type)); break;
              case UnityEngine.GameObject gameObject: list.Add(gameObject.GetComponent(type)); break;
            }
          }

          return list;
        }

        [PatchMethod(AggressiveInlining), PatchResolution(0)]
        public GameObjectSharedList<T> ByName(string name) {
          this.RemoveAll(value => name != value.name);
          return new();
        }

        [PatchMethod(AggressiveInlining), PatchResolution(0)]
        public GameObjectSharedList<T> ByTag(string tag) {
          this.RemoveAll(value => value switch {
            UnityEngine.Component  component  => component .tag == tag,
            UnityEngine.GameObject gameObject => gameObject.tag == tag,
            _                                 => false
          });

          return new();
        }

        [PatchMethod(AggressiveInlining), PatchResolution(0)] internal           new void                    Clear                                                                         ()                                                                                                                                =>     base                                                 .Clear        ();
        [PatchMethod(AggressiveInlining), PatchResolution(2)] public             new GameObjectSharedList<U> ConvertAll<U>                                                                 (PatchOdyssey.RefConverter        <T, U> converter) where U : UnityEngine.Object /* ⟶ T */                                       => new(base                                                 .ConvertAll<U>(converter));
        [PatchMethod(AggressiveInlining), PatchResolution(1)] public             new GameObjectSharedList<U> ConvertAll<U>                                                                 (PatchOdyssey.RefReadOnlyConverter<T, U> converter) where U : UnityEngine.Object /* ⟶ T */                                       => new(base                                                 .ConvertAll<U>(converter));
        [PatchMethod(AggressiveInlining), PatchResolution(0)] public             new GameObjectSharedList<U> ConvertAll<U>                                                                 (System.Converter                 <T, U> converter) where U : UnityEngine.Object /* ⟶ T */                                       => new(base                                                 .ConvertAll<U>(converter));
        [PatchMethod(AggressiveInlining), PatchResolution(0)] public                 bool                    Equals                                                                        (in GameObjectSharedList          <T>    list)                                                                                    =>     base                                                 .Equals       ((PatchOdyssey.Collections.SharedList<T>) list);
        [PatchMethod(AggressiveInlining), PatchResolution(2)] internal           new GameObjectSharedList<T> FindAll                                                                       (PatchOdyssey.RefPredicate        <T>    predicate)                                                                               => new(base                                                 .FindAll      (predicate));
        [PatchMethod(AggressiveInlining), PatchResolution(1)] internal           new GameObjectSharedList<T> FindAll                                                                       (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                               => new(base                                                 .FindAll      (predicate));
        [PatchMethod(AggressiveInlining), PatchResolution(0)] internal           new GameObjectSharedList<T> FindAll                                                                       (System.Predicate                 <T>    predicate)                                                                               => new(base                                                 .FindAll      (predicate));
        [PatchMethod(AggressiveInlining), PatchResolution(0)] internal           new GameObjectSharedList<T> GetRange                                                                      (uint                                    index, uint                                      count)                                  => new(base                                                 .GetRange     (index, count));
        [PatchMethod(AggressiveInlining), PatchResolution(0)] internal           new void                    Insert                                                                        (uint                                    index, in T                                      element)                                =>     base                                                 .Insert       (index,   element);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] internal           new void                    InsertRange                                                                   (uint                                    index, System.Collections.Generic.IEnumerable<T> enumerable)                             =>     base                                                 .InsertRange  (index,   enumerable);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] internal           new ref readonly T          Prepend                                                                       (in T                                    element)                                                                                 => ref base                                                 .Prepend      (element);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] internal           new bool                    Remove                                                                        (in T                                    element)                                                                                 =>     base                                                 .Remove       (element);
        [PatchMethod(AggressiveInlining), PatchResolution(2)] internal           new uint                    RemoveAll                                                                     (PatchOdyssey.RefPredicate        <T>    predicate)                                                                               =>     base                                                 .RemoveAll    (predicate);
        [PatchMethod(AggressiveInlining), PatchResolution(1)] internal           new uint                    RemoveAll                                                                     (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                               =>     base                                                 .RemoveAll    (predicate);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] internal           new uint                    RemoveAll                                                                     (System.Predicate                 <T>    predicate)                                                                               =>     base                                                 .RemoveAll    (predicate);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] internal           new void                    RemoveAt                                                                      (uint                                    index)                                                                                   =>     base                                                 .RemoveAt     (index);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] internal           new void                    RemoveRange                                                                   (uint                                    index, uint count)                                                                       =>     base                                                 .RemoveRange  (index, count);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] internal           new void                    Reverse                                                                       ()                                                                                                                                =>     base                                                 .Reverse      ();
        [PatchMethod(AggressiveInlining), PatchResolution(0)] internal           new void                    Reverse                                                                       (uint index, uint count)                                                                                                          =>     base                                                 .Reverse      (index, count);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] internal           new void                    Sort                                                                          ()                                                                                                                                =>     base                                                 .Sort         ();
        [PatchMethod(AggressiveInlining), PatchResolution(2)] internal           new void                    Sort                                                                          (PatchOdyssey.Collections.IRefComparer        <T>? comparer)                                                                      =>     base                                                 .Sort         (comparer);
        [PatchMethod(AggressiveInlining), PatchResolution(1)] internal           new void                    Sort                                                                          (PatchOdyssey.Collections.IRefReadOnlyComparer<T>? comparer)                                                                      =>     base                                                 .Sort         (comparer);
        [PatchMethod(AggressiveInlining), PatchResolution(2)] internal           new void                    Sort                                                                          (PatchOdyssey.RefComparison                   <T>  comparison)                                                                    =>     base                                                 .Sort         (comparison);
        [PatchMethod(AggressiveInlining), PatchResolution(1)] internal           new void                    Sort                                                                          (PatchOdyssey.RefReadOnlyComparison           <T>  comparison)                                                                    =>     base                                                 .Sort         (comparison);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] internal           new void                    Sort                                                                          (System.Collections.Generic.IComparer         <T>? comparer)                                                                      =>     base                                                 .Sort         (comparer);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] internal           new void                    Sort                                                                          (System.Comparison                            <T>  comparison)                                                                    =>     base                                                 .Sort         (comparison);
        [PatchMethod(AggressiveInlining), PatchResolution(2)] internal           new void                    Sort                                                                          (uint                                              index, uint count, PatchOdyssey.Collections.IRefComparer        <T>? comparer) =>     base                                                 .Sort         (index, count, comparer);
        [PatchMethod(AggressiveInlining), PatchResolution(1)] internal           new void                    Sort                                                                          (uint                                              index, uint count, PatchOdyssey.Collections.IRefReadOnlyComparer<T>? comparer) =>     base                                                 .Sort         (index, count, comparer);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] internal           new void                    Sort                                                                          (uint                                              index, uint count, System.Collections.Generic.IComparer         <T>? comparer) =>     base                                                 .Sort         (index, count, comparer);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] protected internal new void                    TrimExcess                                                                    ()                                                                                                                                =>     base                                                 .TrimExcess   ();
        [PatchMethod(AggressiveInlining), PatchResolution(0)] protected internal new void                    TrimExcess                                                                    (uint                        capacity)                                                                                            =>     base                                                 .TrimExcess   (capacity);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                           PatchOdyssey.Collections.IRefEquatable<GameObjectSharedList<T>>.Equals        (ref GameObjectSharedList<T> list)                                                                                                =>     this                                                 .Equals       (in list);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                           PatchOdyssey.Collections.IRefReadOnlyEquatable<GameObjectSharedList<T>>.Equals(in  GameObjectSharedList<T> list)                                                                                                =>     this                                                 .Equals       (in list);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                           System.Collections.Generic.ICollection<T>.Add                                 (T                           element)                                                                                             =>     base                                                 .Add          (in element);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                           System.Collections.Generic.ICollection<T>.Clear                               ()                                                                                                                                =>     base                                                 .Clear        ();
        [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                           System.Collections.Generic.ICollection<T>.Contains                            (T   element)                                                                                                                     =>     base                                                 .Contains     (in element);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                           System.Collections.Generic.ICollection<T>.CopyTo                              (T[] array, int index)                                                                                                            =>     base                                                 .CopyTo       (array, (uint) index);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                           System.Collections.Generic.ICollection<T>.Remove                              (T   element)                                                                                                                     =>     base                                                 .Remove       (in element);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] System.Collections.Generic.IEnumerator<T>      System.Collections.Generic.IEnumerable<T>.GetEnumerator                       ()                                                                                                                                =>     base                                                 .GetEnumerator();
        [PatchMethod(AggressiveInlining), PatchResolution(0)] int                                            System.Collections.Generic.IList<T>.IndexOf                                   (T            element)                                                                                                            =>     base                                                 .IndexOf      (in element);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                           System.Collections.Generic.IList<T>.Insert                                    (int          index, T element)                                                                                                   =>     base                                                 .Insert       ((uint) index, in element);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                           System.Collections.Generic.IList<T>.RemoveAt                                  (int          index)                                                                                                              =>     base                                                 .RemoveAt     ((uint) index);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                           System.Collections.ICollection.CopyTo                                         (System.Array array, int index)                                                                                                   =>     ((System.Collections.ICollection) SharedList<T>.List).CopyTo       (array, index);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] System.Collections.IEnumerator                 System.Collections.IEnumerable.GetEnumerator                                  ()                                                                                                                                =>     base                                                 .GetEnumerator();
        [PatchMethod(AggressiveInlining), PatchResolution(0)] int                                            System.Collections.IList.Add                                                  (object? element)                                                                                                                 {      base                                                 .Add          ((T) element!); return (int) base.Count; }
        [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                           System.Collections.IList.Clear                                                ()                                                                                                                                =>     base                                                 .Clear        ();
        [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                           System.Collections.IList.Contains                                             (object? element)                                                                                                                 =>     base                                                 .Contains     ((T) element!);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] int                                            System.Collections.IList.IndexOf                                              (object? element)                                                                                                                 =>     base                                                 .IndexOf      ((T) element!);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                           System.Collections.IList.Insert                                               (int     index, object? element)                                                                                                  =>     base                                                 .Insert       ((uint) index, (T) element!);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                           System.Collections.IList.Remove                                               (object? element)                                                                                                                 =>     base                                                 .Remove       ((T) element!);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                           System.Collections.IList.RemoveAt                                             (int     index)                                                                                                                   =>     base                                                 .RemoveAt     ((uint) index);
        [PatchMethod(AggressiveInlining), PatchResolution(0)] int                                            System.Collections.IStructuralComparable.CompareTo                            (object?                              value, System.Collections.IComparer         comparer)                                       =>     0;
        [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                           System.Collections.IStructuralEquatable.Equals                                (object?                              value, System.Collections.IEqualityComparer comparer)                                       =>     false;
        [PatchMethod(AggressiveInlining), PatchResolution(0)] int                                            System.Collections.IStructuralEquatable.GetHashCode                           (System.Collections.IEqualityComparer comparer)                                                                                   =>     0;
        [PatchMethod(AggressiveInlining), PatchResolution(0)] object                                         System.ICloneable.Clone                                                       ()                                                                                                                                =>     this.Clone ();
        [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                           System.IEquatable<GameObjectSharedList<T>>.Equals                             (GameObjectSharedList<T> list)                                                                                                    =>     this.Equals(list);
      }

    internal /* readonly */ struct WaitInfo : PatchOdyssey.Collections.IRefEquatable<WaitInfo> /* ⟶ Considered `UnityEngine.MonoBehaviour::Invoke[Repeating](nameof(𝑓) or ((System.Delegate) 𝑓).Method.Name, delay[, interval])` */ {
      private sealed class Waiter : UnityEngine.MonoBehaviour {}

      /* … */
      internal readonly       UnityEngine.Coroutine?                                               coroutine = null;
      internal /* readonly */ PatchOdyssey.Collections.EventHandler<PatchOdyssey.Events.WaitEvent> events    = new(); // ⟶ Information actually stored within its `::WaitEvent`s

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)] public   WaitInfo()                                {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] internal WaitInfo(UnityEngine.Coroutine coroutine) => this.coroutine = coroutine;

      /* … */
      [PatchMethod(AggressiveInlining)] public bool Equals                                                         (in  WaitInfo wait) => wait.events == this.events;
      [PatchMethod(AggressiveInlining)] bool        PatchOdyssey.Collections.IRefEquatable<WaitInfo>.Equals        (ref WaitInfo wait) => this.Equals(in wait);
      [PatchMethod(AggressiveInlining)] bool        PatchOdyssey.Collections.IRefReadOnlyEquatable<WaitInfo>.Equals(in  WaitInfo wait) => this.Equals(in wait);
      [PatchMethod(AggressiveInlining)] bool        System.IEquatable<WaitInfo>.Equals                             (WaitInfo     wait) => this.Equals(wait);
    }

    #if UNITY_EDITOR
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.ReadOnlyInInspectorAttribute))]
      public class ReadOnlyInInspectorDrawer : UnityEditor.PropertyDrawer {
        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) => UnityEditor.EditorGUI.GetPropertyHeight(property, label, true);

        public override void  OnGUI(UnityEngine.Rect position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) {
          // bool isJobReadOnly = property.serializedObject.targetObject.GetType().GetField(property.name)?.IsDefined(typeof(Unity.Collections.ReadOnlyAttribute), true) ?? false;
          // …
          #if false
            UnityEngine.GUI.enabled = false;
            UnityEditor.EditorGUI.PropertyField(position, property, label, true);
            UnityEngine.GUI.enabled = true;
          #else
            using (new UnityEditor.EditorGUI.DisabledScope(true))
            UnityEditor.EditorGUI.PropertyField(position, property, label, true);
          #endif
        }
      }

      public class RefDictionaryDrawer<TKey, TValue> : UnityEditor.PropertyDrawer {
        private                 bool foldout    = false;
        private static readonly bool IsDrawable = RefDictionaryDrawer<TKey, TValue>.DelegateGUIField<TKey>() is not null && RefDictionaryDrawer<TKey, TValue>.DelegateGUIField<TValue>() is not null;

        /* … */
        public static PatchOdyssey.DictionaryGUIField? DelegateGUIField<T>() {
          if (typeof(T) == typeof(bool))                              return [PatchMethod(AggressiveInlining)] static (in UnityEngine.Rect position, object value) => UnityEditor.EditorGUI.Toggle         (position,                              (System.Boolean)             value)                  as object;
          if (typeof(T) == typeof(double))                            return [PatchMethod(AggressiveInlining)] static (in UnityEngine.Rect position, object value) => UnityEditor.EditorGUI.DoubleField    (position,                              (System.Double)              value)                  as object;
          if (typeof(T) == typeof(float))                             return [PatchMethod(AggressiveInlining)] static (in UnityEngine.Rect position, object value) => UnityEditor.EditorGUI.FloatField     (position,                              (System.Single)              value)                  as object;
          if (typeof(T) == typeof(int))                               return [PatchMethod(AggressiveInlining)] static (in UnityEngine.Rect position, object value) => UnityEditor.EditorGUI.IntField       (position,                              (System.Int32)               value)                  as object;
          if (typeof(T) == typeof(long))                              return [PatchMethod(AggressiveInlining)] static (in UnityEngine.Rect position, object value) => UnityEditor.EditorGUI.LongField      (position,                              (System.Int64)               value)                  as object;
          if (typeof(T) == typeof(string))                            return [PatchMethod(AggressiveInlining)] static (in UnityEngine.Rect position, object value) => UnityEditor.EditorGUI.TextField      (position,                              (System.String)              value)                  as object;
          if (typeof(T) == typeof(UnityEngine.AnimationCurve))        return [PatchMethod(AggressiveInlining)] static (in UnityEngine.Rect position, object value) => UnityEditor.EditorGUI.CurveField     (position,                              (UnityEngine.AnimationCurve) value)                  as object;
          if (typeof(T) == typeof(UnityEngine.Bounds))                return [PatchMethod(AggressiveInlining)] static (in UnityEngine.Rect position, object value) => UnityEditor.EditorGUI.BoundsField    (position,                              (UnityEngine.Bounds)         value)                  as object;
          if (typeof(T) == typeof(UnityEngine.BoundsInt))             return [PatchMethod(AggressiveInlining)] static (in UnityEngine.Rect position, object value) => UnityEditor.EditorGUI.BoundsIntField (position,                              (UnityEngine.BoundsInt)      value)                  as object;
          if (typeof(T) == typeof(UnityEngine.Color))                 return [PatchMethod(AggressiveInlining)] static (in UnityEngine.Rect position, object value) => UnityEditor.EditorGUI.ColorField     (position,                              (UnityEngine.Color)          value)                  as object;
          if (typeof(T) == typeof(UnityEngine.Gradient))              return [PatchMethod(AggressiveInlining)] static (in UnityEngine.Rect position, object value) => UnityEditor.EditorGUI.GradientField  (position,                              (UnityEngine.Gradient)       value)                  as object;
          if (typeof(T) == typeof(UnityEngine.Rect))                  return [PatchMethod(AggressiveInlining)] static (in UnityEngine.Rect position, object value) => UnityEditor.EditorGUI.RectField      (position,                              (UnityEngine.Rect)           value)                  as object;
          if (typeof(T) == typeof(UnityEngine.RectInt))               return [PatchMethod(AggressiveInlining)] static (in UnityEngine.Rect position, object value) => UnityEditor.EditorGUI.RectIntField   (position,                              (UnityEngine.RectInt)        value)                  as object;
          if (typeof(T) == typeof(UnityEngine.Vector2))               return [PatchMethod(AggressiveInlining)] static (in UnityEngine.Rect position, object value) => UnityEditor.EditorGUI.Vector2Field   (position, UnityEngine.GUIContent.none, (UnityEngine.Vector2)        value)                  as object;
          if (typeof(T) == typeof(UnityEngine.Vector2Int))            return [PatchMethod(AggressiveInlining)] static (in UnityEngine.Rect position, object value) => UnityEditor.EditorGUI.Vector2IntField(position, UnityEngine.GUIContent.none, (UnityEngine.Vector2Int)     value)                  as object;
          if (typeof(T) == typeof(UnityEngine.Vector3))               return [PatchMethod(AggressiveInlining)] static (in UnityEngine.Rect position, object value) => UnityEditor.EditorGUI.Vector3Field   (position, UnityEngine.GUIContent.none, (UnityEngine.Vector3)        value)                  as object;
          if (typeof(T) == typeof(UnityEngine.Vector3Int))            return [PatchMethod(AggressiveInlining)] static (in UnityEngine.Rect position, object value) => UnityEditor.EditorGUI.Vector3IntField(position, UnityEngine.GUIContent.none, (UnityEngine.Vector3Int)     value)                  as object;
          if (typeof(T) == typeof(UnityEngine.Vector4))               return [PatchMethod(AggressiveInlining)] static (in UnityEngine.Rect position, object value) => UnityEditor.EditorGUI.Vector4Field   (position, UnityEngine.GUIContent.none, (UnityEngine.Vector4)        value)                  as object;
          if (typeof(T).IsEnum)                                       return [PatchMethod(AggressiveInlining)] static (in UnityEngine.Rect position, object value) => UnityEditor.EditorGUI.EnumPopup      (position,                              (System.Enum)                value)                  as object;
          if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T))) return [PatchMethod(AggressiveInlining)] static (in UnityEngine.Rect position, object value) => UnityEditor.EditorGUI.ObjectField    (position,                              (UnityEngine.Object)         value, typeof(T), true) as object;

          return null;
        }

        [PatchMethod(AggressiveInlining)]
        private System.Collections.Generic.IDictionary<TKey, TValue> Ensure(UnityEditor.SerializedProperty property) {
          System.Collections.Generic.IDictionary<TKey, TValue> dictionary = this.fieldInfo.GetValue(property.serializedObject.targetObject) as System.Collections.Generic.IDictionary<TKey, TValue> ?? new PatchOdyssey.Collections.RefDictionary<TKey, TValue>();

          this.fieldInfo.SetValue(property.serializedObject.targetObject, dictionary);
          return dictionary;
        }

        [PatchMethod(AggressiveInlining)]
        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) => base.GetPropertyHeight(property, label) * (RefDictionaryDrawer<TKey, TValue>.IsDrawable && (this.foldout || UnityEditor.EditorPrefs.GetBool(label.text)) ? System.Math.Max(this.Ensure(property).Count, 1) + 1 : 1);

        [PatchMethod(AggressiveInlining)]
        public override void OnGUI(UnityEngine.Rect position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) {
          System.Collections.Generic.IDictionary<TKey, TValue>                     dictionary = this.Ensure(property);
          float                                                                    size       = position.height = base.GetPropertyHeight(property, label);
          (UnityEngine.Rect add, UnityEngine.Rect clear, UnityEngine.Rect foldout) positions  = (
            add    : new(position.x + (position.width - Util.PercOf(size, 200.0f)), position.y, 0.0f           + size, position.height),
            clear  : new(position.x + (position.width - Util.PercOf(size, 100.0f)), position.y, 0.0f           + size, position.height),
            foldout: new(position.x,                                                position.y, position.width - size, position.height)
          );

          // …
          if ((RefDictionaryDrawer<TKey, TValue>.IsDrawable)) {
            base.OnGUI(position, property, label);
            return;
          }

          if (!dictionary.IsReadOnly) {
            if (UnityEngine.GUI.Button(positions.add, new UnityEngine.GUIContent("+", "Add field"), UnityEditor.EditorStyles.miniButton))
            dictionary.TryAdd(typeof(TKey) != typeof(string) ? System.Activator.CreateInstance<TKey>() : (TKey) (string.Empty as object), Traits.IsValueType<TValue>() ? System.Activator.CreateInstance<TValue>() : (TValue) (null as object)!);

            if (UnityEngine.GUI.Button(positions.clear, new UnityEngine.GUIContent("×", "Clear dictionary"), UnityEditor.EditorStyles.miniButtonRight))
            dictionary.Clear();
          }

          UnityEditor.EditorGUI.BeginChangeCheck();
            this.foldout = UnityEditor.EditorPrefs.GetBool(label.text);
            this.foldout = UnityEditor.EditorGUI.  Foldout(positions.foldout, this.foldout, label, true);
          if (UnityEditor.EditorGUI.EndChangeCheck()) UnityEditor.EditorPrefs.SetBool(label.text, this.foldout);

          if (this.foldout) {
            if (dictionary.IsEmpty()) {
              UnityEngine.GUI.Label(new(position.x, position.y + position.height, position.width, position.height), "Dictionary is empty");
              return;
            }

            foreach (System.Collections.Generic.KeyValuePair<TKey, TValue> element in dictionary)
            if (element.Key is not null) {
              (TKey key, TValue value)                                                            = (element.Key, element.Value);
              (UnityEngine.Rect key, UnityEngine.Rect value, UnityEngine.Rect clear) subpositions = (
                key  : new(position.x                                              + (dictionary.IsReadOnly ? size : 0.0f), position.y += position.height, Util.PercOf(position.width - size, 40.0f), position.height),
                value: new(position.x + Util.PercOf(position.width - size,  40.0f) + (dictionary.IsReadOnly ? size : 0.0f), position.y,                    Util.PercOf(position.width - size, 60.0f), position.height),
                clear: new(position.x + Util.PercOf(position.width - size, 100.0f),                                         position.y,                    size,                                      position.height)
              );

              // …
              UnityEditor.EditorGUI.BeginChangeCheck();
                if (dictionary.IsReadOnly) UnityEngine.GUI.Label(subpositions.key, key.ToString());
                else key = (TKey) RefDictionaryDrawer<TKey, TValue>.DelegateGUIField<TKey>()!(subpositions.key, key);
              if (UnityEditor.EditorGUI.EndChangeCheck()) {
                dictionary.Remove(element.Key);
                dictionary.Add   (key, value);

                break;
              }

              UnityEditor.EditorGUI.BeginChangeCheck();
                value = (TValue) RefDictionaryDrawer<TKey, TValue>.DelegateGUIField<TValue>()!(subpositions.value, value!);
              if (UnityEditor.EditorGUI.EndChangeCheck()) {
                dictionary[key] = value;
                break;
              }

              if (!dictionary.IsReadOnly)
              if (UnityEngine.GUI.Button(subpositions.clear, new UnityEngine.GUIContent("×", "Clear item"), UnityEditor.EditorStyles.miniButtonRight)) {
                dictionary.Remove(key);
                break;
              }
            }
          }
        }
      }
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.AnimationCurveDictionary))]         public class AnimationCurveDictionaryDrawer         : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.AnimationCurve> {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.AnimationCurveReadOnlyDictionary))] public class AnimationCurveReadOnlyDictionaryDrawer : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.AnimationCurve> {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.BooleanDictionary))]                public class BooleanDictionaryDrawer                : PatchOdyssey.Collections.RefDictionaryDrawer<string, System     .Boolean>        {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.BooleanReadOnlyDictionary))]        public class BooleanReadOnlyDictionaryDrawer        : PatchOdyssey.Collections.RefDictionaryDrawer<string, System     .Boolean>        {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.BoundsDictionary))]                 public class BoundsDictionaryDrawer                 : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.Bounds>         {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.BoundsReadOnlyDictionary))]         public class BoundsReadOnlyDictionaryDrawer         : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.Bounds>         {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.BoundsIntDictionary))]              public class BoundsIntDictionaryDrawer              : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.BoundsInt>      {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.BoundsIntReadOnlyDictionary))]      public class BoundsIntReadOnlyDictionaryDrawer      : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.BoundsInt>      {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.ColorDictionary))]                  public class ColorDictionaryDrawer                  : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.Color>          {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.ColorReadOnlyDictionary))]          public class ColorReadOnlyDictionaryDrawer          : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.Color>          {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.DoubleDictionary))]                 public class DoubleDictionaryDrawer                 : PatchOdyssey.Collections.RefDictionaryDrawer<string, System     .Double>         {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.DoubleReadOnlyDictionary))]         public class DoubleReadOnlyDictionaryDrawer         : PatchOdyssey.Collections.RefDictionaryDrawer<string, System     .Double>         {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.FloatDictionary))]                  public class FloatDictionaryDrawer                  : PatchOdyssey.Collections.RefDictionaryDrawer<string, System     .Single>         {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.FloatReadOnlyDictionary))]          public class FloatReadOnlyDictionaryDrawer          : PatchOdyssey.Collections.RefDictionaryDrawer<string, System     .Single>         {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.GameObjectDictionary))]             public class GameObjectDictionaryDrawer             : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.GameObject>     {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.GameObjectReadOnlyDictionary))]     public class GameObjectReadOnlyDictionaryDrawer     : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.GameObject>     {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.GradientDictionary))]               public class GradientDictionaryDrawer               : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.Gradient>       {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.GradientReadOnlyDictionary))]       public class GradientReadOnlyDictionaryDrawer       : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.Gradient>       {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.IntDictionary))]                    public class IntDictionaryDrawer                    : PatchOdyssey.Collections.RefDictionaryDrawer<string, System     .Int32>          {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.IntReadOnlyDictionary))]            public class IntReadOnlyDictionaryDrawer            : PatchOdyssey.Collections.RefDictionaryDrawer<string, System     .Int32>          {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.LongDictionary))]                   public class LongDictionaryDrawer                   : PatchOdyssey.Collections.RefDictionaryDrawer<string, System     .Int64>          {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.LongReadOnlyDictionary))]           public class LongReadOnlyDictionaryDrawer           : PatchOdyssey.Collections.RefDictionaryDrawer<string, System     .Int64>          {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.RectDictionary))]                   public class RectDictionaryDrawer                   : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.Rect>           {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.RectReadOnlyDictionary))]           public class RectReadOnlyDictionaryDrawer           : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.Rect>           {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.RectIntDictionary))]                public class RectIntDictionaryDrawer                : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.RectInt>        {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.RectIntReadOnlyDictionary))]        public class RectIntReadOnlyDictionaryDrawer        : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.RectInt>        {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.StringDictionary))]                 public class StringDictionaryDrawer                 : PatchOdyssey.Collections.RefDictionaryDrawer<string, System     .String>         {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.StringReadOnlyDictionary))]         public class StringReadOnlyDictionaryDrawer         : PatchOdyssey.Collections.RefDictionaryDrawer<string, System     .String>         {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.UIntDictionary))]                   public class UIntDictionaryDrawer                   : PatchOdyssey.Collections.RefDictionaryDrawer<string, System     .UInt32>         {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.UIntReadOnlyDictionary))]           public class UIntReadOnlyDictionaryDrawer           : PatchOdyssey.Collections.RefDictionaryDrawer<string, System     .UInt32>         {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.ULongDictionary))]                  public class ULongDictionaryDrawer                  : PatchOdyssey.Collections.RefDictionaryDrawer<string, System     .UInt64>         {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.ULongReadOnlyDictionary))]          public class ULongReadOnlyDictionaryDrawer          : PatchOdyssey.Collections.RefDictionaryDrawer<string, System     .UInt64>         {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector2Dictionary))]                public class Vector2DictionaryDrawer                : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.Vector2>        {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector2ReadOnlyDictionary))]        public class Vector2ReadOnlyDictionaryDrawer        : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.Vector2>        {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector2IntDictionary))]             public class Vector2IntDictionaryDrawer             : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.Vector2Int>     {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector2IntReadOnlyDictionary))]     public class Vector2IntReadOnlyDictionaryDrawer     : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.Vector2Int>     {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector3Dictionary))]                public class Vector3DictionaryDrawer                : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.Vector3>        {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector3ReadOnlyDictionary))]        public class Vector3ReadOnlyDictionaryDrawer        : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.Vector3>        {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector3IntDictionary))]             public class Vector3IntDictionaryDrawer             : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.Vector3Int>     {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector3IntReadOnlyDictionary))]     public class Vector3IntReadOnlyDictionaryDrawer     : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.Vector3Int>     {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector4Dictionary))]                public class Vector4DictionaryDrawer                : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.Vector4>        {}
        [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector4ReadOnlyDictionary))]        public class Vector4ReadOnlyDictionaryDrawer        : PatchOdyssey.Collections.RefDictionaryDrawer<string, UnityEngine.Vector4>        {}

      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.ReadWriteInInspectorAttribute))]
      public class ReadWriteInInspectorDrawer : UnityEditor.PropertyDrawer {
        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, UnityEngine.GUIContent         label)                                  => UnityEditor.EditorGUI.GetPropertyHeight(property, label, true);
        public override void  OnGUI            (UnityEngine.Rect               position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) => UnityEditor.EditorGUI.PropertyField    (position, property, label, true);
      }
    #endif
  }

  public interface Events /* ⟶ Based on `System.EventArgs`; See `https://web.archive.org/web/20130916192216/https://developer.mozilla.org/en-US/docs/Web/API/Event` for naming scheme used (or `https://web.archive.org/web/20250117180031/https://learn.microsoft.com/en-us/dotnet/api/system.threading.manualresetevent?view=net-9.0` for an example) */ {
    public struct LoadEvent : PatchOdyssey.Events {
      public static readonly LoadEvent Default = new() {data = (0u, 1u, false, 0.0, new(string.Empty, System.UriKind.Relative), null), callback = [PatchMethod(AggressiveInlining)] static (object? _, in LoadEvent _) => {}};

      public PatchOdyssey.Handler<LoadEvent>                                                                       callback { get; internal set; } =  LoadEvent.Default.callback;
      public (uint attempts, uint attemptsAllowed, bool cached, double duration, System.Uri path, object? payload) data                            =  LoadEvent.Default.data;
      public uint                                                                                                  attempts                        => this.data.attempts;        // ⟶ Track persistent `LoadUri*(…)` `retries`
      public uint                                                                                                  attemptsAllowed                 => this.data.attemptsAllowed; //    ^^
      public bool                                                                                                  cached                          => this.data.cached;
      public double                                                                                                duration                        => this.data.duration; // ⟶ Time since load till payload
      public double                                                                                                epoch    { get; internal set; } =  UnityEngine.Time.realtimeSinceStartupAsDouble;
      public object?                                                                                               metadata { get;          set; } =  null;
      public System.Uri                                                                                            path                            => this.data.path;
      public object?                                                                                               payload                         => this.data.payload;
      System.Delegate                                                                                              PatchOdyssey.Events.callback { get => this.callback; set => this.callback = (PatchOdyssey.Handler<LoadEvent>) value; }
      object?                                                                                                      PatchOdyssey.Events.data     { get => this.data;     set => this.data     = ((uint, uint, bool, double, System.Uri, object?)) value!; }
      double                                                                                                       PatchOdyssey.Events.epoch    { get => this.epoch;    set => this.epoch    = value; }
      object?                                                                                                      PatchOdyssey.Events.metadata { get => this.metadata; set => this.metadata = value; }

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)]
      public LoadEvent() {}
    }

    public struct WaitEvent : PatchOdyssey.Events {
      public static readonly WaitEvent Default = new() {data = (0.0, 0.0, false), callback = [PatchMethod(AggressiveInlining)] static (object? _, in WaitEvent _) => {}};

      public PatchOdyssey.Handler<WaitEvent>                  callback { get; internal set; } =  WaitEvent.Default.callback;
      public (double delay, double timestamp, bool repeating) data                            =  WaitEvent.Default.data;
      public double                                           delay                           => this.data.delay; // ⟶ Specified delay
      public double                                           epoch    { get; internal set; } =  UnityEngine.Time.realtimeSinceStartupAsDouble;
      public object?                                          metadata { get;          set; } =  null;
      public bool                                             repeating                       => this.data.repeating;
      public double                                           timestamp                       => this.data.timestamp; // ⟶ Next available timestamp to signal a `Wait` event (which could be in the past chronologically)
      System.Delegate                                         PatchOdyssey.Events.callback { get => this.callback; set => this.callback = (PatchOdyssey.Handler<WaitEvent>) value; }
      object?                                                 PatchOdyssey.Events.data     { get => this.data;     set => this.data     = ((double, double, bool)) value!; }
      double                                                  PatchOdyssey.Events.epoch    { get => this.epoch;    set => this.epoch    = value; }
      object?                                                 PatchOdyssey.Events.metadata { get => this.metadata; set => this.metadata = value; }

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)]
      public WaitEvent() {}
    }

    /* … */
    public System.Delegate callback { get; internal set; }
    public object?         data     { get; internal set; }
    public double          epoch    { get; internal set; }
    public object?         metadata { get;          set; }
  }

  /* … */
  public delegate void           ArrayCopier <T>                    (T[]                        sourceArray, uint    sourceIndex, T[] destinationArray, uint destinationIndex, uint count); // ⟶ See `𝑓 Util.Array    <T>.Copy   (…)`
  public delegate ref T          ArrayIndexer<T>                    (T[]                        array,       uint    index);                                                                // ⟶ See `𝑓 Util.Reference<T>.ArrayAt(…)`
  public delegate object         DictionaryGUIField                 (in UnityEngine.Rect        position,    object  value);                                                                // ⟶ Determines how `RefDictionary<…>` (and `RefReadOnlyDictionary<…>`) elements are drawn by the Inspector
  public delegate void           Handler<T>                         (object?                    target,      in T    data) where T : PatchOdyssey.Events;                                   // ⟶ Handles completed `Load`, `Wait`, … operations i.e. `System.EventHandler`
  public delegate object?        Interpolator                       (double                     progress,    object? a, object? b);                                                         // ⟶ Interpolates `::begin` and `::end` properties in `Animation.UIKeyframe["…"]`
  public delegate T              Interpolator                 <T>   (double                     progress,    in T    a, in T    b);                                                         //    ^^
  public delegate ref readonly T ReadOnlySpanIndexer          <T>   (in  System.ReadOnlySpan<T> span, int index);                                                                           // ⟶ See `𝑓 Util.Reference<T>.ReadOnlySpanAt(…)`
  public delegate void           RefAction                    <T>   (ref T                      value);                                                                                     // ⟶ Based on `System.Action<T>`
  public delegate int            RefComparison                <T>   (ref T                      a, ref T b);                                                                                // ⟶ Based on `System.Comparison<T>`; See `𝑓 RefComparer<T>.CompareValue(…)`
  public delegate U              RefConverter                 <T, U>(ref T                      value);                                                                                     // ⟶ Based on `System.Converter<T, U>`
  public delegate bool           RefEqualityComparison        <T>   (ref T                      a, ref T b);                                                                                // ⟶ See `𝑓 RefEqualityComparer<T>.EqualsValue(…)`
  public delegate int            RefHasher                    <T>   (ref T                      value);                                                                                     //
  public delegate bool           RefPredicate                 <T>   (ref T                      value);                                                                                     // ⟶ Based on `System.Predicate<T>`
  public delegate void           RefReadOnlyAction            <T>   (in  T                      value);                                                                                     // ⟶ Based on `System.Action<T>`
  public delegate int            RefReadOnlyComparison        <T>   (in  T                      a, in  T b);                                                                                // ⟶ Based on `System.Comparison<T>`; See `𝑓 RefReadOnlyComparer<T>.CompareValue(…)`
  public delegate U              RefReadOnlyConverter         <T, U>(in  T                      value);                                                                                     // ⟶ Based on `System.Converter<T, U>`
  public delegate bool           RefReadOnlyEqualityComparison<T>   (in  T                      a, in T b);                                                                                 // ⟶ See `𝑓 RefReadOnlyEqualityComparer<T>.EqualsValue(…)`
  public delegate int            RefReadOnlyHasher            <T>   (in  T                      value);                                                                                     //
  public delegate bool           RefReadOnlyPredicate         <T>   (in  T                      value);                                                                                     // ⟶ Based on `System.Predicate<T>`
  public delegate ref T          SpanIndexer                  <T>   (in  System.Span<T>         span, int index);                                                                           // ⟶ See `𝑓 Util.Reference<T>.SpanAt(…)`
  public delegate double         Tweener                            (double                     time);                                                                                      // ⟶ Adjusts interpolation be-tween `Interpolator(…)`’s `progress` from `a` to `b`

  [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false, Inherited = false)] // ⟶ Display property in Unity Inspector as “read-only”
  public sealed class ReadOnlyInInspectorAttribute : UnityEngine.PropertyAttribute /* ⟶ `System.Attribute`, `Unity.Collections.ReadOnlyAttribute` */ {}

  [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false, Inherited = false)] // ⟶ Display property in Unity Inspector as “read-write”
  public sealed class ReadWriteInInspectorAttribute : UnityEngine.PropertyAttribute /* ⟶ `System.Attribute` */ {}

  public readonly ref struct Void { /* ⟶ “error CS0590: UsEr-DeFiNeD oPeRaToRs CaNnOt ReTuRn VoId” */ }
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
  public static class Extensions /* ⟶ Method extensions e.g. `System.Array.Add(this …)` */ {
    [PatchMethod(AggressiveInlining)] public static void Add              (this System.Collections.Queue                             queue,      object?                                               element) => queue     .Enqueue(element);                    // ⟶ Intended for initializer lists only i.e. `new() {…}`
    [PatchMethod(AggressiveInlining)] public static void Add<T>           (this System.Collections.Queue                             queue,      T                                                     element) => queue     .Enqueue(element);                    //    ^^
    [PatchMethod(AggressiveInlining)] public static void Add<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary, System.Collections.Generic.KeyValuePair<TKey, TValue> element) => dictionary.Add    (element.Key, element.Value); //
    [PatchMethod(AggressiveInlining)] public static void Add<T>           (this System.Collections.Generic.Queue      <T>            queue,      T                                                     element) => queue     .Enqueue(element);                    // ⟶ Intended for initializer lists only i.e. `new() {…}`
    [PatchMethod(AggressiveInlining)] public static void Add<T>           (this System.Collections.Generic.Stack      <T>            stack,      T                                                     element) => stack     .Push   (element);                    //    ^^
    [PatchMethod(AggressiveInlining)] public static void Add              (this System.Collections.Stack                             stack,      object?                                               element) => stack     .Push   (element);                    //
    [PatchMethod(AggressiveInlining)] public static void Add<T>           (this System.Collections.Stack                             stack,      T                                                     element) => stack     .Push   (element);                    //

    [PatchMethod(AggressiveInlining)] public static void AddRange<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>> enumerable) { foreach (System.Collections.Generic.KeyValuePair<TKey, TValue> element in dictionary) dictionary.Add(element.Key, element.Value); }
    [PatchMethod(AggressiveInlining)] public static void AddRange<T>           (this System.Collections.Generic.IList      <T>            list,       System.Collections.Generic.IEnumerable<T>                                                     enumerable) { foreach (T                                                     value   in enumerable) list      .Add(value); }

    [PatchMethod(AggressiveInlining)] public static ref readonly object? Append   (this System.Collections.ArrayList             arrayList, in object? value) { arrayList.Add    (value); return ref value; }
    [PatchMethod(AggressiveInlining)] public static ref readonly T       Append<T>(this System.Collections.ArrayList             arrayList, in T       value) { arrayList.Add    (value); return ref value; }
    [PatchMethod(AggressiveInlining)] public static ref readonly T       Append<T>(this System.Collections.Generic.LinkedList<T> list,      in T       value) { list     .AddLast(value); return ref value; }
    [PatchMethod(AggressiveInlining)] public static ref readonly T       Append<T>(this System.Collections.Generic.List      <T> list,      in T       value) { list     .Add    (value); return ref value; }

    [PatchMethod(AggressiveInlining)] public static T[]                                                                       AsCopy<T>           (this    T[]                                                                       array)                            => (T[])          array.Clone();
    [PatchMethod(AggressiveInlining)] public static System.Array                                                              AsCopy              (this    System.Array                                                              array)                            => (System.Array) array.Clone();
    [PatchMethod(AggressiveInlining)] public static System.ArraySegment<T>                                                    AsCopy<T>           (in this System.ArraySegment<T>                                                    arraySegment)                     => new(Util.Array<T>.From(arraySegment));
    [PatchMethod(AggressiveInlining)] public static System.Collections.ArrayList                                              AsCopy              (this    System.Collections.ArrayList                                              arrayList)                        => (System.Collections.ArrayList) arrayList.Clone();
    [PatchMethod(AggressiveInlining)] public static System.Collections.BitArray                                               AsCopy              (this    System.Collections.BitArray                                               bits)                             => (System.Collections.BitArray)  bits     .Clone();
    [PatchMethod(AggressiveInlining)] public static System.Collections.Queue                                                  AsCopy              (this    System.Collections.Queue                                                  queue)                            => (System.Collections.Queue)     queue    .Clone();
    [PatchMethod(AggressiveInlining)] public static System.Collections.Generic.Dictionary      <TKey, TValue>                 AsCopy<TKey, TValue>(this    System.Collections.Generic.Dictionary      <TKey, TValue>                 dictionary)                       => new(dictionary, dictionary.Comparer);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Generic.HashSet         <T>                            AsCopy<T>           (this    System.Collections.Generic.HashSet         <T>                            hashset)                          => new(hashset, hashset.Comparer);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Generic.LinkedList      <T>                            AsCopy<T>           (this    System.Collections.Generic.LinkedList      <T>                            list)                             => new(list);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Generic.List            <T>                            AsCopy<T>           (this    System.Collections.Generic.List            <T>                            list)                             => new(list);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Generic.Queue           <T>                            AsCopy<T>           (this    System.Collections.Generic.Queue           <T>                            queue)                            => new(queue);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Generic.SortedDictionary<TKey, TValue>                 AsCopy<TKey, TValue>(this    System.Collections.Generic.SortedDictionary<TKey, TValue>                 sortedDictionary)                 => new(sortedDictionary, sortedDictionary.Comparer);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Generic.SortedList      <TKey, TValue>                 AsCopy<TKey, TValue>(this    System.Collections.Generic.SortedList      <TKey, TValue>                 sortedList)                       => new(sortedList,       sortedList      .Comparer);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Generic.SortedSet       <T>                            AsCopy<T>           (this    System.Collections.Generic.SortedSet       <T>                            sortedSet)                        => new(sortedSet,        sortedSet       .Comparer);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Generic.Stack           <T>                            AsCopy<T>           (this    System.Collections.Generic.Stack           <T>                            stack)                            => new(stack);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Hashtable                                              AsCopy              (this    System.Collections.Hashtable                                              hashtable)                        => (System.Collections.Hashtable) hashtable.Clone(); // ⟶ `new(hashtable, hashtable.EqualityComparer)`
    [PatchMethod(AggressiveInlining)] public static System.Collections.ObjectModel.ObservableCollection        <T>            AsCopy<T>           (this    System.Collections.ObjectModel.ObservableCollection        <T>            collection)                       => new(collection);
    [PatchMethod(AggressiveInlining)] public static System.Collections.ObjectModel.ReadOnlyCollection          <T>            AsCopy<T>           (this    System.Collections.ObjectModel.ReadOnlyCollection          <T>            collection)                       => new(new System.Collections.Generic.List                    <T>           (collection));
    [PatchMethod(AggressiveInlining)] public static System.Collections.ObjectModel.ReadOnlyDictionary          <TKey, TValue> AsCopy<TKey, TValue>(this    System.Collections.ObjectModel.ReadOnlyDictionary          <TKey, TValue> dictionary)                       => new(new System.Collections.Generic.Dictionary              <TKey, TValue>(dictionary));
    [PatchMethod(AggressiveInlining)] public static System.Collections.ObjectModel.ReadOnlyObservableCollection<T>            AsCopy<T>           (this    System.Collections.ObjectModel.ReadOnlyObservableCollection<T>            collection)                       => new(new System.Collections.ObjectModel.ObservableCollection<T>           (collection));
    [PatchMethod(AggressiveInlining)] public static System.Collections.SortedList                                             AsCopy              (this    System.Collections.SortedList                                             sortedList)                       => (System.Collections.SortedList) sortedList.Clone();
    [PatchMethod(AggressiveInlining)] public static System.Collections.Specialized.HybridDictionary                           AsCopy              (this    System.Collections.Specialized.HybridDictionary                           dictionary)                       => dictionary.AsCopy(false);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Specialized.HybridDictionary                           AsCopy              (this    System.Collections.Specialized.HybridDictionary                           dictionary, bool caseInsensitive) { System.Collections.Specialized.HybridDictionary copy = new(dictionary.Count, caseInsensitive); foreach (System.Collections.DictionaryEntry element in dictionary) { copy.Add(element.Key, element.Value); } return copy; }
    [PatchMethod(AggressiveInlining)] public static System.Collections.Specialized.ListDictionary                             AsCopy              (this    System.Collections.Specialized.ListDictionary                             dictionary)                       { System.Collections.Specialized.ListDictionary   copy = new();                                  foreach (System.Collections.DictionaryEntry element in dictionary) { copy.Add(element.Key, element.Value); } return copy; }
    [PatchMethod(AggressiveInlining)] public static System.Collections.Specialized.NameValueCollection                        AsCopy              (this    System.Collections.Specialized.NameValueCollection                        collection)                       => new(collection);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Specialized.OrderedDictionary                          AsCopy              (this    System.Collections.Specialized.OrderedDictionary                          dictionary)                       { System.Collections.Specialized.OrderedDictionary copy = new(dictionary.Count); foreach (System.Collections.DictionaryEntry element in dictionary) { copy.Add(element.Key, element.Value); }    return copy; }
    [PatchMethod(AggressiveInlining)] public static System.Collections.Specialized.StringCollection                           AsCopy              (this    System.Collections.Specialized.StringCollection                           collection)                       { System.Collections.Specialized.StringCollection  copy = new(); string[] subcopy = new string[collection.Count]; collection.CopyTo(subcopy, 0); copy.AddRange(subcopy);                         return copy; }
    [PatchMethod(AggressiveInlining)] public static System.Collections.Specialized.StringDictionary                           AsCopy              (this    System.Collections.Specialized.StringDictionary                           dictionary)                       { System.Collections.Specialized.StringDictionary  copy = new(); foreach (System.Collections.DictionaryEntry element in dictionary) { copy.Add((string) element.Key, element.Value as string); } return copy; }
    [PatchMethod(AggressiveInlining)] public static System.Collections.Stack                                                  AsCopy              (this    System.Collections.Stack                                                  stack)                            => new(stack);
    [PatchMethod(AggressiveInlining)] public static System.IO.MemoryStream                                                    AsCopy              (this    System.IO.MemoryStream                                                    stream)                           { try { return new(stream.GetBuffer(), 0, (int) stream.Length, stream.CanWrite, true); } catch (System.UnauthorizedAccessException) {} return new(Util.Array.From(stream), 0, (int) stream.Length, stream.CanWrite, false); }
    [PatchMethod(AggressiveInlining)] public static System.Memory        <T>                                                  AsCopy<T>           (in this System.Memory        <T>                                                  memory)                           => memory.Slice(0);
    [PatchMethod(AggressiveInlining)] public static System.ReadOnlyMemory<T>                                                  AsCopy<T>           (in this System.ReadOnlyMemory<T>                                                  memory)                           => memory.Slice(0);
    [PatchMethod(AggressiveInlining)] public static System.Span          <T>                                                  AsCopy<T>           (in this System.Span          <T>                                                  span)                             => span  .Slice(0);
    [PatchMethod(AggressiveInlining)] public static System.ReadOnlySpan  <T>                                                  AsCopy<T>           (in this System.ReadOnlySpan  <T>                                                  span)                             => span  .Slice(0);
    #if NET9_0 || NET9_0_OR_GREATER
      [PatchMethod(AggressiveInlining)]
      public static System.Collections.ObjectModel.ReadOnlyCollection<T> AsCopy<T>(this System.Collections.ObjectModel.ReadOnlySet<T> set) => new(new System.Collections.Generic.HashSet<T>(set));
    #endif

    [PatchMethod(AggressiveInlining)] public static System.Collections.ObjectModel.ReadOnlyCollection<T>            AsReadOnly<T>           (this T[]                                                  array)      => System.Array.AsReadOnly<T>(array);
    [PatchMethod(AggressiveInlining)] public static System.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this System.Collections.Generic.Dictionary <TKey, TValue> dictionary) => new System.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue>(dictionary);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Generic.IReadOnlyDictionary   <TKey, TValue> AsReadOnly<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary) { (dictionary as System.Collections.Generic.Dictionary<TKey, TValue>)?.TrimExcess(); return dictionary is PatchOdyssey.Collections.RefDictionary<TKey, TValue> subdictionary ? subdictionary.AsReadOnly() : new System.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue>(dictionary); } // ⟶ `System.Collections.Generic.CollectionExtensions.AsReadOnly<TKey, TValue>(dictionary);`
    [PatchMethod(AggressiveInlining)] public static System.Collections.Generic.IReadOnlyList         <T>            AsReadOnly<T>           (this System.Collections.Generic.IList      <T>            list)       { (list       as System.Collections.Generic.List      <T>)           ?.TrimExcess(); return list       is PatchOdyssey.Collections.RefList      <T>            sublist       ? sublist      .AsReadOnly() : new System.Collections.ObjectModel.ReadOnlyCollection<T>           (list); }       // ⟶ `System.Collections.Generic.CollectionExtensions.AsReadOnly<T>           (list);`
    [PatchMethod(AggressiveInlining)] public static System.Collections.ObjectModel.ReadOnlyCollection<T>            AsReadOnly<T>           (this System.Collections.Generic.List       <T>            list)       => new System.Collections.ObjectModel.ReadOnlyCollection<T>(list);

    [PatchMethod(AggressiveInlining)] public static int BinarySearch   (this System.Array array,                        object? element)                                                    => System.Array.BinarySearch(array, element);
    [PatchMethod(AggressiveInlining)] public static int BinarySearch   (this System.Array array,                        object? element, System.Collections.IComparer? comparer)            => System.Array.BinarySearch(array, element, comparer);
    [PatchMethod(AggressiveInlining)] public static int BinarySearch   (this System.Array array, int index, int length, object? element)                                                    => System.Array.BinarySearch(array, index, length, element);
    [PatchMethod(AggressiveInlining)] public static int BinarySearch   (this System.Array array, int index, int length, object? element, System.Collections.IComparer? comparer)            => System.Array.BinarySearch(array, index, length, element, comparer);
    [PatchMethod(AggressiveInlining)] public static int BinarySearch<T>(this T[]          array,                        in T    element)                                                    => System.Array.BinarySearch(array, element);
    [PatchMethod(AggressiveInlining)] public static int BinarySearch<T>(this T[]          array,                        in T    element, System.Collections.Generic.IComparer<T>? comparer) => System.Array.BinarySearch(array, element, comparer);
    [PatchMethod(AggressiveInlining)] public static int BinarySearch<T>(this T[]          array, int index, int length, in T    element)                                                    => System.Array.BinarySearch(array, index, length, element);
    [PatchMethod(AggressiveInlining)] public static int BinarySearch<T>(this T[]          array, int index, int length, in T    element, System.Collections.Generic.IComparer<T>? comparer) => System.Array.BinarySearch(array, index, length, element, comparer);

    [PatchMethod(AggressiveInlining)] public static void Clear              (this System.Array                                         array)                        => array.Clear(0, array.Length);
    [PatchMethod(AggressiveInlining)] public static void Clear              (this System.Array                                         array, int index, int length) { System.Array.Clear(array, index, length); }
    [PatchMethod(AggressiveInlining)] public static void Clear<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary)                   { foreach (TKey key in dictionary.Keys) dictionary.Remove(key); } // ⟶ `::Capacity` remains unchanged

    [PatchMethod(AggressiveInlining)] public static bool Contains(this System.Array array, object value) => array.Contains(value, null as System.Collections.IEqualityComparer);
    [PatchMethod(AggressiveInlining)]
    public static bool Contains(this System.Array array, object value, System.Collections.IEqualityComparer? comparer) {
      for (System.Collections.IEnumerator enumerator = array.GetEnumerator(); enumerator.MoveNext(); ) {
        if (comparer?.Equals(enumerator.Current, value) ?? System.Object.ReferenceEquals(enumerator.Current, value))
        return true;
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
    public static bool ContainsValue<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary, in TValue value) {
      foreach (TValue dictionaryValue in dictionary.Values) {
        if ((value as System.IEquatable<TValue>)?.Equals(dictionaryValue) ?? System.Object.ReferenceEquals(value, dictionaryValue))
        return true;
      }

      return false;
    }

    [PatchMethod(AggressiveInlining)]
    public static U[] ConvertAll<T, U>(this T[] array, System.Converter<T, U> converter) => System.Array.ConvertAll<T, U>(array, converter);

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

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectEnumerator EnumerateChildren   (this UnityEngine.GameObject gameObject) => new(GameObjectEnumerator.Kind.Children,    gameObject.transform);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectEnumerator EnumerateDescendants(this UnityEngine.GameObject gameObject) => new(GameObjectEnumerator.Kind.Descendants, gameObject.transform);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectEnumerator EnumerateHierarchy  (this UnityEngine.GameObject gameObject) => new(GameObjectEnumerator.Kind.Hierarchy,   gameObject.transform);

    [PatchMethod(AggressiveInlining)] public static T                     EnsureComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => (T) gameObject.EnsureComponent(typeof(T));
    [PatchMethod(AggressiveInlining)] public static UnityEngine.Component EnsureComponent   (this UnityEngine.GameObject gameObject, System.Type type)               { UnityEngine.Component? component = gameObject.GetComponent(type); return null != component ? component : gameObject.AddComponent(type); }

    [PatchMethod(AggressiveInlining)]
    public static bool Exists<T>(this T[] array, System.Predicate<T> predicate) => System.Array.Exists<T>(array, predicate);

    [PatchMethod(AggressiveInlining)]
    public static void Fill<T>(this T[] array, in T element) {
      #if NET5_0 || NET5_0_OR_GREATER || NETCOREAPP2_1 || NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_0 || NETSTANDARD2_0_OR_GREATER
        new System.Span<T>(array).Fill(element);
        return;
      #endif

      #pragma warning disable CS8500
        if (!array.IsEmpty()) unsafe {
          Util.Reference<T>.First(array) = element;

          for (uint filled = 1u; array.Length > filled; filled <<= 1)
          Util.Array<T>.Copy(array, 0, array, filled, System.Math.Min(filled, (uint) array.Length - filled));
        }
      #pragma warning restore CS8500
    }

    [PatchMethod(AggressiveInlining)]
    public static void Fill<T>(this System.Collections.Generic.IList<T> list, in T element) {
      for (int index = list.Count; 0 != index--; )
      list[index] = element;
    }

    [PatchMethod(AggressiveInlining)] public static T?  Find   <T>(this T[] array, System.Predicate<T> predicate) => System.Array.Find   <T>(array, predicate);
    [PatchMethod(AggressiveInlining)] public static T[] FindAll<T>(this T[] array, System.Predicate<T> predicate) => System.Array.FindAll<T>(array, predicate);

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
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                     FindChildrenByComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject          .FindChildrenByComponent(typeof(T)).ConvertAll([PatchMethod(AggressiveInlining)] static (child) => (T) child);
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
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                     FindDescendantsByComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject          .FindDescendantsByComponent(typeof(T)).ConvertAll([PatchMethod(AggressiveInlining)] static (descendant) => (T) descendant);
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
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                     FindHierarchyByComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject          .FindHierarchyByComponent(typeof(T)).ConvertAll([PatchMethod(AggressiveInlining)] static (_) => (T) _);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component> FindHierarchyByComponent   (this UnityEngine.Component  component,  System.Type type)               => component.gameObject.FindHierarchyByComponent(type);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component> FindHierarchyByComponent   (this UnityEngine.GameObject gameObject, System.Type type)               { GameObjectSharedList<UnityEngine.Component> hierarchy = new((uint) gameObject.transform.childCount); hierarchy.Clear(); foreach (UnityEngine.GameObject child in gameObject.EnumerateHierarchy()) { UnityEngine.Component? component = child.GetComponent(type); if (component is not null) hierarchy.Add(component); } return hierarchy; }

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindHierarchyByName(this UnityEngine.Component  component,  string name) => component.gameObject.FindHierarchyByName(name);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindHierarchyByName(this UnityEngine.GameObject gameObject, string name) { PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> hierarchy = new((uint) gameObject.transform.childCount); hierarchy.Clear(); foreach (UnityEngine.GameObject child in gameObject.EnumerateHierarchy()) { if (child.name == name) hierarchy.Add(child); } return hierarchy; }

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindHierarchyByTag(this UnityEngine.Component  component,  string tag) => component.gameObject.FindHierarchyByTag(tag);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindHierarchyByTag(this UnityEngine.GameObject gameObject, string tag) { PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> hierarchy = new((uint) gameObject.transform.childCount); hierarchy.Clear(); foreach (UnityEngine.GameObject child in gameObject.EnumerateHierarchy()) { if (child.tag == tag) hierarchy.Add(child); } return hierarchy; }

    [PatchMethod(AggressiveInlining)] public static int  FindIndex    <T>(this T[] array,                                    System.Predicate<T> predicate) => System.Array.FindIndex    <T>(array,               predicate);
    [PatchMethod(AggressiveInlining)] public static int  FindIndex    <T>(this T[] array, int              index,            System.Predicate<T> predicate) => System.Array.FindIndex    <T>(array, index,        predicate);
    [PatchMethod(AggressiveInlining)] public static int  FindIndex    <T>(this T[] array, int              index, int count, System.Predicate<T> predicate) => System.Array.FindIndex    <T>(array, index, count, predicate);
    [PatchMethod(AggressiveInlining)] public static T?   FindLast     <T>(this T[] array,                                    System.Predicate<T> predicate) => System.Array.FindLast     <T>(array,               predicate);
    [PatchMethod(AggressiveInlining)] public static int  FindLastIndex<T>(this T[] array,                                    System.Predicate<T> predicate) => System.Array.FindLastIndex<T>(array,               predicate);
    [PatchMethod(AggressiveInlining)] public static int  FindLastIndex<T>(this T[] array, int              index,            System.Predicate<T> predicate) => System.Array.FindLastIndex<T>(array, index,        predicate);
    [PatchMethod(AggressiveInlining)] public static int  FindLastIndex<T>(this T[] array, int              index, int count, System.Predicate<T> predicate) => System.Array.FindLastIndex<T>(array, index, count, predicate);
    [PatchMethod(AggressiveInlining)] public static void ForEach      <T>(this T[] array, System.Action<T> action)                                          => System.Array.ForEach      <T>(array, action);

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component>  GetChildren   (this UnityEngine.Component  component)                                  => component .FindChildrenByComponent(component.GetType());
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      GetChildren<T>(this UnityEngine.Component  component) where T : UnityEngine.Component  => component .FindChildrenByComponent<T>();
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> GetChildren   (this UnityEngine.GameObject gameObject)                                 => gameObject.FindChildren   ([PatchMethod(AggressiveInlining)] static (child) => true);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      GetChildren<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject.FindChildren<T>([PatchMethod(AggressiveInlining)] static (child) => true);

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component>  GetDescendants   (this UnityEngine.Component  component)                                  => component .FindDescendantsByComponent(component.GetType());
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      GetDescendants<T>(this UnityEngine.Component  component) where T : UnityEngine.Component  => component .FindDescendantsByComponent<T>();
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> GetDescendants   (this UnityEngine.GameObject gameObject)                                 => gameObject.FindDescendants   ([PatchMethod(AggressiveInlining)] static (child) => true);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      GetDescendants<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject.FindDescendants<T>([PatchMethod(AggressiveInlining)] static (child) => true);

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component>  GetHierarchy   (this UnityEngine.Component  component)                                  => component .FindHierarchyByComponent(component.GetType());
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      GetHierarchy<T>(this UnityEngine.Component  component) where T : UnityEngine.Component  => component .FindHierarchyByComponent<T>();
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> GetHierarchy   (this UnityEngine.GameObject gameObject)                                 => gameObject.FindHierarchy   ([PatchMethod(AggressiveInlining)] static (child) => true);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      GetHierarchy<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject.FindHierarchy<T>([PatchMethod(AggressiveInlining)] static (child) => true);

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

    [PatchMethod(AggressiveInlining)] public static bool IsEmpty<T>           (this    T[]                                                                       array)            => 0  == array           .Length;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty              (this    System.Array                                                              array)            => 0  == array           .Length;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty<T>           (in this System.ArraySegment<T>                                                    arraySegment)     => 0  == arraySegment    .Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty              (this    System.Collections.ArrayList                                              arrayList)        => 0  == arrayList       .Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty              (this    System.Collections.BitArray                                               bits)             => 0  == bits            .Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty              (this    System.Collections.Queue                                                  queue)            => 0  == queue           .Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty<TKey, TValue>(this    System.Collections.Generic.Dictionary      <TKey, TValue>                 dictionary)       => 0  == dictionary      .Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty<T>           (this    System.Collections.Generic.HashSet         <T>                            hashset)          => 0  == hashset         .Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty<T>           (this    System.Collections.Generic.LinkedList      <T>                            list)             => 0  == list            .Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty<T>           (this    System.Collections.Generic.List            <T>                            list)             => 0  == list            .Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty<T>           (this    System.Collections.Generic.Queue           <T>                            queue)            => 0  == queue           .Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty<TKey, TValue>(this    System.Collections.Generic.SortedDictionary<TKey, TValue>                 sortedDictionary) => 0  == sortedDictionary.Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty<TKey, TValue>(this    System.Collections.Generic.SortedList      <TKey, TValue>                 sortedList)       => 0  == sortedList      .Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty<T>           (this    System.Collections.Generic.SortedSet       <T>                            sortedSet)        => 0  == sortedSet       .Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty<T>           (this    System.Collections.Generic.Stack           <T>                            stack)            => 0  == stack           .Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty              (this    System.Collections.Hashtable                                              hashtable)        => 0  == hashtable       .Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty              (this    System.Collections.IEnumerable                                            enumerable)       => 0u == Util.EnumerableCount(enumerable);
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty<T>           (this    System.Collections.ObjectModel.ObservableCollection        <T>            collection)       => 0  == collection.Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty<T>           (this    System.Collections.ObjectModel.ReadOnlyCollection          <T>            collection)       => 0  == collection.Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty<TKey, TValue>(this    System.Collections.ObjectModel.ReadOnlyDictionary          <TKey, TValue> dictionary)       => 0  == dictionary.Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty<T>           (this    System.Collections.ObjectModel.ReadOnlyObservableCollection<T>            collection)       => 0  == collection.Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty              (this    System.Collections.SortedList                                             sortedList)       => 0  == sortedList.Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty              (this    System.Collections.Specialized.HybridDictionary                           dictionary)       => 0  == dictionary.Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty              (this    System.Collections.Specialized.ListDictionary                             dictionary)       => 0  == dictionary.Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty              (this    System.Collections.Specialized.NameValueCollection                        collection)       => 0  == collection.Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty              (this    System.Collections.Specialized.OrderedDictionary                          dictionary)       => 0  == dictionary.Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty              (this    System.Collections.Specialized.StringCollection                           collection)       => 0  == collection.Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty              (this    System.Collections.Specialized.StringDictionary                           dictionary)       => 0  == dictionary.Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty              (this    System.Collections.Stack                                                  stack)            => 0  == stack     .Count;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty              (this    System.IO.MemoryStream                                                    stream)           => 0L == stream    .Length;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty<T>           (in this System.Memory        <T>                                                  memory)           => 0  == memory    .Length;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty<T>           (in this System.ReadOnlyMemory<T>                                                  memory)           => 0  == memory    .Length;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty<T>           (in this System.Span          <T>                                                  span)             => 0  == span      .Length;
    [PatchMethod(AggressiveInlining)] public static bool IsEmpty<T>           (in this System.ReadOnlySpan  <T>                                                  span)             => 0  == span      .Length;
    #if NET9_0 || NET9_0_OR_GREATER
      [PatchMethod(AggressiveInlining)]
      public static bool IsEmpty<T>(this System.Collections.ObjectModel.ReadOnlySet<T> set) => 0 == set.Count;
    #endif

    [PatchMethod(AggressiveInlining)] public static int LastIndexOf   (this System.Array array, object? value)                       => System.Array.LastIndexOf   (array, value);
    [PatchMethod(AggressiveInlining)] public static int LastIndexOf   (this System.Array array, object? value, int index)            => System.Array.LastIndexOf   (array, value, index);
    [PatchMethod(AggressiveInlining)] public static int LastIndexOf   (this System.Array array, object? value, int index, int count) => System.Array.LastIndexOf   (array, value, index, count);
    [PatchMethod(AggressiveInlining)] public static int LastIndexOf<T>(this T[]          array, T       value)                       => System.Array.LastIndexOf<T>(array, value);
    [PatchMethod(AggressiveInlining)] public static int LastIndexOf<T>(this T[]          array, T       value, int index)            => System.Array.LastIndexOf<T>(array, value, index);
    [PatchMethod(AggressiveInlining)] public static int LastIndexOf<T>(this T[]          array, T       value, int index, int count) => System.Array.LastIndexOf<T>(array, value, index, count);

    [PatchMethod(AggressiveInlining)] public static ref readonly object? Prepend   (this System.Collections.ArrayList             arrayList, in object? value) { arrayList.Insert  (0, value); return ref value; }
    [PatchMethod(AggressiveInlining)] public static ref readonly T       Prepend<T>(this System.Collections.ArrayList             arrayList, in T       value) { arrayList.Insert  (0, value); return ref value; }
    [PatchMethod(AggressiveInlining)] public static ref readonly T       Prepend<T>(this System.Collections.Generic.LinkedList<T> list,      in T       value) { list     .AddFirst(value);    return ref value; }
    [PatchMethod(AggressiveInlining)] public static ref readonly T       Prepend<T>(this System.Collections.Generic.List      <T> list,      in T       value) { list     .Insert  (0, value); return ref value; }

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

    [PatchMethod(AggressiveInlining)] public static void TrimExcess<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary) => dictionary.TrimExcess(dictionary.Count);
    [PatchMethod(AggressiveInlining)] public static void TrimExcess<T>           (this System.Collections.Generic.IList      <T>            list)       => list      .TrimExcess(list      .Count);

    [PatchMethod(AggressiveInlining)]
    public static void TrimExcess<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary, int capacity) {
      switch (dictionary) {
        case PatchOdyssey.Collections.RefDictionary<TKey, TValue> subdictionary: subdictionary.TrimExcess((uint) capacity); break;
        case System.Collections.Generic.Dictionary <TKey, TValue> subdictionary: subdictionary.TrimExcess(capacity);        break;
      }
    }

    [PatchMethod(AggressiveInlining)]
    public static void TrimExcess<T>(this System.Collections.Generic.IList<T> list, int capacity) {
      switch (list) {
        case PatchOdyssey.Collections.RefList   <T> sublist: sublist.TrimExcess((uint) capacity); break;
        case PatchOdyssey.Collections.SharedList<T> sublist: sublist.TrimExcess((uint) capacity); break;
        case System.Collections.Generic.List    <T> sublist: sublist.Capacity = capacity;         break;
      }
    }

    [PatchMethod(AggressiveInlining)]
    public static bool TrueForAll<T>(this T[] array, System.Predicate<T> predicate) {
      return System.Array.TrueForAll<T>(array, predicate);
    }

    [PatchMethod(AggressiveInlining)] public static bool TryAdd   <T>(this System.Collections.Generic.IList<T> list, in T element) { if (!list.Contains(element)) { list.Add(element); return true; } return false; }
    [PatchMethod(AggressiveInlining)] public static T    TryAppend<T>(this System.Collections.Generic.IList<T> list, in T element) { int index = list.IndexOf(element); if (index == -1) { list.Add(element); return element; } return list[index]; }

    [PatchMethod(AggressiveInlining)] public static bool   TryAdd   <TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary, System.Collections.Generic.KeyValuePair<TKey, TValue> element)              =>  dictionary.TryAdd     (element.Key, element.Value);
    [PatchMethod(AggressiveInlining)] public static bool   TryAdd   <TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary, in TKey                                               key, in TValue value) => !dictionary.ContainsKey(key) && ((dictionary[key] = value), _: true)._;
    [PatchMethod(AggressiveInlining)] public static TValue TryAppend<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary, System.Collections.Generic.KeyValuePair<TKey, TValue> element)              =>  dictionary.TryAppend  (element.Key, element.Value);
    [PatchMethod(AggressiveInlining)] public static TValue TryAppend<TKey, TValue>(this System.Collections.Generic.IDictionary<TKey, TValue> dictionary, in TKey                                               key, in TValue value) => !dictionary.TryGetValue(key, out TValue prevalue) ? dictionary[key] = value : prevalue;

    #if !(NETCOREAPP2_1 || NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1 || NETSTANDARD2_1_OR_GREATER)
      [PatchMethod(AggressiveInlining)] public static void TrimExcess<TKey, TValue>(this System.Collections.Generic.Dictionary<TKey, TValue> dictionary)               { /* ⟶ Do nothing… */ }
      [PatchMethod(AggressiveInlining)] public static void TrimExcess<TKey, TValue>(this System.Collections.Generic.Dictionary<TKey, TValue> dictionary, int capacity) { /* ⟶ Do nothing… */ }
    #endif
  }

  public static class Traits /* ⟶ Metaprogramming generics */ {
    private static class TypeInfo<T> {
      public static readonly bool IsConstructibleType = Traits.IsConstructibleType(typeof(T));
      public static readonly bool IsPrimitiveType     = Traits.IsPrimitiveType    (typeof(T));
      public static readonly bool IsReferenceType     = Traits.IsReferenceType    (typeof(T));
      public static readonly bool IsUnmanagedType     = Traits.IsUnmanagedType    (typeof(T));
      public static readonly bool IsValueType         = Traits.IsValueType        (typeof(T));
    }

    internal abstract class ConstructibleConstraint       <T> where T : new()     {}
    internal abstract class NonNullableConstraint         <T> where T : notnull   {}
    internal abstract class NonNullableReferenceConstraint<T> where T : class     {}
    internal abstract class ReferenceConstraint           <T> where T : class?    {}
    internal abstract class UnmanagedConstraint           <T> where T : unmanaged {}
    internal abstract class ValueConstraint               <T> where T : struct    {}

    /* … */
    [PatchMethod(AggressiveInlining), PatchResolution(1)] public static bool IsConstructibleType<T>()                 => Traits.TypeInfo<T>.IsConstructibleType;
    [PatchMethod(AggressiveInlining), PatchResolution(1)] public static bool IsConstructibleType   (System.Type type) { try { typeof(Traits.ConstructibleConstraint<>).MakeGenericType(type); return true; } catch {} return false; }
    [PatchMethod(AggressiveInlining), PatchResolution(1)] public static bool IsPrimitiveType<T>    ()                 => Traits.TypeInfo<T>.IsPrimitiveType;
    [PatchMethod(AggressiveInlining), PatchResolution(1)] public static bool IsPrimitiveType       (System.Type type) => type.IsPrimitive; // ⟶ `type == typeof(bool) || type == typeof(byte) || type == typeof(char) || type == typeof(double) || type == typeof(float) || type == typeof(int) || type == typeof(long) || type == typeof(sbyte) || type == typeof(short) || type == typeof(uint) || type == typeof(ulong) || type == typeof(ushort) || type == typeof(System.IntPtr) || type == typeof(System.UIntPtr)`
    [PatchMethod(AggressiveInlining), PatchResolution(1)] public static bool IsReferenceType<T>    ()                 => Traits.TypeInfo<T>.IsReferenceType;
    [PatchMethod(AggressiveInlining), PatchResolution(1)] public static bool IsReferenceType       (System.Type type) { try { typeof(Traits.ReferenceConstraint<>).MakeGenericType(type); return true; } catch {} return false; }
    [PatchMethod(AggressiveInlining), PatchResolution(0)] public static bool IsUnmanagedType<T>    ()                 => Traits.TypeInfo<T>.IsUnmanagedType;
    [PatchMethod(AggressiveInlining), PatchResolution(0)] public static bool IsUnmanagedType       (System.Type type) { try { typeof(Traits.UnmanagedConstraint<>).MakeGenericType(type); return true; } catch {} return false; }
    [PatchMethod(AggressiveInlining), PatchResolution(1)] public static bool IsValueType<T>        ()                 => Traits.TypeInfo<T>.IsValueType;
    [PatchMethod(AggressiveInlining), PatchResolution(1)] public static bool IsValueType           (System.Type type) { if (type.IsByRef || type == typeof(void) || System.Nullable.GetUnderlyingType(type) is not null) return false; if (type.IsEnum || type.IsPointer || type.IsValueType || typeof(System.Enum).IsAssignableFrom(type) || typeof(System.ValueType).IsAssignableFrom(type)) return true; try { typeof(Traits.ValueConstraint<>).MakeGenericType(type); return true; } catch {} return false; }
  }

  public static partial class Util /* ⟶ Utilities */ {
    public static class Array /* ⟶ Based on `Util.Array<T>` for non-generics */ {
      public readonly struct Enumerable : System.Collections.IEnumerable {
        private readonly System.Collections.IEnumerable value;

        /* … */
        [PatchMethod(AggressiveInlining)]
        public Enumerable(System.Collections.IEnumerable? enumerable) => this.value = enumerable ?? Array.From();

        /* … */
        [PatchMethod(AggressiveInlining)] public System.Collections.IEnumerator GetEnumerator                               () => this.value.GetEnumerator();
        [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator        System.Collections.IEnumerable.GetEnumerator() => this      .GetEnumerator();

        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Array?                                    array)      => new(array);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.ArrayList?                    arrayList)  => new(arrayList);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.BitArray?                     bits)       => new(bits);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.Queue?                        queue)      => new(queue);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.SortedList?                   sortedList) => new(sortedList);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.Specialized.StringCollection? collection) => new(collection);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.Stack?                        stack)      => new(stack);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.IO.MemoryStream?                          stream)     => new(Array.From(stream).ConvertAll([PatchMethod(AggressiveInlining)] static (element) => (object?) element));
      }

      /* … */
      public static void Copy(System.Array sourceArray, uint sourceIndex, System.Array destinationArray, uint destinationIndex, uint count) {
        (System.Type destinationType, System.Type sourceType) = (destinationArray.GetType().GetElementType(), sourceArray.GetType().GetElementType());

        // …
        if (Traits.IsPrimitiveType(destinationType) && Traits.IsPrimitiveType(sourceType)) {
          (int destinationElementSize, int sourceElementSize) = (System.Runtime.InteropServices.Marshal.SizeOf(destinationType), System.Runtime.InteropServices.Marshal.SizeOf(sourceType));

          if (destinationElementSize == sourceElementSize) {
            System.Buffer.BlockCopy(sourceArray, (int) sourceIndex * sourceElementSize, destinationArray, (int) destinationIndex * destinationElementSize, (int) count * sourceElementSize);
            return;
          }
        }

        System.Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, count);
      }

      [PatchMethod(AggressiveInlining)] public static System.Array From()                                                            => System.Array.Empty<object>();
      [PatchMethod(AggressiveInlining)] public static System.Array From(System.Array?                                    array)      => array ?? Array.From();
      [PatchMethod(AggressiveInlining)] public static object?[]    From(System.Collections.ArrayList?                    arrayList)  => arrayList is not null ? arrayList.ToArray() : System.Array.Empty<object?>();
      [PatchMethod(AggressiveInlining)] public static bool   []    From(System.Collections.BitArray?                     bits)       { if (!(bits?.IsEmpty() ?? true)) { bool[] array = new bool[bits!.Length]; uint length = 0u; foreach (bool bit in bits) { array[length++] = bit; } return array; } return System.Array.Empty<bool>(); }
      [PatchMethod(AggressiveInlining)] public static object?[]    From(System.Collections.IEnumerable?                  enumerable) { if (enumerable is not null) { System.Collections.Queue array = new(); for (System.Collections.IEnumerator enumerator = enumerable.GetEnumerator(); enumerator.MoveNext(); ) array.Enqueue(enumerator.Current); return Array.From(array); } return System.Array.Empty<object?>(); }
      [PatchMethod(AggressiveInlining)] public static object?[]    From(System.Collections.Queue?                        queue)      => queue      is not null ? queue.ToArray()               : System.Array.Empty<object?>();
      [PatchMethod(AggressiveInlining)] public static object?[]    From(System.Collections.SortedList?                   sortedList) => sortedList is not null ? Array.From(sortedList.Values) : System.Array.Empty<object?>();
      [PatchMethod(AggressiveInlining)] public static string?[]    From(System.Collections.Specialized.StringCollection? collection) { if (!(collection?.IsEmpty() ?? true)) { string?[] array = new string?[collection!.Count]; uint length = 0u; foreach (string? element in collection) { array[length++] = element; } return array; } return System.Array.Empty<string?>(); }
      [PatchMethod(AggressiveInlining)] public static object?[]    From(System.Collections.Stack?                        stack)      => stack  is not null ? stack .ToArray() : System.Array.Empty<object?>();
      [PatchMethod(AggressiveInlining)] public static byte   []    From(System.IO.MemoryStream?                          stream)     => stream is not null ? stream.ToArray() : System.Array.Empty<byte>   ();
      public static object?[] From(params Array.Enumerable[] enumerables) { System.Collections.Queue array = new(enumerables.Length); foreach (Array.Enumerable enumerable in enumerables) { foreach (object? element in enumerable) array.Enqueue(element); } return Array.From(array); }
    }

    public static class Array<T> /* ⟶ Based on `Util.Reference<T>` for `𝑓 Util.Array<T>.Copy(…)` */ {
      public readonly struct Enumerable : System.Collections.Generic.IEnumerable<T> /* ⟶ Solely for `Util.Array<T>.From(…)` */ {
        public readonly struct Enumerator : System.Collections.Generic.IEnumerator<T> {
          public  T                                       Current => (T) this.enumerator.Current!;
          private readonly System.Collections.IEnumerator enumerator;
          T                                               System.Collections.Generic.IEnumerator<T>.Current => this.Current;
          object                                          System.Collections.IEnumerator.Current            => this.Current!;

          /* … */
          public Enumerator(System.Collections.IEnumerator enumerator) => this.enumerator = enumerator;

          /* … */
          public void Dispose                                () { /* Do nothing… */ }
          public bool MoveNext                               () => this.enumerator.MoveNext();
          public void Reset                                  () => this.enumerator.Reset   ();
          bool        System.Collections.IEnumerator.MoveNext() => this           .MoveNext();
          void        System.Collections.IEnumerator.Reset   () => this           .Reset   ();
          void        System.IDisposable.Dispose             () => this           .Dispose ();
        }

        /* … */
        private readonly System.Collections.IEnumerable value;

        /* … */
        [PatchMethod(AggressiveInlining)]
        public Enumerable(System.Collections.IEnumerable? enumerable) => this.value = enumerable ?? Array<T>.From();

        /* … */
        [PatchMethod(AggressiveInlining)] public Enumerable.Enumerator              GetEnumerator                                          () => new(this.value.GetEnumerator());
        [PatchMethod(AggressiveInlining)] System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() => this.GetEnumerator();
        [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator            System.Collections.IEnumerable.GetEnumerator           () => this.GetEnumerator();

        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(T[]?                                                            array)        => new(array);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Array?                                                   array)        => new(array);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(in System.ArraySegment<T>                                       arraySegment) => new(arraySegment);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.ArraySegment<T>?                                         arraySegment) => new(arraySegment);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.ArrayList?                                   arrayList)    => new(arrayList);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.BitArray?                                    bits)         => new(bits);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.Generic.List                            <T>? list)         => new(list);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.Generic.Queue                           <T>? queue)        => new(queue);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.Generic.Stack                           <T>? stack)        => new(stack);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.Generic.HashSet                         <T>? hashset)      => new(hashset);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.Generic.LinkedList                      <T>? list)         => new(list);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.Generic.SortedSet                       <T>? sortedSet)    => new(sortedSet);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.ObjectModel.ObservableCollection        <T>? collection)   => new(collection as System.Collections.ObjectModel.Collection<T>);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.ObjectModel.ReadOnlyCollection          <T>? collection)   => new(collection);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.ObjectModel.ReadOnlyObservableCollection<T>? collection)   => new(collection as System.Collections.ObjectModel.ReadOnlyCollection<T>);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.Queue?                                       queue)        => new(queue);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.SortedList?                                  sortedList)   => new(sortedList?.Values);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.Specialized.StringCollection?                collection)   => new(collection);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Collections.Stack?                                       stack)        => new(stack);
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.IO.MemoryStream?                                         stream)       => new(Util.Array.From(stream));
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(in System.Memory        <T>                                     memory)       => new(Array<T>  .From(in memory));
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.Memory           <T>?                                    memory)       => new(Array<T>  .From(memory));
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(in System.ReadOnlyMemory<T>                                     memory)       => new(Array<T>  .From(in memory));
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(System.ReadOnlyMemory   <T>?                                    memory)       => new(Array<T>  .From(memory));
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(in System.ReadOnlySpan  <T>                                     span)         => new(Array<T>  .From(span));
        [PatchMethod(AggressiveInlining)] public static implicit operator Enumerable(in System.Span          <T>                                     span)         => new(Array<T>  .From(span));
        #if NET9_0 || NET9_0_OR_GREATER
          [PatchMethod(AggressiveInlining)]
          public static implicit operator Enumerable(this System.Collections.ObjectModel.ReadOnlySet<T> set) => new(set);
        #endif
      }

      private readonly struct Sentinel {}

      /* … */
      public static readonly PatchOdyssey.ArrayCopier<T> Copy = (PatchOdyssey.ArrayCopier<T>) (Traits.IsPrimitiveType<T>() ? ((PatchOdyssey.ArrayCopier<Array<T>.Sentinel>) Array<Array<T>.Sentinel>.UnmanagedCopy).Method.GetGenericMethodDefinition().MakeGenericMethod(typeof(T)).CreateDelegate(typeof(PatchOdyssey.ArrayCopier<T>)) : (PatchOdyssey.ArrayCopier<T>) Array<T>.ManagedCopy<T>);

      /* … */
      [PatchMethod(AggressiveInlining)] public static T[] From      ()                                                              => System.Array.Empty<T>();
      [PatchMethod(AggressiveInlining)] public static T[] From      (T[]?                                             array)        => array ?? Array<T>.From();
      [PatchMethod(AggressiveInlining)] public static T[] From      (PatchOdyssey.Collections.RefReadOnlyList<T>?     list)         { if (!(list?.IsEmpty() ?? true)) { T[] array = new T[list!.Count]; list.CopyTo(array); return array; } return System.Array.Empty<T>(); }
      [PatchMethod(AggressiveInlining)] public static T[] From      (PatchOdyssey.Collections.SharedList     <T>?     list)         { if (!(list?.IsEmpty() ?? true)) { T[] array = new T[list!.Count]; list.CopyTo(array); return array; } return System.Array.Empty<T>(); }
      [PatchMethod(AggressiveInlining)] public static T[] From      (System.Array?                                    array)        => (T[]) (array ?? Array<T>.From());
      [PatchMethod(AggressiveInlining)] public static T[] From      (in System.ArraySegment<T>                        arraySegment) => arraySegment .ToArray();
      [PatchMethod(AggressiveInlining)] public static T[] From      (System.ArraySegment   <T>?                       arraySegment) => arraySegment?.ToArray() ?? Array<T>.From();
      [PatchMethod(AggressiveInlining)] public static T[] From      (System.Collections.ArrayList?                    arrayList)    => (T[]) (arrayList?.ToArray(typeof(T)) ?? Array<T>.From());
      [PatchMethod(AggressiveInlining)] public static T[] From      (System.Collections.BitArray?                     bits)         => Util.Array.From(bits)      .ConvertAll([PatchMethod(AggressiveInlining)] static (element) => Util.GetConverter<bool,    T>()(in element));
      [PatchMethod(AggressiveInlining)] public static T[] From      (System.Collections.IEnumerable?                  enumerable)   => Util.Array.From(enumerable).ConvertAll([PatchMethod(AggressiveInlining)] static (element) => Util.GetConverter<object?, T>()(in element));
      [PatchMethod(AggressiveInlining)] public static T[] From      (System.Collections.Generic.IEnumerable<T>?       enumerable)   { if (enumerable is not null) { System.Collections.Generic.Queue<T> array = new(); using (System.Collections.Generic.IEnumerator<T> enumerator = enumerable.GetEnumerator()) { while (enumerator.MoveNext()) array.Enqueue(enumerator.Current); } return Util.Array<T>.From(array); } return Util.Array<T>.From(); }
      [PatchMethod(AggressiveInlining)] public static T[] From      (System.Collections.Generic.List       <T>?       list)         => list ?.ToArray() ?? Array<T>.From();
      [PatchMethod(AggressiveInlining)] public static T[] From      (System.Collections.Generic.Queue      <T>?       queue)        => queue?.ToArray() ?? Array<T>.From();
      [PatchMethod(AggressiveInlining)] public static T[] From      (System.Collections.Generic.Stack      <T>?       stack)        => stack?.ToArray() ?? Array<T>.From();
      [PatchMethod(AggressiveInlining)] public static T[] From      (System.Collections.Queue?                        queue)        => Util.Array.From(queue)     .ConvertAll([PatchMethod(AggressiveInlining)] static (element) => Util.GetConverter<object?, T>()(in element));
      [PatchMethod(AggressiveInlining)] public static T[] From      (System.Collections.Specialized.StringCollection? collection)   => Util.Array.From(collection).ConvertAll([PatchMethod(AggressiveInlining)] static (element) => Util.GetConverter<string?, T>()(in element));
      [PatchMethod(AggressiveInlining)] public static T[] From      (System.Collections.Stack?                        stack)        => Util.Array.From(stack)     .ConvertAll([PatchMethod(AggressiveInlining)] static (element) => Util.GetConverter<object?, T>()(in element));
      [PatchMethod(AggressiveInlining)] public static T[] From<TKey>(System.Collections.Generic.SortedList<TKey, T>   sortedList)   => !(sortedList?.IsEmpty() ?? true) ? Array<T>.From(sortedList!.Values) : Array<T>.From();
      [PatchMethod(AggressiveInlining)] public static T[] From      (System.Collections.SortedList                    sortedList)   => Util.Array.From(sortedList).ConvertAll([PatchMethod(AggressiveInlining)] static (element) => Util.GetConverter<object?, T>()(in element));
      [PatchMethod(AggressiveInlining)] public static T[] From      (System.IO.MemoryStream?                          stream)       => Util.Array.From(stream)    .ConvertAll([PatchMethod(AggressiveInlining)] static (element) => Util.GetConverter<byte,    T>()(in element));
      [PatchMethod(AggressiveInlining)] public static T[] From      (in System.Memory        <T>                      memory)       => memory .ToArray();
      [PatchMethod(AggressiveInlining)] public static T[] From      (System.Memory           <T>?                     memory)       => memory?.ToArray() ?? Array<T>.From();
      [PatchMethod(AggressiveInlining)] public static T[] From      (in System.ReadOnlyMemory<T>                      memory)       => memory .ToArray();
      [PatchMethod(AggressiveInlining)] public static T[] From      (System.ReadOnlyMemory   <T>?                     memory)       => memory?.ToArray() ?? Array<T>.From();
      [PatchMethod(AggressiveInlining)] public static T[] From      (in System.Span          <T>                      span)         => span   .ToArray();
      [PatchMethod(AggressiveInlining)] public static T[] From      (in System.ReadOnlySpan  <T>                      span)         => span   .ToArray();

      public static T[] From(params Array<T>.Enumerable[] enumerables) {
        System.Collections.Generic.Queue<T> array = new(enumerables.Length);

        // …
        foreach (Array<T>.Enumerable enumerable in enumerables) {
          foreach (T element in enumerable)
          array.Enqueue(element);
        }

        return Array<T>.From(array);
      }

      [PatchMethod(AggressiveInlining)] private        static void ManagedCopy  <U>(U[] sourceArray, uint sourceIndex, U[] destinationArray, uint destinationIndex, uint count)                     => System.Array.Copy      (sourceArray, sourceIndex,                   destinationArray, destinationIndex,                   count);
      [PatchMethod(AggressiveInlining)] private unsafe static void UnmanagedCopy<U>(U[] sourceArray, uint sourceIndex, U[] destinationArray, uint destinationIndex, uint count) where U : unmanaged => System.Buffer.BlockCopy(sourceArray, (int) sourceIndex * sizeof(U), destinationArray, (int) destinationIndex * sizeof(U), (int) count * sizeof(U));
    }

    public static class Load {
      internal /* readonly */ struct LoadCachedIndex { public /* readonly */ uint value; }
      internal /* readonly */ struct LoadFailedIndex { public /* readonly */ uint value; }

      /* … */
      public   const           double                                                                                               Asynchronously = 0.0;
      public   const           uint                                                                                                 Once           = 0u;
      internal static readonly PatchOdyssey.Collections.RefDictionary<(System.Type, System.Uri), PatchOdyssey.Collections.LoadInfo> Pending        = new(16u);
      public   const           uint                                                                                                 Persistently   = uint.MaxValue;
      public   const           double                                                                                               Synchronously  = double.PositiveInfinity;
      public   const           bool                                                                                                 WithCache      = true;
      public   const           bool                                                                                                 WithoutCache   = false;

      /* … */
      [PatchMethod(AggressiveInlining)]
      private static void Idle(object? target, in PatchOdyssey.Events.LoadEvent data) {}

      private static T? Uri<T>(System.Uri path, PatchOdyssey.Handler<PatchOdyssey.Events.LoadEvent> callback, double timeout, bool cached, uint retries, System.Func<System.Uri, UnityEngine.Networking.UnityWebRequest> requester, System.Func<UnityEngine.Networking.UnityWebRequest, T> loader, System.Func<T, T> recacher, PatchOdyssey.Handler<PatchOdyssey.Events.LoadEvent> fallback, bool restarted) where T : class? {
        ref PatchOdyssey.Collections.LoadInfo                load             = ref Load.Pending.TryAppend((typeof(T), path), new());
        uint                                                 attempts         = restarted ? load.events[0].metadata.data.attempts : 1u;
        uint                                                 attemptsAllowed  = System.Math.Max(retries, uint.MaxValue - 1u)      + 1u;
        PatchOdyssey.Collections.Mono<T>                     cachedPayload    = default;
        bool                                                 failed           = false; // ⟶ `UnityEngine.Networking.UnityWebRequest.Result.*Error`, `timeout`, …, e.t.c.
        uint                                                 loadHandlerIndex = restarted ? 0u : default;
        UnityEngine.Networking.UnityWebRequestAsyncOperation operation        = null!; // ⟶ Able to access the `UnityEngine.Application.streamingAssetsPath` directory
        bool                                                 requested        = restarted;
        System.Diagnostics.Stopwatch                         stopwatch        = new(); // ⟶ Track `timeout`

        /* … */
        [PatchMethod(AggressiveInlining)]
        static ref readonly T GetCache(ref PatchOdyssey.Collections.LoadInfo load, ref PatchOdyssey.Collections.Mono<T> payload, System.Func<T, T> recacher) {
          payload = !payload.HasValue ? recacher((T) load.cached!) : payload;
          return ref payload.Value;
        }

        [PatchMethod(AggressiveInlining)]
        static void LoadCached(ref PatchOdyssey.Collections.LoadInfo load, ref PatchOdyssey.Collections.Mono<T> payload, System.Func<T, T> recacher) {
          PatchOdyssey.Collections.SharedList<Load.LoadCachedIndex> resolved = new();

          // …
          for (uint index = resolved.IsEmpty() ? load.events.CountInvocationList() : resolved[resolved.Count - 1u].value; 0u != index--; ) {
            ref PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Events.LoadEvent> loadHandler = ref load.events[index];
            ref PatchOdyssey.Events.LoadEvent                                       loadEvent   = ref loadHandler.metadata;

            // …
            if (loadEvent.data.cached) {
              loadEvent.callback      = loadHandler.value;
              loadEvent.data.duration = UnityEngine.Time.realtimeSinceStartupAsDouble - loadEvent.data.duration;
              loadEvent.data.payload  = GetCache(ref load, ref payload, recacher);

              resolved.Add(new() {value = index});
              loadHandler.Invoke();
            }
          }

          foreach (ref readonly Load.LoadCachedIndex index in resolved)
            load.events.RemoveAt(index.value);

          resolved.Clear();
        }

        [PatchMethod(AggressiveInlining)]
        static void LoadFailed(ref PatchOdyssey.Collections.LoadInfo load) /* ⟶ Invoke currently failed and asynchronous pending handlers, otherwise update remaining pending */ {
          PatchOdyssey.Collections.SharedList<Load.LoadFailedIndex> resolved = new();

          // …
          load.payload = null;

          for (uint index = resolved.IsEmpty() ? load.events.CountInvocationList() : resolved[resolved.Count - 1u].value; 0u != index--; ) {
            ref PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Events.LoadEvent> loadHandler = ref load.events[index];
            ref PatchOdyssey.Events.LoadEvent                                       loadEvent   = ref loadHandler.metadata;
            PatchOdyssey.Handler<PatchOdyssey.Events.LoadEvent>                     fallback    = loadEvent.callback;

            // …
            if (loadEvent.data.attempts == loadEvent.data.attemptsAllowed) {
              loadEvent.callback      = loadHandler.value;
              loadEvent.data.duration = UnityEngine.Time.realtimeSinceStartupAsDouble - loadEvent.data.duration;
              load.events[index]      = new(fallback, loadHandler.target, loadHandler.metadata);

              resolved.Add(new() {value = index});
              loadHandler.Invoke();
            } else ++loadEvent.data.attempts;
          }

          foreach (ref readonly Load.LoadFailedIndex index in resolved)
            load.events.RemoveAt(index.value);

          resolved.Clear();
        }

        [PatchMethod(AggressiveInlining)]
        static T LoadSucceeded(ref PatchOdyssey.Collections.LoadInfo load, UnityEngine.Networking.UnityWebRequest handler, System.Func<UnityEngine.Networking.UnityWebRequest, T> loader) {
          T payload = loader(handler);

          // …
          load.cached = load.payload = payload;

          foreach (ref PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Events.LoadEvent> loadHandler in load.events) {
            ref PatchOdyssey.Events.LoadEvent loadEvent = ref loadHandler.metadata;

            // …
            loadEvent.callback      = loadHandler.value;
            loadEvent.data.duration = UnityEngine.Time.realtimeSinceStartupAsDouble - loadEvent.data.duration;
            loadEvent.data.payload  = payload;

            loadHandler.Invoke();
          }

          load.events.Clear();

          return payload;
        }

        [PatchMethod(Synchronized)]
        /* static */ void LoadUri(UnityEngine.AsyncOperation _) /* ⟶ Captures `load`, `loader`, and `loadHandler` */ {
          ref PatchOdyssey.Collections.LoadInfo                                   load        = ref Load.Pending[(typeof(T), path)];
          ref PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Events.LoadEvent> loadHandler = ref load.events[loadHandlerIndex];
          UnityEngine.Networking.UnityWebRequestAsyncOperation                    operation   = (UnityEngine.Networking.UnityWebRequestAsyncOperation) _;

          // …
          using (UnityEngine.Networking.UnityWebRequest request = operation.webRequest) // ⟶ `request.uri` could be modified through redirection unless `::redirectLimit = 0`
          if (UnityEngine.Networking.UnityWebRequest.Result.Success != request.result) {
            request.disposeDownloadHandlerOnDispose = true;

            if (loadHandler.metadata.data.attempts != loadHandler.metadata.data.attemptsAllowed) {
              operation                                            = requester(path).SendWebRequest();
              operation.completed                                 += LoadUri;
              operation.webRequest.disposeDownloadHandlerOnDispose = typeof(T) != typeof(UnityEngine.Networking.DownloadHandler);
              loadHandler.metadata.data.attempts                  += 1u;
              load.events[loadHandlerIndex]                        = new(loadHandler.value, operation, loadHandler.metadata);
            }

            else {
              LoadFailed(ref load);
              if (!load.events.IsEmpty()) ReloadUri(ref load, requester, loader, recacher);
            }
          } else LoadSucceeded(ref load, request, loader);
        }

        [PatchMethod(AggressiveInlining)]
        static void ReloadUri(ref PatchOdyssey.Collections.LoadInfo load, System.Func<System.Uri, UnityEngine.Networking.UnityWebRequest> requester, System.Func<UnityEngine.Networking.UnityWebRequest, T> loader, System.Func<T, T> recacher) /* ⟶ Use pending asynchronous handler to get payload (ideally does not raise a `System.StackOverflowException`) */ {
          ref PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Events.LoadEvent> loadHandler = ref load.events[0];
          Load.Uri<T>(loadHandler.metadata.data.path, loadHandler.value, Load.Asynchronously, loadHandler.metadata.data.cached, loadHandler.metadata.data.attemptsAllowed - 1u, requester, loader, recacher, loadHandler.metadata.callback, true);
        }

        // … ⟶ Get cache
        if (load.cached is not null) {
          LoadCached(ref load, ref cachedPayload, recacher);

          if (cached) {
            if      (!restarted)             callback(null, new() {callback = callback, data = (attempts, attemptsAllowed, true, 0.0, path, GetCache(ref load, ref cachedPayload, recacher))}); // ⟶ Otherwise already cleared and invoked via `LoadCached(…)`
            else if (!load.events.IsEmpty()) ReloadUri(ref load, requester, loader, recacher);                                                                                                                               // ⟶ Avoid dead-locking the load queue

            return GetCache(ref load, ref cachedPayload, recacher);
          }
        }

        // … ⟶ Get payload
        do {
          stopwatch.Restart();

          // … ⟶ Wait in load queue until prior handler is complete
          if (!requested && !load.events.IsEmpty()) {
            // … ⟶ `Load.Asynchronously` — Handler awaits completion of predecessor
            if (double.IsNaN(timeout) || 0.0 >= timeout) {
              stopwatch.Stop();
              load.events += new PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Events.LoadEvent>(callback, null, new() {callback = fallback, data = (attempts, attemptsAllowed, cached, UnityEngine.Time.realtimeSinceStartupAsDouble, path, null)});

              return null;
            }

            // … ⟶ `Load.Synchronously` — Handler blocks until completion of predecessor
            else {
              while (!load.events.IsEmpty() && stopwatch.Elapsed.TotalSeconds < timeout)                                 continue; // ⟶ `while …`
              if    (attempts != attemptsAllowed && (load.payload is null || stopwatch.Elapsed.TotalSeconds >= timeout)) continue; // ⟶ `do … while`

              stopwatch.Stop();
              (load.payload is not null ? callback : fallback)(null, new() {callback = callback, data = (attempts, attemptsAllowed, cached, stopwatch.Elapsed.TotalSeconds, path, load.payload)});

              return load.payload as T;
            }
          }

          // … ⟶ Future handlers will wait on this handler to complete
          failed                                               = false;
          operation                                            = requester(path).SendWebRequest(); // ⟶ Was unaware of `int UnityEngine.Networking.UnityWebRequest::timeout` beforehand
          operation.webRequest.disposeDownloadHandlerOnDispose = typeof(T) != typeof(UnityEngine.Networking.DownloadHandler);

          if (!requested) {
            loadHandlerIndex = load.events.CountInvocationList();
            load.events     += new PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Events.LoadEvent>(callback, operation, new() {callback = fallback, data = (attempts, attemptsAllowed, cached, UnityEngine.Time.realtimeSinceStartupAsDouble, path, null)});
            requested        = true;
          }

          load.events[loadHandlerIndex] = new(load.events[loadHandlerIndex].value, operation, load.events[loadHandlerIndex].metadata);

          for (UnityEngine.Networking.UnityWebRequest request = operation.webRequest; !failed && UnityEngine.Networking.UnityWebRequest.Result.Success != request.result; failed = failed || UnityEngine.Networking.UnityWebRequest.Result.ConnectionError == request.result || UnityEngine.Networking.UnityWebRequest.Result.DataProcessingError == request.result || UnityEngine.Networking.UnityWebRequest.Result.ProtocolError == request.result)
          if (UnityEngine.Networking.UnityWebRequest.Result.InProgress == request.result) {
            // … ⟶ `Load.Asynchronously`
            if (double.IsNaN(timeout) || 0.0 >= timeout) {
              operation.completed += LoadUri;
              return null;
            }

            // … ⟶ `Load.Synchronously`
            failed = stopwatch.Elapsed.TotalSeconds >= timeout;
          }

          using (UnityEngine.Networking.UnityWebRequest request = operation.webRequest) {
            if (failed) {
              load.events[loadHandlerIndex].metadata.data.attempts += 1u;
              request.disposeDownloadHandlerOnDispose              =  true;

              continue;
            }

            stopwatch.Stop();
            return LoadSucceeded(ref load, request, loader);
          }
        } while (attempts++ != attemptsAllowed);

        // … ⟶ Allow pending handlers to get payload (or available cache)
        if (failed) {
          LoadFailed(ref load);
          if (!load.events.IsEmpty()) ReloadUri(ref load, requester, loader, recacher);
        }

        return null;
      }

      public static Unity.Collections.NativeArray<byte>.ReadOnly? Uri(System.Uri path, PatchOdyssey.Handler<PatchOdyssey.Events.LoadEvent>? callback = null, double timeout = Load.Asynchronously, bool cached = Load.WithCache, uint retries = Load.Persistently, PatchOdyssey.Handler<PatchOdyssey.Events.LoadEvent>? fallback = null) => Load.Uri<UnityEngine.Networking.DownloadHandler>(
        path,
        callback is null ? Load.Idle : ([PatchMethod(AggressiveInlining)] (object? target, in PatchOdyssey.Events.LoadEvent data) => callback(target, new() {callback = callback, data = (data.data.attempts, data.data.attemptsAllowed, data.data.cached, data.data.duration, data.data.path, ((UnityEngine.Networking.DownloadHandler) data.data.payload!).nativeData), epoch = data.epoch, metadata = data.metadata})),
        timeout,
        cached,
        retries,
        ([PatchMethod(AggressiveInlining)] static (path)    => UnityEngine.Networking.UnityWebRequest.Get(path)),
        ([PatchMethod(AggressiveInlining)] static (request) => request.downloadHandler),
        ([PatchMethod(AggressiveInlining)] static (cached)  => cached), // ⟶ `cached.AsCopy()`
        fallback is null ? Load.Idle : fallback,
        false
      )?.nativeData;

      // public static UnityEngine.AudioClip? LoadUriAsAudioClip(string path, System.Action<UnityEngine.AudioClip?>? callback = null, double? timeout = null, bool cached = Load.WithCache, UnityEngine.AudioType? encoding = null) {
      //   return Load.Uri(
      //     typeof(UnityEngine.AudioClip).ToString(), path, callback is null ? static _ => {} : _ => callback(_ as UnityEngine.AudioClip), timeout, cached,
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

      // public static string? LoadUriAsText(string path, System.Action<string?>? callback = null, double? timeout = null, bool cached = Load.WithCache, System.Text.Encoding? encoding = null) {
      //   return Load.Uri(
      //     typeof(string).ToString(), path, callback is null ? static _ => {} : _ => callback(_ as string), timeout, cached,
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

      // public static UnityEngine.Texture2D? LoadUriAsTexture2D(string path, System.Action<UnityEngine.Texture2D?>? callback = null, double? timeout = null, bool cached = Load.WithCache) {
      //   return Load.Uri(
      //     typeof(UnityEngine.Texture2D).ToString(), path, callback is null ? static _ => {} : _ => callback(_ as UnityEngine.Texture2D), timeout, cached,
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
    }

    public static class Reference<T> /* ⟶ Solely for `𝑓 Util.Reference<T>.*At(…)` */ {
      private readonly struct Sentinel {}

      /* … */
      private  static readonly PatchOdyssey.ArrayIndexer<T>        ArrayAt         = (PatchOdyssey.ArrayIndexer<T>) (Traits.IsValueType<T>() ? ((PatchOdyssey.ArrayIndexer<Reference<T>.Sentinel>) Reference<Reference<T>.Sentinel>.UnmanagedArrayAt).Method.GetGenericMethodDefinition().MakeGenericMethod(typeof(T)).CreateDelegate(typeof(PatchOdyssey.ArrayIndexer<T>)) : (PatchOdyssey.ArrayIndexer<T>) Reference<T>.ManagedArrayAt<T>); // ⟶ Damn it Unity, `ref System.Runtime.CompilerServices.Unsafe.Add(ref value, offset)` was perfectly fine
      internal const  uint                                         ManagedByteSize =  8u;                                                                                                                                                                                                                                                                                                                                                     // ⟶ Presumed byte size of managed/ reference types as structured within class types (i.e. `sizeof(void*)`) — relative liberal guess to avoid object splicing
      public   static ref T                                        Null            => ref Util.Reference<T>.Only(new T[] {default!});                                                                                                                                                                                                                                                                                                         // ⟶ Do not get reference to `System.ReadOnlySpan<T>.Empty`
      private  static readonly PatchOdyssey.ReadOnlySpanIndexer<T> ReadOnlySpanAt  = (PatchOdyssey.ReadOnlySpanIndexer<T>) (Traits.IsValueType<T>() ? ((PatchOdyssey.ReadOnlySpanIndexer<Reference<T>.Sentinel>) Reference<Reference<T>.Sentinel>.UnmanagedReadOnlySpanAt).Method.GetGenericMethodDefinition().MakeGenericMethod(typeof(T)).CreateDelegate(typeof(PatchOdyssey.ReadOnlySpanIndexer<T>)) : (PatchOdyssey.ReadOnlySpanIndexer<T>) Reference<T>.ManagedReadOnlySpanAt<T>);
      private  static readonly PatchOdyssey.SpanIndexer        <T> SpanAt          = (PatchOdyssey.SpanIndexer        <T>) (Traits.IsValueType<T>() ? ((PatchOdyssey.SpanIndexer        <Reference<T>.Sentinel>) Reference<Reference<T>.Sentinel>.UnmanagedSpanAt)        .Method.GetGenericMethodDefinition().MakeGenericMethod(typeof(T)).CreateDelegate(typeof(PatchOdyssey.SpanIndexer        <T>)) : (PatchOdyssey.SpanIndexer        <T>) Reference<T>.ManagedSpanAt        <T>);

      /* … */
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public static ref          T At(T[]                       array, uint index) => ref Reference<T>.ArrayAt       (array,   index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public static ref readonly T At(in System.ReadOnlySpan<T> span,  int  index) => ref Reference<T>.ReadOnlySpanAt(in span, index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public static ref          T At(in System.Span        <T> span,  int  index) => ref Reference<T>.SpanAt        (in span, index);

      [PatchMethod(AggressiveInlining), PatchResolution(1)] public static ref          T First(T[]                       array) => ref Reference<T>.Only(array);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public static ref readonly T First(in System.ReadOnlySpan<T> span)  => ref Reference<T>.Only(in span);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public static ref          T First(in System.Span        <T> span)  => ref Reference<T>.Only(in span);

      [PatchMethod(AggressiveInlining), PatchResolution(1)] public static ref          T Last(T[]                       array) => ref Reference<T>.At(array,   (uint) (!array.IsEmpty() ? array.Length - 1 : 0));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public static ref readonly T Last(in System.ReadOnlySpan<T> span)  => ref Reference<T>.At(in span, !span.IsEmpty            ? span .Length - 1 : 0);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public static ref          T Last(in System.Span        <T> span)  => ref Reference<T>.At(in span, !span.IsEmpty            ? span .Length - 1 : 0);

      [PatchMethod(AggressiveInlining)] internal static ref          U ManagedArrayAt       <U>(U[]                       array, uint index) => ref array[index];
      [PatchMethod(AggressiveInlining)] internal static ref readonly U ManagedReadOnlySpanAt<U>(in System.ReadOnlySpan<U> span,  int  index) => ref span [index];
      [PatchMethod(AggressiveInlining)] internal static ref          U ManagedSpanAt        <U>(in System.Span        <U> span,  int  index) => ref span [index];

      [PatchMethod(AggressiveInlining), PatchResolution(1)] public static ref          T Only(T[]                       array) => ref System.Runtime.InteropServices.MemoryMarshal.GetReference(new System.ReadOnlySpan<T>(array));
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public static ref readonly T Only(in System.ReadOnlySpan<T> span)  => ref System.Runtime.InteropServices.MemoryMarshal.GetReference(span);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public static ref          T Only(in System.Span        <T> span)  => ref System.Runtime.InteropServices.MemoryMarshal.GetReference(span);

      [PatchMethod(AggressiveInlining)] private unsafe static ref          U UnmanagedArrayAt       <U>(U[]                       array, uint index) where U : unmanaged { fixed (U* address = array) return ref address[index]; }
      [PatchMethod(AggressiveInlining)] private unsafe static ref readonly U UnmanagedReadOnlySpanAt<U>(in System.ReadOnlySpan<U> span,  int  index) where U : unmanaged { fixed (U* address = span)  return ref *(address + index); }
      [PatchMethod(AggressiveInlining)] private unsafe static ref          U UnmanagedSpanAt        <U>(in System.Span        <U> span,  int  index) where U : unmanaged { fixed (U* address = span)  return ref *(address + index); }
    }

    public static class Wait {
      internal /* readonly */ struct WaitForTimerIndex                             { public /* readonly */ uint value; }
      internal sealed         class  WaitMonoBehaviour : UnityEngine.MonoBehaviour {}

      /* … */
      internal static readonly PatchOdyssey.Collections.RefSortedCollection<double, PatchOdyssey.Collections.WaitInfo> Pending = new(16u) {{double.NaN, new()}};                                         // ⟶ Used `System.Collections.Generic.SortedDictionary<double, PatchOdyssey.Collections.WaitInfo>` prior
      internal static readonly Wait.WaitMonoBehaviour                                                                  Waiter  = new UnityEngine.GameObject("…").AddComponent<Wait.WaitMonoBehaviour>(); // ⟶ Use coroutines for asynchronicity

      /* … */
      [PatchMethod(AggressiveInlining)] public static uint Check         () => Wait.CheckCoroutine();
      [PatchMethod(AggressiveInlining)] public static uint CheckCoroutine() => 0u; // ⟶ Number of coroutines stopped

      public static uint CheckTimer() {
        uint                                                        count     = 0u;
        PatchOdyssey.Collections.SharedList<Wait.WaitForTimerIndex> resolved  = new();
        double                                                      timestamp = UnityEngine.Time.realtimeSinceStartupAsDouble;
        ref readonly PatchOdyssey.Collections.WaitInfo              wait      = ref Wait.Pending[0]; // ⟶ `Wait.Pending.GetValueByRank(double.NaN)`

        // … ⟶ Enumeration is messy because the final design could not succinctly account for a timer-based model
        for (uint index = resolved.IsEmpty() ? wait.events.CountInvocationList() : resolved[resolved.Count - 1u].value; 0u != index--; ) {
          ref PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Events.WaitEvent> waitHandler = ref wait.events[index];
          ref PatchOdyssey.Events.WaitEvent                                       waitEvent   = ref waitHandler.metadata;

          // …
          if (!(timestamp < waitEvent.data.timestamp)) {
            if (!waitEvent.data.repeating) resolved.Add(new() {value = index}); // ⟶ Remove `Wait.ForTimerUntil(…)` handlers, or
            else waitEvent.data.timestamp = timestamp + waitEvent.data.delay;   // ⟶ Update `Wait.ForTimerEvery(…)` handlers

            waitHandler.Invoke(); // ⟶ Assumes `CheckTimer()` is not simultaneously invoked here
          }
        }

        foreach (ref readonly Wait.WaitForTimerIndex index in resolved)
          wait.events.RemoveAt(index.value);

        resolved.Clear();

        return count;
      }

      [PatchMethod(AggressiveInlining)]
      public static void Every(double delay, PatchOdyssey.Handler<PatchOdyssey.Events.WaitEvent> callback) => Wait.ForCoroutineEvery(delay, callback);

      private static void ForCoroutine(double delay, PatchOdyssey.Handler<PatchOdyssey.Events.WaitEvent> callback, bool forever) {
        [PatchMethod(AggressiveInlining)]
        static System.Collections.IEnumerator EnumerateRoutine(double delay, bool forever) {
          do {
            yield return new UnityEngine.WaitForSecondsRealtime((float) delay);
            PatchOdyssey.Collections.WaitInfo wait = Wait.Pending.GetValueByRank(delay); // ⟶ Could be a reference local

            // …
            if (0u == wait.events.CountInvocationList())
            break;

            wait.events.Invoke();
          } while (forever);

          uint index = (uint) Wait.Pending.IndexOfRank(delay);

          Wait.Waiter .StopCoroutine(Wait.Pending[index].coroutine);
          Wait.Pending.RemoveAt     (index);
        }

        ref PatchOdyssey.Collections.WaitInfo wait = ref Wait.Pending.TryAppend(delay, new(Wait.Waiter.StartCoroutine(EnumerateRoutine(delay, forever))));
        wait.events += new PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Events.WaitEvent>(callback, wait.coroutine, new() {callback = callback, data = (delay, UnityEngine.Time.realtimeSinceStartupAsDouble + delay, forever)});
      }

      [PatchMethod(AggressiveInlining)] public  static void ForCoroutineEvery(double                                                                   delay,  PatchOdyssey.Handler<PatchOdyssey.Events.WaitEvent> callback)                                                            => Wait.ForCoroutine(delay, callback, true);
      [PatchMethod(AggressiveInlining)] public  static void ForCoroutineUntil(double                                                                   delay,  PatchOdyssey.Handler<PatchOdyssey.Events.WaitEvent> callback)                                                            => Wait.ForCoroutine(delay, callback, false);
      [PatchMethod(AggressiveInlining)] public  static void ForTimerEvery    (double                                                                   delay,  PatchOdyssey.Handler<PatchOdyssey.Events.WaitEvent> callback)                                                            => Wait.Pending[0].events += new PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Events.WaitEvent>(callback, null, new() {callback = callback, data = (delay, UnityEngine.Time.realtimeSinceStartupAsDouble + delay, true)});
      [PatchMethod(AggressiveInlining)] public  static void ForTimerUntil    (double                                                                   delay,  PatchOdyssey.Handler<PatchOdyssey.Events.WaitEvent> callback)                                                            => Wait.Pending[0].events += new PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Events.WaitEvent>(callback, null, new() {callback = callback, data = (delay, UnityEngine.Time.realtimeSinceStartupAsDouble + delay, false)});
      [PatchMethod(AggressiveInlining)] private static uint Stop             (ref PatchOdyssey.Collections.EventHandler<PatchOdyssey.Events.WaitEvent> events, PatchOdyssey.Handler<PatchOdyssey.Events.WaitEvent> callback)                                                            { uint count = 0u; for (uint index = events.CountInvocationList(); 0u != index--; ) if (callback == events[index].value)                                          { ++count; events.RemoveAt(index); } return count; }
      [PatchMethod(AggressiveInlining)] private static uint Stop             (ref PatchOdyssey.Collections.EventHandler<PatchOdyssey.Events.WaitEvent> events, double                                              delay, PatchOdyssey.Handler<PatchOdyssey.Events.WaitEvent> callback) { uint count = 0u; for (uint index = events.CountInvocationList(); 0u != index--; ) if (callback == events[index].value && delay == events[index].metadata.delay) { ++count; events.RemoveAt(index); } return count; }
      [PatchMethod(AggressiveInlining)] private static uint StopAll          (ref PatchOdyssey.Collections.EventHandler<PatchOdyssey.Events.WaitEvent> events)                                                                                                                          { uint count = events.CountInvocationList(); events.Clear();                                                                                                                                           return count; }
      [PatchMethod(AggressiveInlining)] public  static uint StopCoroutine    ()                                                                                                                                                                                                         { uint count = 0u; for (uint index = Wait.Pending.Count; 0u != --index; ) { count += Wait.StopAll(ref Wait.Pending[index].events); }           return count; }
      [PatchMethod(AggressiveInlining)] public  static uint StopCoroutine    (PatchOdyssey.Handler<PatchOdyssey.Events.WaitEvent> callback)                                                                                                                                             { uint count = 0u; for (uint index = Wait.Pending.Count; 0u != --index; ) { count += Wait.Stop   (ref Wait.Pending[index].events, callback); } return count; }
      [PatchMethod(AggressiveInlining)] public  static uint StopCoroutine    (double                                              delay, PatchOdyssey.Handler<PatchOdyssey.Events.WaitEvent> callback)                                                                                  { int index = Wait.Pending.IndexOfRank(delay); return index > 0 ? Wait.Stop(ref Wait.Pending[(uint) index].events, callback) : 0u; }
      [PatchMethod(AggressiveInlining)] public  static uint StopTimer        ()                                                                                                                                                                                                         => Wait.StopAll(ref Wait.Pending[0].events);
      [PatchMethod(AggressiveInlining)] public  static uint StopTimer        (PatchOdyssey.Handler<PatchOdyssey.Events.WaitEvent> callback)                                                                                                                                             => Wait.Stop(ref Wait.Pending[0].events, callback);
      [PatchMethod(AggressiveInlining)] public  static uint StopTimer        (double                                              delay, PatchOdyssey.Handler<PatchOdyssey.Events.WaitEvent> callback)                                                                                  => Wait.Stop(ref Wait.Pending[0].events, delay, callback);
      [PatchMethod(AggressiveInlining)] public  static void Until            (double                                              delay, PatchOdyssey.Handler<PatchOdyssey.Events.WaitEvent> callback)                                                                                  => Wait.ForCoroutineUntil(delay, callback);
    }

    /* … */
    private static readonly System.Collections.Generic    .Dictionary        <(System.Type, System.Type), System.Delegate>                                 Converters           = new(1);
    private static readonly System.Collections.ObjectModel.ReadOnlyDictionary<System.Type, System.Collections.ObjectModel.ReadOnlyCollection<System.Type>> ImplicitTypeConverts = new System.Collections.Generic.Dictionary<System.Type, System.Collections.ObjectModel.ReadOnlyCollection<System.Type>>() {
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
    private static readonly System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.KeyCode> Keys              = new[] {UnityEngine.KeyCode.A, UnityEngine.KeyCode.Alpha0, UnityEngine.KeyCode.Alpha1, UnityEngine.KeyCode.Alpha2, UnityEngine.KeyCode.Alpha3, UnityEngine.KeyCode.Alpha4, UnityEngine.KeyCode.Alpha5, UnityEngine.KeyCode.Alpha6, UnityEngine.KeyCode.Alpha7, UnityEngine.KeyCode.Alpha8, UnityEngine.KeyCode.Alpha9, UnityEngine.KeyCode.AltGr, UnityEngine.KeyCode.Ampersand, UnityEngine.KeyCode.Asterisk, UnityEngine.KeyCode.At, UnityEngine.KeyCode.B, UnityEngine.KeyCode.BackQuote, UnityEngine.KeyCode.Backslash, UnityEngine.KeyCode.Backspace, UnityEngine.KeyCode.Break, UnityEngine.KeyCode.C, UnityEngine.KeyCode.CapsLock, UnityEngine.KeyCode.Caret, UnityEngine.KeyCode.Clear, UnityEngine.KeyCode.Colon, UnityEngine.KeyCode.Comma, UnityEngine.KeyCode.D, UnityEngine.KeyCode.Delete, UnityEngine.KeyCode.Dollar, UnityEngine.KeyCode.DoubleQuote, UnityEngine.KeyCode.DownArrow, UnityEngine.KeyCode.E, UnityEngine.KeyCode.End, UnityEngine.KeyCode.Equals, UnityEngine.KeyCode.Escape, UnityEngine.KeyCode.Exclaim, UnityEngine.KeyCode.F, UnityEngine.KeyCode.F1, UnityEngine.KeyCode.F10, UnityEngine.KeyCode.F11, UnityEngine.KeyCode.F12, UnityEngine.KeyCode.F13, UnityEngine.KeyCode.F14, UnityEngine.KeyCode.F15, UnityEngine.KeyCode.F2, UnityEngine.KeyCode.F3, UnityEngine.KeyCode.F4, UnityEngine.KeyCode.F5, UnityEngine.KeyCode.F6, UnityEngine.KeyCode.F7, UnityEngine.KeyCode.F8, UnityEngine.KeyCode.F9, UnityEngine.KeyCode.G, UnityEngine.KeyCode.Greater, UnityEngine.KeyCode.H, UnityEngine.KeyCode.Hash, UnityEngine.KeyCode.Help, UnityEngine.KeyCode.Home, UnityEngine.KeyCode.I, UnityEngine.KeyCode.Insert, UnityEngine.KeyCode.J, UnityEngine.KeyCode.K, UnityEngine.KeyCode.Keypad0, UnityEngine.KeyCode.Keypad1, UnityEngine.KeyCode.Keypad2, UnityEngine.KeyCode.Keypad3, UnityEngine.KeyCode.Keypad4, UnityEngine.KeyCode.Keypad5, UnityEngine.KeyCode.Keypad6, UnityEngine.KeyCode.Keypad7, UnityEngine.KeyCode.Keypad8, UnityEngine.KeyCode.Keypad9, UnityEngine.KeyCode.KeypadDivide, UnityEngine.KeyCode.KeypadEnter, UnityEngine.KeyCode.KeypadEquals, UnityEngine.KeyCode.KeypadMinus, UnityEngine.KeyCode.KeypadMultiply, UnityEngine.KeyCode.KeypadPeriod, UnityEngine.KeyCode.KeypadPlus, UnityEngine.KeyCode.L, UnityEngine.KeyCode.LeftAlt, UnityEngine.KeyCode.LeftApple, UnityEngine.KeyCode.LeftArrow, UnityEngine.KeyCode.LeftBracket, UnityEngine.KeyCode.LeftCommand, UnityEngine.KeyCode.LeftControl, UnityEngine.KeyCode.LeftCurlyBracket, UnityEngine.KeyCode.LeftMeta, UnityEngine.KeyCode.LeftParen, UnityEngine.KeyCode.LeftShift, UnityEngine.KeyCode.LeftWindows, UnityEngine.KeyCode.Less, UnityEngine.KeyCode.M, UnityEngine.KeyCode.Menu, UnityEngine.KeyCode.Minus, UnityEngine.KeyCode.N, UnityEngine.KeyCode.Numlock, UnityEngine.KeyCode.O, UnityEngine.KeyCode.P, UnityEngine.KeyCode.PageDown, UnityEngine.KeyCode.PageUp, UnityEngine.KeyCode.Pause, UnityEngine.KeyCode.Percent, UnityEngine.KeyCode.Period, UnityEngine.KeyCode.Pipe, UnityEngine.KeyCode.Plus, UnityEngine.KeyCode.Print, UnityEngine.KeyCode.Q, UnityEngine.KeyCode.Question, UnityEngine.KeyCode.Quote, UnityEngine.KeyCode.R, UnityEngine.KeyCode.Return, UnityEngine.KeyCode.RightAlt, UnityEngine.KeyCode.RightApple, UnityEngine.KeyCode.RightArrow, UnityEngine.KeyCode.RightBracket, UnityEngine.KeyCode.RightCommand, UnityEngine.KeyCode.RightControl, UnityEngine.KeyCode.RightCurlyBracket, UnityEngine.KeyCode.RightMeta, UnityEngine.KeyCode.RightParen, UnityEngine.KeyCode.RightShift, UnityEngine.KeyCode.RightWindows, UnityEngine.KeyCode.S, UnityEngine.KeyCode.ScrollLock, UnityEngine.KeyCode.Semicolon, UnityEngine.KeyCode.Slash, UnityEngine.KeyCode.Space, UnityEngine.KeyCode.SysReq, UnityEngine.KeyCode.T, UnityEngine.KeyCode.Tab, UnityEngine.KeyCode.Tilde, UnityEngine.KeyCode.U, UnityEngine.KeyCode.Underscore, UnityEngine.KeyCode.UpArrow, UnityEngine.KeyCode.V, UnityEngine.KeyCode.W, UnityEngine.KeyCode.X, UnityEngine.KeyCode.Y, UnityEngine.KeyCode.Z}.AsReadOnly();
    public  const  int                                                                             MouseButtonLeft   = 0x0;
    public  const  int                                                                             MouseButtonMiddle = 0x2;
    public  const  int                                                                             MouseButtonRight  = 0x1;
    private static readonly System.Collections.ObjectModel.ReadOnlyCollection<int>                 MouseButtons      = new[] {Util.MouseButtonLeft, Util.MouseButtonRight, Util.MouseButtonMiddle}.AsReadOnly();

    /* … */
    public static UnityEngine.Vector3[] CornersFromRect(in UnityEngine.Rect rectangle) {
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

    [PatchMethod(AggressiveInlining)] public static uint EnumerableCount   (System.Collections.IEnumerable            enumerable) => Util.EnumeratorCount(enumerable.GetEnumerator(), false);
    [PatchMethod(AggressiveInlining)] public static uint EnumerableCount<T>(System.Collections.Generic.IEnumerable<T> enumerable) => Util.EnumeratorCount(enumerable.GetEnumerator(), false);

    [PatchMethod(AggressiveInlining)]
    public static uint EnumeratorCount(System.Collections.IEnumerator enumerator, bool preserve = true) {
      uint count = 0u;
      uint index = 0u;

      // …
      if (preserve) {
        while (enumerator.MoveNext())
        ++index;
      }

      for (enumerator.Reset(); enumerator.MoveNext(); )
      ++count;

      if (preserve) {
        for (enumerator.Reset(); count != index; ++index)
        enumerator.MoveNext(); // ⟶ Reset `enumerator` to initially passed state
      }

      return count;
    }

    [PatchMethod(AggressiveInlining)]
    public static uint EnumeratorCount<T>(System.Collections.Generic.IEnumerator<T> enumerator, bool preserve = true) {
      uint count = Util.EnumeratorCount((System.Collections.IEnumerator) enumerator, preserve);

      // …
      if (!preserve)
      enumerator.Dispose();

      return count;
    }

    [PatchMethod(AggressiveInlining)]
    public static T? EnumeratorMoveTo<T>(in T enumerator, uint position, bool preserve = true) where T : System.Collections.IEnumerator {
      uint count = 0u;
      uint index = 0u;

      // …
      if (preserve) {
        while (enumerator.MoveNext())
        ++index;
      }

      for (enumerator.Reset(); 0u != position--; ++count)
      if (!enumerator.MoveNext()) {
        if (preserve) {
          for (enumerator.Reset(); count != index; ++index)
          enumerator.MoveNext(); // ⟶ Reset `enumerator` to initially passed state
        }

        return default;
      }

      return enumerator;
    }

    public static UnityEngine.Vector2    ExcludeVectorAxes       (in UnityEngine.Vector2    vector, UnityEngine.Vector2    axes) { return new(axes.x != 1.0f ? vector.x : 0.0f, axes.y != 1.0f ? vector.y : 0.0f); }
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

    [PatchMethod(AggressiveInlining)]
    public static string GetAssetPath() => Util.NormalizeURI(UnityEngine.Application.streamingAssetsPath);

    public static PatchOdyssey.RefReadOnlyConverter<T, U> GetConverter<T, U>() {
      if (!Util.Converters.TryGetValue((typeof(T), typeof(U)), out System.Delegate converter)) {
        [PatchMethod(AggressiveInlining)]
        static PatchOdyssey.RefReadOnlyConverter<T, U> GetUnmanagedConverter() {
          System.Linq.Expressions.ParameterExpression parameter = System.Linq.Expressions.Expression.Parameter(typeof(T).MakeByRefType());
          return System.Linq.Expressions.Expression.Lambda<PatchOdyssey.RefReadOnlyConverter<T, U>>(System.Linq.Expressions.Expression.Convert(parameter, typeof(U)), parameter).Compile();
        }

        // …
        if (!typeof(U).IsAssignableFrom(typeof(T))) {
          System.Linq.Expressions.ParameterExpression parameter = System.Linq.Expressions.Expression.Parameter(typeof(T).MakeByRefType());

          // …
          converter = (
            typeof(U).GetConstructors(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public).Find([PatchMethod(AggressiveInlining)] static (constructor) => { System.Reflection.ParameterInfo[]            parameters = constructor.GetParameters();                                                      return (parameters.Length == 1 || (parameters.Length > 1 && Util.Reference<System.Reflection.ParameterInfo>.At(parameters, 1u).HasDefaultValue)) && (Util.Reference<System.Reflection.ParameterInfo>.Only(parameters).ParameterType.GetElementType() ?? Util.Reference<System.Reflection.ParameterInfo>.Only(parameters).ParameterType).IsAssignableFrom(typeof(T)); }) as System.Reflection.MethodBase ??
            typeof(U).GetMethods     (System.Reflection.BindingFlags.Public   | System.Reflection.BindingFlags.Static)                                           .Find([PatchMethod(AggressiveInlining)] static (method)      => { ref readonly System.Reflection.ParameterInfo parameter  = ref Util.Reference<System.Reflection.ParameterInfo>.Only(method.GetParameters()); return (method.Name == "op_Explicit" || method.Name == "op_Implicit")                                                                            && (parameter.ParameterType                                                       .GetElementType() ?? parameter                                                       .ParameterType).IsAssignableFrom(typeof(T)); }) as System.Reflection.MethodBase
          ) switch {
            System.Reflection.ConstructorInfo constructor => System.Linq.Expressions.Expression.Lambda<PatchOdyssey.RefReadOnlyConverter<T, U>>(System.Linq.Expressions.Expression.New (constructor, ((System.Func<System.Linq.Expressions.ParameterExpression, System.Reflection.ParameterInfo[], System.Linq.Expressions.Expression[]>) ([PatchMethod(AggressiveInlining)] static (parameter, parameters) => { System.Linq.Expressions.Expression[] expressions; Util.Reference<System.Reflection.ParameterInfo>.First(parameters) = Util.Reference<System.Reflection.ParameterInfo>.At(parameters, 1u); expressions = parameters.ConvertAll([PatchMethod(AggressiveInlining)] static (parameter) => (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant(parameter.DefaultValue, parameter.ParameterType)); Util.Reference<System.Linq.Expressions.Expression>.Only(expressions) = parameter; return expressions; }))(parameter, constructor.GetParameters())), parameter).Compile(),
            System.Reflection.MethodInfo      method      => System.Linq.Expressions.Expression.Lambda<PatchOdyssey.RefReadOnlyConverter<T, U>>(System.Linq.Expressions.Expression.Call(method,      parameter),                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               parameter).Compile(),
            _                                             => [PatchMethod(AggressiveInlining)] static (in T value) => {
              try { return (U) System.Convert.ChangeType(value, typeof(U), System.Globalization.CultureInfo.InvariantCulture); }
              catch (System.Exception exception) when (exception is System.InvalidCastException || exception is System.OverflowException) {}

              return ((PatchOdyssey.RefReadOnlyConverter<T, U>) (Util.Converters[(typeof(T), typeof(U))] = GetUnmanagedConverter()))(in value);
            } // ⟶ Implicitly casted by `switch` expression to `PatchOdyssey.RefReadOnlyConverter<T, U>` 😎
          };
        } else converter = GetUnmanagedConverter();

        Util.Converters.Add((typeof(T), typeof(U)), converter);
      }

      return (PatchOdyssey.RefReadOnlyConverter<T, U>) converter;
    }

    [PatchMethod(AggressiveInlining)] public static string                                                                              GetDataPath    () => Util.NormalizeURI(UnityEngine.Application.persistentDataPath);
    [PatchMethod(AggressiveInlining)] public static ref readonly System.Collections.ObjectModel.ReadOnlyCollection<int>                 GetMouseButtons() => ref Util.MouseButtons;
    [PatchMethod(AggressiveInlining)] public static ref readonly System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.KeyCode> GetKeys        () => ref Util.Keys;

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

    public static T Max<T>(System.Collections.Generic.IEnumerable<T> enumerable) where T : System.IComparable<T> {
      T[] maximum = null!;

      // …
      foreach (T value in enumerable) {
        if (maximum is null)                 maximum    = new[] {value};
        if (value.CompareTo(maximum[0]) > 0) maximum[0] = value;
      }

      return maximum![0]; // ⟶ `System.IndexOutOfRangeException`
    }
      public static T Max<T>(in T valueA, in T valueB) where T : System.IComparable<T> => valueA.CompareTo(valueB) > 0 ? valueA : valueB;
      public static T Max<T>(params T[] values)                                        => Util.Max(values);

    public static T Min<T>(System.Collections.Generic.IEnumerable<T> enumerable) where T : System.IComparable<T> {
      T[] minimum = null!;

      // …
      foreach (T value in enumerable) {
        if (minimum is null)                 minimum    = new[] {value};
        if (value.CompareTo(minimum[0]) < 0) minimum[0] = value;
      }

      return minimum![0]; // ⟶ `System.IndexOutOfRangeException`
    }
      public static T Min<T>(in T valueA, in T valueB) where T : System.IComparable<T> => valueA.CompareTo(valueB) < 0 ? valueA : valueB;
      public static T Min<T>(params T[] values)                                        => Util.Min(values);

    private static string NormalizeURI(string path) {
      path = path.TrimEnd().Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);

      // …
      while (0 != path.Length && (path.EndsWith(System.IO.Path.DirectorySeparatorChar) || System.String.IsNullOrWhiteSpace(path.Substring(path.Length - 1))))
        path = path.TrimEnd(System.IO.Path.DirectorySeparatorChar).TrimEnd();

      return path;
    }

    [PatchMethod(AggressiveInlining), PatchResolution(0)] public static decimal Perc(decimal percentage) => percentage / 100.0m;
    [PatchMethod(AggressiveInlining), PatchResolution(2)] public static double  Perc(double  percentage) => percentage / 100.0;
    [PatchMethod(AggressiveInlining), PatchResolution(1)] public static float   Perc(float   percentage) => percentage / 100.0f;

    [PatchMethod(AggressiveInlining), PatchResolution(0)] public static decimal PercOf(decimal value, decimal percentage) => System.Math.Min(value, (decimal) percentage) * (System.Math.Max(value, (decimal) percentage) / 100.0m);
    [PatchMethod(AggressiveInlining), PatchResolution(2)] public static decimal PercOf(decimal value, double  percentage) => System.Math.Min(value, (decimal) percentage) * (System.Math.Max(value, (decimal) percentage) / 100.0m);
    [PatchMethod(AggressiveInlining), PatchResolution(1)] public static decimal PercOf(decimal value, float   percentage) => System.Math.Min(value, (decimal) percentage) * (System.Math.Max(value, (decimal) percentage) / 100.0m);
    [PatchMethod(AggressiveInlining), PatchResolution(0)] public static double  PercOf(double  value, decimal percentage) => System.Math.Min(value, (double)  percentage) * (System.Math.Max(value, (double)  percentage) / 100.0);
    [PatchMethod(AggressiveInlining), PatchResolution(2)] public static double  PercOf(double  value, double  percentage) => System.Math.Min(value, (double)  percentage) * (System.Math.Max(value, (double)  percentage) / 100.0);
    [PatchMethod(AggressiveInlining), PatchResolution(1)] public static double  PercOf(double  value, float   percentage) => System.Math.Min(value, (double)  percentage) * (System.Math.Max(value, (double)  percentage) / 100.0);
    [PatchMethod(AggressiveInlining), PatchResolution(0)] public static float   PercOf(float   value, decimal percentage) => System.Math.Min(value, (float)   percentage) * (System.Math.Max(value, (float)   percentage) / 100.0f);
    [PatchMethod(AggressiveInlining), PatchResolution(2)] public static float   PercOf(float   value, double  percentage) => System.Math.Min(value, (float)   percentage) * (System.Math.Max(value, (float)   percentage) / 100.0f);
    [PatchMethod(AggressiveInlining), PatchResolution(1)] public static float   PercOf(float   value, float   percentage) => System.Math.Min(value, (float)   percentage) * (System.Math.Max(value, (float)   percentage) / 100.0f);

    // public static void PreloadURI           (string path)                                         => Load.Uri           (path, null, Util.Load.Asynchronously, Util.Load.WithCache);
    // public static void PreloadURIAsAudioClip(string path, UnityEngine.AudioType? encoding = null) => Load.UriAsAudioClip(path, null, Util.Load.Asynchronously, Util.Load.WithCache, encoding);
    // public static void PreloadURIAsText     (string path, System.Text.Encoding?  encoding = null) => Load.UriAsText     (path, null, Util.Load.Asynchronously, Util.Load.WithCache, encoding);
    // public static void PreloadURIAsTexture2D(string path)                                         => Load.UriAsTexture2D(path, null, Util.Load.Asynchronously, Util.Load.WithCache);

    [PatchMethod(AggressiveInlining)] // ⟶ Origin begins from bottom-left rather than top-left
    public static UnityEngine.Rect RectFromCorners(UnityEngine.Vector3[] corners) => new(Util.Reference<UnityEngine.Vector3>.At(corners, 0u).x, Util.Reference<UnityEngine.Vector3>.At(corners, 0u).y, Util.Reference<UnityEngine.Vector3>.At(corners, 3u).x - Util.Reference<UnityEngine.Vector3>.At(corners, 0u).x, Util.Reference<UnityEngine.Vector3>.At(corners, 1u).y - Util.Reference<UnityEngine.Vector3>.At(corners, 0u).y);

    [PatchMethod(AggressiveInlining)]
    public static ref U Reinterpret<T, U>(ref T value) => ref System.Runtime.CompilerServices.Unsafe.As<T, U>(ref value);


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

    [PatchMethod(AggressiveInlining)] public static object? Switch<T>(in T value, System.Collections.Generic.Dictionary         <T, object> expression, object? fallback = null) => Util.Switch<T>(value, (System.Collections.Generic.IReadOnlyDictionary<T, object>) expression, fallback);
    [PatchMethod(AggressiveInlining)] public static object? Switch<T>(in T value, System.Collections.Generic.IReadOnlyDictionary<T, object> expression, object? fallback = null) => expression?.TryGetValue(value, out object callback) ?? false ? callback : fallback;

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
    private static void Main() => new UnityEngine.GameObject("…", typeof(PatchBehaviour));
  }
}

internal sealed class PatchBehaviour : UnityEngine.MonoBehaviour {
  private void Awake      () {
    UnityEngine.Debug.Log($"Data requesting…");
    // UnityEngine.Debug.Log($"Data requested: {PatchOdyssey.Util.Load.Pending.Count}");
    UnityEngine.Debug.Log($"Data requested @ RefList<ValueType>: {new PatchOdyssey.Collections.RefList<(System.Type, System.Uri)>(16u).Count}");
    UnityEngine.Debug.Log($"Data requested @ RefList<LoadInfo>: {new PatchOdyssey.Collections.RefList<PatchOdyssey.Collections.LoadInfo>(16u).Count}");
    UnityEngine.Debug.Log($"Data requested @ RefEqualityComparer<ValueType>: {new PatchOdyssey.Collections.RefEqualityComparer<(System.Type, System.Uri)>()}");
    UnityEngine.Debug.Log($"Data requested @ RefEqualityComparer<LoadInfo>: {new PatchOdyssey.Collections.RefEqualityComparer<PatchOdyssey.Collections.LoadInfo>()}");
    UnityEngine.Debug.Log($"Data requested @ RefDictionary<string, int>: {new PatchOdyssey.Collections.RefDictionary<string, int>(16u).Count}");
    UnityEngine.Debug.Log($"Data requested @ RefDictionary<ValueType, LoadInfo>: {new PatchOdyssey.Collections.RefDictionary<(System.Type, System.Uri), PatchOdyssey.Collections.LoadInfo>(16u).Count}");

    // PatchOdyssey.Util.Load.Uri(new(new(PatchOdyssey.Util.GetAssetPath()), "Settings.xml"), (object? target, in PatchOdyssey.Events.LoadEvent data) => {
    //   UnityEngine.Debug.Log("Data recieved…");
    // }, PatchOdyssey.Util.Load.Asynchronously, PatchOdyssey.Util.Load.WithCache, PatchOdyssey.Util.Load.Persistently, (object? target, in PatchOdyssey.Events.LoadEvent data) => {
    //   UnityEngine.Debug.Log("Data error…");
    // });
  }

  private void FixedUpdate() {}
  private void OnDestroy  () {}
}
