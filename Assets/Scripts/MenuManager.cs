using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Seçilen zorluk (oyun boyunca kalsın)
    public enum Difficulty { Easy, Hard }
    public static Difficulty SelectedDifficulty = Difficulty.Easy;

    private void Awake()
    {
        // MenuManager sahneler arası silinmesin
        DontDestroyOnLoad(gameObject);
    }

    // START MENU BUTTONS
    public void StartEasy()
    {
        SelectedDifficulty = Difficulty.Easy;
        SceneManager.LoadScene("Level1Easy");
    }

    public void StartHard()
    {
        SelectedDifficulty = Difficulty.Hard;
        SceneManager.LoadScene("Level1Hard");
    }

    // WIN PANEL "NEXT LEVEL" BUTTON
    public void LoadNextLevel()
    {
        string current = SceneManager.GetActiveScene().name;

        if (SelectedDifficulty == Difficulty.Easy)
        {
            if (current == "Level1Easy") SceneManager.LoadScene("Level2Easy");
            else if (current == "Level2Easy") SceneManager.LoadScene("Level3Easy");
            else if (current == "Level3Easy") SceneManager.LoadScene("StartMenu");
            else SceneManager.LoadScene("Level1Easy");
        }
        else // Hard
        {
            if (current == "Level1Hard") SceneManager.LoadScene("Level2Hard");
            else if (current == "Level2Hard") SceneManager.LoadScene("Level3Hard");
            else if (current == "Level3Hard") SceneManager.LoadScene("StartMenu");
            else SceneManager.LoadScene("Level1Hard");
        }
    }

    // Diğer butonlar (istersen kalsın)
    public void QuitGame()
    {
        Debug.Log("Quit game called");
        Application.Quit();
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
