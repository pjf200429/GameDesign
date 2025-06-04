using UnityEngine;

/// <summary>
/// ���С���װ�������ݣ�������ͷ�������ף����̳����������
/// </summary>
public abstract class EquipmentData : ScriptableObject
{
    [Header("������Ϣ")]
    public string EquipmentID;    // Ψһ��ʶ���������๲��
    public string DisplayName;    // UI ��ʾ����
    public Sprite Icon;           // ��Ʒͼ��
    [Tooltip("�����ۼۣ���δ���̵�����")]
    public int Price = 0;
}
