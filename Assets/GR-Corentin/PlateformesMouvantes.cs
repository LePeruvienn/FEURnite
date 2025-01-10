using UnityEngine;

namespace Starter.ThirdPersonCharacter
{

    public class PlateformesMouvantes : MonoBehaviour
    {
        public float speed = 2.0f; // Vitesse du mouvement
        public float height = 5.0f; // Amplitude du mouvement

        private Vector3 startPosition;

        void Start()
        {
            startPosition = transform.position; // Sauvegarde la position de départ
        }

        void Update()
        {
            // Calcule une nouvelle position en oscillant de haut en bas
            float newY = startPosition.y + Mathf.Sin(Time.time * speed) * height;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
}