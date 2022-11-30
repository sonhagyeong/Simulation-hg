using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class PopVehicle : MonoBehaviour
{
    // Stopwatch stopwatch = new Stopwatch();
    // public void OnCollisionEnter(Collision collision){

    //   if (collision.gameObject.tag == "Target"){
    //     gameObject.SetActive(false);
    //     stopwatch.Stop();
    //     float time = stopwatch.ElapsedMilliseconds ;
    //     UnityEngine.Debug.Log(time*0.001);

    //     FileStream fs = new FileStream("Assets/result.csv", FileMode.Append, FileAccess.Write);
    //     StreamWriter streamWriter = new StreamWriter(fs, System.Text.Encoding.Unicode);

    //     var list = new List<string>();
    //     string name = this.gameObject.ToString();
    //     list.Add((time*0.001).ToString());
        
    //     for (int i = 0; i < list.Count; i++)
    //     {
    //         var tmp = list[i];

    //         //Debug.Log(string.Format("{0},{1}", tmp.nIndex, tmp.sName));
    //         streamWriter.WriteLine(string.Format("{0},{1:F4}", name.Substring(0, name.Length - 24), tmp));
    //     }

    //     streamWriter.Close();
    //   }
    //   else {
    //     streamWriter.Start();
    //   }
    // }
    
}