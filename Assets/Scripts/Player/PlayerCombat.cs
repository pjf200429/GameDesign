using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("打击判定")]
    public Transform attackPoint;
    public LayerMask enemyLayers;

    [Header("动画 & 特效")]
    public SPUM_Prefabs spum;                    // 动画覆盖控制器
    public AttackEffectController effectController;

    private IWeaponStrategy currentWeapon;
    private float attackCooldown = 0.5f;
    private float lastAttackTime = -999f;
    private bool isFacingRight => transform.localScale.x > 0;

    void Start()
    {
        // 必要组件检查
        if (spum == null)
            Debug.LogError("[PlayerCombat] spum 未绑定！", this);
        if (effectController == null)
            Debug.LogError("[PlayerCombat] effectController 未绑定！", this);

        spum.OverrideControllerInit();
        // 示例：默认装备一把 ID 为 "Sword01" 的武器
        var item = ItemDatabase.Instance.CreateItem("Sword01") as EquipmentItem;
        EquipWeapon(item);
    }

    /// <summary>
    /// 装备武器：更新动画剪辑列表、打击策略、特效控制器
    /// </summary>
    public void EquipWeapon(EquipmentItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("[PlayerCombat] EquipWeapon 收到 null，跳过");
            return;
        }

        // 拿到通用的 EquipmentData
        var data = item.Data;

        // 如果是武器（WeaponData），执行原有的切换武器逻辑
        if (data is WeaponData weaponData)
        {
            Debug.Log($"[PlayerCombat] 装备武器: {weaponData.EquipmentID}");

            // 切换动画剪辑
            spum.ATTACK_List.Clear();
            spum.ATTACK_List.Add(weaponData.AttackAnimation);
            spum.StateAnimationPairs["ATTACK"] = spum.ATTACK_List;

            // 更新打击逻辑
            currentWeapon = new MeleeAttack(
                weaponData.AttackRange,
                weaponData.Damage,
                enemyLayers,
                weaponData.Offset,
                weaponData.KnockbackForce
            );

            // 更新特效控制器
            effectController.SetCurrentWeapon(item);

            // —— 把 R_Weapon 节点的 Sprite 换成 item.Icon —— 
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
                    Debug.LogWarning("[PlayerCombat] 找到 R_Weapon 但没有 SpriteRenderer。");
            }
            else
            {
                Debug.LogWarning("[PlayerCombat] 未能找到 R_Weapon 节点，路径可能不正确。");
            }
        }
        // 如果是护甲（ArmorData）
        else if (data is ArmorData armorData)
        {
            Debug.Log($"[PlayerCombat] 装备护甲: {armorData.EquipmentID}");

            // 1) 更新玩家防御属性
            var attrs = GetComponentInParent<PlayerAttributes>();
            if (attrs != null)
            {
                Debug.Log($"[PlayerCombat] 准备 SetArmorDefense({armorData.DefenseValue})");
                attrs.SetArmorDefense(armorData.DefenseValue);
            }

            // 2) 替换模型中护甲的 Sprite
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
                    Debug.LogWarning("[PlayerCombat] 找到 BodyArmor 但没有 SpriteRenderer。");
            }
            else
            {
                Debug.LogWarning("[PlayerCombat] 未能找到 BodyArmor 节点，路径可能不正确。");
            }

        }
        // 如果是头盔（HelmetData）
        else if (data is HelmetData helmetData)
        {
            Debug.Log($"[PlayerCombat] 装备头盔: {helmetData.EquipmentID}");

            // 1) 更新玩家头盔防御属性
            var attrs = GetComponentInParent<PlayerAttributes>();
            if (attrs != null)
            {
                Debug.Log($"[PlayerCombat] 准备 SetArmorDefense({helmetData.DefenseValue})");
                attrs.SetHelmetDefense(helmetData.DefenseValue);
            }

            // 2) 替换模型中头盔的 Sprite
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
                    Debug.LogWarning("[PlayerCombat] 找到 11_Helmet1 但没有 SpriteRenderer。");
            }
            else
            {
                Debug.LogWarning("[PlayerCombat] 未能找到 11_Helmet1 节点，路径可能不正确。");
            }

            // （可选）如果只戴头盔不影响武器逻辑，则保持 currentWeapon 不变
        }
        else
        {
            Debug.LogWarning($"[PlayerCombat] 不支持的装备类型: {data.GetType().Name}");
        }
    }



    void Update()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        if (!Input.GetKeyDown(KeyCode.J)) return;

        lastAttackTime = Time.time;
        Debug.Log("[PlayerCombat] 开始播放 ATTACK 动画");
        spum.PlayAnimation(PlayerState.ATTACK, 0);

        // 启动协程，0.1s 后打印当前播放的剪辑，方便确认事件是否挂在了正确的Clip上
        StartCoroutine(CheckAttackAnimationState());
    }

    private IEnumerator CheckAttackAnimationState()
    {
        yield return new WaitForSeconds(0.1f);

        Animator anim = spum._anim;
        if (anim == null)
        {
            Debug.LogError("[PlayerCombat] Animator 组件为空！");
            yield break;
        }

        var clipInfos = anim.GetCurrentAnimatorClipInfo(0);
        if (clipInfos.Length > 0)
        {
            //Debug.Log($"[PlayerCombat] 当前播放剪辑: {clipInfos[0].clip.name}, 正常化时间: {anim.GetCurrentAnimatorStateInfo(0).normalizedTime:F2}");
        }
        else
        {
            Debug.LogWarning("[PlayerCombat] 未检测到任何播放中的剪辑！");
        }
    }

    /// <summary>
    /// Animation Event 调用：务必在对应的 AnimationClip 上挂此 Event，
    /// 函数名填 “OnAttackHitEvent”
    /// </summary>
    public void OnAttackHitEvent()
    {
        // 1) 更新朝向
        if (currentWeapon is MeleeAttack melee)
        {
            melee.SetFacingDirection(isFacingRight);
            Debug.Log("[PlayerCombat] SetFacingDirection 执行完毕");
        }

        // 2) 播放特效/音效
        if (effectController != null)
        {
            effectController.PlayAttackEffect();
            Debug.Log("[PlayerCombat] PlayAttackEffect 调用完毕");
        }
        else
        {
            Debug.LogWarning("[PlayerCombat] effectController 是 null，无法播放特效");
        }

        // 3) 执行打击逻辑
        if (currentWeapon != null)
        {
            currentWeapon.Attack(attackPoint);
            Debug.Log("[PlayerCombat] currentWeapon.Attack 调用完毕");
        }
        else
        {
            Debug.LogWarning("[PlayerCombat] currentWeapon 是 null，跳过攻击逻辑");
        }
    }
}
