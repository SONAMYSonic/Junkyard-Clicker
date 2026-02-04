using UnityEngine;

namespace JunkyardClicker.Feedback
{
    using JunkyardClicker.Core;

    /// <summary>
    /// 피드백 매니저 - 도메인 방식 구현
    /// </summary>
    public class FeedbackManager : MonoBehaviour, IFeedbackManager
    {
        public static FeedbackManager Instance { get; private set; }

        [SerializeField]
        private CameraShake _cameraShake;

        [SerializeField]
        private HitStop _hitStop;

        [SerializeField]
        private DamagePopup _damagePopupPrefab;

        [SerializeField]
        private Transform _popupContainer;

        [SerializeField]
        private FeedbackConfig _config;

        // Camera.main 캐싱 (매번 태그 검색 방지)
        private Camera _mainCamera;

        private void Awake()
        {
            SetupSingleton();
            CacheMainCamera();
            ServiceLocator.Register<IFeedbackManager>(this);
        }

        private void CacheMainCamera()
        {
            _mainCamera = Camera.main;

            if (_mainCamera == null)
            {
                Debug.LogWarning("[FeedbackManager] Main Camera를 찾을 수 없습니다.");
            }
        }

        private void SetupSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

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

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                ServiceLocator.Unregister<IFeedbackManager>();
            }
        }

        private void HandleDamageDealt(int damage)
        {
            PlayFeedback(FeedbackType.NormalHit, damage);
            SpawnDamagePopup(damage, GetPopupSpawnPosition());
        }

        private void HandlePartDestroyed(CarPartType partType)
        {
            PlayFeedback(FeedbackType.PartDestroy);
        }

        private void HandleCarDestroyed(int reward)
        {
            PlayFeedback(FeedbackType.CarDestroy);
        }

        public void PlayFeedback(FeedbackType type, int value = 0)
        {
            float intensity;
            float duration;
            float hitStopMultiplier = 1f;

            switch (type)
            {
                case FeedbackType.NormalHit:
                    intensity = GetConfig().NormalHitShakeIntensity;
                    duration = GetConfig().NormalHitShakeDuration;

                    if (value >= GetConfig().HitStopDamageThreshold)
                    {
                        PlayHitStop(GetConfig().HitStopDuration);
                    }
                    break;

                case FeedbackType.PartDestroy:
                    intensity = GetConfig().PartDestroyShakeIntensity;
                    duration = GetConfig().PartDestroyShakeDuration;
                    hitStopMultiplier = 2f;
                    PlayHitStop(GetConfig().HitStopDuration * hitStopMultiplier);
                    break;

                case FeedbackType.CarDestroy:
                    intensity = GetConfig().CarDestroyShakeIntensity;
                    duration = GetConfig().CarDestroyShakeDuration;
                    hitStopMultiplier = 3f;
                    PlayHitStop(GetConfig().HitStopDuration * hitStopMultiplier);
                    break;

                default:
                    return;
            }

            PlayCameraShake(intensity, duration);
        }

        public void SpawnDamagePopup(int damage, Vector3 position)
        {
            if (_damagePopupPrefab == null)
            {
                return;
            }

            DamagePopup popup = Instantiate(_damagePopupPrefab, position, Quaternion.identity, _popupContainer);
            popup.Initialize(damage);
        }

        private void PlayCameraShake(float intensity, float duration)
        {
            if (_cameraShake != null)
            {
                _cameraShake.Shake(intensity, duration);
            }
        }

        private void PlayHitStop(float duration)
        {
            if (_hitStop != null)
            {
                _hitStop.Stop(duration);
            }
        }

        private FeedbackConfig GetConfig()
        {
            if (_config != null)
            {
                return _config;
            }

            // 기본 설정 사용 (config가 없을 경우)
            return ScriptableObject.CreateInstance<FeedbackConfig>();
        }

        private Vector3 GetPopupSpawnPosition()
        {
            // 카메라가 없으면 기본 위치 반환
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main; // 재시도
                if (_mainCamera == null)
                {
                    return Vector3.zero;
                }
            }

            Vector3 mousePosition = UnityEngine.Input.mousePosition;
            Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(mousePosition);
            worldPosition.z = 0f;

            float randomOffsetX = Random.Range(-0.3f, 0.3f);
            float randomOffsetY = Random.Range(-0.2f, 0.2f);

            return worldPosition + new Vector3(randomOffsetX, randomOffsetY, 0f);
        }
    }
}
