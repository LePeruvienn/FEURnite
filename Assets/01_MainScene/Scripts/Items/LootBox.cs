using Starter.ThirdPersonCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Fusion;


public class LootBox : NetworkBehaviour
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

    public GameObject[] ListWeapon; // Liste des armes
    public GameObject[] ListItem;   // Liste des items
    private NetworkObject spawnedWeapon; // Référence pour l'arme spawnée

    private Transform _spawnItemPosition;
   

    [Networked] private bool IsOpen { get; set; }

    public void Open()
    {
        Debug.Log("Attempting to open loot box...");

        if (status == Status.IsOpen)
        {
            Debug.Log("Loot box is already open.");
            return; // Si le coffre est déjà ouvert, on ne fait rien
        }

        if (Object.HasStateAuthority)
        {
            // Si ce client a l'autorité, ouvre le coffre
            Debug.Log("StateAuthority confirmed. Opening the box...");
            RPC_SetStatus(Status.IsOpen);
        }
        else
        {
            // Tous les clients peuvent exécuter ce RPC
            Debug.Log("Requesting to open loot box from any client.");
            RPC_SetStatus(Status.IsOpen);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SetStatus(Status newStatus)
    {
        Debug.Log($"Setting loot box status to {newStatus}...");
        status = newStatus;

        if (newStatus == Status.IsOpen)
        {
            openChestAnimation();
            ChoiceBoxType(); // Ouvre le bon type de coffre
        }
    }

    private ItemRarity GetItemDrop()
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

        if (listObjectRecuperer.Count == 0)
        {
            
            return null;
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

        if (listObjectRecuperer.Count == 0)
        {
            
            return null;
        }

        int randomIndex = Random.Range(0, listObjectRecuperer.Count);
        return listObjectRecuperer[randomIndex];
    }

    private void ChoiceBoxType()
    {
        Debug.Log($"Box type: {type}");
        if (type == BoxType.WeaponBox)
            OpenWeaponBox();
        else
            OpenItemBox();
    }

    private void OpenItemBox()
    {
        if (HasStateAuthority)
        {
            // Appel du RPC pour ouvrir le coffre des items pour tous les clients
            RPC_OpenItemBox();
        }
        
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_OpenItemBox()
    {
       

        Debug.Log("Opening item box...");
        GameObject item = GetItemFromList();

        if (item != null)
        {
            if (_spawnItemPosition == null)
            {
                _spawnItemPosition = transform.Find("spawnObjectPos").transform;
            }

            // Spawn de l'item sur tous les clients sans autorité spécifique
            NetworkObject netObj = item.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                Runner.Spawn(item, _spawnItemPosition.position, Quaternion.identity, null);
                Debug.Log("Item spawned successfully on all clients with no specific authority.");
            }
        }
    }

    // Correction du RPC pour les armes
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_OpenWeaponBox()
    {
       

        Debug.Log("Opening weapon box");
        GameObject weapon = GetWeaponFromList();

        if (weapon != null)
        {
            Item item = weapon.GetComponent<Item>();

            if (item != null)
            {
                if (_spawnItemPosition == null)
                    _spawnItemPosition = transform.Find("spawnObjectPos").transform;

                // Spawn l'objet sur tous les clients sans donner d'autorité spécifique
                NetworkObject netObj = weapon.GetComponent<NetworkObject>();
                if (netObj != null)
                {
                    spawnedWeapon = Runner.Spawn(weapon, _spawnItemPosition.position, Quaternion.identity, null);
                    Debug.Log("Weapon spawned successfully on all clients with no specific authority.");
                }
            }
        }
    }



    


    private void OpenWeaponBox()
    {
        if (HasStateAuthority)
        {
            // Appel du RPC pour ouvrir le coffre pour tous les clients
            RPC_OpenWeaponBox();
        }
        else
        {
            Debug.LogWarning("Only the player with state authority can open the weapon box.");
        }
    }

    private void openChestAnimation()
    {
        Debug.Log("Playing open chest animation...");
        if (Animator.GetBool("isOpen") == false)
        {
            Animator.SetBool("isOpen", true);
        }
    }
}
