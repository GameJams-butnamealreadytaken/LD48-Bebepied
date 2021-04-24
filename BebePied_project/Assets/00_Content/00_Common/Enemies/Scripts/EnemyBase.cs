using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Characteristics")] 
    public float MaxHealth;
    public float MaxSpeed;

    [Header("Physics")]
    public Rigidbody Body;
    
    private float CurrentHealth;
    
    protected virtual void Start()
    {
        InitializeCharacteristics();
    }

    protected virtual void Update()
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

    protected virtual void OnDamageTaken(float oldHealth, float newHealth)
    {}
    
    protected virtual void OnDeath()
    {} 
    
    void OnCollisionEnter(Collision collision)
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
