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
        // 파일 저장 위치
        [SerializeField] private string filePath = "Assets/result.csv";

        // StopWatch
        public Stopwatch totalWatch;
        public Stopwatch stationWatch;

        public List<float> stationWatchList;
        public float totalTime;


        // Truck Information
        private TruckInfo truckInfo;
        private Vector3 truckOrigin;
        private Vector3 truckDestination;
        
        
    
        void Start()
        {
            totalWatch = new Stopwatch();
            stationWatch = new Stopwatch();
            // TimerStart(totalWatch);
            // TimerStart(stationWatch);
            
            stationWatchList = new List<float>();

            // Get truck origin and destination
            truckInfo = this.gameObject.GetComponent<TruckInfo>();
            GetTruckInformation(truckInfo);
            // departureTime = totalWatch.ElapsedMilliseconds / 1000f;

            totalWatch.Start();
            stationWatch.Start();
        }

        public void TimerStart(Stopwatch _watch)
        {
            _watch = new Stopwatch();
            _watch.Start();
        }

        public float TimerStop(Stopwatch _watch)
        {
            _watch.Stop();
            // Convert milliseconds to minutes
            // arrivalTime = _watch.ElapsedMilliseconds / 60000f; 

            return _watch.ElapsedMilliseconds / 1000f;
        }


        private void GetTruckInformation(TruckInfo _truckInfo)
        {
            truckOrigin = _truckInfo.origin;
            truckDestination = _truckInfo.destination;
        }
        
       

        private void SaveToCSV(string _filePath, string _truckName, string _routeName, Vector3 _origin, Vector3 _destination, List<float> _arrivalTimeList)
        {
            // Check if the CSV file exists
            if(!File.Exists(_filePath))
            {
                // Create a new CSV file and write the data
                using (StreamWriter sw = File.CreateText(_filePath))
                {
                    string header = "Truck_id, Route_id, Origin, Destination, Arrival Time";

                    // Write the header and data to the CSV file
                    sw.WriteLine(header);
                    // sw.WriteLine(newLine);
                }
            }

            // Read the existing content of the CSV file
            string[] lines = File.ReadAllLines(_filePath);
            // Append the new data to the content
            string newLine = string.Format("{0},{1},{2},{3},{4}",
                _truckName, _routeName, _origin, _destination, _arrivalTimeList);
            string updatedContent = string.Join("\n", lines) + "\n" + newLine;

            // Write the updated content back to the CSV file
            File.WriteAllText(_filePath, updatedContent);

        }

        
    }
}