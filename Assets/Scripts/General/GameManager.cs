using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int playerHP = 10;
    public int gold = 0;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI waveText;
    public GameObject gameOverUI;
    public GameObject goldPrefab;

    private bool isPaused = false;
    private bool isAnimating = false; // Prevents toggling during animation
    public EnemySpawner enemySpawner;

    // Reference to the Canvas Group for blocking input
    private CanvasGroup shopCanvasGroup;

    // References to the Continue and Quit buttons
    public Button continueButton;
    public Button quitButton;
    public GameObject pausePanel;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (hpText == null || goldText == null)
        {
            Debug.LogError("Text fields not assigned in the Inspector.");
        }

        pausePanel.SetActive(false);
    }

    public void PauseGame()
    {
        if (isAnimating) return;

        isPaused = !isPaused;

        // Find all instances of TowerDragHandler in the scene
        TowerDragAndDrop[] towerDragHandlers = FindObjectsOfType<TowerDragAndDrop>();

        if (isPaused)
        {
            Time.timeScale = 0f; // Pause the game

            // Block input to everything except the Continue and Quit buttons
            ToggleButtons(false);
            continueButton.interactable = true;
            quitButton.interactable = true;
            pausePanel.SetActive(true); // Show pause UI

            // Disable all TowerDragHandler components
            foreach (TowerDragAndDrop towerDragHandler in towerDragHandlers)
            {
                towerDragHandler.enabled = false; // Disable the script
            }
        }
        else
        {
            Time.timeScale = 1f; // Resume the game

            // Re-enable all buttons
            ToggleButtons(true);
            pausePanel.SetActive(false); // Hide pause UI

            // Re-enable all TowerDragHandler components
            foreach (TowerDragAndDrop towerDragHandler in towerDragHandlers)
            {
                towerDragHandler.enabled = true; // Enable the script again
            }
        }
    }


    private void ToggleButtons(bool enable)
    {
        // Disable or enable all buttons in the UI except Continue and Quit
        Button[] allButtons = FindObjectsOfType<Button>();
        foreach (Button button in allButtons)
        {
            // Only enable Continue and Quit buttons, disable all others
            if (button != continueButton && button != quitButton)
            {
                button.interactable = enable;
            }
        }
    }

    void Start()
    {
        if (enemySpawner == null)
        {
            enemySpawner = FindObjectOfType<EnemySpawner>();
        }
        UpdateUI();
    }

    public void TakeDamage()
    {
        playerHP--;
        UpdateUI();
        if (playerHP <= 0)
        {
            GameOver();
        }
    }

    public void AddGold(int amount)
    {
        StartCoroutine(LerpGoldUI(gold, gold + amount));
        gold += amount;
    }

    private IEnumerator LerpGoldUI(int startValue, int endValue)
    {
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            int newGold = (int)Mathf.Lerp(startValue, endValue, elapsed / duration);
            goldText.text = "Gold: " + newGold;
            yield return null;
        }

        goldText.text = "Gold: " + endValue;
    }

    public void UpdateUI()
    {
        hpText.text = "HP: " + playerHP;
        goldText.text = "Gold: " + gold;
    }

    void GameOver()
    {
        if (enemySpawner != null)
        {
            enemySpawner.StopSpawning();
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        foreach (GameObject tower in towers)
        {
            Destroy(tower);
        }


        hpText.text = "HP: 0";
        gameOverUI.SetActive(true);
    }

    public void UpdateWaveText(int waveIndex)
    {
        waveText.text = $"Wave {waveIndex + 1}";
    }
}
