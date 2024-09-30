using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class mousePosition3D : MonoBehaviour
{
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    //Serial

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit[] hits = Physics.RaycastAll(ray, 999f, aimColliderLayerMask);

        // Loop through all hits from the ray
        foreach (RaycastHit hit in hits)
        {
            // Check if the current hit is not the one with the "ItemRaycast" tag
            if (hit.collider.tag != "ItemRaycast")
            {
                // If it's not, set the position to this point
                transform.position = hit.point;
                break; // Stop checking after the first valid hit
            }

            Debug.Log("You hit an object with tag: " + hit.collider.tag);
        }
    }
}