using UnityEngine;

namespace JunkyardClicker.Feedback
{
    using Core;

    public class FeedbackManager : MonoBehaviour
    {
        [SerializeField]
        private CameraShake _cameraShake;

        [SerializeField]
        private HitStop _hitStop;

        [SerializeField]
        private DamagePopup _damagePopupPrefab;

        [SerializeField]
        private Transform _popupContainer;

        [Header("Settings")]
        [SerializeField]
        private float _normalHitShakeIntensity = 0.1f;

        [SerializeField]
        private float _normalHitShakeDuration = 0.1f;

        [SerializeField]
        private float _partDestroyShakeIntensity = 0.3f;

        [SerializeField]
        private float _partDestroyShakeDuration = 0.2f;

        [SerializeField]
        private float _carDestroyShakeIntensity = 0.5f;

        [SerializeField]
        private float _carDestroyShakeDuration = 0.3f;

        [SerializeField]
        private float _hitStopDuration = 0.03f;

        private void OnEnable()
        {
            GameEvents.OnDamageDealt += HandleDamageDealt;
            GameEvents.OnPartDestroyed += HandlePartDestroyed;
            GameEvents.OnCarDestroyed += HandleCarDestroyed;
        }

        private void OnDisable()
        {
            GameEvents.OnDamageDealt -= HandleDamageDealt;
            GameEvents.OnPartDestroyed -= HandlePartDestroyed;
            GameEvents.OnCarDestroyed -= HandleCarDestroyed;
        }

        private void HandleDamageDealt(int damage)
        {
            PlayHitFeedback(damage);
            SpawnDamagePopup(damage);
        }

        private void HandlePartDestroyed(CarPartType partType)
        {
            PlayPartDestroyFeedback();
        }

        private void HandleCarDestroyed(int reward)
        {
            PlayCarDestroyFeedback();
        }

        private void PlayHitFeedback(int damage)
        {
            if (_cameraShake != null)
            {
                _cameraShake.Shake(_normalHitShakeIntensity, _normalHitShakeDuration);
            }

            if (_hitStop != null && damage >= 10)
            {
                _hitStop.Stop(_hitStopDuration);
            }
        }

        private void PlayPartDestroyFeedback()
        {
            if (_cameraShake != null)
            {
                _cameraShake.Shake(_partDestroyShakeIntensity, _partDestroyShakeDuration);
            }

            if (_hitStop != null)
            {
                _hitStop.Stop(_hitStopDuration * 2f);
            }
        }

        private void PlayCarDestroyFeedback()
        {
            if (_cameraShake != null)
            {
                _cameraShake.Shake(_carDestroyShakeIntensity, _carDestroyShakeDuration);
            }

            if (_hitStop != null)
            {
                _hitStop.Stop(_hitStopDuration * 3f);
            }
        }

        private void SpawnDamagePopup(int damage)
        {
            if (_damagePopupPrefab == null)
            {
                return;
            }

            Vector3 spawnPosition = GetPopupSpawnPosition();
            DamagePopup popup = Instantiate(_damagePopupPrefab, spawnPosition, Quaternion.identity, _popupContainer);
            popup.Initialize(damage);
        }

        private Vector3 GetPopupSpawnPosition()
        {
            Vector3 mousePosition = UnityEngine.Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            worldPosition.z = 0f;

            float randomOffsetX = Random.Range(-0.3f, 0.3f);
            float randomOffsetY = Random.Range(-0.2f, 0.2f);

            return worldPosition + new Vector3(randomOffsetX, randomOffsetY, 0f);
        }
    }
}
