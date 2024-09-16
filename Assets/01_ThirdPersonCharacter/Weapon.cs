using System;
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
		public Transform CameraPivot;
		public Transform CameraHandle;
        
		[Header("Weapon Stats")]
        public float fireRate; // Bullet per seconds that the weapon shoot
        public int damage; // Damage applying for each bullet to the player
        public int realoadCooldown; // Reload time to get full ammo
        public int chargerAmmoAmount; // Bullet per charger
        public int ammoType; // Type of bullet the weapon use

        // Privates
        private Transform spawnBulletPosition; // Where the bullet is gonna spawn
        
        public override int getType()
        {
            return Item.__TYPE_WEAPON__;
        }

        public override void use()
        {
            // Check is spawnBulletPosition is not null
            if (spawnBulletPosition == null)
                spawnBulletPosition = GameObject.FindGameObjectWithTag("spawnBulletPos").transform; // If he is null we set it
            
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
            Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
            Instantiate(bulletPrefab,spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
        }
    }
}
