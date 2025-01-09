using UnityEngine;
using Fusion;
using System.Collections.Generic;

/// <summary>
/// Handles player connections (spawning of Player instances) at designated spawn points.
/// </summary>
namespace Starter.ThirdPersonCharacter
{
    /// <summary>
    /// Handles player connections (spawning of Player instances).
    /// </summary>
	
    // Game Status
    public enum GameState
    {
        WaitingForPlayers = 1,
        ReadyToLaunch = 2,
        InGame = 3,
        GameEnd = 4
    }

    public sealed class GameManager : NetworkBehaviour
    {
        [Header("Game Manager Config")]
        public NetworkObject PlayerPrefab;
        public NetworkObject CorpsePrefab;
        public float SpawnRadius = 3f;
        public List<Transform> SpawnPoints;
        public Transform SpawnBase;
        // Variable locale pour vérifier si le joueur est déjà spawné

        [Header("DEBUG TOOLS")]
        public bool startGameManually = false; // Use private field for backing
		
		// GAME STATUS
        [Networked] private int _readyPlayerCount { get; set; } = 0; // Players ready
        [Networked] private int _playerCount { get; set; } = 0; // Players ready
        [Networked] private GameState _gameState { get; set; } = GameState.WaitingForPlayers;

		private Dictionary<PlayerRef, NetworkObject> _players = new Dictionary<PlayerRef, NetworkObject>();

		private int _playerID;

        private void Update()
        {
            if (startGameManually)
                startGame ();
			
			startGameManually = false;
        }

        public override void Spawned()
        {
			base.Spawned();

			// If Runner is the server we set GameStatus to Waiting for players
            if (Runner.IsServer)
                _gameState = GameState.WaitingForPlayers;

			switch (_gameState)
			{
				case GameState.InGame:
					Debug.Log (">>>>>>>>>>>>>>>>>>>>");
					Debug.Log ("GAME STATE : InGame");
					Debug.Log (">>>>>>>>>>>>>>>>>>>>");
					break;

				case GameState.WaitingForPlayers:
					Debug.Log (">>>>>>>>>>>>>>>>>>>>");
					Debug.Log ("GAME STATE : WaitingForPlayers");
					Debug.Log (">>>>>>>>>>>>>>>>>>>>");
					break;

				case GameState.GameEnd:
					Debug.Log (">>>>>>>>>>>>>>>>>>>>");
					Debug.Log ("GAME STATE : GameEnd");
					Debug.Log (">>>>>>>>>>>>>>>>>>>>");
					break;
			}

			// On join spawn Player
			playerJoin ();
        }

		public void playerJoin()
		{
			// Increment player amount
			_playerCount++;
			// Set player ID
			_playerID = _playerCount;

			// Calculating spawn position with a random offset
			var randomPositionOffset = Random.insideUnitCircle * SpawnRadius;
			var spawnPosition = SpawnBase.position + new Vector3(randomPositionOffset.x, 0f, randomPositionOffset.y);

			// Spawn the player at the calculated position
			NetworkObject playerInstance = Runner.Spawn(PlayerPrefab, spawnPosition, Quaternion.identity, Runner.LocalPlayer);

			// Store the player instance for future reference (e.g., for moving, despawning, etc.)
			_players[Runner.LocalPlayer] = playerInstance;
		}

        [Rpc(RpcSources.All, RpcTargets.All)]
		public void RPC_movePlayersToSpawnPoint (int index) {

			// Loop through all players and move them to new positions
			foreach (var player in Runner.ActivePlayers)
			{
				// Check if the player is already spawned
				if (_players.ContainsKey(player))
				{
					NetworkObject playerObject = _players[player];

					// Despawn the player object
					Runner.Despawn(playerObject);

					var randomPositionOffset = Random.insideUnitCircle * SpawnRadius;
					var spawnPosition = SpawnPoints[index].position + new Vector3(randomPositionOffset.x, 0f, randomPositionOffset.y);

					// Spawn the player at the new position
					NetworkObject playerInstance = Runner.Spawn(PlayerPrefab, spawnPosition, Quaternion.identity, player);
					_players[player] = playerInstance;  // Store the player instance
				}
			}
		}

		public void startGame () {
			
			Debug.Log (">>> START GAME");

			_gameState = GameState.InGame;

			// Move players to spawn point
			int index = 0;
			RPC_movePlayersToSpawnPoint (_playerID);
		}

        public void PlayerDeath(Vector3 deathPosition, Quaternion deathOrientation)
        {
            RPC_RequestSpawnCorpse(deathPosition, deathOrientation);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RPC_RequestSpawnCorpse(Vector3 deathPosition, Quaternion deathOrientation)
        {
            Debug.Log("Death :" + deathPosition);
            Runner.Spawn(CorpsePrefab, deathPosition, deathOrientation, null);
        }
    }
}
