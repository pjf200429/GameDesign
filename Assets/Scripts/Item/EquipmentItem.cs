// EquipmentItem.cs
// װ������Ʒ���������������� WeaponData ����
using UnityEngine;

public class EquipmentItem : ItemBase
{
    // �������������ݶ��󣬰����˺�����Ч������������
    public WeaponData WeaponData { get; private set; }

    // ���캯���� ItemDatabase ���ã������ȡ���� WeaponData
    public EquipmentItem(string itemId, WeaponData data, string displayName, Sprite icon)
    {
        ItemID = itemId;
        WeaponData = data;
        DisplayName = displayName;
        Icon = icon;
    }

    // װ����Ʒ�� Use ʵ�֣�ͨ����ȥ�����л�����װ����
    public override void Use(GameObject target)
    {
        // ����Ŀ���� WeaponSwitcher ���
        //var switcher = target.GetComponent<WeaponSwitcher>();
        //if (switcher != null)
        //{
        //    switcher.SwitchTo(this);
        //}
    }
}
