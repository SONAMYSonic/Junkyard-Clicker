using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class WebGetImageText : MonoBehaviour
{
    public RawImage MyImage;
    
    private async void Start()
    {
        StartCoroutine(GetTexture());
        
        MyImage.texture = await GetWebTexture("https://placecats.com/bella/300/200?fit=contain&position=top");
    }
    
    private async UniTask<Texture> GetWebTexture(string url)
    {
        var texture = Texture2D.blackTexture;
        try
        {
            texture = ((DownloadHandlerTexture)(await UnityWebRequestTexture.GetTexture(url).SendWebRequest()).downloadHandler).texture;

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        return texture;
    }
 
    IEnumerator GetTexture()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://www.bbc.com/korean/articles/cn42dkdq1lwo");
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
    }
}
