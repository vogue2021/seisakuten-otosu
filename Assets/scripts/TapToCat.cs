using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

[RequireComponent(typeof(ARRaycastManager))]
public class TapToCat : MonoBehaviour
{
    // [SerializeField] private RectTransform rtcanvas;

    private ARRaycastManager arRaycastManager;
    private Vector2 touchPosition;
    private Sprite newSprite;
    private List<Sprite> kotoba = new List<Sprite>();
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private int wordcount = 0;

    private Touch touch;

    public float distanceInFrontOfCamera = 1.0f;

    private ARSessionOrigin arOrigin; //make this public and assign it in the inspector if this script is attacthed to another GO
    private ARAnchorManager anchorManager; // same as above, reference to ARAnchorManager
    private ARPlaneManager arPlaneManager = null;
    bool firstPlaneDetected = false;

    public  TMPro.TextMeshProUGUI debugText;

    void Start()
    {
        arOrigin = GetComponent<ARSessionOrigin>(); // get the ARSessionOrigin component
        anchorManager = GetComponent<ARAnchorManager>();

        Texture2D imagetexture = Resources.Load<Texture2D>("ganlanmao");
        newSprite = Sprite.Create(imagetexture, new Rect(0, 0, imagetexture.width, imagetexture.height), Vector2.one * 0.5f);

        Object[] loadedAssets = Resources.LoadAll("kotoba", typeof(Texture2D));
        
        // Loop through the loaded assets and convert them to Sprites
        for(int i = 0; i < loadedAssets.Length; i++)
        {
            Texture2D texture = loadedAssets[i] as Texture2D;
            if(texture != null) 
            {
                // Create a new Sprite using the Texture
                Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

                // Add the Sprite to the list
                kotoba.Add(sprite);
            }
        }
    }

    void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        arPlaneManager = GetComponent<ARPlaneManager>();
        arPlaneManager.planesChanged += PlanesChanged;
    }

    void PlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (!firstPlaneDetected && args.added != null && args.added.Count > 0)
        {
            debugText.text = ("Active");
            firstPlaneDetected = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            // Touch touch = Input.GetTouch(0);
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                PlaceOnAir(wordcount);
                // PlaceOnPlane();
                wordcount += 1;
            }
        }
    }

    private void PlaceOnAir(int num)
    {
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
        if (num < kotoba.Count)
        {
            cat.sprite = kotoba[num];
        }
        else 
        {
            cat.sprite = newSprite;

        }
        cat.rectTransform.localScale = new Vector3(-cat.sprite.bounds.size.x/1000f, cat.sprite.bounds.size.y/1000f, 1f);

        newCanvasObject.transform.parent = anchor.transform;
        // newCanvasObject.transform.SetParent(anchor.transform , false);

        Debug.Log("goodAnchor"+anchor.transform.position);
        Debug.Log("goodCanvas"+newCanvasObject.transform.position);
        Debug.Log("goodImage"+cat.rectTransform.position);
    }
}
