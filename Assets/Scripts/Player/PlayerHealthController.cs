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

    // 引用玩家属性脚本，从中获取最大生命值和防御力
    private PlayerAttributes playerAttributes;

    public event Action OnPlayerDied;

    void Awake()
    {
        // 尝试获取挂在同一 GameObject 上或子物体上的 PlayerAttributes
        playerAttributes = GetComponent<PlayerAttributes>();
        if (playerAttributes == null)
            playerAttributes = GetComponentInChildren<PlayerAttributes>();

        if (playerAttributes == null)
            Debug.LogError("[PlayerHealthController] 未找到 PlayerAttributes，请确保它挂载在玩家对象上。");
    }

    void Start()
    {
        // 使用属性中定义的最大生命值进行初始化
        if (playerAttributes != null)
        {
            currentHealth = playerAttributes.MaxHealth;
        }
        else
        {
            currentHealth = 100; // 作为后备值
        }

        UpdateHealthUI();
    }

    void Update()
    {
        // 调试：按 H 键受到 10 点伤害
        if (Input.GetKeyDown(KeyCode.H))
            TakeDamage(10);
    }

    void LateUpdate()
    {
        // 保持世界空间血条始终朝向相机
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
        int reduction = Mathf.FloorToInt(playerAttributes.Defense * 0.2f);
        int effectiveDamage = damage - reduction;
        if (effectiveDamage < 0) effectiveDamage = 0;

        currentHealth -= effectiveDamage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"[PlayerHealthController] TakeDamage: 原始伤害={damage}, 防御减免={reduction}, 实际伤害={effectiveDamage}");

        UpdateHealthUI();

        if (currentHealth <= 0)
            OnPlayerDied?.Invoke();
    }

    /// <summary>
    /// 回复生命
    /// </summary>
    public void Heal(int amount)
    {
        if (playerAttributes == null) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, playerAttributes.MaxHealth);
        UpdateHealthUI();
    }

    /// <summary>
    /// 重置生命（通常在重生时调用）
    /// </summary>
    public void ResetHealth()
    {
        if (playerAttributes == null) return;

        currentHealth = playerAttributes.MaxHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (playerAttributes == null)
            return;

        float maxH = playerAttributes.MaxHealth;
        float percent = (float)currentHealth / maxH;

        // 将 Slider 的值规范化到 0-1 的区间
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
}
