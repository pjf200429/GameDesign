using UnityEngine;
using TMPro;

using UnityEngine.UI;

public class GameOverUIManager : MonoBehaviour
{
    public TextMeshProUGUI gameOverText; // Reference to the "Game Over" text
    public TextMeshProUGUI scoreText;    // Reference to the "Score" text
    public Button closeButton;           // Reference to the close button

    private void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseGameOverUI);
        }
        else
        {
            Debug.LogWarning("GameOverUIManager: closeButton is not assigned in the Inspector.");
        }
    }

    /// <summary>
    /// Displays the Game Over UI and updates the score label.
    /// </summary>
    /// <param name="score">Final score to display.</param>
    /// <param name="isNewHighScore">Whether this score is a new high score.</param>
    public void ShowGameOverUI(int score, bool isNewHighScore)
    {
        gameObject.SetActive(true);

        gameOverText.text = "Game Over";
        if (isNewHighScore)
            scoreText.text = "New High Score: " + score;
        else
            scoreText.text = "Score: " + score;
    }

    /// <summary>
    /// Hides the Game Over UI when the close button is clicked.
    /// </summary>
    private void CloseGameOverUI()
    {
        gameObject.SetActive(false);
    }
}
