
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    private PlayerCombat _playerCombat;
    private PlayerInventory _inventory;

    private void Awake()
    {
        _playerCombat = GetComponentInChildren<PlayerCombat>();
        _inventory = GetComponent<PlayerInventory>();

        if (_playerCombat == null)
            Debug.LogError("[WeaponSwitcher] δ�ҵ� PlayerCombat�����������壩����ȷ���������Ӳ㼶�ϡ�", this);
        if (_inventory == null)
            Debug.LogError("[WeaponSwitcher] δ�ҵ� PlayerInventory �������ȷ��������ͬһ GameObject �ϡ�", this);
    }

    /// <summary>
    /// �л���������ָ�������������� PlayerCombat.EquipWeapon ��ִ��ʵ��װ���߼���
    /// </summary>
    /// <param name="item">Ҫ�л����� EquipmentItem�������Ѿ������ڱ����У�</param>
    public void SwitchTo(EquipmentItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("[WeaponSwitcher] SwitchTo �յ� null�����ԡ�");
            return;
        }

        if (_playerCombat != null)
        {
            // ��ӡ������Ϣ���������������
            Debug.Log($"[WeaponSwitcher] ����װ��������{item.DisplayName} (ID:{item.ItemID})");
            _playerCombat.EquipWeapon(item);
        }
        else
        {
            Debug.LogError("[WeaponSwitcher] �޷�װ������Ϊδ�ҵ� PlayerCombat ʵ����");
        }
    }
}
