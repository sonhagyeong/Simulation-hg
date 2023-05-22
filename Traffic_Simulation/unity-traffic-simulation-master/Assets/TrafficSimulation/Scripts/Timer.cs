using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using System.IO;


namespace TrafficSimulation{
    public class Timer : MonoBehaviour
    {
        public float disappearDelay = 3f;
        public float slowingTime = 2f; 
        
        private GameObject vehicle;
        private VehicleAI thisVehicleAI;

        private Stopwatch watch;

        public string filePath = "Assets/result.csv";
        private string truckName;
        private string routeName;

        private string destination;
        private float departureTime;
        private float arrivalTime;

        void Start()
        {
            watch = new Stopwatch();
            departureTime = watch.ElapsedMilliseconds / 1000f;
            watch.Start();
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
            // UnityEngine.Debug.Log("other.gameObject : " + other.gameObject.name);
            thisVehicleAI = vehicle.GetComponent<VehicleAI>();
            routeName = thisVehicleAI.trafficSystem.name;
            destination = other.name;

            // // compare route and palce number
            bool numbersMatch = CompareNumbersAtEnd(routeName, other.name);
            if(numbersMatch)
            {
                StartCoroutine(ReduceSpeed(vehicle));
                thisVehicleAI.vehicleStatus = Status.STOP;
                TimerStop();
                SaveToCSV(filePath, truckName, routeName, destination, departureTime, arrivalTime);
                Invoke("Disappear", disappearDelay);
            }
        }

        private bool CompareNumbersAtEnd(string string1, string string2)
        {
            int number1, number2;
            bool success1 = Int32.TryParse(string1.Substring(string1.LastIndexOf('-') + 1), out number1);
            bool success2 = Int32.TryParse(string2.Substring(string2.LastIndexOf('-') + 1), out number2);

            if (success1 && success2)
            {
                return number1 == number2;
            }
            else
            {
                // Debug.LogWarning("Unable to parse numbers at the end of the strings.");
                return false;
            }
        }

        private System.Collections.IEnumerator ReduceSpeed(GameObject _vehicle)
        {
            Rigidbody rb = _vehicle.GetComponent<Rigidbody>();
            
            Vector3 initialVelocity = rb.velocity;
            float elapsedTime = 0f;

            while (elapsedTime < slowingTime)
            {
                rb.velocity = Vector3.Lerp(initialVelocity, Vector3.zero, elapsedTime / slowingTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            rb.velocity = Vector3.zero; // Ensure velocity is set to zero
        }

        private void Disappear()
        {   
            if(vehicle != null)
            {
                // Deactivate or destroy the object to make it disappear
                vehicle.SetActive(false);
                // Alternatively, you can destroy the object using Destroy(gameObject);
            }

            else
            {
                UnityEngine.Debug.LogError("No target object assigned.");
            }
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