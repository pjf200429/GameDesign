using UnityEngine;
public class PlayerPersistent : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject); 
    }
}