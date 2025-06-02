// ItemBase.cs
// ������ࣺ������Ʒ���̳��Դ��࣬����ͨ�ýӿ�
using UnityEngine;

/// <summary>
/// ��Ʒ����ö�٣��������ֲ�ͬ������Ʒ������������Ʒ�ȣ�
/// </summary>
public enum ItemType
{
    Weapon,
    Consumable,
    // �Ժ�ɸ�����Ҫ��չ��Armor��Material��QuestItem ��
}

public abstract class ItemBase
{
    // Ψһ ID�����������ݿ�� DropTable ��Ӧ
    public string ItemID { get; protected set; }

    // ��ʾ�����ơ�ͼ���
    public string DisplayName { get; protected set; }
    public Sprite Icon { get; protected set; }

    // ÿ�����������д�����ض�Ӧ�� ItemType
    public abstract ItemType Type { get; }

    /// <summary>
    /// ʹ����Ʒ�ĳ��󷽷���������ʵ��
    /// target ͨ���� GameObject����ҡ����˵ȣ�
    /// </summary>
    public abstract void Use(GameObject target);
}
