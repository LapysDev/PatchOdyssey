using PatchOdyssey;

/* … */
public static partial class Settings {
  public  static          double     LoadTimeoutMaximum = 1.2; // ⟶ Synchronous
  private static readonly System.Uri ResetUri           = new(System.IO.Path.Combine(new[] {Util.Path.Assets, "Settings.xml"}));
  private static readonly System.Uri Uri                = new(System.IO.Path.Combine(new[] {Util.Path.Data,   "Settings.xml"}));
  #if UNITY_EDITOR
    public static bool HumanReadable = true;
  #else
    public static bool HumanReadable = false;
  #endif

  /* … */
  public static bool AddProperty<T>(string path, in T value) where T : System.IConvertible {
    if (Settings.EnsureAsXml() is System.Xml.XmlDocument data && path is not null) {
      System.Xml.XmlNode? node           = data as System.Xml.XmlNode;
      string[]            components     = path.Split('/', System.StringSplitOptions.None);
      uint                componentCount = (uint) components.Length;

      // …
      path = string.Empty;

      try { node = data.SelectSingleNode("/Settings"); }
      catch (System.Xml.XPath.XPathException) {}

      for ((uint index, System.Xml.XmlNode prenode, System.Xml.XmlNode? subnode) = (0u, node!, null); ; ++index) {
        add:
        if (index == componentCount - 1u) {
          System.Xml.XmlElement element = data.CreateElement(components[index]);

          // … ⟶ Add new property `element` to XML `data`
          element.InnerText = System.Convert.ToString(value);

          try {
            if (
              subnode is not null
              ? element == subnode.AppendChild(element) ? node == prenode.AppendChild(node!) : false
              : element == node  !.AppendChild(element)
            ) return Settings.Update(data);
          } catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.InvalidOperationException) {}

          break; // ⟶ `return false`
        }

        prenode = node!;
        path   += components[index];
        try { node = Settings.GetPropertyAsXml(path); } catch (System.Xml.XPath.XPathException) {}

        // … ⟶ `path` did not refer to any pre-existing property, so add the remaining `components` iteratively —
        if (node is null) {
          while (index != componentCount - 1u) {
            System.Xml.XmlElement element = data.CreateElement(components[index++]);

            // …
            if (subnode is null) {
              node    = null == node ? element : node;
              subnode = element;

              continue;
            }

            try { if (element != subnode.AppendChild(element)) return false; }
            catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.InvalidOperationException) {}
          } goto add; // ⟶ or stall `--index` iteration to `add`
        } else path += '/'; // ⟶ — otherwise acknowledge existing `path` as valid
      }
    }

    return false;
  }

  private static byte[]?                 Ensure      () { string path = Settings.Uri.LocalPath; if (!System.IO.File.Exists(path)) { if (!Settings.Reset()) return null; } return System.IO.File.ReadAllBytes(path); }
  private static string?                 EnsureAsText() { try { return System.Text.Encoding.Default.GetString(Settings.Ensure()); } catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.ArgumentNullException || exception is System.Text.DecoderFallbackException) {} return null; }
  private static System.Xml.XmlDocument? EnsureAsXml () { string? content = Settings.EnsureAsText(); if (content is not null) { System.Xml.XmlDocument document = new(); try { document.PreserveWhitespace = Settings.HumanReadable; document.LoadXml(content); return document; } catch (System.Xml.XmlException) {} } return null; }

  [PatchMethod(AggressiveInlining)]
  public static System.Nullable<T> GetProperty<T>(string path) where T : unmanaged /* System.IConvertible */ {
    if (typeof(T) == typeof(bool))                                         return ((System.Nullable<T>) (object?) Settings.GetPropertyAsBoolean (path));
    if (typeof(T) == typeof(byte))                                         return ((System.Nullable<T>) (object?) Settings.GetPropertyAsByte    (path));
    if (typeof(T) == typeof(char))                                         return ((System.Nullable<T>) (object?) Settings.GetPropertyAsChar    (path));
    if (typeof(T) == typeof(decimal))                                      return ((System.Nullable<T>) (object?) Settings.GetPropertyAsDecimal (path));
    if (typeof(T) == typeof(double))                                       return ((System.Nullable<T>) (object?) Settings.GetPropertyAsDouble  (path));
    if (typeof(T) == typeof(float))                                        return ((System.Nullable<T>) (object?) Settings.GetPropertyAsSingle  (path));
    if (typeof(T) == typeof(int))                                          return ((System.Nullable<T>) (object?) Settings.GetPropertyAsInt32   (path));
    if (typeof(T) == typeof(long))                                         return ((System.Nullable<T>) (object?) Settings.GetPropertyAsInt64   (path));
    if (typeof(T) == typeof(sbyte))                                        return ((System.Nullable<T>) (object?) Settings.GetPropertyAsSByte   (path));
    if (typeof(T) == typeof(short))                                        return ((System.Nullable<T>) (object?) Settings.GetPropertyAsInt16   (path));
    if (typeof(T) == typeof(uint))                                         return ((System.Nullable<T>) (object?) Settings.GetPropertyAsUInt32  (path));
    if (typeof(T) == typeof(ulong))                                        return ((System.Nullable<T>) (object?) Settings.GetPropertyAsUInt64  (path));
    if (typeof(T) == typeof(ushort))                                       return ((System.Nullable<T>) (object?) Settings.GetPropertyAsUInt16  (path));
    if (typeof(T) == typeof(System.DateTime))                              return ((System.Nullable<T>) (object?) Settings.GetPropertyAsDateTime(path));
    if (typeof(T) == typeof(Unity.Collections.NativeArray<byte>))          { byte[]? value = Settings.GetProperty(path); Unity.Collections.NativeArray<byte> property = new(value?.Length ?? 0, value is not null ? Unity.Collections.Allocator.Temp : Unity.Collections.Allocator.None); property.CopyFrom(value); return Util.Convert<Unity.Collections.NativeArray<byte>,          T>(property); }
    if (typeof(T) == typeof(Unity.Collections.NativeArray<byte>.ReadOnly)) { byte[]? value = Settings.GetProperty(path); Unity.Collections.NativeArray<byte> property = new(value?.Length ?? 0, value is not null ? Unity.Collections.Allocator.Temp : Unity.Collections.Allocator.None); property.CopyFrom(value); return Util.Convert<Unity.Collections.NativeArray<byte>.ReadOnly, T>(property.AsReadOnly()); }
    if (typeof(System.Enum).IsAssignableFrom(typeof(T)))                   return ((System.Nullable<T>) (object?) Settings.GetPropertyAsEnum<T>(path));

    return null;
  }

  [PatchMethod(AggressiveInlining)]
  public static T? GetProperty<T>(string path, object? _ = null) where T : class? /* System.IConvertible */ {
    System.Type type = typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(System.Nullable<>) ? System.Nullable.GetUnderlyingType(typeof(T)) : typeof(T);
    return (
      type.GetElementType() == typeof(byte) ? ((T?) (object?) Settings.GetProperty        (path)) :
      type == typeof(string)                ? ((T?) (object?) Settings.GetPropertyAsString(path)) :
      type == typeof(System.Xml.XmlNode)    ? ((T?) (object?) Settings.GetPropertyAsXml   (path)) :
      null
    );
  }

  public static byte[]?             GetProperty          (string path)                     => Settings.GetPropertyAsString(path) is string property ? System.Convert.FromBase64String(property)                                                                                                                                                                                                                                                                                                                                                                                                                                                 : null;
  public static bool?               GetPropertyAsBoolean (string path)                     => Settings.GetPropertyAsString(path) is string property ? !System.String.IsNullOrWhiteSpace(property) && (string.Equals(property.Trim(), "false", System.StringComparison.OrdinalIgnoreCase) || (!double.TryParse(property, out double value) || 0.0 != value))                                                                                                                                                                                                                                                                                     : null;
  public static byte?               GetPropertyAsByte    (string path)                     => Settings.GetPropertyAsString(path) is string property ? byte           .TryParse   (property, out byte            value) ? value : byte           .MaxValue                                                                                                                                                                                                                                                                                                                                                                                       : null;
  public static char?               GetPropertyAsChar    (string path)                     => Settings.GetPropertyAsString(path) is string property ? char           .TryParse   (property, out char            value) ? value : char           .MaxValue                                                                                                                                                                                                                                                                                                                                                                                       : null;
  public static System.DateTime?    GetPropertyAsDateTime(string path)                     => Settings.GetPropertyAsString(path) is string property ? System.DateTime.TryParse   (property, out System.DateTime value) ? value : System.DateTime.UnixEpoch                                                                                                                                                                                                                                                                                                                                                                                      : null;
  public static decimal?            GetPropertyAsDecimal (string path)                     => Settings.GetPropertyAsString(path) is string property ? decimal        .TryParse   (property, out decimal         value) ? value : decimal        .MinValue                                                                                                                                                                                                                                                                                                                                                                                       : null;
  public static double?             GetPropertyAsDouble  (string path)                     => Settings.GetPropertyAsString(path) is string property ? double         .TryParse   (property, out double          value) ? value : double         .NaN                                                                                                                                                                                                                                                                                                                                                                                            : null;
  public static System.Nullable<T>  GetPropertyAsEnum<T> (string path) where T : unmanaged => Settings.GetPropertyAsString(path) is string property ? System.Enum    .TryParse<T>(property, out T               value) ? value : default(T)                                                                                                                                                                                                                                                                                                                                                                                                     : null;
  public static short?              GetPropertyAsInt16   (string path)                     => Settings.GetPropertyAsString(path) is string property ? short          .TryParse   (property, out short           value) ? value : short          .MinValue                                                                                                                                                                                                                                                                                                                                                                                       : null;
  public static int?                GetPropertyAsInt32   (string path)                     => Settings.GetPropertyAsString(path) is string property ? int            .TryParse   (property, out int             value) ? value : int            .MinValue                                                                                                                                                                                                                                                                                                                                                                                       : null;
  public static long?               GetPropertyAsInt64   (string path)                     => Settings.GetPropertyAsString(path) is string property ? long           .TryParse   (property, out long            value) ? value : long           .MinValue                                                                                                                                                                                                                                                                                                                                                                                       : null;
  public static sbyte?              GetPropertyAsSByte   (string path)                     => Settings.GetPropertyAsString(path) is string property ? sbyte          .TryParse   (property, out sbyte           value) ? value : sbyte          .MinValue                                                                                                                                                                                                                                                                                                                                                                                       : null;
  public static float?              GetPropertyAsSingle  (string path)                     => Settings.GetPropertyAsString(path) is string property ? float          .TryParse   (property, out float           value) ? value : float          .NaN                                                                                                                                                                                                                                                                                                                                                                                            : null;
  public static string?             GetPropertyAsString  (string path)                     { if (Settings.GetPropertyAsXml(path) is System.Xml.XmlNode property) try { switch (property.NodeType) { case System.Xml.XmlNodeType.Attribute: case System.Xml.XmlNodeType.Text: case System.Xml.XmlNodeType.XmlDeclaration: return property.Value ?? string.Empty; case System.Xml.XmlNodeType.Element: case System.Xml.XmlNodeType.ProcessingInstruction: return property.InnerText ?? string.Empty; } } catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.InvalidOperationException) {} return null; }
  public static ushort?             GetPropertyAsUInt16  (string path)                     => Settings.GetPropertyAsString(path) is string property ? ushort.TryParse(property, out ushort value) ? value : ushort.MaxValue                                                                                                                                                                                                                                                                                                                                                                                                                     : null;
  public static uint?               GetPropertyAsUInt32  (string path)                     => Settings.GetPropertyAsString(path) is string property ? uint  .TryParse(property, out uint   value) ? value : uint  .MaxValue                                                                                                                                                                                                                                                                                                                                                                                                                     : null;
  public static ulong?              GetPropertyAsUInt64  (string path)                     => Settings.GetPropertyAsString(path) is string property ? ulong .TryParse(property, out ulong  value) ? value : ulong .MaxValue                                                                                                                                                                                                                                                                                                                                                                                                                     : null;
  public static System.Xml.XmlNode? GetPropertyAsXml     (string path)                     { try { return Settings.EnsureAsXml()?.SelectSingleNode($"/Settings/{path}"); } catch (System.Xml.XPath.XPathException) {}                                                                                                                                                                                                                                                                                                                                                                                                                      return null; }

  [PatchMethod(AggressiveInlining)]
  public static bool HasProperty(string path) {
    return Settings.GetPropertyAsXml(path) is not null;
  }

  [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
  private static void Main() {
    if (!System.IO.File.Exists(Settings.Uri.LocalPath))
    Settings.Reset(Util.Load.Asynchronously);
  }

  public static bool RemoveProperty(string path) {
    try {
      if (Settings.GetPropertyAsXml(path) is System.Xml.XmlNode property && property.ParentNode is not null) {
        if (property == property.ParentNode.RemoveChild(property))
        return Settings.Update(Settings.EnsureAsXml()!);
      }
    } catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.Xml.XmlException || exception is System.Xml.XPath.XPathException) {}

    return false;
  }

  public static bool Reset()               => Settings.Reset (Settings.LoadTimeoutMaximum);
  public static bool Reset(double timeout) => Settings.Update(Util.Load.Uri(Settings.ResetUri, Util.Load.Asynchronously == timeout ? static (object? target, in Events.LoadEvent data) => { /* if (data.payload is Unity.Collections.NativeArray<byte>.ReadOnly subdata) Settings.Update(in subdata); */ } : null, timeout, Util.Load.WithoutCache)); // ⟶ `Util.Load.Asynchronously == timeout` is asynchronously likely `true`

  [PatchMethod(AggressiveInlining)] public  static bool SetProperty         (string path, byte[]                                          value)                                  => Settings.SetProperty<string>(path, System.Convert.ToBase64String(value));
  [PatchMethod(AggressiveInlining)] public  static bool SetProperty         (string path, in System.ReadOnlySpan          <byte>          value)                                  => Settings.SetProperty        (path, Util.Array<byte>.From        (value));
  [PatchMethod(AggressiveInlining)] public  static bool SetProperty         (string path, in Unity.Collections.NativeArray<byte>          value)                                  => Settings.SetProperty        (path, Util.Array<byte>.From        (value));
  [PatchMethod(AggressiveInlining)] public  static bool SetProperty         (string path, in Unity.Collections.NativeArray<byte>.ReadOnly value)                                  => Settings.SetProperty        (path, Util.Array<byte>.From        (value));
  [PatchMethod(AggressiveInlining)] public  static bool SetProperty<T>      (string path, in T                                            value) where T : System.IConvertible    { if (Settings.HasProperty(path)) { if (Settings.GetPropertyAsXml(path) is System.Xml.XmlNode property) { try { return Settings.SetPropertyByXml<T>(property, in value); } catch (System.Xml.XPath.XPathException) {} } return false; } return Settings.AddProperty<T>(path, in value); }
  [PatchMethod(AggressiveInlining)] public  static bool SetPropertyAsBoolean(string path, bool                                            value)                                  => Settings.SetProperty<bool>   (path, value);
  [PatchMethod(AggressiveInlining)] public  static bool SetPropertyAsByte   (string path, byte                                            value)                                  => Settings.SetProperty<byte>   (path, value);
  [PatchMethod(AggressiveInlining)] public  static bool SetPropertyAsChar   (string path, char                                            value)                                  => Settings.SetProperty<char>   (path, value);
  [PatchMethod(AggressiveInlining)] public  static bool SetPropertyAsDecimal(string path, decimal                                         value)                                  => Settings.SetProperty<decimal>(path, value);
  [PatchMethod(AggressiveInlining)] public  static bool SetPropertyAsDouble (string path, double                                          value)                                  => Settings.SetProperty<double> (path, value);
  [PatchMethod(AggressiveInlining)] public  static bool SetPropertyAsEnum<T>(string path, in T                                            value) where T : unmanaged, System.Enum => Settings.SetProperty<T>      (path, value);
  [PatchMethod(AggressiveInlining)] public  static bool SetPropertyAsInt16  (string path, short                                           value)                                  => Settings.SetProperty<short>  (path, value);
  [PatchMethod(AggressiveInlining)] public  static bool SetPropertyAsInt32  (string path, int                                             value)                                  => Settings.SetProperty<int>    (path, value);
  [PatchMethod(AggressiveInlining)] public  static bool SetPropertyAsInt64  (string path, long                                            value)                                  => Settings.SetProperty<long>   (path, value);
  [PatchMethod(AggressiveInlining)] public  static bool SetPropertyAsSByte  (string path, sbyte                                           value)                                  => Settings.SetProperty<sbyte>  (path, value);
  [PatchMethod(AggressiveInlining)] public  static bool SetPropertyAsSingle (string path, float                                           value)                                  => Settings.SetProperty<float>  (path, value);
  [PatchMethod(AggressiveInlining)] public  static bool SetPropertyAsString (string path, string                                          value)                                  => Settings.SetProperty<string> (path, value);
  [PatchMethod(AggressiveInlining)] public  static bool SetPropertyAsUInt16 (string path, ushort                                          value)                                  => Settings.SetProperty<ushort> (path, value);
  [PatchMethod(AggressiveInlining)] public  static bool SetPropertyAsUInt32 (string path, uint                                            value)                                  => Settings.SetProperty<uint>   (path, value);
  [PatchMethod(AggressiveInlining)] public  static bool SetPropertyAsUInt64 (string path, ulong                                           value)                                  => Settings.SetProperty<ulong>  (path, value);
  [PatchMethod(AggressiveInlining)] public  static bool SetPropertyAsXml    (string path, System.Xml.XmlNode                              value)                                  { switch (value.NodeType) { case System.Xml.XmlNodeType.Attribute: case System.Xml.XmlNodeType.Text: case System.Xml.XmlNodeType.XmlDeclaration: return Settings.SetProperty<string>(path, value.Value); case System.Xml.XmlNodeType.Element: case System.Xml.XmlNodeType.ProcessingInstruction: return Settings.SetProperty<string>(path, value.InnerText); } return false; }

  private static bool SetPropertyByXml<T>(System.Xml.XmlNode node, in T value) where T : System.IConvertible {
    if (value is bool boolean)
    return Settings.SetPropertyByXml<int>(node, boolean ? 1 : 0);

    if (node is not null)
    try {
      switch (node.NodeType) {
        case System.Xml.XmlNodeType.Attribute     :
        case System.Xml.XmlNodeType.Text          :
        case System.Xml.XmlNodeType.XmlDeclaration: node.Value = System.Convert.ToString(value); break;

        case System.Xml.XmlNodeType.CDATA                :               //
        case System.Xml.XmlNodeType.Comment              :               //
        case System.Xml.XmlNodeType.Document             :               // ⟶ `node.InnerText = …;`
        case System.Xml.XmlNodeType.DocumentFragment     :               // ⟶ `node.InnerXml  = …;`
        case System.Xml.XmlNodeType.DocumentType         :               //
        case System.Xml.XmlNodeType.EndElement           :               //
        case System.Xml.XmlNodeType.EndEntity            :               //
        case System.Xml.XmlNodeType.Entity               :               // ⟶ `node.InnerText = …;`
        case System.Xml.XmlNodeType.EntityReference      :               // ⟶ `node.Value     = …;`
        case System.Xml.XmlNodeType.Notation             :               // ⟶ `node.InnerXml  = …;`
        case System.Xml.XmlNodeType.None                 :               //
        case System.Xml.XmlNodeType.SignificantWhitespace:               // ⟶ `node.Value = …;`
        case System.Xml.XmlNodeType.Whitespace           :               // ⟶ `node.Value = …;`
        default                                          : return false; //

        case System.Xml.XmlNodeType.Element              :
        case System.Xml.XmlNodeType.ProcessingInstruction: node.InnerText = System.Convert.ToString(value); break;
      }

      return Settings.Update(System.Xml.XmlNodeType.Document == node.NodeType ? (System.Xml.XmlDocument) node : node.OwnerDocument);
    } catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.InvalidOperationException) {}

    return false;
  }

  private static bool Update(byte[]                                          content) { if (content is not null) try { System.IO.File.WriteAllBytes(Settings.Uri.LocalPath, content); return true; } catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.ArgumentNullException || exception is System.IO.DirectoryNotFoundException || exception is System.IO.IOException || exception is System.IO.PathTooLongException || exception is System.NotSupportedException || exception is System.Security.SecurityException || exception is System.UnauthorizedAccessException) {} return false; }
  private static bool Update(string                                          content) { if (content is not null) try { System.IO.File.WriteAllText (Settings.Uri.LocalPath, content); return true; } catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.ArgumentNullException || exception is System.IO.DirectoryNotFoundException || exception is System.IO.IOException || exception is System.IO.PathTooLongException || exception is System.NotSupportedException || exception is System.Security.SecurityException || exception is System.UnauthorizedAccessException) {} return false; }
  private static bool Update(in System.ReadOnlySpan<byte>                    content) =>                                                                       Settings.Update(Util.Array<byte>.From(content)); // ⟶ Unfortunately, Unity does not support `System.IO.File.WriteAllBytes(string, System.ReadOnlySpan<byte>)` yet
  private static bool Update(System.Xml.XmlDocument                          content) =>                                                                       Settings.Update(content?.OuterXml!);
  private static bool Update(in Unity.Collections.NativeArray<byte>.ReadOnly content) =>                                                                       Settings.Update(Util.Array<byte>.From(content));
  private static bool Update(Unity.Collections.NativeArray<byte>.ReadOnly?   content) => content is Unity.Collections.NativeArray<byte>.ReadOnly subcontent && Settings.Update(subcontent);
}

public static partial class Settings {
  public static object? GetPropertyAsMonsters   (string path)                                             => default;
  public static bool    SetPropertyAsMonsters<T>(string path, in T value) where T : UnityEngine.Component => default;
}
