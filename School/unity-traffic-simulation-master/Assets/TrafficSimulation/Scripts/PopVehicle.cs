using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;

// 이 스크립는 Truck한테 있어야함
public class PopVehicle : MonoBehaviour
{
    int endNumberOfTruck;

    Stopwatch stopwatch = new Stopwatch();

    public void OnTriggerEnter(Collider collider)
    {   
        int initialNumberOfTruck = GameObject.Find("Trucks").transform.childCount;

        //트럭의 trucknametag와 부딪힌 오브젝트의 태그와 동일한 경우 
        if (collider.gameObject.tag == GetComponent<SetNameTag>().truckNameTag)
        {
            gameObject.SetActive(false);
            // Destroy(gameObject);
            stopwatch.Stop();
            float time = stopwatch.ElapsedMilliseconds ;
            UnityEngine.Debug.Log(time*0.001);

            FileStream fs = new FileStream("Assets/result.csv", FileMode.Append, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fs, System.Text.Encoding.Unicode);

            var list = new List<string>();
            string startSegment = GetComponent<SetNameTag>().segmentNameTag;
            string finishPlace = GetComponent<SetNameTag>().truckNameTag;
            // string name = this.gameObject.ToString();

            Invoke("GetValue", 1);

            // UnityEngine.Debug.Log("startSegment : " + startSegment);
            // UnityEngine.Debug.Log("finishPlace : " + finishPlace);
            // UnityEngine.Debug.Log("endNumberOfTruck : " + endNumberOfTruck);

            list.Add((time*0.001).ToString());
            
            for (int i = 0; i < list.Count; i++)
            {
                var tmp = list[i];

                //Debug.Log(string.Format("{0},{1}", tmp.nIndex, tmp.sName));
                // streamWriter.WriteLine(string.Format("{0},{1:F4}", name.Substring(0, name.Length - 24), tmp));
                streamWriter.WriteLine(string.Format("{0},{1},{2:F4},{3},{4}", startSegment, finishPlace, tmp, initialNumberOfTruck, endNumberOfTruck));
            }

            streamWriter.Close();
        }

        else 
        {
            stopwatch.Start();
            // UnityEngine.Debug.Log("No crash");
        }
    }

    public void GetValue()
    {
        var trucksChildren = GameObject.Find("Trucks").GetComponentsInChildren<Transform>();
        endNumberOfTruck = trucksChildren.Length;
    } 
    
}