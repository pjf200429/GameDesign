using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    protected EnemyHealthController healthController;

    [Header("Fall Death Settings")]
    public float deathY = -10f;

    [Header("UI Reference")]
    public Transform healthCanvas;

    protected virtual void Awake()
    {
        healthController = GetComponent<EnemyHealthController>();
        if (healthController != null)
        {
            healthController.OnEnemyDied += OnDeath;
        }
    }

    protected virtual void Update()
    {
        if (transform.position.y < deathY)
        {
            OnDeath();
        }
    }

    protected virtual void LateUpdate()
    {
        if (healthCanvas != null)
        {
            // Prevent flipping of health bar
            Vector3 scale = healthCanvas.localScale;
            scale.x = Mathf.Abs(scale.x);
            healthCanvas.localScale = scale;
            healthCanvas.rotation = Quaternion.identity;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        healthController?.TakeDamage(damage);
    }

    protected virtual void OnDeath()
    {
        Destroy(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (healthController != null)
            healthController.OnEnemyDied -= OnDeath;
    }
}
