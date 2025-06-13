using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    [Header("Builder Reference")]
    public RoomBuilder roomBuilder;

    [HideInInspector] public List<string> currentStageRooms;
    [HideInInspector] public int currentRoomIndex = 0;

    // 缓存即将加载的房间名
    private string _nextSceneName;

    void Awake()
    {
        if (roomBuilder == null)
            Debug.LogError("[RoomManager] 未设置 RoomBuilder！");

        // 订阅场景加载完成事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void GenerateStage(int stageIndex)
    {
        currentStageRooms = roomBuilder.BuildStageRooms(stageIndex);
        currentRoomIndex = 0;
    }

    public void LoadCurrentRoom()
    {
        if (currentStageRooms == null || currentStageRooms.Count == 0)
        {
            Debug.LogError("[RoomManager] 当前房间序列为空！");
            return;
        }


        _nextSceneName = currentStageRooms[currentRoomIndex];
        Debug.Log($"[RoomManager] start loading room：{_nextSceneName} (Index {currentRoomIndex})");


        SceneManager.LoadScene(_nextSceneName);

    }

    public bool LoadNextRoom()
    {
        if (currentStageRooms == null) return false;
        if (currentRoomIndex < currentStageRooms.Count - 1)
        {
            currentRoomIndex++;
            LoadCurrentRoom();
            return true;
        }
        return false;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
 
        if (scene.name != _nextSceneName)
            return;


        var rpGo = GameObject.FindGameObjectWithTag("RespawnPoint");
        if (rpGo == null)
        {
            Debug.LogError($"[RoomManager] 场景“{scene.name}”中未找到 RespawnPoint！");
            return;
        }

  
        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo == null)
        {
            Debug.LogError("[RoomManager] 未找到 Tag=Player 的物体！");
            return;
        }

    
        playerGo.transform.position = rpGo.transform.position;
 
    }
}
