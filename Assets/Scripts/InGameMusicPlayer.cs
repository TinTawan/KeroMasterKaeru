using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMusicPlayer : MonoBehaviour
{
    
    // [SerializeField] AudioSource music;
    [SerializeField] AudioSource[] musicTracks;
    [SerializeField] float volume1; //Used to change the volume of the music 
    [SerializeField] float volume2; //Used to change the volume of the music 

    EndTutorial tutorialEnd;

    // Start is called before the first frame update
    void Start()
    {
        tutorialEnd = FindObjectOfType<EndTutorial>();
        musicTracks = GetComponents<AudioSource>();
       
    }

    // Update is called once per frame
    void Update()
    {
        // Changes both of the volumes at the same time
        musicTracks[0].volume = volume1; 
        musicTracks[1].volume = volume2;
        // Checks if te player has finished the tutorial
        if(!Manager.tutorialComplete)
        {
           musicTracks[0].enabled = true; // Enables  the first audio source(The cave music) 
           musicTracks[1].enabled = false; // Then disables the forest audio Source (Since the player is not in forest)
            
        }
        else
        {
            musicTracks[0].enabled = false; // When player is in forest scene is disables the cave music audio Source
            musicTracks[1].enabled = true; // Enables Forest music if tutorial is completed and moved on to the next area
            
        }
    }
}
