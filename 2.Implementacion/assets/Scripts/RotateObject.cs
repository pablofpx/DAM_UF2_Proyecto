using UnityEngine;

public class RotateObject : MonoBehaviour
{
    
    private Vector3 rotationSpeed = new Vector3(0, 0, 100);

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);        
    }
}
