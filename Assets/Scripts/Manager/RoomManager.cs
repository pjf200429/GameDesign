using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    [Header("Builder Reference")]
    public RoomBuilder roomBuilder;

    [HideInInspector] public List<string> currentStageRooms;
    [HideInInspector] public int currentRoomIndex = 0;

    // ���漴�����صķ�����
    private string _nextSceneName;

    void Awake()
    {
        if (roomBuilder == null)
            Debug.LogError("[RoomManager] δ���� RoomBuilder��");

        // ���ĳ�����������¼�
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
            Debug.LogError("[RoomManager] ��ǰ��������Ϊ�գ�");
            return;
        }

        // ��¼һ����һ��Ҫ���صĳ�����
        _nextSceneName = currentStageRooms[currentRoomIndex];
        Debug.Log($"[RoomManager] ��ʼ���ط��䣺{_nextSceneName} (���� {currentRoomIndex})");

        // ͬ������
        SceneManager.LoadScene(_nextSceneName);
        // ���ﲻҪ�������Һ��ƶ���ң��� OnSceneLoaded �ص�
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

    // ����������ɺ� Unity �Զ�����
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ֻ��Ӧ���Ǹղ�������Ǹ�����
        if (scene.name != _nextSceneName)
            return;

        // �ҵ��³������ RespawnPoint
        var rpGo = GameObject.FindGameObjectWithTag("RespawnPoint");
        if (rpGo == null)
        {
            Debug.LogError($"[RoomManager] ������{scene.name}����δ�ҵ� RespawnPoint��");
            return;
        }

        // �ҵ��־û��� Player
        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo == null)
        {
            Debug.LogError("[RoomManager] δ�ҵ� Tag=Player �����壡");
            return;
        }

        // �����ƶ�
        playerGo.transform.position = rpGo.transform.position;
        Debug.Log($"[RoomManager] ������ڳ�����{scene.name}���� RespawnPoint ��λ");
    }
}
