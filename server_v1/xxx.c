using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class VoiceToImage : NetworkBehaviour
{
    private string apiUrl = "http://your-flask-server.com/upload";

    [SyncVar]
    private string imageUrl;

    [SyncVar]
    private Vector3 imagePosition;

    void Start()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Begin recording from the default microphone
            Microphone.Start(null, false, 10, 44100);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            // End recording
            Microphone.End(null);
            
            // Get the audio clip
            AudioClip clip = Microphone.GetAudioClip();

            // Convert the audio clip to wav format
            byte[] audioBytes = ConvertAudioClipToWav(clip);

            // Upload the audio file
            StartCoroutine(UploadAudio(audioBytes));
        }
    }

    IEnumerator UploadAudio(byte[] audioBytes)
    {
        // Create a form and add the audio bytes to it
        WWWForm form = new WWWForm();
        form.AddBinaryData("audio", audioBytes, "audio.wav", "audio/wav");

        // Send the POST request
        UnityWebRequest www = UnityWebRequest.Post(apiUrl, form);
        yield return www.SendWebRequest();

        // Check for errors
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Get the image filename from the response
            imageUrl = JsonUtility.FromJson<UploadResponse>(www.downloadHandler.text).image_filename;

            // Set the position for the image
            imagePosition = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
            
            // Call the command on the server to update the image url and position for all clients
            CmdUpdateImageUrlAndPosition(imageUrl, imagePosition);
        }
    }

    [Command]
    void CmdUpdateImageUrlAndPosition(string url, Vector3 position)
    {
        // Update the image url and position
        imageUrl = url;
        imagePosition = position;

        // Call the RPC on all clients to update the image
        RpcUpdateImage();
    }

    [ClientRpc]
    void RpcUpdateImage()
    {
        // Get the image texture
        StartCoroutine(GetImageTexture(imageUrl));
    }

    IEnumerator GetImageTexture(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Get the image texture
            Texture2D texture = DownloadHandlerTexture.GetContent(www);

            // Create a new GameObject to display the image
            GameObject imageObject = new GameObject();
            imageObject.transform.position = imagePosition;

            // Add a SpriteRenderer component and set the texture
            SpriteRenderer renderer = imageObject.AddComponent<SpriteRenderer>();
            renderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}

public class UploadResponse
{
    public string audio_filename;
    public string image_filename;
}
