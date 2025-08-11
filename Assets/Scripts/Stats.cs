using PatchOdyssey;

/* … */
public static class Stats {
  public enum Sequenced : byte { Campsite }

  /* … */
  [ReadOnlyInInspector] private static ulong           _Score     = 0uL;
  [ReadOnlyInInspector] public  static ulong           Score      { get => Stats._Score; set { Stats.ScorePrior = Stats.Score; Stats._Score = value; UI.main.scoreTimer.Reset(); } }
  [ReadOnlyInInspector] public  static ulong           ScorePrior { get; private set; } = 0uL;
  [ReadOnlyInInspector] public  static Stats.Sequenced Sequence                         = Stats.Sequenced.Campsite;

  /* … */
  public static void Reset() {
    Stats.Score      = 0uL;
    Stats.ScorePrior = 0uL;
    Stats.Sequence   = Stats.Sequenced.Campsite;
  }
}
