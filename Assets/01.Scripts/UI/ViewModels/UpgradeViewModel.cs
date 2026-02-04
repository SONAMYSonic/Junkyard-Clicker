using System;

namespace JunkyardClicker.UI.ViewModels
{
    using JunkyardClicker.Core;
    using JunkyardClicker.UI.MVVM;

    /// <summary>
    /// 업그레이드 버튼을 위한 ViewModel
    /// DIP: IUpgradeService, ICurrencyService 인터페이스를 통해 의존성 주입
    /// </summary>
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

        public void SetUpgradeType(EUpgradeType type)
        {
            _upgradeType = type;
        }

        public override void Initialize()
        {
            base.Initialize();
            InitializeDependencies();
            SubscribeEvents();
            RefreshAll();
        }

        private void InitializeDependencies()
        {
            ServiceLocator.TryGet(out _upgradeService);
            ServiceLocator.TryGet(out _currencyService);
        }

        private void SubscribeEvents()
        {
            if (_currencyService != null)
            {
                _currencyService.OnDataChanged += RefreshCanUpgrade;
            }
            else
            {
                CurrencyManager.OnDataChanged += RefreshCanUpgrade;
            }

            if (_upgradeService != null)
            {
                _upgradeService.OnUpgraded += RefreshAll;
            }
            else
            {
                UpgradeManager.OnUpgraded += RefreshAll;
            }
        }

        protected override void OnDispose()
        {
            UnsubscribeEvents();
            base.OnDispose();
        }

        private void UnsubscribeEvents()
        {
            if (_currencyService != null)
            {
                _currencyService.OnDataChanged -= RefreshCanUpgrade;
            }
            else
            {
                CurrencyManager.OnDataChanged -= RefreshCanUpgrade;
            }

            if (_upgradeService != null)
            {
                _upgradeService.OnUpgraded -= RefreshAll;
            }
            else
            {
                UpgradeManager.OnUpgraded -= RefreshAll;
            }
        }

        public void RequestUpgrade()
        {
            if (_upgradeService != null)
            {
                _upgradeService.TryUpgrade(_upgradeType);
            }
            else if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.TryUpgrade(_upgradeType);
            }

            OnUpgradeRequested?.Invoke();
        }

        private void RefreshAll()
        {
            RefreshName();
            RefreshLevel();
            RefreshCost();
            RefreshEffect();
            RefreshCanUpgrade();
        }

        private void RefreshName()
        {
            Name.Value = _upgradeType switch
            {
                EUpgradeType.Tool => "도구",
                EUpgradeType.Worker => "직원",
                _ => "???"
            };
        }

        private void RefreshLevel()
        {
            int currentLevel = GetLevel();
            bool isMaxLevel = IsMaxLevel();

            Level.Value = isMaxLevel ? $"Lv.{currentLevel} (MAX)" : $"Lv.{currentLevel}";
        }

        private void RefreshCost()
        {
            bool isMaxLevel = IsMaxLevel();

            if (isMaxLevel)
            {
                Cost.Value = "-";
                return;
            }

            int cost = GetUpgradeCost();
            Currency costCurrency = cost;
            Cost.Value = $"${costCurrency}";
        }

        private void RefreshEffect()
        {
            int currentLevel = GetLevel();

            Effect.Value = _upgradeType switch
            {
                EUpgradeType.Tool => $"클릭 데미지: {GetToolDamage(currentLevel)}",
                EUpgradeType.Worker => $"초당 데미지: {GetWorkerDps(currentLevel)}",
                _ => ""
            };
        }

        private void RefreshCanUpgrade()
        {
            bool isMaxLevel = IsMaxLevel();

            if (isMaxLevel)
            {
                CanUpgrade.Value = false;
                return;
            }

            int cost = GetUpgradeCost();
            CanUpgrade.Value = CanAfford(cost);
        }

        #region Helper Methods (DIP 적용)

        private int GetLevel()
        {
            if (_upgradeService != null)
            {
                return _upgradeService.GetLevel(_upgradeType);
            }

            return UpgradeManager.Instance?.GetLevel(_upgradeType) ?? 0;
        }

        private bool IsMaxLevel()
        {
            if (_upgradeService != null)
            {
                return _upgradeService.IsMaxLevel(_upgradeType);
            }

            return UpgradeManager.Instance?.IsMaxLevel(_upgradeType) ?? true;
        }

        private int GetUpgradeCost()
        {
            if (_upgradeService != null)
            {
                return _upgradeService.GetUpgradeCost(_upgradeType);
            }

            return UpgradeManager.Instance?.GetUpgradeCost(_upgradeType) ?? int.MaxValue;
        }

        private int GetToolDamage(int level)
        {
            if (_upgradeService != null)
            {
                return _upgradeService.GetToolDamage(level);
            }

            return UpgradeManager.Instance?.GetToolDamage(level) ?? 1;
        }

        private int GetWorkerDps(int level)
        {
            if (_upgradeService != null)
            {
                return _upgradeService.GetWorkerDps(level);
            }

            return UpgradeManager.Instance?.GetWorkerDps(level) ?? 0;
        }

        private bool CanAfford(int cost)
        {
            if (_currencyService != null)
            {
                return _currencyService.CanAfford(ECurrencyType.Money, cost);
            }

            return CurrencyManager.Instance?.CanAfford(ECurrencyType.Money, cost) ?? false;
        }

        #endregion
    }
}
