using UnityEngine;
using System.Linq;

/// <summary>
/// RangedEnemy inherits from EnemyBase and uses RangedAttack strategy to shoot projectiles at the player.
/// AI logic:
///   1) Patrol when player is outside detectionRange.
///   2) When player enters detectionRange but is outside attackRange: move toward player.
///   3) Once within attackRange: stop moving, face the player, and fire a projectile (triggered by animation event).
///
/// </summary>
public class RangedEnemy : EnemyBase
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float detectionRange = 8f;
    public float attackRange = 6f;

    [Header("Patrol Settings")]
    public float patrolIntervalMin = 2f;
    public float patrolIntervalMax = 5f;

    [Header("Ground & Obstacle Check")]
    public Vector2 groundCheckOffset = Vector2.down * 0.5f;
    public float groundCheckRadius = 0.2f;
    public float obstacleCheckDistance = 0.5f;
    public LayerMask groundLayer;

    [Header("Ranged Attack Data")]
    public GameObject projectilePrefab;      // Projectile prefab
    public GameObject hitEffectPrefab;       // Hit effect prefab (can be null)
    public int baseDamage = 8;
    public float projectileSpeed = 10f;
    public float maxRange = 12f;
    public LayerMask targetLayers;           // Layers the projectile can hit
    public float hitEffectDuration = 0.5f;

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
        goldDrop = 15;
    }

    protected override void Awake()
    {
        base.Awake();

      
        _player = GameObject.FindWithTag("Player")?.transform;
        if (_player == null)
        {
            Debug.LogWarning("[RangedEnemy] 未找到 Tag 为 'Player' 的对象。");
        }


        _facingRight = spriteHolder.localScale.x > 0;
        _nextPatrolTime = Time.time + Random.Range(patrolIntervalMin, patrolIntervalMax);
    }

    /// <summary>
    /// Create the RangedAttack attack strategy.。
    /// </summary>
    protected override void InitializeAttackStrategy()
    {
        attackStrategy = new RangedAttack(
            projectilePrefab,
            hitEffectPrefab,
            baseDamage,
            projectileSpeed,
            maxRange,
            targetLayers,
            hitEffectDuration
        );
    }

    /// <summary>
    /// Should an attack be carried out: The player must be within the attack range and have a distance of ≤ attackRange from the enemy.
    /// </summary>
    protected override bool ShouldAttack()
    {
        if (_player == null)
            return false;

        Vector2 toPlayer = _player.position - transform.position;
        bool inFacingDirection = _facingRight ? toPlayer.x >= 0 : toPlayer.x <= 0;
        float dist = toPlayer.magnitude;
        return inFacingDirection && dist <= attackRange;
    }

    /// <summary>
    /// Should it be moved: The player is within the detectionRange but the distance is greater than the attackRange.
    /// </summary>
    protected override bool ShouldMove()
    {
        if (_player == null)
            return false;

        float dist = Vector2.Distance(transform.position, _player.position);
        return (dist <= detectionRange) && (dist > attackRange);
    }

    /// <summary>
    /// Movement logic: If attacked, stop and switch to Idle mode. If within the detection range, track the target; otherwise, conduct patrol.
    /// </summary>
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

    /// <summary>
    /// Patrol logic: When reaching the edge of the platform, turn around. And at random intervals, change direction randomly.
    /// </summary>
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

    /// <summary>
    /// Flip the spriteHolder so that the character faces in the same direction as dirX.
    /// </summary>
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
      
        Vector3 targetPos;

        if (_player != null)
        {
    
            Collider2D playerCol = _player.GetComponent<Collider2D>();
            if (playerCol == null)
            {
                playerCol = _player.GetComponentInChildren<Collider2D>();
            }

            if (playerCol != null)
            {
                targetPos = playerCol.bounds.center;
            }
            else
            {
               
                targetPos = _player.position + Vector3.up * 1.0f;
                Debug.LogWarning("[RangedEnemy] 玩家没有 Collider2D，使用 position+up 作为瞄准点。");
            }
        }
        else
        {
        
            targetPos = transform.position + (_facingRight ? Vector3.right : Vector3.left);
            Debug.LogWarning("[RangedEnemy] 玩家引用为 null，向前方发射弹幕。");
        }

   
    
        attackStrategy.Attack(attackPoint, targetPos);
    }

    /// <summary>
    /// When injured, first reduce defense, then call the base class and perform a retreat.
    /// </summary>
    public override void TakeDamage(int dmg)
    {
        int reduction = Mathf.FloorToInt(defense*0.2f);
        int effectiveDamage = dmg - reduction;
        if (effectiveDamage < 0) effectiveDamage = 0;

        base.TakeDamage(effectiveDamage);


        rb.AddForce(Vector2.left * moveSpeed, ForceMode2D.Impulse);
    }
}
