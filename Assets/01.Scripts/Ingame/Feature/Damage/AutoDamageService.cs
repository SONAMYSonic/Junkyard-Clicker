using UnityEngine;

namespace JunkyardClicker.Ingame.Damage
{
    using JunkyardClicker.Core;

    /// <summary>
    /// 자동 데미지 서비스
    /// 일정 시간마다 자동으로 데미지를 적용
    /// </summary>
    public class AutoDamageService : MonoBehaviour
    {
        [SerializeField]
        private float _tickInterval = 1f;

        private IDamageManager _damageManager;
        private float _tickTimer;

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

        private void Update()
        {
            if (_damageManager == null)
            {
                return;
            }

            _tickTimer += Time.deltaTime;

            if (_tickTimer >= _tickInterval)
            {
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
