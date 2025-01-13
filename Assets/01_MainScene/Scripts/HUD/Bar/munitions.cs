using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class munitions : MonoBehaviour {

    private GameObject nbMunArmeObj;
    private GameObject nbMunTotalObj;

    private TextMeshProUGUI nbMunArmeTexte;
    private TextMeshProUGUI nbMunTotalTexte;

    // Start is called before the first frame update
    void Start()
    {
        nbMunArmeObj = GameObject.FindGameObjectWithTag("nbMunArme");
        nbMunTotalObj = GameObject.FindGameObjectWithTag("nbMunTotal");

        nbMunArmeTexte = nbMunArmeObj.GetComponent<TextMeshProUGUI>();
        nbMunTotalTexte = nbMunTotalObj.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setMunitions(int nbMunArme, int nbMunTotal)
    {
        nbMunArmeTexte.text = string.Format("{0:#0}", nbMunArme);
        nbMunTotalTexte.text = string.Format("/ {0:#0}", nbMunTotal);
    }
}
