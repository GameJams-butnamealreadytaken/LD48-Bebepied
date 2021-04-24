using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyBase : MonoBehaviour
{
    [Header("Characteristics")] 
    public float MaxHealth;
    public float MaxSpeed;
    
    private float CurrentHealth;
    
    void Start()
    {
        InitializeCharacteristics();
    }

    void Update()
    {
        
    }

    private void InitializeCharacteristics()
    {
        CurrentHealth = MaxHealth;
    }
}
