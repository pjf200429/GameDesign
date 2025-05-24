using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EnemyHealthController : MonoBehaviour
{
    public Slider slider;
    public int maxHealth = 100;
    private int currentHealth;

    public TextMeshProUGUI healthText;
    public Image fillImage;

    public event Action OnEnemyDied;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(maxHealth / 5);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            OnEnemyDied?.Invoke();
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        float healthPercent = (float)currentHealth / maxHealth;

        slider.maxValue = 1f;
        slider.value = healthPercent;

        if (healthText != null)
        {
            healthText.text = currentHealth + " / " + maxHealth;
        }

        if (fillImage != null)
        {
            Color targetColor = healthPercent > 0.5f
                ? Color.Lerp(Color.yellow, Color.green, (healthPercent - 0.5f) / 0.5f)
                : Color.Lerp(Color.red, Color.yellow, healthPercent / 0.5f);

            fillImage.color = targetColor;
        }
    }
}
