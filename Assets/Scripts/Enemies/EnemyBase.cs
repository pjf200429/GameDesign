using UnityEngine;
using System.Linq;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Attack Settings")]
    public Transform attackPoint;        // Attack detection point
    public float attackCooldown = 1f;    // Attack cooldown in seconds
    protected IWeaponStrategy attackStrategy;
    protected float _lastAttackTime = -999f;  // Time when TriggerAttack was last called

    [Header("Animation (SPUM)")]
    public SPUM_Prefabs spum;            // SPUM system reference (used to play animations)

    [Header("Death Settings")]
    public float deathY = -10f;          // Y position below which enemy dies

    [Header("Drop Settings")]
    [Tooltip("Amount of gold dropped when this enemy dies")]
    public int goldDrop = 0;
     public int scoreDrop = 0;

    public ScoreManager scoreManager;

    protected bool isDead;
    protected enum BaseState { Idle, Moving, Damaged, Dead }
    protected BaseState currentState = BaseState.Idle;

    // Cached component references
    protected EnemyHealthController _healthController;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        _healthController = GetComponent<EnemyHealthController>();
        rb = GetComponent<Rigidbody2D>();
        if (_healthController != null)
            _healthController.OnEnemyDied += HandleDeath;

        // Initialize SPUM if present
        spum?.OverrideControllerInit();
        InitializeAttackStrategy();
    }

    protected virtual void OnDestroy()
    {
        if (_healthController != null)
            _healthController.OnEnemyDied -= HandleDeath;
    }

    private BaseState _lastState;  

    protected virtual void Update()
    {
        if (isDead) return;

        if (transform.position.y < deathY)
        {
            HandleDeath();
            return;
        }

        // Debug only when state changes
        if (currentState != _lastState)
        {
        
            _lastState = currentState;
        
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
                break;
            case BaseState.Dead:
                break;
        }
    }


    protected virtual void UpdateIdle()
    {
        PlayAnimation(PlayerState.IDLE);
        if (ShouldAttack()&&Time.time -_lastAttackTime > attackCooldown)
        {
            TriggerAttack();
        }
        else if (ShouldMove())
            currentState = BaseState.Moving;
    }

    private void UpdateMoving()
    {
        PlayAnimation(PlayerState.MOVE);

        if (ShouldAttack() && Time.time - _lastAttackTime > attackCooldown)
        {
           
            TriggerAttack();
        }
        else
            PerformMovement();
    }


    /// <summary>
    /// TriggerAttack immediately records the cooldown start time, then adjusts animation speed 
    /// so that the attack animation duration is never longer than attackCooldown.
    /// </summary>
    protected virtual void TriggerAttack()
    {
        // 1) Record the time when this attack was triggered
        _lastAttackTime = Time.time;

        // 2) Attempt to find an Animator in this GameObject or its children
        Animator anim = GetComponentInChildren<Animator>();
      
        if (anim != null)
        {
           
            // Find the first animation clip whose name contains "attack"
            var clips = anim.runtimeAnimatorController.animationClips;
            AnimationClip attackClip = clips.FirstOrDefault(c => c.name.ToLower().Contains("attack"));

            if (attackClip != null)
            {
             
                float clipLength = attackClip.length;
                if (attackCooldown < clipLength)
                {
                    // If cooldown is shorter than clip length, speed up the animation
                    anim.speed = clipLength / attackCooldown;
                }
                else
                {
                    // Otherwise, play at normal speed
                    anim.speed = 1f;
                }
            }
            else
            {
                // If no "attack" clip found, default to normal speed
                anim.speed = 1f;
                Debug.LogWarning("[EnemyBase] No AnimationClip containing 'attack' found; using normal speed.");
            }
        }
        else
        {
            Debug.LogWarning("[EnemyBase] No Animator found in children; cannot adjust attack animation speed.");
        }
       
        // 3) Play the attack animation via SPUM system
        PlayAnimation(PlayerState.ATTACK);
    }

    /// <summary>
    /// This method is called by an Animation Event when the hit frame is reached.
    /// It actually deals damage or fires a projectile, but does not modify cooldown.
    /// </summary>
    public virtual void OnAttackHitEvent()
    {
   
        attackStrategy?.Attack(attackPoint, Vector3.zero);
    }

    public virtual void TakeDamage(int dmg)
    {
        _healthController?.TakeDamage(dmg);
        currentState = BaseState.Damaged;
        PlayAnimation(PlayerState.DAMAGED);
        Invoke(nameof(ResumeFromDamaged), 0.5f);
    }

    protected void ResumeFromDamaged()
    {
        if (!isDead)
            currentState = BaseState.Idle;
    }

    protected void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        // Drop gold to player when enemy dies
        scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {

            scoreManager.AddCurrency(goldDrop);
            scoreManager.AddScore(scoreDrop);
        }

        PlayAnimation(PlayerState.DEATH);
        Destroy(gameObject, 0.5f);
    }

    protected void PlayAnimation(PlayerState state)
    {
        // Use SPUM system to play the specified animation
        spum?.PlayAnimation(state, 0);
    }

    // Subclasses must implement these methods
    protected abstract void InitializeAttackStrategy();
    protected abstract bool ShouldAttack();
    protected abstract bool ShouldMove();
    protected abstract void PerformMovement();
}
