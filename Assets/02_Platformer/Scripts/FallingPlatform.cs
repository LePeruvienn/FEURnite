using Fusion;
using UnityEngine;

namespace Starter.Platformer
{
	/// <summary>
	/// A platform that falls when player steps onto it.
	/// </summary>

	// The execution order is respected by Fusion as well. We need FallingPlatform to tick before Player (execution order 0) so
	// platform collider is correctly actived/deactivated before player's move is executed. Otherwise it would cause issues in resimulation.
	[DefaultExecutionOrder(-10)]
	public class FallingPlatform : NetworkBehaviour
	{
		[Header("Setup")]
		public float FallDelay = 0.2f;
		public float ReactivationDelay = 1f;

		[Header("References")]
		public Collider Collider;
		public Rigidbody Platform;

		[Header("Audio")]
		public AudioClip FallAudioClip;
		public float FallAudioVolume = 1f;

		[Networked]
		private NetworkBool _isActive { get; set; } = true;
		[Networked]
		private TickTimer _cooldown { get; set; }

		private Vector3 _originalPosition;

		public override void Spawned()
		{
			// Save original platform position, so we can reset position
			// when platform gets reactivated
			_originalPosition = Platform.transform.position;
		}

		public override void FixedUpdateNetwork()
		{
			if (_cooldown.Expired(Runner) == true)
			{
				if (_isActive)
				{
					// Schedule reactivation
					_isActive = false;
					_cooldown = TickTimer.CreateFromSeconds(Runner, ReactivationDelay);
				}
				else
				{
					// Platform is active again
					_isActive = true;
					_cooldown = default;
				}
			}

			// Synchronize isActive state with actual collider state
			Collider.enabled = _isActive;
		}

		public override void Render()
		{
			// We are not waiting until _isActive state comes from state authority
			// but we estimate the current correct state based on the cooldown expiration.
			// This ensures that clients don't have an advantage (otherwise their time to
			// jump before the platform falls would be FallDelay + Ping).
			bool isActive = _cooldown.Expired(Runner) ? !_isActive : _isActive;

			if (Platform.isKinematic == isActive)
				return; // No change

			// Synchronize isActive state with actual collider and platform state
			Platform.isKinematic = isActive;
			Collider.enabled = isActive;

			if (isActive)
			{
				// Reset fallen platform to its original position
				Platform.transform.position = _originalPosition;
			}
			else
			{
				AudioSource.PlayClipAtPoint(FallAudioClip, transform.position, FallAudioVolume);
				Platform.AddForce(Vector3.down * 30f, ForceMode.Impulse);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			// Platforms falling is initiated only on state authority
			if (HasStateAuthority == false)
				return;

			if (_isActive == false)
				return;

			if (_cooldown.IsRunning == true)
				return; // Falling is already initiated

			if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
				return;

			_cooldown = TickTimer.CreateFromSeconds(Runner, FallDelay);
		}
	}
}
