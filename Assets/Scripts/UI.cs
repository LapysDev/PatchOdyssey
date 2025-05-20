using PatchOdyssey;

/* … */
[UnityEngine.RequireComponent(typeof(UnityEngine.Canvas))]
[UnityEngine.RequireComponent(typeof(UnityEngine.EventSystems.EventSystem))]
[UnityEngine.RequireComponent(typeof(UnityEngine.UI.GraphicRaycaster))]
[UnityEngine.RequireComponent(typeof(UnityEngine.RectTransform))]
public class UI : UnityEngine.MonoBehaviour {
  [System.Serializable]
  public class Components {
    public UnityEngine.GameObject? background                                    = null;
    public UnityEngine.GameObject? combat                                        = null;
    public UnityEngine.GameObject? dialogue                                      = null;
    public UnityEngine.GameObject? inventory                                     = null;
    public UnityEngine.GameObject? menu                                          = null;
    public UnityEngine.GameObject? pause                                         = null;
    public UnityEngine.GameObject? splash                                        = null;
    public (UnityEngine.GameObject? HUD, UnityEngine.GameObject? world) tooltips = (null, null);
  }

  #if UNITY_EDITOR
    // [UnityEditor.CustomPropertyDrawer(typeof(UI.Components))]
    // public class UIComponentsDrawer : UnityEditor.PropertyDrawer {}
  #endif

  /* … */
  public  UI.Components                                  components        = new();
  private LazyMono<UnityEngine.EventSystems.EventSystem> eventSystem       = new();
  private LazyMono<UnityEngine.UI.GraphicRaycaster>      graphicsRaycaster = new();

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
