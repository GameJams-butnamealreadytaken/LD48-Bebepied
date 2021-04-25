using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIScoreScreen : MonoBehaviour
{
	
	[SerializeField] private Image m_backgroundImage;
	
	[SerializeField] private TMP_Text m_ennemiesKilledScoreText;
	[SerializeField] private TMP_Text m_bulletsShotScoreText;
	[SerializeField] private TMP_Text m_sausagesShotScoreText;

	private void Awake()
	{
		m_backgroundImage.gameObject.SetActive(false);
	}

	public void Show(int enemyKilled, int bulletsShot, int sausagesShot)
	{
		//
		// Stop the in game music
		GameManager.GetInstance().StopMusic();
		
		//
		// Unlock the cursor
		Cursor.lockState = CursorLockMode.None;
		
		//
		// Set the data
		m_ennemiesKilledScoreText.text = "" + enemyKilled;
		m_bulletsShotScoreText.text = "" + bulletsShot;
		m_sausagesShotScoreText.text = "" + sausagesShot;
		
		//
		// Show the background image (and texts because they are children)
		m_backgroundImage.gameObject.SetActive(true);
	}

	public void Retry()
	{
		GameManager.GetInstance().Player.ResetStats();
		SceneManager.LoadScene("LevelBase");
		m_backgroundImage.gameObject.SetActive(false);
		GameManager.GetInstance().Player.Activate();	//< Re-activate the player
		
		//
		// Replay the music
		GameManager.GetInstance().StartMusic();
	}

	public void GoToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}

}
