using System.Collections;
using UnityEngine;

public class IlesQuiTombent : MonoBehaviour
{
    public float delaiAvantChute = 2f; // Temps avant que l'île tombe
    public Collider colliderSousLIle; // Collider à ignorer (celui sous l'île)
    private Rigidbody rb; // Composant Rigidbody de l'île
    private Collider colliderDeLIle; // Collider de l'île

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        colliderDeLIle = GetComponent<Collider>();

        if (rb == null || colliderDeLIle == null || colliderSousLIle == null)
        {
            Debug.LogError("Un ou plusieurs composants manquent : Rigidbody, Collider de l'île ou Collider sous l'île.");
            return;
        }

        // Vérifiez si le MeshCollider est convex
        MeshCollider meshCollider = colliderDeLIle as MeshCollider;
        if (meshCollider != null && !meshCollider.convex)
        {
            Debug.LogWarning("Le MeshCollider de l'île n'est pas convex, cela peut poser des problèmes de collision. Activez l'option 'Convex'.");
        }

        rb.isKinematic = true; // L'île reste en place

        // Ignore les collisions entre le MeshCollider et le BoxCollider
        if (colliderDeLIle != null && colliderSousLIle != null)
        {
            Physics.IgnoreCollision(colliderDeLIle, colliderSousLIle);
            Debug.Log($"Collision ignorée entre {colliderDeLIle.name} et {colliderSousLIle.name}");
        }
        else
        {
            Debug.LogError("Colliders non définis correctement ou manquants !");
        }

        StartCoroutine(ChuteApresDelai());
    }

    IEnumerator ChuteApresDelai()
    {
        Debug.Log("Délai avant la chute : " + delaiAvantChute + " secondes");
        yield return new WaitForSeconds(delaiAvantChute); // Attends le délai
        Debug.Log("L'île tombe !");
        rb.isKinematic = false; // Permet à l'île de tomber
    }
}
