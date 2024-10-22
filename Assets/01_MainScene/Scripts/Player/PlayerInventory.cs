using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

namespace Starter.ThirdPersonCharacter
{

	public enum InventoryType
	{
		Hotbar = 0,
		Weapons = 1,
		Items = 2,
	}

	public class PlayerInventory : MonoBehaviour
	{

        [Header("Iventory Config")]
		public GameObject[] starterItems;
		public float pickUpRange;

		// Display
		private InventoryDisplay _inventoryDisplay;

		// Inventory Size
		private int _size = 4;

		// Inventory varaible
		private Transform _origin;
		private GameObject[] _inventory;
		private GameObject[] _weapons;
		private GameObject[] _items;
		private int _selectedIndex = 0;
		private Transform _dropItemOrigin;
		private bool _canPickUp;
		private GameObject _lastPickableObject;
		
		private void Awake ()
		{
			// Set pickUp state
			_canPickUp = false;

			// Setting item origin
			_origin = GameObject.FindGameObjectWithTag("itemOrigin").transform;
			
			// Setting drop origin
			_dropItemOrigin = GameObject.FindGameObjectWithTag("itemDropOrigin").transform;
            
			// Setting up the iventory empty
			_inventory = new GameObject[_size];
			_weapons = new GameObject[_size];
			_items = new GameObject[_size];

			// If there is starters items:
			// We put all the starters items in the inventory
			for (int i = 0; i < _size; i++)
			{
				// If we can put a start item
				if (i < starterItems.Length)
				{
                    // We instantiate the object to create a copy
                    GameObject itemInstance = Instantiate (starterItems[i]);
					// Getting item compenent
					Item item = itemInstance.GetComponent<Item> ();
					if (item != null)
					{
						// If Item script exist, set item state to equipped
						item.setState (ItemState.Equipped);
						// We save his current default position, scale and rotation config
						item.saveDefaultPosAndRotation ();
					}

					// Setting starter item in display
					_inventory[i] = itemInstance;
					
					// Set item pos
					setItem (itemInstance);

					// Hide item
					itemInstance.SetActive (false);
				}
			}


			// Getting InventoryDisplay
			_inventoryDisplay = GetComponentInParent<InventoryDisplay> ();
			// Initialize starterItems in display
			_inventoryDisplay.init (starterItems);
			
			// Update Current selection
			updateSelection();
		}

		public void Update ()
		{
			// Set pickUp to false to default
			_canPickUp = false;
			// Handle detection of pickable objects
			handlePickup ();
		}

		public void pickUp ()
		{
			// If player can pickup an object
			if (_canPickUp == true && _lastPickableObject != null) {
				
				// Get current selection
				GameObject selection = getCurrentSelection ();

				// If current selection is not null, we drop the item selected
				if (selection != null)
					dropCurrentSelection ();

				// Get Item compenent
				Item item = _lastPickableObject.GetComponent<Item> ();
				if (item != null) // If Item script exist, set item state to equipped
					item.setState (ItemState.Equipped);
				
				// Setting starter item in inventory
				_inventory[_selectedIndex] = _lastPickableObject;
				
				// Set item pos
				setItem (_inventory[_selectedIndex]);

				_inventory[_selectedIndex].SetActive (true);

				// Setting it on the display
				_inventoryDisplay.setItem (InventoryType.Hotbar, _selectedIndex, item);
			}
		}

		public void moveItemIndex (InventoryType type, int index, int target)
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

			if (cells == null) return;


			GameObject temp = cells[target];
			cells[target] = cells[index];
			cells[index] = temp;

			updateSelection(); // Update current selection
		}
		
		// Function that is use to switch from selected intems in inventory
		public void switchSelection (float direciton)
		{
			if (direciton < 0f) // Scroll down
			{
				_selectedIndex--; // Tale the previous index
				
				if (_selectedIndex < 0) // If previous item dont exist, take the last item
					_selectedIndex = _inventory.Length - 1;
				
				updateSelection(); // Update current selection
				
			} else if (direciton > 0f) // Scroll up
			{
				_selectedIndex++; // Take the next item
				
				if (_selectedIndex >= _inventory.Length) // If next item dont exist, take the first item
					_selectedIndex = 0;
				
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
			// Get Selected item
			GameObject obj = getCurrentSelection();
			// If current selection is null we stop here
			if (obj == null) return;
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

			// Deleting the item display
			_inventoryDisplay.deleteItem (InventoryType.Hotbar, _selectedIndex);
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

			// Return if the object detected is not an Item;
			if (item == null) return;

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
			// Disable all items
			disableAllItems ();
			
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

		// Getters

		// Return inventory items list
		public Item[] getInventoryData ()
		{
			Item[] items = new Item[_size];

			for (int i = 0; i < _size; i ++)
				items[i] = _inventory[i].GetComponent<Item> ();

			return items;
		}
	}
}
