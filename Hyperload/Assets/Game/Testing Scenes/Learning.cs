using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Learning : MonoBehaviour
{

    public string Playername = "Majnu";
    public int Playerhealth = 100;
    public bool IsAlive = true;
    public float PlayerSpeed = 1.5f;
    public Vector3 Move = new Vector3(10, 10, 10);
    public GameObject Mahesh;
















    // Start is called before the first frame update
    void Start()
    {

        Mahesh.transform.position = Move;
        
    }




   void Printstats()

    {
        Debug.Log("Player Name :" + Playername);
        Debug.Log("Player Health :" + Playerhealth);
        Debug.Log("Player Alive :" + IsAlive);
        Debug.Log("Player Speed :" + PlayerSpeed);
    }




}
