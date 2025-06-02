// PlayerAttributes.cs
using UnityEngine;
using System;

/// <summary>
/// ��������ࣺ�����¼�͹�����ҵĺ������ԣ���Ѫ�����ޡ������������������ƶ��ٶȡ���Ծ�߶ȡ������������Լ��������ʵ�
/// </summary>
public class PlayerAttributes : MonoBehaviour
{
    [Header("��������")]
    [Tooltip("��ҵ��������ֵ")]
    [SerializeField] private int maxHealth = 100;
    [Tooltip("��ҵĹ�����")]
    [SerializeField] private int attack = 10;
    [Tooltip("��ҵķ�����")]
    [SerializeField] private int defense = 5;

    [Header("�ƶ��������")]
    [Tooltip("��ҵ��ƶ��ٶ�")]
    [SerializeField] private float movementSpeed = 5f;
    [Tooltip("��ҵ���Ծ�߶�")]
    [SerializeField] private float jumpHeight = 2f;

    [Header("�������")]
    [Tooltip("��ҵĹ������ʣ�Ĭ�� 1��ʵ���˺� = baseDamage * attackMultiplier")]
    [SerializeField] private float attackMultiplier = 1f;

    [Header("��Դ/����")]
    [Tooltip("��ҳ��еĻ�������")]
    [SerializeField] private int currency = 0;

    // ֻ��������
    public int MaxHealth => maxHealth;
    public int Attack => attack;
    public int Defense => defense;
    public float MovementSpeed => movementSpeed;
    public float JumpHeight => jumpHeight;
    public float AttackMultiplier => attackMultiplier;
    public int Currency => currency;

    // ��ǰ����ֵ������ʱ��仯��
    private int currentHealth;
    public int CurrentHealth
    {
        get => currentHealth;
        private set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }

    // �¼�
    public event Action<int, int> OnHealthChanged;      // (��ǰѪ��, ���Ѫ��)
    public event Action<int> OnAttackChanged;           // (��ǰ������)
    public event Action<int> OnDefenseChanged;          // (��ǰ������)
    public event Action<float> OnMovementSpeedChanged;  // (��ǰ�ƶ��ٶ�)
    public event Action<float> OnJumpHeightChanged;     // (��ǰ��Ծ�߶�)
    public event Action<float> OnAttackMultiplierChanged; // (��ǰ��������)
    public event Action<int> OnCurrencyChanged;         // (��ǰ����)

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // Ѫ�����
    public void Heal(int amount)
    {
        if (amount <= 0) return;
        CurrentHealth += amount;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        int effectiveDamage = Mathf.Max(1, amount - defense);
        CurrentHealth -= effectiveDamage;
    }

    public void RestoreFullHealth()
    {
        CurrentHealth = maxHealth;
    }

    // ���������
    public void AddAttack(int amount)
    {
        if (amount == 0) return;
        attack = Mathf.Max(0, attack + amount);
        OnAttackChanged?.Invoke(attack);
    }

    public void SetAttack(int newAttack)
    {
        attack = Mathf.Max(0, newAttack);
        OnAttackChanged?.Invoke(attack);
    }

    // ���������
    public void AddDefense(int amount)
    {
        if (amount == 0) return;
        defense = Mathf.Max(0, defense + amount);
        OnDefenseChanged?.Invoke(defense);
    }

    public void SetDefense(int newDefense)
    {
        defense = Mathf.Max(0, newDefense);
        OnDefenseChanged?.Invoke(defense);
    }

    // �ƶ����
    public void AddMovementSpeed(float amount)
    {
        if (Mathf.Approximately(amount, 0f)) return;
        movementSpeed = Mathf.Max(0f, movementSpeed + amount);
        OnMovementSpeedChanged?.Invoke(movementSpeed);
    }

    public void SetMovementSpeed(float newSpeed)
    {
        movementSpeed = Mathf.Max(0f, newSpeed);
        OnMovementSpeedChanged?.Invoke(movementSpeed);
    }

    public void AddJumpHeight(float amount)
    {
        if (Mathf.Approximately(amount, 0f)) return;
        jumpHeight = Mathf.Max(0f, jumpHeight + amount);
        OnJumpHeightChanged?.Invoke(jumpHeight);
    }

    public void SetJumpHeight(float newHeight)
    {
        jumpHeight = Mathf.Max(0f, newHeight);
        OnJumpHeightChanged?.Invoke(jumpHeight);
    }

    // �����������
    public void AddAttackMultiplier(float amount)
    {
        if (Mathf.Approximately(amount, 0f)) return;
        attackMultiplier = Mathf.Max(0f, attackMultiplier + amount);
        OnAttackMultiplierChanged?.Invoke(attackMultiplier);
    }

    public void SetAttackMultiplier(float newMultiplier)
    {
        attackMultiplier = Mathf.Max(0f, newMultiplier);
        OnAttackMultiplierChanged?.Invoke(attackMultiplier);
    }

    // �������
    public void AddCurrency(int amount)
    {
        if (amount == 0) return;
        currency = Mathf.Max(0, currency + amount);
        OnCurrencyChanged?.Invoke(currency);
    }

    public bool SpendCurrency(int amount)
    {
        if (amount <= 0) return true;
        if (currency < amount) return false;
        currency -= amount;
        OnCurrencyChanged?.Invoke(currency);
        return true;
    }

    public void SetCurrency(int newAmount)
    {
        currency = Mathf.Max(0, newAmount);
        OnCurrencyChanged?.Invoke(currency);
    }
}
