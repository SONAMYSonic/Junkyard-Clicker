using System;
using OpenAI;
using OpenAI.Images;
using OpenAI.Models;
using UnityEngine;
using UnityEngine.UI;

public class ImageNPC : MonoBehaviour
{
    [SerializeField] private ApiKeyConfig _apiKeyConfig;
    [SerializeField] private RawImage _displayImage;

    private async void Start()
    {
        if (_apiKeyConfig == null) return;
        if (_displayImage == null) return;
        
        string prompt = "Chevollet Corvette C8 Opened Top, driving, on road, beach background, sunny day, some clouds, realistic style";
        
        // 1. ChatGPT 사이트에 API_KEY 로그인한다.
        var api = new OpenAIClient(_apiKeyConfig.OpenAIKey);
        
        // 2. 이미지 생성 요청내용 작성
        var request = new ImageGenerationRequest(
            prompt: prompt,
            model: Model.GPT_Image_1
        );
        
        // 3. 요청을 보내고 응답을 받는다.
        var results = await api.ImagesEndPoint.GenerateImageAsync(request);
        _displayImage.texture = results[0].Texture;
    }
}
