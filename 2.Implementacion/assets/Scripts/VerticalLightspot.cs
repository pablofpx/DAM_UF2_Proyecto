using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class VerticalLightspot : MonoBehaviour
{

    public float lineHeight = 25f; 
    public Color lineColor = Color.yellow; 

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2; 
        lineRenderer.startWidth = 1.3f;
        lineRenderer.endWidth = 1.3f;

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

        Vector3 startPoint = transform.position - Vector3.up * 1f; // Punto en el objeto
        Vector3 endPoint = startPoint + Vector3.up * lineHeight; // Punto más arriba

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }
}