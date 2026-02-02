using UnityEngine;

public class RaceFinishTrigger : MonoBehaviour
{
    [Header("References")]
    public LevelManager levelManager;      // player win (stars)
    public CountdownTimer countdownTimer;  // timeRemaining için (opsiyonel)
    public GameObject gameOverPanel;       // enemy win / time up

    [Header("Stop Controllers")]
    public PlayerController playerController;
    public MonoBehaviour enemyAI;          // EnemyCheckpointAI (ya da kullandığın AI)

    private bool raceEnded = false;

    private void Start()
    {
        if (levelManager == null) levelManager = FindObjectOfType<LevelManager>(true);
        if (countdownTimer == null) countdownTimer = FindObjectOfType<CountdownTimer>(true);

        if (playerController == null) playerController = FindObjectOfType<PlayerController>(true);

        // Enemy AI'nı inspector'dan sürüklemen en sağlıklısı. Yine de bulmaya çalışalım:
        if (enemyAI == null)
        {
            // EnemyCheckpointAI scriptinin adını bilmiyoruz, o yüzden inspector önerilir.
            // Burayı boş bırakabilirsin.
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (raceEnded) return;

        if (other.CompareTag("Enemy"))
        {
            raceEnded = true;
            ShowGameOver();
            return;
        }

        if (other.CompareTag("Player"))
        {
            raceEnded = true;
            ShowWin();
            return;
        }
    }

    private void ShowWin()
    {
        StopRaceMovement();

        // ⭐ Win + yıldızlar için tek otorite LevelManager
        if (levelManager != null)
            levelManager.CompleteLevel();
    }

    private void ShowGameOver()
    {
        StopRaceMovement();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    private void StopRaceMovement()
    {
        if (playerController != null)
            playerController.enabled = false;

        if (enemyAI != null)
            enemyAI.enabled = false;
    }
}
