using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // Store score and currency
    private int score;
    private int currency;
    private int finalScore;


    // Public properties to access score and currency
    public int Score => score;
    public int Currency => currency;
    public int FinalScore => finalScore;

    // Events that are triggered when score and currency change
    public event System.Action<int> OnScoreChanged;
    public event System.Action<int> OnCurrencyChanged;

    // Initialize score and currency
    private void Start()
    {
        score = 0;
        currency = 0;
    }

    // Add to the score
    public void AddScore(int amount)
    {
        score += amount;
        OnScoreChanged?.Invoke(score);  // Trigger event when score changes
    }

    // Add to the currency
    public void AddCurrency(int amount)
    {
        currency += amount;
        OnCurrencyChanged?.Invoke(currency);  // Trigger event when currency changes
    }

    // Spend currency (for purchases, etc.)
  
    public bool isMax( )
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerAttributes attr = player.GetComponent<PlayerAttributes>();
        finalScore = attr.Score;
        Debug.Log(finalScore);
        Debug.Log(attr.MaxScore);
        if (attr.MaxScore < attr.Score)
        {
            attr.SetMaxScore(attr.Score);
            return true;
        }
            return false;
        
    }
    // Reset score and currency to zero
    public void Reset()
    {
        score = 0;
        currency = 0;
        OnScoreChanged?.Invoke(score);  // Trigger event when score is reset
        OnCurrencyChanged?.Invoke(currency);  // Trigger event when currency is reset
    }
}
