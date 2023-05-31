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

        public Vector3 destinationPos;

        private static List<CreateTruckData> truckData_List = CreateTruck.truckDataList;
        
        public int processStatus;
        private Vector3 nowPosition;

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


        private void GetTruckInformation(List<CreateTruckData> dataList)
        {
            // Find the CreateTruckData object with the specific name
            CreateTruckData truckData = dataList.FirstOrDefault(data => data.Name == this.name);

            pickupPositions = truckData.PickupStations;
            dropPositions = truckData.DropStations;

            destinationPos = dropPositions.Last();
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