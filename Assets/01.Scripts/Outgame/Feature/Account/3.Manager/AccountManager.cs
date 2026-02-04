using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

// 매니저의 역할:
// 1. 도메인 관리: 생성/조회/수정/삭제와 같은 비즈니스 로직 
// 2. 외부와의 소통 창구
public class AccountManager : MonoBehaviour
{
    public static AccountManager Instance { get; private set; }

    private Account _currentAccount = null;
    public bool IsLogin => _currentAccount != null;
    public string Email => _currentAccount?.Email ?? string.Empty;

    private IAccountRepository _repository;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        _repository = new FirebaseAccountRepository();
    }

    // 로그인 시도
    public async UniTask<AccountResult> TryLogin(string email, string password)
    {
        // 1. 유효성 검사 (도메인 규칙)
        try
        {
            Account account = new Account(email, password);
        }
        catch (Exception ex)
        {
            return new AccountResult
            {
                Success = false,
                ErrorMessage = ex.Message,
            };
        }

        // 2. 레포지토리를 이용한 로그인
        AccountResult result = await _repository.Login(email, password);
        if (result.Success)
        {
            _currentAccount = result.Account;
            return new AccountResult
            {
                Success = true,
                Account = _currentAccount,
            };
        }
        else
        {
            return new AccountResult
            {
                Success = false,
                ErrorMessage = result.ErrorMessage,
            };
        }
    }

    // 회원가입 시도
    public async UniTask<AccountResult> TryRegister(string email, string password)
    {
        // 1. 유효성 검사 (도메인 규칙)
        try
        {
            Account account = new Account(email, password);
        }
        catch (Exception ex)
        {
            return new AccountResult
            {
                Success = false,
                ErrorMessage = ex.Message,
            };
        }
        
        // 2. 레포지토리를 이용한 회원가입
        AccountResult result = await _repository.Register(email, password);
        if (result.Success)
        {
            return new AccountResult
            {
                Success = true,
            };
        }
        else
        {
            return new AccountResult
            {
                Success = false,
                ErrorMessage = result.ErrorMessage,
            };
        }
    }

    // 로그아웃
    public void Logout()
    {
        _currentAccount = null;
        _repository.Logout();
    }
}
