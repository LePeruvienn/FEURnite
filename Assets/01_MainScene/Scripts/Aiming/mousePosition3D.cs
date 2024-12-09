using Starter.ThirdPersonCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;
using Fusion;

public class mousePosition3D : NetworkBehaviour
{
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    //Serial

    // ############################# teste dodo
    [Networked]
    private Vector3 TargetPosition { get; set; }
    // ############################# teste dodo

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
	public override void FixedUpdateNetwork()
    {
        //if (!Object.HasInputAuthority) return;  // ############################# teste dodo

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit[] hits = Physics.RaycastAll(ray, 999f, aimColliderLayerMask);

        // Loop through all hits from the ray
        foreach (RaycastHit hit in hits)
        {   
            // Check if the current hit is not the one with the "ItemRaycast" tag
            if (hit.collider.tag == "WALL")
            {
                // If it's not, set the position to this point
                //transform.position = hit.point;
                // ############################# teste dodo
                TargetPosition = hit.point;
                // ############################# teste dodo
                break; // Stop checking after the first valid hit
            }
        }
    }

    public override void Render()
    {
        // Synchronisez la position sur tous les clients
        transform.position = TargetPosition;
    }
}
