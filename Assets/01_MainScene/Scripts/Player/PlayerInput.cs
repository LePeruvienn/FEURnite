using UnityEngine;

namespace Starter.ThirdPersonCharacter
{
	/// <summary>
	/// Structure holding player input.
	/// </summary>
	public struct GameplayInput
	{
		public Vector2 LookRotation;
		public Vector2 MoveDirection;
		// Movement keybindings
		public bool Jump;
		public bool Sprint;
		public bool Aiming;
		public bool Shoot;
		public float Scroll;
		// Item keybindings
		public bool DropItem;
		public bool Interact;
		// Weapon keybindings
		public bool RealoadWeapon;
		// Inventory keybindings
		public bool ToggleInventory;
        public bool FirstInvSlot;
        public bool SecondInvSlot;
        public bool ThirdInvSlot;
        public bool FourthInvSlot;
        public bool Emote; 
        public bool Coin; 
    }

	/// <summary>
	/// PlayerInput handles accumulating player input from Unity.
	/// </summary>
	public sealed class PlayerInput : MonoBehaviour
	{
		public GameplayInput CurrentInput => _input;
		private GameplayInput _input;

		public void ResetInput()
		{
			// Reset input after it was used to detect changes correctly again
			_input.MoveDirection = default;
			_input.Jump = false;
			_input.Sprint = false;
			_input.Aiming = false;
            _input.Shoot = false;
            _input.Scroll = 0f;
			_input.DropItem = false;
			_input.Interact = false;
			_input.RealoadWeapon = false;
			_input.ToggleInventory = false;
            _input.FirstInvSlot = false;
            _input.SecondInvSlot = false;
            _input.ThirdInvSlot = false;
            _input.FourthInvSlot = false;
            _input.Emote = false;
            _input.Coin = false;
        }

		private void Update()
		{

			_input.ToggleInventory |= Input.GetKeyDown("e");

			// Accumulate input only if the cursor is locked.
			if (Cursor.lockState != CursorLockMode.Locked)
				return;

			// Accumulate input from Keyboard/Mouse. Input accumulation is mandatory (at least for look rotation here) as Update can be
			// called multiple times before next FixedUpdateNetwork is called - common if rendering speed is faster than Fusion simulation.

			var lookRotationDelta = new Vector2(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));
			_input.LookRotation = ClampLookRotation(_input.LookRotation + lookRotationDelta);

			var moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			_input.MoveDirection = moveDirection.normalized;

			_input.Jump |= Input.GetButtonDown("Jump");
			_input.Sprint |= Input.GetButton("Sprint");
			_input.Aiming |= Input.GetButton("Fire2");
            _input.Shoot |= Input.GetButton("Fire1");
            _input.Scroll = Input.GetAxis("Mouse ScrollWheel");
			_input.DropItem |= Input.GetKeyDown("g");
			_input.Interact |= Input.GetKeyDown("f");
			_input.RealoadWeapon |= Input.GetKeyDown("r"); 
			_input.FirstInvSlot |= Input.GetKeyDown(KeyCode.Alpha1);
            _input.SecondInvSlot |= Input.GetKeyDown(KeyCode.Alpha2);
            _input.ThirdInvSlot |= Input.GetKeyDown(KeyCode.Alpha3);
            _input.FourthInvSlot |= Input.GetKeyDown(KeyCode.Alpha4);
            _input.Emote |= Input.GetKeyDown("i");
            _input.Coin |= Input.GetKeyDown("c");
        }

		private Vector2 ClampLookRotation(Vector2 lookRotation)
		{
			lookRotation.x = Mathf.Clamp(lookRotation.x, -80f, 70f);
			return lookRotation;
		}
	}
}
