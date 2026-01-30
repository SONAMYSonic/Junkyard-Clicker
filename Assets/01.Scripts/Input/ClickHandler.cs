using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace JunkyardClicker.Input
{
    using Car;
    using Core;

    public class ClickHandler : MonoBehaviour
    {
        [SerializeField]
        private Camera _mainCamera;

        private Car _currentCar;

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }
        }

        private void OnEnable()
        {
            GameEvents.OnCarSpawned += HandleCarSpawned;
        }

        private void OnDisable()
        {
            GameEvents.OnCarSpawned -= HandleCarSpawned;
        }

        private void Start()
        {
            // 이미 스폰된 차량이 있으면 연결
            var existingCar = FindAnyObjectByType<Car>();
            if (existingCar != null)
            {
                _currentCar = existingCar;
            }
        }

        private void HandleCarSpawned(Car car)
        {
            // 이벤트에서 Car 참조를 직접 받아서 타이밍 문제 해결
            _currentCar = car;
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

            if (_currentCar == null || _currentCar.IsDestroyed)
            {
                return;
            }

            Vector2 worldPosition = GetWorldPosition();
            int damage = CalculateDamage();

            CarPart clickedPart = _currentCar.GetPartAtPosition(worldPosition);

            if (clickedPart != null)
            {
                _currentCar.TakeDamageOnPart(clickedPart, damage);
            }
            else
            {
                _currentCar.TakeDamage(damage);
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

        private int CalculateDamage()
        {
            if (NewUpgradeManager.Instance == null)
            {
                return 1;
            }

            return NewUpgradeManager.Instance.ToolDamage;
        }

        public void SetCar(Car car)
        {
            _currentCar = car;
        }
    }
}
