using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ImageDisplay : MonoBehaviour
{
    public RawImage rawImage;
    public Image bobImage;
    private Sprite newSprite;

    private string imageURL = "http://10.100.176.125:5000/images/"; // 服务器图片的URL

    public void Bob()
    {
        // 替换为服务器处理后返回的图像文件名
        string imageFilename = "processed_image.png";

        // 向服务器请求图像
        StartCoroutine(LoadImage(imageURL + imageFilename));
    }

    private IEnumerator LoadImage(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // 从请求中获取纹理
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 0.0001f);

                // 在RawImage组件中显示纹理
                // rawImage.texture = texture;
                bobImage.sprite = newSprite;
                
            
            }
            else
            {
                Debug.Log("无法加载图像：" + www.error);
            }
        }
    }
}