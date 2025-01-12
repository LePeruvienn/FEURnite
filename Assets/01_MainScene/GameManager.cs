using UnityEngine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Starter.ThirdPersonCharacter
{
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
        public NetworkObject CorpsePrefab;
        [SerializeField] private List<NetworkPrefabRef> itemPrefabs;

        [Header("Spawn Config")]
        public float SpawnRadius = 3f;
        public List<Transform> SpawnPoints;
        public Transform SpawnBase;
        public Transform SpawnSpectator;

        [Header("UI Elements")]
        public GameObject _WinnerWindows;
        public GameObject _LooserWindows;
        public TextMeshProUGUI _playerInGame;

        [Header("Island Config")]
        public IlesQuiTombent fallingIslandManager;
        public Countdown timer;
        public int spawnFallingsTime;
        public int interFallingTime;
        public int timeBeforeReset;

        [Header("Debug")]
        public bool startGameManually = false;

        [Networked(OnChanged = nameof(OnWinnerChanged))] private PlayerRef _winningPlayer { get; set; }
        [Networked] private int _playerCount { get; set; } = 0;
        [Networked] private GameState _gameState { get; set; } = GameState.WaitingForPlayers;

        private NetworkObject _localPlayerInstance;
        private Coroutine _islandFallingCoroutine;
        private bool _spawned = false;

        private float _timeSinceLastCheck = 0f;
        private const float _checkInterval = 5f;

        private void Update()
        {
            if (startGameManually)
            {
                startGame();
                startGameManually = false;
            }

            if (_spawned && _gameState == GameState.InGame)
            {
                _timeSinceLastCheck += Time.deltaTime;

                if (_timeSinceLastCheck >= _checkInterval)
                {
                    checkForWinner();
                    _timeSinceLastCheck = 0f;
                }
            }
        }

        public override void Spawned()
        {
            base.Spawned();
            _WinnerWindows.SetActive(false);
            _LooserWindows.SetActive(false);

            if (Runner.IsServer)
                _gameState = GameState.WaitingForPlayers;

            playerJoin();
            _spawned = true;
        }

        private void playerJoin()
        {
            Transform spawnPoint = _gameState == GameState.WaitingForPlayers ? SpawnBase : SpawnSpectator;
            Vector3 randomPositionOffset = Random.insideUnitCircle * SpawnRadius;
            Vector3 spawnPosition = spawnPoint.position + new Vector3(randomPositionOffset.x, 0f, randomPositionOffset.y);

            NetworkObject playerInstance = Runner.Spawn(PlayerPrefab, spawnPosition, Quaternion.identity, Runner.LocalPlayer);
            _localPlayerInstance = playerInstance;

            PlayerInventory inventory = playerInstance.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                AddItemsToPlayer(inventory);
            }
        }

        private void AddItemsToPlayer(PlayerInventory inventory)
        {
            GameObject[] createdItems = new GameObject[itemPrefabs.Count];
            int i = 0;

            foreach (var itemPrefab in itemPrefabs)
            {
                NetworkObject item = Runner.Spawn(itemPrefab, Vector3.zero, Quaternion.identity, null);
                createdItems[i] = item.gameObject;
                inventory.AddItem(item, i);
                i++;
            }

            inventory.initAdd(createdItems);
        }

        private void checkForWinner()
        {
            int numberPlayerAlive = 0;
            Player lastPlayerAlive = null;

            GameObject[] playersObjs = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject playerObj in playersObjs)
            {
                Player player = playerObj.GetComponent<Player>();
                if (player != null && player.isAlive)
                {
                    numberPlayerAlive++;
                    lastPlayerAlive = player;
                }
            }

            Debug.Log("Number of players alive: " + numberPlayerAlive);
            _playerInGame.text = numberPlayerAlive.ToString();

            if (numberPlayerAlive <= 1 && _winningPlayer == default)
            {
                if (lastPlayerAlive != null)
                {
                    _winningPlayer = lastPlayerAlive.Object.InputAuthority;
                }

                StartCoroutine(endGame());
            }
        }

        private IEnumerator endGame()
        {
            _gameState = GameState.GameEnd;
            yield return new WaitForSeconds(timeBeforeReset);
            resetGame();
        }

        private static void OnWinnerChanged(Changed<GameManager> changed)
        {
            changed.Behaviour.DisplayWinnerOrLoser();
        }

        private void DisplayWinnerOrLoser()
        {
            if (Runner.LocalPlayer == _winningPlayer)
            {
                _WinnerWindows.SetActive(true);
                Debug.Log("YOU WIN!");
            }
            else
            {
                _LooserWindows.SetActive(true);
                Debug.Log("YOU LOSE!");
            }
        }

        private void resetGame()
        {
            RPC_respawnPlayersToBase();
            _gameState = GameState.WaitingForPlayers;
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RPC_respawnPlayersToBase()
        {
            _WinnerWindows.SetActive(false);
            _LooserWindows.SetActive(false);

            StopCoroutine(_islandFallingCoroutine);
            fallingIslandManager.resetAll();

            if (_localPlayerInstance != null)
                Runner.Despawn(_localPlayerInstance);

            Vector3 randomPositionOffset = Random.insideUnitCircle * SpawnRadius;
            Vector3 spawnPosition = SpawnBase.position + new Vector3(randomPositionOffset.x, 0f, randomPositionOffset.y);

            NetworkObject playerInstance = Runner.Spawn(PlayerPrefab, spawnPosition, Quaternion.identity, Runner.LocalPlayer);
            _localPlayerInstance = playerInstance;

            PlayerInventory inventory = playerInstance.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                AddItemsToPlayer(inventory);
            }
        }
    }
}
