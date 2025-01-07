using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrainMouvement : MonoBehaviour
{
    public Transform isleCenter; // Assign the isle's center transform in the Inspector
    public float rotationSpeed = 10f; // Speed of rotation

    void Update()
    {
        // Rotate around the isle's center
        transform.RotateAround(isleCenter.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
