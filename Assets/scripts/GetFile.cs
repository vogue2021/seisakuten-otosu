using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GetFile : MonoBehaviour
{
    //APIのエンドポイントURL
    private string url = "{URL}";

    public void DownloadPngFile(string filePath)
    {
        StartCoroutine(GetData(filePath));
    }

    private IEnumerator GetData(string filePath)
    {
        //UnityWebRequestを作成し、ファイルをGetリクエストで送信
        UnityWebRequest request = UnityWebRequest.Get(url);

        //urlに接続してデータが帰ってくるまで待機状態にする
        yield return request.SendWebRequest();

        //エラー確認
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to download the file: " + request.error);
        }
        else
        {

            Debug.Log("File download completed");
            Debug.Log("Server response: " + request.downloadHandler.text);

            byte[] pngdata = request.downloadHandler.data;
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(pngdata);
        }
    }
}
