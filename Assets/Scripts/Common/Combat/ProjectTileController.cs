using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ProjectileController : MonoBehaviour
{
    [Header("Destroy Layers (collide on these layers to destroy the projectile without damage)")]
    public LayerMask destroyLayers;

    [Header("Target Layers (collide on these layers to deal damage and destroy the projectile)")]
    public LayerMask targetLayers;

    private float damage;
    private float speed;
    private float maxRange;
    private GameObject hitEffectPrefab;
    private float hitEffectDuration;
    private GameObject owner;

    private Vector3 startPosition;
    private Rigidbody2D rb;
    private ParticleSystem ps;

   
    public void Initialize(
        float damage,
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

        ps = GetComponentInChildren<ParticleSystem>();
        ps?.Play();
    }

    private void Update()
    {
  
        if (Vector3.Distance(startPosition, transform.position) >= maxRange)
        {
   
           
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject == owner) return;

        int otherLayerMask = 1 << other.gameObject.layer;

  
        if ((otherLayerMask & targetLayers) != 0)
        {
            if (other.TryGetComponent<IDamageable>(out var dmgable))
            {
                dmgable.TakeDamage((int)damage);
            }

            SpawnHitEffect();
            Destroy(gameObject);
        }

        else if ((otherLayerMask & destroyLayers) != 0)
        {
       
            Destroy(gameObject);
        }
    }


    private void SpawnHitEffect()
    {
        if (hitEffectPrefab == null)
        {
            Debug.LogWarning("[ProjectileController] hitEffectPrefab 为空，无法播放特效。");
            return;
        }

      

        GameObject fx = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        if (fx == null)
        {
            Debug.LogError("[ProjectileController] Instantiate 返回 null，特效没有被创建。");
            return;
        }

        ParticleSystem[] systems = fx.GetComponentsInChildren<ParticleSystem>();
        if (systems.Length == 0)
        {
            Debug.LogWarning($"[ProjectileController] {hitEffectPrefab.name} 上没有找到任何 ParticleSystem。");
        }
        foreach (var particle in systems)
        {
   
            if (!particle.isPlaying)
            {
                particle.Play();
          
            }
        }

        Destroy(fx, hitEffectDuration);
    }

}
