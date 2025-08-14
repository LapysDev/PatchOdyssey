using PatchOdyssey;

/* … */
[UnityEngine.DefaultExecutionOrder(6)]
[UnityEngine.DisallowMultipleComponent]
public sealed class Area : GameComponent {
  public static readonly System.Collections.Generic.List<Area> All             = new(8); // --> UnityEngine.Object.FindObjectsByType<Area>(UnityEngine.FindObjectsInactive.Include, UnityEngine.FindObjectsSortMode.None)
  public static          Timeframe                             EntryTransition = new(2.0, Timeframe.EaseIn);
  public static          Timeframe                             ExitTransition  = new(1.0, Timeframe.EaseOut);

  [ReadWriteInInspector]                            public           bool                                                                        alternativeAutomatically = true;
  [ReadWriteInInspector]                            public           string                                                                      areaName                 = string.Empty;
  [ReadWriteInInspector]                            public           bool                                                                        invincibleAutomatically  = false;
  [ReadOnlyInInspector, UnityEngine.SerializeField] private          bool                                                                        isLocked                 = false;
  [ReadOnlyInInspector]                             public           bool                                                                        isReset                  = false;
  [ReadOnlyInInspector]                             private readonly System.Collections.Generic.List<(Entity entity, bool isInvincible)>         locked                   = new();
  [ReadWriteInInspector]                            public           UnityEngine.Events.UnityEvent                                               onComplete               = new();
  [ReadWriteInInspector]                            public           bool                                                                        respawnAutomatically     = false;
  [ReadWriteInInspector]                            public           bool                                                                        scoreAutomatically       = true;
  [ReadOnlyInInspector, UnityEngine.SerializeField] private          uint                                                                        spawnCount               = 0u;
  [ReadWriteInInspector]                            public           float                                                                       spawnDelay               = 1.0f;
  [ReadWriteInInspector]                            public           System.Collections.Generic.List<Entity>                                     spawnPrefabrications     = new();
  [ReadWriteInInspector]                            private readonly System.Collections.Generic.List<(Entity entity, UnityEngine.Vector3, uint)> spawns                   = new();
  [ReadWriteInInspector]                            public           System.Collections.Generic.List<Entity.Team>                                spawnTeams               = new();

  /* … ->> Must be hollow to work? */
  private void Awake() {
    this.areaName        = string.IsNullOrWhiteSpace(this.areaName) ? base.name : this.areaName;
    this.spawns.Capacity = this.spawnPrefabrications.Count;

    Area.All.Add(this);
  }

  private void OnTriggerEnter(UnityEngine.Collider collider) {
    if (collider.TryGetComponent(out Player _)) {
      Area.EntryTransition.Reset();
      Area.ExitTransition .Reset();

      this.isReset = false;

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
      areaBounds?.Expand(1.0f + (player.movement.speed * UnityEngine.Mathf.Max(1.0f, player.movement.speedFactor))); // ->> ╮( ˘ ､˘ )╭

      if (areaBounds?.Contains(player.transform.position) ?? false) {
        this.isLocked = 0 != this.spawnPrefabrications.Count;

        if (this.invincibleAutomatically) {
          this.locked.Add((player, player.isInvincible));
          player.isInvincible = true;
        }
      }
    }
  }

  private        void OnDestroy() => Area.All.Remove(this);
  public  static void Reset    () { foreach (Area area in Area.All) { area.isReset = true; area.Respawn(); area.spawns.Clear(); } }
  private        void Respawn  () => this.spawnCount = 0u;

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
        if (this.isReset || this.spawnCount == this.spawnPrefabrications.Count)
        yield break;

        /* … */
        Entity              entity;
        UnityEngine.Bounds  spawnBounds         = new(this.transform.position, UnityEngine.Vector3.one);
        UnityEngine.Vector3 spawnPosition       = UnityEngine.Vector3.zero;
        Entity              spawnPrefabrication = this.spawnPrefabrications[(int) this.spawnCount];
        Entity.Team         spawnTeam           = this.spawnCount < this.spawnTeams.Count ? this.spawnTeams[(int) this.spawnCount] : spawnPrefabrication.team;

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

        entity      = UnityEngine.Object.Instantiate(spawnPrefabrication, spawnPosition, UnityEngine.Quaternion.identity).GetComponent<Entity>();
        entity.team = spawnTeam;
        this.spawnCount++;

        this.spawns.Add((entity, entity.defeat.position, entity.score + (uint) (UnityEngine.Random.value * entity.score * 0.2f)));

        if (entity is Tamer tamer && UnityEngine.Random.value > 0.2f) {
          Monster monster;
          System.Collections.Generic.List<Monster> monsters = new(
            Assets.main.spawnables.monsters.antilleryPrefabrication.Count +
            Assets.main.spawnables.monsters.borkaPrefabrication    .Count +
            Assets.main.spawnables.monsters.molemPrefabrication    .Count +
            Assets.main.spawnables.monsters.sirpensPrefabrication  .Count
          );

          // …
          monsters.AddRange(Assets.main.spawnables.monsters.antilleryPrefabrication);
          monsters.AddRange(Assets.main.spawnables.monsters.borkaPrefabrication);
          monsters.AddRange(Assets.main.spawnables.monsters.molemPrefabrication);
          monsters.AddRange(Assets.main.spawnables.monsters.sirpensPrefabrication);

          monster           = UnityEngine.Object.Instantiate(monsters[Game.Randomizer.Next(monsters.Count)], -spawnPosition, UnityEngine.Quaternion.identity, entity.transform).GetComponent<Monster>();
          monster.following = tamer;
          monster.team      = tamer.team;

          tamer.followers.Add(monster);
        }

        yield return new UnityEngine.WaitForSeconds(this.spawnDelay);
        base.StartCoroutine(DeployEntity());
      }

      /* … */
      if (0u == this.spawnCount) {
        if (!this.isReset)
        base.StartCoroutine(DeployEntity());
      }

      else if (this.spawnCount == this.spawnPrefabrications.Count) {
        this.isLocked = false;

        for (int index = this.spawns.Count; 0 != index--; ) {
          (Entity entity, UnityEngine.Vector3 defeatPosition, uint score) = this.spawns[index];

          // …
          if (null != entity) {
            this.isLocked      = this.isLocked || Entity.Team.Player != entity.team;
            this.spawns[index] = (entity, entity.defeat.position, score);

            continue;
          }

          // … ->> Render score above defeated `entity`
          if (this.scoreAutomatically) {
            if (null != Assets.main.primitives.text && 0u != score) {
              TMPro.TextMeshProUGUI     scoreText          = UnityEngine.Object.Instantiate(Assets.main.primitives.text, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity, UI.main.HUD.layout!.transform).GetComponent<TMPro.TextMeshProUGUI>();
              UnityEngine.Vector2       scoreTextPosition  = (UnityEngine.Vector2.Scale(this.worldCamera!.WorldToViewportPoint(defeatPosition), UI.main.rectTransform.sizeDelta) - (UI.main.rectTransform.sizeDelta * 0.5f)) / UI.main.canvas.scaleFactor;
              UnityEngine.RectTransform scoreTextTransform = (UnityEngine.RectTransform) scoreText.transform;

              /* … */
              System.Collections.IEnumerator DestroyScoreText() {
                yield return new UnityEngine.WaitForSecondsRealtime((float) UI.main.scoreTimer.duration);
                UnityEngine.Object.Destroy(scoreText.gameObject);
              }

              /* … */
              scoreText.CrossFadeAlpha(0.25f, (float) UI.main.scoreTimer.duration * 0.75f, true);
              scoreText.transform.SetSiblingIndex(0);

              scoreTextTransform.anchorMin        = new(0.5f, 0.5f);
              scoreTextTransform.anchorMax        = new(0.5f, 0.5f);
              scoreTextTransform.anchoredPosition = scoreTextPosition;
              scoreText.text                      = score.ToString();
              scoreText.richText                  = false;
              scoreText.overrideColorTags         = true;
              scoreText.overflowMode              = TMPro.TextOverflowModes.Truncate;
              scoreText.outlineWidth              = 1.5f;
              scoreText.outlineColor              = new((byte) 0u, (byte) 0u, (byte) 0u, (byte) 127u);
              scoreText.maskable                  = false;
              scoreText.margin                    = UnityEngine.Vector4.zero;
              scoreText.isOrthographic            = true;
              scoreText.extraPadding              = false;
              scoreText.enableVertexGradient      = false;
              scoreText.color                     = UnityEngine.Color.white;
              scoreText.autoSizeTextContainer     = true;
              scoreText.alignment                 = TMPro.TextAlignmentOptions.Center | TMPro.TextAlignmentOptions.Midline;

              base.StartCoroutine(DestroyScoreText());

              #if DEBUG || DEVELOPMENT_BUILD
                UnityEngine.Debug.DrawRay(defeatPosition, UnityEngine.Vector3.forward, UnityEngine.Color.cyan,    2.0f, false);
                UnityEngine.Debug.DrawRay(defeatPosition, UnityEngine.Vector3.right,   UnityEngine.Color.magenta, 2.0f, false);
                UnityEngine.Debug.DrawRay(defeatPosition, UnityEngine.Vector3.up,      UnityEngine.Color.yellow,  2.0f, false);
              #endif
            }

            Stats.Score += score;
          }

          // …
          this.spawns.RemoveAt(index);
        }
      }

      // …
      if (!this.isLocked) {
        foreach ((Entity entity, bool isInvincible) in this.locked)
        entity.isInvincible = isInvincible;

        for (int index = this.spawns.Count; 0 != index--; )
        if (this.spawns[index].entity is not Monster monster || !monster.isMounted) {
          this.spawns[index].entity.isDefeated = true;
          this.spawns.RemoveAt(index);
        }

        this.locked.Clear();
        this.spawns.Clear();

        this.onComplete.Invoke();
      }
    }

    else if (this.respawnAutomatically)
      this.Respawn();
  }
}
