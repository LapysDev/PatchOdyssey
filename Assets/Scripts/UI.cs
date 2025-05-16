using PatchOdyssey;

/* … */
[UnityEngine.RequireComponent(typeof(UnityEngine.Canvas))]
[UnityEngine.RequireComponent(typeof(UnityEngine.EventSystems.EventSystem))]
[UnityEngine.RequireComponent(typeof(UnityEngine.UI.GraphicRaycaster))]
[UnityEngine.RequireComponent(typeof(UnityEngine.RectTransform))]
public class UI : UnityEngine.MonoBehaviour {
  // public  GameObjectReadOnlyDictionary                   components        = new(new GameObjectDictionary(16u) {});
  public  GameObjectDictionary                           components        = new(16u);
  private LazyMono<UnityEngine.EventSystems.EventSystem> eventSystem       = new();
  private LazyMono<UnityEngine.UI.GraphicRaycaster>      graphicsRaycaster = new();

  /* … */
  private void Awake() {
    this.eventSystem       = new(this.GetComponent<UnityEngine.EventSystems.EventSystem>);
    this.graphicsRaycaster = new(this.GetComponent<UnityEngine.UI.GraphicRaycaster>);
  }

  private void Update() {
    RefReadOnlyDictionary<string, UnityEngine.GameObject> dictionaryA = new(new RefDictionary<string, UnityEngine.GameObject>(16u) {});
    GameObjectReadOnlyDictionary                          dictionaryB = new(new GameObjectDictionary(16u) {});

    foreach (PointerInfo pointer in Util.Pointers.Any) {
      System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult> raycasts = new();

      // …
      this.graphicsRaycaster.Value.Raycast(new((UnityEngine.EventSystems.EventSystem) this.eventSystem) {position = pointer.position}, raycasts);
    }
  }
}
