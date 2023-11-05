// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.XR.ARFoundation;
// using UnityEngine.XR.ARSubsystems;
// using UnityEngine.XR.ARKit;
// using UnityEngine.UI;
// using System.IO;

// [RequireComponent(typeof(ARRaycastManager))]
// public class PlaceInARWorld : MonoBehaviour
// {
//     private ARRaycastManager arRaycastManager;
//     private Vector2 touchPosition;
//     private Sprite newSprite;
//     private List<Sprite> kotoba = new List<Sprite>();
//     static List<ARRaycastHit> hits = new List<ARRaycastHit>();
//     private ARSession m_ARSession;
//     private int wordcount = 0;
//     private Touch touch;
//     public float distanceInFrontOfCamera = 1.0f;
//     private ARSessionOrigin arOrigin;
//     private ARAnchorManager anchorManager;
//     private ARPlaneManager arPlaneManager = null;
//     bool firstPlaneDetected = false;
//     public TMPro.TextMeshProUGUI debugText;

//     void Start()
//     {
//         arOrigin = GetComponent<ARSessionOrigin>();
//         anchorManager = GetComponent<ARAnchorManager>();

//         Texture2D imagetexture = Resources.Load<Texture2D>("ganlanmao");
//         newSprite = Sprite.Create(imagetexture, new Rect(0, 0, imagetexture.width, imagetexture.height), Vector2.one * 0.5f);

//         Object[] loadedAssets = Resources.LoadAll("kotoba", typeof(Texture2D));

//         for(int i = 0; i < loadedAssets.Length; i++)
//         {
//             Texture2D texture = loadedAssets[i] as Texture2D;
//             if(texture != null) 
//             {
//                 Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
//                 kotoba.Add(sprite);
//             }
//         }

//         m_ARSession = GetComponent<ARSession>();
//         var subsystem = m_ARSession.subsystem as ARKitSessionSubsystem;
//         if (subsystem != null)
//         {
//             var worldMap = LoadARWorldMap();
//             if (worldMap != null)
//             {
//                 subsystem.ApplyWorldMap(worldMap);
//             }
//         }
//     }

//     void Awake()
//     {
//         arRaycastManager = GetComponent<ARRaycastManager>();
//         arPlaneManager = GetComponent<ARPlaneManager>();
//         arPlaneManager.planesChanged += PlanesChanged;
//     }

//     void PlanesChanged(ARPlanesChangedEventArgs args)
//     {
//         if (!firstPlaneDetected && args.added != null && args.added.Count > 0)
//         {
//             debugText.text = ("Active");
//             firstPlaneDetected = true;
//         }
//     }

//     void Update()
//     {
//         if (Input.touchCount > 0)
//         {
//             touch = Input.GetTouch(0);
//             if (touch.phase == TouchPhase.Began)
//             {
//                 PlaceOnAir(wordcount);
//                 wordcount += 1;
//             }
//         }
//     }

//     private void PlaceOnAir(int num)
//     {
//         Vector3 airPos = arOrigin.camera.transform.position + arOrigin.camera.transform.forward * distanceInFrontOfCamera;
//         Quaternion airQua = Quaternion.LookRotation(-arOrigin.camera.transform.forward, arOrigin.camera.transform.up);
//         var anchor = anchorManager.AddAnchor(new Pose(airPos, airQua));

//         GameObject newCanvasObject = new GameObject();
//         Canvas newCanvas =  newCanvasObject.AddComponent<Canvas>();
//         newCanvas.renderMode = RenderMode.WorldSpace;

//         GameObject imageObject = new GameObject();
//         imageObject.transform.SetParent(newCanvasObject.transform , false);
//         imageObject.transform.position = airPos;
//         imageObject.transform.rotation = airQua;

//         Image cat = imageObject.AddComponent<Image>();
//         if (num < kotoba.Count)
//         {
//             cat.sprite = kotoba[num];
//         }
//         else 
//         {
//             cat.sprite = newSprite;
//         }
//         cat.rectTransform.localScale = new Vector3(-cat.sprite.bounds.size.x/1000f, cat.sprite.bounds.size.y/1000f, 1f);

//         newCanvasObject.transform.parent = anchor.transform;
//     }

//     public void OnApplicationPause(bool pauseStatus)
//     {
//         if (pauseStatus)
//         {
//             var subsystem = m_ARSession.subsystem as ARKitSessionSubsystem;
//             if (subsystem != null)
//             {
//                 subsystem.GetARWorldMapAsync(OnWorldMapCompleted);
//             }
//         }
//     }

//     void OnWorldMapCompleted(ARWorldMapRequestStatus status, ARWorldMap worldMap)
//     {
//         if (status == ARWorldMapRequestStatus.Success)
//         {
//             SaveARWorldMap(worldMap);
//         }
//         else
//         {
//             Debug.Log("Failed to serialize ARWorldMap");
//         }
//     }

//     static string path => Path.Combine(Application.persistentDataPath, "arWorldMap");

//     void SaveARWorldMap(ARWorldMap worldMap)
//     {
//         var serializedWorldMap = worldMap.Serialize(ARWorldMap);
//         File.WriteAllBytes(path, serializedWorldMap);
//     }


//     ARWorldMap LoadARWorldMap()
//     {
//         if (File.Exists(path))
//         {
//             var serializedWorldMap = File.ReadAllBytes(path);
//             var worldMap = new ARWorldMap();
//             if (worldMap.TryDeserialize(serializedWorldMap))
//             {
//                 return worldMap;
//             }
//         }
//         Debug.Log("Failed to deserialize ARWorldMap");
//         return null;
//     }

// }
