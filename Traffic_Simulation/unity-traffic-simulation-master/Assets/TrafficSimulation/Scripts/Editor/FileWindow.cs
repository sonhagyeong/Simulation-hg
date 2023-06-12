using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace TrafficSimulation {    
    public class FileWindow : EditorWindow
    {   
        private static string routefilePath;
        private static string intersectionfilePath = "C:\\Users\\USER\\workspace\\intersectionPoints.csv";

        private static TrafficSystem wps;

        // Intersection Collider Parameters
        private static List<Vector3> intersections = new List<Vector3>();
        private static Vector3 intersectionSize = new Vector3(80,10,80);
        private static float intersectionPos_y = intersectionSize.y/2;

        // Station Collider Parameters
        private static Vector3 placeSize = new Vector3(20,10,10);
        private static float placePos_y = placeSize.y/2;
        
        // Route Parameters
        private static List<List<Vector3>> routes = new List<List<Vector3>>();
        private static Dictionary<int, List<Vector3>> routeDictionary = new Dictionary<int, List<Vector3>>();
        
        private static float route_Pos_y = 1.5f;
        private static Vector3 newPoint;
        
        // Corner Positions
        private static List<Vector3> cornerPositions = new List<Vector3>{new Vector3(0,0,0), new Vector3(1350,0,0), new Vector3(1350,0,600), new Vector3(0,0,600)};
        
        // private static List<Vector3> checkingRotateList;
        private static bool isRotate;        
        private static Vector3 newRoPoint;
        // ChangeToRotate Parameters
        private static float x1;
        private static float x2;
        private static float x3;
        private static float x4;

        private static float z1;
        private static float z2;
        private static float z3;
        private static float z4;


        
        
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
                    // routes = CreateRouteList(routefilePath, routes, routeDictionary);
                    routeDictionary = CreateRouteList(routefilePath);

                    // CreateRoutes(routes, cornerPositions, intersections);
                    CreateRoutes(routeDictionary, cornerPositions, intersections);

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
            // EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (GUILayout.Button("Create Corners in Scene", GUILayout.Width(200)))
            {   
                if(intersectionfilePath != null)
                {
                    CreateCorners(cornerPositions);
                }
                
                else
                {
                    Debug.LogError("Check Corners List");
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
            GUILayout.Space(10);
            
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


        private static bool RotatePosition(Vector3 prePosition, Vector3 nowPosition, Vector3 nextPosition, List<Vector3> coordinateList)
        {   
            isRotate = CheckIfCoordinateExists(prePosition, nowPosition, nextPosition, coordinateList);
            return isRotate;
        }
        
        private static bool CheckIfCoordinateExists(Vector3 prePo, Vector3 nowPo, Vector3 nextPo, List<Vector3> coordinateList)
        {   
            float pre_now_x = prePo.x - nowPo.x;
            float pre_now_z = prePo.z - nowPo.z;
            float now_next_x = nowPo.x - nextPo.x;
            float now_next_z = nowPo.z - nextPo.z;

            bool case_1 = pre_now_x == 0 && pre_now_z != 0 && now_next_x != 0 && now_next_z == 0;
            bool case_2 = pre_now_x != 0 && pre_now_z == 0 && now_next_x == 0 && now_next_z != 0;

            bool isExist = coordinateList.Contains(nowPo) && (case_1 || case_2);
            return isExist;
        }

        private static List<Vector3> ChangeToRotate(Vector3 previousPoint, Vector3 nowPoint, Vector3 nextPoint)
        {   
            float nowPoint_x = nowPoint.x;
            float nowPoint_z = nowPoint.z;

            float axis_x_pre_now = previousPoint.x - nowPoint_x;
            float axis_x_next_now = nextPoint.x - nowPoint_x;

            float axis_z_pre_now = previousPoint.z - nowPoint_z;
            float axis_z_next_now = nextPoint.z - nowPoint_z;

            List<Vector3> rotatePoints = new List<Vector3>();

            float Num_1 = 2f;
            float Num_2 = 12f;
            float Num_3 = 40f;
            
            if(axis_x_pre_now == 0 && axis_z_next_now == 0)
            {   
                if(axis_z_pre_now > 0)
                {
                    // Down Right (ㄴ 모양)
                    if(axis_x_next_now < 0)
                    {
                        x2 = nowPoint_x - Num_1;
                        x3 = nowPoint_x - Num_2;
                        x4 = nowPoint_x - Num_3;

                    }

                    // Down Left (ㄴ 모양)
                    else if(axis_x_next_now > 0)
                    {                        
                        x2 = nowPoint_x + Num_1;
                        x3 = nowPoint_x + Num_2;
                        x4 = nowPoint_x + Num_3;

                     
                    }
                    x1 = nowPoint_x;

                    z1 = nowPoint_z + Num_3;
                    z2 = nowPoint_z + Num_2;
                    z3 = nowPoint_z + Num_1;
                    z4 = nowPoint_z;
                }

                else if(axis_z_pre_now < 0)
                {
                    // Up left 2 (ㄱ 모양)
                    if(axis_x_next_now < 0)
                    {
                        x2 = nowPoint_x - Num_1;
                        x3 = nowPoint_x - Num_2;
                        x4 = nowPoint_x - Num_3;

                    }

                    // Up right 2(ㄱ 반대모양)
                    else if(axis_x_next_now > 0)
                    {
                        x2 = nowPoint_x + Num_1;
                        x3 = nowPoint_x + Num_2;
                        x4 = nowPoint_x + Num_3;

                    }
                    x1 = nowPoint_x;

                    z1 = nowPoint_z - Num_3;
                    z2 = nowPoint_z - Num_2;
                    z3 = nowPoint_z - Num_1;
                    z4 = nowPoint_z;
                }
                
            }

            else if(axis_z_pre_now == 0 && axis_x_next_now == 0)
            {
                if(axis_z_next_now > 0)
                {
                    // Up left (ㄴ 반대 모양)
                    if(axis_x_pre_now < 0)
                    {
                        x1 = nowPoint_x - Num_3;
                        x2 = nowPoint_x - Num_2;
                        x3 = nowPoint_x - Num_1;
  
                    }

                    // Up right (ㄴ 모양)
                    else if(axis_x_pre_now > 0)
                    {
                        x1 = nowPoint_x + Num_3;
                        x2 = nowPoint_x + Num_2;
                        x3 = nowPoint_x + Num_1;
                     
                    }

                    x4 = nowPoint_x;

                    z1 = nowPoint_z;
                    z2 = nowPoint_z + Num_1;
                    z3 = nowPoint_z + Num_2;
                    z4 = nowPoint_z + Num_3;

                }

                else if(axis_z_next_now < 0)
                {
                    // Down right 2 (ㄱ 모양)
                    if(axis_x_pre_now < 0)
                    {
                        x1 = nowPoint_x - Num_3;
                        x2 = nowPoint_x - Num_2;
                        x3 = nowPoint_x - Num_1;
              
                    }

                    // Down left 2 (ㄱ 반대 모양)
                    else if(axis_x_pre_now > 0)
                    {
                        x1 = nowPoint_x + Num_3;
                        x2 = nowPoint_x + Num_2;
                        x3 = nowPoint_x + Num_1;
               
                    }

                    x4 = nowPoint_x;


                    z1 = nowPoint_z;
                    z2 = nowPoint_z - Num_1;
                    z3 = nowPoint_z - Num_2;
                    z4 = nowPoint_z - Num_3;
                }
                
            }
     
            rotatePoints.Add(new Vector3(x1, 0f, z1));
            rotatePoints.Add(new Vector3(x2, 0f, z2));
            rotatePoints.Add(new Vector3(x3, 0f, z3));
            rotatePoints.Add(new Vector3(x4, 0f, z4));

            return rotatePoints;
        }

        public static List<Vector3> EditPathPoints(Vector3 nowPoint, Vector3 nextPoint)
        {   
            float axis_x_next_now = nextPoint.x - nowPoint.x;
            float axis_z_next_now = nextPoint.z - nowPoint.z;

            List<Vector3> EditPoints = new List<Vector3>();

            if(axis_x_next_now < 0)
            {
                nowPoint.z += 7.5f;
                nextPoint.z += 7.5f;
            }

            else if(axis_x_next_now > 0)
            {
                nowPoint.z -= 7.5f;
                nextPoint.z -= 7.5f;
            }

            if(axis_z_next_now < 0)
            {
                nowPoint.x -= 7.5f;
                nextPoint.x -= 7.5f;
            }

            else if(axis_z_next_now > 0)
            {
                nowPoint.x += 7.5f;
                nextPoint.x += 7.5f;
            }

            EditPoints.Add(nowPoint);
            EditPoints.Add(nextPoint);

            return EditPoints;
        }


        private static Dictionary<int, List<Vector3>> CreateRouteList(string _routefilePath)
        {   
            if (!File.Exists(_routefilePath))
            {
                Debug.LogError("File does not exist: " + _routefilePath);
                // return null;
            }

            Dictionary<int, List<Vector3>> _routeDictionary = new Dictionary<int, List<Vector3>>();

            using (StreamReader reader = new StreamReader(_routefilePath))
            {
                // Skip the first line
                reader.ReadLine();

                // string line = reader.ReadLine();
                string[] fields;

                // routeNum과 리스트를 매핑할 딕셔너리
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    fields = line.Split(',');

                    if (fields.Length == 4)
                    {
                        int routeNum = int.Parse(fields[0]);

                        if (!_routeDictionary.TryGetValue(routeNum, out List<Vector3> routePoints))
                        {
                            routePoints = new List<Vector3>();
                            _routeDictionary.Add(routeNum, routePoints);
                        }

                        float x = float.Parse(fields[1]);
                        float y = float.Parse(fields[2]);
                        float z = float.Parse(fields[3]);

                        Vector3 point = new Vector3(x, y, z);
                        routePoints.Add(point);

                    }
                }

                // _routes.AddRange(_routeDictionary.Values);
            }

            return _routeDictionary;
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
                        float x = float.Parse(fields[0]);
                        float y = float.Parse(fields[1]);
                        float z = float.Parse(fields[2]);

                        Vector3 point = new Vector3(x, y, z);                        
                        intersections.Add(point);
                    }

                    line = reader.ReadLine();
                }
            }

            return intersections;
        }

        public static void CreateAll(string routefilePath, string intersectionfilePath, List<List<Vector3>> routes, List<Vector3> intersections)
        {   
            intersections = CreateIntersectionList(intersectionfilePath, intersections);
            CreateIntersections(intersections);
            // routes = CreateRouteList(routefilePath, routes, routeDictionary);
            routeDictionary = CreateRouteList(routefilePath);
            // if(routes == null) Debug.LogError("routes is null");
            CreateRoutes(routeDictionary, cornerPositions, intersections);
        }

        // private static void CreateRoutes(List<List<Vector3>> routes, List<Vector3> corners, List<Vector3> intersections)
        private static void CreateRoutes(Dictionary<int, List<Vector3>> _routeDictionary, List<Vector3> _corners, List<Vector3> _intersections)
        {
            EditorHelper.SetUndoGroup("Create Routes");

            List<Vector3> checkingRotateList = new List<Vector3>();
            if(_corners.Count > 0 && _intersections.Count > 0)
            {
                checkingRotateList.AddRange(_corners);
                checkingRotateList.AddRange(_intersections);
            }
            
            else
            {
                Debug.LogError("There is no corners or intersections");
            }
            if(_routeDictionary == null) Debug.Log("routeDictionary is null");

            Debug.Log("_routeDictionary.Keys.Count : " + _routeDictionary.Keys.Count);
            foreach(int dict_key in _routeDictionary.Keys)
            {   
                Debug.Log("dict_key : " + dict_key);
                List<Vector3> route = _routeDictionary[dict_key];
                Debug.Log("route.Count : " + route.Count);
                if(route == null) Debug.LogError("route is null");

                string routeName = "Route-" + dict_key.ToString();

                GameObject mainGo = EditorHelper.CreateGameObject(routeName);
                mainGo.transform.position = Vector3.zero;
                EditorHelper.AddComponent<TrafficSystem>(mainGo);
                EditorHelper.AddComponent<RouteInfo>(mainGo);

                Selection.activeGameObject = mainGo;
                wps = Selection.activeGameObject.GetComponent<TrafficSystem>();
                RouteInfo routeInfo = Selection.activeGameObject.GetComponent<RouteInfo>();

                EditorHelper.BeginUndoGroup("Add Segment", wps);

                AddSegment(route[0], routeName);
                
                for(int p=0; p <route.Count; p++)
                {  
                    List<Vector3> newRotationPoints = new List<Vector3>();
                    List<Vector3> paths = new List<Vector3>();
         
                    if(p > 0 && p+1 < route.Count)
                    {   
                        // 회전하는 위치인 경우
                        if(RotatePosition(route[p-1], route[p], route[p+1], checkingRotateList))
                        {
                            newRotationPoints = ChangeToRotate(route[p-1], route[p], route[p+1]);
                            
                            List<Vector3> rPoints = new List<Vector3>();

                            for(int rPoint = 0; rPoint <newRotationPoints.Count; rPoint++)
                            {   
                                if(rPoint+1 < newRotationPoints.Count)
                                {
                                    rPoints = EditPathPoints(newRotationPoints[rPoint], newRotationPoints[rPoint+1]);

                                    newRoPoint = rPoints[0];
                                    newRoPoint.y = route_Pos_y;
                                    AddWaypoint(newRoPoint);
                                }

                                else
                                {
                                    newRoPoint = rPoints[1];
                                    newRoPoint.y = route_Pos_y;
                                    AddWaypoint(newRoPoint);
                                }
                            }
                        }

                        // 회전구간 아닌 경우
                        else
                        {   
                            // 다시 돌아가는 길인 경우
                            if(p-1 >= 0 && route[p-1] == route[p+1])
                            {   
                                // routeInfo.uTurnNum ++;
                                paths = EditPathPoints(route[p-1], route[p]);
                                newPoint = paths[1];
                                newPoint.y = route_Pos_y;
                                AddWaypoint(newPoint);
                                routeInfo.uTurnStations.Add(route[p]);
                            }

                            paths = EditPathPoints(route[p], route[p+1]);
                            newPoint = paths[0];
                            newPoint.y = route_Pos_y;
                            AddWaypoint(newPoint);
                            
                        }
                    }

                    // p = 0 or p+1 = route.Count
                    else
                    {   
                        if(p == 0)
                        {
                            paths = EditPathPoints(route[p], route[p+1]);

                            newPoint = paths[0];
                            newPoint.y = route_Pos_y;
                            AddWaypoint(newPoint);
                        }
                        

                        if(p+1 == route.Count)
                        {   
                            paths = EditPathPoints(route[p-1], route[p]);

                            newPoint = paths[1];
                            newPoint.y = route_Pos_y;
                            AddWaypoint(newPoint);
                        }
                    }
                }
            

                Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
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

        private static void CreateCorners(List<Vector3> cornersList)
        {
            EditorHelper.SetUndoGroup("Create Corner System");

            GameObject cornersGo = EditorHelper.CreateGameObject("Corners");
            cornersGo.transform.position = Vector3.zero;

            int intId = 0;
            foreach(Vector3 point in cornersList)
            {   
                AddCorners(point, intId);
                intId ++;
            }

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

        private static void AddCorners(Vector3 position, int _intId)
        {
            GameObject parentGO = GameObject.Find("Corners");
            string cornerName = "Corner-" + _intId;
            GameObject cornerGo = EditorHelper.CreateGameObject(cornerName, parentGO.transform);
            cornerGo.transform.position = position;
            cornerGo.name = cornerName;

            BoxCollider bc = EditorHelper.AddComponent<BoxCollider>(cornerGo);

            // change the size of the box collider to fit the intersection
            bc.size = intersectionSize;
            // change the center of the box collider to fit the intersection
            Vector3 center = bc.center;
            center.y = intersectionPos_y;
            bc.center = center;
            bc.isTrigger = true;

            EditorHelper.AddComponent<Corner>(cornerGo);
            
        }

    }
}