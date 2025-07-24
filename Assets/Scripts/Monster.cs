using PatchOdyssey;

/* … */
[UnityEngine.RequireComponent(typeof(UnityEngine.BoxCollider))]
public sealed class Monster : Entity {
  public new UnityEngine.BoxCollider collider  => (UnityEngine.BoxCollider) base.collider;
  public     Player?                 tamer     =  null;
  public     bool                    wrestling =  false;

  /* … */
  private new void Awake() {
    this.movementDamping = 7.0f;
    base.Awake();

    // …
    this.collider.size = new(2.5f, 1.0f, 3.5f);
  }

  private void OnTriggerEnter(UnityEngine.Collider collider) {
    if (collider.GetComponent<Lasoo>() is Lasoo lasoo) {
      lasoo.capture  = this;
      this.wrestling = true;
    }
  }

  private void Update() {}
}
