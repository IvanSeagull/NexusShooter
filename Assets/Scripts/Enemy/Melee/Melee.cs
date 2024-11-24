using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    public int damage = 10;
    public float attackRange = 2f; 
    public float attackCooldown = 1f; 
    private float nextAttackTime = 0f;

    private HealthController healthController;
    public Animator animator;

    private EnemyAI enemyAI;

    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        healthController = GetComponent<HealthController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, enemyAI.player.position); // Calculate distance between the enemy and the player

        if (distanceToPlayer < attackRange && Time.time >= nextAttackTime) // Check if the player is within attack range and it's time to attack
        {
            GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(transform.position);
            enemyAI.isAttacking = true;
            //animator.SetBool("isRunning", false);
            animator.SetTrigger("punch");

            // Attack();

            enemyAI.isAttacking = false;
            nextAttackTime = Time.time + attackCooldown; // Set next attack time
        }
    }

    void Attack()
    {

        // Perform the attack
        PlayerController playerController = enemyAI.player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.GetComponent<HealthController>().TakeDamage(damage);
            // Debug.Log("MELEEE " + playerController.GetComponent<HealthController>().currentHealth); // Debug log to print the player's health
        }
    }
}
