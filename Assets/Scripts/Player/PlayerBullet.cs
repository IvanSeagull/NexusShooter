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
        print("Collided with " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Player")){
            Destroy(gameObject);
        }
        else if(collision.gameObject.CompareTag("Enemy")){
            // print()
            EnemyHealthController healthController = collision.gameObject.GetComponent<EnemyHealthController>();

            if (healthController != null) 
            {
                healthController.TakeDamage(damage);
                print("Health of enemy is " + healthController.currentHealth);
            }
            if(healthController.currentHealth <= 0){
                Destroy(collision.gameObject);
            }

        }
        else{
            print("Collided with " + collision.gameObject.name);
            Destroy(gameObject);
        }
    }
}
