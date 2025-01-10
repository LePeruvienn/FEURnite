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
        public float delaiAvantChute = 5f; // Temps avant que l'�le tombe
        public float delaiAvantDisparition = 10f; // Temps avant que l'�le disparaisse compl�tement
        public float dureeTremblement = 5f; // Dur�e du tremblement
        public float intensiteTremblement = 0.1f; // Intensit� du tremblement
        public Collider colliderSousLIle; // Collider � ignorer (celui sous l'�le)
        private Rigidbody rb; // Composant Rigidbody de l'�le
        private Collider colliderDeLIle; // Collider de l'�le
        private Vector3 positionInitiale; // Position initiale de l'�le

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

            // Lance la coroutine pour faire trembler puis tomber l'�le
            // StartCoroutine(TremblementEtChute());
        }

		private void initObject (GameObject obj) {

            // R�cup�re la position initiale
            positionInitiale = obj.transform.position;

            // R�cup�re le Rigidbody de l'�le
            rb = obj.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Composant Rigidbody manquant ! Veuillez ajouter un Rigidbody � l'�le.");
                return;
            }

            // Configure le Rigidbody
            rb.isKinematic = true; // L'�le reste en place jusqu'� ce qu'elle tombe
		}

        IEnumerator TremblementEtChute()
        {
            Debug.Log("Tremblement pendant " + dureeTremblement + " secondes");

            float elapsedTime = 0f;

            // Tremblement
            while (elapsedTime < dureeTremblement)
            {
                // Calcul d'un d�placement al�atoire autour de la position initiale
                Vector3 tremblement = new Vector3(
                    Random.Range(-intensiteTremblement, intensiteTremblement),
                    Random.Range(-intensiteTremblement, intensiteTremblement),
                    Random.Range(-intensiteTremblement, intensiteTremblement)
                );

                transform.position = positionInitiale + tremblement;

                elapsedTime += Time.deltaTime;
                yield return null; // Attend la prochaine frame
            }

            // Remet l'�le � sa position initiale
            transform.position = positionInitiale;

            // Lance la chute
            Debug.Log("L'�le commence � tomber apr�s le tremblement !");
            rb.isKinematic = false; // Permet au Rigidbody de tomber naturellement

			StartCoroutine(DisparaitreApresDelai());
        }

        IEnumerator DisparaitreApresDelai()
        {
            Debug.Log("L'�le dispara�tra dans " + delaiAvantDisparition + " secondes");
            yield return new WaitForSeconds(delaiAvantDisparition); // Attends le d�lai
            Debug.Log("L'�le dispara�t !");
            Destroy(gameObject); // D�truit l'�le
        }
    }
}
