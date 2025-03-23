using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    GameObject playerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            int randomNumber = Random.Range(-10, 10);
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomNumber, 0, randomNumber), Quaternion.identity);
        }
    }
}
