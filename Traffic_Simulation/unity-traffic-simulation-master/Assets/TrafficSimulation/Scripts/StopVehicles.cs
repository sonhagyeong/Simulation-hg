using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSimulation{
    public class StopVehicles : MonoBehaviour
    {
        [SerializeField] private static int finishedCount;
        [SerializeField] private string nearStationName;

        public float slowingTime = 3f; 
        private float rightNum = 30f;

        private float axis_z = 7.5f;

        private GameObject vehicle;
        private VehicleAI thisVehicleAI;
        public List<GameObject> vehicleQueueList;

        void Start()
        {
            GetStationName();
            vehicleQueueList = new List<GameObject>();
        }

        void OnTriggerEnter(Collider _other)
        {   
            GameObject nearStationOB = GameObject.Find("Stations/" + nearStationName);

            // finishedCount = nearStationOB.GetComponent<StationsInfo>().finishedVehicle_Count;
            
            vehicle = _other.gameObject;

            thisVehicleAI = vehicle.GetComponent<VehicleAI>();


            if(finishedCount > 0)
            {
                StartCoroutine(ReduceSpeed(vehicle, slowingTime));
                thisVehicleAI.vehicleStatus = Status.STOP;
                vehicleQueueList.Add(vehicle);
            }
        
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

            // if(stopPosition.z == -axis_z)
            // {   
            //     Vector3 nearStationPos = new Vector3(stopPosition.x + rightNum, 0f, stopPosition.z + axis_z);
            //     nearStationName = nearStationPos.ToString();
            // }

            // else
            // {
            //     Vector3 nearStationPos = new Vector3(stopPosition.x - rightNum, 0f, stopPosition.z - axis_z);
            //     nearStationName = nearStationPos.ToString();
            // }
        }

        private void GetFinishedCount(GameObject _vehicle, GameObject _nearStation)
        {
            if(_vehicle.transform.rotation.y == 90f)
            {
                // int finishedCount = _nearStation.GetComponent<StationsInfo>().finishedVehicle_Count_toRight;
            }
        }
    }
}