using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainPowerUp : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject rainEffect;

    //offset to instantiate above player
    private Vector3 offset = new Vector3(0, 1f, 0);

    PlayerController pc;

    float timer;
    bool startEffect, destroyObject;

    //array for all the obejcts in the model
    MeshRenderer[] mArray;

    AudioManager am;
    Animator anim;

    private void Start()
    {
        pc = player.GetComponent<PlayerController>();
        am = FindObjectOfType<AudioManager>();
        anim = pc.GetComponentInChildren<Animator>();
    }


    private void Update()
    {
        //make sure swimming is false so player doesnt start swimming when they are being rained on
        if (startEffect)
        {
            //instantiate rain effect above 1m player with player as the parent so they move together
            //also rotated the same direction as the player in y axis so it faces the same way
            Instantiate(rainEffect, player.position + offset, Quaternion.Euler(0f, player.rotation.y,0f), player);
            startEffect = false;
            anim.SetBool("swimming", false);
        }
        if (destroyObject)
        {
            DestroyObject();
            anim.SetBool("swimming", false);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //play rain sound and start effect and destroy methods in update
            startEffect = true;
            destroyObject = true;

            am.StartAudio(AudioManager.Sound.rain, transform.position, .4f);
        }
    }


    void DestroyObject()
    {
        //increment timer with delta time
        timer += Time.deltaTime;

        //after 5 seconds destroy rain effect and set inWater to false
        if (timer >= 5f)
        {
            pc.SetInWater(false);
            destroyObject = false;
        }

        //make all renderers and collider enabled = false so script can still run until the effect has stopped
        gameObject.GetComponent<SphereCollider>().enabled = false;

        //enable = false for all child objects on the model
        mArray = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer m in mArray)
        {
            m.enabled = false;
        }

        //destroy 5.5 seconds after the item has been picked up
        Destroy(gameObject, 5.5f);
    }
}
