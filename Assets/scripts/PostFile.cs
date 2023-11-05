// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Networking;

// public class PostFile : MonoBehaviour
// {
//     // private string url = "http://10.100.176.125:5000/upload";
//     // private string url = "http://192.168.100.137:5050/upload";
//     private string url = "http://10.100.5.53:6789/upload";
    
//     // private string url = "http://192.168.1.153:6789/upload"; 
//     public string selectedDevice;

//     public void UploadWavFile(string filePath)
//     {
//         StartCoroutine(SendRequest(filePath));
//     }

//     private IEnumerator SendRequest(string filePath)
//     {
//         byte[] fileData = System.IO.File.ReadAllBytes(filePath);
        
//         //Create a new WWWForm, to hold the form data.
//         WWWForm form = new WWWForm();
        
//         //Add binary data to the form. Key is 'audio'
//         // form.AddBinaryData("audio", fileData, filePath, "audio/wav");
//         form.AddBinaryData("audio", fileData,  selectedDevice + ".wav", "audio/wav");
        
//         using (UnityWebRequest request = UnityWebRequest.Post(url, form))
//         {
//             yield return request.SendWebRequest();

//             if(request.result != UnityWebRequest.Result.Success)
//             {
//                 Debug.LogError("Failed to upload the file: " + request.error);
//             }
//             else
//             {
//                 Debug.Log("File upload completed");
//                 Debug.Log("Server response: " + request.downloadHandler.text);
//             }
//         }
//     }
// }




using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PostFile : MonoBehaviour
{
    // public PlaceText placeText;
    public string selectedDevice;
    
    // private string url = "http://10.100.20.78:6789/upload"; 
    // private string url = "http://10.100.20.78:6788/upload"; 
    private string url = "http://192.168.100.170:6789/upload"; 
    // void start()
    // {
    //     string selectedDevice = placeText.selectedDevice;
    //     Debug.Log(selectedDevice+".wav");
    // }


    public void UploadWavFile(string filePath)
    {
        StartCoroutine(SendRequest(filePath));
    }

    private IEnumerator SendRequest(string filePath)
    {
        byte[] fileData = System.IO.File.ReadAllBytes(filePath);
        
        WWWForm form = new WWWForm();
        
        // form.AddBinaryData("audioA", fileData, "No1audio.wav", "audio/wav");//hanlin iphone14

        form.AddBinaryData("audioA", fileData, selectedDevice + ".wav", "audio/wav");//hanlin iPad
        
        form.AddBinaryData("audioB", fileData, filePath, "audio/wav");

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();

            if(request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to upload the files: " + request.error);
            }
            else
            {
                Debug.Log("File upload completed");
                Debug.Log("Server response: " + request.downloadHandler.text);
            }
        }
    }
}

