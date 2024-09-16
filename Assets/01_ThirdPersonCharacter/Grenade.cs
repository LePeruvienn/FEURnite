using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Starter.ThirdPersonCharacter
{
    public class Grenade : Item
    {
		[Header("Greande Stats")]
        public int maxDamage;
        public int minDamage;
        public int throwDistance;
        public float explosionRadius;
        
        public override int getType()
        {
            return Item.__TYPE_GRENADE__;
        }

        public override void use()
        {
            // TO DO
        }
    }
}