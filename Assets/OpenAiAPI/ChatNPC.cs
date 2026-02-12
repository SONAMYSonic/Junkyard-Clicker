using System.Collections.Generic;
using OpenAI;
using OpenAI.Audio;
using OpenAI.Chat;
using OpenAI.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatNPC : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resultTextUI;
    [SerializeField] private TMP_InputField _promptTextField;   // 프롬프트: AI에 우리 요청사항을 담은 텍스트
    [SerializeField] private Button _sendButton;
    [SerializeField] private AudioSource _audioSource;
    
    // 대화 내용을 기억할 콘텍스트
    // 메세지는 누적된다. (Request 보낼때마다 토큰이 가히 급수적으로 증가)
    private List<Message> _messages = new List<Message>();
    // 현업에서는 토큰량을 줄이기 위해 여러 가지 기법을 써야 한다. (AI 엔지니어의 역할)
    // - 일정 개수가 넘으면 과거의 기억을 지워가는 방식 (최근 N개만 기억)
    // -       ~         압축하는 방식 (기존 내용 요약 등)
    // - '벡터 DB' (데이터를 벡터화해서) 저장해두고 검색하는 방식 (RAG) 
    
    [SerializeField] private ApiKeyConfig _apiKeyConfig; // ScriptableObject로 키 분리
    
    
    // API 숨기기
    // 1. 환경 변수를 이용하는 방식
    // 2. gitignore에 추가
    // 3. 깃허브 시크릿 파일을 이용한 방식
    
    private void Start()
    {
        // NPC 모드 지침 추가 (역할, 목적, 표현)
        string systemMessage = string.Empty;
        systemMessage += "역할: 너는 아이돌마스터 샤이니 컬러즈에 등장하는 세리자와 아사히야.";
        systemMessage += "목적: 사용자를 프로듀서라고 생각하는 14살 천진난만 아이돌 세리자와 아사히.";
        systemMessage += "표현: 어미의 끝에 ~임다, ~슴다 등 ~임다체를 써야 한다. ~요 대신 ~임다! 가능하면 아하하~ 하는 밝은 성격. 항상 500글자 이내로 답변한다.";
        
        _messages.Add(new Message(Role.Assistant, systemMessage));
        
        // 버튼 클릭 이벤트
        _sendButton.onClick.AddListener(Send);
    }

    private async void Send()
    {
        string prompt = _promptTextField.text;
        if (string.IsNullOrEmpty(prompt))
        {
            return;
        }
        
        // 0. 버튼을 잠근다.
        _sendButton.interactable = false;
        
        // 1. ChatGPT 사이트에 API_KEY 로그인한다.
        var api = new OpenAIClient(_apiKeyConfig.OpenAIKey);
        
        // 2. 프롬프트를 작성해서 콘텍스트에 담는다
        // 역할: 지침(시스템 메세지), 유저가 쓴 메세지, LLM이 응답한 메세지
        _messages.Add(new Message(Role.User, prompt));
        
        // 3. 모델을 선택하고, 요청을 보낸다.
        var chatRequest = new ChatRequest(_messages, Model.GPT4oMini);
        
        // 4. 응답을 비동기로 받는다.
        var response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
        
        // 5. 답변이 여러개일 수 있으므로 첫번쩨를 선택한다. (디폴트: 1개)
        var choice = response.FirstChoice;
        
        // 6. 응답을 콘텍스트에 담는다.
        _messages.Add(new Message(Role.Assistant, choice.Message));
        
        // 결과값을 UI에 출력한다.
        _resultTextUI.text = choice.Message;
        
        // 7. TTS (Text To Speech)
        // 실시간 TTS가 필요하다면.. 한국 성우가 많은 타입캐스트 API이용을 권장
        var request = new SpeechRequest(
            input:choice.Message,
            model: Model.TTS_GPT_4o_Mini,
            voice: Voice.Coral
            );
        var speechClip = await api.AudioEndpoint.GetSpeechAsync(request);
        _audioSource.PlayOneShot(speechClip);
        
        // 초기화
        _promptTextField.text = string.Empty;
        _sendButton.interactable = true;
    }
}
