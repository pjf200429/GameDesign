using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ItemDatabase������Ѹ��� ScriptableObject (WeaponData��ConsumableData��HelmetData��ArmorData) 
/// �� ID �浽�ֵ�����ṩ�� ID ���� ItemBase �Ĺ��ܡ�  
/// ����������һ����������������ӿڣ�GetRandomItemsAllTypes
/// </summary>
public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [Header("������WeaponData�������б�")]
    public WeaponData[] AllWeapons;

    [Header("����Ʒ��ConsumableData�������б�")]
    public ConsumableData[] AllConsumables;

    [Header("ͷ����HelmetData�������б�")]
    public HelmetData[] AllHelmets;

    [Header("���ף�ArmorData�������б�")]
    public ArmorData[] AllArmors;

    // ˽��ӳ�䣺EquipmentID/ConsumableID �� ��Ӧ�� ScriptableObject
    private Dictionary<string, WeaponData> _weaponMap = new Dictionary<string, WeaponData>();
    private Dictionary<string, ConsumableData> _consumableMap = new Dictionary<string, ConsumableData>();
    private Dictionary<string, HelmetData> _helmetMap = new Dictionary<string, HelmetData>();
    private Dictionary<string, ArmorData> _armorMap = new Dictionary<string, ArmorData>();

    private void Awake()
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

        // ��ʼ�� _weaponMap
        if (AllWeapons != null)
        {
            foreach (var w in AllWeapons)
            {
                if (w == null) continue;
                if (string.IsNullOrEmpty(w.EquipmentID)) continue;
                if (!_weaponMap.ContainsKey(w.EquipmentID))
                    _weaponMap[w.EquipmentID] = w;
            }
        }

        // ��ʼ�� _consumableMap
        if (AllConsumables != null)
        {
            foreach (var c in AllConsumables)
            {
                if (c == null) continue;
                if (string.IsNullOrEmpty(c.ConsumableID)) continue;
                if (!_consumableMap.ContainsKey(c.ConsumableID))
                    _consumableMap[c.ConsumableID] = c;
            }
        }

        // ��ʼ�� _helmetMap
        if (AllHelmets != null)
        {
            foreach (var h in AllHelmets)
            {
                if (h == null) continue;
                if (string.IsNullOrEmpty(h.EquipmentID)) continue;
                if (!_helmetMap.ContainsKey(h.EquipmentID))
                    _helmetMap[h.EquipmentID] = h;
            }
        }

        // ��ʼ�� _armorMap
        if (AllArmors != null)
        {
            foreach (var a in AllArmors)
            {
                if (a == null) continue;
                if (string.IsNullOrEmpty(a.EquipmentID)) continue;
                if (!_armorMap.ContainsKey(a.EquipmentID))
                    _armorMap[a.EquipmentID] = a;
            }
        }
    }

    /// <summary>
    /// ����ָ�� ID �� ItemBase ʵ�������Ȳ�����������Ʒ��ͷ�������ף���
    /// </summary>
    public ItemBase CreateItem(string itemID)
    {
        // (1) ����
        if (_weaponMap.TryGetValue(itemID, out var wdata))
        {
            return new EquipmentItem(wdata.EquipmentID, wdata, wdata.Price);
        }

        // (2) ����Ʒ
        if (_consumableMap.TryGetValue(itemID, out var cdata))
        {
            return new ConsumableItem(
                cdata.ConsumableID,
                cdata.DisplayName,
                cdata.Icon,
                cdata.HealAmount,
                cdata.BuffType,
                cdata.BuffDuration,
                cdata.BuffValue,
                cdata.Price
            );
        }

        // (3) ͷ��
        if (_helmetMap.TryGetValue(itemID, out var hdata))
        {
            return new EquipmentItem(hdata.EquipmentID, hdata, hdata.Price);
        }

        // (4) ����
        if (_armorMap.TryGetValue(itemID, out var adata))
        {
            return new EquipmentItem(adata.EquipmentID, adata, adata.Price);
        }

        Debug.LogError($"[ItemDatabase] CreateItem: δ�ҵ� itemID = {itemID}");
        return null;
    }

    /// <summary>
    /// ��ԭ�У����ݴ�������ͣ������ȡ��� ItemBase�������̵������
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
        else if (filterType == ItemType.Helmet)
        {
            var keys = new List<string>(_helmetMap.Keys);
            for (int i = 0; i < count; i++)
            {
                if (keys.Count == 0) break;
                int idx = Random.Range(0, keys.Count);
                result.Add(CreateItem(keys[idx]));
            }
        }
        else if (filterType == ItemType.Armor)
        {
            var keys = new List<string>(_armorMap.Keys);
            for (int i = 0; i < count; i++)
            {
                if (keys.Count == 0) break;
                int idx = Random.Range(0, keys.Count);
                result.Add(CreateItem(keys[idx]));
            }
        }

        return result;
    }

    /// <summary>
    /// �������������������͵�ӳ���л��ܳ����� ID�������ȡ count �� ItemBase��
    /// </summary>
    public List<ItemBase> GetRandomItemsAllTypes(int count)
    {
        // 1. �����������͵� Key��ID��
        List<string> allKeys = new List<string>();
        allKeys.AddRange(_weaponMap.Keys);
        allKeys.AddRange(_consumableMap.Keys);
        allKeys.AddRange(_helmetMap.Keys);
        allKeys.AddRange(_armorMap.Keys);

        // 2. Fisher�CYates ϴ�ƣ���ֱ���������
        for (int i = allKeys.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            // ����
            string tmp = allKeys[i];
            allKeys[i] = allKeys[j];
            allKeys[j] = tmp;
        }

        // 3. ȡǰ count �������������ȡ���У�
        int actualCount = Mathf.Min(count, allKeys.Count);
        List<ItemBase> result = new List<ItemBase>();

        for (int i = 0; i < actualCount; i++)
        {
            ItemBase item = CreateItem(allKeys[i]);
            if (item != null)
                result.Add(item);
        }

        return result;
    }
}
