using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    // Classe abstaire Des items (Utilisable, armes, grenades)
    public abstract class Item : NetworkBehaviour
    {
		[Header("Item config")]
		public Sprite icon;
        public string itemName; // Nom de l'item
        public int stackAmount; // Nombre d'objets qu'on peut stocker en meme temps dans l'inventaire
        public ItemRarity rarity; // Rareté de l'objet
        
        // Privates
        [Networked] private ItemState _state { get; set; } // Etat actuelle : onFloor (au sol), equiped (dans l'inventaire d'un joueur) , selected (Dans la main d'un joueur)
		//Default Tranform saves
		private Vector3 _defaultPosition;
		private Vector3 _defaultScale;
		private Quaternion _defaultRotation;
        
        // Abstract functions
        public abstract ItemType getType(); // Retourne de type de l'objet
        public abstract void use(); // Utiliser l'item
        public abstract BulletType getBulletType();


        public ItemRarity GetRarity()
        {
            return rarity;
        }

        public void saveDefaultPosAndRotation () 
		{
			// Save current position scale and rotation in the default data
			_defaultPosition = transform.position;
			_defaultScale = transform.lossyScale;
			_defaultRotation = transform.rotation;
		}

		public void setPosAndRotationToDefault () 
		{
			// Set current position scale and rotation to default
			/*transform.localPosition = _defaultPosition != null ? 
				_defaultPosition : Vector3.zero;*/

			transform.localScale = _defaultScale != null ?
				_defaultScale : transform.lossyScale;

			transform.localRotation = _defaultRotation != null ?
				_defaultRotation : Quaternion.identity;
		}
        
        // State getter and setter
        public void setState(ItemState state)
        {
            // Setting state
            _state = state;
            // Update object
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
                    updatePhysics(false);
                    break;
                
                case ItemState.Selected:
                    updatePhysics(false);
                    break;
            }
        }

        private void updatePhysics(bool _bool)
        {
            // Get the Rigidbody component and disable it (if it exists)
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = !_bool; // Making it kinematic so it no longer interacts with physics

            // Get all the Collider components and disable them
            Collider[] colliders = GetComponents<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = _bool; // Disable the collider
            }
        }
    }
}
