using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StartTrigger : MonoBehaviour
{
    [Header("Stage to Begin (1-based)")]
    public int startStage = 1;

    // 内部标记：只触发一次
    bool _triggered = false;
    Collider2D _col;

    void Awake()
    {
        // 缓存 Collider2D 并强制设为触发器
        _col = GetComponent<Collider2D>();
        if (_col == null)
        {
            Debug.LogError("[StartTrigger] 找不到 Collider2D 组件！");
        }
        else
        {
            _col.isTrigger = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
       

        if (_triggered)
        {
          
            return;
        }

        if (!other.CompareTag("Player"))
        {
         
            return;
        }

        _triggered = true;
     

        // 1. 生成房间序列
        var rm = FindObjectOfType<RoomManager>();
        if (rm == null)
        {
          
        }
        else
        {
            rm.GenerateStage(startStage);
            Debug.Log($"[StartTrigger] 生成的房间序列：{string.Join(" -> ", rm.currentStageRooms)}");
        }

        // 2. 通知 LevelManager 正式开始
        var lm = FindObjectOfType<LevelManager>();
        if (lm == null)
        {
          
        }
        else
        {
            lm.BeginStage(startStage);
        }

        // 3. 禁用自己，防止再次触发
        gameObject.SetActive(false);
    }
}
