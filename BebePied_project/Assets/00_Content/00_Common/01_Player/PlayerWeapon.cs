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

	private float m_timeSinceLastShoot = 0f;
	private float m_timeToNextShoot = 0f;
	private bool m_isShooting = false;

	public bool Shooting => m_isShooting;

	private void Update()
	{
		//
		//
		InternalUpdate();
		
		//
		//
		m_timeSinceLastShoot += Time.deltaTime;
		
		//
		// Reset the shooting boolean if we didn't shoot for too long
		if (m_timeSinceLastShoot > m_timeToNextShoot)
		{
			m_isShooting = false;
			StopShooting();
		}
	}

	public void Shoot(ProjectileData projectileData)
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
				projectileComponent.Damages = projectileData.Damages;
				
				//
				// Ensure the bullet rigidbody has its rotation constrained so the bullets do not rotate after being fired
				projectileGO.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
				
				//
				// Set the collider to trigger
				projectileGO.GetComponent<Collider>().isTrigger = true;
				
				//
				// Eject the projectile
				projectileGO.GetComponent<Rigidbody>().AddForce(-bulletDirection * projectileData.FireVelocity, ForceMode.Impulse);
			}
		}
	}

	protected abstract void InternalUpdate();
	
	protected abstract void StartShooting();
	
	protected abstract void StopShooting();

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(m_cannonPosition.position, 0.05f);
		Gizmos.DrawLine(m_cannonPosition.position, m_cannonPosition.position + (-m_cannonPosition.forward * 1.2f) );
	}
}
