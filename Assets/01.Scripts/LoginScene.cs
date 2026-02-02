using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginScene : MonoBehaviour
{
    // 화면 모드 (로그인 / 회원가입)
    private enum SceneMode
    {
        Login,
        Register
    }
    
    private SceneMode _mode = SceneMode.Login;
    
    // UI 요소들 (Inspector에서 연결)
    [SerializeField] private GameObject _passwordConfirmObject;
    [SerializeField] private Button _gotoRegisterButton;
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _gotoLoginButton;
    [SerializeField] private Button _registerButton;

    [SerializeField] private TextMeshProUGUI _messageTextUI;
    
    [SerializeField] private TMP_InputField _emailInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [SerializeField] private TMP_InputField _passwordConfirmInputField;
    
    private void Start()
    {
        AddButtonEvents();
        Refresh();
    }

    // 버튼 이벤트 연결
    private void AddButtonEvents()
    {
        _gotoRegisterButton.onClick.AddListener(GotoRegister);
        _loginButton.onClick.AddListener(Login);
        _gotoLoginButton.onClick.AddListener(GotoLogin);
        _registerButton.onClick.AddListener(Register);
    }

    // 화면 모드에 따라 UI 표시/숨김
    private void Refresh()
    {
        _passwordConfirmObject.SetActive(_mode == SceneMode.Register);
        _gotoRegisterButton.gameObject.SetActive(_mode == SceneMode.Login);
        _loginButton.gameObject.SetActive(_mode == SceneMode.Login);
        _gotoLoginButton.gameObject.SetActive(_mode == SceneMode.Register);
        _registerButton.gameObject.SetActive(_mode == SceneMode.Register);
    }

    // 실시간 이메일 검증 (InputField의 OnValueChanged에 연결)
    public void OnEmailTextChanged(string email)
    {
        var emailSpec = new AccountEmailSpecification();
        if (!emailSpec.IsSatisfiedBy(email))
        {
            _messageTextUI.text = emailSpec.ErrorMessage;
            _loginButton.interactable = false;
            return;
        }
        
        _messageTextUI.text = "올바른 이메일 형식입니다.";
        _loginButton.interactable = true;
    }
    
    // 로그인 실행
    private void Login()
    {
        string email = _emailInputField.text;
        string password = _passwordInputField.text;
        
        var result = AccountManager.Instance.TryLogin(email, password);
        if (result.Success)
        {
            _messageTextUI.text = "로그인 성공!";
            // TODO: 게임 씬으로 이동
            // SceneManager.LoadScene("GameScene");
        }
        else
        {
            _messageTextUI.text = result.ErrorMessage;
        }
    }

    // 회원가입 실행
    private void Register()
    {
        string email = _emailInputField.text;
        string password = _passwordInputField.text;
        string passwordConfirm = _passwordConfirmInputField.text;
        
        // 비밀번호 확인 일치 검사
        if (string.IsNullOrEmpty(passwordConfirm) || password != passwordConfirm)
        {
            _messageTextUI.text = "비밀번호를 확인해주세요.";
            return;
        }

        var result = AccountManager.Instance.TryRegister(email, password);
        if (result.Success)
        {
            _messageTextUI.text = "회원가입 성공! 로그인해주세요.";
            GotoLogin();
        }
        else
        {
            _messageTextUI.text = result.ErrorMessage;
        }
    }

    // 로그인 화면으로 전환
    private void GotoLogin()
    {
        _mode = SceneMode.Login;
        Refresh();
    }

    // 회원가입 화면으로 전환
    private void GotoRegister()
    {
        _mode = SceneMode.Register;
        Refresh();
    }
}
