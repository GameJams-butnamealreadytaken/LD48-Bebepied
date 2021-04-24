/*
	 ____  _         _   _         _      _____ _         _ _
	|    \|_|___ ___|_|_| |___ ___| |_   |   __| |_ _ _ _| |_|___
	|  |  | |_ -|_ -| | . | -_|   |  _|  |__   |  _| | | . | | . |
	|____/|_|___|___|_|___|___|_|_|_|    |_____|_| |___|___|_|___|

	This file is the property of Dissident Studio (www.dissidentstudio.fr)

	Project		:	Dissident Studio Unity Toolkit
	Author		:	Yannis Beaux (Kranck)
	Date		:	18 / 10 / 2020
	Description	:	Component that takes care of the camera movements for a first person character controller
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DissidentStudio
{
	namespace Toolkit
	{
		namespace FPSController
		{
			/// <summary>
			/// Takes care of the camera control for a first person character controller
			/// </summary>
			[AddComponentMenu("Dissident Studio/Character Controller/First Person/Camera Controller")]
			[RequireComponent(typeof(CameraControllerInputsProvider))]
			public class CameraController : MonoBehaviour
			{
				//
				// TODO: Remove these variables from here
				private const float m_baseSensitivityMultiplicator = 10.0f;	//< Used to apply a base sensitivity to the look
				
				//
				// The camera controller inputs provider
				private CameraControllerInputsProvider m_cameraControllerInputsProvider;
				
				
				//
				// Vertical rotation
				private float m_verticalRotation = 0f;	//< The current vertical rotation 

				//
				// Horizontal rotation
				private float m_horizontalRotation = 0f;	//< The current horizontal rotation

				//
				// Old input
				private float m_oldVerticalInput = 0f;		//< The old input value for the vertical axis
				private float m_oldHorizontalInput = 0f;	//< The old input value for the horizontal axis
				
				//
				// Smoothing
				[SerializeField]
				[Tooltip("Enable or not the smoothing of the camera. Highly recommended.")]
				private bool m_enableSmoothing = true;
				[SerializeField]
				[Range(0.0f, 1.0f)]
				[Tooltip("The smoothing factor of the camera. It is the time the camera takes to reach its future position in seconds. 0.2f is generally a good value")]
				private float m_smoothingFactor = 0.2f;
				
				//
				// Sensitivity
				[SerializeField] [Tooltip("The sensitivity of the look.")]
				private float m_sensitivity = 2.0f;
				
				//
				// Other
				[SerializeField]
				[Tooltip("Activate the inversion of the vertical axis")]
				private bool m_invertVerticalAxis = false;
				public bool InvertVerticalAxis
				{
					get => m_invertVerticalAxis;
					set => m_invertVerticalAxis = value;
				}

				private float m_currentSmoothVelocityVertical = 0.0f;
				private float m_currentSmoothVelocityHorizontal = 0.0f;

#region LookAt Variables
				
				//
				// Look At
				// The look at is used to force the camera to look at a certain point.
				// Really useful when you want to attract the attention of the player on a certain spot
				//
				private bool m_isLookingAtTransform = false;	//< Is the camera currently looking at a point ? 
				private Transform m_lookingAtTransform = null;	//< The transform the camera is looking at
				private float m_lookingAtStartTime = 0f;	//< The time the looking at intent started
				private float m_lookingAtDuration = 0f;		//< THe duration of the look at
				private Quaternion m_startOrientation = Quaternion.identity;		//< The start orientation when the look at was triggered
				private bool m_lookAtKeepControl = false;
				
#endregion
				
				/// <summary>
				/// The forward vector of the camera (the direction in which it is looking)
				/// </summary>
				public Vector3 Forward
				{
					get { return transform.forward; }
				}
				
				/// <summary>
				/// The right vector of the camera
				/// </summary>
				public Vector3 Right
				{
					get { return transform.right; }
				}

				private bool m_active = true;	//< A boolean used to know if the camera controller is active or not, by default it is active

				/// <summary>
				/// Set/Get this controller active state. If the camera controller is active then the player can control it, otherwise it will do nothing
				/// Useful when you want the player to do nothing like in a loading screen for example
				/// </summary>
				public bool Active
				{
					get => m_active;
					set => m_active = value;
				}

				private void Awake()
				{
					//
					// Store the start horizontal and vertical rotation
					m_horizontalRotation = transform.localRotation.y;
					m_verticalRotation = transform.localRotation.x;
					
					//
					// Retrieve the camera controller inputs provider
					m_cameraControllerInputsProvider = GetComponent<CameraControllerInputsProvider>();
					if (!m_cameraControllerInputsProvider)
					{
						Debug.LogError("[Dissident Studio] : Can't use FPS Camera Controller without a camera controller inputs provider !");
					}
				}

				private void FixedUpdate()	//< The camera update is called in the FixedUpdate to match the Character Controller
											//  Since the character controller is a rigidbody, we updated it in the physic update
											//  BUT, if we udpate the character in the FixedUpdate we have to update the camera accordingly (i.e at the same frame)
											//  so to guarantee this, we update it in the fixed update too.
				{
					//
					// WARNING : The FixedUpdate thing is really important since, if we don't do this, everything will be jittery when moving the character and the
					// camera at the same time ! 
					//
					
					//
					// If the camera controller is not active, then we do nothing
					if (!m_active)
					{
						return;
					}
					
					//
					// Handle the look at
					if (m_isLookingAtTransform)	//< If we are doing a look at
					{
						//
						// Retrieve the percentage of the look at
						float percentage =(Time.time - m_lookingAtStartTime) /  m_lookingAtDuration ;
						if (percentage >= 1.0f)	//< Percentage = 100%, stop the lookat
						{
							m_isLookingAtTransform = false;	//< stop the look at intent
						}

						//
						// Some maths now.
						// We retrieve the quaternion which represent the new orientation of the player by retrieving the directional 
						// vector between the point we want to look at and our position
						// We need to do this every frame in case the character moves
						Quaternion lookAtOrientation = Quaternion.LookRotation(m_lookingAtTransform.position - transform.position);
						
						//
						// Now we compute the spherical interpolation between the start orientation and the orientation of the lookat
						Quaternion slerpedOrientation = Quaternion.Slerp(m_startOrientation, lookAtOrientation, percentage);
						
						//
						// We zero the Z component of the vector to avoid tilting the camera on the sides during the lookat 
						Vector3 slerpedOrientationEulerAngles = slerpedOrientation.eulerAngles;
						slerpedOrientationEulerAngles.z = 0f;
						
						//
						// finally set the rotation of the transform to the computed slerped orientation
						transform.rotation = Quaternion.Euler(slerpedOrientationEulerAngles);

						//
						// Don't forget to make the horizontal and vertical movement with the current transform to not restart from the start 
						// orientation when moving the camera
						SetVerticalAndHorizontalAnglesFromCurrentTransform();
						
						//
						// Last but not least, if the user wanted to prevent the player from moving the camera, we return
						// to avoid computing the camera inputs and moving the camera
						if (!m_lookAtKeepControl)
						{
							return;	//< This way the player can't control the camera during the lookat, this is purely a design choice 
						}
					}

					//
					// Retrieve the look value, Don't forget to multiply with delta time
					Vector2 lookValue = m_cameraControllerInputsProvider.GetLookInput() * Time.fixedDeltaTime;	//< Fixed update means fixed delta time
					
					//
					// we multiply the look value with the base sensitivity factor
					lookValue *= (m_sensitivity * m_baseSensitivityMultiplicator);
					
					//
					// Multiply the look value with a factor when using a gamepad
					// TODO: Vidocq only, remove it and make it generic !
					if (GetComponentInParent<PlayerInput>().currentControlScheme == "Gamepad")
					{
						lookValue *= 10f;
					}

					//
					// Compute the vertical and horizontal movement
					float horizontalInput = lookValue.x;
					float verticalInput = m_invertVerticalAxis ? lookValue.y : -lookValue.y;

					//
					// SMOOTH
					//

					//
					// If the smooth is activated, we smooth the value
					if (m_enableSmoothing)
					{
						m_oldVerticalInput = Mathf.SmoothDamp(m_oldVerticalInput, verticalInput, ref m_currentSmoothVelocityVertical,
							m_smoothingFactor);
						m_oldHorizontalInput = Mathf.SmoothDamp(m_oldHorizontalInput, horizontalInput, ref m_currentSmoothVelocityHorizontal,
							m_smoothingFactor);
					}
					else
					{
						//
						// Store the current rotation as the old onr
						m_oldVerticalInput = verticalInput;
						m_oldHorizontalInput = horizontalInput;
					}
					
					//
					// Compute the new angles
					m_verticalRotation += m_oldVerticalInput;
					m_horizontalRotation += m_oldHorizontalInput;
					
					//
					// Clamp the vertical rotation between the two boundaries.
					// TODO: Add variables for this
					m_verticalRotation = Mathf.Clamp(m_verticalRotation, -60.0f, 60.0f);

					//
					// Rotate the camera
					transform.localRotation = Quaternion.Euler(new Vector3(m_verticalRotation, 0.0f, 0.0f));

					//
					// Rotate the body
					// TODO: Rotate the body or the camera by doing this ? : 
					transform.localRotation = Quaternion.Euler(new Vector3(m_verticalRotation, m_horizontalRotation, 0.0f));
					// transform.parent.transform.localRotation = Quaternion.Euler(m_horizontalRotation * Vector3.up);
				}
				
				/// <summary>
				/// Instantly look at the given transform. This should not be used in game normally since it "teleports" the camera
				/// </summary>
				/// <param name="lookAtTransform">The transform you want the camera to look at</param>
				public void InstantlyLookAt(Transform lookAtTransform)
				{
					//
					// Directly look at the transform
					transform.LookAt(lookAtTransform);
				}

				/// <summary>
				/// Set the variables verticalRotation and horizontalRotation to the value according to the current orientation of the camera
				/// Used when forcing the player to look at a certain point to restart the camera controller from this orientation instead
				/// of going back to the rotation prior to the lookat
				/// </summary>
				public void SetVerticalAndHorizontalAnglesFromCurrentTransform()
				{
					//
					// Retrieve the new rotation of the transform
					// Specific "hack for the vertical rotation to retrieve it correctly"
					// TODO: Is there a better way to do this ? 
					if (transform.localRotation.eulerAngles.x < 180f)
					{
						m_verticalRotation = Mathf.Clamp((transform.localRotation.eulerAngles.x), -60.0f, 60.0f);
					}
					else
					{
						m_verticalRotation = Mathf.Clamp(-(360.0f - transform.localRotation.eulerAngles.x), -60.0f, 60.0f);
					}
					m_horizontalRotation = transform.localRotation.eulerAngles.y;
				}

				public Vector2 GetCurrentRotation()
				{
					return new Vector2(m_horizontalRotation, m_verticalRotation);
				}

				public void SetCurrentRotation(Vector2 rotation)
				{
					m_horizontalRotation = rotation.x;
					m_verticalRotation = rotation.y;
				}

				/// <summary>
				/// Force the camera to look at a certain transform.
				/// We need to specify the transform, the time the camera will take to look at the target, and if the user still has control
				/// NOTE :
				/// The allowCameraControl is purely a design choice. If this is enabled (which is the default value), the player will be able
				/// to move the camera during the look at operation. It is a better choice to not constrin the player to totally
				/// loose its influence on the game but it is a possibility that if the player forces the movement of the camera in another direction
				/// that the camera will never look at the desired point truly, so take this into account.
				/// TODO: Add a factor to reduce or increase the impact of the player movements during the camera lookat ?
				/// </summary>
				/// <param name="lookAtTransform">The transform to look at</param>
				/// <param name="timeToLookAtInSeconds">The time the camera will take to look at the target</param>
				/// <param name="allowCameraControl">Do the player has the control on the camera</param>
				public void LookAt(Transform lookAtTransform, float timeToLookAtInSeconds, bool allowCameraControl = true)
				{
					//
					// Start the look at by filling some value
					m_isLookingAtTransform = true;	//< We are looking a t something
					m_lookingAtTransform = lookAtTransform;	//< Store the transform we look at
					m_lookingAtDuration = timeToLookAtInSeconds;	//< The time we have to look at the target in seconds
					m_lookingAtStartTime = Time.time;	//< Store the start time
					m_startOrientation = transform.rotation;	//< Store the start duration for the slerp
					m_lookAtKeepControl = allowCameraControl;	//< Do the player still has the control on the camera ? 
				}
			}
		}
	}
}
