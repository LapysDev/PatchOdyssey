using PatchOdyssey;

/* … */
#nullable enable annotations

/* … */
public class Game : UnityEngine.MonoBehaviour {
  [ReadOnlyInInspector]  public        bool    isPlaying = false;
  [ReadOnlyInInspector]  public static Game?   main      = null;
  [ReadWriteInInspector] public        Player? player    = null;

  /* … */
  private void Awake() {
    Game.main ??= this;
  }

  public void Exit() {
    UnityEngine.Application.Quit();
    #if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
      UnityEditor.EditorApplication.ExitPlaymode();
    #endif
  }

  public void LoadChunk(UnityEngine.GameObject chunk) {
    /* [1]: Load prefab `UnityEngine.GameObject` called `chunk` */
    /* [2]: Clone/ instantiate prefab as existing `UnityEngine.GameObject` within the current `UnityEngine.SceneManagement.Scene` scenes */
    /* [3]: Find "ground" `UnityEngine.GameObject` child component */
    /* [4]: Variegate the elevation of a random selection of its faces to create the illusion of procedurally-generated rough terrain */
  }

  public void Pause() {
    if (this.isPlaying) {
      UI.main?.LoadBackground("menu-background-2.png");
      UI.main?.LoadComponent ("pause");

      UnityEngine.Debug.Log("[Game::Pause()]");
    }

    this.isPlaying = false;
  }

  public void Play() {
    if (!this.isPlaying) {
      UI.main?.UnloadAllComponents();
      UI.main?.UnloadBackground   ();

      UnityEngine.Debug.Log("[Game::Play()]");
      /*  [0]: Blog update: Asset Loading, Code Style, File Structure, GitHub, Settings Serialization */
      /*  [1]: `Game::LoadChunk(…)` the prototype testing chunk */
      /*  [2]: Relocate existing (set with the Inspector) `Player` object at the chunk's spawnpoint  */
      /*  [3]: Setup gameplay view (hint: consider `UnityEngine.Camera.main`) */
      /*  [4]: Introduce movement */
      /*  [5]: Introduce interaction */
      /*  [6]: Introduce wild monster */
      /*  [7]: Tame wild monster — other re-introduce another wild monster */
      /*  [8]: Use pet monster */
      /*  [9]: Battle enemy explorer (one with and one without pet monster) */
      /* [10]: Prototype ended */
    }

    this.isPlaying = true;
  }

  public void ToMenu() {
    this.Pause();

    UI.main?.UnloadAllComponents();
    UI.main?.LoadComponent      ("menu");
    UI.main?.LoadBackground     ("menu-background-1.png");
  }

  private void Update() {
    Util.StopWaiting();
  }
}
