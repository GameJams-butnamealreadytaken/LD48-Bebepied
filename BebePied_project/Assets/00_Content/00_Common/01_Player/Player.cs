using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DissidentStudio.Toolkit.FPSController;
using UnityEngine;
using UnityEngine.InputSystem;
using CharacterController = DissidentStudio.Toolkit.FPSController.CharacterController;

public class Player : MonoBehaviour
{

	[SerializeField] 
	private UIScoreScreen m_scoreScreenUI;
	
	[SerializeField]
	[Tooltip("The projectile that the player has at the beginning of the game")]
	public ProjectileData m_defaultProjectile;
	

	private bool m_isActive = false;	//< Is the player active (can shoot mainly)
	private int m_ennemiesKilled;	//< The number of ennemies killed
	private int m_bulletsShot;	//< The number of bullets shot
	private int m_sausagesShot;	//< The number of sausages shot
	private ProjectileData m_currentProjectileData;
	private BonusData m_currentBonusData = null;	//< The current bonus
	private int m_additionalDamages = 0;
	private float m_currentBonusDuration = 0f;
	
	// Start is called before the first frame update
	void Start()
	{
		//
		// Lock the cursor 
		Cursor.lockState = CursorLockMode.Locked;
		
		//
		// Activate the player
		Activate();
		
		//
		// Set default projectile as current
		m_currentProjectileData = m_defaultProjectile;
	}

	// Update is called once per frame
	void Update()
	{
		//
		// If the player is not active we do nothing
		if (!m_isActive)
		{
			return;
		}
		
		//
		// Update duration and remove bonus if duration has finished
		if (m_currentBonusData)
		{
			m_currentBonusDuration += Time.deltaTime;
			if (m_currentBonusDuration >= m_currentBonusData.Duration)
			{
				RemoveBonus();
			}
		}

		//
		// Test if the player is shooting
		if (GetComponent<PlayerInput>().actions["Shoot"].ReadValue<float>() >= 0.5f)
		{
			//
			// Shoot with the weapon
			GetComponentInChildren<PlayerWeaponHolder>().Shoot(m_currentProjectileData, m_additionalDamages);
			
			//
			// Update the stats
			m_bulletsShot += m_currentProjectileData.FireProjectilesCount;
			if (m_currentProjectileData.IsSausage)
			{
				m_sausagesShot += m_currentProjectileData.FireProjectilesCount;
			}
		}
	}

	public void Activate()
	{
		m_isActive = true;
		GetComponent<CharacterController>().Active = true;
		GetComponentInChildren<CameraController>().Active = true;
	}

	public void Deactivate()
	{
		m_isActive = false;
		GetComponent<CharacterController>().Active = false;
		GetComponentInChildren<CameraController>().Active = false;
	}
	
	public void ResetStats()
	{
		m_ennemiesKilled = 0;
		m_bulletsShot = 0;
		m_sausagesShot = 0;
	}

	public void IncrementEnemyKilledStat()
	{
		m_ennemiesKilled++;
	}

	/// <summary>
	/// KIll the player
	/// </summary>
	public void Kill()
	{
		//
		// Show the UI
		m_scoreScreenUI.Show(m_ennemiesKilled, m_bulletsShot, m_sausagesShot);
		
		//
		// Deactivate the player
		Deactivate();
	}

	/// <summary>
	/// Make the camera shake
	/// </summary>
	/// <param name="intensity">The intensity of the shake, 1 is a pretty high value for example</param>
	/// <param name="duration">The duration of the shake</param>
	public void ShakeCamera(float intensity, float duration)
	{
		StopAllCoroutines();
		StartCoroutine(CameraShake(intensity, duration));
	}
	
	public IEnumerator CameraShake(float intensity,float duration)
	{
		//
		// TODO: I'm actually thinking about the mistakes in my life when seeing this code, please, do something...
		//
		// Apply the amplitude gain to the noise component during the coroutine
		while (duration > 0)
		{
			GetComponentInChildren<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = intensity;
			duration -= Time.deltaTime;
			yield return null;
		}
 
		//
		// Reapply the start intensity
		GetComponentInChildren<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
	}

	public void AddBonus(BonusData data)
	{
		//
		// Start by removing the effect of the current bonus
		RemoveBonus();
		
		//
		// Add the effect of the new bonus
		m_currentBonusData = data;
		switch (m_currentBonusData.Type)
		{
			case EBonusType.Damage:
			{
				m_additionalDamages = m_currentBonusData.Damages;
			}break;
			case EBonusType.Speed:
			{
				GetComponent<CharacterController>().OverrideSpeed(GetComponent<CharacterController>().WalkSpeed + m_currentBonusData.Speed);
			}break;
			case EBonusType.Armor:
				break;
			case EBonusType.Projectile:
			{
				m_currentProjectileData = m_currentBonusData.Projectile;
			}break;
			case EBonusType.Max:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		
		//
		//
		m_currentBonusDuration = 0f;
	}

	public void RemoveBonus()
	{
		//
		// We do nothing if we don't have an active bonus
		if (!m_currentBonusData)
		{
			return;
		}
		
		switch (m_currentBonusData.Type)
		{
			case EBonusType.Damage:
			{
				m_additionalDamages = 0;
			}break;
			case EBonusType.Speed:
			{
				GetComponent<CharacterController>().ResetOverrideSpeed();
			}break;
			case EBonusType.Armor:
			{
				// TODO:
			}break;
			case EBonusType.Projectile:
			{
				m_currentProjectileData = m_defaultProjectile;
			}break;
			case EBonusType.Max:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}
