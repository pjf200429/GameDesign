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
    /// ��ʼ���ӵ��Ļ����������� RangedAttack.Attack() ���á�
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

        // ����β�����ӣ�����еĻ���
        ps = GetComponentInChildren<ParticleSystem>();
        ps?.Play();
    }

    private void Update()
    {
        // �ӵ��ɹ������̺��Զ�����
        if (Vector3.Distance(startPosition, transform.position) >= maxRange)
        {
            // ���� ��β���� ��Ч
           
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �����뷢������������ײ
        if (other.gameObject == owner) return;

        int otherLayerMask = 1 << other.gameObject.layer;

        // ײ�������ˡ��㣬����˺�������������Ч
        if ((otherLayerMask & targetLayers) != 0)
        {
            if (other.TryGetComponent<IDamageable>(out var dmgable))
            {
                dmgable.TakeDamage(damage);
            }

            SpawnHitEffect();
            Destroy(gameObject);
        }
        // ײ���������ٲ㡱��ֱ�Ӳ�����Ч������
        else if ((otherLayerMask & destroyLayers) != 0)
        {
       
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �ڵ�ǰ transform.position ���� hitEffectPrefab�����ֶ� Play()��
    /// </summary>
    private void SpawnHitEffect()
    {
        if (hitEffectPrefab == null)
        {
            Debug.LogWarning("[ProjectileController] hitEffectPrefab Ϊ�գ��޷�������Ч��");
            return;
        }

        // �����־��ȷ�ϴ���� hitEffectPrefab ����
        Debug.Log($"[ProjectileController] ׼��ʵ����������Ч��{hitEffectPrefab.name}��λ�ã�{transform.position}");

        // 1) ʵ����������Ч Prefab
        GameObject fx = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        if (fx == null)
        {
            Debug.LogError("[ProjectileController] Instantiate ���� null����Чû�б�������");
            return;
        }

        // 2) �ֶ�Ѱ�Ҳ��������� ParticleSystem
        ParticleSystem[] systems = fx.GetComponentsInChildren<ParticleSystem>();
        if (systems.Length == 0)
        {
            Debug.LogWarning($"[ProjectileController] {hitEffectPrefab.name} ��û���ҵ��κ� ParticleSystem��");
        }
        foreach (var particle in systems)
        {
            // �������ϵͳû���Զ����ţ�ǿ�Ƶ��� Play()
            if (!particle.isPlaying)
            {
                particle.Play();
                Debug.Log($"[ProjectileController] �ֶ����� Play()���� {particle.gameObject.name} �ϲ������ӡ�");
            }
        }

        // 3) �������
        Debug.Log($"[ProjectileController] ��ʵ����������������Ч��{hitEffectPrefab.name}");

        // 4) �ӳ�������Чʵ��
        Destroy(fx, hitEffectDuration);
    }

}
