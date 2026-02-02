using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI countdownText;

    [Header("Settings")]
    public float timeRemaining = 60f;

    private bool stopped;
    private LevelManager levelManager;

    void Start()
    {
        if (countdownText == null) Debug.LogError("Assign countdownText in Inspector!");
        levelManager = FindFirstObjectByType<LevelManager>(FindObjectsInactive.Include);
        stopped = false;
    }

    void Update()
    {
        if (stopped) return;

        if (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            if (countdownText != null)
                countdownText.text = "Time: " + Mathf.RoundToInt(timeRemaining);
        }
        else
        {
            stopped = true;
            if (countdownText != null) countdownText.text = "Time's Up!";
            if (levelManager != null) levelManager.GameOver("Time's Up!");
        }
    }

    public void StopTimer()
    {
        stopped = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
