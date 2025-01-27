#nullable enable annotations

namespace PatchOdyssey {
  public class ReadOnlyAttribute : UnityEngine.PropertyAttribute {}

  [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.ReadOnlyAttribute))]
  public class ReadOnlyDrawer : UnityEditor.PropertyDrawer {
    public override float GetPropertyHeight(UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) {
      return UnityEditor.EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(UnityEngine.Rect position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) {
      UnityEngine.GUI.enabled = false;
      UnityEditor.EditorGUI.PropertyField(position, property, label, true);
      UnityEngine.GUI.enabled = true;
    }
  }

  [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false, Inherited = false)]
  public sealed class ReadWriteAttribute : System.Attribute {}

  [System.Serializable]
  public class SerializedDictionary<TKey, TValue> : System.Collections.Generic.IDictionary<TKey, TValue> {
    public struct Enumerator : System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>> {
      public           System.Collections.Generic.KeyValuePair<TKey, TValue> Current => this.current;
      private          System.Collections.Generic.KeyValuePair<TKey, TValue> current;
      private readonly PatchOdyssey.SerializedDictionary      <TKey, TValue> dictionary;
      private          int                                                   index;
      object                                                                 System.Collections.IEnumerator.Current => Current;
      public           int                                                   version;

      /* … */
      public void Dispose() {}

      internal Enumerator(PatchOdyssey.SerializedDictionary<TKey, TValue> dictionary) {
        this.current    = default;
        this.dictionary = dictionary;
        this.index      = 0;
        this.version    = dictionary.version;
      }

      public bool MoveNext() {
        if (this.dictionary.version != this.version)
        throw new System.InvalidOperationException($"Dictionary version {this.version} must be the same as Enumerator version {this.dictionary.version}");

        for (; this.dictionary.count > this.index; ++this.index)
        if (this.dictionary.hashes[this.index] >= 0) {
          this.current = new(this.dictionary.keys[index], this.dictionary.values[index]);
          this.index  += 1;

          return true;
        }

        this.current = default;
        this.index   = this.dictionary.count + 1;

        return false;
      }

      void System.Collections.IEnumerator.Reset() {
        if (this.dictionary.version != this.version)
        throw new System.InvalidOperationException($"Dictionary version {this.version} must be the same as Enumerator version {this.dictionary.version}");

        this.current = default;
        this.index   = 0;
      }
    }

    /* … */
    public                                                            System.Collections.Generic.Dictionary<TKey, TValue> AsDictionary => new System.Collections.Generic.Dictionary<TKey, TValue>(this);
    [UnityEngine.HideInInspector, UnityEngine.SerializeField] private int[]                                               buckets      =  null;
    private readonly                                                  System.Collections.Generic.IEqualityComparer<TKey>  comparer     =  System.Collections.Generic.EqualityComparer<TKey>.Default;
    public                                                            int                                                 Count        => this.count - this.freeCount;
    [UnityEngine.HideInInspector, UnityEngine.SerializeField] private int                                                 count        =  0;
    [UnityEngine.HideInInspector, UnityEngine.SerializeField] private int                                                 freeCount    =  0;
    [UnityEngine.HideInInspector, UnityEngine.SerializeField] private int                                                 freeList     =  0;
    [UnityEngine.HideInInspector, UnityEngine.SerializeField] private int[]                                               hashes       =  null;
    public                                                            bool                                                IsReadOnly   => false;
    public                                                            System.Collections.Generic.ICollection<TKey>        Keys         { get { TKey[] keys = new TKey[this.Count]; System.Array.Copy(this.keys, 0, keys, 0, this.Count); return keys; } }
    [UnityEngine.HideInInspector, UnityEngine.SerializeField] private TKey[]                                              keys         = null;
    [UnityEngine.HideInInspector, UnityEngine.SerializeField] private int []                                              next         = null;
    public                                                            System.Collections.Generic.ICollection<TValue>      Values       { get { TValue[] values = new TValue[this.Count]; System.Array.Copy(this.values, 0, values, 0, this.Count); return values; } }
    [UnityEngine.HideInInspector, UnityEngine.SerializeField] private TValue[]                                            values       = null;
    [UnityEngine.HideInInspector, UnityEngine.SerializeField] public  int                                                 version      = 0;

    /* … */
    public SerializedDictionary()                                                                : this(0,          null)     {}
    public SerializedDictionary(int                                                  capacity)   : this(capacity,   null)     {}
    public SerializedDictionary(System.Collections.Generic.IEqualityComparer<TKey>   comparer)   : this(0,          comparer) {}
    public SerializedDictionary(System.Collections.Generic.IDictionary<TKey, TValue> dictionary) : this(dictionary, null)     {}

    public SerializedDictionary(int capacity, System.Collections.Generic.IEqualityComparer<TKey> comparer) {
      if (capacity < 0)
        throw new System.ArgumentOutOfRangeException($"SerializedDictionary capacity is less than 0");

      this.Initialize(capacity);
      this.comparer = comparer ?? System.Collections.Generic.EqualityComparer<TKey>.Default;
    }

    public SerializedDictionary(System.Collections.Generic.IDictionary<TKey, TValue> dictionary, System.Collections.Generic.IEqualityComparer<TKey> comparer) : this(null != dictionary ? dictionary.Count : 0, comparer) {
      if (null == dictionary)
      throw new System.ArgumentNullException($"SerializedDictionary dictionary is null");

      foreach (System.Collections.Generic.KeyValuePair<TKey, TValue> current in dictionary)
      this.Add(current.Key, current.Value);
    }

    /* … */
    public void Add(System.Collections.Generic.KeyValuePair<TKey, TValue> item) {
      this.Add(item.Key, item.Value);
    }

    public void Add(TKey key, TValue value) {
      this.Insert(key, value, true);
    }

    public void Clear() {
      if (this.count <= 0)
      return;

      for (int index = 0; index != this.buckets.Length; ++index)
        this.buckets[index] = -1;

      System.Array.Clear(this.hashes, 0, this.count);
      System.Array.Clear(this.keys,   0, this.count);
      System.Array.Clear(this.next,   0, this.count);
      System.Array.Clear(this.values, 0, this.count);

      this.count     = 0;
      this.freeCount = 0;
      this.freeList  = -1;
      this.version  += 1;
    }

    public bool Contains(System.Collections.Generic.KeyValuePair<TKey, TValue> item) {
      int index = this.FindIndex(item.Key);
      return index >= 0 && System.Collections.Generic.EqualityComparer<TValue>.Default.Equals(this.values[index], item.Value);
    }

    public bool ContainsKey(TKey key) {
      return this.FindIndex(key) >= 0;
    }

    public bool ContainsValue(TValue value) {
      System.Func<TValue?, TValue?, bool> comparer = null == value ? (x, y) => null == x : System.Collections.Generic.EqualityComparer<TValue>.Default.Equals;

      // …
      for (int index = 0; index != this.count; ++index) {
        if (this.hashes[index] >= 0 && comparer(this.values[index], value))
        return true;
      }

      return false;
    }

    public void CopyTo(System.Collections.Generic.KeyValuePair<TKey, TValue>[] array, int offset) {
      if (null == array)                       throw new System.ArgumentNullException      ($"Array is null");
      if (offset < 0 || offset > array.Length) throw new System.ArgumentOutOfRangeException($"SerializedDictionary.CopyTo(…) {{index: {offset}, array: {{Length: {array.Length}}}}}");
      if (Count > array.Length - offset)       throw new System.ArgumentException          ($"The number of elements in the dictionary ({Count}) is greater than the available space from offset to the end of the destination array ({array.Length})");

      for (int index = 0; index != this.count; ++index) {
        if (this.hashes[index] >= 0)
        array[offset++] = new(this.keys[index], this.values[index]);
      }
    }

    private int FindIndex(TKey key) {
      if (null == key)
      throw new System.ArgumentNullException($"Dictionary key is null");

      if (null != this.buckets) {
        int hash = this.comparer.GetHashCode(key) & 0x7FFFFFFF;

        for (int index = this.buckets[hash % this.buckets.Length]; index >= 0; index = this.next[index]) {
          if (hash == this.hashes[index] && this.comparer.Equals(this.keys[index], key))
          return index;
        }
      }

      return -1;
    }

    public Enumerator GetEnumerator() {
      return new(this);
    }

    public static int GetPrime(int minimum) {
      if (minimum < 0)
      throw new System.ArgumentException($"Dictionary prime minimum is less than 0");

      foreach (int prime in new[] {3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919, 1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437, 187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263, 1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369}) {
        if (minimum <= prime)
        return prime;
      }

      for (int index = minimum | 1; index != 0x7FFFFFFF; index += 2) {
        if (index == 2)
        return index;

        if (0 != (index & 1)) {
          int  limit  = (int) System.Math.Sqrt((double) index);
          bool primed = true;

          // …
          for (int subindex = 3; limit >= subindex; subindex += 2)
          if (0 == index % subindex) {
            primed = false;
            break;
          }

          if (primed && 0 != (index - 1) % 101)
          return index;
        }
      }

      return minimum;
    }

    private void Initialize(int capacity) {
      int prime = SerializedDictionary<TKey, TValue>.GetPrime(capacity);

      // …
      this.buckets  = new int[prime];
      this.freeList = -1;
      this.hashes   = new int   [prime];
      this.keys     = new TKey  [prime];
      this.next     = new int   [prime];
      this.values   = new TValue[prime];

      System.Array.Fill(this.buckets, -1);
    }

    private void Insert(TKey key, TValue value, bool insert) {
      int hash      = 0;
      int hashIndex = 0;
      int number    = 0;

      // …
      if (null == key)
        throw new System.ArgumentNullException($"Dictionary key is `null`");

      if (null == this.buckets)
        this.Initialize(0);

      hash      = this.comparer.GetHashCode(key) & 0x7FFFFFFF;
      hashIndex = hash % this.buckets.Length;

      for (int index = this.buckets[hashIndex]; index >= 0; index = next[index]) {
        if (hash == this.hashes[index] && this.comparer.Equals(this.keys[index], key)) {
          if (insert) throw new System.ArgumentException($"Dictionary key already exists: `{key}`");

          this.values[index] = value;
          this.version      += 1;
          return;
        }

        ++number;
      }

      if (this.freeCount > 0) {
        number          = this.freeList;
        this.freeCount -= 1;
        this.freeList   = this.next[number];
      }

      else {
        if (this.count == this.keys.Length) {
          this.Resize();
          hashIndex = hash % this.buckets.Length;
        }

        number      = this.count;
        this.count += 1;
      }

      this.next   [number]    = this.buckets[hashIndex];
      this.buckets[hashIndex] = number;
      this.hashes [number]    = hash;
      this.keys   [number]    = key;
      this.values [number]    = value;
      this.version           += 1;
    }

    public bool Remove(TKey key) {
      int hash      = 0;
      int hashIndex = 0;
      int number    = -1;

      // …
      if (key == null)
        throw new System.ArgumentNullException($"Dictionary key is null");

      hash      = this.comparer.GetHashCode(key) & 0x7FFFFFFF;
      hashIndex = hash % this.buckets.Length;

      for (int index = this.buckets[hashIndex]; index >= 0; index = this.next[index], number = index)
      if (hash == this.hashes[index] && this.comparer.Equals(this.keys[index], key)) {
        if (number < 0) this.buckets[hashIndex] = this.next[index];
        else            this.next   [number]    = this.next[index];

        this.version      += 1;
        this.values[index] = default;
        this.next  [index] = this.freeList;
        this.keys  [index] = default;
        this.hashes[index] = -1;
        this.freeList      = index;
        this.freeCount    += 1;

        return true;
      }

      return false;
    }

    public bool Remove(System.Collections.Generic.KeyValuePair<TKey, TValue> item) {
      return this.Remove(item.Key);
    }

    private void Resize() {
      this.Resize(this.count < 0x7FEFFFFD && this.count * 2 > 0x7FEFFFFD ? 0x7FEFFFFD : SerializedDictionary<TKey, TValue>.GetPrime(this.count * 2), false);
    }

    private void Resize(int capacity, bool regenerateHashes) {
      int[]    buckets = new int   [capacity];
      int[]    hashes  = new int   [capacity];
      TKey[]   keys    = new TKey  [capacity];
      int[]    next    = new int   [capacity];
      TValue[] values  = new TValue[capacity];

      // …
      for (int index = 0; buckets.Length != index; ++index)
        buckets[index] = -1;

      System.Array.Copy(this.hashes, 0, hashes, 0, this.count);
      System.Array.Copy(this.keys,   0, keys,   0, this.count);
      System.Array.Copy(this.next,   0, next,   0, this.count);
      System.Array.Copy(this.values, 0, values, 0, this.count);

      if (regenerateHashes)
      for (int index = 0; index != this.count; index++) {
        if (hashes[index] != -1)
        hashes[index] = this.comparer.GetHashCode(keys[index]) & 0x7FFFFFFF;
      }

      for (int index = 0; index != this.count; index++) {
        int hashIndex = hashes[index] % capacity;

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

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey,TValue>> System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey,TValue>>.GetEnumerator() {
      return this.GetEnumerator();
    }

    public bool TryGetValue(TKey key, out TValue value) {
      int index = this.FindIndex(key);

      // …
      if (index >= 0) {
        value = this.values[index];
        return true;
      }

      value = default;
      return false;
    }

    public TValue this[TKey key] {
      get {
        int index = this.FindIndex(key);

        if (index >= 0) return this.values[index];
        throw new System.Collections.Generic.KeyNotFoundException(key.ToString());
      }

      set {
        this.Insert(key, value, false);
      }
    }

    public TValue this[TKey key, TValue _] {
      get {
        int index = this.FindIndex(key);
        return index >= 0 ? this.values[index] : _;
      }
    }
  }
    [System.Serializable] public class AnimationCurveDictionary : PatchOdyssey.SerializedDictionary<string, UnityEngine.AnimationCurve> {}
    [System.Serializable] public class BooleanDictionary        : PatchOdyssey.SerializedDictionary<string, System.Boolean>             {}
    [System.Serializable] public class BoundsDictionary         : PatchOdyssey.SerializedDictionary<string, UnityEngine.Bounds>         {}
    [System.Serializable] public class BoundsIntDictionary      : PatchOdyssey.SerializedDictionary<string, UnityEngine.BoundsInt>      {}
    [System.Serializable] public class ColorDictionary          : PatchOdyssey.SerializedDictionary<string, UnityEngine.Color>          {}
    [System.Serializable] public class DoubleDictionary         : PatchOdyssey.SerializedDictionary<string, System.Double>              {}
    [System.Serializable] public class FloatDictionary          : PatchOdyssey.SerializedDictionary<string, System.Single>              {}
    [System.Serializable] public class GameObjectDictionary     : PatchOdyssey.SerializedDictionary<string, UnityEngine.GameObject>     {}
    [System.Serializable] public class GradientDictionary       : PatchOdyssey.SerializedDictionary<string, UnityEngine.Gradient>       {}
    [System.Serializable] public class IntDictionary            : PatchOdyssey.SerializedDictionary<string, System.Int32>               {}
    [System.Serializable] public class LongDictionary           : PatchOdyssey.SerializedDictionary<string, System.Int64>               {}
    [System.Serializable] public class RectDictionary           : PatchOdyssey.SerializedDictionary<string, UnityEngine.Rect>           {}
    [System.Serializable] public class RectIntDictionary        : PatchOdyssey.SerializedDictionary<string, UnityEngine.RectInt>        {}
    [System.Serializable] public class StringDictionary         : PatchOdyssey.SerializedDictionary<string, System.String>              {}
    [System.Serializable] public class UIntDictionary           : PatchOdyssey.SerializedDictionary<string, System.UInt32>              {}
    [System.Serializable] public class ULongDictionary          : PatchOdyssey.SerializedDictionary<string, System.UInt64>              {}
    [System.Serializable] public class Vector2Dictionary        : PatchOdyssey.SerializedDictionary<string, UnityEngine.Vector2>        {}
    [System.Serializable] public class Vector2IntDictionary     : PatchOdyssey.SerializedDictionary<string, UnityEngine.Vector2Int>     {}
    [System.Serializable] public class Vector3Dictionary        : PatchOdyssey.SerializedDictionary<string, UnityEngine.Vector3>        {}
    [System.Serializable] public class Vector3IntDictionary     : PatchOdyssey.SerializedDictionary<string, UnityEngine.Vector3Int>     {}
    [System.Serializable] public class Vector4Dictionary        : PatchOdyssey.SerializedDictionary<string, UnityEngine.Vector4>        {}

  public class SerializedDictionaryDrawer<TDictionary> : UnityEditor.PropertyDrawer where TDictionary : class?, new() {
    private const  float                                  BUTTON_HEIGHT       =  17.0f;
    private const  float                                  BUTTON_WIDTH        =  18.0f;
    private static (System.Type? key, System.Type? value) DICTIONARY_GENERICS => GetDictionaryGenerics();
    private static System.Type                            DICTIONARY_TYPE     => GetDictionaryType    ();
    private        TDictionary?                           dictionary          =  null;
    private        bool                                   foldout             =  false;

    /* … */
    private void EnsureDictionary(UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) {
      if (null == this.dictionary) {
        this.dictionary = this.fieldInfo.GetValue(property.serializedObject.targetObject) as TDictionary;
        this.foldout    = UnityEditor.EditorPrefs.GetBool(label.text);

        if (null == this.dictionary)
        this.fieldInfo.SetValue(property.serializedObject.targetObject, this.dictionary = new TDictionary());
      }
    }

    private static (System.Type?, System.Type?) GetDictionaryGenerics() {
      System.Type[] generics = GetDictionaryType()?.GetGenericArguments();
      return (generics?.Length ?? 0) < 2 ? (null, null) : (generics[0], generics[1]);
    }

    private static System.Type GetDictionaryType() {
      for (System.Type type = typeof(TDictionary); null != type; type = type.BaseType) {
        System.Type[] generics   = type.GetGenericArguments();
        System.Type[] interfaces = type.GetInterfaces      ();

        // …
        if (generics.Length == 2)
        if (
          type                            ==                                  typeof(System.Collections.Generic. Dictionary<,>).MakeGenericType(generics) ||
          interfaces.GetLowerBound(0) - 1 != System.Array.IndexOf(interfaces, typeof(System.Collections.Generic.IDictionary<,>).MakeGenericType(generics))
        ) return type;
      }

      return typeof(TDictionary);
    }

    public override float GetPropertyHeight(UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) {
      this.EnsureDictionary(property, label);
      return BUTTON_HEIGHT * (this.foldout ? (GetDictionaryType().GetProperty("Count")?.GetValue(this.dictionary) as int? ?? 0) + 1 : 1);
    }

    private bool IsReadOnlyDictionary() {
      if ((null, null) != DICTIONARY_GENERICS) {
        System.Type type = typeof(PatchOdyssey.SerializedReadOnlyDictionary<,>).MakeGenericType(new[] {DICTIONARY_GENERICS.key, DICTIONARY_GENERICS.value});

        // …
        if (type == typeof(TDictionary) || typeof(TDictionary).IsSubclassOf(type))
        return true;
      }

      return null != this.dictionary && (DICTIONARY_TYPE.GetProperty("IsReadOnly")?.GetValue(this.dictionary) as bool? ?? false);
    }

    public override void OnGUI(UnityEngine.Rect position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) {
      bool                                                                     isReadOnly = this.IsReadOnlyDictionary();
      (UnityEngine.Rect add, UnityEngine.Rect clear, UnityEngine.Rect foldout) positions;

      // …
      this.EnsureDictionary(property, label);

      position .height  = BUTTON_HEIGHT;
      positions.foldout = new(position.x,                                            position.y, position.width - BUTTON_WIDTH, position.height);
      positions.clear   = new(position.x + (position.width - (BUTTON_WIDTH * 1.0f)), position.y, 0.0f           + BUTTON_WIDTH, position.height);
      positions.add     = new(position.x + (position.width - (BUTTON_WIDTH * 2.0f)), position.y, 0.0f           + BUTTON_WIDTH, position.height);

      // …
      if ((null, null) == DICTIONARY_GENERICS)
      return;

      if (!isReadOnly) {
        if (UnityEngine.GUI.Button(positions.add, new UnityEngine.GUIContent("+", "Add item"), UnityEditor.EditorStyles.miniButton))
        try {
          object key   =  DICTIONARY_GENERICS.key == typeof(string) ? ""   : System.Activator.CreateInstance(DICTIONARY_GENERICS.key);
          object value = !DICTIONARY_GENERICS.value.IsValueType     ? null : System.Activator.CreateInstance(DICTIONARY_GENERICS.value);

          // …
          if (!(DICTIONARY_TYPE.GetMethod("ContainsKey", new[] {DICTIONARY_GENERICS.key})?.Invoke(this.dictionary, new object[] {key}) as bool? ?? true))
          DICTIONARY_TYPE.GetMethod("Add", new[] {DICTIONARY_GENERICS.key, DICTIONARY_GENERICS.value})?.Invoke(this.dictionary, new object[] {key, value});
        } catch (System.Exception exception) { UnityEngine.Debug.LogError($"{exception.Message}"); }

        if (UnityEngine.GUI.Button(positions.clear, new UnityEngine.GUIContent("×", "Clear dictionary"), UnityEditor.EditorStyles.miniButtonRight))
        DICTIONARY_TYPE.GetMethod("Clear", new System.Type[] {})?.Invoke(this.dictionary, null);
      }

      UnityEditor.EditorGUI.BeginChangeCheck();
      this.foldout = UnityEditor.EditorGUI.Foldout(positions.foldout, this.foldout, label, true);
      if (UnityEditor.EditorGUI.EndChangeCheck()) UnityEditor.EditorPrefs.SetBool(label.text, this.foldout);

      if (!this.foldout)
      return;

      for (
        object enumerator = DICTIONARY_TYPE.GetMethod("GetEnumerator")?.Invoke(this.dictionary, null);
        enumerator?.GetType().GetMethod("MoveNext", new System.Type[] {})?.Invoke(enumerator, null) as bool? ?? false;
      ) {
        (UnityEngine.Rect key, UnityEngine.Rect remove, UnityEngine.Rect value) subpositions;
        object                                                                  item = enumerator.GetType() .GetProperty("Current")?.GetValue(enumerator);
        (object key, object value)                                                   = (item    ?.GetType()?.GetProperty("Key")    ?.GetValue(item), item?.GetType()?.GetProperty("Value")?.GetValue(item));

        /* … */
        static object Field(UnityEngine.Rect position, object value, System.Type type) {
          if (null == type) {
            UnityEditor.EditorGUI.LabelField(position, $"{value}");
            return value;
          }

          if (type == typeof(bool))                              return (UnityEditor.EditorGUI.Toggle         (position,                              (System.Boolean)             (value as object))             as object);
          if (type == typeof(double))                            return (UnityEditor.EditorGUI.DoubleField    (position,                              (System.Double)              (value as object))             as object);
          if (type == typeof(float))                             return (UnityEditor.EditorGUI.FloatField     (position,                              (System.Single)              (value as object))             as object);
          if (type == typeof(int))                               return (UnityEditor.EditorGUI.IntField       (position,                              (System.Int32)               (value as object))             as object);
          if (type == typeof(long))                              return (UnityEditor.EditorGUI.LongField      (position,                              (System.Int64)               (value as object))             as object);
          if (type == typeof(string))                            return (UnityEditor.EditorGUI.TextField      (position,                              (System.String)              (value as object))             as object);
          if (type == typeof(UnityEngine.AnimationCurve))        return (UnityEditor.EditorGUI.CurveField     (position,                              (UnityEngine.AnimationCurve) (value as object))             as object);
          if (type == typeof(UnityEngine.Bounds))                return (UnityEditor.EditorGUI.BoundsField    (position,                              (UnityEngine.Bounds)         (value as object))             as object);
          if (type == typeof(UnityEngine.BoundsInt))             return (UnityEditor.EditorGUI.BoundsIntField (position,                              (UnityEngine.BoundsInt)      (value as object))             as object);
          if (type == typeof(UnityEngine.Color))                 return (UnityEditor.EditorGUI.ColorField     (position,                              (UnityEngine.Color)          (value as object))             as object);
          if (type == typeof(UnityEngine.Gradient))              return (UnityEditor.EditorGUI.GradientField  (position,                              (UnityEngine.Gradient)       (value as object))             as object);
          if (type == typeof(UnityEngine.Rect))                  return (UnityEditor.EditorGUI.RectField      (position,                              (UnityEngine.Rect)           (value as object))             as object);
          if (type == typeof(UnityEngine.RectInt))               return (UnityEditor.EditorGUI.RectIntField   (position,                              (UnityEngine.RectInt)        (value as object))             as object);
          if (type == typeof(UnityEngine.Vector2))               return (UnityEditor.EditorGUI.Vector2Field   (position, UnityEngine.GUIContent.none, (UnityEngine.Vector2)        (value as object))             as object);
          if (type == typeof(UnityEngine.Vector2Int))            return (UnityEditor.EditorGUI.Vector2IntField(position, UnityEngine.GUIContent.none, (UnityEngine.Vector2Int)     (value as object))             as object);
          if (type == typeof(UnityEngine.Vector3))               return (UnityEditor.EditorGUI.Vector3Field   (position, UnityEngine.GUIContent.none, (UnityEngine.Vector3)        (value as object))             as object);
          if (type == typeof(UnityEngine.Vector3Int))            return (UnityEditor.EditorGUI.Vector3IntField(position, UnityEngine.GUIContent.none, (UnityEngine.Vector3Int)     (value as object))             as object);
          if (type == typeof(UnityEngine.Vector4))               return (UnityEditor.EditorGUI.Vector4Field   (position, UnityEngine.GUIContent.none, (UnityEngine.Vector4)        (value as object))             as object);
          if (type.IsEnum)                                       return (UnityEditor.EditorGUI.EnumPopup      (position,                              (System.Enum)                (value as object))             as object);
          if (typeof(UnityEngine.Object).IsAssignableFrom(type)) return (UnityEditor.EditorGUI.ObjectField    (position,                              (UnityEngine.Object)         (value as object), type, true) as object);

          UnityEngine.Debug.LogError($"Type `{type}` is not supported");
          return value;
        }

        // …
        position    .y     += BUTTON_HEIGHT;
        subpositions.key    = new(position.x                                                     + (isReadOnly ? BUTTON_WIDTH : 0.0f), position.y, (position.width - BUTTON_WIDTH) * (2.0f / 5.0f), position.height);
        subpositions.value  = new(position.x + ((position.width - BUTTON_WIDTH) * (2.0f / 5.0f)) + (isReadOnly ? BUTTON_WIDTH : 0.0f), position.y, (position.width - BUTTON_WIDTH) * (3.0f / 5.0f), position.height);
        subpositions.remove = new(position.x + ((position.width - BUTTON_WIDTH) * (5.0f / 5.0f)),                                      position.y, BUTTON_WIDTH,                                    position.height);

        UnityEditor.EditorGUI.BeginChangeCheck();
        key = Field(subpositions.key, key, isReadOnly ? null : DICTIONARY_GENERICS.key);
        if (UnityEditor.EditorGUI.EndChangeCheck()) {
          try {
            DICTIONARY_TYPE.GetMethod("Remove", new[] {DICTIONARY_GENERICS.key})                           .Invoke(this.dictionary, new object[] {item?.GetType()?.GetProperty("Key")?.GetValue(item)});
            DICTIONARY_TYPE.GetMethod("Add",    new[] {DICTIONARY_GENERICS.key, DICTIONARY_GENERICS.value}).Invoke(this.dictionary, new object[] {key, value});
          } catch (System.Exception exception) when (exception is not System.NullReferenceException) { UnityEngine.Debug.LogError($"{exception.Message}"); }

          break;
        }

        UnityEditor.EditorGUI.BeginChangeCheck();
        value = Field(subpositions.value, value, DICTIONARY_GENERICS.value);
        if (UnityEditor.EditorGUI.EndChangeCheck()) {
          DICTIONARY_TYPE.GetProperty((System.Attribute.GetCustomAttribute(typeof(TDictionary), typeof(System.Reflection.DefaultMemberAttribute)) as System.Reflection.DefaultMemberAttribute)?.MemberName ?? "", new[] {DICTIONARY_GENERICS.key})?.GetSetMethod()?.Invoke(this.dictionary, new object[] {key, value});
          break;
        }

        if (!isReadOnly)
        if (UnityEngine.GUI.Button(subpositions.remove, new UnityEngine.GUIContent("×", "Remove item"), UnityEditor.EditorStyles.miniButtonRight)) {
          DICTIONARY_TYPE.GetMethod("Remove", new[] {DICTIONARY_GENERICS.key})?.Invoke(this.dictionary, new object[] {key});
          break;
        }
      }
    }
  }
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.AnimationCurveDictionary))]         public class AnimationCurveDictionaryDrawer         : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.AnimationCurveDictionary>         {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.AnimationCurveReadOnlyDictionary))] public class AnimationCurveReadOnlyDictionaryDrawer : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.AnimationCurveReadOnlyDictionary> {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.BooleanDictionary))]                public class BooleanDictionaryDrawer                : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.BooleanDictionary>                {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.BooleanReadOnlyDictionary))]        public class BooleanReadOnlyDictionaryDrawer        : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.BooleanReadOnlyDictionary>        {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.BoundsDictionary))]                 public class BoundsDictionaryDrawer                 : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.BoundsDictionary>                 {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.BoundsIntDictionary))]              public class BoundsIntDictionaryDrawer              : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.BoundsIntDictionary>              {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.BoundsIntReadOnlyDictionary))]      public class BoundsIntReadOnlyDictionaryDrawer      : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.BoundsIntReadOnlyDictionary>      {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.BoundsReadOnlyDictionary))]         public class BoundsReadOnlyDictionaryDrawer         : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.BoundsReadOnlyDictionary>         {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.ColorDictionary))]                  public class ColorDictionaryDrawer                  : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.ColorDictionary>                  {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.ColorReadOnlyDictionary))]          public class ColorReadOnlyDictionaryDrawer          : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.ColorReadOnlyDictionary>          {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.DoubleDictionary))]                 public class DoubleDictionaryDrawer                 : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.DoubleDictionary>                 {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.DoubleReadOnlyDictionary))]         public class DoubleReadOnlyDictionaryDrawer         : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.DoubleReadOnlyDictionary>         {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.FloatDictionary))]                  public class FloatDictionaryDrawer                  : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.FloatDictionary>                  {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.FloatReadOnlyDictionary))]          public class FloatReadOnlyDictionaryDrawer          : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.FloatReadOnlyDictionary>          {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.GameObjectDictionary))]             public class GameObjectDictionaryDrawer             : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.GameObjectDictionary>             {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.GameObjectReadOnlyDictionary))]     public class GameObjectReadOnlyDictionaryDrawer     : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.GameObjectReadOnlyDictionary>     {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.GradientDictionary))]               public class GradientDictionaryDrawer               : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.GradientDictionary>               {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.GradientReadOnlyDictionary))]       public class GradientReadOnlyDictionaryDrawer       : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.GradientReadOnlyDictionary>       {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.IntDictionary))]                    public class IntDictionaryDrawer                    : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.IntDictionary>                    {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.IntReadOnlyDictionary))]            public class IntReadOnlyDictionaryDrawer            : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.IntReadOnlyDictionary>            {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.LongDictionary))]                   public class LongDictionaryDrawer                   : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.LongDictionary>                   {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.LongReadOnlyDictionary))]           public class LongReadOnlyDictionaryDrawer           : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.LongReadOnlyDictionary>           {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.RectDictionary))]                   public class RectDictionaryDrawer                   : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.RectDictionary>                   {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.RectIntDictionary))]                public class RectIntDictionaryDrawer                : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.RectIntDictionary>                {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.RectIntReadOnlyDictionary))]        public class RectIntReadOnlyDictionaryDrawer        : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.RectIntReadOnlyDictionary>        {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.RectReadOnlyDictionary))]           public class RectReadOnlyDictionaryDrawer           : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.RectReadOnlyDictionary>           {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.StringDictionary))]                 public class StringDictionaryDrawer                 : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.StringDictionary>                 {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.StringReadOnlyDictionary))]         public class StringReadOnlyDictionaryDrawer         : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.StringReadOnlyDictionary>         {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.UIntDictionary))]                   public class UIntDictionaryDrawer                   : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.UIntDictionary>                   {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.UIntReadOnlyDictionary))]           public class UIntReadOnlyDictionaryDrawer           : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.UIntReadOnlyDictionary>           {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.ULongDictionary))]                  public class ULongDictionaryDrawer                  : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.ULongDictionary>                  {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.ULongReadOnlyDictionary))]          public class ULongReadOnlyDictionaryDrawer          : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.ULongReadOnlyDictionary>          {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Vector2Dictionary))]                public class Vector2DictionaryDrawer                : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.Vector2Dictionary>                {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Vector2IntDictionary))]             public class Vector2IntDictionaryDrawer             : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.Vector2IntDictionary>             {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Vector2IntReadOnlyDictionary))]     public class Vector2IntReadOnlyDictionaryDrawer     : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.Vector2IntReadOnlyDictionary>     {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Vector2ReadOnlyDictionary))]        public class Vector2ReadOnlyDictionaryDrawer        : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.Vector2ReadOnlyDictionary>        {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Vector3Dictionary))]                public class Vector3DictionaryDrawer                : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.Vector3Dictionary>                {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Vector3IntDictionary))]             public class Vector3IntDictionaryDrawer             : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.Vector3IntDictionary>             {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Vector3IntReadOnlyDictionary))]     public class Vector3IntReadOnlyDictionaryDrawer     : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.Vector3IntReadOnlyDictionary>     {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Vector3ReadOnlyDictionary))]        public class Vector3ReadOnlyDictionaryDrawer        : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.Vector3ReadOnlyDictionary>        {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Vector4Dictionary))]                public class Vector4DictionaryDrawer                : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.Vector4Dictionary>                {}
    [UnityEditor.CustomPropertyDrawer(typeof(PatchOdyssey.Vector4ReadOnlyDictionary))]        public class Vector4ReadOnlyDictionaryDrawer        : PatchOdyssey.SerializedDictionaryDrawer<PatchOdyssey.Vector4ReadOnlyDictionary>        {}

  [System.Serializable]
  public class SerializedReadOnlyDictionary<TKey, TValue> : PatchOdyssey.SerializedDictionary<TKey, TValue> /* → `System.Collections.Generic.IReadOnlyDictionary<…>` */ {
    public new bool IsReadOnly => true;

    [System.Obsolete("", true)] public  new void Clear ()                                    {}
    [System.Obsolete("", true)] private     void Insert(TKey key, TValue value, bool insert) {}
    [System.Obsolete("", true)] public  new bool Remove(TKey key)                            => false;
  }
    [System.Serializable] public class AnimationCurveReadOnlyDictionary : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.AnimationCurve> {}
    [System.Serializable] public class BooleanReadOnlyDictionary        : PatchOdyssey.SerializedReadOnlyDictionary<string, System.Boolean>             {}
    [System.Serializable] public class BoundsReadOnlyDictionary         : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.Bounds>         {}
    [System.Serializable] public class BoundsIntReadOnlyDictionary      : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.BoundsInt>      {}
    [System.Serializable] public class ColorReadOnlyDictionary          : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.Color>          {}
    [System.Serializable] public class DoubleReadOnlyDictionary         : PatchOdyssey.SerializedReadOnlyDictionary<string, System.Double>              {}
    [System.Serializable] public class FloatReadOnlyDictionary          : PatchOdyssey.SerializedReadOnlyDictionary<string, System.Single>              {}
    [System.Serializable] public class GameObjectReadOnlyDictionary     : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.GameObject>     {}
    [System.Serializable] public class GradientReadOnlyDictionary       : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.Gradient>       {}
    [System.Serializable] public class IntReadOnlyDictionary            : PatchOdyssey.SerializedReadOnlyDictionary<string, System.Int32>               {}
    [System.Serializable] public class LongReadOnlyDictionary           : PatchOdyssey.SerializedReadOnlyDictionary<string, System.Int64>               {}
    [System.Serializable] public class RectReadOnlyDictionary           : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.Rect>           {}
    [System.Serializable] public class RectIntReadOnlyDictionary        : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.RectInt>        {}
    [System.Serializable] public class StringReadOnlyDictionary         : PatchOdyssey.SerializedReadOnlyDictionary<string, System.String>              {}
    [System.Serializable] public class UIntReadOnlyDictionary           : PatchOdyssey.SerializedReadOnlyDictionary<string, System.UInt32>              {}
    [System.Serializable] public class ULongReadOnlyDictionary          : PatchOdyssey.SerializedReadOnlyDictionary<string, System.UInt64>              {}
    [System.Serializable] public class Vector2ReadOnlyDictionary        : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.Vector2>        {}
    [System.Serializable] public class Vector2IntReadOnlyDictionary     : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.Vector2Int>     {}
    [System.Serializable] public class Vector3ReadOnlyDictionary        : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.Vector3>        {}
    [System.Serializable] public class Vector3IntReadOnlyDictionary     : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.Vector3Int>     {}
    [System.Serializable] public class Vector4ReadOnlyDictionary        : PatchOdyssey.SerializedReadOnlyDictionary<string, UnityEngine.Vector4>        {}
}

namespace PatchOdyssey {
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
      public static T[]                      FindChildrenByComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => System.Array.ConvertAll(gameObject.FindChildren(child => null != child.GetComponent<T>()),  child => child.GetComponent<T>());
      public static UnityEngine.Component[]  FindChildrenByComponent   (this UnityEngine.GameObject gameObject, System.Type type)               => System.Array.ConvertAll(gameObject.FindChildren(child => null != child.GetComponent(type)), child => child.GetComponent(type));
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
      descendants.AddRange(pending[0].FindChildren(child => { pending.Add(child); return predicate(child); }));

      return descendants.ToArray();
    }
      public static T[]                      FindDescendantsByComponent<T>(this UnityEngine.GameObject gameObject) where T : UnityEngine.Component => System.Array.ConvertAll(gameObject.FindDescendants(child => null != child.GetComponent<T>()),  child => child.GetComponent<T>());
      public static UnityEngine.Component[]  FindDescendantsByComponent   (this UnityEngine.GameObject gameObject, System.Type type)               => System.Array.ConvertAll(gameObject.FindDescendants(child => null != child.GetComponent(type)), child => child.GetComponent(type));
      public static UnityEngine.GameObject[] FindDescendantsByTag         (this UnityEngine.GameObject gameObject, string      tag)                => gameObject.FindDescendants(child => child.tag == tag);

    public static UnityEngine.GameObject? GetChild(this UnityEngine.GameObject gameObject, uint index) {
      foreach (UnityEngine.Transform transform in gameObject.transform) {
        if (0u == index--)
        return transform.gameObject;
      }

      return null;
    }

    public static UnityEngine.GameObject[] GetChildren   (this UnityEngine.GameObject gameObject) => gameObject.FindChildren   (_ => true);
    public static UnityEngine.GameObject[] GetDescendants(this UnityEngine.GameObject gameObject) => gameObject.FindDescendants(_ => true);
    public static UnityEngine.GameObject   GetParent     (this UnityEngine.GameObject gameObject) => gameObject.transform.parent.gameObject;

    public static bool HasChild     (this UnityEngine.GameObject gameObject, UnityEngine.GameObject child) => null != gameObject.FindChild     (_ => _ == child);
    public static bool HasDescendant(this UnityEngine.GameObject gameObject, UnityEngine.GameObject child) => null != gameObject.FindDescendant(_ => _ == child);

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

    public static string GetAssetPath() {
      return Util.NormalizeURI(UnityEngine.Application.streamingAssetsPath);
    }

    public static string GetDataPath() {
      return Util.NormalizeURI(UnityEngine.Application.persistentDataPath);
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

    public static UnityEngine.KeyCode[] GetKeyCodes() {
      return new[] {UnityEngine.KeyCode.A, UnityEngine.KeyCode.Alpha0, UnityEngine.KeyCode.Alpha1, UnityEngine.KeyCode.Alpha2, UnityEngine.KeyCode.Alpha3, UnityEngine.KeyCode.Alpha4, UnityEngine.KeyCode.Alpha5, UnityEngine.KeyCode.Alpha6, UnityEngine.KeyCode.Alpha7, UnityEngine.KeyCode.Alpha8, UnityEngine.KeyCode.Alpha9, UnityEngine.KeyCode.AltGr, UnityEngine.KeyCode.Ampersand, UnityEngine.KeyCode.Asterisk, UnityEngine.KeyCode.At, UnityEngine.KeyCode.B, UnityEngine.KeyCode.BackQuote, UnityEngine.KeyCode.Backslash, UnityEngine.KeyCode.Backspace, UnityEngine.KeyCode.Break, UnityEngine.KeyCode.C, UnityEngine.KeyCode.CapsLock, UnityEngine.KeyCode.Caret, UnityEngine.KeyCode.Clear, UnityEngine.KeyCode.Colon, UnityEngine.KeyCode.Comma, UnityEngine.KeyCode.D, UnityEngine.KeyCode.Delete, UnityEngine.KeyCode.Dollar, UnityEngine.KeyCode.DoubleQuote, UnityEngine.KeyCode.DownArrow, UnityEngine.KeyCode.E, UnityEngine.KeyCode.End, UnityEngine.KeyCode.Equals, UnityEngine.KeyCode.Escape, UnityEngine.KeyCode.Exclaim, UnityEngine.KeyCode.F, UnityEngine.KeyCode.F1, UnityEngine.KeyCode.F10, UnityEngine.KeyCode.F11, UnityEngine.KeyCode.F12, UnityEngine.KeyCode.F13, UnityEngine.KeyCode.F14, UnityEngine.KeyCode.F15, UnityEngine.KeyCode.F2, UnityEngine.KeyCode.F3, UnityEngine.KeyCode.F4, UnityEngine.KeyCode.F5, UnityEngine.KeyCode.F6, UnityEngine.KeyCode.F7, UnityEngine.KeyCode.F8, UnityEngine.KeyCode.F9, UnityEngine.KeyCode.G, UnityEngine.KeyCode.Greater, UnityEngine.KeyCode.H, UnityEngine.KeyCode.Hash, UnityEngine.KeyCode.Help, UnityEngine.KeyCode.Home, UnityEngine.KeyCode.I, UnityEngine.KeyCode.Insert, UnityEngine.KeyCode.J, UnityEngine.KeyCode.K, UnityEngine.KeyCode.Keypad0, UnityEngine.KeyCode.Keypad1, UnityEngine.KeyCode.Keypad2, UnityEngine.KeyCode.Keypad3, UnityEngine.KeyCode.Keypad4, UnityEngine.KeyCode.Keypad5, UnityEngine.KeyCode.Keypad6, UnityEngine.KeyCode.Keypad7, UnityEngine.KeyCode.Keypad8, UnityEngine.KeyCode.Keypad9, UnityEngine.KeyCode.KeypadDivide, UnityEngine.KeyCode.KeypadEnter, UnityEngine.KeyCode.KeypadEquals, UnityEngine.KeyCode.KeypadMinus, UnityEngine.KeyCode.KeypadMultiply, UnityEngine.KeyCode.KeypadPeriod, UnityEngine.KeyCode.KeypadPlus, UnityEngine.KeyCode.L, UnityEngine.KeyCode.LeftAlt, UnityEngine.KeyCode.LeftApple, UnityEngine.KeyCode.LeftArrow, UnityEngine.KeyCode.LeftBracket, UnityEngine.KeyCode.LeftCommand, UnityEngine.KeyCode.LeftControl, UnityEngine.KeyCode.LeftCurlyBracket, UnityEngine.KeyCode.LeftMeta, UnityEngine.KeyCode.LeftParen, UnityEngine.KeyCode.LeftShift, UnityEngine.KeyCode.LeftWindows, UnityEngine.KeyCode.Less, UnityEngine.KeyCode.M, UnityEngine.KeyCode.Menu, UnityEngine.KeyCode.Minus, UnityEngine.KeyCode.N, UnityEngine.KeyCode.Numlock, UnityEngine.KeyCode.O, UnityEngine.KeyCode.P, UnityEngine.KeyCode.PageDown, UnityEngine.KeyCode.PageUp, UnityEngine.KeyCode.Pause, UnityEngine.KeyCode.Percent, UnityEngine.KeyCode.Period, UnityEngine.KeyCode.Pipe, UnityEngine.KeyCode.Plus, UnityEngine.KeyCode.Print, UnityEngine.KeyCode.Q, UnityEngine.KeyCode.Question, UnityEngine.KeyCode.Quote, UnityEngine.KeyCode.R, UnityEngine.KeyCode.Return, UnityEngine.KeyCode.RightAlt, UnityEngine.KeyCode.RightApple, UnityEngine.KeyCode.RightArrow, UnityEngine.KeyCode.RightBracket, UnityEngine.KeyCode.RightCommand, UnityEngine.KeyCode.RightControl, UnityEngine.KeyCode.RightCurlyBracket, UnityEngine.KeyCode.RightMeta, UnityEngine.KeyCode.RightParen, UnityEngine.KeyCode.RightShift, UnityEngine.KeyCode.RightWindows, UnityEngine.KeyCode.S, UnityEngine.KeyCode.ScrollLock, UnityEngine.KeyCode.Semicolon, UnityEngine.KeyCode.Slash, UnityEngine.KeyCode.Space, UnityEngine.KeyCode.SysReq, UnityEngine.KeyCode.T, UnityEngine.KeyCode.Tab, UnityEngine.KeyCode.Tilde, UnityEngine.KeyCode.U, UnityEngine.KeyCode.Underscore, UnityEngine.KeyCode.UpArrow, UnityEngine.KeyCode.V, UnityEngine.KeyCode.W, UnityEngine.KeyCode.X, UnityEngine.KeyCode.Y, UnityEngine.KeyCode.Z};
    }

    public static int[] GetMouseButtons() {
      return new int[] {0x0, 0x1, 0x2};
    }

    public static System.Func<T, T> IIFE<T>(System.Action<T> function) {
      return _ => { function(_); return _; };
    }

    public static System.Func<T, T> IIFE<T>(System.Func<T, T> function) /* → Immediately-Invoked Function Expression */ {
      return function;
    }

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

    private static string NormalizeURI(string path) {
      path = path.TrimEnd().Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);

      // …
      while (0 != path.Length && (path.EndsWith(System.IO.Path.DirectorySeparatorChar) || System.String.IsNullOrWhiteSpace(path.Substring(path.Length - 1))))
        path = path.TrimEnd(System.IO.Path.DirectorySeparatorChar).TrimEnd();

      return path;
    }

    public static float Percent(float percent) {
      return percent / 100.0f;
    }

    public static void PreloadURI(string path) {
      Util.LoadURI(path, null, 0.0f, false);
    }

    public static void PreloadURIAsAudioClip(string path, UnityEngine.AudioType? encoding = null) {
      Util.LoadURIAsAudioClip(path, null, 0.0f, false, encoding);
    }

    public static void PreloadURIAsText(string path, System.Text.Encoding? encoding = null) {
      Util.LoadURIAsText(path, null, 0.0f, false, encoding);
    }

    public static UnityEngine.Rect RectFromCorners(UnityEngine.Vector3[] corners) {
      // → Origin begins from bottom-left rather than top-left
      return new(corners[0].x, corners[0].y, corners[3].x - corners[0].x, corners[1].y - corners[0].y);
    }

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
