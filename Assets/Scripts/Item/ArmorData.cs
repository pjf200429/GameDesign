// ArmorData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Item/ArmorData")]
public class ArmorData : EquipmentData
{
    [Header("��������")]
    public int DefenseValue;      // �����ṩ�ķ���ֵ

    [Header("��������")]
    public float Weight;          // ���׵�������Ӱ���ٶȵȣ�
   // public float Durability;      // �����;öȣ���ѡ�������;����ģ�
    public string SetName;        // �������װЧ��������д��װ����
}
