using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopBehavior : MonoBehaviour
{
    private enum ShopType { Seeker, Sniper, Scatter }
    
    [Header("Upgrade Buttons")]
    [SerializeField] private Button speedButton;
    [SerializeField] private Button rangeButton;

    [Header("Shop Switching Buttons")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    
    [Header("Upgrade Costs")]
    [SerializeField] private int sniperSpeedUpgradeCost = 10;
    [SerializeField] private int sniperRangeUpgradeCost = 15;
    [SerializeField] private int seekerSpeedUpgradeCost = 20;
    [SerializeField] private int seekerRangeUpgradeCost = 25;
    [SerializeField] private int scatterSpeedUpgradeCost = 30;
    [SerializeField] private int bulletCountUpgradeCost = 35;

    [Header("Upgrade Labels")]
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private TextMeshProUGUI shopTitle;
    
    [Header("Button Labels")]
    [SerializeField] private TextMeshProUGUI speedButtonText;
    [SerializeField] private TextMeshProUGUI rangeButtonText;

    private int seekerSpeedLevel = 1;
    private int seekerRangeLevel = 1;
    private int sniperSpeedLevel = 1;
    private int sniperRangeLevel = 1;
    private int scatterBulletCountLevel = 1;
    private int scatterSpeedLevel = 1;
    
    private const int maxLevel = 5;

    private ShopType currentShop = ShopType.Seeker;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is null");
            return;
        }
        // Add listeners for upgrades
        speedButton.onClick.AddListener(UpgradeSpeed);
        rangeButton.onClick.AddListener(UpgradeRange);
        
        // Add listeners for shop switching
        nextButton.onClick.AddListener(SwitchToNextShop);
        previousButton.onClick.AddListener(SwitchToPreviousShop);
        
        UpdateShopUI();
        UpdateUpgradeTexts();
    }

    private void Update()
    {
        UpdateButtonState();
    }

    private void UpdateButtonState()
{
    bool hasSeekerTowers = FindObjectsOfType<BasicTower>().Length > 0;
    bool hasSniperTowers = FindObjectsOfType<SniperTower>().Length > 0;
    bool hasScatterTowers = FindObjectsOfType<RadiusTower>().Length > 0;

    int speedUpgradeCost = 0;
    int rangeUpgradeCost = 0;

    // Set the appropriate upgrade costs based on the current shop
    if (currentShop == ShopType.Seeker)
    {
        speedUpgradeCost = seekerSpeedUpgradeCost;
        rangeUpgradeCost = seekerRangeUpgradeCost;
    }
    else if (currentShop == ShopType.Sniper)
    {
        speedUpgradeCost = sniperSpeedUpgradeCost;
        rangeUpgradeCost = sniperRangeUpgradeCost;
    }
    else if (currentShop == ShopType.Scatter)
    {
        speedUpgradeCost = scatterSpeedUpgradeCost; 
        rangeUpgradeCost = bulletCountUpgradeCost; // Bullet count cost for scatter
    }

    bool canUpgradeSpeed = GameManager.Instance.gold >= speedUpgradeCost &&
                           (currentShop == ShopType.Seeker ? seekerSpeedLevel < maxLevel :
                            currentShop == ShopType.Sniper ? sniperSpeedLevel < maxLevel : scatterSpeedLevel < maxLevel);

    bool canUpgradeRange = GameManager.Instance.gold >= rangeUpgradeCost &&
                           (currentShop == ShopType.Seeker ? seekerRangeLevel < maxLevel :
                            currentShop == ShopType.Sniper ? sniperRangeLevel < maxLevel :
                            currentShop == ShopType.Scatter ? scatterBulletCountLevel < maxLevel : false);

    if (currentShop == ShopType.Seeker)
    {
        SetButtonState(speedButton, canUpgradeSpeed && hasSeekerTowers, seekerSpeedLevel);
        SetButtonState(rangeButton, canUpgradeRange && hasSeekerTowers, seekerRangeLevel);
    }
    else if (currentShop == ShopType.Sniper)
    {
        SetButtonState(speedButton, canUpgradeSpeed && hasSniperTowers, sniperSpeedLevel);
        SetButtonState(rangeButton, canUpgradeRange && hasSniperTowers, sniperRangeLevel);
    }
    else if (currentShop == ShopType.Scatter)
    {
        SetButtonState(speedButton, canUpgradeSpeed && hasScatterTowers, scatterSpeedLevel);
        SetButtonState(rangeButton, canUpgradeRange && hasScatterTowers, scatterBulletCountLevel); // Using rangeButton for BulletCount
    }
}

    private void SetButtonState(Button button, bool canUpgrade, int level)
    {
        button.interactable = canUpgrade;
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

        if (level >= maxLevel)
        {
            buttonText.text = "Fully Upgraded";
            button.interactable = false;
        }

        ColorBlock colors = button.colors;
        colors.normalColor = canUpgrade ? Color.white : Color.gray;
        colors.disabledColor = Color.gray;
        button.colors = colors;
    }

    private void UpgradeSpeed()
    {
        int currentUpgradeCost = 0;
        int currentSpeedLevel = 0;

        // Determine the current upgrade cost and level based on the shop type
        if (currentShop == ShopType.Seeker)
        {
            currentUpgradeCost = seekerSpeedUpgradeCost;
            currentSpeedLevel = seekerSpeedLevel;
        }
        else if (currentShop == ShopType.Sniper)
        {
            currentUpgradeCost = sniperSpeedUpgradeCost;
            currentSpeedLevel = sniperSpeedLevel;
        }
        else if (currentShop == ShopType.Scatter)
        {
            currentUpgradeCost = scatterSpeedUpgradeCost; // You can define a specific cost for Scatter if necessary
            currentSpeedLevel = scatterSpeedLevel;
        }

        // Check if there is enough gold and if the current level is below the max
        if (GameManager.Instance.gold < currentUpgradeCost || currentSpeedLevel >= maxLevel)
            return;

        // Deduct gold and perform the upgrade
        GameManager.Instance.gold -= currentUpgradeCost;
    
        if (currentShop == ShopType.Seeker)
        {
            foreach (var turret in FindObjectsOfType<RadiusTower>())
            {
                turret.UpgradeSpeed(1);  // Adjust as needed
            }

            seekerSpeedUpgradeCost += 5;
            seekerSpeedLevel++;
        }
        else if (currentShop == ShopType.Sniper)
        {
            foreach (var turret in FindObjectsOfType<RadiusTower>())
            {
                turret.UpgradeSpeed(1);  // Adjust as needed
            }
            sniperSpeedUpgradeCost += 5;
            sniperSpeedLevel++;
        }
        else if (currentShop == ShopType.Scatter)
        {
            foreach (var tower in FindObjectsOfType<RadiusTower>())
            {
                tower.UpgradeSpeed(1f);  // Adjust as needed
            }
            scatterSpeedUpgradeCost += 5;
            scatterSpeedLevel++;
        }

        // Update UI and texts after upgrade
        UpdateUpgradeTexts();
        GameManager.Instance.UpdateUI();
    }


    private void UpgradeRange()
    {
        int currentUpgradeCost = 0;
        int currentRangeLevel = 0;

        // Determine the current upgrade cost and level based on the shop type
        if (currentShop == ShopType.Seeker)
        {
            currentUpgradeCost = seekerRangeUpgradeCost;
            currentRangeLevel = seekerRangeLevel;
        }
        else if (currentShop == ShopType.Sniper)
        {
            currentUpgradeCost = sniperRangeUpgradeCost;
            currentRangeLevel = sniperRangeLevel;
        }
        else if (currentShop == ShopType.Scatter)
        {
            currentUpgradeCost = bulletCountUpgradeCost; 
        }

        if (GameManager.Instance.gold < currentUpgradeCost || currentRangeLevel >= maxLevel)
            return;

        GameManager.Instance.gold -= currentUpgradeCost;
    
        if (currentShop == ShopType.Seeker)
        {
            foreach (var turret in FindObjectsOfType<RadiusTower>())
            {
                turret.UpgradeRange(0.3f); 
            }
            seekerRangeUpgradeCost += 5;
            seekerRangeLevel++;
        }
        else if (currentShop == ShopType.Sniper)
        {
            foreach (var turret in FindObjectsOfType<RadiusTower>())
            {
                turret.UpgradeRange(0.5f); 
            }
            sniperRangeUpgradeCost += 5;
            sniperRangeLevel++;
        }
        else if (currentShop == ShopType.Scatter)
        {
            foreach (var tower in FindObjectsOfType<RadiusTower>())
            {
                tower.IncreaseBulletCount(1); 
            }
            bulletCountUpgradeCost += 5;
            scatterBulletCountLevel++;
        }

        // Update UI and texts after upgrade
        UpdateUpgradeTexts();
        GameManager.Instance.UpdateUI();
    }



    private void SwitchToNextShop()
    {
        currentShop = (currentShop == ShopType.Seeker) ? ShopType.Sniper : 
            (currentShop == ShopType.Sniper) ? ShopType.Scatter : ShopType.Seeker;
        UpdateShopUI();
    }

    private void SwitchToPreviousShop()
    {
        currentShop = (currentShop == ShopType.Sniper) ? ShopType.Seeker : 
            (currentShop == ShopType.Scatter) ? ShopType.Sniper : ShopType.Scatter;
        UpdateShopUI();
    }

    private void UpdateShopUI()
    {
        if (currentShop == ShopType.Seeker)
        {
            shopTitle.text = "Seeker Tower Upgrades";
        }
        else if (currentShop == ShopType.Sniper)
        {
            shopTitle.text = "Sniper Tower Upgrades";
        }
        else if (currentShop == ShopType.Scatter)
        {
            shopTitle.text = "Scatter Tower Upgrades";
        }
        UpdateUpgradeTexts();
    }

    private void UpdateUpgradeTexts()
    {
        if (currentShop == ShopType.Seeker)
        {
            speedText.text = "Attack Speed Lvl. " + seekerSpeedLevel;
            rangeText.text = "Tower Range Lvl. " + seekerRangeLevel;
            speedButtonText.text = seekerSpeedUpgradeCost + " Gold";
            rangeButtonText.text = seekerRangeUpgradeCost + " Gold";
        }
        else if (currentShop == ShopType.Sniper)
        {
            speedText.text = "Attack Speed Lvl. " + sniperSpeedLevel;
            rangeText.text = "Sniper Range Lvl. " + sniperRangeLevel;
            speedButtonText.text = sniperSpeedUpgradeCost + " Gold";
            rangeButtonText.text = sniperRangeUpgradeCost + " Gold";
        }
        else if (currentShop == ShopType.Scatter)
        {
            speedText.text = "Attack Speed Lvl. " + scatterSpeedLevel;
            rangeText.text = "Bullet Count Lvl. " + scatterBulletCountLevel;
            speedButtonText.text = scatterSpeedUpgradeCost + " Gold";
            rangeButtonText.text = bulletCountUpgradeCost + " Gold";
        }
    }
    public int GetSpeedLevel()
    {
        return (currentShop == ShopType.Seeker) ? seekerSpeedLevel : 
            (currentShop == ShopType.Sniper) ? sniperSpeedLevel : scatterSpeedLevel;
    }

    public int GetRangeLevel()
    {
        return (currentShop == ShopType.Seeker) ? seekerRangeLevel : 
            (currentShop == ShopType.Sniper) ? sniperRangeLevel : scatterBulletCountLevel;
    }
}