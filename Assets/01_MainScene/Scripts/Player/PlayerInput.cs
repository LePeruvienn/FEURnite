using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

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
        public bool Play;
    }

	/// <summary>
	/// PlayerInput handles accumulating player input from Unity.
	/// </summary>
	public sealed class PlayerInput : MonoBehaviour
	{
        public GameplayInput CurrentInput => _input;
        private GameplayInput _input;
        private InputBinding BindingInput;

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
			_input.Play = false;
        }

        private void Start()
        {
            BindingInput = FindObjectOfType<InputBinding>();
        }

        private void Update()
		{
            _input.DropItem |= UnityEngine.Input.GetKeyDown((KeyCode)BindingInput.getInputDico()["DropItem"]);
            _input.Sprint |= UnityEngine.Input.GetKey((KeyCode)BindingInput.getInputDico()["Sprint"]);
            _input.Emote |= UnityEngine.Input.GetKeyDown((KeyCode)BindingInput.getInputDico()["Emote"]);
            _input.Interact |= UnityEngine.Input.GetKeyDown((KeyCode)BindingInput.getInputDico()["Interact"]);
            _input.RealoadWeapon |= UnityEngine.Input.GetKeyDown((KeyCode)BindingInput.getInputDico()["RealoadWeapon"]);
            _input.ToggleInventory |= UnityEngine.Input.GetKeyDown((KeyCode)BindingInput.getInputDico()["ToggleInventory"]);

            // Accumulate input only if the cursor is locked.
            if (Cursor.lockState != CursorLockMode.Locked)
                return;

            // Accumulate input from Keyboard/Mouse. Input accumulation is mandatory (at least for look rotation here) as Update can be
            // called multiple times before next FixedUpdateNetwork is called - common if rendering speed is faster than Fusion simulation.

            var lookRotationDelta = new Vector2(-UnityEngine.Input.GetAxisRaw("Mouse Y"), UnityEngine.Input.GetAxisRaw("Mouse X"));
            _input.LookRotation = ClampLookRotation(_input.LookRotation + lookRotationDelta);

            var moveDirection = new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Vertical"));
            _input.MoveDirection = moveDirection.normalized;

            _input.Jump |= UnityEngine.Input.GetButtonDown("Jump");
            _input.Aiming |= UnityEngine.Input.GetButton("Fire2");
            _input.Shoot |= UnityEngine.Input.GetButton("Fire1");
            _input.Scroll = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
            _input.FirstInvSlot |= UnityEngine.Input.GetKeyDown(KeyCode.Alpha1);
            _input.SecondInvSlot |= UnityEngine.Input.GetKeyDown(KeyCode.Alpha2);
            _input.ThirdInvSlot |= UnityEngine.Input.GetKeyDown(KeyCode.Alpha3);
            _input.FourthInvSlot |= UnityEngine.Input.GetKeyDown(KeyCode.Alpha4);
            _input.Coin |= UnityEngine.Input.GetKeyDown("c");
            _input.Play |= UnityEngine.Input.GetKeyDown("p");
        }

		private Vector2 ClampLookRotation(Vector2 lookRotation)
		{
			lookRotation.x = Mathf.Clamp(lookRotation.x, -80f, 70f);
			return lookRotation;
		}
	}
}
