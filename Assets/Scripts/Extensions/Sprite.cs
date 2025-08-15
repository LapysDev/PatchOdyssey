#if UNITY_EDITOR
namespace PatchOdyssey {
  public static partial class Editor {
    [UnityEditor.MenuItem("Patch Odyssey/Create Sprite")]
    public static void CreateSprite() {
      UnityEngine.Texture2D[] textures = UnityEditor.Selection.GetFiltered<UnityEngine.Texture2D>(UnityEditor.SelectionMode.Assets) ?? System.Array.Empty<UnityEngine.Texture2D>();

      // …
      if (0u == textures.Length) {
        UnityEditor.EditorUtility.DisplayDialog("Create Sprite", "Please select one or more GameObject assets.", "OK");
        return;
      }

      foreach (UnityEngine.Texture2D texture in textures)
      UnityEditor.AssetDatabase.CreateAsset(
        UnityEngine.Sprite.Create(texture, new(0.0f, 0.0f, texture.width, texture.height), new(0.5f, 0.5f), 100.0f),
        System.IO.Path.ChangeExtension(UnityEditor.AssetDatabase.GetAssetPath(texture), ".sprite.asset")
      );

      UnityEditor.AssetDatabase.SaveAssets();
      UnityEditor.AssetDatabase.Refresh   ();
    }

    [UnityEditor.MenuItem("CONTEXT/Texture2D/Patch Odyssey/Create Sprite")]
    public static void CreateTexture2DSprite() => Editor.CreateSprite();

    /* … */
    [UnityEditor.MenuItem("Patch Odyssey/Create Sprite",                   validate = true)] public static bool ValidateCreateSprite         () => 0u != (UnityEditor.Selection.GetFiltered<UnityEngine.Texture2D>(UnityEditor.SelectionMode.Assets) ?? System.Array.Empty<UnityEngine.Texture2D>()).Length;
    [UnityEditor.MenuItem("CONTEXT/Texture2D/Patch Odyssey/Create Sprite", validate = true)] public static bool ValidateCreateTexture2DSprite() => Editor.ValidateCreateSprite();
  }
}
#endif
