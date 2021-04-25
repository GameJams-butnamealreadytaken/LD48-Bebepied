using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

	public ProjectileData m_defaultProjectile;
	
	// Start is called before the first frame update
	void Start()
	{
		//
		// Lock the cursor 
		Cursor.lockState = CursorLockMode.Locked;
	}

	// Update is called once per frame
	void Update()
	{
		//
		// Test if the player is shooting
		if (GetComponent<PlayerInput>().actions["Shoot"].ReadValue<float>() >= 0.5f)
		{
			GetComponentInChildren<PlayerWeaponHolder>().Shoot(m_defaultProjectile);
		}
	}

	/// <summary>
	/// KIll the player
	/// </summary>
	public void Kill()
	{
		Debug.Log("You is dead");
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
}
