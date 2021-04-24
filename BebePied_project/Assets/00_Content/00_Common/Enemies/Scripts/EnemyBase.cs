using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [Header("Characteristics")] 
    public float MaxHealth;
    public float MaxSpeed;

    [Header("Physics")]
    public Rigidbody Body;
    
    private float CurrentHealth;

    private NavMeshAgent NavigationAgent;
    
    void Start()
    {
        InitializeCharacteristics();

        NavigationAgent = GetComponent<NavMeshAgent>();
        NavigationAgent.acceleration = MaxSpeed;
    }

    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        if (CurrentHealth <= 0)
        {
            // Already dead
            return;
        }

        float oldHealth = CurrentHealth;
        CurrentHealth -= damage;
        
        OnDamageTaken(oldHealth, CurrentHealth);

        if (CurrentHealth <= 0)
        {
            OnDeath();
        }
    }

    protected void SetDestination(Vector3 newDestination)
    {
        NavigationAgent.isStopped = false;
        NavigationAgent.destination = newDestination;
    }

    protected void StopMovement()
    {
        NavigationAgent.isStopped = true;
        NavigationAgent.destination = transform.position;
    }

    protected virtual void OnDamageTaken(float oldHealth, float newHealth)
    {}
    
    protected virtual void OnDeath()
    {} 
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(ObjectTags.Bullet))
        {
            TakeDamage(5);
        }
    }

    private void InitializeCharacteristics()
    {
        CurrentHealth = MaxHealth;
    }
}
