using System.Collections;
using System.Collections.Generic;
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
			GetComponentInChildren<PlayerWeapon>().Shoot(m_defaultProjectile);
		}
	}

	/// <summary>
	/// KIll the player
	/// </summary>
	public void Kill()
	{
		Debug.Log("You is dead");
	}
}
