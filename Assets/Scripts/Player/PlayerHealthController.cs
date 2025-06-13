using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerHealthController : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public Slider slider;
    private int currentHealth;
  

    [Header("UI Elements")]
    public TextMeshProUGUI healthText;
    public Image fillImage;
    public Transform healthCanvas; // World-space canvas for the health bar

    private PlayerAttributes playerAttributes;

    public event Action OnPlayerDied;

    void Awake()
    {
     
        playerAttributes = GetComponent<PlayerAttributes>();
        if (playerAttributes == null)
            playerAttributes = GetComponentInChildren<PlayerAttributes>();

     
    }

    void Start()
    {

        if (playerAttributes != null)
        {
            currentHealth = playerAttributes.GetTeleportHealth();
           
        }
        else
        {
            return;
        }

        UpdateHealthUI();
    }

    void Update()
    {
     
    }

    void LateUpdate()
    {
        if (healthCanvas != null)
            healthCanvas.rotation = Quaternion.identity;
    }

    /// <summary>
    /// IDamageable 接口方法：被攻击时调用
    /// </summary>
    /// <param name="damage">原始伤害值</param>
    public void TakeDamage(int damage)
    {
        if (playerAttributes == null)
        {
            Debug.LogWarning("[PlayerHealthController] 无法计算伤害：缺少 PlayerAttributes。");
            return;
        }

        // 计算减少量：防御力 * 0.2，再向下取整
        int reduction = Mathf.FloorToInt((playerAttributes.ArmorDefense + playerAttributes.HelmetDefense) * playerAttributes.DefenseMultiplier);
        int effectiveDamage = damage - reduction;
        if (effectiveDamage < 0) effectiveDamage = 0;

        currentHealth -= effectiveDamage;
        currentHealth = Mathf.Max(currentHealth, 0);


        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            playerAttributes.setCurrentScore(-300);
            OnPlayerDied?.Invoke();
        }
    }

    /// <summary>
    /// Recovery
    /// </summary>
    public void Heal(int amount)
    {
        if (playerAttributes == null) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, playerAttributes.MaxHealth);
        UpdateHealthUI();
    }

    /// <summary>
    /// Reset 
    /// </summary>
    public void ResetHealth()
    {
        if (playerAttributes == null) return;

        currentHealth = playerAttributes.MaxHealth;
        UpdateHealthUI();
    }

    public void UpdateHealthUI()
    {
        if (playerAttributes == null)
            return;

        float maxH = playerAttributes.MaxHealth;
        float percent = (float)currentHealth / maxH;

        slider.maxValue = 1f;
        slider.value = percent;

        if (healthText != null)
            healthText.text = $"{currentHealth} / {playerAttributes.MaxHealth}";

        if (fillImage != null)
        {
            Color c = percent > 0.5f
                ? Color.Lerp(Color.yellow, Color.green, (percent - 0.5f) * 2f)
                : Color.Lerp(Color.red, Color.yellow, percent * 2f);
            fillImage.color = c;
        }
    }
    public int getCurrentHealth
    {
        get { return currentHealth; }
    }
}
