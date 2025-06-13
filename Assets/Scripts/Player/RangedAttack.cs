using UnityEngine;

/// <summary>
/// Implements IWeaponStrategy for ranged weapons.
/// Instantiates a projectile prefab and initializes its parameters.
/// 由此类整体负责：决定子弹朝向、实例化并为它设置速度。
/// </summary>
public class RangedAttack : IWeaponStrategy
{
    private GameObject projectilePrefab;   // Prefab for the projectile GameObject
    private GameObject hitEffectPrefab;    // Prefab for the hit effect (can be null)
    private float baseDamage;                // Base damage dealt by the projectile
    private float projectileSpeed;         // Speed at which the projectile moves
    private float maxRange;                // Maximum travel distance of the projectile
    private LayerMask targetLayers;        // Layers that the projectile can hit
    private float hitEffectDuration;       // Duration to keep the hit effect alive
    

    /// <summary>
    /// Constructor: assign all necessary projectile parameters.
    /// </summary>
    /// <param name="projectilePrefab">Prefab for the projectile GameObject.</param>
    /// <param name="hitEffectPrefab">Prefab for the hit effect GameObject.</param>
    /// <param name="damage">Damage dealt on impact.</param>
    /// <param name="projectileSpeed">Speed at which the projectile moves.</param>
    /// <param name="maxRange">Maximum distance the projectile can travel.</param>
    /// <param name="targetLayers">LayerMask specifying valid hit targets.</param>
    /// <param name="hitEffectDuration">Duration to keep the hit effect alive.</param>
    public RangedAttack(
        GameObject projectilePrefab,
        GameObject hitEffectPrefab,
        int damage,
        float projectileSpeed,
        float maxRange,
        LayerMask targetLayers,
        float hitEffectDuration)
    {
        this.projectilePrefab = projectilePrefab;
        this.hitEffectPrefab = hitEffectPrefab;
        this.baseDamage = damage;
        this.projectileSpeed = projectileSpeed;
        this.maxRange = maxRange;
        this.targetLayers = targetLayers;
        this.hitEffectDuration = hitEffectDuration;

    }

  

    /// <summary>
    /// Executes a ranged attack:
    ///   1) 根据玩家朝向决定方向 dir
    ///   2) Instantiate 子弹 Prefab
    ///   3) 为子弹实例设置 localScale 翻转（保持贴图朝向），并直接给它的 Rigidbody2D.velocity 赋值
    ///   4) 调用 ProjectileController.Initialize(...) 初始化其参数
    /// </summary>
    /// <param name="attackOrigin">
    /// Transform that determines spawn position (e.g., a firePoint on the player).
    /// </param>
    public void Attack(Transform attackOrigin, Vector3 targetPos)

    {
        PlayerAttributes attrs = attackOrigin.GetComponentInParent<PlayerAttributes>();
        float multiplier = attrs != null ? attrs.AttackMultiplier : 1f;
        baseDamage = baseDamage * multiplier;

        if (projectilePrefab == null)
        {
            Debug.LogWarning("[RangedAttack] projectilePrefab is null; cannot fire.");
            return;
        }

        // 1) 计算生成位置
        Vector3 spawnPos = attackOrigin.position;

        // 2) 计算从 spawnPos 指向 targetPos 的方向向量，并归一化
        Vector3 dirVector = (targetPos - spawnPos).normalized;
        if (dirVector == Vector3.zero)
        {
            // 如果目标就在挂点位置，默认为向右
            dirVector = Vector3.right;
        }


        // 3) 实例化子弹 Prefab，rotation 始终为 identity
        GameObject projGO = Object.Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        // 4) 根据 dirVector.x 翻转 localScale.x，让贴图朝向正确
        Vector3 localScale = projGO.transform.localScale;
        localScale.x = dirVector.x > 0f ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
        projGO.transform.localScale = localScale;

        // 5) 给子弹刚体赋速度：沿 dirVector 方向
        Rigidbody2D rb = projGO.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = dirVector * projectileSpeed;

        }
        else
        {
            Debug.LogWarning("[RangedAttack] 子弹实例缺少 Rigidbody2D 组件，无法设置速度。");
        }

    

        // 6) 初始化 ProjectileController，传入剩余参数
        var projCtrl = projGO.GetComponent<ProjectileController>();
        if (projCtrl != null)
        {
            projCtrl.Initialize(
                damage: baseDamage,
                speed: projectileSpeed,
                maxRange: maxRange,
                targetLayers: targetLayers,
                hitEffectPrefab: hitEffectPrefab,
                hitEffectDuration: hitEffectDuration,
                owner: attackOrigin.root.gameObject
            );
        }
        else
        {
            Debug.LogWarning("[RangedAttack] ProjectileController not found on the prefab.");
        }
    }
}
