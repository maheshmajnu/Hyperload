using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PlayerSetup : MonoBehaviourPunCallbacks
{
    
    
    public GameObject UiCanvas;
    public MovementStateManager movementStateManager;
    public AimStateManager aimStateManager;
    public ActionStateManager actionStateManager;
    public WeaponManager weaponManager;
    // Start is called before the first frame update
    void Start()
    {
        movementStateManager = GetComponent<MovementStateManager>();
        aimStateManager = GetComponent<AimStateManager>();
        actionStateManager = GetComponent<ActionStateManager>();
        weaponManager = GetComponentInChildren<WeaponManager>();


        if (photonView.IsMine)
        {
            
            UiCanvas.SetActive(true);

            movementStateManager.enabled = true;
            aimStateManager.enabled = true;
            actionStateManager.enabled = true;
            weaponManager.enabled = true;
        }
        else
        {
            
            UiCanvas.SetActive(false);

            movementStateManager.enabled = false;
            aimStateManager.enabled = false;
            actionStateManager.enabled = false;
            weaponManager .enabled = false;
        }
    }

    


}
