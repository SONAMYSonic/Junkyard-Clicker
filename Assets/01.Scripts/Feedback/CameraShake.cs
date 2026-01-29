using System.Collections;
using UnityEngine;

namespace JunkyardClicker.Feedback
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField]
        private Transform _cameraTransform;

        [SerializeField]
        private AnimationCurve _shakeCurve;

        private Vector3 _originalPosition;
        private Coroutine _shakeCoroutine;

        private void Awake()
        {
            if (_cameraTransform == null)
            {
                _cameraTransform = Camera.main != null ? Camera.main.transform : transform;
            }

            _originalPosition = _cameraTransform.localPosition;

            if (_shakeCurve == null || _shakeCurve.length == 0)
            {
                _shakeCurve = CreateDefaultCurve();
            }
        }

        public void Shake(float intensity, float duration)
        {
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
            }

            _shakeCoroutine = StartCoroutine(ShakeRoutine(intensity, duration));
        }

        private IEnumerator ShakeRoutine(float intensity, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float progress = elapsed / duration;
                float curveValue = _shakeCurve.Evaluate(progress);
                float currentIntensity = intensity * curveValue;

                float offsetX = Random.Range(-1f, 1f) * currentIntensity;
                float offsetY = Random.Range(-1f, 1f) * currentIntensity;

                _cameraTransform.localPosition = _originalPosition + new Vector3(offsetX, offsetY, 0f);

                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            _cameraTransform.localPosition = _originalPosition;
            _shakeCoroutine = null;
        }

        private AnimationCurve CreateDefaultCurve()
        {
            return new AnimationCurve(
                new Keyframe(0f, 1f),
                new Keyframe(0.1f, 1f),
                new Keyframe(1f, 0f)
            );
        }

        public void StopShake()
        {
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
                _shakeCoroutine = null;
            }

            _cameraTransform.localPosition = _originalPosition;
        }
    }
}
