using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelBase : MonoBehaviour
{
    [Serializable]
    public class EnemyTypeNumber
    {
        public GameObject Type;
        public int Count;
        public float DelayFromPrevious;
    }

    [Header("Spawning")] public EnemyTypeNumber[] EnemyList;

    [Header("Options")] public int Width = 100;
    public int Height = 100;
    public Vector3 LevelCenter = Vector3.zero;
    public int StartDivider = 16;

    private List<Vector3> TakenSpots = new List<Vector3>();
    private float SecondsSinceLastSpawn = 0;
    private int CurrentEnemyIndex = 0;

    private EnemyCounter EnemyCounter;

    // Start is called before the first frame update
    private void Start()
    {
        EnemyCounter = GetComponent<EnemyCounter>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (CurrentEnemyIndex < EnemyList.Length)
        {
            SecondsSinceLastSpawn += Time.deltaTime;
            int divider = StartDivider;
            EnemyTypeNumber enemyTypeNumber = EnemyList[CurrentEnemyIndex];
            if (enemyTypeNumber.DelayFromPrevious < SecondsSinceLastSpawn)
            {
                SecondsSinceLastSpawn = 0;
                ++CurrentEnemyIndex;
                for (int enemyCount = 0; enemyCount < enemyTypeNumber.Count; enemyCount++)
                {

                    Vector3 position;
                    Vector3 roundPosition;
                    int tooMuchLoops = 100;
                    do
                    {
                        position = LevelCenter;
                        position.x += Random.Range(-Width / 2, Width / 2);
                        position.z += Random.Range(-Height / 2, Height / 2);
                        roundPosition.x = (int) (position.x / divider);
                        roundPosition.y = (int) (position.y / divider);
                        roundPosition.z = (int) (position.z / divider);
                        if (--tooMuchLoops <= 0)
                        {
                            if (divider > 1)
                            {
                                divider /= 2;
                                tooMuchLoops = 100;
                            }
                            else
                            {
                                break;
                            }
                        }
                    } while (TakenSpots.Contains(roundPosition));

                    TakenSpots.Add(roundPosition);

                    GameObject enemyInstance = Instantiate(enemyTypeNumber.Type, position, Quaternion.identity);
                    
                    EnemyPylon Pylon = enemyInstance.GetComponent<EnemyPylon>();
                    Pylon.EnemyCounter = EnemyCounter;

                    EnemyBase enemy = enemyInstance.GetComponent<EnemyBase>();
                    if (!enemy)
                    {
                        enemy = enemyInstance.GetComponentInChildren<EnemyBase>();
                    }

                    enemy.SetPlayer(GameManager.GetInstance().Player.gameObject);
                }

            }
        }
    }
}
