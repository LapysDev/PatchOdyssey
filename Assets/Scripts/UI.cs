using PatchOdyssey;

/* … */
[UnityEngine.RequireComponent(typeof(UnityEngine.Canvas))]
[UnityEngine.RequireComponent(typeof(UnityEngine.EventSystems.EventSystem))]
[UnityEngine.RequireComponent(typeof(UnityEngine.UI.GraphicRaycaster))]
[UnityEngine.RequireComponent(typeof(UnityEngine.RectTransform))]
public class UI : UnityEngine.MonoBehaviour {
  public  StringDictionary                               components        = new();
  private LazyMono<UnityEngine.EventSystems.EventSystem> eventSystem       = new();
  private LazyMono<UnityEngine.UI.GraphicRaycaster>      graphicsRaycaster = new();
  public  StringList                                     texts16           = new();

  /* … */
  private void Awake() {
    this.eventSystem       = new(this.GetComponent<UnityEngine.EventSystems.EventSystem>);
    this.graphicsRaycaster = new(this.GetComponent<UnityEngine.UI.GraphicRaycaster>);
  }

  private void Update() {
    foreach (PointerInfo pointer in Util.Pointers.Any) {
      System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult> raycasts = new();

      // …
      this.graphicsRaycaster.Value.Raycast(new((UnityEngine.EventSystems.EventSystem) this.eventSystem) {position = pointer.position}, raycasts);
    }
  }
}
