using UnityEngine;

public class InputController : MonoBehaviour
{
    private PlayerInventory _playerInventory;

    private void Awake()
    {
        // Find PlayerInventory in the scene (assumes one exists on your player object)
        _playerInventory = FindObjectOfType<PlayerInventory>();
        if (_playerInventory == null)
        {
            Debug.LogError("[InputController] PlayerInventory not found - make sure a PlayerInventory component exists in the scene.");
        }
    }

    private void Update()
    {
        //_playerInventory = FindObjectOfType<PlayerInventory>();
        //if (_playerInventory == null)
        //{
        //    Debug.LogError("[InputController] PlayerInventory not found - make sure a PlayerInventory component exists in the scene.");
        //}
        // Toggle inventory UI
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ToggleInventory();
            }
        }

        // Press P: print all items in backpack
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (_playerInventory == null) return;

            Debug.Log("=== Backpack Contents ===");
            foreach (var item in _playerInventory.Items)
            {
                if (item != null)
                    Debug.Log($"- {item.DisplayName} (ID: {item.ItemID})");
                else
                    Debug.Log("- null item");
            }
        }

        // Press F1: add Sword01
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (_playerInventory == null) return;

            ItemBase sword1 = ItemDatabase.Instance.CreateItem("Sword01");
            if (sword1 != null)
            {
                if (_playerInventory.AddItem(sword1))
                    Debug.Log("[InputController] Added Sword01 to backpack.");
                else
                    Debug.LogWarning("[InputController] Could not add Sword01 - backpack full or addition failed.");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase missing WeaponData with ID = Sword01.");
            }
        }

        // Press F2: add Sword02
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (_playerInventory == null) return;

            ItemBase sword2 = ItemDatabase.Instance.CreateItem("Sword02");
            if (sword2 != null)
            {
                if (_playerInventory.AddItem(sword2))
                    Debug.Log("[InputController] Added Sword02 to backpack.");
                else
                    Debug.LogWarning("[InputController] Could not add Sword02 - backpack full or addition failed.");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase missing WeaponData with ID = Sword02.");
            }
        }

        // Press F3: add Portion01 (Restoration potion)
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (_playerInventory == null) return;

            ItemBase portion = ItemDatabase.Instance.CreateItem("Portion01");
            if (portion != null)
            {
                if (_playerInventory.AddItem(portion))
                    Debug.Log("[InputController] Added Portion01 (Restoration potion) to backpack.");
                else
                    Debug.LogWarning("[InputController] Could not add Portion01 - backpack full or addition failed.");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase missing ConsumableData with ID = Portion01.");
            }
        }

        // Press F4: add Portion02 (Attack potion)
        if (Input.GetKeyDown(KeyCode.F4))
        {
            if (_playerInventory == null) return;

            ItemBase atkPotion = ItemDatabase.Instance.CreateItem("Portion02");
            if (atkPotion != null)
            {
                if (_playerInventory.AddItem(atkPotion))
                    Debug.Log("[InputController] Added Portion02 (Attack potion) to backpack.");
                else
                    Debug.LogWarning("[InputController] Could not add Portion02 - backpack full or addition failed.");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase missing ConsumableData with ID = Portion02.");
            }
        }

        // Press F5: add Portion03 (Defense potion)
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (_playerInventory == null) return;

            ItemBase defPotion = ItemDatabase.Instance.CreateItem("Portion03");
            if (defPotion != null)
            {
                if (_playerInventory.AddItem(defPotion))
                    Debug.Log("[InputController] Added Portion03 (Defense potion) to backpack.");
                else
                    Debug.LogWarning("[InputController] Could not add Portion03 - backpack full or addition failed.");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase missing ConsumableData with ID = Portion03.");
            }
        }

        // Press F6: add Sword03
        if (Input.GetKeyDown(KeyCode.F6))
        {
            if (_playerInventory == null) return;

            ItemBase sword3 = ItemDatabase.Instance.CreateItem("Sword03");
            if (sword3 != null)
            {
                if (_playerInventory.AddItem(sword3))
                    Debug.Log("[InputController] Added Sword03 to backpack.");
                else
                    Debug.LogWarning("[InputController] Could not add Sword03 - backpack full or addition failed.");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase missing WeaponData with ID = Sword03.");
            }
        }

        // Press F7: add Armor01
        if (Input.GetKeyDown(KeyCode.F7))
        {
            if (_playerInventory == null) return;

            ItemBase armor = ItemDatabase.Instance.CreateItem("Armor01");
            if (armor != null)
            {
                if (_playerInventory.AddItem(armor))
                    Debug.Log("[InputController] Added Armor01 to backpack.");
                else
                    Debug.LogWarning("[InputController] Could not add Armor01 - backpack full or addition failed.");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase missing ArmorData with ID = Armor01.");
            }
        }

        // Press F8: add Helmet01
        if (Input.GetKeyDown(KeyCode.F8))
        {
            if (_playerInventory == null) return;

            ItemBase helmet = ItemDatabase.Instance.CreateItem("Helmet01");
            if (helmet != null)
            {
                if (_playerInventory.AddItem(helmet))
                    Debug.Log("[InputController] Added Helmet01 to backpack.");
                else
                    Debug.LogWarning("[InputController] Could not add Helmet01 - backpack full or addition failed.");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase missing HelmetData with ID = Helmet01.");
            }
        }

        // Press F9: add Magic01 (ranged weapon)
        if (Input.GetKeyDown(KeyCode.F9))
        {
            if (_playerInventory == null) return;

            ItemBase magic1 = ItemDatabase.Instance.CreateItem("Magic01");
            if (magic1 != null)
            {
                if (_playerInventory.AddItem(magic1))
                    Debug.Log("[InputController] Added Magic01 (ranged weapon) to backpack.");
                else
                    Debug.LogWarning("[InputController] Could not add Magic01 - backpack full or addition failed.");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase missing Data with ID = Magic01.");
            }
        }

        // Press F10: add Magic02 (ranged weapon)
        if (Input.GetKeyDown(KeyCode.F10))
        {
            if (_playerInventory == null) return;

            ItemBase magic2 = ItemDatabase.Instance.CreateItem("Magic02");
            if (magic2 != null)
            {
                if (_playerInventory.AddItem(magic2))
                    Debug.Log("[InputController] Added Magic02 (ranged weapon) to backpack.");
                else
                    Debug.LogWarning("[InputController] Could not add Magic02 - backpack full or addition failed.");
            }
            else
            {
                Debug.LogError("[InputController] ItemDatabase missing Data with ID = Magic02.");
            }
        }

        // Press M: open shop UI
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OpenShop();
            }
        }
    }


}
