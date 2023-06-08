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
            }
        }

        // Read the existing content of the CSV file
        string[] lines = File.ReadAllLines(_filePath);

        // Convert the List<float> to a comma-separated string
        UnityEngine.Debug.Log(this.name + "_arrivalTimeList.Count: " + _arrivalTimeList.Count);
        string arrivalTimeValues = string.Join(",", _arrivalTimeList);
        UnityEngine.Debug.Log("arrivalTimeValues: " + arrivalTimeValues);

        // Convert the Vector3 values to strings without including commas
        string originValue = _origin.ToString().Replace(",", string.Empty);
        string destinationValue = _destination.ToString().Replace(",", string.Empty);

        // Append the new data to the content
        string newLine = string.Format("{0},{1},{2},{3},{4},{5}", _truckName, _routeName, originValue, destinationValue, _totalTime, arrivalTimeValues);

        // Append the new line to the CSV file
        File.AppendAllText(_filePath, newLine + "\n");
    }
}
