using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject chestUIPrefab; 
    public Transform uiSpawnPoint;    

    private bool isOpened = false;
    private GameObject currentUI;

    void OnMouseDown()
    {
        if (isOpened) return;

 
        if (currentUI == null && chestUIPrefab != null)
        {
            currentUI = Instantiate(chestUIPrefab, uiSpawnPoint.position, Quaternion.identity, null);
            ChestUI chestUI = currentUI.GetComponent<ChestUI>();
            if (chestUI != null)
            {
                chestUI.Init(this); 
            }
        }
    }


    public void CloseChest()
    {
        isOpened = true;
        if (currentUI != null)
        {
            Destroy(currentUI);
            currentUI = null;
        }
    
    }
}
