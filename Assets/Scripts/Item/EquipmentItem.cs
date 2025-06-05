using UnityEngine;

/// <summary>
/// ͳһ�������С���װ����Ʒ����Armor��Helmet��MeleeWeapon��RangedWeapon��
/// </summary>
public class EquipmentItem : ItemBase
{
    /// <summary>
    /// �ȿ����� ArmorData��HelmetData��MeleeWeaponData �� RangedWeaponData
    /// </summary>
    public EquipmentData Data { get; private set; }

    /// <summary>
    /// ���캯�������� itemId����Ӧ�� EquipmentData �ͼ۸�
    /// </summary>
    public EquipmentItem(string itemId, EquipmentData data, int price)
        : base(itemId, data.DisplayName, data.Icon, price)
    {
        Data = data;
    }

    /// <summary>
    /// ���� Data ��ʵ�����ͣ����ض�Ӧ�� ItemType
    /// </summary>
    public override ItemType Type
    {
        get
        {
            if (Data is ArmorData) return ItemType.Armor;
            if (Data is HelmetData) return ItemType.Helmet;
            if (Data is MeleeWeaponData) return ItemType.MeleeWeapon;
            if (Data is RangedWeaponData) return ItemType.RangedWeapon;
            // Ĭ�ϻ��ˣ��������ã������� Armor
            return ItemType.Armor;
        }
    }

    /// <summary>
    /// ���� Data �����ͣ�����Ŀ�����ϵĲ�ͬ���л������򡰴���������ִ�� Equip/Use �߼�
    /// </summary>
    public override void Use(GameObject target)
    {
        if (target == null)
        {
            Debug.LogWarning("[EquipmentItem] Use �յ� null GameObject��������");
            return;
        }

        // ֱ�Ӵ�Ŀ�������� WeaponSwitcher�����ٲ�� Armor/Helmet/Melee/Ranged��
        var weaponSwitcher = target.GetComponent<WeaponSwitcher>();
        if (weaponSwitcher != null)
        {
            // ֻҪ�ѵ�ǰ��� EquipmentItem��this������ SwitchTo���ú��� EquipWeapon ȥ�����ͷ�֧
            weaponSwitcher.SwitchTo(this);
        }
        else
        {
            Debug.LogWarning($"[EquipmentItem] �޷���Ŀ���������ҵ� WeaponSwitcher���޷�װ����Ʒ (ID: {ItemID}, Name: {DisplayName})��");
        }
    }

}
