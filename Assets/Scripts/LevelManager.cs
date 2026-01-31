using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Must Assign These")]
    public WinScreen winScreen;
    public PlayerController playerController;
    public CountdownTimer countdownTimer;

    [Header("Scoring Settings")]
    public float threeStarTime = 20f;
    public float twoStarTime = 10f;

    void Start()
    {
        if (winScreen == null) winScreen = FindObjectOfType<WinScreen>(true);
        if (playerController == null) playerController = FindObjectOfType<PlayerController>(true);
        if (countdownTimer == null) countdownTimer = FindObjectOfType<CountdownTimer>(true);

        Debug.Log(winScreen != null ? "WinScreen ready!" : "WARNING: WinScreen missing");
    }

    public void CompleteLevel()
    {
        if (winScreen == null)
        {
            winScreen = FindObjectOfType<WinScreen>(true);
            if (winScreen == null)
            {
                Debug.LogError("CRITICAL: WinScreen reference permanently missing");
                return;
            }
        }

        int stars = CalculateStars();
        winScreen.ShowStars(stars);
    }

    private int CalculateStars()
    {
        float remainingTime = countdownTimer.timeRemaining;
        bool perfectRun = !playerController.TouchedGrass && !playerController.HitWall;

        if (remainingTime >= threeStarTime && perfectRun) return 3;
        if (remainingTime >= twoStarTime) return 2;
        return 1;
    }
}