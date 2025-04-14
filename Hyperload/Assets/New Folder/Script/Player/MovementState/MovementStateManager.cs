using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    public float currentMoveSpeed;
    public float walkSpeed = 3, walkBackSpeed = 2;
    public float runSpeed = 7, runBackSpeed = 5;
    public float crouchSpeed = 2, crouchBackSpeed = 1;
    public float airspeed = 1.5f;

    [HideInInspector] public Vector3 dir;
    [HideInInspector] public float hzInput,vrInput;
    CharacterController characterController;


    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask groundMask;
    Vector3 spherePos;

    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpforce = 10;
    [SerializeField] float slamDownForce = -30f;
    [HideInInspector] public bool jumped;
    private float originalJumpForce;
    private bool jumpForceTemporarilyDisabled = false;
    Vector3 velocity;

    public MovementBaseState previousState;
    public MovementBaseState currentState;

    public IdleState Idle = new IdleState();
    public WalkState Walk = new WalkState();
    public CrouchState Crouch = new CrouchState();
    public RunState Run = new RunState();
    public JumpState Jump = new JumpState();


    [HideInInspector] public Animator anim;

    [HideInInspector] public bool isLaunching = false; // Add this

    
    // Start is called before the first frame update
    void Start()
    {
        originalJumpForce = jumpforce;
        anim = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        SwiitchState(Idle);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        GetDirectionAndMove();
        Gravity();
        Falling();

        anim.SetFloat("hrInput", hzInput);
        anim.SetFloat("vrInput", vrInput);

        currentState.UpdateState(this);

        if (IsGrounded())
        {
            isLaunching = false;
        }

        if (!IsGrounded() && Input.GetKeyDown(KeyCode.G))
        {
            SlamDown();
        }
    }

    public void SwiitchState(MovementBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    void GetDirectionAndMove()
    {
        hzInput = Input.GetAxisRaw("Horizontal");
        vrInput = Input.GetAxisRaw("Vertical");
        Vector3 airDir = Vector3.zero;
        if (!IsGrounded()) airDir = transform.forward * vrInput + transform.right * hzInput;
        else dir = transform.forward * vrInput + transform.right * hzInput;



        dir.Normalize();

        characterController.Move((dir * currentMoveSpeed + airDir.normalized * airspeed) * Time.deltaTime);

    }

    public bool IsGrounded()
    {
        spherePos = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
        if(Physics.CheckSphere(spherePos, characterController.radius - 0.05f, groundMask)) return true;
        return false;
    }

    void Gravity()
    {
        if (!IsGrounded()) velocity.y += gravity * Time.deltaTime;
        else if (velocity.y < 0) velocity.y = -2;

        characterController.Move(velocity * Time.deltaTime);
    }

    void Falling() => anim.SetBool("Falling", !IsGrounded());

    public void JumpForce()
    {
        //Debug.Log("JumpForce called. Current force: " + jumpforce);
        velocity.y += jumpforce;
    }

    public void Jumped() => jumped = true; 

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spherePos, characterController.radius - 0.05f);
    }

    public void Launch(Vector3 force)
    {
        velocity.y = 0; // Reset current Y velocity
        velocity += force;
        isLaunching = true;
    }
    public void TemporarilyDisableJumpForce(float duration)
    {
        if (jumpForceTemporarilyDisabled) return;

        jumpForceTemporarilyDisabled = true;
        jumpforce = 0;
        StartCoroutine(RestoreJumpForceAfterDelay(duration));
    }

    private IEnumerator RestoreJumpForceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        jumpforce = originalJumpForce;
        jumpForceTemporarilyDisabled = false;
    }
    public void ResetVerticalVelocity()
    {
        velocity.y = 0;
    }

    public void SlamDown()
    {
        Debug.Log("Slammed down!");
        velocity.y = slamDownForce;
    }

}
