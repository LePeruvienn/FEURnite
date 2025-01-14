using Starter.ThirdPersonCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Fusion;

namespace Starter.ThirdPersonCharacter
{
    ///// PARTIE DES EFFETS !!

    // Enum for effectStats
    public enum EffectStat 
	{
		Heal = 1,
		JumpPower = 2,
		Speed = 3,
		Shield = 4
	}

	// Structures des effets
	[System.Serializable]
	public class Effect
	{
		public EffectStat stats;
		public int amount;
		public int duration;
	}
	
	///// PARTIE USABLE

	// Enum for Usables states 
	public enum UsableState
	{
		Ready = 1,
		Using = 2
    }

    public class Usable : Item
    {
		[Header("Usable Stats")]
        public int useCooldown; // Time to wait without interuption to use the usable

		[Header("Effect Config")]
        public List<Effect> effects = new List<Effect> ();

		public AudioSource audioSource;

		public static AudioClip HealSound;   
		public static AudioClip JumpSpeedSound;
		public static AudioClip ShieldSound;


        // Privates
        private Animator _playerAnimator;
        private UsableState _currentUsableState;
		private PlayerInventory _playerInverntory;
		private PlayerModel _playerModel;
	
		

	
		[Rpc(RpcSources.All, RpcTargets.All)]
		public void RPC_PlaySound(int soundId)
		{
			AudioClip soundToPlay = null;

			// Associe l'ID du son au bon clip audio
			switch (soundId)
			{
				case 1:
					soundToPlay = HealSound;
					break;
				case 2:
					soundToPlay = JumpSpeedSound;
					break;
				case 3:
					soundToPlay = ShieldSound;
					break;
				default:
					Debug.LogWarning("Unknown sound ID");
					break;
			}
			

			if (audioSource != null && soundToPlay != null)
			{
				audioSource.PlayOneShot(soundToPlay);
			}
		}



		// Run when program starts
		public void Start ()
		{
			 HealSound = Resources.Load<AudioClip>("SoundEffects/HealSound");
			 JumpSpeedSound = Resources.Load<AudioClip>("SoundEffects/JumpSpeedSound");
			 ShieldSound = Resources.Load<AudioClip>("SoundEffects/ShieldSound");

			// Set state to ready
			_currentUsableState = UsableState.Ready;
			
		}

        public override ItemType getType()
        {
            return ItemType.Usable;
        }

        public override BulletType getBulletType()
        {
            return BulletType.Pistol;
        }

        public override void use()
        {
			// If usable state is already Using stop
			if (_currentUsableState == UsableState.Using)
				return;

			// Set state to using
			_currentUsableState = UsableState.Using;

            // Getting PlayerAnimator
            if (_playerAnimator == null)
				_playerAnimator = GetComponentInParent<Animator>();

            // Start use couroutine
            StartCoroutine (useCouroutine ());
			if (effects.Count > 0)
			{
				Effect firstEffect = effects[0];
				int soundId = -1;

				// Associate the sound ID with the effect
				switch (firstEffect.stats)
				{
					case EffectStat.Heal:
						soundId = 1;
						break;
					case EffectStat.JumpPower:
						soundId = 2;
						break;
					case EffectStat.Shield:
						soundId = 3;
						break;
					default:
						break;
				}

				// Play the sound via RPC
				if (soundId != -1)
				{
					
					RPC_PlaySound(soundId);

				}
			}
			
			
        }

		private IEnumerator useCouroutine ()
        {
            //Realod animation
            _playerAnimator.SetTrigger("EatTrigger");
	
			// Wait cooldown
			yield return new WaitForSeconds(useCooldown);

			// Apply effect to player
			applyEffects();

			// Set state to ready
			_currentUsableState = UsableState.Ready;
		}

		private void applyEffects ()
		{
			// For earch effect we applie his effect to the player
			for (int i = 0; i < effects.Count; i ++)
			{
				// Get effects attributes
				EffectStat stat = effects[i].stats;
				int amount = effects[i].amount;
				int duration = effects[i].duration;

				// Get player model if null
				if (_playerModel == null)
					_playerModel = GetComponentInParent<PlayerModel> ();

				// Apply effect to the right player's stat
				switch (stat)
				{
					case EffectStat.Heal:
						// Heal Player
						_playerModel.heal (amount);
						// Stop
						break;

					case EffectStat.JumpPower:
						// Add JumForce to player
						_playerModel.jumpPower += amount;
						// Stop
						break;

					case EffectStat.Speed:
						// Add Speed to player
						_playerModel.speed += amount;
						// Stop
						break;

					case EffectStat.Shield:
						// Add shield to player
						_playerModel.addShield (amount);
						// Stop
						break;
				}
			}

			// Debug message (Remove it later)
			Debug.Log ("Effects Applied !");

			// Get Player inventory if null
			if (_playerInverntory == null)
				_playerInverntory = GetComponentInParent<PlayerInventory> ();

			// Destroy objet
			_playerInverntory.destoryCurrentSelection ();
		}
    }
}
