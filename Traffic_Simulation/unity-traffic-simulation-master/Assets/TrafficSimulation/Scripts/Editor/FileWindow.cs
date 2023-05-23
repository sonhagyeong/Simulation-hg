using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace TrafficSimulation {    
    public class FileWindow : EditorWindow
    {   
        private string routefilePath;
        private string intersectionfilePath;

        private static TrafficSystem wps;


        private List<List<Vector3>> routes = new List<List<Vector3>>();
        private static float route_Pos_y = 1.5f;


        private List<Vector3> intersections = new List<Vector3>();
        private static Vector3 intersectionSize = new Vector3(110,10,110);
        private static float intersectionPos_y = intersectionSize.y/2;

        private static List<Vector3> newPoints;
        private static Vector3 placeSize = new Vector3(20,10,10);
        private static float placePos_y = placeSize.y/2;
        private static List<Vector3> checkingRotateList;
        private static bool isRotate;
        private static List<Vector3> cornerPositions = new List<Vector3>{new Vector3(0,0,0), new Vector3(1350,0,0), new Vector3(1350,0,600), new Vector3(0,0,600)};
        
        private static float x1;
        private static float x2;
        private static float x3;

        private static float z1;
        private static float z2;
        private static float z3;
        
        
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
                if(routefilePath != null)
                {
                    routes = CreateRouteList(routefilePath, routes);
                    CreateRoutes(routes, cornerPositions, intersections);
                }
                
                else
                {
                    Debug.LogError("Check route file Path");
                }
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Create Intersections in Scene", GUILayout.Width(200)))
            {   
                if(intersectionfilePath != null)
                {
                    intersections = CreateIntersectionList(intersectionfilePath, intersections);
                    CreateIntersections(intersections);
                }
                
                else
                {
                    Debug.LogError("Check intersection file Path");
                }
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
                if(routefilePath != null && intersectionfilePath != null)
                {
                    CreateAll(routefilePath, intersectionfilePath, routes, intersections);
                }

                else
                {
                    Debug.LogError("Check files Path");
                }
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

                // int currentIntersection = -1;
                
                while(line != null)
                {
                    fields = line.Split(',');

                    if(fields.Length == 3)
                    {
                        // int intersectionNum = int.Parse(fields[3]);

                        // if(intersectionNum != currentIntersection)
                        // {
                        //     currentIntersection = intersectionNum;
                        // }

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
            CreateRoutes(routes, cornerPositions, intersections);
        }

        private static void CreateRoutes(List<List<Vector3>> routes, List<Vector3> corners, List<Vector3> intersections)
        {
            EditorHelper.SetUndoGroup("Create Routes");

            GameObject places = EditorHelper.CreateGameObject("Places");
            places.transform.position = Vector3.zero;

            if(corners.Count > 0 && intersections.Count > 0)
            {
                checkingRotateList = new List<Vector3>();
                checkingRotateList.AddRange(corners);
                checkingRotateList.AddRange(intersections);
            }
            
            else
            {
                Debug.LogError("There is no corners or intersections");
            }

            for(int i=0; i<routes.Count; i++)
            {   
                List<Vector3> route = routes[i];
                string routeName = "Route-" + i;

                GameObject mainGo = EditorHelper.CreateGameObject(routeName);
                mainGo.transform.position = Vector3.zero;
                EditorHelper.AddComponent<TrafficSystem>(mainGo);

                // GameObject segmentsGo = EditorHelper.CreateGameObject("Segments", mainGo.transform);
                // segmentsGo.transform.position = Vector3.zero;

                Selection.activeGameObject = mainGo;
                wps = Selection.activeGameObject.GetComponent<TrafficSystem>();

                EditorHelper.BeginUndoGroup("Add Segment", wps);

                AddSegment(route[0], routeName);

                for(int p=0; p <route.Count; p++)
                {  
                    List<Vector3> newRotationPoints = new List<Vector3>();
                    
                    if(p > 0 && p+1 < route.Count)
                    {
                        if(RotatePosition(route[p], checkingRotateList))
                        {
                            Debug.Log(">>> Rotate nowPosition : "+  route[p]);
                            newRotationPoints = ChangeToRotate(route[p-1], route[p], route[p+1]);
                        
                            foreach(Vector3 rPoint in newRotationPoints)
                            {   
                                Debug.Log(" rPoint : " + rPoint);
                                Vector3 newRoPoint = rPoint;
                                newRoPoint.y = route_Pos_y;
                                AddWaypoint(newRoPoint);
                            }
                        }
                    }

                    else
                    {
                        Vector3 newPoint = route[p];
                        newPoint.y = route_Pos_y;
                        AddWaypoint(newPoint);

                        if(p+1 == route.Count)
                        {
                            GameObject place = EditorHelper.CreateGameObject("place-" + i, places.transform);
                            place.transform.position = newPoint;
                            if(place.GetComponent<Collider>() == null)
                            {
                                AddCollider(place);
                            }   
                        }
                    }

                    // Vector3 newPoint = route[p];
                    // newPoint.y = route_Pos_y;
                    // AddWaypoint(newPoint);

                    // if(p+1 < route.Count)
                    // {   
                        // Vector3 nowPosition = route[p];
                        // if(nowPosition == Vector3.zero)
                        // {
                        //     Debug.Log("nowPosition is zero");
                        //     nowPosition.x = 25f;
                        //     route[p] = nowPosition;
                        //     Debug.Log("changed nowPosition : " + route[p]);
                        // }

                        

                        // Vector3 newPoint = route[p];
                        // newPoint.y = route_Pos_y;
                        // AddWaypoint(newPoint);

                        // newPoints = EditRoutePositions(route[p], route[p+1]);
                        // route[p+1] = newPoints[1];

                        // Vector3 newPoint = newPoints[0];
                        // newPoint.y = route_Pos_y;
                        // AddWaypoint(newPoint);
                    // }

                    // else
                    // {   
                    //     Vector3 newPoint = newPoints[1];
                    //     newPoint.y = route_Pos_y;
                    //     AddWaypoint(newPoint);

                    //     GameObject place = EditorHelper.CreateGameObject("place-" + i, places.transform);
                    //     place.transform.position = newPoints[1];
                    //     if(place.GetComponent<Collider>() == null)
                    //     {
                    //         AddCollider(place);
                    //     }
                    // }
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
            // GameObject segGo = EditorHelper.CreateGameObject(routeName, wps.transform.GetChild(0).transform);
            GameObject segGo = EditorHelper.CreateGameObject(routeName, wps.transform);

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

        public static void AddCollider(GameObject obj)
        {
            BoxCollider bc = obj.AddComponent<BoxCollider>();
            bc.size = placeSize;
            Vector3 center = bc.center;
            center.y = placePos_y;
            bc.center = center;
            bc.isTrigger = true;
        }

        private static bool RotatePosition(Vector3 nowPosition, List<Vector3> coordinateList)
        {   
            isRotate = CheckIfCoordinateExists(nowPosition, coordinateList);
            return isRotate;
        }
        
        private static bool CheckIfCoordinateExists(Vector3 coordinate, List<Vector3> coordinateList)
        {
            return coordinateList.Contains(coordinate);
        }

        private static List<Vector3> ChangeToRotate(Vector3 previousPoint, Vector3 nowPoint, Vector3 nextPoint)
        {   
            Debug.Log("previousPoint : " + previousPoint);
            Debug.Log("nowPoint : " + nowPoint);
            Debug.Log("nextPoint : " + nextPoint);

            float nowPoint_x = nowPoint.x;
            float nowPoint_z = nowPoint.z;

            float axis_x_pre_now = previousPoint.x - nowPoint_x;
            float axis_x_next_now = nextPoint.x - nowPoint_x;

            float axis_z_pre_now = previousPoint.z - nowPoint_z;
            float axis_z_next_now = nextPoint.z - nowPoint_z;

            List<Vector3> rotatePoints = new List<Vector3>();

            float Num_1 = 15f;
            float Num_2 = Num_1 * 2;

            // float x1;
            // float x2;
            // float x3;

            // float z1;
            // float z2;
            // float z3;

            // case 1
            if(axis_x_next_now == 0 && axis_z_pre_now == 0)
            {
                // To up
                if(axis_z_next_now > 0)
                {   
                    if(axis_x_pre_now < 0)
                    {   
                        Debug.Log("up left");
                        x1 = nowPoint_x - Num_2;
                        x2 = nowPoint_x - Num_1;
                    }

                    // right
                    else if(axis_x_pre_now > 0)
                    {   
                        Debug.Log("up right");
                        x1 = nowPoint_x + Num_2;
                        x2 = nowPoint_x + Num_1;
                    }

                    x3 = nowPoint_x;

                    z1 = nowPoint_z;
                    z2 = Calculate_Z_Up(z1, Num_1);
                    z3 = Calculate_Z_Up(z1, Num_2);
                    // }
                }

                // To Down 1
                else if(axis_z_next_now < 0)
                {
                    //left
                    if(axis_x_pre_now > 0)
                    {
                        Debug.Log("case 1 down left");
                        x1 = nowPoint_x + Num_2;
                        x2 = nowPoint_x + Num_1;
                    }

                    //right
                    else if(axis_x_pre_now < 0)
                    {
                        Debug.Log("case 1 down right");
                        x1 = nowPoint_x - Num_2;
                        x2 = nowPoint_x - Num_1;
                    }

                    x3 = nowPoint_x;

                    z1 = nowPoint_z;
                    z2 = Calculate_Z_Down(z1, Num_1);
                    z3 = Calculate_Z_Down(z1, Num_2);
                }
            }

            // case 2
            else if(axis_x_pre_now == 0 && axis_z_next_now == 0)
            {   
                // To down 2
                if(axis_z_pre_now > 0 )
                {
                    // right
                    if(axis_x_next_now < 0)
                    {
                        Debug.Log("case 2 down right");
                        x2 = nowPoint_x - Num_1;
                        x3 = nowPoint_x - Num_2;
                    }

                    //left
                    else if(axis_x_next_now > 0)
                    {
                        Debug.Log("case 2 down left");
                        x2 = nowPoint_x + Num_1;
                        x3 = nowPoint_x + Num_2;
                    }

                    x1 = nowPoint_x;

                    z3 = nowPoint_z;
                    z1 = Calculate_Z_Down2(z3, Num_2);
                    z2 = Calculate_Z_Down2(z3, Num_1);
                    
                }

                // To up 2
                else if(axis_z_pre_now < 0)
                {   
                    // right
                    if(axis_x_next_now > 0)
                    {
                        Debug.Log("case 2 up right");
                        x2 = nowPoint_x + Num_1;
                        x3 = nowPoint_x + Num_2;
                    }

                    // left
                    else if(axis_x_next_now < 0)
                    {
                        Debug.Log("case 2 up left");
                        x2 = nowPoint_x - Num_1;
                        x3 = nowPoint_x - Num_2;
                    }

                    x1 = nowPoint_x;

                    z3 = nowPoint_z;
                    z1 = Calculate_Z_Up2(z3, Num_2);
                    z2 = Calculate_Z_Up2(z3, Num_1);
                }
                
            }
            

            Debug.Log(" --- Original rotatePoints Count " + rotatePoints.Count);
            rotatePoints.Add(new Vector3(x1, 0f, z1));
            rotatePoints.Add(new Vector3(x2, 0f, z2));
            rotatePoints.Add(new Vector3(x3, 0f, z3));

            Debug.Log( "rotatePoints: " + rotatePoints[0] + " " + rotatePoints[1] + " " + rotatePoints[2]);
            return rotatePoints;
            // }
     
        }

        private static float Calculate_Z_Up(float start_z, float uNum)
        {
            return start_z + uNum;
        }
        
        private static float Calculate_Z_Down(float start_z, float dNum)
        {
            return start_z - dNum;
        }

        private static float Calculate_Z_Down2(float start_z, float dNum)
        {
            return start_z + dNum;
        }

        private static float Calculate_Z_Up2(float start_z, float dNum)
        {
            return start_z - dNum;
        }
    }
}