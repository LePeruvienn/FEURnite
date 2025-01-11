using UnityEngine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

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
        InGame = 2,
        GameEnd = 2,
    }

    public sealed class GameManager : NetworkBehaviour
    {
        [Header("Game Manager Config")]
        public NetworkObject PlayerPrefab;
        [SerializeField] private List<NetworkPrefabRef> itemPrefabs;
        public NetworkObject CorpsePrefab;
		
        [Header("Game Manager Config")]
        public float SpawnRadius = 3f;
        public List<Transform> SpawnPoints;
        public Transform SpawnBase;
        public Transform SpawnSpectator;

        [Header("In Game HUD")]
		public Countdown timer;

        [Header("Falling Inslad Cycle Config")]
		public IlesQuiTombent fallingInslandManager;
		public int spawnFallingsTime;
		public int interFallingTime;
		public int timeBeforeReset;


        [Header("DEBUG TOOLS")]
        public bool startGameManually = false; // Use private field for backing
		
		// GAME STATUS
		[Networked] private int _playerCount { get; set; } = 0;
        [Networked] private int _readyPlayerCount { get; set; } = 0; // Players ready
        [Networked] private GameState _gameState { get; set; } = GameState.WaitingForPlayers;

		// Save local player Instance
		private NetworkObject _localPlayerInstance;

		// Falling Insland Couroutine
		private Coroutine _inslandFallingCoroutine;
		

		// Only Used for debug startGame
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

			// On join spawn Player
			playerJoin ();
        }

		public void playerJoin ()
		{
			// Set Spawn Point depending of the game State
			Transform spawnPoint = _gameState == GameState.WaitingForPlayers ?
				SpawnBase : SpawnSpectator;
			
			// Calculating spawn position with a random offset
			Vector3 randomPositionOffset = Random.insideUnitCircle * SpawnRadius;
			Vector3 spawnPosition = spawnPoint.position + new Vector3(randomPositionOffset.x, 0f, randomPositionOffset.y);

			// Spawn the player at the calculated position
			NetworkObject playerInstance = Runner.Spawn(PlayerPrefab, spawnPosition, Quaternion.identity, Runner.LocalPlayer);

			// Ajouter des items au joueur
            PlayerInventory inventory = playerInstance.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                AddItemsToPlayer(inventory);
            }

			// Store the player instance for future references 
			_localPlayerInstance = playerInstance;

			// Si le joueur rejoint une partie en cours
			if (_gameState != GameState.WaitingForPlayers) {

				// ...
				// Appliquer des changement pour mettre le joueur en spectateur
				// ...
			}
		}

        private void AddItemsToPlayer (PlayerInventory inventory)
        {
            GameObject[] createdItems = new GameObject[itemPrefabs.Count];
            int i = 0;
            foreach (var itemPrefab in itemPrefabs)
            {
                // Créer l'item
                NetworkObject item = Runner.Spawn(itemPrefab, Vector3.zero, Quaternion.identity,null);
                createdItems[i] = item.gameObject;
                // Ajouter l'item à l'inventaire du joueur
                inventory.AddItem(item,i);
                i++;
            }
            inventory.initAdd(createdItems);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
		private void RPC_movePlayerToSpawnPoint (int index, PlayerRef playerRef) {

			// If current client is not the player targeted we return
			if (Runner.LocalPlayer != playerRef)
				return;

			// Despawn the player object
			Runner.Despawn(_localPlayerInstance);

			var randomPositionOffset = Random.insideUnitCircle * SpawnRadius;
			var spawnPosition = SpawnPoints[index].position + new Vector3(randomPositionOffset.x, 0f, randomPositionOffset.y);
			index--;

			// Spawn the player at the new position
			NetworkObject playerInstance = Runner.Spawn(PlayerPrefab, spawnPosition, Quaternion.identity, Runner.LocalPlayer);
			_localPlayerInstance = playerInstance;  // Store the player instance

			// Ajouter des items au joueur
            PlayerInventory inventory = playerInstance.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                AddItemsToPlayer(inventory);
            }
		}

		public void respawnPlayers () {
			// Init lastIndex avaible
			int lastIndex = SpawnPoints.Count - 1;

			// For all players avaibles
			foreach (PlayerRef playerRef in Runner.ActivePlayers) {
				
				// if index is out of array's bound we return an error
				if (lastIndex < 0) {
					
					Debug.LogError ("GAME MANAGER : Too much players for spawn points. CANNOT SPAWN ALL PLAYER !!");
					return;
				}
				// Move player to spawnPoint
				RPC_movePlayerToSpawnPoint (lastIndex, playerRef);
				// Update last index
				lastIndex--;
			}
		}

		public void startGame () {
			
			Debug.Log (">>> START GAME");

			// Update game State
			_gameState = GameState.InGame;

			// Spawn players to spawn points
			respawnPlayers ();
			
			RPC_startFallingIslandCoroutine ();
		}

        [Rpc(RpcSources.All, RpcTargets.All)]
		public void RPC_startFallingIslandCoroutine () {

			_inslandFallingCoroutine = StartCoroutine (startFallingIslandCycle ());
		}

		private IEnumerator startFallingIslandCycle ()
		{
			// On fait tomber 2 fois des iles
			int maxRepeats = 2;
			// On initialise le nombre de répétition à zéro
			int repeatCount = 0;

			while (repeatCount < maxRepeats)
			{
				// taking the right waitimg time
				int timeToWait = repeatCount == 0 ?
					spawnFallingsTime : interFallingTime;

				// StartTimer
				timer.InitializeTimer (timeToWait);
				timer.ResumeTimer ();

				// Wait
				yield return new WaitForSeconds(timeToWait); // Wait for 5 minutes

				// First fall spawns
				if (repeatCount == 0)
					fallingInslandManager.fallInslands (InslandType.Spawn);
				
				// Fall inter and plateformes
				if (repeatCount == 1){
					fallingInslandManager.fallInslands (InslandType.Plateformes);
					fallingInslandManager.fallInslands (InslandType.Inter);
				}

				// Increment reapeat count
				repeatCount++;
			}

			Debug.Log(">>> GAME STATUS MESSAGE : ALL ISLANDS ARE GONE !!");
		}

		private void checkForWinner ()
		{
			// Init number of players alive
			int numberPlayerAlive = 0;
			Player lastPlayerAlive = null;

			// Get All players game Object
			GameObject[] playersObjs = GameObject.FindGameObjectsWithTag ("Player");

			// For all players check if there are alivek:
			foreach (GameObject playerObj in playersObjs) {
				
				Player player = playerObj.GetComponent<Player> ();

				if (player != null && player.isAlive) {

					numberPlayerAlive++;
					lastPlayerAlive = player;
				}
			}

			Debug.Log ("numberPlayerAlive = " + numberPlayerAlive);

			if (numberPlayerAlive == 1 && lastPlayerAlive != null) {
				
				Debug.Log ("THERE IS A WINNER");

				// Set that we is the winner
				lastPlayerAlive.isWinner = true;

				// Start endGame couroutine
				StartCoroutine (endGame ());
			}
		}

		private IEnumerator endGame () {
			
			_gameState = GameState.GameEnd;

			// Celebrate his win !
			RPC_celebrateWinner();

			yield return new WaitForSeconds(timeBeforeReset); // Wait for 5 minutes

			resetGame();
		}

        [Rpc(RpcSources.All, RpcTargets.All)]
		private void RPC_celebrateWinner () {
			
			// GET LOCAL PLAYER
			Player player = _localPlayerInstance.GetComponent<Player> ();
			
			// TODO !!! MUST MAKE A BETTER CELEBRATION
			if (player.isWinner) {

				Debug.Log ("             ");
				Debug.Log (">>>>>>>>>>>>>");
				Debug.Log ("             ");
				Debug.Log (" YOU WIN GG !");
				Debug.Log ("             ");
				Debug.Log (">>>>>>>>>>>>>");
				Debug.Log ("             ");

			} else {

				Debug.Log (" YOU LOSE :'(");
			}
				
		}

		private void resetGame ()
		{
			RPC_respawnPlayerToBase ();

			// Set status = WaitingForPlayers
			_gameState = GameState.WaitingForPlayers;
		}

        [Rpc(RpcSources.All, RpcTargets.All)]
		private void RPC_respawnPlayerToBase () {

			StopCoroutine (_inslandFallingCoroutine);
			fallingInslandManager.resetAll ();

			// Despawn the player object if is set
			if (_localPlayerInstance != null)
				Runner.Despawn(_localPlayerInstance);

			// Switch Camera
            CameraSwitcher cameraSwitcher = FindObjectOfType<CameraSwitcher>();
			cameraSwitcher.ToggleFreecam (false);

			var randomPositionOffset = Random.insideUnitCircle * SpawnRadius;
			var spawnPosition = SpawnBase.position + new Vector3(randomPositionOffset.x, 0f, randomPositionOffset.y);

			// Spawn the player at the new position
			NetworkObject playerInstance = Runner.Spawn(PlayerPrefab, spawnPosition, Quaternion.identity, Runner.LocalPlayer);
			_localPlayerInstance = playerInstance;  // Store the player instance

			// Ajouter des items au joueur
            PlayerInventory inventory = playerInstance.GetComponent<PlayerInventory>();

			// Add items to inventory
            if (inventory != null)
                AddItemsToPlayer(inventory);
		}

        public void PlayerDeath(Vector3 deathPosition, Quaternion deathOrientation)
        {
			// Check if a player win the game
			if (_gameState == GameState.InGame)
				checkForWinner ();
			
			// Spawn corpse
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
