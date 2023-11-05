using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using System.Collections;

public class PlaceText : MonoBehaviour
{
    private ARRaycastManager arRaycastManager;
    private ARSessionOrigin arOrigin;
    private ARAnchorManager anchorManager;
    private ARPlaneManager arPlaneManager = null;

    bool firstPlaneDetected = false;

    public TMPro.TextMeshProUGUI debugText;
    public float distanceInFrontOfCamera = 0.5f;
    public float Delay;

    public string selectedDevice;
    
    private Sprite newSprite;
    // private string imageURL = "http://10.100.176.125:5000/images/"; // commons arjen
    // private string imageURL = "http://192.168.100.137:5050/images/"; // inami2.4
    // private string imageURL = "http://10.100.5.53:6789/images/"; // commons hanlin
    // private string imageURL = "http://10.100.20.78:6789/images/"; // commons hanlin
    // private string imageURL = "http://10.100.20.78:6788/images/"; // commons hanlin
    private string imageURL = "http://192.168.100.170:6789/images/";


    // private string imageURL = "http://192.168.1.153:6789/images/"; // 家G

    void Start()
    {
        arOrigin = GetComponent<ARSessionOrigin>(); // get the ARSessionOrigin component
        anchorManager = GetComponent<ARAnchorManager>();

        // Texture2D imagetexture = Resources.Load<Texture2D>("earth");
        // newSprite = Sprite.Create(imagetexture, new Rect(0, 0, imagetexture.width, imagetexture.height), Vector2.one * 0.5f,100.0f);
    }


    void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        arPlaneManager = GetComponent<ARPlaneManager>();
        arPlaneManager.planesChanged += PlanesChanged;
    }

    private void PlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (!firstPlaneDetected && args.added != null && args.added.Count > 0)
        {
            // debugText.text = "Active";
            debugText.text = "";
            firstPlaneDetected = true;
        }
    }

    private IEnumerator LoadImage(string url, float delay)
    {
        yield return new WaitForSeconds(delay);
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                // Debug.Log("New sprite created. Size: " + newSprite.rect.size);
            }
            else
            {
                Debug.Log("Unable to load image: " + www.error);
            }
        }
        
        while (newSprite == null)
        {
            yield return null;  // wait for the next frame
        }

        Vector3 airPos = arOrigin.camera.transform.position + arOrigin.camera.transform.forward * distanceInFrontOfCamera;
        Quaternion airQua = Quaternion.LookRotation(-arOrigin.camera.transform.forward, arOrigin.camera.transform.up);
        var anchor = anchorManager.AddAnchor(new Pose(airPos, airQua));

        GameObject newCanvasObject = new GameObject();
        Canvas newCanvas =  newCanvasObject.AddComponent<Canvas>();
        newCanvas.renderMode = RenderMode.WorldSpace;

        GameObject imageObject = new GameObject();
        imageObject.transform.SetParent(newCanvasObject.transform , false);
        imageObject.transform.position = airPos;
        imageObject.transform.rotation = airQua;

        Image cat = imageObject.AddComponent<Image>();
        cat.sprite = newSprite;
        cat.rectTransform.localScale = new Vector3(-cat.sprite.bounds.size.x/1000f, cat.sprite.bounds.size.y/1000f, 1f);

        newCanvasObject.transform.parent = anchor.transform;

        // Deleting the image from the server
        using (UnityWebRequest deleteRequest = new UnityWebRequest(url, "DELETE"))
        {
            yield return deleteRequest.SendWebRequest();

            if (deleteRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Image successfully deleted from server.");
            }
            else
            {
                Debug.Log("Error deleting image from server: " + deleteRequest.error);
            }
        }
        
        newSprite = null;

        // newCanvasObject.transform.SetParent(anchor.transform , false);

        // Debug.Log("catAnchor"+anchor.transform.position);
        // Debug.Log("catCanvas"+newCanvasObject.transform.position);
        // Debug.Log("catImage"+cat.rectTransform.position);

        // yield break;
    }


    public void PlaceOnAir()
    {
        // string imageFilename = "processed_image.png";
        // string imageFilename = "No1audio.png"; //hanlin iphone14
        string imageFilename = selectedDevice + ".png"; //hanlin ipad
        StartCoroutine(LoadImage(imageURL + imageFilename, Delay));
    }

}
