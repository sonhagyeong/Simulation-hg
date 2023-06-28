using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        public int truckWorkStationsNum;
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
        private float checkRange_1 = 15f;
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
        private Stopwatch truckTotalWatch;
        private Stopwatch truckStationWatch;
        private Stopwatch noReasonStopWatch;
        [SerializeField]private List<float> truckStationWatchList = new List<float>();

        private ExitPlayMode exitPlayMode;
        private Vector3 startPos;
        private Vector3 firstStationPos;

        private Rigidbody rb;
        private BoxCollider bc;
        public NowStatus nowStatus;

        // Start is called before the first frame update
        void Awake()
        {   
            vehicle = this.gameObject;
            thisVehicleAI = vehicle.GetComponent<VehicleAI>();

            truckTimer = vehicle.GetComponent<Timer>();

            truckTotalWatch = new Stopwatch();
            truckStationWatch = new Stopwatch();
            noReasonStopWatch = new Stopwatch();
            
        }

        void Start()
        {
            truckStatus = 0;
            nowTurnNum = 0;

            truckTotalWatch.Start();
            truckStationWatch.Start();
  

            exitPlayMode = GameObject.Find("Roads").GetComponent<ExitPlayMode>();
            // truckWorkStationsNum = truckWorkStations.Count;
            rb = vehicle.GetComponent<Rigidbody>();
            bc = vehicle.GetComponent<BoxCollider>();
            nowStatus = NowStatus.NONE;
        }
        
        
        void Update()
        {
            if((thisVehicleAI.vehicleStatus == Status.GO || thisVehicleAI.vehicleStatus == Status.SLOW_DOWN) && rb.velocity.magnitude < 0.05f && nowStatus == NowStatus.NONE)
            {  
                // UnityEngine.Debug.Log(this.name + "  아무이유없이 멈췄음!");


                Vector3 nowPos = vehicle.transform.position;

                
        
                float vehicleRotationY = vehicle.transform.rotation.eulerAngles.y;

                // 0도
                float bias = 45f;
                float move = 0.05f;
                if((vehicleRotationY >= 0 - bias && vehicleRotationY <= 0 + bias)|| (vehicleRotationY >= 360 - bias && vehicleRotationY <= 360 + bias))
                {
                    vehicle.transform.position = nowPos + new Vector3(0f, 0f, move);
                    UnityEngine.Debug.Log(vehicle.name + " Move up side");
                    if(thisVehicleAI.vehicleStatus == Status.GO)
                    {
                        vehicle.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    }
                }

                // 180도
                else if((vehicleRotationY >= 180 - bias && vehicleRotationY <= 180 + bias) || (vehicleRotationY >= -180 - bias && vehicleRotationY <= -180 + bias))
                {
                    vehicle.transform.position = nowPos + new Vector3(0f, 0f, -move);
                    UnityEngine.Debug.Log(vehicle.name + " Move down side");
                    if(thisVehicleAI.vehicleStatus == Status.GO)
                    {
                        vehicle.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                    }
                }

                // 90도 _rotationY >= 85 && _rotationY <= 95
                else if((vehicleRotationY >= 90 - bias && vehicleRotationY <= 90 + bias) || (vehicleRotationY >= -270 - bias && vehicleRotationY <= -270 + bias))
                {
                    vehicle.transform.position = nowPos + new Vector3(move, 0f, 0f);
                    UnityEngine.Debug.Log(vehicle.name + " Move right side");
                    if(thisVehicleAI.vehicleStatus == Status.GO)
                    {
                        vehicle.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                    }
                }

                // 270도
                else if((vehicleRotationY >= -90 - bias && vehicleRotationY <= -90 + bias) || (vehicleRotationY >= 270 - bias && vehicleRotationY <= 270 + bias))
                {
                    vehicle.transform.position = nowPos + new Vector3(-move, 0f, 0f);
                    UnityEngine.Debug.Log(vehicle.name + " Move left side");
                    if(thisVehicleAI.vehicleStatus == Status.GO)
                    {
                        vehicle.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
                    }
                }

                else
                {
                    UnityEngine.Debug.LogError("You have to again check vehicle's rotationY");
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

                nowStation_FinishedVehicle_toLeft = nowStationInfo.finishedQueueList_toLeft != null ? nowStationInfo.finishedQueueList_toLeft.Count : 0;
                nowStation_FinishedVehicle_toRight = nowStationInfo.finishedQueueList_toRight != null ? nowStationInfo.finishedQueueList_toRight.Count : 0;

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

                if(CheckRotation_IsToRight(vehicle))
                {   
                    if(nowStation_FinishedVehicle_toRight > 0)
                    {   
                        UnityEngine.Debug.Log(this.name + " has to stop");
                        // 작업이 완료된 트럭이 있다면 도착한 vehicle 감속 및 멈춤
                        StartCoroutine(ReduceSpeed(vehicle, short_slowingTime));
                        thisVehicleAI.vehicleStatus = Status.STOP;
                        nowStatus = NowStatus.WAITING;
                    }

                    else
                    {   
                        UnityEngine.Debug.Log(this.name + " can go");
                        // // 작업이 완료된 트럭이 없다면 도착한 vehicle 출발
                        thisVehicleAI.vehicleStatus = Status.GO;
                    }
                }

                // 왼쪽 방향으로 가는 경우
                else
                {   
                    if(nowStation_FinishedVehicle_toLeft > 0)
                    {
                        UnityEngine.Debug.Log(this.name + " has to stop");
  
                        // 작업이 완료된 트럭이 있다면 도착한 vehicle 감속 및 멈춤

                        StartCoroutine(ReduceSpeed(vehicle, short_slowingTime));
                        thisVehicleAI.vehicleStatus = Status.STOP;
                        nowStatus = NowStatus.WAITING;
                    }

                    else
                    {   
                        UnityEngine.Debug.Log(this.name + " can go");

                        // // 작업이 완료된 트럭이 없다면 도착한 vehicle 출발
                        thisVehicleAI.vehicleStatus = Status.GO;
                    }
                }

                if(truckStatus < truckWorkStations.Count)
                {
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

            else if(_other.gameObject.tag == "AutonomousVehicle")
            {
                UnityEngine.Debug.LogError(this.name + " is crashed with " + _other.gameObject.name);
            }

        }

        private IEnumerator WorkingProcess()
        {   
            nowStatus = NowStatus.PROCESSING;
            // 시작 위치와 첫번째 작업장이 같은 경우 or 이전의 작업장과 현재 작업장이 같은 경우 
            if(IsStartPosEqualNowStation(startPos, nowStationPos, truckStatus) || truckStatus > 0 && truckWorkStations[truckStatus -1] == truckWorkStations[truckStatus])
            {
                if(rb == null)
                {
                    rb = vehicle.GetComponent<Rigidbody>();
                }

                rb.velocity = Vector3.zero;
                UnityEngine.Debug.Log(vehicle.name + " 's startPos and nowStationPos are same !!! ---> station : " + nowStationPos);
            }

            else
            {
                // 감속
                thisVehicleAI.vehicleStatus = Status.SLOW_DOWN;
                yield return StartCoroutine(ReduceSpeed(vehicle, long_slowingTime));   
            }

            
            thisVehicleAI.vehicleStatus = Status.STOP;

            if(truckStationWatch == null)
            {
                UnityEngine.Debug.LogError(this.name + "  stationWatch Component is null !!!");
            }
            
            if(truckStationWatchList == null)
            {
                UnityEngine.Debug.LogError(this.name + " truck Station Watch List is null !!!");
            }

            
            float stationArrivalTime = truckStationWatch.ElapsedMilliseconds / 1000f;

            truckStationWatchList.Add(stationArrivalTime);
            UnityEngine.Debug.Log(vehicle.name + " stationWatch Stop !!! ---> now station : " + nowStation.name + " arrival time : " + stationArrivalTime);
            
            originalPos = vehicle.transform.position;

            truckStationWatch.Stop();
            // yield return StartCoroutine(MoveToProcess());
            MoveToProcess();
            truckStationWatch.Reset();
            UnityEngine.Debug.Log( vehicle.name + " stationWatch reset");
            
            // destination이 아닌 경우
            UnityEngine.Debug.Log(vehicle.name + " nowStationPos : " + nowStationPos + ", truckStatus : " + truckStatus + ", truckWorkStations[truckStatus] : " + truckWorkStations[truckStatus]);
       
            if(!IsDestination(nowStationPos, truckWorkStations, truckStatus, truckWorkStationsNum))
            {
                UnityEngine.Debug.Log(this.name + " doesn't arrives destination");
                yield return StartCoroutine(Processing());
                yield return StartCoroutine(MoveToOriginalPos());     
            }
            
            // destination인 경우
            else
            {
                UnityEngine.Debug.Log(this.name + " arrives destination");
                yield return StartCoroutine(LastProcessing());
            }
            
            
            UnityEngine.Debug.Log(vehicle.name + " nowStatus : " + nowStatus);
            
            truckStationWatch.Start();
            UnityEngine.Debug.Log(this.name + " updated truckStatus : " + truckStatus);
            UnityEngine.Debug.Log(this.name + "truckTotalWatch.ElapsedMilliseconds / 1000f : " + truckTotalWatch.ElapsedMilliseconds / 1000f);

            nowStatus = NowStatus.NONE;
        }

        private void MoveToProcess()
        {
            nowStatus = NowStatus.PROCESSING;

            if(bc == null)
            {
                bc = vehicle.GetComponent<BoxCollider>();
            }

            bc.enabled = false;
            
            UnityEngine.Debug.Log(this.name + " turnStations.Count : " + turnStations.Count + " nowTurnNum : " + nowTurnNum);

            // 유턴하는 곳인지 확인, 출발지에서 유턴하는 경우는 없음.
            if(turnStations.Count > 0 && truckStationWatchList.Count > 1 && nowTurnNum < turnStations.Count && nowStationPos == turnStations[nowTurnNum])
            {   
                if(CheckRotation_IsToRight(vehicle))
                {   
                    UnityEngine.Debug.Log(vehicle.name + " 's rotation changes to right !!!");
                    vehicle.transform.rotation = Quaternion.Euler(0, 270f, 0);
                }

                else
                {
                    UnityEngine.Debug.Log(vehicle.name + " 's rotation changes to left !!!");
                    vehicle.transform.rotation = Quaternion.Euler(0, 90f, 0);
                }           
            }   

            vehicle.transform.position = nowStationPos + new Vector3(0, 0, toStationNum);
            nowStationInfo.processQueueList.Add(vehicle);

        }

        private IEnumerator Processing()
        {   
            UnityEngine.Debug.Log(this.name + " processing ---> station : " + nowStationPos);

            // station이 작업 처리 할 수 있는 지 확인
            while(!IsStationAvailable(nowStation))
            {
                yield return new WaitForSeconds(checkDelay);
            }

            // Get Station information
            nowStationInfo.stationStatus += 1;

            nowStationInfo.processQueueList.Remove(vehicle);

            yield return new WaitForSeconds(processTime);

            nowStationInfo.stationStatus -= 1;

            PlusFinishedVehicle(nowStationInfo, vehicle);

            truckStatus+= 1;
        }

        private IEnumerator MoveToOriginalPos()
        {
        
            // 시작 위치와 첫번째 작업장이 같지 않은 경우
            if(!IsStartPosEqualNowStation(startPos, nowStationPos, truckStatus) || !(truckStatus > 0 && truckWorkStations[truckStatus -1] == truckWorkStations[truckStatus]))
            {
                // 유턴했는지 확인
                if(turnStations.Count > 0 && truckStationWatchList.Count > 1 && nowTurnNum < turnStations.Count && nowStationPos == turnStations[nowTurnNum])
                {
                    UnityEngine.Debug.Log(this.name + " u-turn station !!! ---> station : " + nowStationPos);
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
            }
            

            // 작업이 끝나면 주변에 트럭이 있는지 확인
            while(ExistAnyTruck(originalPos, checkRange_1, checkRange_2))
            {   
                yield return new WaitForSeconds(checkDelay);
            }

            UnityEngine.Debug.Log(this.name + " can go to next station ! ");
            bc.enabled = true;

            vehicle.transform.position = originalPos;
        
            MinusFinishedVehicle(nowStationInfo, vehicle);

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

            nowStationInfo.processQueueList.Remove(vehicle);

            yield return new WaitForSeconds(processTime);

            nowStationInfo.stationStatus -= 1;

            if(truckTimer != null)
            {
                if(truckTotalWatch == null)
                {
                    UnityEngine.Debug.LogError(this.name + " _truckTimer.totalWatch is null !!!");
                }
                truckTotalWatch.Stop();
                float truckTotalTime = truckTotalWatch.ElapsedMilliseconds / 1000f;

                // float totalTime = truckTimer.TimerStop(truckTimer.totalWatch);
                UnityEngine.Debug.Log(vehicle.name + " totalTime is " + truckTotalTime);
                if(exitPlayMode == null)
                {
                    exitPlayMode = GameObject.Find("Roads").GetComponent<ExitPlayMode>();
                }

                exitPlayMode.nowTruckCount += 1;
                truckDestination = nowStationPos;
                // truckTimer.SaveToCSV(truckTimer.filePath, vehicle.name, truckRouteName, truckOrigin, truckDestination, totalTime, truckTimer.stationWatchList);
                truckTimer.SaveToCSV(truckTimer.filePath, vehicle.name, truckRouteName, truckOrigin, truckDestination, truckTotalTime, truckStationWatchList);

            }

            else
            {
                UnityEngine.Debug.LogError("Timer 컴포넌트를 찾을 수 없습니다.");
            }

            vehicle.SetActive(false);
        }
        

        private bool CheckRotation_IsToRight(GameObject _vehicle)
        {
            if (_vehicle == null)
            {
                UnityEngine.Debug.LogError(this.name + " --> The _vehicle object is null. Make sure it is properly initialized before calling this method.");
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
            if(CheckRotation_IsToRight(vehicle))
            {
                _stationInfo.finishedQueueList_toRight.Add(_vehicle);
            }

            else
            {
                _stationInfo.finishedQueueList_toLeft.Add(_vehicle);
            }
        }


        private void MinusFinishedVehicle(StationsInfo _stationInfo, GameObject _vehicle)
        {   
            if(CheckRotation_IsToRight(_vehicle))
            {
                _stationInfo.finishedQueueList_toRight.Remove(_vehicle);
            }

            else
            {
                _stationInfo.finishedQueueList_toLeft.Remove(_vehicle);
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