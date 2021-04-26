using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DissidentStudio.Toolkit.FPSController;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using CharacterController = DissidentStudio.Toolkit.FPSController.CharacterController;
using Random = System.Random;

public class Player : MonoBehaviour
{

	[SerializeField] 
	private UIScoreScreen m_scoreScreenUI;
	
	[SerializeField] 
	private UIInGamePause m_inGamePauseUI;
	
	[SerializeField]
	[Tooltip("The projectile that the player has at the beginning of the game")]
	public ProjectileData m_defaultProjectile;

	[SerializeField] [Tooltip("The audio source to emit the bonus pick up sound")]
	private AudioSource m_audioSourceBonusPickUp;
	
	[SerializeField] [Tooltip("The audio source to emit shoot sounds")]
	private AudioSource m_audioSourceShoot;
	
	[SerializeField] [Tooltip("The clip played when a bonus ends")]
	private AudioClip m_bonusEndClip;

	public AudioSource ShootAudioSource => m_audioSourceShoot;
	

	private bool m_isActive = false;	//< Is the player active (can shoot mainly)
	private int m_ennemiesKilled;	//< The number of ennemies killed
	private int m_bulletsShot;	//< The number of bullets shot
	public int m_bulletHit;	//< The number of bullet that hit an enemy
	private int m_sausagesShot;	//< The number of sausages shot
	private int m_currentWave = 1;
	private ProjectileData m_currentProjectileData;
	private BonusData m_currentBonusData = null;	//< The current bonus
	private float m_currentBonusDuration = 0f;	//< The duration of the current active bonus (random)
	private int m_additionalDamages = 0;
	private float m_currentBonusElapsedTime = 0f;
	private int m_killCountForBonus = 0;
	private int m_killCountToNextBonus = 0;
	
	
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
		
		//
		//
		m_killCountToNextBonus = UnityEngine.Random.Range(20, 30);
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
		//
		if (GetComponent<PlayerInput>().actions["Pause"].triggered)
		{
			m_inGamePauseUI.Show();
		}
		
		//
		// Update duration and remove bonus if duration has finished
		if (m_currentBonusData)
		{
			m_currentBonusElapsedTime += Time.deltaTime;
			if (m_currentBonusElapsedTime >= m_currentBonusDuration)
			{
				RemoveBonus();
				m_currentBonusData = null;
			}
		}

		//
		// Test if the player is shooting
		if (GetComponent<PlayerInput>().actions["Shoot"].ReadValue<float>() >= 0.5f)
		{
			//
			// Shoot with the weapon
			GetComponentInChildren<PlayerWeaponHolder>().Shoot(m_currentProjectileData, m_additionalDamages);
		}
	}

	public void OnWeaponShoot()
    {
		//
		// Update the stats if not in tuto
		if (m_currentWave > 1)
		{
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
		m_bulletHit = 0;
		m_sausagesShot = 0;
	}

	public void IncrementEnemyKilledStat(Transform enemyPosition)
	{
		m_killCountForBonus++;
		if (m_killCountForBonus >= m_killCountToNextBonus)
		{
			RaycastHit hit;
			if (Physics.Raycast(enemyPosition.position + Vector3.up * 5.0f, Vector3.down, out hit, 1000.0f, LayerMask.GetMask(ObjectTags.Ground)))
			{
				Vector3 bonusPosition = new Vector3(enemyPosition.position.x, hit.point.y + 0.5f, enemyPosition.position.z);

				// Spawn Bonus
				int bonusID = UnityEngine.Random.Range(0, GameManager.GetInstance().m_bonusManager.m_ennemyBonusData.Count);
				GameObject  go = Instantiate(GameManager.GetInstance().m_bonusManager.m_ennemyBonusData[bonusID].Prefab, bonusPosition, Quaternion.identity);
				go.transform.position = bonusPosition;
				BonusBehavior bonusBehaviour = go.AddComponent<BonusBehavior>();
				bonusBehaviour.m_bonusData = GameManager.GetInstance().m_bonusManager.m_ennemyBonusData[bonusID];

				m_killCountForBonus = 0;
				m_killCountToNextBonus = UnityEngine.Random.Range(20, 30);
			}
		}
		m_ennemiesKilled++;
	}

	/// <summary>
	/// KIll the player
	/// </summary>
	public void Kill()
	{
		if (m_currentBonusData)
		{
			if (m_currentBonusData.Type == EBonusType.Armor)
			{
				return;
			}
		}
		else if (GameManager.GetInstance().CurrentLevel.IsTuto)
		{
			return;
		}
		
		//
		// Remove the current bonus
		RemoveBonus();
		
		//
		// Show the UI
		m_scoreScreenUI.Show(m_currentWave - 1, m_ennemiesKilled, m_bulletsShot, m_bulletHit, m_sausagesShot);
		
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
		RemoveBonus(true);

		//
		// Add the effect of the new bonus
		m_currentBonusData = data;
		switch (m_currentBonusData.Type)
		{
			case EBonusType.Damage:
			{
				m_additionalDamages = m_currentBonusData.Damages;
				m_currentProjectileData = m_currentBonusData.Projectile;	//< Damage also overrides the projectile
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
		// Play the sound of the bonus pickup
		if (!m_audioSourceBonusPickUp.isPlaying)
		{
			m_audioSourceBonusPickUp.PlayOneShot(data.Clip, 1f);
		}

		//
		// Reset the duration of the current bonus
		m_currentBonusElapsedTime = 0f;
		m_currentBonusDuration = UnityEngine.Random.Range(m_currentBonusData.Duration, m_currentBonusData.Duration * 1.75f);
		
		//
		// Set bonus text in ui
		GetComponentInChildren<UIInGame>().StartBonus(data.Text, (int)m_currentBonusDuration);
	}

	public void RemoveBonus(bool withNextBonus = false)
	{
		//
		// We do nothing if we don't have an active bonus
		if (!m_currentBonusData)
		{
			return;
		}
		
		//
		// If there is no next bonus, we play the sound
		if (!withNextBonus)
		{
			if (!m_audioSourceBonusPickUp.isPlaying)
			{
				m_audioSourceBonusPickUp.PlayOneShot(m_bonusEndClip, 0.3f);
			}
		}

		//
		// Reset bonus text in ui
		GetComponentInChildren<UIInGame>().ResetBonus();
		
		switch (m_currentBonusData.Type)
		{
			case EBonusType.Damage:
			{
				m_additionalDamages = 0;
				m_currentProjectileData = m_defaultProjectile;
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

	public int GetCurrentWave()
	{
		return m_currentWave;
	}

	public void IncrementWave()
	{
		m_currentWave++;
	}

	public void ResetWave(bool bMainMenu)
	{
		m_currentWave = bMainMenu ? 1 : 2;
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag(ObjectTags.BulletEnemy))
		{
			Kill();
		}
		else if (other.CompareTag(ObjectTags.EndLevel))
        {
			if (GameManager.GetInstance().CurrentLevel.LevelLogic.bIsMapEnded)
			{
				IncrementWave();
				if (SceneManager.GetActiveScene().name != "MainMenu")
				{
					GameManager.GetInstance().LoadNextLevel();
				}
				else
				{
					SceneManager.LoadScene("LevelBase");
				}
			}
            else
            {
				Kill();
            }
        }
	}
}
