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
        // public float processTime = 8f;
        // public float slowingTime = 3f; 
        public float speedTime = 10f;
        public float maxSpeed = 20f;
        
        // private GameObject vehicle;
        // private VehicleAI thisVehicleAI;

        public Stopwatch totalWatch;
        public Stopwatch stationWatch;

        // 파일 저장 위치
        // [SerializeField] private string filePath = "Assets/result.csv";

        private string truckName;
        private string routeName;

        
        private float departureTime;
        private float arrivalTime;
        private float stationArrivalTime;

        public List<Vector3> pickupPositions;
        public List<Vector3> dropPositions;
        // [SerializeField] private Vector3 destinationPos;
        public Vector3 destinationPos;



        private static List<CreateTruckData> truckData_List = CreateTruck.truckDataList;
        

        // public static int finishedVehicle_Count;

        public int processStatus;
        // private float rigthToLeft = 15f;
        // private float leftToRight = 30f;
        // private float checkRange = 20f;

        private Vector3 nowPosition;
        // private Collider vehicleCollider;

        void Start()
        {
            totalWatch = new Stopwatch();
            stationWatch = new Stopwatch();

            departureTime = totalWatch.ElapsedMilliseconds / 1000f;

            totalWatch.Start();
            stationWatch.Start();

            GetTruckInformation(truckData_List);
            processStatus = 0;
        }


        public void TimerStop(Stopwatch _watch)
        {
            _watch.Stop();
            // Convert milliseconds to minutes
            arrivalTime = _watch.ElapsedMilliseconds / 60000f; 
            // UnityEngine.Debug.Log(vehicle + "Time elapsed: " + arrivalTime + " min");
        }

        // void OnTriggerEnter(Collider _other)
        // {
        //     vehicle = this.gameObject;

        //     thisVehicleAI = vehicle.GetComponent<VehicleAI>();
            
        //     vehicleCollider = vehicle.GetComponent<Collider>();

        //     // if(finishedVehicle_Count > 0 && vehicleStatus == 0)
        //     // {
        //     //     StartCoroutine(ReduceSpeed(vehicle, slowingTime));
        //     //     thisVehicleAI.vehicleStatus = Status.STOP;
        //     // }

        //     if(vehicleStatus == 0 && ( CompareStations(pickupPositions, _other.name) || CompareStations(dropPositions, _other.name)) )
        //     {   
        //         StartCoroutine(ReduceSpeed(vehicle, slowingTime));
        //         thisVehicleAI.vehicleStatus = Status.STOP;

        //         StartCoroutine(MoveToProcess(slowingTime + 1f));
                
        //         vehicleStatus = 1;
                
        //         // destination이 아닌 station에 도착했을 때
        //         if(!CompareDestination(destinationPos, _other.name))
        //         {
        //             // Processing: Wait for the process time
        //             StartCoroutine(Processing(processTime, _other));
        //         }

        //         // when vehicle arrives at the destination
        //         else
        //         {
        //             TimerStop(totalWatch);
        //         }
        //     }

        // }

        
        // void OnTriggerExit(Collider _other)
        // {
        //     vehicleStatus = 0;
        // }


        
        // private System.Collections.IEnumerator ReduceSpeed(GameObject _vehicle, float _slowingTime)
        // private System.Collections.IEnumerator ReduceSpeed(GameObject _vehicle, float _slowingTime)
        // {   
        //     UnityEngine.Debug.Log("ReduceSpeed");
        //     Rigidbody rb = _vehicle.GetComponent<Rigidbody>();
            
        //     Vector3 initialVelocity = rb.velocity;
        //     float elapsedTime = 0f;

        //     while (elapsedTime < _slowingTime)
        //     {
        //         rb.velocity = Vector3.Lerp(initialVelocity, Vector3.zero, elapsedTime / _slowingTime);
        //         elapsedTime += Time.deltaTime;
        //         yield return null;
        //     }

        //     // thisVehicleAI.vehicleStatus = Status.STOP;
        //     rb.velocity = Vector3.zero; // Ensure velocity is set to zero
        // }

        // private IEnumerator IncreaseSpeed(GameObject _vehicle, float _speedTime, float _maxSpeed)
        // {   
        //     UnityEngine.Debug.Log("IncreaseSpeed");
        //     thisVehicleAI.vehicleStatus = Status.GO;
            
        //     Rigidbody rb = _vehicle.GetComponent<Rigidbody>();
        //     // rb.centerOfMass = new Vector3(0f, -5f, 0f);

        //     Vector3 initialVelocity = rb.velocity;
        //     float elapsedTime = 0f;

        //     while (elapsedTime < _speedTime)
        //     {
        //         // rb.velocity = Vector3.Lerp(Vector3.zero, initialVelocity, elapsedTime / _speedTime) + Vector3.forward * _maxSpeed;
        //         rb.velocity = Vector3.Lerp(Vector3.zero, initialVelocity + Vector3.forward * _maxSpeed, elapsedTime / _speedTime);
        //         elapsedTime += Time.deltaTime;
        //         yield return null;
        //     }

        //     rb.velocity = initialVelocity + Vector3.forward * _maxSpeed; // Ensure velocity reaches the desired speed
        // }

        private void GetTruckInformation(List<CreateTruckData> dataList)
        {
            // Find the CreateTruckData object with the specific name
            CreateTruckData truckData = dataList.FirstOrDefault(data => data.Name == this.name);

            pickupPositions = truckData.PickupStations;
            dropPositions = truckData.DropStations;

            destinationPos = dropPositions.Last();
        }
        
        // private bool CompareStations(List<Vector3> stationList, string otherName)
        // {
        //     // Convert the vectorList to a List<string> using LINQ
        //     List<string> stationStrList = stationList.Select(v => v.ToString()).ToList();
        //     return stationStrList.Contains(otherName);
        // }

        // private bool CompareDestination(Vector3 destiPo, string otherName)
        // {
        //     // Convert the vectorList to a List<string> using LINQ
        //     string destiStr = destiPo.ToString();
        //     return destiStr == otherName;
        // }

        // After moveDelay seconds, move to waiting area
        // private IEnumerator MoveToProcess(float _moveDelay)
        // {
        //     yield return new WaitForSeconds(_moveDelay);

        //     nowPosition = vehicle.transform.position;

        //     vehicleCollider.enabled = false;

        //     // Move to waiting area
        //     if(vehicle.transform.rotation.y == 90f)
        //     {   
        //         UnityEngine.Debug.Log("this truck's rotation : 90, Move to waiting area");
        //         vehicle.transform.position = nowPosition + new Vector3(0,0,rigthToLeft);
        //     }

        //     else
        //     {
        //         UnityEngine.Debug.Log("this truck's rotation : 90, Move to waiting area");
        //         vehicle.transform.position = nowPosition + new Vector3(0,0,leftToRight);
        //     }
        // }


        // // After processTime seconds, move to original position
        // private IEnumerator Processing(float _processTime, Collider _other)
        // {   
        //     yield return new WaitForSeconds(_processTime);
        //     UnityEngine.Debug.Log("Processing Done");
            
        //     // _other.gameObject.GetComponent<StationsInfo>().finishedVehicle_Count += 1;
        //     // finishedVehicle_Count += 1;
            
        //     // The delay between checks for trucks
        //     float checkDelay = 0.1f; 

        //     while(ExistAnyTruck(checkRange))
        //     {
        //         yield return new WaitForSeconds(checkDelay);
        //     }

        //     // Move to original position
        //     UnityEngine.Debug.Log("Move to original position");

        //     // _other.gameObject.GetComponent<StationsInfo>().finishedVehicle_Count -= 1;
        //     // finishedVehicle_Count -= 1;
        //     // UnityEngine.Debug.Log(_other.name + " finishedVehicle_Count : " + _other.gameObject.GetComponent<StationsInfo>().finishedVehicle_Count);

        //     // Return to original position 
        //     vehicle.transform.position = nowPosition;
        //     vehicleCollider.enabled = true;

        //     thisVehicleAI.vehicleStatus = Status.GO;
        // }
        
        

        // private bool ExistAnyTruck(float _checkRange)
        // {
        //     Vector3 currentPosition = this.transform.position;
        //     // The range on the x-axis to check for vehicles

        //     // Perform a raycast to check for vehicles
        //     RaycastHit hit;

        //     bool rightSide = Physics.Raycast(currentPosition, Vector3.right, out hit, _checkRange);
        //     bool leftSide = Physics.Raycast(currentPosition, Vector3.left, out hit, _checkRange);

        //     return rightSide || leftSide;
        // }

        // private IEnumerator WaitForTruckClear()
        // {
        //     UnityEngine.Debug.Log("WaitForTruckClear");
        //     // The delay between checks for trucks
        //     float checkDelay = 0.1f;

        //     UnityEngine.Debug.Log("ExistAnyTruck : " + ExistAnyTruck());
        //     while(ExistAnyTruck())
        //     {
        //         yield return new WaitForSeconds(checkDelay);
        //     }

        //     // No trucks detected, activate the object
        //     vehicle.SetActive(true);
        //     thisVehicleAI.vehicleStatus = Status.GO;
        //     UnityEngine.Debug.Log(vehicle + " Appear");
        // }


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