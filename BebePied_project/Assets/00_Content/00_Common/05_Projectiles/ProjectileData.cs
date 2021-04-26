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
	[Tooltip("The life time of the projectile")]
	private float m_lifeTime = 10.0f;

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

	[Header("Physic")] 
	[SerializeField]
	[Tooltip("Activate the constrain of the rotation of the projectiles. Should be false for all projectiles except the sausages")]
	private bool m_constrainProjectileRotation = true;

	[SerializeField] 
	[Tooltip("Set to false to deactivate the physics of the projectile")]
	private bool m_deactivatePhysics = false;

	[Header("Other")] 
	[SerializeField]
	[Tooltip("Is this projectile a sausage ? Needed for the stats")]
	private bool m_isSausage = false;

	[Header("Sound")] [SerializeField] [Tooltip("The sounds that this projectile emits")]
	private List<AudioClip> m_sounds = new List<AudioClip>();


	public int Damages => m_damages;
	public float LifeTime => m_lifeTime;

	public float FireVelocity=> m_fireVelocity;
	
	public GameObject Prefab => m_projectilePrefab;

	public int FireProjectilesCount => m_firedProjectilesCount;

	public float TimeBetweenFires => m_timeBetweenFires;

	public float DispersionFactor => m_dispersionFactor;

	public bool ConstrainProjectilesRotation => m_constrainProjectileRotation;

	public bool DeactivatePhysics => m_deactivatePhysics;

	public bool IsSausage => m_isSausage;

	public List<AudioClip> Sounds => m_sounds;
}
