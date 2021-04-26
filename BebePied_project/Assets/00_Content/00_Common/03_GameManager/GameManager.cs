using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string FirstSceneToLoad;
    public Player Player;
    public TMP_Text TutoTextGameObject;
    public AudioSource m_musicAudioSource;
    public BonusManager m_bonusManager;
    public LevelBase CurrentLevel; 

    private static GameManager instance;
    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (FirstSceneToLoad.Length > 0)
        {
            SceneManager.LoadScene(FirstSceneToLoad);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);   
    }

    public void StartMusic()
    {
        if (!m_musicAudioSource.isPlaying)
            m_musicAudioSource.Play();
    }

    public void PauseMusic()
    {
        m_musicAudioSource.Pause();
    }

    public void StopMusic()
    {
        m_musicAudioSource.Stop();
    }
}
