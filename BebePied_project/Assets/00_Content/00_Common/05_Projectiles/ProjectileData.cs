using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The data for a projectile. Describes the settings of this projectile
/// </summary>
[CreateAssetMenu(menuName = "Bebe Pied/Projectiles/Create Projectile Data", fileName = "Projectile")]
public class ProjectileData : ScriptableObject
{
	[Header("Base")]
	[SerializeField] 
	[Tooltip("The damages of the projectile")]
	private int m_damages;

	[SerializeField] 
	[Tooltip("The velocity with which the projectile is fired")]
	private float m_fireVelocity = 40f;
	
	[SerializeField] 
	private GameObject m_projectilePrefab;

	[Header("Emission")] 
	[SerializeField] 
	[Tooltip("The number of projectiles that are fired when shooting")]
	private int m_firedProjectilesCount = 1;

	[SerializeField] 
	[Tooltip("The time that the weapon must wait to fire again")]
	private float m_timeBetweenFires = 0.02f;

	[SerializeField] 
	[Range(0f, 2f)]
	[Tooltip("Dispersion factor")]
	private float m_dispersionFactor = 0.0f;

	public int Damages => m_damages;

	public float FireVelocity=> m_fireVelocity;
	
	public GameObject Prefab => m_projectilePrefab;

	public int FireProjectilesCount => m_firedProjectilesCount;

	public float TimeBetweenFires => m_timeBetweenFires;

	public float DispersionFactor => m_dispersionFactor;

}
