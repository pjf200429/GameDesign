
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
            Debug.LogError("[WeaponSwitcher] 未找到 PlayerCombat（包括子物体），请确保挂载在子层级上。", this);
        if (_inventory == null)
            Debug.LogError("[WeaponSwitcher] 未找到 PlayerInventory 组件，请确保挂载在同一 GameObject 上。", this);
    }

    /// <summary>
    /// 切换到背包中指定的武器，调用 PlayerCombat.EquipWeapon 来执行实际装备逻辑。
    /// </summary>
    /// <param name="item">要切换到的 EquipmentItem（必须已经存在于背包中）</param>
    public void SwitchTo(EquipmentItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("[WeaponSwitcher] SwitchTo 收到 null，忽略。");
            return;
        }

        if (_playerCombat != null)
        {
            // 打印调试信息：输出武器的名字
            Debug.Log($"[WeaponSwitcher] 正在装备武器：{item.DisplayName} (ID:{item.ItemID})");
            _playerCombat.EquipWeapon(item);
        }
        else
        {
            Debug.LogError("[WeaponSwitcher] 无法装备，因为未找到 PlayerCombat 实例。");
        }
    }
}
