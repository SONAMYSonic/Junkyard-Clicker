using UnityEngine;

namespace JunkyardClicker.UI.ViewModels
{
    using JunkyardClicker.Core;
    using JunkyardClicker.UI.MVVM;
    using CarEntity = JunkyardClicker.Car.CarEntity;

    /// <summary>
    /// HP 바 UI를 위한 ViewModel
    /// </summary>
    public class HpBarViewModel : ViewModelBase
    {
        public Observable<float> HpRatio { get; } = new Observable<float>(1f);
        public Observable<string> HpText { get; } = new Observable<string>("0 / 0");
        public Observable<string> CarName { get; } = new Observable<string>("");
        public Observable<Color> GradeColor { get; } = new Observable<Color>(Color.white);
        public Observable<Color> HpBarColor { get; } = new Observable<Color>(Color.green);

        private CarEntity _currentCar;

        public override void Initialize()
        {
            base.Initialize();
            GameEvents.OnCarSpawned += HandleCarSpawned;
            GameEvents.OnDamageDealt += HandleDamageDealt;

            var existingCar = Object.FindAnyObjectByType<CarEntity>();
            if (existingCar != null)
            {
                SetCar(existingCar);
            }
        }

        protected override void OnDispose()
        {
            GameEvents.OnCarSpawned -= HandleCarSpawned;
            GameEvents.OnDamageDealt -= HandleDamageDealt;
            base.OnDispose();
        }

        private void HandleCarSpawned(CarEntity car)
        {
            SetCar(car);
        }

        private void HandleDamageDealt(int damage)
        {
            RefreshHpBar();
        }

        private void SetCar(CarEntity car)
        {
            _currentCar = car;
            RefreshCarInfo();
            RefreshHpBar();
        }

        private void RefreshCarInfo()
        {
            if (_currentCar == null || _currentCar.Data == null)
            {
                return;
            }

            CarData data = _currentCar.Data;
            CarName.Value = data.CarName;
            GradeColor.Value = data.GetGradeColor();
        }

        private void RefreshHpBar()
        {
            if (_currentCar == null)
            {
                return;
            }

            float hpRatio = _currentCar.HpRatio;
            HpRatio.Value = hpRatio;
            HpText.Value = $"{_currentCar.CurrentHp} / {_currentCar.MaxHp}";
            HpBarColor.Value = GetHpColor(hpRatio);
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
