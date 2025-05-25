using PatchOdyssey;

/* … */
[PatchExecutionOrder(PatchBehaviour.DefaultExecutionOrder + 1)]
public sealed class Game : UnityEngine.MonoBehaviour {
  public enum State : byte {
    Credits,
    Menu,
    Settings,
    Tutorial
  }

  /* … */
  public static Game.State        CurrentState = Game.State.Menu;
  public static readonly string[] GeneralHints = new[] {
    "Don’t forget to take breaks every now and again…",
    "Having fun is mandatory, the monsters demand it",
    "Hydration check? Stay hydrated and drink some water!",
    "The more monsters tamed makes your team more powerful",
    "WASD keys to move around"
  };
  public static bool       Paused        { get; private set; } = false;
  public static Game.State PreviousState { get; private set; } = Game.State.Menu;

  /* … */
  public static void LoadState(in Game.State state) {
    if (Game.CurrentState == state)
    return;

    // … ⟶ Transition the current Game state to the new Game state
    switch (Game.PreviousState = Game.CurrentState) {
      case Game.State.Credits: switch (state) {
        case Game.State.Menu: UI.Main.LoadComponent("menu"); break;
      } break;

      case Game.State.Menu: switch (state) {
        case Game.State.Credits : UI.Main.LoadComponent("credits");  break;
        case Game.State.Settings: UI.Main.LoadComponent("settings"); break;
        case Game.State.Tutorial: break;
      } break;

      case Game.State.Settings: switch (state) {
        case Game.State.Menu: UI.Main.LoadComponent("menu"); break;
      } break;

      case Game.State.Tutorial: break;
    }

    // … ⟶ Handle the new Game state regardless of transition
    switch (Game.CurrentState = state) {
      case Game.State.Credits : break;
      case Game.State.Menu    : break;
      case Game.State.Settings: break;
      case Game.State.Tutorial: break;
    }
  }
//   [PatchOdyssey.ReadOnlyInInspector]  public                bool                 isLoaded          = false;
//   [PatchOdyssey.ReadOnlyInInspector]  public                bool                 isPlaying         = false;
//   [PatchOdyssey.ReadOnlyInInspector]  public static         Game?                main              = null;
//   [PatchOdyssey.ReadWriteInInspector] public                Player?              player            = null;
//   [PatchOdyssey.ReadWriteInInspector] public                GameObjectDictionary prototypeData     = new();                                                                           // TODO (Lapys)
//   [PatchOdyssey.ReadOnlyInInspector]  public /* readonly */ BooleanDictionary    prototypeMetadata = new() {{"clearing:begin", false}, {"clearing:end", false}, {"mounting", false}}; // TODO (Lapys)

//   private Chunk LoadChunk(Chunk chunk) {
//     /* TODO (Lapys) */
//     /* [0]: "Play" button should say "Explore" instead */
//     /* [1]: Load prefab `UnityEngine.GameObject` called `chunk` */
//     /* [2]: Clone/ instantiate prefab as existing `UnityEngine.GameObject` within the current `UnityEngine.SceneManagement.Scene` scenes */
//     /* [3]: Find "ground" `UnityEngine.GameObject` child component */
//     /* [4]: Variegate the elevation of a random selection of its faces to create the illusion of procedurally-generated rough terrain */
//     return UnityEngine.Object.Instantiate(chunk);
//   }

//   public void Pause() {
//     if (this.isPlaying) {
//       UI.main?.LoadBackground("menu-background-2.png");
//       UI.main?.LoadComponent ("pause");

//       UnityEngine.Debug.Log("[Game::Pause()]");
//     }

//     this.isPlaying = false;
//   }

//   public void Play() {
//     if (!this.isPlaying) {
//       UI.main?.UnloadAllComponents();
//       UI.main?.UnloadBackground   ();

//       UnityEngine.Debug.Log("[Game::Play()]");
//       this.LoadComponent();

//       /* TODO (Lapys) */
//       /*  [0]: Blog update: Asset Loading, Code Style, File Structure, GitHub, Settings Serialization */
//       /*  [1]: `Game::LoadChunk(…)` the prototype testing chunk */
//       /*  [2]: Relocate existing (set with the Inspector) `Player` object at the chunk's spawnpoint  */
//       /*  [3]: Setup gameplay view (hint: consider `UnityEngine.Camera.main`) */
//       /*  [4]: Introduce movement */
//       /*  [5]: Introduce interaction */
//       /*  [6]: Introduce wild monster */
//       /*  [7]: Tame wild monster — other re-introduce another wild monster */
//       /*  [7B]: Bait Items, Environment Re-scaping, Lasoo, Pheromones, Tranquilizers, Trapping (Box or Net) */
//       /*  [7C]: Nickname, Gameplay Personality */
//       /*  [8]: Use pet monster */
//       /*  [9]: Battle enemy explorer (one with and one without pet monster) */
//       /* [10]: Prototype ended */
//     }

//     this.isPlaying = true;
//   }

//   public void ToMenu() {
//     this.Pause ();
//     this.Unload();

//     UI.main?.UnloadAllComponents();
//     UI.main?.LoadComponent      ("menu");
//     UI.main?.LoadBackground     ("menu-background-1.png");
//   }

//   private void Unload() {
//     if (this.isLoaded) {
//       UnityEngine.Debug.Log("[Game::Unload()]");
//     }

//     this.isLoaded = false;
//   }

//   private void Update() {
//     if (this.isLoaded && null != this.player) {
//       const float        boundsErrorMargin = 0.1f;
//       UnityEngine.Bounds playerBounds      = this.player.GetComponent<UnityEngine.Collider>().bounds;
//       const float        playerMountRange  = 1.0f;

//       // TODO (Lapys)
//       playerBounds.Expand(boundsErrorMargin);

//       if (System.Array.Exists(Util.ArrayFrom(this.prototypeData?["clearing"]?.FindHierarchyByComponent<UnityEngine.Collider>()!), collider => collider.bounds.Intersects(playerBounds)))
//         this.prototypeMetadata["clearing:begin"] = true;

//       else if (this.prototypeMetadata["clearing:begin"]) {
//         this.prototypeMetadata["clearing:end"] = true;
//         this.prototypeMetadata["mounting"]     = (
//           System.Array.Exists(Util.ArrayFrom(this.prototypeData?["mounting#0"]?.FindHierarchyByComponent<UnityEngine.Collider>()!), collider => collider.bounds.Intersects(playerBounds)) ||
//           System.Array.Exists(Util.ArrayFrom(this.prototypeData?["mounting#1"]?.FindHierarchyByComponent<UnityEngine.Collider>()!), collider => collider.bounds.Intersects(playerBounds))
//         );
//       }

//       // TODO (Lapys)
//       playerBounds.Expand(playerMountRange);

//       foreach (NPC npc in UnityEngine.Object.FindObjectsByType<NPC>(UnityEngine.FindObjectsSortMode.None))
//       if (npc != this.player) {
//         npc.transform.LookAt(this.player.transform, UnityEngine.Vector3.up);

//         // TODO (Lapys) → Move to `NPC` script and update `UI` script (including `InputActions`)
//         if (npc is Mount) {
//           UnityEngine.Bounds npcBounds = npc.GetComponent<UnityEngine.Collider>().bounds;

//           // …
//           npcBounds.Expand(boundsErrorMargin + playerMountRange);

//           if (npcBounds.Intersects(playerBounds)) {
//             if (UnityEngine.Input.GetKeyUp(UnityEngine.KeyCode.Space)) {
//               if (!player.mountIsChanged)
//                 player.Dismount();

//               player.mountIsChanged = false;
//             }

//             else if (UI.main?.keyboards["active"].Exists(_ => UnityEngine.KeyCode.Space == _.key) ?? false) {
//               if (null == player.mount)
//               player.Mount((npc as Mount)!);
//             }
//           }
//         }
//       }

//       if      (false) {}
//       else if (this.prototypeMetadata["clearing:begin"] && !this.prototypeMetadata["clearing:end"]) UI.main?.ShowText("Arrows/ WASD to move");
//       else if (this.prototypeMetadata["mounting"])                                                  UI.main?.ShowText("Space to dismount/ ride");
//       else                                                                                          UI.main?.HideText();
//     }

//     Util.Unwait();
//   }
}
