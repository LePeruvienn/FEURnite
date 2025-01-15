using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Starter.ThirdPersonCharacter
{
    public class Scoreboard : MonoBehaviour
    {
        [SerializeField]
        GameObject playerScoreboardItem;

        [SerializeField]
        Transform playerScoreboardList;

        private float updateInterval = 1f; // Time interval in seconds
        private float timeSinceLastUpdate = 0f;

        private void Update()
        {
            // Increment the time since the last update
            timeSinceLastUpdate += Time.deltaTime;

            // Check if the update interval has passed
            if (timeSinceLastUpdate >= updateInterval)
            {
                timeSinceLastUpdate = 0f; // Reset the timer
                UpdateScoreboard(); // Call the update method
            }
        }

        private void UpdateScoreboard()
        {
            foreach (Transform child in playerScoreboardList)
            {
                Destroy (child.gameObject);
            }

            // Find all objects with the "Player" tag
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            // Populate the scoreboard with current players
            foreach (GameObject obj in players)
            {
				Player player = obj.GetComponent<Player> ();
                GameObject itemGO = Instantiate(playerScoreboardItem, playerScoreboardList);
                PlayerScoreboardItem item = itemGO.GetComponent<PlayerScoreboardItem>();
                if (item != null && player != null)
                {
                    // Pass the player object to the Setup method
                    item.Setup(player);
                }
            }
        }
    }
}
