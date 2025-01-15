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

        public void Setup(Player player)
        {
            usernameText.text = PlayerPrefs.GetString("PlayerName");
            killsText.text = "Kills : " + 1;
            deathsText.text = "Deaths : " + 1;
        }

    }
}