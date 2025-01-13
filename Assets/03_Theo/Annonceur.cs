using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Annonceur : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI announceText;
    private float showingTime;

    void Start()
    {
        
    }

    void Update()
    {
        showingTime -= Time.deltaTime;
        if (showingTime <= 0)
        {
            Clear();
        }
    }

    public void Init(int showingTime)
    {
        this.showingTime = showingTime;
    }

    public void Annonce(string message)
    {
        announceText.text = message;
    }

    public void Clear()
    {
        announceText.text = "";
    }
}
