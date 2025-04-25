global using Animation  = PatchOdyssey.Animation;                                                     //
global using PatchBurst = Unity.Burst.BurstCompile;                                                   //
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
global using PatchUnburst = Unity.Burst.BurstDiscard;                                                 // ⟶ `Unity.Burst.CompilerServices.IgnoreWarning(…)`?
global using static System.Runtime.CompilerServices.MethodImplOptions;                                // ⟶ Use case: `𝑓 PatchMethod(AggressiveOptimization, …)`
global using static System.Runtime.InteropServices.LayoutKind;                                        // ⟶ Use case: `𝑓 PatchLayout(Sequential, …)`

/* C# Polyfills */
namespace System.Runtime.Versioning {
  [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
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

#if !(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
  #pragma warning disable CA1066 // ⟶ Implement IEquatable when overriding `System.Object.Equals(…)`; Equality comparison of `System.HashCode` is disabled by design
  #pragma warning disable CA1815 // ⟶ Override equals and operator equals on value types; Equality comparison of `System.HashCode` is disabled by design
  #pragma warning disable CA2231 // ⟶ Overload operator equals on overriding value type `Equals(…)`; Equality comparison of `System.HashCode` is disabled by design
  #pragma warning disable SA1202 // ⟶ Elements should be ordered by access; Preserving original layout of members

  namespace System {
    // ⟶ `System.HashCode` @ `https://web.archive.org/web/20250115200832/https://learn.microsoft.com/en-us/dotnet/api/system.hashcode?view=net-9.0`
    public struct HashCode /* ⟶ Uses non-cryptographic “xxHash32” (https://web.archive.org/web/20150814055024/https://github.com/Cyan4973/xxHash) hash algorithm */ {
      private const           uint Prime1 = 2654435761u;
      private const           uint Prime2 = 2246822519u;
      private const           uint Prime3 = 3266489917u;
      private const           uint Prime4 = 668265263u;
      private const           uint Prime5 = 374761393u;
      private static readonly uint Seed   = HashCode.GenerateGlobalSeed();

      private uint length;
      private uint queueA;
      private uint queueB;
      private uint queueC;
      private uint valueA;
      private uint valueB;
      private uint valueC;
      private uint valueD;

      /* … */
      private void Add(int value) {
        uint length   = this.length++;
        uint position = length % 4;

        // … ⟶ `switch` statements can not be inlined
        if (position == 0u) { this.queueA = (uint) value; return; }
        if (position == 1u) { this.queueB = (uint) value; return; }
        if (position == 2u) { this.queueC = (uint) value; return; }

        /* if (position == 3u) */ {
          if (length == 3u)
            HashCode.Initialize(out this.valueA, out this.valueB, out this.valueC, out this.valueD);

          this.valueA = HashCode.Round(this.valueA, this.queueA);
          this.valueB = HashCode.Round(this.valueB, this.queueB);
          this.valueC = HashCode.Round(this.valueC, this.queueC);
          this.valueD = HashCode.Round(this.valueD, (uint) value);
        }
      }

      [PatchMethod(AggressiveInlining)] public void Add<T>(in T value)                                                            => this.Add(value?.GetHashCode() ?? 0);
      [PatchMethod(AggressiveInlining)] public void Add<T>(in T value, System.Collections.Generic.IEqualityComparer<T>? comparer) => this.Add(value is not null ? comparer?.GetHashCode(value) ?? value.GetHashCode() : 0);

      public void AddBytes(in System.ReadOnlySpan<byte> value) {
        ref byte position = ref System.Runtime.InteropServices.MemoryMarshal.GetReference(value);
        ref byte end      = ref System.Runtime.CompilerServices.Unsafe.Add               (ref position, value.Length);

        // Add four bytes at a time until the input has fewer than four bytes remaining.
        while ((nint) System.Runtime.CompilerServices.Unsafe.ByteOffset(ref position, ref end) >= sizeof(int)) {
          this.Add(System.Runtime.CompilerServices.Unsafe.ReadUnaligned<int>(ref position));
          position = ref System.Runtime.CompilerServices.Unsafe.Add(ref position, sizeof(int));
        }

        // Add the remaining bytes a single byte at a time.
        while (System.Runtime.CompilerServices.Unsafe.IsAddressLessThan(ref position, ref end)) {
          this.Add((int) position);
          position = ref System.Runtime.CompilerServices.Unsafe.Add(ref position, 1);
        }
      }

      [PatchMethod(AggressiveInlining)]
      public static int Combine<T1>(in T1 objectA) {
        uint hash  = HashCode.MixEmptyState() + 4u;
        uint hashA = (uint) (objectA?.GetHashCode() ?? 0);

        // …
        hash = HashCode.QueueRound(hash, hashA);

        return (int) HashCode.MixFinal(hash);
      }

      [PatchMethod(AggressiveInlining)]
      public static int Combine<T1, T2>(in T1 objectA, in T2 objectB) {
        uint hash                = HashCode.MixEmptyState() + 8u;
        (uint hashA, uint hashB) = ((uint) (objectA?.GetHashCode() ?? 0), (uint) (objectB?.GetHashCode() ?? 0));

        // …
        hash = HashCode.QueueRound(hash, hashA);
        hash = HashCode.QueueRound(hash, hashB);

        return (int) HashCode.MixFinal(hash);
      }

      [PatchMethod(AggressiveInlining)]
      public static int Combine<T1, T2, T3>(in T1 objectA, in T2 objectB, in T3 objectC) {
        uint hash                            = HashCode.MixEmptyState() + 12u;
        (uint hashA, uint hashB, uint hashC) = ((uint) (objectA?.GetHashCode() ?? 0), (uint) (objectB?.GetHashCode() ?? 0), (uint) (objectC?.GetHashCode() ?? 0));

        // …
        hash = HashCode.QueueRound(hash, hashA);
        hash = HashCode.QueueRound(hash, hashB);
        hash = HashCode.QueueRound(hash, hashC);

        return (int) HashCode.MixFinal(hash);
      }

      [PatchMethod(AggressiveInlining)]
      public static int Combine<T1, T2, T3, T4>(in T1 objectA, in T2 objectB, in T3 objectC, in T4 objectD) {
        uint hash;
        (uint hashA, uint hashB, uint hashC, uint hashD) = ((uint) (objectA?.GetHashCode() ?? 0), (uint) (objectB?.GetHashCode() ?? 0), (uint) (objectC?.GetHashCode() ?? 0), (uint) (objectD?.GetHashCode() ?? 0));

        // …
        HashCode.Initialize(out uint valueA, out uint valueB, out uint valueC, out uint valueD);

        (valueA, valueB, valueC, valueD) = (HashCode.Round(valueA, hashA), HashCode.Round(valueB, hashB), HashCode.Round(valueC, hashC), HashCode.Round(valueD, hashD));
        hash                             = HashCode.MixState(valueA, valueB, valueC, valueD) + 16u;

        return (int) HashCode.MixFinal(hash);
      }

      [PatchMethod(AggressiveInlining)]
      public static int Combine<T1, T2, T3, T4, T5>(in T1 objectA, in T2 objectB, in T3 objectC, in T4 objectD, in T5 objectE) {
        uint hash;
        (uint hashA, uint hashB, uint hashC, uint hashD, uint hashE) = ((uint) (objectA?.GetHashCode() ?? 0), (uint) (objectB?.GetHashCode() ?? 0), (uint) (objectC?.GetHashCode() ?? 0), (uint) (objectD?.GetHashCode() ?? 0), (uint) (objectE?.GetHashCode() ?? 0));

        // …
        HashCode.Initialize(out uint valueA, out uint valueB, out uint valueC, out uint valueD);

        (valueA, valueB, valueC, valueD) = (HashCode.Round(valueA, hashA), HashCode.Round(valueB, hashB), HashCode.Round(valueC, hashC), HashCode.Round(valueD, hashD));
        hash                             = HashCode.MixState  (valueA, valueB, valueC, valueD) + 20u;
        hash                             = HashCode.QueueRound(hash, hashE);

        return (int) HashCode.MixFinal(hash);
      }

      [PatchMethod(AggressiveInlining)]
      public static int Combine<T1, T2, T3, T4, T5, T6>(in T1 objectA, in T2 objectB, in T3 objectC, in T4 objectD, in T5 objectE, in T6 objectF) {
        uint hash;
        (uint hashA, uint hashB, uint hashC, uint hashD, uint hashE, uint hashF) = ((uint) (objectA?.GetHashCode() ?? 0), (uint) (objectB?.GetHashCode() ?? 0), (uint) (objectC?.GetHashCode() ?? 0), (uint) (objectD?.GetHashCode() ?? 0), (uint) (objectE?.GetHashCode() ?? 0), (uint) (objectF?.GetHashCode() ?? 0));

        // …
        HashCode.Initialize(out uint valueA, out uint valueB, out uint valueC, out uint valueD);

        (valueA, valueB, valueC, valueD) = (HashCode.Round(valueA, hashA), HashCode.Round(valueB, hashB), HashCode.Round(valueC, hashC), HashCode.Round(valueD, hashD));
        hash                             = HashCode.MixState  (valueA, valueB, valueC, valueD) + 24u;
        hash                             = HashCode.QueueRound(hash, hashE);
        hash                             = HashCode.QueueRound(hash, hashF);

        return (int) HashCode.MixFinal(hash);
      }

      [PatchMethod(AggressiveInlining)]
      public static int Combine<T1, T2, T3, T4, T5, T6, T7>(in T1 objectA, in T2 objectB, in T3 objectC, in T4 objectD, in T5 objectE, in T6 objectF, in T7 objectG) {
        uint hash;
        (uint hashA, uint hashB, uint hashC, uint hashD, uint hashE, uint hashF, uint hashG) = ((uint) (objectA?.GetHashCode() ?? 0), (uint) (objectB?.GetHashCode() ?? 0), (uint) (objectC?.GetHashCode() ?? 0), (uint) (objectD?.GetHashCode() ?? 0), (uint) (objectE?.GetHashCode() ?? 0), (uint) (objectF?.GetHashCode() ?? 0), (uint) (objectG?.GetHashCode() ?? 0));

        // …
        HashCode.Initialize(out uint valueA, out uint valueB, out uint valueC, out uint valueD);

        (valueA, valueB, valueC, valueD) = (HashCode.Round(valueA, hashA), HashCode.Round(valueB, hashB), HashCode.Round(valueC, hashC), HashCode.Round(valueD, hashD));
        hash                             = HashCode.MixState  (valueA, valueB, valueC, valueD) + 28u;
        hash                             = HashCode.QueueRound(hash, hashE);
        hash                             = HashCode.QueueRound(hash, hashF);
        hash                             = HashCode.QueueRound(hash, hashG);

        return (int) HashCode.MixFinal(hash);
      }

      [PatchMethod(AggressiveInlining)]
      public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(in T1 objectA, in T2 objectB, in T3 objectC, in T4 objectD, in T5 objectE, in T6 objectF, in T7 objectG, in T8 objectH) {
        uint hash;
        (uint hashA, uint hashB, uint hashC, uint hashD, uint hashE, uint hashF, uint hashG, uint hashH) = ((uint) (objectA?.GetHashCode() ?? 0), (uint) (objectB?.GetHashCode() ?? 0), (uint) (objectC?.GetHashCode() ?? 0), (uint) (objectD?.GetHashCode() ?? 0), (uint) (objectE?.GetHashCode() ?? 0), (uint) (objectF?.GetHashCode() ?? 0), (uint) (objectG?.GetHashCode() ?? 0), (uint) (objectH?.GetHashCode() ?? 0));

        // …
        HashCode.Initialize(out uint valueA, out uint valueB, out uint valueC, out uint valueD);

        (valueA, valueB, valueC, valueD) = (HashCode.Round(valueA, hashA), HashCode.Round(valueB, hashB), HashCode.Round(valueC, hashC), HashCode.Round(valueD, hashD));
        (valueA, valueB, valueC, valueD) = (HashCode.Round(valueA, hashE), HashCode.Round(valueB, hashF), HashCode.Round(valueC, hashG), HashCode.Round(valueD, hashH));
        hash                             = HashCode.MixState(valueA, valueB, valueC, valueD) + 32u;

        return (int) HashCode.MixFinal(hash);
      }

      [PatchMethod(AggressiveInlining)]
      private static void Initialize(out uint valueA, out uint valueB, out uint valueC, out uint valueD) {
        valueA = HashCode.Seed + HashCode.Prime1 + HashCode.Prime2;
        valueB = HashCode.Seed + HashCode.Prime2;
        valueC = HashCode.Seed;
        valueD = HashCode.Seed - HashCode.Prime1;
      }

      [PatchMethod(AggressiveInlining)] private static uint MixEmptyState()                                                   => HashCode.Prime5 + HashCode.Seed;
      [PatchMethod(AggressiveInlining)] private static uint MixFinal     (uint hash)                                          { hash ^= hash >> 15; hash *= HashCode.Prime2; hash ^= hash >> 13; hash *= HashCode.Prime3; hash ^= hash >> 16; return hash; }
      [PatchMethod(AggressiveInlining)] private static uint MixState     (uint valueA, uint valueB, uint valueC, uint valueD) => HashCode.RotateLeft(valueA, 1) + HashCode.RotateLeft(valueB, 7) + HashCode.RotateLeft(valueC, 12) + HashCode.RotateLeft(valueD, 18);
      [PatchMethod(AggressiveInlining)] private static uint QueueRound   (uint hash,   uint queuedValue)                      => HashCode.Prime4 * HashCode.RotateLeft(hash + (HashCode.Prime3 * queuedValue), 17);
      [PatchMethod(AggressiveInlining)] private static uint RotateLeft   (uint value,  int  offset)                           => (value << offset) | (value >> (32 - offset));
      [PatchMethod(AggressiveInlining)] private static uint Round        (uint hash,   uint input)                            => HashCode.Prime1 * HashCode.RotateLeft(hash + (HashCode.Prime2 * input),       13);

      public int ToHashCode() {
        uint hash     = this.length < 4u ? HashCode.MixEmptyState() : HashCode.MixState(this.valueA, this.valueB, this.valueC, this.valueD);
        uint position = this.length % 4u;

        // …
        hash += this.length * 4u;

        if (position > 0u) { hash = HashCode.QueueRound(hash, this.queueA);
        if (position > 1u) { hash = HashCode.QueueRound(hash, this.queueB);
        if (position > 2u) { hash = HashCode.QueueRound(hash, this.queueC);
        } } }

        return (int) HashCode.MixFinal(hash);
      }

      #pragma warning disable CS0809 // ⟶ Obsolete member overrides non-obsolete member; Disallowing `HashCode::Equals(…)` and `HashCode::GetHashCode()` is by design
      #pragma warning disable CA1065 // ⟶ Do not raise exceptions in unexpected locations — also by design
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Obsolete("`HashCode` is a mutable `struct` and should not be compared with other `HashCode`s.", error: true)]
        public override bool Equals(object? _) => throw new System.NotSupportedException("`HashCode` is a mutable `struct` and should not be compared with other `HashCode`s.");

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Obsolete("`HashCode` is a mutable `struct` and should not be compared with other `HashCode`s. Use `HashCode.ToHashCode()` to retrieve the computed hash code.", error: true)]
        public override int GetHashCode() => throw new System.NotSupportedException("`HashCode` is a mutable `struct` and should not be compared with other `HashCode`s. Use `HashCode.ToHashCode()` to retrieve the computed hash code.");
      #pragma warning restore CA1065
      #pragma warning restore CS0809

      #pragma warning disable CA5394
        // ⟶ The .NET runtime uses `Interop.GetRandomBytes(…)`, but this suffices for a polyfill
        // ⟶ Avoid insecure randomness; Use a different value at each program execution, no cryptography involved
        private static uint GenerateGlobalSeed() => (uint) new System.Random().Next(int.MinValue, int.MaxValue);
      #pragma warning restore CA5394
    }
  }
#endif

#if NET46X || NET47X || NET48X || NETCOREAPP2X || NETSTANDARD2_0
  namespace System {
    // ⟶ `System.Index` @ `https://web.archive.org/web/20241129171203/https://learn.microsoft.com/en-us/dotnet/api/system.index?view=net-9.0`
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.Diagnostics.DebuggerNonUserCode]
    public readonly struct Index : System.IEquatable<Index> {
      public  static   Index Start     => new(0);
      public  static   Index End       => new(~0);
      public           bool  IsFromEnd => this.value < 0;
      public           int   Value     => this.value < 0 ? ~this.value : this.value;
      private readonly int   value;

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
        public  System.Collections.Generic.KeyValuePair<string, object?> Current { get { System.Collections.DictionaryEntry element = this.Current; return new(element.Key, element.Value); } }
        private System.Collections.IDictionaryEnumerator                 enumerator;
        System.Collections.Generic.KeyValuePair<string, object?>         System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, object?>>.Current => this           .Current;
        System.Collections.DictionaryEntry                               System.Collections.IDictionaryEnumerator.Entry                                                           => this.enumerator.Current;
        object                                                           System.Collections.IDictionaryEnumerator.Key                                                             => this.enumerator.Current.Key;
        object?                                                          System.Collections.IDictionaryEnumerator.Value                                                           => this.enumerator.Current.Value;
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
        [PatchMethod(AggressiveInlining)] UIKeyframe.KeyEnumerator                       GetEnumerator                                               () => new(keyframe);
        [PatchMethod(AggressiveInlining)] System.Collections.Generic.IEnumerator<string> System.Collections.Generic.IEnumerable<string>.GetEnumerator() => this.GetEnumerator();
        [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator                 System.Collections.IEnumerable.GetEnumerator                () => this.GetEnumerator();
      }

      public struct KeyEnumerator : System.Collections.Generic.IEnumerator<string> {
        public  string                                   Current => (string) this.enumerator.Current.Key;
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
        [PatchMethod(AggressiveInlining)] UIKeyframe.ValueEnumerator                     GetEnumerator                                               () => new(keyframe);
        [PatchMethod(AggressiveInlining)] System.Collections.Generic.IEnumerator<string> System.Collections.Generic.IEnumerable<string>.GetEnumerator() => this.GetEnumerator();
        [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator                 System.Collections.IEnumerable.GetEnumerator                () => this.GetEnumerator();
      }

      public struct ValueEnumerator : System.Collections.Generic.IEnumerator<object?> {
        public  object?                                  Current => this.enumerator.Current.Value;
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
      public            uint                                           Count                                                                                                          => base.Count;
      internal readonly System.Collections.Specialized.ListDictionary? begin                                                                                                          =  null;
      internal          System.Collections.Specialized.ListDictionary? end                                                                                                            { get => this.properties; private init {} };
      public   readonly string?                                        name                                                                                                           =  null; // ⟶ Explicitly not `string.Empty`
      public            System.Collections.Specialized.ListDictionary  properties                                                                                                     => (System.Collections.Specialized.ListDictionary) this;
      int                                                              System.Collections.Generic.IReadOnlyCollection<System.Collections.Generic.KeyValuePair<string, object?>>.Count => base.Count;
      System.Collections.Generic.IEnumerable<string>                   System.Collections.Generic.IReadOnlyDictionary<string, object?>.Keys                                           => new UIKeyframe.KeyEnumerable  (this);
      System.Collections.Generic.IEnumerable<object?>                  System.Collections.Generic.IReadOnlyDictionary<string, object?>.Values                                         => new UIKeyframe.ValueEnumerable(this);
      int                                                              System.Collections.ICollection.Count                                                                           => base.Count;
      bool                                                             System.Collections.ICollection.IsSynchronized                                                                  => false;
      object                                                           System.Collections.ICollection.SyncRoot                                                                        => this;
      bool                                                             System.Collections.IDictionary.IsFixedSize                                                                     => true;
      bool                                                             System.Collections.IDictionary.IsReadOnly                                                                      => true;
      System.Collections.ICollection                                   System.Collections.IDictionary.Keys                                                                            => base.Keys;
      System.Collections.ICollection                                   System.Collections.IDictionary.Values                                                                          => base.Values;

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)]
      public UIKeyframe(string name, System.Collections.Generic.IReadOnlyDictionary<string, object?> properties) {
        this.name = name;

        foreach (System.Collections.Generic.KeyValuePair<TKey, TValue> property in properties)
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
      object?        System.Collections.Generic.IReadOnlyDictionary<string, object?>.this[string key] { get; }
      object?        System.Collections.IDictionary.this                                 [object key] { get => base[key]; set => base[key] = value; }
    }

    public class UISequence : PatchOdyssey.Animation.UIKeyframe, System.Collections.IDictionary, System.Collections.Generic.IReadOnlyDictionary<string, object?> {
      private static   System.Collections.Generic.Dictionary<System.Type, (System.Reflection.MethodInfo multiplication, System.Reflection.MethodInfo subtraction)> InterpolationOperators                                                                                         =  new(3);
      public  required double                                                                                                                                      delay                                                                                                          =  0.0;
      public  required double                                                                                                                                      duration                                                                                                       =  0.0;
      public  required PatchOdyssey.Tweener                                                                                                                        easing                                                                                                         =  PatchOdyssey.Animation.Function  .Linear;
      public  required PatchOdyssey.Interpolator                                                                                                                   interpolator                                                                                                   =  PatchOdyssey.Animation.UISequence.Interpolate;
      public           bool                                                                                                                                        isDone                                                                                                         => UnityEngine.Time.realtimeSinceStartupAsDouble >= this.delay + this.duration + this.timestamp;
      private readonly System.Collections.Generic.SortedList<double, PatchOdyssey.Animation.UIKeyframe>                                                            keyframes                                                                                                      =  new(1);
      private          double                                                                                                                                      timestamp                                                                                                      =  0.0;
      int                                                                                                                                                          System.Collections.Generic.IReadOnlyCollection<System.Collections.Generic.KeyValuePair<string, object?>>.Count => base.Count;
      System.Collections.Generic.IEnumerable<string>                                                                                                               System.Collections.Generic.IReadOnlyDictionary<string, object?>.Keys                                           => new PatchOdyssey.Animation.UIKeyframe.KeyEnumerable(this);
      System.Collections.Generic.IEnumerable<object?>                                                                                                              System.Collections.Generic.IReadOnlyDictionary<string, object?>.Values                                         => ???;
      int                                                                                                                                                          System.Collections.ICollection.Count                                                                           => base.Count;
      bool                                                                                                                                                         System.Collections.ICollection.IsSynchronized                                                                  => false;
      object                                                                                                                                                       System.Collections.ICollection.SyncRoot                                                                        => this;
      bool                                                                                                                                                         System.Collections.IDictionary.IsFixedSize                                                                     => false;
      bool                                                                                                                                                         System.Collections.IDictionary.IsReadOnly                                                                      => false;
      System.Collections.ICollection                                                                                                                               System.Collections.IDictionary.Keys                                                                            => base.Keys;
      System.Collections.ICollection                                                                                                                               System.Collections.IDictionary.Values                                                                          => ???;

      /* TODO */
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
      [PatchConstructor, PatchMethod(AggressiveInlining)]
      public UISequence(string name, double duration, double delay, PatchOdyssey.Tweener easing, PatchOdyssey.Interpolator interpolator, System.Collections.Generic.IReadOnlyDictionary<string, object?> begin, System.Collections.Generic.IReadOnlyDictionary<string, object?> end) : base(name, begin, end) {
        this.delay        = delay;
        this.duration     = duration;
        this.easing       = easing       ?? this.easing;
        this.interpolator = interpolator ?? this.interpolator;
        this.timestamp    = UnityEngine.Time.realtimeSinceStartupAsDouble;
      }

      /* … ⟶ `𝑓 Add([optional] keyframe, progress, properties) { … }` */
      [PatchMethod(AggressiveInlining)] public void Add             (double progress, System.Collections.Generic.Dictionary         <string, object?> properties) => this.Add($"#{this.keyframes.Count + 1}", progress, properties);
      [PatchMethod(AggressiveInlining)] public void Add             (double progress, System.Collections.Generic.IReadOnlyDictionary<string, object?> properties) => this.Add($"#{this.keyframes.Count + 1}", progress, properties);
      [PatchMethod(AggressiveInlining)] public void Add(string name, double progress, System.Collections.Generic.Dictionary         <string, object?> properties) => this.Add(name,                           progress, (System.Collections.Generic.IReadOnlyDictionary<string, object?>) properties);
      [PatchMethod(AggressiveInlining)]
      public void Add(string name, double progress, System.Collections.Generic.IReadOnlyDictionary<string, object?> properties) {
        foreach (System.Collections.Generic.KeyValuePair<double, PatchOdyssey.Animation.UIKeyframe> enumerated in this.keyframes) {
          if (enumerated.Key == progress || (name is not null && enumerated.Value.name == name))
          return;
        }

        this.keyframes.Add(progress, new(name, properties)); // ⟶ `System.Collections.Generic.SortedList<…>` keeps `::keyframes` sorted
      }

      [PatchMethod(AggressiveInlining)]
      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
        return ???;
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
        if (!InterpolationOperators.TryGetValue(type, out operators) && (operators.subtraction = type.GetMethod("op_Subtraction", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, new[] {type, type}, null)) is not null)
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
            InterpolationOperators.Add(type, operators);

            break;
          }
        }

        return operators.multiplication?.Invoke(null, new[] {operators.subtraction?.Invoke(null, new[] {b, a}), System.Convert.ChangeType(progress, progressType)});
      }

      [PatchMethod(AggressiveInlining)]
      public void Reset() {
        this.timestamp = UnityEngine.Time.realtimeSinceStartupAsDouble;
      }

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
      object?        System.Collections.Generic.IReadOnlyDictionary<string, object?>.this[string key] { get; }
      object?        System.Collections.IDictionary.this                                 [object key] { get => base[key]; set => base[key] = value; }

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
    public interface Event : /* ⟶ Based on `System.EventArgs`; See `https://web.archive.org/web/20250117180031/https://learn.microsoft.com/en-us/dotnet/api/system.threading.manualresetevent?view=net-9.0` (or `https://web.archive.org/web/20130916192216/https://developer.mozilla.org/en-US/docs/Web/API/Event`) for naming scheme used */ {
      System.Delegate callback { get; internal set; };
      object?         data     { get; internal set; };
      double          epoch    { get; internal set; };
      object?         metadata { get;          set; };
    }
      public struct LoadEvent : PatchOdyssey.Collections.Event {
        public static readonly LoadEvent Default = new() {data = (0u, 1u, false, 0.0, new(string.Empty, System.UriKind.Relative), null)};

        public PatchOdyssey.Handler<PatchOdyssey.Collections.LoadEvent>                                              callback { get; internal set; }         =  [PatchMethod(AggressiveInlining)] static (_, _) => {}; //
        public (uint attempts, uint attemptsAllowed, bool cached, double duration, System.Uri path, object? payload) data     { get; internal set; }         =  LoadEvent.Default.data;                                //
        public uint                                                                                                  attempts                                => this.data.attempts;                                    // ⟶ Track persistent `LoadUri*(…)` `retries`
        public uint                                                                                                  attemptsAllowed                         => this.data.attemptsAllowed;                             //    ^^
        public uint                                                                                                  cached                                  => this.data.cached;                                      //
        public double                                                                                                duration                                => this.data.duration;                                    // ⟶ Time since load till payload
        public double                                                                                                epoch    { get; internal set; }         =  UnityEngine.Time.realtimeSinceStartupAsDouble;         //
        public object?                                                                                               metadata { get;          set; }         =  null;                                                  //
        public System.Uri                                                                                            path                                    => this.data.path;                                        //
        public object?                                                                                               payload                                 => this.data.payload;                                     //
        System.Delegate                                                                                              PatchOdyssey.Collections.Event.callback { get => this.callback; internal set => this.callback = (PatchOdyssey.Handler<PatchOdyssey.Collections.LoadEvent>) value; }
        object?                                                                                                      PatchOdyssey.Collections.Event.data     { get => this.data;     internal set => this.data     = value; }
        double                                                                                                       PatchOdyssey.Collections.Event.epoch    { get => this.epoch;    internal set => this.epoch    = value; }
        object?                                                                                                      PatchOdyssey.Collections.Event.metadata { get => this.metadata;          set => this.metadata = value; }

        /* … */
        [PatchConstructor, PatchMethod(AggressiveInlining)]
        public LoadEvent() {}
      }

      public struct WaitEvent : PatchOdyssey.Collections.Event {
        public static readonly WaitEvent Default = new() {data = (0.0, 0.0)};

        public PatchOdyssey.Handler<PatchOdyssey.Collections.WaitEvent> callback { get; internal set; }         =  [PatchMethod(AggressiveInlining)] static (_, _) => {}; //
        public (double delay, double timestamp)                         data     { get; internal set; }         =  WaitEvent.Default.data;                                //
        public double                                                   delay                                   => this.data.delay;                                       // ⟶ Specified delay
        public double                                                   epoch    { get; internal set; }         =  UnityEngine.Time.realtimeSinceStartupAsDouble;         //
        public object?                                                  metadata { get;          set; }         =  null;                                                  //
        public double                                                   timestamp                               => this.data.timestamp;                                   // ⟶ Next available timestamp to signal a `WaitForTimer` event (which could be in the past chronologically)
        System.Delegate                                                 PatchOdyssey.Collections.Event.callback { get => this.callback; internal set => this.callback = (PatchOdyssey.Handler<PatchOdyssey.Collections.WaitEvent>) value; }
        object?                                                         PatchOdyssey.Collections.Event.data     { get => this.data;     internal set => this.data     = value; }
        double                                                          PatchOdyssey.Collections.Event.epoch    { get => this.epoch;    internal set => this.epoch    = value; }
        object?                                                         PatchOdyssey.Collections.Event.metadata { get => this.metadata;          set => this.metadata = value; }

        /* … */
        [PatchConstructor, PatchMethod(AggressiveInlining)]
        public WaitEvent() {}
      }

    public sealed class EventHandler<T> : PatchOdyssey.Collections.RefList<PatchOdyssey.Collections.HandlerInfo<T>>, System.ICloneable where T : PatchOdyssey.Collections.Event, new() /* ⟶ `event` @ `https://web.archive.org/web/20220923174214/https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/event` */ {
      [PatchConstructor, PatchMethod(AggressiveInlining)] public EventHandler()                       : base(1u)                                                                                 {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public EventHandler(EventHandler<T> events) : base((PatchOdyssey.Collections.RefList<PatchOdyssey.Collections.HandlerInfo<T>>) events) {}

      /* … ⟶ Keep `𝑓 Add(…)`, `𝑓 AddRange(…)`, `𝑓 Append(…)`, `𝑓 Clear(…)`, `𝑓 Insert(…)`, `𝑓 InsertRange(…)`, `𝑓 Prepend(…)`, `𝑓 Remove(…)`, `𝑓 RemoveAll(…)`, `𝑓 RemoveAt(…)`, `𝑓 RemoveRange(…)` accessible for multicast queuing, privately inherit `class System.Collections.Generic.List` methods */
      [PatchMethod(AggressiveInlining)] public   new              void                                                                                 Add                    (in PatchOdyssey.Collections.HandlerInfo<T>                                      handler)                                                                                             { if (PatchOdyssey.Collections.HandlerInfo<T>.DefaultValue != handler.value) base.Add(in handler); }
      [PatchMethod(AggressiveInlining)] public   new              void                                                                                 AddRange               (System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.HandlerInfo<T>> handlers)                                                                                            { foreach (PatchOdyssey.Collections.HandlerInfo<T> handler in handlers)      base.Add(in handler); }
      [PatchMethod(AggressiveInlining)] public   new ref readonly PatchOdyssey.Collections.HandlerInfo<T>                                              Append                 (in PatchOdyssey.Collections.HandlerInfo<T>                                      handler)                                                                                             => ref base.Append(in handler);
      [PatchMethod(AggressiveInlining)] private  new              EventHandler<T>                                                                      AsCopy                 ()                                                                                                                                                                                    => new(this);
      [PatchMethod(AggressiveInlining)] private  new              EventHandler<T>                                                                      AsReadOnly             ()                                                                                                                                                                                    => this;
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  BinarySearch           (in PatchOdyssey.Collections.HandlerInfo<T> handler)                                                                                                                                  => base.BinarySearch(in handler);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  BinarySearch           (in PatchOdyssey.Collections.HandlerInfo<T> handler,                                                                System.Collections.Generic.IComparer<T>? comparer)                => base.BinarySearch(in handler, comparer);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  BinarySearch           (uint                                       index, uint length, in PatchOdyssey.Collections.HandlerInfo<T> handler, System.Collections.Generic.IComparer<T>? comparer)                => base.BinarySearch(index, length, in handler, comparer);
      [PatchMethod(AggressiveInlining)] public   new              void                                                                                 Clear                  ()                                                                                                                                                                                    => base.Clear       ();
      [PatchMethod(AggressiveInlining)] public   /* virtual */    object                                                                               Clone                  ()                                                                                                                                                                                    => base.Clone       ();
      [PatchMethod(AggressiveInlining)] public   static           void                                                                                 Combine                (ref EventHandler                       <T>                                 events, in PatchOdyssey.Collections.HandlerInfo<T> handler)                                               { if (PatchOdyssey.Collections.HandlerInfo<T>.DefaultValue != handler.value) events.Add(in handler); } // ⟶ Based on `𝑓 System.Delegate.Combine(…)`
      [PatchMethod(AggressiveInlining)] private  new              bool                                                                                 Contains               (in PatchOdyssey.Collections.HandlerInfo<T>                                 handler)                                                                                                  =>     base.Contains  (in handler);
      [PatchMethod(AggressiveInlining)] private  new              EventHandler<U>                                                                      ConvertAll<U>          (PatchOdyssey.RefConverter              <T, U>                              converter) where U : PatchOdyssey.Collections.Event, new()                                                => new(base.ConvertAll(converter));
      [PatchMethod(AggressiveInlining)] private  new              EventHandler<U>                                                                      ConvertAll<U>          (PatchOdyssey.RefReadOnlyConverter      <T, U>                              converter) where U : PatchOdyssey.Collections.Event, new()                                                => new(base.ConvertAll(converter));
      [PatchMethod(AggressiveInlining)] private  new              EventHandler<U>                                                                      ConvertAll<U>          (System.Converter                       <T, U>                              converter) where U : PatchOdyssey.Collections.Event, new()                                                => new(base.ConvertAll(converter));
      [PatchMethod(AggressiveInlining)] private  new              void                                                                                 CopyTo                 (PatchOdyssey.Collections.HandlerInfo<T>[]                                  array)                                                                                                    =>     base.CopyTo    (array);
      [PatchMethod(AggressiveInlining)] private  new              void                                                                                 CopyTo                 (PatchOdyssey.Collections.HandlerInfo<T>[]                                  array, uint                                      index)                                                   =>     base.CopyTo    (array, index);
      [PatchMethod(AggressiveInlining)] private  new              void                                                                                 CopyTo                 (uint                                                                       index, PatchOdyssey.Collections.HandlerInfo<T>[] array, uint arrayIndex, uint length)                     =>     base.CopyTo    (index, array, arrayIndex, length);
      [PatchMethod(AggressiveInlining)] public                    void                                                                                 DynamicInvoke          (params object?[]?                                                          arguments)                                                                                                { foreach (ref readonly PatchOdyssey.Collections.HandlerInfo<T> handler in this) handler.DynamicInvoke(); }
      [PatchMethod(AggressiveInlining)] public   override         bool                                                                                 Equals                 (object?                                                                    value)                                                                                                    =>     base.Equals       (value);
      [PatchMethod(AggressiveInlining)] private  new              bool                                                                                 Exists                 (PatchOdyssey.RefPredicate        <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                =>     base.Exists       (predicate);
      [PatchMethod(AggressiveInlining)] private  new              bool                                                                                 Exists                 (PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                =>     base.Exists       (predicate);
      [PatchMethod(AggressiveInlining)] private  new              bool                                                                                 Exists                 (System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                =>     base.Exists       (predicate);
      [PatchMethod(AggressiveInlining)] private  new              PatchOdyssey.Collections.HandlerInfo<T>?                                             Find                   (PatchOdyssey.RefPredicate        <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                =>     base.Find         (predicate);
      [PatchMethod(AggressiveInlining)] private  new              PatchOdyssey.Collections.HandlerInfo<T>?                                             Find                   (PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                =>     base.Find         (predicate);
      [PatchMethod(AggressiveInlining)] private  new              PatchOdyssey.Collections.HandlerInfo<T>?                                             Find                   (System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                =>     base.Find         (predicate);
      [PatchMethod(AggressiveInlining)] private  new              EventHandler<T>                                                                      FindAll                (PatchOdyssey.RefPredicate        <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                => new(base.FindAll      (predicate));
      [PatchMethod(AggressiveInlining)] private  new              EventHandler<T>                                                                      FindAll                (PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                => new(base.FindAll      (predicate));
      [PatchMethod(AggressiveInlining)] private  new              EventHandler<T>                                                                      FindAll                (System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                => new(base.FindAll      (predicate));
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  FindIndex              (PatchOdyssey.RefPredicate        <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                =>     base.FindIndex    (predicate);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  FindIndex              (PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                =>     base.FindIndex    (predicate);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  FindIndex              (System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                =>     base.FindIndex    (predicate);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  FindIndex              (uint                                                                       index,              PatchOdyssey.RefPredicatee       <PatchOdyssey.Collections.HandlerInfo<T>> predicate) =>     base.FindIndex    (index,         predicate);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  FindIndex              (uint                                                                       index,              PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>> predicate) =>     base.FindIndex    (index,         predicate);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  FindIndex              (uint                                                                       index,              System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>> predicate) =>     base.FindIndex    (index,         predicate);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  FindIndex              (uint                                                                       index, uint length, PatchOdyssey.RefPredicatee       <PatchOdyssey.Collections.HandlerInfo<T>> predicate) =>     base.FindIndex    (index, length, predicate);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  FindIndex              (uint                                                                       index, uint length, PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>> predicate) =>     base.FindIndex    (index, length, predicate);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  FindIndex              (uint                                                                       index, uint length, System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>> predicate) =>     base.FindIndex    (index, length, predicate);
      [PatchMethod(AggressiveInlining)] private  new              PatchOdyssey.Collections.HandlerInfo<T>?                                             FindLast               (PatchOdyssey.RefPredicate        <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                =>     base.FindLast     (predicate);
      [PatchMethod(AggressiveInlining)] private  new              PatchOdyssey.Collections.HandlerInfo<T>?                                             FindLast               (PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                =>     base.FindLast     (predicate);
      [PatchMethod(AggressiveInlining)] private  new              PatchOdyssey.Collections.HandlerInfo<T>?                                             FindLast               (System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                =>     base.FindLast     (predicate);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  FindLastIndex          (PatchOdyssey.RefPredicate        <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                =>     base.FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  FindLastIndex          (PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                =>     base.FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  FindLastIndex          (System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                                =>     base.FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  FindLastIndex          (uint                                                                       index,              PatchOdyssey.RefPredicate        <PatchOdyssey.Collections.HandlerInfo<T>> predicate) =>     base.FindLastIndex(index,         predicate);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  FindLastIndex          (uint                                                                       index,              PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>> predicate) =>     base.FindLastIndex(index,         predicate);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  FindLastIndex          (uint                                                                       index,              System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>> predicate) =>     base.FindLastIndex(index,         predicate);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  FindLastIndex          (uint                                                                       index, uint length, PatchOdyssey.RefPredicate        <PatchOdyssey.Collections.HandlerInfo<T>> predicate) =>     base.FindLastIndex(index, length, predicate);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  FindLastIndex          (uint                                                                       index, uint length, PatchOdyssey.RefReadOnlyPredicate<PatchOdyssey.Collections.HandlerInfo<T>> predicate) =>     base.FindLastIndex(index, length, predicate);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  FindLastIndex          (uint                                                                       index, uint length, System.Predicate                 <PatchOdyssey.Collections.HandlerInfo<T>> predicate) =>     base.FindLastIndex(index, length, predicate);
      [PatchMethod(AggressiveInlining)] private  new              void                                                                                 ForEach                (PatchOdyssey.RefAction        <PatchOdyssey.Collections.HandlerInfo<T>>    action)                                                                                                   =>     base.ForEach      (action);
      [PatchMethod(AggressiveInlining)] private  new              void                                                                                 ForEach                (PatchOdyssey.RefReadOnlyAction<PatchOdyssey.Collections.HandlerInfo<T>>    action)                                                                                                   =>     base.ForEach      (action);
      [PatchMethod(AggressiveInlining)] private  new              void                                                                                 ForEach                (System.Action                 <PatchOdyssey.Collections.HandlerInfo<T>>    action)                                                                                                   =>     base.ForEach      (action);
      [PatchMethod(AggressiveInlining)] private  new              PatchOdyssey.Collections.RefList<PatchOdyssey.Collections.HandlerInfo<T>>.Enumerator GetEnumerator          ()                                                                                                                                                                                    =>     base.GetEnumerator();
      [PatchMethod(AggressiveInlining)] public   override         int                                                                                  GetHashCode            ()                                                                                                                                                                                    =>     base.GetHashCode  ();
      [PatchMethod(AggressiveInlining)] public   /* virtual */    System.Delegate[]                                                                    GetInvocationList      ()                                                                                                                                                                                    => Util.ArrayFrom(base.ConvertAll(static (in PatchOdyssey.Collections.HandlerInfo<T> handler) => (System.Delegate) ([PatchMethod(AggressiveInlining)] () => handler.Invoke())));
      [PatchMethod(AggressiveInlining)] private  new              EventHandler<T>                                                                      GetRange               (uint                                       index, uint length)                                                                                                                       => new(base.GetRange(index, length));
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  IndexOf                (in PatchOdyssey.Collections.HandlerInfo<T> handler)                                                                                                                                  =>     base.IndexOf (in handler);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  IndexOf                (in PatchOdyssey.Collections.HandlerInfo<T> handler, uint                                                                            index)                                           =>     base.IndexOf (in handler, index);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  IndexOf                (in PatchOdyssey.Collections.HandlerInfo<T> handler, uint                                                                            index, uint length)                              =>     base.IndexOf (in handler, index, length);
      [PatchMethod(AggressiveInlining)] internal new              void                                                                                 Insert                 (uint                                       index,   in PatchOdyssey.Collections.HandlerInfo<T>                                      handler)                                         { if (PatchOdyssey.Collections.HandlerInfo<T>.DefaultValue != handler.value) base.Insert(index, in handler); }
      [PatchMethod(AggressiveInlining)] internal new              void                                                                                 InsertRange            (uint                                       index,   System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.HandlerInfo<T>> handlers)                                        { foreach (PatchOdyssey.Collections.HandlerInfo<T> handler in handlers) { if (PatchOdyssey.Collections.HandlerInfo<T>.DefaultValue != handler.value) base.Insert(index++, in handler); } }
      [PatchMethod(AggressiveInlining)] public                    void                                                                                 Invoke                 ()                                                                                                                                                                                    { foreach (ref readonly PatchOdyssey.Collections.HandlerInfo<T> handler in this) handler.Invoke(); }
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  LastIndexOf            (in PatchOdyssey.Collections.HandlerInfo<T>                                       handler)                                                                                            =>     base.LastIndexOf(in handler);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  LastIndexOf            (in PatchOdyssey.Collections.HandlerInfo<T>                                       handler, uint index)                                                                                =>     base.LastIndexOf(in handler, index);
      [PatchMethod(AggressiveInlining)] private  new              int                                                                                  LastIndexOf            (in PatchOdyssey.Collections.HandlerInfo<T>                                       handler, uint index, uint length)                                                                   =>     base.LastIndexOf(in handler, index, length);
      [PatchMethod(AggressiveInlining)] public   new              ref readonly PatchOdyssey.Collections.HandlerInfo<T>                                 Prepend                (in PatchOdyssey.Collections.HandlerInfo<T>                                       handler)                                                                                            => ref base.Prepend    (in handler);
      [PatchMethod(AggressiveInlining)] public   new              bool                                                                                 Remove                 (in PatchOdyssey.Collections.HandlerInfo<T>                                       handler)                                                                                            =>     base.Remove     (in handler);
      [PatchMethod(AggressiveInlining)] public   static           void                                                                                 Remove                 (ref EventHandler                       <T>                                       events, in PatchOdyssey.Collections.HandlerInfo<T> handler)                                         { for (int index = events.Count; 0 != index--; ) if (events[index].value == handler.value) { events.RemoveAt(index); return; } } // ⟶ Based on `𝑓 System.Delegate.Remove(…)`
      [PatchMethod(AggressiveInlining)] public   new              int                                                                                  RemoveAll              (PatchOdyssey.RefPredicate              <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                          => base.RemoveAll  (predicate);
      [PatchMethod(AggressiveInlining)] public   new              int                                                                                  RemoveAll              (PatchOdyssey.RefReadOnlyPredicate      <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                          => base.RemoveAll  (predicate);
      [PatchMethod(AggressiveInlining)] public   new              int                                                                                  RemoveAll              (System.Predicate                       <PatchOdyssey.Collections.HandlerInfo<T>> predicate)                                                                                          => base.RemoveAll  (predicate);
      [PatchMethod(AggressiveInlining)] public   new              void                                                                                 RemoveAt               (uint                                                                             index)                                                                                              => base.RemoveAt   (index);
      [PatchMethod(AggressiveInlining)] public   new              void                                                                                 RemoveRange            (uint                                                                             index, uint length)                                                                                 => base.RemoveRange(index, length);
      [PatchMethod(AggressiveInlining)] private  new              void                                                                                 Reverse                ()                                                                                                                                                                                    => base.Reverse    ();
      [PatchMethod(AggressiveInlining)] private  new              void                                                                                 Reverse                (uint index, uint length)                                                                                                                                                             => base.Reverse    (index, length);
      [PatchMethod(AggressiveInlining)] private  new              void                                                                                 Sort                   ()                                                                                                                                                                                    => base.Sort       ();
      [PatchMethod(AggressiveInlining)] private  new              void                                                                                 Sort                   (PatchOdyssey.RefComparison          <T>? comparison)                                                                                                                                 => base.Sort       (comparison);
      [PatchMethod(AggressiveInlining)] private  new              void                                                                                 Sort                   (PatchOdyssey.RefReadOnlyComparison  <T>? comparison)                                                                                                                                 => base.Sort       (comparison);
      [PatchMethod(AggressiveInlining)] private  new              void                                                                                 Sort                   (System.Collections.Generic.IComparer<T>? comparer)                                                                                                                                   => base.Sort       (comparer);
      [PatchMethod(AggressiveInlining)] private  new              void                                                                                 Sort                   (System.Comparison                   <T>? comparison)                                                                                                                                 => base.Sort       (comparison);
      [PatchMethod(AggressiveInlining)] private  new              void                                                                                 Sort                   (uint                                     index, uint length, System.Collections.Generic.IComparer<T>? comparer)                                                                      => base.Sort       (index, length, comparer);
      [PatchMethod(AggressiveInlining)] private  new              PatchOdyssey.Collections.HandlerInfo<T>[]                                            ToArray                ()                                                                                                                                                                                    => base.ToArray    ();
      [PatchMethod(AggressiveInlining)] private  new              void                                                                                 TrimExcess             ()                                                                                                                                                                                    => base.TrimExcess ();
      [PatchMethod(AggressiveInlining)] private  new              void                                                                                 TrimExcess             (uint                                 capacity)                                                                                                                                       => base.TrimExcess (capacity);
      [PatchMethod(AggressiveInlining)] private  new              bool                                                                                 TrueForAll             (PatchOdyssey.RefPredicate        <T> predicate)                                                                                                                                      => base.TrueForAll (predicate);
      [PatchMethod(AggressiveInlining)] private  new              bool                                                                                 TrueForAll             (PatchOdyssey.RefReadOnlyPredicate<T> predicate)                                                                                                                                      => base.TrueForAll (predicate);
      [PatchMethod(AggressiveInlining)] private  new              bool                                                                                 TrueForAll             (System.Predicate                 <T> predicate)                                                                                                                                      => base.TrueForAll (predicate);
      [PatchMethod(AggressiveInlining)] object                                                                                                         System.ICloneable.Clone()                                                                                                                                                                                    => base.Clone      ();

      /* … */
      [PatchMethod(AggressiveInlining)] public static EventHandler<T> operator +(ref EventHandler<T> events, in PatchOdyssey.Collections.HandlerInfo<T> handler) { EventHandler<T>.Combine(ref events, in handler); return events; }
      [PatchMethod(AggressiveInlining)] public static EventHandler<T> operator -(ref EventHandler<T> events, in PatchOdyssey.Collections.HandlerInfo<T> handler) { EventHandler<T>.Remove (ref events, in handler); return events; }

      [PatchMethod(AggressiveInlining)] public static implicit operator System.Delegate(in EventHandler<T> events) => [PatchMethod(AggressiveInlining)] () => events.Invoke();
    }

    public struct GameObjectEnumerator : /* ⟶ Ranged `foreach …` shorthand support */ System.Collections.Generic.IEnumerable<UnityEngine.GameObject>, System.Collections.Generic.IEnumerator<UnityEngine.GameObject> {
      private enum HierarchyCurrent : sbyte { Unmoved = -1, Pending,     Moved };
      public  enum Kind             : byte  { Children,     Descendants, Hierarchy };

      public                 UnityEngine.GameObject                                  Current                                                                => (GameObjectEnumerator.HierarchyCurrent.Pending == this.uproot ? this.root : (UnityEngine.Transform) this.subenumerator.Current).gameObject;
      private readonly       GameObjectEnumerator.Kind                               kind                                                                   =  default;
      private readonly       System.Collections.Generic.Queue<UnityEngine.Transform> hierarchy                                                              =  new();
      private readonly       UnityEngine.Transform                                   root                                                                   =  null!;
      private /* required */ System.Collections.IEnumerator                          subenumerator                                                          =  null!;
      private /* required */ GameObjectEnumerator.HierarchyCurrent                   uproot                                                                 =  GameObjectEnumerator.HierarchyCurrent.Unmoved;
      UnityEngine.GameObject                                                         System.Collections.Generic.IEnumerator<UnityEngine.GameObject>.Current => this.Current;
      object                                                                         System.Collections.IEnumerator.Current                                 => this.Current!;

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
      [PatchMethod(AggressiveInlining)] public void                 Dispose      () { /* Do nothing… */ }
      [PatchMethod(AggressiveInlining)] public GameObjectEnumerator GetEnumerator() => this;

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
              if (0 == this.hierarchy.Count)
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

      [PatchMethod(AggressiveInlining)] System.Collections.Generic.IEnumerator<UnityEngine.GameObject> System.Collections.Generic.IEnumerable<UnityEngine.GameObject>.GetEnumerator() => this.GetEnumerator();
      [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator                                 System.Collections.IEnumerable.GetEnumerator                                () => this.GetEnumerator();
      [PatchMethod(AggressiveInlining)] bool                                                           System.Collections.IEnumerator.MoveNext                                     () => this.MoveNext     ();
      [PatchMethod(AggressiveInlining)] void                                                           System.Collections.IEnumerator.Reset                                        () => this.Reset        ();
      [PatchMethod(AggressiveInlining)] void                                                           System.IDisposable.Dispose                                                  () => this.Dispose      ();
    }

    public readonly struct HandlerInfo<T> where T : PatchOdyssey.Collections.Event, new() /* ⟶ Based on `System.MulticastDelegate` */ {
      internal readonly PatchOdyssey.Handler<T> value    = HandlerInfo<T>.DefaultValue; // ⟶ Callback function(s)
      internal readonly object?                 target   = null;                        // ⟶ Callback source
      internal readonly T                       metadata = new();                       // ⟶ Callback event data

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)] public HandlerInfo(in HandlerInfo<T> handler) : this(handler.value, handler.target, handler.metadata) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)]
      public HandlerInfo(PatchOdyssey.Handler<T> value, object? target, T metadata) {
        this.metadata = metadata;
        this.target   = target;
        this.value    = value ?? HandlerInfo<T>.DefaultValue;
      }

      /* … */
      [PatchMethod(AggressiveInlining)] internal /* virtual */ object            Clone            ()                                 => new HandlerInfo<T>((PatchOdyssey.Handler<T>) this.value.Clone(), this.target, this.metadata);
      [PatchMethod(NoInlining)]         internal static        void              DefaultValue     (object?           target, T data) {}
      [PatchMethod(AggressiveInlining)] internal               void              DynamicInvoke    (params object?[]? arguments)      => this.value.DynamicInvoke(new object?[] {this.target, this.metadata});
      [PatchMethod(AggressiveInlining)] public   override      bool              Equals           (object?           value)          => this.GetHashCode() == value.GetHashCode();
      [PatchMethod(AggressiveInlining)] public   override      int               GetHashCode      ()                                 => System.HashCode.Combine(this.metadata, this.target, this.value);
      [PatchMethod(AggressiveInlining)] internal /* virtual */ System.Delegate[] GetInvocationList()                                 => this.value.GetInvocationList();
      [PatchMethod(AggressiveInlining)] internal               void              Invoke           ()                                 => this.value.Invoke(this.target, this.metadata);

      /* … */
      [PatchMethod(AggressiveInlining)] public static implicit operator PatchOdyssey.Handler<T>(HandlerInfo<T> handler) => handler.value;
    }

    internal readonly struct LoadInfo {
      internal static readonly System.Collections.Generic.Dictionary<(System.Type, System.Uri), PatchOdyssey.Collections.LoadInfo> LOADS = new(16);
      internal          object?                                                                   cached   = null;
      internal readonly PatchOdyssey.Collections.EventHandler<PatchOdyssey.Collections.LoadEvent> handlers = new();
      internal          object?                                                                   payload  = null;

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)] internal LoadInfo()                                                                                    {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] internal LoadInfo(in PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Collections.LoadEvent> handler) { this.handlers = new(handler); }
    }

    private interface Mono {
      public abstract bool    HasValue { get; }
      public abstract object? Value    { get; }
    }

    public readonly struct Mono<T> : PatchOdyssey.Collections.Mono /* ⟶ Based on `System.Nullable<T>` */ {
      public readonly bool HasValue = false;
      public readonly T    Value    = default!;
      object?              PatchOdyssey.Collections.Mono.HasValue => this.HasValue;
      object?              PatchOdyssey.Collections.Mono.Value    => this.HasValue ? this.Value : null;

      /* … */
      [PatchMethod(AggressiveInlining)] public  Mono()           {}
      [PatchMethod(AggressiveInlining)] private Mono(in T value) { this.HasValue = true; this.Value = value; }

      /* … */
      [PatchMethod(AggressiveInlining)]
      public override bool Equals(object? value) => value switch {
        Mono<T>                       mono => mono.HasValue == this.HasValue && System.Collections.Generic.EqualityComparer<T>.Default.Equals(mono.Value, this.Value),
        PatchOdyssey.Collections.Mono mono => object.Equals(mono.Value, ((PatchOdyssey.Collections.Mono) this).Value),
        _                                  => object.Equals(this.Value, value)
      };

      [PatchMethod(AggressiveInlining)] public override int     GetHashCode      ()           => this.HasValue ? this.Value!.GetHashCode() : base.GetHashCode();
      [PatchMethod(AggressiveInlining)] public readonly T       GetValueOrDefault()           => this.GetValueOrDefault(default!);
      [PatchMethod(AggressiveInlining)] public readonly T       GetValueOrDefault(T fallback) => this.HasValue ? this.Value : fallback;
      [PatchMethod(AggressiveInlining)] public override string? ToString         ()           => this.HasValue ? this.Value!.ToString() : string.Empty;

      [PatchMethod(AggressiveInlining)] public static Mono<T> operator +(in Mono<T> mono, in T value) => mono.HasValue ? mono : new(value);

      [PatchMethod(AggressiveInlining)] public static explicit operator T      (in Mono<T> mono)  => mono.Value;
      [PatchMethod(AggressiveInlining)] public static implicit operator Mono<T>(in T       value) => new(value);
    }

    public abstract class RefComparer<T> : PatchOdyssey.Collections.RefReadOnlyComparer<T>;

    [System.Serializable]
    public class RefDictionary<TKey, TValue> : System.Collections.Generic.IDictionary<TKey, TValue>, System.Collections.ICollection, System.Collections.IDictionary /* ⟶ Based on `System.Collections.Generic.Dictionary<TKey, TValue>`; See `https://web.archive.org/web/20240722203244/https://discussions.unity.com/t/finally-a-serializable-dictionary-for-unity-extracted-from-system-collections-generic/586385` */ {
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

        public ref TValue this[in TAlternateKey key] { get => ref Util.Reference<TValue>(); }
      }

      public struct Enumerator : System.Collections.Generic.IEnumerator<PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>>, System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.IDictionaryEnumerator /* ⟶ Based on `System.Collections.Generic.Dictionary<TKey, TValue>.Enumerator` */ {
        private class Index { public uint value = 0u; }

        public                 ref PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>   Current => ref System.Runtime.InteropServices.MemoryMarshal.GetReference(this.current);
        private /* required */     PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>[] current;
        private readonly           RefDictionary                           <TKey, TValue>   dictionary;
        private /* required */     Enumerator.Index                                         index;
        PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>                              System.Collections.Generic.IEnumerator<PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>>.Current => this.Current;
        System.Collections.Generic.KeyValuePair <TKey, TValue>                              System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Current  => this.Current;
        System.Collections.DictionaryEntry                                                  System.Collections.IDictionaryEnumerator.Entry                                                         => new(this.Current.Key, this.Current.Value);
        object                                                                              System.Collections.IDictionaryEnumerator.Key                                                           => this.Current.Key;
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
            Util.ArrayReference(this.current) = new(this.dictionary, this.index.value++);
            return true;
          }

          Util.ArrayReference(this.current) = default;
          this.index.value                  = this.dictionary.count;

          return false;
        }

        [PatchMethod(AggressiveInlining)] public void Reset                                  () { this.current = new PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>[] {default}; this.index = new() {value = 0u}; }
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
        [PatchMethod(AggressiveInlining)] void                                         System.Collections.Generic.ICollection<TKey>.Add          (TKey element)                  { throw new System.NotSupportedException("Dictionary key collection is read-only"); }
        [PatchMethod(AggressiveInlining)] void                                         System.Collections.Generic.ICollection<TKey>.Clear        ()                              { throw new System.NotSupportedException("Dictionary key collection is read-only"); }
        [PatchMethod(AggressiveInlining)] bool                                         System.Collections.Generic.ICollection<TKey>.Contains     (TKey   element)                => this.Contains(in element);
        [PatchMethod(AggressiveInlining)] void                                         System.Collections.Generic.ICollection<TKey>.CopyTo       (TKey[] array, int index)       => this.CopyTo  (array, (uint) index);
        [PatchMethod(AggressiveInlining)] bool                                         System.Collections.Generic.ICollection<TKey>.Remove       (TKey   element)                { throw new System.NotSupportedException("Dictionary key collection is read-only"); }
        [PatchMethod(AggressiveInlining)] System.Collections.Generic.IEnumerator<TKey> System.Collections.Generic.IEnumerable<TKey>.GetEnumerator()                              => this.GetEnumerator();
        [PatchMethod(AggressiveInlining)] void                                         System.Collections.ICollection.CopyTo                     (System.Array array, int index) => this.CopyTo       ((TKey[]) array, (uint) index);
        [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator               System.Collections.IEnumerable.GetEnumerator              ()                              => this.GetEnumerator();
      }

      public /* sealed */ class ValueCollection : System.Collections.Generic.ICollection<TValue>, System.Collections.Generic.IReadOnlyCollection<TValue>, System.Collections.ICollection /* ⟶ Based on `System.Collections.Generic.Dictionary<TKey, TValue>.ValueCollection` */ {
        public struct Enumerator : System.Collections.Generic.IEnumerator<TValue> /* ⟶ Based on `System.Collections.Generic.Dictionary<TKey, TValue>.ValueCollection.Enumerator` */ {
          public  ref          TValue                                 Current => ref this.enumerator.Current.Value;
          private     readonly RefDictionary<TKey, TValue>.Enumerator enumerator;
          TValue                                                      System.Collections.Generic.IEnumerator<TValue>.Current => this.Current;
          object                                                      System.Collections.IEnumerator.Current                 => this.Current!;

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
        public           int                         Count                                                        => (int) this.dictionary.Count;
        private readonly RefDictionary<TKey, TValue> dictionary                                                   =  null!;
        bool                                         System.Collections.Generic.ICollection<TValue>.IsReadOnly    => true;
        int                                          System.Collections.Generic.IReadOnlyCollection<TValue>.Count => this.Count;
        int                                          System.Collections.ICollection.Count                         => this.Count;
        bool                                         System.Collections.ICollection.IsSynchronized                => false;
        object                                       System.Collections.ICollection.SyncRoot                      => this;

        /* … */
        [PatchMethod(AggressiveInlining)]
        public ValueCollection(RefDictionary<TKey, TValue> dictionary) => this.dictionary = dictionary;

        /* … */
        [PatchMethod(AggressiveInlining)] public bool                                    Contains                                                    (in TValue element)             => this.dictionary.ContainsValue(in element);
        [PatchMethod(AggressiveInlining)] public void                                    CopyTo                                                      (TValue[]  array, uint index)   { foreach (ref readonly TValue element in this) array[index++] = element; }
        [PatchMethod(AggressiveInlining)] public ValueCollection.Enumerator              GetEnumerator                                               ()                              => new(this.dictionary);
        [PatchMethod(AggressiveInlining)] void                                           System.Collections.Generic.ICollection<TValue>.Add          (TValue element)                { throw new System.NotSupportedException("Dictionary key collection is read-only"); }
        [PatchMethod(AggressiveInlining)] void                                           System.Collections.Generic.ICollection<TValue>.Clear        ()                              { throw new System.NotSupportedException("Dictionary key collection is read-only"); }
        [PatchMethod(AggressiveInlining)] bool                                           System.Collections.Generic.ICollection<TValue>.Contains     (TValue   element)              => this.Contains(in element);
        [PatchMethod(AggressiveInlining)] void                                           System.Collections.Generic.ICollection<TValue>.CopyTo       (TValue[] array, int index)     => this.CopyTo  (array, (uint) index);
        [PatchMethod(AggressiveInlining)] bool                                           System.Collections.Generic.ICollection<TValue>.Remove       (TValue   element)              { throw new System.NotSupportedException("Dictionary key collection is read-only"); }
        [PatchMethod(AggressiveInlining)] System.Collections.Generic.IEnumerator<TValue> System.Collections.Generic.IEnumerable<TValue>.GetEnumerator()                              => this.GetEnumerator();
        [PatchMethod(AggressiveInlining)] void                                           System.Collections.ICollection.CopyTo                       (System.Array array, int index) => this.CopyTo       ((TValue[]) array, (uint) index);
        [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator                 System.Collections.IEnumerable.GetEnumerator                ()                              => this.GetEnumerator();
      }

      /* … */
      private const           uint                                           CapacityMaximum = 0x7FEFFFFDu;
      private static readonly PatchOdyssey.Collections.RefReadOnlyList<uint> Primes          = new(new[] {3u, 7u, 11u, 17u, 23u, 29u, 37u, 47u, 59u, 71u, 89u, 107u, 131u, 163u, 197u, 239u, 293u, 353u, 431u, 521u, 631u, 761u, 919u, 1103u, 1327u, 1597u, 1931u, 2333u, 2801u, 3371u, 4049u, 4861u, 5839u, 7013u, 8419u, 10103u, 12143u, 14591u, 17519u, 21023u, 25229u, 30293u, 36353u, 43627u, 52361u, 62851u, 75431u, 90523u, 108631u, 130363u, 156437u, 187751u, 225307u, 270371u, 324449u, 389357u, 467237u, 560689u, 672827u, 807403u, 968897u, 1162687u, 1395263u, 1674319u, 2009191u, 2411033u, 2893249u, 3471899u, 4166287u, 4999559u, 5999471u, 7199369u});
      private const           uint                                           PrimeMaximum    = 0x7FFFFFFFu;

      [UnityEngine.HideInInspector, UnityEngine.SerializeField] private           PatchOdyssey.Collections.RefList<int>              buckets                                                                                                  =  new();
      public                                                                      uint                                               Capacity                                                                                                 => this.count;
      public                                                             readonly System.Collections.Generic.IEqualityComparer<TKey> Comparer                                                                                                 =  System.Collections.Generic.EqualityComparer<TKey>.Default;
      public                                                                      uint                                               Count                                                                                                    => this.count - this.free.count;
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] private           uint                                               count                                                                                                    =  0u;
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] private           (uint count, int list)                             free                                                                                                     =  (0u, -1);
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] private           PatchOdyssey.Collections.RefList<int>              hashes                                                                                                   =  new();
      public                                                                      RefDictionary<TKey, TValue>.KeyCollection          Keys                                                                                                     => new(this);
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] internal          PatchOdyssey.Collections.RefList<TKey>             keys                                                                                                     =  new();
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] private           PatchOdyssey.Collections.RefList<int>              next                                                                                                     =  new();
      public                                                                      RefDictionary<TKey, TValue>.ValueCollection        Values                                                                                                   => new(this);
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] internal          PatchOdyssey.Collections.RefList<TValue>           values                                                                                                   =  new();
      bool                                                                                                                           System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.IsReadOnly => false;
      int                                                                                                                            System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Count      => ((int) this.Count);
      System.Collections.Generic.ICollection<TKey>                                                                                   System.Collections.Generic.IDictionary<TKey, TValue>.Keys                                                => this.Keys;
      System.Collections.Generic.ICollection<TValue>                                                                                 System.Collections.Generic.IDictionary<TKey, TValue>.Values                                              => this.Values;
      int                                                                                                                            System.Collections.ICollection.Count                                                                     => (int) this.Count;
      bool                                                                                                                           System.Collections.ICollection.IsSynchronized                                                            => false;
      object                                                                                                                         System.Collections.ICollection.SyncRoot                                                                  => this;
      bool                                                                                                                           System.Collections.IDictionary.IsFixedSize                                                               => false;
      bool                                                                                                                           System.Collections.IDictionary.IsReadOnly                                                                => false;
      System.Collections.ICollection                                                                                                 System.Collections.IDictionary.Keys                                                                      => this.Keys;
      System.Collections.ICollection                                                                                                 System.Collections.IDictionary.Values                                                                    => this.Values;

      /* … ⟶ Availability of `System.Runtime.InteropServices.CollectionMarshal.GetValueRefOrNullRef(…)` would replace `RefDictionary<T…>`’s entire purpose */
      [PatchConstructor, PatchMethod(AggressiveInlining)] public RefDictionary()                                                                                                                                                                             : this(0u,                                                                                         null!)    {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public RefDictionary(uint                                                                                                 capacity)                                                                : this(capacity,                                                                                   null!)    {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public RefDictionary(System.Collections.Generic.IEqualityComparer<TKey>                                                   comparer)                                                                : this(0u,                                                                                         comparer) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public RefDictionary(System.Collections.Generic.IDictionary      <TKey, TValue>                                           dictionary)                                                              : this((System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>>) dictionary, null!)    {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public RefDictionary(System.Collections.Generic.IEnumerable      <PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>> enumerable)                                                              : this(enumerable,                                                                                 null!)    {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public RefDictionary(System.Collections.Generic.IEnumerable      <System.Collections.Generic.KeyValuePair<TKey, TValue>>  enumerable)                                                              : this(enumerable,                                                                                 null!)    {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public RefDictionary(System.Collections.Generic.IDictionary      <TKey, TValue>                                           dictionary, System.Collections.Generic.IEqualityComparer<TKey> comparer) : this((System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>>) dictionary, comparer) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public RefDictionary(System.Collections.Generic.IEnumerable      <PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>> enumerable, System.Collections.Generic.IEqualityComparer<TKey> comparer) : this(enumerable is null ? 0u : Util.EnumerableCount(enumerable),                                 comparer) => this.AddRange(enumerable!);
      [PatchConstructor, PatchMethod(AggressiveInlining)] public RefDictionary(System.Collections.Generic.IEnumerable      <System.Collections.Generic.KeyValuePair<TKey, TValue>>  enumerable, System.Collections.Generic.IEqualityComparer<TKey> comparer) : this(enumerable is null ? 0u : Util.EnumerableCount(enumerable),                                 comparer) => this.AddRange(enumerable!);
      [PatchConstructor, PatchMethod(AggressiveInlining)] public RefDictionary(uint                                                                                                 capacity,   System.Collections.Generic.IEqualityComparer<TKey> comparer)                                                                                                              { RefDictionary<TKey, TValue>.Ensure(); this.Initialize(capacity); this.Comparer = comparer ?? System.Collections.Generic.EqualityComparer<TKey>.Default; }

      /* … */
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public void Add     (in PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>                                      element)              => this.Add   (in element.Key, in element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void Add     (in System.Collections.Generic.KeyValuePair <TKey, TValue>                                      element)              => this.Add   (element.Key,    element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void Add     (in TKey                                                                                        key, in TValue value) { if (key is not null) this.Insert(in key, in value, true); }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public void AddRange(System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>> enumerable)           { foreach (PatchOdyssey.Collections.RefKeyValuePair                <TKey, TValue> element in enumerable) this.Add(in element); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void AddRange(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>  enumerable)           { foreach (System.Collections.Generic.KeyValuePair<TKey, TValue> element in enumerable) this.Add(element); }

      [PatchMethod(AggressiveInlining)]
      public void Clear() {
        this.hashes .Clear();
        this.keys   .Clear();
        this.next   .Clear();
        this.values .Clear();

        this.count = 0u;
        this.free  = (count: 0u, list: -1);

        for (uint index = this.buckets.Count; 0u != index--; )
        this.buckets[index] = -1;
      }

      [PatchMethod(AggressiveInlining), PatchResolution(1)] public bool Contains     (in PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue> element)           { int index = this.FindIndex(element.Key); return index != -1 && System.Collections.Generic.EqualityComparer<TValue>.Default.Equals(this.values[(uint) index], element.Value); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public bool Contains     (in System.Collections.Generic.KeyValuePair<TKey, TValue>  element)           { int index = this.FindIndex(element.Key); return index != -1 && System.Collections.Generic.EqualityComparer<TValue>.Default.Equals(this.values[(uint) index], element.Value); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public bool ContainsKey  (in TKey                                                   key)               => this.FindIndex(in key) != -1;
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public bool ContainsValue(in TValue                                                 value)             { System.Func<TValue, TValue, bool> comparer = value is null ? static (value, _) => value is null : System.Collections.Generic.EqualityComparer<TValue>.Default.Equals; for (uint index = this.count; 0u != index--; ) { if (this.hashes[index] >= 0 && comparer(this.values[index], value)) return true; } return false; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public void CopyTo       (PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>[]  array, uint index) { for (uint subindex = 0u; subindex != this.count; ++subindex) { if (this.hashes[subindex] >= 0) array[index++] = new(in this.keys[subindex], in this.values[subindex]); } }

      public static PatchOdyssey.DictionaryGUIField? DelegateGUIField<T>() {
        #if UNITY_EDITOR
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
        #endif

        return null;
      }

      [PatchMethod(AggressiveInlining)]
      public static void Ensure() {
        if (RefDictionary<TKey, TValue>.DelegateGUIField<TKey>  () is null) throw new System.NotSupportedException($"Type `{typeof(TKey)  }` is not supported for `RefDictionary`");
        if (RefDictionary<TKey, TValue>.DelegateGUIField<TValue>() is null) throw new System.NotSupportedException($"Type `{typeof(TValue)}` is not supported for `RefDictionary`");
      }

      [PatchMethod(AggressiveInlining)]
      public void EnsureCapacity(uint capacity) { /* Do nothing… */ }

      [PatchMethod(AggressiveInlining)]
      private int FindIndex(in TKey key) {
        if (key is not null && 0 != this.buckets.Count) {
          int hash = this.Comparer.GetHashCode(key) & (int) RefDictionary<TKey, TValue>.PrimeMaximum;

          // …
          for (int index = this.buckets[(uint) (hash % this.buckets.Count)]; index >= 0; index = this.next[(uint) index]) {
            if (hash == this.hashes[(uint) index] && this.Comparer.Equals(this.keys[(uint) index], key))
            return index;
          }
        }

        return -1;
      }

      [PatchMethod(AggressiveInlining)] public RefDictionary<TKey, TValue>.AlternateLookup<TAlternateKey> GetAlternateLookup<TAlternateKey>() => new(this);
      [PatchMethod(AggressiveInlining)] public RefDictionary<TKey, TValue>.Enumerator                     GetEnumerator                    () => new(this);

      [PatchMethod(AggressiveInlining)]
      public static uint GetPrime(uint minimum) {
        [PatchMethod(AggressiveInlining)]
        static uint Sqrt(uint number) /* ⟶ `System.Math.Sqrt(double)` exists but is suboptimal for (unsigned) integer-only roots */{
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

      [PatchMethod(AggressiveInlining)]
      private void Initialize(uint capacity) {
        capacity = RefDictionary<TKey, TValue>.GetPrime(capacity);

        this.buckets.EnsureCapacity(capacity, true);
        this.hashes .EnsureCapacity(capacity, true);
        this.keys   .EnsureCapacity(capacity, true);
        this.next   .EnsureCapacity(capacity, true);
        this.values .EnsureCapacity(capacity, true);

        this.buckets.Count = this.hashes.Count = this.keys.Count = this.next.Count = this.values.Count = capacity;
        this.free.list     = -1;

        for (uint index = this.buckets.Count; 0u != index--; )
        this.buckets[index] = -1;
      }

      private ref TValue Insert(in TKey key, in TValue value, bool immutable) {
        if (0 == this.buckets.Count) this.Initialize(0u);

        uint freeIndex = 0u;
        int  hash      = this.Comparer.GetHashCode(key) & (int) RefDictionary<TKey, TValue>.PrimeMaximum;
        uint hashIndex = (uint) (hash % this.buckets.Count);

        // …
        for (int index = this.buckets[hashIndex]; index >= 0; ++freeIndex, index = this.next[(uint) index])
        if (hash == this.hashes[(uint) index] && this.Comparer.Equals(this.keys[(uint) index], key)) {
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
            this.Resize(RefDictionary<TKey, TValue>.CapacityMaximum >> 0 > this.count && RefDictionary<TKey, TValue>.CapacityMaximum >> 1 < this.count ? RefDictionary<TKey, TValue>.CapacityMaximum : RefDictionary<TKey, TValue>.GetPrime(this.count << 1));

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

      [PatchMethod(AggressiveInlining), PatchResolution(1)] public bool Remove(in PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue> element) => (this.TryGetValue(in element.Key, out TValue value) && System.Collections.Generic.EqualityComparer<TValue>.Default.Equals(value, element.Value) ? this.Remove(in element.Key) : false);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public bool Remove(in System.Collections.Generic.KeyValuePair <TKey, TValue> element) => (this.TryGetValue(element.Key,    out TValue value) && System.Collections.Generic.EqualityComparer<TValue>.Default.Equals(value, element.Value) ? this.Remove(element.Key)    : false);
      public bool Remove(in TKey key) {
        if (key is not null) {
          int  freeIndex = -1;
          int  hash      = this.Comparer.GetHashCode(key) & (int) RefDictionary<TKey, TValue>.PrimeMaximum;
          uint hashIndex = (uint) (hash % this.buckets.Count);

          // …
          for (int index = this.buckets[hashIndex]; index >= 0; freeIndex = index = this.next[(uint) index])
          if (hash == this.hashes[(uint) index] && this.Comparer.Equals(this.keys[(uint) index], key)) {
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

        for (uint index = this.buckets.Count; 0u != index--; )
        this.buckets[index] = -1;

        for (uint index = 0; index != this.count; ++index) {
          uint hashIndex = (uint) (this.hashes[index] % capacity);

          // …
          this.next   [index]     = this.buckets[hashIndex];
          this.buckets[hashIndex] = (int) index;
        }
      }

      [PatchMethod(AggressiveInlining), PatchResolution(0)] public override string?                                                                                       ToString                                                                                                   ()                                                                                     { uint end = this.Count, index = 0u; if (0u != this.Count) unsafe { System.Text.StringBuilder builder = new(); char* separator = stackalloc char[] {',', ' '}; foreach (ref readonly PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue> element in this) { builder.Append(element); if (end == ++index) return builder.ToString(); builder.Append(separator, 2); } } }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                                                                          TrimExcess                                                                                                 ()                                                                                     { /* Do nothing… */ }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          void                                                                                          TrimExcess                                                                                                 (uint                                                           capacity)              { /* Do nothing… */ }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          bool                                                                                          TryAdd                                                                                                     (in PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>      element)               => this.TryAdd(in element.Key, in element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                                                                          TryAdd                                                                                                     (in System.Collections.Generic.KeyValuePair <TKey, TValue>      element)               => this.TryAdd(element.Key,    element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                                                                          TryAdd                                                                                                     (in TKey                                                        key, in TValue value)  { if (key is not null && !this.ContainsKey(in key)) { this.Add(in key, in value); return true; } return false; }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public          ref TValue                                                                                    TryAppend                                                                                                  (in PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>      element)               => ref this.TryAppend(in element.Key, in element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          ref TValue                                                                                    TryAppend                                                                                                  (in System.Collections.Generic.KeyValuePair<TKey, TValue>       element)               { (TKey key, TValue value)[] pair = new[] {(element.Key, element.Value)}; return ref this.TryAppend(in System.Runtime.InteropServices.MemoryMarshal.GetReference(pair).key, in System.Runtime.InteropServices.MemoryMarshal.GetReference(pair).value); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          ref TValue                                                                                    TryAppend                                                                                                  (in TKey                                                        key, in TValue  value) { int index = this.FindIndex(in key); return ref (index != -1 ? ref this.values[(uint) index] : ref (key is null ? ref Util.Reference<TValue>() : ref this.Insert(in key, in value, true))); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                                                                          TryGetAlternateLookup<TAlternateKey>                                                                       (out RefDictionary<TKey, TValue>.AlternateLookup<TAlternateKey> lookup)                { lookup = this.GetAlternateLookup<TAlternateKey>(); return false; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public          bool                                                                                          TryGetValue                                                                                                (in TKey                                                        key, out TValue value) { int index = this.FindIndex(in key); if (index != -1) { value = this.values[(uint) index]; return true; } value = default!; return false; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 void                                                                                          System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Add          (System.Collections.Generic.KeyValuePair<TKey, TValue>          element)               => this.Add     (in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 void                                                                                          System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Clear        ()                                                                                     => this.Clear   ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 bool                                                                                          System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Contains     (System.Collections.Generic.KeyValuePair<TKey, TValue>   element)                      => this.Contains(in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 void                                                                                          System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.CopyTo       (System.Collections.Generic.KeyValuePair<TKey, TValue>[] array, int index)             { using (RefDictionary<TKey, TValue>.Enumerator enumerator = this.GetEnumerator()) { while (enumerator.MoveNext()) array[index++] = enumerator.Current; } }
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 bool                                                                                          System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Remove       (System.Collections.Generic.KeyValuePair<TKey, TValue>   element)                      => this.Remove       (in element);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>> System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>.GetEnumerator()                                                                                     => this.GetEnumerator();
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 void                                                                                          System.Collections.Generic.IDictionary<TKey, TValue>.Add                                                   (TKey         key, TValue value)                                                       => this.Add          (in key, in value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 bool                                                                                          System.Collections.Generic.IDictionary<TKey, TValue>.ContainsKey                                           (TKey         key)                                                                     => this.ContainsKey  (in key);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 bool                                                                                          System.Collections.Generic.IDictionary<TKey, TValue>.Remove                                                (TKey         key)                                                                     => this.Remove       (in key);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 bool                                                                                          System.Collections.Generic.IDictionary<TKey, TValue>.TryGetValue                                           (TKey         key,   out TValue value)                                                 => this.TryGetValue  (in key, out value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 void                                                                                          System.Collections.ICollection.CopyTo                                                                      (System.Array array, int        index)                                                 { foreach (ref readonly PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue> element in this) switch (array) { case System.Collections.Generic.KeyValuePair<TKey, TValue>[] elements: elements[index++] = element; break; default: ((PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>[]) array)[index++] = element; break; } }
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 void                                                                                          System.Collections.IDictionary.Add                                                                         (object       key,   object?    value)                                                 => this.Add          ((TKey) key, (TValue) value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 void                                                                                          System.Collections.IDictionary.Clear                                                                       ()                                                                                     => this.Clear        ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 bool                                                                                          System.Collections.IDictionary.Contains                                                                    (object key)                                                                           => this.Contains     ((TKey) key);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 System.Collections.IDictionaryEnumerator                                                      System.Collections.IDictionary.GetEnumerator                                                               ()                                                                                     => this.GetEnumerator();
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 void                                                                                          System.Collections.IDictionary.Remove                                                                      (object key)                                                                           => this.Remove       ((TKey) key);
      [PatchMethod(AggressiveInlining), PatchResolution(0)]                 System.Collections.IEnumerator                                                                System.Collections.IEnumerable.GetEnumerator                                                               ()                                                                                     => this.GetEnumerator();

      public ref          TValue this                                                     [in TKey key]                  { [PatchMethod(AggressiveInlining)] get { int index = this.FindIndex(in key); if (index != -1) return ref this.values[(uint) index]; if (key is null) throw new System.Collections.Generic.KeyNotFoundException(key?.ToString() ?? string.Empty); return ref this.Insert(in key, Util.Reference<TValue>(), false); } }
      public ref readonly TValue this                                                     [in TKey key, in TValue value] { [PatchMethod(AggressiveInlining)] get { int index = this.FindIndex(in key); if (index != -1) return ref this.values[(uint) index]; return ref value; } }
      TValue                     System.Collections.Generic.IDictionary<TKey, TValue>.this[TKey    key]                  { [PatchMethod(AggressiveInlining)] get { int index = this.FindIndex(in key); if (index == -1) { throw new System.Collections.Generic.KeyNotFoundException(key?.ToString() ?? string.Empty); } return this.Values[(uint) index]; } set => this[in key] = value; }
      object?                    System.Collections.IDictionary.this                      [object  key]                  { get => ((System.Collections.Generic.IDictionary<TKey, TValue>) this)[(TKey) key]; set => ((System.Collections.Generic.IDictionary<TKey, TValue>) this)[(TKey) key] = (TValue) value; }
    }
      [System.Serializable] public class AnimationCurveDictionary : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.AnimationCurve> { [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveDictionary() : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveDictionary(int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveDictionary(System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveDictionary(System.Collections.Generic.IDictionary<string, UnityEngine.AnimationCurve> dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveDictionary(int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveDictionary(System.Collections.Generic.IDictionary<string, UnityEngine.AnimationCurve> dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class BooleanDictionary        : PatchOdyssey.Collections.RefDictionary<string, System     .Boolean>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanDictionary       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanDictionary       (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanDictionary       (System.Collections.Generic.IDictionary<string, System     .Boolean>        dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanDictionary       (System.Collections.Generic.IDictionary<string, System     .Boolean>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class BoundsDictionary         : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Bounds>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsDictionary        () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsDictionary        (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsDictionary        (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsDictionary        (System.Collections.Generic.IDictionary<string, UnityEngine.Bounds>         dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsDictionary        (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsDictionary        (System.Collections.Generic.IDictionary<string, UnityEngine.Bounds>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class BoundsIntDictionary      : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.BoundsInt>      { [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntDictionary     () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntDictionary     (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntDictionary     (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntDictionary     (System.Collections.Generic.IDictionary<string, UnityEngine.BoundsInt>      dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntDictionary     (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntDictionary     (System.Collections.Generic.IDictionary<string, UnityEngine.BoundsInt>      dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class ColorDictionary          : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Color>          { [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorDictionary         () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorDictionary         (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorDictionary         (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorDictionary         (System.Collections.Generic.IDictionary<string, UnityEngine.Color>          dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorDictionary         (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorDictionary         (System.Collections.Generic.IDictionary<string, UnityEngine.Color>          dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class DoubleDictionary         : PatchOdyssey.Collections.RefDictionary<string, System     .Double>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleDictionary        () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleDictionary        (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleDictionary        (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleDictionary        (System.Collections.Generic.IDictionary<string, System     .Double>         dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleDictionary        (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleDictionary        (System.Collections.Generic.IDictionary<string, System     .Double>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class FloatDictionary          : PatchOdyssey.Collections.RefDictionary<string, System     .Single>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatDictionary         () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatDictionary         (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatDictionary         (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatDictionary         (System.Collections.Generic.IDictionary<string, System     .Single>         dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatDictionary         (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatDictionary         (System.Collections.Generic.IDictionary<string, System     .Single>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class GameObjectDictionary     : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.GameObject>     { [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectDictionary    () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectDictionary    (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectDictionary    (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.GameObject>     dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectDictionary    (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.GameObject>     dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class GradientDictionary       : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Gradient>       { [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientDictionary      () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientDictionary      (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientDictionary      (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientDictionary      (System.Collections.Generic.IDictionary<string, UnityEngine.Gradient>       dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientDictionary      (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientDictionary      (System.Collections.Generic.IDictionary<string, UnityEngine.Gradient>       dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class IntDictionary            : PatchOdyssey.Collections.RefDictionary<string, System     .Int32>          { [PatchConstructor, PatchMethod(AggressiveInlining)] public IntDictionary           () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public IntDictionary           (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public IntDictionary           (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public IntDictionary           (System.Collections.Generic.IDictionary<string, System     .Int32>          dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public IntDictionary           (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public IntDictionary           (System.Collections.Generic.IDictionary<string, System     .Int32>          dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class LongDictionary           : PatchOdyssey.Collections.RefDictionary<string, System     .Int64>          { [PatchConstructor, PatchMethod(AggressiveInlining)] public LongDictionary          () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public LongDictionary          (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public LongDictionary          (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public LongDictionary          (System.Collections.Generic.IDictionary<string, System     .Int64>          dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public LongDictionary          (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public LongDictionary          (System.Collections.Generic.IDictionary<string, System     .Int64>          dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class RectDictionary           : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Rect>           { [PatchConstructor, PatchMethod(AggressiveInlining)] public RectDictionary          () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectDictionary          (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectDictionary          (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectDictionary          (System.Collections.Generic.IDictionary<string, UnityEngine.Rect>           dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectDictionary          (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectDictionary          (System.Collections.Generic.IDictionary<string, UnityEngine.Rect>           dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class RectIntDictionary        : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.RectInt>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntDictionary       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntDictionary       (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.RectInt>        dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.RectInt>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class StringDictionary         : PatchOdyssey.Collections.RefDictionary<string, System     .String>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public StringDictionary        () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public StringDictionary        (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public StringDictionary        (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public StringDictionary        (System.Collections.Generic.IDictionary<string, System     .String>         dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public StringDictionary        (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public StringDictionary        (System.Collections.Generic.IDictionary<string, System     .String>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class UIntDictionary           : PatchOdyssey.Collections.RefDictionary<string, System     .UInt32>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntDictionary          () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntDictionary          (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntDictionary          (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntDictionary          (System.Collections.Generic.IDictionary<string, System     .UInt32>         dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntDictionary          (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntDictionary          (System.Collections.Generic.IDictionary<string, System     .UInt32>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class ULongDictionary          : PatchOdyssey.Collections.RefDictionary<string, System     .UInt64>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongDictionary         () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongDictionary         (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongDictionary         (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongDictionary         (System.Collections.Generic.IDictionary<string, System     .UInt64>         dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongDictionary         (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongDictionary         (System.Collections.Generic.IDictionary<string, System     .UInt64>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class Vector2Dictionary        : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Vector2>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2Dictionary       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2Dictionary       (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2Dictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2>        dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2Dictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class Vector2IntDictionary     : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Vector2Int>     { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntDictionary    () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntDictionary    (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntDictionary    (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2Int>     dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntDictionary    (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2Int>     dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class Vector3Dictionary        : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Vector3>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3Dictionary       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3Dictionary       (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3Dictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3>        dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3Dictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class Vector3IntDictionary     : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Vector3Int>     { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntDictionary    () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntDictionary    (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntDictionary    (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3Int>     dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntDictionary    (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3Int>     dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class Vector4Dictionary        : PatchOdyssey.Collections.RefDictionary<string, UnityEngine.Vector4>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4Dictionary       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4Dictionary       (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4Dictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector4>        dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4Dictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4Dictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector4>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }

    public struct RefKeyValuePair<TKey, TValue> /* ⟶ Based on `System.Collections.Generic.KeyValuePair<TKey, TValue>` */ {
      internal     readonly PatchOdyssey.Collections.RefDictionary<TKey, TValue> dictionary;
      internal     readonly uint                                                 index;
      public   ref readonly TKey                                                 Key => ref (this.referenced ? ref this.dictionary.keys[this.index] : ref System.Runtime.InteropServices.MemoryMarshal.GetReference(this.pair).key);
      internal     readonly (TKey key, TValue value)[]                           pair; // ⟶ `System.Collections.Generic.KeyValuePair<TKey, TValue>` but `ref`-errable
      internal     readonly bool                                                 referenced;
      public   ref          TValue                                               Value => ref (this.referenced ? ref this.dictionary.values[this.index] : ref System.Runtime.InteropServices.MemoryMarshal.GetReference(this.pair).value);

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)] internal RefKeyValuePair(PatchOdyssey.Collections.RefDictionary<TKey, TValue> dictionary, uint      index) { this.dictionary = dictionary; this.index = index; this.pair = null!;                this.referenced = true; }
      [PatchConstructor, PatchMethod(AggressiveInlining)] public   RefKeyValuePair(in TKey                                              key,        in TValue value) { this.dictionary = null!;      this.index = 0u;    this.pair = new[] {(key, value)}; this.referenced = false; }

      /* … */
      [PatchMethod(AggressiveInlining)] public          void   Deconstruct(out TKey key, out TValue value) { key = this.Key; value = this.Value; }
      [PatchMethod(AggressiveInlining)] public override string ToString   ()                               => $"[{this.Key}, {this.Value}]";

      [PatchMethod(AggressiveInlining)]
      public static implicit operator System.Collections.Generic.KeyValuePair<TKey, TValue>(in RefKeyValuePair<TKey, TValue> pair) => new(pair.Key, pair.Value);
    }

    [System.Serializable]
    public class RefList<T> : PatchOdyssey.Collections.RefReadOnlyList<T>, System.Collections.Generic.IList<T>, System.Collections.ICollection, System.Collections.IList, System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable, System.ICloneable {
      public new struct Enumerator : System.Collections.Generic.IEnumerator<T> {
        public           ref T                                                  Current => ref this.enumerator.list.GetValue((uint) this.enumerator.index);
        private required PatchOdyssey.Collections.RefReadOnlyList<T>.Enumerator enumerator;
        T                                                                       System.Collections.Generic.IEnumerator<T>.Current => this.Current;
        object                                                                  System.Collections.IEnumerator.Current            => this.Current!;

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
      [PatchConstructor, PatchMethod(AggressiveInlining)] protected RefList(T[]                                         array, uint index, uint length)                                      => System.Array.Copy(array, (int) index, this.Items = PatchOdyssey.Collections.RefReadOnlyList<T>.CreateInstance(this.capacity = RefList<T>.GetCapacity(this.Count = length)), 0, (int) length);

      [PatchConstructor, PatchMethod(AggressiveInlining)]
      public RefList(System.Collections.Generic.IEnumerable<T> enumerable) {
        this.Count = Util.EnumerableCount(enumerable);

        if (0u != this.Count) {
          this.capacity = RefList<T>.GetCapacity(this.Count);
          this.Items    = new T[this.capacity];

          using (System.Collections.Generic.IEnumerator<T> enumerator = enumerable.GetEnumerator()) {
            for (uint index = 0u; enumerator.MoveNext(); ++index)
            this.SetValue(enumerator.Current, index);
          }
        }
      }

      /* … */
      /* REF READ-ONLY PREDICATE */
      [PatchMethod(AggressiveInlining)] public                 void                  Add           (in T                                      element)                                                                                          => this.Insert     (this.Count, element);
      [PatchMethod(AggressiveInlining)] public    ref readonly T                     Append        (in T                                      element)                                                                                          {  this.Insert     (this.Count, element); return ref element; }
      [PatchMethod(AggressiveInlining)] public                 void                  AddRange      (System.Collections.Generic.IEnumerable<T> enumerable)                                                                                       => this.InsertRange(this.Count, enumerable);
      [PatchMethod(AggressiveInlining)] public    new          RefList<T>            AsCopy        ()                                                                                                                                           => new(this);
      [PatchMethod(AggressiveInlining)] public                 void                  Clear         ()                                                                                                                                           { this.capacity = this.Count = 0u; this.Items = System.Array.Empty<T>(); }
      [PatchMethod(AggressiveInlining)] public                 RefList<U>            ConvertAll<U> (PatchOdyssey.RefConverter        <T, U> converter)                                                                                          { RefList<U> list = new(this.Count); for (; list.Count != this.Count; ++list.Count) { list.SetValue(converter(ref this.GetValue(list.Count)), list.Count); } return list; }
      [PatchMethod(AggressiveInlining)] public    new          RefList<U>            ConvertAll<U> (PatchOdyssey.RefReadOnlyConverter<T, U> converter)                                                                                          => base.ConvertAll(converter);
      [PatchMethod(AggressiveInlining)] public    new          RefList<U>            ConvertAll<U> (System.Converter                 <T, U> converter)                                                                                          => base.ConvertAll(converter);
      [PatchMethod(AggressiveInlining)] public                 uint                  EnsureCapacity(uint                                    capacity, bool precise = false)                                                                     { if (capacity > this.capacity) { T[] list = this.Items; System.Array.Copy(list, 0, this.Items = RefReadOnlyList<T>.CreateInstance(this.capacity = !precise ? RefList<T>.GetCapacity(capacity) : capacity), 0, (int) this.Count); } return this.capacity; }
      [PatchMethod(AggressiveInlining)] public                 bool                  Exists        (PatchOdyssey.RefPredicate        <T>    predicate)                                                                                          { for (uint index = 0u; index != this.Count; ++index) {                                           if (predicate(ref this.GetValue(index))) return true; }    return false; }
      [PatchMethod(AggressiveInlining)] public    new          bool                  Exists        (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                          => base.Exists(predicate);
      [PatchMethod(AggressiveInlining)] public    new          bool                  Exists        (System.Predicate                 <T>    predicate)                                                                                          => base.Exists(predicate);
      [PatchMethod(AggressiveInlining)] public                 T?                    Find          (PatchOdyssey.RefPredicate        <T>    predicate)                                                                                          { for (uint index = 0u; index != this.Count; ++index) { ref T element = ref this.GetValue(index); if (predicate(ref element))              return element; } return default; }
      [PatchMethod(AggressiveInlining)] public    new          T?                    Find          (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                          => base.Find(predicate);
      [PatchMethod(AggressiveInlining)] public    new          T?                    Find          (System.Predicate                 <T>    predicate)                                                                                          => base.Find(predicate);
      [PatchMethod(AggressiveInlining)] public                 RefList<T>            FindAll       (PatchOdyssey.RefPredicate        <T>    predicate)                                                                                          { RefList<T> list = new(this.Count); for (uint index = 0u; index != this.Count; ++index) { ref T element = ref this.GetValue(index); if (predicate(ref element)) list.SetValue(in element, list.Count++); } list.TrimExcess(); return list; }
      [PatchMethod(AggressiveInlining)] public    new          RefList<T>            FindAll       (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                          => base.FindAll  (predicate);
      [PatchMethod(AggressiveInlining)] public    new          RefList<T>            FindAll       (System.Predicate                 <T>    predicate)                                                                                          => base.FindAll  (predicate);
      [PatchMethod(AggressiveInlining)] public                 int                   FindIndex     (PatchOdyssey.RefPredicate        <T>    predicate)                                                                                          => this.FindIndex(0u, this.Count, predicate);
      [PatchMethod(AggressiveInlining)] public    new          int                   FindIndex     (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                          => base.FindIndex(predicate);
      [PatchMethod(AggressiveInlining)] public    new          int                   FindIndex     (System.Predicate                 <T>    predicate)                                                                                          => base.FindIndex(predicate);
      [PatchMethod(AggressiveInlining)] public                 int                   FindIndex     (uint                                    index, PatchOdyssey.RefPredicate        <T> predicate)                                              { for                                                       (; index < this.Count; ++index) { if (predicate(ref this.GetValue(index))) return (int) index; } return -1; }
      [PatchMethod(AggressiveInlining)] public    new          int                   FindIndex     (uint                                    index, PatchOdyssey.RefReadOnlyPredicate<T> predicate)                                              => base.FindIndex(index, predicate);
      [PatchMethod(AggressiveInlining)] public    new          int                   FindIndex     (uint                                    index, System.Predicate                 <T> predicate)                                              => base.FindIndex(index, predicate);
      [PatchMethod(AggressiveInlining)] public                 int                   FindIndex     (uint                                    index, uint                                 length, PatchOdyssey.RefPredicate        <T> predicate) { for (uint end = System.Math.Min(this.Count, index + length); index < end;        ++index) { if (predicate(ref this.GetValue(index))) return (int) index; } return -1; }
      [PatchMethod(AggressiveInlining)] public    new          int                   FindIndex     (uint                                    index, uint                                 length, PatchOdyssey.RefReadOnlyPredicate<T> predicate) => base.FindIndex(index, length, predicate);
      [PatchMethod(AggressiveInlining)] public    new          int                   FindIndex     (uint                                    index, uint                                 length, System.Predicate                 <T> predicate) => base.FindIndex(index, length, predicate);
      [PatchMethod(AggressiveInlining)] public                 int                   FindLastIndex (PatchOdyssey.RefPredicate        <T>    predicate)                                                                                          => this.FindLastIndex(0u, this.Count, predicate);
      [PatchMethod(AggressiveInlining)] public    new          int                   FindLastIndex (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                          => base.FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining)] public    new          int                   FindLastIndex (System.Predicate                 <T>    predicate)                                                                                          => base.FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining)] public                 int                   FindLastIndex (uint                                    index, PatchOdyssey.RefPredicate        <T> predicate)                                              { for (uint end = this.Count;                                  end-- > index; ) { if (predicate(ref this.GetValue(end))) return (int) end; } return -1; }
      [PatchMethod(AggressiveInlining)] public    new          int                   FindLastIndex (uint                                    index, PatchOdyssey.RefReadOnlyPredicate<T> predicate)                                              => base.FindLastIndex(index, predicate);
      [PatchMethod(AggressiveInlining)] public    new          int                   FindLastIndex (uint                                    index, System.Predicate                 <T> predicate)                                              => base.FindLastIndex(index, predicate);
      [PatchMethod(AggressiveInlining)] public                 int                   FindLastIndex (uint                                    index, uint                                 length, PatchOdyssey.RefPredicate        <T> predicate) { for (uint end = System.Math.Min(this.Count, index + length); end-- > index; ) { if (predicate(ref this.GetValue(end))) return (int) end; } return -1; }
      [PatchMethod(AggressiveInlining)] public    new          int                   FindLastIndex (uint                                    index, uint                                 length, PatchOdyssey.RefReadOnlyPredicate<T> predicate) => base.FindLastIndex(index, length, predicate);
      [PatchMethod(AggressiveInlining)] public    new          int                   FindLastIndex (uint                                    index, uint                                 length, System.Predicate                 <T> predicate) => base.FindLastIndex(index, length, predicate);
      [PatchMethod(AggressiveInlining)] public                 void                  ForEach       (PatchOdyssey.RefAction        <T>       action)                                                                                             { for (uint index = 0u; index != this.Count; ++index) action(ref this.GetValue(index)); }
      [PatchMethod(AggressiveInlining)] public    new          void                  ForEach       (PatchOdyssey.RefReadOnlyAction<T>       action)                                                                                             => base.ForEach(action);
      [PatchMethod(AggressiveInlining)] public    new          void                  ForEach       (System.Action                 <T>       action)                                                                                             => base.ForEach(action);
      [PatchMethod(AggressiveInlining)] protected static       uint                  GetCapacity   (uint                                    count)                                                                                              { if (0u != count) { count = System.Math.Max(count, 4u) - 1u; count |= count >> 1; count |= count >> 2; count |= count >> 4; count |= count >> 8; count |= count >> 16; return count + 1u; } return 0u; }
      [PatchMethod(AggressiveInlining)] public    new          RefList<T>.Enumerator GetEnumerator ()                                                                                                                                           => new(this);
      [PatchMethod(AggressiveInlining)] public    new          RefList<T>            GetRange      (uint index, uint length)                                                                                                                    => new(this.Items, index, length);

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

        for (uint index = begin + ((end - begin) / 2u); begin != index--; )
        Build(this, end, index, comparison);

        for (uint index = end; begin != index--; ) {
          (this.GetValue(begin), this.GetValue(index)) = (this.GetValue(index), this.GetValue(begin));
          Build(this, index, begin, comparison);
        }
      }

      [PatchMethod(AggressiveInlining)]
      public void Insert(uint index, in T element) {
        if (this.capacity == this.Count)
          this.EnsureCapacity(this.Count + 1u);

        if (index <= this.Count) {
          for (uint subindex = this.Count; index != subindex--; )
            this.SetValue(in this.GetValue(subindex + 0u), subindex + 1u);

          this.SetValue(in element, index);
        }

        ++this.Count;
      }

      private void InsertionSort(uint begin, uint end, PatchOdyssey.RefComparison<T> comparison) /* ⟶ See `https://web.archive.org/web/20240629152053/https://www.geeksforgeeks.org/insertion-sort-algorithm/` */ {
        for (uint index = begin + 1u; end != index; ++index) {
          T   element  = this.GetValue(index);
          int subindex = ((int) index) - 1;

          // …
          for (; subindex >= 0 && comparison(ref this.GetValue((uint) subindex), ref element) > 0; --subindex)
            this.SetValue(in this.GetValue((uint) subindex + 0u), (uint) subindex + 1u);

          this.SetValue(in element, (uint) ++subindex);
        }
      }

      public void InsertRange(uint index, System.Collections.Generic.IEnumerable<T> enumerable) {
        uint count = Util.EnumerableCount(enumerable);

        // …
        if (this.capacity < this.Count + count)
        this.EnsureCapacity(this.Count + count);

        if (index < this.Count) {
          for (uint subindex = this.Count - index; 0u != subindex--; )
            this.SetValue(in this.GetValue(index + subindex), count + index + subindex);

          this.Count += count;
        } else /* if (index > this.Count) */ (count, this.Count) = (count - System.Math.Min(count, index - this.Count), this.Count + count);

        using (System.Collections.Generic.IEnumerator<T> enumerator = enumerable.GetEnumerator())
        while (0u != count--) {
          enumerator.MoveNext();
          this.SetValue(enumerator.Current, index++);
        }
      }

      [PatchMethod(AggressiveInlining)]
      public ref readonly T Prepend(in T element) {
        this.Insert(0, in element);
        return ref element;
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

      [PatchMethod(AggressiveInlining)] public      bool       Remove                                             (in T                                 element)                                                                     { int index = base.IndexOf(element); if (index != -1) { this.RemoveAt((uint) index); return true; } return false; }
      [PatchMethod(AggressiveInlining)] public      uint       RemoveAll                                          (PatchOdyssey.RefPredicate        <T> predicate)                                                                   { T[] array = (T[]) base.Items.Clone(); uint length = 0u; for (uint index = 0u; index != base.Count; ++index) { ref T element = ref base.GetValue(index); if (!predicate(ref element)) array[length++] = element; } length = base.Count - length; base.Count -= length; this.Items = array; return length; }
      [PatchMethod(AggressiveInlining)] public      uint       RemoveAll                                          (PatchOdyssey.RefReadOnlyPredicate<T> predicate)                                                                   => this.RemoveAll((ref T element) => predicate(in element));
      [PatchMethod(AggressiveInlining)] public      uint       RemoveAll                                          (System.Predicate                 <T> predicate)                                                                   => this.RemoveAll((ref T element) => predicate   (element));
      [PatchMethod(AggressiveInlining)] public      void       RemoveAt                                           (uint                                 index)                                                                       { while                                (++index < base.Count)                      { base.SetValue(in base.GetValue(index - 0u), index - 1u); } base.Count -= index == base.Count ? 1u : 0u; }
      [PatchMethod(AggressiveInlining)] public      void       RemoveRange                                        (uint                                 index, uint length)                                                          { for (uint subindex = index + length; subindex < base.Count; ++index, ++subindex) { base.SetValue(in base.GetValue(subindex),   index); }      base.Count -= index <= base.Count ? base.Count - index : 0u; }
      [PatchMethod(AggressiveInlining)] public      void       Reverse                                            ()                                                                                                                 => this      .Reverse(0u,          base.Count);
      [PatchMethod(AggressiveInlining)] public      void       Reverse                                            (uint index, uint length)                                                                                          => base.Items.Reverse((int) index, (int) length);
      [PatchMethod(AggressiveInlining)] public  new RefList<T> Slice                                              (uint index, uint length)                                                                                          => new(base.Items, index, length);
      [PatchMethod(AggressiveInlining)] public      void       Sort                                               ()                                                                                                                 => this.Sort(0u, base.Count, System.Collections.Generic.Comparer<T>.Default);
      [PatchMethod(AggressiveInlining)] public      void       Sort                                               (PatchOdyssey.RefComparison          <T>  comparison)                                                              => this.Sort(0u, base.Count, comparison);
      [PatchMethod(AggressiveInlining)] public      void       Sort                                               (PatchOdyssey.RefReadOnlyComparison  <T>  comparison)                                                              => this.Sort((ref T a, ref T b) => comparison(in a, in b));
      [PatchMethod(AggressiveInlining)] public      void       Sort                                               (System.Collections.Generic.IComparer<T>? comparer)                                                                => this.Sort(0u, base.Count, comparer);
      [PatchMethod(AggressiveInlining)] public      void       Sort                                               (System.Comparison                   <T>  comparison)                                                              => this.Sort((ref T a, ref T b) => comparison(a,    b));
      [PatchMethod(AggressiveInlining)] private     void       Sort                                               (uint                                     begin, uint end,    PatchOdyssey.RefComparison          <T>  comparison) { if (base.Count <= 16u) this.InsertionSort(begin, end, comparison); else if (base.Count > System.Math.Log(base.Count) * 2.0) this.HeapSort(begin, end, comparison); else this.QuickSort(begin, end - 1u, comparison); }
      [PatchMethod(AggressiveInlining)] public      void       Sort                                               (uint                                     index, uint length, System.Collections.Generic.IComparer<T>? comparer)   { comparer ??= System.Collections.Generic.Comparer<T>.Default; this.Sort(index, System.Math.Min(base.Count, index + length), (ref T a, ref T b) => comparer!.Compare(a, b)); }
      [PatchMethod(AggressiveInlining)] public      void       TrimExcess                                         ()                                                                                                                 => this.TrimExcess(base.Count);
      [PatchMethod(AggressiveInlining)] public      void       TrimExcess                                         (uint                                 capacity)                                                                    { capacity = RefList<T>.GetCapacity(capacity); if (capacity < this.capacity && capacity >= base.Count) System.Array.Resize(ref base.Items, (int) (this.capacity = capacity)); }
      [PatchMethod(AggressiveInlining)] public      bool       TrueForAll                                         (PatchOdyssey.RefPredicate        <T> predicate)                                                                   { for (uint index = 0u; index != base.Count; ++index) { if (!predicate(ref base.GetValue(index))) return false; } return true; }
      [PatchMethod(AggressiveInlining)] public  new bool       TrueForAll                                         (PatchOdyssey.RefReadOnlyPredicate<T> predicate)                                                                   => base      .TrueForAll (predicate);
      [PatchMethod(AggressiveInlining)] public  new bool       TrueForAll                                         (System.Predicate                 <T> predicate)                                                                   => base      .TrueForAll (predicate);
      [PatchMethod(AggressiveInlining)] void                   System.Collections.Generic.ICollection<T>.Add      (T                                    element)                                                                     => this      .Add        (element);
      [PatchMethod(AggressiveInlining)] void                   System.Collections.Generic.ICollection<T>.Clear    ()                                                                                                                 => this      .Clear      ();
      [PatchMethod(AggressiveInlining)] bool                   System.Collections.Generic.ICollection<T>.Contains (T            element)                                                                                             => base      .Contains   (element);
      [PatchMethod(AggressiveInlining)] void                   System.Collections.Generic.ICollection<T>.CopyTo   (T[]          array, int index)                                                                                    => base      .CopyTo     (array, (uint) index);
      [PatchMethod(AggressiveInlining)] bool                   System.Collections.Generic.ICollection<T>.Remove   (T            element)                                                                                             => this      .Remove     (element);
      [PatchMethod(AggressiveInlining)] int                    System.Collections.Generic.IList<T>.IndexOf        (T            element)                                                                                             => base      .IndexOf    (element);
      [PatchMethod(AggressiveInlining)] void                   System.Collections.Generic.IList<T>.Insert         (int          index, T element)                                                                                    => this      .Insert     ((uint) index, element);
      [PatchMethod(AggressiveInlining)] void                   System.Collections.Generic.IList<T>.RemoveAt       (int          index)                                                                                               => this      .RemoveAt   ((uint) index);
      [PatchMethod(AggressiveInlining)] void                   System.Collections.ICollection.CopyTo              (System.Array array, int index)                                                                                    => base      .CopyTo     ((T[]) array, (uint) index);
      [PatchMethod(AggressiveInlining)] int                    System.Collections.IList.Add                       (object?      element)                                                                                             {  this      .Add        ((T) element!); return (int) this.Count; }
      [PatchMethod(AggressiveInlining)] void                   System.Collections.IList.Clear                     ()                                                                                                                 => this      .Clear      ();
      [PatchMethod(AggressiveInlining)] bool                   System.Collections.IList.Contains                  (object? element)                                                                                                  => base.     .Contains   ((T) element!);
      [PatchMethod(AggressiveInlining)] int                    System.Collections.IList.IndexOf                   (object? element)                                                                                                  => base.     .IndexOf    ((T) element!);
      [PatchMethod(AggressiveInlining)] void                   System.Collections.IList.Insert                    (int     index, object? element)                                                                                   => this      .Insert     ((uint) index, (T) element!);
      [PatchMethod(AggressiveInlining)] void                   System.Collections.IList.Remove                    (object? element)                                                                                                  => this      .Remove     ((T) element!);
      [PatchMethod(AggressiveInlining)] void                   System.Collections.IList.RemoveAt                  (int     index)                                                                                                    => this      .RemoveAt   ((uint) index);
      [PatchMethod(AggressiveInlining)] int                    System.Collections.IStructuralComparable.CompareTo (object?                              value, System.Collections.IComparer         comparer)                        => comparer  .Compare    (base, value);
      [PatchMethod(AggressiveInlining)] bool                   System.Collections.IStructuralEquatable.Equals     (object?                              value, System.Collections.IEqualityComparer comparer)                        => comparer  .Equals     (base, value);
      [PatchMethod(AggressiveInlining)] int                    System.Collections.IStructuralEquatable.GetHashCode(System.Collections.IEqualityComparer comparer)                                                                    => comparer  .GetHashCode(base);
      [PatchMethod(AggressiveInlining)] object                 System.ICloneable.Clone                            ()                                                                                                                 => base      .Clone      ();

      /* … */
      public new ref T          this                                    [uint         index] => ref this.GetValue(index);
      public new     RefList<T> this                                    [System.Range range] => new(this.Items[range]);
      T                         System.Collections.Generic.IList<T>.this[int          index] { get => this[(uint) index]; set => this[(uint) index] = value; }
      object?                   System.Collections.IList.this           [int          index] { get => this[(uint) index]; set => this[(uint) index] = (T) value!; }
    }
      [System.Serializable] public class AnimationCurveList : PatchOdyssey.Collections.RefList<UnityEngine.AnimationCurve> { [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveList() : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveList(uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveList(System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class BooleanList        : PatchOdyssey.Collections.RefList<System     .Boolean>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanList       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanList       (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanList       (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class BoundsList         : PatchOdyssey.Collections.RefList<UnityEngine.Bounds>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsList        () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsList        (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsList        (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class BoundsIntList      : PatchOdyssey.Collections.RefList<UnityEngine.BoundsInt>      { [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntList     () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntList     (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntList     (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class ColorList          : PatchOdyssey.Collections.RefList<UnityEngine.Color>          { [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorList         () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorList         (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorList         (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class DoubleList         : PatchOdyssey.Collections.RefList<System     .Double>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleList        () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleList        (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleList        (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class FloatList          : PatchOdyssey.Collections.RefList<System     .Single>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatList         () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatList         (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatList         (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class GameObjectList     : PatchOdyssey.Collections.RefList<UnityEngine.GameObject>     { [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectList    () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectList    (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectList    (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class GradientList       : PatchOdyssey.Collections.RefList<UnityEngine.Gradient>       { [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientList      () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientList      (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientList      (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class IntList            : PatchOdyssey.Collections.RefList<System     .Int32>          { [PatchConstructor, PatchMethod(AggressiveInlining)] public IntList           () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public IntList           (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public IntList           (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class LongList           : PatchOdyssey.Collections.RefList<System     .Int64>          { [PatchConstructor, PatchMethod(AggressiveInlining)] public LongList          () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public LongList          (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public LongList          (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class RectList           : PatchOdyssey.Collections.RefList<UnityEngine.Rect>           { [PatchConstructor, PatchMethod(AggressiveInlining)] public RectList          () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectList          (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectList          (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class RectIntList        : PatchOdyssey.Collections.RefList<UnityEngine.RectInt>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntList       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntList       (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntList       (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class StringList         : PatchOdyssey.Collections.RefList<System     .String>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public StringList        () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public StringList        (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public StringList        (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class UIntList           : PatchOdyssey.Collections.RefList<System     .UInt32>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntList          () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntList          (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntList          (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class ULongList          : PatchOdyssey.Collections.RefList<System     .UInt64>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongList         () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongList         (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongList         (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class Vector2List        : PatchOdyssey.Collections.RefList<UnityEngine.Vector2>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2List       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2List       (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2List       (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class Vector2IntList     : PatchOdyssey.Collections.RefList<UnityEngine.Vector2Int>     { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntList    () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntList    (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntList    (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class Vector3List        : PatchOdyssey.Collections.RefList<UnityEngine.Vector3>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3List       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3List       (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3List       (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class Vector3IntList     : PatchOdyssey.Collections.RefList<UnityEngine.Vector3Int>     { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntList    () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntList    (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntList    (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class Vector4List        : PatchOdyssey.Collections.RefList<UnityEngine.Vector4>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4List       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4List       (uint capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4List       (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }

    public abstract class RefReadOnlyComparer<T> : System.Collections.Generic.Comparer<T>;

    [System.Serializable]
    public class RefReadOnlyDictionary<TKey, TValue> : PatchOdyssey.Collections.RefDictionary<TKey, TValue>, System.Collections.Generic.IDictionary<TKey, TValue>, System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>, System.Collections.IDictionary /* ⟶ Retroactively designed after `RefDictionary<…>`’s implementation */ {
      public struct Enumerator : System.Collections.Generic.IEnumerator<PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>> /* ⟶ Based on `System.Collections.Generic.Dictionary<TKey, TValue>.KeyCollection.Enumerator` */ {
        public  readonly ref                                           PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>   Current { [PatchMethod(AggressiveInlining)] get { this.current = new[] {new PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>(this.enumerator.Current)}; return ref Util.ArrayReference(this.current); } }
        public                                                         PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>[] current;
        private readonly                                               RefDictionary<TKey, TValue>.Enumerator                           enumerator;
        PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>                                                                  System.Collections.Generic.IEnumerator<PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>>.Current => this.Current;
        object                                                                                                                          System.Collections.IEnumerator.Current                                                                         => this.Current!;

        /* … */
        [PatchConstructor, PatchMethod(AggressiveInlining)]
        internal Enumerator(RefReadOnlyDictionary<TKey, TValue> dictionary) => this.enumerator = ((PatchOdyssey.Collections.RefDictionary<TKey, TValue>) dictionary).GetEnumerator();

        /* … */
        [PatchMethod(AggressiveInlining)] public void Dispose                                () => this.enumerator.Dispose ();
        [PatchMethod(AggressiveInlining)] public bool MoveNext                               () => this.enumerator.MoveNext();
        [PatchMethod(AggressiveInlining)] public void Reset                                  () => this.enumerator.Reset   ();
        [PatchMethod(AggressiveInlining)] bool        System.Collections.IEnumerator.MoveNext() => this           .MoveNext();
        [PatchMethod(AggressiveInlining)] void        System.Collections.IEnumerator.Reset   () => this           .Reset   ();
        [PatchMethod(AggressiveInlining)] void        System.IDisposable.Dispose             () => this           .Dispose ();
      }

      public sealed class KeyCollection : PatchOdyssey.Collections.RefDictionary<TKey, TValue>.KeyCollection {
        // ⟶ Aliases `PatchOdyssey.Collections.RefDictionary<TKey, TValue>.KeyCollection.Enumerator` for its `struct Enumerator` subtype
        [PatchMethod(AggressiveInlining)]
        public KeyCollection(RefReadOnlyDictionary<TKey, TValue> dictionary) : base((PatchOdyssey.Collections.RefDictionary<TKey, TValue>) dictionary) {}
      }

      public sealed class ValueCollection : PatchOdyssey.Collections.RefDictionary<TKey, TValue>.ValueCollection {
        public struct Enumerator : System.Collections.Generic.IEnumerator<TValue> /* ⟶ Based on `System.Collections.Generic.Dictionary<TKey, TValue>.ValueCollection.Enumerator` */ {
          public  ref readonly TValue                                                 Current => ref this.enumerator.Current.Value;
          private     readonly RefDictionary<TKey, TValue>.ValueCollection.Enumerator enumerator;
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
        public ValueCollection.Enumerator GetEnumerator() => new(base.dictionary);
      }

      public static readonly RefReadOnlyDictionary<TKey, TValue>            Empty                                                                                                    =  new();
      public new             System.Collections.Generic.IEnumerable<TKey>   Keys                                                                                                     => base.Keys;
      public new             System.Collections.Generic.IEnumerable<TValue> Values                                                                                                   => base.Values;
      bool                                                                  System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.IsReadOnly => true;
      int                                                                   System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Count      => ((int) this.Count);
      System.Collections.Generic.ICollection<TKey>                          System.Collections.Generic.IDictionary<TKey, TValue>.Keys                                                => this.Keys;
      System.Collections.Generic.ICollection<TValue>                        System.Collections.Generic.IDictionary<TKey, TValue>.Values                                              => this.Values;
      int                                                                   System.Collections.Generic.IReadOnlyCollection<TKey, TValue>.Count                                       => ((int) this.Count);
      System.Collections.Generic.IEnumerable<TKey>                          System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.Keys                                        => this.Keys;
      System.Collections.Generic.IEnumerable<TValue>                        System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.Values                                      => this.Values;
      bool                                                                  System.Collections.IDictionary.IsFixedSize                                                               => false;
      bool                                                                  System.Collections.IDictionary.IsReadOnly                                                                => true;
      System.Collections.ICollection                                        System.Collections.IDictionary.Keys                                                                      => this.Keys;
      System.Collections.ICollection                                        System.Collections.IDictionary.Values                                                                    => this.Values;

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)] public RefReadOnlyDictionary()                                                                                                                                                                             : base()                     {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public RefReadOnlyDictionary(System.Collections.Generic.IEqualityComparer<TKey>                                                   comparer)                                                                : base(comparer)             {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public RefReadOnlyDictionary(PatchOdyssey.Collections.RefDictionary      <TKey, TValue>                                           dictionary)                                                              : base(dictionary)           {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public RefReadOnlyDictionary(System.Collections.Generic.IEnumerable      <PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>> enumerable)                                                              : base(enumerable)           {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public RefReadOnlyDictionary(System.Collections.Generic.IEnumerable      <System.Collections.Generic.KeyValuePair <TKey, TValue>> enumerable)                                                              : base(enumerable)           {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public RefReadOnlyDictionary(PatchOdyssey.Collections.RefDictionary      <TKey, TValue>                                           dictionary, System.Collections.Generic.IEqualityComparer<TKey> comparer) : base(dictionary, comparer) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public RefReadOnlyDictionary(System.Collections.Generic.IEnumerable      <PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>> enumerable, System.Collections.Generic.IEqualityComparer<TKey> comparer) : base(enumerable, comparer) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public RefReadOnlyDictionary(System.Collections.Generic.IEnumerable      <System.Collections.Generic.KeyValuePair <TKey, TValue>> enumerable, System.Collections.Generic.IEqualityComparer<TKey> comparer) : base(enumerable, comparer) {}

      /* … */
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public new void                                           Add                                                                     (in PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue> element)                                        => this.Add(element.Key, element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public new void                                           Add                                                                     (in System.Collections.Generic.KeyValuePair <TKey, TValue> element)                                        => this.Add(element.Key, element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public new void                                           Add                                                                     (in TKey                                                   key, in TValue value)                           { throw new System.NotSupportedException("Dictionary is read-only"); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public new void                                           AddRange                                                                (System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>> enumerable) { throw new System.NotSupportedException("Dictionary is read-only"); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public new void                                           Clear                                                                   ()                                                                                                         { throw new System.NotSupportedException("Dictionary is read-only"); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public new RefReadOnlyDictionary<TKey, TValue>.Enumerator GetEnumerator                                                           ()                                                                                                         => new(this); // ⟶ `System.Collections.Generic.IEnumerable<PatchOdyssey.Collections.RefReadOnlyKeyValuePair<TKey, TValue>>`
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public new bool                                           Remove                                                                  (in TKey                                                   key)                                            { throw new System.NotSupportedException("Dictionary is read-only"); return false; }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public new bool                                           Remove                                                                  (in PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue> element)                                        { throw new System.NotSupportedException("Dictionary is read-only"); return false; }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public new bool                                           Remove                                                                  (in System.Collections.Generic.KeyValuePair <TKey, TValue> element)                                        { throw new System.NotSupportedException("Dictionary is read-only"); return false; }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public new bool                                           TryAppend                                                               (in PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue> element)                                        => this.TryAdd(in element.Key, in element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public new bool                                           TryAppend                                                               (in System.Collections.Generic.KeyValuePair <TKey, TValue> element)                                        => this.TryAdd(in element.Key, in element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public new bool                                           TryAppend                                                               (in TKey                                                   key, in TValue value)                           { throw new System.NotSupportedException("Dictionary is read-only"); }
      [PatchMethod(AggressiveInlining), PatchResolution(1)] public new bool                                           TryAdd                                                                  (in PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue> element)                                        => this.TryAdd(in element.Key, in element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public new bool                                           TryAdd                                                                  (in System.Collections.Generic.KeyValuePair <TKey, TValue> element)                                        => this.TryAdd(in element.Key, in element.Value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] public new bool                                           TryAdd                                                                  (in TKey                                                   key, in TValue value)                           { throw new System.NotSupportedException("Dictionary is read-only"); }
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                      System.Collections.Generic.IDictionary<TKey, TValue>.Add                (TKey                                                      key, TValue    value)                           => this.Add        (in key, in value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                      System.Collections.Generic.IDictionary<TKey, TValue>.ContainsKey        (TKey                                                      key)                                            => this.ContainsKey(in key);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                      System.Collections.Generic.IDictionary<TKey, TValue>.Remove             (TKey                                                      key)                                            => this.Remove     (in key);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                      System.Collections.Generic.IDictionary<TKey, TValue>.TryGetValue        (TKey                                                      key, out TValue value)                          => this.TryGetValue(in key, out value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                      System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey                                                      key)                                            => this.ContainsKey(in key);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                      System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey                                                      key,   out TValue value)                        => this.TryGetValue(in key, out value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                      System.Collections.ICollection.CopyTo                                   (System.Array                                              array, int        index)                        => ((PatchOdyssey.Collections.RefDictionary<TKey, TValue>) this).CopyTo(array, index);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                      System.Collections.IDictionary.Add                                      (object                                                    key,   object?    value)                        => this.Add          ((TKey) key, (TValue) value);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                      System.Collections.IDictionary.Clear                                    ()                                                                                                         => this.Clear        ();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] bool                                                      System.Collections.IDictionary.Contains                                 (object key)                                                                                               => this.Contains     ((TKey) key);
      [PatchMethod(AggressiveInlining), PatchResolution(0)] System.Collections.IDictionaryEnumerator                  System.Collections.IDictionary.GetEnumerator                            ()                                                                                                         => this.GetEnumerator();
      [PatchMethod(AggressiveInlining), PatchResolution(0)] void                                                      System.Collections.IDictionary.Remove                                   (object key)                                                                                               => this.Remove       ((TKey) key);

      TValue System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.this[TKey key] { [PatchMethod(AggressiveInlining)] get {
        int index = this.FindIndex(in key);

        // …
        if (index == -1)
          throw new System.Collections.Generic.KeyNotFoundException(key?.ToString() ?? string.Empty);

        return this.Values[(uint) index];
      } }
    }
      [System.Serializable] public class AnimationCurveReadOnlyDictionary : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.AnimationCurve> { [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveReadOnlyDictionary(int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveReadOnlyDictionary(System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveReadOnlyDictionary(System.Collections.Generic.IDictionary<string, UnityEngine.AnimationCurve> dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveReadOnlyDictionary(int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveReadOnlyDictionary(System.Collections.Generic.IDictionary<string, UnityEngine.AnimationCurve> dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class BooleanReadOnlyDictionary        : PatchOdyssey.Collections.RefReadOnlyDictionary<string, System     .Boolean>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanReadOnlyDictionary       (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanReadOnlyDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, System     .Boolean>        dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanReadOnlyDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, System     .Boolean>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class BoundsReadOnlyDictionary         : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.Bounds>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsReadOnlyDictionary        (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsReadOnlyDictionary        (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsReadOnlyDictionary        (System.Collections.Generic.IDictionary<string, UnityEngine.Bounds>         dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsReadOnlyDictionary        (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsReadOnlyDictionary        (System.Collections.Generic.IDictionary<string, UnityEngine.Bounds>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class BoundsIntReadOnlyDictionary      : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.BoundsInt>      { [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntReadOnlyDictionary     (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntReadOnlyDictionary     (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntReadOnlyDictionary     (System.Collections.Generic.IDictionary<string, UnityEngine.BoundsInt>      dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntReadOnlyDictionary     (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntReadOnlyDictionary     (System.Collections.Generic.IDictionary<string, UnityEngine.BoundsInt>      dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class ColorReadOnlyDictionary          : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.Color>          { [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorReadOnlyDictionary         (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorReadOnlyDictionary         (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorReadOnlyDictionary         (System.Collections.Generic.IDictionary<string, UnityEngine.Color>          dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorReadOnlyDictionary         (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorReadOnlyDictionary         (System.Collections.Generic.IDictionary<string, UnityEngine.Color>          dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class DoubleReadOnlyDictionary         : PatchOdyssey.Collections.RefReadOnlyDictionary<string, System     .Double>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleReadOnlyDictionary        (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleReadOnlyDictionary        (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleReadOnlyDictionary        (System.Collections.Generic.IDictionary<string, System     .Double>         dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleReadOnlyDictionary        (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleReadOnlyDictionary        (System.Collections.Generic.IDictionary<string, System     .Double>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class FloatReadOnlyDictionary          : PatchOdyssey.Collections.RefReadOnlyDictionary<string, System     .Single>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatReadOnlyDictionary         (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatReadOnlyDictionary         (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatReadOnlyDictionary         (System.Collections.Generic.IDictionary<string, System     .Single>         dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatReadOnlyDictionary         (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatReadOnlyDictionary         (System.Collections.Generic.IDictionary<string, System     .Single>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class GameObjectReadOnlyDictionary     : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.GameObject>     { [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectReadOnlyDictionary    (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectReadOnlyDictionary    (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectReadOnlyDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.GameObject>     dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectReadOnlyDictionary    (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectReadOnlyDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.GameObject>     dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class GradientReadOnlyDictionary       : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.Gradient>       { [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientReadOnlyDictionary      (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientReadOnlyDictionary      (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientReadOnlyDictionary      (System.Collections.Generic.IDictionary<string, UnityEngine.Gradient>       dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientReadOnlyDictionary      (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientReadOnlyDictionary      (System.Collections.Generic.IDictionary<string, UnityEngine.Gradient>       dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class IntReadOnlyDictionary            : PatchOdyssey.Collections.RefReadOnlyDictionary<string, System     .Int32>          { [PatchConstructor, PatchMethod(AggressiveInlining)] public IntReadOnlyDictionary           (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public IntReadOnlyDictionary           (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public IntReadOnlyDictionary           (System.Collections.Generic.IDictionary<string, System     .Int32>          dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public IntReadOnlyDictionary           (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public IntReadOnlyDictionary           (System.Collections.Generic.IDictionary<string, System     .Int32>          dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class LongReadOnlyDictionary           : PatchOdyssey.Collections.RefReadOnlyDictionary<string, System     .Int64>          { [PatchConstructor, PatchMethod(AggressiveInlining)] public LongReadOnlyDictionary          (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public LongReadOnlyDictionary          (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public LongReadOnlyDictionary          (System.Collections.Generic.IDictionary<string, System     .Int64>          dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public LongReadOnlyDictionary          (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public LongReadOnlyDictionary          (System.Collections.Generic.IDictionary<string, System     .Int64>          dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class RectReadOnlyDictionary           : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.Rect>           { [PatchConstructor, PatchMethod(AggressiveInlining)] public RectReadOnlyDictionary          (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectReadOnlyDictionary          (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectReadOnlyDictionary          (System.Collections.Generic.IDictionary<string, UnityEngine.Rect>           dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectReadOnlyDictionary          (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectReadOnlyDictionary          (System.Collections.Generic.IDictionary<string, UnityEngine.Rect>           dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class RectIntReadOnlyDictionary        : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.RectInt>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntReadOnlyDictionary       (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntReadOnlyDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.RectInt>        dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntReadOnlyDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.RectInt>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class StringReadOnlyDictionary         : PatchOdyssey.Collections.RefReadOnlyDictionary<string, System     .String>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public StringReadOnlyDictionary        (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public StringReadOnlyDictionary        (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public StringReadOnlyDictionary        (System.Collections.Generic.IDictionary<string, System     .String>         dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public StringReadOnlyDictionary        (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public StringReadOnlyDictionary        (System.Collections.Generic.IDictionary<string, System     .String>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class UIntReadOnlyDictionary           : PatchOdyssey.Collections.RefReadOnlyDictionary<string, System     .UInt32>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntReadOnlyDictionary          (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntReadOnlyDictionary          (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntReadOnlyDictionary          (System.Collections.Generic.IDictionary<string, System     .UInt32>         dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntReadOnlyDictionary          (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntReadOnlyDictionary          (System.Collections.Generic.IDictionary<string, System     .UInt32>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class ULongReadOnlyDictionary          : PatchOdyssey.Collections.RefReadOnlyDictionary<string, System     .UInt64>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongReadOnlyDictionary         (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongReadOnlyDictionary         (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongReadOnlyDictionary         (System.Collections.Generic.IDictionary<string, System     .UInt64>         dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongReadOnlyDictionary         (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongReadOnlyDictionary         (System.Collections.Generic.IDictionary<string, System     .UInt64>         dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class Vector2ReadOnlyDictionary        : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.Vector2>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2ReadOnlyDictionary       (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2ReadOnlyDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2ReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2>        dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2ReadOnlyDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2ReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class Vector2IntReadOnlyDictionary     : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.Vector2Int>     { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntReadOnlyDictionary    (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntReadOnlyDictionary    (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntReadOnlyDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2Int>     dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntReadOnlyDictionary    (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntReadOnlyDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector2Int>     dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class Vector3ReadOnlyDictionary        : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.Vector3>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3ReadOnlyDictionary       (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3ReadOnlyDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3ReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3>        dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3ReadOnlyDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3ReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class Vector3IntReadOnlyDictionary     : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.Vector3Int>     { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntReadOnlyDictionary    (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntReadOnlyDictionary    (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntReadOnlyDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3Int>     dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntReadOnlyDictionary    (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntReadOnlyDictionary    (System.Collections.Generic.IDictionary<string, UnityEngine.Vector3Int>     dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }
      [System.Serializable] public class Vector4ReadOnlyDictionary        : PatchOdyssey.Collections.RefReadOnlyDictionary<string, UnityEngine.Vector4>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4ReadOnlyDictionary       (int capacity) : base(capacity) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4ReadOnlyDictionary       (System.Collections.Generic.IEqualityComparer<string> comparer) : base(comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4ReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector4>        dictionary) : base(dictionary) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4ReadOnlyDictionary       (int capacity, System.Collections.Generic.IEqualityComparer<string> comparer) : base(capacity, comparer) {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4ReadOnlyDictionary       (System.Collections.Generic.IDictionary<string, UnityEngine.Vector4>        dictionary, System.Collections.Generic.IEqualityComparer<string> comparer) : base(dictionary, comparer) {} }

    internal struct RefReadOnlyKeyValuePair<TKey, TValue> /* ⟶ See `PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue>` */ {
      public   ref readonly TKey                                                   Key => ref this.pair.Key;
      private      readonly PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue> pair;
      public   ref readonly TValue                                                 Value => ref this.pair.Value;

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)]
      internal RefReadOnlyKeyValuePair(in PatchOdyssey.Collections.RefKeyValuePair<TKey, TValue> pair) => this.pair = pair;

      /* … */
      [PatchMethod(AggressiveInlining)] public          void   Deconstruct(out TKey key, out TValue value) => this.pair.Deconstruct(out key, out value);
      [PatchMethod(AggressiveInlining)] public override string ToString   ()                               => this.pair.ToString   ();

      [PatchMethod(AggressiveInlining)]
      public static implicit operator System.Collections.Generic.KeyValuePair<TKey, TValue>(in RefReadOnlyKeyValuePair<TKey, TValue> pair) => new(pair.Key, pair.Value);
    }

    [System.Serializable]
    public class RefReadOnlyList<T> : System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IReadOnlyList<T>, System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable, System.ICloneable /* ⟶ Based on `System.Collections.Generic.List<T>` and `System.Collections.ObjectModel.ReadOnlyCollection<T>` */ {
      public struct Enumerator : System.Collections.Generic.IEnumerator<T> {
        public   ref readonly T                  Current => ref this.list.GetValue((uint) this.index);
        internal required     int                index;
        internal required     RefReadOnlyList<T> list;
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
      public                                                                             uint               Capacity                                                => this.Count;
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] public                   uint               Count { get; internal set; }                            =  0u;
      public                                                             static readonly RefReadOnlyList<T> Empty                                                   = new();
      [UnityEngine.HideInInspector, UnityEngine.SerializeField] internal                 T[]                Items                                                   =  System.Array.Empty<T>();
      int                                                                                                   System.Collections.Generic.IReadOnlyCollection<T>.Count => ((int) this.Count);

      /* … ⟶ Availability of `System.Runtime.InteropServices.CollectionMarshal.AsSpan(…)` would replace `RefReadOnlyList<T>`’s entire purpose */
      [PatchConstructor, PatchMethod(AggressiveInlining)] public             RefReadOnlyList()                                                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] protected          RefReadOnlyList(uint               capacity)                       =>                                  this.Items = RefReadOnlyList<T>.CreateInstance(capacity);
      [PatchConstructor, PatchMethod(AggressiveInlining)] public             RefReadOnlyList(RefReadOnlyList<T> list)                           { this.Count = list.Count;          this.Items = (T[]) list.Items.Clone(); }
      [PatchConstructor, PatchMethod(AggressiveInlining)] protected internal RefReadOnlyList(T[]                array)                          { this.Count = (uint) array.Length; this.Items = array; }
      [PatchConstructor, PatchMethod(AggressiveInlining)] protected          RefReadOnlyList(T[]                array, uint index, uint length) => System.Array.Copy(array, (int) index, this.Items = RefReadOnlyList<T>.CreateInstance(this.Count = length), 0, (int) length);

      [PatchConstructor, PatchMethod(AggressiveInlining)]
      public RefReadOnlyList(System.Collections.Generic.IEnumerable<T> enumerable) {
        this.Count = Util.EnumerableCount(enumerable);

        if (0u != this.Count) {
          this.Items = new T[this.Count];

          using (System.Collections.Generic.IEnumerator<T> enumerator = enumerable.GetEnumerator()) {
            for (uint index = 0u; enumerator.MoveNext(); ++index)
            this.SetValue(enumerator.Current, index);
          }
        }
      }

      /* … */
      [PatchMethod(AggressiveInlining)] public            RefReadOnlyList<T>            AsCopy                                                 ()                                                                                                                                                                  => new(this);
      [PatchMethod(AggressiveInlining)] public            RefReadOnlyList<T>            AsReadOnly                                             ()                                                                                                                                                                  =>     this;
      [PatchMethod(AggressiveInlining)] public            int                           BinarySearch                                           (in T                                    element)                                                                                                                   => this.Items.BinarySearch(0,           (int) this.Count, element);
      [PatchMethod(AggressiveInlining)] public            int                           BinarySearch                                           (in T                                    element, System.Collections.Generic.IComparer<T>? comparer)                                                                => this      .BinarySearch(0u,          this.Count,       element, comparer);
      [PatchMethod(AggressiveInlining)] public            int                           BinarySearch                                           (uint                                    index,   uint                                     length, in T element, System.Collections.Generic.IComparer<T>? comparer) => this.Items.BinarySearch((int) index, (int) length,     element, comparer);
      [PatchMethod(AggressiveInlining)] public            object                        Clone                                                  ()                                                                                                                                                                  => this      .AsCopy      ();
      [PatchMethod(AggressiveInlining)] public            bool                          Contains                                               (in T                                    element)                                                                                                                   => this.IndexOf(element, 0u, this.Count) != -1;
      [PatchMethod(AggressiveInlining)] public            RefReadOnlyList<U>            ConvertAll<U>                                          (PatchOdyssey.RefReadOnlyConverter<T, U> converter)                                                                                                                 { RefReadOnlyList<U> list = new(this.Count); for (; list.Count != this.Count; ++list.Count) { list.SetValue(converter(in this.GetValue(list.Count)), list.Count); } return list; }
      [PatchMethod(AggressiveInlining)] public            RefReadOnlyList<U>            ConvertAll<U>                                          (System.Converter                 <T, U> converter)                                                                                                                 => this.ConvertAll((in T element) => converter(element));
      [PatchMethod(AggressiveInlining)] public            void                          CopyTo                                                 (T[]                                     array)                                                                                                                     => this.CopyTo    (0u, array, 0u,    this.Count);
      [PatchMethod(AggressiveInlining)] public            void                          CopyTo                                                 (T[]                                     array, uint index)                                                                                                         => this.CopyTo    (0u, array, index, this.Count);
      [PatchMethod(AggressiveInlining)] public            void                          CopyTo                                                 (uint                                    index, T[]  array, uint arrayIndex, uint length)                                                                           => System.Array.Copy(this.Items, index, array, arrayIndex, length); // ⟶ Possible over-read
      [PatchMethod(AggressiveInlining)] internal static   T[]                           CreateInstance                                         (uint                                    length)                                                                                                                    => 0u != length ? new T[length] : System.Array.Empty<T>(); // ⟶ Faster with `System.GC.AllocateUninitializedArray<T>(…, false)`
      [PatchMethod(AggressiveInlining)] public            bool                          Exists                                                 (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                                                 { for (uint index = 0u; index != this.Count; ++index) { if (predicate(in this.GetValue(index))) return true; } return false; }
      [PatchMethod(AggressiveInlining)] public            bool                          Exists                                                 (System.Predicate                 <T>    predicate)                                                                                                                 => this.Exists((in T element) => predicate(element));
      [PatchMethod(AggressiveInlining)] public            T?                            Find                                                   (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                                                 { for (uint index = 0u; index != this.Count; ++index) { ref readonly T element = ref this.GetValue(index); if (predicate(in element)) return element; } return default; }
      [PatchMethod(AggressiveInlining)] public            T?                            Find                                                   (System.Predicate                 <T>    predicate)                                                                                                                 => this.Find((in T element) => predicate(element));
      [PatchMethod(AggressiveInlining)] public            RefReadOnlyList<T>            FindAll                                                (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                                                 { RefReadOnlyList<T> list = new(this.Count); for (uint index = 0u; index != this.Count; ++index) { ref readonly T element = ref this.GetValue(index); if (predicate(in element)) list.SetValue(in element, list.Count++); } return list; }
      [PatchMethod(AggressiveInlining)] public            RefReadOnlyList<T>            FindAll                                                (System.Predicate                 <T>    predicate)                                                                                                                 => this.FindAll  ((in T element) => predicate(element));
      [PatchMethod(AggressiveInlining)] public            int                           FindIndex                                              (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                                                 => this.FindIndex(0u, this.Count, predicate);
      [PatchMethod(AggressiveInlining)] public            int                           FindIndex                                              (System.Predicate                 <T>    predicate)                                                                                                                 => this.FindIndex(0u, this.Count, predicate);
      [PatchMethod(AggressiveInlining)] public            int                           FindIndex                                              (uint                                    index, PatchOdyssey.RefReadOnlyPredicate<T> predicate)                                                                     { for (; index < this.Count; ++index) { if (predicate(in this.GetValue(index))) return (int) index; } return -1; }
      [PatchMethod(AggressiveInlining)] public            int                           FindIndex                                              (uint                                    index, System.Predicate                 <T> predicate)                                                                     => this.FindIndex(index, index <= this.Count ? this.Count - index : 0u, predicate);
      [PatchMethod(AggressiveInlining)] public            int                           FindIndex                                              (uint                                    index, uint                                 length, PatchOdyssey.RefReadOnlyPredicate<T> predicate)                        { for (uint end = System.Math.Min(this.Count, index + length); end > index; ++index) { if (predicate(in this.GetValue(index))) return (int) index; } return -1; }
      [PatchMethod(AggressiveInlining)] public            int                           FindIndex                                              (uint                                    index, uint                                 length, System.Predicate                 <T> predicate)                        => this.Items.FindIndex    ((int) index, (int) length, predicate);
      [PatchMethod(AggressiveInlining)] public            int                           FindLastIndex                                          (PatchOdyssey.RefReadOnlyPredicate<T>    predicate)                                                                                                                 => this      .FindLastIndex(0u,          this.Count,   predicate);
      [PatchMethod(AggressiveInlining)] public            int                           FindLastIndex                                          (System.Predicate                 <T>    predicate)                                                                                                                 => this      .FindLastIndex(0u,          this.Count,   predicate);
      [PatchMethod(AggressiveInlining)] public            int                           FindLastIndex                                          (uint                                    index, PatchOdyssey.RefReadOnlyPredicate<T> predicate)                                                                     { for (uint end = this.Count; end-- > index; ) { if (predicate(in this.GetValue(end))) return (int) end; } return -1; }
      [PatchMethod(AggressiveInlining)] public            int                           FindLastIndex                                          (uint                                    index, System.Predicate                 <T> predicate)                                                                     => this.FindLastIndex(index, index <= this.Count ? this.Count - index : 0u, predicate);
      [PatchMethod(AggressiveInlining)] public            int                           FindLastIndex                                          (uint                                    index, uint                                 length, PatchOdyssey.RefReadOnlyPredicate<T> predicate)                        { for (uint end = System.Math.Min(this.Count, index + length); end-- > index; ) { if (predicate(in this.GetValue(end))) return (int) end; } return -1; }
      [PatchMethod(AggressiveInlining)] public            int                           FindLastIndex                                          (uint                                    index, uint                                 length, System.Predicate                 <T> predicate)                        => this.Items.FindLastIndex((int) index, (int) length, predicate);
      [PatchMethod(AggressiveInlining)] public            void                          ForEach                                                (PatchOdyssey.RefReadOnlyAction<T>       action)                                                                                                                    { for (uint index = 0u; index != this.Count; ++index) action(in this.GetValue(index)); }
      [PatchMethod(AggressiveInlining)] public            void                          ForEach                                                (System.Action                 <T>       action)                                                                                                                    => this.ForEach((in T element) => action(element));
      [PatchMethod(AggressiveInlining)] public            RefReadOnlyList<T>.Enumerator GetEnumerator                                          ()                                                                                                                                                                  => new(this);
      [PatchMethod(AggressiveInlining)] public            RefReadOnlyList<T>            GetRange                                               (uint index, uint length)                                                                                                                                           => new(this.Items, index, length);
      [PatchMethod(AggressiveInlining)] internal          ref T                         GetValue                                               (uint index)                                                                                                                                                        => ref Util.ArrayReferenceAt(in this.Items, index);
      [PatchMethod(AggressiveInlining)] public            int                           IndexOf                                                (in T element)                                                                                                                                                      => this      .IndexOf    (element, 0u,          this.Count);
      [PatchMethod(AggressiveInlining)] public            int                           IndexOf                                                (in T element, uint index)                                                                                                                                          => this      .IndexOf    (element, index,       index <= this.Count ? this.Count - index : 0u);
      [PatchMethod(AggressiveInlining)] public            int                           IndexOf                                                (in T element, uint index, uint length)                                                                                                                             => this.Items.IndexOf    (element, (int) index, (int) length);
      [PatchMethod(AggressiveInlining)] public            int                           LastIndexOf                                            (in T element)                                                                                                                                                      => this      .LastIndexOf(element, 0u,          this.Count);
      [PatchMethod(AggressiveInlining)] public            int                           LastIndexOf                                            (in T element, uint index)                                                                                                                                          => this      .LastIndexOf(element, index,       index <= this.Count ? this.Count - index : 0u);
      [PatchMethod(AggressiveInlining)] public            int                           LastIndexOf                                            (in T element, uint index, uint length)                                                                                                                             => this.Items.LastIndexOf(element, (int) index, (int) length);
      [PatchMethod(AggressiveInlining)] internal          void                          SetValue                                               (in T element, uint index)                                                                                                                                          => Util.ArrayReferenceAt(in this.Items, index) = element;
      [PatchMethod(AggressiveInlining)] public            RefReadOnlyList<T>            Slice                                                  (uint index,   uint length)                                                                                                                                         => new(this.Items, index, length);
      [PatchMethod(AggressiveInlining)] public            T[]                           ToArray                                                ()                                                                                                                                                                  { T[] array = (T[]) this.Items.Clone(); System.Array.Resize(ref array, (int) this.Count); return array; }
      [PatchMethod(AggressiveInlining)] public   override string?                       ToString                                               ()                                                                                                                                                                  { uint end = this.Count, index = 0u; if (end != index) unsafe { System.Text.StringBuilder builder = new(); for (char* separator = stackalloc char[] {',', ' '}; ; builder.Append(separator, 2)) { builder.Append(this.GetValue(index)); if (end == ++index) return builder.ToString(); } } return string.Empty; }
      [PatchMethod(AggressiveInlining)] public            bool                          TrueForAll                                             (PatchOdyssey.RefReadOnlyPredicate<T> predicate)                                                                                                                    { for (uint index = 0u; index != this.Count; ++index) { if (!predicate(in this.GetValue(index))) return false; } return true; }
      [PatchMethod(AggressiveInlining)] public            bool                          TrueForAll                                             (System.Predicate                 <T> predicate)                                                                                                                    => this.TrueForAll((in T element) => predicate(element));
      [PatchMethod(AggressiveInlining)] System.Collections.Generic.IEnumerator<T>       System.Collections.Generic.IEnumerable<T>.GetEnumerator()                                                                                                                                                                  => (System.Collections.Generic.IEnumerator<T>) this.GetEnumerator();
      [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator                  System.Collections.IEnumerable.GetEnumerator           ()                                                                                                                                                                  => (System.Collections.IEnumerator)            this.GetEnumerator();
      [PatchMethod(AggressiveInlining)] int                                             System.Collections.IStructuralComparable.CompareTo     (object?                              value, System.Collections.IComparer         comparer)                                                                         => comparer.Compare    (this, value);
      [PatchMethod(AggressiveInlining)] bool                                            System.Collections.IStructuralEquatable.Equals         (object?                              value, System.Collections.IEqualityComparer comparer)                                                                         => comparer.Equals     (this, value);
      [PatchMethod(AggressiveInlining)] int                                             System.Collections.IStructuralEquatable.GetHashCode    (System.Collections.IEqualityComparer comparer)                                                                                                                     => comparer.GetHashCode(this);
      [PatchMethod(AggressiveInlining)] object                                          System.ICloneable.Clone                                ()                                                                                                                                                                  => this    .Clone      ();

      /* … */
      public ref readonly T                  this                                            [uint         index] => ref this.GetValue(index);
      public              RefReadOnlyList<T> this                                            [System.Range range] => new(this.Items[range]);
      T                                      System.Collections.Generic.IReadOnlyList<T>.this[int          index] => this[(uint) index];
    }
      [System.Serializable] public class AnimationCurveReadOnlyList : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.AnimationCurve> { [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveReadOnlyList() : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public AnimationCurveReadOnlyList(System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class BooleanReadOnlyList        : PatchOdyssey.Collections.RefReadOnlyList<System     .Boolean>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanReadOnlyList       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BooleanReadOnlyList       (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class BoundsReadOnlyList         : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.Bounds>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsReadOnlyList        () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsReadOnlyList        (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class BoundsIntReadOnlyList      : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.BoundsInt>      { [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntReadOnlyList     () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public BoundsIntReadOnlyList     (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class ColorReadOnlyList          : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.Color>          { [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorReadOnlyList         () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ColorReadOnlyList         (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class DoubleReadOnlyList         : PatchOdyssey.Collections.RefReadOnlyList<System     .Double>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleReadOnlyList        () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public DoubleReadOnlyList        (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class FloatReadOnlyList          : PatchOdyssey.Collections.RefReadOnlyList<System     .Single>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatReadOnlyList         () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public FloatReadOnlyList         (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class GameObjectReadOnlyList     : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.GameObject>     { [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectReadOnlyList    () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GameObjectReadOnlyList    (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class GradientReadOnlyList       : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.Gradient>       { [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientReadOnlyList      () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public GradientReadOnlyList      (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class IntReadOnlyList            : PatchOdyssey.Collections.RefReadOnlyList<System     .Int32>          { [PatchConstructor, PatchMethod(AggressiveInlining)] public IntReadOnlyList           () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public IntReadOnlyList           (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class LongReadOnlyList           : PatchOdyssey.Collections.RefReadOnlyList<System     .Int64>          { [PatchConstructor, PatchMethod(AggressiveInlining)] public LongReadOnlyList          () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public LongReadOnlyList          (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class RectReadOnlyList           : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.Rect>           { [PatchConstructor, PatchMethod(AggressiveInlining)] public RectReadOnlyList          () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectReadOnlyList          (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class RectIntReadOnlyList        : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.RectInt>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntReadOnlyList       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public RectIntReadOnlyList       (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class StringReadOnlyList         : PatchOdyssey.Collections.RefReadOnlyList<System     .String>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public StringReadOnlyList        () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public StringReadOnlyList        (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class UIntReadOnlyList           : PatchOdyssey.Collections.RefReadOnlyList<System     .UInt32>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntReadOnlyList          () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public UIntReadOnlyList          (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class ULongReadOnlyList          : PatchOdyssey.Collections.RefReadOnlyList<System     .UInt64>         { [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongReadOnlyList         () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public ULongReadOnlyList         (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class Vector2ReadOnlyList        : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.Vector2>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2ReadOnlyList       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2ReadOnlyList       (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class Vector2IntReadOnlyList     : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.Vector2Int>     { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntReadOnlyList    () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector2IntReadOnlyList    (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class Vector3ReadOnlyList        : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.Vector3>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3ReadOnlyList       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3ReadOnlyList       (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class Vector3IntReadOnlyList     : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.Vector3Int>     { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntReadOnlyList    () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector3IntReadOnlyList    (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }
      [System.Serializable] public class Vector4ReadOnlyList        : PatchOdyssey.Collections.RefReadOnlyList<UnityEngine.Vector4>        { [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4ReadOnlyList       () : base() {} [PatchConstructor, PatchMethod(AggressiveInlining)] public Vector4ReadOnlyList       (System.Collections.Generic.IEnumerable<T> enumerable) : base(enumerable) {} }

    [System.Runtime.CompilerServices.CollectionBuilder(typeof(Sequence), nameof(Sequence.Create))]
    internal readonly ref struct Sequence : System.Collections.Generic.IEnumerable<int> /* ⟶ Based on collection expressions i.e. `[1, 2, …, 3]` */ {
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

    public class SharedList<T> : System.Collections.Generic.IList<T>, System.Collections.IList, System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable, System.ICloneable {
      protected internal static PatchOdyssey.Collections.RefList<T> List = new(); // ⟶ Singleton composition over inheritance
      public uint Capacity                                             { get => SharedList<T>.List.Capacity; set => SharedList<T>.List.Capacity = value; }
      public uint Count                                                => SharedList<T>.List.Count;
      int         System.Collections.Generic.ICollection<T>.Count      => SharedList<T>.List.Count;
      bool        System.Collections.Generic.ICollection<T>.IsReadOnly => false;
      int         System.Collections.ICollection.Count                 => SharedList<T>.List.Count;
      bool        System.Collections.ICollection.IsSynchronized        => false;
      object      System.Collections.ICollection.SyncRoot              => SharedList<T>.List;
      bool        System.Collections.IList.IsFixedSize                 => false;
      bool        System.Collections.IList.IsReadOnly                  => false;

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(uint                                      capacity = 0u)                                                                                   { SharedList<T>.List.EnsureCapacity(System.Math.Max(SharedList<T>.List.Capacity, capacity)); }
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(T[]                                       array)                   : this(array,                  (uint) array       .Length)              {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Array                              array)                   : this(array,                  (uint) array       .Length)              {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.ArraySegment<T>                    arraySegment)            : this(arraySegment,           (uint) arraySegment.Count)               {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.ArrayList              arrayList)               : this(arrayList,              (uint) arrayList   .Count)               {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.Generic.HashSet    <T> hashset)                 : this(hashset,                (uint) hashset     .Count)               {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.Generic.IEnumerable<T> enumerable)              : this(enumerable,             (uint) Util.EnumerableCount(enumerable)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.Generic.LinkedList <T> list)                    : this(list,                   (uint) list     .Count)                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.Generic.List       <T> list)                    : this(list,                   (uint) list     .Count)                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.Generic.Queue      <T> queue)                   : this(queue,                  (uint) queue    .Count)                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.Generic.SortedSet  <T> sortedSet)               : this(sortedSet,              (uint) sortedSet.Count)                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.Generic.Stack      <T> stack)                   : this(stack,                  (uint) stack    .Count)                  {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.IEnumerable            enumerable)              : this(enumerable,             (uint) Util.EnumerableCount(enumerable)) {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.Queue                  queue)                   : this(queue,                  (uint) queue     .Count)                 {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.SortedList             sortedList)              : this(sortedList,             (uint) sortedList.Count)                 {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Collections.Stack                  stack)                   : this(stack,                  (uint) stack     .Count)                 {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.Memory        <T>                  memory)                  : this(Util.ArrayFrom(memory), (uint) memory    .Length)                {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] public  SharedList(System.ReadOnlyMemory<T>                  memory)                  : this(Util.ArrayFrom(memory), (uint) memory    .Length)                {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] private SharedList(System.Collections.IEnumerable            enumerable, uint length) : this(length)                                                          { if (enumerable is SharedList<T>) return; SharedList<T>.List.Clear(); foreach (object value in enumerable) SharedList<T>.List.Add((T) value); }
      [PatchConstructor, PatchMethod(AggressiveInlining)] private SharedList(System.Collections.Generic.IEnumerable<T> enumerable, uint length) : this(length)                                                          { if (enumerable is SharedList<T>) return; SharedList<T>.List.Clear(); SharedList<T>.List.AddRange(enumerable); }

      /* … */
      [PatchMethod(AggressiveInlining)] public void                                                 Add                                                    (in T                                      element)                                                             =>     SharedList<T>.List.Add          (in element);
      [PatchMethod(AggressiveInlining)] public void                                                 AddRange                                               (System.Collections.Generic.IEnumerable<T> enumerable)                                                          =>     SharedList<T>.List.AddRange     (enumerable);
      [PatchMethod(AggressiveInlining)] public ref readonly T                                       Append                                                 (in T                                      element)                                                             => ref SharedList<T>.List.Append       (in element);
      [PatchMethod(AggressiveInlining)] public System.Collections.ObjectModel.ReadOnlyCollection<T> AsReadOnly                                             ()                                                                                                              =>     SharedList<T>.List.AsReadOnly   ();
      [PatchMethod(AggressiveInlining)] public int                                                  BinarySearch                                           (in T element)                                                                                                  =>     SharedList<T>.List.BinarySearch (in element);
      [PatchMethod(AggressiveInlining)] public int                                                  BinarySearch                                           (in T element,                      System.Collections.Generic.IComparer<T>? comparer)                          =>     SharedList<T>.List.BinarySearch (in element, comparer);
      [PatchMethod(AggressiveInlining)] public int                                                  BinarySearch                                           (uint index, uint count, T element, System.Collections.Generic.IComparer<T>? comparer)                          =>     SharedList<T>.List.BinarySearch (index, count, element, comparer);
      [PatchMethod(AggressiveInlining)] public void                                                 Clear                                                  ()                                                                                                              =>     SharedList<T>.List.Clear        ();
      [PatchMethod(AggressiveInlining)] public object                                               Clone                                                  ()                                                                                                              =>     SharedList<T>.List.Clone        ();
      [PatchMethod(AggressiveInlining)] public bool                                                 Contains                                               (in T                            element)                                                                       =>     SharedList<T>.List.Contains     (in element);
      [PatchMethod(AggressiveInlining)] public SharedList<U>                                        ConvertAll<U>                                          (PatchOdyssey.RefConverter<T, U> converter)                                                                     => new(SharedList<T>.List.ConvertAll<U>(converter));
      [PatchMethod(AggressiveInlining)] public SharedList<U>                                        ConvertAll<U>                                          (System.Converter         <T, U> converter)                                                                     => new(SharedList<T>.List.ConvertAll<U>(converter));
      [PatchMethod(AggressiveInlining)] public void                                                 CopyTo                                                 (T[]                             array)                                                                         =>     SharedList<T>.List.CopyTo       (array);
      [PatchMethod(AggressiveInlining)] public void                                                 CopyTo                                                 (T[]                             array, uint index)                                                             =>     SharedList<T>.List.CopyTo       (array, index);
      [PatchMethod(AggressiveInlining)] public void                                                 CopyTo                                                 (uint                            index, T[]  array, uint arrayIndex, uint count)                                =>     SharedList<T>.List.CopyTo       (index, array, arrayIndex, count);
      [PatchMethod(AggressiveInlining)] public bool                                                 Exists                                                 (PatchOdyssey.RefPredicate<T>    predicate)                                                                     =>     SharedList<T>.List.Exists       (predicate);
      [PatchMethod(AggressiveInlining)] public bool                                                 Exists                                                 (System.Predicate         <T>    predicate)                                                                     =>     SharedList<T>.List.Exists       (predicate);
      [PatchMethod(AggressiveInlining)] public T?                                                   Find                                                   (PatchOdyssey.RefPredicate<T>    predicate)                                                                     =>     SharedList<T>.List.Find         (predicate);
      [PatchMethod(AggressiveInlining)] public T?                                                   Find                                                   (System.Predicate         <T>    predicate)                                                                     =>     SharedList<T>.List.Find         (predicate);
      [PatchMethod(AggressiveInlining)] public SharedList<T>                                        FindAll                                                (PatchOdyssey.RefPredicate<T>    predicate)                                                                     => new(SharedList<T>.List.FindAll      (predicate));
      [PatchMethod(AggressiveInlining)] public SharedList<T>                                        FindAll                                                (System.Predicate         <T>    predicate)                                                                     => new(SharedList<T>.List.FindAll      (predicate));
      [PatchMethod(AggressiveInlining)] public int                                                  FindIndex                                              (PatchOdyssey.RefPredicate<T>    predicate)                                                                     =>     SharedList<T>.List.FindIndex    (predicate);
      [PatchMethod(AggressiveInlining)] public int                                                  FindIndex                                              (System.Predicate         <T>    predicate)                                                                     =>     SharedList<T>.List.FindIndex    (predicate);
      [PatchMethod(AggressiveInlining)] public int                                                  FindIndex                                              (uint                            index,             PatchOdyssey.RefPredicate<T> predicate)                     =>     SharedList<T>.List.FindIndex    (index,        predicate);
      [PatchMethod(AggressiveInlining)] public int                                                  FindIndex                                              (uint                            index,             System.Predicate         <T> predicate)                     =>     SharedList<T>.List.FindIndex    (index,        predicate);
      [PatchMethod(AggressiveInlining)] public int                                                  FindIndex                                              (uint                            index, uint count, PatchOdyssey.RefPredicate<T> predicate)                     =>     SharedList<T>.List.FindIndex    (index, count, predicate);
      [PatchMethod(AggressiveInlining)] public int                                                  FindIndex                                              (uint                            index, uint count, System.Predicate         <T> predicate)                     =>     SharedList<T>.List.FindIndex    (index, count, predicate);
      [PatchMethod(AggressiveInlining)] public T?                                                   FindLast                                               (PatchOdyssey.RefPredicate<T>    predicate)                                                                     =>     SharedList<T>.List.FindLast     (predicate);
      [PatchMethod(AggressiveInlining)] public T?                                                   FindLast                                               (System.Predicate         <T>    predicate)                                                                     =>     SharedList<T>.List.FindLast     (predicate);
      [PatchMethod(AggressiveInlining)] public int                                                  FindLastIndex                                          (PatchOdyssey.RefPredicate<T>    predicate)                                                                     =>     SharedList<T>.List.FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining)] public int                                                  FindLastIndex                                          (System.Predicate         <T>    predicate)                                                                     =>     SharedList<T>.List.FindLastIndex(predicate);
      [PatchMethod(AggressiveInlining)] public int                                                  FindLastIndex                                          (uint                            index,             PatchOdyssey.RefPredicate<T> predicate)                     =>     SharedList<T>.List.FindLastIndex(index,        predicate);
      [PatchMethod(AggressiveInlining)] public int                                                  FindLastIndex                                          (uint                            index,             System.Predicate         <T> predicate)                     =>     SharedList<T>.List.FindLastIndex(index,        predicate);
      [PatchMethod(AggressiveInlining)] public int                                                  FindLastIndex                                          (uint                            index, uint count, PatchOdyssey.RefPredicate<T> predicate)                     =>     SharedList<T>.List.FindLastIndex(index, count, predicate);
      [PatchMethod(AggressiveInlining)] public int                                                  FindLastIndex                                          (uint                            index, uint count, System.Predicate         <T> predicate)                     =>     SharedList<T>.List.FindLastIndex(index, count, predicate);
      [PatchMethod(AggressiveInlining)] public void                                                 ForEach                                                (PatchOdyssey.RefAction<T>       action)                                                                        =>     SharedList<T>.List.ForEach      (action);
      [PatchMethod(AggressiveInlining)] public void                                                 ForEach                                                (System.Action         <T>       action)                                                                        =>     SharedList<T>.List.ForEach      (action);
      [PatchMethod(AggressiveInlining)] public PatchOdyssey.Collections.RefList<T>.Enumerator       GetEnumerator                                          ()                                                                                                              =>     SharedList<T>.List.GetEnumerator();
      [PatchMethod(AggressiveInlining)] public SharedList<T>                                        GetRange                                               (uint                index, uint count)                                                                         => new(SharedList<T>.List.GetRange     (index, count));
      [PatchMethod(AggressiveInlining)] public int                                                  IndexOf                                                (in T                element)                                                                                   =>     SharedList<T>.List.IndexOf      (in element);
      [PatchMethod(AggressiveInlining)] public int                                                  IndexOf                                                (in T                element, uint                                      index)                                  =>     SharedList<T>.List.IndexOf      (in element, index);
      [PatchMethod(AggressiveInlining)] public int                                                  IndexOf                                                (in T                element, uint                                      index, uint count)                      =>     SharedList<T>.List.IndexOf      (in element, index, count);
      [PatchMethod(AggressiveInlining)] public void                                                 Insert                                                 (uint                index,   in T                                      element)                                =>     SharedList<T>.List.Insert       (index,   element);
      [PatchMethod(AggressiveInlining)] public void                                                 InsertRange                                            (uint                index,   System.Collections.Generic.IEnumerable<T> enumerable)                             =>     SharedList<T>.List.InsertRange  (index,   enumerable);
      [PatchMethod(AggressiveInlining)] public int                                                  LastIndexOf                                            (in T                element)                                                                                   =>     SharedList<T>.List.LastIndexOf  (in element);
      [PatchMethod(AggressiveInlining)] public int                                                  LastIndexOf                                            (in T                element, uint index)                                                                       =>     SharedList<T>.List.LastIndexOf  (in element, index);
      [PatchMethod(AggressiveInlining)] public int                                                  LastIndexOf                                            (in T                element, uint index, uint count)                                                           =>     SharedList<T>.List.LastIndexOf  (in element, index, count);
      [PatchMethod(AggressiveInlining)] public ref readonly T                                       Prepend                                                (in T                element)                                                                                   => ref SharedList<T>.List.Prepend      (in element);
      [PatchMethod(AggressiveInlining)] public bool                                                 Remove                                                 (in T                element)                                                                                   =>     SharedList<T>.List.Remove       (in element);
      [PatchMethod(AggressiveInlining)] public int                                                  RemoveAll                                              (PatchOdyssey.RefPredicate<T> predicate)                                                                        =>     SharedList<T>.List.RemoveAll    (predicate);
      [PatchMethod(AggressiveInlining)] public int                                                  RemoveAll                                              (System.Predicate         <T> predicate)                                                                        =>     SharedList<T>.List.RemoveAll    (predicate);
      [PatchMethod(AggressiveInlining)] public void                                                 RemoveAt                                               (uint                index)                                                                                     =>     SharedList<T>.List.RemoveAt     (index);
      [PatchMethod(AggressiveInlining)] public void                                                 RemoveRange                                            (uint                index, uint count)                                                                         =>     SharedList<T>.List.RemoveRange  (index, count);
      [PatchMethod(AggressiveInlining)] public void                                                 Reverse                                                ()                                                                                                              =>     SharedList<T>.List.Reverse      ();
      [PatchMethod(AggressiveInlining)] public void                                                 Reverse                                                (uint index, uint count)                                                                                        =>     SharedList<T>.List.Reverse      (index, count);
      [PatchMethod(AggressiveInlining)] public void                                                 Sort                                                   ()                                                                                                              =>     SharedList<T>.List.Sort         ();
      [PatchMethod(AggressiveInlining)] public void                                                 Sort                                                   (PatchOdyssey.RefComparison          <T>? comparison)                                                           =>     SharedList<T>.List.Sort         (comparison);
      [PatchMethod(AggressiveInlining)] public void                                                 Sort                                                   (System.Collections.Generic.IComparer<T>? comparer)                                                             =>     SharedList<T>.List.Sort         (comparer);
      [PatchMethod(AggressiveInlining)] public void                                                 Sort                                                   (System.Comparison                   <T>? comparison)                                                           =>     SharedList<T>.List.Sort         (comparison);
      [PatchMethod(AggressiveInlining)] public void                                                 Sort                                                   (uint                                     index, uint count, System.Collections.Generic.IComparer<T>? comparer) =>     SharedList<T>.List.Sort         (index, count, comparer);
      [PatchMethod(AggressiveInlining)] public T[]                                                  ToArray                                                ()                                                                                                              =>     SharedList<T>.List.ToArray      ();
      [PatchMethod(AggressiveInlining)] public void                                                 TrimExcess                                             ()                                                                                                              =>     SharedList<T>.List.TrimExcess   ();
      [PatchMethod(AggressiveInlining)] public void                                                 TrimExcess                                             (uint capacity)                                                                                                 =>     SharedList<T>.List.TrimExcess   (capacity);
      [PatchMethod(AggressiveInlining)] public bool                                                 TrueForAll                                             (PatchOdyssey.RefPredicate<T> predicate)                                                                        =>     SharedList<T>.List.TrueForAll   (predicate);
      [PatchMethod(AggressiveInlining)] public bool                                                 TrueForAll                                             (System.Predicate         <T> predicate)                                                                        =>     SharedList<T>.List.TrueForAll   (predicate);
      [PatchMethod(AggressiveInlining)] void                                                        System.Collections.Generic.ICollection<T>.Add          (T                            element)                                                                          =>     this              .Add          (in element);
      [PatchMethod(AggressiveInlining)] void                                                        System.Collections.Generic.ICollection<T>.Clear        ()                                                                                                              =>     this              .Clear        ();
      [PatchMethod(AggressiveInlining)] bool                                                        System.Collections.Generic.ICollection<T>.Contains     (T   element)                                                                                                   =>     this              .Contains     (in element);
      [PatchMethod(AggressiveInlining)] void                                                        System.Collections.Generic.ICollection<T>.CopyTo       (T[] array, int index)                                                                                          =>     this              .CopyTo       (array, index);
      [PatchMethod(AggressiveInlining)] bool                                                        System.Collections.Generic.ICollection<T>.Remove       (T   element)                                                                                                   =>     this              .Remove       (in element);
      [PatchMethod(AggressiveInlining)] System.Collections.Generic.IEnumerator<T>                   System.Collections.Generic.IEnumerable<T>.GetEnumerator()                                                                                                              =>     this              .GetEnumerator();
      [PatchMethod(AggressiveInlining)] int                                                         System.Collections.Generic.IList<T>.IndexOf            (T            element)                                                                                          =>     this              .IndexOf      (in element);
      [PatchMethod(AggressiveInlining)] void                                                        System.Collections.Generic.IList<T>.Insert             (int          index, T element)                                                                                 =>     this              .Insert       (index, in element);
      [PatchMethod(AggressiveInlining)] void                                                        System.Collections.Generic.IList<T>.RemoveAt           (int          index)                                                                                            =>     this              .RemoveAt     (index);
      [PatchMethod(AggressiveInlining)] void                                                        System.Collections.ICollection.CopyTo                  (System.Array array, int index)                                                                                 =>     this              .CopyTo       (array, (uint) index);
      [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator                              System.Collections.IEnumerable.GetEnumerator           ()                                                                                                              =>     this              .GetEnumerator();
      [PatchMethod(AggressiveInlining)] int                                                         System.Collections.IList.Add                           (object? element)                                                                                               =>     this              .Add          ((T) element);
      [PatchMethod(AggressiveInlining)] void                                                        System.Collections.IList.Clear                         ()                                                                                                              =>     this              .Clear        ();
      [PatchMethod(AggressiveInlining)] bool                                                        System.Collections.IList.Contains                      (object? element)                                                                                               =>     this              .Contains     ((T) element);
      [PatchMethod(AggressiveInlining)] int                                                         System.Collections.IList.IndexOf                       (object? element)                                                                                               =>     this              .IndexOf      ((T) element);
      [PatchMethod(AggressiveInlining)] void                                                        System.Collections.IList.Insert                        (int     index, object? element)                                                                                =>     this              .Insert       (index, (T) element);
      [PatchMethod(AggressiveInlining)] void                                                        System.Collections.IList.Remove                        (object? element)                                                                                               =>     this              .Remove       ((T) element);
      [PatchMethod(AggressiveInlining)] void                                                        System.Collections.IList.RemoveAt                      (int     index)                                                                                                 =>     this              .RemoveAt     (index);
      [PatchMethod(AggressiveInlining)] int                                                         System.Collections.IStructuralComparable.CompareTo     (object?                              value, System.Collections.IComparer         comparer)                     =>     this              .CompareTo    (value, comparer);
      [PatchMethod(AggressiveInlining)] bool                                                        System.Collections.IStructuralEquatable.Equals         (object?                              value, System.Collections.IEqualityComparer comparer)                     =>     this              .Equals       (value, comparer);
      [PatchMethod(AggressiveInlining)] int                                                         System.Collections.IStructuralEquatable.GetHashCode    (System.Collections.IEqualityComparer comparer)                                                                 =>     this              .GetHashCode  (comparer);
      [PatchMethod(AggressiveInlining)] object                                                      System.ICloneable.Clone                                ()                                                                                                              =>     this              .Clone        ();

      public ref T          this                                    [uint         index] => ref SharedList<T>.List[index];
      public     RefList<T> this                                    [System.Range range] =>     SharedList<T>.List[range];
      T                     System.Collections.Generic.IList<T>.this[int          index] { get => this[(uint) index]; set => this[(uint) index] = value; }
      object?               System.Collections.IList.this           [int          index] { get => this[(uint) index]; set => this[(uint) index] = (T) value!; }
    }
      public class GameObjectSharedList<T> : PatchOdyssey.Collections.SharedList<T>, System.Collections.Generic.IList<T>, System.Collections.IList, System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable, System.ICloneable where T : UnityEngine.Object /* ⟶ `UnityEngine.Component` or `UnityEngine.GameObject` */ {
        int    System.Collections.Generic.ICollection<T>.Count      => base.Count;
        bool   System.Collections.Generic.ICollection<T>.IsReadOnly => false;
        int    System.Collections.ICollection.Count                 => base.Count;
        bool   System.Collections.ICollection.IsSynchronized        => false;
        object System.Collections.ICollection.SyncRoot              => PatchOdyssey.Collections.SharedList<T>.List;
        bool   System.Collections.IList.IsFixedSize                 => false;
        bool   System.Collections.IList.IsReadOnly                  => false;

        /* … */
        [PatchConstructor, PatchMethod(AggressiveInlining)] internal GameObjectSharedList(uint                    capacity = 0u) : base(capacity) {}
        [PatchConstructor, PatchMethod(AggressiveInlining)] internal GameObjectSharedList(GameObjectSharedList<T> list)          : base(list)     {}
        [PatchConstructor, PatchMethod(AggressiveInlining)] private  GameObjectSharedList(SharedList          <T> list)          : base(list)     {}

        /* … */
        [PatchMethod(AggressiveInlining)] internal new void           Add     (in T                                      element)    =>     base.Add     (element);
        [PatchMethod(AggressiveInlining)] internal new void           AddRange(System.Collections.Generic.IEnumerable<T> enumerable) =>     base.AddRange(enumerable);
        [PatchMethod(AggressiveInlining)] internal new ref readonly T Append  (in T                                      element)    => ref base.Append  (element);

        [PatchMethod(AggressiveInlining)] public GameObjectSharedList<U> ByComponent<U>() where U : UnityEngine.Component => this.ByComponent(typeof(U)).ConvertAll(static element => (U) element);
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

        [PatchMethod(AggressiveInlining)]
        public GameObjectSharedList<T> ByName(string name) {
          this.RemoveAll(value => name != value.name);
          return PatchOdyssey.Collections.SharedList<T>.List;
        }

        [PatchMethod(AggressiveInlining)]
        public GameObjectSharedList<T> ByTag(string tag) {
          this.RemoveAll(value => value switch {
            UnityEngine.Component  component  => component .tag == tag,
            UnityEngine.GameObject gameObject => gameObject.tag == tag,
            _                                 => false
          });

          return PatchOdyssey.Collections.SharedList<T>.List;
        }

        [PatchMethod(AggressiveInlining)] internal           new void                    Clear                                                  ()                                                                                                              =>     base.Clear                      ();
        [PatchMethod(AggressiveInlining)] public             new GameObjectSharedList<U> ConvertAll<U>                                          (System.Converter<T, U> converter) where U : UnityEngine.Object /* ⟶ T */                                      => new(base.ConvertAll<U>              (converter));
        [PatchMethod(AggressiveInlining)] internal           new GameObjectSharedList<T> FindAll                                                (System.Predicate<T>    predicate)                                                                              => new(base.FindAll                    (predicate));
        [PatchMethod(AggressiveInlining)] internal           new GameObjectSharedList<T> GetRange                                               (uint                   index, uint                                      count)                                 => new(base.GetRange                   (index, count));
        [PatchMethod(AggressiveInlining)] internal           new void                    Insert                                                 (uint                   index, in T                                      element)                               =>     base.Insert                     (index,   element);
        [PatchMethod(AggressiveInlining)] internal           new void                    InsertRange                                            (uint                   index, System.Collections.Generic.IEnumerable<T> enumerable)                            =>     base.InsertRange                (index,   enumerable);
        [PatchMethod(AggressiveInlining)] internal           new ref readonly T          Prepend                                                (in T                   element)                                                                                => ref base.Prepend                    (element);
        [PatchMethod(AggressiveInlining)] internal           new bool                    Remove                                                 (in T                   element)                                                                                =>     base.Remove                     (element);
        [PatchMethod(AggressiveInlining)] internal           new int                     RemoveAll                                              (System.Predicate<T>    predicate)                                                                              =>     base.RemoveAll                  (predicate);
        [PatchMethod(AggressiveInlining)] internal           new void                    RemoveAt                                               (uint                   index)                                                                                  =>     base.RemoveAt                   (index);
        [PatchMethod(AggressiveInlining)] internal           new void                    RemoveRange                                            (uint                   index, uint count)                                                                      =>     base.RemoveRange                (index, count);
        [PatchMethod(AggressiveInlining)] internal           new void                    Reverse                                                ()                                                                                                              =>     base.Reverse                    ();
        [PatchMethod(AggressiveInlining)] internal           new void                    Reverse                                                (uint index, uint count)                                                                                        =>     base.Reverse                    (index, count);
        [PatchMethod(AggressiveInlining)] internal           new void                    Sort                                                   ()                                                                                                              =>     base.Sort                       ();
        [PatchMethod(AggressiveInlining)] internal           new void                    Sort                                                   (System.Collections.Generic.IComparer<T>? comparer)                                                             =>     base.Sort                       (comparer);
        [PatchMethod(AggressiveInlining)] internal           new void                    Sort                                                   (System.Comparison                   <T>? comparison)                                                           =>     base.Sort                       (comparison);
        [PatchMethod(AggressiveInlining)] internal           new void                    Sort                                                   (uint                                     index, uint count, System.Collections.Generic.IComparer<T>? comparer) =>     base.Sort                       (index, count, comparer);
        [PatchMethod(AggressiveInlining)] protected internal new void                    TrimExcess                                             ()                                                                                                              =>     base.TrimExcess                 ();
        [PatchMethod(AggressiveInlining)] protected internal new void                    TrimExcess                                             (uint capacity)                                                                                                 =>     base.TrimExcess                 (capacity);
        [PatchMethod(AggressiveInlining)] void                                           System.Collections.Generic.ICollection<T>.Add          (T    element)                                                                                                  =>     base.Add                        (in element);
        [PatchMethod(AggressiveInlining)] void                                           System.Collections.Generic.ICollection<T>.Clear        ()                                                                                                              =>     base.Clear                      ();
        [PatchMethod(AggressiveInlining)] bool                                           System.Collections.Generic.ICollection<T>.Contains     (T   element)                                                                                                   =>     base.Contains                   (in element);
        [PatchMethod(AggressiveInlining)] void                                           System.Collections.Generic.ICollection<T>.CopyTo       (T[] array, int index)                                                                                          =>     base.CopyTo                     (array, index);
        [PatchMethod(AggressiveInlining)] bool                                           System.Collections.Generic.ICollection<T>.Remove       (T   element)                                                                                                   =>     base.Remove                     (in element);
        [PatchMethod(AggressiveInlining)] System.Collections.Generic.IEnumerator<T>      System.Collections.Generic.IEnumerable<T>.GetEnumerator()                                                                                                              =>     base.GetEnumerator              ();
        [PatchMethod(AggressiveInlining)] int                                            System.Collections.Generic.IList<T>.IndexOf            (T            element)                                                                                          =>     base.IndexOf                    (in element);
        [PatchMethod(AggressiveInlining)] void                                           System.Collections.Generic.IList<T>.Insert             (int          index, T element)                                                                                 =>     base.Insert                     (index, in element);
        [PatchMethod(AggressiveInlining)] void                                           System.Collections.Generic.IList<T>.RemoveAt           (int          index)                                                                                            =>     base.RemoveAt                   (index);
        [PatchMethod(AggressiveInlining)] void                                           System.Collections.ICollection.CopyTo                  (System.Array array, int index)                                                                                 =>     base.CopyTo                     (array, (uint) index);
        [PatchMethod(AggressiveInlining)] System.Collections.IEnumerator                 System.Collections.IEnumerable.GetEnumerator           ()                                                                                                              =>     base.GetEnumerator              ();
        [PatchMethod(AggressiveInlining)] int                                            System.Collections.IList.Add                           (object? element)                                                                                               =>     base.Add                        ((T) element);
        [PatchMethod(AggressiveInlining)] void                                           System.Collections.IList.Clear                         ()                                                                                                              =>     base.Clear                      ();
        [PatchMethod(AggressiveInlining)] bool                                           System.Collections.IList.Contains                      (object? element)                                                                                               =>     base.Contains                   ((T) element);
        [PatchMethod(AggressiveInlining)] int                                            System.Collections.IList.IndexOf                       (object? element)                                                                                               =>     base.IndexOf                    ((T) element);
        [PatchMethod(AggressiveInlining)] void                                           System.Collections.IList.Insert                        (int     index, object? element)                                                                                =>     base.Insert                     (index, (T) element);
        [PatchMethod(AggressiveInlining)] void                                           System.Collections.IList.Remove                        (object? element)                                                                                               =>     base.Remove                     ((T) element);
        [PatchMethod(AggressiveInlining)] void                                           System.Collections.IList.RemoveAt                      (int     index)                                                                                                 =>     base.RemoveAt                   (index);
        [PatchMethod(AggressiveInlining)] int                                            System.Collections.IStructuralComparable.CompareTo     (object?                              value, System.Collections.IComparer         comparer)                     =>     base.CompareTo                  (value, comparer);
        [PatchMethod(AggressiveInlining)] bool                                           System.Collections.IStructuralEquatable.Equals         (object?                              value, System.Collections.IEqualityComparer comparer)                     =>     base.Equals                     (value, comparer);
        [PatchMethod(AggressiveInlining)] int                                            System.Collections.IStructuralEquatable.GetHashCode    (System.Collections.IEqualityComparer comparer)                                                                 =>     base.GetHashCode                (comparer);
        [PatchMethod(AggressiveInlining)] object                                         System.ICloneable.Clone                                ()                                                                                                              =>     base.Clone                      ();
      }

    internal readonly struct WaitInfo /* ⟶ Considered `UnityEngine.MonoBehaviour::Invoke[Repeating](nameof(𝑓) or ((System.Delegate) 𝑓).Method.Name, delay[, interval])` */ {
      private sealed class Waiter : UnityEngine.MonoBehaviour {}

      /* … */
      internal static          UnityEngine.MonoBehaviour                                                              WAIT  = new UnityEngine.GameObject("…").AddComponent<WaitInfo.Waiter>();
      internal static readonly System.Collections.Generic.SortedDictionary<double, PatchOdyssey.Collections.WaitInfo> WAITS = new(new System.Collections.Generic.Dictionary<double, PatchOdyssey.Collections.WaitInfo>(16) {{double.NaN, new()}});

      internal readonly UnityEngine.Coroutine?                                                    coroutine = null;
      internal readonly PatchOdyssey.Collections.EventHandler<PatchOdyssey.Collections.WaitEvent> handlers  = new();

      /* … */
      [PatchConstructor, PatchMethod(AggressiveInlining)] public   WaitInfo()                                                                                                                                                                           {}
      [PatchConstructor, PatchMethod(AggressiveInlining)] internal WaitInfo(in PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Collections.WaitEvent> handler)                                                                                        { this.handlers = new(handler); }
      [PatchConstructor, PatchMethod(AggressiveInlining)] internal WaitInfo(UnityEngine.Coroutine                                                       coroutine, in PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Collections.WaitEvent> handler) { this.handlers = new(handler); this.coroutine = coroutine; }
    }
  }

  /* … */
  public delegate object  DictionaryGUIField         (UnityEngine.Rect position, object value);                                          //
  public delegate void    Handler<T>                 (object? target,            in T    data) where T : PatchOdyssey.Collections.Event; // ⟶ Handles completed `Load`, `Wait`, … operations i.e. `System.EventHandler`
  public delegate object? Interpolator               (double  progress,          object? a, object? b);                                  // ⟶ Interpolates `::begin` and `::end` properties in `Animation.UIKeyframe["…"]`
  public delegate T       Interpolator         <T>   (double  progress,          in T    a, in T    b);                                  //    ^^
  public delegate void    RefAction            <T>   (ref T   value);                                                                    // ⟶ Based on `System.Action<T>`
  public delegate int     RefComparison        <T>   (ref T   a, ref T b);                                                               // ⟶ Based on `System.Comparison<T>`
  public delegate U       RefConverter         <T, U>(ref T   value);                                                                    // ⟶ Based on `System.Converter<T, U>`
  public delegate bool    RefPredicate         <T>   (ref T   value);                                                                    // ⟶ Based on `System.Predicate<T>`
  public delegate void    RefReadOnlyAction    <T>   (in  T   value);                                                                    // ⟶ Based on `System.Action<T>`
  public delegate int     RefReadOnlyComparison<T>   (in  T   a, in  T b);                                                               // ⟶ Based on `System.Comparison<T>`
  public delegate U       RefReadOnlyConverter <T, U>(in  T   value);                                                                    // ⟶ Based on `System.Converter<T, U>`
  public delegate bool    RefReadOnlyPredicate <T>   (in  T   value);                                                                    // ⟶ Based on `System.Predicate<T>`
  public delegate double  Tweener                    (double  time);                                                                     // ⟶ Adjusts interpolation be-tween `Interpolator(…)`’s `progress` from `a` to `b`

  public sealed class ReadOnlyInInspectorAttribute : UnityEngine.PropertyAttribute, Unity.Collections.ReadOnlyAttribute {
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
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.ReadOnlyInInspectorAttribute))]
    public class ReadOnlyInInspectorDrawer : UnityEditor.PropertyDrawer {
      public override float GetPropertyHeight(UnityEditor.SerializedProperty property, UnityEngine.GUIContent         label)                                  => UnityEditor.EditorGUI.GetPropertyHeight(property, label, true);
      public override void  OnGUI            (UnityEngine.Rect               position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) { UnityEngine.GUI.enabled = false; UnityEditor.EditorGUI.PropertyField(position, property, label, true); UnityEngine.GUI.enabled = true; }
    }

    public class RefDictionaryDrawer<TKey, TValue> : UnityEditor.PropertyDrawer {
      private bool foldout = false;

      /* … */
      [PatchMethod(AggressiveInlining)]
      private System.Collections.Generic.IDictionary<TKey, TValue> Ensure(UnityEditor.SerializedProperty property) {
        System.Collections.Generic.IDictionary<TKey, TValue> dictionary = this.fieldInfo.GetValue(property.serializedObject.targetObject) as System.Collections.Generic.IDictionary<TKey, TValue> ?? new PatchOdyssey.Collections.RefDictionary<TKey, TValue>();

        this.fieldInfo.SetValue(property.serializedObject.targetObject, dictionary);
        return dictionary;
      }

      [PatchMethod(AggressiveInlining)]
      public override float GetPropertyHeight(UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) {
        return base.GetPropertyHeight(property, label) * (this.foldout || UnityEditor.EditorPrefs.GetBool(label.text) ? System.Math.Max(this.Ensure(property).Count, 1) + 1 : 1);
      }

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
        if (!dictionary.IsReadOnly) {
          if (UnityEngine.GUI.Button(positions.add, new UnityEngine.GUIContent("+", "Add field"), UnityEditor.EditorStyles.miniButton))
          dictionary.TryAdd(typeof(TKey) != typeof(string) ? System.Activator.CreateInstance<TKey>() : (TKey) (string.Empty as object), typeof(TValue).IsValueType ? System.Activator.CreateInstance<TValue>() : (TValue) (null as object)!);

          if (UnityEngine.GUI.Button(positions.clear, new UnityEngine.GUIContent("×", "Clear dictionary"), UnityEditor.EditorStyles.miniButtonRight))
          dictionary.Clear();
        }

        UnityEditor.EditorGUI.BeginChangeCheck();
          this.foldout = UnityEditor.EditorPrefs.GetBool(label.text);
          this.foldout = UnityEditor.EditorGUI.  Foldout(positions.foldout, this.foldout, label, true);
        if (UnityEditor.EditorGUI.EndChangeCheck()) UnityEditor.EditorPrefs.SetBool(label.text, this.foldout);

        if (this.foldout) {
          if (0 == dictionary.Count) {
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
              else key = (TKey) PatchOdyssey.Collections.RefDictionary<TKey, TValue>.DelegateGUIField<TKey>()!(subpositions.key, key);
            if (UnityEditor.EditorGUI.EndChangeCheck()) {
              dictionary.Remove(element.Key);
              dictionary.Add   (key, value);

              break;
            }

            UnityEditor.EditorGUI.BeginChangeCheck();
              value = (TValue) PatchOdyssey.Collections.RefDictionary<TKey, TValue>.DelegateGUIField<TValue>()!(subpositions.value, value!);
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
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.AnimationCurveDictionary))]         public class AnimationCurveDictionaryDrawer         : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.AnimationCurve> {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.AnimationCurveReadOnlyDictionary))] public class AnimationCurveReadOnlyDictionaryDrawer : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.AnimationCurve> {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.BooleanDictionary))]                public class BooleanDictionaryDrawer                : PatchOdyssey.RefDictionaryDrawer<string, System     .Boolean>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.BooleanReadOnlyDictionary))]        public class BooleanReadOnlyDictionaryDrawer        : PatchOdyssey.RefDictionaryDrawer<string, System     .Boolean>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.BoundsDictionary))]                 public class BoundsDictionaryDrawer                 : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.Bounds>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.BoundsReadOnlyDictionary))]         public class BoundsReadOnlyDictionaryDrawer         : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.Bounds>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.BoundsIntDictionary))]              public class BoundsIntDictionaryDrawer              : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.BoundsInt>      {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.BoundsIntReadOnlyDictionary))]      public class BoundsIntReadOnlyDictionaryDrawer      : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.BoundsInt>      {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.ColorDictionary))]                  public class ColorDictionaryDrawer                  : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.Color>          {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.ColorReadOnlyDictionary))]          public class ColorReadOnlyDictionaryDrawer          : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.Color>          {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.DoubleDictionary))]                 public class DoubleDictionaryDrawer                 : PatchOdyssey.RefDictionaryDrawer<string, System     .Double>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.DoubleReadOnlyDictionary))]         public class DoubleReadOnlyDictionaryDrawer         : PatchOdyssey.RefDictionaryDrawer<string, System     .Double>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.FloatDictionary))]                  public class FloatDictionaryDrawer                  : PatchOdyssey.RefDictionaryDrawer<string, System     .Single>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.FloatReadOnlyDictionary))]          public class FloatReadOnlyDictionaryDrawer          : PatchOdyssey.RefDictionaryDrawer<string, System     .Single>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.GameObjectDictionary))]             public class GameObjectDictionaryDrawer             : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.GameObject>     {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.GameObjectReadOnlyDictionary))]     public class GameObjectReadOnlyDictionaryDrawer     : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.GameObject>     {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.GradientDictionary))]               public class GradientDictionaryDrawer               : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.Gradient>       {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.GradientReadOnlyDictionary))]       public class GradientReadOnlyDictionaryDrawer       : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.Gradient>       {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.IntDictionary))]                    public class IntDictionaryDrawer                    : PatchOdyssey.RefDictionaryDrawer<string, System     .Int32>          {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.IntReadOnlyDictionary))]            public class IntReadOnlyDictionaryDrawer            : PatchOdyssey.RefDictionaryDrawer<string, System     .Int32>          {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.LongDictionary))]                   public class LongDictionaryDrawer                   : PatchOdyssey.RefDictionaryDrawer<string, System     .Int64>          {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.LongReadOnlyDictionary))]           public class LongReadOnlyDictionaryDrawer           : PatchOdyssey.RefDictionaryDrawer<string, System     .Int64>          {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.RectDictionary))]                   public class RectDictionaryDrawer                   : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.Rect>           {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.RectReadOnlyDictionary))]           public class RectReadOnlyDictionaryDrawer           : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.Rect>           {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.RectIntDictionary))]                public class RectIntDictionaryDrawer                : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.RectInt>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.RectIntReadOnlyDictionary))]        public class RectIntReadOnlyDictionaryDrawer        : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.RectInt>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.StringDictionary))]                 public class StringDictionaryDrawer                 : PatchOdyssey.RefDictionaryDrawer<string, System     .String>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.StringReadOnlyDictionary))]         public class StringReadOnlyDictionaryDrawer         : PatchOdyssey.RefDictionaryDrawer<string, System     .String>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.UIntDictionary))]                   public class UIntDictionaryDrawer                   : PatchOdyssey.RefDictionaryDrawer<string, System     .UInt32>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.UIntReadOnlyDictionary))]           public class UIntReadOnlyDictionaryDrawer           : PatchOdyssey.RefDictionaryDrawer<string, System     .UInt32>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.ULongDictionary))]                  public class ULongDictionaryDrawer                  : PatchOdyssey.RefDictionaryDrawer<string, System     .UInt64>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.ULongReadOnlyDictionary))]          public class ULongReadOnlyDictionaryDrawer          : PatchOdyssey.RefDictionaryDrawer<string, System     .UInt64>         {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector2Dictionary))]                public class Vector2DictionaryDrawer                : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.Vector2>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector2ReadOnlyDictionary))]        public class Vector2ReadOnlyDictionaryDrawer        : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.Vector2>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector2IntDictionary))]             public class Vector2IntDictionaryDrawer             : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.Vector2Int>     {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector2IntReadOnlyDictionary))]     public class Vector2IntReadOnlyDictionaryDrawer     : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.Vector2Int>     {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector3Dictionary))]                public class Vector3DictionaryDrawer                : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.Vector3>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector3ReadOnlyDictionary))]        public class Vector3ReadOnlyDictionaryDrawer        : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.Vector3>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector3IntDictionary))]             public class Vector3IntDictionaryDrawer             : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.Vector3Int>     {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector3IntReadOnlyDictionary))]     public class Vector3IntReadOnlyDictionaryDrawer     : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.Vector3Int>     {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector4Dictionary))]                public class Vector4DictionaryDrawer                : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.Vector4>        {}
      [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Collections.Vector4ReadOnlyDictionary))]        public class Vector4ReadOnlyDictionaryDrawer        : PatchOdyssey.RefDictionaryDrawer<string, UnityEngine.Vector4>        {}

    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.ReadWriteInInspectorAttribute))]
    public class ReadWriteInInspectorDrawer : UnityEditor.PropertyDrawer {
      public override float GetPropertyHeight(UnityEditor.SerializedProperty property, UnityEngine.GUIContent         label)                                  => UnityEditor.EditorGUI.GetPropertyHeight(property, label, true);
      public override void  OnGUI            (UnityEngine.Rect               position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) => UnityEditor.EditorGUI.PropertyField    (position, property, label, true);
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

    [PatchMethod(AggressiveInlining)] public static ref readonly object? Append   (this System.Collections.ArrayList             arrayList, in object? value) { arrayList.Add    (value); return value; }
    [PatchMethod(AggressiveInlining)] public static ref readonly T       Append<T>(this System.Collections.ArrayList             arrayList, in T       value) { arrayList.Add    (value); return value; }
    [PatchMethod(AggressiveInlining)] public static ref readonly T       Append<T>(this System.Collections.Generic.LinkedList<T> list,      in T       value) { list     .AddLast(value); return value; }
    [PatchMethod(AggressiveInlining)] public static ref readonly T       Append<T>(this System.Collections.Generic.List      <T> list,      in T       value) { list     .Add    (value); return value; }

    [PatchMethod(AggressiveInlining)] public static T[]                                                                       AsCopy<T>           (this T[]                                                                       array)                            => (T[])          array.Clone();
    [PatchMethod(AggressiveInlining)] public static System.Array                                                              AsCopy              (this System.Array                                                              array)                            => (System.Array) array.Clone();
    [PatchMethod(AggressiveInlining)] public static System.ArraySegment<T>                                                    AsCopy<T>           (this System.ArraySegment<T>                                                    arraySegment)                     => new(Util.ArrayFrom(arraySegment));
    [PatchMethod(AggressiveInlining)] public static System.Collections.ArrayList                                              AsCopy              (this System.Collections.ArrayList                                              arrayList)                        => (System.Collections.ArrayList) arrayList.Clone();
    [PatchMethod(AggressiveInlining)] public static System.Collections.BitArray                                               AsCopy              (this System.Collections.BitArray                                               bits)                             => (System.Collections.BitArray)  bits     .Clone();
    [PatchMethod(AggressiveInlining)] public static System.Collections.Queue                                                  AsCopy              (this System.Collections.Queue                                                  queue)                            => (System.Collections.Queue)     queue    .Clone();
    [PatchMethod(AggressiveInlining)] public static System.Collections.Generic.SortedDictionary<TKey, TValue>                 AsCopy<TKey, TValue>(this System.Collections.Generic.Dictionary      <TKey, TValue>                 dictionary)                       => new(dictionary, dictionary.Comparer);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Generic.HashSet         <T>                            AsCopy<T>           (this System.Collections.Generic.HashSet         <T>                            hashset)                          => new(hashset, hashset.Comparer);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Generic.LinkedList      <T>                            AsCopy<T>           (this System.Collections.Generic.LinkedList      <T>                            list)                             => new(list);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Generic.List            <T>                            AsCopy<T>           (this System.Collections.Generic.List            <T>                            list)                             => new(list);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Generic.Queue           <T>                            AsCopy<T>           (this System.Collections.Generic.Queue           <T>                            queue)                            => new(queue);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Generic.SortedDictionary<TKey, TValue>                 AsCopy<TKey, TValue>(this System.Collections.Generic.SortedDictionary<TKey, TValue>                 sortedDictionary)                 => new(sortedDictionary, sortedDictionary.Comparer);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Generic.SortedList      <TKey, TValue>                 AsCopy<TKey, TValue>(this System.Collections.Generic.SortedList      <TKey, TValue>                 sortedList)                       => new(sortedList,       sortedList      .Comparer);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Generic.SortedSet       <T>                            AsCopy<T>           (this System.Collections.Generic.SortedSet       <T>                            sortedSet)                        => new(sortedSet,        sortedSet       .Comparer);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Generic.Stack           <T>                            AsCopy<T>           (this System.Collections.Generic.Stack           <T>                            stack)                            => new(stack);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Hashtable                                              AsCopy              (this System.Collections.Hashtable                                              hashtable)                        => (System.Collections.Hashtable) hashtable.Clone(); // ⟶ `new(hashtable, hashtable.EqualityComparer)`
    [PatchMethod(AggressiveInlining)] public static System.Collections.ObjectModel.ObservableCollection        <T>            AsCopy<T>           (this System.Collections.ObjectModel.ObservableCollection        <T>            collection)                       => new(collection);
    [PatchMethod(AggressiveInlining)] public static System.Collections.ObjectModel.ReadOnlyCollection          <T>            AsCopy<T>           (this System.Collections.ObjectModel.ReadOnlyCollection          <T>            collection)                       => new(new System.Collections.Generic.List                    <T>           (collection));
    [PatchMethod(AggressiveInlining)] public static System.Collections.ObjectModel.ReadOnlyDictionary          <TKey, TValue> AsCopy<TKey, TValue>(this System.Collections.ObjectModel.ReadOnlyDictionary          <TKey, TValue> dictionary)                       => new(new System.Collections.Generic.Dictionary              <TKey, TValue>(dictionary));
    [PatchMethod(AggressiveInlining)] public static System.Collections.ObjectModel.ReadOnlyObservableCollection<T>            AsCopy<T>           (this System.Collections.ObjectModel.ReadOnlyObservableCollection<T>            collection)                       => new(new System.Collections.ObjectModel.ObservableCollection<T>           (collection));
    [PatchMethod(AggressiveInlining)] public static System.Collections.ObjectModel.ReadOnlyCollection          <T>            AsCopy<T>           (this System.Collections.ObjectModel.ReadOnlySet                 <T>            set)                              => new(new System.Collections.Generic.HashSet                 <T>           (set));
    [PatchMethod(AggressiveInlining)] public static System.Collections.SortedList                                             AsCopy              (this System.Collections.SortedList                                             sortedList)                       => (System.Collections.SortedList) sortedList.Clone();
    [PatchMethod(AggressiveInlining)] public static System.Collections.Specialized.HybridDictionary                           AsCopy              (this System.Collections.Specialized.HybridDictionary                           dictionary)                       => dictionary.AsCopy(false);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Specialized.HybridDictionary                           AsCopy              (this System.Collections.Specialized.HybridDictionary                           dictionary, bool caseInsensitive) { System.Collections.Specialized.HybridDictionary copy = new(dictionary.Count, caseInsensitive); foreach (System.Collections.DictionaryEntry element in dictionary) { copy.Add(element.Key, element.Value); } return copy; };
    [PatchMethod(AggressiveInlining)] public static System.Collections.Specialized.ListDictionary                             AsCopy              (this System.Collections.Specialized.ListDictionary                             dictionary)                       { System.Collections.Specialized.ListDictionary copy = new(); foreach (System.Collections.DictionaryEntry element in dictionary) { copy.Add(element.Key, element.Value); } return copy; }
    [PatchMethod(AggressiveInlining)] public static System.Collections.Specialized.NameValueCollection                        AsCopy              (this System.Collections.Specialized.NameValueCollection                        collection)                       => new(collection);
    [PatchMethod(AggressiveInlining)] public static System.Collections.Specialized.OrderedDictionary                          AsCopy              (this System.Collections.Specialized.OrderedDictionary                          dictionary)                       { System.Collections.Specialized.OrderedDictionary copy = new(dictionary.Count); foreach (System.Collections.DictionaryEntry element in dictionary) { copy.Add(element.Key, element.Value); } return copy; }
    [PatchMethod(AggressiveInlining)] public static System.Collections.Specialized.StringCollection                           AsCopy              (this System.Collections.Specialized.StringCollection                           collection)                       { System.Collections.Specialized.StringCollection copy = new(); string[] subcopy = new string[collection.Count]; collection.CopyTo(subcopy, 0); copy.AddRange(subcopy); return copy; }
    [PatchMethod(AggressiveInlining)] public static System.Collections.Specialized.StringDictionary                           AsCopy              (this System.Collections.Specialized.StringDictionary                           dictionary)                       { System.Collections.Specialized.StringDictionary copy = new(dictionary.Count); foreach (System.Collections.DictionaryEntry element in dictionary) { copy.Add(element.Key, element.Value); } return copy; }
    [PatchMethod(AggressiveInlining)] public static System.Collections.Stack                                                  AsCopy              (this System.Collections.Stack                                                  stack)                            => new(stack);
    [PatchMethod(AggressiveInlining)] public static System.IO.MemoryStream                                                    AsCopy              (this System.IO.MemoryStream                                                    stream)                           { try { return new(stream.GetBuffer(), 0, stream.Length, stream.CanWrite, true); } catch (System.UnauthorizedAccessException exception) {} return new(Util.ArrayFrom(stream), 0, stream.Length, stream.CanWrite, false); }
    [PatchMethod(AggressiveInlining)] public static System.Memory        <T>                                                  AsCopy<T>           (this System.Memory        <T>                                                  memory)                           => new(Util.ArrayFrom(memory));
    [PatchMethod(AggressiveInlining)] public static System.ReadOnlyMemory<T>                                                  AsCopy<T>           (this System.ReadOnlyMemory<T>                                                  memory)                           => new(Util.ArrayFrom(memory));

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

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectEnumerator EnumerateChildren   (this UnityEngine.GameObject gameObject) => new(GameObjectEnumerator.Kind.Children,    gameObject.transform);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectEnumerator EnumerateDescendants(this UnityEngine.GameObject gameObject) => new(GameObjectEnumerator.Kind.Descendants, gameObject.transform);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectEnumerator EnumerateHierarchy  (this UnityEngine.GameObject gameObject) => new(GameObjectEnumerator.Kind.Hierarchy,   gameObject.transform);

    [PatchMethod(AggressiveInlining)] public static T                     EnsureComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => (T) gameObject.EnsureComponent(typeof(T));
    [PatchMethod(AggressiveInlining)] public static UnityEngine.Component EnsureComponent   (this UnityEngine.GameObject gameObject, System.Type type)               { UnityEngine.Component? component = gameObject.GetComponent(type); return null != component ? component : gameObject.AddComponent(type); }

    [PatchMethod(AggressiveInlining)]
    public static bool Exists<T>(this T[] array, System.Predicate<T> predicate) {
      return System.Array.Exists<T>(array, predicate);
    }

    [PatchMethod(AggressiveInlining)]
    public static T? Find<T>(this T[] array, System.Predicate<T> predicate) {
      return System.Array.Find<T>(array, predicate);
    }

    [PatchMethod(AggressiveInlining)]
    public static T[] FindAll<T>(this T[] array, System.Predicate<T> predicate) {
      return System.Array.FindAll<T>(array, predicate);
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
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                     FindChildrenByComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject          .FindChildrenByComponent(typeof(T)).ConvertAll([PatchMethod(AggressiveInlining)] static child => (T) child);
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
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                     FindDescendantsByComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject          .FindDescendantsByComponent(typeof(T)).ConvertAll([PatchMethod(AggressiveInlining)] static descendant => (T) descendant);
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
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                     FindHierarchyByComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject          .FindHierarchyByComponent(typeof(T)).ConvertAll([PatchMethod(AggressiveInlining)] static _ => (T) _);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component> FindHierarchyByComponent   (this UnityEngine.Component  component,  System.Type type)               => component.gameObject.FindHierarchyByComponent(type);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component> FindHierarchyByComponent   (this UnityEngine.GameObject gameObject, System.Type type)               { GameObjectSharedList<UnityEngine.Component> hierarchy = new((uint) gameObject.transform.childCount); hierarchy.Clear(); foreach (UnityEngine.GameObject child in gameObject.EnumerateHierarchy()) { UnityEngine.Component? component = child.GetComponent(type); if (component is not null) hierarchy.Add(component); } return hierarchy; }

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindHierarchyByName(this UnityEngine.Component  component,  string name) => component.gameObject.FindHierarchyByName(name);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindHierarchyByName(this UnityEngine.GameObject gameObject, string name) { PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> hierarchy = new((uint) gameObject.transform.childCount); hierarchy.Clear(); foreach (UnityEngine.GameObject child in gameObject.EnumerateHierarchy()) { if (child.name == name) hierarchy.Add(child); } return hierarchy; }

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindHierarchyByTag(this UnityEngine.Component  component,  string tag) => component.gameObject.FindHierarchyByTag(tag);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> FindHierarchyByTag(this UnityEngine.GameObject gameObject, string tag) { PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> hierarchy = new((uint) gameObject.transform.childCount); hierarchy.Clear(); foreach (UnityEngine.GameObject child in gameObject.EnumerateHierarchy()) { if (child.tag == tag) hierarchy.Add(child); } return hierarchy; }

    [PatchMethod(AggressiveInlining)] public static int FindIndex<T>(this T[] array,                       System.Predicate<T> predicate) { return System.Array.FindIndex<T>(array, predicate); }
    [PatchMethod(AggressiveInlining)] public static int FindIndex<T>(this T[] array, int index,            System.Predicate<T> predicate) { return System.Array.FindIndex<T>(array, index, predicate); }
    [PatchMethod(AggressiveInlining)] public static int FindIndex<T>(this T[] array, int index, int count, System.Predicate<T> predicate) { return System.Array.FindIndex<T>(array, index, count, predicate); }

    [PatchMethod(AggressiveInlining)]
    public static T? FindLast<T>(this T[] array, System.Predicate<T> predicate) {
      return System.Array.FindLast<T>(array, predicate);
    }

    [PatchMethod(AggressiveInlining)] public static int FindLastIndex<T>(this T[] array,                       System.Predicate<T> predicate) { return System.Array.FindLastIndex<T>(array, predicate); }
    [PatchMethod(AggressiveInlining)] public static int FindLastIndex<T>(this T[] array, int index,            System.Predicate<T> predicate) { return System.Array.FindLastIndex<T>(array, index, predicate); }
    [PatchMethod(AggressiveInlining)] public static int FindLastIndex<T>(this T[] array, int index, int count, System.Predicate<T> predicate) { return System.Array.FindLastIndex<T>(array, index, count, predicate); }

    [PatchMethod(AggressiveInlining)]
    public static void ForEach<T>(this T[] array, System.Action<T> action) {
      System.Array.ForEach<T>(array, action);
    }

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component>  GetChildren   (this UnityEngine.Component  component)                                  => component .FindChildrenByComponent(component.GetType());
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      GetChildren<T>(this UnityEngine.Component  component) where T : UnityEngine.Component  => component .FindChildrenByComponent<T>();
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> GetChildren   (this UnityEngine.GameObject gameObject)                                 => gameObject.FindChildren   ([PatchMethod(AggressiveInlining)] static child => true);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      GetChildren<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject.FindChildren<T>([PatchMethod(AggressiveInlining)] static child => true);

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component>  GetDescendants   (this UnityEngine.Component  component)                                  => component .FindDescendantsByComponent(component.GetType());
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      GetDescendants<T>(this UnityEngine.Component  component) where T : UnityEngine.Component  => component .FindDescendantsByComponent<T>();
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> GetDescendants   (this UnityEngine.GameObject gameObject)                                 => gameObject.FindDescendants   ([PatchMethod(AggressiveInlining)] static child => true);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      GetDescendants<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject.FindDescendants<T>([PatchMethod(AggressiveInlining)] static child => true);

    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.Component>  GetHierarchy   (this UnityEngine.Component  component)                                  => component .FindHierarchyByComponent(component.GetType());
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      GetHierarchy<T>(this UnityEngine.Component  component) where T : UnityEngine.Component  => component .FindHierarchyByComponent<T>();
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<UnityEngine.GameObject> GetHierarchy   (this UnityEngine.GameObject gameObject)                                 => gameObject.FindHierarchy   ([PatchMethod(AggressiveInlining)] static child => true);
    [PatchMethod(AggressiveInlining)] public static PatchOdyssey.Collections.GameObjectSharedList<T>                      GetHierarchy<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => gameObject.FindHierarchy<T>([PatchMethod(AggressiveInlining)] static child => true);

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

    [PatchMethod(AggressiveInlining)] public static ref readonly object? Prepend   (this System.Collections.ArrayList             arrayList, in object? value) { arrayList.Insert  (0, value); return value; }
    [PatchMethod(AggressiveInlining)] public static ref readonly T       Prepend<T>(this System.Collections.ArrayList             arrayList, in T       value) { arrayList.Insert  (0, value); return value; }
    [PatchMethod(AggressiveInlining)] public static ref readonly T       Prepend<T>(this System.Collections.Generic.LinkedList<T> list,      in T       value) { list     .AddFirst(value);    return value; }
    [PatchMethod(AggressiveInlining)] public static ref readonly T       Prepend<T>(this System.Collections.Generic.List      <T> list,      in T       value) { list     .Insert  (0, value); return value; }

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
        case PatchOdyssey.Collections.RefDictionary<T> subdictionary: subdictionary.TrimExcess((uint) capacity); break;
        case System.Collections.Generic.Dictionary <T> subdictionary: subdictionary.TrimExcess(capacity);        break;
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

  public static partial class Util /* ⟶ Utilities */ {
    /* TODO */
    public   const double LoadAsynchronously = 0.0;
    public   const uint   LoadOnce           = 0u;
    public   const uint   LoadPersistently   = uint.MaxValue - 1u;
    public   const bool   LoadWithCache      = true;
    public   const bool   LoadWithoutCache   = false;
    public   const double LoadSynchronously  = double.PositiveInfinity;
    public   const int    MouseButtonLeft    = 0x0;   //
    public   const int    MouseButtonMiddle  = 0x2;   //
    public   const int    MouseButtonRight   = 0x1;   //
    internal const int    RefSize            = 8;     // ⟶ Presumed byte size of managed/ reference types as structured within class types (i.e. `sizeof(void*)`) — relative liberal guess to avoid object splicing

    private static readonly System.Collections.ObjectModel.ReadOnlyCollection<int>                                                                         MOUSE_BUTTONS          = new[] {Util.MouseButtonLeft, Util.MouseButtonRight, Util.MouseButtonMiddle}.AsReadOnly();
    private static readonly System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.KeyCode>                                                         KEYS                   = new[] {UnityEngine.KeyCode.A, UnityEngine.KeyCode.Alpha0, UnityEngine.KeyCode.Alpha1, UnityEngine.KeyCode.Alpha2, UnityEngine.KeyCode.Alpha3, UnityEngine.KeyCode.Alpha4, UnityEngine.KeyCode.Alpha5, UnityEngine.KeyCode.Alpha6, UnityEngine.KeyCode.Alpha7, UnityEngine.KeyCode.Alpha8, UnityEngine.KeyCode.Alpha9, UnityEngine.KeyCode.AltGr, UnityEngine.KeyCode.Ampersand, UnityEngine.KeyCode.Asterisk, UnityEngine.KeyCode.At, UnityEngine.KeyCode.B, UnityEngine.KeyCode.BackQuote, UnityEngine.KeyCode.Backslash, UnityEngine.KeyCode.Backspace, UnityEngine.KeyCode.Break, UnityEngine.KeyCode.C, UnityEngine.KeyCode.CapsLock, UnityEngine.KeyCode.Caret, UnityEngine.KeyCode.Clear, UnityEngine.KeyCode.Colon, UnityEngine.KeyCode.Comma, UnityEngine.KeyCode.D, UnityEngine.KeyCode.Delete, UnityEngine.KeyCode.Dollar, UnityEngine.KeyCode.DoubleQuote, UnityEngine.KeyCode.DownArrow, UnityEngine.KeyCode.E, UnityEngine.KeyCode.End, UnityEngine.KeyCode.Equals, UnityEngine.KeyCode.Escape, UnityEngine.KeyCode.Exclaim, UnityEngine.KeyCode.F, UnityEngine.KeyCode.F1, UnityEngine.KeyCode.F10, UnityEngine.KeyCode.F11, UnityEngine.KeyCode.F12, UnityEngine.KeyCode.F13, UnityEngine.KeyCode.F14, UnityEngine.KeyCode.F15, UnityEngine.KeyCode.F2, UnityEngine.KeyCode.F3, UnityEngine.KeyCode.F4, UnityEngine.KeyCode.F5, UnityEngine.KeyCode.F6, UnityEngine.KeyCode.F7, UnityEngine.KeyCode.F8, UnityEngine.KeyCode.F9, UnityEngine.KeyCode.G, UnityEngine.KeyCode.Greater, UnityEngine.KeyCode.H, UnityEngine.KeyCode.Hash, UnityEngine.KeyCode.Help, UnityEngine.KeyCode.Home, UnityEngine.KeyCode.I, UnityEngine.KeyCode.Insert, UnityEngine.KeyCode.J, UnityEngine.KeyCode.K, UnityEngine.KeyCode.Keypad0, UnityEngine.KeyCode.Keypad1, UnityEngine.KeyCode.Keypad2, UnityEngine.KeyCode.Keypad3, UnityEngine.KeyCode.Keypad4, UnityEngine.KeyCode.Keypad5, UnityEngine.KeyCode.Keypad6, UnityEngine.KeyCode.Keypad7, UnityEngine.KeyCode.Keypad8, UnityEngine.KeyCode.Keypad9, UnityEngine.KeyCode.KeypadDivide, UnityEngine.KeyCode.KeypadEnter, UnityEngine.KeyCode.KeypadEquals, UnityEngine.KeyCode.KeypadMinus, UnityEngine.KeyCode.KeypadMultiply, UnityEngine.KeyCode.KeypadPeriod, UnityEngine.KeyCode.KeypadPlus, UnityEngine.KeyCode.L, UnityEngine.KeyCode.LeftAlt, UnityEngine.KeyCode.LeftApple, UnityEngine.KeyCode.LeftArrow, UnityEngine.KeyCode.LeftBracket, UnityEngine.KeyCode.LeftCommand, UnityEngine.KeyCode.LeftControl, UnityEngine.KeyCode.LeftCurlyBracket, UnityEngine.KeyCode.LeftMeta, UnityEngine.KeyCode.LeftParen, UnityEngine.KeyCode.LeftShift, UnityEngine.KeyCode.LeftWindows, UnityEngine.KeyCode.Less, UnityEngine.KeyCode.M, UnityEngine.KeyCode.Menu, UnityEngine.KeyCode.Minus, UnityEngine.KeyCode.N, UnityEngine.KeyCode.Numlock, UnityEngine.KeyCode.O, UnityEngine.KeyCode.P, UnityEngine.KeyCode.PageDown, UnityEngine.KeyCode.PageUp, UnityEngine.KeyCode.Pause, UnityEngine.KeyCode.Percent, UnityEngine.KeyCode.Period, UnityEngine.KeyCode.Pipe, UnityEngine.KeyCode.Plus, UnityEngine.KeyCode.Print, UnityEngine.KeyCode.Q, UnityEngine.KeyCode.Question, UnityEngine.KeyCode.Quote, UnityEngine.KeyCode.R, UnityEngine.KeyCode.Return, UnityEngine.KeyCode.RightAlt, UnityEngine.KeyCode.RightApple, UnityEngine.KeyCode.RightArrow, UnityEngine.KeyCode.RightBracket, UnityEngine.KeyCode.RightCommand, UnityEngine.KeyCode.RightControl, UnityEngine.KeyCode.RightCurlyBracket, UnityEngine.KeyCode.RightMeta, UnityEngine.KeyCode.RightParen, UnityEngine.KeyCode.RightShift, UnityEngine.KeyCode.RightWindows, UnityEngine.KeyCode.S, UnityEngine.KeyCode.ScrollLock, UnityEngine.KeyCode.Semicolon, UnityEngine.KeyCode.Slash, UnityEngine.KeyCode.Space, UnityEngine.KeyCode.SysReq, UnityEngine.KeyCode.T, UnityEngine.KeyCode.Tab, UnityEngine.KeyCode.Tilde, UnityEngine.KeyCode.U, UnityEngine.KeyCode.Underscore, UnityEngine.KeyCode.UpArrow, UnityEngine.KeyCode.V, UnityEngine.KeyCode.W, UnityEngine.KeyCode.X, UnityEngine.KeyCode.Y, UnityEngine.KeyCode.Z}.AsReadOnly();
    private static readonly System.Collections.Generic    .Dictionary        <(System.Type, System.Type), System.Delegate>                                 DELEGATED_CONVERTS     = new(1);
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
    [PatchMethod(AggressiveInlining)] public static object[]     ArrayFrom   ()                                                             =>                 System.Array.Empty<object>();
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>()                                                             =>                 System.Array.Empty<T>     ();
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(T[]                                             array)        =>        array ?? System.Array.Empty<T>     ();
    [PatchMethod(AggressiveInlining)] public static System.Array ArrayFrom   (System.Array                                    array)        =>        array ?? System.Array.Empty<object>();
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(System.Array                                    array)        => (T[]) (array ?? System.Array.Empty<T>     ());
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(System.ArraySegment<T>                          arraySegment) => arraySegment.ToArray();
    [PatchMethod(AggressiveInlining)] public static object[]     ArrayFrom   (System.Collections.ArrayList                    arrayList)    =>       arrayList.ToArray();
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(System.Collections.ArrayList                    arrayList)    => (T[]) arrayList.ToArray(typeof(T));
    [PatchMethod(AggressiveInlining)] public static bool  []     ArrayFrom   (System.Collections.BitArray                     bits)         { bool[] array = new bool[bits.Length]; uint length = 0u; foreach (bool bit in bits) { array[length++] = bit; } return array; }
    [PatchMethod(AggressiveInlining)] public static object[]     ArrayFrom   (System.Collections.IEnumerable                  enumerable)   { System.Collections.Generic.Queue<object> array = new(); for (System.Collections.IEnumerator enumerator = enumerable.GetEnumerator(); enumerator.MoveNext(); ) array.Enqueue(enumerator.Current); return Util.ArrayFrom(array); }
    [PatchMethod(AggressiveInlining)] public static object[]     ArrayFrom   (System.Collections.Queue                        queue)        => queue.ToArray();
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(System.Collections.Generic.IEnumerable<T>       enumerable)   { System.Collections.Generic.Queue<T> array = new(); using System.Collections.Generic.IEnumerator<T> enumerator = enumerable.GetEnumerator(); while (enumerator.MoveNext()) { array.Enqueue(enumerator.Current); } return Util.ArrayFrom(array); }
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(System.Collections.Generic.List       <T>       list)         => list .ToArray();
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(System.Collections.Generic.Queue      <T>       queue)        => queue.ToArray();
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(System.Collections.Generic.Stack      <T>       stack)        => stack.ToArray();
    [PatchMethod(AggressiveInlining)] public static string[]     ArrayFrom   (System.Collections.Specialized.StringCollection collection)   { string[] array = new string[collection.Count]; uint length = 0u; foreach (string element in collection) { array[length++] = element; } return array; }
    [PatchMethod(AggressiveInlining)] public static object[]     ArrayFrom   (System.Collections.Stack                        stack)        => stack .ToArray();
    [PatchMethod(AggressiveInlining)] public static byte  []     ArrayFrom   (System.IO.MemoryStream                          stream)       => stream.ToArray();
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(System.Memory        <T>                        memory)       => memory.ToArray();
    [PatchMethod(AggressiveInlining)] public static T     []     ArrayFrom<T>(System.ReadOnlyMemory<T>                        memory)       => memory.ToArray();

    public static T[] ArrayFrom<T>(System.Collections.Generic.IEnumerable<T> enumerableA, System.Collections.Generic.IEnumerable<T> enumerableB) {
      if (enumerableB is null) return enumerableA is not null ? Util.ArrayFrom(enumerableA) : System.Array.Empty<T>();
      if (enumerableA is null) return enumerableB is not null ? Util.ArrayFrom(enumerableB) : System.Array.Empty<T>();

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
        if (concatenation is null)                                           return System.Array.Empty<T>();
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

    [PatchMethod(AggressiveInlining)] public static ref T ArrayReference  <T>(in T[] array)             { return ref System.Runtime.InteropServices.MemoryMarshal.GetReference(new System.Span<T>(array)); }
    [PatchMethod(AggressiveInlining)] public static ref T ArrayReferenceAt<T>(in T[] array, uint index) { return ref System.Runtime.CompilerServices.Unsafe.Add(ref Util.ArrayReference(in array), (int) index); }

    [PatchMethod(AggressiveInlining)] public static uint CheckWait            () => Util.CheckWaitForCoroutine();
    [PatchMethod(AggressiveInlining)] public static uint CheckWaitForCoroutine() => 0u;

    public static uint CheckWaitForTimer() {
      uint                              count        = 0u;
      double                            timestamp    = UnityEngine.Time.realtimeSinceStartupAsDouble;
      PatchOdyssey.Collections.WaitInfo wait         = PatchOdyssey.Collections.WaitInfo.WAITS[double.NaN];

      // … ⟶ Enumeration is messy because the final design could not succinctly account for a timer-based model
      foreach (PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Collections.WaitEvent> waitHandler in wait.handlers.AsCopy()) {
        (double waitDelay, double waitTimestamp) = waitHandler.metadata.data;

        // …
        if (!(timestamp < waitTimestamp)) {
          if (double.IsNaN(waitDelay) || 0.0 >= waitDelay) { ++count; wait.handlers.Remove(waitHandler); }                                                                                                  // ⟶ Remove `WaitForTimerUntil(…)` handlers, or
          else wait.handlers[wait.handlers.IndexOf(waitHandler)] = new(waitHandler.value, waitHandler.target, new() {callback = waitHandler.metadata.callback, data = (waitDelay, timestamp + waitDelay)}); // ⟶ Update `WaitForTimerEvery(…)` handlers

          waitHandler.metadata.data = (System.Math.Abs(waitDelay), waitHandler.metadata.data.timestamp);
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

        return null;
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

    /* TODO */
    private static T? LoadUri<T>(System.Uri path, PatchOdyssey.Handler<PatchOdyssey.Collections.LoadEvent> callback, double timeout, bool cached, uint retries, System.Func<System.Uri, UnityEngine.Networking.UnityWebRequest> requester, System.Func<UnityEngine.Networking.UnityWebRequest, T?> loader, System.Func<T, T> recacher, PatchOdyssey.Handler<PatchOdyssey.Collections.LoadEvent> fallback, bool restarted) /* where T : class? */ {
      uint                                                                     attemptsAllowed = retries - (uint.MaxValue == retries ? 1u : 0u) + 1u;
      T[]?                                                                     cachedPayload   = null;
      PatchOdyssey.Collections.LoadInfo                                        load            = PatchOdyssey.Collections.LoadInfo.LOADS.TryAppend((typeof(T), path), new());
      PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Collections.LoadEvent> loadHandler     = restarted ? load.handlers[0] : default;
      uint                                                                     attempts        = restarted ? loadHandler.metadata.data.attempts : 1u;
      UnityEngine.Networking.UnityWebRequestAsyncOperation                     operation       = null!; // ⟶ Able to access the `UnityEngine.Application.streamingAssetsPath` directory
      bool                                                                     requested       = restarted;
      bool                                                                     requestFailed   = false; // ⟶ `UnityEngine.Networking.UnityWebRequest.Result.*Error`, `timeout`, …, e.t.c.
      System.Diagnostics.Stopwatch                                             stopwatch       = new();

      [PatchMethod(AggressiveInlining)]
      static void ClearAsynchronousCached(in PatchOdyssey.Collections.LoadInfo load, ref T[]? cachedPayload, System.Func<T, T> recacher) {
        foreach (PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Collections.LoadEvent> loadHandler in load.handlers.AsCopy())
        if (loadHandler.metadata.data.cached) {
          (uint attempts, uint attemptsAllowed, bool cached, double duration, System.Uri path, object? payload) data = loadHandler.metadata.data;

          // …
          data.duration = UnityEngine.Time.realtimeSinceStartupAsDouble - data.duration;
          data.payload  = (cachedPayload ??= new[] {recacher((T) load.cached)})[0];

          load.handlers.Remove(loadHandler);
          loadHandler.value(null, new() {callback = loadHandler.value, data = data});
        }
      }

      [PatchMethod(AggressiveInlining)]
      static ref T GetCache(ref T[]? cachedPayload, System.Func<T, T> recacher) {
        cachedPayload ??= new[] {recacher((T) load.cached)};
        return ref cachedPayload[0];
      }

      void LoadAsynchronousUri(UnityEngine.AsyncOperation _) /* ⟶ Captures `load` and `loadHandler` */ {
        UnityEngine.Networking.UnityWebRequestAsyncOperation operation                                         = (UnityEngine.Networking.UnityWebRequestAsyncOperation) _;
        UnityEngine.Networking.UnityWebRequest               request                                           = operation.webRequest; // ⟶ `request.uri` could be modified through redirection
        (uint attempts, uint attemptsAllowed, bool cached, double timestamp, System.Uri path, object? payload) = loadHandler.metadata.data;

        /* TODO */
        // …
        if (UnityEngine.Networking.UnityWebRequest.Result.Success != request.result) {
          request.Dispose();

          if (attempts != attemptsAllowed) {
            operation   = requester(path).SendWebRequest();
            loadHandler = load.handlers[load.handlers.IndexOf(loadHandler)] = new(loadHandler.value, operation, new() {callback = loadHandler.metadata.callback, data = (attempts + 1u, attemptsAllowed, cached, timestamp, path, payload)});

            operation.completed += LoadAsynchronousUri;
          }

          else {
            load.payload = null;

            foreach (PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Collections.LoadEvent> loadHandler in load.handlers.AsCopy()) {
              // recurse here
            }
          }
        }

        else {
          // onsuccess?
          request.Dispose();
        }
      }

      [PatchMethod(AggressiveInlining)]
      static void ReloadAsynchronousUri(in PatchOdyssey.Collections.LoadInfo load) /* ⟶ Use pending asynchronous handler to get payload (ideally does not raise a `System.StackOverflowException`) */ {
        PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Collections.LoadEvent> loadHandler = load.handlers[0];
        Util.LoadUri<T>(loadHandler.metadata.data.path, loadHandler.value, Util.LoadAsynchronously, loadHandler.metadata.data.cached, loadHandler.metadata.data.attemptsAllowed - 1u, requester, loader, recacher, loadHandler.metadata.callback, true);
      }

      // … ⟶ Get (available) cached payload
      if (load.cached is not null) {
        ClearAsynchronousCached(load, ref cachedPayload, recacher);

        if (cached) {
          if      (!restarted)               callback(null, new() {callback = callback, data = (attempts, attemptsAllowed, true, 0.0, path, GetCache(ref cachedPayload, recacher))}); // ⟶ Otherwise already cleared and invoked via `ClearAsynchronousCached(…)`
          else if (0 != load.handlers.Count) ReloadAsynchronousUri(load); // ⟶ TODO: Fancy comment about eager immediate hit not locking up the load queue

          return GetCache(ref cachedPayload, recacher);
        }
      }

      // … ⟶ Get payload
      do {
        stopwatch.Restart();

        // … ⟶ Wait until prior handler is complete
        if (!requested && !restarted && 0 != load.handlers.Count) {
          // … ⟶ `Util.LoadAsynchronously` — Handler awaits completion of predecessor
          if (double.IsNaN(timeout) || 0.0 >= timeout) {
            load.handlers.Add (new(callback, null, new() {callback = fallback, data = (attempts, attemptsAllowed, cached, UnityEngine.Time.realtimeSinceStartupAsDouble, path, null)}));
            stopwatch    .Stop();

            return null;
          }

          // … ⟶ `Util.LoadSynchronously` — Handler blocks until completion of predecessor
          else {
            while (0 != load.handlers.Count && stopwatch.Elapsed.TotalSeconds < timeout)                               continue;
            if    (attempts != attemptsAllowed && (load.payload is null || stopwatch.Elapsed.TotalSeconds >= timeout)) continue;

            stopwatch.Stop();
            (load.payload is not null ? callback : fallback)(null, new() {callback = callback, data = (attempts, attemptsAllowed, cached, stopwatch.Elapsed.TotalSeconds, path, load.payload)});

            return load.payload;
          }
        }

        // … ⟶ Future handlers will wait on this handler to complete
        operation     = requester(path).SendWebRequest(); // ⟶ Was unaware of `int UnityEngine.Networking.UnityWebRequest::timeout` beforehand
        requestFailed = false;

        if (requested)
          // ⟶ Update the handler’s `.target`
          loadHandler = load.handlers[restarted ? 0u : load.handlers.IndexOf(loadHandler)] = new(loadHandler.value, operation, new() {callback = loadHandler.metadata.callback, data = (attempts, attemptsAllowed, cached, loadHandler.metadata.data.duration, path, null)});

        else {
          loadHandler = load.handlers.Append(new(callback, operation, new() {callback = fallback, data = (attempts, attemptsAllowed, cached, UnityEngine.Time.realtimeSinceStartupAsDouble, path, null)}));
          requested   = true;
        }

        for (UnityEngine.Networking.UnityWebRequest request = operation.webRequest; UnityEngine.Networking.UnityWebRequest.Result.Success != request.result; ) {
          if (UnityEngine.Networking.UnityWebRequest.Result.ConnectionError == request.result || UnityEngine.Networking.UnityWebRequest.Result.DataProcessingError == request.result || UnityEngine.Networking.UnityWebRequest.Result.ProtocolError == request.result) {
            requestFailed = true;
            request.Dispose();

            break;
          }

          if (UnityEngine.Networking.UnityWebRequest.Result.InProgress == request.result) {
            // … ⟶ `Util.LoadAsynchronously` — ???
            if (double.IsNaN(timeout) || 0.0 >= timeout) {
              operation.completed += LoadAsynchronousUri;
              return null;
            }

            // … ⟶ `Util.LoadSynchronously` — ???
            // if (stopwatch.Elapsed.TotalSeconds < timeout)
            // continue; // ⟶ Wait until `request.result` is successful
          }
        }

        // onsuccess? HandleURIRequest(load, request);

        if (requestFailed)
        continue;
      } while (attempts++ != attemptsAllowed);

      // … ⟶ Allow pending handlers to get payload (or available cache)
      if (requestFailed) {
        load.payload = null;

        foreach (PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Collections.LoadEvent> loadHandler in load.handlers.AsCopy()) {
          (uint attempts, uint attemptsAllowed, bool cached, double timestamp, System.Uri path, object? payload) data     = loadHandler.metadata.data;
          ref PatchOdyssey.Handler<PatchOdyssey.Collections.LoadEvent>                                           fallback = ref loadHandler.metadata.callback;

          // … ⟶ Invoke currently failed and asynchronous pending handlers, otherwise update remaining pending
          if (data.attempts == data.attemptsAllowed || loadHandler.target == operation) {
            data.attempts = loadHandler.target == operation ? attempts : data.attempts;
            data.duration = UnityEngine.Time.realtimeSinceStartupAsDouble - data.duration;

            load.handlers.Remove(loadHandler);
            fallback(loadHandler.target, new() {callback = loadHandler.value, data = data});
          }

          // … ⟶ — otherwise update remaining pending handlers
          else {
            data.attempts                                    += 1u;
            load.handlers[load.handlers.IndexOf(loadHandler)] = new(loadHandler.value, loadHandler.target, new() {callback = fallback, data = data});
          }
        }

        // … ⟶ Use pending asynchronous handler to get payload (ideally does not raise a `System.StackOverflowException`)
        if (0 != load.handlers.Count) {
          loadHandler = load.handlers[0];
          Util.LoadUri<T>(path, loadHandler.value, Util.LoadAsynchronously, loadHandler.metadata.data.cached, loadHandler.metadata.data.attemptsAllowed - 1u, requester, loader, recacher, loadHandler.metadata.callback, true);
        }
      }

      // LoadUri("haha.txt", (target = operation, loadEvent) => { loadEvent.callback; loadEvent.data.payload; }, Util.LoadSynchronously, Util.LoadWithCache, Util.LoadOnce, 𝑓, 𝑓, 𝑓, (target, loadEvent) => {})
      return null;
    }

    public static byte[]? LoadUri(string path, System.Action<byte[]?>? callback = null, double? timeout = null, bool cached = Util.LoadWithCache) {
      return Util.LoadUri(
        typeof(byte[]).ToString(), path, callback is null ? static _ => {} : _ => callback(_ as byte[]), timeout, cached,
        static path    => UnityEngine.Networking.UnityWebRequest.Get(path),
        static request => request.downloadHandler.data,
        static predata => predata, // ⟶ `(predata as byte[]).Clone() as byte[]`
        false
      ) as byte[];
    }

    // public static UnityEngine.AudioClip? LoadUriAsAudioClip(string path, System.Action<UnityEngine.AudioClip?>? callback = null, double? timeout = null, bool cached = Util.LoadWithCache, UnityEngine.AudioType? encoding = null) {
    //   return Util.LoadUri(
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

    // public static string? LoadUriAsText(string path, System.Action<string?>? callback = null, double? timeout = null, bool cached = Util.LoadWithCache, System.Text.Encoding? encoding = null) {
    //   return Util.LoadUri(
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

    // public static UnityEngine.Texture2D? LoadUriAsTexture2D(string path, System.Action<UnityEngine.Texture2D?>? callback = null, double? timeout = null, bool cached = Util.LoadWithCache) {
    //   return Util.LoadUri(
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

    // public static void PreloadURI           (string path)                                         => Util.LoadUri           (path, null, Util.LoadAsynchronously, Util.LoadWithCache);
    // public static void PreloadURIAsAudioClip(string path, UnityEngine.AudioType? encoding = null) => Util.LoadUriAsAudioClip(path, null, Util.LoadAsynchronously, Util.LoadWithCache, encoding);
    // public static void PreloadURIAsText     (string path, System.Text.Encoding?  encoding = null) => Util.LoadUriAsText     (path, null, Util.LoadAsynchronously, Util.LoadWithCache, encoding);
    // public static void PreloadURIAsTexture2D(string path)                                         => Util.LoadUriAsTexture2D(path, null, Util.LoadAsynchronously, Util.LoadWithCache);

    public static UnityEngine.Rect RectFromCorners(UnityEngine.Vector3[] corners) {
      // ⟶ Origin begins from bottom-left rather than top-left
      return new(corners[0].x, corners[0].y, corners[3].x - corners[0].x, corners[1].y - corners[0].y);
    }

    [PatchMethod(AggressiveInlining)]
    public static ref T Reference<T>() {
      return ref System.Runtime.InteropServices.MemoryMarshal.GetReference(new T[] {default!});
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

    [PatchMethod(AggressiveInlining)]
    public static uint StopWaitForCoroutine() {
      // … ⟶ Skip past the non-coroutine timer-based `double.NaN` entry — which is always sorted as first
      using (System.Collections.Generic.SortedDictionary<double, PatchOdyssey.Collections.WaitInfo>.ValueCollection.Enumerator enumerator = Util.EnumeratorMoveTo(PatchOdyssey.Collections.WaitInfo.WAITS.Values.GetEnumerator(), 1u, false))
      for (uint count = 0u; ; ++count) {
        if (enumerator.MoveNext()) {
          enumerator.Current.handlers.Clear();
          continue;
        }

        return count;
      }
    }

    [PatchMethod(AggressiveInlining)]
    public static uint StopWaitForCoroutine(PatchOdyssey.Handler<PatchOdyssey.Collections.WaitEvent> callback) {
      // … ⟶ Skip past the non-coroutine timer-based `double.NaN` entry — which is always sorted as first
      using (System.Collections.Generic.SortedDictionary<double, PatchOdyssey.Collections.WaitInfo>.ValueCollection.Enumerator enumerator = Util.EnumeratorMoveTo(PatchOdyssey.Collections.WaitInfo.WAITS.Values.GetEnumerator(), 1u, false))
      for (uint count = 0u; ; ) {
        if (enumerator.MoveNext()) {
          for (int index = enumerator.Current.handlers.Count; 0 != index--; )
          if (callback == enumerator.Current.handlers[index].value) {
            ++count;
            enumerator.Current.handlers.RemoveAt(index);
          }

          continue;
        }

        return count;
      }
    }

    [PatchMethod(AggressiveInlining)]
    public static uint StopWaitForCoroutine(double delay, PatchOdyssey.Handler<PatchOdyssey.Collections.WaitEvent> callback) {
      uint count = 0u;

      // …
      if (!double.IsNaN(delay) && PatchOdyssey.Collections.WaitInfo.WAITS.TryGetValue(delay, out PatchOdyssey.Collections.WaitInfo wait)) {
        for (int index = wait.handlers.Count; 0 != index--; )
        if (callback == wait.handlers[index].value) {
          ++count;
          wait.handlers.RemoveAt(index);
        }
      }

      return count;
    }

    [PatchMethod(AggressiveInlining)]
    public static uint StopWaitForTimer() {
      PatchOdyssey.Collections.WaitInfo.WAITS[double.NaN].handlers.Clear();
    }

    [PatchMethod(AggressiveInlining)]
    public static uint StopWaitForTimer(PatchOdyssey.Handler<PatchOdyssey.Collections.WaitEvent> callback) {
      uint                                                                                   count        = 0u;
      ref readonly PatchOdyssey.Collections.EventHandler<PatchOdyssey.Collections.WaitEvent> waitHandlers = ref PatchOdyssey.Collections.WaitInfo.WAITS[double.NaN].handlers;

      // …
      for (int index = waitHandlers.Count; 0 != index--; )
      if (callback == waitHandlers[index].value) {
        ++count;
        waitHandlers.RemoveAt(index);
      }

      return count;
    }

    [PatchMethod(AggressiveInlining)]
    public static uint StopWaitForTimer(double delay, PatchOdyssey.Handler<PatchOdyssey.Collections.WaitEvent> callback) {
      uint                                                                                   count        = 0u;
      ref readonly PatchOdyssey.Collections.EventHandler<PatchOdyssey.Collections.WaitEvent> waitHandlers = ref PatchOdyssey.Collections.WaitInfo.WAITS[double.NaN].handlers;

      // …
      for (int index = waitHandlers.Count; 0 != index--; )
      if (callback == waitHandlers[index].value && delay == waitHandlers[index].metadata.delay) {
        ++count;
        waitHandlers.RemoveAt(index);
      }

      return count;
    }

    public static object? Switch<T>(in T value, System.Collections.Generic.Dictionary<T, object> expression, object? fallback = null) => Util.Switch<T>(value, (System.Collections.Generic.IReadOnlyDictionary<T, object>) expression, fallback);
    public static object? Switch<T>(in T value, System.Collections.Generic.IReadOnlyDictionary<T, object> expression, object? fallback = null) {
      return expression?.TryGetValue(value, out object callback) ?? false ? callback : fallback;
    }

    [PatchMethod(AggressiveInlining)]
    public static void WaitEvery(double delay, PatchOdyssey.Handler<PatchOdyssey.Collections.WaitEvent> callback) => Util.WaitForCoroutineEvery(delay, callback);

    private static void WaitForCoroutine(double delay, PatchOdyssey.Handler<PatchOdyssey.Collections.WaitEvent> callback, bool forever) {
      [PatchMethod(AggressiveInlining)]
      static System.Collections.IEnumerator EnumerateRoutine(double delay, bool forever) {
        do {
          yield return new UnityEngine.WaitForSecondsRealtime((float) delay);

          if (0u == PatchOdyssey.Collections.WaitInfo.WAITS[delay].handlers.handlers.Count)
          break;

          PatchOdyssey.Collections.WaitInfo.WAITS[delay].handlers.Invoke();
        } while (forever);

        PatchOdyssey.Collections.WaitInfo.WAIT.StopCoroutine(wait.coroutine);
      }

      if (PatchOdyssey.Collections.WaitInfo.WAITS.TryGetValue(delay, out PatchOdyssey.Collections.WaitInfo wait))
        wait.handlers.Add(new PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Collections.WaitEvent>(callback, wait.coroutine, new() {callback = callback, data = (delay, UnityEngine.Time.realtimeSinceStartupAsDouble + delay)}));

      else {
        UnityEngine.Coroutine coroutine = PatchOdyssey.Collections.WaitInfo.WAIT.StartCoroutine(EnumerateRoutine(delay, forever));
        PatchOdyssey.Collections.WaitInfo.WAITS.Add(delay, new(coroutine, new new PatchOdyssey.Collections.HandlerInfo<PatchOdyssey.Collections.WaitEvent>(callback, coroutine, new() {callback = callback, data = (delay, UnityEngine.Time.realtimeSinceStartupAsDouble + delay)})));
      }
    }

    [PatchMethod(AggressiveInlining)] public static void WaitForCoroutineEvery(double delay, PatchOdyssey.Handler<PatchOdyssey.Collections.WaitEvent> callback) => Util.WaitForCoroutine(delay, callback, true);
    [PatchMethod(AggressiveInlining)] public static void WaitForCoroutineUntil(double delay, PatchOdyssey.Handler<PatchOdyssey.Collections.WaitEvent> callback) => Util.WaitForCoroutine(delay, callback, false);
    [PatchMethod(AggressiveInlining)] public static void WaitForTimerEvery    (double delay, PatchOdyssey.Handler<PatchOdyssey.Collections.WaitEvent> callback) => PatchOdyssey.Collections.WaitInfo.WAITS[double.NaN].handlers.Add(new(callback, null, new() {callback = callback, data = (+delay, UnityEngine.Time.realtimeSinceStartupAsDouble + delay)}));
    [PatchMethod(AggressiveInlining)] public static void WaitForTimerUntil    (double delay, PatchOdyssey.Handler<PatchOdyssey.Collections.WaitEvent> callback) => PatchOdyssey.Collections.WaitInfo.WAITS[double.NaN].handlers.Add(new(callback, null, new() {callback = callback, data = (-delay, UnityEngine.Time.realtimeSinceStartupAsDouble + delay)}));
    [PatchMethod(AggressiveInlining)] public static void WaitUntil            (double delay, PatchOdyssey.Handler<PatchOdyssey.Collections.WaitEvent> callback) => Util.WaitForCoroutineUntil(delay, callback);

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
