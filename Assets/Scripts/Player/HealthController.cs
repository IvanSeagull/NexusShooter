using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth = 100;

    public Slider healthBar;

    void Start()
    {
        currentHealth = maxHealth;

        // Initialize the health bar with the player's starting health
       UpdateHealthBar(currentHealth, maxHealth);
    }

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    // Method to take damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthBar(currentHealth, maxHealth);

    }

    // Method to heal
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateHealthBar(currentHealth, maxHealth);
    }
}
