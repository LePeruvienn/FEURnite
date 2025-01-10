using UnityEngine;

namespace Starter.ThirdPersonCharacter
{

    public class PlatformesRotations : MonoBehaviour
    {
        public Vector3 rotationAxis = new Vector3(1, 0, 0); // Axe de rotation (X par défaut)
        public float rotationSpeed = 30f; // Vitesse de rotation en degrés par seconde

        void Update()
        {
            // Appliquer la rotation autour de l'axe spécifié
            transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
        }
    }
}