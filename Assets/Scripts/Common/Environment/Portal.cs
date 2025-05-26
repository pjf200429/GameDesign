using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Portal : MonoBehaviour
{
    [Header("到下一个房间前的延迟（秒）")]
    public float delay = 0.2f;

    [Header("是否在传送后关闭本房间")]
    public bool deactivateCurrentRoom = true;

    bool _triggered = false;
    Collider2D _col;

    void Awake()
    {
        _col = GetComponent<Collider2D>();
        if (_col != null)
            _col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 只对 Player 生效，且只触发一次
        if (_triggered || !other.CompareTag("Player")) return;

        _triggered = true;
        Debug.Log("[Portal] 玩家进入传送门，准备跳转下一个房间…");

        // 开始协程，延迟调用下一个房间
        StartCoroutine(DoTeleport(other.gameObject));
    }

    IEnumerator DoTeleport(GameObject player)
    {
        yield return new WaitForSeconds(delay);

        // 调用 LevelManager 切换房间／阶段
        var lm = FindObjectOfType<LevelManager>();
        if (lm != null)
        {
            lm.NextRoomOrStage();
            Debug.Log("[Portal] 已请求切换到下一个房间。");
        }
        else
        {
            Debug.LogError("[Portal] 未找到 LevelManager，无法切换房间！");
        }

        // 可选：把当前房间 GameObject（通常是根节点）禁用
        if (deactivateCurrentRoom)
        {
            var roomRoot = transform.root.gameObject;
            roomRoot.SetActive(false);
            Debug.Log("[Portal] 已关闭当前房间。");
        }

        // 禁用自己，避免重复触发
        gameObject.SetActive(false);
    }
}





