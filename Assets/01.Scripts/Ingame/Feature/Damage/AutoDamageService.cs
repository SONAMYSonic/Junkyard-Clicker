using System;
using UnityEngine;

namespace JunkyardClicker.Resource
{
    using JunkyardClicker.Core;

    /// <summary>
    /// 자동 데미지 핸들러
    /// IAutoDamageService 인터페이스 구현
    /// 일정 시간마다 자동으로 데미지를 적용
    /// </summary>
    public class AutoDamageHandler : MonoBehaviour, IAutoDamageService
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
            set => _tickInterval = Mathf.Max(0.1f, value); // 최소 0.1초
        }

        private void Awake()
        {
            ServiceLocator.Register<IAutoDamageService>(this);
        }

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

        private void OnDestroy()
        {
            ServiceLocator.Unregister<IAutoDamageService>();
        }

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

        public void ResetTimer()
        {
            _tickTimer = 0f;
        }
    }
}
