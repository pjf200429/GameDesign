
using Unity.VisualScripting;
using UnityEngine;


public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;
    public float meleeRange = 1.0f;
    public int meleeDamage = 20;
    public LayerMask enemyLayers;

    public GameObject hitEffectPrefab;
    public AudioClip attackSound;
    public Vector2 attackOffset = new Vector2(1f, 0f);
    public float knockbackForce = 10f;

    private IWeaponStrategy currentWeapon;
    private AudioSource audioSource;

    private float attackCooldown = 0.5f;
    private float lastAttackTime = -999f;

    private bool isFacingRight = true; //  新增字段：记录当前朝向

    private SPUM_Prefabs spum;

    public AnimationClip attackClip;  // 在 Inspector 拖入 ATTACK.anim




    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spum = GetComponent<SPUM_Prefabs>();

        //  只在 PlayerCombat.cs 中设置动画 clip 列表
        if (spum != null)
        {
            spum.OverrideControllerInit();  // 确保 AnimatorOverrideController 初始化

            if (attackClip != null)
            {
                spum.ATTACK_List.Clear();
                spum.ATTACK_List.Add(attackClip);
                spum.StateAnimationPairs["ATTACK"] = spum.ATTACK_List;
              
            }
            else
            {
                Debug.LogWarning("[PlayerCombat] 请在 Inspector 中拖入 attackClip（ATTACK 动画）！");
            }
        }

        currentWeapon = new MeleeAttack(
            meleeRange,
            meleeDamage,
            enemyLayers,
            attackOffset,
            hitEffectPrefab,
            attackSound,
            audioSource,
            knockbackForce
        );
    }



    void Update()
    {
        isFacingRight = transform.localScale.x > 0;

        if (Input.GetKeyDown(KeyCode.J) && Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            if (currentWeapon is MeleeAttack melee)
            {
                melee.SetFacingDirection(isFacingRight);
            }

            currentWeapon?.Attack(attackPoint);

            // 播放攻击动画（动态切换 ATTACK 状态绑定的 Clip）
            if (spum != null && spum.StateAnimationPairs.ContainsKey("ATTACK") && spum.StateAnimationPairs["ATTACK"].Count > 0)
            {
                Debug.Log("<color=cyan>[PlayerCombat]</color> 播放 ATTACK 动画 clip: " + spum.StateAnimationPairs["ATTACK"][0].name);
                spum.PlayAnimation(PlayerState.ATTACK, 0);

                StartCoroutine(CheckAttackAnimationState());
            }
            else
            {
                Debug.LogWarning("<color=yellow>[PlayerCombat]</color> ATTACK 动画未设置或找不到");
            }
        }
    }

    private System.Collections.IEnumerator CheckAttackAnimationState()
    {
        yield return new WaitForSeconds(0.1f);

        Animator anim = spum._anim;

        if (anim == null)
        {
            Debug.LogError("[Animator] 未找到 Animator 组件！");
            yield break;
        }

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        var clipInfo = anim.GetCurrentAnimatorClipInfo(0);

        Debug.Log($"<color=green>[Animator]</color> 当前状态是否是 ATTACK: {stateInfo.IsName("Base Layer.ATTACK")}, 状态名 Hash: {stateInfo.fullPathHash}");

        if (clipInfo.Length > 0)
        {
            Debug.Log($"<color=green>[Animator]</color> 当前播放 Clip: {clipInfo[0].clip.name}, 正常化时间: {stateInfo.normalizedTime}");
        }
        else
        {
            Debug.LogWarning("<color=orange>[Animator]</color> 当前没有播放任何动画！");
        }
    }


    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position + (Vector3)attackOffset, meleeRange);
    }
}


