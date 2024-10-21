
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

namespace Starter.ThirdPersonCharacter
{
	public class ItemIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		// References
		private Image _itemIcon;
		private RectTransform _rectTransformIcon;
		private Canvas _canvas;
		private CanvasGroup _canvasGroup;

		private void Start()
		{
			_itemIcon = GetComponent<Image> ();
			_rectTransformIcon = GetComponent<RectTransform> ();
			_canvas = GetComponentInParent<Canvas> ();
			_canvasGroup = GetComponent<CanvasGroup> ();
		}

		public void resetPos ()
		{
			// Reseting Icon pos
			_rectTransformIcon.anchoredPosition = Vector3.zero;
		}

		public void removeIcon ()
		{
			// Removing the image by setting sprite to null
			_itemIcon.sprite = null;

			// Making the image fully transparent
			Color tempColor = _itemIcon.color;
			tempColor.a = 0f;
			_itemIcon.color = tempColor;

			// Reseting Icon pos
			resetPos ();
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

		public Sprite getSprite ()
		{
			return _itemIcon.sprite;
		}

		// ----------------------------
		// DRANG & DROP EVENTS FUNCTIONS
		// ----------------------------
		
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

			// Getting obj
			GameObject obj = eventData.pointerCurrentRaycast.gameObject;
			
			// Check if the pointer is not over a valid drop area
			if (obj == null)
			{
				resetPos();  // Reset position if no object is detected
				return;
			}

			// Getting itemCell Obj
			ItemCell itemCell = obj.GetComponent<ItemCell> ();

			// Reset if null
			if (itemCell == null)
				resetPos ();
		}
	}
}
