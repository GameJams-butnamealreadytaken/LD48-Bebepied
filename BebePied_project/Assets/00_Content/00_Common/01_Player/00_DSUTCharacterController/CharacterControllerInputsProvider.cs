/*
	 ____  _         _   _         _      _____ _         _ _
	|    \|_|___ ___|_|_| |___ ___| |_   |   __| |_ _ _ _| |_|___
	|  |  | |_ -|_ -| | . | -_|   |  _|  |__   |  _| | | . | | . |
	|____/|_|___|___|_|___|___|_|_|_|    |_____|_| |___|___|_|___|

	This file is the property of Dissident Studio (www.dissidentstudio.fr)

	Project		:	Dissident Studio Unity Toolkit
	Author		:	Yannis Beaux (Kranck)
	Date		:	9 / 11 / 2020
	Description	:	Base class that is used to provide inputs to the FPS character controller
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DissidentStudio
{
	namespace Toolkit
	{
		namespace FPSController
		{
			public class CharacterControllerInputsProvider : MonoBehaviour
			{
				/// <summary>
				/// Use this method to return the movement input. X is the horizontal and Y the vertical input
				/// </summary>
				/// <returns>A vector 2 representing the input for the player movement</returns>
				public virtual Vector2 GetMovementInput()
				{
					return Vector2.zero;
				}

				/// <summary>
				/// Use this method to return the crouch input. True if the player press the crouch input, false otherwise
				/// </summary>
				/// <returns>True if the player is crouching, false otherwise</returns>
				public virtual bool GetCrouchInput()
				{
					return false;
				}

				/// <summary>
				/// Use this method to return the run input. True if the player is running, false otherwise
				/// </summary>
				/// <returns>True if the player is running, false otherwise</returns>
				public virtual bool GetRunInput()
				{
					return false;
				}

				/// <summary>
				/// Use this method to return the input for the god mode. The god mode allow the player to fly around the scene, pass through walls etc... (cheat)
				/// </summary>
				/// <returns>True if the input to toggle the god mode is pressed, false otherwise</returns>
				public virtual bool GetGodModeInput()
				{
					return false;
				}

				/// <summary>
				/// Use this method to return the input for the jump.
				/// </summary>
				/// <returns>Return true if the input to jump is pressed, false otherwise</returns>
				public virtual bool GetJumpInput()
				{
					return false;
				}
			}
		}
	}
}
