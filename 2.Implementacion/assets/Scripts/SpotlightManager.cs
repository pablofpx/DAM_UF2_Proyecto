using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpotlightManager : MonoBehaviour
{
    public List<GameObject> spotLights;
    private int collectedPoints = 0;
    private bool gameStarted = false; 

    void Start()
    {
        // se desactivan todos excepto el primero
        for (int i = 1; i < spotLights.Count; i++)
        {
            spotLights[i].SetActive(false);
        }

        spotLights[0].SetActive(true);
    }

    public void OnPointCollected(GameObject collectedPoint)
    {
        collectedPoints++;
        collectedPoint.SetActive(false); // oculta el punto recogido

        if (!gameStarted)
        {
            gameStarted = true; 
        }

        if (collectedPoints >= spotLights.Count)
        {
            WinGame();
        }
        else
        {
            ActivateNextPoint();
        }
    }

    void ActivateNextPoint()
    {
        if (!gameStarted) return; 

        List<GameObject> inactivePoints = spotLights.FindAll(p => !p.activeSelf && p != spotLights[0]);
        if (inactivePoints.Count > 0)
        {
            int randomIndex = Random.Range(0, inactivePoints.Count);
            inactivePoints[randomIndex].SetActive(true);
        }
    }

    void WinGame()
    {
        SceneManager.LoadScene("EndScene");
    }
}
