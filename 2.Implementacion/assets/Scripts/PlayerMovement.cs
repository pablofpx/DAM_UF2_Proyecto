using System;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Base")]
    public CharacterController controlador;
    public Transform camara; 
    public LayerMask groundMask;
    public Animator anim;
    WallRunning wallRunning;

    [Header("Movement")]
    public float walkingSpeed = 10f;
    public float sprintSpeed = 11.5f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 6f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;

    [Header("Attack")]
    public GameObject attackWeapon;
    public GameObject attackObject;
    public Transform meleeHit;

    public Vector3 velocity;
    public Vector3 velocityGrapple; // tbd
    bool isGrounded;
    bool canDoubleJump;
    bool disableGravity = false;
    public bool freeze;
    public bool activeGrapple;
    bool enableMovementOnNextTouch;
    
    void Start()
    {
        controlador = GetComponent<CharacterController>();
        wallRunning = GetComponent<WallRunning>();
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    void Update()
    {
        // check suelo para resetear la velocidad de caída

        
        // logica para saltar, se calcula si está en el suelo o si está saltando
        
        CheckGround();
        ApplyGravity();
        wallRunning.StateMachine();

        if (!wallRunning.IsWallRunning())
        {
            HandleMovement();
        }

        HandleJump();
        //HandleAttack();
    }

    void FixedUpdate()
    {
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
        freeze = false;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();
            GetComponent<Grappling>().StopGrapple();
        }
    }

    void HandleMovement()
    {
        if (wallRunning.IsWallRunning()) // quizás haya que quitar esto porque bloquea el movimiento
        {
            anim.SetBool("isJumping", false);
            return;
        }

        if (activeGrapple) return; // mientras se ejecuta el grapple no te puedes mover
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 direccion = new Vector3(x, 0, z).normalized;

        if (freeze) // para el grappling hook 
        {
            Debug.Log("freeze");
            return;
        }

        if (direccion.magnitude >= 0.1f)
        {
            // ajustar la dirección del movimiento con base en la orientación de la cámara
            float anguloDestino = Mathf.Atan2(direccion.x, direccion.z) * Mathf.Rad2Deg + camara.eulerAngles.y;
            Quaternion rotacion = Quaternion.Euler(0, anguloDestino, 0);
            Vector3 direccionFinal = rotacion * Vector3.forward;
            
            controlador.Move(direccionFinal * walkingSpeed * Time.deltaTime);
            
            // logica para correr
            if (Input.GetButton("Fire3"))
            {
                controlador.Move(direccionFinal * sprintSpeed * Time.deltaTime);
                anim.SetFloat("Speed", 2f);
            }
            else
            {
                anim.SetFloat("Speed", 1f);
            }
        }
        else
        {
            anim.SetFloat("Speed", 0f); // idle
        }
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            anim.SetTrigger("Jump");
            anim.SetBool("isJumping", true);

            if (wallRunning.IsWallRunning())
            {
                wallRunning.WallJump();
            }
            else if (isGrounded)
            {
                Debug.Log("Salto normal");
                SetVelocityY(Mathf.Sqrt(jumpHeight * -2 * gravity));
                canDoubleJump = true;
            }
            else if (canDoubleJump) // hay un bug en la primera ejecucion del salto no puedes hacer uno doble --- arreglado 
            {
                Debug.Log("Doble salto");
                SetVelocityY(Mathf.Sqrt((jumpHeight + 3f) * -2 * gravity)); // saltar el doble (?)
                canDoubleJump = false;
                anim.SetTrigger("Jump");
            }
        }

        //if (isGrounded)
        //{
            //anim.SetBool("isJumping", false);
        //}
    }

    public void ResetJump()
    {
        canDoubleJump = true; // para que pueda saltar cuando hace walljump
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
            canDoubleJump = true;
            wallRunning.StopWallRun();
            anim.SetBool("isJumping", false);
        }
    }

    void ApplyGravity()
    {
        if(!disableGravity && !activeGrapple)
        {
            if (!isGrounded)
            {
                velocity.y += gravity * 1.5f * Time.deltaTime;
            }
            else if (velocity.y < 0)
            {
                velocity.y = -2f;
            }
        }
        controlador.Move(velocity * Time.deltaTime);
    }

    public void SetGravity(bool state)
    {
        disableGravity = !state;
        if (disableGravity)
        {
            velocity.y = 0;
        }
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        velocity = newVelocity;
    }

    public void SetVelocityY(float newY)
    {
        velocity.y = newY;
    }

    public void SetGrappleVelocity() // igual lo cambio si no me gusta
    {
        enableMovementOnNextTouch = true;
        velocity = velocityGrapple;
    }

    // no se implementa 

    //void HandleAttack()
    //{
        //if (Input.GetButtonDown("Fire1"))
        //{
            //Debug.Log("Ataque!");
            //Attack();
        //}
    //}

    //void Attack()
    //{
        //attackObject = Instantiate(attackWeapon, meleeHit);
        //attackObject.SetActive(true); 
    //}

    // funciones para calcular el grapple
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;
        velocityGrapple = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight); // igual lo cambio
        Invoke(nameof(SetGrappleVelocity), 0.1f);
    }
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity)) * 1.2f;

        return velocityXZ + velocityY;
    }
}