using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

namespace Starter.ThirdPersonCharacter
{
	public class ItemCell : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
	{

		private TextMeshProUGUI  _itemName;
		private Image _itemIcon;
		private RectTransform _rectTransformIcon;
		private Canvas _canvas;
		private CanvasGroup _canvasGroup;

		// Start is called before the first frame update
		private void Start()
		{
			// Getting references
			GameObject itemNameObj = transform.Find ("itemName").gameObject;
			GameObject itemIconObj = transform.Find ("itemIcon").gameObject;
			// Settting components
			_itemName = itemNameObj.GetComponent<TextMeshProUGUI> ();
			_itemIcon = itemIconObj.GetComponent<Image> ();
			_rectTransformIcon = _itemIcon.GetComponent<RectTransform> ();
			_canvas = _itemIcon.GetComponentInParent<Canvas> ();
			_canvasGroup = GetComponent<CanvasGroup> ();
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

		// ----------------------------
		// DRANG & DROP EVENTS FUNCTIONS
		// ----------------------------

		public void OnPointerDown (PointerEventData eventData)
		{
			Debug.Log ("POINTER DOWN");
		}
		
		public void OnBeginDrag (PointerEventData eventData)
		{
			_canvasGroup.alpha = 0.6f;
			_canvasGroup.blocksRaycasts = false;
		}

		public void OnDrag (PointerEventData eventData)
		{
			_rectTransformIcon.anchoredPosition += eventData.delta / _canvas.scaleFactor;
		}

		public void OnEndDrag (PointerEventData eventData)
		{
			_canvasGroup.alpha = 1f;
			_canvasGroup.blocksRaycasts = true;
		}

		public void OnDrop (PointerEventData eventData)
		{
			if (eventData.pointerDrag == null) return;

			ItemCell cell = eventData.pointerDrag.GetComponent<ItemCell> ();

			if (cell == null || cell == this) return;
			
			cell.setName ("DROOPED");
		}
	}
}
