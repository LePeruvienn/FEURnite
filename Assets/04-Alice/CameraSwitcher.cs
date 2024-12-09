using Starter.ThirdPersonCharacter;
using UnityEngine;

public sealed class CameraSwitcher : MonoBehaviour
{
    public GameObject playerCamera; // Cam�ra li�e au joueur
    public GameObject freecamCamera; // Cam�ra libre
    private bool isFreecamActive = false;

    public void ToggleFreecam()
    {
        isFreecamActive = !isFreecamActive;

        // Activer/D�sactiver les cam�ras
        playerCamera.SetActive(!isFreecamActive);
        freecamCamera.SetActive(isFreecamActive);
    }
}
