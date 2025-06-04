using UnityEngine;

public class InputController : MonoBehaviour
{
    private PlayerInventory _playerInventory;

    private void Awake()
    {
        // 在场景中查找 PlayerInventory（假设场景里有一个挂了该脚本的玩家物体）
        _playerInventory = FindObjectOfType<PlayerInventory>();
        if (_playerInventory == null)
        {
            Debug.LogError("[InputController] 找不到 PlayerInventory，请确认场景中有一个对象挂了 PlayerInventory 组件。");
        }
    }

    private void Update()
    {
        // 切换背包界面（原本功能）
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ToggleInventory();
            }
        }

        // 按下 F1：尝试获得 Sword01
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (_playerInventory == null) return;

            ItemBase sword1 = ItemDatabase.Instance.CreateItem("Sword01");
            if (sword1 != null)
            {
                bool added = _playerInventory.AddItem(sword1);
                if (added)
                    Debug.Log("[InputController] 已将 Sword01 添加到背包。");
                else
                    Debug.LogWarning("[InputController] 背包已满或添加失败，无法获得 Sword01。");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase 中未找到 ID = Sword01 的武器，请确认 WeaponData 已正确配置。");
            }
        }

        // 按下 F2：尝试获得 Sword02
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (_playerInventory == null) return;

            ItemBase sword2 = ItemDatabase.Instance.CreateItem("Sword02");
            if (sword2 != null)
            {
                bool added = _playerInventory.AddItem(sword2);
                if (added)
                    Debug.Log("[InputController] 已将 Sword02 添加到背包。");
                else
                    Debug.LogWarning("[InputController] 背包已满或添加失败，无法获得 Sword02。");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase 中未找到 ID = Sword02 的武器，请确认 WeaponData 已正确配置。");
            }
        }

        // 按下 F3：尝试获得 Portion01（Restoration portion 消耗品）
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (_playerInventory == null) return;

            ItemBase portion = ItemDatabase.Instance.CreateItem("Portion01");
            if (portion != null)
            {
                bool added = _playerInventory.AddItem(portion);
                if (added)
                    Debug.Log("[InputController] 已将 Portion01（Restoration portion）添加到背包。");
                else
                    Debug.LogWarning("[InputController] 背包已满或添加失败，无法获得 Portion01（Restoration portion）。");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase 中未找到 ID = Portion01，请确认 ConsumableData 已正确配置。");
            }
        }

        // 按下 F4：尝试获得 攻击药水（AttackPotion）
        if (Input.GetKeyDown(KeyCode.F4))
        {
            if (_playerInventory == null) return;

            ItemBase atkPotion = ItemDatabase.Instance.CreateItem("Portion02");
            if (atkPotion != null)
            {
                bool added = _playerInventory.AddItem(atkPotion);
                if (added)
                    Debug.Log("[InputController] 已将 AttackPotion（攻击药水）添加到背包。");
                else
                    Debug.LogWarning("[InputController] 背包已满或添加失败，无法获得 AttackPotion（攻击药水）。");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase 中未找到 ID = Portion02，请确认 ConsumableData（攻击药水）已正确配置。");
            }
        }

        // 按下 F5：尝试获得 防御药水（DefensePotion）
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (_playerInventory == null) return;

            ItemBase defPotion = ItemDatabase.Instance.CreateItem("Portion03");
            if (defPotion != null)
            {
                bool added = _playerInventory.AddItem(defPotion);
                if (added)
                    Debug.Log("[InputController] 已将 DefensePotion（防御药水）添加到背包。");
                else
                    Debug.LogWarning("[InputController] 背包已满或添加失败，无法获得 DefensePotion（防御药水）。");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase 中未找到 ID = Portion03，请确认 ConsumableData（防御药水）已正确配置。");
            }
        }

        // —— 新增：按下 F6：尝试获得 Sword03 —— 
        if (Input.GetKeyDown(KeyCode.F6))
        {
            if (_playerInventory == null) return;

            ItemBase sword3 = ItemDatabase.Instance.CreateItem("Sword03");
            if (sword3 != null)
            {
                bool added = _playerInventory.AddItem(sword3);
                if (added)
                    Debug.Log("[InputController] 已将 Sword03 添加到背包。");
                else
                    Debug.LogWarning("[InputController] 背包已满或添加失败，无法获得 Sword03。");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase 中未找到 ID = Sword03 的武器，请确认 WeaponData 已正确配置。");
            }
        }

        // —— 新增：按下 F7：尝试获得 Armor01 —— 
        if (Input.GetKeyDown(KeyCode.F7))
        {
            if (_playerInventory == null) return;

            ItemBase armor = ItemDatabase.Instance.CreateItem("Armor01");
            if (armor != null)
            {
                bool added = _playerInventory.AddItem(armor);
                if (added)
                    Debug.Log("[InputController] 已将 Armor01（护甲）添加到背包。");
                else
                    Debug.LogWarning("[InputController] 背包已满或添加失败，无法获得 Armor01（护甲）。");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase 中未找到 ID = Armor01，请确认 ArmorData 已正确配置。");
            }
        }

        // —— 新增：按下 F8：尝试获得 Helmet01 —— 
        if (Input.GetKeyDown(KeyCode.F8))
        {
            if (_playerInventory == null) return;

            ItemBase helmet = ItemDatabase.Instance.CreateItem("Helmet01");
            if (helmet != null)
            {
                bool added = _playerInventory.AddItem(helmet);
                if (added)
                    Debug.Log("[InputController] 已将 Helmet01（头盔）添加到背包。");
                else
                    Debug.LogWarning("[InputController] 背包已满或添加失败，无法获得 Helmet01（头盔）。");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase 中未找到 ID = Helmet01，请确认 HelmetData 已正确配置。");
            }
        }

        // —— 新增：按下 M 键，打开商店面板 —— 
        if (Input.GetKeyDown(KeyCode.M))
        {
            // 只有当在 ShopRoom 场景并且 UIManager 成功缓存了 shopCanvas 时，这里才会真正打开商店
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OpenShop();
            }
        }
    }
}
