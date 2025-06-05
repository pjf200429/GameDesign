using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;            // Origin for melee hit detection or ranged projectile spawn
    public LayerMask enemyLayers;            // Layers that can be hit by melee or ranged attacks

    [Header("Animation & Effects")]
    public SPUM_Prefabs spum;                // Animation override controller
    public AttackEffectController effectController;

    private IWeaponStrategy currentWeapon;   // Either a MeleeAttack or RangedAttack
    private float attackCooldown = 0.5f;
    private float lastAttackTime = -999f;


    private Vector3 lastMouseWorldPos;
    void Start()
    {
        // Component validation
        if (spum == null)
            Debug.LogError("[PlayerCombat] spum not assigned!", this);
        if (effectController == null)
            Debug.LogError("[PlayerCombat] effectController not assigned!", this);
        if (attackPoint == null)
            Debug.LogError("[PlayerCombat] attackPoint not assigned!", this);

        spum.OverrideControllerInit();

        // Example: Equip a default weapon ("Sword01")
        var item = ItemDatabase.Instance.CreateItem("Sword01") as EquipmentItem;
        EquipWeapon(item);
    }

    /// <summary>
    /// Equips an EquipmentItem. Sets up animation clips, weapon strategy, and effect controller.
    /// </summary>
    public void EquipWeapon(EquipmentItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("[PlayerCombat] EquipWeapon received null, skipping.");
            return;
        }

        var data = item.Data;

        // MELEE WEAPON BRANCH
        if (data is MeleeWeaponData meleeData)
        {
            Debug.Log($"[PlayerCombat] Equipping Melee Weapon: {meleeData.EquipmentID}");

            // 1) Switch to melee attack animation
            spum.ATTACK_List.Clear();
            spum.ATTACK_List.Add(meleeData.AttackAnimation);
            spum.StateAnimationPairs["ATTACK"] = spum.ATTACK_List;

            // 2) Set up melee attack logic
            currentWeapon = new MeleeAttack(
                meleeData.AttackRange,
                meleeData.Damage,
                enemyLayers,
                meleeData.Offset,
                meleeData.KnockbackForce
            );

            // 3) Update the effect controller to use this melee weapon
            effectController.SetCurrentWeapon(item);

            // 4) Swap the sprite at R_Weapon node to show the new weapon icon
            Transform rootTransform = spum.gameObject.transform;
            Transform rWeaponTf = rootTransform.Find(
                "UnitRoot/Root/BodySet/P_Body/ArmSet/ArmR/P_RArm/P_Weapon/R_Weapon"
            );
            if (rWeaponTf != null)
            {
                var sr = rWeaponTf.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sprite = item.Icon;
                else
                    Debug.LogWarning("[PlayerCombat] Found R_Weapon but no SpriteRenderer.");
            }
            else
            {
                Debug.LogWarning("[PlayerCombat] Could not find R_Weapon node; path may be incorrect.");
            }
        }
        // RANGED WEAPON BRANCH
        else if (data is RangedWeaponData rangedData)
        {
            Debug.Log($"[PlayerCombat] Equipping Ranged Weapon: {rangedData.EquipmentID}");

            // 1) Switch to ranged attack animation (if provided)
            spum.ATTACK_List.Clear();
            spum.ATTACK_List.Add(rangedData.AttackAnimation);
            spum.StateAnimationPairs["ATTACK"] = spum.ATTACK_List;

            // 2) Set up ranged attack logic
            currentWeapon = new RangedAttack(
                rangedData.ProjectilePrefab,    // Particle bullet prefab
                rangedData.HitEffectPrefab,     // Hit effect prefab (can be null)
                rangedData.Damage,              // Base damage
                rangedData.ProjectileSpeed,     // Projectile speed
                rangedData.MaxRange,            // Maximum travel distance
                enemyLayers,                    // Layers it can hit (e.g. Enemy)
                rangedData.HitEffectDuration    // How long to keep hit effect alive
            );

            // 3) Swap the sprite at R_Weapon node to show the ranged weapon icon
            Transform rootTransform = spum.gameObject.transform;
            Transform rWeaponTf = rootTransform.Find(
                "UnitRoot/Root/BodySet/P_Body/ArmSet/ArmR/P_RArm/P_Weapon/R_Weapon"
            );
            if (rWeaponTf != null)
            {
                var sr = rWeaponTf.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sprite = item.Icon;
                else
                    Debug.LogWarning("[PlayerCombat] Found R_Weapon but no SpriteRenderer.");
            }
            else
            {
                Debug.LogWarning("[PlayerCombat] Could not find R_Weapon node; path may be incorrect.");
            }
        }
        // ARMOR BRANCH
        else if (data is ArmorData armorData)
        {
            Debug.Log($"[PlayerCombat] Equipping Armor: {armorData.EquipmentID}");

            // 1) Update player defense attribute
            var attrs = GetComponentInParent<PlayerAttributes>();
            if (attrs != null)
            {
                Debug.Log($"[PlayerCombat] Calling SetArmorDefense({armorData.DefenseValue})");
                attrs.SetArmorDefense(armorData.DefenseValue);
            }

            // 2) Swap the BodyArmor sprite
            Transform rootTransform = spum.gameObject.transform;
            Transform bodyArmorTf = rootTransform.Find(
                "UnitRoot/Root/BodySet/P_Body/Body/P_ArmorBody/BodyArmor"
            );
            if (bodyArmorTf != null)
            {
                var sr = bodyArmorTf.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sprite = armorData.Icon;
                else
                    Debug.LogWarning("[PlayerCombat] Found BodyArmor but no SpriteRenderer.");
            }
            else
            {
                Debug.LogWarning("[PlayerCombat] Could not find BodyArmor node; path may be incorrect.");
            }

            currentWeapon = null;
        }
        // HELMET BRANCH
        else if (data is HelmetData helmetData)
        {
            Debug.Log($"[PlayerCombat] Equipping Helmet: {helmetData.EquipmentID}");

            // 1) Update player helmet defense attribute
            var attrs = GetComponentInParent<PlayerAttributes>();
            if (attrs != null)
            {
                Debug.Log($"[PlayerCombat] Calling SetHelmetDefense({helmetData.DefenseValue})");
                attrs.SetHelmetDefense(helmetData.DefenseValue);
            }

            // 2) Swap the Helmet sprite
            Transform rootTransform = spum.gameObject.transform;
            Transform helmetTf = rootTransform.Find(
                "UnitRoot/Root/BodySet/P_Body/HeadSet/P_Head/P_Helmet/11_Helmet1"
            );
            if (helmetTf != null)
            {
                var sr = helmetTf.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sprite = helmetData.Icon;
                else
                    Debug.LogWarning("[PlayerCombat] Found 11_Helmet1 but no SpriteRenderer.");
            }
            else
            {
                Debug.LogWarning("[PlayerCombat] Could not find 11_Helmet1 node; path may be incorrect.");
            }

            // Equipping a helmet does not change currentWeapon
        }
        else
        {
            Debug.LogWarning($"[PlayerCombat] Unsupported equipment type: {data.GetType().Name}");
        }
    }

    void Update()
    {
        // Prevent attacking again until cooldown has elapsed
        if (Time.time - lastAttackTime < attackCooldown) return;
        if (!Input.GetMouseButtonDown(0)) return;

        lastAttackTime = Time.time;

        // 1) 先把鼠标位置从屏幕坐标转为世界坐标
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        worldPos.z = 0f;
        lastMouseWorldPos = worldPos;
        Debug.Log("[PlayerCombat] Playing ATTACK animation");
        spum.PlayAnimation(PlayerState.ATTACK, 0);


    }

    /// <summary>
    /// Called by an Animation Event at the hit frame (optional alternative to coroutine).
    /// Make sure the event is named exactly "OnAttackHitEvent".
    /// </summary>
    public void OnAttackHitEvent()
    {
        // 2) Play melee-specific effect if this is a melee weapon
        if (currentWeapon is MeleeAttack)
        {
            if (effectController != null)
            {
                effectController.PlayAttackEffect();
                Debug.Log("[PlayerCombat] PlayAttackEffect executed");
            }
            else
            {
                Debug.LogWarning("[PlayerCombat] effectController is null; cannot play effect");
            }
        }

        // 3) Execute the attack logic (either melee or ranged)
        if (currentWeapon != null)
        {
           
            currentWeapon.Attack(attackPoint, lastMouseWorldPos);
            Debug.Log("[PlayerCombat] currentWeapon.Attack executed");
        }
        else
        {
            Debug.LogWarning("[PlayerCombat] currentWeapon is null; skipping attack logic");
        }
    }
}
