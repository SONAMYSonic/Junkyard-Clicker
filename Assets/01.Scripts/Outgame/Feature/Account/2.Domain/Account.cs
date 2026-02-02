using System;

// 계정 도메인 모델
// 계정의 핵심 데이터와 규칙을 담당
public class Account
{
    public readonly string Email;
    public readonly string Password;
    
    public Account(string email, string password)
    {
        // 이메일 검증 (Specification 패턴 사용)
        var emailSpec = new AccountEmailSpecification();
        if (!emailSpec.IsSatisfiedBy(email))
        {
            throw new ArgumentException(emailSpec.ErrorMessage);
        }
        
        // 비밀번호 검증
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("비밀번호는 비어있을 수 없습니다.");
        }
        
        if (password.Length < 6 || 15 < password.Length)
        {
            throw new ArgumentException("비밀번호는 6~15자 사이어야 합니다.");
        }
        
        Email = email;
        Password = password;
    }
}
