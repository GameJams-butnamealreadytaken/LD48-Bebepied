using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOctopus : EnemyBase
{
    private float VerticalMaxSpeed = 0.02f;
    private float VerticalMaxSpeedDescentToPlayer = 1f;
    private float CurrentHeightObjective;

    private bool InitialDescent = true;
    private bool ShouldDescentToPlayer = false;
    private float DistanceBeforeDescentToPlayer = 15.0f;

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
        
        if (InitialDescent)
        {
            NavigationAgent.baseOffset -= VerticalMaxSpeed;
            if (NavigationAgent.baseOffset < -1.0f)
            {
                // Avoid going too low and colliding with walking enemies
                NavigationAgent.baseOffset = -1.0f;
                InitialDescent = false;
            }

            return;
        }

        if (ShouldDescentToPlayer)
        {
            NavigationAgent.baseOffset -= VerticalMaxSpeedDescentToPlayer * Time.deltaTime;
            if (transform.position.y <= Player.transform.position.y)
            {
                NavigationAgent.baseOffset += VerticalMaxSpeedDescentToPlayer * Time.deltaTime;    
            }
        }
        else
        {
            NavigationAgent.baseOffset += VerticalMaxSpeedDescentToPlayer * Time.deltaTime;
            if (NavigationAgent.baseOffset >= -1.0f)
            {
                NavigationAgent.baseOffset = -1.0f;
            }
        }
    }

    protected override void OnUpdateAI()
    {
        SetDestination(Player.transform.position);

        Vector2 this2DPosition = new Vector2(transform.position.x, transform.position.z);
        Vector2 player2DPosition = new Vector2(Player.transform.position.x, Player.transform.position.z);
        ShouldDescentToPlayer = Vector2.Distance(this2DPosition, player2DPosition) <= DistanceBeforeDescentToPlayer;
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
