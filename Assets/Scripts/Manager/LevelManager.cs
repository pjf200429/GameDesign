using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Stage & Manager")]
    public int startStage = 1;
    public int totalStages = 3;
    public RoomManager roomManager;  // 在 Inspector 中拖入同一个 GameManager 上的 RoomManager

    private int currentStage;

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
        Debug.Log($"[LevelManager] 开始第 {stageIndex} 阶段");
        roomManager.GenerateStage(stageIndex);
        roomManager.LoadCurrentRoom();
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
                SceneManager.LoadScene("Home");
                Destroy(gameObject);
            }
        }
    }

    // e.g. 在玩家通过传送门时由 Portal 脚本调用：
    // FindObjectOfType<LevelManager>().NextRoomOrStage();
}
