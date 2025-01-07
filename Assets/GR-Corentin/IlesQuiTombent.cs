using System.Collections;
using UnityEngine;

public class IlesQuiTombent : MonoBehaviour
{
    public float delaiAvantChute = 2f; // Temps avant que l'�le tombe
    public Collider colliderSousLIle; // Collider � ignorer (celui sous l'�le)
    private Rigidbody rb; // Composant Rigidbody de l'�le
    private Collider colliderDeLIle; // Collider de l'�le

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        colliderDeLIle = GetComponent<Collider>();

        if (rb == null || colliderDeLIle == null || colliderSousLIle == null)
        {
            Debug.LogError("Un ou plusieurs composants manquent : Rigidbody, Collider de l'�le ou Collider sous l'�le.");
            return;
        }

        // V�rifiez si le MeshCollider est convex
        MeshCollider meshCollider = colliderDeLIle as MeshCollider;
        if (meshCollider != null && !meshCollider.convex)
        {
            Debug.LogWarning("Le MeshCollider de l'�le n'est pas convex, cela peut poser des probl�mes de collision. Activez l'option 'Convex'.");
        }

        rb.isKinematic = true; // L'�le reste en place

        // Ignore les collisions entre le MeshCollider et le BoxCollider
        if (colliderDeLIle != null && colliderSousLIle != null)
        {
            Physics.IgnoreCollision(colliderDeLIle, colliderSousLIle);
            Debug.Log($"Collision ignor�e entre {colliderDeLIle.name} et {colliderSousLIle.name}");
        }
        else
        {
            Debug.LogError("Colliders non d�finis correctement ou manquants !");
        }

        StartCoroutine(ChuteApresDelai());
    }

    IEnumerator ChuteApresDelai()
    {
        Debug.Log("D�lai avant la chute : " + delaiAvantChute + " secondes");
        yield return new WaitForSeconds(delaiAvantChute); // Attends le d�lai
        Debug.Log("L'�le tombe !");
        rb.isKinematic = false; // Permet � l'�le de tomber
    }
}
