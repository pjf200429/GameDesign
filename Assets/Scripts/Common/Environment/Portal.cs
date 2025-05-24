using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour
{
    [Header("Portal����Ŀ��")]
    public GameObject targetRoom;
    public Transform targetSpawnPoint;

    [Header("Portal����")]
    public bool deactivateCurrentRoom = true;
    private bool hasTeleported = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTeleported && other.CompareTag("Player"))
        {
            hasTeleported = true;
            Debug.Log("[Portal] ��Ҵ��������ţ�");

            StartCoroutine(HandleTeleport(other));
        }
    }

    private IEnumerator HandleTeleport(Collider2D player)
    {
        // �ۼӷ�������
        RoomManager.Instance.currentRoomIndex++;

        yield return null;

        // ��ȡĿ�������
        Transform nextSpawn = RoomManager.Instance.GetCurrentRespawnPoint();
        if (nextSpawn != null)
        {
            player.transform.position = nextSpawn.position;

            var movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.respawnPoint = nextSpawn;
                Debug.Log($"[Portal] �ɹ����͵��� {RoomManager.Instance.currentRoomIndex} �����䣡");
            }
        }
        else
        {
            Debug.LogWarning($"[Portal] �Ҳ����� {RoomManager.Instance.currentRoomIndex} ������ĳ����㣡");
        }

        // �رյ�ǰ����
        if (deactivateCurrentRoom)
        {
            Transform currentRoom = transform.root;
            if (currentRoom != null)
            {
                currentRoom.gameObject.SetActive(false);
                Debug.Log("[Portal] ��ǰ�����ѹرգ�");
            }
        }
    }
}




