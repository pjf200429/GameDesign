using UnityEngine;
using System;
using static BossBehaviorTree;
using System.Collections;

[RequireComponent(typeof(EnemyHealthController), typeof(Rigidbody2D))]
public class BossSkillController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;

    [Header("General Settings")]
    public Transform attackPoint;
    public BossAnimationManager animationManager;


    [Header("Shield Settings")]
    public ParticleSystem shieldEffectPrefab;
    private ParticleSystem _shieldEffectInstance;
    private Transform _spriteHolder; 
    private Action<bool> shieldCallback;
    public float shieldCooldown = 3f; 


    [Header("Common Settings")]
    public LayerMask playerLayer;

    [Header("Melee Skill: MeleeSweep")]
    public float meleeSweepRange;
    public int meleeSweepDamage;
    public Vector2 meleeSweepOffset;
    public float meleeSweepKnockback;
    public float meleeSweepCooldown;
    public AnimationClip meleeSweepClip;

    [Header("Melee Skill: JumpSmash")]
    public float jumpSmashRange;
    public int jumpSmashDamage;
    public Vector2 jumpSmashOffset;
    public float jumpSmashKnockback;
    public float jumpSmashCooldown;
    public AnimationClip jumpSmashClip;

    [Header("Ranged Skill: Fireball")]
    public GameObject projectilePrefab;
    public GameObject hitEffectPrefab;
    public int fireballDamage;
    public float fireballSpeed;
    public float fireballRange;
    public float fireballCooldown;
    public AnimationClip fireballClip;

    [Header("Ranged Skill: ClusterBomb")]
    public int clusterDamage;
    public float clusterSpeed;
    public float clusterRange;
    public float clusterCooldown;
    public AnimationClip clusterClip;

    [Header("Melee Skill: Charge")]
    public float chargeRange;
    public int chargeDamage;
    public Vector2 chargeOffset;
    public float chargeKnockback;
    public float chargeCooldown;
    public AnimationClip chargeClip;

    [Header("Melee Skill: RageCombo")]
    public float rageComboRange;
    public int rageComboDamage;
    public Vector2 rageComboOffset;
    public float rageComboKnockback;
    public float rageComboCooldown;

    [Header("Other Skills")]
    public GameObject minionPrefab;
    public Transform[] summonPoints;
    public float summonCooldown;

   

    private float _lastMeleeSweepTime = -999f;
    private float _lastJumpSmashTime = -999f;
    private float _lastFireballTime = -999f;
    private float _lastClusterTime = -999f;
    private float _lastChargeTime = -999f;
    private float _lastSummonTime = -999f;
    private float _lastShieldTime = -999f;
    private float _lastRageComboTime = -999f;

    private Rigidbody2D rb;
    private Transform _player;
   


    private IWeaponStrategy attackStrategy;

    public float LastMeleeSweepTime => _lastMeleeSweepTime;
    public float LastJumpSmashTime => _lastJumpSmashTime;
    public float LastFireballTime => _lastFireballTime;
    public float LastClusterTime => _lastClusterTime;
    public float LastChargeTime => _lastChargeTime;
    public float LastSummonTime => _lastSummonTime;
    public float LastShieldTime => _lastShieldTime;
    public float LastRageComboTime => _lastRageComboTime;

    public IWeaponStrategy currentStrategy => attackStrategy;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _player = GameObject.FindWithTag("Player")?.transform;
    }

    public void MeleeSweep(bool facingRight)
    {
        _lastMeleeSweepTime = Time.time;
        var melee = new MeleeAttackStrategy(
            meleeSweepRange, meleeSweepDamage, playerLayer, meleeSweepOffset, meleeSweepKnockback
        );
        melee.SetFacingDirection(facingRight);
        attackStrategy = melee;
        animationManager.SetDefaultAttackAnimation(meleeSweepClip);
    }

    public void JumpSmash(bool facingRight,float direction)
    {
        _lastJumpSmashTime = Time.time;
        if (_player != null)
        {
            if (direction > 0)
               transform.position = _player.position - Vector3.right * 2f;
            else
                transform.position = _player.position + Vector3.right * 2f;
        }
        var melee = new MeleeAttackStrategy(
            jumpSmashRange, jumpSmashDamage, playerLayer, jumpSmashOffset, jumpSmashKnockback
        );
        melee.SetFacingDirection(facingRight);
        attackStrategy = melee;
        animationManager.SetDefaultAttackAnimation(jumpSmashClip);
    }

    public void FireballVolley()
    {
        _lastFireballTime = Time.time;
        var ranged = new RangedAttack(
            projectilePrefab, hitEffectPrefab, fireballDamage, fireballSpeed, fireballRange, playerLayer, 1.5f
        );
        attackStrategy = ranged;
        animationManager.SetDefaultAttackAnimation(fireballClip);
    }

    public void ClusterBomb()
    {
        _lastClusterTime = Time.time;
        var ranged = new RangedAttack(
            projectilePrefab, hitEffectPrefab, clusterDamage, clusterSpeed, clusterRange, playerLayer, 1.5f
        );
        attackStrategy = ranged;
        animationManager.SetDefaultAttackAnimation(clusterClip);
    }

    public void ChargeAttack()
    {
        _lastChargeTime = Time.time;
        var melee = new MeleeAttackStrategy(
            chargeRange, chargeDamage, playerLayer, chargeOffset, chargeKnockback
        );
        melee.SetFacingDirection(transform.localScale.x > 0);
        attackStrategy = melee;
        animationManager.SetDefaultAttackAnimation(chargeClip);
    }

    public void RageCombo(bool facingRight)
    {
        _lastRageComboTime = Time.time;
        var melee = new MeleeAttackStrategy(
            rageComboRange, rageComboDamage, playerLayer, rageComboOffset, rageComboKnockback
        );
        melee.SetFacingDirection(facingRight);
        attackStrategy = melee;
 
    }

    public void DodgeAndCounter(bool facingRight)
    {
        var melee = new MeleeAttackStrategy(
            rageComboRange, rageComboDamage, playerLayer, rageComboOffset, rageComboKnockback
        );
        melee.SetFacingDirection(facingRight);
        attackStrategy = melee;
  
    }

    public void SummonMinions(GameObject minion, Transform[] points)
    {
        _lastSummonTime = Time.time;
        foreach (var p in points)
            Instantiate(minion, p.position, Quaternion.identity);
    }

    public void ActivateShield()
    {
        _lastShieldTime = Time.time;

       
        var spriteObj = GameObject.FindWithTag("Boss");
        if (spriteObj != null)
            _spriteHolder = spriteObj.transform;
        else
        {
            Debug.LogWarning("Could not find Boss spriteHolder with Tag 'Boss'.");
            return;
        }

      
        if (shieldEffectPrefab != null && _spriteHolder != null)
        {
            _shieldEffectInstance = Instantiate(shieldEffectPrefab, _spriteHolder.position, Quaternion.identity);

         
            _shieldEffectInstance.transform.localScale = new Vector3(1f, 1f, 1f); 

       
            _shieldEffectInstance.transform.position = _spriteHolder.position + new Vector3(0, 1f, 0); 

         
            StartCoroutine(ShieldEffectFollow());
        }
    }




    private IEnumerator ShieldEffectFollow()
    {
        while (_shieldEffectInstance != null && _spriteHolder != null)
        {
        
            _shieldEffectInstance.transform.position = _spriteHolder.position + new Vector3(0, 1f, 0);
            yield return null;
        }
    }

    public void DeactivateShield()
    {
       
        
        if (_shieldEffectInstance != null)
        {
            Destroy(_shieldEffectInstance.gameObject);
            _shieldEffectInstance = null;
        }
    }

   



    public bool IsSkillReady(BossSkillType type)
    {
        float now = Time.time;
        float last = 0f;
        float cooldown = 0f;

        switch (type)
        {
            case BossSkillType.MeleeSweep:
                last = LastMeleeSweepTime;
                cooldown = meleeSweepCooldown;
                break;
            case BossSkillType.JumpSmash:
                last = LastJumpSmashTime;
                cooldown = jumpSmashCooldown;
                break;
            case BossSkillType.Fireball:
                last = LastFireballTime;
                cooldown = fireballCooldown;
                break;
            case BossSkillType.ClusterBomb:
                last = LastClusterTime;
                cooldown = clusterCooldown;
                break;
            case BossSkillType.Charge:
                last = LastChargeTime;
                cooldown = chargeCooldown;
                break;
            case BossSkillType.RageCombo:
                last = LastRageComboTime;
                cooldown = rageComboCooldown;
                break;
            default:
                Debug.LogWarning($"[IsSkillReady] Unknown skill type: {type}");
                return false;
        }

        float delta = now - last;
        bool ready = delta >= cooldown;

     //   Debug.Log($"[IsSkillReady] Skill: {type} | Now: {now:F2} | Last: {last:F2} | Cooldown: {cooldown:F2} | Delta: {delta:F2} | Ready: {ready}");

        return ready;
    }

}
