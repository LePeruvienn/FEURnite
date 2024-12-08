using System;
using System.Collections;
using UnityEngine;
using Fusion;

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
        public float shootDelay; // Time to wait between each bullet
        public int damage; // Damage applying for each bullet to the player
        public int reloadCooldown; // Reload time to get full ammo
        public int startAmmoAmount; // Amount of bullets currently in the charger
        public int chargerAmmoAmount; // Bullets per charger
        public BulletType bulletType; // Type of bullet the weapon uses

        [Header("Weapon style")]
        public ParticleSystem muzzleFalshParticles;

        // Privates
        private Animator _playerAnimator;
        private WeaponState _currentWeaponState;
        private int _currentAmmoAmount;
        [SerializeField] private Transform _spawnBulletPosition; // Where the bullet is gonna spawn
        private float _nextFireTime = 0f;

        [NonSerialized] private NetworkRunner _runner; // Prevent _runner from being serialized

        // Run when program starts
        public void Start()
        {
            // Set current ammo to start Ammo amount
            _currentAmmoAmount = startAmmoAmount;
            // Set weapon state to ready
            _currentWeaponState = WeaponState.Ready;

            if (_runner == null)
            {
                _runner = FindObjectOfType<NetworkRunner>();
                Debug.Log("NetworkRunner n'est pas trouvé dans la scène !");
                
            }
        }

        public override ItemType getType()
        {
            return ItemType.Weapon;
        }

        public override void use()
        {
            // Return if weapon doesn't have ammo left!
            if (_currentAmmoAmount <= 0 || _currentWeaponState == WeaponState.Reloading) return;

            // Check if player can fire
            if (Time.time >= _nextFireTime)
            {
                // Shoot a bullet
                shoot();
                // Remove bullet from current charger
                _currentAmmoAmount--;
                // Set the _nextFireTime
                _nextFireTime = Time.time + shootDelay;
            }
        }

        private void shoot()
        {
            // Setting up raycast variables
            Vector3 mouseWorldPosition = Vector3.zero; // Default vector
            Vector3 rayOrigin = new Vector3(0.5f, 0.5f, 0f); // Center of the screen
            float rayLength = 500f; // Raycast length

            // Doing the raycast!
            Ray ray = Camera.main.ViewportPointToRay(rayOrigin);

            // Setting output variable
            RaycastHit hit;

            // If raycast hit
            if (Physics.Raycast(ray, out hit, rayLength))
            {
                mouseWorldPosition = hit.point; // Set the target point to the point hit by the raycast
            }

            // Direction to shoot the bullet
            Vector3 aimDir = (mouseWorldPosition - _spawnBulletPosition.position).normalized;

            // Ensure bulletPrefab is set
            if (bulletPrefab == null)
            {
                Debug.LogError("bulletPrefab n'est pas défini !");
                return;
            }
            if (_spawnBulletPosition == null)
            {
                Debug.LogError("_spawnBulletPosition n'est pas défini !");
                return;
            }
            if (_runner == null)
            {
                Debug.LogError("NetworkRunner n'est pas initialisé !");
                return;
            }

            try
            {
                // Spawn bullet on the network
                _runner.Spawn(bulletPrefab, _spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));

                // Instantiate muzzle flash (local effect)
                Instantiate(muzzleFalshParticles, _spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            }
            catch (Exception e)
            {
                Debug.LogError($"Erreur lors du spawn : {e.Message}");
            }
        }

        public void reload()
        {
            // If item is already Reloading stop
            if (_currentWeaponState == WeaponState.Reloading) return;

            // Set status to reloading
            _currentWeaponState = WeaponState.Reloading;

            // Check if we are already full ammo
            if (_currentAmmoAmount >= chargerAmmoAmount) return;

            // Getting PlayerAnimator
            if (_playerAnimator == null)
                _playerAnimator = GetComponentInParent<Animator>();

            // Start reload coroutine
            StartCoroutine(reloadCouroutine());
        }

        private IEnumerator reloadCouroutine()
        {
            // Reload animation
            _playerAnimator.SetTrigger("ReloadTrigger");

            // Wait for reload cooldown
            yield return new WaitForSeconds(reloadCooldown);

            // Set charger to full
            _currentAmmoAmount = chargerAmmoAmount;

            // Set weapon state to ready
            _currentWeaponState = WeaponState.Ready;

            // Debug message (delete it later)
            Debug.Log("Reload Complete !");
        }
    }
}
