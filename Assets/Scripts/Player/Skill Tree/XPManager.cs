using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMesh Pro namespace

public class XPManager : MonoBehaviour
{
    public static XPManager instance;
    public static int xp = 0;
    public Slider xpSlider; // Reference to the XP Slider
    public TextMeshProUGUI levelText; // Reference to the Level Text
    public TextMeshProUGUI skillPointsText; // Reference to the Skill Points Text
    public HealthController healthController; // Reference to the HealthController script
    public PlayerController playerController; // Reference to the PlayerController script
    private int level = 1;
    private int xpToNextLevel = 10;
    private static int skillPoints = 0; // Skill points that the player can use
    public List<Button> upgradeButtons; // List of upgrade buttons
    public List<TextMeshProUGUI> upgradeCostTexts; // List of upgrade cost texts
    private List<Upgrade> upgrades = new List<Upgrade>(); // List of available upgrades
    public SoundController soundController;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (xpSlider != null)
        {
            xpSlider.maxValue = xpToNextLevel;
            xpSlider.value = xp;
        }

        if (levelText != null)
        {
            levelText.text = "Level: " + level;
        }

        // Initialise the upgrades
        upgrades.Add(new HealthUpgrade("Increase Health", 1, healthController, 10));
        upgrades.Add(new SpeedUpgrade("Increase Speed", 1, playerController, 20.0f));

        UpdateSkillPointsText();
        UpdateUpgradeButtons();
    }

    public void AddXP(int amount)
    {
        xp += amount;
        Debug.Log("XP added: " + amount + ". Total XP: " + xp);
        CheckLevelUp();
        UpdateXPSlider();
    }

    public int GetXP()
    {
        return xp;
    }

    private void CheckLevelUp()
    {
        while (xp >= xpToNextLevel)
        {
            xp -= xpToNextLevel;
            level++;
            soundController.Play(soundController.levelUp, 0.3f);
            xpToNextLevel += 50; // Increase the XP required for the next level
            AwardSkillPoints();
            UpdateLevelText();
        }
    }

    private void AwardSkillPoints()
    {
        if (level <= 5)
        {
            skillPoints += 1;
        }
        else
        {
            skillPoints += 2;
        }
        Debug.Log("Skill points awarded. Total skill points: " + skillPoints);
        UpdateSkillPointsText();
    }

    private void UpdateXPSlider()
    {
        if (xpSlider != null)
        {
            xpSlider.maxValue = xpToNextLevel;
            xpSlider.value = xp;
        }
    }

    private void UpdateLevelText()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + level;
        }
    }

    private void UpdateSkillPointsText()
    {
        if (skillPointsText != null)
        {
            skillPointsText.text = "Skill Points: " + skillPoints;
        }
    }

    private void UpdateUpgradeButtons()
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            if (i < upgradeButtons.Count && i < upgradeCostTexts.Count)
            {
                Upgrade upgrade = upgrades[i];
                upgradeButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = upgrade.name;
                upgradeCostTexts[i].text = "Cost: " + upgrade.skillPointsCost + " Skill Points";
                int index = i; // Capture the index for the lambda
                upgradeButtons[i].onClick.RemoveAllListeners();
                upgradeButtons[i].onClick.AddListener(() => UseSkillPointsForUpgrade(index));
            }
        }
    }

    public void UseSkillPointsForUpgrade(int index)
    {
        if (index < upgrades.Count)
        {
            Upgrade upgrade = upgrades[index];
            if (UseSkillPoints(upgrade.skillPointsCost))
            {
                upgrade.ApplyUpgrade();
                Debug.Log("Skill points used for " + upgrade.name);
                UpdateUpgradeButtons(); // Update the button text and cost
            }
            else
            {
                Debug.Log("Not enough skill points for " + upgrade.name);
            }
        }
    }

    // Public method to use/subtract skill points
    public bool UseSkillPoints(int amount)
    {
        if (skillPoints >= amount)
        {
            skillPoints -= amount;
            Debug.Log("Skill points used: " + amount + ". Total skill points: " + skillPoints);
            UpdateSkillPointsText();
            return true;
        }
        else
        {
            Debug.Log("Not enough skill points. Total skill points: " + skillPoints);
            return false;
        }
    }
}