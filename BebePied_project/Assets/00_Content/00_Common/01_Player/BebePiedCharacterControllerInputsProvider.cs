using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BebePiedCharacterControllerInputsProvider : DissidentStudio.Toolkit.FPSController.CharacterControllerInputsProvider
{

	
	/// <summary>
	/// Use this method to return the movement input. X is the horizontal and Y the vertical input
	/// </summary>
	/// <returns>A vector 2 representing the input for the player movement</returns>
	public override Vector2 GetMovementInput()
	{
		return GetComponent<PlayerInput>().actions["Move"].ReadValue<Vector2>();
	}

	public override bool GetJumpInput()
	{
		return GetComponent<PlayerInput>().actions["Jump"].triggered;
	}
	
}
