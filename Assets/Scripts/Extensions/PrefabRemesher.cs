#if UNITY_EDITOR
namespace PatchOdyssey {
  public sealed class PrefabRemesher : UnityEditor.EditorWindow {
    private string                  refreshExtension = "FBX";
    private UnityEngine.GameObject? source           = null;
    private string                  sourceExtension  = "MB";

    // …
    unsafe private static bool MeshEquals(UnityEngine.Mesh meshA, UnityEngine.Mesh meshB) {
      if (!string.Equals(meshA.name, meshB.name, System.StringComparison.Ordinal))
      return false;

      try {
        using (UnityEngine.Mesh.MeshDataArray meshDataArrayA = UnityEngine.Mesh.AcquireReadOnlyMeshData(meshA))
        using (UnityEngine.Mesh.MeshDataArray meshDataArrayB = UnityEngine.Mesh.AcquireReadOnlyMeshData(meshB)) {
          UnityEngine.Mesh.MeshData meshDataA         = meshDataArrayA[0];
          UnityEngine.Mesh.MeshData meshDataB         = meshDataArrayB[0];
          int                       meshVerticesCount = meshA.vertexCount;

          // …
          if (meshVerticesCount != meshB.vertexCount)
          return false;

          using (Unity.Collections.NativeArray<UnityEngine.Vector3> meshVerticesA = new(meshVerticesCount, Unity.Collections.Allocator.Temp))
          using (Unity.Collections.NativeArray<UnityEngine.Vector3> meshVerticesB = new(meshVerticesCount, Unity.Collections.Allocator.Temp)) {
            meshDataA.GetVertices(meshVerticesA);
            meshDataB.GetVertices(meshVerticesB);

            // …
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            static bool VertexEquals(float a, float b) => 1.0e-4 >= UnityEngine.Mathf.Abs(a - b);

            for (UnityEngine.Vector3*
              meshVerticesIteratorA = (UnityEngine.Vector3*) Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(meshVerticesA),
              meshVerticesIteratorB = (UnityEngine.Vector3*) Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(meshVerticesB)
            ; 0 != meshVerticesCount--; ++meshVerticesIteratorA, ++meshVerticesIteratorB)
            if (
              !VertexEquals(meshVerticesIteratorA -> x, meshVerticesIteratorB -> x) ||
              !VertexEquals(meshVerticesIteratorA -> y, meshVerticesIteratorB -> y) ||
              !VertexEquals(meshVerticesIteratorA -> z, meshVerticesIteratorB -> z)
            ) return false;
          }
        }
      } catch (System.InvalidOperationException exception) { UnityEngine.Debug.LogError(exception.Message); } // ->> “nOt AlLoWeD tO aCcEsS vErTeX dAtA oN mEsH (iSrEaDaBlE iS fAlSe; ReAd/WrItE mUsT bE eNaBlEd In ImPoRt SeTtInGs)”

      return true;
    }

    private static void MeshRefresh(UnityEngine.GameObject refreshed, string extension, string refreshedExtension, bool strict = false) {
      string prefabPath = UnityEditor.PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(refreshed);

      // …
      extension          = '.' + extension.ToLower();
      refreshed          = !string.IsNullOrEmpty(prefabPath) ? UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(prefabPath) ?? refreshed : refreshed;
      refreshedExtension = '.' + refreshedExtension.ToLower();

      // …
      bool Refresh<T>() where T : UnityEngine.Component {
        bool isRefreshed = true;

        // …
        foreach (T component in refreshed.GetComponentsInChildren<T>(true)) {
          UnityEngine.Mesh? mesh = component switch { UnityEngine.MeshFilter filter => filter.sharedMesh, UnityEngine.SkinnedMeshRenderer renderer => renderer.sharedMesh, _ => null as UnityEngine.Mesh };
          UnityEngine.Mesh? refreshedMesh;

          // …
          if (null != mesh) {
            string meshPath = UnityEditor.AssetDatabase.GetAssetPath(mesh);
            string refreshedMeshPath;

            // …
            if (!strict || meshPath.EndsWith(extension)) {
              refreshedMeshPath = System.IO.Path.ChangeExtension(meshPath, refreshedExtension);
              refreshedMesh     = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Mesh>(refreshedMeshPath);

              foreach (UnityEngine.Object subasset in (UnityEditor.AssetDatabase.LoadAllAssetRepresentationsAtPath(refreshedMeshPath) ?? System.Array.Empty<UnityEngine.Object>()))
              if (subasset is UnityEngine.Mesh submesh && PrefabRemesher.MeshEquals(mesh, submesh)) {
                refreshedMesh = submesh;
                break;
              }

              if (refreshedMesh is not null) {
                UnityEngine.Debug.Log($"“{refreshed.name}” refreshed “{mesh.name}” reference from `{meshPath}` to `{refreshedMeshPath}/{refreshedMesh.name}`");

                switch (component) {
                  case UnityEngine.MeshFilter          filter:   filter  .sharedMesh = refreshedMesh; continue;
                  case UnityEngine.SkinnedMeshRenderer renderer: renderer.sharedMesh = refreshedMesh; continue;
                }
              } else UnityEngine.Debug.LogWarning($"“{refreshed.name}” not refreshed from `{meshPath}`");
            }
          }

          isRefreshed = false;
        }

        return isRefreshed;
      }

      if (!Refresh<UnityEngine.MeshFilter>() && !Refresh<UnityEngine.SkinnedMeshRenderer>()) {
        UnityEngine.Debug.Log($"“{refreshed.name}” refreshed no meshes");
        return;
      }

      // …
      try {
        if      (UnityEditor.PrefabUtility.IsPartOfPrefabAsset   (refreshed)) UnityEditor.PrefabUtility.SavePrefabAsset    (refreshed);
        else if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(refreshed)) UnityEditor.PrefabUtility.ApplyPrefabInstance(refreshed, UnityEditor.InteractionMode.UserAction);
      } catch (System.ArgumentException exception) { UnityEngine.Debug.LogError(exception.Message); } // ->> “cAn'T sAvE a PrEfAb InStAnCe”

      UnityEditor.AssetDatabase.SaveAssets();
      UnityEngine.Debug.Log($"“{refreshed.name}” refreshed meshes");
    }

    private void OnGUI() {
      UnityEngine.GUILayout.Label("Copy Prefab Overrides", UnityEditor.EditorStyles.boldLabel);

      this.sourceExtension  = UnityEditor.EditorGUILayout.TextField("Source File Extension", this.sourceExtension) .ToUpper();
      this.refreshExtension = UnityEditor.EditorGUILayout.TextField("Target File Extension", this.refreshExtension).ToUpper();

      if (UnityEngine.GUILayout.Button("Refresh mesh references") && null != this.source && this.refreshExtension != this.sourceExtension)
      MeshRefresh(this.source, this.sourceExtension, this.refreshExtension);
    }

    [UnityEditor.MenuItem("Patch Odyssey/Prefab Remesher")]
    public static void Remesh() {
      PrefabRemesher          remesher  = UnityEditor.EditorWindow.GetWindow<PrefabRemesher>("Prefab Remesher");
      UnityEngine.Transform[] transforms = UnityEditor.Selection.GetTransforms(UnityEditor.SelectionMode.Assets) ?? System.Array.Empty<UnityEngine.Transform>();

      remesher.source = 0u != transforms.Length ? transforms[0].gameObject : null;
    }

    [UnityEditor.MenuItem("Patch Odyssey/Prefab Remesher (`.mb` to `.fbx`)")]
    public static void RemeshMBtoFBX() {
      UnityEngine.Transform[] transforms = UnityEditor.Selection.GetTransforms(UnityEditor.SelectionMode.Assets) ?? System.Array.Empty<UnityEngine.Transform>();

      if (0u != transforms.Length)
      MeshRefresh(transforms[0].gameObject, "MB", "FBX");
    }
  }
}
#endif
