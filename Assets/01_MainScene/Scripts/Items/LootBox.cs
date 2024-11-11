using Starter.ThirdPersonCharacter;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Search;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;
using static LootBox;

public class LootBox : MonoBehaviour
{

    
    public enum BoxRarity
    {
        Common = 1,
        Rare = 2,
        Epic = 3,
        Legendary = 4
    }
    public enum Status
    {
        IsOpen = 1,
        IsClose = 0
    }
    public enum BoxType
    {
        WeaponBox = 1,
        ItemBox = 2
    }


    public BoxRarity boxRarity;
    public Status status;
    public BoxType type;
    public Animator Animator;

    public GameObject[] ListWeapon; // liste des armes Commun
    public GameObject[] ListItem; // liste items hors armes

    private Transform _spawnItemPosition;
    public void Open()
    {
        // Vérifie si le coffre est déjà ouvert
        if (status == Status.IsOpen)
            return; // Retourne si le coffre est ouvert

        ChoiceBoxType();

    }


    private ItemRarity GetItemDrop() // Retourne la rareté d'un item en utilisant les probabilités
    {
        int randomValue = Random.Range(0, 100);

        switch (boxRarity)
        {
            case BoxRarity.Common:
                return GetItemFromProbabilities(randomValue, 85, 12, 3, 0);
            case BoxRarity.Rare:
                return GetItemFromProbabilities(randomValue, 60, 25, 10, 5);
            case BoxRarity.Epic:
                return GetItemFromProbabilities(randomValue, 40, 30, 20, 10);
            case BoxRarity.Legendary:
                return GetItemFromProbabilities(randomValue, 30, 20, 20, 30);
            default:
                return ItemRarity.Common;
        }
    }

    private ItemRarity GetItemFromProbabilities(int randomValue, int commonProb, int rareProb, int epicProb, int legendaryProb)
    {
        if (randomValue < commonProb)
            return ItemRarity.Common;
        else if (randomValue < commonProb + rareProb)
            return ItemRarity.Rare;
        else if (randomValue < commonProb + rareProb + epicProb)
            return ItemRarity.Epic;
        else
            return ItemRarity.Legendary;
    }

    private GameObject GetWeaponFromList()
    {
        ItemRarity itemRarity = GetItemDrop();
        List<GameObject> listObjectRecuperer = new List<GameObject>();

        foreach (GameObject wp in ListWeapon)
        {
            Item weapon = wp.GetComponent<Item>();
            if (weapon.GetRarity() == itemRarity)
            {
                listObjectRecuperer.Add(wp);
            }
        }
        int randomIndex = Random.Range(0, listObjectRecuperer.Count);
        return listObjectRecuperer[randomIndex];
    }

    private GameObject GetItemFromList()
    {
        ItemRarity itemRarity = GetItemDrop();
        List<GameObject> listObjectRecuperer = new List<GameObject>();

        foreach (GameObject wp in ListItem)
        {
            Item item = wp.GetComponent<Item>();
            if (item.GetRarity() == itemRarity)
            {
                listObjectRecuperer.Add(wp);
            }
        }
        int randomIndex = Random.Range(0, listObjectRecuperer.Count);
        return listObjectRecuperer[randomIndex];
    }

    private void ChoiceBoxType()
    {
        if (type == BoxType.WeaponBox)
            OpenWeaponBox();
        
        else
            OpenItemBox();
        
    }
    private void OpenItemBox()
    {
        GameObject item = GetItemFromList();
        if (item != null)
        {
            if (_spawnItemPosition == null)
            {
                _spawnItemPosition = transform.Find("spawnObjectPos").transform;

                StartCoroutine(SpawnWeapon(item, _spawnItemPosition.position, Quaternion.identity));
                openChestAnimation();
                Debug.Log("Ouverture du coffre");

                status = Status.IsOpen; // Change le statut du coffre
            }
        }
    }
    private void OpenWeaponBox()
    {
        GameObject weapon = GetWeaponFromList(); // Récupère l'arme aléatoirement choisis 

        if (weapon != null) // Vérifie si weapon n'est pas null
        {
            Item item = weapon.GetComponent<Item>(); // Tente de récupérer le composant Item

            if (item != null) // Vérifie si le GameObject a bien un composant Item
            {
                // Utilisation de la méthode publique d'Item
                print(item.GetRarity()); // Exemple de méthode dans la classe Item

                // Assure que _spawnItemPosition est initialisé
                if (_spawnItemPosition == null)
                    _spawnItemPosition = transform.Find("spawnObjectPos").transform;

                StartCoroutine(SpawnWeapon(weapon, _spawnItemPosition.position, Quaternion.identity));
                openChestAnimation();
                Debug.Log("Ouverture du coffre");

                // Change le statut du coffre
                status = Status.IsOpen;
            }
            else
            {
                Debug.LogError("weapon n'est pas une composante de item.");
            }
        }
        else
        {
            Debug.LogError("GetItemFromList() a retourné null. ( weapon = null )");
        }
    }

    private IEnumerator SpawnWeapon(GameObject item, Vector3 spawnItemPosition, Quaternion identity)
    {
        yield return new WaitForSeconds(1);
        Instantiate(item, spawnItemPosition, identity);
    }

    private void openChestAnimation()
    {
        if (Animator.GetBool("isOpen") == false)
        {
            Animator.SetBool("isOpen", true); // ouvre le chest

            /*if (GlowTransform == null)
            {
                GlowTransform = transform; // or assign another Transform here if needed
            }

            GlowParticule.transform.position = new Vector3(
                0,
                transform.position.y + 0.60f,
                0
            );
            ParticleSystem Glow = Instantiate(GlowParticule, GlowTransform);*/
        }
    }

    

}


