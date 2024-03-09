using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTutorial : MonoBehaviour
{
    [SerializeField] private Transform forestSpawn;
    [SerializeField] private GameObject player;

    CharacterController cc;
    LevelLoader loadLevel;

    bool endTrigger;

    private void Start()
    {
        loadLevel = GameObject.FindObjectOfType<LevelLoader>();

        cc = player.GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (endTrigger)
        {
            //re enable character controller and tell manager the tutorial is over
            EndSequence();

            Manager.tutorialComplete = true;
            cc.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Character Controller must be disabled to teleport
            cc.enabled = false;

            endTrigger = true;

            //reset players health when they enter the forest
            Manager.health = 100;
        }
    }

    void EndSequence()
    {
        //teleport player to forest start
        player.transform.position = forestSpawn.position;

        endTrigger = false;
    }


    public bool GetEndTrigger()
    {
        return endTrigger;
    }
}
