using Starter.ThirdPersonCharacter;
using UnityEngine;

public sealed class CameraSwitcher : MonoBehaviour
{
    public GameObject playerCamera; // Caméra liée au joueur
    public GameObject freecamCamera; // Caméra libre
    private bool isFreecamActive = false;

    public void ToggleFreecam()
    {
        isFreecamActive = !isFreecamActive;

        // Activer/Désactiver les caméras
        playerCamera.SetActive(!isFreecamActive);
        freecamCamera.SetActive(isFreecamActive);
    }
}
