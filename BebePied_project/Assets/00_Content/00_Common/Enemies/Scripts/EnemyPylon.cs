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

        // TODO Spawn animation

        //Debug
        SpawnEnemyWave();
    }

    protected override void Update()
    {
        base.Update();

        //TODO Spawn enemy waves
    }

    private void SpawnEnemyWave()
    {
        int enemyCount = Random.Range(3, 6);
        int enemyType = 0;

        for (int i = 0; i < enemyCount; ++i)
        {
            Instantiate(EnemyList[enemyType], transform.position, Quaternion.identity);
        }
    }
}
