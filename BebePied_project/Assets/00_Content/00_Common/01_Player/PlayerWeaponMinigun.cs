using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWeaponMinigun : PlayerWeapon
{

	public List<AudioClip> m_shotAudioClips = new List<AudioClip>();
	public GameObject m_barrelGun;
	private float m_currentSpeed = 0;


	protected override void InternalUpdate()
	{
		//
		// Update the speed of the minigum
		if (Shooting)
		{
			m_currentSpeed += 12f;
			if (m_currentSpeed >= 480f)
			{
				m_canShoot = true;	//< THe minigun now can shoot
				m_currentSpeed = 480f;
			}
		}
		else
		{
			m_canShoot = false;	//< We can't shoot until the speed is at maximum
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
	float LastEventTime = 0;
	protected override void OnShoot()
	{
		AudioSource.PlayClipAtPoint(m_shotAudioClips[Random.Range(0, m_shotAudioClips.Count)], transform.position, 0.5f);
		
		//
		// Beurk, shake the camera by accessing the player, can be done in a much more beautiful way
		GetComponentInParent<Player>().ShakeCamera(1f, 0.3f);
	}
}
