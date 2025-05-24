using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    private enum EnemyState { Patrolling, Chasing, Retreating }

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float detectionRange = 5f;

    [Header("Patrol Behavior")]
    public float patrolRange = 3f;
    public float randomPatrolIntervalMin = 2f;
    public float randomPatrolIntervalMax = 5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.4f;
    public LayerMask groundLayer;

    [Header("Visuals")]
    public Transform spriteHolder;

    private Transform player;
    private Rigidbody2D rb;
    private Vector3 startPosition;
    private int patrolDirection = 1;

    private float nextDirectionChangeTime = 0f;
    private EnemyState currentState = EnemyState.Patrolling;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void FixedUpdate()
    {
        float distanceToPlayer = player != null ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;

        switch (currentState)
        {
            case EnemyState.Patrolling:
                if (distanceToPlayer <= detectionRange)
                    currentState = EnemyState.Chasing;
                else
                    Patrol();
                break;

            case EnemyState.Chasing:
                if (distanceToPlayer > detectionRange)
                {
                    currentState = EnemyState.Patrolling;
                }
                else if (!IsGroundAhead())
                {
                    currentState = EnemyState.Retreating;
                }
                else
                {
                    ChasePlayer();
                }
                break;

            case EnemyState.Retreating:
                if (distanceToPlayer > detectionRange)
                {
                    currentState = EnemyState.Patrolling;
                }
                else
                {
                    RetreatFromCliff();
                }
                break;
        }
    }

    private void Patrol()
    {
        if (!IsGroundAhead())
        {
            patrolDirection *= -1;
            Flip(patrolDirection);
            nextDirectionChangeTime = Time.time + Random.Range(randomPatrolIntervalMin, randomPatrolIntervalMax);
        }

        if (Time.time >= nextDirectionChangeTime)
        {
            patrolDirection = Random.Range(0, 2) == 0 ? -1 : 1;
            Flip(patrolDirection);
            nextDirectionChangeTime = Time.time + Random.Range(randomPatrolIntervalMin, randomPatrolIntervalMax);
        }

        rb.velocity = new Vector2(patrolDirection * moveSpeed, rb.velocity.y);
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float targetVelocity = direction.x * moveSpeed;
        rb.velocity = new Vector2(targetVelocity, rb.velocity.y);
        Flip(direction.x);
    }

    private void RetreatFromCliff()
    {
        // 反方向远离玩家
        float direction = (player.position.x - transform.position.x) > 0 ? -1f : 1f;
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
        Flip(direction);
    }

    private bool IsGroundAhead()
    {
        return groundCheck != null && Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Flip(float direction)
    {
        if (spriteHolder != null)
        {
            Vector3 scale = spriteHolder.localScale;
            scale.x = direction > 0 ? 1 : -1;
            spriteHolder.localScale = scale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}


