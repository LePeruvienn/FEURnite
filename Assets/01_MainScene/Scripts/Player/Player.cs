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
		public PlayerModel PlayerModel;
		public PlayerInput PlayerInput;
		public PlayerInventory PlayerInventory;
		public InventoryDisplay InventoryDisplay;
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
		[Networked]
		private NetworkBool _isAiming { get; set; } // Ajout d'une variable pour savoir si le joueur est en train de vise
		[Networked]
		private NetworkBool _isMoving { get; set; }
		private NetworkBool _isShooting { get; set; }
		private Vector3 _moveVelocity;

		[Networked]
		public bool DebugIsDead {get; set;}

		[Networked]
		public bool isAlive {get; set;} = true;

		[Networked]
		public bool isWinner {get; set;} = false;

		private GameManager gameManager;
		private CameraSwitcher cameraSwitcher;

		// Shoot mecanism
		[SerializeField] private LayerMask aimColliderLayerMask = new LayerMask(); // à supprimé éventuellement ?

        // Animation input
        public MultiAimConstraint multiAimConstraintBody;
        public MultiAimConstraint multiAimConstraintHead;
        public MultiAimConstraint multiAimConstraintWeapon;
        public TwoBoneIKConstraint multiAimConstraintArm;

        [Networked]
        private NetworkObject multiAimConstraintBodyObject { get; set; } // Référence au NetworkObject

        [Networked]
        private NetworkObject multiAimConstraintHeadObject { get; set; } // Référence au NetworkObject

        [Networked]
        private NetworkObject multiAimConstraintWeaponObject { get; set; } // Référence au NetworkObject

        [Networked]
        private NetworkObject multiAimConstraintArmObject { get; set; } // Référence au NetworkObject
       
		[Networked]
		private float BodyWeight { get; set; }
        [Networked]
        private float HeadWeight { get; set; }
        [Networked]
        private float WeaponWeight { get; set; }
        [Networked]
        private float ArmWeight { get; set; }


        // Animation IDs
        private int _animIDSpeed;
		private int _animIDGrounded;
		private int _animIDJump;
		private int _animIDFreeFall;
		private int _animIDMotionSpeed;
		private int _animIDAim;
		private int _animIDMoving;
		private int _animIDReload;

        // ############################# teste dodo

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_Reload()
        {
            Animator.SetTrigger(_animIDReload);
        }

        //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        //private void RPC_UpdateAimState(bool isAiming)
        //{
        //	_isAiming = isAiming;
        //}

        // ############################# teste dodo

        public override void Spawned() {

			base.Spawned ();

            multiAimConstraintBodyObject = multiAimConstraintBody.gameObject.GetComponent<NetworkObject>();
            multiAimConstraintHeadObject = multiAimConstraintHead.gameObject.GetComponent<NetworkObject>();
            multiAimConstraintWeaponObject = multiAimConstraintWeapon.gameObject.GetComponent<NetworkObject>();
            multiAimConstraintArmObject = multiAimConstraintArm.gameObject.GetComponent<NetworkObject>();

            DebugIsDead = false;
            isAlive = true;
		}

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

            multiAimConstraintArm.weight = ArmWeight;
            multiAimConstraintWeapon.weight = WeaponWeight;
            multiAimConstraintBody.weight = BodyWeight;
            multiAimConstraintHead.weight = HeadWeight;
        }

        private void Awake()
		{

			// Cacher le curseur et verrouiller comme avant
			Cursor.lockState = CursorLockMode.Locked;  // Verrouiller le curseur au centre de l'écran
			Cursor.visible = false;

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
				jumpImpulse = JumpImpulse * PlayerModel.jumpPower; // Player Model jumpower
				_isJumping = true;
			}

			// Check current weapon type selected
			GameObject selectedObj = PlayerInventory.getCurrentSelection();

			// Setup selected item
			Item selectedItem = null;
            ItemType selectedType = ItemType.None;
            BulletType selectedBulletType = BulletType.Pistol;

            if (selectedObj != null)
				selectedItem = selectedObj.GetComponent<Item> ();
	
			if (selectedItem != null)
			{
                selectedType = selectedItem.getType();
                selectedBulletType = selectedItem.getBulletType();
				if(selectedBulletType == BulletType.Sniper)
				{
					aimFOV = 2f;
				}
				else
				{   
					aimFOV = 20f;
				}
            }

            // Set is Aiming to true if player is aiming and selected item is a weapon
            _isAiming = (selectedType == ItemType.Weapon) ?
                input.Aiming : false;

			// Set is Shooting to true if player Shoot
			_isShooting = input.Shoot;

			// It feels better when the player falls quicker
			KCC.SetGravity(KCC.RealVelocity.y >= 0f ? UpGravity : DownGravity);

			// On définis la variable speed du joueur
			float speed;

			if (_isAiming) // Si il vise
			{
				speed = AimSpeed;
			}
			else if (input.Sprint) // Si il est en train de courrir
			{
				speed = SprintSpeed;
                if (selectedType == ItemType.Weapon)
                {
                    Weapon selectedWeapon = (Weapon)selectedItem;
                    if (selectedWeapon.weight != null)
                    {
                        speed = SprintSpeed * selectedWeapon.weight._weight;
                    }
                }
            }
            else // Si il fait aucun des deux, alors il marche
			{
				speed = WalkSpeed;
                if (selectedType == ItemType.Weapon)
                {
                    Weapon selectedWeapon = (Weapon)selectedItem;
                    if (selectedWeapon.weight != null)
                    {
                        speed = WalkSpeed * selectedWeapon.weight._weight;
                    }
                }
            }

            // Multiplie by playerSpeed
            speed *= PlayerModel.speed;
			//Debug.Log ("speed: ", speed);

			// Inventory Update
			PlayerInventory.switchSelection(input.Scroll); 
			
			if (input.FirstInvSlot)
            {
                PlayerInventory.switchToSelection(0);
            }
            if (input.SecondInvSlot)
            {
                PlayerInventory.switchToSelection(1);
            }
            if (input.ThirdInvSlot)
            {
                PlayerInventory.switchToSelection(2);
            }
            if (input.FourthInvSlot)
            {
                PlayerInventory.switchToSelection(3);
            }
            // Drop item
            if (input.DropItem)
			{
				PlayerInventory.dropCurrentSelection();
			}

			if (input.ToggleInventory)
				InventoryDisplay.toggleInventory();


			var lookRotation = Quaternion.Euler(0f, input.LookRotation.y, 0f);
			// Calculate correct move direction from input (rotated based on camera look)
			var moveDirection = lookRotation * new Vector3(input.MoveDirection.x, 0f, input.MoveDirection.y);
			var desiredMoveVelocity = moveDirection * speed;


			//create a Raycast to get the mouse position to use for the constaint
			Vector3 mouseWorldPosition = Vector3.zero;
			Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
			RaycastHit[] hits = Physics.RaycastAll(ray, 999f, aimColliderLayerMask);

			// Loop through all hits from the ray
			foreach (RaycastHit hit in hits)
			{
				// Check if the current hit is not the one with the "ItemRaycast" tag
				if (hit.collider.tag == "WALL")
				{
					// If it's not, set the position to this point
					mouseWorldPosition = hit.point;
					break; // Stop checking after the first valid hit
				}
			}

			// Get the forward direction of the player
			Vector3 playerForward = KCC.transform.forward;
			mouseWorldPosition.y = transform.position.y;

			// Calculate the direction to the target
			Vector3 directionToTarget = (mouseWorldPosition - KCC.transform.position).normalized;
			// Calculate the angle between the player's forward direction and the direction to the target
			float angleToTarget = Vector3.Angle(playerForward, directionToTarget);
			// Get the currentRotation 
			Quaternion currentRotation = KCC.TransformRotation;
			Quaternion targetRotation = Quaternion.LookRotation(directionToTarget.normalized);

			//speed of the rotation
			var multiplicator = 1;
			//speed of the Player
			float acceleration;

			// Rotate the character towards move direction over time
			if (desiredMoveVelocity == Vector3.zero)
			{
				// No desired move velocity - we are stopping
				acceleration = KCC.IsGrounded ? GroundDeceleration : AirDeceleration;

				if ((_isAiming || angleToTarget > 80f) && KCC.IsGrounded) // if is aiming or moving the camera we activate the Constrainte on the animation to make him aim properly
				{
                    ActivateConstraintAim();
                    targetRotation = Quaternion.LookRotation(directionToTarget.normalized);
					multiplicator = 4;
				}
				else
				{
                    DeactivateConstraintAim();
                }
                // we alwase need te constraint for movement si le joueur ne mouv pas
                ActivateConstraintMovement();
            }
			else
			{
				if (_isAiming && KCC.IsGrounded)// si le joueur mouv et qu'il vise on doit activer les constraint sur l'animation 
				{
                    ActivateConstraintAim();
                    targetRotation = Quaternion.LookRotation(directionToTarget.normalized);
					multiplicator = 3;
				}
				else
				{
                    DeactivateConstraintAim();
                    targetRotation = Quaternion.LookRotation(moveDirection);
					multiplicator = 1;
				}
				if (angleToTarget > 130f) // si le joueur regarde deriére lui pour évité des bug on désactive la constraint
				{
                    DeactivateConstraintMovement();
				}
				else
				{
                    ActivateConstraintMovement();
				}
			}

			// Smoothly rotate the character towards the LookTarget
			Quaternion nextRotation = Quaternion.Lerp(KCC.TransformRotation, targetRotation, RotationSpeed * Runner.DeltaTime * multiplicator);
			// AND Apply the rotation that we calculated 
			KCC.SetLookRotation(nextRotation.eulerAngles);

			//controle la vitesse du player 
			acceleration = KCC.IsGrounded ? GroundAcceleration : AirAcceleration;
			_moveVelocity = Vector3.Lerp(_moveVelocity, desiredMoveVelocity, acceleration * Runner.DeltaTime);

			// Ensure consistent movement speed even on steep slope
			if (KCC.ProjectOnGround(_moveVelocity, out var projectedVector))
			{
				_moveVelocity = projectedVector;
			}

			KCC.Move(_moveVelocity, jumpImpulse);

			// Check if Player is moving
			_isMoving = _moveVelocity.magnitude > 0.1f;

			// Get current selected object and type
			GameObject currentSelection = PlayerInventory.getCurrentSelection();

			// Setting default itemType and currentItem to null 
			ItemType itemType = ItemType.None;
			Item currentItem = null;

			// If current selection is not null
			if (currentSelection != null)
			{

				currentItem = currentSelection.GetComponent<Item>();
				// Trying to get item type
				if (currentItem != null)
					itemType = currentItem.getType();
			}

			// If player press Fire1
			if (input.Shoot)
			{
				PlayerInventory.useCurrentSelection();// We use current selected Item



				// If player is not shooting and his item is a weapon we check if he wants to reaload
			}
			else if (currentItem != null && input.RealoadWeapon && itemType == ItemType.Weapon)
			{
                // ############################# teste dodo
                RPC_Reload();
                // ############################# teste dodo

				Weapon weapon = (Weapon)currentItem;  // Set current Item as a weapon
				weapon.reload(); // Relaod the weapon

			} // If Player is interacting with a pickable object
			else if (input.Interact && PlayerInventory.canPickUp())
			{
                // PickUp Item
                PlayerInventory.pickUp ();
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
            // ############################# teste dodo
            _animIDReload = Animator.StringToHash("ReloadTrigger");
            // ############################# teste dodo
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

        // Activate Constraint on the mouvement. for the head and body.
        private void ActivateConstraintMovement()
        {
            BodyWeight = 1f;
            HeadWeight = 1f;
        }

        // Deactivate Constraint on the mouvement. for the head and body.
        private void DeactivateConstraintMovement()
        {
            BodyWeight = 0f;
            HeadWeight = 0f;
        }

        // Activate Constraint when aiming. for the arm and the weapon.
        private void ActivateConstraintAim()
        {
            ArmWeight = 1f;
            WeaponWeight = 1f;
        }

        // Deactivate Constraint when not aiming. for the arm and the weapon.
        private void DeactivateConstraintAim()
        {
            ArmWeight = 0f;
            WeaponWeight = 0f;
        }

        private void Start()
		{
			gameManager = FindObjectOfType<GameManager>();
            cameraSwitcher = FindObjectOfType<CameraSwitcher>();

            if (gameManager == null)
			{
				Debug.LogError("GameManager not found in scene");
            }

                if (cameraSwitcher == null)
			{
				Debug.LogError("CameraSwitcher not found in scene");
            }
        }

        private void Update()
        {
            if (DebugIsDead)
            {
				// PlayerInventory.enabled = false;

                DebugIsDead = false;

                // Envoie les coordonnées de mort au GameManager
                gameManager.PlayerDeath(transform.position, transform.rotation);

                // Passage du joueur en mode spectateur
                //cameraSwitcher.ToggleFreecam();

                // Fait disparaître le corps du joueur
                //Destroy(gameObject);
            }
        }
    }
}
