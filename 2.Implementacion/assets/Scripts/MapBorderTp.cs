using UnityEngine;

public class MapBorderTp : MonoBehaviour
{
    public Transform spawnPoint;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Wall"))
        {
            transform.position = spawnPoint.position;
        }

        // mensaje por pantalla?
    }
}
