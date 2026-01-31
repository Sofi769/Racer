using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPanelManager : MonoBehaviour
{
    // Method to go to the menu
    public void GoToMenu()
    {
        Debug.Log("GoToMenu called"); 
        SceneManager.LoadScene("StartMenu"); 
    }

    // Method to load the next level
    public void LoadNextLevel()
    {
        Debug.Log("LoadNextLevel called"); 
        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextLevelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextLevelIndex); 
        }
        else
        {
            Debug.Log("No more levels!"); 
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit game called"); 
        Application.Quit();
    }
}