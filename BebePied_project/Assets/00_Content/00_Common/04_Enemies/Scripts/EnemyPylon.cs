using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPylon : EnemyBase
{
    [Header("Spawning")]
    public int SpawnFrequency;
    public GameObject SpawnFloor;
    public GameObject SpawnFly;
    public GameObject[] EnemyFloorList;
    public GameObject[] EnemyFlyList;

    [Header("Bonus")] public BonusData[] BonusDataList;

    public void OnSpawnEnded()
    {
        StartCoroutine("SpawnEnemyWave");
    }

    IEnumerator SpawnEnemyWave()
    {
        yield return new WaitForSeconds(0.5f);

        for (;;)
        {
            int enemyCount = Random.Range(1 * GameManager.GetInstance().Player.GetCurrentWave(), 2 * GameManager.GetInstance().Player.GetCurrentWave());
            // int enemyCount = 100;

            for (int i = 0; i < enemyCount; ++i)
            {
                if (EnemyFloorList.Length > 0)
                {
                    GameObject enemyInstance = Instantiate(EnemyFloorList[Random.Range(0, EnemyFloorList.Length)], SpawnFloor.transform.position, Quaternion.identity);
                    EnemyBase enemy = enemyInstance.GetComponent<EnemyBase>();
                    enemy.Player = Player;
                    enemy.EnemyCounter = EnemyCounter;
                }

                if (EnemyFlyList.Length > 0)
                {
                    GameObject enemyInstance = Instantiate(EnemyFlyList[Random.Range(0, EnemyFlyList.Length)], SpawnFly.transform.position, Quaternion.identity);
                    EnemyBase enemy = enemyInstance.GetComponent<EnemyBase>();
                    enemy.Player =Player;
                    enemy.EnemyCounter = EnemyCounter;
                }
                
            }
            
            yield return new WaitForSeconds(SpawnFrequency);
        }
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        
        // Spawn Bonus
        int bonusID = Random.Range(0, BonusDataList.Length);
        GameObject  go = Instantiate(BonusDataList[bonusID].Prefab, SpawnFloor.transform.position, Quaternion.identity);
        go.transform.position = SpawnFloor.transform.position;
        BonusBehavior bonusBehaviour = go.AddComponent<BonusBehavior>();
        bonusBehaviour.m_bonusData = BonusDataList[bonusID];

        SetAutoDestroyOnDeath(false);
        Destroy(transform.parent.gameObject);
    }
}
