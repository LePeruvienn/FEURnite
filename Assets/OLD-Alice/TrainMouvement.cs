using Fusion;
using UnityEngine;

public class TrainMouvement : NetworkBehaviour
{
    public Transform isleCenter; // Assign the isle's center transform in the Inspector
    public GameObject trainObject;

    [Networked]
    private Vector3 isleCenterPosition { get; set; }
    [Networked]
    private float rotationSpeed { get; set; } // Speed of rotation
    [Networked]
    private NetworkObject theTrain { get; set; }
    [Networked]
    private NetworkTransform theTrainTransform { get; set; }

    public override void Spawned()
    {

        base.Spawned();

        theTrain = trainObject.gameObject.GetComponent<NetworkObject>();
        rotationSpeed = 10f;
        isleCenterPosition = isleCenter.position;
        theTrainTransform = trainObject.gameObject.GetComponent<NetworkTransform>();
    }

    public override void Render()
    {
        // Rotate around the isle's center
        //transform.RotateAround(isleCenter.position, Vector3.up, rotationSpeed * Time.deltaTime);
        transform.RotateAround(isleCenterPosition, Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
