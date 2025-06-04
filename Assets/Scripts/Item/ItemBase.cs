// ItemBase.cs
using UnityEngine;

/// <summary>
/// Item ����ö�٣��������ֲ�ͬ������Ʒ������������Ʒ��ͷ�������׵ȣ�
/// </summary>
public enum ItemType
{
    Weapon,
    Consumable,
    Helmet,
    Armor
    // ����ɸ�����Ҫ��չ��Material��QuestItem ��
}

/// <summary>
/// ������ࣺ������Ʒ���̳��Դ��࣬����ͨ�ýӿ�
/// </summary>
public abstract class ItemBase
{
    // Ψһ ID�����ڸ����ݿ⡢������Ӧ
    public string ItemID { get; protected set; }

    // UI ��ʾ���ơ�ͼ��
    public string DisplayName { get; protected set; }
    public Sprite Icon { get; protected set; }

    // �۸�ֻ�б������������ڹ���ʱ��ֵ���ⲿ�޷������
    public int Price { get; protected set; }

    // ÿ�����������д�����ض�Ӧ�� ItemType
    public abstract ItemType Type { get; }

    /// <summary>
    /// ���캯�����������඼��Ҫ�ڴ���ʱ���� price
    /// </summary>
    protected ItemBase(string itemId, string displayName, Sprite icon, int price)
    {
        ItemID = itemId;
        DisplayName = displayName;
        Icon = icon;
        Price = price;
    }

    /// <summary>
    /// ʹ����Ʒ�ĳ��󷽷���������ʵ��
    /// target ͨ���� GameObject����ҡ����˵ȣ�
    /// </summary>
    public abstract void Use(GameObject target);
}
