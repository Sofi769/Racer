using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject winPanel;
    public GameObject gameOverPanel;

    [Header("UI Scripts")]
    public WinScreen winScreen;       // WinPanel üzerinde olmalı
    public LoseScreen loseScreen;     // GameOverPanel üzerinde olmalı

    [Header("Controllers")]
    public PlayerController playerController;
    public MonoBehaviour enemyAI;     // Enemy AI scriptini inspector'dan ver

    [Header("Timer (optional)")]
    public CountdownTimer countdownTimer;

    [Header("Car Visual (optional)")]
    public CarSpriteDimmer carDimmer;

    [Header("Scoring Settings (timeRemaining üzerinden)")]
    public float threeStarTime = 20f;
    public float twoStarTime = 10f;

    private Rigidbody2D playerRb;
    private bool raceEnded = false;

    private void Awake()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    private void Start()
    {
        if (winScreen == null) winScreen = FindFirstObjectByType<WinScreen>(FindObjectsInactive.Include);
        if (loseScreen == null) loseScreen = FindFirstObjectByType<LoseScreen>(FindObjectsInactive.Include);

        if (playerController == null) playerController = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);
        if (countdownTimer == null) countdownTimer = FindFirstObjectByType<CountdownTimer>(FindObjectsInactive.Include);

        if (playerController != null)
        {
            playerRb = playerController.GetComponent<Rigidbody2D>();
            if (carDimmer == null) carDimmer = playerController.GetComponent<CarSpriteDimmer>();
        }

        raceEnded = false;
        Time.timeScale = 1f;
    }

    public void CompleteLevel()
    {
        if (raceEnded) return;
        raceEnded = true;

        Debug.Log("LEVEL END -> WIN (CompleteLevel called)");

        StopRaceMovement();

        if (carDimmer != null)
            carDimmer.DimForWin();

        int stars = CalculateStars();

        if (winPanel != null)
        {
            winPanel.SetActive(true);
            ForcePanelVisible(winPanel);
        }

        if (winScreen != null)
            winScreen.ShowStars(stars);

        if (countdownTimer != null)
            countdownTimer.StopTimer();

        Time.timeScale = 0f;
    }

    public void GameOver(string reason = null)
    {
        if (raceEnded) return;
        raceEnded = true;

        Debug.Log("LEVEL END -> LOSE (GameOver called) Reason: " + reason);

        StopRaceMovement();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            ForcePanelVisible(gameOverPanel);
        }

        if (loseScreen != null)
            loseScreen.ShowLose(reason);

        if (countdownTimer != null)
            countdownTimer.StopTimer();

        Time.timeScale = 0f;
    }

    private void ForcePanelVisible(GameObject panel)
    {
        // Scale bazen 0 kalıyor, garanti 1 yap
        panel.transform.localScale = Vector3.one;

        // CanvasGroup varsa görünür + tıklanır yap
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
    }

    private void StopRaceMovement()
    {
        if (playerController != null) playerController.enabled = false;
        if (enemyAI != null) enemyAI.enabled = false;

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
            playerRb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private int CalculateStars()
    {
        bool perfectRun = playerController != null && !playerController.TouchedGrass && !playerController.HitWall;

        float remaining = (countdownTimer != null) ? countdownTimer.timeRemaining : -1f;

        // Timer yoksa: perfect 3, değilse 2
        if (remaining < 0f) return perfectRun ? 3 : 2;

        if (remaining >= threeStarTime && perfectRun) return 3;
        if (remaining >= twoStarTime) return 2;
        return 1;
    }
}
