using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家背包管理类：负责存储玩家当前拥有的所有物品实例，
/// 并提供添加、移除、使用、装备等接口，同时触发背包变化事件。
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    [Header("背包容量（-1 表示无限制）")]
    [Tooltip("-1 表示不限制背包大小")]
    public int maxSlots = -1;

    // 存放所有 ItemBase 的列表
    private List<ItemBase> _items = new List<ItemBase>();

    /// <summary>
    /// 只读访问：获取背包内所有物品（按“实例列表”返回）
    /// </summary>
    public IReadOnlyList<ItemBase> Items => _items.AsReadOnly();

    //------------- 可选：当前已装备的武器 -------------
    private EquipmentItem _equippedWeapon;
    /// <summary>
    /// 当前装备的武器（如果没有装备，则返回 null）
    /// </summary>
    public EquipmentItem EquippedWeapon => _equippedWeapon;

    /// <summary>
    /// 当背包内容发生变化时触发（包括 AddItem、RemoveItem、UseItem、EquipWeapon 内部均会调用）
    /// 外部（如 InventoryUI）可订阅此事件来刷新界面。
    /// </summary>
    public event Action OnInventoryChanged;

    private void Awake()
    {
        // 如果需要在场景切换时保留背包，可取消注释下面这行
        // DontDestroyOnLoad(gameObject);
    }

    #region 添加物品

    /// <summary>
    /// 向背包添加一个 ItemBase 实例
    /// </summary>
    /// <param name="item">通过 ItemDatabase.CreateItem(... ) 得到的实例</param>
    /// <returns>添加成功返回 true；如果背包满或 item 为 null 则返回 false</returns>
    public bool AddItem(ItemBase item)
    {
        if (item == null)
        {
            Debug.LogWarning("[PlayerInventory] AddItem 传入了 null，跳过。");
            return false;
        }

        // 如果背包有限制且已满
        if (maxSlots > 0 && _items.Count >= maxSlots)
        {
            Debug.LogWarning("[PlayerInventory] 背包已满，无法添加：" + item.DisplayName);
            return false;
        }

        // 如果是 ConsumableItem，需要判断是否与已有同 ID 项目合并堆叠
        if (item is ConsumableItem consumable)
        {
            // 找到背包里第一个相同 ID 的 ConsumableItem
            var existing = _items.Find(x => x.ItemID == consumable.ItemID) as ConsumableItem;
            if (existing != null)
            {
                // 合并堆叠：只增加数量，不新增列表项
                existing.AddQuantity(consumable.Quantity);
                Debug.Log($"[PlayerInventory] 堆叠消耗品：{existing.DisplayName} 数量 += {consumable.Quantity}，当前数量 = {existing.Quantity}");
                // 触发背包变化事件并返回
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        // 否则，直接添加到列表中
        _items.Add(item);
        Debug.Log($"[PlayerInventory] 添加物品：{item.DisplayName} (ID: {item.ItemID})");

        //// 如果添加的是武器，且当前还没有装备，则默认自动装备（可根据需求删掉此段）
        //if (item is EquipmentItem equip && _equippedWeapon == null)
        //{
        //    EquipWeapon(equip);
        //}

        // 触发背包变化事件
        OnInventoryChanged?.Invoke();
        return true;
    }

    #endregion

    #region 移除物品

    /// <summary>
    /// 从背包移除一个物品实例
    /// </summary>
    /// <param name="item">要移除的实例</param>
    /// <returns>移除成功返回 true，否则返回 false</returns>
    public bool RemoveItem(ItemBase item)
    {
        if (item == null) return false;

        bool removed = _items.Remove(item);
        if (removed)
        {
            Debug.Log($"[PlayerInventory] 移除物品：{item.DisplayName} (ID: {item.ItemID})");

            // 如果移除的是当前装备的武器，需要取消装备
            if (item is EquipmentItem && _equippedWeapon == item)
            {
                _equippedWeapon = null;
                Debug.Log("[PlayerInventory] 装备已被移除，当前无武器装备。");
                // TODO：可触发切换到空手或随机下一个武器
            }

            // 触发背包变化事件
            OnInventoryChanged?.Invoke();
        }
        return removed;
    }

    #endregion

    #region 使用物品

    /// <summary>
    /// 使用背包中的某个物品（实例），对目标执行效果。  
    /// 通常 target 是玩家自己（gameObject），也可根据需求传入其他对象。
    /// 如果是 ConsumableItem，使用后会自动从背包中移除或减少数量。
    /// 如果是 EquipmentItem，使用逻辑一般只是“装备或切换”，不移除。
    /// </summary>
    /// <param name="item">要使用的实例</param>
    /// <param name="target">使用对象（通常是玩家自己）</param>
    public void UseItem(ItemBase item, GameObject target)
    {
        if (item == null || !_items.Contains(item)) return;

        item.Use(target);

        // 如果是 ConsumableItem，要一旦“吃掉”就减少数量或移除背包
        if (item is ConsumableItem consumable)
        {
            consumable.ReduceQuantity(1);
            Debug.Log($"[PlayerInventory] 消耗品 {consumable.DisplayName} 使用后数量 = {consumable.Quantity}");
            if (consumable.Quantity <= 0)
            {
                _items.Remove(consumable);
                Debug.Log($"[PlayerInventory] {consumable.DisplayName} 数量为 0，已从背包移除。");
            }
        }
        else if (item is EquipmentItem equip)
        {
            // 如果是装备，先调用 EquipWeapon
            EquipWeapon(equip);
            // 不从列表移除
        }

        // 触发背包变化事件
        OnInventoryChanged?.Invoke();
    }

    #endregion

    #region 装备武器

    /// <summary>
    /// 装备某把武器（前提：该武器实例已经在背包里）
    /// </summary>
    /// <param name="weapon">要装备的 Weapon</param>
    public void EquipWeapon(EquipmentItem weapon)
    {
        if (weapon == null)
        {
            Debug.LogWarning("[PlayerInventory] EquipWeapon 传入了 null，忽略。");
            return;
        }

        if (!_items.Contains(weapon))
        {
            Debug.LogWarning($"[PlayerInventory] 背包中不存在武器：{weapon.DisplayName}");
            return;
        }

        _equippedWeapon = weapon;
        Debug.Log($"[PlayerInventory] 装备武器：{weapon.DisplayName}");

        // 如果你有一个 WeaponSwitcher 或 PlayerCombat，用来将当前 WeaponData 绑定到玩家手上：
        var switcher = GetComponent<WeaponSwitcher>();
        if (switcher != null)
        {
            switcher.SwitchTo(weapon);
        }

        // 触发背包变化事件（如果 UI 或其它逻辑要显示当前装备）
        OnInventoryChanged?.Invoke();
    }

    #endregion

    #region 查询接口

    /// <summary>
    /// 检查背包中是否已有指定 ID 的物品（按实例比较或按 ID 比较都可）
    /// </summary>
    public bool HasItem(string itemID)
    {
        foreach (var it in _items)
        {
            if (it.ItemID == itemID) return true;
        }
        return false;
    }

    /// <summary>
    /// 获取背包里指定 ID 的第一个实例（如果有），没有则返回 null
    /// </summary>
    public ItemBase GetItem(string itemID)
    {
        return _items.Find(it => it.ItemID == itemID);
    }

    #endregion

    #region （可选）清空背包

    /// <summary>
    /// 清空背包里所有物品（慎用）  
    /// </summary>
    public void ClearAll()
    {
        _items.Clear();
        _equippedWeapon = null;
        Debug.Log("[PlayerInventory] 背包已清空。");
        // 触发背包变化事件
        OnInventoryChanged?.Invoke();
    }

    #endregion
}
