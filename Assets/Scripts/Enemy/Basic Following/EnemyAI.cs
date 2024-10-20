using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float detectionRange = 10f; 
    public float stoppingDistance = 1f; // The distance at which the enemy will stop moving towards the player doesn't seem to work in navmesh

    private NavMeshAgent agent; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Get the NavMesh Agent component
        agent.stoppingDistance = stoppingDistance; // Set the stopping distance
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position); // Calculate the distance between the enemy and the player

        if (distanceToPlayer < detectionRange) // Check if the player is within the detection range
        {
            agent.SetDestination(player.position); // Move towards the player
        }
        else
        {
            agent.ResetPath(); // Stop moving if the player is out of range
        }
    }
}