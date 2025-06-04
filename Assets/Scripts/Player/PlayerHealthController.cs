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
        // ʹ�������ж�����������ֵ���г�ʼ��
        if (playerAttributes != null)
        {
            currentHealth = playerAttributes.MaxHealth;
        }
        else
        {
            currentHealth = 100; 
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
    /// IDamageable �ӿڷ�����������ʱ����
    /// </summary>
    /// <param name="damage">ԭʼ�˺�ֵ</param>
    public void TakeDamage(int damage)
    {
        if (playerAttributes == null)
        {
            Debug.LogWarning("[PlayerHealthController] �޷������˺���ȱ�� PlayerAttributes��");
            return;
        }

        // ����������������� * 0.2��������ȡ��
        int reduction = Mathf.FloorToInt((playerAttributes.ArmorDefense + playerAttributes.HelmetDefense) * playerAttributes.DefenseMultiplier);
        int effectiveDamage = damage - reduction;
        if (effectiveDamage < 0) effectiveDamage = 0;

        currentHealth -= effectiveDamage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"[PlayerHealthController] TakeDamage: ԭʼ�˺�={damage}, ��������={reduction}, ʵ���˺�={effectiveDamage}");

        UpdateHealthUI();

        if (currentHealth <= 0)
            OnPlayerDied?.Invoke();
    }

    /// <summary>
    /// �ظ�����
    /// </summary>
    public void Heal(int amount)
    {
        if (playerAttributes == null) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, playerAttributes.MaxHealth);
        UpdateHealthUI();
    }

    /// <summary>
    /// ����������ͨ��������ʱ���ã�
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
