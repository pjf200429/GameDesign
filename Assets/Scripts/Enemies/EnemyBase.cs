// EnemyBase.cs
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    protected EnemyHealthController healthController;
    protected Rigidbody2D rb;

    [Header("Animation (SPUM)")]
    public SPUM_Prefabs spum;

    [Header("Fall Death Settings")]
    public float deathY = -10f;

    protected bool isDead = false;

    protected enum BaseState { Idle, Moving, Damaged, Dead }
    protected BaseState currentState = BaseState.Idle;

    protected virtual void Awake()
    {
        healthController = GetComponent<EnemyHealthController>();
        rb = GetComponent<Rigidbody2D>();

        if (healthController != null)
            healthController.OnEnemyDied += HandleDeath;

        if (spum != null)
            spum.OverrideControllerInit();
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
                OnIdle();
                break;
            case BaseState.Moving:
                OnMove();
                break;
            case BaseState.Damaged:
                // 受击期间，子类可重写行为
                break;
        }
    }

    protected virtual void OnIdle()
    {
        PlayAnimation(PlayerState.IDLE);
        if (ShouldMove())
            ChangeState(BaseState.Moving);
    }

    protected virtual void OnMove()
    {
        PerformMovement();
        PlayAnimation(PlayerState.MOVE);
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        healthController?.TakeDamage(damage);
        ChangeState(BaseState.Damaged);
        PlayAnimation(PlayerState.DAMAGED);
        Invoke(nameof(ResumeFromDamaged), 0.5f);

    }

    private void ResumeFromDamaged()
    {
        if (!isDead)
            ChangeState(BaseState.Idle);
    }

    protected virtual void HandleDeath()
    {
        if (isDead) return;

        isDead = true;
        ChangeState(BaseState.Dead);
        PlayAnimation(PlayerState.DEATH);

        // 这里用一个固定延迟销毁，0.5 秒后 Destroy
        Destroy(gameObject, 0.5f);
    }

    protected void ChangeState(BaseState newState)
    {
        currentState = newState;
    }

    protected void PlayAnimation(PlayerState animState)
    {
        if (spum != null)
            spum.PlayAnimation(animState, 0);
    }

    protected virtual void OnDamaged() { }

    protected virtual void OnDestroy()
    {
        if (healthController != null)
            healthController.OnEnemyDied -= HandleDeath;
    }

    //—— 子类必须实现 ——//
    protected abstract bool ShouldMove();
    protected abstract void PerformMovement();
}
