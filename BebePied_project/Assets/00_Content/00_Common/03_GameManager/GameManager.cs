using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string FirstSceneToLoad;
    
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
        SceneManager.LoadScene(FirstSceneToLoad);
    }

    // Update is called once per frame
    void Update()
    {
    }

}
