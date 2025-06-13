// EnemyHealthController.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EnemyHealthController : MonoBehaviour
{
    public Slider slider;
    public int maxHealth = 500;
    private int currentHealth;

    public TextMeshProUGUI healthText;
    public Image fillImage;

    public event Action OnEnemyDied;

    private float _lastDamageTime;  

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    void Start()
    {
        currentHealth = maxHealth;
        _lastDamageTime = Time.time;
        UpdateHealthUI();
    }

    void Update()
    {
       
    }

    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthUI();
        _lastDamageTime = Time.time;
        if (currentHealth <= 0)
        {
            OnEnemyDied?.Invoke();
        }
    }

    public float GetLastDamageTime()
    {
        float temp = _lastDamageTime;
     
        return temp;
    }
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        float healthPercent = (float)currentHealth / maxHealth;

        if (slider != null)
        {
            slider.maxValue = 1f;
            slider.value = healthPercent;
        }

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
