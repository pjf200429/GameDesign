using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("����ж�")]
    public Transform attackPoint;
    public LayerMask enemyLayers;

    [Header("���� & ��Ч")]
    public SPUM_Prefabs spum;                    // �������ǿ�����
    public AttackEffectController effectController;

    private IWeaponStrategy currentWeapon;
    private float attackCooldown = 0.5f;
    private float lastAttackTime = -999f;
    private bool isFacingRight => transform.localScale.x > 0;

    void Start()
    {
        // ��Ҫ������
        if (spum == null)
            Debug.LogError("[PlayerCombat] spum δ�󶨣�", this);
        if (effectController == null)
            Debug.LogError("[PlayerCombat] effectController δ�󶨣�", this);

        spum.OverrideControllerInit();
        // ʾ����Ĭ��װ��һ�� ID Ϊ "Sword01" ������
        var item = ItemDatabase.Instance.CreateItem("Sword01") as EquipmentItem;
        EquipWeapon(item);
    }

    /// <summary>
    /// װ�����������¶��������б�������ԡ���Ч������
    /// </summary>
    public void EquipWeapon(EquipmentItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("[PlayerCombat] EquipWeapon �յ� null������");
            return;
        }

        WeaponData data = item.WeaponData;
        Debug.Log($"[PlayerCombat] װ������: {data.WeaponID}");

        // �л���������
        spum.ATTACK_List.Clear();
        spum.ATTACK_List.Add(data.AttackAnimation);
        spum.StateAnimationPairs["ATTACK"] = spum.ATTACK_List;

        // ���´���߼�
        currentWeapon = new MeleeAttack(
            data.AttackRange,
            data.Damage,
            enemyLayers,
            data.Offset,
            data.KnockbackForce
        );

        // ������Ч������
        effectController.SetCurrentWeapon(item);
    }

    void Update()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        if (!Input.GetKeyDown(KeyCode.J)) return;

        lastAttackTime = Time.time;
        Debug.Log("[PlayerCombat] ��ʼ���� ATTACK ����");
        spum.PlayAnimation(PlayerState.ATTACK, 0);

        // ����Э�̣�0.1s ���ӡ��ǰ���ŵļ���������ȷ���¼��Ƿ��������ȷ��Clip��
        StartCoroutine(CheckAttackAnimationState());
    }

    private IEnumerator CheckAttackAnimationState()
    {
        yield return new WaitForSeconds(0.1f);

        Animator anim = spum._anim;
        if (anim == null)
        {
            Debug.LogError("[PlayerCombat] Animator ���Ϊ�գ�");
            yield break;
        }

        var clipInfos = anim.GetCurrentAnimatorClipInfo(0);
        if (clipInfos.Length > 0)
        {
            //Debug.Log($"[PlayerCombat] ��ǰ���ż���: {clipInfos[0].clip.name}, ������ʱ��: {anim.GetCurrentAnimatorStateInfo(0).normalizedTime:F2}");
        }
        else
        {
            Debug.LogWarning("[PlayerCombat] δ��⵽�κβ����еļ�����");
        }
    }

    /// <summary>
    /// Animation Event ���ã�����ڶ�Ӧ�� AnimationClip �ϹҴ� Event��
    /// �������� ��OnAttackHitEvent��
    /// </summary>
    public void OnAttackHitEvent()
    {
      //  Debug.Log($"[PlayerCombat] OnAttackHitEvent fired! FacingRight={isFacingRight}, currentWeapon={(currentWeapon == null ? "null" : currentWeapon.GetType().Name)}");

        // 1) ���³���
        if (currentWeapon is MeleeAttack melee)
        {
            melee.SetFacingDirection(isFacingRight);
            Debug.Log("[PlayerCombat] SetFacingDirection ִ�����");
        }

        // 2) ������Ч/��Ч
        if (effectController != null)
        {
            effectController.PlayAttackEffect();
            Debug.Log("[PlayerCombat] PlayAttackEffect �������");
        }
        else
        {
            Debug.LogWarning("[PlayerCombat] effectController �� null���޷�������Ч");
        }

        // 3) ִ�д���߼�
        if (currentWeapon != null)
        {
            currentWeapon.Attack(attackPoint);
            Debug.Log("[PlayerCombat] currentWeapon.Attack �������");
        }
        else
        {
            Debug.LogWarning("[PlayerCombat] currentWeapon �� null�����������߼�");
        }
    }
}
