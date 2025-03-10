using UnityEngine;

public class MapPointTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<SpotlightManager>().OnPointCollected(gameObject);
        }
    }
}
