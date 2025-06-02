using UnityEngine;

[System.Serializable]
public class DropTableEntry
{
    [Tooltip("要掉落的 ItemID，对应 ItemDatabase 中注册的 ItemID")]
    public string ItemID;

    [Tooltip("百分比概率，比如 0.0 - 1.0")]
    [Range(0f, 1f)]
    public float DropChance = 0.1f;

    [Tooltip("掉落数量最小值")]
    public int MinCount = 1;

    [Tooltip("掉落数量最大值")]
    public int MaxCount = 1;
}
