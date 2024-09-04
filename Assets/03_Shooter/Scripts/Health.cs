using System;
using Fusion;
using UnityEngine;

namespace Starter.Shooter
{
	/// <summary>
	/// A common component that represents entity health.
	/// It is used for both players and chickens.
	/// </summary>
	public class Health : NetworkBehaviour
	{
		[Header("Setup")]
		public int InitialHealth = 3;
		public float DeathTime;

		[Header("References")]
		public Transform ScalingRoot;
		public GameObject VisualRoot;
		public GameObject DeathRoot;

		public Action<Health> Killed;

		public bool IsAlive => CurrentHealth > 0;
		public bool IsFinished => _networkHealth <= 0 && _deathCooldown.Expired(Runner);
		public int CurrentHealth => HasStateAuthority ? _networkHealth : _localHealth;

		[Networked]
		private int _networkHealth { get; set; }
		[Networked]
		private TickTimer _deathCooldown { get; set; }

		private int _lastVisibleHealth;
		private int _localHealth;
		private int _localDataExpirationTick;

		public void TakeHit(int damage, bool reportKill = false)
		{
			if (IsAlive == false)
				return;

			RPC_TakeHit(damage, reportKill);

			if (HasStateAuthority == false)
			{
				// To have responsive hit reactions on all clients we trust
				// local health value for some time after the health change
				_localHealth =  Mathf.Max(0, _localHealth - damage);
				_localDataExpirationTick = GetLocalDataExpirationTick();
			}
		}

		public void Revive()
		{
			_networkHealth = InitialHealth;
			_deathCooldown = default;
		}

		public override void Spawned()
		{
			if (HasStateAuthority)
			{
				// Set initial health
				Revive();
			}

			_localHealth = _networkHealth;
			_lastVisibleHealth = _networkHealth;
		}

		public override void Despawned(NetworkRunner runner, bool hasState)
		{
			Killed = null;
		}

		public override void Render()
		{
			if (Object.LastReceiveTick >= _localDataExpirationTick)
			{
				// Local health data expired, just use network health from now on
				_localHealth = _networkHealth;
			}

			VisualRoot.SetActive(IsAlive && IsAliveInterpolated());
			DeathRoot.SetActive(IsAlive == false);

			// Check if hit should be shown
			if (_lastVisibleHealth > CurrentHealth)
			{
				// Show hit reaction by simple scale (but not for local player).
				// Scaling root scale is lerped back to one in the Player script.
				if (HasStateAuthority == false && ScalingRoot != null)
				{
					ScalingRoot.localScale = new Vector3(0.85f, 1.15f, 0.85f);
				}
			}

			_lastVisibleHealth = CurrentHealth;
		}

		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		private void RPC_TakeHit(int damage, bool reportKill = false, RpcInfo info = default)
		{
			if (IsAlive == false)
				return;

			_networkHealth -= damage;

			if (IsAlive == false)
			{
				// Entity died, let's start death cooldown
				_networkHealth = 0;
				_deathCooldown = TickTimer.CreateFromSeconds(Runner,  DeathTime);

				if (reportKill)
				{
					// We are using targeted RPC to send kill confirmation
					// only to the killer client
					RPC_KilledBy(info.Source);
				}
			}
		}

		[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
		private void RPC_KilledBy([RpcTarget] PlayerRef playerRef)
		{
			Killed?.Invoke(this);
		}

		private int GetLocalDataExpirationTick()
		{
			// How much time it takes to receive response from the server
			float expirationTime = (float)Runner.GetPlayerRtt(Runner.LocalPlayer);

			// Additional safety 200 ms
			expirationTime += 0.2f;

			int expirationTicks = Mathf.CeilToInt(expirationTime * Runner.TickRate);
			//Debug.Log($"Expiration time {expirationTime}, ticks {expirationTicks}");

			return Runner.Tick + expirationTicks;
		}

		private bool IsAliveInterpolated()
		{
			// We use interpolated value when checking if object should be made visible in Render.
			// This helps with showing player visual at the correct position right away after respawn
			// (= player won't be visible before KCC teleport that is interpolated as well).
			var interpolator = new NetworkBehaviourBufferInterpolator(this);
			return interpolator.Int(nameof(_networkHealth)) > 0;
		}
	}
}
