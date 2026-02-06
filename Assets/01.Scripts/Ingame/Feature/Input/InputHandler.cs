using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace JunkyardClicker.Input
{
    using JunkyardClicker.Core;
    using JunkyardClicker.Resource;

    /// <summary>
    /// 입력 핸들러
    /// IInputHandler 인터페이스 구현
    /// 클릭/터치 입력을 감지하고 이벤트 발행
    /// </summary>
    public class InputHandler : MonoBehaviour, IInputHandler
    {
        [SerializeField]
        private Camera _mainCamera;

        private IDamageManager _damageManager;
        private bool _isEnabled = true;

        public event Action<Vector2> OnClicked;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }

            ServiceLocator.Register<IInputHandler>(this);
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

        private void OnDestroy()
        {
            ServiceLocator.Unregister<IInputHandler>();
        }

        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

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

            Vector2 worldPosition = GetWorldPosition();

            // 이벤트 발행 (다른 시스템이 구독 가능)
            OnClicked?.Invoke(worldPosition);

            // 데미지 적용
            if (_damageManager != null)
            {
                _damageManager.ApplyClickDamage(worldPosition);
            }
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
