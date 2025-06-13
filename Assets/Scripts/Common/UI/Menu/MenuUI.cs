using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Main Menu UI: handles New Game and Load Game buttons
/// </summary>
public class MenuUI : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button newGameButton;
    public Button loadGameButton;


    [Header("Player")]
    [Tooltip("Assign your Player prefab here")]
    public GameObject playerPrefab;

    private PlayerAttributes pa;
    private PlayerInventory pi;
    private const string SAVE_FILENAME = "player_save.json";

    private void Awake()
    {
        

        // Bind button events
        newGameButton.onClick.AddListener(OnNewGame);
        loadGameButton.onClick.AddListener(OnLoadGame);

        // Disable Load if no save exists
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILENAME);
        loadGameButton.interactable = File.Exists(path);
    }

    /// <summary>
    /// Start a new game: delete save and load first scene
    /// </summary>
    private void OnNewGame()
    {
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILENAME);
        if (File.Exists(path)) File.Delete(path);
        HideMenu();

        SceneManager.LoadScene("Home");
        var playerGO = Instantiate(playerPrefab);
        playerGO.tag = "Player";
        playerGO.transform.position = new Vector3(8f, 5f, 0f);

        pi = playerGO.GetComponent<PlayerInventory>();
        pa = playerGO.GetComponent<PlayerAttributes>();

        ItemBase sword1 = ItemDatabase.Instance.CreateItem("Sword01");
        pi.AddItem(sword1);
        pi.UseItem(sword1, pi.gameObject);

        pa.SetTeleportHealth(pa.MaxHealth); 

    }

    /// <summary>
    /// Load saved game
    /// </summary>
    private void OnLoadGame()
    {
        HideMenu();
        loadScene();
        var playerGO = Instantiate(playerPrefab);
       
        // make sure it's tagged "Player" if you rely on that
        playerGO.tag = "Player";

        // 3. Grab the new PlayerAttributes component
        pa = playerGO.GetComponent<PlayerAttributes>();
        
        pa.LoadPlayer();
        playerGO.transform.position = new Vector3(8f, 5f, 0f);

    }

    /// <summary>
    /// Hides the menu UI and disables buttons
    /// </summary>
    private void HideMenu()
    {
        newGameButton.interactable = false;
        loadGameButton.interactable = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Callback after scene load: position player at (-8, -4, 0)
    /// </summary>
    private void PositionPlayerOnLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Home") return;

        int playerLayer = LayerMask.NameToLayer("Player");
        var transforms = FindObjectsOfType<Transform>();
        foreach (var t in transforms)
        {
            if (t.gameObject.layer == playerLayer)
            {
                t.position = new Vector3(16f, 5f, 0f);
                break;
            }
        }

        SceneManager.sceneLoaded -= PositionPlayerOnLoad;
    }

    public void loadScene()
    {
     
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILENAME);
        if (!File.Exists(path))
        {
            Debug.Log("[PlayerAttributes] No save file found.");
            return;
        }
        string json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<PlayerSaveData>(json);
        var lm = FindObjectOfType<LevelManager>();
        var rm = FindObjectOfType<RoomManager>();
        lm.currentStage = data.savedStage;
     
        if (lm.currentStage != 0)
        {
                       
            rm.currentStageRooms = new List<string>(data.savedRoomSequence);
            rm.currentRoomIndex = data.savedRoomIndex;
         
            rm.LoadCurrentRoom();

        }
        else
        {
            SceneManager.LoadScene("Home");
        }
    }
}