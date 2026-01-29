using UnityEngine;
using UnityEngine.EventSystems;

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
            GameEvents.OnCarSpawned += RefreshCarReference;
        }

        private void OnDisable()
        {
            GameEvents.OnCarSpawned -= RefreshCarReference;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                HandleClick();
            }
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
            Vector3 mousePosition = UnityEngine.Input.mousePosition;
            Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(mousePosition);
            return new Vector2(worldPosition.x, worldPosition.y);
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

        private void RefreshCarReference()
        {
            _currentCar = FindAnyObjectByType<Car>();
        }
    }
}
