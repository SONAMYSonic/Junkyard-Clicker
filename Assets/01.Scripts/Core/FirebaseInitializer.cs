using System;
using Cysharp.Threading.Tasks;
using Firebase;
using UnityEngine;

/// <summary>
/// Firebase 초기화 담당
/// 다른 Manager들이 Firebase 사용 전 InitializationTask를 await해야 함
/// </summary>
public class FirebaseInitializer : MonoBehaviour
{
    public static FirebaseInitializer Instance { get; private set; }

    /// <summary>
    /// Firebase 초기화 완료 대기용 Task
    /// 사용법: await FirebaseInitializer.InitializationTask;
    /// </summary>
    public static UniTask InitializationTask => _initializationSource.Task;

    private static UniTaskCompletionSource _initializationSource = new UniTaskCompletionSource();

    /// <summary>
    /// Firebase 초기화 완료 여부
    /// </summary>
    public static bool IsInitialized { get; private set; }

    /// <summary>
    /// Firebase 초기화 성공 여부
    /// </summary>
    public static bool IsAvailable { get; private set; }

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

    private void Start()
    {
        InitFirebase().Forget();
    }

    private async UniTask InitFirebase()
    {
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
    }

    /// <summary>
    /// 초기화 상태 리셋 (테스트용)
    /// </summary>
    public static void ResetForTesting()
    {
        IsInitialized = false;
        IsAvailable = false;
        _initializationSource = new UniTaskCompletionSource();
    }
}
