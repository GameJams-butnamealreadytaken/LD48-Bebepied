using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

	private int m_damages;	//< The damages that this projectile deals
	
	private float m_lifeTime;	//< The time this projectile will leave
	private float m_currentLivingTime;	//< The time this projectile has been living

	/// <summary>
	/// Return the damages that this projectiles inflicts
	/// </summary>
	/// <returns>The damages points that this projectile inflicts</returns>
	public int Damages
	{
		get => m_damages;
		set => m_damages = value;
	}
	
	/// <summary>
	/// Return the damages that this projectiles inflicts
	/// </summary>
	/// <returns>The damages points that this projectile inflicts</returns>
	public float LifeTime
	{
		get => m_lifeTime;
		set => m_lifeTime = value;
	}

	private void Update()
	{
		//
		// Increase living time
		m_currentLivingTime += Time.deltaTime;
		
		//
		// If the living time is superior to 5 seconds, we delete it
		if (m_currentLivingTime >= m_lifeTime)
		{
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		//
		// Destroy the projectile only if it is not colliding with another projectile
		if (!other.transform.CompareTag(ObjectTags.Bullet))
		{
			Destroy(gameObject);
		}
	}
}
