// MeleeAttack.cs
using UnityEngine;

public class MeleeAttack : IWeaponStrategy
{
    private float range;
    private int baseDamage;
    private LayerMask targetLayers;
    private Vector2 offset;
    private float knockbackForce;


    public MeleeAttack(
        float range,
        int damage,
        LayerMask targetLayers,
        Vector2 offset,
        float knockbackForce)
    {
        this.range = range;
        this.baseDamage = damage;
        this.targetLayers = targetLayers;
        this.offset = offset;
        this.knockbackForce = knockbackForce;
    }


    public void Attack(Transform attackOrigin, Vector3 targetPos)
    {
        // 1. 先从 attackOrigin（通常是 Player 的攻击挂点） 找到 PlayerAttributes，获取攻击倍率
        PlayerAttributes attrs = attackOrigin.GetComponentInParent<PlayerAttributes>();
        float multiplier = attrs != null ? attrs.AttackMultiplier : 1f;

        // 2. 计算最终伤害 = baseDamage × multiplier（向下取整为整数）
        int finalDamage = Mathf.FloorToInt(baseDamage * multiplier);

        // 3. 计算打击中心
        Vector2 center = (Vector2)attackOrigin.position + offset;

        // 4. 碰撞检测：在 center 半径为 range 的圆内检测所有属于 targetLayers 的 collider
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, range, targetLayers);
        foreach (var hit in hits)
        {
            // —— 对命中的每一个 Collider2D 执行“造成伤害 + 击退” —— 

            // 4a. 扣血：如果命中对象实现了 IDamageable，就调用它的 TakeDamage(finalDamage)
            if (hit.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(finalDamage);
            }

            // 4b. 击退：如果命中对象有刚体，就给它一个击退冲量
            if (hit.TryGetComponent<Rigidbody2D>(out var rb))
            {
                Vector2 dir = ((Vector2)hit.transform.position - center).normalized;
                float massFactor = Mathf.Clamp(rb.mass, 1f, 5f);
                float force = knockbackForce / massFactor;
                rb.AddForce(dir * force, ForceMode2D.Impulse);

                // 限制最大水平速度，避免击退过猛
                if (Mathf.Abs(rb.velocity.x) > 10f)
                {
                    rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * 10f, rb.velocity.y);
                }
            }
        }
    }
}
