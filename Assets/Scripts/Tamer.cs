using PatchOdyssey;

/* … */
public class Tamer : PatchEntity {
  public new static readonly List<PatchEntity> Any = new(8u);

  /* … */
  [ReadWriteInInspector]                             public        string              givenName { get; private set; } =  string.Empty;
  [ReadWriteInInspector]                             private       bool                isLatching                      => null != this.latchedMonster;
  [ReadWriteInInspector]                             public        bool                isNamed  { get; private set; }  =  false; // ⟶ Unnamed 😢
  [ReadWriteInInspector]                             public        bool                isTaming { get; private set; }  =  false;
  [ReadOnlyInInspector, UnityEngine.SerializeField]  public        double              latchCompletion                 =  Util.Perc(0.0);
  [ReadOnlyInInspector, UnityEngine.SerializeField]  public        UnityEngine.Vector3 latchEulerAnglesOrigin          =  UnityEngine.Vector3.zero;
  [ReadOnlyInInspector, UnityEngine.SerializeField]  private       bool                latched                         =  false; // ⟶ Editor-only
  [ReadOnlyInInspector]                              public        Monster?            latchedMonster                  =  null;
  [ReadOnlyInInspector]                              public        Monster?            monster                         =  null;
  [ReadOnlyInInspector,  UnityEngine.SerializeField] private   new string              name                            =  string.Empty;             // ⟶ Editor-only
  [ReadOnlyInInspector,  UnityEngine.SerializeField] private       bool                named                           =  false;                    // ⟶ Editor-only
  [ReadOnlyInInspector,  UnityEngine.SerializeField] private       bool                taming                          =  false;                    // ⟶ Editor-only
  [ReadWriteInInspector, UnityEngine.SerializeField] protected     Lasoo               tamingLasoo                     =  null!;                    // ⟶ Must be a prefabrication set via Unity Editor’s Inspector
  [ReadOnlyInInspector]                              public        UnityEngine.Vector3 tamingLasooPosition             =  UnityEngine.Vector3.zero; // ⟶ Yes. It’s spelled “lasso”, instead

  /* … */
  protected override void Awake() {
    base.Awake();

    this.team        = PatchEntity.Team.HostileMonsters;
    this.tamingLasoo = null != this.tamingLasoo ? Util.Prefab<Lasoo>(this.tamingLasoo, this) : null!;
  }

  public void Dismount() {
    if (null != this.monster) {
      PatchEntity.Flee(this.monster);
      this.monster = null;
    }
  }

  protected override void OnDestroy() { base.OnDestroy(); ((RefList<PatchEntity>) Tamer.Any).Remove(this); }
  protected override void OnEnable () { base.OnEnable (); ((RefList<PatchEntity>) Tamer.Any).Add   (this); }

  internal void Mount(Monster monster) {
    this.Untame();
    this.monster = monster;
  }

  private new void OnAI() {
    this.latched = this.isLatching;
    this.name    = this.givenName;
    this.named   = this.isNamed;
    this.taming  = this.isTaming;

    // …
    if (this is not Player) {
      /* AI */
    }

    // …
    if (null == this.monster) {
      if (this.isTaming) {
        // …
        if (this.isLatching) {
          Monster latchedMonster = this.latchedMonster!;

          // …
          if      (null != latchedMonster.summoner)          this.Untame();
          else if (this.latchCompletion == Util.Perc(100.0)) this.Mount (latchedMonster);
        }

        // this.tamingLasoo.SetActive(true);
      }

      else {
        // this.tamingLasoo.SetActive(false);
      }
    }

    else {}
  }

  public void Rename(string name) {
    if (!Game.VerifyName(name))
    return;

    base.name     += (string.IsNullOrEmpty(base.name) ? string.Empty : " ") + $"“{name}”";
    this.givenName = name;
    this.isNamed   = true;
  }

  public void Tame() => this.isTaming = true;

  public void Untame() {
    this.latched        = this.isTaming = false;
    this.latchedMonster = null;
  }
}
