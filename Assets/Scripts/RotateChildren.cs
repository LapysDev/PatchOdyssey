[UnityEngine.ExecuteInEditMode]
public class RotateChildren : UnityEngine.MonoBehaviour {
  public bool rightAngleRotation = false;
  public bool toggleRotation     = false;
  public bool uniformRotation    = true;

  /* … */
  public void Update() {
    if (this.toggleRotation) {
      UnityEngine.Transform transform = this.transform;

      // …
      for (int index = transform.childCount; 0 != index--; )
        transform.GetChild(index).localRotation = this.rightAngleRotation ? UnityEngine.Quaternion.Euler(new(
          90.0f * UnityEngine.Random.Range(0, 3) * (UnityEngine.Random.value > 0.5f ? +1.0f : -1.0f),
          90.0f * UnityEngine.Random.Range(0, 3) * (UnityEngine.Random.value > 0.5f ? +1.0f : -1.0f),
          90.0f * UnityEngine.Random.Range(0, 3) * (UnityEngine.Random.value > 0.5f ? +1.0f : -1.0f)
        )) : this.uniformRotation ? UnityEngine.Random.rotationUniform : UnityEngine.Random.rotation;

      this.toggleRotation = false;
    }
  }
}
