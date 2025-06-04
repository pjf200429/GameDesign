using UnityEngine;

/// <summary>
/// һ����Ʒ�����ݶ��壺���ơ�ͼ�ꡢ�۸�������
/// </summary>
[CreateAssetMenu(fileName = "NewItemData", menuName = "Shop/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("��Ʒ������Ϣ")]
    public string itemName;      // Ӣ��ע�ͣ�Name of the item
    public Sprite icon;          // Ӣ��ע�ͣ�Icon sprite
    public int price;            // Ӣ��ע�ͣ�Price in game currency
    [TextArea] public string description; // Ӣ��ע�ͣ�Detailed description for �����顱��ťʱ��ʾ

    // ����Ը�����Ҫ�ټ������ֶΣ�����ϡ�жȡ����͵ȵ�
}
