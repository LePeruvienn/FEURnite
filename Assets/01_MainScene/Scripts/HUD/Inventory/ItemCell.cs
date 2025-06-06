using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

namespace Starter.ThirdPersonCharacter
{

	public enum ItemCellStatus
	{
		Free = 0,
		Occuped = 1,
	}

	public class ItemCell : MonoBehaviour, IDropHandler
	{
		// References
		private InventoryDisplay _iventoryDisplay;

		// Privates
		private InventoryType _type;
		private ItemCellStatus _status;
		private int _index;
		
		// References
		private TextMeshProUGUI  _itemName;
		private ItemIcon _itemIcon;

		// If item is a in game bar cell
		private Image _select;

		// Start is called before the first frame update
		private void Start()
		{
			// Getting references
			GameObject itemNameObj = transform.Find ("itemName").gameObject;
			GameObject itemIconObj = transform.Find ("itemIcon").gameObject;

			// Settting components
			_itemName = itemNameObj.GetComponent<TextMeshProUGUI> ();
			_itemIcon = itemIconObj.GetComponent<ItemIcon> ();
		}

		public void initDisplay (InventoryDisplay inventoryDisplay)
		{
			_iventoryDisplay = inventoryDisplay;
		}

		public InventoryType getType ()
		{
			return _type;
		}

		public void setType (InventoryType type)
		{
			_type = type;
		}

		public ItemCellStatus getStatus ()
		{
			return _status;
		}

		public void setStatus (ItemCellStatus status)
		{
			_status = status;
		}

		public int getIndex ()
		{
			return _index;
		}

		public void setIndex (int index)
		{
			_index = index;
		}

		public void setName (string name)
		{
			_itemName.text = name;
		}

		public void resetName ()
		{
			_itemName.text = "";
		}

		public string getName ()
		{
			return _itemName.text;
		}

		public Sprite getIcon ()
		{
			return _itemIcon.getSprite ();
		}

		public void setIcon (Sprite icon)
		{
			_itemIcon.setIcon (icon);
		}

		public void clearIcon ()
		{
			_itemIcon.removeIcon ();
		}

		public void initSelect ()
		{
			Debug.Log ("Init select ....");

			Transform selectTransform = transform.Find ("select");

			if (selectTransform == null) return;

			Debug.Log ("Founded select !!");

			GameObject selectObj = selectTransform.gameObject;

			_select = selectObj.GetComponent<Image> ();
		}

		public Image getSelect ()
		{
			return _select;
		}

		// ----------------------------
		// DRANG & DROP EVENTS FUNCTIONS
		// ----------------------------

		public void OnDrop (PointerEventData eventData)
		{
			// if there is not item to drop stop
			if (eventData.pointerDrag == null) return;

			// Getting item icon
			ItemIcon itemIcon = eventData.pointerDrag.GetComponent<ItemIcon> ();

			// If object is not a item icon we stop
			if (itemIcon == null) return;

			// Get the ItemCell of the dragged icon
			ItemCell sourceItemCell = itemIcon.GetComponentInParent<ItemCell> ();

			// Check if we are dropping on the same cell
			if (sourceItemCell == this)
			{
				itemIcon.resetPos ();
				return;  // If it's the same cell, don't do anything
			}

			// Check if sourceItemCell is free
			if (sourceItemCell.getStatus () == ItemCellStatus.Free)
				return;

			// Moving items in the player invData
			PlayerInventory playerInventory = _iventoryDisplay.getPlayerInventory ();
			playerInventory.moveItemIndex (sourceItemCell.getType (), _type, sourceItemCell.getIndex (), _index);

			// Checking if we are occuped with an item
			if (_status == ItemCellStatus.Occuped)
			{
				// Swapping items

				// Get the current target into a temp var
				Sprite targetCellIcon = _itemIcon.getSprite();
				string targetCellName = getName();

				// Setting current varaible with the new values
				setIcon(itemIcon.getSprite());
				setName(sourceItemCell.getName());

				// Setting the sources cells icons with the temp var
				sourceItemCell.setIcon(targetCellIcon);
				sourceItemCell.setName(targetCellName);

				// Updating hotbar if hotbar is concerned
				if (sourceItemCell.getType () == InventoryType.Hotbar || _type == InventoryType.Hotbar)
					_iventoryDisplay.updateInGameHotbar ();

				return;
			}

			// Setting new values to current cell
			setIcon (itemIcon.getSprite ());
			setName (sourceItemCell.getName ());
			// Setting currentItem Cell status to occuped
			_status = ItemCellStatus.Occuped;

			// Reseting other cell
			sourceItemCell.resetName ();
			itemIcon.removeIcon ();
			// Setting source item cell status to freea
			sourceItemCell.setStatus (ItemCellStatus.Free);

			// Updating hotbar if hotbar is concerned
			if (sourceItemCell.getType () == InventoryType.Hotbar || _type == InventoryType.Hotbar)
				_iventoryDisplay.updateInGameHotbar ();
		}
	}
}
