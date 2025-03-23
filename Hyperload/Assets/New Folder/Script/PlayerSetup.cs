using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject MainCamera;
    public GameObject thirdPersonCamera;
    public GameObject AimCamera;
    public GameObject UiCanvas;
    public PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponentInChildren<PlayerController>();

        if(photonView.IsMine)
        {
            MainCamera.SetActive(true);
            thirdPersonCamera.SetActive(true);
            AimCamera.SetActive(true);
            UiCanvas.SetActive(true);
            playerController.enabled = true;
        }
        else
        {
            MainCamera.SetActive(false);
            thirdPersonCamera.SetActive(false);
            AimCamera.SetActive(false);
            UiCanvas.SetActive(false);
            playerController.enabled = false;
        }
    }

    
}
