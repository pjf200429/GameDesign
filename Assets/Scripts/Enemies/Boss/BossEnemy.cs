using UnityEngine;
using System.Collections;
using System.Linq;
using static BossBehaviorTree;

public class BossEnemy : EnemyBase
{
    [Header("Phase Thresholds")]
    [Range(0f, 1f)] public float phase2Threshold = 0.5f;

    [Header("Skill Controller")]
    public BossSkillController skillController;
    public BossBehaviorTree behaviorTree;

    [Header("Visuals")]
    public Transform spriteHolder;

    [Header("Summon Settings")]
    public GameObject MinionPrefab;
    public Transform[] SummonPoints;

    private bool _pendingShieldRequest = false;
    private bool _clearShieldRequest = false;

    private bool _vulnerable = true;   // Track whether the Boss is vulnerable (i.e., can take damage)
    private float shieldTimer;
    private float _idleLockTimer = 0f; // Time remaining during post-attack lockout

    private float _lastHealth;         // Track last health value
    private float _damageThreshold = 0.05f;  // 5% of max health

    public bool IsVulnerable => _vulnerable;
    public bool PendingShieldRequest => _pendingShieldRequest;
    public bool ClearShieldRequest => _clearShieldRequest;
    public Transform Player => GameObject.FindWithTag("Player")?.transform;

    public float ShieldTimer => shieldTimer;

    public PlayerHealthController phc => GameObject.FindWithTag("Player")?.GetComponent<PlayerHealthController>();
    public EnemyHealthController ehc => GameObject.FindWithTag("Boss")?.GetComponent<EnemyHealthController>();
    public float PlayerDistance => Player != null ? Vector2.Distance(transform.position, Player.position) : Mathf.Infinity;
    public bool FacingRight { get; set; } = true;

    protected override void Awake()
    {
        base.Awake();
        behaviorTree = new BossBehaviorTree(this, skillController,phc,ehc);
    }

    protected override void InitializeAttackStrategy() { /* Handled by behavior tree */ }

    protected override bool ShouldAttack()
    {
        if (Player == null) return false;

        // 1) Cone check: only attack if player is within б└30бу of facing direction
        Vector2 toPlayer = ((Vector2)Player.position - (Vector2)transform.position).normalized;
        Vector2 facingDir = FacingRight ? Vector2.right : Vector2.left;
        if (Vector2.Angle(facingDir, toPlayer) > 30f) return false;

        // 2) Delegate to behavior tree skill-selection logic
        return behaviorTree.ShouldAttack(PlayerDistance);
    }

    protected override void TriggerAttack()
    {
        _lastAttackTime = Time.time;

        // 1) Execute selected skill
        switch (behaviorTree.NextSkill)
        {
            case BossSkillType.MeleeSweep:
                skillController.MeleeSweep(FacingRight);
                break;
            case BossSkillType.JumpSmash:
                skillController.JumpSmash(FacingRight,spriteHolder.localScale.x);
                break;
            case BossSkillType.Fireball:
                skillController.FireballVolley();
                break;
            case BossSkillType.ClusterBomb:
                skillController.ClusterBomb();
                break;
            case BossSkillType.Charge:
                skillController.ChargeAttack();
                break;
            case BossSkillType.RageCombo:
                skillController.RageCombo(FacingRight);
                break;
            case BossSkillType.DodgeCounter:
                skillController.DodgeAndCounter(FacingRight);
                break;
        }

        // 2) Play attack animation
        PlayAnimation(PlayerState.ATTACK);

        // 3) Enter idle FSM state but keep animation playing
        currentState = BaseState.Idle;

        // 4) Initialize post-attack lockout: animation length * 0.8 + skill-specific recover
        float animTime = GetAttackAnimationLength() * 0.8f;
        float recover = GetAttackRecoverTime();
        _idleLockTimer = animTime + recover;
    }

    public void SetShieldTime(float time)
    {
        shieldTimer = time;
    }

    public void SetPeningRequest(bool request)
    {
        _pendingShieldRequest = request;
    }

    public void SetClearRequest(bool request)
    {
        _clearShieldRequest= request;
    }

    protected override void UpdateIdle()
    {
        // Always play idle animation
        PlayAnimation(PlayerState.IDLE);

        // If still in post-attack lockout, decrement timer and skip decisions
        if (_idleLockTimer > 0f)
        {
            _idleLockTimer -= Time.deltaTime;
            return;
        }

        // After lockout, decide next action
        if ( ShouldAttack())
            TriggerAttack();
        else if (ShouldMove())
            currentState = BaseState.Moving;
    }

    protected override bool ShouldMove()
    {
        if (Player == null) return false;
        return PlayerDistance < 15f && !ShouldAttack();
    }

    protected override void PerformMovement()
    {
        if (Player == null)
        {
            Patrol();
            return;
        }

        if (PlayerDistance > 15f)
        {
            Patrol();
        }
        else
        {
            // Chase player
            float dir = Mathf.Sign(Player.position.x - transform.position.x);
            FacingRight = dir > 0;
            spriteHolder.localScale = new Vector3(FacingRight ? 1 : -1, 1, 1);
            rb.velocity = new Vector2(dir * skillController.moveSpeed, rb.velocity.y);
        }
    }

    private float _nextPatrolTime;
    private int _patrolDir = 1;

    private void Patrol()
    {
        // Change direction at intervals
        if (Time.time > _nextPatrolTime)
        {
            _patrolDir = Random.value > 0.5f ? 1 : -1;
            _nextPatrolTime = Time.time + Random.Range(2f, 4f);
        }

        FacingRight = _patrolDir > 0;
        spriteHolder.localScale = new Vector3(FacingRight ? 1 : -1, 1, 1);
        rb.velocity = new Vector2(_patrolDir * skillController.moveSpeed * 0.5f, rb.velocity.y);
    }

    private float GetAttackAnimationLength()
    {
        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null && anim.runtimeAnimatorController != null)
        {
            var clip = anim.runtimeAnimatorController.animationClips
                          .FirstOrDefault(c => c.name.ToLower().Contains("attack"));
            if (clip != null) return clip.length;
        }
        Debug.LogWarning("[BossEnemy] Attack clip not found, using default 0.5s");
        return 0.5f;
    }

    private float GetAttackRecoverTime()
    {
        switch (behaviorTree.NextSkill)
        {
            case BossSkillType.MeleeSweep: return 0.4f;
            case BossSkillType.JumpSmash: return 0.8f;
            case BossSkillType.Fireball: return 0.4f;
            case BossSkillType.ClusterBomb: return 0.6f;
            case BossSkillType.Charge: return 0.7f;
            case BossSkillType.RageCombo: return 1.0f;
            case BossSkillType.DodgeCounter: return 0.4f;
            default: return 0.5f;
        }
    }

    public void CheckForShieldTrigger(int dmg)
    {
        float currentHealth = _healthController.CurrentHealth;

        // Calculate health loss percentage
        float healthLost = _lastHealth - currentHealth;
        float damagePercentage = healthLost / _healthController.MaxHealth;

        // If the damage exceeds the threshold (5%), activate the shield
        if (damagePercentage > _damageThreshold)
        {
            _pendingShieldRequest = true;
        }

        // Update last health for future comparisons
        _lastHealth = currentHealth;

    }
        // Update last health for future comparisons
       

    public override void TakeDamage(int dmg)
    {
        if (!_vulnerable)  // Check if Boss is invulnerable
        {
            Debug.Log("Boss is invulnerable, no damage taken.");
            return; // If not vulnerable, stop the damage
        }

        CheckForShieldTrigger(dmg);
        _healthController?.TakeDamage(dmg);
       
        currentState = BaseState.Damaged;
        PlayAnimation(PlayerState.DAMAGED);

        // After stagger, resume Idle
        currentState = BaseState.Idle;
        PlayAnimation(PlayerState.IDLE);
        StartCoroutine(HitStunCoroutine(dmg * 0.2f));
    }

    private IEnumerator HitStunCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (isDead) yield break;

        if ( ShouldAttack())
            TriggerAttack();
        else if (ShouldMove())
            currentState = BaseState.Moving;
        else
            currentState = BaseState.Idle;
    }

    public void SetInvulnerable(bool invuln)
    {
        _vulnerable = invuln; // If invulnerable, set vulnerable to false
        Debug.Log("[BossEnemy] Invulnerability set to: " + invuln);
    }

    public override void OnAttackHitEvent()
    {
        if (skillController.currentStrategy != null && Player != null)
        {
            Collider2D playerCol = Player.GetComponent<Collider2D>()
                                ?? Player.GetComponentInChildren<Collider2D>();
            Vector3 targetPos = playerCol != null ? playerCol.bounds.center : Player.position;
            skillController.currentStrategy.Attack(attackPoint, targetPos);
        }
        else
        {
            Debug.LogWarning("[BossEnemy] Attack strategy or player missing");
        }
    }
}
