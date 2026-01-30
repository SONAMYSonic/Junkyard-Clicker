using UnityEngine;

public class Account : MonoBehaviour
{
    // 이메일
    // 비밀번호
    public string Email;
    public string Password;
    
    // 이메일 규칙:
    // 0. 비어있으면 안 된다.
    // 1. 올바른 이메일 이어야 한다.
    // 2. 동일한 이메일이면 중복 안된다...
    
    // 비밀번호 규칙
    // 0. 비어있으면 안 된다.
    // 1. 6자리 이상 12자 이하 (대문자 1개이상 포함, 특수문자 1개 이상 포함)
}
