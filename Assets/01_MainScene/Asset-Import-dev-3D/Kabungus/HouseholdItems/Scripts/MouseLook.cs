using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    public Transform playerBody;
    public float smoothing = 0.01f;
    [HideInInspector] public Vector2 smoothedVelocity;
    [HideInInspector] public Vector2 currentLookingPos;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector2 inputValues = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        inputValues = Vector2.Scale(inputValues, new Vector2(mouseSensitivity, mouseSensitivity));
        smoothedVelocity = Vector2.Lerp(smoothedVelocity, inputValues, (1f / smoothing) * Time.deltaTime);

        currentLookingPos += smoothedVelocity;
        currentLookingPos.y = Mathf.Clamp(currentLookingPos.y, -90f, 90f);
        transform.localRotation = Quaternion.AngleAxis(-currentLookingPos.y, Vector3.right);
        playerBody.localRotation = Quaternion.AngleAxis(currentLookingPos.x, playerBody.up);

    }
}
