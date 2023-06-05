using System.Collections;
using System.Collections.Generic;
// using System.Diagnostics;
using UnityEngine;

namespace TrafficSimulation{
    public class TruckInfo : MonoBehaviour
    {

        public List<Vector3> truckWorkStations;
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
        // private int nowTurnNum;
        public int turnNum;


        private GameObject nowStation;
        private StationsInfo nowStationInfo;
        private int nowStation_FinishedVehicle_toLeft;
        private int nowStation_FinishedVehicle_toRight;

        // To check Station's Status
        private float checkDelay = 0.1f; 

        private Timer truckTimer;

        private ExitPlayMode exitPlayMode;
        private Vector3 startPos;

        // Start is called before the first frame update
        void Start()
        {
            truckStatus = 0;
            // nowTurnNum = 0;
            vehicle = this.gameObject;
            thisVehicleAI = vehicle.GetComponent<VehicleAI>();
            truckTimer = vehicle.GetComponent<Timer>();
            exitPlayMode = GameObject.Find("Roads").GetComponent<ExitPlayMode>();
        }

        void OnTriggerEnter(Collider _other)
        {   
            if(_other.gameObject.tag == "Station")
            {
                // Get Station's Information
                nowStation = _other.gameObject;
                nowStationInfo = nowStation.GetComponent<StationsInfo>();

                nowStation_FinishedVehicle_toLeft = nowStationInfo.finishedVehicle_toLeft_Count;
                nowStation_FinishedVehicle_toRight = nowStationInfo.finishedVehicle_toRight_Count;

                
                if(vehicle == null)
                {
                    vehicle = this.gameObject;
                    thisVehicleAI = vehicle.GetComponent<VehicleAI>();
                }

                // vehicle이 어느 방향으로 가는지 확인
                // 오른쪽 방향으로 가는 경우
                if(CheckRotation_IsToRight(vehicle))
                {   
                    startPos = vehicle.transform.position + new Vector3(0, 0, 7.5f);

                    // Debug.Log(this.name + " is going to right");
                    // 작업이 완료된 트럭이 있는지 확인
                    if(nowStation_FinishedVehicle_toRight > 0)
                    {   
                        // UnityEngine.Debug.Log(vehicle.name +" have to wait, there is finished vehicle to right");
                        // 작업이 완료된 트럭이 있다면 도착한 vehicle 감속 및 멈춤
                        StartCoroutine(ReduceSpeed(vehicle, short_slowingTime));
                        thisVehicleAI.vehicleStatus = Status.STOP;
                    }

                    // else if(nowStation_FinishedVehicle_toRight == 0 && thisVehicleAI.vehicleStatus == Status.STOP)
                    else
                    {
                        // // 작업이 완료된 트럭이 없다면 도착한 vehicle 출발
                        thisVehicleAI.vehicleStatus = Status.GO;
                    }
                }

                // 왼쪽 방향으로 가는 경우
                else
                {   
                    startPos = vehicle.transform.position + new Vector3(0, 0, -7.5f);

                    // Debug.Log(this.name + " is going to left");
                    if(nowStation_FinishedVehicle_toLeft > 0)
                    {
                        // UnityEngine.Debug.Log(vehicle.name + " have to wait, there is finished vehicle to left");
                        // 작업이 완료된 트럭이 있다면 도착한 vehicle 감속 및 멈춤
                        StartCoroutine(ReduceSpeed(vehicle, short_slowingTime));
                        thisVehicleAI.vehicleStatus = Status.STOP;
                    }

                    // else if(nowStation_FinishedVehicle_toLeft == 0 && thisVehicleAI.vehicleStatus == Status.STOP)
                    else
                    {
                        // // 작업이 완료된 트럭이 없다면 도착한 vehicle 출발
                        thisVehicleAI.vehicleStatus = Status.GO;
                    }
                }

                // 작업하는 곳인지 확인
                string toWorkStation_Name = truckWorkStations[truckStatus].ToString();

                if(startPos.ToString() == toWorkStation_Name)
                {
                    // 바로 작업 시작
                }

                // 트럭이 작업해야하는 곳인 경우
                else if(toWorkStation_Name == nowStation.name)
                {
                    truckStatus += 1;
                    // 도착지인 경우
                    if(IsDestination(toWorkStation_Name, truckDestination))
                    {
                        Debug.Log(vehicle.name +" arrives destination!!!");
                        StartCoroutine(LastWorkingProcess(vehicle, thisVehicleAI, nowStation, nowStationInfo, long_slowingTime, toStationNum, turnNum,
                                                            moveDelay, checkDelay, checkRange_1, checkRange_2, processTime, truckTimer, exitPlayMode, truckRouteName, truckOrigin, truckDestination));
                    }
                    
                    // 도착지가 아닌 경우
                    else
                    {  
                        StartCoroutine(WorkingProcess(vehicle, thisVehicleAI, nowStation, nowStationInfo, long_slowingTime, toStationNum, turnNum,
                                                                moveDelay, checkDelay, checkRange_1, checkRange_2, processTime, truckTimer));
                    }
                }
            }
        }

        
        private IEnumerator WorkingProcess(GameObject _vehicle, VehicleAI _vehicleAI, GameObject _station, StationsInfo _stationInfo, float _slowingTime, 
                                            float _toStationNum, int _turnNum, float _moveDelay, float _checkDelay, float _checkRange_1, float _checkRange_2, float _processTime, Timer _truckTimer)
        {
            // 감속
            // Debug.Log(_vehicle.name + " SLOW_DOWN");
            _vehicleAI.vehicleStatus = Status.SLOW_DOWN;
            yield return StartCoroutine(ReduceSpeed(_vehicle, _slowingTime));

            // yield return new WaitForSeconds(3f);
            // Debug.Log(_vehicle.name + " STOP");
            _vehicleAI.vehicleStatus = Status.STOP;

            if(_truckTimer == null)
            {
                _truckTimer = _vehicle.GetComponent<Timer>();
                Debug.Log(this.name + " assign _truckTimer Component !!!");
            }
            
            if(_truckTimer.stationWatch != null)
            {   
                float stationArrivalTime = _truckTimer.TimerStop(_truckTimer.stationWatch);
                _truckTimer.stationWatchList.Add(stationArrivalTime);
                Debug.Log(this.name + " stationWatch Stop !!!");
            }

            else
            {
                Debug.LogError(this.name + " _truckTimer.stationWatch 컴포넌트를 찾을 수 없습니다.");
            }

            originalPos = _vehicle.transform.position;

            yield return StartCoroutine(MoveToProcess(_vehicle, _station, _stationInfo, _moveDelay, _toStationNum, _turnNum));

            yield return StartCoroutine(Processing(_processTime, _station, _stationInfo, _vehicle, _checkDelay));

            yield return StartCoroutine(MoveToOriginalPos(_vehicle, _vehicleAI, _station, _stationInfo, originalPos, _turnNum, _checkRange_1, _checkRange_2, _checkDelay));

            if(_truckTimer != null)
            {
                _truckTimer.stationWatch.Reset();
                Debug.Log( vehicle.name + " stationWatch reset");
                _truckTimer.stationWatch.Start();
            }
        }

        private IEnumerator LastWorkingProcess(GameObject _vehicle, VehicleAI _vehicleAI, GameObject _station, StationsInfo _stationInfo, float _slowingTime, float _toStationNum, int _turnNum, float _moveDelay, float _checkDelay, 
                                                        float _checkRange_1, float _checkRange_2, float _processTime, Timer _truckTimer, ExitPlayMode _exitPlayMode, string _routeName, Vector3 _origin, Vector3 _destination)
        {
            // 감속
            yield return StartCoroutine(ReduceSpeed(_vehicle, _slowingTime));
            _vehicleAI.vehicleStatus = Status.STOP;

            if(_truckTimer == null)
            {
                _truckTimer = _vehicle.GetComponent<Timer>();
                Debug.Log(this.name + " assign _truckTimer Component !!!");
            }

          
            if(_truckTimer.stationWatch != null)
            {   
                float stationArrivalTime = _truckTimer.TimerStop(_truckTimer.stationWatch);
                Debug.Log(_vehicle.name + " stationArrivalTime : " + stationArrivalTime);
                _truckTimer.stationWatchList.Add(stationArrivalTime);
            }
            else
            {
                Debug.LogError(this.name + " LastWorkingProcess _truckTimer.stationWatch 컴포넌트를 찾을 수 없습니다.");
            }

            yield return StartCoroutine(MoveToProcess(_vehicle, _station, _stationInfo, _moveDelay, _toStationNum, _turnNum));

            yield return StartCoroutine(LastProcessing(_processTime, _station, _stationInfo, _vehicle, _checkDelay, _truckTimer, _exitPlayMode, _routeName, _origin, _destination));
        }


        private IEnumerator MoveToProcess(GameObject _vehicle, GameObject _station, StationsInfo _stationInfo, float _delay, float _toStationNum, int _turnNum)
        {
            yield return new WaitForSeconds(_delay);

            // UnityEngine.Debug.Log(_vehicle.name + "Move to Process");
            // After _delay seconds, move to process

            _vehicle.GetComponent<Collider>().enabled = false;

            if(_turnNum > 0)
            {
                if(CheckRotation_IsToRight(_vehicle))
                {
                    _vehicle.transform.rotation = Quaternion.Euler(0, 270f, 0);
                }

                else
                {
                    _vehicle.transform.rotation = Quaternion.Euler(0, 90f, 0);
                }
            }

            _vehicle.transform.position = _station.transform.position + new Vector3(0,0,_toStationNum);
        
            _stationInfo.queueList.Add(_vehicle);

        }
        

        private IEnumerator Processing(float _processTime, GameObject _station, StationsInfo _stationInfo, GameObject _vehicle, float _checkDelay)
        {   
            // station이 작업 처리 할 수 있는 지 확인
            while(!IsStationAvailable(_station))
            {
                yield return new WaitForSeconds(_checkDelay);
            }

            // Get Station information
            _stationInfo.stationStatus += 1;

            _stationInfo.queueList.Remove(_vehicle);

            yield return new WaitForSeconds(_processTime);

            PlusFinishedVehicle(_stationInfo, _vehicle);
        }


        private IEnumerator LastProcessing(float _processTime, GameObject _station, StationsInfo _stationInfo, GameObject _vehicle, float _checkDelay, 
                                                        Timer _truckTimer, ExitPlayMode _exitPlayMode, string _routeName, Vector3 _origin, Vector3 _destination)
        {   
            // station이 작업 처리 할 수 있는 지 확인
            while(!IsStationAvailable(_station))
            {
                yield return new WaitForSeconds(_checkDelay);
            }

            // Get Station information
            _stationInfo.stationStatus += 1;

            _stationInfo.queueList.Remove(_vehicle);

            yield return new WaitForSeconds(_processTime);

            _stationInfo.stationStatus -= 1;

            if(_truckTimer != null)
            {
                float totalTime = _truckTimer.TimerStop(_truckTimer.totalWatch);
                Debug.Log( _vehicle.name + " totalTime is " + totalTime);
                _exitPlayMode.nowTruckCount += 1;
                _truckTimer.SaveToCSV(_truckTimer.filePath, _vehicle.name, _routeName, _origin, _destination, totalTime, _truckTimer.stationWatchList);
            }

            else
            {
                Debug.LogError("Timer 컴포넌트를 찾을 수 없습니다.");
            }

            _vehicle.SetActive(false);
        }


        private IEnumerator MoveToOriginalPos(GameObject _vehicle, VehicleAI _vehicleAI, GameObject _station, StationsInfo _stationInfo, Vector3 _originalPos, int _turnNum, float _checkRange_1, float _checkRange_2, float _checkDelay)
        {
            // 작업이 끝나면 주변에 트럭이 있는지 확인
            while(ExistAnyTruck(_originalPos, _checkRange_1, _checkRange_2))
            {   
                // UnityEngine.Debug.Log("there is vehicle near original position");
                yield return new WaitForSeconds(_checkDelay);
            }

            if(_turnNum > 0)
            {
                if(CheckRotation_IsToRight(_vehicle))
                {
                    _originalPos = _station.transform.position + new Vector3(0, 0, -7.5f);
                }

                else
                {
                    _originalPos = _station.transform.position + new Vector3(0, 0, 7.5f);
                }
            }

            _vehicle.transform.position = _originalPos;

            _stationInfo.stationStatus -= 1;
        
            MinusFinishedVehicle(_stationInfo, _vehicle);


            _vehicle.GetComponent<Collider>().enabled = true;
            _vehicleAI.vehicleStatus = Status.GO;

        }


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
            // UnityEngine.Debug.Log(vehicle.name + " rb.velocity is 0!!");
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
            // Debug.Log(_vehicle.name + " PlusFinishedVehicle");
            
            if(CheckRotation_IsToRight(_vehicle))
            {
                _stationInfo.finishedVehicle_toRight_Count += 1;
                // _station.GetComponent<StationsInfo>().finishedVehicle_toRight_Count += 1;
                // return _stationInfo.finishedVehicle_toRight_Count;
            }

            else
            {
                _stationInfo.finishedVehicle_toLeft_Count += 1;
                // _station.GetComponent<StationsInfo>().finishedVehicle_toLeft_Count += 1;
                // return _stationInfo.finishedVehicle_toLeft_Count;
            }
        }


        private void MinusFinishedVehicle(StationsInfo _stationInfo, GameObject _vehicle)
        {   
            // Debug.Log(_vehicle.name + " MinusFinishedVehicle");
            if(CheckRotation_IsToRight(_vehicle))
            {
                // _station.GetComponent<StationsInfo>().finishedVehicle_toRight_Count -= 1;
                _stationInfo.finishedVehicle_toRight_Count -= 1;
                // return _stationInfo.finishedVehicle_toRight_Count;
            }

            else
            {
                // _station.GetComponent<StationsInfo>().finishedVehicle_toLeft_Count -= 1;
                _stationInfo.finishedVehicle_toLeft_Count -= 1;
                // return _stationInfo.finishedVehicle_toLeft_Count;
            }
        }


        private bool IsDestination(string _stationName, Vector3 _destination)
        {
            return _stationName == _destination.ToString();
        }
 
    }
}