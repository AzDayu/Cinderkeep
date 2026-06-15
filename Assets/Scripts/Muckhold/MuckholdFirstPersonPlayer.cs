using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace OODong.Muckhold
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(MuckholdInventory))]
    public sealed class MuckholdFirstPersonPlayer : MonoBehaviour
    {
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private MuckholdInventory _inventory;
        [SerializeField] private MuckholdHudView _hudView;
        [SerializeField] private MuckholdPickaxeView _pickaxeView;
        [SerializeField] private MuckholdAutoShooter _manualShooter;
        [SerializeField] private MuckholdPlaceableItemFactory _placeableItemFactory;
        [SerializeField] private LayerMask _interactionMask = ~0;
        [SerializeField] private float _moveSpeed = 6f;
        [SerializeField] private float _lookSensitivity = 0.12f;
        [SerializeField] private float _gravity = -18f;
        [SerializeField] private float _interactionDistance = 4f;
        [SerializeField] private int _pickaxeDamage = 1;
        [SerializeField] private bool _lockCursorOnStart = true;

        private Vector3 _verticalVelocity;
        private IMuckholdInteractable _currentInteractable;
        private int _selectedQuickSlotIndex;
        private float _yaw;
        private float _pitch;
        private bool _isCursorLocked;

        public MuckholdInventory Inventory => _inventory;
        public int SelectedQuickSlotIndex => _selectedQuickSlotIndex;

        private void Awake()
        {
            ResolveReferences();
            _yaw = transform.eulerAngles.y;
            _pitch = _playerCamera != null ? _playerCamera.transform.localEulerAngles.x : 0f;
        }

        private void Start()
        {
            SetCursorLock(_lockCursorOnStart);
            _hudView?.SetInventory(_inventory);
            _hudView?.SetInventoryOpen(false);
            _hudView?.SetSelectedQuickSlot(_selectedQuickSlotIndex);
            _hudView?.SetStatus("Tab/I: Inventory, E: Interact, Left Click: Use quick slot, 1-7: Select");
        }

        private void Update()
        {
            ToggleCursorLockFromInput();
            LookFromInput();
            MoveFromInput();
            SelectQuickSlotFromInput();
            FindInteractable();

            if (WasInteractPressed() && _currentInteractable != null)
            {
                _currentInteractable.Interact(this);
            }

            if (_isCursorLocked && WasPrimaryUsePressed())
            {
                UseSelectedQuickSlot(false);
            }

            if (_isCursorLocked && WasSecondaryUsePressed())
            {
                UseSelectedQuickSlot(true);
            }
        }

        public bool HasItem(MuckholdItemId itemId)
        {
            return _inventory != null && _inventory.HasItem(itemId);
        }

        public MuckholdItemId GetSelectedQuickSlotItem()
        {
            return _inventory != null ? _inventory.GetQuickSlotItem(_selectedQuickSlotIndex) : MuckholdItemId.None;
        }

        public void AddItem(MuckholdItemId itemId, int count)
        {
            _inventory?.AddItem(itemId, count);
        }

        public void PlayPickaxeSwing()
        {
            _pickaxeView?.PlaySwing();
        }

        public void ShowStatus(string message)
        {
            _hudView?.SetStatus(message);
        }

        public void ShowMiningProgress(float progress)
        {
            _hudView?.SetMiningProgress(progress);
        }

        public void SetReferences(
            CharacterController characterController,
            Camera playerCamera,
            MuckholdInventory inventory,
            MuckholdHudView hudView,
            MuckholdPickaxeView pickaxeView)
        {
            _characterController = characterController;
            _playerCamera = playerCamera;
            _inventory = inventory;
            _hudView = hudView;
            _pickaxeView = pickaxeView;
            _manualShooter = GetComponent<MuckholdAutoShooter>();
            _placeableItemFactory = GetComponent<MuckholdPlaceableItemFactory>();
        }

        private void ResolveReferences()
        {
            if (_characterController == null)
            {
                _characterController = GetComponent<CharacterController>();
            }

            if (_inventory == null)
            {
                _inventory = GetComponent<MuckholdInventory>();
            }

            if (_playerCamera == null)
            {
                _playerCamera = GetComponentInChildren<Camera>();
            }

            if (_manualShooter == null)
            {
                _manualShooter = GetComponent<MuckholdAutoShooter>();
            }

            if (_placeableItemFactory == null)
            {
                _placeableItemFactory = GetComponent<MuckholdPlaceableItemFactory>();
            }
        }

        private void MoveFromInput()
        {
            Vector2 movementInput = ReadMovementInput();
            Vector3 movement = (transform.right * movementInput.x) + (transform.forward * movementInput.y);
            movement.y = 0f;

            if (_characterController.isGrounded && _verticalVelocity.y < 0f)
            {
                _verticalVelocity.y = -2f;
            }

            _verticalVelocity.y += _gravity * Time.deltaTime;
            Vector3 velocity = (movement.normalized * _moveSpeed) + _verticalVelocity;
            _characterController.Move(velocity * Time.deltaTime);
        }

        private void LookFromInput()
        {
            if (!_isCursorLocked || _playerCamera == null)
            {
                return;
            }

            Vector2 lookDelta = ReadLookDelta();
            if (lookDelta.sqrMagnitude <= 0f)
            {
                return;
            }

            _yaw += lookDelta.x * _lookSensitivity;
            _pitch = Mathf.Clamp(_pitch - lookDelta.y * _lookSensitivity, -70f, 72f);
            transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
            _playerCamera.transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private void FindInteractable()
        {
            _currentInteractable = null;

            if (_playerCamera == null)
            {
                _hudView?.SetPrompt(string.Empty);
                return;
            }

            Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, _interactionDistance, _interactionMask, QueryTriggerInteraction.Collide))
            {
                _hudView?.SetPrompt(string.Empty);
                return;
            }

            MonoBehaviour[] behaviours = hit.collider.GetComponentsInParent<MonoBehaviour>();
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] is IMuckholdInteractable interactable && interactable.CanInteract(this))
                {
                    _currentInteractable = interactable;
                    _hudView?.SetPrompt(interactable.GetPrompt());
                    return;
                }
            }

            _hudView?.SetPrompt(string.Empty);
        }

        private void SelectQuickSlotFromInput()
        {
            for (int i = 0; i < MuckholdInventoryModel.QuickSlotCount; i++)
            {
                if (WasQuickSlotPressed(i))
                {
                    _selectedQuickSlotIndex = i;
                    _hudView?.SetSelectedQuickSlot(_selectedQuickSlotIndex);
                    MuckholdItemId itemId = GetSelectedQuickSlotItem();
                    ShowStatus($"Selected {i + 1}: {MuckholdItemCatalog.GetDisplayName(itemId)}");
                }
            }
        }

        private void ToggleCursorLockFromInput()
        {
            if (WasInventoryTogglePressed())
            {
                bool nextOpen = _hudView == null || !_hudView.IsInventoryOpen;
                _hudView?.SetInventoryOpen(nextOpen);
                SetCursorLock(!nextOpen);
            }

            if (WasCancelPressed())
            {
                _hudView?.SetInventoryOpen(false);
                SetCursorLock(false);
            }
        }

        private void UseSelectedQuickSlot(bool isSecondaryUse)
        {
            MuckholdItemId itemId = GetSelectedQuickSlotItem();
            switch (itemId)
            {
                case MuckholdItemId.Pickaxe:
                    UsePickaxe();
                    break;
                case MuckholdItemId.Arrow:
                    UseArrow();
                    break;
                case MuckholdItemId.Stone:
                case MuckholdItemId.Ore:
                    _placeableItemFactory?.TryPlaceItem(this, _playerCamera, itemId);
                    break;
                case MuckholdItemId.Apple:
                    UseApple();
                    break;
                default:
                    ShowStatus(isSecondaryUse ? "Right click: no quick slot item" : "Left click: no quick slot item");
                    break;
            }
        }

        private void UsePickaxe()
        {
            PlayPickaxeSwing();

            if (_currentInteractable is MuckholdMineableNode)
            {
                _currentInteractable.Interact(this);
                return;
            }

            if (TryHitEnemy())
            {
                ShowStatus("Pickaxe hit enemy");
                return;
            }

            ShowStatus("Pickaxe swing");
        }

        private void UseArrow()
        {
            if (_inventory == null || !_inventory.HasItem(MuckholdItemId.Arrow))
            {
                ShowStatus("Arrow is empty");
                return;
            }

            if (_manualShooter == null || !_manualShooter.TryFireNearestEnemy())
            {
                ShowStatus("No enemy in arrow range");
                return;
            }

            _inventory.TryRemoveItem(MuckholdItemId.Arrow, 1);
            ShowStatus("Arrow fired");
        }

        private bool TryHitEnemy()
        {
            if (_playerCamera == null)
            {
                return false;
            }

            Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, _interactionDistance, _interactionMask, QueryTriggerInteraction.Collide))
            {
                return false;
            }

            MuckholdEnemy enemy = hit.collider.GetComponentInParent<MuckholdEnemy>();
            if (enemy == null)
            {
                return false;
            }

            enemy.TakeDamage(_pickaxeDamage);
            return true;
        }

        private void UseApple()
        {
            if (_inventory == null || !_inventory.TryRemoveItem(MuckholdItemId.Apple, 1))
            {
                ShowStatus("Apple is empty");
                return;
            }

            ShowStatus("Apple used");
        }

        private void SetCursorLock(bool isLocked)
        {
            _isCursorLocked = isLocked;
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isLocked;
        }

        private Vector2 ReadMovementInput()
        {
#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return Vector2.zero;
            }

            float horizontal = 0f;
            float vertical = 0f;

            if (keyboard.aKey.isPressed)
            {
                horizontal -= 1f;
            }

            if (keyboard.dKey.isPressed)
            {
                horizontal += 1f;
            }

            if (keyboard.sKey.isPressed)
            {
                vertical -= 1f;
            }

            if (keyboard.wKey.isPressed)
            {
                vertical += 1f;
            }

            return new Vector2(horizontal, vertical);
#else
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
#endif
        }

        private Vector2 ReadLookDelta()
        {
#if ENABLE_INPUT_SYSTEM
            Mouse mouse = Mouse.current;
            return mouse != null ? mouse.delta.ReadValue() : Vector2.zero;
#else
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 12f;
#endif
        }

        private bool WasInteractPressed()
        {
#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            return keyboard != null && keyboard.eKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(KeyCode.E);
#endif
        }

        private bool WasPrimaryUsePressed()
        {
#if ENABLE_INPUT_SYSTEM
            Mouse mouse = Mouse.current;
            return mouse != null && mouse.leftButton.wasPressedThisFrame;
#else
            return Input.GetMouseButtonDown(0);
#endif
        }

        private bool WasSecondaryUsePressed()
        {
#if ENABLE_INPUT_SYSTEM
            Mouse mouse = Mouse.current;
            return mouse != null && mouse.rightButton.wasPressedThisFrame;
#else
            return Input.GetMouseButtonDown(1);
#endif
        }

        private bool WasInventoryTogglePressed()
        {
#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            return keyboard != null && (keyboard.tabKey.wasPressedThisFrame || keyboard.iKey.wasPressedThisFrame);
#else
            return Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I);
#endif
        }

        private bool WasCancelPressed()
        {
#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            return keyboard != null && keyboard.escapeKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(KeyCode.Escape);
#endif
        }

        private bool WasQuickSlotPressed(int zeroBasedIndex)
        {
#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return false;
            }

            Key key = GetQuickSlotKey(zeroBasedIndex);
            return key != Key.None && keyboard[key].wasPressedThisFrame;
#else
            KeyCode keyCode = GetQuickSlotKeyCode(zeroBasedIndex);
            return keyCode != KeyCode.None && Input.GetKeyDown(keyCode);
#endif
        }

#if ENABLE_INPUT_SYSTEM
        private Key GetQuickSlotKey(int zeroBasedIndex)
        {
            switch (zeroBasedIndex)
            {
                case 0:
                    return Key.Digit1;
                case 1:
                    return Key.Digit2;
                case 2:
                    return Key.Digit3;
                case 3:
                    return Key.Digit4;
                case 4:
                    return Key.Digit5;
                case 5:
                    return Key.Digit6;
                case 6:
                    return Key.Digit7;
                default:
                    return Key.None;
            }
        }
#else
        private KeyCode GetQuickSlotKeyCode(int zeroBasedIndex)
        {
            switch (zeroBasedIndex)
            {
                case 0:
                    return KeyCode.Alpha1;
                case 1:
                    return KeyCode.Alpha2;
                case 2:
                    return KeyCode.Alpha3;
                case 3:
                    return KeyCode.Alpha4;
                case 4:
                    return KeyCode.Alpha5;
                case 5:
                    return KeyCode.Alpha6;
                case 6:
                    return KeyCode.Alpha7;
                default:
                    return KeyCode.None;
            }
        }
#endif
    }
}
