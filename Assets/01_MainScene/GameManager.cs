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
    /// 

    public sealed class GameManager : NetworkBehaviour
    {
        public NetworkObject PlayerPrefab;
        public NetworkObject CorpsePrefab;
        public float SpawnRadius = 3f;
        public List<Transform> SpawnPoints;
        public Transform SpawnBase;
        [Networked] private int _playerCount { get; set; } = 0;
        // Variable locale pour vérifier si le joueur est déjà spawné
       
        public override void Spawned()
        {
            
            

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

        private void OnDrawGizmosSelected()
        {
            if (SpawnPoints != null)
            {
                foreach (var spawnPoint in SpawnPoints)
                {
                    if (spawnPoint != null)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireSphere(spawnPoint.position, SpawnRadius);
                    }
                }
            }
        }

        public void PlayerDeath(Vector3 deathPosition, Quaternion deathOrientation)
        {
            RPC_RequestSpawnCorpse(deathPosition, deathOrientation);
            Runner.Spawn(PlayerPrefab, deathPosition, deathOrientation, Object.InputAuthority);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RPC_RequestSpawnCorpse(Vector3 deathPosition, Quaternion deathOrientation)
        {
            Debug.Log("Death :" + deathPosition);
            Runner.Spawn(CorpsePrefab, deathPosition, deathOrientation, null);
        }
    }
}
