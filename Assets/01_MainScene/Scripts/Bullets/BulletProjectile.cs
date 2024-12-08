using Fusion;
using UnityEngine;

public class BulletProjectile : NetworkBehaviour
{
    private Rigidbody bulletRigidbody;
    [SerializeField] private GameObject vfxHitRed;
    [SerializeField] private GameObject vfxHitBlack;

    public override void Spawned()
    {
        Debug.LogWarning("--bullet ID received : " + Object.Id + " position arme :" + Object.transform.position );

        bulletRigidbody = GetComponent<Rigidbody>();
    }

    public override void FixedUpdateNetwork()
    {
 
         float speed = 50f;
         bulletRigidbody.velocity = transform.forward * speed;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject vfxToSpawn = null;
        Vector3 spawnPosition = transform.position; // Position par défaut (avant la correction)
        Debug.LogWarning("--particule ID received : " + Object.Id + " position arme :" + Object.transform.position);

        if (other.GetComponent<BulletTarget>() != null)
        {
            // Hit target
            vfxToSpawn = vfxHitRed;
        }
        else
        {
            // Hit wall
            vfxToSpawn = vfxHitBlack;
        }

        if (vfxToSpawn != null)
        {
            // Si une collision a eu lieu avec un objet, ajustez la position de spawn des particules
            if (other.TryGetComponent(out Collider colliderHit))
            {
                spawnPosition = colliderHit.ClosestPointOnBounds(transform.position); // Utilisez la position d'impact réelle
            }

            // Spawn particle effect using Runner.Spawn to sync it across all clients
            Instantiate(vfxToSpawn, spawnPosition, Quaternion.identity);
        }

        // Despawn bullet
        Runner.Despawn(Object); // Destroy the bullet NetworkObject via Fusion
    }


}
