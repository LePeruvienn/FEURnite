using UnityEngine;
using UnityEngine.UI;

namespace Starter.ThirdPersonCharacter
{
    public class PlayerScoreboardItem : MonoBehaviour
    {

        [SerializeField]
        Text usernameText;

        [SerializeField]
        Text killsText;

        [SerializeField]
        Text deathsText;

        public void Setup (Player player)
        {
            usernameText.text = player.name;
			killsText.text = "";
            deathsText.text = player.isAlive ? "Alive" : "DEAD!";
        }

    }
}
