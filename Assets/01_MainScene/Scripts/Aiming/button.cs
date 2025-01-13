using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button : MonoBehaviour
{
    public GameObject buttonStart;
    private bool _isDisplayed;
    // Start is called before the first frame update
    void Start()
    {
        buttonStart.SetActive(true);
        _isDisplayed = true;
    }

    // Update is called once per frame
    void Update()
    {
        _isDisplayed = buttonStart.activeSelf;
        if (_isDisplayed)
        {
            // Rendre le curseur visible, mais garder les inputs actifs
            Cursor.lockState = CursorLockMode.Confined;  // Confiner le curseur dans la fenêtre
            Cursor.visible = true;
        }
        else
        {
            // Cacher le curseur et verrouiller comme avant
            Cursor.lockState = CursorLockMode.Locked;  // Verrouiller le curseur au centre de l'écran
            Cursor.visible = false;
        }
    }
}
