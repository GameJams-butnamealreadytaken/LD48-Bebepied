using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BebePiedCameraControllerInputsProvider : DissidentStudio.Toolkit.FPSController.CameraControllerInputsProvider
{


	public override Vector2 GetLookInput()
	{
		return GetComponentInParent<PlayerInput>().actions["Look"].ReadValue<Vector2>();
	}
	

}
