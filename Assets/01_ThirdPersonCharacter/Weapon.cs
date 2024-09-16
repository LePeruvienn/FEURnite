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
        public Transform spawnBulletPosition; // Where the bullet is gonna spawn
        public GameObject bulletPrefab; // The bullet to use when shooting
        
		[Header("Weapon Stats")]
        public float fireRate; // Bullet per seconds that the weapon shoot
        public int damage; // Damage applying for each bullet to the player
        public int realoadCooldown; // Reload time to get full ammo
        public int chargerAmmoAmount; // Bullet per charger
        public int ammoType; // Type of bullet the weapon use

        public override int getType()
        {
            return Item.__TYPE_WEAPON__;
        }

        public override void use()
        {
            // TODO
        }
    }
}
