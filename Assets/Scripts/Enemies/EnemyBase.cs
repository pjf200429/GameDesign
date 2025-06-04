
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Attack Settings")]
    public Transform attackPoint;        // Attack detection point
    public float attackCooldown = 1f;    // Attack cooldown in seconds
    protected IWeaponStrategy attackStrategy;
    private float _lastAttackTime = -999f;

    [Header("Animation (SPUM)")]
    public SPUM_Prefabs spum;

    [Header("Death Settings")]
    public float deathY = -10f;          // Y position below which enemy dies

    [Header("Drop Settings")]
    [Tooltip("Amount of gold dropped when this enemy dies")]
    public int goldDrop = 0;

    protected bool isDead;
    protected enum BaseState { Idle, Moving, Damaged, Dead }
    protected BaseState currentState = BaseState.Idle;

    // Cached component references
    private EnemyHealthController _healthController;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        _healthController = GetComponent<EnemyHealthController>();
        rb = GetComponent<Rigidbody2D>();
        if (_healthController != null)
            _healthController.OnEnemyDied += HandleDeath;

        spum?.OverrideControllerInit();
        InitializeAttackStrategy();
    }

    protected virtual void OnDestroy()
    {
        if (_healthController != null)
            _healthController.OnEnemyDied -= HandleDeath;
    }

    protected virtual void Update()
    {
        if (isDead) return;

        if (transform.position.y < deathY)
        {
            HandleDeath();
            return;
        }

        switch (currentState)
        {
            case BaseState.Idle:
                UpdateIdle();
                break;
            case BaseState.Moving:
                UpdateMoving();
                break;
            case BaseState.Damaged:
                // Paused while damaged
                break;
        }
    }

    private void UpdateIdle()
    {
        PlayAnimation(PlayerState.IDLE);

        if (CanAttack() && ShouldAttack())
            TriggerAttack();
        else if (ShouldMove())
            currentState = BaseState.Moving;
    }

    private void UpdateMoving()
    {
        PlayAnimation(PlayerState.MOVE);

        if (CanAttack() && ShouldAttack())
            TriggerAttack();
        else
            PerformMovement();
    }

    private bool CanAttack() => Time.time - _lastAttackTime >= attackCooldown;

    private void TriggerAttack()
    {
        _lastAttackTime = Time.time;
        PlayAnimation(PlayerState.ATTACK);
    }

    /// <summary>
    /// Animation Event calls this method when attack animation hits. Subclasses may override to pass facing direction first.
    /// </summary>
    public virtual void OnAttackHitEvent()
    {
        attackStrategy?.Attack(attackPoint);
    }

    public virtual void TakeDamage(int dmg)
    {
        _healthController?.TakeDamage(dmg);
        currentState = BaseState.Damaged;
        PlayAnimation(PlayerState.DAMAGED);
        Invoke(nameof(ResumeFromDamaged), 0.5f);
    }

    private void ResumeFromDamaged()
    {
        if (!isDead)
            currentState = BaseState.Idle;
    }

    protected void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        // Drop gold to player when enemy dies
        var playerGO = GameObject.FindWithTag("Player");
        if (playerGO != null)
        {
            var attrs = playerGO.GetComponent<PlayerAttributes>();
            if (attrs != null)
            {
                attrs.AddCurrency(goldDrop);
                Debug.Log($"[EnemyBase] Enemy {name} died and dropped {goldDrop} gold.");
            }
            else
            {
                Debug.LogWarning("[EnemyBase] Found Player object but missing PlayerAttributes component; cannot add gold.");
            }
        }
        else
        {
            Debug.LogWarning("[EnemyBase] Could not find GameObject tagged 'Player'; cannot drop gold.");
        }

        PlayAnimation(PlayerState.DEATH);
        Destroy(gameObject, 0.5f);
    }

    protected void PlayAnimation(PlayerState state)
    {
        spum?.PlayAnimation(state, 0);
    }

    // Subclasses must implement these methods
    protected abstract void InitializeAttackStrategy();
    protected abstract bool ShouldAttack();
    protected abstract bool ShouldMove();
    protected abstract void PerformMovement();
}
