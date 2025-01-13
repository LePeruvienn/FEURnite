using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Starter.ThirdPersonCharacter
{
	public class Countdown : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI timerText;
		[SerializeField] private float startValue;

        private float remainingTime;
		private bool isPaused = true;
		//private bool betweenIslandFall = false;
        private bool spawnIslandFallen = false;
		private bool intermediateIslandFallen = false;

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

            timerText.color = remainingTime <= 15 ?
                    Color.red : Color.white;

            if (isPaused)
            {
                timerText.color = Color.yellow;
                timerText.text = "Waiting for the game to start...";
            }
			else if (!spawnIslandFallen)
			{
				timerText.text = string.Format("Time before the spawn islands fall : \n {0:00}:{1:00}", minutes, seconds);
			}
			else if (spawnIslandFallen && !intermediateIslandFallen)
			{
                timerText.text = string.Format("Time before the intermediate islands fall : \n {0:00}:{1:00}", minutes, seconds);
            }
			else if (intermediateIslandFallen)
			{
				timerText.text = "";
			}
		}

		public void spawnIslandFell()
		{
			spawnIslandFallen = true;
		}

		public void intermediateIslandFell()
		{
			intermediateIslandFallen = true;
		}

    }
}
