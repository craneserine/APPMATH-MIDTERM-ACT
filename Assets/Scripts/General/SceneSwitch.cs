using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GameScreen");
    }
    
    public void MainMenu()
    {
        SceneManager.LoadScene("StartScreen");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("GameScreen");
    }
}
