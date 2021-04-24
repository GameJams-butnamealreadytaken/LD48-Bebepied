using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The data for a projectile. Describes the settings of this projectile
/// </summary>
[CreateAssetMenu(menuName = "Bebe Pied/Projectiles/Create Projectile Data", fileName = "Projectile")]
public class ProjectileData : ScriptableObject
{
	
	[SerializeField] 
	[Tooltip("The damages of the projectile")]
	private int m_damages;

	[SerializeField] 
	[Tooltip("The velocity with which the projectile is fired")]
	private float m_fireVelocity = 40f;
	
	[SerializeField] 
	private GameObject m_projectilePrefab;

	public int Damages
	{
		get => m_damages;
	}

	public float FireVelocity
	{
		get => m_fireVelocity;
	}
	
	public GameObject Prefab
	{
		get => m_projectilePrefab;
	}

}
