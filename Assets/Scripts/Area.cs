using PatchOdyssey;

/* … */
[UnityEngine.DefaultExecutionOrder(5)]
[UnityEngine.DisallowMultipleComponent]
public sealed class Area : UnityEngine.MonoBehaviour {
  [ReadWriteInInspector]                             public  string                                  areaName  =  string.Empty;
  [ReadWriteInInspector]                             private UnityEngine.Collider[]                  colliders => this.GetComponents<UnityEngine.Collider>();
  [ReadOnlyInInspector,  UnityEngine.SerializeField] private bool                                    isLocked  =  false;
  [ReadWriteInInspector, UnityEngine.SerializeField] private System.Collections.Generic.List<Entity> spawns    =  new();

  /* … */
  private void OnTriggerEnter(UnityEngine.Collider collider) {
    // UI.main.hud.areaText
  }

  private void OnTriggerExit(UnityEngine.Collider collider) {
    // … ->> Lock in
    if (collider.GetComponent<Player>() is Player player) {
      UnityEngine.Bounds? areaBounds = null;

      // …
      foreach (UnityEngine.Collider subcollider in this.colliders) {
        UnityEngine.Bounds colliderBounds = subcollider.bounds;

        // …
        if (areaBounds is UnityEngine.Bounds bounds) {
          bounds.Encapsulate(colliderBounds.center - colliderBounds.extents);
          bounds.Encapsulate(colliderBounds.center + colliderBounds.extents);

          areaBounds = bounds;
        } else areaBounds = colliderBounds;
      }

      if (areaBounds?.Contains(player.transform.position) ?? false)
      this.isLocked = 0 != this.spawns.Count;
    }
  }

  private void Update() {
    // … ->> Lock in
    foreach (UnityEngine.Collider collider in this.colliders)
    collider.isTrigger = !this.isLocked;

    // … ->> Spawning
    for (int index = this.spawns.Count; 0 != index--; ) {
      if (null == this.spawns[index])
      this.spawns.RemoveAt(index);
    }

    if (0 == this.spawns.Count)
    this.isLocked = false;
  }
}
