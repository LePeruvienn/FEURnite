using UnityEngine;
using UnityEngine.UI;

namespace Starter.ThirdPersonCharacter
{
    public class PlayerNameplate : MonoBehaviour
    {
		private Camera cam;

		private void Awake()
		{
			cam = Camera.main;
		}

		private void Update()
		{
			transform.forward = cam.transform.forward;
		}
    }
}
