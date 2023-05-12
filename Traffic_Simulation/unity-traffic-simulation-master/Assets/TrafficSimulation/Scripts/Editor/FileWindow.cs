using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
namespace TrafficSimulation {    
    public class FileWindow : EditorWindow
    {   
        private string routefilePath;
        private string intersectionfilePath;

        // private List<List<Vector3>> routes = new List<List<Vector3>>();
        // List<Vector3> route = new List<Vector3>();
        private List<List<Vector3>> routes = new List<List<Vector3>>();

        private static TrafficSystem wps;
        private static float route_Pos_y = 1.5f;
        private Vector2 scrollPosition;

        private System.Collections.Generic.List<Vector3> intersections = new System.Collections.Generic.List<Vector3>();
        private static Vector3 intersectionSize = new Vector3(5,5,5);
        private static float intersectionPos_y = intersectionSize.y/2;


        
        [MenuItem("Component/Traffic Simulation/File Window")]
        public static void ShowWindow()
        {
            // EditorWindow.GetWindow(typeof(FileWindow));
            FileWindow window = EditorWindow.GetWindow<FileWindow>();
            window.titleContent = new GUIContent("Input File");
            window.Show();
        }

        private void OnGUI()
        {   
            EditorGUILayout.Space();
            if(GUILayout.Button("Open Route Data File"))
            {
                routefilePath = EditorUtility.OpenFilePanel("Select File", "", "");
                if(routefilePath != "")
                {
                    Debug.Log("Selected file path: " + routefilePath);
                }
            }

            GUILayout.Label("File Path: " + routefilePath);

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();


            if(GUILayout.Button("Create Routes List"))
            {
                List<List<Vector3>> now_routes = CreateRouteList(routefilePath, routes);

                foreach(List<Vector3> route in routes)
                {
                    // Show the points of the route
                    foreach(Vector3 point in route)
                    {
                        Debug.Log("Route " + routes.IndexOf(route) + " Point " + point);
                    }
                }
            }

            GUILayout.Label("Route Count : " + routes.Count);

            GUILayout.Space(5);

            if(GUILayout.Button("Reset route"))
            {
                routes.Clear();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);


            if(GUILayout.Button("Open Intersections Data File"))
            {
                intersectionfilePath = EditorUtility.OpenFilePanel("Select File", "", "");
                if(intersectionfilePath != "")
                {
                    Debug.Log("Selected file path: " + intersectionfilePath);
                }
            }


            // Create the buttons
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create Routes in Scene", GUILayout.Width(150)))
            {
                CreateRoutes(routes);
            }
            
            GUILayout.Space(10);

            if (GUILayout.Button("Create Intersections", GUILayout.Width(150)))
            {
                // CreateIntersections(intersections);
            }
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);

            GUILayout.BeginVertical();
            if (GUILayout.Button("Create All"))
            {
                // CreateAll(routes, intersections);
                Close();
            }
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
        }

        //Create a list by route
        private List<List<Vector3>> CreateRouteList(string routefilePath, List<List<Vector3>> routes)
        {   
            if (!File.Exists(routefilePath))
            {
                Debug.LogError("File does not exist: " + routefilePath);
                return null;
            }

            using (StreamReader reader = new StreamReader(routefilePath))
            {   
                // Skip the first line
                reader.ReadLine(); 

                string line = reader.ReadLine();
                string[] fields;

                int currentRoute = -1;
                
                while(line != null)
                {
                    fields = line.Split(',');

                    if(fields.Length == 4)
                    {
                        int routeNum = int.Parse(fields[3]);

                        if(routeNum != currentRoute)
                        {
                            currentRoute = routeNum;
                            routes.Add(new List<Vector3>());
                        }

                        float x = float.Parse(fields[0]);
                        float y = float.Parse(fields[1]);
                        float z = float.Parse(fields[2]);

                        Vector3 point = new Vector3(x, y, z);                        
                        routes[currentRoute].Add(point);
                        // Debug.Log("currentRoute : "+ currentRoute + " point : " + point);
                    }

                    line = reader.ReadLine();
                }
            }

            return routes;
        }
        
        private static void CreateAll(List<List<Vector3>> routes, List<Vector3> intersections)
        {   
            if(routes.Count > 0 & intersections.Count > 0)
            {
                CreateIntersections(intersections);
                CreateRoutes(routes);
                
            }
        }

        private static void CreateRoutes(List<List<Vector3>> routes)
        {
            EditorHelper.SetUndoGroup("Create Routes");

            for(int i=0; i<routes.Count; i++)
            {   
                // string routeName = routeNames[i];
                List<Vector3> route = routes[i];
                string routeName = "Route-" + i;

                GameObject mainGo = EditorHelper.CreateGameObject(routeName);
                mainGo.transform.position = Vector3.zero;
                EditorHelper.AddComponent<TrafficSystem>(mainGo);

                GameObject segmentsGo = EditorHelper.CreateGameObject("Segments", mainGo.transform);
                segmentsGo.transform.position = Vector3.zero;

                Selection.activeGameObject = mainGo;
                wps = Selection.activeGameObject.GetComponent<TrafficSystem>();

                EditorHelper.BeginUndoGroup("Add Segment", wps);
                AddSegment(route[0], routeName);

                foreach(Vector3 point in route)
                {   
                    Vector3 newPoint = point;
                    newPoint.y = route_Pos_y;

                    AddWaypoint(newPoint);
                }

            }
            //Close Undo Operation
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        private static void CreateIntersections(List<Vector3> intersections)
        {
            EditorHelper.SetUndoGroup("Create Intersection System");
            
            GameObject mainGo = EditorHelper.CreateGameObject("Intersection System");
            mainGo.transform.position = Vector3.zero;
            EditorHelper.AddComponent<TrafficSystem>(mainGo);

            GameObject intersectionsGo = EditorHelper.CreateGameObject("Intersections", mainGo.transform);
            intersectionsGo.transform.position = Vector3.zero;

            Selection.activeGameObject = mainGo;
            wps = Selection.activeGameObject.GetComponent<TrafficSystem>();

            EditorHelper.BeginUndoGroup("Add Intersection", wps);

            foreach(Vector3 point in intersections)
            {
                AddIntersection(point);
            }
            
            //Close Undo Operation
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        private static void AddWaypoint(Vector3 position) 
        {
            GameObject go = EditorHelper.CreateGameObject("Waypoint-" + wps.curSegment.waypoints.Count, wps.curSegment.transform);
            go.transform.position = position;

            Waypoint wp = EditorHelper.AddComponent<Waypoint>(go);
            wp.Refresh(wps.curSegment.waypoints.Count, wps.curSegment);

            //Record changes to the TrafficSystem (string not relevant here)
            Undo.RecordObject(wps.curSegment, "");
            wps.curSegment.waypoints.Add(wp);
        }

        private static void AddSegment(Vector3 position, string routeName) 
        {
            int segId = wps.segments.Count;
            GameObject segGo = EditorHelper.CreateGameObject(routeName, wps.transform.GetChild(0).transform);
            segGo.transform.position = position;

            wps.curSegment = EditorHelper.AddComponent<Segment>(segGo);
            wps.curSegment.id = segId;
            wps.curSegment.waypoints = new List<Waypoint>();
            wps.curSegment.nextSegments = new List<Segment>();

            //Record changes to the TrafficSystem (string not relevant here)
            Undo.RecordObject(wps, "");
            wps.segments.Add(wps.curSegment);
        }

        private static void AddIntersection(Vector3 position)
        {
            int intId = wps.intersections.Count;
            GameObject intGo = EditorHelper.CreateGameObject("Intersection-" + intId, wps.transform.GetChild(0).transform);
            intGo.transform.position = position;

            BoxCollider bc = EditorHelper.AddComponent<BoxCollider>(intGo);
            
            // change the size of the box collider to fit the intersection
            bc.size = intersectionSize;
            // change the center of the box collider to fit the intersection
            Vector3 center = bc.center;
            center.y = intersectionPos_y;
            bc.center = center;
            bc.isTrigger = true;

            Intersection intersection = EditorHelper.AddComponent<Intersection>(intGo);
            intersection.id = intId;

            //Record changes to the TrafficSystem (string not relevant here)
            Undo.RecordObject(wps, "");
            wps.intersections.Add(intersection);   
        }


    }
}