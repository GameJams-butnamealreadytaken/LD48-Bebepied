using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

	private int m_damages;	//< The damages that this projectile deals
	private float m_livingTime;	//< The time this projectile has been living

	/// <summary>
	/// Return the damages that this projectiles inflicts
	/// </summary>
	/// <returns>The damages points that this projectile inflicts</returns>
	public int Damages
	{
		get => m_damages;
		set => m_damages = value;
	}

	private void Update()
	{
		//
		// Increase living time
		m_livingTime += Time.deltaTime;
		
		//
		// If the living time is superior to 10 seconds, we delete it
		if (m_livingTime >= 10f)
		{
			Destroy(gameObject);
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		//
		// Destroy the projectile
		Destroy(gameObject);
	}
}
