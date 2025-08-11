using PatchOdyssey;

/* … */
[UnityEngine.DefaultExecutionOrder(6)]
[UnityEngine.DisallowMultipleComponent]
public sealed class NPC : GameComponent {
  [ReadOnlyInInspector]  public string                                  birthName           = string.Empty;
  [ReadWriteInInspector] public Timeframe                               chatTransition      = new(1.0);
  [ReadWriteInInspector] public System.Collections.Generic.List<string> monologue           = new(1);
  [ReadOnlyInInspector]  public uint                                    monologueCount      = 0u;
  [ReadWriteInInspector] public Timeframe                               monologueTransition = new(2.0);
  [ReadOnlyInInspector]  public Player?                                 monologuing         = null;
  [ReadOnlyInInspector]  public UnityEngine.Vector3                     turnDirection       = UnityEngine.Vector3.back;

  /* … */
  private void Awake() {
    this.birthName     = string.IsNullOrEmpty(this.birthName) ? base.name : this.birthName;
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

  protected override void Update() {
    bool                isLooking       = false;
    UnityEngine.Vector3 targetDirection = this.turnDirection;

    // …
    base.Update();

    // … ->> Chatting
    if (0 != this.monologue.Count) {
      System.Collections.Generic.List<Entity> entities = Entity.All.FindAll(static entity => entity is Player);

      // …
      entities.Sort((entityA, entityB) => System.Math.Sign((entityA.transform.position - this.transform.position).sqrMagnitude - (entityB.transform.position - this.transform.position).sqrMagnitude));

      foreach (Entity entity in entities) {
        Player player = (Player) entity;

        // …
        targetDirection = !isLooking ? (this.transform.position - player.transform.position).normalized : targetDirection;
        isLooking       = true;

        if (player.isChatting && this.InteractsWith(player)) {
          this.chatTransition     .Reset();
          this.monologueTransition.Reset();

          this.monologueCount = 0u;
          this.monologuing    = player;

          break;
        }
      }
    }

    // … ->> Monologuing
    if (null != this.monologuing) {
      Player target = (Player) this.monologuing;

      // …
      if (this.monologue.Count <= this.monologueCount || !this.InteractsWith(target)) {
        foreach (UI.HeadsUpDisplay.Containers containers in UI.main.HUD.containers) {
          if (null != containers.chat)
          UI.main.ChangeContainer(containers.chat, UI.ContainerVisibility.Hidden);
        }

        this.monologuing         = null;
        target.moveAutomatically = true;
      }

      else {
        string monologue = this.monologue[(int) this.monologueCount];

        // …
        target.moveAutomatically = false;

        foreach (UI.HeadsUpDisplay.Containers containers in UI.main.HUD.containers) {
          if (null != containers.chat)
          UI.main.ChangeContainer(containers.chat, UI.ContainerVisibility.Visible, this.chatTransition);

          if (null != containers.chatName)
          containers.chatName.text = this.birthName;

          if (null != containers.chatText) {
            containers.chatText.color = UnityEngine.Color.Lerp(UnityEngine.Color.white.Transparent(), UnityEngine.Color.white, (float) this.monologueTransition.progress * 3.333333f);
            containers.chatText.text  = monologue.Substring(0, (int) System.MathF.Ceiling(monologue.Length * (float) this.monologueTransition.progress));
          }
        }

        // …
        if (target.isChatting) {
          if (this.monologueTransition.isLooped)
            ++this.monologueCount;

          else {
            this.chatTransition     .Finish();
            this.monologueTransition.Finish();
          }
        }
      }
    }
  }
}
