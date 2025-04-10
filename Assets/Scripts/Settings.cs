// using PatchOdyssey;

// /* … */
// public static class Settings {
//   public  static          float  DELAY_MAXIMUM = 2.0f;
//   private static readonly string RESET_URI     = System.IO.Path.Combine(new[] {Util.GetAssetPath(), "Settings.xml"});
//   private static readonly string URI           = System.IO.Path.Combine(new[] {Util.GetDataPath (), "Settings.xml"});

//   /* … */
//   public static bool AddProperty<T>(string path, T value) where T : System.IConvertible {
//     System.Xml.XmlDocument? data = Settings.EnsureAsXML();

//     // …
//     if (null != data && null != path) {
//       string[]            components = path.Split('/');
//       System.Xml.XmlNode? node       = data as System.Xml.XmlNode;

//       // …
//       path = "";

//       try { node = data.SelectSingleNode("/Settings"); }
//       catch (System.Xml.XPath.XPathException) {}

//       if (null != node) {
//         System.Xml.XmlNode  prenode = node;
//         System.Xml.XmlNode? subnode = null;

//         // …
//         for (int index = 0; index != components.Length; ++index) {
//           if (index == components.Length - 1) {
//             System.Xml.XmlElement element = data.CreateElement(components[index]);

//             // …
//             element.InnerText = System.Convert.ToString(value);

//             try {
//               if (
//                 null != subnode
//                 ? element == subnode!.AppendChild(element) ? node == prenode!.AppendChild(node!) : false
//                 : element == node   !.AppendChild(element)
//               ) return Settings.Update(data);
//             } catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.InvalidOperationException) {}

//             continue;
//           }

//           prenode = node!;
//           path   += components[index];

//           try { node = data.SelectSingleNode($"/Settings/{path}"); }
//           catch (System.Xml.XPath.XPathException) {}

//           if (null != node) {
//             path += '/';
//             continue;
//           }

//           for (; index != components.Length - 1; ++index) {
//             System.Xml.XmlElement element = data.CreateElement(components[index]);

//             // …
//             if (null != subnode) {
//               try { if (element != subnode.AppendChild(element)) return false; }
//               catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.InvalidOperationException) {}
//             }

//             node    = null == node ? element : node;
//             subnode = element;
//           }

//           --index;
//         }
//       }
//     }

//     return false;
//   }

//   private static byte[]? Ensure() {
//     if (!System.IO.File.Exists(URI)) {
//       if (!Settings.Reset(DELAY_MAXIMUM))
//       return null;
//     }

//     return System.IO.File.ReadAllBytes(URI);
//   }

//   private static string? EnsureAsText() {
//     try { return System.Text.Encoding.UTF8.GetString(Settings.Ensure()); }
//     catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.ArgumentNullException || exception is System.Text.DecoderFallbackException) {}

//     return null;
//   }

//   private static System.Xml.XmlDocument? EnsureAsXML() {
//     string? content = Settings.EnsureAsText();

//     // …
//     if (null != content) {
//       System.Xml.XmlDocument document = new();

//       try { document.LoadXml(content); return document; }
//       catch (System.Xml.XmlException) {}
//     }

//     return null;
//   }

//   public static byte[]  GetProperty         (string path) => default!;
//   public static bool?   GetPropertyAsBoolean(string path) { string? property = Settings.GetPropertyAsString(path); return null != property ? !System.String.IsNullOrWhiteSpace(property) && property.Trim().ToLower() != "false" : null; }
//   public static double? GetPropertyAsDouble (string path) { string? property = Settings.GetPropertyAsString(path); return null != property ? double.TryParse(property, out double value) ? value : double.NaN                    : null; }
//   public static float?  GetPropertyAsFloat  (string path) { string? property = Settings.GetPropertyAsString(path); return null != property ? float .TryParse(property, out float  value) ? value : float .NaN                    : null; }
//   public static int?    GetPropertyAsInt    (string path) { string? property = Settings.GetPropertyAsString(path); return null != property ? int   .TryParse(property, out int    value) ? value : int   .MinValue               : null; }
//   public static long?   GetPropertyAsLong   (string path) { string? property = Settings.GetPropertyAsString(path); return null != property ? long  .TryParse(property, out long   value) ? value : long  .MinValue               : null; }
//   public static uint?   GetPropertyAsUInt   (string path) { string? property = Settings.GetPropertyAsString(path); return null != property ? uint  .TryParse(property, out uint   value) ? value : uint  .MaxValue               : null; }
//   public static ulong?  GetPropertyAsULong  (string path) { string? property = Settings.GetPropertyAsString(path); return null != property ? ulong .TryParse(property, out ulong  value) ? value : ulong .MaxValue               : null; }

//   public static string? GetPropertyAsString(string path) {
//     System.Xml.XmlNode? property = Settings.GetPropertyAsXML(path);

//     // …
//     if (null != property)
//     try {
//       switch (property.NodeType) {
//         case System.Xml.XmlNodeType.Attribute     :
//         case System.Xml.XmlNodeType.Text          :
//         case System.Xml.XmlNodeType.XmlDeclaration: return property.Value ?? "";

//         case System.Xml.XmlNodeType.Element              :
//         case System.Xml.XmlNodeType.ProcessingInstruction: return property.InnerText ?? "";
//       }
//     } catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.InvalidOperationException) {}

//     return null;
//   }

//   public static System.Xml.XmlNode? GetPropertyAsXML(string path) {
//     try {
//       if (null != path)
//       return Settings.EnsureAsXML()?.SelectSingleNode($"/Settings/{path}");
//     } catch (System.Xml.XPath.XPathException) {}

//     return null;
//   }

//   public static bool HasProperty(string path) {
//     return null != Settings.GetPropertyAsXML(path);
//   }

//   [UnityEngine.RuntimeInitializeOnLoadMethod]
//   private static void Main() {
//     if (!System.IO.File.Exists(URI))
//     Settings.Reset(1.0f);
//   }

//   public static bool RemoveProperty(string path) {
//     try {
//       System.Xml.XmlDocument? data     = Settings.EnsureAsXML();
//       System.Xml.XmlNode?     property = data?.SelectSingleNode($"/Settings/{path}");

//       // …
//       if (null != property ? null != property.ParentNode : false) {
//         if (property == property.ParentNode.RemoveChild(property))
//         return Settings.Update(data!);
//       }
//     } catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.Xml.XmlException || exception is System.Xml.XPath.XPathException) {}

//     return false;
//   }

//   public static bool Reset(float? loadDurationMaximum = null) {
//     byte[] data = Util.LoadURI(RESET_URI, null, loadDurationMaximum ?? DELAY_MAXIMUM, Util.LoadDirectly)!;
//     return null != data && Settings.Update(data);
//   }

//   public static bool SetProperty<T>(string path, T value) where T : System.IConvertible {
//     if (Settings.HasProperty(path)) {
//       if (null != path) {
//         try { return Settings.SetPropertyAsXML(Settings.EnsureAsXML()?.SelectSingleNode($"/Settings/{path}")!, value); }
//         catch (System.Xml.XPath.XPathException) {}
//       }

//       return false;
//     }

//     return Settings.AddProperty<T>(path, value);
//   }

//   private static bool SetPropertyAsXML<T>(System.Xml.XmlNode node, T value) where T : System.IConvertible {
//     if (null != node)
//     try {
//       switch (node.NodeType) {
//         case System.Xml.XmlNodeType.Attribute     :
//         case System.Xml.XmlNodeType.Text          :
//         case System.Xml.XmlNodeType.XmlDeclaration: node.Value = System.Convert.ToString(value); break;

//         case System.Xml.XmlNodeType.CDATA                :               //
//         case System.Xml.XmlNodeType.Comment              :               //
//         case System.Xml.XmlNodeType.Document             :               // → `node.InnerText = …;`
//         case System.Xml.XmlNodeType.DocumentFragment     :               // → `node.InnerXml  = …;`
//         case System.Xml.XmlNodeType.DocumentType         :               //
//         case System.Xml.XmlNodeType.EndElement           :               //
//         case System.Xml.XmlNodeType.EndEntity            :               //
//         case System.Xml.XmlNodeType.Entity               :               // → `node.InnerText = …;`
//         case System.Xml.XmlNodeType.EntityReference      :               // → `node.Value     = …;`
//         case System.Xml.XmlNodeType.Notation             :               // → `node.InnerXml  = …;`
//         case System.Xml.XmlNodeType.None                 :               //
//         case System.Xml.XmlNodeType.SignificantWhitespace:               // → `node.Value = …;`
//         case System.Xml.XmlNodeType.Whitespace           :               // → `node.Value = …;`
//         default                                          : return false; //

//         case System.Xml.XmlNodeType.Element              :
//         case System.Xml.XmlNodeType.ProcessingInstruction: node.InnerText = System.Convert.ToString(value); break;
//       }

//       return System.Xml.XmlNodeType.Document == node.NodeType ? Settings.Update((node as System.Xml.XmlDocument)!) : Settings.Update(node.OwnerDocument);
//     } catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.InvalidOperationException) {}

//     return false;
//   }

//   private static bool Update(byte[] content) {
//     try { System.IO.File.WriteAllBytes(URI, content ?? new byte[] {}); return true; }
//     catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.ArgumentNullException || exception is System.IO.DirectoryNotFoundException || exception is System.IO.IOException || exception is System.IO.PathTooLongException || exception is System.NotSupportedException || exception is System.Security.SecurityException || exception is System.UnauthorizedAccessException) {}

//     return false;
//   }

//   private static bool Update(string content) {
//     try { System.IO.File.WriteAllText(URI, content ?? ""); return true; }
//     catch (System.Exception exception) when (exception is System.ArgumentException || exception is System.ArgumentNullException || exception is System.IO.DirectoryNotFoundException || exception is System.IO.IOException || exception is System.IO.PathTooLongException || exception is System.NotSupportedException || exception is System.Security.SecurityException || exception is System.UnauthorizedAccessException) {}

//     return false;
//   }

//   private static bool Update(System.Xml.XmlDocument content) {
//     return Settings.Update(content?.OuterXml!);
//   }

//   /* … → Game-specific */
//   public static object? GetPropertyAsMonsters(string path)               => default;
//   public static bool    SetPropertyAsMonsters(string path, object value) => default;
// }
