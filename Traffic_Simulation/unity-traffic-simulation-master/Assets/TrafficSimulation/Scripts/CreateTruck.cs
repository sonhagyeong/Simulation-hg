using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

namespace TrafficSimulation{
    public class CreateTruck : MonoBehaviour
    {

        [SerializeField] private string truckFilePath = "C:\\Users\\USER\\workspace\\Trucks.csv";


        // private List<string> truckDataList = new List<string>();
        public static List<CreateTruckData> truckDataList = new List<CreateTruckData>();

        private static float truckRotation_y;

        // Station Parameters
        private static Vector3 stationSize = new Vector3(75,10,75);
        private static float stationPos_y = stationSize.y/2;

        // Start is called before the first frame update
        void Start()
        {
            ReadFile(truckFilePath);
            CreateStations(truckDataList);
            if(ExistRoute(truckDataList))
            {
                Debug.Log("All routes exist");
                CreateTrucks(truckDataList);
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
                    
                    List<Vector3> pickupStations = new List<Vector3>();
                    List<Vector3> dropStations = new List<Vector3>();   
                    
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

                            if (i % 2 == 0)
                            {
                                pickupStations.Add(station);
                            }

                            else
                            {
                                dropStations.Add(station);
                            }
                        }
                        
                        else
                        {
                            break; // No more values in the next column, exit the loop
                        }
                    }

                    truckData.CreateData(truckName, truckRoute, pickupStations, dropStations);
                    truckDataList.Add(truckData);
                }
            }

        }

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


        public static void CreateTrucks(List<CreateTruckData> dataList)
        {
            foreach (CreateTruckData data in dataList)
            {   
                // Generate a random number between 1 and 4 (inclusive)
                int randomNumber = Random.Range(1, 5);
                string prefabPath = "Truck" + randomNumber.ToString();
                string routeName = "Route-" + data.Route;
                GameObject truckPrefab = Resources.Load(prefabPath) as GameObject;

                if (truckPrefab != null)
                {
                    GameObject truck = Instantiate(truckPrefab);
                    truck.name = data.Name;
                    Transform routeTransform = GameObject.Find(routeName).transform;

                    Vector3 routePosition = routeTransform.Find(routeName + "/Waypoint-0").transform.position;
                    truck.transform.position = new Vector3(routePosition.x, 0f, routePosition.z);
                    truck.transform.rotation = Quaternion.Euler(0, GetTruckRotation(routeTransform, routeName), 0);

                    // Set the truck's route
                    truck.GetComponent<VehicleAI>().trafficSystem = GameObject.Find(routeName).GetComponent<TrafficSystem>();
                }

                else
                {
                    Debug.LogError("Truck prefab not found: " + prefabPath);
                }

            }
        }

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

        public static void CreateStations(List<CreateTruckData> dataList)
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
                if(data.PickupStations.Count > 0)
                {   
                    for(int i = 0; i <data.PickupStations.Count; i++)
                    {
                        Vector3 pickupStation = data.PickupStations[i];
                        Vector3 dropStation = data.DropStations[i];

                        string pickupStationName = pickupStation.ToString();
                        string dropStationName = dropStation.ToString();

                        // Create a pickup station
                        if(!ExistStation(stationsOB.transform, pickupStationName))
                        {
                            GameObject pickupOB = new GameObject(pickupStationName);
                            pickupOB.transform.parent = stationsOB.transform;
                            pickupOB.transform.position = pickupStation;
                            AddCollider(pickupOB); 
                        }
                        
                        // Create a drop station
                        if(!ExistStation(stationsOB.transform, dropStationName))
                        {
                            GameObject dropOB = new GameObject(dropStationName);
                            dropOB.transform.parent = stationsOB.transform;
                            dropOB.transform.position = dropStation; 
                            AddCollider(dropOB);
                        }
                    }
                }
                
                else
                {
                    Debug.LogError("No pickup stations found for truck: " + data.Name);
                }

            }
        }

        public static bool ExistStation(Transform stationsTransform, string stationName)
        {   
            bool isExist = true;
            if(stationsTransform.Find(stationName) == null)
            {
                isExist = false;
            }

            return isExist;
        }

        public static void AddCollider(GameObject obj)
        {
            BoxCollider bc = obj.AddComponent<BoxCollider>();
            bc.size = stationSize;
            Vector3 center = bc.center;
            center.y = stationPos_y;
            bc.center = center;
            bc.isTrigger = true;
        }
        
    }
}