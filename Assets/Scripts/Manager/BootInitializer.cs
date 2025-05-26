using UnityEngine;
using UnityEngine.SceneManagement;

public class BootInitializer : MonoBehaviour
{
    [Header("Initial Scene Settings")]
    public string firstSceneName = "Home";

    private void Awake()
    {
        // Ensure this initializer object is not destroyed when loading new scenes
        DontDestroyOnLoad(this.gameObject);

      

        // Load the first actual room scene
        SceneManager.LoadScene(firstSceneName);
    }
}




