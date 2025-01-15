using UnityEngine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;

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
        GameEnd = 3,
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

        [Header("SKIN LIST")]
        public List<NetworkObject> playersPrefabs;

        [Header("LOOT BOXES")]
		public List<LootBox> lootboxes;

        [Header("In Game HUD")]
		public Countdown timer;
        public GameObject _WinnerWindows;
        public GameObject _LooserWindows;
        public TextMeshProUGUI _playerInGame;
		public Annonceur annonceur;

        [Header("Falling Inslad Cycle Config")]
		public IlesQuiTombent fallingInslandManager;
		public int spawnFallingsTime;
		public int interFallingTime;
		public int timeBeforeReset;

		
        [Header("DEBUG TOOLS")]
        private bool startGameButton = false; // Use private field for backing
		
		// GAME STATUS
		[Networked] private PlayerRef _winningPlayer { get; set; }
		[Networked] private int _playerCount { get; set; } = 0;
        [Networked] private int _readyPlayerCount { get; set; } = 0; // Players ready
        [Networked] private GameState _gameState { get; set; } = GameState.WaitingForPlayers;

		// Save local player Instance
		private NetworkObject _localPlayerInstance;

		// Falling Insland Couroutine
		private Coroutine _inslandFallingCoroutine;

		// True if the object is spawned()
		private bool _spawned = false;
		
		// Check winner interval vars
		private float _timeSinceLastCheck = 0f;
		private const float _checkInterval = 5f; // Interval in seconds

        public AudioClip lobySong;
        private GameObject audioObject;
        private AudioSource audioSource;

        // Only Used for debug startGame
        private void Update()
        {
            if (startGameButton && _gameState != GameState.InGame)
                startGame ();
			
			startGameButton = false;

			if (!_spawned) return;

			// Get nb of player
			nbPlayer();

			// Check winner very 5 seconds
			if (_gameState == GameState.InGame)
			{
				// Increment the time
				_timeSinceLastCheck += Time.deltaTime;

				// Check for winner every 5 seconds
				if (_timeSinceLastCheck >= _checkInterval)
				{
					checkForWinner();
					_timeSinceLastCheck = 0f; // Reset the timer
				}
			}

			if (_gameState != GameState.InGame && !audioSource.isPlaying)
			{
                audioSource.Play();
            }

        }
		public void ActivestartGameButton()
		{
			startGameButton = true;
        }
        public override void Spawned()
        {
			base.Spawned();

			// Crée un objet temporaire
			audioObject = new GameObject("TemporaryAudio");
            audioSource = audioObject.AddComponent<AudioSource>();

            // Configure l'AudioSource en mode 2D
            audioSource.clip = lobySong;
            audioSource.spatialBlend = 0.0f; // Mode 2D
            audioSource.volume = 0.15f;
            audioSource.loop = true;

            _WinnerWindows.SetActive(false);
            _LooserWindows.SetActive(false);
            // If Runner is the server we set GameStatus to Waiting for players
            if (Runner.IsServer)
                _gameState = GameState.WaitingForPlayers;
			
			// On join spawn Player
			playerJoin ();

			_spawned = true;
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

			} else {
				Debug.Log (">>> Player dont have inventory");
			}

			// Store the player instance for future references 
			_localPlayerInstance = playerInstance;

			if (_gameState != GameState.WaitingForPlayers) {

				Runner.Despawn (_localPlayerInstance);

				// Set Player to spectator mode
				CameraSwitcher cameraSwitcher = FindObjectOfType<CameraSwitcher> ();
				cameraSwitcher.ToggleFreecam (true);
			}
		}

        private void AddItemsToPlayer(PlayerInventory inventory)
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
		private void RPC_movePlayerToSpawnPoint (int index, PlayerRef playerRef, int prefabIndex) {

			// If current client is not the player targeted we return
			if (Runner.LocalPlayer != playerRef)
				return;

			// Despawn the player object
			Runner.Despawn(_localPlayerInstance);

			var randomPositionOffset = Random.insideUnitCircle * SpawnRadius;
			var spawnPosition = SpawnPoints[index].position + new Vector3(randomPositionOffset.x, 0f, randomPositionOffset.y);
			index--;

			// Spawn the player at the new position
			NetworkObject playerInstance = Runner.Spawn(playersPrefabs[prefabIndex] ?? PlayerPrefab, spawnPosition, Quaternion.identity, Runner.LocalPlayer);
			_localPlayerInstance = playerInstance;  // Store the player instance

			// Ajouter des items au joueur
            PlayerInventory inventory = playerInstance.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                AddItemsToPlayer(inventory);
            }
		}

		public void respawnPlayers () {
			// Init a list of available indices (0 to 8)
			List<int> availableIndices = new List<int>();
			for (int i = 0; i < 8; i++) {
				availableIndices.Add(i);
			}

			// For all players available
			foreach (PlayerRef playerRef in Runner.ActivePlayers) {

				// if no indices are left, we return an error
				if (availableIndices.Count == 0) {
					Debug.LogError ("GAME MANAGER : Too many players for spawn points. CANNOT SPAWN ALL PLAYERS !!");
					return;
				}

				// Get a random index from the available indices
				int randomIndex = Random.Range(0, availableIndices.Count);

				int selectedIndex = availableIndices[randomIndex];
				availableIndices.RemoveAt(randomIndex);

				// Get skins !!! Or take default skin if not set
				NetworkObject prefab = playersPrefabs[selectedIndex];

				if (prefab == null)
					prefab = PlayerPrefab;

				// Move player to spawnPoint
				RPC_movePlayerToSpawnPoint(selectedIndex, playerRef, selectedIndex);
			}
		}

		public void startGame () {

			// Spawn players to spawn points
			respawnPlayers ();
			
			RPC_startFallingIslandCoroutine ();

            audioSource.Stop();
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
		public void RPC_startFallingIslandCoroutine () {

            // Update game State
            _gameState = GameState.InGame;
            // Clear annonceur
            annonceur.Annonce("");
			// Debut de la coroutine
			_inslandFallingCoroutine = StartCoroutine (startFallingIslandCycle ());
		}

		private IEnumerator startFallingIslandCycle ()
		{
			timer.gameObject.SetActive (true);
			// On fait tomber 2 fois des iles
			int maxRepeats = 2;
			// On initialise le nombre de répétition à zéro
			int repeatCount = 0;

            // Initialisation de l'annonceur avec temps de tremblement des îles
            annonceur.Init(timeBeforeReset);

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
                {
                    annonceur.Annonce("Les îles de spawn vont tomber !");
                    fallingInslandManager.fallIslands(IslandType.Spawn);
					timer.spawnIslandFell();
                }
				
				// Fall inter and plateformes
				if (repeatCount == 1)
                {
                    annonceur.Annonce("Les îles intermédiaires vont tomber !");
                    fallingInslandManager.fallIslands (IslandType.Plateformes);
					fallingInslandManager.fallIslands (IslandType.Inter);
					timer.intermediateIslandFell();
                }

				// Increment reapeat count
				repeatCount++;
			}
		}

		private void nbPlayer()
		{
            // Init number of players alive
            int numberPlayerAlive = 0;

            // Get All players game Object
            GameObject[] playersObjs = GameObject.FindGameObjectsWithTag("Player");

            // For all players check if there are alivek:
            foreach (GameObject playerObj in playersObjs)
            {

                Player player = playerObj.GetComponent<Player>();

                if (player != null && player.isSpawned && player.isAlive)
                {

                    numberPlayerAlive++;
                }
            }

            _playerInGame.text = string.Format("{0:#0}", numberPlayerAlive);
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
            _playerInGame.text = string.Format("{0:#0}", numberPlayerAlive);

			if (numberPlayerAlive <= 1) {
				
				Debug.Log ("THERE IS A WINNER");
				Debug.Log (numberPlayerAlive);

				// Set game state to end
				_gameState = GameState.GameEnd;

				// Set that we is the winner
				RPC_setWinningPlayer (lastPlayerAlive.Object.InputAuthority);

				// Start endGame couroutine
				StartCoroutine (endGame ());
			}
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_DeleteWeapon()
        {
            GameObject[] Weapons = GameObject.FindGameObjectsWithTag("Weapon");

            foreach (GameObject Weapon in Weapons)
            {
                NetworkObject NetworkWeapon = Weapon.GetComponent<NetworkObject>();

				if( NetworkWeapon != null){

                    Runner.Despawn (NetworkWeapon);
                }
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
		public void RPC_setWinningPlayer (PlayerRef playerRef) {
			
			_winningPlayer = playerRef;
		}

        private IEnumerator endGame () {
			
			_gameState = GameState.GameEnd;

			// Celebrate his win !
			RPC_celebrateWinner();

			yield return new WaitForSeconds(timeBeforeReset); // Wait for 5 minutes

			resetGame();
		}

        [Rpc(RpcSources.All, RpcTargets.All)]
		private async void RPC_celebrateWinner () {
			
			// TODO !!! MUST MAKE A BETTER CELEBRATION
			if (_winningPlayer == Runner.LocalPlayer) {

                _WinnerWindows.SetActive(true);
                Debug.Log ("             ");
				Debug.Log (">>>>>>>>>>>>>");
				Debug.Log ("             ");
				Debug.Log (" YOU WIN GG !");
				Debug.Log ("             ");
				Debug.Log (">>>>>>>>>>>>>");
				Debug.Log ("             ");

			} else {

                _LooserWindows.SetActive(true);
            }
		}

		private void resetGame ()
		{
			// Reset lootboxes
			resetAllLootBoxes ();
            // Respawn all players to base
            RPC_respawnPlayerToBase ();
            // Reset all corpse
            clearCorpse();
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
		private void RPC_respawnPlayerToBase () {

			// Set Win window to false
			_LooserWindows.SetActive(false);
			_WinnerWindows.SetActive(false);

			// Set
			annonceur.Annonce("En attente de joueur !");
			timer.gameObject.SetActive (false);
			
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

            // Set status = WaitingForPlayers
            _gameState = GameState.WaitingForPlayers;
        }

		private void resetAllLootBoxes ()
		{
			Debug.Log ("RESET ALL LOOT BOXES");
			for (int i = 0; i < lootboxes.Count; i++) {
				
				LootBox lootbox = lootboxes[i];
				lootbox.reset ();
			}
		}

		private void clearCorpse()
		{
			GameObject[] corpses = GameObject.FindGameObjectsWithTag("Corpse");

            foreach (GameObject corpse in corpses)
			{
				Destroy(corpse);
			}
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
            var corpse = Runner.Spawn(CorpsePrefab, deathPosition, deathOrientation, null);
            if (corpse != null)
			{
				corpse.tag = "Corpse";
            }
        }

		public GameState getGameState () {
			return _gameState;
		}
    }
}
