using System;

namespace JunkyardClicker.UI.ViewModels
{
    using JunkyardClicker.UI.MVVM;

    /// <summary>
    /// 업그레이드 버튼을 위한 ViewModel
    /// </summary>
    public class UpgradeViewModel : ViewModelBase
    {
        private EUpgradeType _upgradeType;

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
            CurrencyManager.OnDataChanged += RefreshCanUpgrade;
            NewUpgradeManager.OnUpgraded += RefreshAll;
            RefreshAll();
        }

        protected override void OnDispose()
        {
            CurrencyManager.OnDataChanged -= RefreshCanUpgrade;
            NewUpgradeManager.OnUpgraded -= RefreshAll;
            base.OnDispose();
        }

        public void RequestUpgrade()
        {
            if (NewUpgradeManager.Instance != null)
            {
                NewUpgradeManager.Instance.TryUpgrade(_upgradeType);
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
            if (NewUpgradeManager.Instance == null)
            {
                return;
            }

            int currentLevel = NewUpgradeManager.Instance.GetLevel(_upgradeType);
            bool isMaxLevel = NewUpgradeManager.Instance.IsMaxLevel(_upgradeType);

            Level.Value = isMaxLevel ? $"Lv.{currentLevel} (MAX)" : $"Lv.{currentLevel}";
        }

        private void RefreshCost()
        {
            if (NewUpgradeManager.Instance == null)
            {
                return;
            }

            bool isMaxLevel = NewUpgradeManager.Instance.IsMaxLevel(_upgradeType);

            if (isMaxLevel)
            {
                Cost.Value = "-";
                return;
            }

            int cost = NewUpgradeManager.Instance.GetUpgradeCost(_upgradeType);
            Currency costCurrency = cost;
            Cost.Value = $"${costCurrency}";
        }

        private void RefreshEffect()
        {
            if (NewUpgradeManager.Instance == null)
            {
                return;
            }

            int currentLevel = NewUpgradeManager.Instance.GetLevel(_upgradeType);

            Effect.Value = _upgradeType switch
            {
                EUpgradeType.Tool => $"클릭 데미지: {NewUpgradeManager.Instance.GetToolDamage(currentLevel)}",
                EUpgradeType.Worker => $"초당 데미지: {NewUpgradeManager.Instance.GetWorkerDps(currentLevel)}",
                _ => ""
            };
        }

        private void RefreshCanUpgrade()
        {
            if (NewUpgradeManager.Instance == null || CurrencyManager.Instance == null)
            {
                CanUpgrade.Value = false;
                return;
            }

            bool isMaxLevel = NewUpgradeManager.Instance.IsMaxLevel(_upgradeType);

            if (isMaxLevel)
            {
                CanUpgrade.Value = false;
                return;
            }

            int cost = NewUpgradeManager.Instance.GetUpgradeCost(_upgradeType);
            CanUpgrade.Value = CurrencyManager.Instance.CanAfford(ECurrencyType.Money, cost);
        }
    }
}
