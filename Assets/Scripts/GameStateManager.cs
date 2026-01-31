using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject winPanel;

    void Start()
    {
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void PlayerWins()
    {
        winPanel.SetActive(true);
        Time.timeScale = 0f; 
    }
}