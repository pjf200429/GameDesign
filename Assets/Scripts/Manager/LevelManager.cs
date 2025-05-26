using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Stage & Manager")]
    public int startStage = 1;
    public int totalStages = 3;
    public RoomManager roomManager;  // �� Inspector ������ͬһ�� GameManager �ϵ� RoomManager

    private int currentStage;

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
        Debug.Log($"[LevelManager] ��ʼ�� {stageIndex} �׶�");
        roomManager.GenerateStage(stageIndex);
        roomManager.LoadCurrentRoom();
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
                SceneManager.LoadScene("Home");
                Destroy(gameObject);
            }
        }
    }

    // e.g. �����ͨ��������ʱ�� Portal �ű����ã�
    // FindObjectOfType<LevelManager>().NextRoomOrStage();
}
