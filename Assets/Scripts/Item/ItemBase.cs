// ItemBase.cs
// ������ࣺ������Ʒ���̳��Դ��࣬����ͨ�ýӿ�
using UnityEngine;

public abstract class ItemBase
{
    // Ψһ ID�����������ݿ�� DropTable ��Ӧ
    public string ItemID { get; protected set; }

    // ��ʾ�����ơ�ͼ��ȿ��ɻ���洢����������չ
    public string DisplayName { get; protected set; }
    public Sprite Icon { get; protected set; }

    // ʹ����Ʒ�ĳ��󷽷���������ʵ��
    // target ͨ���� GameObject����ҡ����˵ȣ� 
    public abstract void Use(GameObject target);
}
