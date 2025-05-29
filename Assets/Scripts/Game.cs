using PatchOdyssey;

/* … */
public static class Game {
  public enum State : byte { Gameplay, Menu }

  /* … */
  public  static bool              ArenaOccupied { get; private set; } = false; // ⟶ Keeps `Player`s from leaving before the brawl is over
  public  static uint              ArenaScore    { get; private set; } = 0u;
  public  static bool              ArenaVisited  { get; private set; } = false;
  private static Game.State        CurrentState                        = Game.State.Menu;
  public  static readonly string[] GeneralHints                        = new[] {
    "Don’t forget to take breaks every now and again…",
    "Having fun is mandatory, the monsters demand it",
    "Hydration check? Stay hydrated and drink some water!",
    "The more monsters tamed makes your team more powerful",
    "WASD keys to move around"
  };
  public static          Tamer      Guide      = null!;
  public static readonly string[][] GuideChats = new[] {
    new[] {"WASD keys to move around…", "but you’ve figured that out already haven’t you?"}
  };
  public   static BooleanDictionary HasAskedName                    = new(1u);
  public   static uint              HighScore { get; private set; } = 0u;
  internal static GameBehaviour     Object                          = null!;
  public   static bool              Paused { get; private set; }    = true;
  private  static Game.State        PreviousState                   = Game.State.Menu;
  public   static uint              Score { get; private set; }     = 0u;

  /* … */
  public static void       Freeze  (bool freeze = true) => Game.Object?.OnFreeze(freeze);
  public static Game.State GetState()                   => Game.CurrentState;

  [PatchMethod(NoInlining)]
  public static void Load() {
    Game.ArenaVisited = Settings.GetProperty<bool>("Statistics/ArenaVisited") ?? Game.ArenaVisited;
    Game.HighScore    = Settings.GetProperty<uint>("Statistics/HighScore")    ?? Game.HighScore;

    foreach (string name in (Settings.GetProperty<string>("Statistics/HasAskedName") ?? string.Empty).Split(',', System.StringSplitOptions.None))
    Game.HasAskedName[name] = true;
  }

  public static bool LoadState(in Game.State state) {
    if (Game.CurrentState != state) {
      Game.State previousState = Game.PreviousState;

      // …
      Game.PreviousState = Game.CurrentState;
      Game.CurrentState  = state;

      // … ⟶ Transition the current Game state to the new Game state
      switch (previousState) {
        case Game.State.Gameplay: switch (state) { case Game.State.Menu    : break; } break;
        case Game.State.Menu    : switch (state) { case Game.State.Gameplay: break; } break;
      }

      // … ⟶ Handle the new Game state regardless of transition
      switch (state) {
        case Game.State.Gameplay: break;
        case Game.State.Menu    : break;
      }

      // …
      return true;
    }

    return false;
  }

  [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.AfterSceneLoad)]
  private static void Main() => new UnityEngine.GameObject("… 🎮", typeof(GameBehaviour)); // ⟶ Similar pattern as with `PatchBehaviour` and `PatchOdyssey.Util.Game`

  [PatchMethod(NoInlining)] public static bool Pause     () { if (!Game.Paused)                                        { Game.Paused = true;  Game.Object?.OnPause(true); return true; } return false; }
  [PatchMethod(NoInlining)] public static bool Play      () { if  (Game.Paused && Game.LoadState(Game.State.Gameplay)) { Game.Paused = false; Game.Object?.OnPlay ();     return true; } return false; }
  [PatchMethod(NoInlining)] public static void Quit      () => Util.Game.Quit();
  [PatchMethod(NoInlining)] public static bool QuitToMenu() { if (Game.LoadState(Game.State.Menu))                       { Game.Pause(); Game.Save(); Game.Object?.OnQuitToMenu();      return true; } return false; }
  [PatchMethod(NoInlining)] public static bool Unpause   () { if (Game.Paused && Game.State.Gameplay == Game.GetState()) { Game.Paused = false;       Game.Object?.OnPause     (false); return true; } return false; }

  [PatchMethod(NoInlining)]
  public static void Save() {
    Settings.SetProperty<bool>  ("Statistics/ArenaVisited", Game.ArenaVisited);
    Settings.SetProperty<string>("Statistics/HasAskedName", string.Join(',', Util.Array<string>.From(Game.HasAskedName.Keys).FindAll(static name => Game.HasAskedName[name])));
    // Settings.SetProperty<uint>  ("Statistics/HighScore",    System.Math.Max(Game.HighScore, Game.Score)); /* 📅 📅 📅 */
  }

  public static void Unfreeze() => Game.Freeze(false);

  public static bool VerifyDuration(double duration) => !double.IsInfinity(duration) && !double.IsNaN(duration);
  public static bool VerifyName    (string name) /* ⟶ Censorship? What’s that? 😭 */ {
    // … ⟶ Checks that the given `name` is comprised only of ASCII characters and is not an empty text value
    return !string.IsNullOrEmpty(name) && System.Text.RegularExpressions.Regex.IsMatch(name, @"^[\x20-\x7E]*$");
  }
}

[PatchExecutionOrder(PatchBehaviour.DefaultExecutionOrder + 2)]
public sealed class GameBehaviour : UnityEngine.MonoBehaviour {
  private void Awake() {
    Game.Freeze();

    Util.Game.OnEnd   += static (timestamp) => { Game.Pause(); Game.Save(); Game.Quit(); };
    Util.Game.OnStart += static (timestamp) => Game.Load();
    Util.UI  .OnBlur  += static (timestamp) => { if (Game.State.Gameplay == Game.GetState()) Game.Pause(); };
    Util.UI  .OnFocus += static (timestamp) => { if (Game.State.Gameplay == Game.GetState()) Game.Play (); };
  }

  private void OnEnable() => Game.Object = this;

  internal void OnFreeze(bool frozen) {
    if (frozen) {
      PatchEntity.Any.ForEach(PatchEntity.Halt);
    }

    else {
      PatchEntity.Any.ForEach(PatchEntity.Go);
    }
  }

  internal void OnQuitToMenu() {
    UI.Main.LoadBackgroundComponent();
    UI.Main.LoadComponent          ("menu");
    UI.Main.Transition             ();
  }

  internal void OnPause(bool paused) {
    if (paused) {
      Game.Freeze();
      UI.Main.LoadComponent("pause");
    }

    else {
      UI.Main.UnloadBackgroundComponent();
      UI.Main.LoadComponent            (string.Empty);

      Game.Unfreeze();
    }
  }

  internal void OnPlay() {
    UI.Main.Transition();
    this.OnPause(false);

    /* 📅 📅 📅 */
    UI.Load.UriAsMusic("fight.mp3", UI.Load.PlayMusic);
    UI.Load.UriAsSound("game-start.mp3", UI.Load.PlaySound);
    UI.Main.PROJECT_DEADLINE_CAME_UP_ANIM.Reset();
    UI.Main.PROJECT_DEADLINE_CAME_UP.SetAlpha(0.0f);
  }

  private void Start() => Game.Load();
  UnityEngine.Camera komomura = null!;
  UnityEngine.Vector3 hiiii = UnityEngine.Vector3.zero;
  private float scalene = 0.0f;
  private List<(Monster, uint)> soundsForThem = new();
  private void Update() {
    if (Game.Paused || Game.State.Gameplay != Game.GetState())
    return;

    // …
    // UI.Load.UriAsSound("waves.mp3", immediate: false, UI.Load.PlaySound);

    /* 📅 📅 📅 */
    if (!UI.Main.PROJECT_DEADLINE_CAME_UP_ANIM.isFinished) {
      komomura ??= UnityEngine.Camera.main;
      hiiii = komomura.transform.localPosition;
      UI.Main.PROJECT_DEADLINE_CAME_UP.SetAlpha((float) UI.Main.PROJECT_DEADLINE_CAME_UP_ANIM.easedProgress);
      UI.Main.PROJECT_DEADLINE_CAME_UP.transform.localScale *= Util.Perc(100.1f);
    }

    else {
      Player player = (Player) Player.Any[0];

      UI.Main.PROJECT_DEADLINE_CAME_UP.SetAlpha(0);

      if (player.moveDirection == UnityEngine.Vector3.zero)
        scalene = System.Math.Max(scalene - 0.01f, 0.0f);
      else scalene = System.Math.Min(scalene + 0.005f, 1.0f);

      if (null != komomura)
      komomura.transform.localPosition = Util.Vector.ExcludeY(komomura.transform.localPosition, value: Util.Lerp((double) scalene, hiiii.y, hiiii.y * 2.0f));

      if (0u == UI.Main.PROJECT_DEADLINE_CAME_UP_COUNT) {
        UI.Main.PROJECT_DEADLINE_CAME_UP_COUNT = UI.Main.PROJECT_DEADLINE_CAME_UP_MIN + (uint) (Util.Random() * 2.0f);
        UI.Main.PROJECT_DEADLINE_CAME_UP_MIN += Util.Random() < 0.3 ? 1u : 0u;
        UI.Main.PROJECT_DEADLINE_CAME_UP_PRESCORE = Settings.GetProperty<uint>("Statistics/HighScore") ?? 0u;
        UI.Main.PROJECT_DEADLINE_CAME_UP_SCO1.text = ((uint) UI.Main.PROJECT_DEADLINE_CAME_UP_SCORE).ToString("D10");
        UI.Main.PROJECT_DEADLINE_CAME_UP_SCO2.text = ((uint) UI.Main.PROJECT_DEADLINE_CAME_UP_PRESCORE).ToString("D10");
        player.SetMaterialColor(new((float) Util.Random() * 0.3f, (float) Util.Random() * 0.6f, (float) Util.Random() * 1.0f));
        player.firingCooldown.duration = 0.0;
        player.projectileLifetime = 5.0;
        player.projectileDamage = 50.0f;

        UI.Load.UriAsSound("summon.mp3", UI.Load.PlaySound);
        for (uint count = UI.Main.PROJECT_DEADLINE_CAME_UP_COUNT; 0u != count--; )
        if (Util.Prefab<Monster>(UI.Main.PROJECT_DEADLINE_CAME_UP_MON, player.transform.position + Util.Vector.ExcludeY(Util.Vector.Random<UnityEngine.Vector3>() * 15.0f), UnityEngine.Quaternion.identity) is Monster monster) {
          bool peaceful = Util.Random() < 0.05;
          bool friendly = Util.Random() < 0.1;

          monster.moveSpeed = (float) (Util.Random() * 3.0f) + 3.0f;

          if      (friendly) { monster.team = PatchEntity.Team.Violent; monster.SetMaterialColor(UnityEngine.Color.cyan); monster.moveSpeed *= 1.5f; }
          else if (peaceful) { monster.team = PatchEntity.Team.Peaceful; monster.SetMaterialColor(UnityEngine.Color.green);  }
          else               { monster.team = PatchEntity.Team.HostilePlayers; monster.SetMaterialColor(new(((float) Util.Random() * 0.5f) + 0.2f, ((float) Util.Random() * 0.5f) + 0.2f, ((float) Util.Random() * 0.5f) + 0.2f)); }

          monster.fleeAnimationIntensity = 0.125f * (float) (Util.Random() * 0.5f);
          monster.firingCooldown.duration = System.Math.Max(0.35, ((Util.Random() * 1.5) + 0.5) - (UI.Main.PROJECT_DEADLINE_CAME_UP_MIN / 10.0));
          monster.projectileDamage = 5.0f + UI.Main.PROJECT_DEADLINE_CAME_UP_MIN;
        }
      }

      else {
        UnityEngine.Bounds bounds = player.GetColliderBounds();

        bounds.Expand(0.75f);
        player.moveSpeed = 10.0f;
        player.projectileDamage = UnityEngine.Mathf.Min(1.0f, player.projectileDamage - 0.01f);

        if (Util.Keys.HasPressed(UnityEngine.KeyCode.Return) || Util.Keys.HasPressed(UnityEngine.KeyCode.Space) || Util.Eval(_ => {
          foreach (PointerInfo pointer in Util.Pointers.Any) {
            if (DeviceState.BEGIN == pointer.state && !Util.Pointers.IsId(pointer.state))
            return true;
          }

          return false;
        }, false)) {
          player.moveSpeed = 4.0f;
          if (PatchEntity.Fire(player))
          player.projectiles[(int) (player.projectiles.Count - 1)].moveDirection = player.moveDirection == UnityEngine.Vector3.back ? UnityEngine.Vector3.forward : player.moveDirection;
          UI.Load.UriAsSound(Util.RandomBoolean() ? "shoot-1.mp3" : "shoot-2.mp3", UI.Load.PlaySound);
        }

        foreach (Projectile projectile in player.projectiles)
        foreach (Monster monster in Monster.Any) {
          UnityEngine.Bounds monsterBounds = monster.GetColliderBounds();
          monsterBounds.Expand(3.0f);
          if (!monster.isDefeated && monsterBounds.Intersects(projectile.GetColliderBounds())) {
            PatchEntity.Damage(monster, player.projectileDamage);
            if (!soundsForThem.Exists(a => a.Item1 == monster)) {
              UI.Load.UriAsSound("ded.mp3", sound => { if (!sound.isPlaying) sound.Play(); });
              soundsForThem.Add((monster, 60u));
            }
          }
        }

        for (uint i = soundsForThem.Count; 0u != i--; )
        if (0u == soundsForThem[i].Item2--) {
          soundsForThem.RemoveAt(i);
          break;
        }

        UI.Main.components[string.Empty].SetAlpha(Util.Perc(3.0f));
        UI.Main.PROJECT_DEADLINE_CAME_UP_LIFE.SetMaterialColor(player.health/ player.healthMaximum < Util.Perc(50.0f) ? UnityEngine.Color.yellow : UnityEngine.Color.green);

        foreach (Monster monster in Monster.Any) {
          if (monster.team == PatchEntity.Team.Violent && !monster.projectiles.IsEmpty()) {
            monster.projectiles[monster.projectiles.Count - 1].SetMaterialColor(UnityEngine.Color.white);
          } else if (PatchEntity.Team.Peaceful == monster.team) monster.transform.LookAt(player.transform);

          monster.moveDirection = 7.5f > UnityEngine.Vector3.Distance(monster.transform.position, player.transform.position) ? monster.transform.forward : UnityEngine.Vector3.zero;
          monster.projectileDamage += 0.01f;

          foreach (PatchEntity bruh in PatchEntity.GetHostiles(monster)) {
            UnityEngine.Bounds bruhz = bruh.GetColliderBounds();
            bruhz.Expand(0.75f);
            foreach (Projectile projectile in monster.projectiles)
            if (bruhz.Intersects(projectile.GetColliderBounds())) {
              UI.Main.components[string.Empty].SetAlpha(Util.Perc(10.0f));
              PatchEntity.Damage(bruh, monster.projectileDamage);

              if (bruh is Player) {
                UI.Load.UriAsSound("ouch.mp3", UI.Load.PlaySound);
                UI.Main.PROJECT_DEADLINE_CAME_UP_LIFE.SetMaterialColor(UnityEngine.Color.red);
                projectile.Die();
              }
            }
          }

          if (monster.isDefeated) {
            if (!monster.isFleeing) {
              soundsForThem.RemoveAll(a => a.Item1 == monster);
              UI.Load.UriAsSound("damage.mp3", UI.Load.PlaySound);
              UI.Main.PROJECT_DEADLINE_CAME_UP_COUNT--;
            }
            PatchEntity.Flee(monster);
            UI.Main.PROJECT_DEADLINE_CAME_UP_SCORE += 10.0;
          }
        }

        Settings.LoadTimeoutMaximum = 0.0;
        UI.Main.PROJECT_DEADLINE_CAME_UP_SCORE += 0.1;
        UI.Main.PROJECT_DEADLINE_CAME_UP_LIFE.localScale = Util.Lerp(50.0, UI.Main.PROJECT_DEADLINE_CAME_UP_LIFE.localScale, Util.Vector.ExcludeX(UI.Main.PROJECT_DEADLINE_CAME_UP_LIFE.localScale, value: player.health / player.healthMaximum));
        Settings.SetProperty<uint>("Statistics/HighScore", System.Math.Max((uint) UI.Main.PROJECT_DEADLINE_CAME_UP_SCORE, UI.Main.PROJECT_DEADLINE_CAME_UP_PRESCORE));
        UI.Main.PROJECT_DEADLINE_CAME_UP_SCO1.text = ((uint) UI.Main.PROJECT_DEADLINE_CAME_UP_SCORE).ToString("D10");
        UI.Main.PROJECT_DEADLINE_CAME_UP_SCO2.text = ((uint) UI.Main.PROJECT_DEADLINE_CAME_UP_PRESCORE).ToString("D10");
        UI.Main.components[string.Empty].transform.localEulerAngles += Util.Vector.MaskZ(UnityEngine.Vector3.one) * Util.Perc(20.0f);
        player.SetMaterialColor(Util.Eval(color => { color.a = 0.5f + ((player.health / player.healthMaximum) / 2.0f); return color; }, player.GetComponent<UnityEngine.Renderer>().material.color));

        if (player.health == 0) {
          UI.Main.PROJECT_DEADLINE_CAME_UP.SetAlpha(1);
          UI.Main.PROJECT_DEADLINE_CAME_UP.text = "u ded\nbye now";

          UI.Load.UriAsMusic("game-over.mp3", UI.Load.PlayMusic);
          Util.Wait.Until(3.0, _ => Game.Quit());
        }
      }
    }
  }
}
