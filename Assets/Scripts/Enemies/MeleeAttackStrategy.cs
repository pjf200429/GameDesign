using UnityEngine;


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

 
    public void SetFacingDirection(bool facingRight)
    {
        _isFacingRight = facingRight;
    }

    
    public void Attack(Transform attackPoint, Vector3 destination)
    {
     
        Vector2 localOffset = new Vector2(
            _offset.x * (_isFacingRight ? 1f : -1f),
            _offset.y
        );
        Vector2 center = (Vector2)attackPoint.position + localOffset;

     
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, _range, _targetLayers);
        foreach (var hit in hits)
        {
   
            float dx = hit.transform.position.x - center.x;
            if (_isFacingRight ? dx < 0f : dx > 0f)
                continue;

     
            if (hit.TryGetComponent<IDamageable>(out var target))
                target.TakeDamage(_damage);


            if (_knockbackForce > 0f && hit.TryGetComponent<Rigidbody2D>(out var rb))
            {
                Vector2 dir = ((Vector2)hit.transform.position - center).normalized;
                float force = _knockbackForce / Mathf.Clamp(rb.mass, 1f, 5f);
                rb.AddForce(dir * force, ForceMode2D.Impulse);
            }
        }
    }
}
