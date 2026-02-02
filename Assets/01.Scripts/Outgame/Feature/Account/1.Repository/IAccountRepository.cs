// 계정 저장소가 가져야 할 약속(인터페이스)
public interface IAccountRepository
{
    bool IsEmailAvailable(string email);
    AuthResult Register(string email, string password);
    AuthResult Login(string email, string password);
    void Logout();
}
