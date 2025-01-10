using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Starter.ThirdPersonCharacter
{

    public class IlesQuiTombent : MonoBehaviour
    {
        [Header("Inslands Lists")]
		public List<GameObject> spawnIslands;
		public List<GameObject> spawnPlatformes;
		public List<GameObject> interInslands;
		public List<GameObject> interPlateformes;

        [Header("Insland Falling Timers")]
        public float delaiAvantChute = 5f; // Temps avant que l'île tombe
        public float delaiAvantDisparition = 10f; // Temps avant que l'île disparaisse complètement
        public float dureeTremblement = 5f; // Durée du tremblement
        public float intensiteTremblement = 0.1f; // Intensité du tremblement
        public Collider colliderSousLIle; // Collider à ignorer (celui sous l'île)
        private Rigidbody rb; // Composant Rigidbody de l'île
        private Collider colliderDeLIle; // Collider de l'île
        private Vector3 positionInitiale; // Position initiale de l'île

		// Here we init all the inslands and plateformes
        void Start()
        {
			// Init spawns islands
			foreach (GameObject insland in spawnIslands)
				initObject (insland);
			
			// Init spawns plateformes
			foreach (GameObject plateforme in spawnPlatformes)
				initObject (plateforme);

			// Init inter islands
			foreach (GameObject insland in interInslands)
				initObject (insland);
			
			// Init inter plateformes
			foreach (GameObject plateforme in interPlateformes)
				initObject (plateforme);

            // Lance la coroutine pour faire trembler puis tomber l'île
            // StartCoroutine(TremblementEtChute());
        }

		private void initObject (GameObject obj) {

            // Récupère la position initiale
            positionInitiale = obj.transform.position;

            // Récupère le Rigidbody de l'île
            rb = obj.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Composant Rigidbody manquant ! Veuillez ajouter un Rigidbody à l'île.");
                return;
            }

            // Configure le Rigidbody
            rb.isKinematic = true; // L'île reste en place jusqu'à ce qu'elle tombe
		}

        IEnumerator TremblementEtChute()
        {
            Debug.Log("Tremblement pendant " + dureeTremblement + " secondes");

            float elapsedTime = 0f;

            // Tremblement
            while (elapsedTime < dureeTremblement)
            {
                // Calcul d'un déplacement aléatoire autour de la position initiale
                Vector3 tremblement = new Vector3(
                    Random.Range(-intensiteTremblement, intensiteTremblement),
                    Random.Range(-intensiteTremblement, intensiteTremblement),
                    Random.Range(-intensiteTremblement, intensiteTremblement)
                );

                transform.position = positionInitiale + tremblement;

                elapsedTime += Time.deltaTime;
                yield return null; // Attend la prochaine frame
            }

            // Remet l'île à sa position initiale
            transform.position = positionInitiale;

            // Lance la chute
            Debug.Log("L'île commence à tomber après le tremblement !");
            rb.isKinematic = false; // Permet au Rigidbody de tomber naturellement

			StartCoroutine(DisparaitreApresDelai());
        }

        IEnumerator DisparaitreApresDelai()
        {
            Debug.Log("L'île disparaîtra dans " + delaiAvantDisparition + " secondes");
            yield return new WaitForSeconds(delaiAvantDisparition); // Attends le délai
            Debug.Log("L'île disparaît !");
            Destroy(gameObject); // Détruit l'île
        }
    }
}
