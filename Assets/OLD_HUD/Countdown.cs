using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Countdown : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    private float remainingTime;
    private bool isPaused = false;

    void Start()
    {
        InitializeTimer(2f);
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
        isPaused = false;
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
