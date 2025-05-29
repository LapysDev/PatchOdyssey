using PatchOdyssey;

/* … */
public class Monster : PatchEntity {
  public  new static readonly List<PatchEntity> Any                     = new(32u); // ⟶ Actually a modifiable `List`, but externally comes off immutable
  public  const               double            SpecialCooldownDefault  = 14.0;     // ⟶ in Seconds
  public  const               double            SummonedLifetimeDefault = 90.0;     // ⟶ in Seconds

  /* … */
  [ReadOnlyInInspector]                              protected          bool                                         allowAI { get; private set; }            =  false; // ⟶ Propagate `::OnAI()` event listener or otherwise
  [ReadOnlyInInspector]                              public             double                                       summonedLifetime { get; protected set; } =  Monster.SummonedLifetimeDefault;
  [ReadOnlyInInspector]                              public             Monster?                                     summoner         { get; protected set; } =  null;
  [ReadWriteInInspector, UnityEngine.SerializeField] protected          System.Collections.Generic.List<PatchEntity> summonPrefabs                            =  new(5); // ⟶ Prefab-only
  [ReadOnlyInInspector,  System.NonSerialized]       protected readonly System.Collections.Generic.List<PatchEntity> summons                                  =  new(10);
  [ReadOnlyInInspector]                              public             AnimationSequence                            specialCooldown                          =  new(Monster.SpecialCooldownDefault, AnimationSequence.Idle, AnimationSequence.Idle);
  [ReadOnlyInInspector]                              public             Tamer?                                       tamer                                    =  null;
  [ReadOnlyInInspector]                              public    new      PatchEntity.Team                             team                                     { get => null == this.summoner ? base.team : this.summoner.team; set => base.team = null == this.summoner ? value : this.summoner.team; }

  /* … */
  protected override void Awake() {
    base.Awake();

    this.firingCooldown.duration = 2.0;
    this.projectileLifetime      = 10.0f;
    this.health                  = this.healthMaximum = 75.0f;
    this.team                    = PatchEntity.Team.Violent;

    if (null != this.summoner) {
      this.transform.localScale *= Util.Perc(80.0f);
      Util.Wait.Until(this.summonedLifetime, static (object? _, in Events.WaitEvent data) => PatchEntity.Flee((PatchEntity) data.metadata!), this);
    }
  }

  private new void OnAI() {
    UnityEngine.Transform transform = this.transform;

    // …
    this.allowAI = false;

    if (null != this.tamer) {
      // … ⟶ `Tamer` controls this `Monster`
      if (null == this.summoner)
      return;

      // … ⟶ Boot out the `Tamer` 🥾
      this.tamer.Dismount();
      this.tamer = null;
    }

    this.allowAI = true;

    // … ⟶ Default behaviour
    if (this.specialCooldown.isFinished) {
      if (this.Special())
      this.specialCooldown.Reset();
    }

    if (Util.Eval(hostiles => { hostiles.Sort((entityA, entityB) => Util.Vector.DistanceSquared(entityA.transform.position, transform.position) > Util.Vector.DistanceSquared(entityB.transform.position, transform.position) ? +1 : -1); return !hostiles.IsEmpty() ? hostiles[0] : null; }, PatchEntity.GetHostiles(this)) is PatchEntity hostile)
    transform.LookAt(hostile.transform.position);
  }

  public virtual bool Special() => false; // Do nothing…

  public void Summon(in Monster monster) {}

  protected override void OnDestroy() { base.OnDestroy(); ((RefList<PatchEntity>) Monster.Any).Remove(this); }
  protected override void OnEnable () { base.OnEnable (); ((RefList<PatchEntity>) Monster.Any).Add   (this); }
}
