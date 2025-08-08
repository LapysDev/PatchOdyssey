[UnityEngine.DefaultExecutionOrder(0)]
[UnityEngine.DisallowMultipleComponent]
public sealed class Assets : UnityEngine.MonoBehaviour {
  [System.Serializable]
  public /* readonly */ struct Bullet {
    public UnityEngine.Material?  material;
    public UnityEngine.GameObject prefabrication;
  }

  [System.Serializable]
  public /* readonly */ struct Lasoo {
    public UnityEngine.Material?  captureIndicatorMaterial;
    public UnityEngine.GameObject meshPrefabrication;
    public UnityEngine.GameObject ropeMeshPrefabrication;
  }

  [System.Serializable]
  public /* readonly */ struct Hairs {
    public /* readonly */ System.Collections.Generic.List<UnityEngine.Material>   materials;
    public /* readonly */ System.Collections.Generic.List<UnityEngine.GameObject> meshPrefabrications;
  }

  [System.Serializable]
  public /* readonly */ struct Primitives /* ->> `UnityEngine.GameObject.CreatePrimitive(…)` but more robust; Not intended for non-performance-critical code */ {
    public UnityEngine.GameObject? capsule;
    public UnityEngine.GameObject? cube;
    public UnityEngine.GameObject? cylinder;
    public UnityEngine.GameObject? plane;
    public UnityEngine.GameObject? quad;
    public UnityEngine.GameObject? sphere;
  }

  [System.Serializable]
  public /* readonly */ struct Spawnable {
    [System.Serializable]
    public /* readonly */ struct Monsters {
      public /* readonly */ System.Collections.Generic.List<UnityEngine.GameObject> antilleryPrefabrication;
      public /* readonly */ System.Collections.Generic.List<UnityEngine.GameObject> borkaPrefabrication;
      public /* readonly */ System.Collections.Generic.List<UnityEngine.GameObject> molemPrefabrication;
      public /* readonly */ System.Collections.Generic.List<UnityEngine.GameObject> sirpensPrefabrication;
      public /* readonly */ System.Collections.Generic.List<UnityEngine.GameObject> tyragePrefabrication;
    }

    [System.Serializable]
    public /* readonly */ struct Tamers {
      public /* readonly */ System.Collections.Generic.List<UnityEngine.GameObject> magnatePrefabrication;
      public /* readonly */ System.Collections.Generic.List<UnityEngine.GameObject> nomadPrefabrication;
    }

    /* … */
    public /* readonly */ Assets.Spawnable.Monsters monsters;
    public /* readonly */ Assets.Spawnable.Tamers   tamers;
  }

  [System.Serializable]
  public /* readonly */ struct UI {
    public /* readonly */ UnityEngine.UI.RawImage ammobar;
    public /* readonly */ UnityEngine.UI.RawImage healthbar;
  }

  /* … */
  public static Assets main = null!;

  public Assets.Bullet                 bullet               = new() {material  = null,    prefabrication      = null!};
  public Assets.Hairs                  hairs                = new() {materials = new(21), meshPrefabrications = new(8)};
  public Assets.Lasoo                  lasoo                = new() {captureIndicatorMaterial = null, meshPrefabrication = null!, ropeMeshPrefabrication = null!};
  public bool                          outlineAutomatically = true;
  public UnityEngine.Material?         outlineMaterial      = null;
  public Assets.Primitives             primitives           = new() {capsule = null, cube = null, cylinder = null, plane = null, quad = null, sphere = null};
  public bool                          shadowAutomatically  = true;
  public UnityEngine.Material?         shadowMaterial       = null;
  public Assets.Spawnable              spawnables           = new() {monsters = new() {antilleryPrefabrication = new(1), borkaPrefabrication = new(1), molemPrefabrication = new(1), sirpensPrefabrication = new(1), tyragePrefabrication = new(1)}, tamers = new() {magnatePrefabrication = new(2), nomadPrefabrication = new(2)}};
  public Assets.UI                     ui                   = new() {ammobar = null, healthbar = null};
  public UnityEngine.Rendering.Volume? volume               = null;

  /* … */
  private void Awake() {
    if (Assets.main is not null && Assets.main != this) {
      UnityEngine.Object.DestroyImmediate(this, false);
      return;
    }

    Assets.main                     = this;
    Assets.main.gameObject.isStatic = true;

    foreach (UnityEngine.Transform transform in this.transform)
    transform.SetParent(null, true);

    this.transform.SetParent(null, true);
    UnityEngine.Object.DontDestroyOnLoad(Assets.main.gameObject);
  }

  private void OnDestroy() {
    if (Assets.main == this)
    Assets.main = null!;
  }

  private void Start() => Assets.main.gameObject.SetActive(false);
}
