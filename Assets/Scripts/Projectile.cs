using PatchOdyssey;

/* … */
public sealed class Projectile : PatchEntity {
  [ReadOnlyInInspector, System.NonSerialized] public PatchEntity? firer = null;
}
