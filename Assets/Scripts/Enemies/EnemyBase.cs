// EnemyBase.cs
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Attack Settings")]
    public Transform attackPoint;        // 攻击检测挂点
    public float attackCooldown = 1f;    // 攻击冷却
    protected IWeaponStrategy attackStrategy;
    private float _lastAttackTime = -999f;

    [Header("Animation (SPUM)")]
    public SPUM_Prefabs spum;

    [Header("Death Settings")]
    public float deathY = -10f;

    protected bool isDead;
    protected enum BaseState { Idle, Moving, Damaged, Dead }
    protected BaseState currentState = BaseState.Idle;

    // 缓存组件引用
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
        if (transform.position.y < deathY) { HandleDeath(); return; }

        switch (currentState)
        {
            case BaseState.Idle: UpdateIdle(); break;
            case BaseState.Moving: UpdateMoving(); break;
            case BaseState.Damaged: /* 受击停顿 */   break;
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

    /// <summary>动画事件挂在此方法上，子类可 override 先传朝向</summary>
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
        PlayAnimation(PlayerState.DEATH);
        Destroy(gameObject, 0.5f);
    }

    protected void PlayAnimation(PlayerState st)
        => spum?.PlayAnimation(st, 0);

    // —— 子类必须实现 —— 
    protected abstract void InitializeAttackStrategy();
    protected abstract bool ShouldAttack();
    protected abstract bool ShouldMove();
    protected abstract void PerformMovement();
}
