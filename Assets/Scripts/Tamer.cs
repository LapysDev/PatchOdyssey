using PatchOdyssey;

/* … */
[UnityEngine.DefaultExecutionOrder(2)]
[UnityEngine.RequireComponent(typeof(UnityEngine.SphereCollider))]
public class Tamer : Entity {
  public static readonly UnityEngine.Vector3 MountingPosition = (UnityEngine.Vector3.back * 0.200f) + (UnityEngine.Vector3.up * 1.125f);

  [UnityEngine.Header("Tamer")]
  [ReadOnlyInInspector]  public           UnityEngine.GameObject?                                hair               =  null;
  [ReadWriteInInspector] public           bool                                                   hairAutomatically  =  true;
  [ReadWriteInInspector] public  new      UnityEngine.SphereCollider                             collider           => (UnityEngine.SphereCollider) base.collider;
  [ReadOnlyInInspector]  private readonly System.Collections.Generic.List<UnityEngine.Transform> mountingTransforms =  new();

  /* … */
  protected virtual void Capture(Entity entity) {
    if (base.isDefeated)
    return;

    // …
    if (entity is Monster monster) {
      monster.following                = this;
      monster.isInvincible             = true;
      monster.team                     = base.team;
      this.mountingTransforms.Capacity = System.Math.Max(this.mountingTransforms.Capacity, this.transform.hierarchyCount);

      monster.transform.SetParent(this.transform, true);
      this.followers.Add(monster);
      this.transform.ForEach(transform => {
        if (monster.transform == transform)
        return false;

        if (null != transform.GetComponent<UnityEngine.Renderer>() && !this.mountingTransforms.Contains(transform)) {
          transform.localPosition += Tamer.MountingPosition;
          this.mountingTransforms.Add(transform);

          return false;
        }

        return true;
      });
    }

    // NOTE (Lapys) ->> Other kinds of `Entity`s may entail other actions like switch activation, NPC interaction, e.t.c.
  }

  protected virtual void Release(Entity entity) {
    if (base.isDefeated)
    return;

    // …
    if (entity is Monster monster) {
      monster.following  = null;
      monster.isDefeated = true;

      base.followers.Remove(monster);

      if (0 == base.followers.Count) {
        foreach (UnityEngine.Transform transform in this.mountingTransforms) {
          if (null != transform)
          transform.localPosition -= Tamer.MountingPosition;
        }

        this.mountingTransforms.Clear();
      }
    }

    // NOTE (Lapys) ->> Other kinds of `Entity`s may entail other actions like switch de-activation, NPC interaction, e.t.c.
  }

  private void Start() {
    UnityEngine.Material?   hairMaterial        = Random(Assets.main.hairs.materials);
    UnityEngine.Material[]  hairMaterials       = System.Array.Empty<UnityEngine.Material>();
    UnityEngine.GameObject? hairMeshFabrication = Random(Assets.main.hairs.meshPrefabrications);

    /* … */
    static T? Random<T>(System.Collections.Generic.List<T> list) where T : UnityEngine.Object {
      int count = list.Count;
      int index = UnityEngine.Random.Range(0, count);

      // …
      for (int subindex = index; ; ) {
        if (list[subindex] is not null)
        return list[subindex];

        subindex = (subindex + 1) % count;

        if (index == subindex)
        return null;
      }
    }

    /* … ->> Hair */
    if (this.hairAutomatically && hairMeshFabrication is not null) {
      UnityEngine.Transform hairTransform = ((UnityEngine.GameObject) UnityEngine.Object.Instantiate(hairMeshFabrication, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity, this.transform)).transform;

      // …
      if (hairMaterial is null) {
        hairMaterial       = new(UnityEngine.Shader.Find("Standard") ?? UnityEngine.Shader.Find("Diffuse"));
        hairMaterial.name  = "human-hair";
        hairMaterial.color = Game.Randomizer.Next(5) switch {
          0 => UnityEngine.Color.HSVToRGB(UnityEngine.Random.Range(18.00f, 30.00f) / 360.0f, UnityEngine.Random.Range(0.50f, 0.75f), UnityEngine.Random.Range(0.10f, 0.25f)), // ->> Black Brown
          1 => UnityEngine.Color.HSVToRGB(UnityEngine.Random.Range(30.00f, 60.00f) / 360.0f, UnityEngine.Random.Range(0.10f, 0.30f), UnityEngine.Random.Range(0.85f, 1.00f)), // ->> Light Blonde
          2 => UnityEngine.Color.HSVToRGB(UnityEngine.Random.Range(20.00f, 35.00f) / 360.0f, UnityEngine.Random.Range(0.50f, 0.75f), UnityEngine.Random.Range(0.20f, 0.35f)), // ->> Medium Brown
          3 => UnityEngine.Color.HSVToRGB(UnityEngine.Random.Range(5.00f,  20.00f) / 360.0f, UnityEngine.Random.Range(0.60f, 1.00f), UnityEngine.Random.Range(0.50f, 0.75f)), // ->> Red Blonde
          4 => UnityEngine.Color.HSVToRGB(UnityEngine.Random.Range(25.00f, 40.00f) / 360.0f, UnityEngine.Random.Range(0.40f, 0.60f), UnityEngine.Random.Range(0.60f, 0.80f)), // ->> Warm Blonde
          _ => UnityEngine.Random.ColorHSV()
        };
      }

      hairMaterials      = Assets.main.outlineAutomatically && Assets.main.outlineMaterial is not null ? new UnityEngine.Material[] {hairMaterial, Assets.main.outlineMaterial} : new UnityEngine.Material[] {hairMaterial};
      hairTransform.name = "Hair";

      hairTransform.SetLocalPositionAndRotation(UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity);
      this.transform.ForEach<UnityEngine.Renderer>(renderer => {
        UnityEngine.Material[] materials          = renderer.sharedMaterials;
        bool                   materialsIsUpdated = hairTransform.IsChildOf(this.transform);

        // …
        if (!materialsIsUpdated) {
          for (uint index = (uint) materials.Length; 0u != index--; )
          if (materials[index].name.TrimStart().StartsWith("human-hair", System.StringComparison.OrdinalIgnoreCase)) {
            materials[index]   = hairMaterial;
            materialsIsUpdated = true;
          }

          if (Assets.main.outlineAutomatically && Assets.main.outlineMaterial is not null) {
            UnityEngine.Material[] submaterials = new UnityEngine.Material[materials.Length + 1];

            // …
            materials.CopyTo(submaterials, 0);

            submaterials[materials.Length] = Assets.main.outlineMaterial;
            materials                      = submaterials;
          }
        }

        if      (materialsIsUpdated) renderer.sharedMaterials = materials;
        else if (this.transform.GetComponent<UnityEngine.MeshFilter>() is UnityEngine.MeshFilter meshFilter && null != meshFilter) {
          string name = meshFilter.name.Trim();

          if (
            string.Equals(name, "brow",  System.StringComparison.OrdinalIgnoreCase) ||
            string.Equals(name, "brows", System.StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith("Brow", System.StringComparison.OrdinalIgnoreCase)
          ) renderer.sharedMaterials = hairMaterials;
        }
      });
    }
  }

  protected override void Update() {
    base.Update();

    if (Game.IsPaused || this.isDefeated)
    return;

    // … ->> Bullet
    foreach (Bullet bullet in this.bullets)
    if (null != bullet && !bullet.isHit) {
      bullet.rigidBody.AddForce    (bullet.shootDirection * this.shoot.speed, UnityEngine.ForceMode.Impulse);
      bullet.rigidBody.MoveRotation(UnityEngine.Quaternion.Euler(
        UnityEngine.Vector3.Scale(UnityEngine.Vector3.forward + UnityEngine.Vector3.right, bullet.rigidBody.rotation.eulerAngles) +                       // ->> Remove Y-axis orientation
        UnityEngine.Vector3.Scale(UnityEngine.Vector3.up, UnityEngine.Quaternion.LookRotation(bullet.shootDirection, UnityEngine.Vector3.up).eulerAngles) // ->> Apply  Y-axis orientation
      ));
    }
  }
}
