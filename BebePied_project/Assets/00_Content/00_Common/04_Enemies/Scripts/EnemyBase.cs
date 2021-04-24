using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyBase : MonoBehaviour
{
    [Header("Characteristics")] 
    public float MaxHealth;
    public float MaxSpeed;

    [Header("Death")]
    public GameObject DeathParticlePrefab;
    public AudioClip[] DeathSounds;

    [Header("Spawn")] 
    public AudioClip[] SpawnSounds;
    
    protected Rigidbody Body;

    protected AudioSource AudioPlayer;
    protected NavMeshAgent NavigationAgent;


    private float CurrentHealth;
    private float HealthMultiplier = 1.0f;
    
    public GameObject Player;
    private bool AutoStartAI = true;
    private bool AutoDestroyOnDeath = true;
    private bool AIRunning;

    protected virtual void Start()
    {
        InitializeCharacteristics();
        
        NavigationAgent = GetComponent<NavMeshAgent>();
        Body = GetComponent<Rigidbody>();
        AudioPlayer = GetComponent<AudioSource>();

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

        if (CurrentHealth <= 0)
        {
            if (DeathParticlePrefab != null)
            {
                GameObject deathParticle = Instantiate(DeathParticlePrefab, DeathParticlePrefab.transform.position, DeathParticlePrefab.transform.rotation, null);
                ParticleSystem deathParticleSystem = deathParticle.GetComponent<ParticleSystem>();
                if (deathParticleSystem != null)
                {
                    deathParticleSystem.Play();
                }
            }

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

    protected void PlayRandomSoundInArray(AudioClip[] soundArray, Vector3 position)
    {
        if (soundArray.Length > 0)
        {
            AudioClip soundToPlay = soundArray[Random.Range(0, soundArray.Length)];
            AudioSource.PlayClipAtPoint(soundToPlay, position);   
        }
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
    {} 
    
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
