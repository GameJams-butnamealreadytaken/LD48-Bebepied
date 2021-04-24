using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the weapons of the player
/// </summary>
public class PlayerWeaponHolder : MonoBehaviour
{
	[SerializeField] 
	[Tooltip("The start weapon of the player")]
	private PlayerWeapon m_startWeapon;
	
	private PlayerWeapon m_currentWeapon;

	private void Start()
	{
		m_currentWeapon = m_startWeapon;
	}

	public void Shoot(ProjectileData projectileData)
	{
		if (null != m_currentWeapon)
		{
			m_currentWeapon.Shoot(projectileData);
		}
	}
}
