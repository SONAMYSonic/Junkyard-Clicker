// 인증 결과를 담는 구조체
// 로그인/회원가입의 성공 여부와 에러메시지, 계정 정보를 담음
public struct AuthResult
{
    public bool Success;
    public string ErrorMessage;
    public Account Account;
}
