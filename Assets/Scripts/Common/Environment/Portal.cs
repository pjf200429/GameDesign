using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour
{
    [Header("Portal传送目标")]
    public GameObject targetRoom;
    public Transform targetSpawnPoint;

    [Header("Portal设置")]
    public bool deactivateCurrentRoom = true;
    private bool hasTeleported = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTeleported && other.CompareTag("Player"))
        {
            hasTeleported = true;
            Debug.Log("[Portal] 玩家触发传送门！");

            StartCoroutine(HandleTeleport(other));
        }
    }

    private IEnumerator HandleTeleport(Collider2D player)
    {
        // 累加房间索引
        RoomManager.Instance.currentRoomIndex++;

        yield return null;

        // 获取目标出生点
        Transform nextSpawn = RoomManager.Instance.GetCurrentRespawnPoint();
        if (nextSpawn != null)
        {
            player.transform.position = nextSpawn.position;

            var movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.respawnPoint = nextSpawn;
                Debug.Log($"[Portal] 成功传送到第 {RoomManager.Instance.currentRoomIndex} 个房间！");
            }
        }
        else
        {
            Debug.LogWarning($"[Portal] 找不到第 {RoomManager.Instance.currentRoomIndex} 个房间的出生点！");
        }

        // 关闭当前房间
        if (deactivateCurrentRoom)
        {
            Transform currentRoom = transform.root;
            if (currentRoom != null)
            {
                currentRoom.gameObject.SetActive(false);
                Debug.Log("[Portal] 当前房间已关闭！");
            }
        }
    }
}




