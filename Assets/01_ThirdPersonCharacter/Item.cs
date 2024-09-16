using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Starter.ThirdPersonCharacter
{
    // Classe abstaire Des items (Utilisable, armes, grenades)
    public abstract class Item : MonoBehaviour
    {
        // STATIC: Item types
        protected static int __TYPE_WEAPON__ = 1;
        protected static int __TYPE_USABLE__ = 2;
        protected static int __TYPE_GRENADE__ = 3;
        
        // STATIC: Item states
        protected static int __STATE_ON_FLOOR__ = 1;
        protected static int __STATE_EQUIEPED__ = 2;
        protected static int __STATE_SELECTED__ = 3;
        
        // STATIC: Item states
        protected static int __RARITY_COMMON__ = 1;
        protected static int __RARITY_RARE__ = 2;
        protected static int __RARITY_EPIC__ = 3;
        protected static int __RARITY_LEGENDARY__ = 4;
        
		[Header("Item config")]
        public string name; // Nom de l'item
        public int stackAmount; // Nombre d'objets qu'on peut stocker en meme temps dans l'inventaire
        public int rarity; // Rareté de l'objet
        
        // Privates
        private int _state; // Etat actuelle : onFloor (au sol), equiped (dans l'inventaire d'un joueur) , selected (Dans la main d'un joueur)
        
        // Abstract functions
        public abstract int getType(); // Retourne de type de l'objet
        public abstract void use(); // Utiliser l'item

        public void drop() // Drop l'item (pas possible si déja au sol)
        {

        }

        public void pickUp () // Récupérer l'item (seulement possible si au sol)
        {
            
        }
    }
}