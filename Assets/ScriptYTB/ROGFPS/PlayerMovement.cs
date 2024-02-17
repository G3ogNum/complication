using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RootMotion.FinalIK;

public class PlayerMovement : MonoBehaviour
{
    [Header("Buff")]
    public float speedBuff = 1f;


    [Header("Movement")] 
    private float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    public float walkSpeed=7f;
    public float sprintSpeed=10f;
    public float crouchSpeed=3.5f;
    public float slideSpeed=30f;
    public float wallRunSpeed=12f;
    public float climbSpeed=6f;

    public float speedIncreaseMutiplier=1.5f;
    public float slopeIncreaseMutiplier=2.5f;

    public float groundDrag=4f;

    public float speedChangeLimit = 4.0f;

    [Header("Jumping")]
    public float jumpForce=12f;
    public float jumpCoolDown=0.25f;
    public float airMultiplier=0.4f;
    bool readyToJump = true;

    [Header("Crouching")]
    public float crouchYScale=0.5f;
    private float startScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode SlideKey = KeyCode.C;

    [Header("Ground Check")] 
    public float playerHeight=2;
    public LayerMask whatIsGrounded;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle=40f;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("References")]
    public Climbing climbingScript;

    //public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;

    public Animator animator;
    public AimIK aimIK;

    private RaycastHit groundHit;

    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air,
        sliding,
        wallrunning,
        climbing,
    }

    public bool sliding;
    public bool crouching;
    public bool wallrunning;
    public bool climbing;
    // Start is called before the first frame update
    void Start()
    {

        
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        climbingScript = GetComponent<Climbing>();
        readyToJump = true;

        startScale = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(speedBuff);
        
        //cast if on ground
        grounded = Physics.SphereCast(transform.position,0.45f, Vector3.down, out groundHit, playerHeight*0.3f,whatIsGrounded);
    //    Debug.Log(grounded);
        MyInput();
        SpeedControl();
        StateHandler();


        //deal with drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
        //Debug.Log(rb.drag);
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }
    
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
       

     /*   aimIK.solver.SetIKPositionWeight(Input.GetKey(KeyCode.W) ? 
            Mathf.LerpUnclamped(aimIK.solver.GetIKPositionWeight(), 0f, Time.deltaTime * 4)
            : Mathf.LerpUnclamped(aimIK.solver.GetIKPositionWeight(), 1f, Time.deltaTime * 4));
*/
        animator.SetBool("Forward", Input.GetKey(KeyCode.W));
        animator.SetBool("Back", Input.GetKey(KeyCode.S));
        animator.SetBool("Left", Input.GetKey(KeyCode.A));
        animator.SetBool("Right", Input.GetKey(KeyCode.D));
        animator.SetBool("Sprint", Input.GetKey(sprintKey));

        animator.SetFloat("Horizontal", horizontalInput);
        animator.SetFloat("Vertical", verticalInput);

        animator.SetBool("Jump", Input.GetKeyDown(jumpKey));
        //when to jump
        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            
            readyToJump = false;
            animator.SetTrigger("Jump");
            Jump();
            Invoke(nameof(ResetJump), jumpCoolDown);
        }

        //start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down*5, ForceMode.Impulse);
        }
        //stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        //Mode Climbing
        if (climbing)
        {
            state = MovementState.climbing;
            desiredMoveSpeed = climbSpeed;
            //Debug.Log("climbSpeed");
        }

        //Mode WallRun
        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallRunSpeed;
            //Debug.Log("wallRunSpeed");
        }
        //Mode Sliding
        if (sliding)
        {
            state = MovementState.sliding;

            //increase speed by every second
            if (OnSlope() && rb.velocity.y < 1.0f)
                desiredMoveSpeed = slideSpeed;
            else
                desiredMoveSpeed = sprintSpeed;

        }
        //Mode Crouching
        else if (grounded && Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
            //Debug.Log("crouchSpeed");
        }

        //Mode Sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
            //Debug.Log("sprintSpeed");
        }
        //Mode walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
            //Debug.Log("walkSpeed");
        }

        //Mode Air
        else
        {
            state = MovementState.air;
            //Debug.Log("airSpeed");
        }

        //check if desiredMoveSpeed has changed drastically haidai 
        // 4 called speed change limit
        if(Mathf.Abs(desiredMoveSpeed-lastDesiredMoveSpeed)> speedChangeLimit && moveSpeed!=0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }
        lastDesiredMoveSpeed = desiredMoveSpeed;
        //Debug.Log(moveSpeed);
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        //smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while(time<difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMutiplier * slopeIncreaseMutiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMutiplier;
            yield return null;
        }
        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        //TODO:only W is down can sprint

        if (climbingScript.exitingWall) return;

        //calculator movement direction
        //moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection = new Vector3(horizontalInput, 0, verticalInput) ;
        moveDirection = transform.TransformDirection(moveDirection);
        //on slope
        if (OnSlope()&&!exitingSlope)
        {
            rb.AddForce(speedBuff * GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y!=0)
            {
                
                // rb.AddForce(GetVerticalToSlope() * 800f, ForceMode.Force);
               // Debug.Log(GetVerticalToSlope());
                 rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        

        //on ground
        if (grounded)
            rb.AddForce(speedBuff * moveDirection.normalized * moveSpeed * 20f, ForceMode.Force);

        //in air
        else if(!grounded)
            rb.AddForce(speedBuff * moveDirection.normalized * moveSpeed * 10f*airMultiplier, ForceMode.Force);

        //turn gravity of while on slope
        if(!wallrunning)
            rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        //limiting speed on slope
        if (OnSlope()&&!exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        //limiting speed on ground or in air
        else
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            //�����ƶ��ٶȣ������Ҫ�Ļ���
            if (flatVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }
    }
    

    private void Jump()
    {
        exitingSlope = true;
      //  Debug.Log("jump");
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(speedBuff * transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position,Vector3.down,out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
          //  Debug.Log(angle);
            return angle < maxSlopeAngle && angle != 0;
            
        }
        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    private Vector3 GetVerticalToSlope()
    {
        Vector3 SlopeV = new Vector3(-1, -1, 0);
        return Vector3.ProjectOnPlane(SlopeV, slopeHit.normal).normalized;
    }
}
