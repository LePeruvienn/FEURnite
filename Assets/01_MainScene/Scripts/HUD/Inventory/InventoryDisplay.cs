using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Starter.ThirdPersonCharacter
{
	public class InventoryDisplay : MonoBehaviour
	{
		// Reference
		private PlayerInventory _playerInventory;

		// Entry var
		private GameObject[] _starterItems;

		// Interface objects
		private GameObject _inventoryCanvas;
		private GameObject _hotbar;
		private GameObject _weapons;
		private GameObject _items;

		private GameObject _inGameBarObj;
		private InGameHotbar _inGameBar;

		// Interface cells
		private ItemCell[] _hotbarCells;
		private ItemCell[] _weaponsCells;
		private ItemCell[] _itemsCells;

		private ItemCell[] _inGameBarObjCells;

		// Privates variables
		private bool _isDisplayed = false;

		public void init (GameObject[] starterItems)
		{
			// Getting PlayerInventory
			_playerInventory = GetComponentInParent<PlayerInventory> ();
			
			// Setting starters items
			_starterItems = starterItems;

			// Getting Intefaces Objects
			_inventoryCanvas = GameObject.FindGameObjectWithTag ("cnvInventory");
			_inGameBarObj = _inventoryCanvas.transform.Find ("InGameBar").gameObject;
			_hotbar = _inventoryCanvas.transform.Find ("HotBar").gameObject;
			_weapons = _inventoryCanvas.transform.Find ("WeaponsBox").gameObject;
			_items = _inventoryCanvas.transform.Find ("ItemsBox").gameObject;

			// Getting in game hotbar script
			_inGameBar.GetComponent<InGameHotbar> ();

			// Disabling Inventory showing
			_inGameBarObj.SetActive (!_isDisplayed);
			_hotbar.SetActive (_isDisplayed);
			_weapons.SetActive (_isDisplayed);
			_items.SetActive (_isDisplayed);

			// Getting Inventory elements
			_hotbarCells = _hotbar.transform.Find("CellsParent").GetComponentsInChildren<ItemCell>();
			_weaponsCells = _weapons.transform.Find("CellsParent").GetComponentsInChildren<ItemCell>();
			_itemsCells = _items.transform.Find("CellsParent").GetComponentsInChildren<ItemCell>();
			_inGameBarObjCells = _inGameBarObj.transform.Find("CellsParent").GetComponentsInChildren<ItemCell>();

			// Initiating cells
			for (int i = 0; i < _hotbarCells.Length; i++)
			{		
				_hotbarCells[i].initDisplay (this);
				_hotbarCells[i].setType (InventoryType.Hotbar);
				_hotbarCells[i].setStatus (ItemCellStatus.Free);
				_hotbarCells[i].setIndex (i);
			}


			for (int i = 0; i < _weaponsCells.Length; i++)
			{		
				_weaponsCells[i].initDisplay (this);
				_weaponsCells[i].setType (InventoryType.Weapons);
				_weaponsCells[i].setStatus (ItemCellStatus.Free);
				_weaponsCells[i].setIndex (i);
			}

			for (int i = 0; i < _itemsCells.Length; i++)
			{		
				_itemsCells[i].initDisplay (this);
				_itemsCells[i].setType (InventoryType.Items);
				_itemsCells[i].setStatus (ItemCellStatus.Free);
				_itemsCells[i].setIndex (i);
			}
		

			// Setting up inventory items
			for (int i = 0; i < starterItems.Length; i++)
			{
				// Getting object and item instance
				GameObject obj = starterItems[i];
				Item item = obj.GetComponent<Item> ();

				// Check if item instance exist
				if (item == null)
					continue;

				// Checking if we can still put items into the hotbar
				if (i < PlayerInventory.__HOTBAR_SIZE__)
				{
					// Setting into hotbar
					_hotbarCells[i].setName (item.itemName);
					_hotbarCells[i].setIcon (item.icon);
					_hotbarCells[i].setStatus (ItemCellStatus.Occuped);

					// Setting into in game bar
					_inGameBarObjCells[i].setName (item.itemName);
					_inGameBarObjCells[i].setIcon (item.icon);
				}
			}
		}

		public PlayerInventory getPlayerInventory ()
		{
			return _playerInventory;
		}

		// Show or unshow player inventory
		public void toggleInventory()
		{
			_isDisplayed = !_isDisplayed;

			// Disabling Inventory showing
			_inGameBarObj.SetActive (!_isDisplayed);
			_hotbar.SetActive (_isDisplayed);
			_weapons.SetActive (_isDisplayed);
			_items.SetActive (_isDisplayed);

			if (_isDisplayed)
			{
				// Rendre le curseur visible, mais garder les inputs actifs
				Cursor.lockState = CursorLockMode.Confined;  // Confiner le curseur dans la fenêtre
				Cursor.visible = true;
			}
			else
			{
				// Cacher le curseur et verrouiller comme avant
				Cursor.lockState = CursorLockMode.Locked;  // Verrouiller le curseur au centre de l'écran
				Cursor.visible = false;
			}
		}

		public void deleteItem (InventoryType type, int index)
		{
			ItemCell[] cells = null;

			switch (type)
			{
				case InventoryType.Weapons:
					cells = _weaponsCells;
					break;

				case InventoryType.Hotbar:
					cells = _hotbarCells;
					break;

				case InventoryType.Items:
					cells = _itemsCells;
					break;
			}

			if (cells != null) {

				cells[index].clearIcon ();
				cells[index].resetName ();
				cells[index].setStatus (ItemCellStatus.Free);
			}
		}

		public void setItem (InventoryType type, int index, Item item)
		{
			ItemCell[] cells = null;

			switch (type)
			{
				case InventoryType.Weapons:
					cells = _weaponsCells;
					break;

				case InventoryType.Hotbar:
					_inGameBarObjCells[index].setIcon (item.icon);
					_inGameBarObjCells[index].setName (item.itemName);
					cells = _hotbarCells;
					break;

				case InventoryType.Items:
					cells = _itemsCells;
					break;
			}

			if (cells != null) 
			{
				cells[index].setIcon (item.icon);
				cells[index].setName (item.itemName);
				cells[index].setStatus (ItemCellStatus.Occuped);
			}
		}
	}
}
