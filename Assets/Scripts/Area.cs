using PatchOdyssey;

/* … */
[UnityEngine.DefaultExecutionOrder(6)]
[UnityEngine.DisallowMultipleComponent]
public sealed class Area : GameComponent {
  public static Timeframe EntryTransition = new(2.0, Timeframe.EaseIn);
  public static Timeframe ExitTransition  = new(1.0, Timeframe.EaseOut);

  [ReadWriteInInspector]                            public           bool                                    alternativeAutomatically = true;
  [ReadWriteInInspector]                            public           string                                  areaName                 = string.Empty;
  [ReadOnlyInInspector, UnityEngine.SerializeField] private          bool                                    isLocked                 = false;
  [ReadWriteInInspector]                            public           bool                                    respawnAutomatically     = false;
  [ReadOnlyInInspector, UnityEngine.SerializeField] private          uint                                    spawnCount               = 0u;
  [ReadWriteInInspector]                            public           float                                   spawnDelay               = 1.0f;
  [ReadWriteInInspector]                            public           System.Collections.Generic.List<Entity> spawnPrefabrications     = new();
  [ReadWriteInInspector]                            private readonly System.Collections.Generic.List<Entity> spawns                   = new();

  /* … ->> Must be hollow to work? */
  private void Awake() => this.spawns.Capacity = this.spawnPrefabrications.Count;

  private void OnTriggerEnter(UnityEngine.Collider collider) {
    if (collider.TryGetComponent(out Player _)) {
      Area.EntryTransition.Reset();
      Area.ExitTransition .Reset();

      if (UI.main.HUD.layoutContainers.area is not null)
      UI.main.HUD.layoutContainers.area.text = this.areaName;
    }
  }

  private void OnTriggerExit(UnityEngine.Collider collider) {
    if (collider.TryGetComponent(out Player player)) {
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
      this.isLocked = 0 != this.spawnPrefabrications.Count;
    }
  }

  protected override void Update() {
    base.Update();

    // … ->> Area
    if (!Area.EntryTransition.isElapsed)
    Area.ExitTransition.Reset();

    if (UI.main.HUD.layoutContainers.area is not null)
    UI.main.HUD.layoutContainers.area.color = UnityEngine.Color.Lerp(UnityEngine.Color.white.Transparent(), UnityEngine.Color.white, (float) (Area.EntryTransition.isElapsed ? 1.0 - Area.ExitTransition.easedProgress : Area.EntryTransition.easedProgress));

    // … ->> Lock in
    foreach (UnityEngine.Collider collider in base.colliders)
    collider.isTrigger = !this.isLocked;

    // … ->> Spawning
    if (this.isLocked) {
      System.Collections.IEnumerator DeployEntity() {
        if (this.spawnCount == this.spawnPrefabrications.Count) {
          UnityEditor.EditorApplication.isPaused = true;
          yield break;
        }

        /* … */
        Entity              entity;
        UnityEngine.Bounds  spawnBounds         = new(this.transform.position, UnityEngine.Vector3.one);
        UnityEngine.Vector3 spawnPosition       = UnityEngine.Vector3.zero;
        Entity              spawnPrefabrication = this.spawnPrefabrications[(int) this.spawnCount++];

        // …
        for (bool isBounded = false; !isBounded; spawnBounds.Expand(1.0f)) {
          foreach (UnityEngine.Collider collider in base.colliders)
          if (collider.bounds.Intersects(spawnBounds)) {
            isBounded = true;
            break;
          }
        }

        spawnPosition.x = UnityEngine.Random.Range(spawnBounds.min.x, spawnBounds.max.x) * 1.0f;
        spawnPosition.y = UnityEngine.Random.Range(spawnBounds.min.y, spawnBounds.max.y) * 0.0f;
        spawnPosition.z = UnityEngine.Random.Range(spawnBounds.min.z, spawnBounds.max.z) * 1.0f;

        // …
        if (this.alternativeAutomatically)
        switch (spawnPrefabrication) {
          case Monster spawnMonster: switch (spawnMonster.kind) {
            case Monster.Kind.Antillery: spawnPrefabrication = Assets.main.spawnables.monsters.antilleryPrefabrication[UnityEngine.Random.Range(0, Assets.main.spawnables.monsters.antilleryPrefabrication.Count)]; break;
            case Monster.Kind.Borka:     spawnPrefabrication = Assets.main.spawnables.monsters.borkaPrefabrication    [UnityEngine.Random.Range(0, Assets.main.spawnables.monsters.borkaPrefabrication    .Count)]; break;
            case Monster.Kind.Molem:     spawnPrefabrication = Assets.main.spawnables.monsters.molemPrefabrication    [UnityEngine.Random.Range(0, Assets.main.spawnables.monsters.molemPrefabrication    .Count)]; break;
            case Monster.Kind.Sirpens:   spawnPrefabrication = Assets.main.spawnables.monsters.sirpensPrefabrication  [UnityEngine.Random.Range(0, Assets.main.spawnables.monsters.sirpensPrefabrication  .Count)]; break;
            case Monster.Kind.Tyrage:    spawnPrefabrication = Assets.main.spawnables.monsters.tyragePrefabrication   [UnityEngine.Random.Range(0, Assets.main.spawnables.monsters.tyragePrefabrication   .Count)]; break;
          } break;

          case Tamer spawnTamer: switch (spawnTamer.team) {
            case Entity.Team.Explorer: spawnPrefabrication = Assets.main.spawnables.tamers.explorerPrefabrication[UnityEngine.Random.Range(0, Assets.main.spawnables.tamers.explorerPrefabrication.Count)]; break;
            case Entity.Team.Nomad:    spawnPrefabrication = Assets.main.spawnables.tamers.nomadPrefabrication   [UnityEngine.Random.Range(0, Assets.main.spawnables.tamers.nomadPrefabrication   .Count)]; break;
            case Entity.Team.Magnate:  spawnPrefabrication = Assets.main.spawnables.tamers.magnatePrefabrication [UnityEngine.Random.Range(0, Assets.main.spawnables.tamers.magnatePrefabrication .Count)]; break;
          } break;
        }

        entity = UnityEngine.Object.Instantiate(spawnPrefabrication, spawnPosition, UnityEngine.Quaternion.identity).GetComponent<Entity>();
        this.spawns.Add(entity);

        if (entity is Tamer tamer && UnityEngine.Random.value > 0.2f) {
          Monster monster;
          System.Collections.Generic.List<Monster> monsters = new(
            Assets.main.spawnables.monsters.antilleryPrefabrication.Count +
            Assets.main.spawnables.monsters.borkaPrefabrication    .Count +
            Assets.main.spawnables.monsters.molemPrefabrication    .Count +
            Assets.main.spawnables.monsters.sirpensPrefabrication  .Count +
            Assets.main.spawnables.monsters.tyragePrefabrication   .Count
          );

          // …
          monsters.AddRange(Assets.main.spawnables.monsters.antilleryPrefabrication);
          monsters.AddRange(Assets.main.spawnables.monsters.borkaPrefabrication);
          monsters.AddRange(Assets.main.spawnables.monsters.molemPrefabrication);
          monsters.AddRange(Assets.main.spawnables.monsters.sirpensPrefabrication);
          monsters.AddRange(Assets.main.spawnables.monsters.tyragePrefabrication);

          monster           = UnityEngine.Object.Instantiate(monsters[Game.Randomizer.Next(monsters.Count)], -spawnPosition, UnityEngine.Quaternion.identity, entity.transform).GetComponent<Monster>();
          monster.following = tamer;
          monster.team      = tamer.team;

          tamer.followers.Add(monster);
        }

        yield return new UnityEngine.WaitForSeconds(this.spawnDelay);
        base.StartCoroutine(DeployEntity());
      }

      /* … */
      if (0u == this.spawnCount)
        base.StartCoroutine(DeployEntity());

      else if (this.spawnCount == this.spawnPrefabrications.Count) {
        this.isLocked = false;

        for (int index = this.spawns.Count; 0 != index--; ) {
          if (Entity.Team.Player != this.spawns[index].team)
          this.isLocked = true;

          if (null == this.spawns[index])
          this.spawns.RemoveAt(index);
        }
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
