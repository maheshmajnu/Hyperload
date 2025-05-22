using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class AimStateManager : MonoBehaviourPunCallbacks
{
    public AimBaseState currentState;
    public HipFireState Hip = new HipFireState();
    public AimState Aim = new AimState();

    [SerializeField] float mouseScence = 1;
    public bool invertY = false;
    [SerializeField] Transform camFollowPos;
    float xAxis, yAxis;

    [HideInInspector] public Animator anim;
    [HideInInspector] public CinemachineVirtualCamera vCam;
    public float aimFov = 40;
    [HideInInspector] public float hipFov;
    [HideInInspector] public float currentFov;
    public float fovSmoothSpeed = 10;

    public Transform aimPos;
    [HideInInspector] public Vector3 actualAimPosition;
    [SerializeField] float aimSmoothSpeed = 20f;
    [SerializeField] LayerMask aimMask;

    float xFollowPos;
    float yFollowPos, ogYpos;
    [SerializeField] float crouchCamHeight = 0.6f;
    [SerializeField] float shoulderSwapSpeed = 10;
    MovementStateManager moving;

    private void Start()
    {
        moving = GetComponent<MovementStateManager>();
        xFollowPos = camFollowPos.localPosition.x;
        ogYpos = camFollowPos.localPosition.y;
        yFollowPos = ogYpos;

        vCam = GetComponentInChildren<CinemachineVirtualCamera>();
        hipFov = vCam.m_Lens.FieldOfView;

        anim = GetComponent<Animator>();
        SwitchState(Hip);
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        xAxis += Input.GetAxisRaw("Mouse X") * mouseScence;
        float mouseY = Input.GetAxisRaw("Mouse Y");
        yAxis += (invertY ? -mouseY : mouseY) * mouseScence;

        yAxis = Mathf.Clamp(yAxis, -80, 80);

        vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, currentFov, fovSmoothSpeed * Time.deltaTime);

        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimMask))
        {
            aimPos.position = Vector3.Lerp(aimPos.position, hit.point, aimSmoothSpeed * Time.deltaTime);
            actualAimPosition = hit.point;

            // Send the aim position to others
            photonView.RPC("RPC_UpdateAimPos", RpcTarget.Others, actualAimPosition);
        }

        MoveCamera();
        currentState.UpdateState(this);
    }

    private void LateUpdate()
    {
        if (!photonView.IsMine) return;

        camFollowPos.localEulerAngles = new Vector3(yAxis, camFollowPos.localEulerAngles.y, camFollowPos.localEulerAngles.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, xAxis, transform.eulerAngles.z);
    }

    public void SwitchState(AimBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    void MoveCamera()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt)) xFollowPos = -xFollowPos;
        yFollowPos = moving.currentState == moving.Crouch ? crouchCamHeight : ogYpos;

        Vector3 newFollowPos = new Vector3(xFollowPos, yFollowPos, camFollowPos.localPosition.z);
        camFollowPos.localPosition = Vector3.Lerp(camFollowPos.localPosition, newFollowPos, shoulderSwapSpeed * Time.deltaTime);
    }

    [PunRPC]
    public void RPC_UpdateAimPos(Vector3 networkAim)
    {
        if (aimPos != null)
        {
            aimPos.position = networkAim;
        }
        else
        {
            Debug.LogWarning("AimPos not assigned in RPC_UpdateAimPos");
        }
    }
}
