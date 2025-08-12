using PatchOdyssey;

/* … */
[UnityEngine.DefaultExecutionOrder(6)]
[UnityEngine.DisallowMultipleComponent]
public sealed class NPC : GameComponent {
  [System.Serializable]
  public /* readonly */ struct Premonologue /* ->> Cached set of values before and after chatting to an `NPC` */ {
    [ReadOnlyInInspector] public bool isAggressive;
    [ReadOnlyInInspector] public bool isInvincible;
    [ReadOnlyInInspector] public bool lasooAutomatically;
    [ReadOnlyInInspector] public bool moveAutomatically;
    [ReadOnlyInInspector] public bool statisticsHealth;
    [ReadOnlyInInspector] public bool statisticsShoot;
  }

  /* … */
  public static NPC?      Chatting            = null;
  public static Timeframe ChatTransition      = new(1.0);
  public static uint      Count               = 0u;
  public static Timeframe MonologueTransition = new(0.5);

  [ReadWriteInInspector] public string                                  birthName         = string.Empty;
  [ReadOnlyInInspector]  public Player?                                 interacting       = null;
  [ReadWriteInInspector] public System.Collections.Generic.List<string> monologue         = new(1);
  [ReadOnlyInInspector]  public uint                                    monologueCount    = 0u;
  [ReadOnlyInInspector]  public Player?                                 monologuing       = null;
  [ReadOnlyInInspector]  public NPC.Premonologue                        premonologue      = new() {isAggressive = false, isInvincible = false, moveAutomatically = false, statisticsHealth = false, statisticsShoot = false};
  [ReadOnlyInInspector]  public bool                                    turnAutomatically = false;
  [ReadOnlyInInspector]  public UnityEngine.Vector3                     turnDirection     = UnityEngine.Vector3.back;

  /* … */
  private void Awake() {
    this.birthName     = string.IsNullOrWhiteSpace(this.birthName) ? base.name : this.birthName;
    this.turnDirection = this.transform.forward;
  }

  private bool InteractsWith(Player player) {
    foreach (UnityEngine.Collider collider in base.colliders) {
      UnityEngine.Bounds bounds = collider.bounds;

      // …
      foreach (UnityEngine.Collider subcollider in player.colliders) {
        if (bounds.Intersects(subcollider.bounds))
        return true;
      }
    }

    return false;
  }

  private void OnDisable() => --NPC.Count;
  private void OnEnable () => ++NPC.Count;

  private void TryChat() {
    UnityEngine.Debug.Log("HELLO WORLD");

    if (null != this.monologuing)
    this.monologuing.isChatting = true;
  }

  protected override void Update() {
    bool                isLooking       = false;
    UnityEngine.Vector3 targetDirection = this.turnDirection;

    // …
    base.Update();

    // … ->> Chatting
    this.interacting = this.monologuing;

    if (0 != this.monologue.Count && null == this.monologuing) {
      System.Collections.Generic.List<Entity> entities = Entity.All.FindAll(static entity => entity is Player);

      // …
      entities.Sort((entityA, entityB) => System.Math.Sign((entityA.transform.position - this.transform.position).sqrMagnitude - (entityB.transform.position - this.transform.position).sqrMagnitude));

      foreach (Entity entity in entities) {
        Player player = (Player) entity;

        // …
        targetDirection = !isLooking ? (player.transform.position - this.transform.position).normalized : targetDirection;
        isLooking       = true;

        if (this.InteractsWith(player)) {
          NPC.Chatting     = null == NPC.Chatting     ? this   : NPC.Chatting;
          this.interacting = null == this.interacting ? player : this.interacting;

          if (player.isChatting) {
            NPC.ChatTransition     .Reset();
            NPC.MonologueTransition.Reset();

            this.monologueCount                  = 0u;
            this.monologuing                     = player;
            this.premonologue.isAggressive       = player.isAggressive;
            this.premonologue.isInvincible       = player.isInvincible;
            this.premonologue.moveAutomatically  = player.moveAutomatically;
            this.premonologue.lasooAutomatically = player.lasooAutomatically;
            this.premonologue.statisticsHealth   = player.statistics.health;
            this.premonologue.statisticsShoot    = player.statistics.shoot;
          }

          break;
        }
      }
    }

    if (null != NPC.Chatting) {
      if (null != UI.main.buttons.chat) {
        UnityEngine.Vector2       chatOffset    = UnityEngine.Vector2.up * 150.0f;
        UnityEngine.Vector2       chatPosition  = (UnityEngine.Vector2.Scale(base.worldCamera!.WorldToViewportPoint(NPC.Chatting.transform.position), UI.main.rectTransform.sizeDelta) - (UI.main.rectTransform.sizeDelta * 0.5f)) / UI.main.canvas.scaleFactor;
        UnityEngine.RectTransform chatTransform = (UnityEngine.RectTransform) UI.main.buttons.chat.transform;

        // …
        chatTransform.anchoredPosition = UnityEngine.Vector2.LerpUnclamped(chatTransform.anchoredPosition, chatOffset + chatPosition, 0.1f / (NPC.Count | 1));
        chatTransform.anchorMin        = new(0.5f, 0.5f);
        chatTransform.anchorMax        = new(0.5f, 0.5f);

        if (null != UI.main.buttons.chat.targetGraphic)
        UI.main.buttons.chat.targetGraphic.color = UnityEngine.Color.LerpUnclamped(UI.main.buttons.chat.targetGraphic.color, UI.main.buttons.chat.targetGraphic.color.Opaque(), 0.1f / (NPC.Count | 1));
      }
    }

    else {
      if (null != UI.main.buttons.chat && null != UI.main.buttons.chat.targetGraphic)
      UI.main.buttons.chat.targetGraphic.color = UnityEngine.Color.LerpUnclamped(UI.main.buttons.chat.targetGraphic.color, UI.main.buttons.chat.targetGraphic.color.Transparent(), 0.2f / (NPC.Count | 1));
    }

    // … ->> Looking
    if (this == NPC.Chatting || this.turnAutomatically)
    this.transform.localRotation = UnityEngine.Quaternion.SlerpUnclamped(this.transform.localRotation, UnityEngine.Quaternion.Euler(UnityEngine.Vector3.Scale(UnityEngine.Vector3.up, UnityEngine.Quaternion.LookRotation(targetDirection, UnityEngine.Vector3.up).eulerAngles)), 0.15f);

    // … ->> Monologuing
    if (null != this.monologuing) {
      string chatMonologue = string.Empty;
      Player target        = (Player) this.monologuing;

      // …
      if (this.monologue.Count <= this.monologueCount || !this.InteractsWith(target)) {
        NPC.Chatting              = null;
        this.monologueCount       = 0u;
        this.monologuing          = null;
        target.isAggressive       = this.premonologue.isAggressive;
        target.isInvincible       = this.premonologue.isInvincible;
        target.moveAutomatically  = this.premonologue.moveAutomatically;
        target.lasooAutomatically = this.premonologue.lasooAutomatically;
        target.statistics.health  = this.premonologue.statisticsHealth;
        target.statistics.shoot   = this.premonologue.statisticsShoot;

        foreach (UI.HeadsUpDisplay.Containers containers in UI.main.HUD.containers) {
          if (null != containers.chat)
          UI.main.ChangeContainer(containers.chat, UI.ContainerVisibility.Hidden);
        }
      }

      else {
        chatMonologue             = this.monologue[(int) this.monologueCount];
        chatMonologue             = chatMonologue.Substring(0, (int) System.MathF.Ceiling(chatMonologue.Length * (float) NPC.MonologueTransition.progress));
        target.isAggressive       = true;
        target.isInvincible       = true;
        target.moveAutomatically  = false;
        target.lasooAutomatically = false;
        target.statistics.health  = false;
        target.statistics.shoot   = false;
        target.shoot.cooldown.Reset();

        foreach (UI.HeadsUpDisplay.Containers containers in UI.main.HUD.containers) {
          if (null != containers.chat)     { UI.main.ChangeContainer(containers.chat, UI.ContainerVisibility.Visible, NPC.ChatTransition); }
          if (null != containers.chatName) { containers.chatName.text = this.birthName; }
          if (null != containers.chatText) { containers.chatText.text = chatMonologue; containers.chatText.color = UnityEngine.Color.Lerp(containers.chatText.color.Transparent(), containers.chatText.color.Opaque(), (float) NPC.MonologueTransition.progress * 3.333333f); }
        }

        // … ->> Next line of monologue
        if (target.isChatting) {
          if (NPC.MonologueTransition.isLooped) ++this.monologueCount;
          else { NPC.ChatTransition.Finish(); NPC.MonologueTransition.Finish(); }
        }
      }
    }
  }
}
