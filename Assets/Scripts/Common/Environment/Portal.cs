using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Portal : MonoBehaviour
{
    [Header("����һ������ǰ���ӳ٣��룩")]
    public float delay = 0.2f;

    [Header("�Ƿ��ڴ��ͺ�رձ�����")]
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
        // ֻ�� Player ��Ч����ֻ����һ��
        if (_triggered || !other.CompareTag("Player")) return;

        _triggered = true;
        Debug.Log("[Portal] ��ҽ��봫���ţ�׼����ת��һ�����䡭");

        // ��ʼЭ�̣��ӳٵ�����һ������
        StartCoroutine(DoTeleport(other.gameObject));
    }

    IEnumerator DoTeleport(GameObject player)
    {
        yield return new WaitForSeconds(delay);

        // ���� LevelManager �л����䣯�׶�
        var lm = FindObjectOfType<LevelManager>();
        if (lm != null)
        {
            lm.NextRoomOrStage();
            Debug.Log("[Portal] �������л�����һ�����䡣");
        }
        else
        {
            Debug.LogError("[Portal] δ�ҵ� LevelManager���޷��л����䣡");
        }

        // ��ѡ���ѵ�ǰ���� GameObject��ͨ���Ǹ��ڵ㣩����
        if (deactivateCurrentRoom)
        {
            var roomRoot = transform.root.gameObject;
            roomRoot.SetActive(false);
            Debug.Log("[Portal] �ѹرյ�ǰ���䡣");
        }

        // �����Լ��������ظ�����
        gameObject.SetActive(false);
    }
}





