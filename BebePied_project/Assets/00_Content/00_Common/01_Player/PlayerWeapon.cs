using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Manages the weapon of the player that shoots the given projectiles
/// </summary>
public class PlayerWeapon : MonoBehaviour
{
	[SerializeField]
	private Vector3 m_weaponFrontVector = Vector3.forward;

	[SerializeField] 
	private Transform m_cannonPosition;

	private float m_timeSinceLastShoot = 0f;

	private void Update()
	{
		m_timeSinceLastShoot += Time.deltaTime;
	}

	public void Shoot(ProjectileData projectileData)
	{
		if (m_timeSinceLastShoot >= 0.05f)
		{
			GameObject projectileGO = GameObject.Instantiate(projectileData.Prefab, 
				m_cannonPosition.position + new Vector3(Random.Range(-0.1f, 0.1f),Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)), 
				Quaternion.identity);
			projectileGO.GetComponent<Rigidbody>().AddForce(m_cannonPosition.forward * projectileData.FireVelocity, ForceMode.Impulse);
			Projectile projectileComponent = projectileGO.AddComponent<Projectile>();
			projectileComponent.Damages = projectileData.Damages;

			m_timeSinceLastShoot = 0f;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(m_cannonPosition.position, 0.05f);
		Gizmos.DrawLine(m_cannonPosition.position, m_cannonPosition.position + (m_weaponFrontVector * 2f) );
	}
}
