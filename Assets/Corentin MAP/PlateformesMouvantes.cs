using Fusion;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Starter.ThirdPersonCharacter
{

    public class PlateformesMouvantes : NetworkBehaviour
    {
        private GameObject platformObject;

        [Networked]
        public float speed { get; set; } // Vitesse du mouvement
        [Networked]
        public float height { get; set; } // Amplitude du mouvement
        [Networked]
        private Vector3 startPosition { get; set; }
        private NetworkObject thePlatform { get; set; }
        [Networked]
        private NetworkTransform thePlatformTransform { get; set; }

        public override void Spawned()
        {
            base.Spawned();

            platformObject = this.gameObject;

            thePlatform = platformObject.gameObject.GetComponent<NetworkObject>();
            thePlatformTransform = platformObject.gameObject.GetComponent<NetworkTransform>();
            speed = 1.0f;
            height = 5.0f;
            startPosition = transform.position; // Sauvegarde la position de départ
        }

        public override void Render()
        {
            base.Render();

            // Calcule une nouvelle position en oscillant de haut en bas
            float newY = startPosition.y + Mathf.Sin(Time.time * speed) * height;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
}