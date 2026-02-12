using System.Collections.Generic;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenAiApiTest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resultTextUI;
    [SerializeField] private TMP_InputField _promptTextField;   // 프롬프트: AI에 우리 요청사항을 담은 텍스트
    [SerializeField] private Button _sendButton;
    
    [SerializeField] private ApiKeyConfig _apiKeyConfig; // ScriptableObject로 키 분리
    
    // API 숨기기
    // 1. 환경 변수를 이용하는 방식
    // 2. gitignore에 추가
    // 3. 깃허브 시크릿 파일을 이용한 방식
    
    private async void Start()
    {
        // 1. ChatGPT 사이트에 API_KEY 로그인한다.
        var api = new OpenAIClient(_apiKeyConfig.OpenAIKey);
        
        // 2. 프롬프트를 작성한다
        var messages = new List<Message>
        {
            new Message(Role.User, "너는 누구니?"),
        };
        
        // 3. 모델을 선택하고, 요청을 보낸다.
        var chatRequest = new ChatRequest(messages, Model.GPT4oMini);
        
        // 4. 응답을 비동기로 받는다.
        var response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
        
        // 5. 답변이 여러개일 수 있으므로 첫번쩨를 선택한다. (디폴트: 1개)
        var choice = response.FirstChoice;
        
        // 6. 초기화
        _promptTextField.text = string.Empty;
        _resultTextUI.text = choice.Message;
    }
}
