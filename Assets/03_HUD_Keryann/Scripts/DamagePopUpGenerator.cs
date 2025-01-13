using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Starter.ThirdPersonCharacter
{
	public class DamagePopUpGenerator : MonoBehaviour
	{
		public static DamagePopUpGenerator current;
		public GameObject prefab;

		private void Awake()
		{
			current = this;
		}

		public void CreatePopUp(Vector3 position, string text, Color color)
		{
			var popup = Instantiate(prefab, position, Quaternion.identity);
			var temp = popup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
			temp.text = text;
			temp.faceColor = color;

			//Destroy Timer
			Destroy(popup, 1f);
		}
	}
}
