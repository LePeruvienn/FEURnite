using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Bar : MonoBehaviour
{
    [Header("Components")]
    public RawImage imgLevelBar;// image du niveau de la barre de vie
    public RectTransform imgOffsetLevelBar;// image de l'annimation de la barre de vie(demander a Maxence si pas claire)
    public TextMeshProUGUI txtBar;//le texte du niveau de vie actuelle du perso
    public TextMeshProUGUI txtMaxBar;//le texte du niveau de vie max du perso

    // permet de transformer la barre de vie
    RectTransform _BarTransform;

    // Longeur maximal de la barre de vie (pixels)
    float _barWidthMax;
    // hauteur maximal de la barre de vie (pixels)
    float _barHeightMax;

    Coroutine _coroutineBarSet;

    void Awake()
    {
        // set transform de la barre de vie
        _BarTransform = imgLevelBar.GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Assigner les mesure de la barre de vie maximale
        Vector2 dimensions = imgLevelBar.transform.parent.GetComponent<RectTransform>().sizeDelta;
        _barWidthMax = dimensions.x;
        _barHeightMax = dimensions.y;
        //Appliquer cette largeur à la barre de vie
        _BarTransform.sizeDelta = new Vector2(_barWidthMax, _barHeightMax);
    }

    public void SetBar(float health, float maxHealth)
    {
        //Calculer le pourcentage de vie 
        float healthPercent = health / maxHealth;
        if (health < 0) 
        { 
            health = 0;
        }
        //Modifier les valeur des chmp de texte
        txtBar.text = string.Format("{0:#0}", health);
        txtMaxBar.text = string.Format("/ {0:#0}", maxHealth);
        //Appliquer la valeur de vie dans la barre de vie
        _BarTransform.sizeDelta = new Vector2(healthPercent * _barWidthMax, _barHeightMax);
        //Demare la coroutine de offset si elle n'existe pas déja
        if (_coroutineBarSet != null)
        {
            StopCoroutine(_coroutineBarSet);
        }
        _coroutineBarSet = StartCoroutine(SetOffset(healthPercent));
    }

    //fonction qui gere la Barre parralele qui permet de voir combien on a perdu de vie il y a qu'elle que seconde
    IEnumerator SetOffset(float targetPercent)
    {
        //Délai de quelques millisecondes
        yield return new WaitForSeconds(0.75f);
        // cible du with offset
        float targetWidth = _barWidthMax * targetPercent;
        //largeur actuelle de la barre
        float currentWidth = imgOffsetLevelBar.sizeDelta.x;
        //Variable de sizeDelta
        Vector2 sizeDelta = imgOffsetLevelBar.sizeDelta;
        while (currentWidth > targetWidth)
        {
            //calculer la nouvelle largeur
            sizeDelta.x = Mathf.Lerp(currentWidth, targetWidth, Time.deltaTime * 10f);
            //Modifier la valeur actuelle
            currentWidth = sizeDelta.x;
            //Afficher la nouvelle valeur
            imgOffsetLevelBar.sizeDelta = sizeDelta;
            //Attendre la prochaine frame
            yield return null;
        }
        sizeDelta.x = targetWidth;
        imgOffsetLevelBar.sizeDelta = sizeDelta;
    }
}