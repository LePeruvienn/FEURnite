using TMPro;
using UnityEngine;

namespace Starter
{
	/// <summary>
	/// Component that handle showing nicknames above player
	/// </summary>
	public class UINameplate : MonoBehaviour
	{
		public TextMeshProUGUI NicknameText;

		private Transform _cameraTransform;

		public void SetNickname(string nickname)
		{
			NicknameText.text = nickname;
		}

		private void Awake()
		{
			_cameraTransform = Camera.main.transform;
			NicknameText.text = string.Empty;
		}

		private void LateUpdate()
		{
			// Rotate nameplate toward camera
			transform.rotation = _cameraTransform.rotation;
		}
	}
}
