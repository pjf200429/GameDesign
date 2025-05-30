using UnityEngine;

public class EventForwarder : MonoBehaviour
{
    public GameObject target;  

    public void OnAttackHitEvent()
    {
        target.SendMessage("OnAttackHitEvent", SendMessageOptions.DontRequireReceiver);
    }
}
