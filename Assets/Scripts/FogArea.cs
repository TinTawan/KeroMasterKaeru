using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FogArea : MonoBehaviour
{
    [Header("Fog Sections")]
    [SerializeField] private GameObject[] fogAreas;
    [SerializeField] private int[] amountToUnlock;

    [Header("Canvas Elements")]
    [SerializeField] private Slider[] areaSlider;
    [SerializeField] private TextMeshProUGUI[] areaFliesText; 
    [SerializeField] private TextMeshProUGUI[] areaUnlockText;

    bool playSound;
    bool oneTimePlay = true;
    bool twoTimePlay = true;

    private void Update()
    {
        Area(0);
        Area(1);

        if(playSound)
        {
            AudioManager am = FindObjectOfType<AudioManager>();
            am.StartAudio(AudioManager.Sound.flyRequirement, transform.position, .7f);
            playSound = false;
        }
    }

    void Area(int index)
    {
        GameObject fog = fogAreas[index];
        int unlockInt = amountToUnlock[index];

        areaSlider[index].maxValue = unlockInt;
        areaSlider[index].value = Manager.flies;
        areaFliesText[index].text = unlockInt.ToString() + " Flies";

        if (Manager.flies >= unlockInt)
        {
            fog.SetActive(false);

            areaFliesText[index].enabled = false;
            areaUnlockText[index].enabled = false;


            // Has to be hard coded 
            // Check how many files player and checks if the sound has been played once 
            if(Manager.flies == 20 && oneTimePlay)
            {
                playSound = true;   // Plays sound
                oneTimePlay = false; // Reset
            } 
            if (Manager.flies == 30 && twoTimePlay)
            {
                playSound = true;
                twoTimePlay = false;
            }
        
            
            
        }


    }

}
