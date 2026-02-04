using System;
using Cysharp.Threading.Tasks;
using Firebase.Auth;

public class FirebaseAccountRepository : IAccountRepository
{
    private FirebaseAuth _auth;

    public FirebaseAccountRepository()
    {
        _auth = FirebaseAuth.DefaultInstance;
    }

    public async UniTask<AccountResult> Register(string email, string password)
    {
        try
        {
            await _auth.CreateUserWithEmailAndPasswordAsync(email, password).AsUniTask();
            await UniTask.SwitchToMainThread();

            return new AccountResult
            {
                Success = true,
            };
        }
        catch (Exception e)
        {
            await UniTask.SwitchToMainThread();

            return new AccountResult
            {
                Success = false,
                ErrorMessage = e.Message
            };
        }
    }

    public async UniTask<AccountResult> Login(string email, string password)
    {
        try
        {
            await _auth.SignInWithEmailAndPasswordAsync(email, password).AsUniTask();
            await UniTask.SwitchToMainThread();

            return new AccountResult
            {
                Success = true,
                Account = new Account(email, password)
            };
        }
        catch (Exception e)
        {
            await UniTask.SwitchToMainThread();

            return new AccountResult
            {
                Success = false,
                ErrorMessage = e.Message
            };
        }
    }

    public void Logout()
    {
        _auth.SignOut();
    }
}