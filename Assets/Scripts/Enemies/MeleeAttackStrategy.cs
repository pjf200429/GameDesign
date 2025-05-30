using UnityEngine;

/// <summary>
/// 近战攻击策略：在指定范围内检测 IDamageable 并造成伤害，只对面朝方向一侧生效，可选偏移与击退效果。
/// </summary>
public class MeleeAttackStrategy : IWeaponStrategy
{
    private readonly float _range;
    private readonly int _damage;
    private readonly LayerMask _targetLayers;
    private readonly Vector2 _offset;
    private readonly float _knockbackForce;
    private bool _isFacingRight = true;

    public MeleeAttackStrategy(
        float range,
        int damage,
        LayerMask targetLayers,
        Vector2 offset,
        float knockbackForce
    )
    {
        _range = range;
        _damage = damage;
        _targetLayers = targetLayers;
        _offset = offset;
        _knockbackForce = knockbackForce;
    }

    /// <summary>设置当前朝向（true=右，false=左）</summary>
    public void SetFacingDirection(bool facingRight)
    {
        _isFacingRight = facingRight;
    }

    /// <summary>
    /// 发起一次打击
    /// </summary>
    public void Attack(Transform attackPoint)
    {
        // 计算检测中心（考虑朝向偏移）
        Vector2 localOffset = new Vector2(
            _offset.x * (_isFacingRight ? 1f : -1f),
            _offset.y
        );
        Vector2 center = (Vector2)attackPoint.position + localOffset;

        // 圆形检测
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, _range, _targetLayers);
        foreach (var hit in hits)
        {
            // 只命中朝向一侧
            float dx = hit.transform.position.x - center.x;
            if (_isFacingRight ? dx < 0f : dx > 0f)
                continue;

            // 扣血
            if (hit.TryGetComponent<IDamageable>(out var target))
                target.TakeDamage(_damage);

            // 可选：击退
            if (_knockbackForce > 0f && hit.TryGetComponent<Rigidbody2D>(out var rb))
            {
                Vector2 dir = ((Vector2)hit.transform.position - center).normalized;
                float force = _knockbackForce / Mathf.Clamp(rb.mass, 1f, 5f);
                rb.AddForce(dir * force, ForceMode2D.Impulse);
            }
        }
    }
}
