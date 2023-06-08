﻿using System.Collections;
using System.Collections.Generic;
// using System.Diagnostics;
using UnityEngine;

namespace TrafficSimulation{

    public enum NowStatus
    {
        WAITING,
        PROCESSING,
        NONE
    }

    public class TruckInfo : MonoBehaviour
    {

        public List<Vector3> truckWorkStations;
        private int truckWorkStationsNum;
        public Vector3 truckOrigin;
        public Vector3 truckDestination;
        public string truckRouteName;
        public int truckStatus;

        
        private GameObject vehicle;
        private VehicleAI thisVehicleAI;

        
        public float short_slowingTime = 1.5f;
        public float long_slowingTime = 5f;

        public float moveDelay = 1f;
        public float processTime = 10f;


        private Vector3 originalPos;
        
        private float toStationNum = 25f;
        private float checkRange_1 = 20f;
        private float checkRange_2 = 1f;
        // 현재 유턴 횟수
        [SerializeField] private int nowTurnNum;
        // public int turnNum;
        public List<Vector3> turnStations;


        private GameObject nowStation;
        private Vector3 nowStationPos;
        private StationsInfo nowStationInfo;
        private int nowStation_FinishedVehicle_toLeft;
        private int nowStation_FinishedVehicle_toRight;

        // To check Station's Status
        private float checkDelay = 0.1f; 

        private Timer truckTimer;

        private ExitPlayMode exitPlayMode;
        private Vector3 startPos;
        private Vector3 firstStationPos;

        private Rigidbody rb;
        private BoxCollider bc;
        public NowStatus nowStatus;
        
        // Start is called before the first frame update
        void Start()
        {   
            truckStatus = 0;
            nowTurnNum = 0;
            
            vehicle = this.gameObject;
            thisVehicleAI = vehicle.GetComponent<VehicleAI>();
            truckTimer = vehicle.GetComponent<Timer>();
            exitPlayMode = GameObject.Find("Roads").GetComponent<ExitPlayMode>();
            truckWorkStationsNum = truckWorkStations.Count;
            rb = vehicle.GetComponent<Rigidbody>();
            bc = vehicle.GetComponent<BoxCollider>();
            nowStatus = NowStatus.NONE;
        }

        void Update()
        {
            if(thisVehicleAI.vehicleStatus == Status.GO && rb.velocity.magnitude < 1f && nowStatus == NowStatus.NONE)
            {  
                // Debug.Log(this.name + "  아무이유없이 멈췄음!,  vehicleStatus : " + thisVehicleAI.vehicleStatus + " , rb.velocity.magnitude : " + rb.velocity.magnitude + " , nowStatus : " + nowStatus);
                
                Vector3 nowPos = vehicle.transform.position;
                
                float vehicleRotationY = vehicle.transform.rotation.eulerAngles.y;

                // 0도
                float bias = 45f;
                float move = 0.01f;
                if((vehicleRotationY >= 0 - bias && vehicleRotationY <= 0 + bias)|| (vehicleRotationY >= 360 - bias && vehicleRotationY <= 360 + bias))
                {
                    vehicle.transform.position = nowPos + new Vector3(0f, 0f, move);
                    Debug.Log(vehicle.name + " Move up side");
                }

                // 180도
                else if((vehicleRotationY >= 180 - bias && vehicleRotationY <= 180 + bias) || (vehicleRotationY >= -180 - bias && vehicleRotationY <= -180 + bias))
                {
                    vehicle.transform.position = nowPos + new Vector3(0f, 0f, -move);
                    Debug.Log(vehicle.name + " Move down side");
                }

                // 90도 _rotationY >= 85 && _rotationY <= 95
                else if((vehicleRotationY >= 90 - bias && vehicleRotationY <= 90 + bias) || (vehicleRotationY >= -270 - bias && vehicleRotationY <= -270 + bias))
                {
                    vehicle.transform.position = nowPos + new Vector3(move, 0f, 0f);
                    Debug.Log(vehicle.name + " Move right side");
                }

                // 270도
                else if((vehicleRotationY >= -90 - bias && vehicleRotationY <= -90 + bias) || (vehicleRotationY >= 270 - bias && vehicleRotationY <= 270 + bias))
                {
                    vehicle.transform.position = nowPos + new Vector3(-move, 0f, 0f);
                    Debug.Log(vehicle.name + " Move left side");
                }

                else
                {
                    Debug.LogError("You have to again check vehicle's rotationY");
                }
               
            }
        }

        void OnTriggerEnter(Collider _other)
        {   
            if(_other.gameObject.tag == "Station")
            {
                // Get Station's Information
                nowStation = _other.gameObject;
                nowStationPos = nowStation.transform.position;
                nowStationInfo = nowStation.GetComponent<StationsInfo>();

                nowStation_FinishedVehicle_toLeft = nowStationInfo.finishedVehicle_toLeft_Count;
                nowStation_FinishedVehicle_toRight = nowStationInfo.finishedVehicle_toRight_Count;

                
                if(vehicle == null)
                {
                    vehicle = this.gameObject;
                    thisVehicleAI = vehicle.GetComponent<VehicleAI>();
                }

                if(truckStatus == 0)
                {
                    startPos = GameObject.Find(truckRouteName).transform.GetChild(0).transform.position;
                    firstStationPos = truckWorkStations[0];
                }
                
                // vehicle이 어느 방향으로 가는지 확인
                // 오른쪽 방향으로 가는 경우
                if(CheckRotation_IsToRight(vehicle))
                {   
                    if(nowStation_FinishedVehicle_toRight > 0)
                    {   
                        Debug.Log(this.name + " has to stop");
                        // 작업이 완료된 트럭이 있다면 도착한 vehicle 감속 및 멈춤
                        StartCoroutine(ReduceSpeed(vehicle, short_slowingTime));
                        thisVehicleAI.vehicleStatus = Status.STOP;
                        nowStatus = NowStatus.WAITING;
                    }

                    else
                    {   
                        Debug.Log(this.name + " can go");
                        // // 작업이 완료된 트럭이 없다면 도착한 vehicle 출발
                        thisVehicleAI.vehicleStatus = Status.GO;
                        nowStatus = NowStatus.NONE;
                    }
                }

                // 왼쪽 방향으로 가는 경우
                else
                {   
                    if(nowStation_FinishedVehicle_toLeft > 0)
                    {
                        Debug.Log(this.name + " has to stop");
  
                        // 작업이 완료된 트럭이 있다면 도착한 vehicle 감속 및 멈춤

                        StartCoroutine(ReduceSpeed(vehicle, short_slowingTime));
                        thisVehicleAI.vehicleStatus = Status.STOP;
                        nowStatus = NowStatus.WAITING;
                    }

                    else
                    {   
                        Debug.Log(this.name + " can go");

                        // // 작업이 완료된 트럭이 없다면 도착한 vehicle 출발
                        thisVehicleAI.vehicleStatus = Status.GO;
                        nowStatus = NowStatus.NONE;
                    }
                }

                // 작업하는 곳인지 확인
                Vector3 toWorkStationPos= truckWorkStations[truckStatus];
                // 트럭이 작업해야하는 곳인 경우
                if(nowStationPos == toWorkStationPos)
                {   
                    nowStatus = NowStatus.PROCESSING;
                    StartCoroutine(WorkingProcess());
                }
            }
        }

        private IEnumerator WorkingProcess()
        {
            Debug.Log(vehicle.name + " nowStatus : " + nowStatus);
            // 시작 위치와 첫번째 작업장이 같은 경우
            if(IsStartPosEqualNowStation(startPos, nowStationPos, truckStatus))
            {
                if(rb == null)
                {
                    rb = vehicle.GetComponent<Rigidbody>();
                }

                rb.velocity = Vector3.zero;
                Debug.Log(vehicle.name + " 's startPos and nowStationPos are same !!!");
            }

            else
            {
                Debug.Log(vehicle.name + " arrive working station !!! ---> station : " + nowStationPos);
                
                // 감속
                thisVehicleAI.vehicleStatus = Status.SLOW_DOWN;
                yield return StartCoroutine(ReduceSpeed(vehicle, long_slowingTime));
            }
            
            thisVehicleAI.vehicleStatus = Status.STOP;

            if(truckTimer == null)
            {
                truckTimer = vehicle.GetComponent<Timer>();
                Debug.Log(this.name + " assign _truckTimer Component !!!");
            }

            if(truckTimer.stationWatch == null)
            {
                truckTimer.stationWatch = vehicle.GetComponent<Timer>().stationWatch;
                Debug.LogError(this.name + " assign  _truckTimer.stationWatch Component !!!");
            }

            else
            {
                Debug.Log(this.name + " already has _truckTimer.stationWatch Component !!!");
            }
        
            if(IsStartPosEqualNowStation(startPos, nowStationPos, truckStatus))
            {
                if(truckTimer.stationWatchList == null)
                {
                    Debug.LogError(this.name + " _truckTimer.stationWatchList is null !!!");
                }

                else
                {
                    truckTimer.stationWatchList.Add(0f);
                    Debug.Log(vehicle.name + " stationWatch Stop !!! ---> now station : " + nowStation.name + " arrival time : " + 0f);
                }
                
            }

            else
            {
                float stationArrivalTime = truckTimer.TimerStop(truckTimer.stationWatch);
                truckTimer.stationWatchList.Add(stationArrivalTime);
                Debug.Log(vehicle.name + " stationWatch Stop !!! ---> now station : " + nowStation.name + " arrival time : " + stationArrivalTime);
            }

            Debug.Log(this.name + " truckTimer.stationWatchList.Count : " + truckTimer.stationWatchList.Count);
            
            originalPos = vehicle.transform.position;

            yield return StartCoroutine(MoveToProcess());
    
            // destination이 아닌 경우
            if(!IsDestination(nowStationPos, truckWorkStations, truckStatus, truckWorkStationsNum))
            {
                Debug.Log(this.name + "--> truckStatus : "+ truckStatus + ", truckWorkStationsNum : "+ truckWorkStationsNum);
                yield return StartCoroutine(Processing());
                yield return StartCoroutine(MoveToOriginalPos());     
                
                if(truckTimer != null)
                {
                    truckTimer.stationWatch.Reset();
                    Debug.Log( vehicle.name + " stationWatch reset");
                    truckTimer.stationWatch.Start();
                }

            }
            
            // destination인 경우
            else if(IsDestination(nowStationPos, truckWorkStations, truckStatus, truckWorkStationsNum))
            {
                Debug.Log(this.name + " arrives destination");
                Debug.Log(this.name + "--> truckStatus : "+ truckStatus + ", truckWorkStationsNum : "+ truckWorkStationsNum);

                yield return StartCoroutine(LastProcessing());
            }
            
            else
            {
                Debug.LogError(this.name + " has to check truckStatus --> truckStatus : "+ truckStatus + ", truckWorkStationsNum : "+ truckWorkStationsNum);
            }
            truckStatus++;
            
            Debug.Log(this.name + " truckStatus : " + truckStatus);
            // Debug.Log(vehicle.name + " updated nowStatus : " + nowStatus);

            nowStatus = NowStatus.NONE;
        }

        private IEnumerator MoveToProcess()
        {
            yield return new WaitForSeconds(moveDelay);

            bc.enabled = false;

            if(turnStations.Count > 0  && nowStationPos == turnStations[nowTurnNum])
            {
                if(CheckRotation_IsToRight(vehicle))
                {   
                    vehicle.transform.rotation = Quaternion.Euler(0, 270f, 0);
                }

                else
                {
                    vehicle.transform.rotation = Quaternion.Euler(0, 90f, 0);
                }
            }
            

            vehicle.transform.position = nowStationPos + new Vector3(0, 0, toStationNum);
        
            nowStationInfo.queueList.Add(vehicle);

        }

        private IEnumerator Processing()
        {   
            // station이 작업 처리 할 수 있는 지 확인
            while(!IsStationAvailable(nowStation))
            {
                yield return new WaitForSeconds(checkDelay);
            }

            // Get Station information
            nowStationInfo.stationStatus += 1;

            nowStationInfo.queueList.Remove(vehicle);

            yield return new WaitForSeconds(processTime);

            nowStationInfo.stationStatus -= 1;

            PlusFinishedVehicle(nowStationInfo, vehicle);
        }

        private IEnumerator MoveToOriginalPos()
        {
            // 작업이 끝나면 주변에 트럭이 있는지 확인
            while(ExistAnyTruck(originalPos, checkRange_1, checkRange_2))
            {   
                // UnityEngine.Debug.Log("there is vehicle near original position");
                yield return new WaitForSeconds(checkDelay);
            }

            // 유턴해야하는 곳인지 확인
            if(turnStations.Count > 0  && nowStationPos == turnStations[nowTurnNum])
            {
                if(CheckRotation_IsToRight(vehicle))
                {   
                    originalPos = nowStationPos + new Vector3(0, 0, -7.5f);
                }

                else
                {
                    originalPos = nowStationPos + new Vector3(0, 0, 7.5f);
                }

                nowTurnNum += 1;
            }

            vehicle.transform.position = originalPos;
        
            MinusFinishedVehicle(nowStationInfo, vehicle);

            bc.enabled = true;
            thisVehicleAI.vehicleStatus = Status.GO;
        }
        
        private IEnumerator LastProcessing()
        {   
            // station이 작업 처리 할 수 있는 지 확인
            while(!IsStationAvailable(nowStation))
            {
                yield return new WaitForSeconds(checkDelay);
            }

            // Get Station information
            nowStationInfo.stationStatus += 1;

            nowStationInfo.queueList.Remove(vehicle);

            yield return new WaitForSeconds(processTime);

            nowStationInfo.stationStatus -= 1;

            if(truckTimer != null)
            {
                float totalTime = truckTimer.TimerStop(truckTimer.totalWatch);
                Debug.Log(vehicle.name + " totalTime is " + totalTime);
                if(exitPlayMode == null)
                {
                    exitPlayMode = GameObject.Find("Roads").GetComponent<ExitPlayMode>();
                }

                exitPlayMode.nowTruckCount += 1;
                truckDestination = nowStationPos;
                truckTimer.SaveToCSV(truckTimer.filePath, vehicle.name, truckRouteName, truckOrigin, truckDestination, totalTime, truckTimer.stationWatchList);
            }

            else
            {
                Debug.LogError("Timer 컴포넌트를 찾을 수 없습니다.");
            }

            vehicle.SetActive(false);
        }

        

        // private IEnumerator LastWorkingProcessing(GameObject _vehicle, VehicleAI _vehicleAI, GameObject _station, StationsInfo _stationInfo, Vector3 _nowStationPos, List<Vector3> _turnStations, float _slowingTime, float _toStationNum, float _moveDelay, float _checkDelay, 
        //                                                 float _checkRange_1, float _checkRange_2, float _processTime, Timer _truckTimer, ExitPlayMode _exitPlayMode, string _routeName, Vector3 _origin, Vector3 _destination)
        // {
        //     // 감속
        //     yield return StartCoroutine(ReduceSpeed(_vehicle, _slowingTime));
        //     _vehicleAI.vehicleStatus = Status.STOP;

        //     if(_truckTimer == null)
        //     {
        //         _truckTimer = _vehicle.GetComponent<Timer>();
        //         Debug.Log(this.name + " assign _truckTimer Component !!!");
        //     }

          
        //     if(_truckTimer.stationWatch != null)
        //     {   
        //         float stationArrivalTime = _truckTimer.TimerStop(_truckTimer.stationWatch);
        //         Debug.Log(_vehicle.name + " stationArrivalTime : " + stationArrivalTime);
        //         _truckTimer.stationWatchList.Add(stationArrivalTime);
        //     }
        //     else
        //     {
        //         Debug.LogError(this.name + " LastWorkingProcess _truckTimer.stationWatch 컴포넌트를 찾을 수 없습니다.");
        //     }

        //     // yield return StartCoroutine(MoveToProcess(_vehicle, _stationInfo, _nowStationPos, _turnStations, _nowTurnNum, _moveDelay, _toStationNum));
        //     yield return StartCoroutine(MoveToProcess(_vehicle, _stationInfo, _nowStationPos, _turnStations, _moveDelay, _toStationNum));


        //     yield return StartCoroutine(LastProcessing(_processTime, _station, _stationInfo, _vehicle, _checkDelay, _truckTimer, _exitPlayMode, _routeName, _origin, _destination));
        // }

        

        private bool CheckRotation_IsToRight(GameObject _vehicle)
        {
            if (_vehicle == null)
            {
                Debug.LogError(this.name + " --> The _vehicle object is null. Make sure it is properly initialized before calling this method.");
                return false;
            }

            float _rotationY = _vehicle.transform.eulerAngles.y;
            // Debug.Log(_vehicle.name + " _rotationY : " + _rotationY);
            if(_rotationY >= 85 && _rotationY <= 95)
            {
                return true;
            }

            else
            {
                return false;
            }
        }


        private bool IsStationAvailable(GameObject _station)
        {
            bool isAvailable = false;
            
            // Get Station's Status
            StationsInfo _stationInfo = _station.GetComponent<StationsInfo>();
            int _stationStatus = _stationInfo.stationStatus;
            int _stationCapacity = _stationInfo.stationCapacity;

            if(_stationStatus <= _stationCapacity)
            {
                isAvailable = true;
            }

            return isAvailable;
        }


        public IEnumerator ReduceSpeed(GameObject _vehicle, float _slowingTime)
        {   
            // UnityEngine.Debug.Log(vehicle.name + " reduces Speed");
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


        private bool ExistAnyTruck(Vector3 _position, float _checkRange_1, float _checkRange_2)
        {   
            // // Perform a raycast to check for vehicles on both sides
            // bool rightSide = CheckRaycast(_position, Vector3.right, _checkRange);
            // bool leftSide = CheckRaycast(_position, Vector3.left, _checkRange);

            // Perform a raycast to check for vehicles on both sides
            bool rightSide = CheckRaycast(_position, new Vector3(1f, 0f, 0f), _checkRange_1);
            bool leftSide = CheckRaycast(_position, new Vector3(-1f, 0f, 0f), _checkRange_1);
            bool forwardSide = CheckRaycast(_position, new Vector3(0f, 0f, 1f), _checkRange_2);
            bool backwardSide = CheckRaycast(_position, new Vector3(0f, 0f, -1f), _checkRange_2);

            return rightSide || leftSide || forwardSide || backwardSide;

            // return rightSide || leftSide;
        }


        private bool CheckRaycast(Vector3 _position, Vector3 _direction, float _range)
        {
            // Perform a raycast in the specified direction
            RaycastHit hit;
            if (Physics.Raycast(_position, _direction, out hit, _range))
            {   
                return true;
            }

            return false;
        }


        private void PlusFinishedVehicle(StationsInfo _stationInfo, GameObject _vehicle)
        {   
            if(CheckRotation_IsToRight(_vehicle))
            {
                _stationInfo.finishedVehicle_toRight_Count += 1;
            }

            else
            {
                _stationInfo.finishedVehicle_toLeft_Count += 1;
            }
        }


        private void MinusFinishedVehicle(StationsInfo _stationInfo, GameObject _vehicle)
        {   
            if(CheckRotation_IsToRight(_vehicle))
            {
                _stationInfo.finishedVehicle_toRight_Count -= 1;
            }

            else
            {
                _stationInfo.finishedVehicle_toLeft_Count -= 1;
            }
        }


        private bool IsDestination(Vector3 _nowStationPos, List<Vector3> _truckWorkStations, int _truckStatus, int _truckWorkStationsNum)
        {
            return _nowStationPos == _truckWorkStations[_truckWorkStationsNum-1] && _truckStatus == _truckWorkStationsNum - 1;
        }

        private bool IsStartPosEqualNowStation(Vector3 _startPos, Vector3 _nowStaitonPos, int _truckStatus)
        {
            return _startPos == _nowStaitonPos && _truckStatus == 0;
        }

    }
}