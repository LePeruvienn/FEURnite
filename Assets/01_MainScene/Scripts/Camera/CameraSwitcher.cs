using Starter.ThirdPersonCharacter;
using UnityEngine;

public sealed class CameraSwitcher : MonoBehaviour
{
    public GameObject playerCamera; // Cam�ra li�e au joueur
    public GameObject freecamCamera; // Cam�ra libre
    //private bool isFreecamActive = false;

    public void ToggleFreecam(bool isFreecamActive) { 

        // Activer/D�sactiver les cam�ras
        playerCamera.SetActive(!isFreecamActive);
        freecamCamera.SetActive(isFreecamActive);

        Debug.Log(isFreecamActive ? "Cam�ra libre activ�e" : "Cam�ra libre d�sactiv�e");
    }
}