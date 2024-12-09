using UnityEngine;

public class FreecamController : MonoBehaviour
{
    public float speed = 10f;
    public float lookSpeed = 2f;

    private float yaw = 0f;
    private float pitch = 0f;

    void Update()
    {
        // D�placement (WASD ou fl�ches directionnelles)
        float moveX = Input.GetAxis("Horizontal"); // A/D ou fl�ches gauche/droite
        float moveZ = Input.GetAxis("Vertical");   // W/S ou fl�ches haut/bas

        Vector3 movement = transform.forward * moveZ + transform.right * moveX;
        transform.position += movement * speed * Time.deltaTime;

        // Rotation (Souris)
        yaw += Input.GetAxis("Mouse X") * lookSpeed;
        pitch -= Input.GetAxis("Mouse Y") * lookSpeed;
        pitch = Mathf.Clamp(pitch, -90f, 90f); // Limiter l'angle de vision vertical

        transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        
    }
}
