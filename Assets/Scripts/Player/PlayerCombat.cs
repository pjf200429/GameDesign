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

        // �õ�ͨ�õ� EquipmentData
        var data = item.Data;

        // �����������WeaponData����ִ��ԭ�е��л������߼�
        if (data is WeaponData weaponData)
        {
            Debug.Log($"[PlayerCombat] װ������: {weaponData.EquipmentID}");

            // �л���������
            spum.ATTACK_List.Clear();
            spum.ATTACK_List.Add(weaponData.AttackAnimation);
            spum.StateAnimationPairs["ATTACK"] = spum.ATTACK_List;

            // ���´���߼�
            currentWeapon = new MeleeAttack(
                weaponData.AttackRange,
                weaponData.Damage,
                enemyLayers,
                weaponData.Offset,
                weaponData.KnockbackForce
            );

            // ������Ч������
            effectController.SetCurrentWeapon(item);

            // ���� �� R_Weapon �ڵ�� Sprite ���� item.Icon ���� 
            Transform rootTransform = spum.gameObject.transform;
            Transform rWeaponTf = rootTransform.Find(
                "UnitRoot/Root/BodySet/P_Body/ArmSet/ArmR/P_RArm/P_Weapon/R_Weapon"
            );
            if (rWeaponTf != null)
            {
                var sr = rWeaponTf.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sprite = item.Icon;
                else
                    Debug.LogWarning("[PlayerCombat] �ҵ� R_Weapon ��û�� SpriteRenderer��");
            }
            else
            {
                Debug.LogWarning("[PlayerCombat] δ���ҵ� R_Weapon �ڵ㣬·�����ܲ���ȷ��");
            }
        }
        // ����ǻ��ף�ArmorData��
        else if (data is ArmorData armorData)
        {
            Debug.Log($"[PlayerCombat] װ������: {armorData.EquipmentID}");

            // 1) ������ҷ�������
            var attrs = GetComponentInParent<PlayerAttributes>();
            if (attrs != null)
            {
                Debug.Log($"[PlayerCombat] ׼�� SetArmorDefense({armorData.DefenseValue})");
                attrs.SetArmorDefense(armorData.DefenseValue);
            }

            // 2) �滻ģ���л��׵� Sprite
            Transform rootTransform = spum.gameObject.transform;
            Transform bodyArmorTf = rootTransform.Find(
                "UnitRoot/Root/BodySet/P_Body/Body/P_ArmorBody/BodyArmor"
            );
            if (bodyArmorTf != null)
            {
                var sr = bodyArmorTf.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sprite = armorData.Icon;
                else
                    Debug.LogWarning("[PlayerCombat] �ҵ� BodyArmor ��û�� SpriteRenderer��");
            }
            else
            {
                Debug.LogWarning("[PlayerCombat] δ���ҵ� BodyArmor �ڵ㣬·�����ܲ���ȷ��");
            }

        }
        // �����ͷ����HelmetData��
        else if (data is HelmetData helmetData)
        {
            Debug.Log($"[PlayerCombat] װ��ͷ��: {helmetData.EquipmentID}");

            // 1) �������ͷ����������
            var attrs = GetComponentInParent<PlayerAttributes>();
            if (attrs != null)
            {
                Debug.Log($"[PlayerCombat] ׼�� SetArmorDefense({helmetData.DefenseValue})");
                attrs.SetHelmetDefense(helmetData.DefenseValue);
            }

            // 2) �滻ģ����ͷ���� Sprite
            Transform rootTransform = spum.gameObject.transform;
            Transform helmetTf = rootTransform.Find(
                "UnitRoot/Root/BodySet/P_Body/HeadSet/P_Head/P_Helmet/11_Helmet1"
            );
            if (helmetTf != null)
            {
                var sr = helmetTf.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sprite = helmetData.Icon;
                else
                    Debug.LogWarning("[PlayerCombat] �ҵ� 11_Helmet1 ��û�� SpriteRenderer��");
            }
            else
            {
                Debug.LogWarning("[PlayerCombat] δ���ҵ� 11_Helmet1 �ڵ㣬·�����ܲ���ȷ��");
            }

            // ����ѡ�����ֻ��ͷ����Ӱ�������߼����򱣳� currentWeapon ����
        }
        else
        {
            Debug.LogWarning($"[PlayerCombat] ��֧�ֵ�װ������: {data.GetType().Name}");
        }
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
