using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;

public class RagdollManager : MonoBehaviourPunCallbacks
{
    private Rigidbody[] rbs;
    private CharacterController characterController;
    private Animator anim;

    private MovementStateManager movementStateManager;
    private AimStateManager aimStateManager;
    private ActionStateManager actionStateManager;
    private WeaponManager weaponManager;
    private GameLogic gameLogic;
    // Start is called before the first frame update
    void Start()
    {
        movementStateManager = GetComponent<MovementStateManager>();
        aimStateManager = GetComponent<AimStateManager>();
        actionStateManager = GetComponent<ActionStateManager>();
        weaponManager = GetComponentInChildren<WeaponManager>();
        gameLogic = GetComponentInChildren<GameLogic>();

        rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs) rb.isKinematic = true;

        characterController = GetComponent<CharacterController>();
        anim = GetComponentInParent<Animator>();
    }

    public void TriggerRagdoll()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RPC_ActivateRagdoll", RpcTarget.All);
        }
    }

    [PunRPC]
    public void RPC_ActivateRagdoll()
    {
        if (movementStateManager) movementStateManager.enabled = false;
        if (aimStateManager) aimStateManager.enabled = false;
        if (actionStateManager) actionStateManager.enabled = false;
        if (weaponManager) weaponManager.enabled = false;
        if (gameLogic) gameLogic.enabled = false;

        foreach (Rigidbody rb in rbs)
            rb.isKinematic = false;

        if (characterController != null)
            characterController.enabled = false;

        if (anim != null)
            anim.enabled = false;
    }
}
