using PatchOdyssey;

/* … */
public sealed class Monster : Entity {
  public Player? tamer = null;

  /* … */
  private void OnCollsionEnter(UnityEngine.Collision collision) {
    UnityEngine.Debug.Log($"Collided with “{collision.gameObject.name}”");
  }

  private void Update() {}
}
