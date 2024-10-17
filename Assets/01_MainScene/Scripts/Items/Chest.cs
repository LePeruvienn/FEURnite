using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Animator Animator;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Animator.GetBool("isOpen") == false)
        {

            Debug.Log("Open");
            Animator.SetBool("isOpen", true); // ouvre le chest 
        }
    }
}
