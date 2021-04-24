using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGhast : EnemyBase
{

    protected override void Start()
    {
        base.Start();

        Debug.DrawLine(transform.position, transform.position + Vector3.down * 100.0f, Color.yellow, 2.0f);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100.0f, ~LayerMask.NameToLayer("GroundFly")))
        {
            NavigationAgent.baseOffset = hit.distance;
        }
    }

    protected override void Update()
    {
        base.Update();

        //NavigationAgent.baseOffset += Random.Range(-1.0f, 1.0f);
        //NavigationAgent.baseOffset = Mathf.Clamp(NavigationAgent.baseOffset, 0, 5);
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
