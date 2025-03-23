using PatchOdyssey;

/* … */
[UnityEngine.RequireComponent(typeof(UnityEngine.CapsuleCollider))]
public class Player : NPC {
  [PatchOdyssey.ReadWriteInInspector] public UnityEngine.Light?  spotlight       = null;                     // TODO (Lapys)
  [PatchOdyssey.ReadOnlyInInspector]  public UnityEngine.Vector3 spotlightOrigin = UnityEngine.Vector3.zero; // TODO (Lapys)

  /* … */
  new private void Start() {
    base.Start();

    if (null != this.spotlight)
    this.spotlightOrigin = this.spotlight.transform.position;
  }

  new private void Update() {
    UnityEngine.CapsuleCollider playerCollider = this.gameObject.GetComponent<UnityEngine.CapsuleCollider>();

    // …
    base.Update();

    playerCollider.enabled       = true;
    playerCollider.excludeLayers = 0x0; // → `~UnityEngine.Physics.AllLayers`
    playerCollider.includeLayers = 0x0; // → `~UnityEngine.Physics.AllLayers`
    playerCollider.isTrigger     = false;

    if (null != this.spotlight)
    this.spotlight.type = UnityEngine.LightType.Directional;
  }
}
