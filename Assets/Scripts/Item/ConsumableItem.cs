//// ConsumableItem.cs
//// ��������Ʒ������ҩˮ���������
//using UnityEngine;

//public class ConsumableItem : ItemBase
//{
//    // ���ܵĻظ�����Buff ���͵����� ItemData
//    public int HealAmount { get; private set; }
//    public BuffType BuffType { get; private set; }
//    public float BuffDuration { get; private set; }
//    public float BuffValue { get; private set; }

//    public ConsumableItem(
//        string itemId,
//        string displayName,
//        Sprite icon,
//        int healAmount,
//        BuffType buff,
//        float duration,
//        float value)
//    {
//        ItemID = itemId;
//        DisplayName = displayName;
//        Icon = icon;
//        HealAmount = healAmount;
//        BuffType = buff;
//        BuffDuration = duration;
//        BuffValue = value;
//    }

//    public override void Use(GameObject target)
//    {
//        // ʹ��ʱ�ָ�����
//        var health = target.GetComponent<PlayerHealthController>();
//        if (health != null && HealAmount > 0)
//        {
//            health.Heal(HealAmount);
//        }
//        // ʹ��ʱ��� Buff
//        if (BuffDuration > 0 && BuffType != BuffType.None)
//        {
//            BuffManager.Instance.AddBuff(BuffType, BuffDuration, BuffValue);
//        }
//    }
//}
