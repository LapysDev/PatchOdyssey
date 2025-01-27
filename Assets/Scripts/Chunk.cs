using PatchOdyssey;

/* … */
public class Chunk : UnityEngine.MonoBehaviour {
  public enum Type : byte {
    Free,
    Wall
  };

  /* … */
  public Chunk.Type mode = Chunk.Type.Free;

  /* … */
  private void Start () {}
  private void Update() {}
}
