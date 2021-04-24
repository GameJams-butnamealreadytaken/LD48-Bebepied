using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPylon : EnemyBase
{
    [Header("Spawning")]
    public int SpawnFrequency;
    public GameObject[] EnemyList;

    protected override void Start()
    {
        base.Start();

        // TODO Wait spawn animation end before starting coroutine

        StartCoroutine("SpawnEnemyWave");
    }

    IEnumerator SpawnEnemyWave()
    {
        for (;;)
        {
            yield return new WaitForSeconds(SpawnFrequency);

            int enemyCount = Random.Range(4, 6);
            int enemyType = 0;

            for (int i = 0; i < enemyCount; ++i)
            {
                GameObject enemyInstance = Instantiate(EnemyList[enemyType], transform.position, Quaternion.identity);
                EnemyBase enemy = enemyInstance.GetComponent<EnemyBase>();
                enemy.SetPlayer(Player);
                enemy.StartAI();
            }
        }
    }
}
