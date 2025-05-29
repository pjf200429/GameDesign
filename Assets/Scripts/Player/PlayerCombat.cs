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

        WeaponData data = item.WeaponData;
        Debug.Log($"[PlayerCombat] 装备武器: {data.WeaponID}");

        // 切换动画剪辑
        spum.ATTACK_List.Clear();
        spum.ATTACK_List.Add(data.AttackAnimation);
        spum.StateAnimationPairs["ATTACK"] = spum.ATTACK_List;

        // 更新打击逻辑
        currentWeapon = new MeleeAttack(
            data.AttackRange,
            data.Damage,
            enemyLayers,
            data.Offset,
            data.KnockbackForce
        );

        // 更新特效控制器
        effectController.SetCurrentWeapon(item);
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
      //  Debug.Log($"[PlayerCombat] OnAttackHitEvent fired! FacingRight={isFacingRight}, currentWeapon={(currentWeapon == null ? "null" : currentWeapon.GetType().Name)}");

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
