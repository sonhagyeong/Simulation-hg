using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using UnityEngine;
using System.Linq;



namespace TrafficSimulation{
    public class Timer : MonoBehaviour
    {
        public float disappearDelay = 3f;
        public float processTime = 8f;
        public float slowingTime = 2f; 
        public float speedTime = 10f;
        public float maxSpeed = 20f;
        
        private GameObject vehicle;
        private VehicleAI thisVehicleAI;

        private Stopwatch totalWatch;
        private Stopwatch stationWatch;

        // 파일 저장 위치
        [SerializeField] private string filePath = "Assets/result.csv";

        private string truckName;
        private string routeName;

        
        private float departureTime;
        private float arrivalTime;
        private float stationArrivalTime;

        private List<Vector3> pickupPositions;
        private List<Vector3> dropPositions;
        [SerializeField] private Vector3 destinationPos;

        private List<GameObject> vehiclesInStation;

        private static List<CreateTruckData> truckData_List = CreateTruck.truckDataList;
        
        private int stationCount;

        void Start()
        {
            totalWatch = new Stopwatch();
            stationWatch = new Stopwatch();

            departureTime = totalWatch.ElapsedMilliseconds / 1000f;

            totalWatch.Start();
            stationWatch.Start();

            GetTruckInformation(truckData_List);
            vehiclesInStation = new List<GameObject>();
            stationCount = 0;
        }

        public void TimerStop(Stopwatch _watch)
        {
            _watch.Stop();
            // Convert milliseconds to minutes
            arrivalTime = _watch.ElapsedMilliseconds / 60000f; 
            UnityEngine.Debug.Log(vehicle + "Time elapsed: " + arrivalTime + " min");
        }

        void OnTriggerEnter(Collider other)
        {   
            vehicle = this.gameObject;
            truckName = vehicle.name;

            vehiclesInStation.Add(vehicle);

            // UnityEngine.Debug.Log("other.gameObject : " + other.gameObject.name);
            thisVehicleAI = vehicle.GetComponent<VehicleAI>();
            routeName = thisVehicleAI.trafficSystem.name;

            // compare other.name and pickup station
            if(stationCount == 0 &&(CompareStations(pickupPositions, other.name) || CompareStations(dropPositions, other.name)))
            {

                StartCoroutine(ReduceSpeed(vehicle, slowingTime));
                thisVehicleAI.vehicleStatus = Status.STOP;

                // disappear after desapperDelay seconds
                Invoke("Disappear", disappearDelay);

                vehiclesInStation.Remove(vehicle);
                

                stationCount++;
                
                // destination이 아닌 station에 도착했을 때
                if(!CompareDestination(destinationPos, other.name))
                {
                    Invoke("Appear", processTime);
                }

                // when vehicle arrives at the destination
                else
                {
                    TimerStop(totalWatch);
                    // save the result
                    SaveToCSV(filePath, truckName, routeName, destinationPos, departureTime, arrivalTime);
                    
                }
            }
        }

        void OnTriggerExit(Collider _other)
        {
            vehiclesInStation.Remove(vehicle);
            stationCount = 0;
        }


        
        // private System.Collections.IEnumerator ReduceSpeed(GameObject _vehicle, float _slowingTime)
        private System.Collections.IEnumerator ReduceSpeed(GameObject _vehicle, float _slowingTime)
        {   
            UnityEngine.Debug.Log("ReduceSpeed");
            Rigidbody rb = _vehicle.GetComponent<Rigidbody>();
            
            Vector3 initialVelocity = rb.velocity;
            float elapsedTime = 0f;

            while (elapsedTime < _slowingTime)
            {
                rb.velocity = Vector3.Lerp(initialVelocity, Vector3.zero, elapsedTime / _slowingTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // thisVehicleAI.vehicleStatus = Status.STOP;
            rb.velocity = Vector3.zero; // Ensure velocity is set to zero
        }

        private IEnumerator IncreaseSpeed(GameObject _vehicle, float _speedTime, float _maxSpeed)
        {   
            UnityEngine.Debug.Log("IncreaseSpeed");
            thisVehicleAI.vehicleStatus = Status.GO;
            
            Rigidbody rb = _vehicle.GetComponent<Rigidbody>();
            // rb.centerOfMass = new Vector3(0f, -5f, 0f);

            Vector3 initialVelocity = rb.velocity;
            float elapsedTime = 0f;

            while (elapsedTime < _speedTime)
            {
                // rb.velocity = Vector3.Lerp(Vector3.zero, initialVelocity, elapsedTime / _speedTime) + Vector3.forward * _maxSpeed;
                rb.velocity = Vector3.Lerp(Vector3.zero, initialVelocity + Vector3.forward * _maxSpeed, elapsedTime / _speedTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            rb.velocity = initialVelocity + Vector3.forward * _maxSpeed; // Ensure velocity reaches the desired speed
        }

        private void Disappear()
        {   
            if(vehicle != null)
            {   
                UnityEngine.Debug.Log(vehicle + " Disappear");
                // Deactivate or destroy the object to make it disappear
                vehicle.SetActive(false);
                // Alternatively, you can destroy the object using Destroy(gameObject);
            }

            else
            {
                UnityEngine.Debug.LogError("No target object assigned.");
            }
        }

        private void Appear()
        {   
            if(vehicle != null)
            {
                UnityEngine.Debug.Log(vehicle + " Appear");
                vehicle.SetActive(true);
                thisVehicleAI.vehicleStatus = Status.GO;
                // StartCoroutine(IncreaseSpeed(vehicle, speedTime, maxSpeed));
            }

            else
            {
                UnityEngine.Debug.LogError("No target object assigned.");
            }
            
        }

        private void GetTruckInformation(List<CreateTruckData> dataList)
        {
            // Find the CreateTruckData object with the specific name
            CreateTruckData truckData = dataList.FirstOrDefault(data => data.Name == this.name);

            pickupPositions = truckData.PickupStations;
            dropPositions = truckData.DropStations;

            destinationPos = dropPositions.Last();
        }
        
        private bool CompareStations(List<Vector3> stationList, string otherName)
        {
            // Convert the vectorList to a List<string> using LINQ
            List<string> stationStrList = stationList.Select(v => v.ToString()).ToList();
            return stationStrList.Contains(otherName);
        }

        private bool CompareDestination(Vector3 destiPo, string otherName)
        {
            // Convert the vectorList to a List<string> using LINQ
            string destiStr = destiPo.ToString();
            return destiStr == otherName;
        }

        private void SaveToCSV(string _filePath, string _truckName, string _routeName, Vector3 _destination, float _departureTime, float _arrivalTime)
        {
            // Check if the CSV file exists
            if(!File.Exists(_filePath))
            {
                // Create a new CSV file and write the data
                using (StreamWriter sw = File.CreateText(_filePath))
                {
                    string header = "Truck Name, Route, Destination, Departure Time, Arrival Time";

                    // Write the header and data to the CSV file
                    sw.WriteLine(header);
                    // sw.WriteLine(newLine);
                }
            }

            // Read the existing content of the CSV file
            string[] lines = File.ReadAllLines(_filePath);
            // Append the new data to the content
            string newLine = string.Format("{0},{1},{2},{3},{4}",
                _truckName, _routeName, _destination, _departureTime, _arrivalTime);
            string updatedContent = string.Join("\n", lines) + "\n" + newLine;

            // Write the updated content back to the CSV file
            File.WriteAllText(_filePath, updatedContent);

        }

        
    }
}