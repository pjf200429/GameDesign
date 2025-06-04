using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 全局唯一的 UI 管理器 (UIManager)：
/// 1. 管理背包面板的打开/关闭；
/// 2. 管理商店主面板（ShopCanvas）的打开/关闭，但不管商店内部逻辑；
/// 3. 在切换到 ShopRoom 场景时，自动查找并缓存 ShopCanvas，当离开该场景时隐藏并置空；  
/// </summary>
public class UIManager : MonoBehaviour
{
    // 单例
    public static UIManager Instance { get; private set; }

    [Header("背包面板（在 Inspector 中赋值）")]
    public GameObject inventoryCanvas;

    // 商店主 Canvas（在 ShopRoom 场景加载时动态查找，无需在 Inspector 赋值）
    private GameObject shopCanvas;

    // “商店场景” 的名字一定要和下面常量一致
    private const string SHOP_SCENE_NAME = "ShopRoom";
    // “商店面板” 的根 GameObject 名称必须和下面常量一致
    private const string SHOP_CANVAS_NAME = "ShopCanvas";

    private void Awake()
    {
        // 单例检查
        if (Instance == null)
        {
            Instance = this;
           

            // 确保背包一开始隐藏
            if (inventoryCanvas != null)
            {
                inventoryCanvas.SetActive(false);
            }
            else
            {
                Debug.LogError("[UIManager] Awake: inventoryCanvas 未赋值！");
            }

            // 一开始没有 ShopCanvas 的引用
            shopCanvas = null;

            // 监听场景加载
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 每次新场景加载完毕后都会调用：
    /// 如果加载的是 ShopRoom 场景，则尝试查找名为 “ShopCanvas” 的 GameObject 并隐藏它；
    /// 如果当前不是 ShopRoom 场景，则把 shopCanvas 隐藏并置为 null 以便 GC。
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == SHOP_SCENE_NAME)
        {
            // 在 ShopRoom 场景里查找 ShopCanvas
            GameObject obj = GameObject.Find(SHOP_CANVAS_NAME);
            if (obj != null)
            {
                shopCanvas = obj;
                shopCanvas.SetActive(false);
                Debug.Log("[UIManager] 在 ShopRoom 场景找到并隐藏 ShopCanvas");
            }
            else
            {
                Debug.LogWarning($"[UIManager] ShopRoom 场景加载完毕，但找不到名为 '{SHOP_CANVAS_NAME}' 的 GameObject");
                shopCanvas = null;
            }
        }
        else
        {
            // 离开 ShopRoom 场景，确保隐藏并置空
            if (shopCanvas != null)
            {
                shopCanvas.SetActive(false);
            }
            shopCanvas = null;
        }
    }

    #region —— Inventory 界面相关 —— 

    public void ToggleInventory()
    {
        if (inventoryCanvas == null)
        {
            Debug.LogError("[UIManager] ToggleInventory: inventoryCanvas 未赋值！");
            return;
        }
        inventoryCanvas.SetActive(!inventoryCanvas.activeSelf);
    }

    public void OpenInventory()
    {
        if (inventoryCanvas == null)
        {
            Debug.LogError("[UIManager] OpenInventory: inventoryCanvas 未赋值！");
            return;
        }
        inventoryCanvas.SetActive(true);
    }

    public void CloseInventory()
    {
        if (inventoryCanvas == null)
        {
            Debug.LogError("[UIManager] CloseInventory: inventoryCanvas 未赋值！");
            return;
        }
        inventoryCanvas.SetActive(false);
    }

    #endregion

    #region —— Shop 主面板打开/关闭 —— 

    /// <summary>
    /// 切换商店主面板（ShopCanvas）的显示/隐藏。
    /// 只有在 shopCanvas 不为 null（处于 ShopRoom 场景）时才生效；否则打印警告。
    /// </summary>
    public void ToggleShop()
    {
        if (shopCanvas == null)
        {
            Debug.LogWarning("[UIManager] ToggleShop: 当前不在 ShopRoom 场景或 ShopCanvas 不存在");
            return;
        }
        shopCanvas.SetActive(!shopCanvas.activeSelf);
    }

    /// <summary>
    /// 强制打开商店面板
    /// </summary>
    public void OpenShop()
    {
        if (shopCanvas == null)
        {
            Debug.LogWarning("[UIManager] OpenShop: 当前不在 ShopRoom 场景或 ShopCanvas 不存在");
            return;
        }
        shopCanvas.SetActive(true);
    }

    /// <summary>
    /// 强制关闭商店面板
    /// </summary>
    public void CloseShop()
    {
        if (shopCanvas == null)
        {
            Debug.LogWarning("[UIManager] CloseShop: 当前不在 ShopRoom 场景或 ShopCanvas 不存在");
            return;
        }
        shopCanvas.SetActive(false);
    }

    #endregion
}
