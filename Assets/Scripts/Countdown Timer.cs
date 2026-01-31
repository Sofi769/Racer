using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI countdownText;
    public GameObject gameOverPanel;
    public GameObject winPanel;

    [Header("Settings")]
    public float timeRemaining = 60f;

    private bool isGameOver;

    void Start()
    {
        if (countdownText == null) Debug.LogError("Assign countdownText in Inspector!");
        if (gameOverPanel == null) Debug.LogError("Assign gameOverPanel in Inspector!");
        if (winPanel == null) Debug.LogError("Assign winPanel in Inspector!");

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }

    void Update()
    {
        if (isGameOver) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            countdownText.text = "Time: " + Mathf.RoundToInt(timeRemaining);
        }
        else
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        if (countdownText != null) countdownText.text = "Time's Up!";
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void PlayerWins()
    {
        isGameOver = true;
        if (winPanel != null) winPanel.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}