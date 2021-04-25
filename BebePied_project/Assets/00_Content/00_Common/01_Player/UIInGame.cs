using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInGame : MonoBehaviour
{

	[SerializeField] private TMP_Text m_messageText;
	[SerializeField] private TMP_Text m_durationText;

	private void Awake()
	{
		m_messageText.text = "";
		m_durationText.text = "";
	}

	public void StartBonus(string bonusMessage, int bonusDuration)
	{
		StopAllCoroutines();
		m_messageText.text = bonusMessage;
		m_durationText.text = "" + bonusDuration;
		StartCoroutine(AutomaticRemoveMessage(bonusDuration));
	}

	public void ResetBonus()
	{
		StopAllCoroutines();
		m_messageText.text = "";
		m_durationText.text = "";
	}
	
	public IEnumerator AutomaticRemoveMessage(float duration)
	{
		float fullDuration = duration;
		while (duration > 0)
		{
			m_durationText.text = "" + duration;
			duration -= Time.deltaTime;

			if (duration <= fullDuration / 4f)
			{
				m_messageText.text = "";
			}

			yield return null;
		}
 
		//
		// Reapply the start intensity
		m_messageText.text = "";
		m_durationText.text = "";
	}
}
