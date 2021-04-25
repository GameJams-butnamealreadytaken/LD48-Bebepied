using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelLogic : MonoBehaviour
{
    public List<GameObject> TopBrokenFloorObjects;
    
    public GameObject PlayerSpawn;

    private Animator Animator;

    private bool bIsMapEnded;
    private bool bEndAnimIsTriggered;

    public List<TMP_Text> CurrentLevelTexts;

    private void Start()
    {
        bIsMapEnded = false;
        bEndAnimIsTriggered = false;
        Animator = GetComponent<Animator>();
    }

    public void UpdateCurrentLevel(int currentLevel)
    {
        for (int i = 0; i < CurrentLevelTexts.Count; ++i)
        {
            CurrentLevelTexts[i].text = "" + currentLevel;
        }
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
        if (bIsMapEnded && !bEndAnimIsTriggered)
        {
            bEndAnimIsTriggered = true;

            Animator.SetTrigger("ChainHit");
            
            //
            // Repaly the music
            GameManager.GetInstance().StartMusic();
        }
    }
}
