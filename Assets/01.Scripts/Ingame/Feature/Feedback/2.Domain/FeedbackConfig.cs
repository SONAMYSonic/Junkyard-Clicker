using UnityEngine;

namespace JunkyardClicker.Feedback
{
    /// <summary>
    /// 피드백 설정 데이터
    /// </summary>
    [CreateAssetMenu(fileName = "FeedbackConfig", menuName = "JunkyardClicker/Feedback Config")]
    public class FeedbackConfig : ScriptableObject
    {
        [Header("Normal Hit")]
        [SerializeField]
        private float _normalHitShakeIntensity = 0.1f;

        [SerializeField]
        private float _normalHitShakeDuration = 0.1f;

        [Header("Part Destroy")]
        [SerializeField]
        private float _partDestroyShakeIntensity = 0.3f;

        [SerializeField]
        private float _partDestroyShakeDuration = 0.2f;

        [Header("Car Destroy")]
        [SerializeField]
        private float _carDestroyShakeIntensity = 0.5f;

        [SerializeField]
        private float _carDestroyShakeDuration = 0.3f;

        [Header("Hit Stop")]
        [SerializeField]
        private float _hitStopDuration = 0.03f;

        [SerializeField]
        private int _hitStopDamageThreshold = 10;

        public float NormalHitShakeIntensity => _normalHitShakeIntensity;
        public float NormalHitShakeDuration => _normalHitShakeDuration;
        public float PartDestroyShakeIntensity => _partDestroyShakeIntensity;
        public float PartDestroyShakeDuration => _partDestroyShakeDuration;
        public float CarDestroyShakeIntensity => _carDestroyShakeIntensity;
        public float CarDestroyShakeDuration => _carDestroyShakeDuration;
        public float HitStopDuration => _hitStopDuration;
        public int HitStopDamageThreshold => _hitStopDamageThreshold;
    }
}
