// HelmetData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Item/HelmetData")]
public class HelmetData : EquipmentData
{
    [Header("头盔属性")]
    public int DefenseValue;      // 头盔提供的防御值

    [Header("附加效果（可选）")]
    public float Weight;          // 头盔的重量（影响速度等）
    public string Description;    // 头盔描述文本
}
