using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelLoader : MonoBehaviour
{
    public GameObject loadingScreen; 
    public Slider slider;
    public TextMeshProUGUI loadingText;
    public void LoadLevel(int sceneIndex)
    {
        // asynchronous loading needs to be in a coroutine
        StartCoroutine(LoadAsynchronously(sceneIndex));
       
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        // Takes in the scene index and loads that scene then shows the loading scree
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);

        // Updates the Bar UI until the loading completes
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // Changes the 0.9 to 1
            
            slider.value = progress;
            loadingText.text = progress * 100f + "%";
            yield return null;
        }
    }
}
