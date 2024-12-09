using Starter.ThirdPersonCharacter;
using UnityEngine;

public sealed class CameraSwitcher : MonoBehaviour
{
    public GameObject playerCamera; // Cam�ra li�e au joueur
    public GameObject freecamCamera; // Cam�ra libre
    private bool isFreecamActive = false;

    private Player playerController;
    

    private void Start()
    {
        playerController = FindObjectOfType<Player>();
        if (playerController == null)
        {
            Debug.LogError("Player not found in scene");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // Appuie sur F pour switcher
        {
            ToggleFreecam();
        }

        //if (playerController.DebugIsDead == false)
        //{
        //    ToggleFreecam();
        //}
    }

    public void ToggleFreecam()
    {
        isFreecamActive = !isFreecamActive;

        // Activer/D�sactiver les cam�ras
        playerCamera.SetActive(!isFreecamActive);
        freecamCamera.SetActive(isFreecamActive);
    }
}
