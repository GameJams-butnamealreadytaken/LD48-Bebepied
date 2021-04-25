using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLostSoul : EnemyBase
{
    protected override void OnStartAI()
    {
        EnemyCounter.UpdateSpawnStats(EnemyType, false);
    }

    protected override void OnStopAI()
    {
    }

    protected override void OnUpdateAI()
    {
        SetDestination(Player.transform.position);
    }

    protected override void OnDamageTaken(float oldHealth, float newHealth)
    {
        
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        EnemyCounter.UpdateSpawnStats(EnemyType, true);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.collider.CompareTag(ObjectTags.Player))
        {
            Player playerComponent = collision.collider.GetComponent<Player>();
            playerComponent.Kill();
        }
    }
}
