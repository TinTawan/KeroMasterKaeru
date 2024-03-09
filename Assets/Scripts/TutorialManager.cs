using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{

    public GameObject[] popUps;
    private int popUpIndex;
    [SerializeField] PlayerController player;
    [SerializeField] Grappling gr;

    
    // Start is called before the first frame update
    void Start()
    {
        //Checks if the player is in the tutorial cave
        
        if(SceneManager.GetActiveScene().name != "Scene1")
        {
            Destroy(gameObject);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        bool water = player.ReturnInWater();
        float charge = player.ReturnCharge();
        float maxCharge = player.ReturnMaxCharge();
        bool grappleHit = gr.GetHitGrappleObject();
        bool thirdPers = player.ReturnThridPers();
        KeyCode switchPers = player.ReturnSwitchPers();
        bool flyCaught = gr.GetFlyCaught();

        // Checks if player is out of the tutorial area
        // Also if player uses cheats and doesnt finish tutorial it removes the pop ups after
        if(Manager.tutorialComplete)
        {
            popUpIndex = 7;
        }

        // Goes through all the popups in array and enables them 
        for (int i = 0; i < popUps.Length; i++)
        {
            if (i == popUpIndex)
            {
                popUps[popUpIndex].SetActive(true);
                
            }
            else
            {
                popUps[i].SetActive(false);
            }
        }

        // These if statements checks if the player has done what the tutorial says then goes to the next
        if(popUpIndex == 0)
        {
            if(Input.GetButtonDown("Vertical") || Input.GetButtonDown("Horizontal"))
            {
                popUpIndex++;
            }
            
        }
        else if (popUpIndex == 1)
        {
            if (Input.GetButtonDown("Jump"))
            {
                popUpIndex++;
            }
        }
        else if (popUpIndex == 2)
        {
            if (water)
            {
                popUpIndex++;
            }
        }
        else if (popUpIndex == 3)
        {
            
            if (Input.GetButtonDown("Jump") && charge >= maxCharge)
            {
                popUpIndex++;
            }
                
            
        }
        else if (popUpIndex == 4)
        {
            if (grappleHit)
            {
                popUpIndex++;
            }
        }
        else if (popUpIndex == 5)
        {
            if (flyCaught)
            {
                popUpIndex++;
            }
        }
        else if (popUpIndex == 6)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                popUpIndex++;
            }
        }



    }
}
