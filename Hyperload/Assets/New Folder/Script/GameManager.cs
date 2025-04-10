using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    

    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            int randomNumber = Random.Range(-10, 10);
            GameObject playerObj = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomNumber, 0, randomNumber), Quaternion.identity);

        }
    }
}
