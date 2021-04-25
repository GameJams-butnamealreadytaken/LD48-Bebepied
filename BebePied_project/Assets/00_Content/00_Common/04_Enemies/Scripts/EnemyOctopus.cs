using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOctopus : EnemyBase
{
    private float VerticalMaxSpeed = 0.02f;
    private float CurrentHeightObjective;

    protected override void Start()
    {
        base.Start();

        // Get distance between fly navmesh and current spawn location to avoid navmesh agent instantly sticking to navmesh height
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100.0f, ~LayerMask.NameToLayer(ObjectTags.NavmeshFly)))
        {
            NavigationAgent.baseOffset = hit.distance;
        }

        CurrentHeightObjective = Random.Range(-1.0f, 3.0f);
    }

    protected override void Update()
    {
        base.Update();

        NavigationAgent.baseOffset -= VerticalMaxSpeed;
        if (NavigationAgent.baseOffset < -1.0f)
        {
            // Avoid going too low and colliding with walking enemies
            NavigationAgent.baseOffset = -1.0f;
        }
    }

    protected override void OnUpdateAI()
    {
        SetDestination(Player.transform.position);
    }

    protected override void OnStartAI()
    {
        EnemyCounter.UpdateSpawnStats(EnemyType, false);
    }

    protected override void OnStopAI()
    {
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
