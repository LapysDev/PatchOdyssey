using PatchOdyssey;

/* … */
public sealed class Sirpens : Monster {
  protected override void Awake() {
    base.Awake();
    PatchEntity.Immunize(this);
  }

  private new void OnAI() {
    foreach (PatchEntity projectile in this.projectiles) {
      // Make them poisonous on contact?
    }

    // …
    if (!this.allowAI)
    return;
  }

  public override bool Special() => true;
}
