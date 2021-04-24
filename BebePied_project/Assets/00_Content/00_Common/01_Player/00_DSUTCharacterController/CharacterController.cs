/*
	 ____  _         _   _         _      _____ _         _ _
	|    \|_|___ ___|_|_| |___ ___| |_   |   __| |_ _ _ _| |_|___
	|  |  | |_ -|_ -| | . | -_|   |  _|  |__   |  _| | | . | | . |
	|____/|_|___|___|_|___|___|_|_|_|    |_____|_| |___|___|_|___|

	This file is the property of Dissident Studio (www.dissidentstudio.fr)

	Project		:	Dissident Studio Unity Toolkit
	Author		:	Yannis Beaux (Kranck)
	Date		:	27 / 10 / 2020
	Description	:	Controller for a first person character controller. Takes care of all the movements part and inform other components
*/

using System;
// using DissidentStudio.Toolkit.Utils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;

/*
 *
 * Theorical structure :
 *
 * + FPSPlayer (GO)
 * - Character Controller
 * - Animation Controller
 * - Sound Controller
 * 		+ Camera Holder (GO)
 * 		- Camera Controller
 * 			-> Camera
 *
 * 
 */

namespace DissidentStudio.Toolkit.FPSController
{
	// TODO: For now we use a capsule that is on the character controller for collisions etc...
	// It would be interesting to create our own capsule by using parameter values in the inspector
	
	/// <summary>
	/// Manages the control of the first person controller
	/// </summary>
	[AddComponentMenu("Dissident Studio/Character Controller/First Person/Character Controller")]
	[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody),typeof(CharacterControllerInputsProvider))]
	public class CharacterController : MonoBehaviour/*, Toolkit.IToolkitInGameDebugListener*/
	{

		private CharacterControllerInputsProvider m_characterControllerInputsProvider;

		[Header("Base")]
		[SerializeField]
		[Tooltip("The base speed of the player (when walking)")]
		private float m_walkSpeed = 1.0f;

		[Header("Running")] 
		[SerializeField] 
		[Tooltip("Set to true to enable running, false otherwise")]
		private bool m_canRun = true;
		
		[SerializeField]
		[Tooltip("The speed of the player when running")]
		private float m_runSpeed = 2.0f;

		[Header("Crouching")]
		[SerializeField]
		[Tooltip("Set to true to enable crouching, false otherwise")]
		private bool m_canCrouch = true;
		
		[SerializeField]
		[Tooltip("The speed of the player when crouching")]
		private float m_crouchedSpeed = 0.5f;

		[SerializeField] 
		[Tooltip("The factor with which the height of the player will be affected when crouching ")]
		private float m_crouchedFactor = 1.53f;	//< Arbitray value calculated measuring myself

		[SerializeField] 
		[Tooltip("The duration when crouching and standing up")]
		private float m_crouchingAndStandUpDuration = 1.7f;

		[Header("Jump")]
		[SerializeField]
		[Tooltip("True if the character can jump, false otherwise")]
		private bool m_canJump = true;
		
		[SerializeField]
		[Tooltip("Specify the number of additional jumps. By default it is zero, the player can only jump once.")]
		private int m_additionalJumps = 0;
		
		[SerializeField]
		[Tooltip("The force applied to the character when jumping")]
		private float m_jumpForce = 2.0f;

		[SerializeField] 
		[Tooltip("Is the player able to control its movement in the air ?")]
		private bool m_enableAirControl = false;

		[Header("Settings")] 
		[SerializeField] 
		[Tooltip("Limit the movements of the player if he is going backward")]
		private bool m_limitCharacterSpeedBackward = false;
		
		[SerializeField] 
		[Range(0f, 1.0f)]
		[Tooltip("The factor that is applied to the speed of the player when he is moving backwards (1 = full speed | 0 = no speed)")]
		private float m_characterSpeedReductionFactorBackward = 1.0f;

		[SerializeField] 
		[Tooltip("Limit the movements of the player if he is going sideways")]
		private bool m_limitCharacterSpeedSideways = false;
		
		[SerializeField] 
		[Range(0f, 1.0f)]
		[Tooltip("The factor that is applied to the speed of the player when he is moving sideways (1 = full speed | 0 = no speed)")]
		private float m_characterSpeedReductionFactorSideways = 1.0f;
		
#region Components

		private CameraController m_cameraController;	//< The camera controller associated to this character controller
		private Rigidbody m_rigidbody;	//< The rigidbody of this controller
		
#endregion
		
#region State

		private bool m_crouched = false;	//< Is the character crouched ?
		private bool m_moving = false;	//< Is the character moving ?
		private bool m_running = false;	//< Is the character running
		private float m_heightOfColliderBeforeCrouching;	//< Used to store the height of the collider before crouching, to have the height of the player dynamic

		/// <summary>
		/// Indicates if the character is moving. It does not inform you that the character is walking, running or moving crouched,
		/// only that the character is in movement
		/// </summary>
		public bool Moving => m_moving;

		/// <summary>
		/// Indicates if the character is crouched.
		/// </summary>
		/*public bool Crouched
		{
			get => m_crouched;
			set
			{
				if (value)
				{
					Crouch();
				}
				else
				{
					StandUp();
				}
			}
		}*/

		/// <summary>
		/// Indicates if the character is running
		/// </summary>
		public bool Running => m_running;

#endregion

		private float m_timeMovingSinceLastStepSound = 0.0f;	//< TODO: Move, improve ? Do something
		private bool m_isCrouching = false;	//< Are we crouching ?
		private bool m_isStandingUp = false;	//< Are we standing up ? 
		private float m_crouchingOrStandingUpStartTime = 0f;	//< The time at which a crouch or stand up inten has been started
		private int m_currentJumpCount = 0;	//< Counts the number of jumps the player has done. It is resetted when the player touches the ground

		private bool m_active = true;	//< Is the character controller active.

		/// <summary>
		/// The active state of the character controller. If it is inactive then it will not receive any player inputs and stay static
		/// It is useful to set this value in a loading screen for example
		/// </summary>
		public bool Active
		{
			get => m_active;
			set => m_active = value;
		}
		
		//
		// Cheats (only in debug)
#if DEBUG
		[Header("Cheats")]
		private bool m_godMode = false;	//< Special mode that allows to fly, pass through walls etc...
										//  Only fit for debug purposes.

		/// <summary>
		/// The god mod state of the player
		/// </summary>
		public bool GodMode
		{
			get => m_godMode;
			set
			{
				//
				// We now are in god mode
				m_godMode = value;

				//
				// Set the rigidbody to kinematic if we are flying
				m_rigidbody.isKinematic = m_godMode;
				m_rigidbody.useGravity = !m_godMode;
				
				//
				// "Reset" some states
				// if (m_crouched) {StandUp();}	//< Uncrouch to not fly while crouched
			}
		}
#endif	//DEBUG
		
		// Called when script is instantiated
		private void Awake()
		{
			//
			// We ensure we can find a camera controller in the children
			m_cameraController = GetComponentInChildren<CameraController>();
			if (!m_cameraController)
			{
				Debug.LogError( "[DissidentStudio] : Character Controller has no camera controller in its children !");
			}

			//
			// Retrieve the rigidbody
			m_rigidbody = GetComponent<Rigidbody>();
			if (!m_rigidbody)
			{
				Debug.LogError( "[DissidentStudio] : Character Controller has no rigidbody !");
			}

			//
			// Freeze the rigidbody rotation
			// m_rigidbody.freezeRotation = true;
			m_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX |
			                          RigidbodyConstraints.FreezeRotationY |
			                          RigidbodyConstraints.FreezeRotationZ;
			
			//
			// Retrieve the character controller input provider
			m_characterControllerInputsProvider = GetComponent<CharacterControllerInputsProvider>();
			if (!m_characterControllerInputsProvider)
			{
				Debug.LogError("[DissidentStudio] : Character Controller has no inputs provider !");
			}
			
/*#if DEBUG
			//
			// Register the debug listener for the character controller
			ToolkitInGameDebug.Instance.AddListener(this, ToolkitInGameDebug.DebugCategory.Player);
			
			//
			// Add debug infos
			DebugEntryFolder entryFolder = DebugManager.Instance.GetFolder("Character Controller");
			entryFolder.AddEntry("GodMode", nameof(GodMode), this, this.GetType(), GodMode.GetType(), true);
			entryFolder.AddEntry("Crouched", nameof(Crouched), this, this.GetType(), Crouched.GetType(), true);
			entryFolder.AddEntry("Moving", nameof(Moving), this, this.GetType(), Moving.GetType(), false);
			entryFolder.AddEntry("Running", nameof(Running), this, this.GetType(), Running.GetType(), false);
#endif //DEBUG*/
		}

		private void Update()
		{
			//
			// Update the crouching system.
			// We do this in the normal update and not the fixed to avoid problems
			// UpdateCrouch();
			
			//
			// If the player can jump
			if (m_canJump)
			{
				//
				// We can only jump when we are grounded
				// TODO: Add double jump ? 
				if (GetComponent<GroundDetector>().Grounded)
				{
					//
					// Reset the jump count
					m_currentJumpCount = 0;
					
					//
					// Check the jump input
					if (GetComponent<CharacterControllerInputsProvider>().GetJumpInput())
					{
						m_currentJumpCount++;	//< Increase the number of jumps
						Jump();	//< Make the character jump
					}
				}
				else
				{
					//
					// The character is in air, but he still can jump if he has additional jump
					if (GetComponent<CharacterControllerInputsProvider>().GetJumpInput()
					    && m_currentJumpCount <= m_additionalJumps)	//< Check if he can still jump
					{
						m_currentJumpCount++;	//< Increase the number of jumps
						Jump();	//< Make the character jump
					}
				}
			}
		}

		private void FixedUpdate()
		{
			//
			// if the character controller is inactive we do nothing
			if (!m_active)
			{
				return;
			}
			
			//
			// Cheats. If we are in debug and in fly mode, we do not go through the normal update but instead use a CheatUpdate method
#if DEBUG
			//
			// We are in fly mode, we "cheat update"
			if (m_godMode) { UpdateGodMode(); }
#endif	//DEBUG

			//
			// Retrieve the vertical and horizontal input values
			Vector2 input = m_characterControllerInputsProvider.GetMovementInput();
			
			//
			// If the input is not null, it means we are moving
			if (input.x != 0f || input.y != 0f)
			{
				m_moving = true;	//< We are now moving
			}
			else
			{
				m_moving = false;
			}

			//
			// Multiply the input with the delta time
			// verticalInput *= Time.deltaTime;
			// horizontalInput *= Time.deltaTime;

			//
			// IS the character running ? 
			if (m_canRun) //< If the character can run
			{
				//
				// Ensure the player is not crouched
				if (!m_crouched)
				{
					//
					// Test if the run input is pressed
					m_running = m_characterControllerInputsProvider.GetRunInput();
				}
			}

			//
			// Compute the final speed of the character
			float characterSpeed = 0f;
			if (m_crouched)
			{
				characterSpeed = m_crouchedSpeed;	//< Crouched = crouched speed
			}
			else
			{
				characterSpeed = m_running ? m_runSpeed : m_walkSpeed;	//< Running or not running ? (that is the question)
			}
			
			//
			// Adapt the movement speed depending if the character is configured to reduce the movement speed backward or sideways
			// In order to do this we clamp the input value between both factors
			if (m_limitCharacterSpeedBackward)
			{
				// We reduce only the minimum (since going backwards mean inputs.y < 0)
				// When y > = it means the player goes forward
				input.y = Mathf.Clamp(input.y, -1f * m_characterSpeedReductionFactorBackward, 1f);
			}
			if (m_limitCharacterSpeedSideways)
			{
				//
				// Here we limit in both directions 
				input.x = Mathf.Clamp(input.x, -1f * m_characterSpeedReductionFactorSideways, 1f * m_characterSpeedReductionFactorSideways);
			}

			//
			// Set the velocity of the rigidbody
			Vector3 verticalMove = m_cameraController.Forward * input.y;
			Vector3 horizontalMove = m_cameraController.Right * input.x;
			Vector3 move = verticalMove + horizontalMove;
			move *= characterSpeed;	//< Multiply by the computed character speed
			
			//
			// Ensure the character is not in the air
			if (GetComponent<GroundDetector>().Grounded)
			{
				//
				// The character is grounded we can move it
				m_rigidbody.velocity = new Vector3(move.x, m_rigidbody.velocity.y, move.z);
			}
			else
			{
				//
				// The character is not grounded, we can't apply the movements if the air control is not activated
				if (m_enableAirControl)
				{
					//
					// TODO: Add a parameter for the move factor ? ?
					m_rigidbody.velocity = new Vector3(move.x / 2f, m_rigidbody.velocity.y, move.z / 2f);
				}
			}
		}

		private void UpdateGodMode()
		{
			//
			// Retrieve the vertical and horizontal input values
			Vector2 input = m_characterControllerInputsProvider.GetMovementInput();
			input *= m_characterControllerInputsProvider.GetRunInput() ? 0.8f : 0.2f;	//< Force the reduction of the input to reduce the speed. Takes the run input into account for the reduction
			
			//
			//
			m_rigidbody.MovePosition(transform.position + (input.y * m_cameraController.Forward) + (input.x * m_cameraController.Right));
			// TODO: Character stops when moving head in fly mode
			// TODO: Speed adjustment
		}
		
#region Run

		// Nothing in there in fact
		// TODO : Add method to smooth the running speed. Like : start running, increase the speed, stop running, decrease the speed ?

#endregion
		
/*
 No crouch in this project
#region Crouch
		

		private void UpdateCrouch()
		{
			//
			// Test if we can crouch or stand up depending on the input and our current state
			if (m_characterControllerInputsProvider.GetCrouchInput() && !m_isCrouching && !m_isStandingUp)
			{
				if (m_crouched)	//< If we are crouched, we can stand up
				{
					StandUp();
				}
				else
				{
					Crouch();	//< if we are stand up we can crouch
				}
			}
			
			//
			// If we are crouching, we update the capsule collider accordingly
			if (m_isCrouching)
			{
				bool finished = false;	//< Is the crouching finished ? 
				float value = Misc.Lerp(m_crouchingOrStandingUpStartTime
					, m_crouchingAndStandUpDuration
					, m_heightOfColliderBeforeCrouching
					, m_heightOfColliderBeforeCrouching / m_crouchedFactor
					, out finished);
				// Set the height of the collider
				GetComponent<CapsuleCollider>().height = value;
				
				//
				// If the operation is finished, we set the booleans accordingly
				if (finished)
				{
					m_isCrouching = false;
					m_crouched = true;
				}
			}
			else if (m_isStandingUp)
			{
				bool finished = false;	//< Is the operation finished ? 
				float value = Misc.Lerp(m_crouchingOrStandingUpStartTime
					, m_crouchingAndStandUpDuration
					, m_heightOfColliderBeforeCrouching / m_crouchedFactor
					, m_heightOfColliderBeforeCrouching
					, out finished);
				// Set the height of the collider
				GetComponent<CapsuleCollider>().height = value;
				
				//
				// If the operation is finished, we set the booleans accordingly
				if (finished)
				{
					m_isStandingUp = false;
					m_crouched = false;
				}
			}
		}

		public void Crouch()
		{
			m_isCrouching = true;	//< We are now crouching
			m_crouchingOrStandingUpStartTime = Time.time;	//< Store the start time
			m_heightOfColliderBeforeCrouching = GetComponent<CapsuleCollider>().height;	//< Store the height of the capsule when starting the crouch
		}

		public void StandUp()
		{
			m_isStandingUp = true;	//< We are standing up
			m_crouchingOrStandingUpStartTime = Time.time;	//< Store the start time
		}
		
#endregion	// Crouch */

#region Jump

		private void Jump()
		{
			Debug.Log("Jumping");
			m_rigidbody.AddForce(0.0f, m_jumpForce, 0.0f, ForceMode.Impulse);
		}

#endregion

/*#region Debug

		public void DrawGUI()
		{
#if DEBUG
			//
			// Player informations
			GUILayout.Box("[Player]");
			GUILayout.Label("P : " + transform.position +		//< Position
			                " | M : " + Moving +	//< Is the player moving
			                " | C : " + Crouched + 	//< Is the player crouched ? 
			                " | R : "  + Running);	//< Is the player running ? 
					
			//
			// Activate/Deactivate god mode
			GodMode = GUILayout.Toggle(GodMode, ("God mode"));
#endif	//DEBUG
		}

#endregion*/
	}
}