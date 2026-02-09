using UnityEngine;

namespace JunkyardClicker.UI.ViewModels
{
    using JunkyardClicker.Core;
    using JunkyardClicker.UI.MVVM;
    using CarEntity = JunkyardClicker.Car.CarEntity;

    // HP 바 UI를 위한 ViewModel
    public class HpBarViewModel : ViewModelBase
    {
        public Observable<float> HpRatio { get; } = new Observable<float>(1f);
        public Observable<string> HpText { get; } = new Observable<string>("0 / 0");
        public Observable<string> CarName { get; } = new Observable<string>("");
        public Observable<Color> GradeColor { get; } = new Observable<Color>(Color.white);
        public Observable<Color> HpBarColor { get; } = new Observable<Color>(Color.green);

        private CarEntity _currentCar;

        // 초기화 및 이벤트 구독
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

        // 이벤트 구독 해제
        protected override void OnDispose()
        {
            GameEvents.OnCarSpawned -= HandleCarSpawned;
            GameEvents.OnDamageDealt -= HandleDamageDealt;
            base.OnDispose();
        }

        // 차량 스폰 시 처리
        private void HandleCarSpawned(CarEntity car)
        {
            SetCar(car);
        }

        // 데미지 발생 시 HP 바 갱신
        private void HandleDamageDealt(int damage)
        {
            RefreshHpBar();
        }

        // 현재 차량 설정
        private void SetCar(CarEntity car)
        {
            _currentCar = car;
            RefreshCarInfo();
            RefreshHpBar();
        }

        // 차량 정보 갱신
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

        // HP 바 UI 갱신
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

        // HP 비율에 따른 색상 반환
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
