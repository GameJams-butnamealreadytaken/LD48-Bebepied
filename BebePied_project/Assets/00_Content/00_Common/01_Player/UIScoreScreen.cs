using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIScoreScreen : MonoBehaviour
{
	
	[SerializeField] private Image m_backgroundImage;
	
	[SerializeField] private TMP_Text m_reachedLevelText;
	[SerializeField] private TMP_Text m_ennemiesKilledScoreText;
	[SerializeField] private TMP_Text m_bulletsShotScoreText;
	[SerializeField] private TMP_Text m_sausagesShotScoreText;

	[SerializeField] private Selectable m_firstSelected;

	private void Awake()
	{
		m_backgroundImage.gameObject.SetActive(false);
	}

	public void Show(int reachedLevel, int enemyKilled, int bulletsShot, int sausagesShot)
	{
		//
		//
		if (m_firstSelected)
		{
			EventSystem.current.SetSelectedGameObject(m_firstSelected.gameObject);
			
			//
			// We explicitly force selection
			m_firstSelected.Select();
			m_firstSelected.OnSelect(null);
		}
		
		//
		// Stop the in game music
		GameManager.GetInstance().StopMusic();
		
		//
		// Unlock the cursor
		Cursor.lockState = CursorLockMode.None;

		//
		// Set the data
		m_reachedLevelText.text = "" + reachedLevel;
		m_ennemiesKilledScoreText.text = "" + enemyKilled;
		m_bulletsShotScoreText.text = "" + bulletsShot;
		m_sausagesShotScoreText.text = "" + sausagesShot;
		
		//
		// Show the background image (and texts because they are children)
		m_backgroundImage.gameObject.SetActive(true);
	}

	public void Retry()
	{
		//
		// Relock the cursor
		Cursor.lockState = CursorLockMode.Locked;
		
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
		GameManager.GetInstance().Player.Activate();	//< Re-activate the player
		m_backgroundImage.gameObject.SetActive(false);
	}

}
