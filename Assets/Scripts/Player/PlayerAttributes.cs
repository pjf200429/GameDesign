// PlayerAttributes.cs
using UnityEngine;
using System;

/// <summary>
/// 玩家属性类：负责记录和管理玩家的核心属性，如血量上限、攻击力、防御力、移动速度、跳跃高度、货币数量、以及攻击倍率等
/// </summary>
public class PlayerAttributes : MonoBehaviour
{
    [Header("基础属性")]
    [Tooltip("玩家的最大生命值")]
    [SerializeField] private int maxHealth = 100;
    [Tooltip("玩家的攻击力")]
    [SerializeField] private int attack = 10;
    [Tooltip("玩家的防御力")]
    [SerializeField] private int defense = 5;

    [Header("移动相关属性")]
    [Tooltip("玩家的移动速度")]
    [SerializeField] private float movementSpeed = 5f;
    [Tooltip("玩家的跳跃高度")]
    [SerializeField] private float jumpHeight = 2f;

    [Header("攻击相关")]
    [Tooltip("玩家的攻击倍率，默认 1；实际伤害 = baseDamage * attackMultiplier")]
    [SerializeField] private float attackMultiplier = 1f;

    [Header("资源/货币")]
    [Tooltip("玩家持有的货币数量")]
    [SerializeField] private int currency = 0;

    // 只读访问器
    public int MaxHealth => maxHealth;
    public int Attack => attack;
    public int Defense => defense;
    public float MovementSpeed => movementSpeed;
    public float JumpHeight => jumpHeight;
    public float AttackMultiplier => attackMultiplier;
    public int Currency => currency;

    // 当前生命值（运行时会变化）
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

    // 事件
    public event Action<int, int> OnHealthChanged;      // (当前血量, 最大血量)
    public event Action<int> OnAttackChanged;           // (当前攻击力)
    public event Action<int> OnDefenseChanged;          // (当前防御力)
    public event Action<float> OnMovementSpeedChanged;  // (当前移动速度)
    public event Action<float> OnJumpHeightChanged;     // (当前跳跃高度)
    public event Action<float> OnAttackMultiplierChanged; // (当前攻击倍率)
    public event Action<int> OnCurrencyChanged;         // (当前货币)

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // 血量相关
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

    // 攻击力相关
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

    // 防御力相关
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

    // 移动相关
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

    // 攻击倍率相关
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

    // 货币相关
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
