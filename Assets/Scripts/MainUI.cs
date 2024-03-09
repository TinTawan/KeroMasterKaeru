using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainUI : MonoBehaviour
{
    private Animator anim;
    private PlayerController pc;
    private Grappling grappleScript;
    AudioManager am;
    

    [SerializeField] LevelLoader loadLevel;

    public enum GameState { Menu, Paused, Playing, Dead, EndGame }
    public GameState currentState;

    //UI panels
    [SerializeField] private GameObject mainMenuUI, gameUI, pausedUI, deadUI, endGameUI, mainMenuTitleScreen, mainMenuCredits;

    //Game playing objects
    [SerializeField] private TextMeshProUGUI fliesNum, healthNum;
    [SerializeField] private Slider healthBar, grappleCooldownBar, chargedJumpBar, flyBar;
    [SerializeField] private Image AimImage;



    private void Start()
    {
        //aim cursor doesnt appear on the menu or end scene
        am = FindObjectOfType<AudioManager>();
        AimImage.enabled = false;
        

        if(currentState == GameState.Playing)
        {
            //these components are only needed during the game play
            pc = GameObject.Find("Player").GetComponent<PlayerController>();
            grappleScript = GameObject.Find("Player").GetComponent<Grappling>();
            anim = GameObject.Find("Player").GetComponentInChildren<Animator>();
        }
    }

    private void Awake()
    {
        //set game state depending on scene name when scene wakes
        if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            CheckGameState(GameState.Menu);
        }
        else if(SceneManager.GetActiveScene().name == "Scene1")
        {
            CheckGameState(GameState.Playing);
            
        }
        else if (SceneManager.GetActiveScene().name == "EndGame")
        {
            CheckGameState(GameState.EndGame);
        }
    }

    private void Update()
    {
        //check inputs and update game play UI when in playing state
        if (currentState == GameState.Playing && pc != null)
        {
            PauseInputs();

            if (!pc.thirdCamActive)
            {
                AimImage.enabled = true;
                
            }
            else
            {
                AimImage.enabled = false;

            }

            DisplayFlies();
            DisplayHealth();
            DisplayCharge();
            StartCoroutine(DisplayGrappleCountDown());
        }
        
    }

    //switch statement to run methods depending on which state the game is in
    public void CheckGameState(GameState newGameState)
    {
        currentState = newGameState;
        switch (currentState)
        {
            case GameState.Menu:
                MenuState();
                break;
            case GameState.Playing:
                GameActiveState();
                break;
            case GameState.Paused:
                PausedState();
                break;
            case GameState.Dead:
                DeadState();
                break;
            case GameState.EndGame:
                EndGameState();
                break;
        }
    }

    //set correct UI elements active
    void MenuState()
    {
        Cursor.visible = true;

        mainMenuUI.SetActive(true);
        gameUI.SetActive(false);
        pausedUI.SetActive(false);
        deadUI.SetActive(false);
        endGameUI.SetActive(false);

        mainMenuTitleScreen.SetActive(true);
        mainMenuCredits.SetActive(false);
    }
    void GameActiveState()
    {
        //time scale normal and hide cursor in play mode
        Time.timeScale = 1f;
        Cursor.visible = false;

        gameUI.SetActive(true);
        pausedUI.SetActive(false);
        mainMenuUI.SetActive(false);
        deadUI.SetActive(false);
        endGameUI.SetActive(false);

    }
    void PausedState()
    {
        //time scale stopped and show cursor to press buttons in pause
        Time.timeScale = 0f;
        Cursor.visible = true;

        pausedUI.SetActive(true);
        gameUI.SetActive(false);
        mainMenuUI.SetActive(false);
        deadUI.SetActive(false);
        endGameUI.SetActive(false);
    }
    void DeadState()
    {
        //play death sound and allow cursor to press buttons on UI
        am.StartAudio(AudioManager.Sound.death, pc.transform.position, 0.5f);

        Time.timeScale = 0f;
        Cursor.visible = true;

        deadUI.SetActive(true);
        pausedUI.SetActive(false);
        gameUI.SetActive(false);
        mainMenuUI.SetActive(false);
        endGameUI.SetActive(false);
    }
    void EndGameState()
    {
        //time stopped in end game, show cursor and dont lock it so player can press buttons
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        endGameUI.SetActive(true);
        mainMenuUI.SetActive(false);
        deadUI.SetActive(false);
        pausedUI.SetActive(false);
        gameUI.SetActive(false);

        
    }

    //check for escape key press
    void PauseInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(currentState == GameState.Playing)
            {
                CheckGameState(GameState.Paused);
            }
            else if(currentState == GameState.Paused)
            {
                CheckGameState(GameState.Playing);
            }
        }
    }

    //show the flies on the UI bar
    //max values set depending on which area the players in
    void DisplayFlies()
    {
        if(!Manager.tutorialComplete)
        {
            flyBar.maxValue = 10;
        }
        else
        {
            flyBar.maxValue = Manager.fliesToWin;
        }
        
        fliesNum.text = Manager.flies.ToString();
        flyBar.value = Manager.flies;
    }


    //change health bar and number to show on UI
    void DisplayHealth()
    {
        int healhInt = (int)Manager.health;
        healthNum.text = healhInt.ToString();

        healthBar.value = Manager.health;
    }

    //display jump charge level on UI bar
    void DisplayCharge()
    {
        chargedJumpBar.value = pc.chargePower; 
        chargedJumpBar.maxValue = pc.MaxchargePower;
    }

    //display tongue cooldown on UI
    IEnumerator DisplayGrappleCountDown()
    {
        grappleCooldownBar.value = grappleScript.GetGrappleCountDownTimer();

        if (grappleScript.GetHitGrappleObject())
        {
            yield return new WaitForSeconds(1f);
            grappleScript.SetHitGrappleObject(false);
        }
    }


    //Set game state for UI buttons
    public void StartGame()
    {
        //incase state changes when player in water turn them false
        anim.SetBool("swimming", false);
        pc.inWater = false;
        if (!Manager.tutorialComplete)
        {
            //if player loses in cave, re load the scene
            loadLevel.LoadLevel(1);
        }
        else //checkpoint system allows fog areas to stay unlocked if player dies after unlocking them
        {
            //teleport player back to start of forest
            //Character Controller must be disabled to teleport
            pc.gameObject.GetComponent<CharacterController>().enabled = false;

            Transform forestSpawn = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
            pc.transform.position = forestSpawn.position;

            //reset health and flies
            Manager.health = 100;
            Manager.flies = 0;
        }

        pc.gameObject.GetComponent<CharacterController>().enabled = true;


        CheckGameState(GameState.Playing);
    }
    public void ResumeGame()
    {
        //set state to playing
        CheckGameState(GameState.Playing);
    }
    public void GoToMainMenu()
    {
        //load menu and change to menu state
        loadLevel.LoadLevel(0);
        CheckGameState(GameState.Menu);
    }
   
    public void QuitGame()
    {
        Application.Quit();
    }

    //hide and show correct ui screens for credits and back to menu
    public void CreditsButton()
    {
        mainMenuCredits.SetActive(true);
        mainMenuTitleScreen.SetActive(false);
    }
    public void BackButton()
    {
        mainMenuCredits.SetActive(false);
        mainMenuTitleScreen.SetActive(true);
    }


}
