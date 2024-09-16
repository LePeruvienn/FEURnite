using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Starter.ThirdPersonCharacter
{
    public class Weapon : Item
    {
        // STATIC VARIABLES
        protected static int __BULLET_TYPE_SNIPER__ = 1;
        protected static int __BULLET_TYPE_RIFLE__ = 2;
        protected static int __BULLET_TYPE_PISTOL__ = 3;
        protected static int __BULLET_TYPE_ROCKET__ = 4;
        
		[Header("Weapon References")]
        public GameObject bulletPrefab; // The bullet to use when shooting
        
		[Header("Weapon Stats")]
        public float fireRate; // Bullet per seconds that the weapon shoot
        public int damage; // Damage applying for each bullet to the player
        public int realoadCooldown; // Reload time to get full ammo
        public int chargerAmmoAmount; // Bullet per charger
        public int ammoType; // Type of bullet the weapon use

        // Privates
        private Transform spawnBulletPosition; // Where the bullet is gonna spawn
        private LayerMask aimColliderLayerMask = new LayerMask();
        
        public override int getType()
        {
            return Item.__TYPE_WEAPON__;
        }

        public override void use()
        {
            // Check is spawnBulletPosition is not null
            if (spawnBulletPosition == null)
                spawnBulletPosition = GameObject.FindGameObjectWithTag("spawnBulletPos").transform; // If he is null we set it
            
            // Get where we are aiming
            Vector3 mouseWorldPosition = Vector3.zero;
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray, out RaycastHit rayCastHit, 999f, aimColliderLayerMask))
            {
                mouseWorldPosition = rayCastHit.point;
            }
            
            // Shoot the bullet prefab
            Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
            Instantiate(bulletPrefab,spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
        }
    }
}
