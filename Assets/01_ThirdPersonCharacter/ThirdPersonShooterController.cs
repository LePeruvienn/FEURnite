using GLTFast.Addons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Fusion.Addons.SimpleKCC;
using Fusion;
using Starter.ThirdPersonCharacter;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class ThirdPersonShooterController : NetworkBehaviour
{

    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;

    public SimpleKCC KCC;
    public PlayerInput PlayerInput;

    // Update is called once per frame
    private void Update()
    {

        /*Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2 (Screen.width /2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit rayCastHit, 999f, aimColliderLayerMask)) { 
            debugTransform.position = rayCastHit.point;
            mouseWorldPosition = rayCastHit.point;
        }


        Debug.Log(PlayerInput.CurrentInput.Aiming);
        if (PlayerInput.CurrentInput.Aiming)
        {
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            Debug.Log(aimDirection);
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f); 
            



            var currentRotation = KCC.TransformRotation;
            var targetRotation = Quaternion.LookRotation(moveDirection);
            var nextRotation = Quaternion.Lerp(currentRotation, targetRotation, RotationSpeed * Runner.DeltaTime);

            KCC.SetLookRotation(nextRotation.eulerAngles);
        }
        else {

            Debug.Log("Aiming == false");
        }*/
    }
}
