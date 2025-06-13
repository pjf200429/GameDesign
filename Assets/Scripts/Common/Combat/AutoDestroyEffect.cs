using UnityEngine;

public class AutoDestroyEffect : MonoBehaviour
{
    [Tooltip("Effect duration")]
    public float lifeTime = 0.5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}

