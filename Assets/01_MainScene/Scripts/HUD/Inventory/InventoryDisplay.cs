using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Starter.ThirdPersonCharacter
{
	public class InventoryDisplay : MonoBehaviour
	{
		public Sprite inGameBarSelectSprite;

		// Reference
		private PlayerInventory _playerInventory;

		// Entry var
		private GameObject[] _starterItems;

		// Interface objects
		private GameObject _inventoryCanvas;
		private GameObject _hotbar;
		private GameObject _weapons;
		private GameObject _items;
		private GameObject _inGameBar;

		// Interface cells
		private ItemCell[] _hotbarCells;
		private ItemCell[] _weaponsCells;
		private ItemCell[] _itemsCells;
		private ItemCell[] _inGameBarCells;

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
			_inGameBar = _inventoryCanvas.transform.Find ("InGameBar").gameObject;
			_hotbar = _inventoryCanvas.transform.Find ("HotBar").gameObject;
			_weapons = _inventoryCanvas.transform.Find ("WeaponsBox").gameObject;
			_items = _inventoryCanvas.transform.Find ("ItemsBox").gameObject;

			// Disabling Inventory showing
			_inGameBar.SetActive (!_isDisplayed);
			_hotbar.SetActive (_isDisplayed);
			_weapons.SetActive (_isDisplayed);
			_items.SetActive (_isDisplayed);

			// Getting Inventory elements
			_hotbarCells = _hotbar.transform.Find("CellsParent").GetComponentsInChildren<ItemCell>();
			_weaponsCells = _weapons.transform.Find("CellsParent").GetComponentsInChildren<ItemCell>();
			_itemsCells = _items.transform.Find("CellsParent").GetComponentsInChildren<ItemCell>();
			_inGameBarCells = _inGameBar.transform.Find("CellsParent").GetComponentsInChildren<ItemCell>();

			// Initiaing in game bar cells
			for (int i = 0; i < _inGameBarCells.Length; i++)
			{
				_inGameBarCells[i].initSelect ();
				_inGameBarCells[i].resetName ();
			}

			// Initiating cells
			for (int i = 0; i < _hotbarCells.Length; i++)
			{		
				_hotbarCells[i].initDisplay (this);
				_hotbarCells[i].setType (InventoryType.Hotbar);
				_hotbarCells[i].setStatus (ItemCellStatus.Free);
				_hotbarCells[i].setIndex (i);
				_hotbarCells[i].resetName ();
			}


			for (int i = 0; i < _weaponsCells.Length; i++)
			{		
				_weaponsCells[i].initDisplay (this);
				_weaponsCells[i].setType (InventoryType.Weapons);
				_weaponsCells[i].setStatus (ItemCellStatus.Free);
				_weaponsCells[i].setIndex (i);
				_weaponsCells[i].resetName ();
			}

			for (int i = 0; i < _itemsCells.Length; i++)
			{		
				_itemsCells[i].initDisplay (this);
				_itemsCells[i].setType (InventoryType.Items);
				_itemsCells[i].setStatus (ItemCellStatus.Free);
				_itemsCells[i].setIndex (i);
				_itemsCells[i].resetName ();
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
					_inGameBarCells[i].setName (item.itemName);
					_inGameBarCells[i].setIcon (item.icon);
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
			_inGameBar.SetActive (!_isDisplayed);
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
					_inGameBarCells[index].clearIcon ();
					_inGameBarCells[index].resetName ();
					_inGameBarCells[index].setStatus (ItemCellStatus.Free);
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
					_inGameBarCells[index].setIcon (item.icon);
					_inGameBarCells[index].setName (item.itemName);
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

		public void updateInGameHotbarSelection (int index)
		{
			for (int i = 0; i < _inGameBarCells.Length; i++)
			{
				Image select = _inGameBarCells[i].getSelect ();

				if (i == index)
				{
					// Making the image visible
					Color tempColor = select.color;
					tempColor.a = 1f;
					select.color = tempColor;
				}
				else {
					// Making the image fully transparent
					Color tempColor = select.color;
					tempColor.a = 0f;
					select.color = tempColor;
				}
			}
		}

		public void updateInGameHotbar ()
		{
			for (int i = 0; i < _hotbarCells.Length; i ++)
			{
				ItemCell cell = _hotbarCells[i];

				if (cell.getStatus () == ItemCellStatus.Occuped)
				{
					_inGameBarCells[i].setIcon (cell.getIcon ());
					_inGameBarCells[i].setName (cell.getName ());
					_inGameBarCells[i].setStatus (ItemCellStatus.Occuped);
				}
				else 
				{
					_inGameBarCells[i].clearIcon ();
					_inGameBarCells[i].resetName ();
					_inGameBarCells[i].setStatus (ItemCellStatus.Free);
				}
			}
		}
	}
}
