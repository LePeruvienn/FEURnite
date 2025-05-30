using System;
using System.Collections;
using UnityEngine;
using Fusion;
using Fusion.Statistics;
using UnityEngine.UIElements;
using System.Net;
using Unity.VisualScripting;

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
        Knife = 5,
        Shotgun = 6
    }

    [RequireComponent(typeof(Rigidbody))]
    public class Weapon : Item
    {
		private DamagePopUpGenerator _popupGenerator;

        [Header("Weapon References")]
        public GameObject bulletPrefab; // The bullet to use when shooting

		[Header("Weapon Shoot Config")]
        public float shootDelay; // Time to wait between each bullets
        public float stabDelayDamage; // Time to wait between each stab
		public ShootType shootType;

		[Header("Weapon Bullet Spread")]
		public bool addBulletSpread = true;
		public Vector3 bulletSpreadVariance = new Vector3 (0.1f, 0.1f, 0.1f);

		[Header("Weapon Bullet Shoot config")]
        public BulletType bulletType; // Type of bullet the weapon use (OLNY WORK IF WEAPON IS NOT ON HIT SCAN !)

		[Header("Weapon Bullet HitScan Config")]
        public bool hasATrail = true;
        public TrailRenderer bulletTrail; // Type of bullet the weapon use (OLNY WORK IF WEAPON IS NOT ON HIT SCAN !)
        public int trailSpeed; // Type of bullet the weapon use (OLNY WORK IF WEAPON IS NOT ON HIT SCAN !)
        
		[Header("Weapon Stats")]
        public int damage; // Damage applying for each bullet to the player
        public bool hasAmmo = true;
        public int reloadCooldown; // Reload time to get full ammo
        public int startAmmoAmount; // Amount of bullet currenty in the charger
        public int bulletSize; // Amount of bullet currenty in the charger
        public int chargerAmmoAmount; // Bullets per charger
        public WeaponProperties weight; // Poids de l'arme  
        public int bulletCount = 1; // Number of bullets to spawn
        public float bulletRange = 10f; // range of bullets

        [Header("Weapon style")]
        public ParticleSystem muzzleFalshParticles;

        [Header("Weapon Sound")]
        public AudioSource audioSource;
        public static AudioClip audioClip;
        public static AudioClip ReloadAudioClip;
        public static AudioClip PistolAudioClip;
        public static AudioClip SniperAudioClip;
        public static AudioClip RocketAudioClip;
        public static AudioClip KnifeAudioClip;
        public static AudioClip ShotgunAudioClip;
        public static AudioClip NoAmmoAudioClip;


        // Privates
        private Animator _playerAnimator;
        private WeaponState _currentWeaponState;
        private int _currentAmmoAmount;
        [SerializeField] private Transform _spawnBulletPosition; // Where the bullet is gonna spawn
        private float _nextFireTime = 0f;

        [NonSerialized] private NetworkRunner _runner; // Prevent _runner from being serialized
        
        private GameObject munition;
        private munitions textMunition;
        private PlayerInventory playerInv;

		void OnDisable()
		{
			_currentWeaponState = WeaponState.Ready;
		}

        // Run when program starts
        public override void Spawned()
        {
			_popupGenerator = FindObjectOfType<DamagePopUpGenerator>();
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
            PistolAudioClip = Resources.Load<AudioClip>("DE_Sound");
            SniperAudioClip = Resources.Load<AudioClip>("SniperSound");
            RocketAudioClip = Resources.Load<AudioClip>("RocketSound");
            KnifeAudioClip = Resources.Load<AudioClip>("KnifeSound");
            ShotgunAudioClip = Resources.Load<AudioClip>("ShotgunSound");
            NoAmmoAudioClip = Resources.Load<AudioClip>("NoAmmoSound");

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

        public void Update()
        {
            munition = GameObject.FindGameObjectWithTag("Mun");
            playerInv = GetComponentInParent<PlayerInventory>();

            if (munition != null && playerInv != null && playerInv.getCurrentSelection() == gameObject)
            {
                textMunition = munition.GetComponent<munitions>();
 
                textMunition.setMunitions(_currentAmmoAmount, chargerAmmoAmount);
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
            
            
            if ( _currentWeaponState == WeaponState.Reloading) return;
            
            // Check if player can fire
            if (Time.time >= _nextFireTime)
            {
                // Return if weapon doesn't have ammo left!
                if (_currentAmmoAmount <= 0)
                {
                    audioSource.PlayOneShot(NoAmmoAudioClip);
                    _nextFireTime = Time.time + shootDelay;
                    return;
                }
                RPCFireSound();
                // Shoot deping on the shoot type
                if (shootType == ShootType.HitScan)
                {
                    if(bulletType == BulletType.Knife)
                    {
                        // Start stab couroutine
                        StartCoroutine(stabDamage());
                        StartCoroutine(stabCouroutine());
                    }
                    else
                    {
                        for (int i = 0; i < bulletCount; i++)
                        {
                            shoot_hitScan();
                        }
                    }

                }
                else
                {
                    shoot_bullet();
                }

                if (hasAmmo)
                {
                    // Remove bullet from current charger
                    _currentAmmoAmount--;
                }
                // Set the _nextFireTime
                _nextFireTime = Time.time + shootDelay;
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]

        public void RPCFireSound()
        {
            if (audioSource != null)
            {
                switch (bulletType)
                {
                    case BulletType.Sniper:
                        audioSource.PlayOneShot(SniperAudioClip);
                        break;
                    case BulletType.Rifle:
                        audioSource.PlayOneShot(audioClip);
                        break;
                    case BulletType.Pistol:
                        audioSource.PlayOneShot(PistolAudioClip);
                        break;
                    case BulletType.Rocket:
                        audioSource.PlayOneShot(RocketAudioClip);
                        break;
                    case BulletType.Knife:
                        audioSource.PlayOneShot(KnifeAudioClip);
                        break;
                    case BulletType.Shotgun:
                        audioSource.PlayOneShot(ShotgunAudioClip);
                        break;
                    default:
                        Debug.LogWarning("PAS DE SON NON NON NON");
                        break;
                }
            }
            else
            {
                Debug.LogWarning("Audio pas trouvé");
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

        public Vector2 GetRandomPointInCircle(float radius)
        {
            float angle = UnityEngine.Random.Range(0, 2 * Mathf.PI); // Random angle in radians
            float distance = UnityEngine.Random.Range(0, radius);    // Random distance within the radius
            float x = Mathf.Cos(angle) * distance;
            float y = Mathf.Sin(angle) * distance;
            return new Vector2(x, y);
        }

        public void shoot_hitScan () {

            // Setting up raycast variables
            Vector3 mouseWorldPosition = Vector3.zero; // Default vector
            Vector3 rayOrigin = new Vector3(0.5f, 0.5f, 0f); // Center of the screen

            // Doing the raycast!
            Ray ray = Camera.main.ViewportPointToRay(rayOrigin);

            // Setting output variable
            RaycastHit hit;

			// Add Bullet spread if it's true
			Vector3 shootDirection = addBulletSpread ? applyBulletSpread(ray.direction) : ray.direction;

            // Default endpoint if nothing is hit
            Vector3 endPoint = ray.origin + shootDirection * bulletRange;

            // If raycast hit
            if (Physics.Raycast(ray.origin, shootDirection, out hit, bulletRange))
            {
				// Get hit position
                mouseWorldPosition = hit.point; // Set the target point to the point hit by the raycast

                // Update the endpoint to the hit point
                endPoint = hit.point;

                // Getting player Model
                PlayerModel playerModel = hit.collider != null ?
					hit.collider.GetComponentInParent<PlayerModel> () :
					null;


				// If ELement hit has a player Model we apply the damages
				if (playerModel != null) {

					// GeneratePopup
					_popupGenerator.CreatePopUp(hit.point, damage.ToString(), Color.white);
					RPC_takeDamage (playerModel, damage);
				}
            }

            if (hasATrail)
            {
                // Spawn the trail from the bullet's start position to the endpoint
                RPC_SpawnTrail(_spawnBulletPosition.position, endPoint);
            }
        }

		[Rpc(RpcSources.All, RpcTargets.All)]
		public void RPC_takeDamage (PlayerModel playerModel, int damage) {
			
			if (playerModel)
				playerModel.takeDamage (damage);
		} 

		[Rpc(RpcSources.All, RpcTargets.All)]
		public void RPC_SpawnTrail(Vector3 startPoint, Vector3 hitPoint)
		{
			// Spawn trail on the network
			var trailObject = Runner.Spawn(bulletTrail.gameObject, startPoint, Quaternion.identity);
			var trail = trailObject.GetComponent<TrailRenderer>();

			if (trail != null)
			{
				// Start the trail movement coroutine
				StartCoroutine(spawnTrail(trail, hitPoint));
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
            if (hasAmmo)
            {
                // If item is already Reloading stop
                if (_currentWeaponState == WeaponState.Reloading) return;

                // Set status to reloading
                _currentWeaponState = WeaponState.Reloading;

                // Check if we are already full ammo
                if (_currentAmmoAmount >= chargerAmmoAmount) return;

                // Start reload coroutine
                StartCoroutine(reloadCouroutine());
                if (ReloadAudioClip != null)
                {
                    audioSource.PlayOneShot(ReloadAudioClip);
                }
                else
                {
                    Debug.Log("audio de rechargement non trouvé");

                }
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

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RPC_Cuting()
        {
            // Getting PlayerAnimator
            if (_playerAnimator == null)
                _playerAnimator = GetComponentInParent<Animator>();

            //stab animation
            _playerAnimator.SetTrigger("StabTrigger");
        }

        private IEnumerator stabCouroutine()
        {
            // networked methode
            RPC_Cuting();

            // Wait for reload cooldown
            yield return new WaitForSeconds(shootDelay);
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

        private IEnumerator stabDamage()
        {
            // Wait for reload cooldown
            yield return new WaitForSeconds(stabDelayDamage);

            for (int i = 0; i < bulletCount; i++)
            {
                shoot_hitScan();
            }
        }
    }

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
    public class WeaponProperties : ScriptableObject
    {
        public float _weight;
    }

}
