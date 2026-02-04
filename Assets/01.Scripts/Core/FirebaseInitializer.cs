using System;
using Cysharp.Threading.Tasks;
using Firebase;
using UnityEngine;

public class FirebaseInitializer : MonoBehaviour
{
    public static FirebaseInitializer Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
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
                Debug.Log("Firebase 초기화 성공!");
            }
            else
            {
                Debug.LogError("Firebase 초기화 실패: " + status);
            }
        }
        catch (FirebaseException e)
        {
            Debug.LogError("Firebase 초기화 실패: " + e.Message);
        }
        catch (Exception e)
        {
            Debug.LogError("초기화 실패: " + e.Message);
        }
    }
}
