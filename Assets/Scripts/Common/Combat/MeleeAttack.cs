using UnityEngine;

public class MeleeAttack : IWeaponStrategy
{
    private float range;
    private int damage;
    private LayerMask targetLayers;
    private Vector2 offset;

    private GameObject hitEffectPrefab;
    private AudioClip attackSound;
    private AudioSource audioSource;

    private float knockbackForce;
    private bool isFacingRight = true; // 记录玩家朝向

    public MeleeAttack(float range, int damage, LayerMask targetLayers, Vector2 offset = default,
                       GameObject hitEffectPrefab = null, AudioClip attackSound = null, AudioSource audioSource = null,
                       float knockbackForce = 0f)
    {
        this.range = range;
        this.damage = damage;
        this.targetLayers = targetLayers;
        this.offset = offset;

        this.hitEffectPrefab = hitEffectPrefab;
        this.attackSound = attackSound;
        this.audioSource = audioSource;
        this.knockbackForce = knockbackForce;
    }

    public void SetFacingDirection(bool facingRight)
    {
        isFacingRight = facingRight;
    }

    public void Attack(Transform attackOrigin)
    {
        Vector2 center = (Vector2)attackOrigin.position + offset;

        if (hitEffectPrefab != null)
        {
            GameObject effect = GameObject.Instantiate(hitEffectPrefab, center, Quaternion.identity);

            Vector3 scale = effect.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (isFacingRight ? 1f : -1f);
            effect.transform.localScale = scale;
        }

        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, range, targetLayers);

        foreach (var hit in hits)
        {
            IDamageable target = hit.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 hitDirection = ((Vector2)hit.transform.position - center).normalized;

                // 模拟真实世界击退逻辑：考虑质量并限制最大速度
                float massFactor = Mathf.Clamp(rb.mass, 1f, 5f);
                float adjustedForce = knockbackForce / massFactor;

                rb.AddForce(hitDirection * adjustedForce, ForceMode2D.Impulse);

                // 限制最大水平击退速度（避免击飞过远）
                if (Mathf.Abs(rb.velocity.x) > 10f)
                {
                    rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * 10f, rb.velocity.y);
                }
            }
        }
    }
}



