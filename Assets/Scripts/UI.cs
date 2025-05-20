using PatchOdyssey;

/* … */
[UnityEngine.RequireComponent(typeof(UnityEngine.Canvas))]
[UnityEngine.RequireComponent(typeof(UnityEngine.EventSystems.EventSystem))]
[UnityEngine.RequireComponent(typeof(UnityEngine.UI.GraphicRaycaster))]
[UnityEngine.RequireComponent(typeof(UnityEngine.RectTransform))]
public class UI : UnityEngine.MonoBehaviour {
  public delegate ref UnityEngine.GameObject? RefComponentAccessor();

  /* … */
  [System.Serializable]
  public sealed class Components {
    [UnityEngine.SerializeField] public UnityEngine.GameObject?                                      background = null;
    [UnityEngine.SerializeField] public UnityEngine.GameObject?                                      combat     = null;
    [UnityEngine.SerializeField] public UnityEngine.GameObject?                                      dialogue   = null;
    [UnityEngine.SerializeField] public UnityEngine.GameObject?                                      inventory  = null;
    [UnityEngine.SerializeField] public UnityEngine.GameObject?                                      menu       = null;
    [UnityEngine.SerializeField] public UnityEngine.GameObject?                                      pause      = null;
    [UnityEngine.SerializeField] public UnityEngine.GameObject?                                      splash     = null;
    [UnityEngine.SerializeField] public (UnityEngine.GameObject? HUD, UnityEngine.GameObject? world) tooltips   = (null, null);

    /* … */
    internal System.Collections.ObjectModel.ReadOnlyDictionary<string, UI.RefComponentAccessor> AsDictionary() => new System.Collections.Generic.Dictionary<string, UI.RefComponentAccessor>() {
      {"Background",     () => ref this.background},
      {"Combat",         () => ref this.combat},
      {"Dialogue",       () => ref this.dialogue},
      {"Inventory",      () => ref this.inventory},
      {"Menu",           () => ref this.menu},
      {"Pause",          () => ref this.pause},
      {"Splash",         () => ref this.splash},
      {"Tooltips:HUD",   () => ref this.tooltips.HUD},
      {"Tooltips:World", () => ref this.tooltips.world}
    }.AsReadOnly();
  }

  #if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(UI.Components))]
    public class ComponentsDrawer : UnityEditor.PropertyDrawer {
      private UI.Components Ensure(ref UnityEditor.SerializedProperty property) {
        UI.Components components = this.fieldInfo.GetValue(property.serializedObject.targetObject) as UI.Components ?? new UI.Components();

        this.fieldInfo.SetValue(property.serializedObject.targetObject, components);
        return components;
      }

      public override float GetPropertyHeight(UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) => base.GetPropertyHeight(property, label) * 10.0f;

      public override void OnGUI(UnityEngine.Rect position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) {
        position.height = base.GetPropertyHeight(property, label);
        UnityEditor.EditorGUI.LabelField(position, label);

        foreach (System.Collections.Generic.KeyValuePair<string, UI.RefComponentAccessor> _ in this.Ensure(ref property).AsDictionary()) {
          UnityEngine.Color                              color     = UnityEngine.GUI.color;
          UnityEngine.GameObject                         component = null!;
          (UnityEngine.Rect key, UnityEngine.Rect value) positions = (
            key  : new(position.x + Util.PercOf(position.width,  5.0f), position.y += position.height, Util.PercOf(position.width, 35.0f), position.height),
            value: new(position.x + Util.PercOf(position.width, 40.0f), position.y,                    Util.PercOf(position.width, 60.0f), position.height)
          );

          // …
          UnityEngine.GUI.color = new(UnityEngine.GUI.color.r, UnityEngine.GUI.color.g, UnityEngine.GUI.color.b, UnityEngine.GUI.color.a * 0.5f);
          UnityEngine.GUI.Label(positions.key, _.Key);
          UnityEngine.GUI.color = color;

          UnityEditor.EditorGUI.BeginChangeCheck();
          component = (UnityEngine.GameObject) UnityEditor.EditorGUI.ObjectField(positions.value, _.Value(), typeof(UnityEngine.GameObject), true);

          if (UnityEditor.EditorGUI.EndChangeCheck()) {
            _.Value() = component;
            Util.Game.AskToSave();
          }
        }
      }
    }
  #endif

  /* … */
  public  GameObjectList objects33 = new() {null!};
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
