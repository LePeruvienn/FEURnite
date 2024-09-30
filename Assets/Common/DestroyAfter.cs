using UnityEngine;

namespace Starter
{
	/// <summary>
	/// Simple component that destroys gameobject after specified time.
	/// </summary>
	public class DestroyAfter : MonoBehaviour
	{
		public float DestroyTime = 2f;

		private void Update()
		{
			DestroyTime -= Time.deltaTime;

			if (DestroyTime <= 0f)
			{
				Destroy(gameObject);
			}
		}
	}
}
