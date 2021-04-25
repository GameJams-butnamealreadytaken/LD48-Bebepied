using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGhast : EnemyBase
{
    [Header("AI")]
    public float MinDistanceToPlayer;
    public ProjectileData MunitionType;
    public float TimeBetweenShots;
    public GameObject ShotPoint;

    private float VerticalMaxSpeed = 0.02f;

    private Vector3 CurrentDestination;
    private float TimeSinceLastShot = 100000;

    protected override void Start()
    {
        base.Start();

        // Get distance between fly navmesh and current spawn location to avoid navmesh agent instantly sticking to navmesh height
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100.0f, ~LayerMask.NameToLayer(ObjectTags.NavmeshFly)))
        {
            NavigationAgent.baseOffset = hit.distance;
        }
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
            if (RotateTowardPlayer(1.0f, 10.0f))
            {
                TimeSinceLastShot = 0.0f;
                Animator.SetTrigger("Attack");   
            }
        }
    }
    public void OnShootFromAnimation()
    {
        ShotTowardPlayer(MunitionType, ShotPoint.transform.position);
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
