using UnityEngine;
using System.Collections;

public class InitiateManager : MonoBehaviour
{
    [Header("��ڼ��")]
    public Collider2D entranceTrigger;

    private bool hasStarted = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasStarted && other.CompareTag("Player"))
        {
            hasStarted = true;
            Debug.Log("[InitiateManager] ��⵽��ң�׼�����ɷ��䣡");
            StartCoroutine(HandleEntrance(other));
        }
    }

    private IEnumerator HandleEntrance(Collider2D player)
    {
        // ���� RoomManager ���ɷ���
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.GenerateRooms();
        }
        else
        {
            Debug.LogError("[InitiateManager] �Ҳ��� RoomManager ʵ����");
            yield break;
        }

        // �ȴ����з���������
        while (!RoomManager.Instance.roomsLoaded)
        {
            Debug.Log("[InitiateManager] �ȴ����������...");
            yield return null;
        }

       // Debug.Log("[InitiateManager] ���������ɣ�");

        // ��ʼ�� currentRoomIndex Ϊ 0
        RoomManager.Instance.currentRoomIndex = 0;

        // ��ȡ��0������ĳ�����
        Transform targetSpawn = RoomManager.Instance.GetCurrentRespawnPoint();
        if (targetSpawn != null)
        {
            // �������
            player.transform.position = targetSpawn.position;

            // ������ҵ� respawnPoint
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.respawnPoint = targetSpawn;
                Debug.Log("[InitiateManager] ����״δ��ͳɹ���respawnPoint����ɣ�");
            }
            else
            {
                Debug.LogWarning("[InitiateManager] �������û���ҵ� PlayerMovement �����");
            }
        }
        else
        {
           // Debug.LogError("[InitiateManager] �Ҳ�������0�ĳ����㣡");
        }
    }
}



