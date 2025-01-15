using UnityEngine;
using UnityEngine.UI;

namespace Starter.ThirdPersonCharacter
{
    public class PlayerNameplate : MonoBehaviour
    {
		private Camera cam;

		private void Awake ()
		{
			cam = Camera.main;
		}

		private void Update ()
		{
			if (cam == null)
				cam = Camera.main;

			if (cam != null)
				transform.forward = cam.transform.forward;
		}
    }
}
