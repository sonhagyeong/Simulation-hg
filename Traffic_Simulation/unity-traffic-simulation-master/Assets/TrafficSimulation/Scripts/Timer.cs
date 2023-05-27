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
        public float waitSecond = 8f;
        public float slowingTime = 2f; 
        public float speedTime = 4f;
        public float maxSpeed = 20f;
        
        private GameObject vehicle;
        private VehicleAI thisVehicleAI;

        private Stopwatch watch;

        // 파일 저장 위치
        [SerializeField] private string filePath = "Assets/result.csv";

        private string truckName;
        private string routeName;

        
        private float departureTime;
        private float arrivalTime;

        private List<Vector3> pickupPositions;
        private List<Vector3> dropPositions;
        [SerializeField] private Vector3 destinationPos;

        private List<GameObject> vehiclesInStation;

        private static List<CreateTruckData> truckData_List = CreateTruck.truckDataList;
        
        private int stationCount;

        void Start()
        {
            watch = new Stopwatch();
            departureTime = watch.ElapsedMilliseconds / 1000f;
            watch.Start();
            GetTruckInformation(truckData_List);
            vehiclesInStation = new List<GameObject>();
            stationCount = 0;
        }

        public void TimerStop()
        {
            watch.Stop();
            arrivalTime = watch.ElapsedMilliseconds / 1000f;
            UnityEngine.Debug.Log("Time elapsed: " + arrivalTime + " s");
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
                // StartCoroutine(ReduceSpeed(vehicle, slowingTime));
                // thisVehicleAI.vehicleStatus = Status.STOP;
                StartCoroutine(ReduceSpeed(vehicle, slowingTime, thisVehicleAI));
                // disappear after desapperDelay seconds
                Invoke("Disappear", disappearDelay);

                vehiclesInStation.Remove(vehicle);
                

                stationCount++;
                
                Invoke("Appear", waitSecond);
                thisVehicleAI.vehicleStatus = Status.GO;

                // // disappear while waitSecond seconds
                // StartCoroutine(DisableObjectCoroutine(waitSecond, destinationPos, other, stationCount, vehicle, thisVehicleAI, vehiclesInStation));

                // UnityEngine.Debug.Log("stationCount : " + stationCount);

                // Invoke("Appear", waitSecond);
                
                // thisVehicleAI.vehicleStatus = Status.GO;
                // StartCoroutine(IncreaseSpeed(vehicle, speedTime, maxSpeed));
                

                // if(CompareDestination(destinationPos, other.name))
                // {   
                //     UnityEngine.Debug.Log("Destination reached");
                //     TimerStop();
                //     // SaveToCSV(filePath, truckName, routeName, destinationPos, departureTime, arrivalTime);
                // }

                // else
                // {   
                //     if(vehiclesInStation.Count == 0)
                //     {  
                //         UnityEngine.Debug.Log("stationCount : " + stationCount);
                //         vehicle.SetActive(true);
                //         thisVehicleAI.vehicleStatus = Status.GO;
                //         UnityEngine.Debug.Log("Go");
                //         vehiclesInStation.Add(vehicle);
                //     }
                // }

            }

            // else if(CompareStations(pickupPositions, other.name) || CompareStations(dropPositions, other.name))
            // {
            //     Invoke("Appear", waitSecond);
            //     StartCoroutine(IncreaseSpeed(vehicle, speedTime, maxSpeed));
            //     thisVehicleAI.vehicleStatus = Status.GO;
            // }
        }

        void OnTriggerExit(Collider _other)
        {
            vehiclesInStation.Remove(vehicle);
            stationCount = 0;
        }

        private IEnumerator DisableObjectCoroutine(float waitsecond, Vector3 _position, Collider _other, int _stationCount, GameObject _vehicle, VehicleAI _thisVehicleAI, List<GameObject> _vehiclesInStation)
        {   
            UnityEngine.Debug.Log("DisableObjectCoroutine");

            // yield return new WaitForSeconds(waitsecond); // Wait for 5 seconds
            yield return new WaitForSeconds(5.0f);

            if(CompareDestination(_position, _other.name))
            {   
                UnityEngine.Debug.Log("Destination reached");
                TimerStop();
                // SaveToCSV(filePath, truckName, routeName, destinationPos, departureTime, arrivalTime);
            }

            else
            {   
                UnityEngine.Debug.Log("Here is not destination! ");
                if(_vehiclesInStation.Count == 0)
                {  
                    UnityEngine.Debug.Log("_stationCount : " + _stationCount);
                    _vehicle.SetActive(true);
                    _thisVehicleAI.vehicleStatus = Status.GO;
                    UnityEngine.Debug.Log("Go");
                    _vehiclesInStation.Add(_vehicle);
                }
            }
        }

        
        // private System.Collections.IEnumerator ReduceSpeed(GameObject _vehicle, float _slowingTime)
        private System.Collections.IEnumerator ReduceSpeed(GameObject _vehicle, float _slowingTime, VehicleAI _thisVehicleAI)
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

            _thisVehicleAI.vehicleStatus = Status.STOP;
            rb.velocity = Vector3.zero; // Ensure velocity is set to zero

            
        }

        private IEnumerator ActivateDelayedCoroutine(GameObject _vehicle, float _delay)
        {   
            UnityEngine.Debug.Log("ActivateDelayedCoroutine");
            yield return new WaitForSeconds(_delay); // Delay before activating the object
            _vehicle.SetActive(true);
            UnityEngine.Debug.Log("Activate");

            // Code to increase speed or perform any other desired actions
        }
        private IEnumerator IncreaseSpeed(GameObject _vehicle, float _speedTime, float _maxSpeed)
        {
            Rigidbody rb = _vehicle.GetComponent<Rigidbody>();
            
            Vector3 initialVelocity = rb.velocity;
            float elapsedTime = 0f;

            while (elapsedTime < _speedTime)
            {
                rb.velocity = Vector3.Lerp(Vector3.zero, initialVelocity, elapsedTime / _speedTime) + Vector3.forward * _maxSpeed;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            rb.velocity = initialVelocity + Vector3.forward * 20f; // Ensure velocity reaches the desired speed
        }
        private void Disappear()
        {   
            if(vehicle != null)
            {   
                UnityEngine.Debug.Log("Disappear");
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
                UnityEngine.Debug.Log("Appear");
                vehicle.SetActive(true);
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

        private void SaveToCSV(string filePath, string truckName, string routeName, string destination, float departureTime, float arrivalTime)
        {
            // Check if the CSV file exists
            if(!File.Exists(filePath))
            {
                // Create a new CSV file and write the data
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    string header = "Truck Name, Route, Destination, Departure Time, Arrival Time";

                    // Write the header and data to the CSV file
                    sw.WriteLine(header);
                    // sw.WriteLine(newLine);
                }
            }

            // Read the existing content of the CSV file
            string[] lines = File.ReadAllLines(filePath);
            // Append the new data to the content
            string newLine = string.Format("{0},{1},{2},{3},{4}",
                truckName, routeName, destination, departureTime, arrivalTime);
            string updatedContent = string.Join("\n", lines) + "\n" + newLine;

            // Write the updated content back to the CSV file
            File.WriteAllText(filePath, updatedContent);

        }

        
    }
}