using System.Collections;
using UnityEngine;

namespace JunkyardClicker.Feedback
{
    /// <summary>
    /// 히트 스톱 효과 (타임 스케일 일시 정지)
    /// </summary>
    public class HitStop : MonoBehaviour
    {
        private Coroutine _stopCoroutine;

        public void Stop(float duration)
        {
            if (_stopCoroutine != null)
            {
                StopCoroutine(_stopCoroutine);
            }

            _stopCoroutine = StartCoroutine(DoHitStop(duration));
        }

        private IEnumerator DoHitStop(float duration)
        {
            Time.timeScale = 0f;

            yield return new WaitForSecondsRealtime(duration);

            Time.timeScale = 1f;
            _stopCoroutine = null;
        }

        private void OnDisable()
        {
            Time.timeScale = 1f;
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }
    }
}
