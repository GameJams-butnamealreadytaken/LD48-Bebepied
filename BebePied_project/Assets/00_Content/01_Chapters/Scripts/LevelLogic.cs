using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelLogic : MonoBehaviour
{
    public List<GameObject> TopBrokenFloorObjects;
    
    public GameObject PlayerSpawn;

    private Animator Animator;

    public bool bIsMapEnded;
    public bool bEndAnimIsTriggered;

    public List<TMP_Text> CurrentLevelTexts;

    private void Start()
    {
        bIsMapEnded = false;
        bEndAnimIsTriggered = false;
        Animator = GetComponent<Animator>();

        UpdateCurrentLevel(GameManager.GetInstance().Player.GetCurrentWave() - 1);
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
            // Replay the music
            GameManager.GetInstance().StartMusic();
        }
    }

    public void TriggerEndChainExplosion()
    {
        GameManager.GetInstance().TutoTextGameObject.enabled = false;
        GameManager.GetInstance().TutoTitleTextGameObject.enabled = false;
        GameManager.GetInstance().Player.GetComponentInChildren<UIInGame>().ResetBonus();


        GetComponentInChildren<ChainLogic>().DestroyEffect();
    }
}
