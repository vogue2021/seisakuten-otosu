// using System.Collections;
// using System.IO;
// using Unity.Collections;
// using UnityEngine;
// using UnityEngine.XR.ARFoundation;
// using UnityEngine.XR.ARKit;
// using UnityEngine.XR.ARSubsystems;

// public class PlaceInWorldMap : MonoBehaviour
// {

// // need to be Platform Specific!


//     private ARAnchorManager anchorManager;
//     private ARSession arSession;
//     private ARKitSessionSubsystem arKitSessionSubsystem;

//     private void Awake()
//     {
//         arSession = FindObjectOfType<ARSession>();
//         anchorManager = GetComponent<ARAnchorManager>();

//         if (arSession.subsystem is ARKitSessionSubsystem)
//         {
//             arKitSessionSubsystem = (ARKitSessionSubsystem)arSession.subsystem;
//         }
//         else
//         {
//             Debug.LogError("ARWorldMap is not supported on this platform.");
//         }
//     }

//     public void SaveARWorldMap()
//     {
//         if (arKitSessionSubsystem != null)
//         {
//             arKitSessionSubsystem.GetARWorldMapAsync(OnWorldMapReceived);
//         }
//     }

//     private void OnWorldMapReceived(ARWorldMapRequestStatus status, ARWorldMap worldMap)
//     {
//         if (status == ARWorldMapRequestStatus.Success)
//         {
//             byte[] worldMapData = worldMap.Serialize(Allocator.Temp).ToArray();
//             string path = Path.Combine(Application.persistentDataPath, "ARWorldMap");
//             File.WriteAllBytes(path, worldMapData);
//             worldMap.Dispose();
//         }
//         else
//         {
//             Debug.LogError($"Failed to receive ARWorldMap. Reason: {status}");
//         }
//     }


//     // public void LoadARWorldMap()
//     // {
//     //     string path = Path.Combine(Application.persistentDataPath, "ARWorldMap");
//     //     if (File.Exists(path))
//     //     {
//     //         byte[] worldMapData = File.ReadAllBytes(path);
//     //         var worldMap = ARWorldMap.Deserialize(worldMapData);
//     //         if (arKitSessionSubsystem != null)
//     //         {
//     //             arKitSessionSubsystem.ApplyWorldMap(worldMap);
//     //         }
//     //     }
//     //     else
//     //     {
//     //         Debug.LogError("No ARWorldMap found at path: " + path);
//     //     }
//     // }

//     private void OnDisable()
//     {
//         SaveARWorldMap();
//     }

//     private void OnEnable()
//     {
//         // LoadARWorldMap();
//     }
// }
