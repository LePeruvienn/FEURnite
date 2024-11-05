using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Starter.ThirdPersonCharacter
{

    // Enums for item types
    public enum ItemType
    {
        None = 0,
        Weapon = 1,
        Usable = 2,
        Grenade = 3
    }

    // Enums for item states
    public enum ItemState
    {
        OnFloor = 1,
        Equipped = 2,
        Selected = 3
    }

    // Enums for item rarity
    public enum ItemRarity
    {
        Common = 1,
        Rare = 2,
        Epic = 3,
        Legendary = 4
    }
    public abstract class Item : NetworkBehaviour
    {


        [Header("Item config")]
        public string itemName; // Nom de l'item
        public int stackAmount; // Nombre d'objets qu'on peut stocker en même temps dans l'inventaire
        public ItemRarity rarity; // Rareté de l'objet

        // Privates
        private ItemState _state; // État actuel
        private Vector3 _defaultPosition;
        private Vector3 _defaultScale;
        private Quaternion _defaultRotation;

        // Abstract functions
        public abstract ItemType getType(); // Retourne le type de l'objet
        public abstract void use(); // Utiliser l'item


        public void saveDefaultPosAndRotation()
        {
            // Save current position scale and rotation in the default data
            _defaultPosition = transform.position;
            _defaultScale = transform.lossyScale;
            _defaultRotation = transform.rotation;
        }

        public void setPosAndRotationToDefault()
        {
            // Set current position scale and rotation to default
            transform.localPosition = _defaultPosition != null ?
                _defaultPosition : Vector3.zero;

            transform.localScale = _defaultScale != null ?
                _defaultScale : transform.lossyScale;

            transform.localRotation = _defaultRotation != null ?
                _defaultRotation : Quaternion.identity;
        }

        // State getter and setter
        public void setState(ItemState state)
        {
            _state = state;
            updateState();
        }

        public ItemState getState()
        {
            return _state;
        }

        // States updater
        private void updateState()
        {
            switch (_state)
            {
                case ItemState.OnFloor:
                    updatePhysics(true);
                    break;

                case ItemState.Equipped:
                case ItemState.Selected:
                    updatePhysics(false);
                    break;
            }
        }

        private void updatePhysics(bool _bool)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = !_bool;

            Collider[] colliders = GetComponents<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = _bool;
            }
        }
    }
}
