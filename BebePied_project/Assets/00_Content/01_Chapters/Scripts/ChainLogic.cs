using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLogic : MonoBehaviour
{
    public GameObject ExplosionPrefab;
    public AudioClip ExplosionSound;

    private bool AlreadyCollided = false;

    public void OnCollisionEnter(Collision collision)
    {
        transform.parent.GetComponent<LevelLogic>().TriggerEndChainCollision();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (AlreadyCollided)
        {
            return;
        }

        AlreadyCollided = true;
        
        transform.parent.GetComponent<LevelLogic>().TriggerEndChainCollision();
    }

    public void DestroyEffect()
    {
        GameObject instantiatedObject = Instantiate(ExplosionPrefab, transform.position, transform.rotation, null);
        ParticleSystem instantiatedParticleSystem = instantiatedObject.GetComponentInChildren<ParticleSystem>();
        if (instantiatedParticleSystem != null)
        {
            instantiatedParticleSystem.Play();
        }

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = ExplosionSound;
        audioSource.Play();
    }
}
