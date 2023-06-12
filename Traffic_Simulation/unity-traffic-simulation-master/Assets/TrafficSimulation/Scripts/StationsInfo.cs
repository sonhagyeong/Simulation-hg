using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationsInfo : MonoBehaviour
{

    // 현재 작업 중인 트럭 개수
    public int stationStatus;
    // 작업 가능한 Max 트럭 개수
    public int stationCapacity;

    // 작업 대기 중인 트럭 리스트
    public List<GameObject> processQueueList;

    // 작업 끝난 트럭 리스트
    public List<GameObject> finishedQueueList_toLeft;
    public List<GameObject> finishedQueueList_toRight;

    void Awake()
    {
        stationStatus = 0;
        stationCapacity = 2;
    
        processQueueList = new List<GameObject>();
        finishedQueueList_toLeft = new List<GameObject>();
        finishedQueueList_toRight = new List<GameObject>();
    }

    
}