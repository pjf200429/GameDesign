using UnityEngine;

public class EquipmentItem : ItemBase
{
    public WeaponData WeaponData { get; private set; }

    public EquipmentItem(string itemId, WeaponData data, string displayName, Sprite icon)
    {
        ItemID = itemId;
        WeaponData = data;
        DisplayName = displayName;
        Icon = icon;
    }

    public override ItemType Type => ItemType.Weapon;

    public override void Use(GameObject target)
    {
        // ��Ŀ������ϵ� WeaponSwitcher ����ʵ�ʻ����߼�
        // var switcher = target.GetComponent<WeaponSwitcher>();
        // if (switcher != null) switcher.SwitchTo(this);
    }
}
