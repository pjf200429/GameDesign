// PlayerAttributes.cs
using UnityEngine;
using System;

/// <summary>
/// 玩家属性类：负责记录和管理玩家的核心属性，如血量上限、攻击力、防御力（头盔防御 + 护甲防御 + 防御系数）、移动速度、跳跃高度、货币数量、以及攻击倍率等
/// </summary>
public class PlayerAttributes : MonoBehaviour
{
    [Header("基础属性")]
    [Tooltip("玩家的最大生命值")]
    [SerializeField] private int maxHealth = 100;
    [Tooltip("玩家的攻击力")]
    [SerializeField] private int attack = 10;

    [Header("防御相关")]
    [Tooltip("头盔提供的防御值")]
    [SerializeField] private int helmetDefense = 0;
    [Tooltip("护甲提供的防御值")]
    [SerializeField] private int armorDefense = 0;
    [Tooltip("可抵消的伤害=Defense的和*multiplier")]
    [SerializeField] private float defenseMultiplier = 0.2f;

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
    public int HelmetDefense => helmetDefense;
    public int ArmorDefense => armorDefense;
    public float DefenseMultiplier => defenseMultiplier;
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
    public event Action<int, int> OnHealthChanged;            // (当前血量, 最大血量)
    public event Action<int> OnAttackChanged;                 // (当前攻击力)
    public event Action<int> OnHelmetDefenseChanged;          // (当前头盔防御)
    public event Action<int> OnArmorDefenseChanged;           // (当前护甲防御)
    public event Action<float> OnDefenseMultiplierChanged;    // (当前防御系数)
    public event Action<float> OnMovementSpeedChanged;        // (当前移动速度)
    public event Action<float> OnJumpHeightChanged;           // (当前跳跃高度)
    public event Action<float> OnAttackMultiplierChanged;     // (当前攻击倍率)
    public event Action<int> OnCurrencyChanged;               // (当前货币)

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    #region 血量相关
    public void Heal(int amount)
    {
        if (amount <= 0) return;
        CurrentHealth += amount;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        // 计算总防御 = (头盔防御 + 护甲防御) * 防御系数
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

    #region 攻击力相关
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

    #region 防御相关
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

    #region 移动相关
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

    #region 攻击倍率相关
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

    #region 货币相关
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
