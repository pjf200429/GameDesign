using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HomeManager : MonoBehaviour
{
    [Header("设置")]
    public string startRoomSceneName = "Home"; // 出生地房间场景名
    public Transform player; // 玩家引用

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
            // 传送玩家
            player.position = respawn.transform.position;

            // 动态绑定 PlayerMovement 的 RespawnPoint
            var movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.respawnPoint = respawn.transform;
               
            }
            else
            {
                Debug.LogWarning("玩家上没有 PlayerMovement 脚本！");
            }

        
        }
        else
        {
            Debug.LogWarning("RespawnPoint 或 Player 找不到！");
        }
    }
}

