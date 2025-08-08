using PatchOdyssey;

/* … */
public sealed class Stats {
  public enum Sequenced : byte { Campsite }

  /* … */
  [ReadOnlyInInspector] public static ulong           Score    = 0uL;
  [ReadOnlyInInspector] public static Stats.Sequenced Sequence = Stats.Sequenced.Campsite;

  /* … */
  public static void Reset() {
    Stats.Score    = 0uL;
    Stats.Sequence = Stats.Sequenced.Campsite;
  }
}
