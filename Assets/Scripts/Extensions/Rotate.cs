#if UNITY_EDITOR
namespace PatchOdyssey {
  public static partial class Editor {
    [UnityEditor.MenuItem("Patch Odyssey/Randomize Rotation")]                    public unsafe static void RandomizeRotation            () => Editor.RotateSelected("Randomize Rotation",                    &RandomizeRotation);
    [UnityEditor.MenuItem("Patch Odyssey/Randomize Rotation Orthogonally (90°)")] public unsafe static void RandomizeRotationOrthogonally() => Editor.RotateSelected("Randomize Rotation Orthogonally (90°)", &RandomizeRotationOrthogonally);
    [UnityEditor.MenuItem("Patch Odyssey/Randomize Rotation Uniformly")]          public unsafe static void RandomizeRotationUniformly   () => Editor.RotateSelected("Randomize Rotation Uniformly",          &RandomizeRotationUniformly);

    [UnityEditor.MenuItem("CONTEXT/Transform/Patch Odyssey/Randomize Rotation")]                    public unsafe static void RandomizeTransformRotation            () => Editor.RotateSelected("Randomize Rotation",                    &RandomizeRotation);
    [UnityEditor.MenuItem("CONTEXT/Transform/Patch Odyssey/Randomize Rotation Orthogonally (90°)")] public unsafe static void RandomizeTransformRotationOrthogonally() => Editor.RotateSelected("Randomize Rotation Orthogonally (90°)", &RandomizeRotationOrthogonally);
    [UnityEditor.MenuItem("CONTEXT/Transform/Patch Odyssey/Randomize Rotation Uniformly")]          public unsafe static void RandomizeTransformRotationUniformly   () => Editor.RotateSelected("Randomize Rotation Uniformly",          &RandomizeRotationUniformly);

    /* … */
    [UnityEditor.MenuItem("Patch Odyssey/Randomize Rotation",                    validate = true)] public static bool ValidateRandomizeRotation            () => 0u != (UnityEditor.Selection.GetTransforms(UnityEditor.SelectionMode.Assets) ?? System.Array.Empty<UnityEngine.Transform>()).Length;
    [UnityEditor.MenuItem("Patch Odyssey/Randomize Rotation Orthogonally (90°)", validate = true)] public static bool ValidateRandomizeRotationOrthogonally() => Editor.ValidateRandomizeRotation();
    [UnityEditor.MenuItem("Patch Odyssey/Randomize Rotation Uniformly",          validate = true)] public static bool ValidateRandomizeRotationUniformly   () => Editor.ValidateRandomizeRotation();

    [UnityEditor.MenuItem("CONTEXT/Transform/Patch Odyssey/Randomize Rotation",                    validate = true)] public static bool ValidateRandomizeTransformRotation            () => Editor.ValidateRandomizeRotation();
    [UnityEditor.MenuItem("CONTEXT/Transform/Patch Odyssey/Randomize Rotation Orthogonally (90°)", validate = true)] public static bool ValidateRandomizeTransformRotationOrthogonally() => Editor.ValidateRandomizeRotation();
    [UnityEditor.MenuItem("CONTEXT/Transform/Patch Odyssey/Randomize Rotation Uniformly",          validate = true)] public static bool ValidateRandomizeTransformRotationUniformly   () => Editor.ValidateRandomizeRotation();

    /* … */
    private static void RandomizeRotation            (ref UnityEngine.Transform transform) => transform.localRotation = UnityEngine.Random.rotation;
    private static void RandomizeRotationOrthogonally(ref UnityEngine.Transform transform) => transform.localRotation = UnityEngine.Quaternion.Euler(new(90.0f * UnityEngine.Random.Range(0, 3), 90.0f * UnityEngine.Random.Range(0, 3), 90.0f * UnityEngine.Random.Range(0, 3)));
    private static void RandomizeRotationUniformly   (ref UnityEngine.Transform transform) => transform.localRotation = UnityEngine.Random.rotationUniform;

    private unsafe static void RotateSelected(string name, delegate*<ref UnityEngine.Transform, void> rotate) {
      UnityEngine.Transform[] transforms     = UnityEditor.Selection.GetTransforms(UnityEditor.SelectionMode.Assets) ?? System.Array.Empty<UnityEngine.Transform>();
      uint                    transformCount = (uint) transforms.Length;

      // …
      if (0u == transformCount) {
        UnityEditor.EditorUtility.DisplayDialog(name, "Please select one or more GameObject assets.", "OK");
        return;
      }

      for (uint index = 0u; index != transformCount; )
      rotate(ref transforms[index++]);
    }
  }
}
#endif
