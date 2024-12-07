using Fusion.Addons.SimpleKCC;
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

        [Header("Greande Effect")]
        public ParticleSystem explosionEffectPrefab; // les particules qui spawn lors de l'explosion
        public AudioClip explosionAudioClip; // sound lors de l'explosion
        public float explosionAudioVolume = 0.5f;

        // Private
        private Transform spawnGrenadePosition; // Where the bullet is gonna spawn
		private PlayerInventory _playerInventory;
        private Animator _playerAnimator;
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

            // Getting PlayerAnimator
            if (_playerAnimator == null)
                _playerAnimator = GetComponentInParent<Animator>();

            // Start dropAndThrow coroutine
            StartCoroutine(dropAndThrowAfterDelay());

            // Start exploding coroutine
            StartCoroutine(explodeAfterDelay());
        }

        private IEnumerator dropAndThrowAfterDelay()
        {
            _playerAnimator.SetTrigger("LaunchTrigger");
            Debug.Log("LaunchTrigger");

            // Wait the delay before explode
            yield return new WaitForSeconds(0.8f);

            dropAndThrow();
        }

        private IEnumerator explodeAfterDelay()
		{
			// Wait the delay before explode
			yield return new WaitForSeconds (timeBeforeExplode);

			// Make the greande explode
			explode ();
		}

        private void dropAndThrow()
        {
            // Drop Grenade
            _playerInventory.dropCurrentSelection();

            // Getting camera transform
            Transform cameraTransform = Camera.main.transform;

            // Setting up throwForce toward rotation of the camera
            Vector3 forceToAdd = (cameraTransform.forward * throwForce) + (cameraTransform.up * throwUpwardForce);

            // Add the force to the grenade rigidBody
            _rigidBody.AddForce(forceToAdd);

            _playerAnimator.ResetTrigger("LaunchTrigger");
        }

        private void explode()
        {
            ParticleSystem Explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(explosionAudioClip, transform.position, explosionAudioVolume);
            Destroy(gameObject);
        }
    }
}
