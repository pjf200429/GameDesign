// MeleeEnemy.cs
using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    private enum EnemyState { Patrolling, Chasing, Retreating }
    private EnemyState enemyState = EnemyState.Patrolling;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float detectionRange = 5f;

    [Header("Patrol Behavior")]
    public float randomPatrolIntervalMin = 2f;
    public float randomPatrolIntervalMax = 5f;

    [Header("Ground Check (Local Offset)")]
    public Vector2 groundCheckOffset = new Vector2(0f, -0.5f);
    public float groundCheckRadius = 0.4f;
    public LayerMask groundLayer;

    [Header("Visuals")]
    public Transform spriteHolder;

    private bool isFacingRight;
    private int patrolDir = 1;
    private float nextChangeTime = 0f;
    private Transform player;

    protected override void Awake()
    {
        base.Awake();

        isFacingRight = spriteHolder != null && spriteHolder.localScale.x > 0;

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    protected override bool ShouldMove()
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= detectionRange;
    }

    protected override void PerformMovement()
    {
        float dist = player != null
            ? Vector2.Distance(transform.position, player.position)
            : Mathf.Infinity;

        switch (enemyState)
        {
            case EnemyState.Patrolling:
                if (dist <= detectionRange) enemyState = EnemyState.Chasing;
                else Patrol();
                break;

            case EnemyState.Chasing:
                if (dist > detectionRange) enemyState = EnemyState.Patrolling;
                else if (!IsGroundAhead()) enemyState = EnemyState.Retreating;
                else Chase();
                break;

            case EnemyState.Retreating:
                if (dist > detectionRange) enemyState = EnemyState.Patrolling;
                else Retreat();
                break;
        }
    }

    private void Patrol()
    {
        if (!IsGroundAhead())
        {
            patrolDir *= -1;
            TryFlip(patrolDir);
            nextChangeTime = Time.time + Random.Range(randomPatrolIntervalMin, randomPatrolIntervalMax);
        }

        if (Time.time >= nextChangeTime)
        {
            patrolDir = Random.value < .5f ? -1 : 1;
            TryFlip(patrolDir);
            nextChangeTime = Time.time + Random.Range(randomPatrolIntervalMin, randomPatrolIntervalMax);
        }

        rb.velocity = new Vector2(patrolDir * moveSpeed, rb.velocity.y);
    }

    private void Chase()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        TryFlip((int)Mathf.Sign(dir.x));
        rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);
    }

    private void Retreat()
    {
        float dir = (player.position.x - transform.position.x) > 0 ? -1f : 1f;
        TryFlip((int)dir);
        rb.velocity = new Vector2(dir * moveSpeed, rb.velocity.y);
    }

    private bool IsGroundAhead()
    {
        Vector2 checkPos = (Vector2)transform.position +
            new Vector2(groundCheckOffset.x * patrolDir, groundCheckOffset.y);
        return Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);
    }

    private void TryFlip(int direction)
    {
        if (direction > 0 && !isFacingRight) Flip();
        else if (direction < 0 && isFacingRight) Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        if (spriteHolder != null)
        {
            Vector3 s = spriteHolder.localScale;
            s.x *= -1;
            spriteHolder.localScale = s;
        }
    }

    protected override void OnDamaged()
    {
        // ¶îÍâ»÷ÍË
        rb.AddForce(Vector2.left * moveSpeed, ForceMode2D.Impulse);
    }
}
