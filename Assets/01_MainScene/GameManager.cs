using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Starter.ThirdPersonCharacter;

namespace Starter.ThirdPersonCharacter
{
    /// <summary>
    /// Manages player connections, spawning of Player instances, and item management.
    /// </summary>
    public sealed class GameManager : NetworkBehaviour
    {
        public NetworkObject PlayerPrefab; // Prefab for player instantiation
        public float SpawnRadius = 3f; // Radius for random player spawn position
        public List<NetworkObject> starterItems; // Prefabs of items to give to the player
        public NetworkObject test;

        public override void Spawned()
        {
            // Spawn player at a random position
            var randomPositionOffset = Random.insideUnitCircle * SpawnRadius;
            var spawnPosition = transform.position + new Vector3(randomPositionOffset.x, transform.position.y, randomPositionOffset.y);

            NetworkObject playerNetworkObject = Runner.Spawn(PlayerPrefab, spawnPosition, Quaternion.identity, Runner.LocalPlayer);
            PlayerInventory playerInventory = playerNetworkObject.GetComponent<PlayerInventory>();
             spawnPosition = transform.position + new Vector3(randomPositionOffset.x, transform.position.y, randomPositionOffset.y);
             Runner.Spawn(test, spawnPosition, Quaternion.identity, Runner.LocalPlayer);
            // Spawn and add starter items to the player's inventory
            if (playerInventory != null)
            {
                SpawnAndAddStarterItems(playerInventory);
            }
        }

        private void SpawnAndAddStarterItems(PlayerInventory playerInventory)
        {
            if (starterItems == null || starterItems.Count == 0)
            {
                Debug.LogWarning("La liste des items de départ est vide ou non assignée.");
                return;
            }

            foreach (NetworkObject itemPrefab in starterItems)
            {
                if (itemPrefab == null)
                {
                    Debug.LogError("Un des préfabriqués dans starterItems est nul.");
                    continue;
                }

                // Calculer la position de spawn pour l'item
                var itemSpawnPosition = transform.position + Random.insideUnitSphere * 1.0f;

                // Spawner l'item dans le monde du jeu
                NetworkObject itemInstance = Runner.Spawn(itemPrefab, itemSpawnPosition, Quaternion.identity, Runner.LocalPlayer);

                // Ajouter l'item à l'inventaire du joueur
                bool addedSuccessfully = playerInventory.addItem(itemInstance);

                if (!addedSuccessfully)
                {
                    Runner.Despawn(itemInstance); // Utiliser Despawn pour les objets en réseau
                    Debug.LogWarning("L'inventaire est plein. Impossible d'ajouter l'item : " + itemPrefab.name);
                }
            }
        }


        private void OnDrawGizmosSelected()
        {
            // Visualize spawn area in the editor
            Gizmos.DrawWireSphere(transform.position, SpawnRadius);
        }
    }
}
