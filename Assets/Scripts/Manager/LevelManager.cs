using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Stage & Manager")]
    public int startStage = 0;
    public int totalStages = 3;
    public RoomManager roomManager;
    public ScoreManager scoreManager;
    public GameObject gameOverCanvas;
    public GameOverUIManager gameOverUIManager;
    public int currentStage;

    [Header("Score and Currency UI Canvas")]
    [SerializeField] private GameObject scoreAndCurrencyCanvas;

    private void Awake()
    {
       
    }

    private void Start()
    {
       
        currentStage = startStage;
       

    }

    /// <summary>
    /// 初始化并开始一个阶段
    /// </summary>
    public void BeginStage(int stageIndex)
    {
        currentStage++;
        roomManager.GenerateStage(stageIndex);
        roomManager.LoadCurrentRoom();

        // Ensure scoreManager is active
        if (!scoreManager.gameObject.activeInHierarchy)
            scoreManager.gameObject.SetActive(true);

        // If scoreManager is not assigned, find it
        if (scoreManager == null)
            scoreManager = FindObjectOfType<ScoreManager>();
        
    }

    /// <summary>
    /// 切换到下一个房间；如果本阶段房间已跑完，则开始下一阶段
    /// </summary>
    public void NextRoomOrStage()
    {
        bool hasNext = roomManager.LoadNextRoom();
        if (!hasNext)
        {
            currentStage++;
            if (currentStage <= totalStages)
            {
                BeginStage(currentStage);
            }
            else
            {

                Debug.Log("[LevelManager] 所有阶段已完成，游戏胜利！");

                gameOverCanvas.SetActive(true);
                gameOverUIManager = FindObjectOfType<GameOverUIManager>();
                if (scoreManager.isMax())             
                    gameOverUIManager.ShowGameOverUI(scoreManager.FinalScore, true);
                else
                    gameOverUIManager.ShowGameOverUI(scoreManager.FinalScore, false);
               
                SceneManager.LoadScene("Home");
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                playerObject.transform.position = new Vector3(8f, 5f, 0f);
                PlayerAttributes attr = playerObject.GetComponent<PlayerAttributes>();
                attr.Reset();
                currentStage = 0; 
                
            }
        }
    }
   
    // e.g. 在玩家通过传送门时由 Portal 脚本调用：
    // FindObjectOfType<LevelManager>().NextRoomOrStage();
}
