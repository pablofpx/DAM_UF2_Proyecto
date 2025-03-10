using System.Collections;
using UnityEngine;

public class AttackBehaviour : MonoBehaviour
{
    private float duration = 0.2f; // duration of attack window 
    private Collider attackCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackCollider = GetComponent<Collider>();
        StartCoroutine(Attack());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Attack()
    {
        attackCollider.enabled = true;
        yield return new WaitForSeconds(duration);
        attackCollider.enabled = false;
        Destroy(gameObject, duration);
    }

    void OnTriggerEnter(Collider other)
    {
        // la logica se implementa despues
    }

}
