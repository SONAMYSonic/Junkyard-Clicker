using UnityEngine;

namespace JunkyardClicker.Feedback
{
    /// <summary>
    /// 카메라 흔들림 효과
    /// </summary>
    public class CameraShake : MonoBehaviour
    {
        private Vector3 _originalPosition;
        private float _shakeIntensity;
        private float _shakeDuration;
        private float _shakeTimer;

        private void Awake()
        {
            _originalPosition = transform.localPosition;
        }

        private void Update()
        {
            if (_shakeTimer > 0)
            {
                float progress = _shakeTimer / _shakeDuration;
                float currentIntensity = _shakeIntensity * progress;

                transform.localPosition = _originalPosition + Random.insideUnitSphere * currentIntensity;

                _shakeTimer -= Time.unscaledDeltaTime;

                if (_shakeTimer <= 0)
                {
                    transform.localPosition = _originalPosition;
                }
            }
        }

        public void Shake(float intensity, float duration)
        {
            _shakeIntensity = intensity;
            _shakeDuration = duration;
            _shakeTimer = duration;
        }
    }
}
