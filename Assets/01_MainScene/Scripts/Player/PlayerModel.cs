using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Starter.ThirdPersonCharacter
{
	public class PlayerModel : NetworkBehaviour
	{
        [Header("Script bar")]
        private Bar HealthBar;
        private Bar ShieldBar;
        private Shield SuperShieldBar;


        [Header("Player's Health")]
		public int startHealth;
		public int maxHealth;

        [Header("Player's Shield")]
		public int startShield;
		public int maxShield;

        [Header("Player's SuperShield")]
		public int startSuperShield;
		public int maxSuperShield;
		public int superShieldRegenCooldown;
		public int superShieldRegenAmount;
        public float lastTimeHeat;

        [Header("Others")]
		public float speed;
		public float jumpPower;

		// privates
		[Networked] private int _health {get; set;}
		[Networked] private int _shield {get; set;}
		[Networked] private int _superShield {get; set;}

		[Networked] private bool _isAlive {get; set;} = true;

		public override void Spawned () 
		{
			
            if (HasStateAuthority == true)
            {
                base.Spawned();
                GameObject barHealt = GameObject.FindGameObjectWithTag("healBar");
                GameObject barSuperShield = GameObject.FindGameObjectWithTag("superShield");
                GameObject barShield = GameObject.FindGameObjectWithTag("ShieldBar");
                HealthBar = barHealt.GetComponent<Bar>();
                ShieldBar = barShield.GetComponent<Bar>();
                SuperShieldBar = barSuperShield.GetComponent<Shield>();

                // Setting start values
                _health = startHealth;
                _shield = startShield;
                _superShield = startSuperShield;
                lastTimeHeat = Time.time;
                HealthBar.SetBar(_health, maxHealth);
                ShieldBar.SetBar(_shield, maxShield);
                // Check if speed and jump values are negative
                if (speed < 0f) speed = 1f;
                if (jumpPower < 0f) jumpPower = 1f;
				
            }
				

            
		}

		public int getCurrentTotalHealth () 
		{
			return _health + _shield + _superShield;
		}

		public void takeDamage (int amount) 
		{
			Debug.Log ("PLAYER TAKE DAMAGE : " + amount);
			// Initialisez leftAmount
			int leftAmount = amount;

			// If player have supershield
			if (_superShield > 0)
			{
				// Make _superShield take damage
				_superShield -= leftAmount;

				// If supershield can all the damage we stop here
				if (_superShield > 0) {
					if (HasStateAuthority == true)
					{
						SuperShieldBar.SetSuperShield(_superShield, maxShield);//set la barre du super bouclier en fonction du max du super bouclier et du bouclier
					}
					return;
                }

                // If supershield cant talke all the damage, 
                // we reset damage left
                leftAmount = _superShield  * -1;
				// We set supershield to 0
				_superShield = 0;
			}
			// If player have shield
			if (_shield > 0)
			{
				// Make _shield take damage
				_shield -= leftAmount;

				// If shield can all the damage we stop here
				if (_shield > 0)
				{
					if (HasStateAuthority == true)
					{
						ShieldBar.SetBar(_shield, maxShield);//set la barre de bouclier en fonction du max de bouclier et du bouclier
					}
					return;
				}

				// If shield cant talke all the damage, 
				// we reset damage left
				leftAmount = _shield  * -1;
				// We set shield to 0
				_shield = 0;
			}

			// Appy damage to  the heatlh
			_health -= leftAmount;

			// If player health is below 0 we kill him !
			if (_health <= 0)
			{
				// Make player die
				die ();
				// Set health to 0
				_health = 0;
			}
			if (HasStateAuthority == true)
			{
				HealthBar.SetBar(_health, maxHealth);//set la barre de vie en fonction du max de pv et de la vie actuelle
				ShieldBar.SetBar(_shield, maxShield);//set la barre de bouclier en fonction du max de bouclier et du bouclier
				SuperShieldBar.SetSuperShield(_superShield, maxSuperShield);//set la barre du super bouclier en fonction du max du super bouclier et du bouclier
			}

        }

        public void die ()
		{
			Debug.LogWarning ("DIE !!!");
			
			Player player = GetComponent<Player> ();
			// pour le debug ???
			player.DebugIsDead = true;

			// Vrai isAlive qui marche bien (celui qui permet de compter le nombre de joueur vivant restant)
			player.isAlive = false;
		}

		public void heal (int amount) 
		{
			// Add amount to the player
			_health += amount;
			// Check if health is above max health
			if (_health > maxHealth)
				_health = maxHealth; // Set health to max health
		}

		public void addShield (int amount)
		{
			// Add amount to the player
			_shield += amount;
			// Check if shield is above max shield
			if (_shield > maxShield)
				_shield = maxShield; // Set shield to max shield
		}

		public void addSuperShield (int amount)
		{
			// Add amount to the player
			_superShield += amount;
			// Check if superShield is above max superShield
			if (_superShield > maxSuperShield)
				_superShield = maxSuperShield; // Set superShield to max superShield
		}

		public void handleSuperShield ()
		{
            //regarde si on peut regenere le surbouclier
            if (_superShield < maxSuperShield && Time.time > lastTimeHeat + superShieldRegenCooldown)
            {
                _superShield += (int)(superShieldRegenAmount * Time.deltaTime);
                SuperShieldBar.SetSuperShield(_superShield, maxSuperShield);
            }

        }

    }
}
