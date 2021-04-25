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

    [Header("Bonus")] public GameObject[] BonusList;

    public void OnSpawnEnded()
    {
        StartCoroutine("SpawnEnemyWave");
    }

    IEnumerator SpawnEnemyWave()
    {
        yield return new WaitForSeconds(0.5f);

        for (;;)
        {
            int enemyCount = Random.Range(4, 6);
            int enemyType = 0;

            for (int i = 0; i < enemyCount; ++i)
            {
                if (EnemyFloorList.Length > enemyType)
                {
                    GameObject enemyInstance = Instantiate(EnemyFloorList[enemyType], SpawnFloor.transform.position, Quaternion.identity);
                    EnemyBase enemy = enemyInstance.GetComponent<EnemyBase>();
                    enemy.Player = Player;
                    enemy.EnemyCounter = EnemyCounter;
                }

                if (EnemyFlyList.Length > enemyType)
                {
                    GameObject enemyInstance = Instantiate(EnemyFlyList[enemyType], SpawnFly.transform.position, Quaternion.identity);
                    EnemyBase enemy = enemyInstance.GetComponent<EnemyBase>();
                    enemy.Player =Player;
                    enemy.EnemyCounter = EnemyCounter;
                }

                yield return new WaitForSeconds(SpawnFrequency);
            }
        }
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        // Spawn Bonus
        int bonusID = Random.Range(0, BonusList.Length);
        Instantiate(BonusList[bonusID], SpawnFloor.transform.position, Quaternion.identity);

        SetAutoDestroyOnDeath(false);
        Destroy(transform.parent.gameObject);
    }
}
