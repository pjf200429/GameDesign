using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerHealthController : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public Slider slider;
    public int maxHealth = 100;
    private int currentHealth;

    [Header("UI Elements")]
    public TextMeshProUGUI healthText;
    public Image fillImage;
    public Transform healthCanvas; // World-space canvas for the health bar

    public event Action OnPlayerDied;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        // Debug: press H to take 10 damage
        if (Input.GetKeyDown(KeyCode.H))
            TakeDamage(10);
    }

    void LateUpdate()
    {
        // Keep the world-space health bar upright
        if (healthCanvas != null)
            healthCanvas.rotation = Quaternion.identity;
    }

    /// <summary>
    /// IDamageable 接口方法：被攻击时调用
    /// </summary>
    /// <param name="damage">受到的伤害值</param>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log("[Player] TakeDamage called with " + damage);

        UpdateHealthUI();

        if (currentHealth <= 0)
            OnPlayerDied?.Invoke();
    }

    /// <summary>
    /// 恢复生命
    /// </summary>
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateHealthUI();
    }

    /// <summary>
    /// 重置生命（通常在重生时调用）
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        float percent = (float)currentHealth / maxHealth;

        slider.maxValue = 1f;
        slider.value = percent;

        if (healthText != null)
            healthText.text = $"{currentHealth} / {maxHealth}";

        if (fillImage != null)
        {
            Color c = percent > 0.5f
                ? Color.Lerp(Color.yellow, Color.green, (percent - 0.5f) * 2f)
                : Color.Lerp(Color.red, Color.yellow, percent * 2f);
            fillImage.color = c;
        }
    }
}
