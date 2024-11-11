using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ResolutionChanger : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    // Liste de résolutions à proposer dans le Dropdown
    private Resolution[] resolutions;

    void Start()
    {
        // Récupère toutes les résolutions disponibles sur l'écran de l'utilisateur
        resolutions = Screen.resolutions;

        // Efface les options actuelles et les remplit avec les résolutions disponibles
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
        }

        // Ajoute les options de résolutions au Dropdown
        resolutionDropdown.AddOptions(options);

        // Abonne la fonction ChangeResolution à l'événement de changement de valeur du Dropdown
        resolutionDropdown.onValueChanged.AddListener(ChangeResolution);

        // Initialiser la sélection à la résolution actuelle de l'écran
        resolutionDropdown.value = GetCurrentResolutionIndex();
    }

    // Cette fonction change la résolution en fonction de l'option sélectionnée dans le Dropdown
    private void ChangeResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    // Trouve l'index de la résolution actuelle dans le tableau de résolutions
    private int GetCurrentResolutionIndex()
    {
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (Screen.currentResolution.width == resolutions[i].width && Screen.currentResolution.height == resolutions[i].height)
            {
                return i;
            }
        }
        return 0; // Si la résolution actuelle n'est pas trouvée, retourne le premier élément
    }
}