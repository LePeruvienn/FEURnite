using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody bulletRigidbody;
    [SerializeField] private Transform vfxHitRed;
    [SerializeField] private Transform vfxHitBlack;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    private void Update()
    {
        float speed = 50f;
        bulletRigidbody.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {   

        if (other.GetComponent<BulletTarget>() != null)
        {
            //hit target
            Instantiate(vfxHitRed, transform.position, Quaternion.identity);
        }
        else
        {
            //hit wall
            Instantiate(vfxHitBlack, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject);
    }
}
