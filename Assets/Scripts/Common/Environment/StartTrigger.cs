using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StartTrigger : MonoBehaviour
{
    [Header("Stage to Begin (1-based)")]
    public int startStage = 1;


    bool _triggered = false;
    Collider2D _col;

    void Awake()
    {

        _col = GetComponent<Collider2D>();
        if (_col == null)
        {

        }
        else
        {
            _col.isTrigger = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
       

        if (_triggered)
        {
          
            return;
        }

        if (!other.CompareTag("Player"))
        {
         
            return;
        }

        _triggered = true;
     


        var rm = FindObjectOfType<RoomManager>();
        if (rm == null)
        {
          
        }
        else
        {
            rm.GenerateStage(startStage);
  
        }

 
        var lm = FindObjectOfType<LevelManager>();
        if (lm == null)
        {
          
        }
        else
        {
            lm.BeginStage(startStage);
          
          
        }

    
        gameObject.SetActive(false);
    }
}
