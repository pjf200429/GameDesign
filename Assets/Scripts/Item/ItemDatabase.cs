// ItemDatabase.cs
// 单例，存储并实例化所有 ItemBase
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [Header("武器（WeaponData）配置列表")]
    [Tooltip("在 Inspector 中关联的所有 WeaponData（ScriptableObject）")]
    public WeaponData[] AllWeapons;

    [Header("消耗品（ConsumableData）配置列表")]
    [Tooltip("在 Inspector 中关联的所有 ConsumableData（ScriptableObject）")]
    public ConsumableData[] AllConsumables;

    // 内部映射：ID → WeaponData
    private Dictionary<string, WeaponData> _weaponMap = new Dictionary<string, WeaponData>();

    // 内部映射：ID → ConsumableData
    private Dictionary<string, ConsumableData> _consumableMap = new Dictionary<string, ConsumableData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
    
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 构建武器数据缓存
        if (AllWeapons != null)
        {
            foreach (var w in AllWeapons)
            {
                if (w == null)
                {
                    Debug.LogWarning("[ItemDatabase] AllWeapons 数组中存在 null 元素，已跳过。");
                    continue;
                }

                if (string.IsNullOrEmpty(w.WeaponID))
                {
                    Debug.LogWarning($"[ItemDatabase] WeaponData '{w.name}' 未设置 WeaponID，已跳过。");
                    continue;
                }

                if (!_weaponMap.ContainsKey(w.WeaponID))
                    _weaponMap[w.WeaponID] = w;
                else
                    Debug.LogWarning($"[ItemDatabase] 重复的 WeaponID: {w.WeaponID}（Asset 名称：{w.name}），已忽略后续重复项。");
            }
        }

        // 构建可消耗品数据缓存
        if (AllConsumables != null)
        {
            foreach (var c in AllConsumables)
            {
                if (c == null)
                {
                    Debug.LogWarning("[ItemDatabase] AllConsumables 数组中存在 null 元素，已跳过。");
                    continue;
                }

                if (string.IsNullOrEmpty(c.ConsumableID))
                {
                    Debug.LogWarning($"[ItemDatabase] ConsumableData '{c.name}' 未设置 ConsumableID，已跳过。");
                    continue;
                }

                if (!_consumableMap.ContainsKey(c.ConsumableID))
                    _consumableMap[c.ConsumableID] = c;
                else
                    Debug.LogWarning($"[ItemDatabase] 重复的 ConsumableID: {c.ConsumableID}（Asset 名称：{c.name}），已忽略后续重复项。");
            }
        }
    }

    /// <summary>
    /// 创建指定 ID 的 ItemBase 实例。
    /// 1. 优先从武器映射中查找 WeaponData 并构造 EquipmentItem；
    /// 2. 若未找到，再从可消耗品映射中查找 ConsumableData 并构造 ConsumableItem；
    /// 3. 都未找到则返回 null（并在控制台报错）。
    /// </summary>
    public ItemBase CreateItem(string itemID)
    {
        // 优先从武器数据里查找
        if (_weaponMap.TryGetValue(itemID, out var wdata))
        {
            // 用武器数据构造一个 EquipmentItem
            return new EquipmentItem(
                wdata.WeaponID,
                wdata,
                // 这里用 wdata.name 也可以改为 wdata.DisplayName（如果 WeaponData 定义了 DisplayName 字段）
                wdata.name,
                wdata.Icon
            );
        }

        // 如果武器里没找到，再尝试从可消耗品数据里查找
        if (_consumableMap.TryGetValue(itemID, out var cdata))
        {
            // 用可消耗品数据构造一个 ConsumableItem
            return new ConsumableItem(
                cdata.ConsumableID,
                cdata.DisplayName,
                cdata.Icon,
                cdata.HealAmount,
                cdata.BuffType,
                cdata.BuffDuration,
                cdata.BuffValue
            );
        }

        Debug.LogError($"[ItemDatabase] CreateItem 时传入了不存在的 itemID: {itemID}");
        return null;
    }

    /// <summary>
    /// （可选）示例：根据传入的类型，随机或条件获取多件 ItemBase，用于商店或掉落表。
    /// </summary>
    public List<ItemBase> GetRandomItems(int count, ItemType filterType)
    {
        var result = new List<ItemBase>();

        if (filterType == ItemType.Weapon)
        {
            var keys = new List<string>(_weaponMap.Keys);
            for (int i = 0; i < count; i++)
            {
                if (keys.Count == 0) break;
                int idx = Random.Range(0, keys.Count);
                result.Add(CreateItem(keys[idx]));
            }
        }
        else if (filterType == ItemType.Consumable)
        {
            var keys = new List<string>(_consumableMap.Keys);
            for (int i = 0; i < count; i++)
            {
                if (keys.Count == 0) break;
                int idx = Random.Range(0, keys.Count);
                result.Add(CreateItem(keys[idx]));
            }
        }

        return result;
    }
}
