using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("The target Transform to follow (usually the Player).")]
    public Transform target;

    [Tooltip("Time in seconds for the camera to catch up to the target.")]
    public float smoothTime = 0.2f;

    // the velocity
    Vector3 velocity = Vector3.zero;

    void Start()
    {
    
    }

    void LateUpdate()
    {
        if (target == null) return;

        
        Vector3 desiredPosition = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            smoothTime
        );
    }
}
