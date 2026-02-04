// 계정 저장소가 가져야 할 약속(인터페이스)
using Cysharp.Threading.Tasks;

public interface IAccountRepository
{
    UniTask<AccountResult> Register(string email, string password);
    UniTask<AccountResult> Login(string email, string password);
    void Logout();
}
