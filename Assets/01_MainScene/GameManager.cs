using UnityEngine;
using Fusion;

namespace Starter.ThirdPersonCharacter
{
	/// <summary>
	/// Handles player connections (spawning of Player instances).
	/// </summary>
	public sealed class GameManager : NetworkBehaviour
	{
		public NetworkObject PlayerPrefab;
		public float SpawnRadius = 3f;

		public override void Spawned()
		{
			var randomPositionOffset = Random.insideUnitCircle * SpawnRadius;
			var spawnPosition = transform.position + new Vector3(randomPositionOffset.x, transform.position.y, randomPositionOffset.y);

			Runner.Spawn(PlayerPrefab, spawnPosition, Quaternion.identity, Runner.LocalPlayer);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireSphere(transform.position, SpawnRadius);
		}
	}
}
