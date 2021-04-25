using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLostSoul : EnemyBase
{
    private bool Deflating = false;
    
    protected override void Update() 
    {
        base.Update();
        if (Deflating)
        {
            Vector3 Scale = transform.localScale;
            Scale.y -= 0.01f;
            if (Scale.y >= 0)
            {
                transform.localScale = Scale;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
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
        SetAutoDestroyOnDeath(false);
        Deflating = true;
        gameObject.GetComponentInChildren<BoxCollider>().enabled = false;
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
