using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ItemDatabase：负责把各类 ScriptableObject (WeaponData、ConsumableData、HelmetData、ArmorData) 
/// 按 ID 存到字典里，并提供按 ID 创建 ItemBase 的功能。  
/// 下面新增了一个“跨类型随机”接口：GetRandomItemsAllTypes
/// </summary>
public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [Header("武器（WeaponData）配置列表")]
    public WeaponData[] AllWeapons;

    [Header("消耗品（ConsumableData）配置列表")]
    public ConsumableData[] AllConsumables;

    [Header("头盔（HelmetData）配置列表")]
    public HelmetData[] AllHelmets;

    [Header("护甲（ArmorData）配置列表")]
    public ArmorData[] AllArmors;

    // 私有映射：EquipmentID/ConsumableID → 对应的 ScriptableObject
    private Dictionary<string, WeaponData> _weaponMap = new Dictionary<string, WeaponData>();
    private Dictionary<string, ConsumableData> _consumableMap = new Dictionary<string, ConsumableData>();
    private Dictionary<string, HelmetData> _helmetMap = new Dictionary<string, HelmetData>();
    private Dictionary<string, ArmorData> _armorMap = new Dictionary<string, ArmorData>();

    private void Awake()
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

        // 初始化 _weaponMap
        if (AllWeapons != null)
        {
            foreach (var w in AllWeapons)
            {
                if (w == null) continue;
                if (string.IsNullOrEmpty(w.EquipmentID)) continue;
                if (!_weaponMap.ContainsKey(w.EquipmentID))
                    _weaponMap[w.EquipmentID] = w;
            }
        }

        // 初始化 _consumableMap
        if (AllConsumables != null)
        {
            foreach (var c in AllConsumables)
            {
                if (c == null) continue;
                if (string.IsNullOrEmpty(c.ConsumableID)) continue;
                if (!_consumableMap.ContainsKey(c.ConsumableID))
                    _consumableMap[c.ConsumableID] = c;
            }
        }

        // 初始化 _helmetMap
        if (AllHelmets != null)
        {
            foreach (var h in AllHelmets)
            {
                if (h == null) continue;
                if (string.IsNullOrEmpty(h.EquipmentID)) continue;
                if (!_helmetMap.ContainsKey(h.EquipmentID))
                    _helmetMap[h.EquipmentID] = h;
            }
        }

        // 初始化 _armorMap
        if (AllArmors != null)
        {
            foreach (var a in AllArmors)
            {
                if (a == null) continue;
                if (string.IsNullOrEmpty(a.EquipmentID)) continue;
                if (!_armorMap.ContainsKey(a.EquipmentID))
                    _armorMap[a.EquipmentID] = a;
            }
        }
    }

    /// <summary>
    /// 创建指定 ID 的 ItemBase 实例（优先查武器、消耗品、头盔、护甲）。
    /// </summary>
    public ItemBase CreateItem(string itemID)
    {
        // (1) 武器
        if (_weaponMap.TryGetValue(itemID, out var wdata))
        {
            return new EquipmentItem(wdata.EquipmentID, wdata, wdata.Price);
        }

        // (2) 消耗品
        if (_consumableMap.TryGetValue(itemID, out var cdata))
        {
            return new ConsumableItem(
                cdata.ConsumableID,
                cdata.DisplayName,
                cdata.Icon,
                cdata.HealAmount,
                cdata.BuffType,
                cdata.BuffDuration,
                cdata.BuffValue,
                cdata.Price
            );
        }

        // (3) 头盔
        if (_helmetMap.TryGetValue(itemID, out var hdata))
        {
            return new EquipmentItem(hdata.EquipmentID, hdata, hdata.Price);
        }

        // (4) 护甲
        if (_armorMap.TryGetValue(itemID, out var adata))
        {
            return new EquipmentItem(adata.EquipmentID, adata, adata.Price);
        }

        Debug.LogError($"[ItemDatabase] CreateItem: 未找到 itemID = {itemID}");
        return null;
    }

    /// <summary>
    /// （原有）根据传入的类型，随机获取多件 ItemBase，用于商店或掉落表。
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
        else if (filterType == ItemType.Helmet)
        {
            var keys = new List<string>(_helmetMap.Keys);
            for (int i = 0; i < count; i++)
            {
                if (keys.Count == 0) break;
                int idx = Random.Range(0, keys.Count);
                result.Add(CreateItem(keys[idx]));
            }
        }
        else if (filterType == ItemType.Armor)
        {
            var keys = new List<string>(_armorMap.Keys);
            for (int i = 0; i < count; i++)
            {
                if (keys.Count == 0) break;
                int idx = Random.Range(0, keys.Count);
                result.Add(CreateItem(keys[idx]));
            }
        }

        return result;
    }

    /// <summary>
    /// 新增方法：从所有类型的映射中汇总出所有 ID，随机抽取 count 件 ItemBase。
    /// </summary>
    public List<ItemBase> GetRandomItemsAllTypes(int count)
    {
        // 1. 汇总所有类型的 Key（ID）
        List<string> allKeys = new List<string>();
        allKeys.AddRange(_weaponMap.Keys);
        allKeys.AddRange(_consumableMap.Keys);
        allKeys.AddRange(_helmetMap.Keys);
        allKeys.AddRange(_armorMap.Keys);

        // 2. Fisher–Yates 洗牌（或直接随机挑）
        for (int i = allKeys.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            // 交换
            string tmp = allKeys[i];
            allKeys[i] = allKeys[j];
            allKeys[j] = tmp;
        }

        // 3. 取前 count 个（如果不够就取所有）
        int actualCount = Mathf.Min(count, allKeys.Count);
        List<ItemBase> result = new List<ItemBase>();

        for (int i = 0; i < actualCount; i++)
        {
            ItemBase item = CreateItem(allKeys[i]);
            if (item != null)
                result.Add(item);
        }

        return result;
    }
}
