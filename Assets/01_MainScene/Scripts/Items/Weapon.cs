using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
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
		Rocket = 4,
        Knife = 5
    }

    [RequireComponent(typeof(Rigidbody))]
    public class Weapon : Item
    {
        [Header("Weapon References")]
        public GameObject bulletPrefab; // The bullet to use when shooting
        
		[Header("Weapon Stats")]
        public float shootDelay; // Time to wait between each bullets
        public float stabDelay; // Time to wait between each stab
        public int damage; // Damage applying for each bullet to the player
        public int reloadCooldown; // Reload time to get full ammo
        public int startAmmoAmount; // Amount of bullet currenty in the charger
        public int bulletSize; // Amount of bullet currenty in the charger
        public int chargerAmmoAmount; // Bullets per charger
        public BulletType bulletType; // Type of bullet the weapon use
        public WeaponProperties weight; // Poids de l'arme

        [Header("Weapon style")]
        public ParticleSystem muzzleFalshParticles;

        [Header("Weapon Sound")]
        public AudioSource audioSource;
        public static AudioClip audioClip;
        public static AudioClip ReloadAudioClip;

        // Privates
        private Animator _playerAnimator;
        private WeaponState _currentWeaponState;
        private int _currentAmmoAmount;
        [SerializeField] private Transform _spawnBulletPosition; // Where the bullet is gonna spawn
        private float _nextFireTime = 0f;

        [NonSerialized] private NetworkRunner _runner; // Prevent _runner from being serialized


        // Run when program starts
        public override void Spawned()
        {
            audioSource = GetComponent<AudioSource>();
            
            if(audioSource == null)
            {
                Debug.LogWarning("audio source pas trouvé");
            }
            else
            {
                Debug.LogWarning("audio source trouvé");
            }

            //chargé les clip audio
            audioClip = Resources.Load<AudioClip>("Pistol Sound Effect");
            ReloadAudioClip = Resources.Load<AudioClip>("ReloadAudio");

            // Set current ammo to start Ammo amount
            _currentAmmoAmount = startAmmoAmount;
            // Set weapon state to ready
            _currentWeaponState = WeaponState.Ready;

            if (_runner == null)
            {
                _runner = FindObjectOfType<NetworkRunner>();
                Debug.Log("NetworkRunner n'est pas trouv� dans la sc�ne !");
                
            }

            
            

        }

        public override ItemType getType()
        {
            return ItemType.Weapon;
        }

        public override BulletType getBulletType()
        {
            return bulletType;
        }

        public override void use ()
        {
            // Return if weapon doesn't have ammo left!
            if (_currentAmmoAmount <= 0 || _currentWeaponState == WeaponState.Reloading) return;
            // Check if player can fire
            if (Time.time >= _nextFireTime)
            {
                if (bulletPrefab != null)// is a weapon with bullet
                {
                    // Shoot a bullet
                    shoot();
                   
                    RPCFireSound();
                    

                    // Remove bullet from current charger
                    _currentAmmoAmount--;
                    // Set the _nextFireTime
                    _nextFireTime = Time.time + shootDelay;
                }
                else 
                {
                    // Start stab couroutine
                    StartCoroutine(stabCouroutine());

                    _nextFireTime = Time.time + stabDelay;
                }
            }
        }
        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPCFireSound()
        {
            
            if (audioSource != null)
            {
                audioSource.PlayOneShot(audioClip);
            }
            else
            {
                Debug.LogWarning("Audio pas trouvé");
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RpcShoot(Vector3 spawnPos, Vector3 aimDir)
        {
            // Spawn bullet sur le serveur
            _runner.Spawn(bulletPrefab, spawnPos, Quaternion.LookRotation(aimDir, Vector3.up),Runner.LocalPlayer);
            

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

            bulletPrefab.GetComponent<BulletProjectile>().damage = damage;

            // Shoot the bullet prefab
            Vector3 aimDir = (mouseWorldPosition - _spawnBulletPosition.position).normalized;
            try
            {
                // Spawn bullet on the network
                RpcShoot(_spawnBulletPosition.position, aimDir);


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

            // ############################# teste dodo
            // Getting PlayerAnimator
            // if (_playerAnimator == null)
            //    _playerAnimator = GetComponentInParent<Animator>();
            // ############################# teste dodo

            // Start reload coroutine
            StartCoroutine(reloadCouroutine());
            if(ReloadAudioClip != null)
            {
                audioSource.PlayOneShot(ReloadAudioClip);
            }
            else
            {
                Debug.Log("audio de rechargement non trouvé");
            }
        }

        private IEnumerator reloadCouroutine()
        {
            // Wait for reload cooldown
            yield return new WaitForSeconds(reloadCooldown);

            // Set charger to full
            _currentAmmoAmount = chargerAmmoAmount;

            // Set weapon state to ready
            _currentWeaponState = WeaponState.Ready;

          // Debug messsage (delete it later)
          Debug.Log ("Reload Complete !");
        }

        private IEnumerator stabCouroutine()
        {

            // Getting PlayerAnimator
            if (_playerAnimator == null)
                _playerAnimator = GetComponentInParent<Animator>();

            //stab animation
            _playerAnimator.SetTrigger("StabTrigger");

            // Wait for reload cooldown
            yield return new WaitForSeconds(stabDelay);

            // Debug messsage (delete it later)
            Debug.Log("stab Complete !");
        }
    }

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
    public class WeaponProperties : ScriptableObject
    {
        public float _weight;
    }

}
