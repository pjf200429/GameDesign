using UnityEngine;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{

    public static UIManager Instance { get; private set; }

    [Header("背包面板（在 Inspector 中赋值）")]
    public GameObject inventoryCanvas;


    private GameObject shopCanvas;


    private const string SHOP_SCENE_NAME = "ShopRoom";

    private const string SHOP_CANVAS_NAME = "ShopCanvas";

    [Header("Score UI")]
    [SerializeField] public GameObject scoreAndCurrencyCanvas;  // Score and Currency UI Canvas
    [SerializeField] public GameObject settlementCanvas;         // Settlement UI Canvas


    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
           

            if (inventoryCanvas != null)
            {
                inventoryCanvas.SetActive(false);
            }
            else
            {
                Debug.LogError("[UIManager] Awake: inventoryCanvas 未赋值！");
            }


            shopCanvas = null;
            if (scoreAndCurrencyCanvas != null)
            {
                scoreAndCurrencyCanvas.SetActive(false);  
            }
            if (settlementCanvas != null)
            {
                settlementCanvas.SetActive(false);  
            }


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
        if (scene.name != "Home" && scoreAndCurrencyCanvas != null)
        {
            scoreAndCurrencyCanvas.SetActive(true);
        }
        if (scene.name== "Home" && scoreAndCurrencyCanvas != null)
        {
            scoreAndCurrencyCanvas.SetActive(false);
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
