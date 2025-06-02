using UnityEngine;

/// <summary>
/// ��������Ʒ�ࣺ�̳��� ItemBase��֧�ֻ�Ѫ�͹� Buff �Ĺ���
/// </summary>
public class ConsumableItem : ItemBase
{
    // ������Ʒʹ�ú�ظ�������ֵ
    public int HealAmount { get; private set; }

    // ��Ӧ�� Buff ���ͣ�������� Buff ���ߣ�����Ϊ BuffType.None��
    public BuffType BuffType { get; private set; }

    // Buff ����ʱ�����룩
    public float BuffDuration { get; private set; }

    // Buff ��ֵ�����磺DefenseUp �� 30��AttackUp �� 0.3f �ȣ�
    public float BuffValue { get; private set; }

    // ��¼��ǰ������Ʒ�ڱ�����Ķѵ�����
    public int Quantity { get; private set; }

    /// <summary>
    /// ����һ���µ� ConsumableItem ʵ������ʼ����Ϊ 1
    /// </summary>
    public ConsumableItem(
        string itemId,
        string displayName,
        Sprite icon,
        int healAmount,
        BuffType buffType,
        float buffDuration,
        float buffValue)
    {
        ItemID = itemId;
        DisplayName = displayName;
        Icon = icon;
        HealAmount = healAmount;
        BuffType = buffType;
        BuffDuration = buffDuration;
        BuffValue = buffValue;

        // �´���ʱ��Ĭ�϶ѵ�����Ϊ 1
        Quantity = 1;
    }

    public override ItemType Type => ItemType.Consumable;

    /// <summary>
    /// ʹ����Ʒʱ���ã��Ȼ�Ѫ��Ȼ��� Buff����� BuffType != None��������Զ���������
    /// </summary>
    public override void Use(GameObject target)
    {
        if (target == null) return;

        // 1) ��Ѫ�߼�
        if (HealAmount > 0)
        {
            var health = target.GetComponent<PlayerHealthController>();
            if (health != null)
            {
                health.Heal(HealAmount);
            }
        }

        // 2) �� Buff �߼�
        if (BuffType != BuffType.None && BuffDuration > 0f)
        {
            var buffManager = target.GetComponent<BuffManager>();
            if (buffManager != null)
            {
                // Ĭ�ϲ����ӣ�ͬ���� Buff ˢ�³���ʱ��
                buffManager.AddBuff(BuffType, BuffDuration, BuffValue, 1, false);
            }
            else
            {
                Debug.LogWarning($"[ConsumableItem] Ŀ�� {target.name} û�й� BuffManager���޷���� Buff {BuffType}");
            }
        }

      
    }

    /// <summary>
    /// ����ǰʵ���������������ڶѵ���
    /// </summary>
    public void AddQuantity(int amount)
    {
        if (amount <= 0) return;
        Quantity += amount;
        Debug.Log($"[ConsumableItem] �ѵ� {DisplayName}���������� = {amount}����ǰ������ = {Quantity}");
    }

    /// <summary>
    /// ���ٵ�ǰʵ��������������������� 0���ⲿ��PlayerInventory�����Ƴ���ʵ��
    /// </summary>
    public void ReduceQuantity(int amount)
    {
        if (amount <= 0) return;
        Quantity = Mathf.Max(0, Quantity - amount);
        Debug.Log($"[ConsumableItem] ���� {DisplayName} ���� = {amount}��ʣ������ = {Quantity}");
    }
}
