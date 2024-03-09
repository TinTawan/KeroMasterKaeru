using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    private static Manager instance;

    public static int flies, fliesToWin;
    public static float health;

    public static bool tutorialComplete;

    static MainUI mainUI;



    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        mainUI = FindObjectOfType<MainUI>();

        flies = 0;
        health = 100f;

        fliesToWin = 100;
    }

   

    public static void AddFlies(int inputFlies)
    {
        flies += inputFlies;

        if(tutorialComplete)
        {
            if (flies >= fliesToWin)
            {
                mainUI.CheckGameState(MainUI.GameState.EndGame);
                SceneManager.LoadScene(2);
            }
        }
        
    }

    public static void AddHealth(float inputHealth)
    {
        health += inputHealth;

        if(health < 1)
        {
            FindObjectOfType<AudioManager>().StartAudio(AudioManager.Sound.death, GameObject.FindGameObjectWithTag("Player").transform.position, .1f);
            mainUI.CheckGameState(MainUI.GameState.Dead);
        }

    }

    
}
