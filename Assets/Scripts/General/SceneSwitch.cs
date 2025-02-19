using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
    
    public void MainMenu()
    {
        SceneManager.LoadScene("Start");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
