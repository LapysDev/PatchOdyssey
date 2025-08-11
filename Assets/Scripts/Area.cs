using PatchOdyssey;

/* … */
[UnityEngine.DefaultExecutionOrder(6)]
[UnityEngine.DisallowMultipleComponent]
public sealed class Area : GameComponent {
  [ReadOnlyInInspector]                             public           bool                                    alternativeAutomatically = true;
  [ReadWriteInInspector]                            public           string                                  areaName                 = string.Empty;
  [ReadWriteInInspector]                            public           Timeframe                               areaEntryTransition      = new(2.0, Timeframe.EaseIn);
  [ReadWriteInInspector]                            public           Timeframe                               areaExitTransition       = new(1.0, Timeframe.EaseOut);
  [ReadOnlyInInspector, UnityEngine.SerializeField] private          bool                                    isLocked                 = false;
  [ReadOnlyInInspector]                             public           bool                                    respawnAutomatically     = false;
  [ReadOnlyInInspector, UnityEngine.SerializeField] private          uint                                    spawnCount               = 0u;
  [ReadOnlyInInspector]                             public           float                                   spawnDelay               = 1.0f;
  [ReadWriteInInspector]                            public           System.Collections.Generic.List<Entity> spawnPrefabrications     = new();
  [ReadWriteInInspector]                            private readonly System.Collections.Generic.List<Entity> spawns                   = new();

  /* … ->> Must be hollow to work? */
  private void Awake() => this.spawns.Capacity = this.spawnPrefabrications.Count;

  private void OnTriggerEnter(UnityEngine.Collider collider) {
    if (collider.TryGetComponent(out Player player)) {
      UnityEngine.Debug.Log($"“{collider.name}” entered “{this.name}”");
      this.areaEntryTransition.Reset();

      if (null != UI.main.HUD.layoutContainers.area) {
        UnityEngine.Debug.Log($"Set name “{this.areaName}”");
        UI.main.HUD.layoutContainers.area.text = this.areaName;
      }
    }
  }

  private void OnTriggerExit(UnityEngine.Collider collider) {
    if (collider.TryGetComponent(out Player player)) {
      UnityEngine.Debug.Log($"“{collider.name}” exited “{this.name}”");
      UnityEngine.Bounds? areaBounds = null;

      // …
      foreach (UnityEngine.Collider subcollider in base.colliders) {
        UnityEngine.Bounds colliderBounds = subcollider.bounds;

        // …
        if (areaBounds is UnityEngine.Bounds bounds) {
          bounds.Encapsulate(colliderBounds.center - colliderBounds.extents);
          bounds.Encapsulate(colliderBounds.center + colliderBounds.extents);

          areaBounds = bounds;
        } else areaBounds = colliderBounds;
      }

      // … ->> Lock in
      if (areaBounds?.Contains(player.transform.position) ?? false)
      this.isLocked = 0 != this.spawns.Count;
    }
  }

  protected override void Update() {
    base.Update();

    // … ->> Area
    if (null != UI.main.HUD.layoutContainers.area) {
      UI.main.HUD.layoutContainers.area.color = UnityEngine.Color.LerpUnclamped(UnityEngine.Color.white.Transparent(), UnityEngine.Color.white, (float) (this.areaEntryTransition.isElapsed ? 1.0 - this.areaExitTransition.easedProgress : this.areaEntryTransition.easedProgress));
      if (!this.areaEntryTransition.isElapsed) this.areaExitTransition.Reset();
    }

    // … ->> Lock in
    foreach (UnityEngine.Collider collider in base.colliders)
    collider.isTrigger = !this.isLocked;

    // … ->> Spawning
    if (this.isLocked) {
      System.Collections.IEnumerator DeployEntity() {
        if (this.spawnCount == this.spawnPrefabrications.Count)
        yield break;

        /* … */
        Entity spawnPrefabrication = this.spawnPrefabrications[(int) this.spawnCount++];

        // …
        if (this.alternativeAutomatically)
        switch (spawnPrefabrication) {
          case Monster monster: switch (monster.kind) {
            case Monster.Kind.Antillery: spawnPrefabrication = Assets.main.spawnables.monsters.antilleryPrefabrication[Game.Randomizer.Next(Assets.main.spawnables.monsters.antilleryPrefabrication.Count)]; break;
            case Monster.Kind.Borka:     spawnPrefabrication = Assets.main.spawnables.monsters.borkaPrefabrication    [Game.Randomizer.Next(Assets.main.spawnables.monsters.borkaPrefabrication    .Count)]; break;
            case Monster.Kind.Molem:     spawnPrefabrication = Assets.main.spawnables.monsters.molemPrefabrication    [Game.Randomizer.Next(Assets.main.spawnables.monsters.molemPrefabrication    .Count)]; break;
            case Monster.Kind.Sirpens:   spawnPrefabrication = Assets.main.spawnables.monsters.sirpensPrefabrication  [Game.Randomizer.Next(Assets.main.spawnables.monsters.sirpensPrefabrication  .Count)]; break;
            case Monster.Kind.Tyrage:    spawnPrefabrication = Assets.main.spawnables.monsters.tyragePrefabrication   [Game.Randomizer.Next(Assets.main.spawnables.monsters.tyragePrefabrication   .Count)]; break;
          } break;

          case Tamer tamer: switch (tamer.team) {
            case Entity.Team.Explorer: spawnPrefabrication = Assets.main.spawnables.tamers.explorerPrefabrication[Game.Randomizer.Next(Assets.main.spawnables.tamers.explorerPrefabrication.Count)]; break;
            case Entity.Team.Nomad:    spawnPrefabrication = Assets.main.spawnables.tamers.nomadPrefabrication   [Game.Randomizer.Next(Assets.main.spawnables.tamers.nomadPrefabrication   .Count)]; break;
            case Entity.Team.Magnate:  spawnPrefabrication = Assets.main.spawnables.tamers.magnatePrefabrication [Game.Randomizer.Next(Assets.main.spawnables.tamers.magnatePrefabrication .Count)]; break;
          } break;
        }

        this.spawns.Add(UnityEngine.Object.Instantiate(spawnPrefabrication, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity).GetComponent<Entity>());
        yield return new UnityEngine.WaitForSeconds(this.spawnDelay);
        base.StartCoroutine(DeployEntity());
      }

      /* … */
      base.StartCoroutine(DeployEntity());

      // …
      this.isLocked = false;

      for (int index = this.spawns.Count; 0 != index--; ) {
        if (null == this.spawns[index])
        this.spawns.RemoveAt(index);

        if (Entity.Team.Player != this.spawns[index].team)
        this.isLocked = true;
      }

      // …
      if (!this.isLocked) {
        for (int index = this.spawns.Count; 0 != index--; )
        if (this.spawns[index] is not Monster monster || !monster.isMounted) {
          UnityEngine.Object.Destroy(this.spawns[index]);
          this.spawns.RemoveAt(index);
        }

        this.spawns.Clear();
      }
    }

    else if (this.respawnAutomatically)
      this.spawnCount = 0u;
  }
}
