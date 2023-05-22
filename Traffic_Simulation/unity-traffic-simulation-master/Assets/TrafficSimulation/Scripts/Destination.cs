using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

namespace TrafficSimulation{
    public class Destination : MonoBehaviour
    {       
        // The duration after which the object should disappear
        public float disappearDelay = 3f;
        public float slowingTime = 2f; 

        private Stopwatch watch;

        private GameObject targetObject;

        // Start is called before the first frame update
        void Start()
        {
            
            
            // Transform routeChild = GameObject.Find(routeName + "/" + routeName).transform;
            // int numChild = routeChild.childCount;
            // UnityEngine.Debug.Log("numChild: " + numChild);
            // Vector3 destinationPosition = routeChild.GetChild(numChild - 1).transform.position;
            // UnityEngine.Debug.Log("Destination position: " + destinationPosition);

            // watch = new Stopwatch();
            // watch.Start();
        }

        void OnTriggerEnter(Collider _other)
        {
            targetObject = _other.gameObject;
            VehicleAI targetVehicleAI = targetObject.GetComponent<VehicleAI>();
            string routeName = targetVehicleAI.trafficSystem.name;
            
            bool numbersMatch = CompareNumbersAtEnd(routeName, this.name);

            if(numbersMatch)
            {
                StartCoroutine(ReduceSpeed(targetObject));
                targetVehicleAI.vehicleStatus = Status.STOP;

                targetObject.SetActive(false);

                // Invoke("Disappear", disappearDelay);
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
            // UnityEngine.Debug.Log("Speed Reduce");
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
            // UnityEngine.Debug.Log("Speed reduced to 0.");
        }

        // private void Disappear()
        // {   
        //     if(targetObject != null)
        //     {
        //         // Deactivate or destroy the object to make it disappear
        //         targetObject.SetActive(false);
        //         // Alternatively, you can destroy the object using Destroy(gameObject);
        //     }

        //     else
        //     {
        //         UnityEngine.Debug.LogError("No target object assigned.");
        //     }
        // }

        // private void OnTriggerEnter(Collider other)
        // {   
        //     if(other.name == "Destination")
        //     {
        //         watch.Stop();
        //         float elapsedSeconds = watch.ElapsedMilliseconds / 1000f;
        //         UnityEngine.Debug.Log("Time elapsed: " + elapsedSeconds + " s");
        //     }
        // }
    }
}