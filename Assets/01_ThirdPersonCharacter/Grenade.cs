using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Starter.ThirdPersonCharacter
{
    public class Grenade : Item
    {
		[Header("Greande Stats")]
        public int maxDamage; // Max damage that the grenade can deal (when player close to explosion)
        public int minDamage; // Min damage taht the grenade can deal (when player far from explosion)
        public int throwDistance; // Throw distance of the grenade 
        public float explosionRadius; // All the radius that the grenade will damage on explode
        public int timeBeforeExplode; // Time before the grenade explode
        
        // Private
        private Transform spawnGrenadePosition; // Where the bullet is gonna spawn
        
        public override ItemType getType()
        {
            return ItemType.Grenade;
        }

        public override void use()
        {
            // TO DO
        }
    }
}
