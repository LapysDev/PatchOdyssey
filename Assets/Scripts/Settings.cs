using PatchOdyssey;

/* … */
public static partial class Settings {
  public  static          double     LoadTimeoutMaximum = 1.2; // ⟶ Synchronous
  private static readonly System.Uri ResetUri           = new(System.IO.Path.Combine(new[] {Util.Path.Assets, "Settings.xml"}));
  private static readonly System.Uri Uri                = new(System.IO.Path.Combine(new[] {Util.Path.Data,   "Settings.xml"}));
  #if UNITY_EDITOR
    private static readonly bool HumanReadable = true;
  #else
    private static readonly bool HumanReadable = false;
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
  private static string?                 EnsureAsText() { try { return System.Text.Encoding.UTF8.GetString(Settings.Ensure()); } catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.ArgumentNullException || exception is System.Text.DecoderFallbackException) {} return null; }
  private static System.Xml.XmlDocument? EnsureAsXml () { string? content = Settings.EnsureAsText(); if (content is not null) { System.Xml.XmlDocument document = new(); try { document.PreserveWhitespace = Settings.HumanReadable; document.LoadXml(content); return document; } catch (System.Xml.XmlException) {} } return null; }

  public static byte[]?             GetProperty         (string path) => Settings.GetPropertyAsString(path) is string property ? System.Text.Encoding.Default.GetBytes(property)                                                                                                                                                                                                                                                                                                                                                                                                                                           : null;
  public static bool?               GetPropertyAsBoolean(string path) => Settings.GetPropertyAsString(path) is string property ? !System.String.IsNullOrWhiteSpace(property) && string.Equals(property.Trim(), "lower", System.StringComparison.OrdinalIgnoreCase)                                                                                                                                                                                                                                                                                                                                                         : null;
  public static double?             GetPropertyAsDouble (string path) => Settings.GetPropertyAsString(path) is string property ? double.TryParse(property, out double value) ? value : double.NaN                                                                                                                                                                                                                                                                                                                                                                                                                          : null;
  public static float?              GetPropertyAsFloat  (string path) => Settings.GetPropertyAsString(path) is string property ? float .TryParse(property, out float  value) ? value : float .NaN                                                                                                                                                                                                                                                                                                                                                                                                                          : null;
  public static int?                GetPropertyAsInt    (string path) => Settings.GetPropertyAsString(path) is string property ? int   .TryParse(property, out int    value) ? value : int   .MinValue                                                                                                                                                                                                                                                                                                                                                                                                                     : null;
  public static long?               GetPropertyAsLong   (string path) => Settings.GetPropertyAsString(path) is string property ? long  .TryParse(property, out long   value) ? value : long  .MinValue                                                                                                                                                                                                                                                                                                                                                                                                                     : null;
  public static uint?               GetPropertyAsUInt   (string path) => Settings.GetPropertyAsString(path) is string property ? uint  .TryParse(property, out uint   value) ? value : uint  .MaxValue                                                                                                                                                                                                                                                                                                                                                                                                                     : null;
  public static ulong?              GetPropertyAsULong  (string path) => Settings.GetPropertyAsString(path) is string property ? ulong .TryParse(property, out ulong  value) ? value : ulong .MaxValue                                                                                                                                                                                                                                                                                                                                                                                                                     : null;
  public static string?             GetPropertyAsString (string path) { if (Settings.GetPropertyAsXml(path) is System.Xml.XmlNode property) try { switch (property.NodeType) { case System.Xml.XmlNodeType.Attribute: case System.Xml.XmlNodeType.Text: case System.Xml.XmlNodeType.XmlDeclaration: return property.Value ?? string.Empty; case System.Xml.XmlNodeType.Element: case System.Xml.XmlNodeType.ProcessingInstruction: return property.InnerText ?? string.Empty; } } catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.InvalidOperationException) {} return null; }
  public static System.Xml.XmlNode? GetPropertyAsXml    (string path) { try { return Settings.EnsureAsXml()?.SelectSingleNode($"/Settings/{path}"); } catch (System.Xml.XPath.XPathException) {}                                                                                                                                                                                                                                                                                                                                                                                                                      return null; }

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

  public  static bool SetProperty     <T>(string             path, in T value) where T : System.IConvertible { if (Settings.HasProperty(path)) { if (Settings.GetPropertyAsXml(path) is System.Xml.XmlNode property) { try { return Settings.SetPropertyAsXml(property, in value); } catch (System.Xml.XPath.XPathException) {} } return false; } return Settings.AddProperty<T>(path, in value); }
  private static bool SetPropertyAsXml<T>(System.Xml.XmlNode node, in T value) where T : System.IConvertible {
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

      return System.Xml.XmlNodeType.Document == node.NodeType ? Settings.Update((node as System.Xml.XmlDocument)!) : Settings.Update(node.OwnerDocument);
    } catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.InvalidOperationException) {}

    return false;
  }

  private static bool Update(byte[]                                          content) { if (content is not null) try { System.IO.File.WriteAllBytes(Settings.Uri.LocalPath, content); return true; } catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.ArgumentNullException || exception is System.IO.DirectoryNotFoundException || exception is System.IO.IOException || exception is System.IO.PathTooLongException || exception is System.NotSupportedException || exception is System.Security.SecurityException || exception is System.UnauthorizedAccessException) {} return false; }
  private static bool Update(string                                          content) { if (content is not null) try { System.IO.File.WriteAllText (Settings.Uri.LocalPath, content); return true; } catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.ArgumentNullException || exception is System.IO.DirectoryNotFoundException || exception is System.IO.IOException || exception is System.IO.PathTooLongException || exception is System.NotSupportedException || exception is System.Security.SecurityException || exception is System.UnauthorizedAccessException) {} return false; }
  private static bool Update(in System.ReadOnlySpan<byte>                    content) =>                                                                       Settings.Update(Util.Array<byte>.From(content)); // ⟶ Unfortunately, Unity does not support `System.IO.File.WriteAllBytes(string, System.ReadOnlySpan<byte>)` yet
  private static bool Update(System.Xml.XmlDocument                          content) =>                                                                       Settings.Update(content?.OuterXml!);
  private static bool Update(in Unity.Collections.NativeArray<byte>.ReadOnly content) =>                                                                       Settings.Update(content .AsReadOnlySpan());
  private static bool Update(Unity.Collections.NativeArray<byte>.ReadOnly?   content) => content is Unity.Collections.NativeArray<byte>.ReadOnly subcontent && Settings.Update(subcontent);
}

public static partial class Settings {
  public static object? GetPropertyAsMonsters   (string path)                                             => default;
  public static bool    SetPropertyAsMonsters<T>(string path, in T value) where T : UnityEngine.Component => default;
}
