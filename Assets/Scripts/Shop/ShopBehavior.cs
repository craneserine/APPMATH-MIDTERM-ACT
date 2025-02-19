using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopBehavior : MonoBehaviour
{
    private enum ShopType { Basic, Sniper, Radius }
    
    [Header("Upgrade Buttons")]
    [SerializeField] private Button speedButton;
    [SerializeField] private Button rangeButton;
    
    [Header("Upgrade Costs")]
    [SerializeField] private int sniperSpeedUpgradeCost = 10;
    [SerializeField] private int sniperRangeUpgradeCost = 15;
    [SerializeField] private int basicSpeedUpgradeCost = 20;
    [SerializeField] private int basicRangeUpgradeCost = 25;
    [SerializeField] private int radiusSpeedUpgradeCost = 30;
    [SerializeField] private int bulletCountUpgradeCost = 35;

    [Header("Upgrade Labels")]
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private TextMeshProUGUI shopTitle;
    
    [Header("Button Labels")]
    [SerializeField] private TextMeshProUGUI speedButtonText;
    [SerializeField] private TextMeshProUGUI rangeButtonText;

    private int basicSpeedLevel = 1;
    private int basicRangeLevel = 1;
    private int sniperSpeedLevel = 1;
    private int sniperRangeLevel = 1;
    private int radiusBulletCountLevel = 1;
    private int radiusSpeedLevel = 1;
    
    private const int maxLevel = 5;

    private ShopType currentShop = ShopType.Basic;

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
        
        UpdateShopUI();
        UpdateUpgradeTexts();
    }

    private void Update()
    {
        UpdateButtonState();
    }

    private void UpdateButtonState()
{
    bool hasBasicTowers = FindObjectsOfType<BasicTower>().Length > 0;
    bool hasSniperTowers = FindObjectsOfType<SniperTower>().Length > 0;
    bool hasRadiusTowers = FindObjectsOfType<RadiusTower>().Length > 0;

    int speedUpgradeCost = 0;
    int rangeUpgradeCost = 0;

    // Set the appropriate upgrade costs based on the current shop
    if (currentShop == ShopType.Basic)
    {
        speedUpgradeCost = basicSpeedUpgradeCost;
        rangeUpgradeCost = basicRangeUpgradeCost;
    }
    else if (currentShop == ShopType.Sniper)
    {
        speedUpgradeCost = sniperSpeedUpgradeCost;
        rangeUpgradeCost = sniperRangeUpgradeCost;
    }
    else if (currentShop == ShopType.Radius)
    {
        speedUpgradeCost = radiusSpeedUpgradeCost; 
        rangeUpgradeCost = bulletCountUpgradeCost; // Bullet count cost for scatter
    }

    bool canUpgradeSpeed = GameManager.Instance.gold >= speedUpgradeCost &&
                           (currentShop == ShopType.Radius ? basicSpeedLevel < maxLevel :
                            currentShop == ShopType.Sniper ? sniperSpeedLevel < maxLevel : radiusSpeedLevel < maxLevel);

    bool canUpgradeRange = GameManager.Instance.gold >= rangeUpgradeCost &&
                           (currentShop == ShopType.Basic ? basicRangeLevel < maxLevel :
                            currentShop == ShopType.Sniper ? sniperRangeLevel < maxLevel :
                            currentShop == ShopType.Radius ? radiusBulletCountLevel < maxLevel : false);

    if (currentShop == ShopType.Basic)
    {
        SetButtonState(speedButton, canUpgradeSpeed && hasBasicTowers, basicSpeedLevel);
        SetButtonState(rangeButton, canUpgradeRange && hasBasicTowers, basicRangeLevel);
    }
    else if (currentShop == ShopType.Sniper)
    {
        SetButtonState(speedButton, canUpgradeSpeed && hasSniperTowers, sniperSpeedLevel);
        SetButtonState(rangeButton, canUpgradeRange && hasSniperTowers, sniperRangeLevel);
    }
    else if (currentShop == ShopType.Radius)
    {
        SetButtonState(speedButton, canUpgradeSpeed && hasRadiusTowers, radiusSpeedLevel);
        SetButtonState(rangeButton, canUpgradeRange && hasRadiusTowers, radiusBulletCountLevel); // Using rangeButton for BulletCount
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
        if (currentShop == ShopType.Basic)
        {
            currentUpgradeCost = basicSpeedUpgradeCost;
            currentSpeedLevel = basicSpeedLevel;
        }
        else if (currentShop == ShopType.Sniper)
        {
            currentUpgradeCost = sniperSpeedUpgradeCost;
            currentSpeedLevel = sniperSpeedLevel;
        }
        else if (currentShop == ShopType.Radius)
        {
            currentUpgradeCost = radiusSpeedUpgradeCost; // You can define a specific cost for Radius if necessary
            currentSpeedLevel = radiusSpeedLevel;
        }

        // Check if there is enough gold and if the current level is below the max
        if (GameManager.Instance.gold < currentUpgradeCost || currentSpeedLevel >= maxLevel)
            return;

        // Deduct gold and perform the upgrade
        GameManager.Instance.gold -= currentUpgradeCost;
    
        if (currentShop == ShopType.Basic)
        {
            foreach (var turret in FindObjectsOfType<RadiusTower>())
            {
                turret.UpgradeSpeed(1);  // Adjust as needed
            }

            basicSpeedUpgradeCost += 5;
            basicSpeedLevel++;
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
        else if (currentShop == ShopType.Radius)
        {
            foreach (var tower in FindObjectsOfType<RadiusTower>())
            {
                tower.UpgradeSpeed(1f);  // Adjust as needed
            }
            radiusSpeedUpgradeCost += 5;
            radiusSpeedLevel++;
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
        if (currentShop == ShopType.Basic)
        {
            currentUpgradeCost = basicRangeUpgradeCost;
            currentRangeLevel = basicRangeLevel;
        }
        else if (currentShop == ShopType.Sniper)
        {
            currentUpgradeCost = sniperRangeUpgradeCost;
            currentRangeLevel = sniperRangeLevel;
        }
        else if (currentShop == ShopType.Radius)
        {
            currentUpgradeCost = bulletCountUpgradeCost; 
        }

        if (GameManager.Instance.gold < currentUpgradeCost || currentRangeLevel >= maxLevel)
            return;

        GameManager.Instance.gold -= currentUpgradeCost;
    
        if (currentShop == ShopType.Basic)
        {
            foreach (var turret in FindObjectsOfType<RadiusTower>())
            {
                turret.UpgradeRange(0.3f); 
            }
            basicRangeUpgradeCost += 5;
            basicRangeLevel++;
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
        else if (currentShop == ShopType.Radius)
        {
            foreach (var tower in FindObjectsOfType<RadiusTower>())
            {
                tower.IncreaseBulletCount(1); 
            }
            bulletCountUpgradeCost += 5;
            radiusBulletCountLevel++;
        }

        // Update UI and texts after upgrade
        UpdateUpgradeTexts();
        GameManager.Instance.UpdateUI();
    }

    private void UpdateShopUI()
    {
        if (currentShop == ShopType.Basic)
        {
            shopTitle.text = "Basic Tower Upgrades";
        }
        else if (currentShop == ShopType.Sniper)
        {
            shopTitle.text = "Sniper Tower Upgrades";
        }
        else if (currentShop == ShopType.Radius)
        {
            shopTitle.text = "Radius Tower Upgrades";
        }
        UpdateUpgradeTexts();
    }

    private void UpdateUpgradeTexts()
    {
        if (currentShop == ShopType.Basic)
        {
            speedText.text = "Attack Speed Lvl. " + basicSpeedLevel;
            rangeText.text = "Tower Range Lvl. " + basicRangeLevel;
            speedButtonText.text = basicSpeedUpgradeCost + " Gold";
            rangeButtonText.text = basicRangeUpgradeCost + " Gold";
        }
        else if (currentShop == ShopType.Sniper)
        {
            speedText.text = "Attack Speed Lvl. " + sniperSpeedLevel;
            rangeText.text = "Sniper Range Lvl. " + sniperRangeLevel;
            speedButtonText.text = sniperSpeedUpgradeCost + " Gold";
            rangeButtonText.text = sniperRangeUpgradeCost + " Gold";
        }
        else if (currentShop == ShopType.Radius)
        {
            speedText.text = "Attack Speed Lvl. " + radiusSpeedLevel;
            rangeText.text = "Bullet Count Lvl. " + radiusBulletCountLevel;
            speedButtonText.text = radiusSpeedUpgradeCost + " Gold";
            rangeButtonText.text = bulletCountUpgradeCost + " Gold";
        }
    }
    public int GetSpeedLevel()
    {
        return (currentShop == ShopType.Basic) ? basicSpeedLevel : 
            (currentShop == ShopType.Sniper) ? sniperSpeedLevel : radiusSpeedLevel;
    }

    public int GetRangeLevel()
    {
        return (currentShop == ShopType.Basic) ? basicRangeLevel : 
            (currentShop == ShopType.Sniper) ? sniperRangeLevel : radiusBulletCountLevel;
    }
}