using PatchOdyssey;

/* … */
[UnityEngine.RequireComponent(typeof(UnityEngine.CapsuleCollider))]
public class Player : NPC {
  new private void Start() {
    base.Start();
  }

  new private void Update() {
    UnityEngine.CapsuleCollider playerCollider = this.gameObject.GetComponent<UnityEngine.CapsuleCollider>();

    // …
    base.Update();

    playerCollider.enabled       = true;
    playerCollider.excludeLayers = 0x0; // → `~UnityEngine.Physics.AllLayers`
    playerCollider.includeLayers = 0x0; // → `~UnityEngine.Physics.AllLayers`
    playerCollider.isTrigger     = false;
  }
}
