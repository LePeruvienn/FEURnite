using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour
{
    [Header("Components")]
    public Image imgLevelSuperShield;// image du niveau du bouclier
    public Image imgLevelOffsetSuperShield;// image du niveau du bouclier
    public Image imgLevelDown;
    public TextMeshProUGUI txtSuperShield;//le texte du niveau de vie actuelle du perso

    Coroutine _coroutineBarSet;
    
    public void SetSuperShield(float health, float maxHealth)
    {
        
        //Calculer le pourcentage de vie 
        float healthPercent = health / maxHealth;
        if (health <= 0) { 
            health = 0;
            imgLevelDown.gameObject.SetActive(true);
            imgLevelOffsetSuperShield.gameObject.SetActive(false);
            imgLevelSuperShield.gameObject.SetActive(false);
            //Modifier les valeur des chmp de texte
            txtSuperShield.text = string.Format("{0:#0}", health);
        }
        else if (health<= maxHealth)
        {
            imgLevelDown.gameObject.SetActive(false);
            imgLevelOffsetSuperShield.gameObject.SetActive(true);
            imgLevelSuperShield.gameObject.SetActive(true);
            //Appliquer la valeur de vie dans la barre de vie
            imgLevelSuperShield.fillAmount = healthPercent;
            if (_coroutineBarSet != null)
            {
                StopCoroutine(_coroutineBarSet);
            }
            _coroutineBarSet = StartCoroutine(SetOffset(healthPercent));
            //Modifier les valeur des chmp de texte
            txtSuperShield.text = string.Format("{0:#0}", health);
        }
    }

    //fonction qui gere la Barre parralele qui permet de voir combien on a perdu de vie il y a qu'elle que seconde
    IEnumerator SetOffset(float targetPercent)
    {
        //Délai de quelques millisecondes
        yield return new WaitForSeconds(0.20f);
        
        //Variable de sizeDelta
        float fillAmount = imgLevelOffsetSuperShield.fillAmount;

        while (fillAmount > targetPercent)
        {
            //calculer la nouvelle largeur
            fillAmount = Mathf.Lerp(fillAmount,targetPercent, Time.deltaTime * 10f);

            //Afficher la nouvelle valeur
            imgLevelOffsetSuperShield.fillAmount = fillAmount;

            //Attendre la prochaine frame
            yield return null;
        }
        fillAmount = targetPercent;
        imgLevelOffsetSuperShield.fillAmount = fillAmount;
    }
}