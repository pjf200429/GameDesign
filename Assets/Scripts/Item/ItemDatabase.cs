// ItemDatabase.cs
// 单例，存储并实例化所有 ItemBase
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    // 在 Inspector 中关联的所有 WeaponData（ScriptableObject）
    public WeaponData[] AllWeapons;
    // 消耗品等其它数据也可以类似存储或从 JSON 加载
    // public ConsumableData[] AllConsumables;

    // 内部映射：ID → 数据资产
    Dictionary<string, WeaponData> _weaponMap = new Dictionary<string, WeaponData>();
    // …可为 ConsumableData 建立类似映射

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // 构建缓存
        foreach (var w in AllWeapons)
            _weaponMap[w.WeaponID] = w;
        // ConsumableData 同理
    }

    /// <summary>
    /// 创建指定 ID 的 ItemBase 实例
    /// </summary>
    public ItemBase CreateItem(string itemID)
    {
        // 优先从武器里找
        if (_weaponMap.TryGetValue(itemID, out var wdata))
        {
            // 用武器数据构造 EquipmentItem
            return new EquipmentItem(
                wdata.WeaponID,
                wdata,
                wdata.name,      // 或 wdata.DisplayName
                wdata.Icon       // 如果在 WeaponData 中定义了 Icon
            );
        }

        // 若要支持 ConsumableItem，可在此继续分支判定
        // if (_consumableMap.TryGetValue(itemID, out var cdata)) { … }

        Debug.LogError($"[ItemDatabase] 未知的 itemID: {itemID}");
        return null;
    }

    /// <summary>
    /// 随机或条件获取多件 ItemBase，用于商店或掉落表
    /// </summary>
   
}
