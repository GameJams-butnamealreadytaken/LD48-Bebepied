using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIInGamePause : MonoBehaviour
{

	public Selectable m_firstSelectedItem;
	private bool m_musicWasPlaying = false;
	
	private void Start()
	{
		gameObject.SetActive(false);
	}

	public void Show()
	{
		Cursor.lockState = CursorLockMode.None;
		if (GameManager.GetInstance().IsMusicPlaying())
		{
			m_musicWasPlaying = true;
		}
		GameManager.GetInstance().PauseMusic();
		Time.timeScale = 0f;
		gameObject.SetActive(true);
		
		
		if (GetComponentInParent<PlayerInput>().currentControlScheme == "Gamepad")
		{
			EventSystem.current.SetSelectedGameObject(m_firstSelectedItem.gameObject);
			
			//
			// We explicitly force selection
			m_firstSelectedItem.Select();
			m_firstSelectedItem.OnSelect(null);
		}
	}

	public void Hide()
	{
		Time.timeScale = 1f;
		gameObject.SetActive(false);
	}

	private void Update()
	{
		if (GetComponentInParent<PlayerInput>().actions["Pause"].triggered)
		{
			Continue();
		}
	}
	
	public void GoToMainMenu()
	{
		//
		// Relock the cursor
		Cursor.lockState = CursorLockMode.Locked;
		
		//
		//
		Hide();
		
		//
		//
		GameManager.GetInstance().Player.ResetWave(true);
		GameManager.GetInstance().StopMusic();
		SceneManager.LoadScene("MainMenu");
		GameManager.GetInstance().Player.Activate();	//< Re-activate the player
	}

	public void Exit()
	{
		//
		// Unlock
		Cursor.lockState = CursorLockMode.None;
		Application.Quit();
	}

	public void Continue()
	{
		//
		// Relock the cursor
		Cursor.lockState = CursorLockMode.Locked;
		if (m_musicWasPlaying)
		{
			GameManager.GetInstance().StartMusic();
		}
		Hide();
	}
}
