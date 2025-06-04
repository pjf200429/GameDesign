// PlayerAttributes.cs
using UnityEngine;
using System;

/// <summary>
/// ��������ࣺ�����¼�͹�����ҵĺ������ԣ���Ѫ�����ޡ�����������������ͷ������ + ���׷��� + ����ϵ�������ƶ��ٶȡ���Ծ�߶ȡ������������Լ��������ʵ�
/// </summary>
public class PlayerAttributes : MonoBehaviour
{
    [Header("��������")]
    [Tooltip("��ҵ��������ֵ")]
    [SerializeField] private int maxHealth = 100;
    [Tooltip("��ҵĹ�����")]
    [SerializeField] private int attack = 10;

    [Header("�������")]
    [Tooltip("ͷ���ṩ�ķ���ֵ")]
    [SerializeField] private int helmetDefense = 0;
    [Tooltip("�����ṩ�ķ���ֵ")]
    [SerializeField] private int armorDefense = 0;
    [Tooltip("�ɵ������˺�=Defense�ĺ�*multiplier")]
    [SerializeField] private float defenseMultiplier = 0.2f;

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
    public int HelmetDefense => helmetDefense;
    public int ArmorDefense => armorDefense;
    public float DefenseMultiplier => defenseMultiplier;
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
    public event Action<int, int> OnHealthChanged;            // (��ǰѪ��, ���Ѫ��)
    public event Action<int> OnAttackChanged;                 // (��ǰ������)
    public event Action<int> OnHelmetDefenseChanged;          // (��ǰͷ������)
    public event Action<int> OnArmorDefenseChanged;           // (��ǰ���׷���)
    public event Action<float> OnDefenseMultiplierChanged;    // (��ǰ����ϵ��)
    public event Action<float> OnMovementSpeedChanged;        // (��ǰ�ƶ��ٶ�)
    public event Action<float> OnJumpHeightChanged;           // (��ǰ��Ծ�߶�)
    public event Action<float> OnAttackMultiplierChanged;     // (��ǰ��������)
    public event Action<int> OnCurrencyChanged;               // (��ǰ����)

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    #region Ѫ�����
    public void Heal(int amount)
    {
        if (amount <= 0) return;
        CurrentHealth += amount;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        // �����ܷ��� = (ͷ������ + ���׷���) * ����ϵ��
        float totalDefFloat = (helmetDefense + armorDefense) * defenseMultiplier;
        int totalDefense = Mathf.FloorToInt(totalDefFloat);

        int effectiveDamage = Mathf.Max(1, amount - totalDefense);
        CurrentHealth -= effectiveDamage;
    }

    public void RestoreFullHealth()
    {
        CurrentHealth = maxHealth;
    }
    #endregion

    #region ���������
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
    #endregion

    #region �������
    public void AddHelmetDefense(int amount)
    {
        if (amount == 0) return;
        helmetDefense = Mathf.Max(0, helmetDefense + amount);
        OnHelmetDefenseChanged?.Invoke(helmetDefense);
    }

    public void SetHelmetDefense(int newHelmetDefense)
    {
        helmetDefense = Mathf.Max(0, newHelmetDefense);
        OnHelmetDefenseChanged?.Invoke(helmetDefense);
    }

    public void AddArmorDefense(int amount)
    {
        if (amount == 0) return;
        armorDefense = Mathf.Max(0, armorDefense + amount);
        OnArmorDefenseChanged?.Invoke(armorDefense);
    }

    public void SetArmorDefense(int newArmorDefense)
    {
        armorDefense = Mathf.Max(0, newArmorDefense);
        OnArmorDefenseChanged?.Invoke(armorDefense);
    }

    public void AddDefenseMultiplier(float amount)
    {
        if (Mathf.Approximately(amount, 0f)) return;
        defenseMultiplier = Mathf.Max(0f, defenseMultiplier + amount);
        OnDefenseMultiplierChanged?.Invoke(defenseMultiplier);
    }

    public void SetDefenseMultiplier(float newMultiplier)
    {
        defenseMultiplier = Mathf.Max(0f, newMultiplier);
        OnDefenseMultiplierChanged?.Invoke(defenseMultiplier);
    }
    #endregion

    #region �ƶ����
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
    #endregion

    #region �����������
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
    #endregion

    #region �������
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
    #endregion
}
