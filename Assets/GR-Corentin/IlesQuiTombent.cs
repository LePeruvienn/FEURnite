using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Starter.ThirdPersonCharacter
{
    // Enum for island type
    public enum IslandType
    {
        Spawn = 1,
        Inter = 2,
        Plateformes = 3
    }

    public class IlesQuiTombent : MonoBehaviour
    {
        [Header("Islands Lists")]
        public List<GameObject> spawnIslands;
        public List<GameObject> interIslands;
        public List<GameObject> plateformes;

        private List<Vector3> _defaultSpawnPos;
        private List<Vector3> _defaultInterPos;
        private List<Vector3> _defaultPlateformesPos;

        private List<Quaternion> _defaultSpawnRotation;
        private List<Quaternion> _defaultInterRotation;
        private List<Quaternion> _defaultPlateformesRotation;

        [Header("Island Falling Timers")]
        public float delaiAvantChute = 5f; // Time before the island falls
        public float delaiAvantDisparition = 10f; // Time before the island completely disappears
        public float dureeTremblement = 5f; // Duration of the shaking
        public float intensiteTremblement = 0.1f; // Intensity of the shaking

		private List<Coroutine> _coroutines;

        void Start()
        {
			// Init coroutine list
			_coroutines = new List<Coroutine> ();

            // Initialize the default position lists
            _defaultSpawnPos = new List<Vector3>();
            _defaultInterPos = new List<Vector3>();
            _defaultPlateformesPos = new List<Vector3>();

            _defaultSpawnRotation = new List<Quaternion>();
            _defaultInterRotation = new List<Quaternion>();
            _defaultPlateformesRotation = new List<Quaternion>();

            // Initialize spawn islands
            foreach (var island in spawnIslands)
            {
                initObject(island);
                _defaultSpawnPos.Add(island.transform.position);
                _defaultSpawnRotation.Add(island.transform.rotation);
            }

            // Initialize inter islands
            foreach (var island in interIslands)
            {
                initObject(island);
                _defaultInterPos.Add(island.transform.position);
                _defaultInterRotation.Add(island.transform.rotation);
            }

            // Initialize platform islands
            foreach (var island in plateformes)
            {
                initObject(island);
                _defaultPlateformesPos.Add(island.transform.position);
                _defaultPlateformesRotation.Add(island.transform.rotation);
            }
        }

        public void fallIslands(IslandType type)
        {
            List<GameObject> islands = null;

            // Get the selected islands
            switch (type)
            {
                case IslandType.Spawn:
                    Debug.Log("FALLING SPAWN");
                    islands = spawnIslands;
                    break;

                case IslandType.Inter:
                    Debug.Log("FALLING INTER");
                    islands = interIslands;
                    break;

                case IslandType.Plateformes:
                    Debug.Log("FALLING PLATEFORMES");
                    islands = plateformes;
                    break;
            }

            if (islands == null || islands.Count == 0)
            {
                Debug.LogWarning("No islands to process for type: " + type);
                return;
            }

            // Start falling process for each island
            foreach (var island in islands)
            {
                _coroutines.Add (StartCoroutine(TremblementEtChute(island)));
            }
        }

        private void initObject(GameObject obj)
        {
            // Retrieve the Rigidbody of the island
            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Missing Rigidbody component! Please add a Rigidbody to the island: " + obj.name);
                return;
            }

            // Configure the Rigidbody
            rb.isKinematic = true; // The island remains stationary until it falls
        }

        IEnumerator TremblementEtChute(GameObject obj)
        {
            Debug.Log("TremblementEtChute started for: " + obj.name);

            Vector3 initialPosition = obj.transform.position;
            float elapsedTime = 0f;

            // Shaking
            while (elapsedTime < dureeTremblement)
            {
                Vector3 shake = new Vector3(
                    Random.Range(-intensiteTremblement, intensiteTremblement),
                    Random.Range(-intensiteTremblement, intensiteTremblement),
                    Random.Range(-intensiteTremblement, intensiteTremblement)
                );

                obj.transform.position = initialPosition + shake;

                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }

            // Reset island to its initial position
            obj.transform.position = initialPosition;

            // Make the island fall
            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Missing Rigidbody component! Please add a Rigidbody to the island: " + obj.name);
                yield break;
            }

            rb.isKinematic = false; // Allow the Rigidbody to fall naturally

            // Start disappearance process
            _coroutines.Add (StartCoroutine(DisparaitreApresDelai(obj)));
        }

        IEnumerator DisparaitreApresDelai(GameObject obj)
        {
            // Wait for the delay
            yield return new WaitForSeconds(delaiAvantDisparition);

            // Make the island disappear
            obj.SetActive(false);
        }

        public void resetAll()
        {
            // Reset spawn islands
            for (int i = 0; i < spawnIslands.Count; i++)
            {
                resetIsland(spawnIslands[i], _defaultSpawnPos[i], _defaultSpawnRotation[i]);
            }

            // Reset inter islands
            for (int i = 0; i < interIslands.Count; i++)
            {
                resetIsland(interIslands[i], _defaultInterPos[i], _defaultInterRotation[i]);
            }

            // Reset platforms
            for (int i = 0; i < plateformes.Count; i++)
            {
                resetIsland(plateformes[i], _defaultPlateformesPos[i], _defaultPlateformesRotation[i]);
            }
        }

        private void resetIsland(GameObject obj, Vector3 defaultPos, Quaternion defaultRotation)
        {
			stopAllCoroutine ();

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Missing Rigidbody component! Please add a Rigidbody to the island: " + obj.name);
                return;
            }

            // Configure the Rigidbody
            rb.isKinematic = true; // The island remains stationary until it falls

            // Reset object position & rotation
            obj.transform.position = defaultPos;
			obj.transform.rotation = defaultRotation;

            // Reactivate the object
            obj.SetActive(true);
        }

		private void stopAllCoroutine ()
		{
			foreach (Coroutine coroutine in _coroutines)
				StopCoroutine (coroutine);
		}
    }
}
