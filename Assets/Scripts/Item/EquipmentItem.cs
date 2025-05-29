// EquipmentItem.cs
// 装备类物品：例如武器，包含 WeaponData 引用
using UnityEngine;

public class EquipmentItem : ItemBase
{
    // 关联的武器数据对象，包含伤害、特效、动画等配置
    public WeaponData WeaponData { get; private set; }

    // 构造函数由 ItemDatabase 调用，传入读取到的 WeaponData
    public EquipmentItem(string itemId, WeaponData data, string displayName, Sprite icon)
    {
        ItemID = itemId;
        WeaponData = data;
        DisplayName = displayName;
        Icon = icon;
    }

    // 装备物品的 Use 实现：通常是去武器切换器里装备它
    public override void Use(GameObject target)
    {
        // 假设目标有 WeaponSwitcher 组件
        //var switcher = target.GetComponent<WeaponSwitcher>();
        //if (switcher != null)
        //{
        //    switcher.SwitchTo(this);
        //}
    }
}
