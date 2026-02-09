using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace JunkyardClicker.Input
{
    using JunkyardClicker.Core;
    using JunkyardClicker.Resource;

    // 입력 핸들러 - 클릭/터치 입력 감지 및 이벤트 발행
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

        // 카메라 설정 및 서비스 등록
        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }

            ServiceLocator.Register<IInputHandler>(this);
        }

        // 의존성 주입
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

        // 서비스 등록 해제
        private void OnDestroy()
        {
            ServiceLocator.Unregister<IInputHandler>();
        }

        // 매 프레임 입력 체크
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

        // 이번 프레임에 클릭이 있었는지 확인
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

        // 클릭 처리
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

        // UI 위에 포인터가 있는지 확인
        private bool IsPointerOverUI()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }

        // 스크린 좌표를 월드 좌표로 변환
        private Vector2 GetWorldPosition()
        {
            Vector2 screenPosition = GetPointerPosition();
            Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(screenPosition);
            return new Vector2(worldPosition.x, worldPosition.y);
        }

        // 현재 포인터 위치 반환
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
