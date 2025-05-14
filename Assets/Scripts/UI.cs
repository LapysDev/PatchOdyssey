using PatchOdyssey;

/* … */
[UnityEngine.RequireComponent(typeof(UnityEngine.Canvas))]
[UnityEngine.RequireComponent(typeof(UnityEngine.EventSystems.EventSystem))]
[UnityEngine.RequireComponent(typeof(UnityEngine.UI.GraphicRaycaster))]
[UnityEngine.RequireComponent(typeof(UnityEngine.RectTransform))]
public class UI : UnityEngine.MonoBehaviour {
  private LazyMono<UnityEngine.EventSystems.EventSystem> eventSystem;
  private LazyMono<UnityEngine.UI.GraphicRaycaster>      graphicsRaycaster;

  /* … */
  private void Awake() {
    this.eventSystem       = new(this.GetComponent<UnityEngine.EventSystems.EventSystem>);
    this.graphicsRaycaster = new(this.GetComponent<UnityEngine.UI.GraphicRaycaster>);
  }

  private void Update() {
    UnityEngine.Debug.Log(Util.Pointers.Any);
    // System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult> uiRaycasts = new();

    // …
    // this.graphicsRaycaster.Value.Raycast(new((UnityEngine.EventSystems.EventSystem) this.eventSystem) {position = information.value}, uiRaycasts);
  }
}
