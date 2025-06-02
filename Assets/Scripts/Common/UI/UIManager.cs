using UnityEngine;

/// <summary>
/// 全局唯一的 UI 管理器 (UIManager)：
/// 1. 在 Awake() 时调用 DontDestroyOnLoad(this.gameObject)，让自己及所有子节点保持常驻；
/// 2. 默认隐藏 InventoryCanvas，只有收到 Toggle/On/Off 调用时才显示或关闭；
/// 3. 提供三种接口：ToggleInventory(), OpenInventory(), CloseInventory()；
///
/// 使用步骤：
/// - 在场景里创建一个空 GameObject，命名为 "UIManager"，把本脚本挂在它上面；
/// - 确认 UIManager 是 BootRoot 的子节点（BootRoot 也调用了 DontDestroyOnLoad）；
/// - 在 Inspector 中把 InventoryCanvas（包含 InventoryPanel → SlotGridArea）拖到 inventoryCanvas 字段；
/// - 之后可在按键监听或按钮事件里直接调用 UIManager.Instance.ToggleInventory() 来切换背包显示/隐藏；
/// - 如果需要单独打开/关闭，也可分别调用 UIManager.Instance.OpenInventory() / CloseInventory()。  
/// </summary>
public class UIManager : MonoBehaviour
{
    // 单例实例
    public static UIManager Instance { get; private set; }

   
    public GameObject inventoryCanvas;

    // 如果后续需要管理其它常驻 UI 面板，比如设置面板、对话框面板，可以在这里再添加字段：
    // public GameObject settingsCanvas;
    // public GameObject dialogueCanvas;
    // ……等等

    private void Awake()
    {
        // 单例检查
        if (Instance == null)
        {
            Instance = this;
            // 保证自己及所有子节点在切换场景时不被销毁
          

            // 确保 inventoryCanvas 已被赋值
            if (inventoryCanvas != null)
            {
                Debug.Log("[UIManager] Awake 时隐藏背包");
                inventoryCanvas.SetActive(false);
            }
            else
            {
                Debug.LogError("[UIManager] Awake 检测到 inventoryCanvas 为 null！");
            }

            // 如果还有别的常驻面板，也可以在这里隐藏
            // if (settingsCanvas != null) settingsCanvas.SetActive(false);
            // if (dialogueCanvas != null) dialogueCanvas.SetActive(false);
        }
        else
        {
            // 如果场景里又意外加载了第二个 UIManager，就销毁掉它
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 切换背包界面显示/隐藏
    /// </summary>
    public void ToggleInventory()
    {
        if (inventoryCanvas == null)
        {
            Debug.LogError("[UIManager] inventoryCanvas 引用为空，无法切换背包显示！");
            return;
        }

        bool isActive = inventoryCanvas.activeSelf;
        inventoryCanvas.SetActive(!isActive);
    }

    /// <summary>
    /// 强制打开背包界面
    /// </summary>
    public void OpenInventory()
    {
        if (inventoryCanvas == null)
        {
            Debug.LogError("[UIManager] inventoryCanvas 引用为空，无法打开背包！");
            return;
        }

        if (!inventoryCanvas.activeSelf)
        {
            inventoryCanvas.SetActive(true);
        }
    }

    /// <summary>
    /// 强制关闭背包界面
    /// </summary>
    public void CloseInventory()
    {
        if (inventoryCanvas == null)
        {
            Debug.LogError("[UIManager] inventoryCanvas 引用为空，无法关闭背包！");
            return;
        }

        if (inventoryCanvas.activeSelf)
        {
            inventoryCanvas.SetActive(false);
        }
    }
}
