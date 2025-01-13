using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Starter.ThirdPersonCharacter
{
	public class Countdown : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI timerText;
		[SerializeField] private float startValue;

		private float remainingTime;
		private bool isPaused = true;

		void Start()
		{
			InitializeTimer(startValue);
		}

		void Update()
		{
			if (!isPaused && remainingTime > 0)
			{
				remainingTime -= Time.deltaTime;
				UpdateTimerText();
			}
			else if (remainingTime <= 0 && timerText.color != Color.red)
			{
				remainingTime = 0;
				timerText.color = Color.red;
				UpdateTimerText();
			}
		}

		public void InitializeTimer(float timeInSeconds)
		{
			remainingTime = timeInSeconds;
			UpdateTimerText();
		}

		public void PauseTimer()
		{
			isPaused = true;
		}

		public void ResumeTimer()
		{
			isPaused = false;
		}

		private void UpdateTimerText()
		{
			int minutes = Mathf.FloorToInt(remainingTime / 60);
			int seconds = Mathf.FloorToInt(remainingTime % 60);
			timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
		}
	}
}
