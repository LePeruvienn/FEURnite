using Fusion;
using Fusion.Addons.SimpleKCC;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Starter.ThirdPersonCharacter
{
    public class Grenade : Item
    {
		[Header("Grenade Stats")]
        public int maxDamage; // Max damage that the grenade can deal (when player close to explosion)
        public int minDamage; // Min damage taht the grenade can deal (when player far from explosion)
        public int throwForce; // Forward throw force
        public int throwUpwardForce; // Upward throw force
        public float explosionRadius; // All the radius that the grenade will damage on explode
        public int timeBeforeExplode; // Time before the grenade explode
		public bool hasBeenTriggered;
		public bool hasExploded;
        public float maxDamageDistance;
        public float minDamageDistance;


        [Header("Greande Effect")]
        public ParticleSystem explosionEffectPrefab; // les particules qui spawn lors de l'explosion
        public AudioClip explosionAudioClip; // sound lors de l'explosion
        public float explosionAudioVolume = 0.5f;

        // Private
        private Transform _spawnGrenadePosition; // Where the bullet is gonna spawn
		private PlayerInventory _playerInventory;
        private Animator _playerAnimator;
        private Rigidbody _rigidBody;
		private SphereCollider _sphereCollider;

        public override void Spawned()
		{
			// Getting player rigidBody
			if (_rigidBody == null)
				_rigidBody = GetComponent<Rigidbody> ();
		}

        
        public override ItemType getType()
        {
            return ItemType.Grenade;
        }

        public override BulletType getBulletType()
        {
            return BulletType.Pistol;
        }

        public override void use()
        {
			// Getting PlayerInventory
			if (_playerInventory == null)
				_playerInventory = GetComponentInParent<PlayerInventory> ();

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

            // Start exploding coroutine
            if (hasBeenTriggered == false)
				StartCoroutine (explodeAfterDelay ());
        }

        private IEnumerator explodeAfterDelay()
		{
			// Wait the delay before explode
			yield return new WaitForSeconds (timeBeforeExplode);

			hasBeenTriggered = true;

            // Make the grenade explode
            explode ();
		}

		private void explode ()
		{
			if (_sphereCollider == null)
			{
                _sphereCollider = GetComponentInParent<SphereCollider>();
                _sphereCollider.name = "collider_vide";

            }

            _sphereCollider.radius = explosionRadius;
            Debug.Log("COLLIDER : " + _sphereCollider.name);
            Debug.Log("RADIUS : " + _sphereCollider.radius);
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (Collider hitCollider in hitColliders)
            {
                // Vérifier si le collider appartient à un PlayerModel
                PlayerModel player = hitCollider.GetComponent<PlayerModel>();
                if (player != null)
                {
                    // Calculer la distance entre le point d'explosion et le joueur
                    float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                    // Calculer les dégâts basés sur la distance
                    int damage = calculateDamage(distance);
                    // Appliquer les dégâts
                    if (damage > 0)
                    {
                        player.takeDamage(damage);
                    }
                }
            }
            // Explosion terminée
            hasExploded = true;
            // Détruire la grenade
            Destroy(gameObject);
        }

        private int calculateDamage(float distance)
        {
            // Si la distance est inférieure ou égale à la distance max, appliquer maxDamage
            if (distance <= maxDamageDistance)
                return maxDamage;

            // Si la distance est entre maxDamageDistance et minDamageDistance, appliquer minDamage
            if (distance <= minDamageDistance)
                return minDamage;

            // Si la distance est au-delà de minDamageDistance, pas de dégâts
            return 0;
        }

        private void dropAndThrow()
        {
            // Drop Grenade
            _playerInventory.dropCurrentSelection();

            // Getting camera transform
            Transform cameraTransform = Camera.main.transform;

            // Setting up throwForce toward rotation of the camera
            Vector3 forceToAdd = (cameraTransform.forward * throwForce) + (cameraTransform.up * throwUpwardForce);

			// Throw gernade RPC
			RPC_ThrowGrenade (forceToAdd);
        }

		[Rpc(RpcSources.All, RpcTargets.All)]
		private void RPC_ThrowGrenade (Vector3 forceToAdd) {

            // Add the force to the grenade rigidBody
            _rigidBody.AddForce(forceToAdd);

            // Getting PlayerAnimator
            // if (_playerAnimator == null)
                // _playerAnimator = GetComponentInParent<Animator>();

            // _playerAnimator.ResetTrigger("LaunchTrigger");
		}
    }
}
