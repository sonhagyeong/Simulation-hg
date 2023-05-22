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

        private static TrafficSystem wps;


        private List<List<Vector3>> routes = new List<List<Vector3>>();
        private static float route_Pos_y = 1.5f;


        private List<Vector3> intersections = new List<Vector3>();
        private static Vector3 intersectionSize = new Vector3(20,5,20);
        private static float intersectionPos_y = intersectionSize.y/2;

        private static List<Vector3> newPoints;

        
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
            GUILayout.Label("Routes Data", EditorStyles.boldLabel);
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

            GUILayout.Label("Intersections Data", EditorStyles.boldLabel);
            if(GUILayout.Button("Open Intersections Data File"))
            {
                intersectionfilePath = EditorUtility.OpenFilePanel("Select File", "", "");
                if(intersectionfilePath != "")
                {
                    Debug.Log("Selected file path: " + intersectionfilePath);
                }
            }
            GUILayout.Label("File Path: " + intersectionfilePath);
            EditorGUILayout.Space(10);



            // Create the buttons
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Route Count : " + routes.Count, GUILayout.Width(150));
            GUILayout.Space(10);
            GUILayout.Label("Intersection Count : " + intersections.Count, GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Routes in Scene", GUILayout.Width(150)))
            {   
                routes = CreateRouteList(routefilePath, routes);
                CreateRoutes(routes);
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Create Intersections in Scene", GUILayout.Width(200)))
            {   
                intersections = CreateIntersectionList(intersectionfilePath, intersections);
                CreateIntersections(intersections);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);


            // Reset Button
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Reset Route", GUILayout.Width(150)))
            {
                routes.Clear();
            }
            GUILayout.Space(10);
            if(GUILayout.Button("Reset Intersection", GUILayout.Width(200)))
            {
                intersections.Clear();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);


            // Create All Button
            GUILayout.BeginVertical();
            if (GUILayout.Button("Create All"))
            {
                CreateAll(routefilePath, intersectionfilePath, routes, intersections);
                // Close();
            }
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
        }

        //Create a list by route
        private static List<List<Vector3>> CreateRouteList(string routefilePath, List<List<Vector3>> routes)
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
        
        private static List<Vector3> CreateIntersectionList(string intersectionfilePath, List<Vector3> intersections)
        {   
            if (!File.Exists(intersectionfilePath))
            {
                Debug.LogError("File does not exist: " + intersectionfilePath);
                return null;
            }

            using (StreamReader reader = new StreamReader(intersectionfilePath))
            {   
                // Skip the first line
                reader.ReadLine(); 

                string line = reader.ReadLine();
                string[] fields;

                int currentIntersection = -1;
                
                while(line != null)
                {
                    fields = line.Split(',');

                    if(fields.Length == 4)
                    {
                        int intersectionNum = int.Parse(fields[3]);

                        if(intersectionNum != currentIntersection)
                        {
                            currentIntersection = intersectionNum;
                        }

                        float x = float.Parse(fields[0]);
                        float y = float.Parse(fields[1]);
                        float z = float.Parse(fields[2]);

                        Vector3 point = new Vector3(x, y, z);                        
                        intersections.Add(point);
                        // Debug.Log("currentIntersection : "+ currentIntersection + " point : " + point);
                    }

                    line = reader.ReadLine();
                }
            }

            return intersections;
        }

        public static List<Vector3> EditRoutePositions(Vector3 nowPosition, Vector3 nextPosition)
        {   
            float axis_x = nextPosition.x - nowPosition.x;
            float axis_z = nextPosition.z - nowPosition.z;

            List<Vector3> newPositions = new List<Vector3>();

            if(axis_x == 0)
            {
                // To front side
                if(axis_z > 0)
                {
                    nowPosition.x += 7.5f;
                    nextPosition.x += 7.5f;
                }

                // To back side
                else
                {
                    nowPosition.x -= 7.5f;
                    nextPosition.x -= 7.5f;
                }
                    
            
            }

            else if(axis_z == 0)
            {   
                // To right side
                if(axis_x > 0)
                {
                    nowPosition.z -= 7.5f;
                    nextPosition.z -= 7.5f;
                }

                // To left side
                else
                {
                    nowPosition.z += 7.5f;
                    nextPosition.z += 7.5f;
                }
            }

            newPositions.Add(nowPosition);
            newPositions.Add(nextPosition);

            return newPositions;
        }

        public static void CreateAll(string routefilePath, string intersectionfilePath, List<List<Vector3>> routes, List<Vector3> intersections)
        {   
            intersections = CreateIntersectionList(intersectionfilePath, intersections);
            CreateIntersections(intersections);
            routes = CreateRouteList(routefilePath, routes);
            CreateRoutes(routes);
        }

        private static void CreateRoutes(List<List<Vector3>> routes)
        {
            EditorHelper.SetUndoGroup("Create Routes");

            for(int i=0; i<routes.Count; i++)
            {   
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

                // foreach(Vector3 point in route)
                // {  
                //     Vector3 newPoint = point;
                //     newPoint.y = route_Pos_y;
                //     AddWaypoint(newPoint);
                // }

                for(int p=0; p <route.Count; p++)
                {  
                    if(p+1 < route.Count)
                    {   
                        newPoints = EditRoutePositions(route[p], route[p+1]);
                        route[p+1] = newPoints[1];
                        Vector3 newPoint = newPoints[0];
                        newPoint.y = route_Pos_y;
                        AddWaypoint(newPoint);
                    }

                    else
                    {   
                        Debug.Log("Last Point");
                        Vector3 newPoint = newPoints[1];
                        newPoint.y = route_Pos_y;
                        AddWaypoint(newPoint);
                    }
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

        public static void AddIntersection(Vector3 position)
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