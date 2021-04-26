using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBonusType
{
	Damage,
	Speed,
	Armor,
	Projectile,
	Max
}

public class BonusBehavior : MonoBehaviour
{
	public BonusData m_bonusData;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag(ObjectTags.Player))
		{
			// 
			// Start bonus in the player
			GameManager.GetInstance().Player.AddBonus(m_bonusData);
			
			//
			// Destroy the bonus object
			Destroy(gameObject);
		}
	}
}
