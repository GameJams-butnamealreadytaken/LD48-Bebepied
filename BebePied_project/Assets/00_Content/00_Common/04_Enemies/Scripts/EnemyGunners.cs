using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGunners : EnemyBase
{
    [Header("AI")] 
    public ProjectileData MunitionType;
    public float MinDistanceToPlayer;
    public float TimeBetweenShots;
    public GameObject ShotPoint;
    
    private Vector3 CurrentDestination;
    private float TimeSinceLastShot = 100000;

    protected override void OnStartAI()
    {
        EnemyCounter.UpdateSpawnStats(EnemyType, false);
        CurrentDestination = Vector3.zero;
    }

    protected override void OnStopAI()
    {
    }

    protected override void OnUpdateAI()
    {
        TimeSinceLastShot += Time.deltaTime;
        
        float distanceToPlayer = Vector3.Distance(transform.position, Player.transform.position);
        float destinationDistanceToPlayer = Vector3.Distance(GetDestination(), Player.transform.position);

        if (CurrentDestination == Vector3.zero || (distanceToPlayer < MinDistanceToPlayer && destinationDistanceToPlayer < MinDistanceToPlayer))
        {
            CurrentDestination = (transform.position - Player.transform.position).normalized * (MinDistanceToPlayer - distanceToPlayer + 1);
            CurrentDestination = Quaternion.Euler(0, Random.Range(-50, 50), 0) * CurrentDestination;
            CurrentDestination += transform.position;
            
            SetDestination(CurrentDestination);
        }
        
        if (distanceToPlayer > MinDistanceToPlayer && TimeSinceLastShot >= TimeBetweenShots)
        {
            TimeSinceLastShot = 0.0f;
            Animator.SetTrigger("Attack");
        }
    }

    public void OnShootFromAnimation()
    {
        ShotTowardPlayer(MunitionType, ShotPoint.transform.position);
    }

    protected override void OnDamageTaken(float oldHealth, float newHealth)
    {
        
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        EnemyCounter.UpdateSpawnStats(EnemyType, true);
    }
}
