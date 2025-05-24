using UnityEngine;

public class AutoDestroyEffect : MonoBehaviour
{
    [Tooltip("特效生存时间（秒）")]
    public float lifeTime = 0.5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}

