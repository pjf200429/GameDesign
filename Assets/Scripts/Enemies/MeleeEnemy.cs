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

    [Header("Ground Check")]
    public Vector2 groundCheckOffset = Vector2.down * 0.5f;
    public float groundCheckRadius = 0.4f;
    public LayerMask groundLayer;

    [Header("Melee Attack Data")]
    public float attackRange = 1.5f;
    public int attackDamage = 10;
    public LayerMask playerLayer;
    public Vector2 attackOffset = Vector2.zero;
    public float knockbackForce = 5f;

    [Header("Visuals")]
    public Transform spriteHolder;

    private Transform _player;
    private bool _facingRight;
    private int _patrolDir = 1;
    private float _nextPatrolTime;

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
        if (_player == null) { Patrol(); return; }

        float dist = Vector2.Distance(transform.position, _player.position);

        if (ShouldAttack())
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            currentState = BaseState.Idle;
        }
        else if (dist <= detectionRange)
        {
            Vector2 dir = (_player.position - transform.position).normalized;
            FlipIfNeeded(dir.x);
            rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);
        }
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
        bool shouldFaceRight = dirX > 0;
        if (shouldFaceRight != _facingRight)
        {
            _facingRight = shouldFaceRight;
            Vector3 s = spriteHolder.localScale;
            s.x *= -1;
            spriteHolder.localScale = s;
        }
    }

    // 重写，为了传递真正的朝向给攻击策略
    public override void OnAttackHitEvent()
    {
        if (attackStrategy is MeleeAttackStrategy melee)
            melee.SetFacingDirection(_facingRight);
        base.OnAttackHitEvent();
    }

    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
        rb.AddForce(Vector2.left * moveSpeed, ForceMode2D.Impulse);
    }
}
