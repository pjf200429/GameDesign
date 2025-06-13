using UnityEngine;
using UnityEngine.SceneManagement;

public class BootInitializer : MonoBehaviour
{
   

    private void Awake()
    {
        // Ensure this initializer object is not destroyed when loading new scenes
        DontDestroyOnLoad(this.gameObject);

      

       
    }
}




