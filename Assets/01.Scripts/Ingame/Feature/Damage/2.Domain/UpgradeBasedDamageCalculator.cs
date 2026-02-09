using JunkyardClicker.Core;

namespace JunkyardClicker.Resource
{
    // 업그레이드 레벨에 기반한 데미지 계산
    public class UpgradeBasedDamageCalculator : IDamageCalculator
    {
        private readonly IUpgradeService _upgradeService;

        // 의존성 주입을 통한 생성자
        public UpgradeBasedDamageCalculator(IUpgradeService upgradeService)
        {
            _upgradeService = upgradeService;
        }

        // 하위 호환성을 위한 기본 생성자
        public UpgradeBasedDamageCalculator()
        {
            if (ServiceLocator.TryGet<IUpgradeService>(out var service))
            {
                _upgradeService = service;
            }
        }

        // 클릭 데미지 계산
        public int CalculateClickDamage()
        {
            if (_upgradeService == null)
            {
                // 폴백: 레거시 코드 지원
                if (UpgradeManager.Instance != null)
                {
                    return UpgradeManager.Instance.ToolDamage;
                }
                return 1;
            }

            return _upgradeService.ToolDamage;
        }

        // 자동 데미지 계산
        public int CalculateAutoDamage()
        {
            if (_upgradeService == null)
            {
                // 폴백: 레거시 코드 지원
                if (UpgradeManager.Instance != null)
                {
                    return UpgradeManager.Instance.WorkerDps;
                }
                return 0;
            }

            return _upgradeService.WorkerDps;
        }
    }
}
