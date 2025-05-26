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

        // 记录一下下一步要加载的场景名
        _nextSceneName = currentStageRooms[currentRoomIndex];
        Debug.Log($"[RoomManager] 开始加载房间：{_nextSceneName} (索引 {currentRoomIndex})");

        // 同步加载
        SceneManager.LoadScene(_nextSceneName);
        // 这里不要再立即找和移动玩家，等 OnSceneLoaded 回调
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

    // 场景加载完成后 Unity 自动调用
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 只回应我们刚才请求的那个场景
        if (scene.name != _nextSceneName)
            return;

        // 找到新场景里的 RespawnPoint
        var rpGo = GameObject.FindGameObjectWithTag("RespawnPoint");
        if (rpGo == null)
        {
            Debug.LogError($"[RoomManager] 场景“{scene.name}”中未找到 RespawnPoint！");
            return;
        }

        // 找到持久化的 Player
        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo == null)
        {
            Debug.LogError("[RoomManager] 未找到 Tag=Player 的物体！");
            return;
        }

        // 最终移动
        playerGo.transform.position = rpGo.transform.position;
        Debug.Log($"[RoomManager] 玩家已在场景“{scene.name}”的 RespawnPoint 复位");
    }
}
