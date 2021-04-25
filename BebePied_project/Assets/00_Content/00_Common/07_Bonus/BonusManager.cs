using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusManager : MonoBehaviour
{


	public List<BonusData> m_towerBonusDatas = new List<BonusData>();
	public List<BonusData> m_ennemyBonusData = new List<BonusData>();


	public float m_ennemyRatioCountForBonus = 15f;	//< 15 monsters killed in ... 
	public float m_ennemyRatioDurationForBonus = 3f;	//< 10 seconds 

}
