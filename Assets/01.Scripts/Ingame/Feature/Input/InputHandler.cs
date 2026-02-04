using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace JunkyardClicker.Ingame.Input
{
    using JunkyardClicker.Core;
    using JunkyardClicker.Ingame.Damage;

    /// <summary>
    /// 입력 처리 핸들러
    /// 클릭/터치 입력을 감지하고 DamageManager에 전달
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        [SerializeField]
        private Camera _mainCamera;

        private IDamageManager _damageManager;

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }
        }

        private void Start()
        {
            if (ServiceLocator.TryGet<IDamageManager>(out var damageManager))
            {
                _damageManager = damageManager;
            }
            else
            {
                _damageManager = DamageManager.Instance;
            }
        }

        private void Update()
        {
            if (WasClickedThisFrame())
            {
                HandleClick();
            }
        }

        private bool WasClickedThisFrame()
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                return true;
            }

            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                return true;
            }

            return false;
        }

        private void HandleClick()
        {
            if (IsPointerOverUI())
            {
                return;
            }

            if (_damageManager == null)
            {
                return;
            }

            Vector2 worldPosition = GetWorldPosition();
            _damageManager.ApplyClickDamage(worldPosition);
        }

        private bool IsPointerOverUI()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }

        private Vector2 GetWorldPosition()
        {
            Vector2 screenPosition = GetPointerPosition();
            Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(screenPosition);
            return new Vector2(worldPosition.x, worldPosition.y);
        }

        private Vector2 GetPointerPosition()
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                return Touchscreen.current.primaryTouch.position.ReadValue();
            }

            if (Mouse.current != null)
            {
                return Mouse.current.position.ReadValue();
            }

            return Vector2.zero;
        }
    }
}
