using System.Collections;
using UnityEngine;

public class IlesQuiTombent : MonoBehaviour
{
    public float delaiAvantChute = 5f; // Temps avant que l'île tombe
    public float delaiAvantDisparition = 10f; // Temps avant que l'île disparaisse complètement
    public float dureeTremblement = 5f; // Durée du tremblement
    public float intensiteTremblement = 0.1f; // Intensité du tremblement
    public Collider colliderSousLIle; // Collider à ignorer (celui sous l'île)
    private Rigidbody rb; // Composant Rigidbody de l'île
    private Collider colliderDeLIle; // Collider de l'île
    private Vector3 positionInitiale; // Position initiale de l'île

    void Start()
    {
        // Récupère la position initiale
        positionInitiale = transform.position;

        // Récupère le Rigidbody de l'île
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Composant Rigidbody manquant ! Veuillez ajouter un Rigidbody à l'île.");
            return;
        }

        // Configure le Rigidbody
        rb.isKinematic = true; // L'île reste en place jusqu'à ce qu'elle tombe

        // Récupère le Collider de l'île
        colliderDeLIle = GetComponent<Collider>();
        if (colliderDeLIle != null && colliderSousLIle != null)
        {
            // Ignore les collisions entre l'île et le Collider en dessous
            Physics.IgnoreCollision(colliderDeLIle, colliderSousLIle);
            Debug.Log("Collision ignorée entre l'île et le Collider en dessous !");
        }
        else
        {
            Debug.LogError("Colliders non définis correctement !");
        }

        // Lance la coroutine pour faire trembler puis tomber l'île
        StartCoroutine(TremblementEtChute());
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
    }

    void OnCollisionEnter(Collision collision)
    {
        // Début de la disparition si l'île touche le sol
        if (!rb.isKinematic)
        {
            Debug.Log("Collision détectée. Début de la disparition !");
            StartCoroutine(DisparaitreApresDelai());
        }
    }

    IEnumerator DisparaitreApresDelai()
    {
        Debug.Log("L'île disparaîtra dans " + delaiAvantDisparition + " secondes");
        yield return new WaitForSeconds(delaiAvantDisparition); // Attends le délai
        Debug.Log("L'île disparaît !");
        Destroy(gameObject); // Détruit l'île
    }
}
