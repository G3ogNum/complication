using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce=200f;
    public float wallJumpForwardForce=0f;
    public float wallJumpUpForce=7f;
    public float wallJumpSideForce=25f;

    public float wallClimbSpeed=1f;
    public float maxWallRunTime=1.5f;
    private float wallRunTimer;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;
    private bool upwardsRunning;
    private bool downwardsRunning;
    public KeyCode JumpKey = KeyCode.Space;
    public KeyCode UpwardsRunKey = KeyCode.LeftShift;
    public KeyCode DownwardsRunKey = KeyCode.LeftControl;


    [Header("Detection")]
    public float wallcheckDistance=0.75f;
    public float minJumpHeight=1f;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exiting")]
    private bool exitingWall;
    public float exitWallTime=0.4f;
    private float exitWallTimer;

    [Header("Gravity")]
    public bool useGravity;
    public float gravityCounterForce=0f;

    [Header("References")]
   // public Transform orientation;
    public MouseLook cam;
    private PlayerMovement pm;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }
    private void FixedUpdate()
    {
        if (pm.wallrunning)
            WallRunningMovement();
    }

    private void CheckForWall()
    {
        /*  wallRight = Physics.Raycast(transform.position, orientation.right,out rightWallHit, wallcheckDistance, whatIsWall);
          wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallcheckDistance, whatIsWall);
  */
        wallRight = Physics.Raycast(transform.position, transform.right, out rightWallHit, wallcheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -transform.right, out leftWallHit, wallcheckDistance, whatIsWall);

    }
    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        //Getting Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(UpwardsRunKey);
        downwardsRunning = Input.GetKey(DownwardsRunKey);


        //State 1 - WallRunning
        if (!pm.climbing&&(wallLeft || wallRight) && verticalInput > 0 && AboveGround()&&!exitingWall)
        {
            if (!pm.wallrunning)
                StartWallRun();

            //wallrun timer
            if (wallRunTimer > 0)
                wallRunTimer -= Time.deltaTime;
            if (wallRunTimer <= 0)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }

            //wall jump
            if (Input.GetKeyDown(JumpKey)) 
                WallJump();
        }

        //State 2 - Exiting
        else if (exitingWall)
        {
            if (pm.wallrunning)
                StopWallRun();
            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;
            if (exitWallTimer <= 0)
                exitingWall = false;
        }

        //State 3 - None
        else
        {
            if (pm.wallrunning)
            {
                StopWallRun();
            }
        }
        
    }

    private void StartWallRun()
    {
        pm.wallrunning = true;

        wallRunTimer = maxWallRunTime;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        //apply camera effects
        cam.DoFov(70f);
        if (wallLeft) cam.DoTile(-5f);
        if (wallRight) cam.DoTile(5f);
    }

    private void WallRunningMovement()
    {
        rb.useGravity = useGravity;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        /*if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;*/
        if ((transform.forward - wallForward).magnitude > (transform.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        //forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        // upwards/downwards force
        if (upwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        if (downwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, -wallcheckDistance, rb.velocity.z);


        //push player to the wall 
        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
            rb.AddForce(-wallNormal * 100, ForceMode.Force);


        //weaken gravity
        if (useGravity)
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
    }

    private void StopWallRun()
    {
        pm.wallrunning = false;

        //reset camera effect
        cam.DoFov(60f);
        cam.DoTile(0);
    }

    private void WallJump()
    {
        // enter exiting wall state
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        /*if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;*/
        if ((transform.forward - wallForward).magnitude > (transform.forward - -wallForward).magnitude)
            wallForward = -wallForward;
        Vector3 forceToApply = wallForward* wallJumpForwardForce + transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        // reset y velocity add force
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
        Debug.Log("WallJump");
    }
}
