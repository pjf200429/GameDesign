using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;  // �����ť�� TextMeshPro����Ҫ���ø������ռ�

/// <summary>
/// InventoryUI��
/// 1. ����ʱ��̬ʵ�������ɸ��ӣ�SlotPrefab���� SlotGridArea��
/// 2. ���� PlayerInventory �ı仯�¼����Զ�ˢ��ÿ�����ӵ� Icon/������
/// 3. ���� SlotOptionPanel������������ť�����顢������ʹ�ã���
/// 4. ��ʼʱ����һ��ˢ�£�ȷ�� UI �뱳������ͬ����
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("����Ԥ���� (SlotPrefab)")]
    [Tooltip("������ SlotUI �ű���Ԥ�����ϵ��˴�")]
    public GameObject slotPrefab;

    [Header("���Ӹ��ڵ� (SlotGridArea)")]
    [Tooltip("UI �㼶�У����� GridLayoutGroup �Ŀ�����")]
    public Transform slotParent;

    [Header("��ұ��� (PlayerInventory)")]
    [Tooltip("�����й��� PlayerInventory ����Ҷ���")]
    public PlayerInventory playerInventory;

    // ����ʱʵ�������������и��Ӷ�Ӧ�� SlotUI ���
    private List<SlotUI> slotUIs = new List<SlotUI>();

    // �����������̶�������ͳһ�� 15 �����ӡ�Ҳ���Ը��� playerInventory.maxSlots ��̬������
    private int slotCount = 15;

    [Header("������� (SlotOptionPanel)")]
    [Tooltip("������/����/ʹ�á�������ť���ڵĸ����")]
    public GameObject slotOptionPanel;

    [Header("������尴ť����")]
    public Button btnDetails;  // Inspector ���롰���顱��ť
    public Button btnDrop;     // Inspector ���롰��������ť
    public Button btnUse;      // Inspector ���롰ʹ�á���ť

    private void Awake()
    {
        // ����� Inspector ��û�� playerInventory�����Զ�����
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            if (playerInventory == null)
                Debug.LogError("[InventoryUI] �������Ҳ��� PlayerInventory������ Inspector ���ֶ���ֵ��");
        }
    }

    private void Start()
    {
        // У�飺slotPrefab��slotParent��playerInventory��slotOptionPanel������ť�������� Inspector �︳��
        bool error = false;
        if (slotPrefab == null)
        {
            Debug.LogError("[InventoryUI] ���� Inspector ��� slotPrefab �Ϻã�");
            error = true;
        }
        if (slotParent == null)
        {
            Debug.LogError("[InventoryUI] ���� Inspector ��� slotParent �Ϻã�");
            error = true;
        }
        if (playerInventory == null)
        {
            Debug.LogError("[InventoryUI] ���� Inspector ��� playerInventory �Ϻã�");
            error = true;
        }
        if (slotOptionPanel == null)
        {
            Debug.LogError("[InventoryUI] ���� Inspector ��� slotOptionPanel ��������壩�Ϻã�");
            error = true;
        }
        if (btnDetails == null || btnDrop == null || btnUse == null)
        {
            Debug.LogError("[InventoryUI] ���� Inspector ��� btnDetails/btnDrop/btnUse ������ť�Ϻã�");
            error = true;
        }
        if (error) return;

        // 1. ��̬ʵ���� slotCount ������
        for (int i = 0; i < slotCount; i++)
        {
            GameObject go = Instantiate(slotPrefab, slotParent);
            go.name = "Slot_" + i;
            SlotUI ui = go.GetComponent<SlotUI>();
            if (ui == null)
            {
                Debug.LogError("[InventoryUI] SlotPrefab Ԥ����ĸ��ڵ�ȱ�� SlotUI �ű���");
                Destroy(go);
                continue;
            }

            // �� SlotUI ֪�����Լ����������Լ��ܻص��� InventoryUI
            ui.slotIndex = i;
            ui.inventoryUI = this;

            slotUIs.Add(ui);
        }

        // 2. ���ز�����壬���������ʱ����ʾ
        slotOptionPanel.SetActive(false);

        // 3. ���ı����仯�¼���PlayerInventory.Add/Remove/Use �ڲ��ᴥ�� OnInventoryChanged
        playerInventory.OnInventoryChanged += RefreshUI;

        // 4. �״��ֶ�ˢ��һ�� UI��ȷ����ʼ���������״̬һ��
        RefreshUI();
    }

    /// <summary>
    /// ˢ�� UI���� playerInventory.Items ������ӳ�䵽��Ӧ�ĸ����
    /// ���� Items.Count �ĸ�������գ�Icon=null, Count=0����
    /// </summary>
    public void RefreshUI()
    {
        IReadOnlyList<ItemBase> items = playerInventory.Items;

        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i < items.Count && items[i] != null)
            {
                slotUIs[i].SetIcon(items[i].Icon);

                int qty = 1;
                if (items[i] is ConsumableItem consumable)
                {
                    qty = consumable.Quantity;
                }
                slotUIs[i].SetCount(qty);
            }
            else
            {
                slotUIs[i].SetIcon(null);
                slotUIs[i].SetCount(0);
            }
        }
    }

    private void OnDestroy()
    {
        // ȡ�����ģ����ⳡ���л�����
        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= RefreshUI;
    }

    /// <summary>
    /// �������ĳ������ʱ���� SlotUI ���ã�
    /// �������Ļ���� screenPos ���������������ʾ������������ťע��ص���
    /// </summary>
    /// <param name="slotIndex">������ĸ�������</param>
    /// <param name="screenPos">�����ʱ����Ļ����</param>
    public void ShowOptionPanelAt(int slotIndex, Vector2 screenPos)
    {
        // 1. �������
        slotOptionPanel.SetActive(true);

        // 2. �����ڸ� Canvas ��������ϵ�е�λ�ã����� Canvas Ϊ Screen Space - Overlay ���ͣ�
        RectTransform panelRect = slotOptionPanel.GetComponent<RectTransform>();
        RectTransform parentRect = slotOptionPanel.transform.parent as RectTransform;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            screenPos,
            null,       // Screen Space - Overlay ʱ���������Ϊ null
            out localPoint);

        // 3. ��һ��Сƫ�ƣ����������ס���ָ��
        panelRect.anchoredPosition = localPoint + new Vector2(10f, -10f);

        // 4. �ȰѾɵĵ���¼��Ƴ�����ֹ�ظ�ע��
        btnDetails.onClick.RemoveAllListeners();
        btnDrop.onClick.RemoveAllListeners();
        btnUse.onClick.RemoveAllListeners();

        // 5. ע���µĻص������� slotIndex
        btnDetails.onClick.AddListener(() => OnClick_Details(slotIndex));
        btnDrop.onClick.AddListener(() => OnClick_Drop(slotIndex));
        btnUse.onClick.AddListener(() => OnClick_Use(slotIndex));
    }

    /// <summary>
    /// ��������顱��ťʱ�Ļص��ӿڣ�slotIndex ��ʾ����һ�����ӱ����
    /// </summary>
    private void OnClick_Details(int slotIndex)
    {
        // �������
        slotOptionPanel.SetActive(false);

        // ��ȡ��Ӧ�� ItemBase
        if (slotIndex < playerInventory.Items.Count)
        {
            ItemBase item = playerInventory.Items[slotIndex];
            if (item != null)
            {
                // TODO: ������ʵ�� ���鿴����Ʒ��ϸ��Ϣ�� ���߼�
                Debug.Log($"[InventoryUI] ���飺{item.DisplayName} (ID:{item.ItemID})");
            }
        }
    }

    /// <summary>
    /// �������������ťʱ�Ļص��ӿ�
    /// </summary>
    private void OnClick_Drop(int slotIndex)
    {
        // �������
        slotOptionPanel.SetActive(false);

        if (slotIndex < playerInventory.Items.Count)
        {
            ItemBase item = playerInventory.Items[slotIndex];
            if (item != null)
            {
                // TODO: ������ʵ�� ���ӱ������Ƴ�����Ʒ�� ���߼�
                playerInventory.RemoveItem(item);
                Debug.Log($"[InventoryUI] ������{item.DisplayName} (ID:{item.ItemID})");
            }
        }
    }

    /// <summary>
    /// �����ʹ�á���ťʱ�Ļص��ӿ�
    /// </summary>
    private void OnClick_Use(int slotIndex)
    {
        // �������
        slotOptionPanel.SetActive(false);

        if (slotIndex < playerInventory.Items.Count)
        {
            ItemBase item = playerInventory.Items[slotIndex];
            if (item != null)
            {
                playerInventory.UseItem(item, playerInventory.gameObject);
                Debug.Log($"[InventoryUI] ʹ�ã�{item.DisplayName} (ID:{item.ItemID})");
            }
        }
    }
}
