using UnityEngine;
using UnityEngine.InputSystem;

namespace JunkyardClicker.Feedback
{
    using JunkyardClicker.Core;

    // 피드백 매니저 - 게임 피드백 효과 관리
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

        // 싱글톤 설정 및 카메라 캐싱
        private void Awake()
        {
            SetupSingleton();
            CacheMainCamera();
            ServiceLocator.Register<IFeedbackManager>(this);
        }

        // 메인 카메라 캐싱
        private void CacheMainCamera()
        {
            _mainCamera = Camera.main;

            if (_mainCamera == null)
            {
                Debug.LogWarning("[FeedbackManager] Main Camera를 찾을 수 없습니다.");
            }
        }

        // 싱글톤 인스턴스 설정
        private void SetupSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        // 이벤트 구독
        private void OnEnable()
        {
            GameEvents.OnDamageDealt += HandleDamageDealt;
            GameEvents.OnPartDestroyed += HandlePartDestroyed;
            GameEvents.OnCarDestroyed += HandleCarDestroyed;
        }

        // 이벤트 구독 해제
        private void OnDisable()
        {
            GameEvents.OnDamageDealt -= HandleDamageDealt;
            GameEvents.OnPartDestroyed -= HandlePartDestroyed;
            GameEvents.OnCarDestroyed -= HandleCarDestroyed;
        }

        // 오브젝트 파괴 시 정리
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                ServiceLocator.Unregister<IFeedbackManager>();
            }
        }

        // 데미지 발생 시 처리
        private void HandleDamageDealt(int damage)
        {
            PlayFeedback(FeedbackType.NormalHit, damage);
            SpawnDamagePopup(damage, GetPopupSpawnPosition());
        }

        // 파츠 파괴 시 처리
        private void HandlePartDestroyed(CarPartType partType)
        {
            PlayFeedback(FeedbackType.PartDestroy);
        }

        // 차량 파괴 시 처리
        private void HandleCarDestroyed(int reward)
        {
            PlayFeedback(FeedbackType.CarDestroy);
        }

        // 피드백 효과 재생
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

        // 데미지 팝업 생성
        public void SpawnDamagePopup(int damage, Vector3 position)
        {
            if (_damagePopupPrefab == null)
            {
                return;
            }

            DamagePopup popup = Instantiate(_damagePopupPrefab, position, Quaternion.identity, _popupContainer);
            popup.Initialize(damage);
        }

        // 카메라 쉐이크 재생
        private void PlayCameraShake(float intensity, float duration)
        {
            if (_cameraShake != null)
            {
                _cameraShake.Shake(intensity, duration);
            }
        }

        // 히트 스탑 재생
        private void PlayHitStop(float duration)
        {
            if (_hitStop != null)
            {
                _hitStop.Stop(duration);
            }
        }

        // 설정 가져오기
        private FeedbackConfig GetConfig()
        {
            if (_config != null)
            {
                return _config;
            }

            // 기본 설정 사용 (config가 없을 경우)
            return ScriptableObject.CreateInstance<FeedbackConfig>();
        }

        // 팝업 생성 위치 계산
        private Vector3 GetPopupSpawnPosition()
        {
            // 카메라가 없으면 기본 위치 반환
            if (_mainCamera == null)
            {
                return Vector3.zero;
            }

            // New Input System 사용
            Vector2 pointerPosition = GetPointerPosition();
            Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(new Vector3(pointerPosition.x, pointerPosition.y, 0f));
            worldPosition.z = 0f;

            float randomOffsetX = Random.Range(-0.3f, 0.3f);
            float randomOffsetY = Random.Range(-0.2f, 0.2f);

            return worldPosition + new Vector3(randomOffsetX, randomOffsetY, 0f);
        }

        // 현재 포인터 위치 반환
        private Vector2 GetPointerPosition()
        {
            // 터치 입력 우선
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                return Touchscreen.current.primaryTouch.position.ReadValue();
            }

            // 마우스 입력
            if (Mouse.current != null)
            {
                return Mouse.current.position.ReadValue();
            }

            return Vector2.zero;
        }
    }
}
