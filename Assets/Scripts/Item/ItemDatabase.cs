// ItemDatabase.cs
// �������洢��ʵ�������� ItemBase
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    // �� Inspector �й��������� WeaponData��ScriptableObject��
    public WeaponData[] AllWeapons;
    // ����Ʒ����������Ҳ�������ƴ洢��� JSON ����
    // public ConsumableData[] AllConsumables;

    // �ڲ�ӳ�䣺ID �� �����ʲ�
    Dictionary<string, WeaponData> _weaponMap = new Dictionary<string, WeaponData>();
    // ����Ϊ ConsumableData ��������ӳ��

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // ��������
        foreach (var w in AllWeapons)
            _weaponMap[w.WeaponID] = w;
        // ConsumableData ͬ��
    }

    /// <summary>
    /// ����ָ�� ID �� ItemBase ʵ��
    /// </summary>
    public ItemBase CreateItem(string itemID)
    {
        // ���ȴ���������
        if (_weaponMap.TryGetValue(itemID, out var wdata))
        {
            // ���������ݹ��� EquipmentItem
            return new EquipmentItem(
                wdata.WeaponID,
                wdata,
                wdata.name,      // �� wdata.DisplayName
                wdata.Icon       // ����� WeaponData �ж����� Icon
            );
        }

        // ��Ҫ֧�� ConsumableItem�����ڴ˼�����֧�ж�
        // if (_consumableMap.TryGetValue(itemID, out var cdata)) { �� }

        Debug.LogError($"[ItemDatabase] δ֪�� itemID: {itemID}");
        return null;
    }

    /// <summary>
    /// �����������ȡ��� ItemBase�������̵������
    /// </summary>
   
}
