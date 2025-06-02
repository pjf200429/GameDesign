using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// ÿ�����ӵ� UI �߼���ʹ�� TextMeshPro ����ʾ�ѵ�����
/// ��Ҫ���� SlotPrefab ���ڵ��ϣ�����Ԥ�����´��������ӽڵ㣺
///   1. IconImage (UI��Image)  ���� ������ʾ��Ʒͼ��
///   2. CountText (UI��TextMeshPro - Text) ���� ������ʾ����
///
/// ͬʱʵ���ˣ�
///   - ������������������������Ʒ�򵯳�������壬������Ӧ
///   - ����Ҽ��������������������չ��
///   - ��ק�ӿ�����
/// </summary>
public class SlotUI : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("ͼ�� Image (�ӽڵ�)")]
    public Image iconImage;      // Inspector �����ӽڵ� IconImage �ϵ� Image ���

    [Header("���� TMP_Text (�ӽڵ�)")]
    public TMP_Text countText;   // Inspector �����ӽڵ� CountText �ϵ� TMP_Text ���

    [HideInInspector]
    public int slotIndex;           // ����ʱ�� InventoryUI ��ֵ���ø��ӵ�����

    [HideInInspector]
    public InventoryUI inventoryUI; // ����ʱ�� InventoryUI ��ֵ�����ڻص� ShowOptionPanelAt(...)

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        // ��ʼ����ͼ�������
        if (iconImage != null)
        {
            iconImage.enabled = false;
        }

        if (countText != null)
        {
            countText.enabled = false;
            countText.text = "";
        }

        // ���� CanvasGroup���Ա���קʱ������͸�������ߴ�͸
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            // ���û�й� CanvasGroup�����Զ����һ�����Ա���ק�����п��� alpha / blocksRaycasts
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    /// <summary>
    /// ���ø�����Ҫ��ʾ��ͼ�ꡣ��� icon Ϊ null�������أ�
    /// �������ò����� iconImage.sprite��
    /// </summary>
    public void SetIcon(Sprite icon)
    {
        if (iconImage == null)
            return;

        if (icon != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = icon;
        }
        else
        {
            iconImage.enabled = false;
            iconImage.sprite = null;
        }
    }

    /// <summary>
    /// ���öѵ���������� qty <= 1���������ı���������ʾ��������ֵ
    /// </summary>
    public void SetCount(int qty)
    {
        if (countText == null)
            return;

        if (qty > 1)
        {
            countText.enabled = true;
            countText.text = qty.ToString();
        }
        else
        {
            countText.enabled = false;
            countText.text = "";
        }
    }

    #region ������ק�����ӿ�

    /// <summary>
    /// ����ҵ���ø���ʱ���������ø�������Ʒ���򵯳�������壻�������κβ�����
    /// �Ҽ������������չ�߼�
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log($"Slot clicked: index={slotIndex}");

            // ���жϸò�λ�Ƿ�����Ʒ�����û����ֱ�ӷ��أ��������
            if (inventoryUI != null)
            {
                var items = inventoryUI.playerInventory.Items;
                if (slotIndex < items.Count && items[slotIndex] != null)
                {
                    // ����ø�������Ʒ���򵯳��������
                    inventoryUI.ShowOptionPanelAt(slotIndex, eventData.position);
                }
                else
                {
                    // �ø���Ϊ�գ�ʲôҲ����
                    Debug.Log($"Slot {slotIndex} is empty; no panel shown.");
                }
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // �Ҽ��������������չ���Ҽ�ֱ�Ӷ�����֮���߼�
            Debug.Log($"[SlotUI] �Ҽ�������ӣ����� {slotIndex}");
            // TODO: �����ﴦ���Ҽ���������ֱ�Ӷ���/����ʹ�õȣ�
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // �����ͣʱ������ʾ Tooltip ����Ϣ�����ջ�������չ
        // Debug.Log($"[SlotUI] �����ͣ���ӣ�{slotIndex}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ���� Tooltip ����Ϣ�����ջ�������չ
        // Debug.Log($"[SlotUI] ����뿪���ӣ�{slotIndex}");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // ��ʼ��קʱ���ø��Ӱ�͸�����Ҳ���������
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }
        // TODO: �����Ҫ����ק����ͼ�꣬��������� iconImage ���Ƶ�һ������ Image ��
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ��ק�����У���ʵ�ָ���ͼ��������
        // ���磺
        // floatingIconRect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // ��ק�������ָ����Ӳ�͸����������������
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        // TODO: ������ק����һ�����ӵĽ����߼������磺
        // if (eventData.pointerEnter != null)
        // {
        //     SlotUI targetSlot = eventData.pointerEnter.GetComponent<SlotUI>();
        //     if (targetSlot != null && targetSlot != this)
        //     {
        //         SwapItemWith(targetSlot);
        //     }
        // }
    }

    #endregion
}
