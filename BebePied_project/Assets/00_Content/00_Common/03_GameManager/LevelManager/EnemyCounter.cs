using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEnemyType
{
    SpawnedWalkMelee,
    SpawnedWalkDistance,
    SpawnedFlyMelee,
    SpawnedFlyDistance,
    MAX
};

// Global count of currently alive enemies in the level to balance spawn from pylons
public class EnemyCounter : MonoBehaviour
{
    private List<int> EnemyCount;

    void Start()
    {
        EnemyCount = new List<int>();
        for (int i = 0; i < (int)EEnemyType.MAX; ++i)
        {
            EnemyCount.Add(0);
        }
    }

    public void UpdateSpawnStats(EEnemyType eEnemyType, bool bDead)
    {
        EnemyCount[(int)eEnemyType] += bDead ? -1 : 1;
    }

    public int GetRemainingEnemyCount()
    {
        int count = 0;
        foreach (var enemyType in EnemyCount)
        {
            count += enemyType;
        }

        return count;
    }

    public EEnemyType GetPriorityEnemyTypeToSpawn()
    {
        return EEnemyType.SpawnedWalkMelee; // TODO
    }
}
