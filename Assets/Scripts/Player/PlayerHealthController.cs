using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerHealthController : MonoBehaviour
{
    public Slider slider;
    public int maxHealth = 100;
    private int currentHealth;

    public TextMeshProUGUI healthText;
    public Image fillImage;
    public Transform healthCanvas; // 血条 Canvas（World Space）

    public event Action OnPlayerDied;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        // Debug Key
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10);
        }
    }

    void LateUpdate()
    {
        // 若需要防止旋转带来的倾斜（一般不会在 2D 情况下有问题），可以保留
        if (healthCanvas != null)
        {
            healthCanvas.rotation = Quaternion.identity;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            OnPlayerDied?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateHealthUI();
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



