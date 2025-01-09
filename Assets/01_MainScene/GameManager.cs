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
		[Networked] private int _playerCount { get; set; } = 0;
        [Networked] private int _readyPlayerCount { get; set; } = 0; // Players ready
        [Networked] private GameState _gameState { get; set; } = GameState.WaitingForPlayers;
		
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

			Debug.Log (">>> GAME STATE");
			Debug.Log (_gameState);
			// Handle On join
			playerJoin ();
        }

		public void playerJoin () {

            Transform selectedSpawnPoint;

            if (SpawnPoints == null || SpawnPoints.Count == 0)
            {
                selectedSpawnPoint = SpawnBase;
            }
            else
            {
                // Sélectionne le point de spawn basé sur _playerCount pour éviter les doublons
                var spawnIndex = _playerCount % SpawnPoints.Count;
                selectedSpawnPoint = SpawnPoints[spawnIndex];

                // Incrémente le compteur pour le prochain joueur
              
            }
            _playerCount++;

            // Calcul de la position avec un décalage aléatoire
            var randomPositionOffset = Random.insideUnitCircle * SpawnRadius;
            var spawnPosition = selectedSpawnPoint.position + new Vector3(randomPositionOffset.x, 0f, randomPositionOffset.y);

            // Spawn du joueur à la position calculée
            Runner.Spawn(PlayerPrefab, spawnPosition, Quaternion.identity, Object.InputAuthority);
		}

		public void startGame () {
			
			Debug.Log (">>> START GAME");

			_gameState = GameState.InGame;
		}

		private void spawnPlayers () {

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
