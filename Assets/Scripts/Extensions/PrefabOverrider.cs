#if UNITY_EDITOR
namespace PatchOdyssey {
  public sealed class PrefabOverrider : UnityEditor.EditorWindow {
    private UnityEngine.GameObject? source = null;
    private UnityEngine.GameObject? target = null;

    // …
    private static void CopyOverrides(UnityEngine.GameObject sourceGameObject, UnityEngine.GameObject targetGameObject) {
      UnityEngine.Component[] sourceComponents = sourceGameObject.GetComponents<UnityEngine.Component>();
      UnityEngine.Transform   sourceTransform  = sourceGameObject.transform;
      UnityEngine.Component[] targetComponents = targetGameObject.GetComponents<UnityEngine.Component>();
      UnityEngine.Transform   targetTransform  = targetGameObject.transform;

      // …
      targetGameObject.SetActive(sourceGameObject.activeSelf);

      targetGameObject.isStatic = sourceGameObject.isStatic;
      targetGameObject.layer    = sourceGameObject.layer;
      targetGameObject.tag      = sourceGameObject.tag;

      for (int index = 0; index != sourceComponents.Length; ++index) {
        ref readonly UnityEngine.Component sourceComponent = ref sourceComponents[index];

        // …
        #if true
          if (sourceComponent is not UnityEngine.MeshFilter && sourceComponent is not UnityEngine.SkinnedMeshRenderer)
        #endif
        {
          UnityEditor.SerializedObject targetSerializedObject = new(index >= targetComponents.Length ? targetGameObject.AddComponent(sourceComponent.GetType()) : targetComponents[index]);

          // …
          for (UnityEditor.SerializedProperty sourceSerializedProperty = (new UnityEditor.SerializedObject(sourceComponent)).GetIterator(); sourceSerializedProperty.NextVisible(true); )
            targetSerializedObject.CopyFromSerializedProperty(sourceSerializedProperty);

          targetSerializedObject.ApplyModifiedProperties();
        }
      }

      // …
      for (int index = 0; index != sourceTransform.childCount && index != targetTransform.childCount; ++index)
      PrefabOverrider.CopyOverrides(sourceTransform.GetChild(index).gameObject, targetTransform.GetChild(index).gameObject);
    }

    private void OnGUI() {
      UnityEngine.GUILayout.Label("Copy Prefab Overrides", UnityEditor.EditorStyles.boldLabel);

      this.source = (UnityEngine.GameObject) UnityEditor.EditorGUILayout.ObjectField("Source Game Object", this.source, typeof(UnityEngine.GameObject), true);
      this.target = (UnityEngine.GameObject) UnityEditor.EditorGUILayout.ObjectField("Target Game Object", this.target, typeof(UnityEngine.GameObject), true);

      if (UnityEngine.GUILayout.Button("Copy Overrides")) {
        if (null == this.source || null == this.target) {
          UnityEngine.Debug.LogWarning("Assign both Source and Target objects");
          return;
        }

        PrefabOverrider.CopyOverrides(this.source, this.target);
        UnityEngine.Debug.Log($"Overrides copied from `{this.source}` to `{this.target}`");
      }
    }

    [UnityEditor.MenuItem("Patch Odyssey/Prefab Overrider")]
    public static void Override() {
      PrefabOverrider         overrider  = UnityEditor.EditorWindow.GetWindow<PrefabOverrider>("Prefab Overrider");
      UnityEngine.Transform[] transforms = UnityEditor.Selection.GetTransforms(UnityEditor.SelectionMode.Assets) ?? System.Array.Empty<UnityEngine.Transform>();

      overrider.source = 0u != transforms.Length ? transforms[0].gameObject : null;
    }
  }
}
#endif
