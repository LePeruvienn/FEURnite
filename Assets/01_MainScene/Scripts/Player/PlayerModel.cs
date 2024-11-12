using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Starter.ThirdPersonCharacter
{
	public class PlayerModel : MonoBehaviour
	{

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


        [Header("Others")]
		public float speed;
		public float jumpPower;

		// privates
		private int _health;
		private int _shield;
		private int _superShield;

		public void Start () 
		{
			// Setting start values
			_health = startHealth;
			_shield = startShield;
			_superShield = startSuperShield;

			// Check if speed and jump values are negative
			if (speed < 0f)	speed = 1f;
			if (jumpPower < 0f)	jumpPower = 1f;
		}

		public int getCurrentTotalHealth () 
		{
			return _health + _shield + _superShield;
		}

		public void takeDamage (int amount) 
		{
			// Initialisez leftAmount
			int leftAmount = amount;

			// If player have supershield
			if (_superShield > 0)
			{
				// Make _superShield take damage
				_superShield -= leftAmount;

				// If supershield can all the damage we stop here
				if (_superShield > 0)
					return;

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
					return;

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
		}

		public void die ()
		{
			// TODO
			Debug.Log ("PLAYER IS DEAD !!");
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
			// TODO
		}
	}
}
