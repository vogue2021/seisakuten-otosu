using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class PlaceImageInAR : MonoBehaviour
{
    public ARRaycastManager arRaycastManager;
    public float distanceInFrontOfCamera = 0.5f;
    public ARSessionOrigin arOrigin;
    public ARAnchorManager anchorManager;
    public ARPlaneManager arPlaneManager = null;
    bool firstPlaneDetected = false;
    public TextMeshProUGUI debugText;

    private Sprite newSprite;

    private string imageURL = "http://10.100.176.125:5000/images/"; // 服务器图片的URL

    // void Start()
    // {
    //     Texture2D imagetexture = Resources.Load<Texture2D>("ganlanmao");
    //     newSprite = Sprite.Create(imagetexture, new Rect(0, 0, imagetexture.width, imagetexture.height), Vector2.one * 0.5f);
    // }

    void Awake()
    {
        arPlaneManager.planesChanged += PlanesChanged;
    }

    private void PlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (!firstPlaneDetected && args.added != null && args.added.Count > 0)
        {
            debugText.text = "Active";
            firstPlaneDetected = true;
        }
    }

    private IEnumerator LoadImage(string url)
    {   
        Texture2D imagetexture = Resources.Load<Texture2D>("earth");
        newSprite = Sprite.Create(imagetexture, new Rect(0, 0, imagetexture.width, imagetexture.height), Vector2.one * 0.5f);
        if(newSprite != null) 
        {
            Debug.Log("cat is ready");
        }
        Vector3 airPos = arOrigin.camera.transform.position + arOrigin.camera.transform.forward * distanceInFrontOfCamera;
        Quaternion airQua = Quaternion.LookRotation(-arOrigin.camera.transform.forward, arOrigin.camera.transform.up);
        var anchor = anchorManager.AddAnchor(new Pose(airPos, airQua));
                
        GameObject newCanvasObject = new GameObject("Canvas");
        Canvas newCanvas = newCanvasObject.AddComponent<Canvas>();
        newCanvas.renderMode = RenderMode.WorldSpace;

        GameObject imageObject = new GameObject("Image");
        imageObject.transform.SetParent(newCanvasObject.transform, false);
        imageObject.transform.position = airPos;
        imageObject.transform.rotation = airQua;

        Image image = imageObject.AddComponent<Image>();
        image.sprite = newSprite;
        image.rectTransform.localScale = new Vector3(-image.sprite.bounds.size.x, image.sprite.bounds.size.y , 1f);

        newCanvasObject.transform.parent = anchor.transform;

        Debug.Log("New sprite created. Size: " + newSprite.rect.size);
        Debug.Log("output Size: " + image.rectTransform.localScale);

        Debug.Log("Anchor"+anchor.transform.position);
        Debug.Log("Canvas"+newCanvasObject.transform.position);
        Debug.Log("ImageObject"+imageObject.transform.position);
        Debug.Log("image"+image.rectTransform.position);
        

        yield break;
        // using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        // {
        //     yield return www.SendWebRequest();

        //     if (www.result == UnityWebRequest.Result.Success)
        //     {
        //         Texture2D texture = DownloadHandlerTexture.GetContent(www);
        //         // newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1f);
        //         Debug.Log("New sprite created. Size: " + newSprite.rect.size);
        //         Vector3 airPos = arOrigin.camera.transform.position + arOrigin.camera.transform.forward * distanceInFrontOfCamera;
        //         Quaternion airQua = Quaternion.LookRotation(-arOrigin.camera.transform.forward, arOrigin.camera.transform.up);

        //         var anchor = anchorManager.AddAnchor(new Pose(airPos, airQua));

        //         // GameObject anchorGO = new GameObject("ARAnchor");
        //         // anchorGO.transform.position = airPos;
        //         // anchorGO.transform.rotation = airQua;
        //         // anchorGO.AddComponent<ARAnchor>();
                
        //         GameObject newCanvasObject = new GameObject("Canvas");
        //         // newCanvasObject.transform.SetParent(anchor.transform, false);

        //         Canvas newCanvas = newCanvasObject.AddComponent<Canvas>();
        //         newCanvas.renderMode = RenderMode.WorldSpace;

        //         GameObject imageObject = new GameObject("Image");
        //         imageObject.transform.SetParent(newCanvasObject.transform, false);
        //         imageObject.transform.position = airPos;
        //         imageObject.transform.rotation = airQua;

        //         Image image = imageObject.AddComponent<Image>();
        //         image.sprite = newSprite;
        //         image.rectTransform.localScale = new Vector3(-image.sprite.bounds.size.x, image.sprite.bounds.size.y , 1f);

        //         newCanvasObject.transform.parent = anchor.transform;

        //         Debug.Log("New sprite created. Size: " + newSprite.rect.size);
        //         Debug.Log("output Size: " + image.rectTransform.localScale);

        //         Debug.Log("Anchor"+anchor.transform.position);
        //         Debug.Log("Canvas"+newCanvasObject.transform.position);
        //         Debug.Log("ImageObject"+imageObject.transform.position);
        //         Debug.Log("image"+image.rectTransform.position);
        //         PlaceImageOnCanvas();
        //     }
        //     else
        //     {
        //         Debug.Log("Unable to load image: " + www.error);
        //     }
        // }
    }

    public void PlaceImageInARSpace(string imageFilename)
    {
        StartCoroutine(LoadImage(imageURL + imageFilename));
    }

    // private void PlaceImageOnCanvas()
    // {
    //     Vector3 airPos = arOrigin.camera.transform.position + arOrigin.camera.transform.forward * distanceInFrontOfCamera;
    //     Quaternion airQua = Quaternion.LookRotation(-arOrigin.camera.transform.forward, arOrigin.camera.transform.up);

    //     GameObject anchorGO = new GameObject("ARAnchor");
    //     anchorGO.transform.position = airPos;
    //     anchorGO.transform.rotation = airQua;
    //     anchorGO.AddComponent<ARAnchor>();
        
    //     GameObject newCanvasObject = new GameObject("Canvas");
    //     newCanvasObject.transform.SetParent(anchorGO.transform, false);

    //     Canvas newCanvas = newCanvasObject.AddComponent<Canvas>();
    //     newCanvas.renderMode = RenderMode.WorldSpace;

    //     GameObject imageObject = new GameObject("Image");
    //     imageObject.transform.SetParent(newCanvasObject.transform, false);
    //     imageObject.transform.position = airPos;
    //     imageObject.transform.rotation = airQua;

    //     Image image = imageObject.AddComponent<Image>();
    //     image.sprite = newSprite;
    //     image.rectTransform.localScale = new Vector3(-image.sprite.bounds.size.x * 1000f, image.sprite.bounds.size.y * 1000f, 1f);

    //     Debug.Log("New sprite created. Size: " + newSprite.rect.size);
    //     Debug.Log("output Size: " + image.rectTransform.localScale);

    //     Debug.Log("Anchor"+anchorGO.transform.position);
    //     Debug.Log("Canvas"+newCanvasObject.transform.position);
    //     Debug.Log("ImageObject"+imageObject.transform.position);
    //     Debug.Log("image"+image.rectTransform.position);
    // }
}
