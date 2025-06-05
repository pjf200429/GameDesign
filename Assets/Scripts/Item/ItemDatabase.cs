using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ItemDatabase: Stores various ScriptableObjects 
/// (MeleeWeaponData, RangedWeaponData, ConsumableData, HelmetData, ArmorData) 
/// in dictionaries by their IDs and provides functionality to create ItemBase instances by ID.
/// Also includes a ¡°GetRandomItemsAllTypes¡± method to randomly pick items across all categories.
/// </summary>
public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [Header("Melee Weapons (MeleeWeaponData) List")]
    public MeleeWeaponData[] AllWeapons;

    [Header("Ranged Weapons (RangedWeaponData) List")]
    public RangedWeaponData[] AllRangedWeapons;

    [Header("Consumables (ConsumableData) List")]
    public ConsumableData[] AllConsumables;

    [Header("Helmets (HelmetData) List")]
    public HelmetData[] AllHelmets;

    [Header("Armors (ArmorData) List")]
    public ArmorData[] AllArmors;

    // Private mappings: ID ¡ú corresponding ScriptableObject
    private Dictionary<string, MeleeWeaponData> _weaponMap = new Dictionary<string, MeleeWeaponData>();
    private Dictionary<string, RangedWeaponData> _rangedWeaponMap = new Dictionary<string, RangedWeaponData>();
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

        // Initialize _weaponMap (melee weapons)
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

        // Initialize _rangedWeaponMap (ranged weapons)
        if (AllRangedWeapons != null)
        {
            foreach (var r in AllRangedWeapons)
            {
                if (r == null) continue;
                if (string.IsNullOrEmpty(r.EquipmentID)) continue;
                if (!_rangedWeaponMap.ContainsKey(r.EquipmentID))
                    _rangedWeaponMap[r.EquipmentID] = r;
            }
        }

        // Initialize _consumableMap (consumables)
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

        // Initialize _helmetMap (helmets)
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

        // Initialize _armorMap (armors)
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
    /// Creates an ItemBase instance given an itemID (priority order: Melee ¡ú Ranged ¡ú Consumable ¡ú Helmet ¡ú Armor).
    /// </summary>
    public ItemBase CreateItem(string itemID)
    {
        // (1) Melee Weapon
        if (_weaponMap.TryGetValue(itemID, out var wdata))
        {
            return new EquipmentItem(wdata.EquipmentID, wdata, wdata.Price);
        }

        // (2) Ranged Weapon
        if (_rangedWeaponMap.TryGetValue(itemID, out var rdata))
        {
            return new EquipmentItem(rdata.EquipmentID, rdata, rdata.Price);
        }

        // (3) Consumable
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

        // (4) Helmet
        if (_helmetMap.TryGetValue(itemID, out var hdata))
        {
            return new EquipmentItem(hdata.EquipmentID, hdata, hdata.Price);
        }

        // (5) Armor
        if (_armorMap.TryGetValue(itemID, out var adata))
        {
            return new EquipmentItem(adata.EquipmentID, adata, adata.Price);
        }

        Debug.LogError($"[ItemDatabase] CreateItem: Could not find itemID = {itemID}");
        return null;
    }

    /// <summary>
    /// (Existing) Returns a list of random items filtered by a specific ItemType (MeleeWeapon, RangedWeapon, Consumable, Helmet, Armor).
    /// Useful for shop inventories or loot tables.
    /// </summary>
    public List<ItemBase> GetRandomItems(int count, ItemType filterType)
    {
        var result = new List<ItemBase>();

        if (filterType == ItemType.MeleeWeapon)
        {
            var keys = new List<string>(_weaponMap.Keys);
            for (int i = 0; i < count; i++)
            {
                if (keys.Count == 0) break;
                int idx = Random.Range(0, keys.Count);
                result.Add(CreateItem(keys[idx]));
            }
        }
        else if (filterType == ItemType.RangedWeapon)
        {
            var keys = new List<string>(_rangedWeaponMap.Keys);
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
    /// (New) Aggregates all IDs across every category and returns a random selection of count ItemBase instances.
    /// </summary>
    public List<ItemBase> GetRandomItemsAllTypes(int count)
    {
        // 1. Aggregate all keys (IDs) from every mapping
        List<string> allKeys = new List<string>();
        allKeys.AddRange(_weaponMap.Keys);
        allKeys.AddRange(_rangedWeaponMap.Keys);
        allKeys.AddRange(_consumableMap.Keys);
        allKeys.AddRange(_helmetMap.Keys);
        allKeys.AddRange(_armorMap.Keys);

        // 2. Fisher¨CYates shuffle
        for (int i = allKeys.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string tmp = allKeys[i];
            allKeys[i] = allKeys[j];
            allKeys[j] = tmp;
        }

        // 3. Take the first count elements (or fewer if there aren't enough)
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
