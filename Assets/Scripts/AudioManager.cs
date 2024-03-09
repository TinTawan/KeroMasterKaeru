using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum Sound
    {
        walk, jump, ribbit, catchFly, grapple, splash, death, rain, charge, tongue, miss, collect, flyRequirement, flycage

    }

    public GameObject audioObject;
    public AudioClip[] walkSound;
    public AudioClip[] jumpSound;
    public AudioClip[] ribbitSound;
    public AudioClip[] catchFlySound;
    public AudioClip[] grappleSound;
    public AudioClip[] splashSound;
    public AudioClip[] deathSound;
    public AudioClip[] rainSound;
    public AudioClip[] chargeSound;
    public AudioClip[] tongueSound;
    public AudioClip[] missSound;
    public AudioClip[] collectItemSound;
    public AudioClip[] flyRequirementSound;
    public AudioClip[] flyCageSound;
    



    public void StartAudio(Sound soundType, Vector3 pos, float vol)
    {
        GameObject newSound = Instantiate(audioObject, pos, Quaternion.identity);
        AudioSet aSet = newSound.GetComponent<AudioSet>();

        switch (soundType)
        {
            case (Sound.walk):
                aSet.clip = walkSound[Random.Range(0, walkSound.Length)];
                break;
            case (Sound.jump):
                aSet.clip = jumpSound[Random.Range(0, jumpSound.Length)];
                break;
            case (Sound.ribbit):
                aSet.clip = ribbitSound[Random.Range(0, ribbitSound.Length)];
                break;
            case (Sound.catchFly):
                aSet.clip = catchFlySound[Random.Range(0, catchFlySound.Length)];
                break;
            case (Sound.grapple):
                aSet.clip = grappleSound[Random.Range(0, grappleSound.Length)];
                break;
            case (Sound.splash):
                aSet.clip = splashSound[Random.Range(0, splashSound.Length)];
                break;
            case (Sound.death):
                aSet.clip = deathSound[Random.Range(0, deathSound.Length)];
                break;
            case (Sound.rain):
                aSet.clip = rainSound[Random.Range(0, rainSound.Length)];
                break;
            case (Sound.charge):
                aSet.clip = chargeSound[Random.Range(0, chargeSound.Length)];
                break;
            case (Sound.tongue):
                aSet.clip = tongueSound[Random.Range(0, tongueSound.Length)];
                break;
            case (Sound.miss):
                aSet.clip = missSound[Random.Range(0, missSound.Length)];
                break;
            case (Sound.collect):
                aSet.clip = collectItemSound[Random.Range(0, collectItemSound.Length)];
                break;
            case (Sound.flyRequirement):
                aSet.clip = flyRequirementSound[Random.Range(0, flyRequirementSound.Length)];
                break;
            case (Sound.flycage):
                aSet.clip = flyCageSound[Random.Range(0, flyCageSound.Length)];
                break;

        }


        aSet.vol = vol;
        aSet.PlaySound();
    }

}
