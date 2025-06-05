using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ProjectileController : MonoBehaviour
{
    [Header("Destroy Layers (collide on these layers to destroy the projectile without damage)")]
    public LayerMask destroyLayers;

    [Header("Target Layers (collide on these layers to deal damage and destroy the projectile)")]
    public LayerMask targetLayers;

    private int damage;
    private float speed;
    private float maxRange;
    private GameObject hitEffectPrefab;
    private float hitEffectDuration;
    private GameObject owner;

    private Vector3 startPosition;
    private Rigidbody2D rb;
    private ParticleSystem ps;

    /// <summary>
    /// 初始化子弹的基础参数，由 RangedAttack.Attack() 调用。
    /// </summary>
    public void Initialize(
        int damage,
        float speed,
        float maxRange,
        LayerMask targetLayers,
        GameObject hitEffectPrefab,
        float hitEffectDuration,
        GameObject owner)
    {
        this.damage = damage;
        this.speed = speed;
        this.maxRange = maxRange;
        this.targetLayers = targetLayers;
        this.hitEffectPrefab = hitEffectPrefab;
        this.hitEffectDuration = hitEffectDuration;
        this.owner = owner;

        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();

        // 播放尾迹粒子（如果有的话）
        ps = GetComponentInChildren<ParticleSystem>();
        ps?.Play();
    }

    private void Update()
    {
        // 子弹飞过最大射程后自动销毁
        if (Vector3.Distance(startPosition, transform.position) >= maxRange)
        {
            // 播放 “尾声” 特效
           
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 避免与发射者自身发生碰撞
        if (other.gameObject == owner) return;

        int otherLayerMask = 1 << other.gameObject.layer;

        // 撞到“敌人”层，造成伤害并播放命中特效
        if ((otherLayerMask & targetLayers) != 0)
        {
            if (other.TryGetComponent<IDamageable>(out var dmgable))
            {
                dmgable.TakeDamage(damage);
            }

            SpawnHitEffect();
            Destroy(gameObject);
        }
        // 撞到“可销毁层”，直接播放特效后销毁
        else if ((otherLayerMask & destroyLayers) != 0)
        {
       
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 在当前 transform.position 生成 hitEffectPrefab，并手动 Play()。
    /// </summary>
    private void SpawnHitEffect()
    {
        if (hitEffectPrefab == null)
        {
            Debug.LogWarning("[ProjectileController] hitEffectPrefab 为空，无法播放特效。");
            return;
        }

        // 输出日志，确认传入的 hitEffectPrefab 名称
        Debug.Log($"[ProjectileController] 准备实例化命中特效：{hitEffectPrefab.name}，位置：{transform.position}");

        // 1) 实例化命中特效 Prefab
        GameObject fx = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        if (fx == null)
        {
            Debug.LogError("[ProjectileController] Instantiate 返回 null，特效没有被创建。");
            return;
        }

        // 2) 手动寻找并启动所有 ParticleSystem
        ParticleSystem[] systems = fx.GetComponentsInChildren<ParticleSystem>();
        if (systems.Length == 0)
        {
            Debug.LogWarning($"[ProjectileController] {hitEffectPrefab.name} 上没有找到任何 ParticleSystem。");
        }
        foreach (var particle in systems)
        {
            // 如果粒子系统没有自动播放，强制调用 Play()
            if (!particle.isPlaying)
            {
                particle.Play();
                Debug.Log($"[ProjectileController] 手动调用 Play()，在 {particle.gameObject.name} 上播放粒子。");
            }
        }

        // 3) 输出调试
        Debug.Log($"[ProjectileController] 已实例化并播放命中特效：{hitEffectPrefab.name}");

        // 4) 延迟销毁特效实例
        Destroy(fx, hitEffectDuration);
    }

}
