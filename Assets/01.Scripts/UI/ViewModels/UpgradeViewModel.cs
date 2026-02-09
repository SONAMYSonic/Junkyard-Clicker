using System;
using UnityEngine;

namespace JunkyardClicker.UI.ViewModels
{
    using JunkyardClicker.Core;
    using JunkyardClicker.UI.MVVM;

    // 업그레이드 버튼을 위한 ViewModel
    public class UpgradeViewModel : ViewModelBase
    {
        private EUpgradeType _upgradeType;
        private IUpgradeService _upgradeService;
        private ICurrencyService _currencyService;

        public Observable<string> Name { get; } = new Observable<string>("");
        public Observable<string> Level { get; } = new Observable<string>("Lv.0");
        public Observable<string> Cost { get; } = new Observable<string>("$0");
        public Observable<string> Effect { get; } = new Observable<string>("");
        public Observable<bool> CanUpgrade { get; } = new Observable<bool>(false);

        public event Action OnUpgradeRequested;

        // 업그레이드 타입 설정
        public void SetUpgradeType(EUpgradeType type)
        {
            _upgradeType = type;
        }

        // 초기화 및 이벤트 구독
        public override void Initialize()
        {
            base.Initialize();

            if (!ServiceLocator.TryGet<IUpgradeService>(out _upgradeService))
            {
                Debug.LogError("[UpgradeViewModel] IUpgradeService를 찾을 수 없습니다.");
                return;
            }

            if (!ServiceLocator.TryGet<ICurrencyService>(out _currencyService))
            {
                Debug.LogError("[UpgradeViewModel] ICurrencyService를 찾을 수 없습니다.");
                return;
            }

            _currencyService.OnDataChanged += RefreshCanUpgrade;
            _upgradeService.OnUpgraded += RefreshAll;

            RefreshAll();
        }

        // 이벤트 구독 해제
        protected override void OnDispose()
        {
            if (_currencyService != null)
            {
                _currencyService.OnDataChanged -= RefreshCanUpgrade;
            }

            if (_upgradeService != null)
            {
                _upgradeService.OnUpgraded -= RefreshAll;
            }

            base.OnDispose();
        }

        // 업그레이드 요청
        public void RequestUpgrade()
        {
            if (_upgradeService == null)
            {
                Debug.LogError("[UpgradeViewModel] IUpgradeService가 없어 업그레이드할 수 없습니다.");
                return;
            }

            _upgradeService.TryUpgrade(_upgradeType);
            OnUpgradeRequested?.Invoke();
        }

        // 모든 UI 갱신
        private void RefreshAll()
        {
            if (_upgradeService == null || _currencyService == null) return;

            RefreshName();
            RefreshLevel();
            RefreshCost();
            RefreshEffect();
            RefreshCanUpgrade();
        }

        // 이름 갱신
        private void RefreshName()
        {
            Name.Value = _upgradeType switch
            {
                EUpgradeType.Tool => "도구",
                EUpgradeType.Worker => "직원",
                _ => "???"
            };
        }

        // 레벨 갱신
        private void RefreshLevel()
        {
            int currentLevel = _upgradeService.GetLevel(_upgradeType);
            bool isMaxLevel = _upgradeService.IsMaxLevel(_upgradeType);

            Level.Value = isMaxLevel ? $"Lv.{currentLevel} (MAX)" : $"Lv.{currentLevel}";
        }

        // 비용 갱신
        private void RefreshCost()
        {
            bool isMaxLevel = _upgradeService.IsMaxLevel(_upgradeType);

            if (isMaxLevel)
            {
                Cost.Value = "-";
                return;
            }

            int cost = _upgradeService.GetUpgradeCost(_upgradeType);
            Currency costCurrency = cost;
            Cost.Value = $"${costCurrency}";
        }

        // 효과 갱신
        private void RefreshEffect()
        {
            int currentLevel = _upgradeService.GetLevel(_upgradeType);

            Effect.Value = _upgradeType switch
            {
                EUpgradeType.Tool => $"클릭 데미지: {_upgradeService.GetToolDamage(currentLevel)}",
                EUpgradeType.Worker => $"초당 데미지: {_upgradeService.GetWorkerDps(currentLevel)}",
                _ => ""
            };
        }

        // 업그레이드 가능 여부 갱신
        private void RefreshCanUpgrade()
        {
            if (_upgradeService == null || _currencyService == null)
            {
                CanUpgrade.Value = false;
                return;
            }

            bool isMaxLevel = _upgradeService.IsMaxLevel(_upgradeType);

            if (isMaxLevel)
            {
                CanUpgrade.Value = false;
                return;
            }

            int cost = _upgradeService.GetUpgradeCost(_upgradeType);
            CanUpgrade.Value = _currencyService.CanAfford(ECurrencyType.Money, cost);
        }
    }
}
