/*
	 ____  _         _   _         _      _____ _         _ _
	|    \|_|___ ___|_|_| |___ ___| |_   |   __| |_ _ _ _| |_|___
	|  |  | |_ -|_ -| | . | -_|   |  _|  |__   |  _| | | . | | . |
	|____/|_|___|___|_|___|___|_|_|_|    |_____|_| |___|___|_|___|

	This file is the property of Dissident Studio (www.dissidentstudio.fr)

	Project		:	Dissident Studio Unity Toolkit
	Author		:	Yannis Beaux (Kranck)
	Date		:	28 / 10 / 2020
	Description	:	Manages the ground detection system of the FPS controller
*/

using System;
using UnityEngine;

namespace DissidentStudio.Toolkit.FPSController
{
	/// <summary>
	/// This class casts sphere from a start position using a specified length to detect ground
	/// Manages the ground detection for the FPS Controller.
	/// The ground detection is used :
	/// - To make sure the player can jump only when grounded
	/// - Align the player on the surface
	/// - Prevent the player to go on too steep surfaces
	/// - Detect the surface to play walk sounds
	/// </summary>
	[AddComponentMenu("Dissident Studio/Character Controller/First Person/Ground Detector")]
	class GroundDetector : MonoBehaviour
	{
		[SerializeField] [Tooltip("The layers that are ignored for the ground check")]
		private LayerMask m_ignoredLayers;

		[SerializeField] [Tooltip("The position at which the cast will start (from start position to start + length)")]
		private Vector3 m_startPosition;

		[SerializeField] [Tooltip("The maximum length at which the cast will be done")]
		private float m_length = 0.0f; //< Only for line detection type

		[SerializeField] [Tooltip("The radius of the sphere used for the cast")]
		private float m_sphereRadius = 0.4f;

		/// <summary>
		/// Return true if the character is grounded, false otherwise
		/// </summary>
		public bool Grounded
		{
			get
			{
				return m_currentCollider != null; //< We are grounded if the current collider is not null
			}
		}

		private GameObject m_currentCollider; //< The current collider that is detected as a ground

		private void Update()
		{
			//
			//
			RaycastHit hit;
			if (Physics.SphereCast(transform.position + m_startPosition, m_sphereRadius, -transform.up, out hit,
				m_length))
			{
				//
				// Store the current collider
				m_currentCollider = hit.collider.gameObject;

				/*//
				// TODO: Use the step sounds only if there is a component, or move this elsewhere
				if (GetComponent<StepSoundsController>())
				{
					//
					// Check if the object has a surface component, if it's the case we use its tag instead of the tage of the object
					if (m_currentCollider.gameObject.GetComponent<StepSoundSurface>() != null)
					{
						GetComponent<StepSoundsController>().SwitchToSurface(m_currentCollider.gameObject.GetComponent<StepSoundSurface>().SurfaceSurfaceTag);
					}
					else
					{
						GetComponent<StepSoundsController>().SwitchToSurface(hit.collider.tag);
					}
				}*/
			}
			else
			{
				m_currentCollider = null;
			}
		}

		private void OnDrawGizmos()
		{
			//
			// We use red to display the gizmo
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(transform.position + m_startPosition, m_sphereRadius);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(transform.position + m_startPosition + (m_length * -transform.up), m_sphereRadius);
		}
	}
}