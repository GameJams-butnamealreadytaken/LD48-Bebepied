using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGunners : EnemyBase
{
    public float MinDistanceToPlayer;
    
    private Vector3 CurrentDestination;
    
    protected override void OnStartAI()
    {
        CurrentDestination = Vector3.zero;
    }

    protected override void OnStopAI()
    {
        
    }

    protected override void OnUpdateAI()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.transform.position);
        float destinationDistanceToPlayer = Vector3.Distance(GetDestination(), Player.transform.position);

        if (CurrentDestination == Vector3.zero || (distanceToPlayer < MinDistanceToPlayer && destinationDistanceToPlayer < MinDistanceToPlayer))
        {
            CurrentDestination = (transform.position - Player.transform.position).normalized * (MinDistanceToPlayer - distanceToPlayer + 1);
            CurrentDestination = Quaternion.Euler(0, Random.Range(-50, 50), 0) * CurrentDestination;
            CurrentDestination += transform.position;
            
            SetDestination(CurrentDestination);
        }
    }

    protected override void OnDamageTaken(float oldHealth, float newHealth)
    {
        
    }

    protected override void OnDeath()
    {
        Destroy(gameObject);
    }
}
