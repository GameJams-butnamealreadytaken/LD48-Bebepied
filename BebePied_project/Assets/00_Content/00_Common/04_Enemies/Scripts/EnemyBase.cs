using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyBase : MonoBehaviour
{
    [Serializable]
    public class SoundEffect
    {
        public AudioClip Clip;
        public float Volume = 1.0f;
        public float AttenuationMinDistance = 1.0f;
        public float AttenuationMaxDistance = 500.0f;
        public AudioRolloffMode AttenuationRollofMode = AudioRolloffMode.Logarithmic;
    }
    
    [Header("Characteristics")] 
    public float MaxHealth;
    public float MaxSpeed;

    [Header("Hit")] 
    public GameObject HitParticlePrefab;
    public Vector3 HitParticleOffset = Vector3.zero;
    public List<SoundEffect> HitSounds;
    
    [Header("Death")]
    public GameObject DeathParticlePrefab;
    public Vector3 DeathParticleOffset = Vector3.zero;
    public List<SoundEffect> DeathSounds = new List<SoundEffect>();

    [Header("Spawn")] 
    public List<SoundEffect> SpawnSounds = new List<SoundEffect>();
    
    protected Rigidbody Body;

    protected AudioSource AudioPlayer;
    protected NavMeshAgent NavigationAgent;

    private float CurrentHealth;
    private float HealthMultiplier = 1.0f;
    
    public GameObject Player;

    public EnemyCounter EnemyCounter;
    public EEnemyType EnemyType;

    private bool AutoStartAI = true;
    private bool AutoDestroyOnDeath = true;
    private bool AIRunning;

    protected Animator Animator;

    protected virtual void Start()
    {
        InitializeCharacteristics();
        
        NavigationAgent = GetComponent<NavMeshAgent>();
        Body = GetComponent<Rigidbody>();
        AudioPlayer = GetComponent<AudioSource>();
        Animator = GetComponent<Animator>();

        if (NavigationAgent)
        {
            NavigationAgent.acceleration = MaxSpeed;
        }

        if (AutoStartAI)
        {
            StartAI();
        }
        
        PlayRandomSoundInArray(SpawnSounds, transform.position);
    }

    protected virtual void Update()
    {
        if (AIRunning)
        {
            OnUpdateAI();
        }
    }

    public void StartAI()
    {
        AIRunning = true;
        OnStartAI();
    }

    public void StopAI()
    {
        AIRunning = false;
        OnStopAI();
    }

    public void SetAutoStartAI(bool autoStart)
    {
        AutoStartAI = autoStart;
    }

    public void SetAutoDestroyOnDeath(bool autoDestroy)
    {
        AutoDestroyOnDeath = autoDestroy;
    }

    public void SetPlayer(GameObject player)
    {
        Player = player;
    }
    
    public void SetHealthMultiplier(float multiplier)
    {
        HealthMultiplier = multiplier;
    }

    public void TakeDamage(float damage)
    {
        if (CurrentHealth <= 0)
        {
            // Already dead
            return;
        }

        float oldHealth = CurrentHealth;
        CurrentHealth -= damage;
        
        OnDamageTaken(oldHealth, CurrentHealth);
        
        PlayParticleSystem(HitParticlePrefab, HitParticleOffset);
        PlayRandomSoundInArray(HitSounds, transform.position);

        if (CurrentHealth <= 0)
        {
            PlayParticleSystem(DeathParticlePrefab, DeathParticleOffset);

            PlayRandomSoundInArray(DeathSounds, transform.position);
            
            OnDeath();

            if (AutoDestroyOnDeath)
            {
                Destroy(gameObject);
            }
        }
    }

    protected Vector3 GetDestination()
    {
        if (NavigationAgent)
        {
            return NavigationAgent.destination;
        }

        return Vector3.zero;
    }

    protected void SetDestination(Vector3 newDestination)
    {
        if (NavigationAgent)
        {
            NavigationAgent.isStopped = false;
            NavigationAgent.destination = newDestination;   
        }
    }

    protected void StopMovement()
    {
        if (NavigationAgent)
        {
            NavigationAgent.isStopped = true;
            NavigationAgent.destination = transform.position;
        }
    }

    protected void PlayParticleSystem(GameObject particleSystemPrefab, Vector3 offset)
    {
        if (particleSystemPrefab != null)
        {
            GameObject instantiatedObject = Instantiate(particleSystemPrefab, transform.position + offset, transform.rotation, null);
            ParticleSystem instantiatedParticleSystem = instantiatedObject.GetComponentInChildren<ParticleSystem>();
            if (instantiatedParticleSystem != null)
            {
                instantiatedParticleSystem.Play();
            }
        }
    }

    protected void PlayRandomSoundInArray(List<SoundEffect> soundArray, Vector3 position, float volume = 1.0f)
    {
        if (soundArray.Count > 0)
        {
            SoundEffect soundEffect = soundArray[Random.Range(0, soundArray.Count)];
            
            GameObject gameObjectToCreate = new GameObject("One shot audio");
            gameObjectToCreate.transform.position = position;
            AudioSource audioSource = (AudioSource) gameObjectToCreate.AddComponent(typeof (AudioSource));
            audioSource.clip = soundEffect.Clip;
            audioSource.spatialBlend = 1f;
            audioSource.minDistance = soundEffect.AttenuationMinDistance;
            audioSource.maxDistance = soundEffect.AttenuationMaxDistance;
            audioSource.rolloffMode = soundEffect.AttenuationRollofMode;
            audioSource.volume = soundEffect.Volume;
            audioSource.Play();
            Destroy(gameObjectToCreate, soundEffect.Clip.length * (Time.timeScale < 0.00999999977648258 ? 0.01f : Time.timeScale));
        }
    }

    protected bool RotateTowardPlayer(float speed, float angleTolerance)
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = Player.transform.position - transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = speed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);

        if (Vector3.Angle(transform.forward, targetDirection.normalized) <= angleTolerance)
        {
            return true;
        }

        return false;
    }

    protected void ShotTowardPlayer(ProjectileData projectileData, Vector3 startPosition)
    {
        //
        // Instantiate the projectile
        GameObject projectileGO = GameObject.Instantiate(projectileData.Prefab, startPosition, Quaternion.identity);
			
        //
        // Set the forward of the projectile
        Vector3 bulletDirection = (Player.transform.position - startPosition).normalized;
        projectileGO.transform.forward = -bulletDirection;
	    if (EnemyType == EEnemyType.SpawnedWalkDistance)
        {
            bulletDirection.y = 0.0f;
        }

        //
        // Add the projectile component and set its damages
        Projectile projectileComponent = projectileGO.AddComponent<Projectile>();
        projectileComponent.Damages = projectileData.Damages;
        projectileComponent.LifeTime = projectileData.LifeTime;

        //
        // Ensure the bullet rigidbody has its rotation constrained so the bullets do not rotate after being fired
        // Only if it is not deactivated in the projectile
        if (projectileData.ConstrainProjectilesRotation)
        {
            projectileGO.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }

        //
        // Deactivate the physics on the projectile if it is asked
        if (projectileData.DeactivatePhysics)
        {
            projectileGO.GetComponent<Rigidbody>().useGravity = false;
        }
			
        //
        // Set the collider to trigger
        projectileGO.GetComponent<Collider>().isTrigger = true;
				
			
        //
        // Eject the projectile
        projectileGO.GetComponent<Rigidbody>().AddForce(bulletDirection * projectileData.FireVelocity, ForceMode.Impulse);
    }
    
    protected virtual void OnStartAI()
    {}
    
    protected virtual void OnStopAI()
    {}
    
    protected virtual void OnUpdateAI()
    {}

    protected virtual void OnDamageTaken(float oldHealth, float newHealth)
    {}

    protected virtual void OnDeath()
    {
        //
        // Increment the enemy killed stat
        GameManager.GetInstance().Player.IncrementEnemyKilledStat();
    } 
    
    protected virtual void OnCollisionEnter(Collision collision)
    {

    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ObjectTags.Bullet))
        {
            Projectile projectile = other.GetComponent<Projectile>();
            TakeDamage(projectile.Damages);
        }
    }

    private void InitializeCharacteristics()
    {
        CurrentHealth = MaxHealth * HealthMultiplier;
    }
}
