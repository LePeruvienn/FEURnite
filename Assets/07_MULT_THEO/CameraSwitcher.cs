using Starter.ThirdPersonCharacter;
using UnityEngine;

public sealed class CameraSwitcher : MonoBehaviour
{
    public GameObject playerCamera; // Caméra liée au joueur
    public GameObject freecamCamera; // Caméra libre
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

        // Activer/Désactiver les caméras
        playerCamera.SetActive(!isFreecamActive);
        freecamCamera.SetActive(isFreecamActive);
    }
}
