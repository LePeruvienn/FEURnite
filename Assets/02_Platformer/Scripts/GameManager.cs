using UnityEngine;
using Fusion;

namespace Starter.Platformer
{
	/// <summary>
	/// Handles player connections (spawning of Player instances).
	/// </summary>
	public sealed class GameManager : NetworkBehaviour
	{
		public int MinCoinsToWin = 10;
		public float GameOverTime = 4f;
		public Player PlayerPrefab;
		public float SpawnRadius = 3f;

		public Player LocalPlayer { get; private set; }
		public bool IsGameFinished  => GameOverTimer.IsRunning;

		[Networked]
		public PlayerRef Winner { get; set; }
		[Networked]
		public TickTimer GameOverTimer { get; set; }

		// Called from UnityEvent on Flag gameobject
		public void OnFlagReached(Player player)
		{
			if (HasStateAuthority == false)
				return;

			if (Winner != PlayerRef.None)
				return; // Someone was faster

			if (player.CollectedCoins < MinCoinsToWin)
				return; // Not enough coins

			Winner = player.Object.StateAuthority;
			GameOverTimer = TickTimer.CreateFromSeconds(Runner, GameOverTime);
		}

		public Vector3 GetSpawnPosition()
		{
			var randomPositionOffset = Random.insideUnitCircle * SpawnRadius;
			return transform.position + new Vector3(randomPositionOffset.x, transform.position.y, randomPositionOffset.y);
		}

		public override void Spawned()
		{
			LocalPlayer = Runner.Spawn(PlayerPrefab, GetSpawnPosition(), Quaternion.identity, Runner.LocalPlayer);
			Runner.SetPlayerObject(Runner.LocalPlayer, LocalPlayer.Object);
		}

		public override void FixedUpdateNetwork()
		{
			if (GameOverTimer.Expired(Runner))
			{
				// Restart the game
				Winner = PlayerRef.None;

				// Prepare players for next round
				foreach (var playerRef in Runner.ActivePlayers)
				{
					RPC_RespawnPlayer(playerRef, GetSpawnPosition(), true);
				}

				// Reset timer
				GameOverTimer = default;
			}
		}

		public override void Despawned(NetworkRunner runner, bool hasState)
		{
			// Clear the reference because UI can try to access it even after despawn
			LocalPlayer = null;
		}

		[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
		private void RPC_RespawnPlayer([RpcTarget] PlayerRef playerRef, Vector3 position, bool resetCoins)
		{
			LocalPlayer.Respawn(position, resetCoins);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireSphere(transform.position, SpawnRadius);
		}
	}
}
