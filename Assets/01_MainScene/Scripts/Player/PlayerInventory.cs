using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using Fusion;
using System.Linq;

namespace Starter.ThirdPersonCharacter
{

	public enum InventoryType
	{
		Hotbar = 0,
		Weapons = 1,
		Items = 2,
	}

	public class PlayerInventory : NetworkBehaviour
	{
		// Statics
		public static int __HOTBAR_SIZE__ = 4;
		public static int __WEAPONS_SIZE__ = 12;
		public static int __ITEMS_SIZE__ = 12;

        [Header("Iventory Config")]
		public float pickUpRange;

		// Display
		private InventoryDisplay _inventoryDisplay;

		// Transform references
		[SerializeField] private Transform _dropItemOrigin;
		[SerializeField] private Transform _origin;

		// Inventory varaible
		private GameObject[] _inventory;
		private GameObject[] _weapons;
		private GameObject[] _items;
		[Networked] private int _selectedIndex {get; set;}
		private bool _canPickUp;
		private GameObject _lastPickableObject;

		public override void Spawned()
		{
			
			
                base.Spawned();
                _selectedIndex = 0;
				_canPickUp = false;

				// Initialize inventory arrays
				_inventory = new GameObject[__HOTBAR_SIZE__];
				_weapons = new GameObject[__WEAPONS_SIZE__];
				_items = new GameObject[__ITEMS_SIZE__];

				
		}

        public void AddItem(NetworkObject spawnedObject, int i)
        {
			// Spawn starter items into the inventory
			if ( i < __HOTBAR_SIZE__)
			{
					
					if (spawnedObject == null)
					{
						Debug.LogError("Impossible d'ajouter un item null.");
						return;
					}

					// Attribuer l'autorité d'entrée au joueur
					/*   if (Runner.IsServer)
					   {
						   item.AssignInputAuthority(Object.InputAuthority);
						   Debug.Log("Autorité d'entrée assignée à l'item.");
					   }
		   */
					// Ajouter l'item à la liste d'inventaire

					// Get the Item component
					Item item = spawnedObject.GetComponent<Item>();
					if (item != null)
					{
						item.setState(ItemState.Equipped);
						item.saveDefaultPosAndRotation();
					}
					else
					{
						Debug.LogWarning("---Item null ID player :" + Runner.LocalPlayer.PlayerId + " itemNetworkObject :" + spawnedObject.Id);
					}
					// Assign it to the inventory
					_inventory[i] = spawnedObject.gameObject;

					// Position the item correctly
					setItem(spawnedObject.gameObject);

					// Hide the item
					spawnedObject.gameObject.SetActive(false);
					RPC_pickup(spawnedObject.Id);
            }
        }
        public void initAdd(GameObject[] starterItems)
        {
            // Initialize the inventory display
            _inventoryDisplay = GetComponentInParent<InventoryDisplay>();

            // Initialize the inventory display with the GameObjects
            _inventoryDisplay.init(starterItems);

            // Update current selection
            updateSelection();
        }
        public void Update ()
		{
			// Set pickUp to false to default
			_canPickUp = false;
			// Handle detection of pickable objects
			handlePickup ();
		}

		public void pickUp()
		{
            // Si le joueur peut ramasser un objet
            if (_canPickUp == true && _lastPickableObject != null )
			{
                Debug.LogWarning("can pickup");
                // Handle Loot box
                LootBox lootBox = _lastPickableObject.GetComponent<LootBox>();
				if (lootBox != null)
				{
					lootBox.Open();
                    Debug.LogWarning("lootBox");
                    return;
				}

				// Handle Server-Side Sync Pickup
				if (!Object.HasStateAuthority)
				{
					Debug.LogWarning("Only State Authority can call updateSelection().");
					return;
				}

				// Get current selection
				GameObject selection = getCurrentSelection();

				// If current selection is not null, drop the item
				if (selection != null)
				{
					Debug.LogWarning("probleme selection");
					dropCurrentSelection();
				}
				NetworkObject netObj = _lastPickableObject.GetComponent<NetworkObject>();

				if (netObj == null)
				{
					Debug.LogWarning("_lastPickableObject don't have NetworkObject component !!");
					return;
				}
               
                /*if (netObj.HasInputAuthority)
				{
					Debug.LogWarning("Try to pickup a object ou have al reday a authority state authority!"+ netObj.HasInputAuthority);
					return;
				}*/

				Debug.LogWarning("Object ID sent: " + netObj.Id);

				// Doing server-side function
				RPC_pickup(netObj.Id);

                // Set item in the display
                Item item = _lastPickableObject.GetComponent<Item>();
				_inventoryDisplay.setItem(InventoryType.Hotbar, _selectedIndex, item);
			}
		}


        [Rpc(RpcSources.All, RpcTargets.All)]
		public void RPC_pickup(NetworkId objectId)
		{
			Debug.Log ("ID received : " + objectId);

			NetworkObject networkObject = Runner.FindObject(objectId);
			if (networkObject == null)
			{
				Debug.LogWarning("Failed to find the object to pick up.");
				return;
			}

			GameObject pickedObject = networkObject.gameObject;
            Debug.LogWarning("pick  authority!" + networkObject.InputAuthority);
            networkObject.RequestStateAuthority();
            Debug.LogWarning("pick  authority!" + networkObject.InputAuthority);
            // Get Item compenent
            Item item = pickedObject.GetComponent<Item> ();
			if (item != null) // If Item script exist, set item state to equipped
				item.setState (ItemState.Equipped);
			
			// Setting starter item in inventory
			_inventory[_selectedIndex] = pickedObject;
			
			// Set item pos
			setItem (_inventory[_selectedIndex]);

			_inventory[_selectedIndex].SetActive (true);
		}

		public void moveItemIndex (InventoryType sourceType, InventoryType targetType, int index, int target)
		{
			GameObject[] sourceCells = getCells (sourceType);
			GameObject[] targetCells = getCells (targetType);

			if (targetCells == null || sourceCells == null) return;

			GameObject temp = targetCells[target];
			targetCells[target] = sourceCells[index];
			sourceCells[index] = temp;

			updateSelection(); // Update current selection
		}
		
		// Function that is use to switch from selected intems in inventory
		public void switchSelection (float direciton)
		{
			if (direciton < 0f) // Scroll down
			{
				_selectedIndex++; // Tale the previous index
				
				if (_selectedIndex >= _inventory.Length) // If next item dont exist, take the first item
					_selectedIndex = 0;
				
				updateSelection(); // Update current selection
				
			} else if (direciton > 0f) // Scroll up
			{
				_selectedIndex--; // Take the next item
				
				if (_selectedIndex < 0) // If previous item dont exist, take the last item
					_selectedIndex = _inventory.Length - 1;
				
				updateSelection(); // Update current selection
			}
		}
		
		// Use current selection
		public void useCurrentSelection()
		{
			// Getting current selection
			GameObject obj = _inventory[_selectedIndex];
			// If had selected an object
			if (obj != null)
			{
				// Getting current item
				Item item = obj.GetComponent<Item>();
				// If object is a item use it
				if (item != null)
					item.use();
            }
		}

		// Return the current selected object
		public GameObject getCurrentSelection()
		{
			return _inventory[_selectedIndex];
		}

		// Drop the current selected item
		public void dropCurrentSelection ()
		{
			// Check if player have authority
			if (!Object.HasStateAuthority)
			{
				Debug.LogWarning("Only State Authority can call updateSelection().");
				return;
			}

			// Doing server-side function
			RPC_dropCurrentSelection();

			// Deleting the item display
			_inventoryDisplay.deleteItem (InventoryType.Hotbar, _selectedIndex);
		}


        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
		public void RPC_dropCurrentSelection()
		{
			// Get Selected item
			GameObject obj = getCurrentSelection();
            
            // If current selection is null we stop here
            if (obj == null) return;
            NetworkObject netObj = obj.GetComponent<NetworkObject>();
            // We get his item component
            Item item = obj.GetComponent<Item>();
			// If item exist we drop it
			if (item != null)
				item.setState(ItemState.OnFloor);

			// Removing obj parent's
			obj.transform.SetParent(null);
            // Adding the object to the scene
            obj.transform.SetPositionAndRotation(_dropItemOrigin.position, Quaternion.identity);
            
            // Clearing the data
			_inventory[_selectedIndex] = null;
            Debug.LogWarning("dropstate authority avant!" + netObj.InputAuthority);
            netObj.ReleaseStateAuthority();
            Debug.LogWarning("dropstate authority!" + netObj.InputAuthority);


        }

		// Desotry the current selected item
		public void destoryCurrentSelection ()
		{
			// Get Selected item
			GameObject obj = getCurrentSelection();

			// If current selection is null we stop here
			if (obj == null) return;

			// Removing object
            Destroy (obj);

            // Clearing the data
			_inventory[_selectedIndex] = null;
		}

		// Return true if player current selection is not attached to an item
		public bool isSelectionEmpy ()
		{
			return _inventory[_selectedIndex] == null;
		}

		public bool canPickUp ()
		{
			return _canPickUp;
		}

		private GameObject pickupRayCast ()
		{
            // Setting up raycast variables
            Vector3 rayOrigin = new Vector3(0.5f, 0.5f, 0f); // center of the screen
            
            // Doing the raycast !
            Ray ray = Camera.main.ViewportPointToRay(rayOrigin);
			
            // Setting raycast output variable
            RaycastHit hit;
            
			// Setting obj output variable
			GameObject lastHit = null;

            // If raycast hit
            if (Physics.Raycast(ray, out hit, pickUpRange))
            {
                lastHit = hit.transform.gameObject; // Set the target point to the point hit by the raycast
            }

			return lastHit;
		}

		private void handlePickup ()
		{
			// Detect Object that player is aiming
			GameObject detectedObj = pickupRayCast ();

			// Return if we dont detect any object
			if (detectedObj == null) return;

			Item item = detectedObj.GetComponent<Item> ();
			LootBox lootBox = detectedObj.GetComponent<LootBox> ();
			

			// Return if the object detected is not an Item;
			if (item == null && lootBox == null) return;

			// Set can pickup to true !
			_canPickUp = true;
			
			// Set _lastPickableObject to the object detected !
			_lastPickableObject = detectedObj;
		}

		private void setItem(GameObject obj)
		{
			// Setting origin to be parent's obj
			obj.transform.SetParent(_origin);

			// Get Item
			Item item = obj.GetComponent<Item> ();
		
			// ### ! THIS PART IS TEMPORARY CASUE item.setPosAndRotationToDefault() dont work well !!!

			// Set all to 0 except we keep his scale
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = obj.transform.lossyScale;
			obj.transform.localRotation = Quaternion.identity;

			return;

			// Setting obj's tranform to his game object param
			if (item != null) 
			{
				// We use set default function of the item
				item.setPosAndRotationToDefault ();
			}
			else
			{
				// Set all to 0 except we keep his scale
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localScale = obj.transform.lossyScale;
				obj.transform.localRotation = Quaternion.identity;
			}
		}

		private void updateSelection()
		{
			// Doing server-side function
			RPC_updateSelection ();

			// Updating UI
			_inventoryDisplay.updateInGameHotbarSelection (_selectedIndex);
		}

        [Rpc(RpcSources.All, RpcTargets.All)]
		public void RPC_updateSelection()
		{
			// Disable all items
			disableAllItems();

			// Get currentSelection
			GameObject selection = getCurrentSelection ();
			// If selection is not null
			if (selection != null)
				selection.SetActive (true); // Active current selected item
		}

		private void disableAllItems()
		{
			// Destroy each child of the gameObject origin
			foreach (Transform child in _origin)
			{
				child.gameObject.SetActive(false); // Desactivate the object
			}
		}

		private void showItem (GameObject item, bool _bool)
		{

			// Get render compenent
			Renderer renderer = item.GetComponent<Renderer>(); // Desactivate the object
			// Disable renderer if exist
			if (renderer != null)
				renderer.enabled = _bool;
		}

		private void hideAllItems ()
		{
			// Destroy each child of the gameObject origin
			foreach (Transform child in _origin)
			{
				// Get render compenent
				Renderer renderer = child.gameObject.GetComponent<Renderer>(); // Desactivate the object
				// Disable renderer if exist
				if (renderer != null)
					renderer.enabled = false;
			}
		}

		private GameObject[] getCells (InventoryType type)
		{
			GameObject[] cells = null;

			switch (type)
			{
				case InventoryType.Weapons:
					cells = _weapons;
					break;

				case InventoryType.Hotbar:
					cells = _inventory;
					break;

				case InventoryType.Items:
					cells = _items;
					break;
			}

			return cells;
		}

		// Getters

		// Return inventory items list
		public Item[] getInventoryData ()
		{
			Item[] items = new Item[__HOTBAR_SIZE__];

			for (int i = 0; i < __HOTBAR_SIZE__; i ++)
				items[i] = _inventory[i].GetComponent<Item> ();

			return items;
		}
	}
}
