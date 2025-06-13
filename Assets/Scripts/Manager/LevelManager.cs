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
    /// ��ʼ������ʼһ���׶�
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
    /// �л�����һ�����䣻������׶η��������꣬��ʼ��һ�׶�
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

                Debug.Log("[LevelManager] ���н׶�����ɣ���Ϸʤ����");

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
   
    // e.g. �����ͨ��������ʱ�� Portal �ű����ã�
    // FindObjectOfType<LevelManager>().NextRoomOrStage();
}
