using JunkyardClicker.Core;

namespace JunkyardClicker.Resource
{
    /// <summary>
    /// 업그레이드 레벨에 기반한 데미지 계산
    /// DIP: IUpgradeService 인터페이스를 통해 의존성 주입
    /// </summary>
    public class UpgradeBasedDamageCalculator : IDamageCalculator
    {
        private readonly IUpgradeService _upgradeService;

        /// <summary>
        /// 의존성 주입을 통한 생성자
        /// </summary>
        /// <param name="upgradeService">업그레이드 서비스</param>
        public UpgradeBasedDamageCalculator(IUpgradeService upgradeService)
        {
            _upgradeService = upgradeService;
        }

        /// <summary>
        /// 하위 호환성을 위한 기본 생성자
        /// ServiceLocator를 통해 자동으로 서비스를 찾음
        /// </summary>
        public UpgradeBasedDamageCalculator()
        {
            if (ServiceLocator.TryGet<IUpgradeService>(out var service))
            {
                _upgradeService = service;
            }
        }

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
