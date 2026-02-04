using System;
using System.Threading;
using Firebase;
using Firebase.Auth;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Firestore;
using TMPro;
using Cysharp.Threading.Tasks;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class FirebaseTutorial : MonoBehaviour
{
    private FirebaseApp _app = null;
    private FirebaseAuth _auth = null;
    private FirebaseFirestore _db = null;
    
    [SerializeField] public TextMeshProUGUI _progressText;
    
    // 이 씬이 시작되면 아래 내용을 단계적으로 수행
    // 각 단계마다 ProgressText의 내용이 바뀐다.
    // 각 단계마다 Debug.Log로 완료를 알린다.
    // 1. 파이어베이스 초기화
    // 2. 로그아웃
    // 3. 재로그인
    // 4. 강아지 추가 (전제조건: 파이어스토어 규칙에 로그인한 사람만 읽기/쓰기 허용)
    
    private void Start()
    {
        RunSequenceAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }
    
    // 각 단계를 순서대로 비동기 처리
    private async UniTask RunSequenceAsync(CancellationToken ct)
    {
        try
        {
            await FirebaseInitializeAsync(ct);
            Logout();
            await LoginAsync("mknoh1004@skku.re.kr", "password123", ct);
            await SaveDogsAsync(ct);
        }
        catch (OperationCanceledException)
        {
            // CancellationToken으로 정상 취소된 경우
            Debug.Log("시퀀스 취소됨 (씬 전환 / 오브젝트 파괴)");
        }
        catch (Exception e)
        {
            // 진짜 에러
            Debug.LogError($"시퀀스 실패: {e}");
            _progressText.text = "에러 발생!";
        }
    }


    // Task란 C# 비동기 작업을 의미한다.
    // 결과값을 Result로 가지고 있고...
    // 진행사항 및 에러값을 또 가지고 있다
    // await는 변수로도 쓸 수 있다 -> async가 붙으면 비동기 작업의 키워드가 된다
    private async UniTask FirebaseInitializeAsync(CancellationToken ct)
    {
        var status = await FirebaseApp.CheckAndFixDependenciesAsync()
            .AsUniTask()
            .AttachExternalCancellation(ct);
        // 이 작업은 유니티가 실행중 CPU 1에게 작업을 시킬 수도 있고, CPU 2에게 작업을 시킬 수도 있다.
        // 작업이 완료되고 나서 유니티가 실행중인 CPU1에서 작업을 이어나가는게 아니라,
        // CPU2에서 MonoBehaviour 작업을 이어나가려 하면 유니티가 뻗는다.

        if (status != DependencyStatus.Available)
            throw new Exception("Firebase initialize failed: " + status);

        // Unity UI 수정은 메인스레드에서 안전하게
        await UniTask.SwitchToMainThread(ct);

        _app = FirebaseApp.DefaultInstance;
        _auth = FirebaseAuth.DefaultInstance;
        _db = FirebaseFirestore.DefaultInstance;

        Debug.Log("Dependency available");
        _progressText.text = "파이어베이스 초기화 성고오오오옹~~";
    }

    private async UniTask SaveDogsAsync(CancellationToken ct)
    {
        Dog dog = new Dog("견훤이", 3);

        await _db.Collection("Dogs").Document("개집").SetAsync(dog)
            .AsUniTask()
            .AttachExternalCancellation(ct);

        await UniTask.SwitchToMainThread(ct);

        Debug.Log("저장 성고오오오옹나이스~");
        _progressText.text = "강아지 저장 성공!";
    }


    private void LoadMyDog()
    {
        _db.Collection("Dogs").Document("개집").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                var snapshot = task.Result;
                if (snapshot.Exists)
                {
                    Dog dog = snapshot.ConvertTo<Dog>();
                    Debug.LogFormat("불러오기 성공! 이름: {0}, 나이: {1}", dog.Name, dog.Age);
                }
                else
                {
                    Debug.Log("불러오기 실패! 문서가 존재하지 않음.");
                }
            }
            else
            {
                Debug.LogError("불러오기 실패! " + task.Exception);
            }
        });
    }

    private void LoadDogs()
    {
        _db.Collection("Dogs").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                var snapshots = task.Result;
                Debug.Log("강아지들 ----------------------------");
                foreach (var snapshot in snapshots.Documents)
                {
                    Dog myDog = snapshot.ConvertTo<Dog>();
                    Debug.LogFormat("이름: {0}, 나이: {1}", myDog.Name, myDog.Age);
                }
                
                Debug.LogError("불러오기 성공!");
            }
            else
            {
                Debug.LogError("불러오기 실패! " + task.Exception);
            }
        });
    }

    private void DeleteDogs()
    {
        // 목표: 특정 강아지 삭제
        _db.Collection("Dogs").WhereEqualTo("Name", "간장게장이").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                var snapshots = task.Result;
                foreach (var snapshot in snapshots.Documents)
                {
                    Dog myDog = snapshot.ConvertTo<Dog>();
                    if (myDog.Name == "간장게장이이")
                    {
                        _db.Collection("Dogs").Document(myDog.Id).DeleteAsync().ContinueWithOnMainThread(task =>
                        {
                            if (task.IsCompletedSuccessfully)
                            {
                                Debug.Log("삭제 성공!");
                            }
                            else
                            {
                                Debug.LogError("삭제 실패! " + task.Exception);
                            }
                        });
                    }
                }
                
                Debug.LogError("불러오기 성공!");
            }
            else
            {
                Debug.LogError("불러오기 실패! " + task.Exception);
            }
        });
    }
    
    public void Register(string email, string password)
    {
        _auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled) {
                Debug.LogError("로그인 취소됨");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("로그인 실패: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            AuthResult result = task.Result;
            Debug.LogFormat("Register Successful: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
        });
    }

    private async UniTask LoginAsync(string email, string password, CancellationToken ct)
    {
        var result = await _auth.SignInWithEmailAndPasswordAsync(email, password)
            .AsUniTask()
            .AttachExternalCancellation(ct);

        await UniTask.SwitchToMainThread(ct);

        var user = result.User;
        Debug.LogFormat("로그인 성공!: {0} ({1})", user.DisplayName, user.UserId);
        _progressText.text = "로그인 성공!";
    }

    
    private void Logout()
    {
        _auth.SignOut();
        Debug.Log("로그아웃 성공!");
        _progressText.text = "로그아웃 성공!";
    }
    
    private void CheckLoginStatus()
    {
        FirebaseUser user = _auth.CurrentUser;
        if (user != null)
        {
            Debug.LogFormat("로그인 상태: {0} ({1})", user.DisplayName, user.UserId);
        }
        else
        {
            Debug.Log("비로그인 상태");
        }
    }

    private void Update()
    {
        if (_app == null) return;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        // New Input System is active (legacy input is disabled)
        if (Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Register("mknoh1004@skku.re.kr", "password123");
        }

        if (Keyboard.current != null && Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            LoginAsync("mknoh1004@skku.re.kr", "password123", this.GetCancellationTokenOnDestroy()).Forget();
        }
        
        if (Keyboard.current != null && Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            Logout();
        }
        
        if (Keyboard.current != null && Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            CheckLoginStatus();
        }
        
        if (Keyboard.current != null && Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            SaveDogsAsync(this.destroyCancellationToken).Forget();
        }

        if (Keyboard.current != null && Keyboard.current.digit6Key.wasPressedThisFrame)
        {
            LoadMyDog();
        }
        
        if (Keyboard.current != null && Keyboard.current.digit7Key.wasPressedThisFrame)
        {
            LoadDogs();
        }
        
#else
        // Legacy Input Manager or Both is active
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Register("mknoh1004@skku.re.kr", "password123");
        }
#endif
    }
}