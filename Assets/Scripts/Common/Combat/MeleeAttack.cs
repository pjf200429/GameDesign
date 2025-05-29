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
        // ����������
        Vector2 center = (Vector2)attackOrigin.position + offset;
        // ��ײ���
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, range, targetLayers);
        foreach (var hit in hits)
        {
            // ��Ѫ
            if (hit.TryGetComponent<IDamageable>(out var target))
                target.TakeDamage(damage);

            // ����
            if (hit.TryGetComponent<Rigidbody2D>(out var rb))
            {
                Vector2 dir = ((Vector2)hit.transform.position - center).normalized;
                float massFactor = Mathf.Clamp(rb.mass, 1f, 5f);
                float force = knockbackForce / massFactor;
                rb.AddForce(dir * force, ForceMode2D.Impulse);

                // �������ˮƽ�ٶ�
                if (Mathf.Abs(rb.velocity.x) > 10f)
                    rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * 10f, rb.velocity.y);
            }
        }
    }
}
