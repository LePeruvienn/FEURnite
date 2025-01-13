using UnityEngine;

namespace Starter.ThirdPersonCharacter
{

    public class PlateformesRotations : MonoBehaviour
    {
        public Vector3 rotationAxis = new Vector3(0, 0, 1); // Axe de rotation
        public float rotationSpeed = 10f; // Vitesse de rotation en degr�s par seconde

        void Update()
        {
            // Appliquer la rotation autour de l'axe sp�cifi�
            transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
        }
    }
}