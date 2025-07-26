using PatchOdyssey;

/* … */
[UnityEngine.RequireComponent(typeof(UnityEngine.BoxCollider))]
public sealed class Monster : Entity {
  public const double MovementInterval      = 1.5;
  public const double MovementIntervalRange = 0.5;

  [ReadWriteInInspector] public new UnityEngine.BoxCollider collider          => (UnityEngine.BoxCollider) base.collider;
  [ReadOnlyInInspector]  public     Player?                 tamer             =  null;
  [ReadWriteInInspector] public     Entity?                 target            =  null;
  [ReadWriteInInspector] public     float                   targetBerth       =  2.0f; // ->> Radius
  [ReadOnlyInInspector]  public     Timeframe               movementInterval  =  new(Monster.MovementInterval);
  [ReadWriteInInspector] public     Entity?                 wrestling         =  null;
  [ReadWriteInInspector] public     float                   wrestlingForce    =  30.0f;
  [ReadOnlyInInspector]  public     Timeframe               wrestlingInterval =  new(4.0);

  /* … */
  private new void Awake() {
    this.movementDamping = 7.0f;
    base.Awake();

    // …
    this.movementInterval.Reset();
    this.movementInterval.Wait ((Monster.MovementInterval + Monster.MovementIntervalRange) * Game.Randomizer.NextDouble());

    this.collider.size = new(2.5f, 1.0f, 3.5f);
  }

  private void OnTriggerEnter(UnityEngine.Collider collider) {
    if (collider.GetComponent<Lasoo>() is not Lasoo lasoo || lasoo.capture is not null || !lasoo.isDeploying())
    return;

    lasoo.capture  = this;
    this.wrestling = lasoo.user;
  }

  protected override void Update() {
    UnityEngine.Transform transform = this.transform;

    // …
    base.Update();

    // … ->> Moving/ Wrestling
    this.movementDirection = this.target is not null ? (this.target.transform.position - transform.position).normalized : UnityEngine.Vector3.zero;

    if (this.wrestling is not null) {
      this.movementDirection = -this.movementDirection;
      this.target            =  this.wrestling;

      if (this.wrestlingInterval.isLooped)
      this.rigidBody.AddForce((this.wrestling.transform.position - transform.position).normalized * (this.wrestlingForce * (float) Game.Randomizer.NextDouble()), UnityEngine.ForceMode.Impulse);

      if (this.wrestling is Player player && player.lasoo!.reachProgress >= 1.0f)
      player.ResetLasoo();
    } else this.wrestlingInterval.Reset();

    if (UnityEngine.Vector3.zero != this.movementDirection && this.movementInterval.isLooped) {
      this.rigidBody.AddForce(this.movementDirection * ((this.movementSpeed * (float) Game.Randomizer.NextDouble() * 0.3f) + (this.movementSpeed * 0.6f)), UnityEngine.ForceMode.Impulse);

      this.movementInterval.duration = Monster.MovementInterval + (Monster.MovementIntervalRange * Game.Randomizer.NextDouble() * (Game.Randomizer.NextDouble() < 0.5 ? +1.0 : -1.0));
      this.turnDirection             = this.movementDirection;
    }

    if (UnityEngine.Vector3.zero != this.turnDirection)
    this.rigidBody.MoveRotation(UnityEngine.Quaternion.Slerp(this.rigidBody.rotation, UnityEngine.Quaternion.LookRotation(this.turnDirection), UnityEngine.Time.unscaledDeltaTime * this.turnSpeed));
  }
}
