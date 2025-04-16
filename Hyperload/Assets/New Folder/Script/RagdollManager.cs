using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RagdollManager : MonoBehaviour
{
    private Rigidbody[] rbs;
    private CharacterController characterController;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs) rb.isKinematic = true;

        characterController = GetComponent<CharacterController>();
        anim = GetComponentInParent<Animator>();
    }

    public void TriggerRagdoll()
    {
        foreach (Rigidbody rb in rbs) rb.isKinematic = false;

        if (characterController != null) characterController.enabled = false; // Disable it here
        
        if (anim != null) anim.enabled = false;
    }
}
