using System;
using System.Collections;
using Unity.Collections;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Base")]
    public Transform orientation;
    private Movement pm;
    private CharacterController controller;

    [Header("Input")]
    private float x;
    private float z;

    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public LayerMask groundMask;
    public float wallRunSpeed = 10.5f;
    public float maxWallRunTime = 2f;
    private float wallRunTimer;
    private Vector3 wallRunVelocity;

    [Header("Detection")]
    public float wallCheckDistance = 0.6f;
    public float minJumpHeight = 1.5f; // probar
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;
    public bool isWallRunning;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pm = GetComponent<Movement>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForWall();
        StateMachine();
    }
    
    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
        Debug.DrawRay(transform.position, orientation.right * wallCheckDistance, wallRight ? Color.green : Color.red);
        Debug.DrawRay(transform.position, -orientation.right * wallCheckDistance, wallLeft ? Color.green : Color.red);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, groundMask);
    }

    public void StateMachine()
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        // logica para correr por la pared, comprueba los raycast, si corres hacia delante y que esté en el aire
        if ((wallLeft || wallRight) && z > 0 && AboveGround())
        {
            if (!isWallRunning)
            {
                StartWallRun();
            }
        }
        else
        {
            if (isWallRunning)
            {
                StopWallRun();
            }
        }

        if (isWallRunning)
        {
            WallRunningMovement();
        }

    }

    void StartWallRun()
    {
        isWallRunning = true;
        wallRunTimer = maxWallRunTime;
        pm.SetGravity(false);
    }

    public void StopWallRun()
    {
        isWallRunning = false;
        pm.SetGravity(true);
    }

    private void WallRunningMovement()
    {
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if(Vector3.Dot(orientation.forward, wallForward) < 0)
        {
            wallForward = -wallForward;
        }

        wallRunVelocity = wallForward * wallRunSpeed;
        controller.Move(wallRunVelocity * Time.deltaTime);
    }

    public void WallJump()
    {
        Debug.Log("isWallRunning"+isWallRunning);
        if (!isWallRunning) return;

        StopWallRun(); 
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        // para impulsarse fuera de la pared
        Vector3 jumpDirection = (wallNormal * 4.5f) + (Vector3.up * 10f); 

        // esto no sirve
        //pm.SetVelocity(new Vector3(jumpDirection.x, 0, jumpDirection.z) * pm.jumpHeight);
        //pm.SetVelocityY(jumpDirection.y + pm.jumpHeight);

        // esto tampoco sirve
        //pm.SetVelocity(jumpDirection * pm.jumpHeight);

        pm.velocity = jumpDirection * pm.jumpHeight;
        
        StartCoroutine(ResetHorizontalVelocity());
        pm.ResetJump();

        Debug.Log("Wall Jump realizado" + jumpDirection);
    }

    public bool IsWallRunning()
    {
        return isWallRunning;
    }
    
    // para resetear la velocidad residual del salto, si no se bugea
    IEnumerator ResetHorizontalVelocity()
    {
        yield return new WaitForSeconds(0.2f);
        pm.SetVelocity(new Vector3(0f,pm.velocity.y, 0f));
    }
}