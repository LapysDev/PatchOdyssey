#if UNITY_EDITOR
namespace PatchOdyssey {
  public static partial class Editor {
    [UnityEditor.MenuItem("Patch Odyssey/No Material")]
    public static void NoMaterialImport() {
      foreach (UnityEngine.Object selected in UnityEditor.Selection.objects) {
        string                     path     = UnityEditor.AssetDatabase.GetAssetPath(selected);
        UnityEditor.ModelImporter? importer = UnityEditor.AssetImporter.GetAtPath(path) as UnityEditor.ModelImporter;

        // …
        if (null == importer)
        continue;

        importer.materialImportMode = UnityEditor.ModelImporterMaterialImportMode.None;
        UnityEditor.AssetDatabase.ImportAsset(path, UnityEditor.ImportAssetOptions.ForceUpdate);
      }
    }
  }
}
#endif
