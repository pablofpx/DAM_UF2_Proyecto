using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grappling : MonoBehaviour
{
    [Header("Base")]
    private Movement pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappeable;
    public LineRenderer lr;

    [Header("Grapple")]
    public float maxGrappleDistance;
    public float grappleDelay;
    public float overshootYAxis;
    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pm = GetComponent<Movement>();        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(grappleKey)) StartGrapple();

        if (grapplingCdTimer > 0) // para resetear el cd
        {
            grapplingCdTimer -= Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
        }
    }

    // logica del gancho
    void StartGrapple()
    {
        if (grapplingCdTimer > 0 ) return;

        grappling = true;

        pm.freeze = true;
        
        // si el raycast da con la superficie grapleable, se ejecuta el grapple
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappeable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelay);
        }
        else
        {
            // si no da con una superficie, simplemente termina el grapple
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelay);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    void ExecuteGrapple()
    {
        pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0 ) highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);
        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        pm.freeze = false;
        pm.activeGrapple = false;
        grappling = false;
        grapplingCdTimer = grapplingCd; // se le aplica el time de cooldown para qeu no lo spamee
        lr.enabled = false;
        StartCoroutine(ResetHorizontalVelocity());
    }
    IEnumerator ResetHorizontalVelocity()
    {
        yield return new WaitForSeconds(0.2f);
        pm.SetVelocity(new Vector3(0f,pm.velocity.y, 0f));
    }
}