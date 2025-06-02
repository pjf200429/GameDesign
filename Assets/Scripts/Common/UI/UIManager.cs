using UnityEngine;

/// <summary>
/// ȫ��Ψһ�� UI ������ (UIManager)��
/// 1. �� Awake() ʱ���� DontDestroyOnLoad(this.gameObject)�����Լ��������ӽڵ㱣�ֳ�פ��
/// 2. Ĭ������ InventoryCanvas��ֻ���յ� Toggle/On/Off ����ʱ����ʾ��رգ�
/// 3. �ṩ���ֽӿڣ�ToggleInventory(), OpenInventory(), CloseInventory()��
///
/// ʹ�ò��裺
/// - �ڳ����ﴴ��һ���� GameObject������Ϊ "UIManager"���ѱ��ű����������棻
/// - ȷ�� UIManager �� BootRoot ���ӽڵ㣨BootRoot Ҳ������ DontDestroyOnLoad����
/// - �� Inspector �а� InventoryCanvas������ InventoryPanel �� SlotGridArea���ϵ� inventoryCanvas �ֶΣ�
/// - ֮����ڰ���������ť�¼���ֱ�ӵ��� UIManager.Instance.ToggleInventory() ���л�������ʾ/���أ�
/// - �����Ҫ������/�رգ�Ҳ�ɷֱ���� UIManager.Instance.OpenInventory() / CloseInventory()��  
/// </summary>
public class UIManager : MonoBehaviour
{
    // ����ʵ��
    public static UIManager Instance { get; private set; }

   
    public GameObject inventoryCanvas;

    // ���������Ҫ����������פ UI ��壬����������塢�Ի�����壬����������������ֶΣ�
    // public GameObject settingsCanvas;
    // public GameObject dialogueCanvas;
    // �����ȵ�

    private void Awake()
    {
        // �������
        if (Instance == null)
        {
            Instance = this;
            // ��֤�Լ��������ӽڵ����л�����ʱ��������
          

            // ȷ�� inventoryCanvas �ѱ���ֵ
            if (inventoryCanvas != null)
            {
                Debug.Log("[UIManager] Awake ʱ���ر���");
                inventoryCanvas.SetActive(false);
            }
            else
            {
                Debug.LogError("[UIManager] Awake ��⵽ inventoryCanvas Ϊ null��");
            }

            // ������б�ĳ�פ��壬Ҳ��������������
            // if (settingsCanvas != null) settingsCanvas.SetActive(false);
            // if (dialogueCanvas != null) dialogueCanvas.SetActive(false);
        }
        else
        {
            // �������������������˵ڶ��� UIManager�������ٵ���
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �л�����������ʾ/����
    /// </summary>
    public void ToggleInventory()
    {
        if (inventoryCanvas == null)
        {
            Debug.LogError("[UIManager] inventoryCanvas ����Ϊ�գ��޷��л�������ʾ��");
            return;
        }

        bool isActive = inventoryCanvas.activeSelf;
        inventoryCanvas.SetActive(!isActive);
    }

    /// <summary>
    /// ǿ�ƴ򿪱�������
    /// </summary>
    public void OpenInventory()
    {
        if (inventoryCanvas == null)
        {
            Debug.LogError("[UIManager] inventoryCanvas ����Ϊ�գ��޷��򿪱�����");
            return;
        }

        if (!inventoryCanvas.activeSelf)
        {
            inventoryCanvas.SetActive(true);
        }
    }

    /// <summary>
    /// ǿ�ƹرձ�������
    /// </summary>
    public void CloseInventory()
    {
        if (inventoryCanvas == null)
        {
            Debug.LogError("[UIManager] inventoryCanvas ����Ϊ�գ��޷��رձ�����");
            return;
        }

        if (inventoryCanvas.activeSelf)
        {
            inventoryCanvas.SetActive(false);
        }
    }
}
