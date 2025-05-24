using UnityEngine;
using System.Collections;

public class InitiateManager : MonoBehaviour
{
    [Header("入口检测")]
    public Collider2D entranceTrigger;

    private bool hasStarted = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasStarted && other.CompareTag("Player"))
        {
            hasStarted = true;
            Debug.Log("[InitiateManager] 检测到玩家，准备生成房间！");
            StartCoroutine(HandleEntrance(other));
        }
    }

    private IEnumerator HandleEntrance(Collider2D player)
    {
        // 调用 RoomManager 生成房间
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.GenerateRooms();
        }
        else
        {
            Debug.LogError("[InitiateManager] 找不到 RoomManager 实例！");
            yield break;
        }

        // 等待所有房间加载完成
        while (!RoomManager.Instance.roomsLoaded)
        {
            Debug.Log("[InitiateManager] 等待房间加载中...");
            yield return null;
        }

       // Debug.Log("[InitiateManager] 房间加载完成！");

        // 初始化 currentRoomIndex 为 0
        RoomManager.Instance.currentRoomIndex = 0;

        // 获取第0个房间的出生点
        Transform targetSpawn = RoomManager.Instance.GetCurrentRespawnPoint();
        if (targetSpawn != null)
        {
            // 传送玩家
            player.transform.position = targetSpawn.position;

            // 更新玩家的 respawnPoint
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.respawnPoint = targetSpawn;
                Debug.Log("[InitiateManager] 玩家首次传送成功，respawnPoint绑定完成！");
            }
            else
            {
                Debug.LogWarning("[InitiateManager] 玩家身上没有找到 PlayerMovement 组件！");
            }
        }
        else
        {
           // Debug.LogError("[InitiateManager] 找不到房间0的出生点！");
        }
    }
}



