// ItemDatabase.cs
// �������洢��ʵ�������� ItemBase
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [Header("������WeaponData�������б�")]
    [Tooltip("�� Inspector �й��������� WeaponData��ScriptableObject��")]
    public WeaponData[] AllWeapons;

    [Header("����Ʒ��ConsumableData�������б�")]
    [Tooltip("�� Inspector �й��������� ConsumableData��ScriptableObject��")]
    public ConsumableData[] AllConsumables;

    // �ڲ�ӳ�䣺ID �� WeaponData
    private Dictionary<string, WeaponData> _weaponMap = new Dictionary<string, WeaponData>();

    // �ڲ�ӳ�䣺ID �� ConsumableData
    private Dictionary<string, ConsumableData> _consumableMap = new Dictionary<string, ConsumableData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
    
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // �����������ݻ���
        if (AllWeapons != null)
        {
            foreach (var w in AllWeapons)
            {
                if (w == null)
                {
                    Debug.LogWarning("[ItemDatabase] AllWeapons �����д��� null Ԫ�أ���������");
                    continue;
                }

                if (string.IsNullOrEmpty(w.WeaponID))
                {
                    Debug.LogWarning($"[ItemDatabase] WeaponData '{w.name}' δ���� WeaponID����������");
                    continue;
                }

                if (!_weaponMap.ContainsKey(w.WeaponID))
                    _weaponMap[w.WeaponID] = w;
                else
                    Debug.LogWarning($"[ItemDatabase] �ظ��� WeaponID: {w.WeaponID}��Asset ���ƣ�{w.name}�����Ѻ��Ժ����ظ��");
            }
        }

        // ����������Ʒ���ݻ���
        if (AllConsumables != null)
        {
            foreach (var c in AllConsumables)
            {
                if (c == null)
                {
                    Debug.LogWarning("[ItemDatabase] AllConsumables �����д��� null Ԫ�أ���������");
                    continue;
                }

                if (string.IsNullOrEmpty(c.ConsumableID))
                {
                    Debug.LogWarning($"[ItemDatabase] ConsumableData '{c.name}' δ���� ConsumableID����������");
                    continue;
                }

                if (!_consumableMap.ContainsKey(c.ConsumableID))
                    _consumableMap[c.ConsumableID] = c;
                else
                    Debug.LogWarning($"[ItemDatabase] �ظ��� ConsumableID: {c.ConsumableID}��Asset ���ƣ�{c.name}�����Ѻ��Ժ����ظ��");
            }
        }
    }

    /// <summary>
    /// ����ָ�� ID �� ItemBase ʵ����
    /// 1. ���ȴ�����ӳ���в��� WeaponData ������ EquipmentItem��
    /// 2. ��δ�ҵ����ٴӿ�����Ʒӳ���в��� ConsumableData ������ ConsumableItem��
    /// 3. ��δ�ҵ��򷵻� null�����ڿ���̨������
    /// </summary>
    public ItemBase CreateItem(string itemID)
    {
        // ���ȴ��������������
        if (_weaponMap.TryGetValue(itemID, out var wdata))
        {
            // ���������ݹ���һ�� EquipmentItem
            return new EquipmentItem(
                wdata.WeaponID,
                wdata,
                // ������ wdata.name Ҳ���Ը�Ϊ wdata.DisplayName����� WeaponData ������ DisplayName �ֶΣ�
                wdata.name,
                wdata.Icon
            );
        }

        // ���������û�ҵ����ٳ��Դӿ�����Ʒ���������
        if (_consumableMap.TryGetValue(itemID, out var cdata))
        {
            // �ÿ�����Ʒ���ݹ���һ�� ConsumableItem
            return new ConsumableItem(
                cdata.ConsumableID,
                cdata.DisplayName,
                cdata.Icon,
                cdata.HealAmount,
                cdata.BuffType,
                cdata.BuffDuration,
                cdata.BuffValue
            );
        }

        Debug.LogError($"[ItemDatabase] CreateItem ʱ�����˲����ڵ� itemID: {itemID}");
        return null;
    }

    /// <summary>
    /// ����ѡ��ʾ�������ݴ�������ͣ������������ȡ��� ItemBase�������̵������
    /// </summary>
    public List<ItemBase> GetRandomItems(int count, ItemType filterType)
    {
        var result = new List<ItemBase>();

        if (filterType == ItemType.Weapon)
        {
            var keys = new List<string>(_weaponMap.Keys);
            for (int i = 0; i < count; i++)
            {
                if (keys.Count == 0) break;
                int idx = Random.Range(0, keys.Count);
                result.Add(CreateItem(keys[idx]));
            }
        }
        else if (filterType == ItemType.Consumable)
        {
            var keys = new List<string>(_consumableMap.Keys);
            for (int i = 0; i < count; i++)
            {
                if (keys.Count == 0) break;
                int idx = Random.Range(0, keys.Count);
                result.Add(CreateItem(keys[idx]));
            }
        }

        return result;
    }
}
