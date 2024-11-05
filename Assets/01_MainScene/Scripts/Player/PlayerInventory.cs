using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Fusion;
 
namespace Starter.ThirdPersonCharacter
{
    public class PlayerInventory : NetworkBehaviour
    {
        [Header("Inventory Config")]
        public int size; // The maximum size of the inventory
        public float pickUpRange; // Range within which items can be picked up

        // Inventory variables
        private Transform _origin; // Transform where picked-up items will be placed
        private NetworkObject[] _inventory; // Array to store items in the inventory
        private int _selectedIndex = 0; // Index of the currently selected inventory slot
        private Transform _dropItemOrigin; // Transform where items will be dropped
        private bool _canPickUp; // Flag to indicate if the player can pick up an item
        private NetworkObject _lastPickableObject; // Reference to the last detected pickable object

        [Networked] private Item _currentItem { get; set; } // The currently held item, synchronized over the network

        public override void Spawned()
        {
            Debug.LogWarning("PLAYER INVENTORY SPAWNED !!");
            init(); // Initialize the inventory when spawned
        }

        private void init()
        {
            // Initialize the pickup state
            _canPickUp = false;

            // Find and assign the item origin (location where items are held in inventory)
            _origin = GameObject.FindWithTag("itemOrigin").transform;

            // Find and assign the drop origin (location where items are dropped)
            _dropItemOrigin = GameObject.FindWithTag("itemDropOrigin").transform;

            // Initialize an empty inventory array
            _inventory = new NetworkObject[size];

            // Update the current selection in the inventory
            updateSelection();
        }

        public void Update()
        {
            // Reset the pickup state each frame
            _canPickUp = false;

            // Check if there are objects within pickup range
            handlePickup();
        }
        public bool addItem(NetworkObject newItem)
        {
            if (newItem == null)
            {
                Debug.LogWarning("Attempted to add a null item to inventory.");
                return false;
            }

            // Check if the item has a NetworkObject component
            if (newItem.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
            {
                // Optionally assign authority when the item is spawned.
                // Ensure that authority is set correctly during the spawn process
                // For example, when spawning the item, you would do something like this:
                // NetworkObject spawnedItem = Runner.Spawn(itemPrefab, spawnPosition, Quaternion.identity, Object.InputAuthority);

                // Mark the item as equipped
                Item itemComponent = newItem.GetComponent<Item>();
                if (itemComponent != null)
                {
                    itemComponent.setState(ItemState.Equipped); // Mark item as equipped
                }

                // Add the item to the inventory if there is space
                for (int i = 0; i < _inventory.Length; i++)
                {
                    if (_inventory[i] == null)
                    {
                        _inventory[i] = newItem;
                        setItem(_inventory[i]); // Optionally set the item's position/rotation
                        _inventory[i].gameObject.SetActive(true); // Activate the item
                        Debug.Log($"Item {newItem.name} added to inventory at index {i}.");
                        return true;
                    }
                }

                Debug.LogWarning("Inventory is full. Cannot add new item.");
                return false;
            }
            else
            {
                Debug.LogWarning("The item does not have a NetworkObject component.");
                return false;
            }
        }




        public void pickUp()
        {
            if (_canPickUp && _lastPickableObject != null)
            {
                NetworkObject selection = getCurrentSelection();

                // Drop the currently selected item if it exists
                if (selection != null)
                    dropCurrentSelection();

                // Get the Item component of the pickable object
                Item item = _lastPickableObject.GetComponent<Item>();
                if (item != null)
                    item.setState(ItemState.Equipped); // Mark item as equipped

                // Place the item in the inventory at the selected index
                _inventory[_selectedIndex] = _lastPickableObject;
                setItem(_inventory[_selectedIndex]); // Set item to inventory position

                _inventory[_selectedIndex].gameObject.SetActive(true); // Activate the object in the inventory
            }
        }

        public void switchSelection(float direction)
        {
            if (direction < 0f) // Scroll down
            {
                _selectedIndex--;
                if (_selectedIndex < 0)
                    _selectedIndex = _inventory.Length - 1; // Wrap around to last index
            }
            else if (direction > 0f) // Scroll up
            {
                _selectedIndex++;
                if (_selectedIndex >= _inventory.Length)
                    _selectedIndex = 0; // Wrap around to first index
            }

            updateSelection(); // Update the selection display
        }

        public void useCurrentSelection()
        {
            NetworkObject obj = _inventory[_selectedIndex];
            if (obj != null)
            {
                Item item = obj.GetComponent<Item>();
                if (item != null)
                    item.use(); // Use the current item
            }
        }

        public NetworkObject getCurrentSelection()
        {
            return _inventory[_selectedIndex]; // Return the current selected item
        }

        public void dropCurrentSelection()
        {
            NetworkObject obj = getCurrentSelection();
            if (obj == null) return;

            // Set the item state to "OnFloor" before dropping
            Item item = obj.GetComponent<Item>();
            if (item != null)
                item.setState(ItemState.OnFloor);

            // Detach from inventory and place at drop position
            obj.transform.SetParent(null);
            obj.transform.SetPositionAndRotation(_dropItemOrigin.position, Quaternion.identity);

            // Remove the item from inventory slot
            _inventory[_selectedIndex] = null;
        }

        public void destoryCurrentSelection()
        {
            // Get the selected item
            NetworkObject obj = getCurrentSelection();

            // Stop if no item is selected
            if (obj == null) return;

            // Remove the item from the scene
            Destroy(obj.gameObject);

            // Clear the inventory slot
            _inventory[_selectedIndex] = null;
        }

        public bool isSelectionEmpty()
        {
            return _inventory[_selectedIndex] == null; // Check if current slot is empty
        }

        public bool canPickUp()
        {
            return _canPickUp; // Return whether player can pick up an item
        }

        private NetworkObject pickupRayCast()
        {
            Vector3 rayOrigin = new Vector3(0.5f, 0.5f, 0f); // Center of the screen for ray origin
            Ray ray = Camera.main.ViewportPointToRay(rayOrigin); // Cast a ray from the camera

            RaycastHit hit;
            NetworkObject lastHit = null;

            // Check for objects within pickup range
            if (Physics.Raycast(ray, out hit, pickUpRange))
            {
                lastHit = hit.transform.GetComponent<NetworkObject>(); // Get the hit object as a NetworkObject
            }

            return lastHit;
        }

        private void handlePickup()
        {
            NetworkObject detectedObj = pickupRayCast();

            if (detectedObj == null) return;

            Item item = detectedObj.GetComponent<Item>();

            if (item == null) return;

            _canPickUp = true; // Allow pickup if an item is detected
            _lastPickableObject = detectedObj; // Set the last pickable item
        }

        private void setItem(NetworkObject obj)
        {
            obj.transform.SetParent(_origin); // Attach the item to inventory origin

            Item item = obj.GetComponent<Item>();
            if (item != null)
            {
                item.setPosAndRotationToDefault(); // Set item to default position/rotation if it's an Item
                obj.transform.localScale = new Vector3(18f, 18f, 18f); // Définir l'échelle souhaitée
                Debug.Log($"Reset scale for {obj.name} to {obj.transform.localScale}");

            }
            else
            {
                obj.transform.SetParent(_origin); // Attachez l'objet à l'origine de l'inventaire
                obj.transform.localPosition = Vector3.zero; // Réinitialisez la position
                obj.transform.localRotation = Quaternion.identity; // Réinitialisez la rotation
                obj.transform.localScale = new Vector3(18f, 18f, 18f); // Définir l'échelle souhaitée
                Debug.Log($"Reset scale for {obj.name} to {obj.transform.localScale}");

                // Forcer l'échelle après un court délai
                StartCoroutine(ResetScale(obj));
            }

            
        }
        private IEnumerator ResetScale(NetworkObject obj)
        {
            yield return null; // Attendre la prochaine frame
            obj.transform.localScale = new Vector3(18f, 18f, 18f); // Réinitialiser l'échelle
            Debug.Log($"Reset scale for {obj.name} to {obj.transform.localScale}");
        }
        private void updateSelection()
        {
            disableAllItems(); // Hide all items in inventory

            NetworkObject selection = getCurrentSelection();
            if (selection == null) return;

            selection.gameObject.SetActive(true); // Display the selected item
        }

        private void disableAllItems()
        {
            foreach (Transform child in _origin)
            {
                NetworkObject networkObj = child.GetComponent<NetworkObject>();
                if (networkObj != null)
                    networkObj.gameObject.SetActive(false); // Disable each item in inventory origin
            }
        }

        private void hideAllItems()
        {
            foreach (Transform child in _origin)
            {
                Renderer renderer = child.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.enabled = false; // Hide all item renderers in inventory
            }
        }
    }
}
