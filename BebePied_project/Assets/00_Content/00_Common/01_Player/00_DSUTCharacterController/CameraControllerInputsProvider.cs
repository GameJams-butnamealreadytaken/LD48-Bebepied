/*
	 ____  _         _   _         _      _____ _         _ _
	|    \|_|___ ___|_|_| |___ ___| |_   |   __| |_ _ _ _| |_|___
	|  |  | |_ -|_ -| | . | -_|   |  _|  |__   |  _| | | . | | . |
	|____/|_|___|___|_|___|___|_|_|_|    |_____|_| |___|___|_|___|

	This file is the property of Dissident Studio (www.dissidentstudio.fr)

	Project		:	Dissident Studio Unity Toolkit
	Author		:	Yannis Beaux (Kranck)
	Date		:	9 / 11 / 2020
	Description	:	Base class that is used to provide inputs to the FPS camera controller
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
			public class CameraControllerInputsProvider : MonoBehaviour
			{
				/// <summary>
				/// Use this method to return the input value of the camera look
				/// </summary>
				/// <returns>The look input</returns>
				public virtual Vector2 GetLookInput()
				{
					return Vector2.zero;
				}
			}
		}
	}
}