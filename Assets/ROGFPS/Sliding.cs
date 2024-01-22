using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    //public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Sliding")]
    public float maxSlideTime=0.75f;
    public float slideForce=200f;
    private float slideTimer;

    public float slideYScale=0.5f;
    private float startYScale;


    [Header("Input")]
    public KeyCode SlideKey=KeyCode.C;
    private float horizontalInput;
    private float verticalInput;

   

    private void Start()
    {
        playerObj = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        
        startYScale = playerObj.localScale.y;

    }
    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(SlideKey) && rb.velocity.magnitude>=pm.sprintSpeed-pm.speedChangeLimit)
            StartSlide();
        if (Input.GetKeyUp(SlideKey) && pm.sliding)
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        if (pm.sliding)
            SlidingMovement();
    }

    private void StartSlide()
    {
        pm.sliding = true;
        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(pm.speedBuff*Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }
    private void SlidingMovement()
    {
        //Vector3 inputDirection = orientation.forward * verticalInput+orientation.right*horizontalInput;
        Vector3 inputDirection = transform.forward * verticalInput + transform.right * horizontalInput;


        if (!pm.grounded)
        {
            rb.AddForce(pm.speedBuff * inputDirection.normalized * slideForce, ForceMode.Force);
        }
        //sliding normal
        else if (!pm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(pm.speedBuff * inputDirection.normalized * slideForce, ForceMode.Force);
            slideTimer -= Time.deltaTime;
           // Debug.Log(rb.velocity.y);
        }

        //sliding on Slope
        else
        {
            rb.AddForce(pm.speedBuff * pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);

        }


        if (slideTimer <= 0)
            StopSlide();
    }
    private void StopSlide()
    {
        pm.sliding = false;

        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }
}
