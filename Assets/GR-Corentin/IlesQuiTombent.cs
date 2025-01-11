using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Starter.ThirdPersonCharacter
{
    // Enum for island type
    public enum InslandType
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

        [Header("Island Falling Timers")]
        public float delaiAvantChute = 5f; // Time before the island falls
        public float delaiAvantDisparition = 10f; // Time before the island completely disappears
        public float dureeTremblement = 5f; // Duration of the shaking
        public float intensiteTremblement = 0.1f; // Intensity of the shaking

        void Start()
        {
            // Initialize the default position lists
            _defaultSpawnPos = new List<Vector3>();
            _defaultInterPos = new List<Vector3>();
            _defaultPlateformesPos = new List<Vector3>();

            // Initialize spawn islands
            foreach (var island in spawnIslands)
            {
                initObject(island);
                _defaultSpawnPos.Add(island.transform.position);
            }

            // Initialize inter islands
            foreach (var island in interIslands)
            {
                initObject(island);
                _defaultInterPos.Add(island.transform.position);
            }

            // Initialize platform islands
            foreach (var island in plateformes)
            {
                initObject(island);
                _defaultPlateformesPos.Add(island.transform.position);
            }
        }

        public void fallInslands(InslandType type)
        {
            List<GameObject> islands = null;

            // Get the selected islands
            switch (type)
            {
                case InslandType.Spawn:
                    Debug.Log("FALLING SPAWN");
                    islands = spawnIslands;
                    break;

                case InslandType.Inter:
                    Debug.Log("FALLING INTER");
                    islands = interIslands;
                    break;

                case InslandType.Plateformes:
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
                StartCoroutine(TremblementEtChute(island));
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
            StartCoroutine(DisparaitreApresDelai(obj));
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
                resetIsland(spawnIslands[i], _defaultSpawnPos[i]);
            }

            // Reset inter islands
            for (int i = 0; i < interIslands.Count; i++)
            {
                resetIsland(interIslands[i], _defaultInterPos[i]);
            }

            // Reset platforms
            for (int i = 0; i < plateformes.Count; i++)
            {
                resetIsland(plateformes[i], _defaultPlateformesPos[i]);
            }
        }

        private void resetIsland(GameObject obj, Vector3 defaultPos)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Missing Rigidbody component! Please add a Rigidbody to the island: " + obj.name);
                return;
            }

            // Configure the Rigidbody
            rb.isKinematic = true; // The island remains stationary until it falls

            // Reset object position
            obj.transform.position = defaultPos;

            // Reactivate the object
            obj.SetActive(true);
        }
    }
}
