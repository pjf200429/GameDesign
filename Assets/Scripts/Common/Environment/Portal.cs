// Portal.cs
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Portal : MonoBehaviour
{
    [Header("Delay before teleport (seconds)")]
    public float delay = 0.2f;

    [Header("Deactivate current room after teleport")]
    public bool deactivateCurrentRoom = true;

    bool _triggered = false;

    private ScoreManager scoreManager;
    Collider2D _col;

    void Awake()
    {
        _col = GetComponent<Collider2D>();
        if (_col != null)
            _col.isTrigger = true;
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered || !other.CompareTag("Player")) return;
        _triggered = true;
        Debug.Log("[Portal] Player entered portal, teleporting...");
        StartCoroutine(DoTeleport(other.gameObject));
    }

    IEnumerator DoTeleport(GameObject player)
    {
        yield return new WaitForSeconds(delay);

        // Get current health from PlayerHealthController
        var healthCtrl = player.GetComponent<PlayerHealthController>();
        var attr = player.GetComponent<PlayerAttributes>();

        if (healthCtrl != null && attr != null)
        {
            int curHealth = healthCtrl.getCurrentHealth;
            attr.SetTeleportHealth(curHealth);

            // Transfer score and currency from ScoreManager to PlayerAttributes
            
            if (scoreManager != null)
            {
                // Pass the score and currency to PlayerAttributes
                attr.ReceiveScoreAndCurrency(scoreManager.Score, scoreManager.Currency);
                // Reset the score and currency in ScoreManager
                scoreManager.Reset();
            }
            else
            {
                Debug.LogWarning("[Portal] Missing ScoreManager script on Player.");
            }
        }
        else
        {
            Debug.LogWarning("[Portal] Missing PlayerHealthController or PlayerAttributes.");
        }

        // Switch room/stage
        var lm = FindObjectOfType<LevelManager>();
        if (lm != null)
        {
            lm.NextRoomOrStage();
            Debug.Log("[Portal] Requested next room.");
        }
        else
        {
            Debug.LogError("[Portal] LevelManager not found!");
        }

        if (deactivateCurrentRoom)
        {
            transform.root.gameObject.SetActive(false);
            Debug.Log("[Portal] Current room deactivated.");
        }

        gameObject.SetActive(false);
    }



}
