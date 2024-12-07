using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthController : MonoBehaviour
{
    public Enemy enemy; 
    public int maxHealth = 100;
    private int currentHealth;
    public int xpReward = 10; // Amount of XP to reward when this enemy is defeated
    public event System.Action<int> onDamageTaken; // event to notify subscribers when damage is taken

    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    void Start()
    {
        currentHealth = maxHealth;
    }
    
    // Method to take damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        onDamageTaken?.Invoke(damage); // Notify subscribers

        if (currentHealth <= 0)
        {
            enemy.SetDead();
            gameObject.GetComponent<Collider>().enabled = false;

            // Reward XP when the enemy is defeated
            XPManager.instance.AddXP(xpReward);
        }
    }

    // Method to heal
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }
}
