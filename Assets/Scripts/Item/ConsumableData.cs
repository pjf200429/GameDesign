// ConsumableData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Item/ConsumableData")]
public class ConsumableData : ScriptableObject
{
    [Header("Detail")]
    public string ConsumableID;
    public string DisplayName;
    public Sprite Icon;

    [Header("Effect")]
    public int HealAmount = 0;
    [Tooltip("Buff type")]
    public BuffType BuffType = BuffType.None;
    [Tooltip("Buffduration")]
    public float BuffDuration = 0f;
    [Tooltip("Buff value")]
    public float BuffValue = 0f;

    [Header("stack")]
    [Tooltip("is stack")]
    public bool IsStackable = false;
    [Tooltip("price")]
    public int Price = 0;
}
