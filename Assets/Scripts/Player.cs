using PatchOdyssey;

/* … */
public sealed class Player : Tamer {
  public  new static readonly List<PatchEntity> Any                  = new(2u);
  public  new const           float             HealthMaximumDefault = 125.0f;
  private     const           float             PassiveHealAmount    = 5.0f;

  public  bool              isKeyboardAndPointerPlayer => 0 == Player.Any.IndexOf(this);
  private AnimationSequence passiveHealAnimation       =  new(1.0, AnimationSequence.Idle, AnimationSequence.Idle);

  /* … */
  protected override void Awake() {
    base.Awake();

    this.health = this.healthMaximum = Player.HealthMaximumDefault;
    this.team   = PatchEntity.Team.HostileMonsters;
  }

  private new void OnAI() /* … ⟶ Failed the “CAPTCHA” 🤖 */ {
    bool                  isKeyboardAndPointerPlayer = this.isKeyboardAndPointerPlayer;
    UnityEngine.Transform transform                  = this.transform;

    // …
    this.moveDirection = (
      isKeyboardAndPointerPlayer && (DeviceState.RELEASE > Util.Keys.Specials.DownArrow  || DeviceState.RELEASE > Util.Keys.Specials.S) ? -UnityEngine.Vector3.forward :
      isKeyboardAndPointerPlayer && (DeviceState.RELEASE > Util.Keys.Specials.UpArrow    || DeviceState.RELEASE > Util.Keys.Specials.W) ?  UnityEngine.Vector3.forward :
      isKeyboardAndPointerPlayer && (DeviceState.RELEASE > Util.Keys.Specials.LeftArrow  || DeviceState.RELEASE > Util.Keys.Specials.A) ? -UnityEngine.Vector3.right   :
      isKeyboardAndPointerPlayer && (DeviceState.RELEASE > Util.Keys.Specials.RightArrow || DeviceState.RELEASE > Util.Keys.Specials.D) ?  UnityEngine.Vector3.right   :
      UnityEngine.Vector3.zero
    );
  }

  public             void OnDefeated() { /* Do nothing… */ }
  protected override void OnDestroy () { base.OnDestroy(); Player.Any.Remove(this); }
  protected override void OnEnable  () { base.OnEnable (); Player.Any.Add   (this); }

  protected override void Update() {
    base.Update();

    // … ⟶ Passive healing
    if (this.passiveHealAnimation.isLooped)
    PatchEntity.Heal(this, Player.PassiveHealAmount);
  }
}
