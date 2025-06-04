using UnityEngine;

/// <summary>
/// ͳһ�������С���װ����Ʒ����Weapon��Helmet��Armor����
/// </summary>
public class EquipmentItem : ItemBase
{
    /// <summary>
    /// �ȿ����� WeaponData��Ҳ������ HelmetData �� ArmorData��
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
            if (Data is WeaponData) return ItemType.Weapon;
            if (Data is HelmetData) return ItemType.Helmet;
            if (Data is ArmorData) return ItemType.Armor;
            // Ĭ�ϻ���Ϊ Weapon����������
            return ItemType.Weapon;
        }
    }

    /// <summary>
    /// ���� Data �����ͣ�����Ŀ�����ϵĲ�ͬ���л�������ִ�� Equip/Use �߼�
    /// </summary>
    public override void Use(GameObject target)
    {
        // ����װ���߼�
        if (Data is WeaponData)
        {
            var weaponSwitcher = target.GetComponent<WeaponSwitcher>();
            if (weaponSwitcher != null)
                weaponSwitcher.SwitchTo(this);
            else
                Debug.LogWarning("[EquipmentItem] �Ҳ��� WeaponSwitcher���޷�װ��������");
        }
        // ͷ��װ���߼�
        else if (Data is HelmetData)
        {
            var helmetSwitcher = target.GetComponent<WeaponSwitcher>();
            if (helmetSwitcher != null)
                helmetSwitcher.SwitchTo(this);
            else
                Debug.LogWarning("[EquipmentItem] �Ҳ��� HelmetSwitcher���޷�װ��ͷ����");
        }
        // ����װ���߼�
        else if (Data is ArmorData)
        {
            var armorSwitcher = target.GetComponent<WeaponSwitcher>();
            if (armorSwitcher != null)
                armorSwitcher.SwitchTo(this);
            else
                Debug.LogWarning("[EquipmentItem] �Ҳ��� ArmorSwitcher���޷�װ�����ס�");
        }
        else
        {
            Debug.LogWarning($"[EquipmentItem] ��֧�ֵĿ�װ�����ͣ�{Data.GetType().Name}");
        }
    }
}
