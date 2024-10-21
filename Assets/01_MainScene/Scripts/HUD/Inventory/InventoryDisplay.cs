using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Starter.ThirdPersonCharacter
{
	public class InventoryDisplay : MonoBehaviour
	{
		// Static variables
		private static int __HOTBAR_SIZE__ = 4;
		private static int __INVENTORY_SIZE__ = 12;

		// Public variables
		public GameObject[] starterItems;

		// Interface objects
		private GameObject _inventoryCanvas;
		private GameObject _hotbar;
		private GameObject _weapons;
		private GameObject _items;

		// Privates variables
		private bool _isDisplayed = false;

		private void Start()
		{
			// Getting Intefaces Objects
			_inventoryCanvas = GameObject.FindGameObjectWithTag ("cnvInventory");
			_hotbar = _inventoryCanvas.transform.Find ("HotBar").gameObject;
			_weapons = _inventoryCanvas.transform.Find ("WeaponsBox").gameObject;
			_items = _inventoryCanvas.transform.Find ("ItemsBox").gameObject;
			// Disabling Inventory showing
			_inventoryCanvas.SetActive (_isDisplayed);

			// Getting Inventory elements
			ItemCell[] _hotbarCells = _hotbar.transform.Find("CellsParent").GetComponentsInChildren<ItemCell>();
			ItemCell[] _weaponsCells = _weapons.transform.Find("CellsParent").GetComponentsInChildren<ItemCell>();
			ItemCell[] _itemsCells = _items.transform.Find("CellsParent").GetComponentsInChildren<ItemCell>();

			// Initiating cells
			for (int i = 0; i < _hotbarCells.Length; i++)
			{		
				_hotbarCells[i].setType (ItemCellType.Hotbar);
				_hotbarCells[i].setStatus (ItemCellStatus.Free);
				_hotbarCells[i].setIndex (i);
			}


			for (int i = 0; i < _weaponsCells.Length; i++)
			{		
				_weaponsCells[i].setType (ItemCellType.Weapons);
				_weaponsCells[i].setStatus (ItemCellStatus.Free);
				_weaponsCells[i].setIndex (i);
			}

			for (int i = 0; i < _itemsCells.Length; i++)
			{		
				_itemsCells[i].setType (ItemCellType.Items);
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
				if (i < __HOTBAR_SIZE__)
				{
					_hotbarCells[i].setName (item.name);
					_hotbarCells[i].setIcon (item.icon);
					_hotbarCells[i].setStatus (ItemCellStatus.Occuped);
				}
			}
		}

		// Show or unshow player inventory
		public void toggleInventory()
		{
			_isDisplayed = !_isDisplayed;
			_inventoryCanvas.SetActive(_isDisplayed);

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
	}
}
