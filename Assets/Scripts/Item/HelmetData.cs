// HelmetData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Item/HelmetData")]
public class HelmetData : EquipmentData
{
    [Header("ͷ������")]
    public int DefenseValue;      // ͷ���ṩ�ķ���ֵ

    [Header("����Ч������ѡ��")]
    public float Weight;          // ͷ����������Ӱ���ٶȵȣ�
    public string Description;    // ͷ�������ı�
}
