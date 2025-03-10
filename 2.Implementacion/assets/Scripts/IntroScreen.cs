using System.Collections;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroScreen : MonoBehaviour
{
    public float controlsDelay=6f; 
    public float objectiveDelay = 3f; 
    public RawImage controls;
    public RawImage objective;
    public TextMeshProUGUI controlsTxt;
    public TextMeshProUGUI objectiveTxt;

    void Start()
    {
        controls.gameObject.SetActive(false);
        controlsTxt.gameObject.SetActive(false);
        objective.gameObject.SetActive(false);
        objectiveTxt.gameObject.SetActive(false);

        StartCoroutine(ShowImages());
    } 

    IEnumerator ShowImages()
    {
        Debug.Log("Esperando controles");
        controls.gameObject.SetActive(true);
        controlsTxt.gameObject.SetActive(true);
        yield return new WaitForSeconds(controlsDelay);

        Debug.Log("esperando objetivo");
        objective.gameObject.SetActive(true);
        objectiveTxt.gameObject.SetActive(true);
        yield return new WaitForSeconds(objectiveDelay);

        Debug.Log("esperando para cambiar de escena");
        yield return new WaitForSeconds(1f);
        LoadGameScene();
    }
    void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene"); // Cambia "GameScene" por el nombre de tu escena principal
    }
}
