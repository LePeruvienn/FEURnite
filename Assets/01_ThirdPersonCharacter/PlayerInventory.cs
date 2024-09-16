using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

namespace Starter.ThirdPersonCharacter
{
	public class PlayerInventory : MonoBehaviour
	{

		public int size;
		public GameObject[] starterItems;

		// Inventory varaible
		private Transform origin;
		private GameObject[] inventory;
		private int selectedIndex = 0;

		private void Awake()
		{
			// Set origin
			origin = GameObject.FindGameObjectWithTag("itemOrigin").transform;
			
			// Setting up the iventory empty
			inventory = new GameObject[size];
			// If there is starters items:
			// We put all the starters items in the inventory
			for (int i = 0; i < size; i++)
			{
				if (i < starterItems.Length) // If we can put a start item
					inventory[i] = starterItems[i];
			}
			
			// Update Current selection
			updateSelection();
		}
		
		// Function that is use to switch from selected intems in inventory
		public void switchSelection (float direciton)
		{
			if (direciton < 0f) // Scroll down
			{
				selectedIndex--; // Tale the previous index
				
				if (selectedIndex < 0) // If previous item dont exist, take the last item
					selectedIndex = inventory.Length - 1;
				
				updateSelection(); // Update current selection
				
			} else if (direciton > 0f) // Scroll up
			{
				selectedIndex++; // Take the next item
				
				if (selectedIndex >= inventory.Length) // If next item dont exist, take the first item
					selectedIndex = 0;
				
				updateSelection(); // Update current selection
			}
		}
		

		// Return the current selected object
		public GameObject getCurrentSelection()
		{
			return inventory[selectedIndex];
		}

		// Set the current selected item to empty
		public void dropCurrentSelection ()
		{
			inventory[selectedIndex] = null;
		}

		// Return true if player current selection is not attached to an item
		public bool isSelectionEmpy ()
		{
			return inventory[selectedIndex] == null;
		}

		private void setItem(GameObject prefab)
		{
			// Adding prefab to the scene
			GameObject item = Instantiate(prefab);
			// Setting origin to be parent's item
			item.transform.SetParent(origin);
			// Setting item's tranform to default
			item.transform.localPosition = Vector3.zero;
			item.transform.localScale = Vector3.one;
			item.transform.localRotation = Quaternion.identity;
		}

		private void updateSelection()
		{
			// Destroy All childs object
			foreach (Transform child in origin)
			{
				GameObject.Destroy(child.gameObject);
			}
			
			// Get currentSelection
			GameObject selection = getCurrentSelection();
			// If selection is not null
			if (selection != null)
				setItem(selection); // Set selection to current item
		}
	}
}