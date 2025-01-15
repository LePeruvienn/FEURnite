using Starter.ThirdPersonCharacter;
using UnityEngine;

public sealed class CameraSwitcher : MonoBehaviour
{
    public GameObject playerCamera; // Caméra liée au joueur
    public GameObject freecamCamera; // Caméra libre

    public void ToggleFreecam(bool isFreecamActive) { 

        // Activer/Désactiver les caméras
        playerCamera.SetActive(!isFreecamActive);
        freecamCamera.SetActive(isFreecamActive);
    }
}
