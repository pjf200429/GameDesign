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
   
            return false;
        }

        // �������������������
        if (maxSlots > 0 && _items.Count >= maxSlots)
        {
           
            return false;
        }

 
        if (item is ConsumableItem consumable)
        {
          
            var existing = _items.Find(x => x.ItemID == consumable.ItemID) as ConsumableItem;
            if (existing != null)
            {
             
                existing.AddQuantity(consumable.Quantity);
           
         
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

       
        _items.Add(item);
       

       
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


        if (item is ConsumableItem consumable)
        {
        
            if (consumable.Quantity <= 0)
            {
                _items.Remove(consumable);
                Debug.Log($"[PlayerInventory] {consumable.DisplayName} ����Ϊ 0���Ѵӱ����Ƴ���");
            }
        }
        else if (item is EquipmentItem equip)
        {
            Debug.Log($"[PlayerInventory] {equip.DisplayName} ");
            item.Use(target);
        }

       
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
  
        // ���������仯�¼�
        OnInventoryChanged?.Invoke();
    }

    #endregion
}
