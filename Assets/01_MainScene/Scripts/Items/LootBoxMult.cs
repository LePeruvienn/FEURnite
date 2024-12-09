
using Starter.ThirdPersonCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class LootBoxMult : NetworkBehaviour
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

    [Header("LootBox Properties")]
    public BoxRarity boxRarity;

    [Networked]
    public Status status { get; set; }

    private Status _previousStatus;

    public BoxType type;
    public Animator animator;
    public GameObject[] listWeapon;
    public GameObject[] listItem;

    private Transform _spawnItemPosition;

    public override void Spawned()
    {
        if (_spawnItemPosition == null)
        {
            _spawnItemPosition = transform.Find("spawnObjectPos").transform;
        }
    }

    public void TryOpen()
    {
        if (status == Status.IsOpen)
            return;

        if (Object.HasStateAuthority)
        {
            status = Status.IsOpen;
            HandleStatusChange();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (_previousStatus != status)
        {
            _previousStatus = status;
            HandleStatusChange();
        }
    }

    private void HandleStatusChange()
    {
        if (status == Status.IsOpen)
        {
            PlayOpenAnimation();
            ChoiceBoxType();
        }
    }

    private void ChoiceBoxType()
    {
        if (type == BoxType.WeaponBox)
            OpenWeaponBox();
        else
            OpenItemBox();
    }

    private void OpenWeaponBox()
    {
        GameObject weapon = GetWeaponFromList();
        if (weapon != null)
        {
            SpawnItem(weapon);
        }
    }

    private void OpenItemBox()
    {
        GameObject item = GetItemFromList();
        if (item != null)
        {
            SpawnItem(item);
        }
    }

    private void SpawnItem(GameObject item)
    {
        if (_spawnItemPosition != null && Object.HasStateAuthority)
        {
            Runner.Spawn(item, _spawnItemPosition.position, Quaternion.identity);
        }
    }

    private GameObject GetWeaponFromList()
    {
        return GetItemFromListByRarity(listWeapon);
    }

    private GameObject GetItemFromList()
    {
        return GetItemFromListByRarity(listItem);
    }

    private GameObject GetItemFromListByRarity(GameObject[] list)
    {
        ItemRarity itemRarity = GetItemDrop();
        List<GameObject> filteredItems = new List<GameObject>();

        foreach (GameObject obj in list)
        {
            Item item = obj.GetComponent<Item>();
            if (item != null && item.GetRarity() == itemRarity)
            {
                filteredItems.Add(obj);
            }
        }

        if (filteredItems.Count > 0)
        {
            int randomIndex = Random.Range(0, filteredItems.Count);
            return filteredItems[randomIndex];
        }

        return null;
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

    private void PlayOpenAnimation()
    {
        if (animator != null && !animator.GetBool("isOpen"))
        {
            animator.SetBool("isOpen", true);
        }
    }
}
