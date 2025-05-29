using UnityEngine;

public class MeleeAttack : IWeaponStrategy
{
    private float range;
    private int damage;
    private LayerMask targetLayers;
    private Vector2 offset;
    private float knockbackForce;
    private bool isFacingRight = true;

    public MeleeAttack(
        float range,
        int damage,
        LayerMask targetLayers,
        Vector2 offset,
        float knockbackForce)
    {
        this.range = range;
        this.damage = damage;
        this.targetLayers = targetLayers;
        this.offset = offset;
        this.knockbackForce = knockbackForce;
    }

    public void SetFacingDirection(bool facingRight)
    {
        isFacingRight = facingRight;
    }

    public void Attack(Transform attackOrigin)
    {
        // 计算打击中心
        Vector2 center = (Vector2)attackOrigin.position + offset;
        // 碰撞检测
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, range, targetLayers);
        foreach (var hit in hits)
        {
            // 扣血
            if (hit.TryGetComponent<IDamageable>(out var target))
                target.TakeDamage(damage);

            // 击退
            if (hit.TryGetComponent<Rigidbody2D>(out var rb))
            {
                Vector2 dir = ((Vector2)hit.transform.position - center).normalized;
                float massFactor = Mathf.Clamp(rb.mass, 1f, 5f);
                float force = knockbackForce / massFactor;
                rb.AddForce(dir * force, ForceMode2D.Impulse);

                // 限制最大水平速度
                if (Mathf.Abs(rb.velocity.x) > 10f)
                    rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * 10f, rb.velocity.y);
            }
        }
    }
}
