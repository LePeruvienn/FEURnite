using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ResolutionChanger : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    // Liste de r�solutions � proposer dans le Dropdown
    private Resolution[] resolutions;

    void Start()
    {
        // R�cup�re toutes les r�solutions disponibles sur l'�cran de l'utilisateur
        resolutions = Screen.resolutions;

        // Efface les options actuelles et les remplit avec les r�solutions disponibles
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
        }

        // Ajoute les options de r�solutions au Dropdown
        resolutionDropdown.AddOptions(options);

        // Abonne la fonction ChangeResolution � l'�v�nement de changement de valeur du Dropdown
        resolutionDropdown.onValueChanged.AddListener(ChangeResolution);

        // Initialiser la s�lection � la r�solution actuelle de l'�cran
        resolutionDropdown.value = GetCurrentResolutionIndex();
    }

    // Cette fonction change la r�solution en fonction de l'option s�lectionn�e dans le Dropdown
    private void ChangeResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    // Trouve l'index de la r�solution actuelle dans le tableau de r�solutions
    private int GetCurrentResolutionIndex()
    {
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (Screen.currentResolution.width == resolutions[i].width && Screen.currentResolution.height == resolutions[i].height)
            {
                return i;
            }
        }
        return 0; // Si la r�solution actuelle n'est pas trouv�e, retourne le premier �l�ment
    }
}