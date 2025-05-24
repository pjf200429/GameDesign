using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RoomManager : MonoBehaviour
{
    [Header("����أ���������")]
    public List<string> allRoomSceneNames;
    public int roomsToGenerate = 3;
    public Transform startPosition;

    private List<GameObject> loadedRoomRoots = new List<GameObject>();
    private List<Transform> roomRespawnPoints = new List<Transform>(); // ÿ�����������
    public static RoomManager Instance { get; private set; }

    public bool roomsLoaded = false;
    public int currentRoomIndex = -1; // ��ʼ��Home������-1

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
            Debug.LogError("[RoomManager] �����б�Ϊ�գ�");
            return;
        }

        if (roomsToGenerate > allRoomSceneNames.Count)
        {
            roomsToGenerate = allRoomSceneNames.Count;
            Debug.LogWarning("[RoomManager] ����������������أ��Զ�������");
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

        //  ��ӡѡ�еķ���˳��
        Debug.Log("[RoomManager] ��Ҫ���صķ����б�");
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
        roomRespawnPoints.Clear(); // �������

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
                Debug.LogError($"[RoomManager] ���� {sceneName} û���ҵ� RoomRoot��");
                continue;
            }

            float minX;
            float roomWidth = MeasureRoomWidth(roomRoot, out minX);
            roomRoot.transform.position += (Vector3)currentPos - new Vector3(minX, 0, 0);

            loadedRoomRoots.Add(roomRoot);

            //  ���ﲹ�䣡�ҵ�ÿ�� RoomRoot ��� RespawnPoint
            Transform respawn = roomRoot.transform.Find("RespawnPoint");
            if (respawn != null)
            {
                roomRespawnPoints.Add(respawn);
                Debug.Log($"[RoomManager] �ҵ� {roomRoot.name} �� RespawnPoint��");
            }
            else
            {
                roomRespawnPoints.Add(null); // ��������һ��
                Debug.LogWarning($"[RoomManager] ���� {roomRoot.name} û�� RespawnPoint��");
            }

            Debug.Log($"[RoomManager] ������ɷ���: {roomRoot.name}����ǰ������{loadedRoomRoots.Count}");

            currentPos += new Vector2(roomWidth, 0);
        }

        ConnectPortals();

        yield return null;
        Debug.Log("[RoomManager] ����������ɣ���ǰ����˳�����£�");
        for (int i = 0; i < loadedRoomRoots.Count; i++)
        {
            if (loadedRoomRoots[i] != null)
                Debug.Log($"[{i}] ������: {loadedRoomRoots[i].name}");
            else
                Debug.LogWarning($"[{i}] ����Ϊ�գ���");
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
                    Debug.Log($"[RoomManager] �ɹ����� {loadedRoomRoots[i].name} to {loadedRoomRoots[i + 1].name}");
                }
                else
                {
                    Debug.LogWarning($"[RoomManager] �޷����� {loadedRoomRoots[i].name}��Ŀ�귿���RespawnPointΪ�գ�");
                }
            }
            else
            {
                Debug.LogWarning($"[RoomManager] {loadedRoomRoots[i].name} û���ҵ� Portal �����");
            }
        }
    }

    public Transform GetCurrentRespawnPoint()
    {
        if (currentRoomIndex < 0 || currentRoomIndex >= roomRespawnPoints.Count)
        {
           // Debug.LogWarning($"[RoomManager] ��ǰ�������� {currentRoomIndex} ��Ч��");
            return null;
        }
        return roomRespawnPoints[currentRoomIndex];
    }

    public bool MoveToNextRoom()
    {
        currentRoomIndex++;
        if (currentRoomIndex >= roomRespawnPoints.Count)
        {
            Debug.LogWarning("[RoomManager] �ѵ������һ�����䣡");
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


