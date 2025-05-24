using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HomeManager : MonoBehaviour
{
    [Header("����")]
    public string startRoomSceneName = "Home"; // �����ط��䳡����
    public Transform player; // �������

    private void Start()
    {
        StartCoroutine(LoadStartRoom());
    }

    private IEnumerator LoadStartRoom()
    {
        var asyncLoad = SceneManager.LoadSceneAsync(startRoomSceneName, LoadSceneMode.Additive);
        yield return asyncLoad;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        GameObject respawn = GameObject.Find("RespawnPoint");

        if (respawn != null && player != null)
        {
            // �������
            player.position = respawn.transform.position;

            // ��̬�� PlayerMovement �� RespawnPoint
            var movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.respawnPoint = respawn.transform;
               
            }
            else
            {
                Debug.LogWarning("�����û�� PlayerMovement �ű���");
            }

        
        }
        else
        {
            Debug.LogWarning("RespawnPoint �� Player �Ҳ�����");
        }
    }
}

