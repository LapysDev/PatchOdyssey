using PatchOdyssey;

/* … */
[PatchExecutionOrder(PatchBehaviour.DefaultExecutionOrder + 1)]
public sealed class Game : UnityEngine.MonoBehaviour {
  public enum State : byte { Gameplay, Menu }

  /* … */
  private static Game.State        CurrentState = Game.State.Menu;
  public  static readonly string[] GeneralHints = new[] {
    "Don’t forget to take breaks every now and again…",
    "Having fun is mandatory, the monsters demand it",
    "Hydration check? Stay hydrated and drink some water!",
    "The more monsters tamed makes your team more powerful",
    "WASD keys to move around"
  };
  public  static bool       Paused { get; private set; } = false;
  private static Game.State PreviousState                = Game.State.Menu;

  /* … */
  public static Game.State GetState() => Game.CurrentState;

  public static void LoadState(in Game.State state) {
    if (Game.CurrentState == state)
    return;

    // … ⟶ Transition the current Game state to the new Game state
    switch (Game.PreviousState = Game.CurrentState) {
      case Game.State.Gameplay: switch (state) {
        case Game.State.Menu: {
          UI.Main.Transition();
        } break;
      } break;

      case Game.State.Menu: switch (state) {
        case Game.State.Gameplay: {
          UI.Main.UnloadComponent("menu");
          UI.Main.Transition     ();
        } break;
      } break;
    }

    // … ⟶ Handle the new Game state regardless of transition
    switch (Game.CurrentState = state) {
      case Game.State.Gameplay: break;
      case Game.State.Menu    : break;
    }
  }
}
