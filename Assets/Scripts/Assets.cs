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
  public /* readonly */ struct Monsters {
    public UnityEngine.GameObject antilleryPrefabrication;
    public UnityEngine.GameObject borkaPrefabrication;
    public UnityEngine.GameObject molemPrefabrication;
    public UnityEngine.GameObject sirpensPrefabrication;
    public UnityEngine.GameObject tyragePrefabrication;
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

  /* … */
  public static Assets main = null!;

  public Assets.Bullet                 bullet               = new() {material  = null,    prefabrication      = null!};
  public Assets.Hairs                  hairs                = new() {materials = new(21), meshPrefabrications = new(8)};
  public Assets.Lasoo                  lasoo                = new() {captureIndicatorMaterial = null, meshPrefabrication = null!, ropeMeshPrefabrication = null!};
  public Assets.Monsters               monsters             = new() {antilleryPrefabrication = null!, borkaPrefabrication = null!, molemPrefabrication = null!, sirpensPrefabrication = null!, tyragePrefabrication = null!};
  public bool                          outlineAutomatically = true;
  public UnityEngine.Material?         outlineMaterial      = null;
  public Assets.Primitives             primitives           = new() {capsule = null, cube = null, cylinder = null, plane = null, quad = null, sphere = null};
  public bool                          shadowAutomatically  = true;
  public UnityEngine.Material?         shadowMaterial       = null;
  public UnityEngine.Rendering.Volume? volume               = null;

  /* … */
  private void Awake() {
    if (Assets.main is not null && Assets.main != this) {
      UnityEngine.Object.DestroyImmediate(this, true);
      return;
    }

    Assets.main              = this;
    this.gameObject.isStatic = true;

    foreach (UnityEngine.Transform transform in this.transform)
      transform.SetParent(null, true);

    UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
  }

  private void OnDestroy() {
    if (Assets.main == this)
    Assets.main = null!;
  }

  private void Start() => this.gameObject.SetActive(false);
}
