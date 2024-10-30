using UnityEngine;
using Fusion;

namespace Starter.Shooter
{
	/// <summary>
	/// Handles player connections (spawning of Player instances).
	/// </summary>
	public sealed class GameManager : NetworkBehaviour
	{
		public Player PlayerPrefab;

		[Networked]
		public PlayerRef BestHunter { get; set; }
		public Player LocalPlayer { get; private set; }

		private SpawnPoint[] _spawnPoints;

		public Vector3 GetSpawnPosition()
		{
			var spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
			var randomPositionOffset = Random.insideUnitCircle * spawnPoint.Radius;
			return spawnPoint.transform.position + new Vector3(randomPositionOffset.x, 0f, randomPositionOffset.y);
		}

		public override void Spawned()
		{
			_spawnPoints = FindObjectsOfType<SpawnPoint>();

			LocalPlayer = Runner.Spawn(PlayerPrefab, GetSpawnPosition(), Quaternion.identity, Runner.LocalPlayer);
			Runner.SetPlayerObject(Runner.LocalPlayer, LocalPlayer.Object);
		}

		public override void FixedUpdateNetwork()
		{
			BestHunter = PlayerRef.None;
			int bestHunterKills = 0;

			foreach (var playerRef in Runner.ActivePlayers)
			{
				var playerObject = Runner.GetPlayerObject(playerRef);
				var player = playerObject != null ? playerObject.GetComponent<Player>() : null;

				if (player == null)
					continue;

				// Calculate the best hunter
				if (player.Health.IsAlive && player.ChickenKills > bestHunterKills)
				{
					bestHunterKills = player.ChickenKills;
					BestHunter = player.Object.StateAuthority;
				}
			}
		}

		public override void Despawned(NetworkRunner runner, bool hasState)
		{
			// Clear the reference because UI can try to access it even after despawn
			LocalPlayer = null;
		}
	}
}
