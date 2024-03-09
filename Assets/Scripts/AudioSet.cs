using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSet : MonoBehaviour
{
    AudioSource source;

    public AudioClip clip;
    public float vol = 1f;

    //set values of source and play
    public void PlaySound()
    {
        source = GetComponent<AudioSource>();
        source.clip = clip;
        source.volume = vol;
        source.Play();
    }


    //destroy sound object when finished playing
    private void Update()
    {
        if (!source.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}

