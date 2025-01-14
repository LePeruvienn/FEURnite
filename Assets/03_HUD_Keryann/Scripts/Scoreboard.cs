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

        private void OnEnable()
        {
            string[] players = { "p1", "p2", "p3" };

            foreach (string player in players)
            {
                GameObject itemGO = Instantiate(playerScoreboardItem, playerScoreboardList);
                PlayerScoreboardItem item = itemGO.GetComponent<PlayerScoreboardItem>();
                if (item != null)
                {
                    item.Setup(Player player);
                }
            }
        }

        private void OnDisable()
        {
            foreach (Transform child in playerScoreboardList)
            {
                Destroy(child.gameObject);
            }
        }
    }
}