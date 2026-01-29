using System.Collections;
using UnityEngine;

namespace JunkyardClicker.Feedback
{
    public class HitStop : MonoBehaviour
    {
        [SerializeField]
        [Range(0f, 0.1f)]
        private float _timeScaleDuringStop = 0f;

        private Coroutine _stopCoroutine;
        private float _originalTimeScale = 1f;

        public void Stop(float duration)
        {
            if (_stopCoroutine != null)
            {
                StopCoroutine(_stopCoroutine);
                Time.timeScale = _originalTimeScale;
            }

            _stopCoroutine = StartCoroutine(StopRoutine(duration));
        }

        private IEnumerator StopRoutine(float duration)
        {
            _originalTimeScale = Time.timeScale;
            Time.timeScale = _timeScaleDuringStop;

            yield return new WaitForSecondsRealtime(duration);

            Time.timeScale = _originalTimeScale;
            _stopCoroutine = null;
        }

        private void OnDestroy()
        {
            if (_stopCoroutine != null)
            {
                Time.timeScale = _originalTimeScale;
            }
        }

        private void OnDisable()
        {
            if (_stopCoroutine != null)
            {
                StopCoroutine(_stopCoroutine);
                Time.timeScale = _originalTimeScale;
                _stopCoroutine = null;
            }
        }
    }
}
