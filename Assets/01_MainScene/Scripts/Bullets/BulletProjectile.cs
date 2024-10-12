using Starter.ThirdPersonCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody bulletRigidbody;
    [SerializeField] private Transform _vfxHitRed;
    [SerializeField] private Transform _vfxHitBlack;

    public int damage;

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
        PlayerModel pModel = other.GetComponent<PlayerModel>();
        if (pModel != null)
        {
            //hits target
            Debug.Log("PV AVANT: " + pModel.getCurrentTotalHealth());
            pModel.takeDamage(damage);
            Instantiate(_vfxHitRed, transform.position, Quaternion.identity);
            Debug.Log("PV APRES: " + pModel.getCurrentTotalHealth());
        }
        else 
        {
            //hit wall
            Instantiate(_vfxHitBlack, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
