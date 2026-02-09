using System;
using Cysharp.Threading.Tasks;
#if !UNITY_WEBGL
using Firebase;
#endif
using UnityEngine;

// Firebase 초기화 담당
public class FirebaseInitializer : MonoBehaviour
{
    public static FirebaseInitializer Instance { get; private set; }

    // Firebase 초기화 완료 대기용 Task
    public static UniTask InitializationTask => _initializationSource.Task;

    private static UniTaskCompletionSource _initializationSource = new UniTaskCompletionSource();

    public static bool IsInitialized { get; private set; }
    public static bool IsAvailable { get; private set; }

    // 싱글톤 설정
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Firebase 초기화 시작
    private void Start()
    {
        InitFirebase().Forget();
    }

    // Firebase 비동기 초기화
    private async UniTask InitFirebase()
    {
#if UNITY_WEBGL
        // WebGL에서는 Firebase를 사용하지 않고 로컬 모드로 동작
        IsAvailable = false;
        IsInitialized = true;
        _initializationSource.TrySetResult();
        Debug.Log("[FirebaseInitializer] WebGL 빌드 - 로컬 모드로 동작");
        await UniTask.CompletedTask;
#else
        try
        {
            DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();

            if (status == DependencyStatus.Available)
            {
                IsAvailable = true;
                IsInitialized = true;
                _initializationSource.TrySetResult();
                Debug.Log("[FirebaseInitializer] Firebase 초기화 성공!");
            }
            else
            {
                IsAvailable = false;
                IsInitialized = true;
                _initializationSource.TrySetResult(); // 실패해도 완료 신호 (로컬 모드로 동작)
                Debug.LogError($"[FirebaseInitializer] Firebase 초기화 실패: {status}");
            }
        }
        catch (FirebaseException e)
        {
            IsAvailable = false;
            IsInitialized = true;
            _initializationSource.TrySetResult();
            Debug.LogError($"[FirebaseInitializer] Firebase 예외: {e.Message}");
        }
        catch (Exception e)
        {
            IsAvailable = false;
            IsInitialized = true;
            _initializationSource.TrySetResult();
            Debug.LogError($"[FirebaseInitializer] 초기화 예외: {e.Message}");
        }
#endif
    }

    // 초기화 상태 리셋 (테스트용)
    public static void ResetForTesting()
    {
        IsInitialized = false;
        IsAvailable = false;
        _initializationSource = new UniTaskCompletionSource();
    }
}
