using PatchOdyssey;

/* … */
[UnityEngine.RequireComponent(typeof(UnityEngine.SphereCollider))]
public class Tamer : Entity {
  public static readonly UnityEngine.Vector3 MountingPosition = (UnityEngine.Vector3.back * 0.200f) + (UnityEngine.Vector3.up * 1.125f);

  [ReadWriteInInspector] public  new      UnityEngine.SphereCollider                             collider   => (UnityEngine.SphereCollider) base.collider;
  [ReadOnlyInInspector]  private readonly System.Collections.Generic.List<UnityEngine.Transform> transforms =  new();

  /* … */
  protected virtual void Capture(Entity entity) {
    if (this.isDefeated)
    return;

    // …
    if (entity is Monster monster) {
      UnityEngine.Transform monsterTransform = monster.transform;

      // …
      monster.following        = this;
      this.transforms.Capacity = System.Math.Max(this.transform.hierarchyCount, this.transforms.Capacity);

      monster.transform.SetParent(this.transform, true);
      this.followers.Add(monster);
      this.transform.ForEach(transform => {
        if (monsterTransform == transform)
        return false;

        if (null != transform.GetComponent<UnityEngine.Renderer>() && !this.transforms.Contains(transform)) {
          transform.localPosition += Tamer.MountingPosition;
          this.transforms.Add(transform);

          return false;
        }

        return true;
      });
    }

    // NOTE (Lapys) ->> Other kinds of `Entity`s may entail other actions like switch activation, NPC interaction, e.t.c.
  }

  protected virtual void Release(Entity entity) {
    if (this.isDefeated)
    return;

    // …
    if (entity is Monster monster) {
      monster.following  = null;
      monster.isDefeated = true;

      this.followers.Remove(monster);

      if (0 == this.followers.Count) {
        foreach (UnityEngine.Transform transform in this.transforms) {
          if (null != transform)
          transform.localPosition -= Tamer.MountingPosition;
        }

        this.transforms.Clear();
      }
    }

    // NOTE (Lapys) ->> Other kinds of `Entity`s may entail other actions like switch de-activation, NPC interaction, e.t.c.
  }
}
