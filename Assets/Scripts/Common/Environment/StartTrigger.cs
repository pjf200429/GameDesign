using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StartTrigger : MonoBehaviour
{
    [Header("Stage to Begin (1-based)")]
    public int startStage = 1;

    // �ڲ���ǣ�ֻ����һ��
    bool _triggered = false;
    Collider2D _col;

    void Awake()
    {
        // ���� Collider2D ��ǿ����Ϊ������
        _col = GetComponent<Collider2D>();
        if (_col == null)
        {
            Debug.LogError("[StartTrigger] �Ҳ��� Collider2D �����");
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
     

        // 1. ���ɷ�������
        var rm = FindObjectOfType<RoomManager>();
        if (rm == null)
        {
          
        }
        else
        {
            rm.GenerateStage(startStage);
            Debug.Log($"[StartTrigger] ���ɵķ������У�{string.Join(" -> ", rm.currentStageRooms)}");
        }

        // 2. ֪ͨ LevelManager ��ʽ��ʼ
        var lm = FindObjectOfType<LevelManager>();
        if (lm == null)
        {
          
        }
        else
        {
            lm.BeginStage(startStage);
        }

        // 3. �����Լ�����ֹ�ٴδ���
        gameObject.SetActive(false);
    }
}
