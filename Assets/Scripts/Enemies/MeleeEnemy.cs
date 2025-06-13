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
    public Vector2 groundCheckOffset = Vector2.down * 0.5f;
    public float groundCheckRadius = 0.2f;
    public float obstacleCheckDistance = 0.5f;
    public LayerMask groundLayer;

    [Header("Melee Attack Data")]
    public float attackRange = 1.5f;
    public int attackDamage = 10;
    public LayerMask playerLayer;
    public Vector2 attackOffset = Vector2.zero;
    public float knockbackForce = 5f;

    [Header("Defensive Data")]
    [Tooltip("Enemy defense for damage reduction (reduction = floor(defense * 0.2))")]
    public int defense = 0;

    [Header("Jump Settings")]
    public float jumpForce = 5f;

    [Header("Visuals")]
    public Transform spriteHolder;

    private Transform _player;
    private bool _facingRight;
    private int _patrolDir = 1;
    private float _nextPatrolTime;
    private bool isGrounded;

    private void Reset()
    {
        // Default gold drop for this type of enemy
        goldDrop = 10;
    }

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
        bool inFacingDirection = _facingRight ? dx >= 0 : dx <= 0;
        return inFacingDirection && Mathf.Abs(dx) <= attackRange;
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

        Vector2 groundCheckPos = (Vector2)transform.position + groundCheckOffset;
        isGrounded = Physics2D.OverlapCircle(groundCheckPos, groundCheckRadius, groundLayer);

        float dist = Vector2.Distance(transform.position, _player.position);

        if (ShouldAttack())
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            currentState = BaseState.Idle;
          
          
        }
        else if (dist <= detectionRange)
        {
            float dirX = (_player.position.x - transform.position.x) >= 0 ? 1f : -1f;
            FlipIfNeeded(dirX);

            Vector2 rayOrigin = (Vector2)transform.position + new Vector2(_facingRight ? 0.1f : -0.1f, 0f);
            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin,
                _facingRight ? Vector2.right : Vector2.left,
                obstacleCheckDistance,
                groundLayer
            );

            if (hit.collider != null && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }

            rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
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
        bool shouldFaceRight = dirX > 0f;
        if (shouldFaceRight != _facingRight)
        {
            _facingRight = shouldFaceRight;
            Vector3 s = spriteHolder.localScale;
            s.x *= -1f;
            spriteHolder.localScale = s;
        }
    }

    
    public override void OnAttackHitEvent()
    {
 
        if (attackStrategy is MeleeAttackStrategy melee)
            melee.SetFacingDirection(_facingRight);
        base.OnAttackHitEvent();
    }

   
    public override void TakeDamage(int dmg)
    {
        int reduction = Mathf.FloorToInt(defense * 0.2f);
        int effectiveDamage = dmg - reduction;
        if (effectiveDamage < 0) effectiveDamage = 0;

        base.TakeDamage(effectiveDamage);

        // Optional: add knockback when damaged
        rb.AddForce(Vector2.left * moveSpeed, ForceMode2D.Impulse);
    }
}
