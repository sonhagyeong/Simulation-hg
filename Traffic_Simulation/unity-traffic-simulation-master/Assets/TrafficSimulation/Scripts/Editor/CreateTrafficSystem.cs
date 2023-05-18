using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TrafficSimulation {
    public class CreateTrafficSystem : EditorWindow
    {   
        private static string routeFilePath = "C:\\Users\\USER\\workspace\\RoutePoints.csv";
        private static string intersectionFilePath = "C:\\Users\\USER\\workspace\\IntersectionPoints.csv";

        private static List<List<Vector3>> routes = new List<List<Vector3>>();
        private static List<Vector3> intersections = new List<Vector3>();
        
        [InitializeOnLoadMethod]
        private static void PlayModeState()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                CreateSystem();
            }
        }

        private static void CreateSystem()
        {
            if(GameObject.FindObjectOfType<TrafficSystem>() == null)
            {   
                Debug.Log("Creating Traffic System");
                FileWindow.CreateAll(routeFilePath, intersectionFilePath, routes, intersections);
            }

            else
            {
                Debug.Log("Traffic System already exists");
            }
        }
        
        
    }
}