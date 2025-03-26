using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject MainCamera;
    public GameObject thirdPersonCameraVC;
    
    public GameObject UiCanvas;
    public MovementStateManager movementStateManager;
    public AimStateManager aimStateManager;
    public ActionStateManager actionStateManager;
    // Start is called before the first frame update
    void Start()
    {
        movementStateManager = GetComponent<MovementStateManager>();
        aimStateManager = GetComponent<AimStateManager>();
        actionStateManager = GetComponent<ActionStateManager>();

        if (photonView.IsMine)
        {
            MainCamera.SetActive(true);
            thirdPersonCameraVC.SetActive(true);
            UiCanvas.SetActive(true);

            movementStateManager.enabled = true;
            aimStateManager.enabled = true;
            actionStateManager.enabled = true;
        }
        else
        {
            MainCamera.SetActive(false);
            thirdPersonCameraVC.SetActive(false);
            UiCanvas.SetActive(false);

            movementStateManager.enabled = false;
            aimStateManager.enabled = false;
            actionStateManager.enabled = false;
        }
    }



}
