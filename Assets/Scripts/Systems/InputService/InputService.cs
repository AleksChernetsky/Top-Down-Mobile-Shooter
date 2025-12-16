using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace TowerDefence.Systems
{
    public class InputService : IInputService, IDisposable
    {
        private Input _input;
        private bool _isEnabled;

        public Input InputActions => _input;
        public bool IsEnabled => _isEnabled;

        public event Action<Vector2> OnTap;
        public event Action<Vector2> OnHold;
        public event Action<Vector2> OnTouchMoved;

        public void Init()
        {
            _input = new Input();

            _input.Game.Tap.performed += HandleTap;
            _input.Game.Hold.performed += HandleHold;
            _input.Game.TouchDelta.performed += HandleTouchMoved;

            Enable();
        }

        public void Enable()
        {
            if (_isEnabled)
            {
                return;
            }

            _input?.Game.Enable();
            _isEnabled = true;
        }

        public void Disable()
        {
            if (!_isEnabled)
            {
                return;
            }

            _input?.Game.Disable();
            _isEnabled = false;
        }

        public void Dispose()
        {
            Disable();

            if (_input == null)
            {
                return;
            }

            _input.Game.Tap.performed -= HandleTap;
            _input.Game.Hold.performed -= HandleHold;
            _input.Game.TouchDelta.performed -= HandleTouchMoved;
            _input.Dispose();
        }

        public bool IsPointerOverUI()
        {
            if (EventSystem.current == null || _input == null)
            {
                return false;
            }

            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = GetTouchPosition()
            };

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            return results.Count > 0;
        }

        public Vector2 GetTouchPosition()
        {
            return _input?.Game.TouchPosition.ReadValue<Vector2>() ?? Vector2.zero;
        }

        private void HandleTap(InputAction.CallbackContext context)
        {
            if (IsPointerOverUI())
            {
                return;
            }

            OnTap?.Invoke(GetTouchPosition());
        }

        private void HandleHold(InputAction.CallbackContext context)
        {
            if (IsPointerOverUI())
            {
                return;
            }

            OnHold?.Invoke(GetTouchPosition());
        }

        private void HandleTouchMoved(InputAction.CallbackContext context)
        {
            if (IsPointerOverUI())
            {
                return;
            }

            var delta = context.ReadValue<Vector2>();
            if (delta.magnitude > 0.1f)
            {
                OnTouchMoved?.Invoke(GetTouchPosition());
            }
        }
    }
}
