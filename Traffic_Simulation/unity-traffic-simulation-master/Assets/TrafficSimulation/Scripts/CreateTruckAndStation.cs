using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System;

namespace TrafficSimulation{
    public class CreateTruckAndStation : MonoBehaviour
    {

        private string truckFilePath = "C:\\Users\\USER\\workspace\\Truck_50_shortest.csv";

        public static List<CreateTruckData> truckDataList = new List<CreateTruckData>();

        private static float truckRotation_y;

        // Station Parameters
        private static Vector3 stationSize = new Vector3(75,10,30);
        private static float stationPos_y = stationSize.y/2;
        private string stationTagName = "Station";

        // 동일한 시작 위치를 가진 트럭들을 포함하는 딕셔너리
        private static Dictionary<Vector3, List<Tuple<string, string, List<Vector3>>>> startPositionDict;

        // 동일한 시작 위치를 가진 트럭들의 생성 주기
        private float createDelay = 2.5f;
        void Start()
        {
            ReadFile(truckFilePath);
            CreateStations(truckDataList, stationTagName);
            if(ExistRoute(truckDataList))
            {   
                IsDuplicateStartPosition(truckDataList);
                CreateTrucks(startPositionDict, createDelay);
                Debug.Log("All routes exist");
                // CreateTrucks(truckDataList);
            }
        }

        public static void ReadFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError("File does not exist: " + filePath);
                return;
            }
            
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                bool isFirstLine = true;

                while ((line = reader.ReadLine()) != null)
                {
                    if (isFirstLine)
                    {
                        isFirstLine = false;
                        continue; // Skip the first line
                    }

                    string[] values = line.Split(',');

                    string truckName = "Truck-" + values[0];
                    string truckRoute = values[1];

                    CreateTruckData truckData = ScriptableObject.CreateInstance<CreateTruckData>();
                    
                    List<Vector3> workStations = new List<Vector3>();

                    for(int i=2; i<values.Length; i+=3)
                    {   
                        if (values[i] != "" && values[i + 1] != "" && values[i + 2] != "")
                        {
                            string xStr = Regex.Match(values[i], @"\(\s*(-?\d+(\.\d+)?)").Groups[1].Value;
                            string yStr = values[i + 1];
                            string zStr = Regex.Match(values[i + 2], @"(-?\d+(\.\d+)?)\s*\)").Groups[1].Value;

                            float x = float.Parse(xStr);
                            float y = float.Parse(yStr);
                            float z = float.Parse(zStr);

                            Vector3 station = new Vector3(x, y, z);

                            workStations.Add(station);
                        }
                        
                        else
                        {
                            break; // No more values in the next column, exit the loop
                        }
                    }

                    truckData.CreateData(truckName, truckRoute, workStations);
                    truckDataList.Add(truckData);
                }
            }

        }


        // 경로 유무 확인 함수
        public static bool ExistRoute(List<CreateTruckData> dataList)
        {
            // Print the truck data
            foreach(CreateTruckData data in dataList)
            {
                if(!GameObject.Find("Route-"+ data.Route))
                {
                    Debug.LogError("Route does not exist: " + data.Route);
                    return false;
                }
            }

            return true;
        }

       
        // 트럭 시작 위치에 따른 회전 방향 설정 함수
        public static float GetTruckRotation(Transform routeTransform, string routeName)
        {
            Vector3 position_1 = routeTransform.Find(routeName + "/Waypoint-0").transform.position;
            Vector3 position_2 = routeTransform.Find(routeName + "/Waypoint-1").transform.position;
            
            if(position_1.x == position_2.x)
            {   
                // If the truck is moving in the positive z direction
                if(position_1.z - position_2.z < 0)
                {
                    truckRotation_y = 0;
                }

                // If the truck is moving in the negative z direction
                else
                {
                    truckRotation_y = 180;
                }
            }

            else if(position_1.z == position_2.z)
            {
                // If the truck is moving in the positive x direction
                if(position_1.x - position_2.x < 0)
                {
                    truckRotation_y = 90;
                }

                // If the truck is moving in the negative x direction
                else
                {
                    truckRotation_y = 270;
                }
            }

            return truckRotation_y;
        }


        // Station 생성 함수
        public void CreateStations(List<CreateTruckData> dataList, string _stationTagName)
        {   
            GameObject stationsOB = GameObject.Find("Stations");

            if(stationsOB == null)
            {
                // Create empty game object named "Stations"
                stationsOB = new GameObject("Stations");
                stationsOB.transform.position = Vector3.zero;
            }

            foreach(CreateTruckData data in dataList)
            {
                if(data.WorkStations.Count > 0)
                {   
                    for(int i = 0; i <data.WorkStations.Count; i++)
                    {
                        Vector3 workStation = data.WorkStations[i];

                        string workStationName = workStation.ToString();

                        // Create a station
                        if(!ExistStation(stationsOB.name, workStationName))
                        {
                            GameObject workStationOB = new GameObject(workStationName);
                            workStationOB.transform.parent = stationsOB.transform;
                            workStationOB.transform.position = workStation;
                            if(ExistTag(_stationTagName))
                            {
                                workStationOB.tag = _stationTagName;
                            }

                            else
                            {
                                Debug.LogError("Tag does not exist: " + _stationTagName);
                            }
                            
                            workStationOB.AddComponent<StationsInfo>();
                            AddCollider(workStationOB);
                        }
                    }
                }
                
                else
                {
                    Debug.LogError("No stations found for truck: " + data.Name);
                }

            }
        }


        // Station 유무 확인 함수
        public static bool ExistStation(string parentOBName, string stationName)
        {   
            bool isExist = true;
            if(GameObject.Find(parentOBName + "/" + stationName) == null)
            {
                isExist = false;
            }

            return isExist;
        }

        
        // 콜라이더 추가 함수
        public static void AddCollider(GameObject obj)
        {
            BoxCollider bc = obj.AddComponent<BoxCollider>();
            bc.size = stationSize;
            Vector3 center = bc.center;
            center.y = stationPos_y;
            bc.center = center;
            bc.isTrigger = true;
        }
        
        
        // 딕셔너리 생성 함수
        private static void IsDuplicateStartPosition(List<CreateTruckData> dataList)
        {
            startPositionDict = new Dictionary<Vector3, List<Tuple<string, string, List<Vector3>>>>();

            foreach(CreateTruckData data in dataList)
            {   
                string parentName = "Route-" + data.Route;
                string childName = parentName + "/Waypoint-0";
                
                GameObject parentOB = GameObject.Find(parentName);

                if(parentOB != null)
                {
                    Transform childTransform = parentOB.transform.Find(childName);

                    if(childTransform != null)
                    {
                        Vector3 startPoint = childTransform.position;

                        Tuple<string, string, List<Vector3>> _truckData = Tuple.Create(data.Name, parentName, data.WorkStations);

                        if(startPositionDict.ContainsKey(startPoint))
                        {  
                            startPositionDict[startPoint].Add(_truckData);
                        }

                        else
                        {
                            startPositionDict[startPoint] = new List<Tuple<string, string, List<Vector3>>> { _truckData };
                        }
                    }

                    else
                    {
                        Debug.LogError("Child object not found: " + childName);
                    }
                }

                else
                {
                    Debug.LogError("Parent object not found: " + parentName);
                }
            }
        }

        
        // 출발 위치가 동일한 트럭이 있는지 확인한 후 트럭 생성하는 함수
        private void CreateTrucks(Dictionary<Vector3, List<Tuple<string, string, List<Vector3>>>> _dictionary, float _createDelay)
        {
            // Print the duplicate start positions
            foreach (KeyValuePair<Vector3, List<Tuple<string, string, List<Vector3>>>> kvp in _dictionary)
            {   
                // Debug.Log("kvp.Value.Count: " + kvp.Value.Count);

                Vector3 key = kvp.Key;
                List<Tuple<string, string, List<Vector3>>> values = kvp.Value;

                // 출발 위치가 동일한 트럭이 있는 경우
                if(values.Count > 1)
                {   
                    StartCoroutine(DuplicatePositionCreateTruck(values, _createDelay));
                }

                // 출발 위치가 동일한 트럭이 없는 경우
                else
                {
                    string truckName = values[0].Item1;
                    string routeName = values[0].Item2;
                    List<Vector3> truckWorkStations = values[0].Item3;

                    CreateTruck(truckName, routeName, truckWorkStations);
                }
                
                
           
            }
        }
        

        // 트럭 생성 함수
        private void CreateTruck(string _truckName, string _routeName, List<Vector3> _workStaions)
        {
            // Generate a random number between 1 and 4 (inclusive)
            int randomNumber = UnityEngine.Random.Range(1, 5);
            string truckPrefabName = "Truck" + randomNumber.ToString();
            // string truckPrefabName = "Truck1";
   
            GameObject truckPrefab = Resources.Load(truckPrefabName) as GameObject;

            if (truckPrefab != null)
            {
                GameObject truck = Instantiate(truckPrefab);
                truck.name = _truckName;
                Transform routeTransform = GameObject.Find(_routeName).transform;

                Vector3 routePosition = routeTransform.Find(_routeName + "/Waypoint-0").transform.position;
                truck.transform.position = new Vector3(routePosition.x, 0f, routePosition.z);
                truck.transform.rotation = Quaternion.Euler(0, GetTruckRotation(routeTransform, _routeName), 0);

                // Set the truck's route
                truck.GetComponent<VehicleAI>().trafficSystem = GameObject.Find(_routeName).GetComponent<TrafficSystem>();

                // Set the truck's work stations and destination
                if(truck.GetComponent<TruckInfo>() == null)
                {   
                    truck.AddComponent<TruckInfo>();
                }

                TruckInfo truckInfo = truck.GetComponent<TruckInfo>();
                truckInfo.truckWorkStations = _workStaions;
                truckInfo.truckWorkStationsNum = _workStaions.Count;

                int workStationCount = _workStaions.Count;
                // truckInfo.truckOrigin = _workStaions[0];
                truckInfo.truckOrigin = truck.transform.position;
                // truckInfo.truckDestination = _workStaions[workStationCount - 1];
                truckInfo.truckRouteName = _routeName;
                truckInfo.turnStations = GameObject.Find(_routeName).GetComponent<RouteInfo>().uTurnStations;
                // truckInfo.turnNum = GameObject.Find(_routeName).GetComponent<RouteInfo>().uTurnNum;
            }

            else
            {
                Debug.LogError("Truck prefab not found: " + truckPrefabName);
            }
        }

        
        // 출발 위치가 동일한 트럭이 있는 경우 트럭 생성 함수
        private IEnumerator DuplicatePositionCreateTruck(List<Tuple<string, string, List<Vector3>>> _values, float _createDelay)
        {
            int duplivatedTruckCount = 0;

            foreach (Tuple<string, string, List<Vector3>> value in _values)
            {
                string truckName = value.Item1;
                string routeName = value.Item2;
                List<Vector3> truckWorkStations = value.Item3;
                CreateTruck(truckName, routeName, truckWorkStations);

                if(duplivatedTruckCount > 0)
                {
                    GameObject waitedTruck = GameObject.Find(truckName);

                    if(waitedTruck != null)
                    {
                        waitedTruck.GetComponent<BoxCollider>().enabled = false;
                        waitedTruck.GetComponent<VehicleAI>().vehicleStatus = Status.STOP;

                        yield return new WaitForSeconds(_createDelay);
                        waitedTruck.GetComponent<BoxCollider>().enabled = true;
                        waitedTruck.GetComponent<VehicleAI>().vehicleStatus = Status.GO;
                    }

                    else
                    {
                        Debug.LogError("Waited truck not found: " + truckName);
                    }
                }
                duplivatedTruckCount++;
            }
        }

        
        // Tag 존재하는지 확인하는 함수
        private bool ExistTag(string _tagName)
        {
            foreach(string existingTag in UnityEditorInternal.InternalEditorUtility.tags)
            {
                if(existingTag == _tagName)
                {
                    return true;
                }
            }

            return false;
        }
    
    }
}