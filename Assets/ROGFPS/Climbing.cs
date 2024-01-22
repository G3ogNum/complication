using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("References")]

   // public Transform orientation;

    private Rigidbody rb;
    private PlayerMovement pm;
    public LayerMask whatIsWall;
    

    [Header("Climbing")]
    public float climbSpeed=10f;
    public float maxClimbTime=0.75f;
    private float climbTimer;

    private bool climbing;

    [Header("ClimbJumping")]
    public float climbJumpUpForce=14f;
    public float climbJumpBackForce=12f;

    public KeyCode jumpKey = KeyCode.Space;
    public int ClimbJumps=1;
    private int climbJumpsLeft;

    [Header("Detection")]
    public float detectionLength=0.7f;
    public float sphereCastRadius=0.25f;
    public float maxWallLookAngle=30f;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    private Transform lastWall;
    private Vector3 lastWallNormal;
    public float minWallNormalAngleChange=5f;

    [Header("Exiting")]
    public bool exitingWall;
    public float exitWallTime=0.2f;
    private float exitWallTimer;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        WallCheck();
        StateMachine();

        if(climbing && !exitingWall) ClimbingMovement();
    }

    private void StateMachine()
    {
        // State 1 - Climbing
        if (wallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle && !exitingWall)
        {
            if (!climbing && climbTimer > 0)
                StartClimbing();
            //Timer
            if (climbTimer > 0) 
                climbTimer -= Time.deltaTime;
            if (climbTimer <= 0)
            {
                StopClimbing();
               // rb.AddForce(Vector3.up*3, ForceMode.Force);
            }
        }

        //State 2 - Exiting
        else if (exitingWall)
        {
            if (climbing) StopClimbing();

            if (exitWallTimer > 0) exitWallTimer -= Time.deltaTime;
            if (exitWallTimer <= 0) exitingWall = false;
        }

        //State 3 - None
        else
        {
            if (climbing)
                StopClimbing();
        }

        if (climbing&&wallFront && Input.GetKeyDown(jumpKey) && climbJumpsLeft > 0) ClimpJump();
        //Debug.Log(wallFront);
    }

    private void WallCheck()
    {
        /* wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall);
         wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);*/

        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, transform.forward, out frontWallHit, detectionLength, whatIsWall);
        wallLookAngle = Vector3.Angle(transform.forward, -frontWallHit.normal);

        bool newWall = frontWallHit.transform != lastWall || Mathf.Abs(Vector3.Angle(lastWallNormal,frontWallHit.normal))>minWallNormalAngleChange ;

        if ((wallFront&&newWall)||pm.grounded)
        {
            climbTimer = maxClimbTime;
            climbJumpsLeft = ClimbJumps;
        }
    }

    private void StartClimbing()
    {
        climbing = true;
        pm.climbing = true;

        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;

        // (can but don`t have to) camera fov change
    }

    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z)*pm.speedBuff;

        //sound effect
    }

    private void StopClimbing()
    {
        climbing = false;
        pm.climbing = false;
        //rb.AddForce(Vector3.up*2, ForceMode.Impulse);
        //particle effect
    }

    private void ClimpJump()
    {
        //SphereCast
        exitingWall = true;
        exitWallTimer = exitWallTime;
        Vector3 forceToApply = transform.up * climbJumpUpForce + frontWallHit.normal * climbJumpBackForce;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        climbJumpsLeft--;
      //  Debug.Log("ClimpJump");
    }
}
