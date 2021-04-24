using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWeaponMinigun : PlayerWeapon
{

	public GameObject m_barrelGun;
	private float m_currentSpeed = 0;


	protected override void InternalUpdate()
	{
		if (Shooting)
		{
			m_currentSpeed += 12f;
			if (m_currentSpeed >= 480f)
			{
				m_currentSpeed = 480f;
			}
		}
		else
		{
			m_currentSpeed -= 12f;
			if (m_currentSpeed <= 0f)
			{
				m_currentSpeed = 0f;
			}
		}
		
		m_barrelGun.transform.Rotate(Vector3.forward, m_currentSpeed * Time.deltaTime);
	}

	protected override void StartShooting()
	{
		// throw new System.NotImplementedException();
	}

	protected override void StopShooting()
	{
		// throw new System.NotImplementedException();
	}
}
