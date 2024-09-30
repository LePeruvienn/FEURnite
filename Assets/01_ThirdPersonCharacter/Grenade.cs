using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Starter.ThirdPersonCharacter
{
    public class Grenade : Item
    {
		[Header("Greande Stats")]
        public int maxDamage; // Max damage that the grenade can deal (when player close to explosion)
        public int minDamage; // Min damage taht the grenade can deal (when player far from explosion)
        public int throwForce; // Forward throw force
        public int throwUpwardForce; // Upward throw force
        public float explosionRadius; // All the radius that the grenade will damage on explode
        public int timeBeforeExplode; // Time before the grenade explode
        
        // Private
        private Transform spawnGrenadePosition; // Where the bullet is gonna spawn
		private PlayerInventory _playerInventory;
		private Rigidbody _rigidBody;
        
        public override ItemType getType()
        {
            return ItemType.Grenade;
        }

        public override void use()
        {
			// Getting PlayerInventory
			if (_playerInventory == null)
				_playerInventory = GetComponentInParent<PlayerInventory> ();

			// Getting player rigidBody
			if (_rigidBody == null)
				_rigidBody = GetComponent<Rigidbody> ();

			// Drop Grenade
			_playerInventory.dropCurrentSelection ();

			// Getting camera transform
			Transform cameraTransform = Camera.main.transform;

			// Setting up throwForce toward rotation of the camera
			Vector3 forceToAdd = (cameraTransform.forward * throwForce) + (cameraTransform.up * throwUpwardForce);

			// Add the force to the grenade rigidBody
			_rigidBody.AddForce (forceToAdd);

			// Start exploding coroutine
			StartCoroutine (explodeAfterDelay ());
        }

		private IEnumerator explodeAfterDelay ()
		{
			// Wait the delay before explode
			yield return new WaitForSeconds (timeBeforeExplode);

			// Make the greande explode
			explode ();
		}

		private void explode ()
		{
			Destroy (gameObject);
		}
    }
}
