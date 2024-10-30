using Fusion;
using UnityEngine;
using UnityEngine.Events;

namespace Starter.Platformer
{
	/// <summary>
	/// Flag object that is used as player finish when min amount of coins is collected.
	/// </summary>
	public class Flag : NetworkBehaviour
	{
		public UnityEvent<Player> FlagReached;

		private void OnTriggerEnter(Collider other)
		{
			// Flag check is triggered only on state authority
			if (HasStateAuthority == false)
				return;

			var player = other.transform.parent != null ? other.transform.parent.GetComponent<Player>() : null;
			if (player != null)
			{
				FlagReached?.Invoke(player);
			}
		}
	}
}
