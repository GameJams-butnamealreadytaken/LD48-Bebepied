using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLogic : MonoBehaviour
{
    private Animator Animator;

    private bool bIsMapEnded;

    private void Start()
    {
        bIsMapEnded = false;
        Animator = GetComponent<Animator>();
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
            Debug.Log("chain hit trigger");
            Animator.SetTrigger("ChainHit");
        }
    }
}
