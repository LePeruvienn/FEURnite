using Fusion;
using System;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

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
        [Networked]
        private Vector3 position { get; set; }

        [Networked]
        private NetworkObject thePlatform { get; set; }
        [Networked]
        private NetworkTransform thePlatformTransform { get; set; }

        [Networked]
        private float newX { get; set; }
        [Networked]
        private float newY { get; set; }
        [Networked]
        private float newZ { get; set; }

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
            // Calcule une nouvelle position en oscillant de haut en bas
            //newX = transform.position.x;
            //newY = startPosition.y + Mathf.Sin(Time.time * speed) * height;
            //newZ = transform.position.z;

            //position = new Vector3(newX, newY, newZ);

            //transform.position = position;
        }

        public override void FixedUpdateNetwork()
        {
            newX = transform.position.x;
            newY = startPosition.y + Mathf.Sin(Time.time * speed) * height;
            newZ = transform.position.z;
            position = new Vector3(newX, newY, newZ);

            // Synchronisez la position avec NetworkTransform
            //thePlatformTransform.Teleport(newPosition);
            if (Runner.IsClient) // Vérifie si c'est un serveur dédié
            {
                // Synchronisation réseau pour les clients
                GetComponent<NetworkTransform>().Teleport(position);
            }
            else
                // Mise à jour immédiate pour l'hôte
                transform.position = position;
        }
    }
}