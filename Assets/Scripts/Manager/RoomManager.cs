using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RoomManager : MonoBehaviour
{
    [Header("房间池（场景名）")]
    public List<string> allRoomSceneNames;
    public int roomsToGenerate = 3;
    public Transform startPosition;

    private List<GameObject> loadedRoomRoots = new List<GameObject>();
    private List<Transform> roomRespawnPoints = new List<Transform>(); // 每个房间出生点
    public static RoomManager Instance { get; private set; }

    public bool roomsLoaded = false;
    public int currentRoomIndex = -1; // 初始是Home，索引-1

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void GenerateRooms()
    {
        if (allRoomSceneNames == null || allRoomSceneNames.Count == 0)
        {
            Debug.LogError("[RoomManager] 房间列表为空！");
            return;
        }

        if (roomsToGenerate > allRoomSceneNames.Count)
        {
            roomsToGenerate = allRoomSceneNames.Count;
            Debug.LogWarning("[RoomManager] 生成数量超出房间池，自动调整！");
        }

        List<string> tempList = new List<string>(allRoomSceneNames);
        List<string> selectedScenes = new List<string>();

        for (int i = 0; i < roomsToGenerate; i++)
        {
            int index = Random.Range(0, tempList.Count);
            selectedScenes.Add(tempList[index]);
            tempList.RemoveAt(index);
        }

        Shuffle(selectedScenes);

        //  打印选中的房间顺序
        Debug.Log("[RoomManager] 将要加载的房间列表：");
        for (int i = 0; i < selectedScenes.Count; i++)
        {
            Debug.Log($"  {i}: {selectedScenes[i]}");
        }

        StartCoroutine(LoadRooms(selectedScenes));
    }


    private IEnumerator LoadRooms(List<string> sceneNames)
    {
        roomsLoaded = false;
        Vector2 currentPos = startPosition != null ? startPosition.position : Vector2.zero;

        loadedRoomRoots.Clear();
        roomRespawnPoints.Clear(); // 必须清空

        foreach (string sceneName in sceneNames)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            Scene loadedScene = SceneManager.GetSceneByName(sceneName);
            GameObject[] rootObjects = loadedScene.GetRootGameObjects();
            GameObject roomRoot = null;

            foreach (var obj in rootObjects)
            {
                if (obj.name == "RoomRoot")
                {
                    roomRoot = obj;
                    break;
                }
            }

            if (roomRoot == null)
            {
                Debug.LogError($"[RoomManager] 房间 {sceneName} 没有找到 RoomRoot！");
                continue;
            }

            float minX;
            float roomWidth = MeasureRoomWidth(roomRoot, out minX);
            roomRoot.transform.position += (Vector3)currentPos - new Vector3(minX, 0, 0);

            loadedRoomRoots.Add(roomRoot);

            //  这里补充！找到每个 RoomRoot 里的 RespawnPoint
            Transform respawn = roomRoot.transform.Find("RespawnPoint");
            if (respawn != null)
            {
                roomRespawnPoints.Add(respawn);
                Debug.Log($"[RoomManager] 找到 {roomRoot.name} 的 RespawnPoint！");
            }
            else
            {
                roomRespawnPoints.Add(null); // 保持索引一致
                Debug.LogWarning($"[RoomManager] 房间 {roomRoot.name} 没有 RespawnPoint！");
            }

            Debug.Log($"[RoomManager] 加载完成房间: {roomRoot.name}，当前总数：{loadedRoomRoots.Count}");

            currentPos += new Vector2(roomWidth, 0);
        }

        ConnectPortals();

        yield return null;
        Debug.Log("[RoomManager] 房间生成完成，当前房间顺序如下：");
        for (int i = 0; i < loadedRoomRoots.Count; i++)
        {
            if (loadedRoomRoots[i] != null)
                Debug.Log($"[{i}] 房间名: {loadedRoomRoots[i].name}");
            else
                Debug.LogWarning($"[{i}] 房间为空！！");
        }
        roomsLoaded = true;
    }


    private void ConnectPortals()
    {
        for (int i = 0; i < loadedRoomRoots.Count - 1; i++)
        {
            Portal portal = loadedRoomRoots[i].GetComponentInChildren<Portal>();
            if (portal != null)
            {
                if (i + 1 < loadedRoomRoots.Count && roomRespawnPoints[i + 1] != null)
                {
                    portal.targetRoom = loadedRoomRoots[i + 1];
                    portal.targetSpawnPoint = roomRespawnPoints[i + 1];
                    Debug.Log($"[RoomManager] 成功连接 {loadedRoomRoots[i].name} to {loadedRoomRoots[i + 1].name}");
                }
                else
                {
                    Debug.LogWarning($"[RoomManager] 无法连接 {loadedRoomRoots[i].name}，目标房间或RespawnPoint为空！");
                }
            }
            else
            {
                Debug.LogWarning($"[RoomManager] {loadedRoomRoots[i].name} 没有找到 Portal 组件！");
            }
        }
    }

    public Transform GetCurrentRespawnPoint()
    {
        if (currentRoomIndex < 0 || currentRoomIndex >= roomRespawnPoints.Count)
        {
           // Debug.LogWarning($"[RoomManager] 当前房间索引 {currentRoomIndex} 无效！");
            return null;
        }
        return roomRespawnPoints[currentRoomIndex];
    }

    public bool MoveToNextRoom()
    {
        currentRoomIndex++;
        if (currentRoomIndex >= roomRespawnPoints.Count)
        {
            Debug.LogWarning("[RoomManager] 已到达最后一个房间！");
            return false;
        }
        return true;
    }

    float MeasureRoomWidth(GameObject roomRoot, out float minX)
    {
        Collider2D[] colliders = roomRoot.GetComponentsInChildren<Collider2D>();

        if (colliders.Length == 0)
        {
            minX = 0;
            return 30f;
        }

        Bounds bounds = colliders[0].bounds;
        foreach (var col in colliders)
            bounds.Encapsulate(col.bounds);

        minX = bounds.min.x;
        return bounds.size.x;
    }

    void Shuffle(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            string temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
}


