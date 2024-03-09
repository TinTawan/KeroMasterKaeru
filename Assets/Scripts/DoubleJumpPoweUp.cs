using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpPoweUp : MonoBehaviour
{
    [SerializeField] private Transform player;
    
    PlayerController pc;

    bool destroyItem;
    float timer;

    MeshRenderer[] mArray;
    AudioManager am;


    // Start is called before the first frame update
    void Start()
    {
        pc = player.GetComponent<PlayerController>();
        am = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // This occurs when the player has collected the item
        if (destroyItem)
        {
            DestroyItem();
        }
    }

    // checks if the player has collected the item
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && pc != null)
        {
            am.StartAudio(AudioManager.Sound.collect, transform.position, .08f);
            destroyItem = true;
        }
    }

    void DestroyItem()
    {
        

        // Disables the collider and messh rendereer so that the effect stays
        gameObject.GetComponent<SphereCollider>().enabled = false;
        mArray = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer m in mArray)
        {
            m.enabled = false;
        }

        // Then destroys the object later
        Destroy(gameObject, 5f);
    }
}
