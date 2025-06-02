using UnityEngine;

[System.Serializable]
public class DropTableEntry
{
    [Tooltip("Ҫ����� ItemID����Ӧ ItemDatabase ��ע��� ItemID")]
    public string ItemID;

    [Tooltip("�ٷֱȸ��ʣ����� 0.0 - 1.0")]
    [Range(0f, 1f)]
    public float DropChance = 0.1f;

    [Tooltip("����������Сֵ")]
    public int MinCount = 1;

    [Tooltip("�����������ֵ")]
    public int MaxCount = 1;
}
