using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;

namespace TrafficSimulation{
// 이 스크립는 Truck한테 있어야함
public class PopVehicle : MonoBehaviour
{
    Stopwatch stopwatch = new Stopwatch();
    
    int initialNumberOfTruck;
    int hitCount = 0;
    
    void Start()
    {
        initialNumberOfTruck = GameObject.FindGameObjectsWithTag("AutonomousVehicle").Length;
    }

    
    private void OnCollisionEnter(Collision collision)
    {   
        List<GameObject> collisionTruck = new List<GameObject>();

        if(collision.gameObject.tag == gameObject.tag)
        {   
            hitCount = hitCount+1;
            UnityEngine.Debug.Log("hitCount : " + hitCount);
            int vehiclesQueueCount = GameObject.Find("Intersection-0").GetComponent<Intersection>().vehiclesQueue.Count;
            UnityEngine.Debug.Log("intersection.vehiclesQueue " + vehiclesQueueCount);
            // for(int i =0; i < vehiclesQueueCount; i++)
            // {
            //     UnityEngine.Debug.Log("intersection.vehiclesQueue[i] " + GameObject.Find("Intersection-0").GetComponent<Intersection>().vehiclesQueue[i]);
            // }
            
            GameObject.Find("Intersection-0").GetComponent<Intersection>().vehiclesQueue.Remove(gameObject);
            GameObject.Find("Intersection-0").GetComponent<Intersection>().vehiclesInIntersection.Remove(gameObject);

            GameObject.Find("Intersection-1").GetComponent<Intersection>().vehiclesQueue.Remove(gameObject);
            GameObject.Find("Intersection-1").GetComponent<Intersection>().vehiclesInIntersection.Remove(gameObject);

            GameObject.Find("Intersection-4").GetComponent<Intersection>().vehiclesQueue.Remove(gameObject);
            GameObject.Find("Intersection-4").GetComponent<Intersection>().vehiclesInIntersection.Remove(gameObject);
            
            GameObject.Find("Intersection-2").GetComponent<Intersection>().vehiclesQueue.Remove(gameObject);
            GameObject.Find("Intersection-2").GetComponent<Intersection>().vehiclesInIntersection.Remove(gameObject);
            
            GameObject.Find("Intersection-3").GetComponent<Intersection>().vehiclesQueue.Remove(gameObject);
            GameObject.Find("Intersection-3").GetComponent<Intersection>().vehiclesInIntersection.Remove(gameObject);

            GameObject.Find("Intersection-5").GetComponent<Intersection>().vehiclesQueue.Remove(gameObject);
            GameObject.Find("Intersection-5").GetComponent<Intersection>().vehiclesInIntersection.Remove(gameObject);

            GameObject.Find("Intersection-6").GetComponent<Intersection>().vehiclesQueue.Remove(gameObject);
            GameObject.Find("Intersection-6").GetComponent<Intersection>().vehiclesInIntersection.Remove(gameObject);


            Destroy(gameObject);
        }

    }

    public void OnTriggerEnter(Collider collider)
    {   
        //트럭의 trucknametag와 부딪힌 오브젝트의 태그와 동일한 경우 
        if (collider.gameObject.tag == GetComponent<SetNameTag>().truckNameTag)
        {   
            stopwatch.Stop();
            // gameObject.SetActive(false);
            Destroy(gameObject);

            int endNumberOfTruck = GameObject.FindGameObjectsWithTag("AutonomousVehicle").Length;
            UnityEngine.Debug.Log("endNumberOfTruck : " + endNumberOfTruck);

            float time = stopwatch.ElapsedMilliseconds ;
            UnityEngine.Debug.Log(time*0.001);

            FileStream fs = new FileStream("Assets/result.csv", FileMode.Append, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fs, System.Text.Encoding.Unicode);

            var list = new List<string>();
            string startSegment = GetComponent<SetNameTag>().segmentNameTag;
            string finishPlace = GetComponent<SetNameTag>().truckNameTag;

            // var trucksChildren = GameObject.Find("Trucks").GetComponentsInChildren<Transform>();
            // int endNumberOfTruck = trucksChildren.Length;

            // string name = this.gameObject.ToString();

            

            // UnityEngine.Debug.Log("startSegment : " + startSegment);
            // UnityEngine.Debug.Log("finishPlace : " + finishPlace);
            

            list.Add((time*0.001).ToString());
            
            for (int i = 0; i < list.Count; i++)
            {
                var tmp = list[i];

                //Debug.Log(string.Format("{0},{1}", tmp.nIndex, tmp.sName));
                // streamWriter.WriteLine(string.Format("{0},{1:F4}", name.Substring(0, name.Length - 24), tmp));
                streamWriter.WriteLine(string.Format("{0},{1},{2:F4},{3},{4}", startSegment, finishPlace, tmp, initialNumberOfTruck, endNumberOfTruck, hitCount));
            }

            streamWriter.Close();
        }

        else 
        {
            stopwatch.Start();
            // UnityEngine.Debug.Log("No crash");
        }
    }

    // public void GetValue()
    // {
    //     var trucksChildren = GameObject.Find("Trucks").GetComponentsInChildren<Transform>();
    //     endNumberOfTruck = trucksChildren.Length;
    // } 
    
}
}