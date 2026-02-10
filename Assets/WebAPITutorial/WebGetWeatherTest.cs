using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class WebGetWeatherTest : MonoBehaviour
{
    // 목표: 서울의 날씨를 받아오자
    private const string API_KEY = "22d91bbbf74d7ea1b86132398d319b4f";
    
    private async void Start()
    {
        float lat = 37.4046984f;
        float lon = 127.1059515f;
        string url = 
            $"https://api.openweathermap.org/data/2.5/onecall?lat={lat}&lon={lon}&appid={API_KEY}";
        Debug.Log(url);
        
        string result = await GetWebText(url);
        Debug.Log(result);
    }

    private async UniTask<string> GetWebText(string url)
    {
        var txt = (await UnityWebRequest.Get(url).SendWebRequest()).downloadHandler.text;
        return txt;
    }
}
