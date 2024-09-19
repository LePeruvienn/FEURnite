using System;
using UnityEngine;
using Fusion;
using Fusion.Addons.SimpleKCC;
using Unity.VisualScripting;
using Random = UnityEngine.Random;
using UnityEngine.Animations.Rigging;

namespace Starter.ThirdPersonCharacter
{
	/// <summary>
	/// Main player scrip - controls player movement and animations.
	/// </summary>
	public sealed class Player : NetworkBehaviour
	{
        [Header("References")]
		public SimpleKCC KCC;
		public PlayerInput PlayerInput;
		public PlayerInventory PlayerInventory;
		public Animator Animator;
		public Transform CameraPivot;
		public Transform CameraHandle;

        [Header("Movement Setup")]
		public float WalkSpeed = 2f;
		public float SprintSpeed = 7f;
		public float AimSpeed = 1f;
		public float JumpImpulse = 14f;
		public float UpGravity = 25f;
		public float DownGravity = 40f;
		public float RotationSpeed = 8f;

		[Header("Movement Accelerations")]
		public float GroundAcceleration = 55f;
		public float GroundDeceleration = 25f;
		public float AirAcceleration = 25f;
		public float AirDeceleration = 1.3f;
		
		[Header("Camera Zoom")]
		public float normalFOV = 40f;    // Regular camera FOV
		public float aimFOV = 20f;       // Zoomed in FOV when aiming
		public float zoomSpeed = 5f;     // Speed of zoom transition
		
		[Header("Sounds")]
        public AudioClip[] FootstepAudioClips;
		public AudioClip LandingAudioClip;
		[Range(0f, 1f)]
		public float FootstepAudioVolume = 0.5f;

		[Networked]
		private NetworkBool _isJumping { get; set; }
		private NetworkBool _isAiming { get; set; } // Ajout d'une variable pour savoir si le joueur est en train de viser
		private NetworkBool _isMoving { get; set; }
        private NetworkBool _isShooting { get; set; }

        private Vector3 _moveVelocity;

        // Shoot mecanism
        [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask(); // à supprimé éventuellement ?

        // Animation IDs
        private int _animIDSpeed;
		private int _animIDGrounded;
		private int _animIDJump;
		private int _animIDFreeFall;
		private int _animIDMotionSpeed;
		private int _animIDAim;
		private int _animIDMoving;

		public override void FixedUpdateNetwork()
		{
			ProcessInput(PlayerInput.CurrentInput);

			if (KCC.IsGrounded)
			{
				// Stop jumping
				_isJumping = false;
			}

			PlayerInput.ResetInput();
		}

		public override void Render()
		{
			Animator.SetFloat(_animIDSpeed, KCC.RealSpeed, 0.15f, Time.deltaTime);
			Animator.SetFloat(_animIDMotionSpeed, 1f);
			Animator.SetBool(_animIDJump, _isJumping);
			Animator.SetBool(_animIDGrounded, KCC.IsGrounded);
			Animator.SetBool(_animIDFreeFall, KCC.RealVelocity.y < -10f);
			Animator.SetBool(_animIDAim, _isAiming);
			Animator.SetBool(_animIDMoving, _isMoving);
		}

		private void Awake()
		{
			AssignAnimationIDs();
		}

		private void LateUpdate()
		{
			// Only local player needs to update the camera
			// Note: In shared mode the local player has always state authority over player's objects.
			if (HasStateAuthority == false)
				return;

			// Update camera pivot and transfer properties from camera handle to Main Camera.
			CameraPivot.rotation = Quaternion.Euler(PlayerInput.CurrentInput.LookRotation);

            Camera.main.transform.SetPositionAndRotation(CameraHandle.position, CameraHandle.rotation);

			// Handle camera zooming based on whether the player is aiming
			float targetFOV = _isAiming ? aimFOV : normalFOV; // Set target FOV
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed); // Smoothly transition to target FOV
        }

		private void ProcessInput(GameplayInput input)
		{

            float jumpImpulse = 0f;

			// Comparing current input buttons to previous input buttons - this prevents glitches when input is lost
			if (KCC.IsGrounded && input.Jump)
			{
				// Set world space jump vector
				jumpImpulse = JumpImpulse;
				_isJumping = true;
			}

			// Set is Aiming to true if player is aiming
			_isAiming = input.Aiming;
            // Set is Shooting to true if player Shoot
            _isShooting = input.Shoot;

            // It feels better when the player falls quicker
            KCC.SetGravity(KCC.RealVelocity.y >= 0f ? UpGravity : DownGravity);
		
			// On définis la variable speed du joueur
			float speed;
			
			if (input.Aiming) // Si il vise
			{
				speed = AimSpeed;
			}
			else if (input.Sprint) // Si il est en train de courrir
			{
				speed = SprintSpeed;
			}
			else // Si il fait aucun des deux, alors il marche
			{
				speed = WalkSpeed;
			}
			
			// Inventory Update
			PlayerInventory.switchSelection(input.Scroll);
			
			var lookRotation = Quaternion.Euler(0f, input.LookRotation.y, 0f);
			// Calculate correct move direction from input (rotated based on camera look)
			var moveDirection = lookRotation * new Vector3(input.MoveDirection.x, 0f, input.MoveDirection.y); 
            var desiredMoveVelocity = moveDirection * speed;

			float acceleration;

			// Plus utile pour le shoot
			/*-------------------------------------------------------------------------------------*/
            Vector3 mouseWorldPosition = Vector3.zero;
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray, out RaycastHit rayCastHit, 999f, aimColliderLayerMask))
            {
                mouseWorldPosition = rayCastHit.point;
            }

            /*-------------------------------------------------------------------------------------*/

            // Determine acceleration based on whether the player is grounded or in the air
            acceleration = (desiredMoveVelocity == Vector3.zero)
                           ? (KCC.IsGrounded ? GroundDeceleration : AirDeceleration)
                           : (KCC.IsGrounded ? GroundAcceleration : AirAcceleration);

            // Get the camera's forward direction (ignore the Y axis to keep the rotation horizontal)
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0;  // Keep only the horizontal rotation part
            cameraForward.Normalize();

            // Smoothly rotate the player based on movement or camera orientation
            if (desiredMoveVelocity != Vector3.zero)
            {
                // Rotate based on movement direction
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                Quaternion nextRotation = Quaternion.Slerp(KCC.TransformRotation, targetRotation, RotationSpeed * Runner.DeltaTime);
                KCC.SetLookRotation(nextRotation.eulerAngles);
            }
            else
            {
                // Rotate the player towards the camera's forward direction if not moving
                Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
                Quaternion nextRotation = Quaternion.Slerp(KCC.TransformRotation, targetRotation, RotationSpeed * Runner.DeltaTime);
                KCC.SetLookRotation(nextRotation.eulerAngles);
            }

            // Aim rotation logic
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 directionToTarget = (worldAimTarget - KCC.transform.position).normalized;
            float angleToTarget = Vector3.Angle(KCC.transform.forward, directionToTarget);

            // Rotate smoothly if aiming or angle is greater than 80 degrees
            if (input.Aiming || angleToTarget > 80f)
            {
                Quaternion targetAimRotation = Quaternion.LookRotation(directionToTarget);
                Quaternion nextAimRotation = Quaternion.Slerp(KCC.TransformRotation, targetAimRotation, RotationSpeed * Runner.DeltaTime * 3);
                KCC.SetLookRotation(nextAimRotation.eulerAngles);

                // Ensure proper acceleration is set while rotating
                acceleration = KCC.IsGrounded ? GroundAcceleration : AirAcceleration;
            }




            /*-------------------------------------------------------------------------------------*/

            _moveVelocity = Vector3.Lerp(_moveVelocity, desiredMoveVelocity, acceleration * Runner.DeltaTime);

			// Ensure consistent movement speed even on steep slope
			if (KCC.ProjectOnGround(_moveVelocity, out var projectedVector))
			{
				_moveVelocity = projectedVector;
			}

			KCC.Move(_moveVelocity, jumpImpulse);
			
			// Check if Player is moving
			_isMoving = _moveVelocity.magnitude > 0.1f;

            //Create and Shoot the bullet
            if (input.Shoot)
			{
				PlayerInventory.useCurrentSelection();
			}
        }

        private void AssignAnimationIDs()
		{
			_animIDSpeed = Animator.StringToHash("Speed");
			_animIDGrounded = Animator.StringToHash("Grounded");
			_animIDJump = Animator.StringToHash("Jump");
			_animIDFreeFall = Animator.StringToHash("FreeFall");
			_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
			_animIDAim = Animator.StringToHash("Aim");
			_animIDMoving = Animator.StringToHash("Moving");
		}

		// Animation event
		private void OnFootstep(AnimationEvent animationEvent)
		{
			if (animationEvent.animatorClipInfo.weight < 0.5f)
				return;

			if (FootstepAudioClips.Length > 0)
			{
				var index = Random.Range(0, FootstepAudioClips.Length);
				AudioSource.PlayClipAtPoint(FootstepAudioClips[index], KCC.Position, FootstepAudioVolume);
			}
		}

		// Animation event
		private void OnLand(AnimationEvent animationEvent)
		{
			AudioSource.PlayClipAtPoint(LandingAudioClip, KCC.Position, FootstepAudioVolume);
		}
	}
}