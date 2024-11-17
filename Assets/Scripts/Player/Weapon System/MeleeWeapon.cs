using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeWeapon : MonoBehaviour
{
    public Camera playerCamera;

    public int damage = 30;
    public float attackRange = 3f;

    void OnMeleeAttack(InputValue value)
    {
        Attack();
    }

    void Attack()
    {
        Debug.Log("Performing melee attack");
        
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, attackRange))
        {
            EnemyHealthController enemyHealthController = hit.collider.GetComponent<EnemyHealthController>();
            if (enemyHealthController != null)
            {
                enemyHealthController.TakeDamage(damage);
            }
        }
    }
}
