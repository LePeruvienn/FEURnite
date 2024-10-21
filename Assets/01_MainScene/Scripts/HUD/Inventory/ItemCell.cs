using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

namespace Starter.ThirdPersonCharacter
{
	public class ItemCell : MonoBehaviour, IDropHandler
	{

		private TextMeshProUGUI  _itemName;
		private ItemIcon _itemIcon;

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

		public void setName (string name)
		{
			_itemName.text = name;
		}

		public void setIcon (Sprite icon)
		{
			_itemIcon.setIcon (icon);
		}

		public void clearIcon ()
		{
			_itemIcon.removeIcon ();
		}

		// ----------------------------
		// DRANG & DROP EVENTS FUNCTIONS
		// ----------------------------

		public void OnDrop (PointerEventData eventData)
		{
			if (eventData.pointerDrag == null) return;

			ItemIcon itemIcon = eventData.pointerDrag.GetComponent<ItemIcon> ();

			if (itemIcon == null) return;

			setIcon (itemIcon.getSprite ());
			itemIcon.removeIcon ();
		}
	}
}
