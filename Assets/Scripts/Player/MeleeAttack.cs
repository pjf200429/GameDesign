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
        // 1. �ȴ� attackOrigin��ͨ���� Player �Ĺ����ҵ㣩 �ҵ� PlayerAttributes����ȡ��������
        PlayerAttributes attrs = attackOrigin.GetComponentInParent<PlayerAttributes>();
        float multiplier = attrs != null ? attrs.AttackMultiplier : 1f;

        // 2. ���������˺� = baseDamage �� multiplier������ȡ��Ϊ������
        int finalDamage = Mathf.FloorToInt(baseDamage * multiplier);

        // 3. ����������
        Vector2 center = (Vector2)attackOrigin.position + offset;

        // 4. ��ײ��⣺�� center �뾶Ϊ range ��Բ�ڼ���������� targetLayers �� collider
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, range, targetLayers);
        foreach (var hit in hits)
        {
            // ���� �����е�ÿһ�� Collider2D ִ�С�����˺� + ���ˡ� ���� 

            // 4a. ��Ѫ��������ж���ʵ���� IDamageable���͵������� TakeDamage(finalDamage)
            if (hit.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(finalDamage);
            }

            // 4b. ���ˣ�������ж����и��壬�͸���һ�����˳���
            if (hit.TryGetComponent<Rigidbody2D>(out var rb))
            {
                Vector2 dir = ((Vector2)hit.transform.position - center).normalized;
                float massFactor = Mathf.Clamp(rb.mass, 1f, 5f);
                float force = knockbackForce / massFactor;
                rb.AddForce(dir * force, ForceMode2D.Impulse);

                // �������ˮƽ�ٶȣ�������˹���
                if (Mathf.Abs(rb.velocity.x) > 10f)
                {
                    rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * 10f, rb.velocity.y);
                }
            }
        }
    }
}
