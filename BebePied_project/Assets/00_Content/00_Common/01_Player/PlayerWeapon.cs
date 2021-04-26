using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Manages the weapon of the player that shoots the given projectiles
/// </summary>
public abstract class PlayerWeapon : MonoBehaviour
{
	[SerializeField] 
	private Transform m_cannonPosition;

	private float m_timeSinceLastShoot = 0f;	//< The time since the last shoot 
	private float m_timeToNextShoot = 0f;	//< THe time to wait before next shoot
	private bool m_isShooting = false;	//< Indicates if the weapon is shooting

	protected bool m_canShoot = false;	//< A boolean that is set in the children to allow the weapon to shoot

	public bool Shooting => m_isShooting;

	private void Update()
	{
		//
		// Update the children
		InternalUpdate();
		
		//
		// Update the time since last shott
		m_timeSinceLastShoot += Time.deltaTime;
		
		//
		// Reset the shooting boolean if we didn't shoot for too long
		if (m_timeSinceLastShoot > m_timeToNextShoot)
		{
			if (m_isShooting)
			{
				StopShooting();
			}
			m_isShooting = false;
		}
	}

	public void Shoot(ProjectileData projectileData, int additionalDamages)
	{
		//
		// Is it ok to shoot now ?
		if (m_timeSinceLastShoot >= m_timeToNextShoot)
		{
			//
			// We are shooting
			if (!m_isShooting)
			{
				StartShooting();
				m_isShooting = true;
			}

			//
			//
			if (m_canShoot)
			{
				//
				// Set the time to the next shoot
				m_timeToNextShoot = projectileData.TimeBetweenFires;
			
				//
				// Reset the time to the next shoot
				m_timeSinceLastShoot = 0f;
			
				//
				// Instantiate as many projectiles as needed
				for (int projectileIndex = 0; projectileIndex < projectileData.FireProjectilesCount; ++projectileIndex)
				{
					//
					// Randomize the start position of the bullet
					Vector3 bulletStartPosition = m_cannonPosition.forward * (Random.insideUnitCircle * 0.3f);
				
					//
					// Instantiate the projectile
					GameObject projectileGO = GameObject.Instantiate(projectileData.Prefab, 
						m_cannonPosition.position + bulletStartPosition, Quaternion.identity);
				
					//
					// Set the forward of the projectile
					Vector3 bulletDirection = m_cannonPosition.forward + (Random.insideUnitSphere * projectileData.DispersionFactor);
					projectileGO.transform.forward = bulletDirection;
				
					//
					// Add the projectile component and set its damages
					Projectile projectileComponent = projectileGO.AddComponent<Projectile>();
					projectileComponent.Damages = projectileData.Damages + additionalDamages;	//< Don't forget to add additional damages
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
					projectileGO.GetComponent<Rigidbody>().AddForce(-bulletDirection * projectileData.FireVelocity, ForceMode.Impulse);
				}
				
				//
				// Call the OnShoot method
				OnShoot(projectileData);
			}
		}
	}

	protected abstract void InternalUpdate();
	
	protected abstract void StartShooting();
	
	protected abstract void StopShooting();

	protected virtual void OnShoot(ProjectileData shotProjectile)
	{
		GetComponentInParent<Player>().ShootAudioSource.PlayOneShot(shotProjectile.Sounds[Random.Range(0, shotProjectile.Sounds.Count)]);
		GetComponentInParent<Player>().OnWeaponShoot();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(m_cannonPosition.position, 0.05f);
		Gizmos.DrawLine(m_cannonPosition.position, m_cannonPosition.position + (-m_cannonPosition.forward * 1.2f) );
	}
}
