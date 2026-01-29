using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JunkyardClicker.UI
{
    using Car;
    using Core;

    public class HpBarUI : MonoBehaviour
    {
        [SerializeField]
        private Image _fillImage;

        [SerializeField]
        private TextMeshProUGUI _hpText;

        [SerializeField]
        private TextMeshProUGUI _carNameText;

        [SerializeField]
        private Image _gradeIndicator;

        private Car _currentCar;

        private void OnEnable()
        {
            GameEvents.OnCarSpawned += HandleCarSpawned;
            GameEvents.OnDamageDealt += HandleDamageDealt;
        }

        private void OnDisable()
        {
            GameEvents.OnCarSpawned -= HandleCarSpawned;
            GameEvents.OnDamageDealt -= HandleDamageDealt;
        }

        private void HandleCarSpawned()
        {
            _currentCar = FindAnyObjectByType<Car>();
            UpdateCarInfo();
            UpdateHpBar();
        }

        private void HandleDamageDealt(int damage)
        {
            UpdateHpBar();
        }

        private void UpdateCarInfo()
        {
            if (_currentCar == null || _currentCar.Data == null)
            {
                return;
            }

            CarData data = _currentCar.Data;

            if (_carNameText != null)
            {
                _carNameText.text = data.CarName;
            }

            if (_gradeIndicator != null)
            {
                _gradeIndicator.color = data.GetGradeColor();
            }
        }

        private void UpdateHpBar()
        {
            if (_currentCar == null)
            {
                return;
            }

            float hpRatio = _currentCar.HpRatio;

            if (_fillImage != null)
            {
                _fillImage.fillAmount = hpRatio;
                _fillImage.color = GetHpColor(hpRatio);
            }

            if (_hpText != null)
            {
                _hpText.text = $"{_currentCar.CurrentHp} / {_currentCar.MaxHp}";
            }
        }

        private Color GetHpColor(float ratio)
        {
            if (ratio > 0.5f)
            {
                return Color.Lerp(Color.yellow, Color.green, (ratio - 0.5f) * 2f);
            }

            return Color.Lerp(Color.red, Color.yellow, ratio * 2f);
        }
    }
}
