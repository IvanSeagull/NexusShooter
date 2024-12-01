using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace

public class HealthController : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth = 100;

    public Slider healthBar;
    public Slider armourBar;

    public int armourDurability;

    // Armour tiers and their corresponding damage multipliers
    public enum ArmourTier { None, Light, Medium, Heavy }
    public ArmourTier currentArmourTier = ArmourTier.None;
    private Dictionary<ArmourTier, float> armourMultipliers = new Dictionary<ArmourTier, float>
    {
        { ArmourTier.None, 1.0f },
        { ArmourTier.Light, 0.75f },
        { ArmourTier.Medium, 0.5f },
        { ArmourTier.Heavy, 0.25f }
    };

    void Start()
    {
        currentHealth = maxHealth;

        // Initialize the health bar with the player's starting health
        UpdateHealthBar(currentHealth, maxHealth);
        UpdateArmourUI(); // Initialize the armour text
    }

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    // Method to take damage
    public void TakeDamage(int damage)
    {
        float damageMultiplier = armourMultipliers[currentArmourTier];
        int adjustedDamage = Mathf.RoundToInt(damage * damageMultiplier);

        armourDurability -= damage - adjustedDamage;     // armour absorbs rest of damage
        armourBar.value = armourDurability;

        if (armourDurability <= 0)
        {
            armourDurability = 0;
            currentArmourTier = ArmourTier.None;
            UpdateArmourUI();
        }

        currentHealth -= adjustedDamage;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthBar(currentHealth, maxHealth);

        if (currentHealth <= 0) 
        {
            FindFirstObjectByType<GameController>().Die();
            gameObject.GetComponent<PlayerController>().SetDead();
        }
    }

    // Method to heal
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateHealthBar(currentHealth, maxHealth);
    }

    // Method to change armour tier
    public void SetArmourTier(ArmourTier newArmourTier)
    {
        currentArmourTier = newArmourTier;
        armourBar.value = armourDurability = 100;
        UpdateArmourUI(); // Update the armour text when the tier changes
    }

    // Method to increase max health
    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth; // Heal the player to full health
        UpdateHealthBar(currentHealth, maxHealth);
        Debug.Log("Max health increased. New max health: " + maxHealth);
    }

    // Method to update the armour text
    private void UpdateArmourUI()
    {
        switch (currentArmourTier)
        {
            case ArmourTier.None:
                armourBar.gameObject.SetActive(false);
                return;

            case ArmourTier.Light:
                armourBar.fillRect.GetComponent<Image>().color = new Color(0.804f, 0.498f, 0.196f);
                armourBar.gameObject.SetActive(true);
                break;

            case ArmourTier.Medium:
                armourBar.fillRect.GetComponent<Image>().color = new Color(0.753f, 0.753f, 0.753f);
                armourBar.gameObject.SetActive(true);
                break;

            case ArmourTier.Heavy:
                armourBar.fillRect.GetComponent<Image>().color = new Color(1f, 0.843f, 0.0f);
                armourBar.gameObject.SetActive(true);
                break;
        }
    }
}
