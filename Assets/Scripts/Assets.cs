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

  /* … */
  public static Assets main = null!;

  public Assets.Bullet         bullet               = new() {material  = null,    prefabrication      = null!};
  public Assets.Hairs          hairs                = new() {materials = new(21), meshPrefabrications = new(8)};
  public Assets.Lasoo          lasoo                = new() {captureIndicatorMaterial = null, meshPrefabrication = null!, ropeMeshPrefabrication = null!};
  public Assets.Monsters       monsters             = new() {antilleryPrefabrication = null!, borkaPrefabrication = null!, molemPrefabrication = null!, sirpensPrefabrication = null!, tyragePrefabrication = null!};
  public bool                  outlineAutomatically = true;
  public UnityEngine.Material? outlineMaterial      = null;

  /* … */
  private void Awake() {
    if (Assets.main is not null && Assets.main != this) {
      UnityEngine.Object.DestroyImmediate(this, true);
      return;
    }

    Assets.main              = this;
    this.gameObject.isStatic = true;

    UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
  }

  private void OnDestroy() {
    if (Assets.main == this)
    Assets.main = null!;
  }

  private void Start() => this.gameObject.SetActive(false);
}
