using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// This window is for creating routes and intersections without file input
namespace TrafficSimulation {
    public class RouteWindow : EditorWindow
    {   
        // private static List<Vector3> route;
        // private static List<List<Vector3>> routes = new List<List<Vector3>>();
        // private static List<string> routeNames = new List<string>();
        private static TrafficSystem wps;

        // Route variables
        private static float route_Pos_y = 1.5f;
        private int numRoutes = 1;
        private int numIntersections = 1;

        private Vector2 scrollPosition_1 = Vector2.zero;
        private Vector2 scrollPosition_2;

        // private System.Collections.Generic.List<System.Collections.Generic.List<Vector3>> routes = new System.Collections.Generic.List<System.Collections.Generic.List<Vector3>>();
        
        private System.Collections.Generic.List<System.Collections.Generic.List<Vector3>> routes = new System.Collections.Generic.List<System.Collections.Generic.List<Vector3>>();
        private System.Collections.Generic.List<string> routeNames = new System.Collections.Generic.List<string>();

        // Intersection variables
        // private System.Collections.Generic.List<System.Collections.Generic.List<Vector3>> intersections = new System.Collections.Generic.List<System.Collections.Generic.List<Vector3>>();

        private System.Collections.Generic.List<Vector3> intersections = new System.Collections.Generic.List<Vector3>();
        private static Vector3 intersectionSize = new Vector3(5,5,5);
        private static float intersectionPos_y = intersectionSize.y/2;


        [MenuItem("Component/Traffic Simulation/Create New Traffic System")]
        public static void ShowWindow()
        {
            RouteWindow window = EditorWindow.GetWindow<RouteWindow>();
            window.titleContent = new GUIContent("Input Values");
            window.Show();
        }

        private void OnGUI()
        {   
            // Create the first scroll view
            EditorGUILayout.BeginVertical();

            GUILayout.Label("Enter New Route Details", EditorStyles.boldLabel);
            numRoutes = EditorGUILayout.IntField(numRoutes);

            // Scroll view for the routes input
            GUILayout.Label("Routes", EditorStyles.boldLabel);
            scrollPosition_1 = EditorGUILayout.BeginScrollView(scrollPosition_1, GUILayout.Height(300));
            while (routes.Count < numRoutes)
            {
                routes.Add(new System.Collections.Generic.List<Vector3>());
                routeNames.Add("Route " + (routes.Count));
            }

            while (routes.Count > numRoutes)
            {
                routes.RemoveAt(routes.Count - 1);
                routeNames.RemoveAt(routeNames.Count - 1);
            }
            
            for (int i = 0; i < routes.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Route {i + 1}", GUILayout.Width(60));
                routeNames[i] = EditorGUILayout.TextField(routeNames[i], GUILayout.Width(200));
                GUILayout.EndHorizontal();
                for (int j = 0; j < routes[i].Count; j++)
                {
                    routes[i][j] = EditorGUILayout.Vector3Field($"Point {j}", routes[i][j]);
                }

                if (GUILayout.Button("Add Point"))
                {
                    routes[i].Add(Vector3.zero);
                }
                EditorGUILayout.Space(30);
            }
            EditorGUILayout.EndScrollView();


            GUILayout.Label("Enter New Intersection Details", EditorStyles.boldLabel);
            numIntersections = EditorGUILayout.IntField(numIntersections);
            

            // Scroll view for the intersections input
            GUILayout.Label("Intersection", EditorStyles.boldLabel);
            scrollPosition_2 = EditorGUILayout.BeginScrollView(scrollPosition_2, GUILayout.Height(300));
            while (intersections.Count < numIntersections)
            {
                intersections.Add(Vector3.zero);
            }

            while (intersections.Count > numIntersections)
            {
                intersections.RemoveAt(intersections.Count - 1);
            }
            
            for (int k = 0; k < intersections.Count; k++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"intersection {k + 1}", GUILayout.Width(100));
                EditorGUILayout.TextField($"intersection {k + 1}", GUILayout.Width(200));
                GUILayout.EndHorizontal();
                intersections[k] = EditorGUILayout.Vector3Field($"Point {k}", intersections[k]);
           
                EditorGUILayout.Space(30);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            // Create the buttons
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create Routes", GUILayout.Width(150)))
            {
                CreateRoutes(routeNames, routes);
            }
            
            GUILayout.Space(10);

            if (GUILayout.Button("Create Intersections", GUILayout.Width(150)))
            {
                CreateIntersections(intersections);
            }
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);

            
            GUILayout.BeginVertical();
            if (GUILayout.Button("Create All"))
            {
                CreateAll(routeNames, routes, intersections);
                Close();
            }
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
      
        }

        private static void CreateAll(List<string> routeNames, List<List<Vector3>> routes, List<Vector3> intersections)
        {   
            if(routes.Count > 0 & intersections.Count > 0)
            {
                CreateIntersections(intersections);
                CreateRoutes(routeNames, routes);
                
            }
        }

        private static void CreateRoutes(List<string> routeNames, List<List<Vector3>> routes)
        {
            EditorHelper.SetUndoGroup("Create Routes");

            for(int i=0; i<routes.Count; i++)
            {   
                string routeName = routeNames[i];
                List<Vector3> route = routes[i];

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