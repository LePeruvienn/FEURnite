using Fusion;
using UnityEngine;

namespace Starter.ThirdPersonCharacter
{
	public class BulletProjectile : NetworkBehaviour
	{
		private Rigidbody bulletRigidbody;
		[SerializeField] private GameObject vfxHitRed;
		[SerializeField] private GameObject vfxHitBlack;

		public int damage;

		private void Awake()
		{
			bulletRigidbody = GetComponent<Rigidbody>();
		}

		public override void FixedUpdateNetwork()
		{
			if (HasStateAuthority) // S'assurer que seul l'autorit� g�re la logique de mouvement
			{
				float speed = 50f;
				bulletRigidbody.velocity = transform.forward * speed;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!HasStateAuthority) return; // Assurez-vous que seul l'autorité gère les collisions

			GameObject vfxToSpawn = null;
			Vector3 spawnPosition = transform.position; // Position par défaut (avant la correction)
			
			if (other.GetComponentInParent<BulletTarget>() != null)
			{
				// Hit targetPlayer
				PlayerModel pModel = other.GetComponentInParent<PlayerModel>();
				if (pModel != null)
				{
					// Take damage to player
					pModel.takeDamage(damage);
					vfxToSpawn = vfxHitRed;
				}
			}
			else
			{
				// Hit wall
				vfxToSpawn = vfxHitBlack;
			}

			if (vfxToSpawn != null)
			{
				// Si une collision a eu lieu avec un objet, ajustez la position de spawn des particules
				if (other.TryGetComponent(out Collider colliderHit))
				{
					spawnPosition = colliderHit.ClosestPointOnBounds(transform.position); // Utilisez la position d'impact réelle
				}

				// Spawn particle effect using Runner.Spawn to sync it across all clients
				Runner.Spawn(vfxToSpawn, spawnPosition, Quaternion.identity);
			}

			// Despawn bullet
			Runner.Despawn(Object); // Destroy the bullet NetworkObject via Fusion
		}
	}
}
