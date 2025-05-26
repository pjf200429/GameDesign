
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

    private bool isFacingRight = true; //  �����ֶΣ���¼��ǰ����

    private SPUM_Prefabs spum;

    public AnimationClip attackClip;  // �� Inspector ���� ATTACK.anim




    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spum = GetComponent<SPUM_Prefabs>();

        //  ֻ�� PlayerCombat.cs �����ö��� clip �б�
        if (spum != null)
        {
            spum.OverrideControllerInit();  // ȷ�� AnimatorOverrideController ��ʼ��

            if (attackClip != null)
            {
                spum.ATTACK_List.Clear();
                spum.ATTACK_List.Add(attackClip);
                spum.StateAnimationPairs["ATTACK"] = spum.ATTACK_List;
              
            }
            else
            {
                Debug.LogWarning("[PlayerCombat] ���� Inspector ������ attackClip��ATTACK ��������");
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

            // ���Ź�����������̬�л� ATTACK ״̬�󶨵� Clip��
            if (spum != null && spum.StateAnimationPairs.ContainsKey("ATTACK") && spum.StateAnimationPairs["ATTACK"].Count > 0)
            {
                Debug.Log("<color=cyan>[PlayerCombat]</color> ���� ATTACK ���� clip: " + spum.StateAnimationPairs["ATTACK"][0].name);
                spum.PlayAnimation(PlayerState.ATTACK, 0);

                StartCoroutine(CheckAttackAnimationState());
            }
            else
            {
                Debug.LogWarning("<color=yellow>[PlayerCombat]</color> ATTACK ����δ���û��Ҳ���");
            }
        }
    }

    private System.Collections.IEnumerator CheckAttackAnimationState()
    {
        yield return new WaitForSeconds(0.1f);

        Animator anim = spum._anim;

        if (anim == null)
        {
            Debug.LogError("[Animator] δ�ҵ� Animator �����");
            yield break;
        }

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        var clipInfo = anim.GetCurrentAnimatorClipInfo(0);

        Debug.Log($"<color=green>[Animator]</color> ��ǰ״̬�Ƿ��� ATTACK: {stateInfo.IsName("Base Layer.ATTACK")}, ״̬�� Hash: {stateInfo.fullPathHash}");

        if (clipInfo.Length > 0)
        {
            Debug.Log($"<color=green>[Animator]</color> ��ǰ���� Clip: {clipInfo[0].clip.name}, ������ʱ��: {stateInfo.normalizedTime}");
        }
        else
        {
            Debug.LogWarning("<color=orange>[Animator]</color> ��ǰû�в����κζ�����");
        }
    }


    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position + (Vector3)attackOffset, meleeRange);
    }
}


