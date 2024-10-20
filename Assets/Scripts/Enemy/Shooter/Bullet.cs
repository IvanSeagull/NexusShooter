using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;
    private bool hasCollided = false; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return; // If the bullet has already collided, return

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null) 
            {
                playerController.GetComponent<HealthController>().TakeDamage(damage);
                Debug.Log("Health is " + playerController.GetComponent<HealthController>().currentHealth);
            }
            hasCollided = true; // Set the flag to true to prevent further collisions
            Destroy(gameObject);
        }
    }
}
