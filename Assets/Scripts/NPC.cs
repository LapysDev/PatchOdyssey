using PatchOdyssey;

/* … */
public class NPC : UnityEngine.MonoBehaviour {
  public enum Mode : byte {
    Idle,
    Combat,
    Independent,
    Follow
  };

  /* … */
  public NPC.Mode mode = NPC.Mode.Idle;

  /* … */
  private void Start () {}
  private void Update() {}
}
