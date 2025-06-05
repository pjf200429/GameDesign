using UnityEngine;

/// <summary>
/// 统一处理所有“可装备物品”（Armor、Helmet、MeleeWeapon、RangedWeapon）
/// </summary>
public class EquipmentItem : ItemBase
{
    /// <summary>
    /// 既可以是 ArmorData、HelmetData、MeleeWeaponData 或 RangedWeaponData
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
            if (Data is ArmorData) return ItemType.Armor;
            if (Data is HelmetData) return ItemType.Helmet;
            if (Data is MeleeWeaponData) return ItemType.MeleeWeapon;
            if (Data is RangedWeaponData) return ItemType.RangedWeapon;
            // 默认回退（不建议用）：返回 Armor
            return ItemType.Armor;
        }
    }

    /// <summary>
    /// 根据 Data 的类型，调用目标身上的不同“切换器”或“触发器”来执行 Equip/Use 逻辑
    /// </summary>
    public override void Use(GameObject target)
    {
        if (target == null)
        {
            Debug.LogWarning("[EquipmentItem] Use 收到 null GameObject，跳过。");
            return;
        }

        // 直接从目标身上拿 WeaponSwitcher（不再拆分 Armor/Helmet/Melee/Ranged）
        var weaponSwitcher = target.GetComponent<WeaponSwitcher>();
        if (weaponSwitcher != null)
        {
            // 只要把当前这个 EquipmentItem（this）传给 SwitchTo，让后续 EquipWeapon 去做类型分支
            weaponSwitcher.SwitchTo(this);
        }
        else
        {
            Debug.LogWarning($"[EquipmentItem] 无法在目标物体上找到 WeaponSwitcher，无法装备物品 (ID: {ItemID}, Name: {DisplayName})。");
        }
    }

}
