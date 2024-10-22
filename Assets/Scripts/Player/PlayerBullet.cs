using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public int damage = 10;
    private bool hasCollided = false; 

    void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return; 
        if (collision.gameObject.CompareTag("Player")){
            Destroy(gameObject);
        }
        else if(collision.gameObject.CompareTag("Enemy")){
            EnemyHealthController healthController = collision.gameObject.GetComponent<EnemyHealthController>();

            if (healthController != null) 
            {
                healthController.TakeDamage(damage);
            }
            if(healthController.currentHealth <= 0){
                Destroy(collision.gameObject);
            }

        }
        else{
            Destroy(gameObject);
        }
    }
}
