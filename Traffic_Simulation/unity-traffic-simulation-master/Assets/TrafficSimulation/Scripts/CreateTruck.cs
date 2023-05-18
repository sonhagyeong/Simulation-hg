using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
namespace TrafficSimulation{
    public class CreateTruck : MonoBehaviour
    {

        [SerializeField] private string truckFilePath = "C:\\Users\\USER\\workspace\\Trucks.csv";

        // private List<string> truckDataList = new List<string>();
        private static List<CreateTruckData> truckDataList = new List<CreateTruckData>();

        // Start is called before the first frame update
        void Start()
        {
            ReadFile(truckFilePath);
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

                    string truckName = values[0];
                    string truckRoute = values[1];

                    CreateTruckData truckData = ScriptableObject.CreateInstance<CreateTruckData>();
                    truckData.CreateData(truckName, truckRoute);
                    truckDataList.Add(truckData);
                }
            }

        }

        public static bool ExistRoute(List<CreateTruckData> dataList)
        {
            // Print the truck data
            foreach (CreateTruckData data in dataList)
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

                GameObject truckPrefab = Resources.Load(prefabPath) as GameObject;

                if (truckPrefab != null)
                {
                    GameObject truck = Instantiate(truckPrefab);
                    truck.name = "Truck-" + data.Name.ToString();
                    Transform routeTransform = GameObject.Find("Route-" + data.Route).transform;
                    truck.transform.position = routeTransform.Find("Segments/Route-" + data.Route).transform.position;

                    // Debug.Log("Truck.name: " + truck.name);
                    // Debug.Log("Truck.transform.position: " + truck.transform.position);

                    // Set the truck's route
                    truck.GetComponent<VehicleAI>().trafficSystem = GameObject.Find("Route-" + data.Route).GetComponent<TrafficSystem>();
                    Debug.Log(truck.GetComponent<VehicleAI>().trafficSystem.name);
                }

                else
                {
                    Debug.LogError("Truck prefab not found: " + prefabPath);
                }

            }
        }
    }
}