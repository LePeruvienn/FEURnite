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
        //Ray ray = new Ray(transform.position, transform.forward);

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit rayCastHit,999f,aimColliderLayerMask))
        {
            if (rayCastHit.collider.tag != "ItemRaycast")
            {
                transform.position = rayCastHit.point;
            }
            Debug.Log("You hit a wall, good job!");
            Debug.Log(rayCastHit.collider.tag);
        }
    }
}