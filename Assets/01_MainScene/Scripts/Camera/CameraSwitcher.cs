using Starter.ThirdPersonCharacter;
using UnityEngine;

public sealed class CameraSwitcher : MonoBehaviour
{
    public GameObject playerCamera; // Cam�ra li�e au joueur
    public GameObject freecamCamera; // Cam�ra libre

    public void ToggleFreecam(bool isFreecamActive) { 

        // Activer/D�sactiver les cam�ras
        playerCamera.SetActive(!isFreecamActive);
        freecamCamera.SetActive(isFreecamActive);
    }
}
