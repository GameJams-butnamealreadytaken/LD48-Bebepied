using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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
		
	}

	/// <summary>
	/// KIll the player
	/// </summary>
	public void Kill()
	{
		Debug.Log("You is dead");
	}
}
