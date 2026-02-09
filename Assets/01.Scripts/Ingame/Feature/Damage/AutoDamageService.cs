using System;
using UnityEngine;

namespace JunkyardClicker.Resource
{
    using JunkyardClicker.Core;

    // 자동 데미지 서비스 - 일정 시간마다 자동으로 데미지 적용
    public class AutoDamageService : MonoBehaviour, IAutoDamageService
    {
        [SerializeField]
        private float _tickInterval = 1f;

        private IDamageManager _damageManager;
        private float _tickTimer;
        private bool _isEnabled = true;

        public event Action OnAutoDamageTick;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        public float TickInterval
        {
            get => _tickInterval;
            set => _tickInterval = Mathf.Max(0.1f, value);
        }

        // 서비스 등록
        private void Awake()
        {
            ServiceLocator.Register<IAutoDamageService>(this);
        }

        // 의존성 주입
        private void Start()
        {
            if (ServiceLocator.TryGet<IDamageManager>(out var damageManager))
            {
                _damageManager = damageManager;
            }
            else
            {
                _damageManager = DamageManager.Instance;
            }
        }

        // 서비스 등록 해제
        private void OnDestroy()
        {
            ServiceLocator.Unregister<IAutoDamageService>();
        }

        // 타이머 업데이트 및 자동 데미지 적용
        private void Update()
        {
            if (!_isEnabled || _damageManager == null)
            {
                return;
            }

            _tickTimer += Time.deltaTime;

            if (_tickTimer >= _tickInterval)
            {
                // 이벤트 발행 (다른 시스템이 구독 가능)
                OnAutoDamageTick?.Invoke();

                // 데미지 적용
                _damageManager.ApplyAutoDamage();
                _tickTimer = 0f;
            }
        }
    }
}
