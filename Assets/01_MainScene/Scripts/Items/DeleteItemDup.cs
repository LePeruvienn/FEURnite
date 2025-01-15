using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteItemDup : MonoBehaviour
{
    void OnTriggerEnter(Collider ItemCollider)
    {
        Debug.Log(ItemCollider.gameObject + "IS DESTROY");
        //Destroy(ItemCollider.gameObject);
    }
}
