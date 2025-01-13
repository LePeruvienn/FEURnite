using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Annonceur : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI announceText;
    private float showingTime;
    private float remainingShowingTime;
    private bool startShowingTimeCountdown = false;

    void Start()
    {
        Clear();
        announceText.color = Color.red;
    }

    void Update()
    {
        if (startShowingTimeCountdown)
        {
            remainingShowingTime -= Time.deltaTime;
            if (remainingShowingTime <= 0)
            {
                remainingShowingTime = showingTime;
                startShowingTimeCountdown = false;
                Clear();
            }
        }
        
    }

    public void Init(int showingTime)
    {
        this.showingTime = showingTime;
        remainingShowingTime = showingTime;
    }

    public void Annonce(string message)
    {
        announceText.text = message;
        startShowingTimeCountdown = true;
    }

    public void Clear()
    {
        announceText.text = "";
    }
}
