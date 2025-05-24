using UnityEngine;

public class AutoDestroyEffect : MonoBehaviour
{
    [Tooltip("��Ч����ʱ�䣨�룩")]
    public float lifeTime = 0.5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}

