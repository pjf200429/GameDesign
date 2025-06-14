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
   
            return false;
        }

        // 如果背包有限制且已满
        if (maxSlots > 0 && _items.Count >= maxSlots)
        {
           
            return false;
        }

 
        if (item is ConsumableItem consumable)
        {
          
            var existing = _items.Find(x => x.ItemID == consumable.ItemID) as ConsumableItem;
            if (existing != null)
            {
             
                existing.AddQuantity(consumable.Quantity);
           
         
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

       
        _items.Add(item);
       

       
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


        if (item is ConsumableItem consumable)
        {
        
            if (consumable.Quantity <= 0)
            {
                _items.Remove(consumable);
                Debug.Log($"[PlayerInventory] {consumable.DisplayName} 数量为 0，已从背包移除。");
            }
        }
        else if (item is EquipmentItem equip)
        {
            Debug.Log($"[PlayerInventory] {equip.DisplayName} ");
            item.Use(target);
        }

       
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
  
        // 触发背包变化事件
        OnInventoryChanged?.Invoke();
    }

    #endregion
}
