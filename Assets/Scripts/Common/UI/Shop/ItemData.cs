using UnityEngine;

/// <summary>
/// 一件商品的数据定义：名称、图标、价格、描述等
/// </summary>
[CreateAssetMenu(fileName = "NewItemData", menuName = "Shop/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("物品基本信息")]
    public string itemName;      // 英文注释：Name of the item
    public Sprite icon;          // 英文注释：Icon sprite
    public int price;            // 英文注释：Price in game currency
    [TextArea] public string description; // 英文注释：Detailed description for “详情”按钮时显示

    // 你可以根据需要再加其他字段，比如稀有度、类型等等
}
