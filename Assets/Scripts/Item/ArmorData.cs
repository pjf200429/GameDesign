// ArmorData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Item/ArmorData")]
public class ArmorData : EquipmentData
{
    [Header("护甲属性")]
    public int DefenseValue;      // 护甲提供的防御值

    [Header("附加属性")]
    public float Weight;          // 护甲的重量（影响速度等）
   // public float Durability;      // 护甲耐久度（可选，用于耐久消耗）
    public string SetName;        // 如果有套装效果，可填写套装名称
}
