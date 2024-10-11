using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Starter.ThirdPersonCharacter
{
	public class InventoryDisplay : MonoBehaviour
	{

		// Interface objects
		public GameObject inventoryCanvas;
		public GameObject hotbar;
		public GameObject weaponsBox;
		public GameObject itemssBox;

		// Player inventory variable
		private PlayerInventory _playerInventory;

		// Privates variables
		private bool _isDisplayed = false;

		private void Start()
		{
			// Disabling Inventory showing
			inventoryCanvas.SetActive (_isDisplayed);
		}

		public void toggleInventory () {

			Debug.Log ("Inventory Toggle");
			
			_isDisplayed = !_isDisplayed;
			inventoryCanvas.SetActive (_isDisplayed);
		}
	}
}
