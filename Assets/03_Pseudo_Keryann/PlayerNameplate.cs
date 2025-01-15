using UnityEngine;
using UnityEngine.UI;

namespace Starter.ThirdPersonCharacter
{
    public class PlayerNameplate : MonoBehaviour
    {
        [SerializeField]
        private Text usernameText;

        void Update()
        {
            usernameText.text = "Bonjour";
        }
    }
}