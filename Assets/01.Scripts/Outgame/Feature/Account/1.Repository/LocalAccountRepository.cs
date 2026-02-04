using UnityEngine;
using Cysharp.Threading.Tasks;

// 로컬 저장소 구현 (PlayerPrefs 사용)
public class LocalAccountRepository : IAccountRepository
{
    // 솔트: 비밀번호에 추가하는 비밀 문자열
    private const string SALT = "junkyard2024";
    
    // 이메일 사용 가능 여부 확인
    public bool IsEmailAvailable(string email)
    {
        if (PlayerPrefs.HasKey(email))
        {
            return false;
        }
        return true;
    }

    // 회원가입
    public UniTask<AccountResult> Register(string email, string password)
    {
        // 이메일 중복검사
        if (!IsEmailAvailable(email))
        {
            return new UniTask<AccountResult>(new AccountResult
            {
                Success = false,
                ErrorMessage = "중복된 계정입니다.",
            });
        }
        
        // 비밀번호 해싱 후 저장
        string hashedPassword = Crypto.HashPassword(password, SALT);
        PlayerPrefs.SetString(email, hashedPassword);
        
        return new UniTask<AccountResult>(new AccountResult
        {
            Success = true,
            Account = new Account(email, hashedPassword),
        });
    }

    // 로그인
    public UniTask<AccountResult> Login(string email, string password)
    {
        // 가입한 적 없으면 실패
        if (!PlayerPrefs.HasKey(email))
        {
            return new UniTask<AccountResult>(new AccountResult
            {
                Success = false,
                ErrorMessage = "아이디와 비밀번호를 확인해주세요.",
            });
        }
        
        // 저장된 해시 비밀번호 가져오기
        string savedHashedPassword = PlayerPrefs.GetString(email);
        
        // 비밀번호 검증 (틀리면 실패)
        if (!Crypto.VerifyPassword(password, savedHashedPassword, SALT))
        {
            return new UniTask<AccountResult>(new AccountResult
            {
                Success = false,
                ErrorMessage = "아이디와 비밀번호를 확인해주세요.",
            });
        }

        return new UniTask<AccountResult>(new AccountResult
        {
            Success = true,
            Account = new Account(email, savedHashedPassword),
        });
    }

    // 로그아웃
    public void Logout()
    {
        Debug.Log("로그아웃 되었습니다.");
    }
}
