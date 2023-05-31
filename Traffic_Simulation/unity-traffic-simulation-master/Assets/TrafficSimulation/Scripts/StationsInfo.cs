using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace TrafficSimulation{
    public class StationsInfo : MonoBehaviour
    {
        public int finishedVehicle_Count_toRight;
        public int finishedVehicle_Count_toLeft;

        private List<GameObject> processingVehicles_List;

        private GameObject vehicle;
        private VehicleAI thisVehicleAI;
        private int thisVehicle_processStatus;
        private Vector3 thisVehicle_Destination;
        private Vector3 nowPosition;
        private List<Vector3> originalPositions;

        private Stopwatch vehicle_TotalWatch;

        private float slowingTime = 3f;
        private float moveDelay = 1f;
        private float processTime = 5f;
        

        private List<Vector3> vehicle_pickupPositions;
        private List<Vector3> vehicle_dropPositions;

        private float rigthToLeft = 15f;
        private float leftToRight = 30f;
        private float checkRange = 20f;

        private Collider vehicleCollider;


        // Start is called before the first frame update
        void Start()
        {   
            finishedVehicle_Count_toRight = 0;
            finishedVehicle_Count_toLeft = 0;
            processingVehicles_List = new List<GameObject>();
            // vehiclesInStation = new List<GameObject>();
        }

        
        void OnTriggerEnter(Collider _other)
        {
            vehicle = _other.gameObject;

            thisVehicleAI = vehicle.GetComponent<VehicleAI>();
            
            Timer vehicle_Info = vehicle.GetComponent<Timer>();

            thisVehicle_processStatus = vehicle_Info.processStatus;
            vehicle_pickupPositions = vehicle_Info.pickupPositions;
            vehicle_dropPositions = vehicle_Info.dropPositions;
            thisVehicle_Destination = vehicle_Info.destinationPos;

            if(thisVehicle_processStatus == 0 && ( CompareStations(vehicle_pickupPositions, this.name) || CompareStations(vehicle_dropPositions, this.name)) )
            {   
                StartCoroutine(ReduceSpeed(vehicle, slowingTime));
                thisVehicleAI.vehicleStatus = Status.STOP;

                // save the original position of the vehicle
                originalPositions.Add(vehicle.transform.position);

                // move to the station for processing
                StartCoroutine(MoveToProcess(slowingTime + moveDelay));
                
                vehicle_Info.processStatus = 1;
                
                // destination이 아닌 station에 도착했을 때
                if(!CompareDestination(thisVehicle_Destination, this.name))
                {   
                    float workingTime = slowingTime + moveDelay + processTime;
                    // Processing: Wait for the process time
                    StartCoroutine(Processing(workingTime));
                }

                // when vehicle arrives at the destination
                else
                {   
                    vehicle_TotalWatch = vehicle_Info.totalWatch;
                    vehicle_Info.TimerStop(vehicle_TotalWatch);
                }
            }
        }

        void OnTriggerExit(Collider _other)
        {
            Timer vehicle_Info = _other.gameObject.GetComponent<Timer>();
            vehicle_Info.processStatus = 0;
        }

        private System.Collections.IEnumerator ReduceSpeed(GameObject _vehicle, float _slowingTime)
        {   
            UnityEngine.Debug.Log(vehicle+ " reduces Speed");
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
        

        private bool CompareStations(List<Vector3> stationList, string _stationName)
        {
            // Convert the vectorList to a List<string> using LINQ
            List<string> stationStrList = stationList.Select(v => v.ToString()).ToList();
            return stationStrList.Contains(_stationName);
        }
        
        private bool CompareDestination(Vector3 destiPo, string _stationName)
        {
            // Convert the vectorList to a List<string> using LINQ
            string destiStr = destiPo.ToString();
            return destiStr == _stationName;
        }

        private IEnumerator MoveToProcess(float _moveDelay)
        {
            yield return new WaitForSeconds(_moveDelay);
            processingVehicles_List.Add(vehicle);
      
            nowPosition = vehicle.transform.position;
            vehicleCollider = vehicle.GetComponent<Collider>();

            vehicleCollider.enabled = false;


            // Move to waiting area
            if(vehicle.transform.rotation.y == 90f)
            {   
                UnityEngine.Debug.Log("this truck's rotation : 90, Move to waiting area");
                vehicle.transform.position = nowPosition + new Vector3(0,0,rigthToLeft);
            }

            else
            {
                UnityEngine.Debug.Log("this truck's rotation : 90, Move to waiting area");
                vehicle.transform.position = nowPosition + new Vector3(0,0,leftToRight);
            }
        }

        private IEnumerator Processing(float _processTime)
        {   
            yield return new WaitForSeconds(_processTime);
            GameObject nowVehicle = processingVehicles_List[0];
            UnityEngine.Debug.Log(nowVehicle + " Processing Done");
            
            // finishedVehicle_Count += 1;
            CalculateFinishedVehicle(nowVehicle);
            
            // The delay between checks for trucks
            float checkDelay = 0.5f; 

            while(ExistAnyTruck(checkRange, nowPosition))
            {
                yield return new WaitForSeconds(checkDelay);
            }

            // Move to original position
            UnityEngine.Debug.Log(nowVehicle + " moves to original position");

            RemoveFinishedVehicle(nowVehicle);
            // finishedVehicle_Count -= 1;
        
            processingVehicles_List.RemoveAt(0);

            // Return to original position
            nowVehicle.transform.position = nowPosition;
            nowVehicle.GetComponent<Collider>().enabled = true;

            nowVehicle.GetComponent<VehicleAI>().vehicleStatus = Status.GO;
        }

        private bool ExistAnyTruck(float _checkRange, Vector3 _currentPosition)
        {
            // The range on the x-axis to check for vehicles

            // Perform a raycast to check for vehicles
            RaycastHit hit;

            bool rightSide = Physics.Raycast(_currentPosition, Vector3.right, out hit, _checkRange);
            bool leftSide = Physics.Raycast(_currentPosition, Vector3.left, out hit, _checkRange);

            return rightSide || leftSide;
        }

        private void CalculateFinishedVehicle(GameObject _vehicle)
        {
            if(_vehicle.transform.rotation.y == 90f)
            {
                finishedVehicle_Count_toRight += 1;
            }

            else
            {
                finishedVehicle_Count_toLeft += 1;
            }
        }

        private void RemoveFinishedVehicle(GameObject _vehicle)
        {
            if(_vehicle.transform.rotation.y == 90f)
            {
                finishedVehicle_Count_toRight -= 1;
            }

            else
            {
                finishedVehicle_Count_toLeft -= 1;
            }
        }
    }
}