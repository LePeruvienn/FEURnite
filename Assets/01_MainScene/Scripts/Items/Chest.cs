using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

public class Chest : MonoBehaviour
{
    public Animator Animator;
    public ParticleSystem GlowParticule; 
    public Transform GlowTransform;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Animator.GetBool("isOpen") == false)
        {
            Animator.SetBool("isOpen", true); // ouvre le chest

            if (GlowTransform == null)
            {
                GlowTransform = transform; // or assign another Transform here if needed
            }

            GlowParticule.transform.position = new Vector3(
                0,
                transform.position.y + 0.60f,
                0
            );
            ParticleSystem Glow = Instantiate(GlowParticule, GlowTransform);
        }
    }
}
