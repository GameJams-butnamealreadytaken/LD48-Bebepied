using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLogic : MonoBehaviour
{
    public List<GameObject> TopBrokenFloorObjects;
    
    public GameObject PlayerSpawn;

    private Animator Animator;

    private bool bIsMapEnded;

    private void Start()
    {
        bIsMapEnded = false;
        Animator = GetComponent<Animator>();
    }

    public void HideUpsideFloor()
    {
        for (int i = 0; i < TopBrokenFloorObjects.Count; ++i)
        {
            TopBrokenFloorObjects[i].SetActive(false);
        }
    }

    public void TriggerEndLevel()
    {
        Animator.SetTrigger("MapEnd");
        bIsMapEnded = true;
    }

    public void TriggerEndChainCollision()
    {
        if (bIsMapEnded)
        {
            Animator.SetTrigger("ChainHit");
        }
    }
}
