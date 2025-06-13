using UnityEngine;

/// <summary>
/// Implements IWeaponStrategy for ranged weapons.
/// Instantiates a projectile prefab and initializes its parameters.
/// �ɴ������帺�𣺾����ӵ�����ʵ������Ϊ�������ٶȡ�
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
    ///   1) ������ҳ���������� dir
    ///   2) Instantiate �ӵ� Prefab
    ///   3) Ϊ�ӵ�ʵ������ localScale ��ת��������ͼ���򣩣���ֱ�Ӹ����� Rigidbody2D.velocity ��ֵ
    ///   4) ���� ProjectileController.Initialize(...) ��ʼ�������
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

        // 1) ��������λ��
        Vector3 spawnPos = attackOrigin.position;

        // 2) ����� spawnPos ָ�� targetPos �ķ�������������һ��
        Vector3 dirVector = (targetPos - spawnPos).normalized;
        if (dirVector == Vector3.zero)
        {
            // ���Ŀ����ڹҵ�λ�ã�Ĭ��Ϊ����
            dirVector = Vector3.right;
        }


        // 3) ʵ�����ӵ� Prefab��rotation ʼ��Ϊ identity
        GameObject projGO = Object.Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        // 4) ���� dirVector.x ��ת localScale.x������ͼ������ȷ
        Vector3 localScale = projGO.transform.localScale;
        localScale.x = dirVector.x > 0f ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
        projGO.transform.localScale = localScale;

        // 5) ���ӵ����帳�ٶȣ��� dirVector ����
        Rigidbody2D rb = projGO.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = dirVector * projectileSpeed;

        }
        else
        {
            Debug.LogWarning("[RangedAttack] �ӵ�ʵ��ȱ�� Rigidbody2D ������޷������ٶȡ�");
        }

    

        // 6) ��ʼ�� ProjectileController������ʣ�����
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
