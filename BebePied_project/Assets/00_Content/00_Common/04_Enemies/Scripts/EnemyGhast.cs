using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGhast : EnemyBase
{

    private float VerticalMaxSpeed = 0.02f;

    protected override void Start()
    {
        base.Start();

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100.0f, ~LayerMask.NameToLayer("GroundFly")))
        {
            NavigationAgent.baseOffset = hit.distance;
        }
    }

    protected override void Update()
    {
        base.Update();

        NavigationAgent.baseOffset -= VerticalMaxSpeed;
        if (NavigationAgent.baseOffset < 0)
        {
            // Avoid going under the navmesh height and colliding with walking enemies
            NavigationAgent.baseOffset = 0;
        }
    }

    protected override void OnUpdateAI()
    {
        SetDestination(Player.transform.position);
    }

    protected override void OnStartAI()
    {

    }

    protected override void OnStopAI()
    {

    }

    protected override void OnDamageTaken(float oldHealth, float newHealth)
    {

    }

    protected override void OnDeath()
    {
        Destroy(gameObject);
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
