[UnityEngine.DisallowMultipleComponent]
public sealed class Assets : UnityEngine.MonoBehaviour {
  public static Assets main = null!;

  [UnityEngine.Header("Bullet (“Friendliness Pellet” 🌻)")]
  public UnityEngine.GameObject monsterBulletMeshPrefabrication = null!;
  public UnityEngine.GameObject playerBulletMeshPrefabrication  = null!;
  public UnityEngine.GameObject tamerBulletMeshPrefabrication   = null!;

  [UnityEngine.Header("Lasoo")]
  public UnityEngine.Material   lasooCaptureIndicatorMaterial = null!;
  public UnityEngine.GameObject lasooMeshPrefabrication       = null!;
  public UnityEngine.GameObject lasooRopeMeshPrefabrication   = null!;

  [UnityEngine.Header("…")]
  public UnityEngine.Material outline              = null!;
  public bool                 outlineAutomatically = true;

  /* … */
  private void Awake() {
    if (Assets.main is not null && Assets.main != this) {
      UnityEngine.Object.DestroyImmediate(this, true);
      return;
    }

    Assets.main = this;
    UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
  }

  private void OnDestroy() {
    if (Assets.main == this)
    Assets.main = null!;
  }

  private void Start() => this.gameObject.SetActive(false);
}
