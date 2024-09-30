using UnityEngine;

namespace Starter
{
	/// <summary>
	/// Simple component that rotates object. Used for Coins.
	/// </summary>
	public class RotateObject : MonoBehaviour
	{
		public float Speed = 90f;
		public bool RandomizeStart = true;

		private void Awake()
		{
			if (RandomizeStart)
			{
				transform.Rotate(0f, Random.Range(0f, 360f), 0f);
			}
		}

		private void Update()
		{
			transform.Rotate(0f, Speed * Time.deltaTime, 0f);
		}
	}
}
