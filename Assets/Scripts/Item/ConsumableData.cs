// ConsumableData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Item/ConsumableData")]
public class ConsumableData : ScriptableObject
{
    [Header("������Ϣ")]
    [Tooltip("��������Ʒ��Ψһ ID�������� ItemDatabase �в���")]
    public string ConsumableID;
    [Tooltip("�� UI ����ʾ������")]
    public string DisplayName;
    [Tooltip("��Ʒͼ�꣨Inventory/����Ƚ���չʾ��")]
    public Sprite Icon;

    [Header("ʹ��Ч��")]
    [Tooltip("ֱ�ӻ�Ѫ��������������Ϳɻ�Ѫ��")]
    public int HealAmount = 0;
    [Tooltip("Ҫʩ�ӵ� Buff ���ͣ�ö�� BuffType�����ж��壩")]
    public BuffType BuffType = BuffType.None;
    [Tooltip("Buff ����ʱ�䣬��λ���롣������� Buff ���� 0")]
    public float BuffDuration = 0f;
    [Tooltip("Buff ǿ����ֵ�����繥�������ٷֱȣ����߼���ֵ��")]
    public float BuffValue = 0f;

    [Header("��ѡ���ѵ����۸���Ϣ")]
    [Tooltip("�Ƿ�ɶѵ��������ͬ ID��")]
    public bool IsStackable = false;
    [Tooltip("�����ۼۣ���δ���̵�����")]
    public int Price = 0;
}
