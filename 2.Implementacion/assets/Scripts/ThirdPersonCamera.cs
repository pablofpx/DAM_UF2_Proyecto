using UnityEngine;

public class ThirdPerson : MonoBehaviour
{
    public Transform objetivo;
    public float sensibilidad = 2.0f;
    public float distancia = 7.0f;
    public float altura = 1f;
    
    private float rotacionX = 0;
    private float rotacionY = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        rotacionX += Input.GetAxis("Mouse X") * sensibilidad;
        rotacionY -= Input.GetAxis("Mouse Y") * sensibilidad;

        // bloquear cámara 
        rotacionY = Mathf.Clamp(rotacionY, -30, 60);

        Vector3 direccion = new Vector3(0, 0, -distancia);
        Quaternion rotacion = Quaternion.Euler(rotacionY, rotacionX, 0);
        
        transform.position = objetivo.position + rotacion * direccion + Vector3.up * altura;
        transform.LookAt(objetivo.position + Vector3.up * altura);
        
        // Rotar el personaje junto con la cámara
        objetivo.rotation = Quaternion.Euler(0, rotacionX, 0);
    }
}
