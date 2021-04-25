using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bebe Pied/Bonuses/Create Bonus Data", fileName = "Bonus")]
public class BonusData : ScriptableObject
{

	[SerializeField] 
	private EBonusType m_eBonusType;

	[SerializeField] 
	private GameObject m_bonusPrefab;

	[SerializeField] 
	private int m_bonusDurationInSeconds = 10;
	
	[SerializeField] 
	private ProjectileData m_newProjectileData;
	
	[SerializeField] 
	private int m_additionalDamages;
	
	[SerializeField] 
	private int m_additionalArmor;
	
	[SerializeField] 
	private int m_additionalSpeed;
	
	
	public EBonusType Type => m_eBonusType;
	
	public GameObject Prefab => m_bonusPrefab;
	
	public int Duration => m_bonusDurationInSeconds = 10;
	
	public ProjectileData Projectile => m_newProjectileData;
	
	public int Damages => m_additionalDamages;
	
	public int Armor => m_additionalArmor;
	
	public int Speed => m_additionalSpeed;

}
