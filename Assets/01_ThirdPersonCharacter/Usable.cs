using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Starter.ThirdPersonCharacter
{
    public class Usable : Item
    {
		[Header("Usable Stats")]
        public int useCooldown; // Time to wait without interuption to use the usable
        
        public override int getType()
        {
            return Item.__TYPE_USABLE__;
        }

        public override void use()
        {
            // TODO
        }
    }
}
