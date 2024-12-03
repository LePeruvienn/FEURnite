using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Starter.ThirdPersonCharacter
{
	// Enum for weapon states
	public enum WeaponState
	{
		Ready = 1,
		Reloading = 2
	}

	// Enum for bullet types
	public enum BulletType
	{
		Sniper = 1,
		Rifle = 2,
		Pistol = 3,
		Rocket = 4
	}

	[RequireComponent(typeof(Rigidbody))]
    public class Weapon : Item
    {
        
		[Header("Weapon References")]
        public GameObject bulletPrefab; // The bullet to use when shooting
        
		[Header("Weapon Stats")]
        public float shootDelay; // Time to wait between each bullets
        public int damage; // Damage applying for each bullet to the player
        public int reloadCooldown; // Reload time to get full ammo
        public int startAmmoAmount; // Amount of bullet currenty in the charger
        public int chargerAmmoAmount; // Bullets per charger
        public BulletType bulletType; // Type of bullet the weapon use

        [Header("Weapon style")]
        public ParticleSystem muzzleFalshParticles;

        // Privates
        private Animator _playerAnimator;
        private WeaponState _currentWeaponState;
        private int _currentAmmoAmount;
        [SerializeField]  private Transform _spawnBulletPosition; // Where the bullet is gonna spawn
		private float _nextFireTime = 0f;

		// Run when program starts
		public void Start ()
		{
			// Set current amoo to start Ammo amount
			_currentAmmoAmount = startAmmoAmount; 
			// Set weapon state to ready
			_currentWeaponState = WeaponState.Ready;
		}

        public override ItemType getType ()
        {
            return ItemType.Weapon;
        }

        public override void use ()
        {

			// Return if weapon dont have ammo left !
			if (_currentAmmoAmount <= 0 || _currentWeaponState == WeaponState.Reloading) return;
			// Check if player can fire
			if (Time.time >= _nextFireTime) {
				// Shoot a bullet
				shoot();
				// Remove bullet from current charger
				_currentAmmoAmount--;
				// Set the _nextFireTime
				_nextFireTime = Time.time + shootDelay;
			}
        }
        

        private void shoot ()
		{
            
            // Setting up raycast variables
            Vector3 mouseWorldPosition = Vector3.zero; // Default vector
            Vector3 rayOrigin = new Vector3(0.5f, 0.5f, 0f); // center of the screen
            float rayLength = 500f; // Raycast length
            
            // Doing the raycast !
            Ray ray = Camera.main.ViewportPointToRay(rayOrigin);
            
            // Setting output variable
            RaycastHit hit;
            
            // If raycast hit
            if (Physics.Raycast(ray, out hit, rayLength))
            {
                mouseWorldPosition = hit.point; // Set the target point to the point hit by the raycast
            } 
            
            // Shoot the bullet prefab
            Vector3 aimDir = (mouseWorldPosition - _spawnBulletPosition.position).normalized;
            Debug.Log(bulletPrefab.name);
            Runner.Spawn(bulletPrefab,_spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            Debug.Log("test");
            Instantiate(muzzleFalshParticles, _spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
        }

        public void reload ()
        {
			// If item is already Reloading stop
			if (_currentWeaponState == WeaponState.Reloading) return;

			// Set status to reloading
			_currentWeaponState = WeaponState.Reloading;

			// Check if we are alreadyFull ammo
			if (_currentAmmoAmount >= chargerAmmoAmount) return;

            // Getting PlayerAnimator
            if (_playerAnimator == null)
                _playerAnimator = GetComponentInParent<Animator>();

            // Start reload couroutine
            StartCoroutine (reloadCouroutine ());

        }

		private IEnumerator reloadCouroutine () 
		{

            //Realod animation
            _playerAnimator.SetTrigger("ReloadTrigger");

            // Wait for reload cooldown
            yield return new WaitForSeconds(reloadCooldown);

			// Set charger to to full
			_currentAmmoAmount = chargerAmmoAmount;

			// Set weapon state to ready
			_currentWeaponState = WeaponState.Ready;

			// Debug messsage (delete it later)
			Debug.Log ("Reload Complete !");
		}
    }
}
