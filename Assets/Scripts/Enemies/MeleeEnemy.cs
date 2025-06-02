// MeleeEnemy.cs
using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float detectionRange = 5f;

    [Header("Patrol Settings")]
    public float patrolIntervalMin = 2f;
    public float patrolIntervalMax = 5f;

    [Header("Ground & Obstacle Check")]
    public Vector2 groundCheckOffset = Vector2.down * 0.5f; // 地面检测偏移
    public float groundCheckRadius = 0.2f;                   // 地面检测半径
    public float obstacleCheckDistance = 0.5f;               // 前方障碍射线长度
    public LayerMask groundLayer;                            // 地面和障碍所在层

    [Header("Melee Attack Data")]
    public float attackRange = 1.5f;
    public int attackDamage = 10;
    public LayerMask playerLayer;
    public Vector2 attackOffset = Vector2.zero;
    public float knockbackForce = 5f;

    [Header("Defensive Data")]
    [Tooltip("敌人的防御力，用于伤害减免 (减免量 = floor(defense × 0.2))")]
    public int defense = 0;

    [Header("Jump Settings")]
    public float jumpForce = 5f; // 跳跃力度

    [Header("Visuals")]
    public Transform spriteHolder;

    private Transform _player;
    private bool _facingRight;
    private int _patrolDir = 1;
    private float _nextPatrolTime;

    // 当前是否在地面上
    private bool isGrounded;

    protected override void Awake()
    {
        base.Awake();
        _player = GameObject.FindWithTag("Player")?.transform;
        _facingRight = spriteHolder.localScale.x > 0;
        _nextPatrolTime = Time.time + Random.Range(patrolIntervalMin, patrolIntervalMax);
    }

    protected override void InitializeAttackStrategy()
    {
        attackStrategy = new MeleeAttackStrategy(
            attackRange,
            attackDamage,
            playerLayer,
            attackOffset,
            knockbackForce
        );
    }

    protected override bool ShouldAttack()
    {
        if (_player == null) return false;
        float dx = _player.position.x - transform.position.x;
        bool inDir = _facingRight ? dx >= 0 : dx <= 0;
        return inDir && Mathf.Abs(dx) <= attackRange;
    }

    protected override bool ShouldMove()
    {
        if (_player == null) return false;
        float dist = Vector2.Distance(transform.position, _player.position);
        return !ShouldAttack() && dist <= detectionRange;
    }

    protected override void PerformMovement()
    {
        if (_player == null)
        {
            Patrol();
            return;
        }

        // —— 更新地面检测 —— 
        Vector2 groundCheckPos = (Vector2)transform.position + groundCheckOffset;
        isGrounded = Physics2D.OverlapCircle(groundCheckPos, groundCheckRadius, groundLayer);

        float dist = Vector2.Distance(transform.position, _player.position);

        // 1) 玩家在近战攻击范围：停下并切回 Idle
        if (ShouldAttack())
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            currentState = BaseState.Idle;
        }
        // 2) 玩家在感知范围内且不在近战范围：执行追击/跳跃
        else if (dist <= detectionRange)
        {
            // 先计算水平方向
            float dirX = (_player.position.x - transform.position.x) >= 0 ? 1f : -1f;
            // 翻转朝向
            FlipIfNeeded(dirX);

            // 检测前方是否有障碍
            Vector2 rayOrigin = (Vector2)transform.position + new Vector2(_facingRight ? 0.1f : -0.1f, 0f);
            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin,
                _facingRight ? Vector2.right : Vector2.left,
                obstacleCheckDistance,
                groundLayer
            );

            if (hit.collider != null && isGrounded)
            {
                // 前方有障碍并且在地面上 → 跳跃
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }

            // 水平追击
            rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
        }
        // 3) 玩家脱离感知范围：巡逻
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        Vector2 checkPos = (Vector2)transform.position +
                           new Vector2(groundCheckOffset.x * _patrolDir, groundCheckOffset.y);
        if (!Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer))
            _patrolDir *= -1;

        if (Time.time >= _nextPatrolTime)
        {
            _patrolDir = Random.value < 0.5f ? -1 : 1;
            _nextPatrolTime = Time.time + Random.Range(patrolIntervalMin, patrolIntervalMax);
        }

        FlipIfNeeded(_patrolDir);
        rb.velocity = new Vector2(_patrolDir * moveSpeed, rb.velocity.y);
    }

    private void FlipIfNeeded(float dirX)
    {
        bool shouldFaceRight = dirX > 0f;
        if (shouldFaceRight != _facingRight)
        {
            _facingRight = shouldFaceRight;
            Vector3 s = spriteHolder.localScale;
            s.x *= -1f;
            spriteHolder.localScale = s;
        }
    }

    // 重写，为了传递正确朝向给攻击策略
    public override void OnAttackHitEvent()
    {
        if (attackStrategy is MeleeAttackStrategy melee)
            melee.SetFacingDirection(_facingRight);
        base.OnAttackHitEvent();
    }

    // 重写 TakeDamage：先用 defense 做一次伤害减免，再调用基类把剩余伤害传给 HealthController
    public override void TakeDamage(int dmg)
    {
        // —— 1. 先计算防御减免量 —— 
        int reduction = Mathf.FloorToInt(defense * 0.2f);
        int effectiveDamage = dmg - reduction;
        if (effectiveDamage < 0) effectiveDamage = 0;

        // —— 2. 调用基类逻辑：基类会把剩余伤害传给 EnemyHealthController 来扣血，并播放受伤动画等
        base.TakeDamage(effectiveDamage);

        // —— 3. （可选）受击后加一个后退冲量
        rb.AddForce(Vector2.left * moveSpeed, ForceMode2D.Impulse);
    }
}
