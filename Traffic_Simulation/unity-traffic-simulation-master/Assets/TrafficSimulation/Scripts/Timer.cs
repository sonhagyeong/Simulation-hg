using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using UnityEngine;
using System.Linq;


public class Timer : MonoBehaviour
{
    // 파일 저장 위치
    public string filePath = "Assets/result.csv";

    // StopWatch
    public Stopwatch totalWatch;
    public Stopwatch stationWatch;
    // public Stopwatch noReasonStopWatch;

    public List<float> stationWatchList;
    public float totalTime;
    
    
    void Start()
    {
        totalWatch = new Stopwatch();
        stationWatch = new Stopwatch();
        // noReasonStopWatch = new Stopwatch();

        stationWatchList = new List<float>();

        totalWatch.Start();
        stationWatch.Start();
    }

    public float TimerStop(Stopwatch _watch)
    {
        _watch.Stop();
        // Convert milliseconds to minutes
        // arrivalTime = _watch.ElapsedMilliseconds / 60000f; 

        return _watch.ElapsedMilliseconds / 1000f;
    }


    // private void GetTruckInformation(TruckInfo _truckInfo)
    // {
    //     truckOrigin = _truckInfo.origin;
    //     truckDestination = _truckInfo.destination;
    // }
    
    

    public void SaveToCSV(string _filePath, string _truckName, string _routeName, Vector3 _origin, Vector3 _destination, float _totalTime, List<float> _arrivalTimeList)
    {
        // Check if the CSV file exists
        if(!File.Exists(_filePath))
        {
            // Create a new CSV file and write the data
            using (StreamWriter sw = File.CreateText(_filePath))
            {
                string header = "Truck_id, Route_id, Origin, Destination, Total Time, Arrival Time";

                // Write the header and data to the CSV file
                sw.WriteLine(header);
                // sw.WriteLine(newLine);
            }
        }

        // Read the existing content of the CSV file
        string[] lines = File.ReadAllLines(_filePath);

        // Convert the List<float> to a comma-separated string
        string arrivalTimeValues = string.Join(",", _arrivalTimeList);
        UnityEngine.Debug.Log("arrivalTimeValues: " + arrivalTimeValues);

        // Convert the Vector3 values to strings without including commas
        string originValue = _origin.ToString().Replace(",", string.Empty);
        string destinationValue = _destination.ToString().Replace(",", string.Empty);
        // string originValue = $"({_origin.x},{_origin.y},{_origin.z})";
        // string destinationValue = $"({_destination.x},{_destination.y},{_destination.z})";

        // Append the new data to the content
        string newLine = string.Format("{0},{1},{2},{3},{4},{5}", _truckName, _routeName, originValue, destinationValue, _totalTime, arrivalTimeValues);

        // Append the new line to the CSV file
        File.AppendAllText(_filePath, newLine + "\n");
        
        // string updatedContent = string.Join("\n", lines) + "\n" + newLine;

        // // Write the updated content back to the CSV file
        // File.WriteAllText(_filePath, updatedContent);

    }

    
}
