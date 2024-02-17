using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
public class testAnimationController : MonoBehaviour
{
    public Animator animator;

    public AimIK aimIK;

    [Header("Settings")]
    public Vector2 clampInDegrees = new Vector2(360, 180);
    
    [Space]
    private Vector2 sensitivity = new Vector2(2, 2);
    [Space]
    public Vector2 smoothing = new Vector2(3, 3);

    [Header("First Person")]
    public GameObject characterBody;
    public GameObject CameraHolder;
    public GameObject TPHolder;

    private Vector2 targetDirection;
    private Vector2 targetCharacterDirection;

    private Vector2 _mouseAbsolute;
    private Vector2 _smoothMouse;

    private Vector2 mouseDelta;

    [HideInInspector]
    public bool scoped;

    public LayerMask aimColliderLayerMask = new LayerMask();

    public Transform debugTransform;


    float duration = 1f;
    float num = 0f;
    float timer = 0f;

    void Start()
    {
        

        // Set target direction to the camera's initial orientation.
        targetDirection = transform.localRotation.eulerAngles;

        // Set target direction for the character body to its inital state.
        if (characterBody)
            targetCharacterDirection = characterBody.transform.localRotation.eulerAngles;

        

    }

    // Update is called once per frame
    void Update()
    {
       

        animator.SetFloat("Horizontal",Input.GetAxis("Horizontal"));
        animator.SetFloat("Vertical", Input.GetAxis("Vertical"));

        #region Keyboard

        animator.SetBool("Sprint", Input.GetKey(KeyCode.LeftShift));
        if (Input.GetKey(KeyCode.Space))
        {
            animator.SetTrigger("Jump");
        }
        else
        {
            animator.ResetTrigger("Jump");
        }

        if (Input.GetKey(KeyCode.W))
        {

            //num = Mathf.LerpUnclamped(aimIK.solver.GetIKPositionWeight(), 0f,  Time.deltaTime*4 );

            aimIK.solver.SetIKPositionWeight(Mathf.LerpUnclamped(aimIK.solver.GetIKPositionWeight(), 0f, Time.deltaTime * 4));
            animator.SetBool("Forward", true);
            //characterBody.transform.position += Vector3.forward*Time.deltaTime*5;
        }
        else
        {
            
            //num = Mathf.LerpUnclamped(num, 1f, Time.deltaTime*4 );
            aimIK.solver.SetIKPositionWeight(Mathf.LerpUnclamped(aimIK.solver.GetIKPositionWeight(), 1f, Time.deltaTime * 4));
            animator.SetBool("Forward", false);
        }

        if (Input.GetKey(KeyCode.A))
        {
            animator.SetBool("Left", true);
        }
        else
        {
            animator.SetBool("Left", false);
        }

        if (Input.GetKey(KeyCode.S))
        {
            animator.SetBool("Back", true);
        }
        else
        {
            animator.SetBool("Back", false);
        }

        if (Input.GetKey(KeyCode.D))
        {
            animator.SetBool("Right", true);
        }
        else
        {
            animator.SetBool("Right", false);
        }

        if (Input.GetKey(KeyCode.F))
        {
            animator.SetBool("TurnLeft", true);
        }
        else
        {
            animator.SetBool("TurnLeft", false);
        }

        if (Input.GetKey(KeyCode.G))
        {
            animator.SetBool("TurnRight", true);
        }
        else
        {
            animator.SetBool("TurnRight", false);
        }
        #endregion

        #region turning
        // Allow the script to clamp based on a desired target value.
        var targetOrientation = Quaternion.Euler(targetDirection);
        var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

        // Get raw mouse input for a cleaner reading on more sensitive mice.
        mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        // Scale input against the sensitivity setting and multiply that against the smoothing value.
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

        // Interpolate mouse movement over time to apply smoothing delta.
        _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
        _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

        // Find the absolute mouse movement value from point zero.
        _mouseAbsolute += _smoothMouse;

        // Clamp and apply the local x value first, so as not to be affected by world transforms.
        if (clampInDegrees.x < 360)
            _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

        // Then clamp and apply the global y value.
        if (clampInDegrees.y < 360)
            _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        //CameraHolder.transform.localRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;

        //TPHolder.transform.localRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetCharacterOrientation * Vector3.right) * targetCharacterOrientation;
        // If there's a character body that acts as a parent to the camera
        if (characterBody)
        {
            //characterBody.GetComponent<PhotonView>().RPC("SetTPAngle", RpcTarget.AllBuffered, TPHolder.transform.localRotation);

            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, Vector3.up);
            //characterBody.transform.localRotation = yRotation * targetCharacterOrientation;

            //rotate of Lumine use this pls
            //characterBody.transform.localRotation = Quaternion.Lerp(characterBody.transform.localRotation, yRotation * targetCharacterOrientation, Time.deltaTime * 20f);

            animator.SetBool("TurnLeft", mouseDelta.x < 0);
            animator.SetBool("TurnRight", mouseDelta.x > 0);
        }
        else
        {
            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
            transform.localRotation *= yRotation;
        }
        #endregion


        #region aimming
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if(Physics.Raycast(ray,out RaycastHit raycastHit,999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
        }
        #endregion
    }
}
