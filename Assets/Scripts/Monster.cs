using PatchOdyssey;

/* … */
[UnityEngine.RequireComponent(typeof(UnityEngine.BoxCollider))]
public sealed class Monster : Entity {
  [ReadOnlyInInspector]                              private          bool                                                  bounceAutomaticallyPrior =  true;
  [ReadWriteInInspector]                             public  new      UnityEngine.BoxCollider                               collider                 => (UnityEngine.BoxCollider) base.collider;
  [ReadOnlyInInspector]                              private readonly System.Collections.Generic.List<UnityEngine.Material> materials                =  new();
  [ReadWriteInInspector]                             public           bool                                                  isMounted                => this.mountAutomatically && this.following is Tamer tamer && null != tamer && 0 != tamer.followers.Count && this == tamer.followers[0];
  [ReadOnlyInInspector]                              private          bool                                                  isMountedPrior           =  false;
  [ReadOnlyInInspector]                              private          UnityEngine.Vector3                                   localScalePrior          =  UnityEngine.Vector3.one;
  [ReadWriteInInspector, UnityEngine.SerializeField] private          bool                                                  mountAutomatically       =  true;
  [ReadOnlyInInspector]                              private          bool                                                  moveAutomaticallyPrior   =  true;
  [ReadOnlyInInspector]                              private          float                                                 movementSpeedPrior       =  2.00f;
  [ReadOnlyInInspector]                              private          bool                                                  priorIsUpdated           =  false;
  [ReadOnlyInInspector]                              private readonly System.Collections.Generic.List<UnityEngine.Renderer> renderers                =  new();
  [ReadOnlyInInspector]                              private readonly System.Collections.Generic.List<UnityEngine.Material> sharedMaterials          =  new();
  [ReadOnlyInInspector]                              private          float                                                 targetBerthPrior         =  2.25f;
  [ReadWriteInInspector]                             public           Entity?                                               wrestling                =  null;
  [ReadWriteInInspector]                             public           float                                                 wrestlingForce           =  30.00f;
  [ReadOnlyInInspector]                              public           Timeframe                                             wrestlingInterval        =  new(4.00);

  /* … */
  protected override void Awake() {
    this.movementDamping = 7.0f;
    base.Awake();

    // …
    this.movementRestCooldown.Reset();
    this.movementRestCooldown.Wait ((this.movementRestDuration + this.movementRestDurationRandomness) * Game.Randomizer.NextDouble());

    this.bounceAutomaticallyPrior = this.bounceAutomatically;
    this.isMountedPrior           = this.isMounted;
    this.localScalePrior          = this.transform.localScale;
    this.moveAutomaticallyPrior   = this.moveAutomatically;
    this.movementSpeedPrior       = this.movementSpeed;
    this.rigidBody.interpolation  = UnityEngine.RigidbodyInterpolation.Extrapolate;
    this.targetBerthPrior         = this.targetBerth;
  }

  protected override void LateUpdate() {
    base.LateUpdate();

    if (Game.IsPaused || this.isDefeated)
    return;

    // …
    this.priorIsUpdated = false;

    if (this.isMounted && !this.isMountedPrior) {
      this.bounceAutomaticallyPrior = this.bounceAutomatically;
      this.localScalePrior          = this.transform.localScale;
      this.moveAutomaticallyPrior   = this.moveAutomatically;
      this.movementSpeedPrior       = this.movementSpeed;
      this.priorIsUpdated           = true;
      this.targetBerthPrior         = this.targetBerth;
    }

    this.isMountedPrior = this.isMounted;
  }

  private void OnDestroy() {
    foreach (UnityEngine.Material material in this.materials)
      UnityEngine.Object.Destroy(material);

    this.materials      .Clear();
    this.renderers      .Clear();
    this.sharedMaterials.Clear();
  }

  private void OnTriggerEnter(UnityEngine.Collider collider) {
    if (this.isDefeated)                                                                                        return;
    if (collider.GetComponent<Lasoo>() is not Lasoo lasoo || lasoo.capture is not null || !lasoo.isDeploying()) return;

    lasoo.capture  = this;
    this.wrestling = lasoo.user;
  }

  protected override void Update() {
    UnityEngine.Transform transform = this.transform;

    // …
    base.Update();

    if (Game.IsPaused || this.isDefeated)
    return;

    // … ->> Setup
      // … ->> Following, not Mounted
      if (null != this.following && !this.isMounted) {
        transform.localScale = this.localScalePrior    * 0.80f;
        this.movementSpeed   = this.movementSpeedPrior * 1.50f;
        this.targetBerth     = this.targetBerthPrior   * 1.15f;
      }

      else {
        transform.localScale = this.localScalePrior;
        this.movementSpeed   = this.movementSpeedPrior;
        this.targetBerth     = this.targetBerthPrior;
      }

      // … ->> Mounted
      if (this.isMounted) {
        if (this.priorIsUpdated) {
          this.bounceAutomatically = false;
          this.moveAutomatically   = false;
        }

        if (0 == this.materials.Count) {
          this.materials      .Capacity = transform.hierarchyCount;
          this.renderers      .Capacity = transform.hierarchyCount;
          this.sharedMaterials.Capacity = transform.hierarchyCount;

          transform.ForEach(transform => {
            if (transform.GetComponent<UnityEngine.Renderer>() is UnityEngine.Renderer renderer && null != renderer) {
              this.sharedMaterials.Add(renderer.sharedMaterial);
              this.renderers      .Add(renderer);
              this.materials      .Add(renderer.material = renderer.material);
            }
          });
        }

        this.rigidBody.Sleep();
      }

      else {
        this.bounceAutomatically = this.bounceAutomaticallyPrior;
        this.moveAutomatically   = this.moveAutomaticallyPrior;

        for (int index = this.materials.Count; 0 != index--; ) {
          this.renderers[index].sharedMaterial = this.sharedMaterials[index];
          UnityEngine.Object.Destroy(this.materials[index]);
        }

        this.materials.Clear();
        this.renderers.Clear();
        this.rigidBody.WakeUp();
        this.sharedMaterials.Clear();
      }

    // … ->> Following
    if (this.isMounted) {
      UnityEngine.Renderer  renderer           = this.GetComponent<UnityEngine.Renderer>();
      UnityEngine.Transform followingTransform = this.following!.transform;
      UnityEngine.Renderer  followingRenderer  = followingTransform.GetComponent<UnityEngine.Renderer>();
      UnityEngine.Bounds    followingBounds    = null != followingRenderer ? followingRenderer.bounds : new(followingTransform.position, UnityEngine.Vector3.zero);
      UnityEngine.Bounds    bounds             = null != renderer ? renderer.bounds : new(transform.position, UnityEngine.Vector3.zero);

      // …
      transform.SetPositionAndRotation(UnityEngine.Vector3.SlerpUnclamped(transform.position, followingTransform.position, 0.65f), UnityEngine.Quaternion.SlerpUnclamped(transform.rotation, followingTransform.rotation, 0.2f));
      this.target = this.following!.target;

      for (int index = this.materials.Count; 0 != index--; )
      this.materials[index].color = UnityEngine.Color.Lerp(this.sharedMaterials[index].color, UnityEngine.Color.white, UnityEngine.Mathf.Min((followingBounds.min - bounds.max).sqrMagnitude, (followingBounds.max - bounds.min).sqrMagnitude) / (UnityEngine.Vector3.one * 8.0f).sqrMagnitude);
    }

    else if (null != this.following) {
      this.movementRestCooldown.Finish();
      this.target = this.following!.target ?? this.following;
    }

    // … ->> Wrestling
    if (this.wrestling is not null) {
      this.target = this.wrestling;

      if (this.wrestling is Player player) {
        if (this.wrestlingInterval.isLooped) // ->> Pulled toward
        this.rigidBody.AddForce((this.wrestling.transform.position - transform.position).normalized * (this.wrestlingForce * (float) Game.Randomizer.NextDouble()), UnityEngine.ForceMode.Impulse);

        if (player.lasoo!.reachProgress >= 1.0f)
        player.ResetLasoo(); // ->> Lasoo stretched too far
      }
    } else this.wrestlingInterval.Reset();

    // … ->> Moving
    this.movementDirection = UnityEngine.Vector3.zero;

    if (this.target is not null) {
      float               berthMagnitudeSquared = (UnityEngine.Vector3.one * this.targetBerth).sqrMagnitude;
      UnityEngine.Vector3 distance              = this.target.transform.position - transform.position;

      // …
      if      (this.wrestling is not null)                    this.movementDirection = -distance.normalized;
      else if (berthMagnitudeSquared < distance.sqrMagnitude) this.movementDirection =  distance.normalized;
    }
  }
}
