using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPylon : EnemyBase
{
    [Header("Enemies To Spawn")]
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
            yield return new WaitForSeconds(10.0f);

            int enemyCount = Random.Range(4, 6);
            int enemyType = 0;

            for (int i = 0; i < enemyCount; ++i)
            {
                Instantiate(EnemyList[enemyType], transform.position, Quaternion.identity);
            }
        }
    }
}
