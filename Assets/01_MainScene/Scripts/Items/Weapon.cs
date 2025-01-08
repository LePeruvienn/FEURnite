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
	
    // Enum for weapon shoot type
    public enum ShootType
    {
        HitScan = 1,
        Bullet = 2
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

		[Header("Weapon Shoot Config")]
        public float shootDelay; // Time to wait between each bullets
        public float stabDelay; // Time to wait between each stab
		public ShootType shootType;

		[Header("Weapon Bullet Spread")]
		public bool addBulletSpread = true;
		public Vector3 bulletSpreadVariance = new Vector3 (0.1f, 0.1f, 0.1f);

		[Header("Weapon Bullet Shoot config")]
        public BulletType bulletType; // Type of bullet the weapon use (OLNY WORK IF WEAPON IS NOT ON HIT SCAN !)

		[Header("Weapon Bullet HitScan Config")]
        public TrailRenderer bulletTrail; // Type of bullet the weapon use (OLNY WORK IF WEAPON IS NOT ON HIT SCAN !)
        public int trailSpeed; // Type of bullet the weapon use (OLNY WORK IF WEAPON IS NOT ON HIT SCAN !)
        
		[Header("Weapon Stats")]
        public int damage; // Damage applying for each bullet to the player
        public int reloadCooldown; // Reload time to get full ammo
        public int startAmmoAmount; // Amount of bullet currenty in the charger
        public int bulletSize; // Amount of bullet currenty in the charger
        public int chargerAmmoAmount; // Bullets per charger
        public WeaponProperties weight; // Poids de l'arme

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
        public override void Spawned()
        {
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
                    // Shoot deping on the shoot type
					if (shootType == ShootType.HitScan)
						shoot_hitScan ();
					else
						shoot_bullet ();

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
        public void RPC_SpawnBullet(Vector3 spawnPos, Vector3 aimDir)
        {
            // Spawn bullet sur le serveur
            _runner.Spawn(bulletPrefab, spawnPos, Quaternion.LookRotation(aimDir, Vector3.up),Runner.LocalPlayer);
        }


        private void shoot_bullet ()
        {
            // Setting up raycast variables
            Vector3 mouseWorldPosition = Vector3.zero; // Default vector
            Vector3 rayOrigin = new Vector3(0.5f, 0.5f, 0f); // Center of the screen
            float rayLength = 500f; // Raycast length

            // Doing the raycast!
            Ray ray = Camera.main.ViewportPointToRay(rayOrigin);

            // Setting output variable
            RaycastHit hit;

			// Add Bullet spread if it's true
			Vector3 shootDirection = addBulletSpread ? applyBulletSpread(ray.direction) : ray.direction;

            // If raycast hit
            if (Physics.Raycast(ray.origin, shootDirection, out hit, rayLength))
            {
				// Get hit position
                mouseWorldPosition = hit.point; // Set the target point to the point hit by the raycast
            }

			// Link damage to the bullet
            bulletPrefab.GetComponent<BulletProjectile>().damage = damage;

            // Shoot the bullet prefab
            Vector3 aimDir = (mouseWorldPosition - _spawnBulletPosition.position).normalized;
            try
            {
                // Spawn bullet on the network
                RPC_SpawnBullet (_spawnBulletPosition.position, aimDir);

                // Instantiate muzzle flash (local effect)
                Instantiate(muzzleFalshParticles, _spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            }
            catch (Exception e)
            {
                Debug.LogError($"Erreur lors du spawn : {e.Message}");
            }
        }

		public void shoot_hitScan () {

            // Setting up raycast variables
            Vector3 mouseWorldPosition = Vector3.zero; // Default vector
            Vector3 rayOrigin = new Vector3(0.5f, 0.5f, 0f); // Center of the screen
            float rayLength = 500f; // Raycast length

            // Doing the raycast!
            Ray ray = Camera.main.ViewportPointToRay(rayOrigin);

            // Setting output variable
            RaycastHit hit;

			// Add Bullet spread if it's true
			Vector3 shootDirection = addBulletSpread ? applyBulletSpread(ray.direction) : ray.direction;

            // If raycast hit
            if (Physics.Raycast(ray.origin, shootDirection, out hit, rayLength))
            {
				// Get hit position
                mouseWorldPosition = hit.point; // Set the target point to the point hit by the raycast

				// Getting player Model
				PlayerModel pModel = hit.collider != null ?
					hit.collider.GetComponentInParent<PlayerModel> () :
					null;

				// If ELement hit has a player Model we apply the damages
				if (pModel != null)
					pModel.takeDamage (damage);

				// Enable tray
                TrailRenderer trail = Instantiate (bulletTrail, _spawnBulletPosition.position, Quaternion.identity);
                StartCoroutine (spawnTrail (trail, hit.point));
            }
		}

		// Use when bullet spread is activate to know where the bullet is going
		private Vector3 applyBulletSpread (Vector3 direction) {

			if (addBulletSpread)
			{
				direction += new Vector3(
					UnityEngine.Random.Range(-bulletSpreadVariance.x, bulletSpreadVariance.x) * 0.1f,
					UnityEngine.Random.Range(-bulletSpreadVariance.y, bulletSpreadVariance.y) * 0.1f,
					UnityEngine.Random.Range(-bulletSpreadVariance.z, bulletSpreadVariance.z) * 0.1f
				);

				direction.Normalize();
			}

			return direction;
		}

		// Reload the current weapon
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

		// Span bullet trail on hitscan
		private IEnumerator spawnTrail (TrailRenderer trail, Vector3 hitPoint)
		{
			Vector3 startPosition = trail.transform.position;
			float distance = Vector3.Distance (trail.transform.position, hitPoint);
			float remainingDistance = distance;

			while (remainingDistance > 0)
			{
				trail.transform.position = Vector3.Lerp (startPosition, hitPoint, 1 - (remainingDistance / distance));

				remainingDistance -= trailSpeed * Time.deltaTime;

				yield return null;
			}

			trail.transform.position = hitPoint;

			Destroy (trail.gameObject, trail.time);
		}
    }

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
    public class WeaponProperties : ScriptableObject
    {
        public float _weight;
    }

}
