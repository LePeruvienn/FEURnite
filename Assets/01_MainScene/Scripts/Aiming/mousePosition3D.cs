using Starter.ThirdPersonCharacter;
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

        float playerDistance = 12f;


        // Loop through all hits from the ray
        foreach (RaycastHit hit in hits)
        {   
            // Calculate the distance from the camera to the hit point
            float hitDistance = Vector3.Distance(Camera.main.transform.position, hit.point);

            // Check if the current hit is not the one with the "ItemRaycast" tag
            if (hitDistance > playerDistance && hit.collider.tag != "ItemRaycast")
            {
                // If it's not, set the position to this point
                transform.position = hit.point;
                Debug.Log(hitDistance+ "hitDistance");
                Debug.Log(playerDistance + "playerDistance");
                break; // Stop checking after the first valid hit
            }
        }
    }
}