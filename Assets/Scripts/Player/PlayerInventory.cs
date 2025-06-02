using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ұ��������ࣺ����洢��ҵ�ǰӵ�е�������Ʒʵ����
/// ���ṩ��ӡ��Ƴ���ʹ�á�װ���Ƚӿڣ�ͬʱ���������仯�¼���
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    [Header("����������-1 ��ʾ�����ƣ�")]
    [Tooltip("-1 ��ʾ�����Ʊ�����С")]
    public int maxSlots = -1;

    // ������� ItemBase ���б�
    private List<ItemBase> _items = new List<ItemBase>();

    /// <summary>
    /// ֻ�����ʣ���ȡ������������Ʒ������ʵ���б����أ�
    /// </summary>
    public IReadOnlyList<ItemBase> Items => _items.AsReadOnly();

    //------------- ��ѡ����ǰ��װ�������� -------------
    private EquipmentItem _equippedWeapon;
    /// <summary>
    /// ��ǰװ�������������û��װ�����򷵻� null��
    /// </summary>
    public EquipmentItem EquippedWeapon => _equippedWeapon;

    /// <summary>
    /// ���������ݷ����仯ʱ���������� AddItem��RemoveItem��UseItem��EquipWeapon �ڲ�������ã�
    /// �ⲿ���� InventoryUI���ɶ��Ĵ��¼���ˢ�½��档
    /// </summary>
    public event Action OnInventoryChanged;

    private void Awake()
    {
        // �����Ҫ�ڳ����л�ʱ������������ȡ��ע����������
        // DontDestroyOnLoad(gameObject);
    }

    #region �����Ʒ

    /// <summary>
    /// �򱳰����һ�� ItemBase ʵ��
    /// </summary>
    /// <param name="item">ͨ�� ItemDatabase.CreateItem(... ) �õ���ʵ��</param>
    /// <returns>��ӳɹ����� true������������� item Ϊ null �򷵻� false</returns>
    public bool AddItem(ItemBase item)
    {
        if (item == null)
        {
            Debug.LogWarning("[PlayerInventory] AddItem ������ null��������");
            return false;
        }

        // �������������������
        if (maxSlots > 0 && _items.Count >= maxSlots)
        {
            Debug.LogWarning("[PlayerInventory] �����������޷���ӣ�" + item.DisplayName);
            return false;
        }

        // ����� ConsumableItem����Ҫ�ж��Ƿ�������ͬ ID ��Ŀ�ϲ��ѵ�
        if (item is ConsumableItem consumable)
        {
            // �ҵ��������һ����ͬ ID �� ConsumableItem
            var existing = _items.Find(x => x.ItemID == consumable.ItemID) as ConsumableItem;
            if (existing != null)
            {
                // �ϲ��ѵ���ֻ�����������������б���
                existing.AddQuantity(consumable.Quantity);
                Debug.Log($"[PlayerInventory] �ѵ�����Ʒ��{existing.DisplayName} ���� += {consumable.Quantity}����ǰ���� = {existing.Quantity}");
                // ���������仯�¼�������
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        // ����ֱ����ӵ��б���
        _items.Add(item);
        Debug.Log($"[PlayerInventory] �����Ʒ��{item.DisplayName} (ID: {item.ItemID})");

        //// �����ӵ����������ҵ�ǰ��û��װ������Ĭ���Զ�װ�����ɸ�������ɾ���˶Σ�
        //if (item is EquipmentItem equip && _equippedWeapon == null)
        //{
        //    EquipWeapon(equip);
        //}

        // ���������仯�¼�
        OnInventoryChanged?.Invoke();
        return true;
    }

    #endregion

    #region �Ƴ���Ʒ

    /// <summary>
    /// �ӱ����Ƴ�һ����Ʒʵ��
    /// </summary>
    /// <param name="item">Ҫ�Ƴ���ʵ��</param>
    /// <returns>�Ƴ��ɹ����� true�����򷵻� false</returns>
    public bool RemoveItem(ItemBase item)
    {
        if (item == null) return false;

        bool removed = _items.Remove(item);
        if (removed)
        {
            Debug.Log($"[PlayerInventory] �Ƴ���Ʒ��{item.DisplayName} (ID: {item.ItemID})");

            // ����Ƴ����ǵ�ǰװ������������Ҫȡ��װ��
            if (item is EquipmentItem && _equippedWeapon == item)
            {
                _equippedWeapon = null;
                Debug.Log("[PlayerInventory] װ���ѱ��Ƴ�����ǰ������װ����");
                // TODO���ɴ����л������ֻ������һ������
            }

            // ���������仯�¼�
            OnInventoryChanged?.Invoke();
        }
        return removed;
    }

    #endregion

    #region ʹ����Ʒ

    /// <summary>
    /// ʹ�ñ����е�ĳ����Ʒ��ʵ��������Ŀ��ִ��Ч����  
    /// ͨ�� target ������Լ���gameObject����Ҳ�ɸ�����������������
    /// ����� ConsumableItem��ʹ�ú���Զ��ӱ������Ƴ������������
    /// ����� EquipmentItem��ʹ���߼�һ��ֻ�ǡ�װ�����л��������Ƴ���
    /// </summary>
    /// <param name="item">Ҫʹ�õ�ʵ��</param>
    /// <param name="target">ʹ�ö���ͨ��������Լ���</param>
    public void UseItem(ItemBase item, GameObject target)
    {
        if (item == null || !_items.Contains(item)) return;

        item.Use(target);

        // ����� ConsumableItem��Ҫһ�����Ե����ͼ����������Ƴ�����
        if (item is ConsumableItem consumable)
        {
            consumable.ReduceQuantity(1);
            Debug.Log($"[PlayerInventory] ����Ʒ {consumable.DisplayName} ʹ�ú����� = {consumable.Quantity}");
            if (consumable.Quantity <= 0)
            {
                _items.Remove(consumable);
                Debug.Log($"[PlayerInventory] {consumable.DisplayName} ����Ϊ 0���Ѵӱ����Ƴ���");
            }
        }
        else if (item is EquipmentItem equip)
        {
            // �����װ�����ȵ��� EquipWeapon
            EquipWeapon(equip);
            // �����б��Ƴ�
        }

        // ���������仯�¼�
        OnInventoryChanged?.Invoke();
    }

    #endregion

    #region װ������

    /// <summary>
    /// װ��ĳ��������ǰ�᣺������ʵ���Ѿ��ڱ����
    /// </summary>
    /// <param name="weapon">Ҫװ���� Weapon</param>
    public void EquipWeapon(EquipmentItem weapon)
    {
        if (weapon == null)
        {
            Debug.LogWarning("[PlayerInventory] EquipWeapon ������ null�����ԡ�");
            return;
        }

        if (!_items.Contains(weapon))
        {
            Debug.LogWarning($"[PlayerInventory] �����в�����������{weapon.DisplayName}");
            return;
        }

        _equippedWeapon = weapon;
        Debug.Log($"[PlayerInventory] װ��������{weapon.DisplayName}");

        // �������һ�� WeaponSwitcher �� PlayerCombat����������ǰ WeaponData �󶨵�������ϣ�
        var switcher = GetComponent<WeaponSwitcher>();
        if (switcher != null)
        {
            switcher.SwitchTo(weapon);
        }

        // ���������仯�¼������ UI �������߼�Ҫ��ʾ��ǰװ����
        OnInventoryChanged?.Invoke();
    }

    #endregion

    #region ��ѯ�ӿ�

    /// <summary>
    /// ��鱳�����Ƿ�����ָ�� ID ����Ʒ����ʵ���Ƚϻ� ID �Ƚ϶��ɣ�
    /// </summary>
    public bool HasItem(string itemID)
    {
        foreach (var it in _items)
        {
            if (it.ItemID == itemID) return true;
        }
        return false;
    }

    /// <summary>
    /// ��ȡ������ָ�� ID �ĵ�һ��ʵ��������У���û���򷵻� null
    /// </summary>
    public ItemBase GetItem(string itemID)
    {
        return _items.Find(it => it.ItemID == itemID);
    }

    #endregion

    #region ����ѡ����ձ���

    /// <summary>
    /// ��ձ�����������Ʒ�����ã�  
    /// </summary>
    public void ClearAll()
    {
        _items.Clear();
        _equippedWeapon = null;
        Debug.Log("[PlayerInventory] ��������ա�");
        // ���������仯�¼�
        OnInventoryChanged?.Invoke();
    }

    #endregion
}
