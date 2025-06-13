using UnityEngine;
using TMPro;

public class ScoreAndCurrencyUI : MonoBehaviour
{
    // References to the UI Text components
    public TextMeshProUGUI scoreText;  // For displaying score
    public TextMeshProUGUI currencyText; // For displaying currency

    // References to ScoreManager and PlayerAttributes components
    [SerializeField] private ScoreManager scoreManager;
    private PlayerAttributes playerAttributes;

    private void Start()
    {
        // Get the Player game object and access ScoreManager and PlayerAttributes
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            scoreManager = player.GetComponent<ScoreManager>();
            playerAttributes = player.GetComponent<PlayerAttributes>();
        }
    }

    private void Update()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
           
            playerAttributes = player.GetComponent<PlayerAttributes>();
        }
        // Only update the UI if ScoreManager and PlayerAttributes are valid
        if (scoreManager != null && playerAttributes != null)
        {
            // Combine the score from ScoreManager and PlayerAttributes
            int totalScore = scoreManager.Score + playerAttributes.Score;
            int totalCurrency = scoreManager.Currency + playerAttributes.Currency;

            // Update the UI Text components
            scoreText.text = "Score: " + totalScore;
            currencyText.text = "Currency: " + totalCurrency;
        }
    }
}
