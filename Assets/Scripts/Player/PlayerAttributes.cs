using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEditor.Progress;


[Serializable]
public class ItemSaveData
{
    public string itemID;
    public int quantity;
    public ItemSaveData(string id, int qty) { itemID = id; quantity = qty; }
}

[Serializable]
public class PlayerSaveData
{
    public int maxHealth;
    public string equippedWeaponID;
    public string equippedArmorID;
    public string equippedHelmetID;
    public int teleportHealth;
    public List<ItemSaveData> backpack;
    public int savedStage;
    public int savedRoomIndex;
    public List<string> savedRoomSequence;
    public int saveMaxScore;
    public int saveCurrentScore;
    public int saveCurrency;
}

/// <summary>
/// 玩家属性类：负责记录和管理玩家的核心属性，如血量上限、攻击力、防御力、移动速度、跳跃高度、货币数量、装备、背包、传送血量等，
/// 并支持保存/加载。
/// </summary>
public class PlayerAttributes : MonoBehaviour
{
    private const string SAVE_FILENAME = "player_save.json";

    [Header("Base attributes")]
    [Tooltip("Max health")]
    [SerializeField] private int maxHealth = 200;
   

    [Header("Denfense")]
    [Tooltip("Defense of helmet")]
    [SerializeField] private int helmetDefense = 0;
    [Tooltip("Defense of armor")]
    [SerializeField] private int armorDefense = 0;
    [Tooltip("可抵消的伤害=Defense的和*multiplier")]
    [SerializeField] private float defenseMultiplier = 0.2f;

    [Header("Move")]
    [Tooltip("Speed")]
    [SerializeField] private float movementSpeed = 5f;
    [Tooltip("Jump height")]
    [SerializeField] private float jumpHeight = 2f;

    [Header("Attack")]
    [Tooltip("attackMultiplier")]
    [SerializeField] private float attackMultiplier = 1f;

    [Header("currency")]
    [Tooltip("currency")]
    [SerializeField] private int currency = 0;
    [SerializeField] private int coin = 0;

    [Header("Equipment")]
    [Tooltip("Equipment ID")]
    [SerializeField] private string equippedWeaponID = "Sword01";
    [Tooltip("Equiped armor ID")]
    [SerializeField] private string equippedArmorID;
    [Tooltip("Equiped helmet ID")]
    [SerializeField] private string equippedHelmetID;

    [Header("heleport")]
    [Tooltip("health")]
    [SerializeField] private int teleportHealth;

    [Header("score")]
    [SerializeField] private int currentScore;
    [SerializeField] private int maxScore;

    public SPUM_Prefabs spum;







    private List<ItemBase> backpackItems = new List<ItemBase>();
    public IReadOnlyList<ItemBase> BackpackItems => backpackItems.AsReadOnly();

 
    private PlayerInventory playerInventory;


    public int MaxHealth => maxHealth;
    public int HelmetDefense => helmetDefense;
    public int ArmorDefense => armorDefense;
    public float DefenseMultiplier => defenseMultiplier;
    public float MovementSpeed => movementSpeed;
    public float JumpHeight => jumpHeight;
    public float AttackMultiplier => attackMultiplier;
    public int Currency => currency;

    public int MaxScore => maxScore;

    public int Score => currentScore;

    public string EquippedWeaponID => equippedWeaponID;
    public string EquippedArmorID => equippedArmorID;
    public string EquippedHelmetID => equippedHelmetID;
    public int TeleportHealth => teleportHealth;


    public event Action<string> OnWeaponEquipped;
    public event Action<string> OnArmorEquipped;
    public event Action<string> OnHelmetEquipped;

   
    public event Action<int> OnHelmetDefenseChanged;
    public event Action<int> OnArmorDefenseChanged;
    public event Action<float> OnDefenseMultiplierChanged;
    public event Action<float> OnMovementSpeedChanged;
    public event Action<float> OnJumpHeightChanged;
    public event Action<float> OnAttackMultiplierChanged;
    public event Action<int> OnCurrencyChanged;

    private void Awake()
    {
        


        playerInventory = GetComponent<PlayerInventory>();
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged += SyncInventory;
            SyncInventory();
        }
    }

    private void OnApplicationQuit()
    {
        SavePlayer();
    }

  
    public void SavePlayer()
    {
        var data = new PlayerSaveData
        {
            maxHealth = this.maxHealth,
            equippedWeaponID = this.equippedWeaponID,
            equippedArmorID = this.equippedArmorID,
            equippedHelmetID = this.equippedHelmetID,
            teleportHealth = this.teleportHealth,
            backpack = new List<ItemSaveData>(),
            savedStage = FindObjectOfType<LevelManager>().currentStage,
            savedRoomIndex = FindObjectOfType<RoomManager>().currentRoomIndex,
            savedRoomSequence = new List<string>(FindObjectOfType<RoomManager>().currentStageRooms),
            saveMaxScore = this.maxScore,
            saveCurrentScore = this.currentScore,
            saveCurrency = this.currency
        };
        foreach (var item in backpackItems)
        {
            if (item is ConsumableItem c)
                data.backpack.Add(new ItemSaveData(item.ItemID, c.Quantity));
            else
                data.backpack.Add(new ItemSaveData(item.ItemID, 1));
        }

        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILENAME);
        File.WriteAllText(path, json);
        Debug.Log($"[PlayerAttributes] Saved player data to: {path}");
    }

  

  
    public void LoadPlayer()
    {
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILENAME);
        if (!File.Exists(path))
        {
            Debug.Log("[PlayerAttributes] No save file found.");
            return;
        }

        string json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<PlayerSaveData>(json);


        maxHealth = data.maxHealth;
        equippedWeaponID = data.equippedWeaponID;
        equippedArmorID = data.equippedArmorID;
        equippedHelmetID = data.equippedHelmetID;
        teleportHealth = data.teleportHealth;
        maxScore = data.saveMaxScore;
        currentScore = data.saveCurrentScore;
        currency = data.saveCurrency;

        if (playerInventory != null)
        {
            playerInventory.ClearAll();
            foreach (var entry in data.backpack)
            {
                for (int i = 0; i < entry.quantity; i++)
                {
                    var itm = ItemDatabase.Instance.CreateItem(entry.itemID);
                    if (itm != null)
                    {
                        playerInventory.AddItem(itm);
                        if (itm.ItemID == equippedWeaponID || itm.ItemID == equippedHelmetID || itm.ItemID == equippedArmorID)
                        {
                            playerInventory.UseItem(itm, playerInventory.gameObject);
                        }
                    } 
                }
            }
            SyncInventory();
        }
        transform.position = new Vector3(8f, 5f, 0f);

    }

    public void setCurrentScore(int score)
    {
        this.currentScore += score;
    }
    public void SetTeleportHealth(int health)
    {
        teleportHealth = health;
    }

    public int GetTeleportHealth()
    {
       return teleportHealth ;
    }

    private void SyncInventory()
    {
        backpackItems.Clear();
        backpackItems.AddRange(playerInventory.Items);
    }

    public void PrintBackpackItems()
    {
        Debug.Log("=== Backpack Items ===");
        foreach (var item in BackpackItems)
            Debug.Log(item != null
                ? $"- {item.DisplayName} (ID: {item.ItemID})"
                : "- null item");
    }

    
  

    public void AddHelmetDefense(int amount)
    {
        if (amount == 0) return;
        helmetDefense = Mathf.Max(0, helmetDefense + amount);
        OnHelmetDefenseChanged?.Invoke(helmetDefense);
    }

    public void SetHelmetDefense(int newHelmetDefense)
    {
        helmetDefense = Mathf.Max(0, newHelmetDefense);
        OnHelmetDefenseChanged?.Invoke(helmetDefense);
    }

    public void AddArmorDefense(int amount)
    {
        if (amount == 0) return;
        armorDefense = Mathf.Max(0, armorDefense + amount);
        OnArmorDefenseChanged?.Invoke(armorDefense);
    }

    public void SetArmorDefense(int newArmorDefense)
    {
        armorDefense = Mathf.Max(0, newArmorDefense);
        OnArmorDefenseChanged?.Invoke(armorDefense);
    }

    public void AddDefenseMultiplier(float amount)
    {
        Debug.Log($"jin{ amount}");
        if (Mathf.Approximately(amount, 0f)) return;
        Debug.Log($"[BuffE] 对象 获得 DefenseUp Buff，Increase Defense = {amount }");
        defenseMultiplier = Mathf.Max(0f, defenseMultiplier + amount);
        OnDefenseMultiplierChanged?.Invoke(defenseMultiplier);
    }

    public void SetDefenseMultiplier(float newMultiplier)
    {
        defenseMultiplier = Mathf.Max(0f, newMultiplier);
        OnDefenseMultiplierChanged?.Invoke(defenseMultiplier);
    }



    public void AddMovementSpeed(float amount)
    {
        if (Mathf.Approximately(amount, 0f)) return;
        movementSpeed = Mathf.Max(0f, movementSpeed + amount);
        OnMovementSpeedChanged?.Invoke(movementSpeed);
    }

    public void SetMovementSpeed(float newSpeed)
    {
        movementSpeed = Mathf.Max(0f, newSpeed);
        OnMovementSpeedChanged?.Invoke(movementSpeed);
    }

    public void AddJumpHeight(float amount)
    {
        if (Mathf.Approximately(amount, 0f)) return;
        jumpHeight = Mathf.Max(0f, jumpHeight + amount);
        OnJumpHeightChanged?.Invoke(jumpHeight);
    }

    public void SetJumpHeight(float newHeight)
    {
        jumpHeight = Mathf.Max(0f, newHeight);
        OnJumpHeightChanged?.Invoke(jumpHeight);
    }



    public void AddAttackMultiplier(float amount)
    {
        if (Mathf.Approximately(amount, 0f)) return;
        attackMultiplier = Mathf.Max(0f, attackMultiplier + amount);
        OnAttackMultiplierChanged?.Invoke(attackMultiplier);
    }

    public void SetAttackMultiplier(float newMultiplier)
    {
        attackMultiplier = Mathf.Max(0f, newMultiplier);
        OnAttackMultiplierChanged?.Invoke(attackMultiplier);
    }



    public void ReceiveScoreAndCurrency(int score, int currency)
    {
       this.currentScore += score;   // 保存得分（如果有）
       this.currency += (currency);        // 用已有方法加金币
    }

    public void SetMaxScore(int score)
    {
        this.maxScore = score;
    }

    public void AddCurrency(int curreny)
    {
        this.currency += currency;
    }

    public bool SpendCurrency(int amount)
    {
        if (amount <= 0) return true;
        if (currency < amount) return false;
        currency -= amount;
        OnCurrencyChanged?.Invoke(currency);
        return true;
    }

    


    public void Reset()
    {
        playerInventory = GetComponent<PlayerInventory>();
        playerInventory.ClearAll();
    
        var itm1 = ItemDatabase.Instance.CreateItem("Sword01");
        playerInventory.AddItem(itm1);
        PlayerCombat playerCombat = FindObjectOfType<PlayerCombat>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (itm1 is EquipmentItem equi0)
            playerCombat.EquipWeapon(equi0);

        if (!string.IsNullOrEmpty(equippedArmorID))
        {
            var itm2 = ItemDatabase.Instance.CreateItem(equippedArmorID);
            if (itm2 is EquipmentItem equi1)
                playerCombat.DeEquipWeapon(equi1);


        }

        if (!string.IsNullOrEmpty(equippedHelmetID))
        {
            var itm3 = ItemDatabase.Instance.CreateItem(equippedHelmetID);
            if (itm3 is EquipmentItem equi2)
                playerCombat.DeEquipWeapon(equi2);
        }
        equippedWeaponID = "Sword01";
        equippedArmorID = null;
        equippedHelmetID = null;
        armorDefense = 0;
        helmetDefense = 0;
        currentScore = 0;
        currency = 0;
        PlayerHealthController playerHealthController = FindObjectOfType<PlayerHealthController>();
        playerHealthController.ResetHealth();
     
      
       
      
     
       



    }


    public void SetEquippedWeapon(string weaponID)
    { equippedWeaponID = weaponID; OnWeaponEquipped?.Invoke(weaponID); }
    public void SetEquippedArmor(string armorID)
    { equippedArmorID = armorID; OnArmorEquipped?.Invoke(armorID); }
    public void SetEquippedHelmet(string helmetID)
    { equippedHelmetID = helmetID; OnHelmetEquipped?.Invoke(helmetID); }

}
