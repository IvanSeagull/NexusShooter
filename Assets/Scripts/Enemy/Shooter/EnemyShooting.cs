using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public Transform player;  // Location of player using transform properties
    public float shootingRange = 10f;  // Range within which the enemy can shoot
    public float fireRate = 1f; // Time between shots
    public GameObject bulletPrefab; // Bullet prefab to shoot
    public Transform firePoint; // Position where the bullet will be spawned
    public float bulletSpeed = 20f; // Speed of the bullet
    private float nextFireTime = 0f;  // Time to fire next bullet

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position); // Calculate the distance between the enemy and the player

        if (distanceToPlayer < shootingRange && Time.time >= nextFireTime) // Check if the player is within shooting range and it's time to fire
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate; // Set next time to fire
        }

        FaceTarget(); // Always face the player when in range
    }
    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation); // Spawn a bullet at the fire point
        Rigidbody rb = bullet.GetComponent<Rigidbody>(); // Get the Rigidbody component of the bullet
        rb.velocity = firePoint.forward * bulletSpeed; // Shoot the bullet in the forward direction of the fire point
    }
    void FaceTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized; // Calculate the direction to look at the player
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // Calculate the rotation to look at the player
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Rotate towards the player
    }
}
