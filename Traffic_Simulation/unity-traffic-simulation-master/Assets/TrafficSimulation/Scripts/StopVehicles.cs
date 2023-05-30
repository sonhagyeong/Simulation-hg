using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSimulation{
    public class StopVehicles : MonoBehaviour
    {
        [SerializeField] private static int vehiclesCount;
        [SerializeField] private string nearStationName;

        public float slowingTime = 3f; 
        private float rightNum = 30f;

        private float axis_z = 7.5f;

        private GameObject vehicle;
        private VehicleAI thisVehicleAI;

        void Start()
        {
            GetStationName();
        }

        void OnTriggerEnter(Collider _other)
        {   
            GameObject nearStationOB = GameObject.Find("Stations/" + nearStationName);

            vehiclesCount = nearStationOB.GetComponent<StationsInfo>().finishedVehicle_Count;
            
            vehicle = _other.gameObject;

            thisVehicleAI = vehicle.GetComponent<VehicleAI>();


            if(vehiclesCount > 0)
            {
                StartCoroutine(ReduceSpeed(vehicle, slowingTime));
                thisVehicleAI.vehicleStatus = Status.STOP;
            }
            // vehiclesCount = StationsInfo.finishedVehicle_Count;

            // Debug.Log("vehiclesCount: " + vehiclesCount);

            // vehicle = _other.gameObject;
            // thisVehicleAI = vehicle.GetComponent<VehicleAI>();

            // if(vehiclesCount > 0)
            // {
            //     ReduceSpeed(vehicle, slowingTime);
            //     thisVehicleAI.vehicleStatus = Status.STOP;
            // }
        }

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

            rb.velocity = Vector3.zero; // Ensure velocity is set to zero
        }

        private void GetStationName()
        {
            Vector3 stopPosition = this.transform.position;

            if(stopPosition.z == -axis_z)
            {   
                Vector3 nearStationPos = new Vector3(stopPosition.x + rightNum, 0f, stopPosition.z + axis_z);
                nearStationName = nearStationPos.ToString();
            }

            else
            {
                Vector3 nearStationPos = new Vector3(stopPosition.x - rightNum, 0f, stopPosition.z - axis_z);
                nearStationName = nearStationPos.ToString();
            }
        }
    }
}