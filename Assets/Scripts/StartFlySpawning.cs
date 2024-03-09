using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFlySpawning : MonoBehaviour
{
    [SerializeField] private GameObject caveSpawner, forestSpawner;

    private void Update()
    {
        if(Manager.flies >= 10)
        {
            //dont spawn flies in the cave once the player has caught enough to leave the cave
            caveSpawner.SetActive(false);
        }

        //set correct spawners active depending on if the tutorial has finsihed
        if (Manager.tutorialComplete)
        {
            forestSpawner.SetActive(true);
        }
        else
        {
            forestSpawner.SetActive(false);
            caveSpawner.SetActive(true);
        }
    }
}
