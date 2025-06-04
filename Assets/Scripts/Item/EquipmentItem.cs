using UnityEngine;

/// <summary>
/// 统一处理所有“可装备物品”（Weapon、Helmet、Armor）。
/// </summary>
public class EquipmentItem : ItemBase
{
    /// <summary>
    /// 既可以是 WeaponData，也可以是 HelmetData 或 ArmorData。
    /// </summary>
    public EquipmentData Data { get; private set; }

    /// <summary>
    /// 构造函数：传入 itemId、对应的 EquipmentData 和价格
    /// </summary>
    public EquipmentItem(string itemId, EquipmentData data, int price)
        : base(itemId, data.DisplayName, data.Icon, price)
    {
        Data = data;
    }

    /// <summary>
    /// 根据 Data 的实际类型，返回对应的 ItemType
    /// </summary>
    public override ItemType Type
    {
        get
        {
            if (Data is WeaponData) return ItemType.Weapon;
            if (Data is HelmetData) return ItemType.Helmet;
            if (Data is ArmorData) return ItemType.Armor;
            // 默认回退为 Weapon，仅作保底
            return ItemType.Weapon;
        }
    }

    /// <summary>
    /// 根据 Data 的类型，调用目标身上的不同“切换器”来执行 Equip/Use 逻辑
    /// </summary>
    public override void Use(GameObject target)
    {
        // 武器装备逻辑
        if (Data is WeaponData)
        {
            var weaponSwitcher = target.GetComponent<WeaponSwitcher>();
            if (weaponSwitcher != null)
                weaponSwitcher.SwitchTo(this);
            else
                Debug.LogWarning("[EquipmentItem] 找不到 WeaponSwitcher，无法装备武器。");
        }
        // 头盔装备逻辑
        else if (Data is HelmetData)
        {
            var helmetSwitcher = target.GetComponent<WeaponSwitcher>();
            if (helmetSwitcher != null)
                helmetSwitcher.SwitchTo(this);
            else
                Debug.LogWarning("[EquipmentItem] 找不到 HelmetSwitcher，无法装备头盔。");
        }
        // 护甲装备逻辑
        else if (Data is ArmorData)
        {
            var armorSwitcher = target.GetComponent<WeaponSwitcher>();
            if (armorSwitcher != null)
                armorSwitcher.SwitchTo(this);
            else
                Debug.LogWarning("[EquipmentItem] 找不到 ArmorSwitcher，无法装备护甲。");
        }
        else
        {
            Debug.LogWarning($"[EquipmentItem] 不支持的可装备类型：{Data.GetType().Name}");
        }
    }
}
