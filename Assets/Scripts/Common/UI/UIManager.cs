using UnityEngine;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{

    public static UIManager Instance { get; private set; }

    [Header("������壨�� Inspector �и�ֵ��")]
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
                Debug.LogError("[UIManager] Awake: inventoryCanvas δ��ֵ��");
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
            // �� ShopRoom ��������� ShopCanvas
            GameObject obj = GameObject.Find(SHOP_CANVAS_NAME);
            if (obj != null)
            {
                shopCanvas = obj;
                shopCanvas.SetActive(false);
                Debug.Log("[UIManager] �� ShopRoom �����ҵ������� ShopCanvas");
            }
            else
            {
                Debug.LogWarning($"[UIManager] ShopRoom ����������ϣ����Ҳ�����Ϊ '{SHOP_CANVAS_NAME}' �� GameObject");
                shopCanvas = null;
            }
        }
        else
        {
            // �뿪 ShopRoom ������ȷ�����ز��ÿ�
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

    #region ���� Inventory ������� ���� 

    public void ToggleInventory()
    {
        if (inventoryCanvas == null)
        {
            Debug.LogError("[UIManager] ToggleInventory: inventoryCanvas δ��ֵ��");
            return;
        }
        inventoryCanvas.SetActive(!inventoryCanvas.activeSelf);
    }

    public void OpenInventory()
    {
        if (inventoryCanvas == null)
        {
            Debug.LogError("[UIManager] OpenInventory: inventoryCanvas δ��ֵ��");
            return;
        }
        inventoryCanvas.SetActive(true);
    }

    public void CloseInventory()
    {
        if (inventoryCanvas == null)
        {
            Debug.LogError("[UIManager] CloseInventory: inventoryCanvas δ��ֵ��");
            return;
        }
        inventoryCanvas.SetActive(false);
    }

    #endregion

    #region ���� Shop ������/�ر� ���� 

    /// <summary>
    /// �л��̵�����壨ShopCanvas������ʾ/���ء�
    /// ֻ���� shopCanvas ��Ϊ null������ ShopRoom ������ʱ����Ч�������ӡ���档
    /// </summary>
    public void ToggleShop()
    {
        if (shopCanvas == null)
        {
            Debug.LogWarning("[UIManager] ToggleShop: ��ǰ���� ShopRoom ������ ShopCanvas ������");
            return;
        }
        shopCanvas.SetActive(!shopCanvas.activeSelf);
    }

    /// <summary>
    /// ǿ�ƴ��̵����
    /// </summary>
    public void OpenShop()
    {
        if (shopCanvas == null)
        {
            Debug.LogWarning("[UIManager] OpenShop: ��ǰ���� ShopRoom ������ ShopCanvas ������");
            return;
        }
        shopCanvas.SetActive(true);
    }

    /// <summary>
    /// ǿ�ƹر��̵����
    /// </summary>
    public void CloseShop()
    {
        if (shopCanvas == null)
        {
            Debug.LogWarning("[UIManager] CloseShop: ��ǰ���� ShopRoom ������ ShopCanvas ������");
            return;
        }
        shopCanvas.SetActive(false);
    }

    #endregion
}
