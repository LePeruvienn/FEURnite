using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody bulletRigidbody;


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
        Destroy(gameObject);
    }
}
