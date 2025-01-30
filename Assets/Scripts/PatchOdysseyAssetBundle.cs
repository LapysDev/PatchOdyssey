using PatchOdyssey;

/* … */
public class PatchOdysseyBuildAssetBundle {
  #if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Build AssetBundle")]
    static void Build() {
      const string path = "Assets/StreamingAssets";

      // …
      if (!System.IO.Directory.Exists(path))
      System.IO.Directory.CreateDirectory(path);

      UnityEditor.BuildPipeline.BuildAssetBundles(path, UnityEditor.BuildAssetBundleOptions.None, UnityEditor.BuildTarget.StandaloneWindows);
      UnityEngine.Debug.Log($"[{UnityEngine.Application.productName}]: AssetBundle built successfully!");
    }
  #endif
}
