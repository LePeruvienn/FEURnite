using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Starter.ThirdPersonCharacter
{
	public class ItemCell : MonoBehaviour
	{
		private TextMeshProUGUI  _itemName;
		private Image _itemIcon;

		// Start is called before the first frame update
		void Start()
		{
			// Getting references
			GameObject itemNameObj = transform.Find ("itemName").gameObject;
			GameObject itemIconObj = transform.Find ("itemIcon").gameObject;
			// Settting components
			_itemName = itemNameObj.GetComponent<TextMeshProUGUI> ();
			_itemIcon = itemIconObj.GetComponent<Image> ();
		}

		public void setName (string name)
		{
			_itemName.text = name;
		}

		public void setIcon (Sprite icon)
		{
			// Setting up image
			_itemIcon.sprite = icon;
			// Setting up image opacity
			Color tempColor = _itemIcon.color;
			tempColor.a = 1f;
			_itemIcon.color = tempColor;
		}
	}
}
